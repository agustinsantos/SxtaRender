// if defined, prefecth threads work only on tasks for future frames
// otherwise they can also execute tasks for the current frame, in parallel
// with main thread
#define STRICT_PREFETCH

// if defined, use _ftime to get current time on Win32
// otherwise use GetSystemTime + SystemTimeToFileTime on Win32
// in any case, on Linux, clock_gettime is used
#define USE_FTIME

// if defined, use busy waiting to get the desired framerate frameRate
// (more precise than using Sleep and pthread_cond_timedwait)
//#define BUSY_WAITING

using log4net;
using SD.Tools.Algorithmia.PriorityQueues;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Sxta.Render.Scenegraph
{
    /// <summary>
    /// A Scheduler that can use multiple threads. This scheduler can work with one
    /// or more threads, and it can try to follow a fixed framerate (i.e. a number
    /// of calls to #run per second). If a prefetch rate is specified, the main
    /// thread executes at least the specified number of prefetching tasks par frame
    /// (when such tasks are available), after all the tasks for the current frame
    /// have been executed. Hence if a prefetch rate is specified, or if a fixed frame
    /// rate is specified, this scheduler supports prefetching of tasks of any kind.
    /// Otherwise, if several threads are used, prefetching of cpu tasks is supported,
    /// but not prefetching of gpu tasks.
    /// </summary>
    public class MultithreadScheduler : Scheduler,  ISwappable<MultithreadScheduler>
    {

        /*
         * Creates a new multithread scheduler.
         *
         * @param prefetchRate the minimum number of prefetch task to execute at
         *      each frame, after all the tasks for the current frame have been
         *      executed (and if prefetching tasks are available). The prefetching
         *      of gpu tasks is only possible if this rate is not 0.
         * @param prefetchQueue the maximum number of prefetching tasks that can be
         *      queued for execution. If a prefetch rate or a fixed frame rate is
         *      specified, this valueC must not be 0. When the queue is full, new
         *      prefetch tasks are simply discarded and #schedule returns false.
         *      This maximum queue size prevents the number of prefetching tasks to
         *      grow unbounded, if new prefetching tasks are generated at a greater
         *      rate than the rate at which they are executed.
         * @param frameRate a fixed framerate that this scheduler should try to
         *      follow, or 0 to not fix any framerate.
         * @param nThreads the number of threads to use in addition to the main
         *      thread of the application. Hence 0 means that only one thread will
         *      be used, the main application thread.
         */
        public MultithreadScheduler(int prefetchRate = 0, int prefetchQueue = 0, float frameRate = 0.0f, int nThreads = 0) :
            base("MultithreadScheduler")
        {
            init(prefetchRate, prefetchQueue, frameRate, nThreads);
        }

        /*
         * Deletes this scheduler.
         */
        ~MultithreadScheduler()
        {
            // we first set the #stop flag to true and signals execution threads to wake
            // them up if they were waiting for tasks to execute; they will then
            // eventually terminate
            lock (cpuTasksCond)
            {
                stop = true;
                Monitor.PulseAll(cpuTasksCond); //pthread_cond_broadcast( cpuTasksCond);
            }
            // we then wait until all threads terminate, and we delete them
            for (int i = 0; i < threads.Count; ++i)
            {
                threads[i].Join();//pthread_join(*((pthread_t*)threads[i]), null);
                //delete (pthread_t*) threads[i];
            }
            // we can then delete the mutex and the conditions
            //TODO pthread_mutex_destroy( mutex);
            //delete   mutex;
            //TODO pthread_cond_destroy( cpuTasksCond);
            //delete   cpuTasksCond;
            //TODO pthread_cond_destroy( allTasksCond);
            //delete   allTasksCond;
            threads.Clear();
            if (bufferedFrames > 0)
            {
                clearBufferedFrames();
            }
            if (statisticsFile != null)
            {
                statisticsFile.Close();
            }

        }

        /*
         * Returns true if the prefetch rate or the fixed frame rate is not null,
         * or if there are several threads and gpuTasks is false. In addition,
         * if there is only one thread for cpu tasks, and for gpu tasks in all
         * cases, the curent number of prefetching tasks waiting for execution must
         * be std.less than the prefetch queue size, otherwise this method returns false.
         */
        public override bool supportsPrefetch(bool gpuTasks)
        {
            if (prefetchRate > 0 || framePeriod > 0.0f || (threads.Count > 0 && !gpuTasks))
            {
                if (gpuTasks || threads.Count == 0)
                {
                    return prefetchQueue.Count < prefetchQueueSize;
                }
                return true;
            }
            return false;
        }

        public override void schedule(Task task)
        {
            ISet<Task> initialized = new HashSet<Task>();
            task.init(initialized);
            lock (mutex)
            {
                bool noCpuTasks = readyCpuTasks.Count == 0;
                ISet<Task> addedTasks = new HashSet<Task>();
                addFlattenedTask(task, addedTasks);
                lock (allTasksCond)
                {
                    Monitor.PulseAll(allTasksCond); //pthread_cond_broadcast( allTasksCond);
                }
                if (noCpuTasks && readyCpuTasks.Count != 0)
                {
                    // if there was no ready CPU tasks before this method was called,
                    // and there are now some ready CPU tasks, signals this to the execution
                    // threads that may be waiting for tasks to execute.
                    lock (cpuTasksCond)
                    {
                        Monitor.PulseAll(cpuTasksCond); //pthread_cond_broadcast( cpuTasksCond);
                    }
                }
                Debug.Assert(allReadyTasks.Count > 0);
            }
        }

        public override void reschedule(Task task, Task.reason r, uint deadline)
        {
            lock (mutex)
            {
                task.setIsDone(false, 0, r);
                if (r == Task.reason.DATA_NEEDED)
                {
                    ISet<Task> visited = new HashSet<Task>();
                    setDeadline(task, deadline, visited);
                }
            }
        }

        public override void run(Task task)
        {
            Sxta.Core.Timer timer = new Sxta.Core.Timer();
            timer.start();
            this.schedule(task);
            double schedule = timer.end();

            string msg = "START tasks: " + immediateTasks.Count + " immediate, " + allReadyTasks.Count + " ready, " + readyCpuTasks.Count + " ready cpu; " +
            dependencies.Count + " + " + inverseDependencies.Count + " dependencies";
            log.Debug(msg);
            //foreach (var it in immediateTasks)
            //    log.Debug("Task to execute: " + it);

            int run = 0; // number of executed tasks
            int prefetched = 0; // number of prefetching tasks executed
            int contextSwitches = 0; // number of context switches performed
            Task previousGpuTask = null; // last GPU task executed

            double deadline = 0.0; // deadline for the end of this method
            DateTime deadlinespec = DateTime.MinValue; // same, in timespec format

            if (monitoredTasks.Count > 0)
            {
                frameStatistics.Clear();
                for (int i = 0; i < monitoredTasks.Count; ++i)
                {
                    frameStatistics.Add(monitoredTasks[i], Tuple.Create<int, float>(0, 0.0f));
                }
                if (bufferedStatistics == null)
                {
                    bufferedStatistics = new float[(2 * monitoredTasks.Count + 2) * 1000];
                }
            }

            if (framePeriod > 0.0)
            {
                // if we have a fixed framerate, we compute the deadline for the end
                // of this method; this is the time at the end of the last call to this
                // method, plus the delay for one frame (minus a small margin)
                deadline = lastFrame + framePeriod - 1000.0;
                // subtrating the current time gives the time interval for this methd
                double delay = deadline - timer.start();
                // we use it to compute the deadline in timespec format
                getAbsoluteTime(out deadlinespec);
                deadlinespec.AddTicks((long)(delay * 1000));
                if (deadlinespec.Ticks > 1000000000)
                {
                    deadlinespec.AddSeconds(deadlinespec.Ticks / 1000000000);
                    //TODO deadlinespec.tv_nsec = deadlinespec.Ticks % 1000000000;
                }
            }


            // we loop to execute all required tasks
            while (true)
            {
                // first step: find or wait for a task ready to be executed
                Task t = null;
                lock (mutex)
                {
                    if (immediateTasks.Count == 0 && framePeriod > 0.0)
                    {
                        // if the tasks for the current frame are completed, and if we have
                        // a fixed framerate, we can use the time until the deadline to
                        // execute some tasks for next few frames
#if BUSY_WAITING
            while (allReadyTasks.empty() && timer.start() < deadline) {
                // so we wait for a ready CPU or GPU task,
                // and stop when the deadline is passed
                pthread_mutex_unlock(  mutex);
                double timeout = min(deadline, timer.start() + 500.0);
                while (timer.start() < timeout) {
                }
                pthread_mutex_lock(  mutex);
            }
#else
                        while (allReadyTasks.Count == 0 && timer.start() < deadline)
                        {
                            // so we wait for a ready CPU or GPU task,
                            // and stop when the deadline is passed
                            //pthread_cond_timedwait(  allTasksCond,   mutex, &deadlinespec);
                            lock (mutex)
                            {
                                Monitor.Wait(allTasksCond, deadlinespec.Millisecond);
                            }
                        }
#endif
                    }
                    else
                    {
                        // here either some tasks for the current frame are not completed
                        // or they are all completed but we do not have a fixed framerate
                        while (immediateTasks.Count != 0 && (allReadyTasks.Count == 0 || allReadyTasks.First().Key.first > 0))
                        {
                            // while some tasks for the current frame remain to be executed,
                            // and while the set of tasks ready to be executed is empty or
                            // contains only tasks for the next frames (deadline > 0), wait
                            //pthread_cond_wait(  allTasksCond,   mutex);
                            lock (allTasksCond)
                            {
                                Monitor.Wait(allTasksCond);
                            }
                        }
                    }
                    // if the deadline is passed or if all the tasks for the current frame
                    // are completed, there may not be any task ready to be executed
                    if (allReadyTasks.Count != 0)
                    {

                        // but if there is at least one we pick one, if possible with the
                        // same execution context as the last executed GPU task
                        t = getTask(allReadyTasks, previousGpuTask == null ? null : previousGpuTask.getContext());
                        if (t.getDeadline() != 0)
                        {
                            // if this task is for the next frames, then all tasks for the
                            // current frame should now be completed (tasks are sorted in
                            // such a way that tasks for the current frame are executed first)
                            Debug.Assert(immediateTasks.Count == 0);
                            // if we do not have executed the required minimum number of
                            // prefetching tasks per frame, we execute this available
                            // prefetching task
                            if (prefetched >= prefetchRate)
                            {
                                // if we do not have a fixed framerate, or if the time remaining
                                // until the deadline is less than the expected duration for this
                                // task, we should stop here; setting t to null will force this
                                if (framePeriod == 0.0 || timer.start() + t.getExpectedDuration() > deadline)
                                {
                                    t = null;
                                }
                            }
                        }

                        if (t != null)
                        {
                            // if we finally have a task to execute, we remove it from the
                            // sets that may contain it (but we do not update the
                            // dependencies yet, this will be done after the task execution
                            // in #taskDone
                            immediateTasks.Remove(t);
                            removeTask(allReadyTasks, t);
                            removeTask(readyCpuTasks, t);
                        }
                    }
                    // we can now release the mutex since we will not read or modify the
                    // shared data structures until #taskDone is called; also the selected
                    // task t cannot be seleted by another thread, since it has been removed
                    // from the task sets.
                }
                //pthread_mutex_unlock(mutex);

                if (t == null)
                {
                    // stops the infinite execution loop
                    break;
                }

                bool changes = false;


                if (!t.isDone())
                {
                    log.Debug((t.getDeadline() > 0 ? "PREFETCH " : "RUN ") + t.GetType());
                    if (t.isGpuTask())
                    {
                        // if t is a GPU task, sets the execution context ...
                        if (previousGpuTask == null)
                        {
                            // ... if no GPU has been executed yet
                            t.begin();
                        }
                        else if (previousGpuTask.getContext() != t.getContext())
                        {
                            // ... or if it is not the same as the one of the last GPU task
                            ++contextSwitches;
                            previousGpuTask.end();
                            t.begin();
                        }
                        // t now becomes the last executed GPU task
                        previousGpuTask = t;
                    }

                    if (t.getCompletionDate() >= t.getPredecessorsCompletionDate())
                    {
                        // t is up to date, it is not necessary to run it
                    }
                    else if (framePeriod > 0.0 || monitoredTasks.Count > 0)
                    {
                        // if we have a fixed framerate we measure the execution time
                        // of each task in order to get statistics about tasks, used to
                        // get estimated durations for future tasks
                        timer.start();
                        changes = t.run();
                        double duration = timer.end();
                        t.setActualDuration((float)duration);
                        if (monitoredTasks.Count > 0)
                        {
                            Tuple<int, float> nt;
                            if (frameStatistics.TryGetValue(t.GetType().FullName, out nt))
                            {
                                nt = new Tuple<int, float>(nt.Item1 + 1, nt.Item2 + (float)duration);
                                frameStatistics[t.GetType().FullName] = nt;
                            }
                        }
                    }
                    else
                    {
                        // otherwise we execute tasks without computing statistics
                        changes = t.run();
                    }

                    ++run;
                    if (t.getDeadline() > 0)
                    {
                        ++prefetched;
                    }
                }
                // this updates the task dependencies, and signals other threads when
                // new tasks become ready to be executed
                taskDone(t, changes);
            }

            if (previousGpuTask != null)
            {
                // restores the context after the last executed GPU task
                previousGpuTask.end();
                previousGpuTask = null;
            }

            log.Debug("END " + run + " run tasks " + contextSwitches + " context switches");

            if (framePeriod > 0.0)
            {
                Task.logStatistics();
            }

            if (framePeriod > 0.0)
            {
#if BUSY_WAITING
        while (timer.start() < deadline) {
        }
#else
                double t = timer.start();
                if (t < deadline)
                {
                    // if we have a fixed framerate and if we still have some time before
                    // the deadline, we should wait until this deadline is passed, otherwise
                    // the framerate would increase
                    int milliseconds = (int)System.Math.Floor((deadline - t) / 1e3);
                    Thread.Sleep(milliseconds);
                }
#endif
            }

            if (monitoredTasks.Count > 0)
            {
                double total = timer.start() - lastFrame;
                if (bufferedFrames == 1000)
                {
                    clearBufferedFrames();
                }
                int s = bufferedFrames * (2 * monitoredTasks.Count + 2);
                bufferedStatistics[s++] = (float)(schedule);
                bufferedStatistics[s++] = (float)(total);
                for (int i = 0; i < monitoredTasks.Count; ++i)
                {
                    Tuple<int, float> nt = frameStatistics[monitoredTasks[i]];
                    bufferedStatistics[s++] = (float)nt.Item1;
                    bufferedStatistics[s++] = nt.Item2;
                }
                bufferedFrames += 1;
            }

            // measures the current time at the end of this method, to compute a
            // deadline for the next call to this method
            lastFrame = timer.start();
        }


        /*
         * Adds the given task type to the tasks whose execution times must be monitored (debug).
         */
        public void monitorTask(string taskType)
        {
            monitoredTasks.Add(taskType);
        }


        /*
         * Initializes this scheduler.
         *
         * See #MultithreadScheduler.
         */
        protected void init(int prefetchRate, int prefetchQueue, float frameRate, int nThreads)
        {
            mutex = new Object();
            allTasksCond = new Object();
            cpuTasksCond = new Object();
            /*
                  pthread_mutexattr_t attrs;
                        pthread_mutexattr_init(&attrs);
                        pthread_mutexattr_settype(&attrs, PTHREAD_MUTEX_RECURSIVE);
                        pthread_mutex_init( mutex, &attrs);
                        pthread_mutexattr_destroy(&attrs);
                        pthread_cond_init( allTasksCond, null);
                        pthread_cond_init( cpuTasksCond, null);
             */
            this.prefetchRate = prefetchRate;
            this.prefetchQueueSize = prefetchQueue;
            framePeriod = frameRate == 0.0f ? 0.0f : 1e6f / frameRate;
            if (prefetchRate > 0 || frameRate > 0.0f)
            {
                Debug.Assert(prefetchQueueSize > 0);
            }
            lastFrame = 0;
            time = 2;
            stop = false;
            for (int i = 0; i < nThreads; ++i)
            {
                Thread thread = new Thread(new ThreadStart(schedulerThread));
                threads.Add(thread);
                thread.Start();
            }
            bufferedStatistics = null;
            bufferedFrames = 0;
            statisticsFile = null;
        }


        /*
         * A key to store tasks in a map. This key is made of the task deadline and
         * of its execution context.
         */
        public struct taskKey : IComparer<taskKey>, IEquatable<taskKey>
        {
            public uint first;
            public object context;

            /*
             * A sort operator for task keys. Task keys are sorted based on their
             * deadlines and if their deadlines are equal, based on their execution
             * context. This ensures that tasks whose result is needed first are
             * executed first, while providing a grouping of tasks that share the same
             * the same execution context (which can save context switches).
             */
            //struct taskKeySort : public std.less<taskKey>
            //{
            //    bool operator()(const taskKey &x, const taskKey &y) const;
            //};

            public int Compare(taskKey x, taskKey y)
            {
                uint xDeadline = x.first;
                uint yDeadline = y.first;
                if (xDeadline == yDeadline)
                {
                    object xCtxt = x.context;
                    object yCtxt = y.context;
                    return 0; // return xCtxt < yCtxt;
                }
                else
                {
                    return (int)xDeadline - (int)yDeadline;
                }

            }

            public bool Equals(taskKey other)
            {
                uint xDeadline = this.first;
                uint yDeadline = other.first;
                if (xDeadline == yDeadline)
                {
                    object xCtxt = this.context;
                    object yCtxt = other.context;
                    return true; // return xCtxt < yCtxt;
                }
                else
                {
                    return false;
                }
            }
        }

        /*
         * A sort operator for tasks. This operator is based on the expected
         * duration of tasks, so that shorter tasks are executed first.
         */
        //struct taskSort : public std.less< Task >
        //{
        //    bool operator()(const Task x, const Task y) const;
        //};

        /*
         * A sorted task set, where tasks are sorted based on their deadline,
         * execution context and expected duration.
         */
        //typedef IDictionary<taskKey, std.set<Task, taskSort>, taskKeySort> SortedTaskSet;

        /*
         * A mutex used to ensure consistent access to the data structures of this
         * scheduler from the various execution threads.
         */
        private object mutex;

        /*
         * A condition to signal to execution threads that new tasks are ready to be
         * executed.
         */
        private object allTasksCond;

        /*
         * A condition to signal to execution threads that new CPU tasks are ready
         * to be executed.
         */
        private object cpuTasksCond;

        /*
         * The threads used to execute tasks, in addition to the main thread.
         */
        private List<Thread> threads = new List<Thread>();

        /*
         * Target frame duration in micro seconds, or 0 if no fixed framerate.
         */
        private float framePeriod;

        /*
         * Minimum number of prefetching tasks to execute per frame (if prefetching
         * tasks are ready to be executed). Additionnal tasks can be executed if
         * there is a fixed framerate and if enough time remains until the deadline
         * for the current frame.
         */
        private int prefetchRate;

        /*
         * Maximum number of prefetching tasks that can be waiting for execution.
         * When the queue size is reached, new prefetching tasks are discarded.
         */
        private int prefetchQueueSize;

        /*
         * Time at the end of the last call to #run.
         */
        private double lastFrame;

        /*
         * Logical time used for task completion dates. This logical time is a
         * counter incremented by one after each task execution.
         */
        private uint time;

        /*
         * True if this scheduler must be stopped. This is used in the destructor
         * to stop the execution threads in order to delete them properly.
         */
        private bool stop;

        /*
         * The primitive tasks that must be executed at the current frame.
         */
        private ISet<Task> immediateTasks = new HashSet<Task>();

        /*
         * The primitive CPU or GPU tasks that are ready to be executed. A task is
         * ready to be executed when all its predecessor tasks are completed.
         */
        private IDictionary<taskKey, SortedSet<Task>> allReadyTasks = new SortedDictionary<taskKey, SortedSet<Task>>(taskKeyComparer);

        /*
         * The primitive CPU tasks that are ready to be executed. A task is ready to
         * be executed when all its predecessor tasks are completed.
         */
        private IDictionary<taskKey, SortedSet<Task>> readyCpuTasks = new SortedDictionary<taskKey, SortedSet<Task>>(taskKeyComparer);

        /*
         * The predecessors of the tasks that remain to be executed.
         */
        private IDictionary<Task, ISet<Task>> dependencies = new Dictionary<Task, ISet<Task>>();

        /*
         * The successors of the tasks that remain to be executed.
         */
        private IDictionary<Task, ISet<Task>> inverseDependencies = new Dictionary<Task, ISet<Task>>();

        /*
         * The prefetching tasks that remain to be executed.
         */
        private ISet<Task> prefetchQueue = new HashSet<Task>();

        /*
         * The task classes whose execution time must be monitored (debug).
         */
        private IList<string> monitoredTasks = new List<string>();

        /*
         * The statistics per frame for monitored task classes. For each frame and
         * for each task type, this map gives the number of tasks of this type that
         * have been executed, and the total execution time.
         */
        private IDictionary<string, Tuple<int, float>> frameStatistics = new Dictionary<string, Tuple<int, float>>();

        /*
         * Buffered array of frame statistics, to avoid writing them to disk at
         * each frame.
         */
        private float[] bufferedStatistics;

        /*
         * Number of frame statistics buffered in #bufferedStatistics.
         */
        private int bufferedFrames;

        /*
         * File to store task execution time statistics per frame for monitored tasks.
         */
        private StreamWriter statisticsFile;

        /*
         * Adds all the primitive tasks of the given task to the set of tasks to be
         * executed. This method calls itself recursively on any TaskGraph, in order
         * to find all the primitive tasks, whatever their level of nesting inside
         * task graphs. It also creates all the needed dependencies with
         * #addFlattenedDependency.
         *
         * @param t the task whose primitive sub tasks must be added.
         * @param[in,out] addedTasks the already added tasks. This method adds the
         *      tasks it adds to this set.
         */
        private void addFlattenedTask(Task t, ISet<Task> addedTasks)
        {
            // NOTE: the mutex should be locked before calling this method!
            if (addedTasks.Contains(t))
            {
                return;
            }
            addedTasks.Add(t);
            if (t.isDone())
            {
                return;
            }
            TaskGraph tg = t as TaskGraph;
            if (tg == null)
            {
                if (t.getDeadline() == 0)
                {
                    immediateTasks.Add(t);
                }
                else
                {
                    prefetchQueue.Add(t);
                }
                insertTask(allReadyTasks, t);
#if STRICT_PREFETCH
                if (!t.isGpuTask() && t.getDeadline() > 0)
                {
#else
                if (!t.isGpuTask())
                {
#endif
                    insertTask(readyCpuTasks, t);
                }
            }
            else
            {
                tg.flattenedFirstTasks.Clear();
                tg.flattenedLastTasks.Clear();
                IEnumerator<Task> i = tg.getAllTasks().GetEnumerator();
                while (i.MoveNext())
                {
                    addFlattenedTask(i.Current, addedTasks);
                }
                i = tg.getFirstTasks().GetEnumerator();
                while (i.MoveNext())
                {
                    Task u = i.Current;
                    TaskGraph ug = u as TaskGraph;
                    if (ug == null)
                    {
                        tg.flattenedFirstTasks.Add(u);
                    }
                    else
                    {
                        foreach (Task task in ug.flattenedFirstTasks)
                            tg.flattenedFirstTasks.Add(task);
                    }
                }
                i = tg.getLastTasks().GetEnumerator();
                while (i.MoveNext())
                {
                    Task u = i.Current;
                    TaskGraph ug = u as TaskGraph;
                    if (ug == null)
                    {
                        tg.flattenedLastTasks.Add(u);
                    }
                    else
                    {
                        foreach (Task task in ug.flattenedLastTasks)
                            tg.flattenedLastTasks.Add(task);
                    }
                }

                i = tg.getAllTasks().GetEnumerator();
                while (i.MoveNext())
                {
                    Task dst = i.Current;
                    if (dst.isDone())
                    {
                        continue;
                    }
                    IEnumerator<Task> j = tg.getInverseDependencies(dst);
                    while (j.MoveNext())
                    {
                        Task src = j.Current;
                        addFlattenedDependency(src, dst);
                    }
                }
            }
        }

        /*
         * Adds all the primitive dependencies between the primitive first tasks of
         * src and the primitive last tasks of dst.
         *
         * @param src a task that must be executed after dst.
         * @param dst a task that must be execute before src.
         */
        private void addFlattenedDependency(Task src, Task dst)
        {
            // NOTE: the mutex should be locked before calling this method!
            TaskGraph srcTg = src as TaskGraph;
            if (srcTg != null)
            {
                foreach (Task srcT in srcTg.flattenedFirstTasks)
                {
                    addFlattenedDependency(srcT, dst);
                }
            }
            else
            {
                TaskGraph dstTg = dst as TaskGraph;
                if (dstTg != null)
                {
                    foreach (Task dstT in dstTg.flattenedLastTasks)
                    {
                        addFlattenedDependency(src, dstT);
                    }
                }
                else
                {
                    ISet<Task> visited = new HashSet<Task>();
                    removeTask(allReadyTasks, src);
                    removeTask(readyCpuTasks, src);
                    if (dependencies.ContainsKey(src))
                        dependencies[src].Add(dst);
                    else
                    {
                        HashSet<Task> destSet = new HashSet<Task>();
                        destSet.Add(dst);
                        dependencies.Add(src, destSet);
                    }
                    if (inverseDependencies.ContainsKey(dst))
                        inverseDependencies[dst].Add(src);
                    else
                    {
                        HashSet<Task> srcSet = new HashSet<Task>();
                        srcSet.Add(src);
                        inverseDependencies.Add(dst, srcSet);
                    }
                    setDeadline(dst, src.getDeadline(), visited);
                    Debug.Assert(src.getDeadline() >= dst.getDeadline());
                }
            }
        }

        /*
         * Sets the deadline of this task. This method ensures that the predecessors
         * of this task, and so on recursively, have a deadline that is std.less than
         * the deadline of this task. This method also updates the sorted sets that
         * may contain this task, since the task order depends on the task deadline.
         *
         * @param t a task.
         * @param deadline the new deadline for this task.
         * @param visited a set of tasks already visited by this method.
         */
        private void setDeadline(Task t, uint deadline, ISet<Task> visited)
        {
            if (visited.Contains(t))
            {
                return;
            }
            visited.Add(t);

            TaskGraph tg = t as TaskGraph;
            if (tg != null)
            {
                foreach (Task i in tg.getAllTasks())
                {
                    setDeadline(i, deadline, visited);
                }
            }

            if (t.getDeadline() > deadline)
            {
                bool b1 = removeTask(allReadyTasks, t);
                bool b2 = removeTask(readyCpuTasks, t);
                t.setDeadline(deadline);
                if (b1)
                {
                    insertTask(allReadyTasks, t);
                }
                if (b2)
                {
                    Debug.Assert(!t.isGpuTask());
#if STRICT_PREFETCH
                    if (t.getDeadline() > 0)
                    {
                        insertTask(readyCpuTasks, t);
                    }
#else
                    insertTask(readyCpuTasks, t);
#endif
                }
                ISet<Task> tasks;
                if (dependencies.TryGetValue(t, out tasks))
                {
                    foreach (Task j in tasks)
                    {
                        setDeadline(j, deadline, visited);
                    }
                }
            }
        }

        /*
         * Updates the data structures after the execution of a task. This method
         * removes the given task from #dependencies and #inverseDependencies. This
         * can make new tasks ready to be executed, which are then added to
         * #allReadyTasks and #readyCpuTasks. Finally t.setIsDone(true) is called.
         *
         * @param t a completed task.
         * @param changes true if the task execution changed the result of its
         *      previous execution.
         */
        private void taskDone(Task t, bool changes)
        {
            lock (mutex)
            {
                uint completionDate = changes ? time : t.getCompletionDate();
                ISet<Task> i;
                if (inverseDependencies.TryGetValue(t, out i))
                {
                    // iterates over the successors of t
                    foreach (Task r in i) // r is a successor of t
                    {
                        // the predecessors of r should not be empty, and should contain t
                        ISet<Task> k;
                        Debug.Assert(dependencies.TryGetValue(r, out k));
                        // we then remove t from the predecessors of r
                        k.Remove(t);
                        // if t was the only remaining predecessor of r,
                        // r is now ready to be executed
                        if (k.Count == 0)
                        {
                            dependencies.Remove(r);
                            // we add it to the set of ready tasks, and signals this to the
                            // execution threads; we do the same for the set of ready CPU
                            // tasks, if r is a CPU tas
                            insertTask(allReadyTasks, r);
                            lock (allTasksCond)
                            {
                                Monitor.PulseAll(allTasksCond); //pthread_cond_broadcast( allTasksCond);
                            }
#if STRICT_PREFETCH
                            if (!r.isGpuTask() && r.getDeadline() > 0)
                            {
#else
                            if (!r.isGpuTask())
                            {
#endif
                                insertTask(readyCpuTasks, r);
                                lock (allTasksCond)
                                {
                                    Monitor.PulseAll(allTasksCond); //pthread_cond_broadcast( allTasksCond);
                                }
                            }
                        }
                    }
                    inverseDependencies.Remove(t);
                }
                prefetchQueue.Remove(t);
                // finally we mark the task as completed
                t.setIsDone(true, completionDate);
                // and we increment the logical time counter
                ++time;
            }
        }

        /*
         * The method executed by the additional threads of this scheduler. This
         * method contains an infinite loop that executes tasks when they are ready
         * to be executed. The method returns only when #stop is set to true.
         */
        private void schedulerThread()
        {
            Sxta.Core.Timer timer = new Sxta.Core.Timer();

            // loop to execute tasks, until the scheduler must be deleted
            while (!stop)
            {
                Task t = null;
                lock (mutex)
                {
                    // wait until we have a CPU task ready to be executed (the additional
                    // threads cannot execute GPU tasks, because OpenGL supports only one
                    // thread at a time), or the scheduler is being deleted
                    while (readyCpuTasks.Count == 0 && !stop)
                    {
                        lock (cpuTasksCond)
                        {
                            Monitor.Wait(cpuTasksCond);
                            //pthread_cond_wait(cpuTasksCond, mutex);
                        }
                    }
                    if (!stop)
                    {
                        //SortedTaskSet.iterator i = readyCpuTasks.begin();
                        //Debug.Assert(i != readyCpuTasks.end());
                        //Debug.Assert(i.second.begin() != i.second.end());
                        // selects the first ready task
                        t = readyCpuTasks.First().Value.First();
                        //t = *(i.second.begin());
#if STRICT_PREFETCH
                        Debug.Assert(t.getDeadline() > 0);
#endif
                        // and removes it from the task sets,
                        // so that other threads will not select it again
                        if (t.getDeadline() == 0)
                        {
                            immediateTasks.Remove(t);
                        }
                        removeTask(allReadyTasks, t);
                        removeTask(readyCpuTasks, t);
                    }
                }

                if (!stop)
                {
                    Debug.Assert(!t.isGpuTask());
                    bool changes = false;
                    if (!t.isDone())
                    {
                        log.Debug("PREFETCH " + t.GetType());

                        // same thing as in the #run method
                        if (t.getCompletionDate() >= t.getPredecessorsCompletionDate())
                        {
                            // t is up to date, it is not necessary to run it
                        }
                        else if (framePeriod > 0.0)
                        {
                            timer.start();
                            changes = t.run();
                            double duration = timer.end();
                            t.setActualDuration((float)duration);
                        }
                        else
                        {
                            changes = t.run();
                        }
                    }
                    taskDone(t, changes);
                }
            }
        }

        /*
         * Writes the buffered frame statistics to the statisticsFile.
         */
        private void clearBufferedFrames()
        {
            if (statisticsFile == null)
            {
                statisticsFile = new StreamWriter("taskStatistics.dat");
                statisticsFile.Write("frame scheduling total");
                for (int i = 0; i < monitoredTasks.Count; ++i)
                {
                    statisticsFile.Write(" {0} {1}", monitoredTasks[i], monitoredTasks[i]);
                }
                statisticsFile.WriteLine();
            }
            int s = 0;
            for (int i = 0; i < bufferedFrames; ++i)
            {
                statisticsFile.Write("{0} {1} {2}", i, bufferedStatistics[s] * 1e-3, bufferedStatistics[s + 1] * 1e-3);
                s += 2;
                for (int j = 0; j < monitoredTasks.Count; ++j)
                {
                    statisticsFile.Write(" {0} {1}", (int)bufferedStatistics[s], bufferedStatistics[s + 1] * 1e-3);
                    s += 2;
                }
                statisticsFile.WriteLine();
            }
            bufferedFrames = 0;
        }

        /*
         * Static method needed by pthread to launch a thread. This method just
         * calls #schedulerThread on the MultithreadScheduler passed as argument.
         *
         * @param arg a MultithreadScheduler.
         */
        public static void schedulerThread(object arg)
        {
            ((MultithreadScheduler)arg).schedulerThread();
        }

        /*
         * Returns a tasks from the given set with, if possible, the same execution
         * context as the given one.
         *
         * @param s a task set.
         * @param previousContext an execution context.
         * @return a task from s with, if possible, the given execution context. The
         *      returned task is not removed from the set.
         */
        private static Task getTask(IDictionary<taskKey, SortedSet<Task>> s, object previousContext)
        {
            var i = s.First();
            // the task set should contain at least one task
            Debug.Assert(s.Count >= 1);
            // we compute the deadline of the first task, i.e., since the tasks are
            // sorted by deadline first, the minimum deadline of the tasks in the set
            uint deadline = i.Key.first;
            // we then try to find a task with this deadline, of context 'previousContext'
            taskKey key = new taskKey() { first = deadline, context = previousContext };
            SortedSet<Task> j;
            if (s.TryGetValue(key, out j))
            {
                // if we find one we return it, this will avoid a context switch
                Debug.Assert(j.Count >= 1);
                return j.First();
            }
            if (previousContext != null)
            {
                // if there is no task with the same context, and if the current context
                // is not empty, we try to find a task with an empty context; this will
                // alse avoid a context switch (and in the meantime other tasks with the
                // same context may become ready)
                key = new taskKey() { first = deadline, context = null };
                if (s.TryGetValue(key, out j))
                {
                    Debug.Assert(j.Count > 1);
                    return j.First();
                }
            }
            // in all other cases we just return the first found task
            return i.Value.First();
        }

        /*
         * Inserts a task in the given set.
         *
         * @param s a task set.
         * @param t the task to be added in s.
         */
        private static void insertTask(IDictionary<taskKey, SortedSet<Task>> s, Task t)
        {
            // computes the key for this task and inserts it
            taskKey key = new taskKey { first = t.getDeadline(), context = t.getContext() };
            if (!s.ContainsKey(key))
            {
                SortedSet<Task> ss = new SortedSet<Task>();
                ss.Add(t);
                s.Add(key, ss);
            }
            else
                s[key].Add(t);

        }


        /*
         * Removes a task from the given set.
         *
         * @param s a task set.
         * @param t the task to be removed from s.
         * @return true if the set contained t.
         */
        private static bool removeTask(IDictionary<taskKey, SortedSet<Task>> s, Task t)
        {
            // computes the key for this task and finds it in the set
            taskKey key = new taskKey { first = t.getDeadline(), context = t.getContext() };
            SortedSet<Task> i;
            if (s.TryGetValue(key, out i))
            {
                // if found, removes it from the set of tasks associated with this key
                bool ok = i.Remove(t);
                if (i.Count == 0)
                {
                    // and if this set becomes empty, removes the key and its value
                    s.Remove(key);
                }
                return ok;
            }
            return false;
        }

        private static void getAbsoluteTime(out DateTime time)
        {
            time = DateTime.Now;
        }

        public void swap(MultithreadScheduler obj)
        {
            throw new NotImplementedException();
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static taskKeySort taskKeyComparer = new taskKeySort();
    }

    /**
     * A sort operator for task keys. Task keys are sorted based on their
     * deadlines and if their deadlines are equal, based on their execution
     * context. This ensures that tasks whose result is needed first are
     * executed first, while providing a grouping of tasks that share the same
     * the same execution context (which can save context switches).
     */
    public class taskKeySort : IComparer<MultithreadScheduler.taskKey>
    {
        public int Compare(MultithreadScheduler.taskKey x, MultithreadScheduler.taskKey y)
        {
            uint xDeadline = x.first;
            uint yDeadline = y.first;
            if (xDeadline == yDeadline)
            {
                int xCtx = (x.context != null ? x.context.GetHashCode() : 0);
                int yCtx = (y.context != null ? y.context.GetHashCode() : 0);
                return xCtx - yCtx;
            }
            else
            {
                return (int)(xDeadline - yDeadline);
            }
        }
    }
}
