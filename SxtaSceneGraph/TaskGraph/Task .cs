using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sxta.Render.Scenegraph
{

    /// <summary>
    /// An abstract Task. A task can be a CPU or GPU task, it has a deadline measured
    /// as the frame number before which the task must be done. A task also has a
    /// complexity, which is used to predict the duration of this task from the
    /// measure of the duration of previous tasks of the same type. A task can be
    /// made of several tasks organized in a graph (see TaskGraph). Finally a GPU task
    /// can have an execution context (for instance an OpenGL %state) which must be
    /// setup before the task is run. In order to reduce the number of context
    /// switches, the context setup and cleanup are isolated in the
    /// #begin and #end methods, while the task itself is implemented in the #run
    /// method. The context itself is returned by the #getContext method, which
    /// allows tasks that share the same context to be executed in a group. For
    /// instance if t1, t2 and t3 are GPU tasks with the same context, they can be
    /// executed with t1.begin, t1.run, t2.run, t3.run and t3.end instead of t1.begin,
    /// t1.run, t1.end, t2.begin, t2.run, t2.end, t3.begin, t3.run, and t3.end, which
    /// saves two context switches.
    /// </summary>
    public class Task : IComparable<Task>
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        /*
         * Possible reasons for which a task must be reexecuted.
         */
        public enum reason
        {
            DEPENDENCY_CHANGED, ///< data used by this task and produced by a predecessor task has changed
            DATA_CHANGED, ///< data used by this task but not produced by another task has changed
            DATA_NEEDED ///< result of this task is needed again by a successor task of this task
        }

        /*
         * Creates a new task.
         *
         * @param type the type of the task.
         * @param gpuTask if the task must be executed on GPU.
         * @param deadline the frame number before which the task must be executed.
         *      0 means that the task must be executed immediately.
         */
        public Task(string type, bool gpuTask, uint deadline)
        {
            this.ObjectName = type;
            this.completionDate = 0;
            this.gpuTask = gpuTask;
            this.deadline = deadline;
            this.predecessorsCompletionDate = 1;
            this.done = false;
            this.expectedDuration = -1.0f;

            if (mutex == null)
            {
                mutex = new object();
                //pthread_mutex_init((pthread_mutex_t*)mutex, null);
            }
        }

        /*
         * Deletes this task.
         */
        //public virtual ~Task(){}

        /*
         * Returns the execution context of this task. This context is used to sort
         * GPU tasks that share the same context, in order to save context switches.
         * The context is unused for CPU tasks, and can be NULL.
         */
        public virtual object getContext()
        {
            return null;
        }

        /*
         * Returns true if this task is a GPU task.
         */
        public bool isGpuTask()
        {
            return gpuTask;
        }

        /*
         * Returns the frame number before which this task must be completed.
         */
        public uint getDeadline()
        {
            return deadline;
        }

        /*
         * Sets the frame number before which this task must be completed. <i>For
         * internal use only</i>. This method is called by schedulers, it must not
         * called directly by users.
         */
        public void setDeadline(uint deadline)
        {
            this.deadline = System.Math.Min(this.deadline, deadline);
        }

        /*
         * Returns the complexity of this task. This number is used to estimate the
         * duration d of this task as d=k*complexity, where k is estimated based on
         * the actual duration and complexity of previous tasks of the same type
         * (see #getTypeInfo).
         */
        public virtual int getComplexity()
        {
            return 1;
        }

        /*
         * Prepares this task before its execution. This method is called when the
         * task is scheduled to be executed. It can perform work that cannot be
         * executed during the task execution itself, such as scheduling new tasks
         * for execution (indeed, once a set of tasks has been scheduled for
         * execution, this set cannot be modified, i.e. a task cannot schedule the
         * execution of a task that was not previously in this set). The default
         * implementation of this task does nothing.
         *
         * @param initialized the tasks already initialized. This set is used to
         *      avoid initializing several times the same task (for tasks that
         *      belong to several task graphs).
         */
        public virtual void init(ISet<Task> initialized)
        {
        }

        /*
         * Sets the execution context for this task. All tasks that share the same
         * execution context must do the same work in this method, i.e. if t1 and t2
         * return the same object in #getContext, then t1.begin and t2.begin should
         * be equivalent.
         */
        public virtual void begin()
        {
        }

        /*
         * Executes this task.
         *
         * @return true if the result of this execution is different from the result
         *      of the last execution of this task. Indeed a task can be executed
         *      several times (see Scheduler#reschedule), and its result may or may
         *      not change at each execution. If the result does not change, then
         *      a task that depends on this task will <i>not</i> be reexecuted if
         *      its own data has not changed (i.e. if the reason for its
         *      rescheduling was DEPENDENCY_CHANGED -- we assume in this framework
         *      that the result of any task is deterministic and depends only on its
         *      own data and on the result of the predecessor tasks). The default
         *      implementation of this method returns true, which is the safe
         *      default result (returning true even if the result of the task has
         *      not changed is not a problem, but returning false while the result
         *      <i>has</i> changed will cause problems).
         */
        public virtual bool run()
        {
            Debug.Assert(!done);
            return true;
        }

        /*
         * Restores the execution context after this task. All tasks that share the
         * same execution context must do the same work in this method, i.e. if t1
         * and t2 return the same object in #getContext, then t1.end and t2.end
         * should be equivalent.
         */
        public virtual void end()
        {
        }

        /*
         * Returns true if this task is completed.
         */
        public virtual bool isDone()
        {
            return done;
        }

        /*
         * Sets the execution %state of this task. If the task is completed and its
         * execution %state is set to "not done" then it will be executed again.
         * <i>For internal use only</i>. This method is called by schedulers, it
         * must not called directly by users.
         *
         * @param done true if task is completed, false otherwise.
         * @param t if done is true, the task's completion date (a frame number).
         * @param r if done is false, indicates why the task must be reexecuted.
         */
        public virtual void setIsDone(bool done, uint t, reason r = reason.DATA_NEEDED)
        {
            if (this.done != done)
            {
                this.done = done;
                if (done || r != reason.DEPENDENCY_CHANGED)
                {
                    this.completionDate = t;
                }
                for (int i = 0; i < listeners.Count; ++i)
                {
                    listeners[i].taskStateChanged(this, done, r);
                }
            }
        }

        /*
         * Returns the time at which this task was completed. This completion date
         * is not reinitialized when the task is marked as not done, to force its
         * reexecution. It is not changed either if the task result does not change
         * after a reexecution (see #run). Hence this date gives the <i>last
         * modification date</i> of the result of this task.
         */
        public uint getCompletionDate()
        {
            return completionDate;
        }

        /*
         * Returns the last completion date of the predecessors of this task.
         */
        public uint getPredecessorsCompletionDate()
        {
            return predecessorsCompletionDate;
        }

        /*
         * Sets the last completion date of the predecessors of this task. <i>For
         * internal use only</i>. This method is called by schedulers, it must not
         * called directly by users.
         *
         * @param t the completion date of a predecessor task of this task.
         */
        public virtual void setPredecessorsCompletionDate(uint t)
        {
            predecessorsCompletionDate = System.Math.Max(predecessorsCompletionDate, t);
        }

        /*
         * Returns the expected duration of this task in micro seconds. The result
         * is based on the complexity of this task (see #getComplexity).
         */
        public float getExpectedDuration()
        {
            if (expectedDuration == -1.0f)
            {
                expectedDuration = 0.0f;
                Type id = getTypeInfo();
                lock (mutex)
                {
                    TaskStatistics stats;
                    if (statistics.TryGetValue(id, out stats))
                    {
                        // to get "valid" statistics, we wait until we have enough samples,
                        // and we ignore the min and max values
                        const int MIN_SAMPLES = 64;
                        if (stats.n >= MIN_SAMPLES)
                        {
                            if (!stats.corrected)
                            {
                                float sum = stats.durationSum - stats.maxDuration - stats.minDuration;
                                float squareSum = stats.durationSum - stats.maxDuration * stats.maxDuration - stats.minDuration * stats.minDuration;
                                stats.durationSum = (sum * stats.n) / (stats.n - 2);
                                stats.durationSquareSum = (squareSum * stats.n) / (stats.n - 2);
                                stats.corrected = true;
                            }
                            float mean = stats.durationSum / stats.n;
                            float squareMean = stats.durationSquareSum / stats.n;
                            float variance = squareMean - mean * mean;
                            float standardDeviation = (float)System.Math.Sqrt(variance);
                            expectedDuration = (mean + 2.0f * standardDeviation) * getComplexity();
                        }
                    }
                }
            }
            return expectedDuration;
        }

        /*
         * Sets the actual duration of this task. This actual duration is used to
         * improve the estimator for the duration of tasks of this type (see
         * #getTypeInfo). <i>For internal use only</i>. This method is called by
         * schedulers, it must not called directly by users.
         *
         * @param duration the actual duration of this task in micro seconds.
         */
        public void setActualDuration(float duration)
        {
            Type id = getTypeInfo();

            lock (mutex)
            {
                TaskStatistics stats;
                if (!statistics.TryGetValue(id, out stats))
                {
                    stats = new TaskStatistics();
                    statistics.Add(id, stats);
                }

                duration = duration / getComplexity();
                stats.durationSum += duration;
                stats.durationSquareSum += duration * duration;
                stats.minDuration = System.Math.Min(duration, stats.minDuration);
                stats.maxDuration = System.Math.Max(duration, stats.maxDuration);
                stats.n += 1;
            }
        }

        /*
         * Adds a listener to this task.
         *
         * @param l a task listener.
         */
        public void addListener(ITaskListener l)
        {
            listeners.Add(l);
        }

        /*
         * Removes a listener from this task.
         *
         * @param l a task listener.
         */
        public void removeListener(ITaskListener l)
        {
            listeners.Remove(l);
        }

        /*
         * Logs the statistics about the execution time of the tasks, depending on
         * their type.
         */
        public static void logStatistics()
        {
            lock (mutex)
            {
                foreach (var st in statistics)
                {
                    TaskStatistics stats = st.Value;
                    float mean = stats.durationSum / stats.n;
                    float squareMean = stats.durationSquareSum / stats.n;
                    float variance = squareMean - mean * mean;
                    float standardDeviation = (float)System.Math.Sqrt(variance);

                    string oss = st.Key.FullName + ": " + mean / 1000.0 + " +/- " + standardDeviation / 1000.0 + "; min/max " + stats.minDuration / 1000.0 + " " + stats.maxDuration / 1000.0;
                    log.Debug(oss);
                }
            }
        }


        protected uint completionDate; ///< time at which this task was completed.

        protected List<ITaskListener> listeners = new List<ITaskListener>(); ///< the listeners of this tasks.

        /* 
         * Returns the type of this task. This type is used to group the execution
         * time statistics of tasks of the same type.
         */
        protected virtual Type getTypeInfo()
        {
            return this.GetType();
        }


        /*
         * Sort operator forstd::type_info objects.
         */
        private struct TypeInfoSort //TODO : public std::less<std::type_info*>
        {
            //TODO bool operator()(const std::type_info *x, const std::type_info *y) const;
        }

        /*
         * Execution time statistics for tasks of a given type.
         */
        private class TaskStatistics
        {
            public float durationSum = 0.0f; ///< sum of the execution times.

            public float durationSquareSum = 0.0f; ///< sum of the squares of the execution times.

            public float minDuration = float.PositiveInfinity; ///< minimum execution time.

            public float maxDuration = 0.0f; ///< maximum execution time.

            public bool corrected = false; ///< true if min and max values have been removed from sum and squareSum

            public int n = 0; ///< number of executions.
        }

        private bool gpuTask; ///< true is this task is a GPU task.

        private uint deadline; ///< frame number before which this tasks must be completed.

        private uint predecessorsCompletionDate; ///< last completion date of the predecessors of this task.

        private bool done; ///< true is the task is completed.

        private float expectedDuration; ///< expected duration of this task.

        private static object mutex; ///< mutex used to synchronize accesses to #statistics

        private string ObjectName;

        /*
         * The execution time statistics for each task type. Maps TaskStatistics to
         *std::type_info objects. //TODO sorted by TypeInfoSort
         */
        private static Dictionary<Type, TaskStatistics> statistics = new Dictionary<Type, TaskStatistics>();

        private ulong order = cnt++;
        private static ulong cnt = ulong.MinValue;


        /**
         * A sort operator for tasks. This operator is based on the expected
         * duration of tasks, so that shorter tasks are executed first.
         */
        public int CompareTo(Task other)
        {
            float xDuration = this.getExpectedDuration();
            float yDuration = other.getExpectedDuration();
            if (xDuration == yDuration)
                return (int)(this.order - other.order);
            else
                return (int)(xDuration - yDuration);
        }
    }

    /// <summary>
    /// A task listener, notified when changes occur in a task.
    /// </summary>
    public interface ITaskListener
    {

        /// <summary>
        ///  Notifies this listener that the execution state of the given task has
        /// changed.
        /// </summary>
        /// <param name="t">the task whose execution state has changed.</param>
        /// <param name="done">the new execution state.</param>
        /// <param name="r">if done is false, the reason why the task must be reexecuted.</param>
        void taskStateChanged(Task t, bool done, Task.reason r);

        /// <summary>
        /// Notifies this listener that the completion date of the given task has
        /// changed.
        /// </summary>
        /// <param name="t">the task whose completion date has changed.</param>
        /// <param name="date">the new completion date.</param>
        void completionDateChanged(Task t, uint date);
    }
}
