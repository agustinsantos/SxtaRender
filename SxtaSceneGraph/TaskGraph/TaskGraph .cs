using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Sxta.Render.Scenegraph
{

    /// <summary>
    /// A task made of several sub tasks that may depend on each other. A dependency
    /// between two tasks means that one task must be executed before the other. More
    /// precisely a dependency from task src to task dst means that dst must be
    /// executed before src. We say that dst is a predecessor task for src, and that
    /// src is a successor task for dst. Likewise, we say that src has a predecessor
    /// task (dst), and that dst has a successor task (src). A task graph is
    /// completed when all its sub tasks are executed.
    /// </summary>
    public class TaskGraph : Task, ITaskListener
    {

        /*
         * An iterator to iterate over a set of tasks.
         */
        //public typedef SetIterator< Task  > TaskIterator;

        /*
         * Creates a new, empty task graph.
         */
        public TaskGraph()
            : base("TaskGraph", false, 0)
        {
        }

        /*
         * Creates a new task graph that encapsulates the given task.
         */
        public TaskGraph(Task t)
            : base("TaskGraph", false, 0)
        {
            addTask(t);
        }

        /*
         * Deletes this tak graph.
         */
        //public virtual ~TaskGraph();

        /*
         * Calls init recursively on all sub tasks of this task graph.
         */
        public override void init(ISet<Task> initialized)
        {
            if (!initialized.Contains(this))
            {
                initialized.Add(this);
                IEnumerable<Task> tasks = getAllTasks();
                initTasks(tasks, initialized);
            }
        }

        /*
         * Calls #setIsDone recursively on all sub tasks of this task graph.
         */
        public override void setIsDone(bool done, uint t, reason r)
        {
            base.setIsDone(done, t, r);
            if (!done)
            { // calls sub tasks recursively only if task must be reexecuted
                // if a dependency of this task graph has changed, then all sub tasks
                // must be reexecuted; otherwise, if the data produced by this graph
                // is needed again then, a priori, only the sub tasks without successors
                // must be reexecuted (these sub tasks may need other sub tasks to be
                // reexecuted if they need their data; in this case they can change
                // their execution state recursively in their own setIsDone method).
                IEnumerable<Task> tasks = (r == reason.DEPENDENCY_CHANGED) ? getAllTasks() : getLastTasks();
                foreach (Task task in tasks)
                {
                    task.setIsDone(done, t, r);
                }
            }
        }

        /*
         * Calls #setPredecessorsCompletionDate on the sub tasks of this task
         * without predecessors.
         */
        public override void setPredecessorsCompletionDate(uint t)
        {
            foreach (Task task in firstTasks)
            {
                task.setPredecessorsCompletionDate(t);
            }
        }

        /*
         * Returns true if this task graph does not contain any sub task.
         */
        public bool isEmpty()
        {
            return allTasks.Count == 0;
        }

        /*
         * Returns all the sub tasks of this task. The returned iterator does not
         * iterate recursively in the sub sub tasks, if any.
         */
        public IEnumerable<Task> getAllTasks()
        {
            return allTasks;
        }

        /*
         * Returns the sub tasks that do not have any predecessor. These tasks are
         * executed first. The returned iterator does not iterate recursively in the
         * sub sub tasks, if any.
         */
        public IEnumerable<Task> getFirstTasks()
        {
            return firstTasks;
        }

        /*
         * Returns the sub tasks that do not have any successor. These tasks are
         * executed last. The returned iterator does not iterate recursively in the
         * sub sub tasks, if any.
         */
        public IEnumerable<Task> getLastTasks()
        {
            return lastTasks;
        }

        /*
         * Returns the predecessor tasks of the given task.
         *
         * @param t a sub task of this task graph.
         */
        public IEnumerator<Task> getDependencies(Task t)
        {
            ISet<Task> val;
            if (dependencies.TryGetValue(t, out val))
            {
                return val.GetEnumerator();
            }
            return Enumerable.Empty<Task>().GetEnumerator();
        }

        /*
         * Returns the successor tasks of the given task.
         *
         * @param t a sub task of this task graph.
         */
        public IEnumerator<Task> getInverseDependencies(Task t)
        {
            ISet<Task> val;
            if (inverseDependencies.TryGetValue(t, out val))
            {
                return val.GetEnumerator();
            }
            return Enumerable.Empty<Task>().GetEnumerator();
        }

        /*
         * Adds a sub task to this task graph. Note that a task can be added to
         * several task graphs at the same time.
         *
         * @param t the task to be added to this sub graph. This task can be a task
         *      graph itself.
         */
        public void addTask(Task t)
        {
            //Debug.Assert(t.cast<TaskGraph>() == NULL || !t.cast<TaskGraph>().isEmpty());
            if (!allTasks.Contains(t))
            {
                // if the task has not already been added we add ourselves has a listener
                t.addListener(this);
                // and we add the task in all three sets of tasks
                allTasks.Add(t);
                firstTasks.Add(t);
                lastTasks.Add(t);
            }
        }

        /*
         * Removes a sub task from this task graph. This sub task must not have any
         * dependencies or inverse dependencies.
         *
         * @param t the task to be removed from this sub graph.
         */
        public void removeTask(Task t)
        {
            if (allTasks.Contains(t))
            {
                // we remove ourselves from the listeners of t
                t.removeListener(this);
                // and we remove t from all data structures
                allTasks.Remove(t);
                firstTasks.Remove(t);
                lastTasks.Remove(t);
                Debug.Assert(!dependencies.ContainsKey(t));
                Debug.Assert(!inverseDependencies.ContainsKey(t));
            }
        }

        /*
         * Adds a dependency between two sub tasks of this task graph.
         *
         * @param src a sub task of this graph that must be executed after dst.
         * @param dst a sub task of this graph that must be executed before src.
         */
        public void addDependency(Task src, Task dst)
        {
            Debug.Assert(allTasks.Contains(src));
            Debug.Assert(allTasks.Contains(dst));
            // src now has a predecessor,
            // so it must be removed from the set of tasks without predecessor
            firstTasks.Remove(src);
            // dst now has a successor,
            // so it must be removed from the set of tasks without successor
            lastTasks.Remove(dst);
            // updates the successors and predecessors maps
            if (!dependencies.ContainsKey(src))
            {
                HashSet<Task> dep = new HashSet<Task>();
                dep.Add(dst);
                dependencies.Add(src, dep);
            }
            else
                dependencies[src].Add(dst);

            if (!inverseDependencies.ContainsKey(dst))
            {
                HashSet<Task> dep = new HashSet<Task>();
                dep.Add(src);
                dependencies.Add(dst, dep);
            }
            else
                inverseDependencies[dst].Add(src);
        }

        /*
         * Removes a dependency between two sub tasks of this task graph.
         *
         * @param src a sub task of this graph that must be executed after dst.
         * @param dst a sub task of this graph that must be executed before src.
         */
        public void removeDependency(Task src, Task dst)
        {
            Debug.Assert(allTasks.Contains(src));
            Debug.Assert(allTasks.Contains(dst));
            // updates the successors and predecessors maps
            dependencies[src].Remove(dst);
            if (dependencies[src].Count == 0)
            {
                dependencies.Remove(src);
                // src has no more predecessors,
                // so it must be added to the set of tasks without predecessors
                firstTasks.Add(src);
            }
            inverseDependencies[dst].Remove(src);
            if (inverseDependencies[dst].Count == 0)
            {
                inverseDependencies.Remove(dst);
                // dst now has no more successors,
                // so it must be added to the set of tasks without successor
                lastTasks.Add(dst);
            }
        }

        /*
         * Removes all dependencies of the given subtask.
         * All deleted dependencies are stored in deletedDependencies.
         *
         * @param src a sub task of this graph.
         * @param[out] deletedDependencies the dependencies that src had.
         */
        public void removeAndGetDependencies(Task src, ISet<Task> deletedDependencies)
        {
            // find the set of dependencies for this task
            ISet<Task> dests;
            if (dependencies.TryGetValue(src, out dests))
            { // there are dependencies
                foreach (Task task in dests)
                {
                    deletedDependencies.Add(task);
                    ISet<Task> dstSet = inverseDependencies[task];
                    bool removed = dstSet.Remove(src);
                    Debug.Assert(removed); // should exist in inverse dependencies
                    if (dstSet.Count == 0)
                    {
                        // dst now has no more successors,
                        // so it must be added to the set of tasks without successor
                        inverseDependencies.Remove(task);
                        lastTasks.Add(task);
                    }
                }
                // erase src dependencies, erase all components
                dependencies.Remove(src);
                // src has no more predecessors,
                // so it must be added to the set of tasks without predecessors
                firstTasks.Add(src);
            }
        }

        /*
         * Removes all the dependencies between the sub tasks of this task graph.
         */
        public void clearDependencies()
        {
            firstTasks.UnionWith(allTasks);
            lastTasks.UnionWith(allTasks);
            dependencies.Clear();
            inverseDependencies.Clear();
        }

        /*
         * Notifies the listeners of this task that its execution state has changed.
         * This method is called when the execution state of a sub task of this graph
         * has changed. Indeed a TaskGraph is a TaskListener that listens to state
         * changes in all its sub tasks.
         */
        public virtual void taskStateChanged(Task t, bool done, Task.reason r)
        {
            Debug.Assert(allTasks.Contains(t));
            if (!done)
            {
                if (r != Task.reason.DATA_NEEDED)
                {
                    base.setIsDone(false, 0, r);

                    // if the result of t is needed again but has not changed, the
                    // tasks that depend on this result need not be reexecuted.
                    // Otherwise we notify these successor tasks that one of their
                    // dependencies has changed, and that they must be reexecuted:
                    IEnumerator<Task> i = getInverseDependencies(t);
                    while (i.MoveNext())
                    {
                        Task s = i.Current;
                        s.setIsDone(false, 0, reason.DEPENDENCY_CHANGED);
                    }
                }
            }
            else
            {
                // updates the predecessor completion date of the successors of t
                completionDateChanged(t, t.getCompletionDate());

                // if a subtask of this task graph is now completed, the task graph
                // itself may become completed (it can of course not become uncompleted).
                foreach (Task i in getAllTasks())
                {
                    if (!i.isDone())
                    {
                        return;
                    }
                }
                base.setIsDone(true, getCompletionDate());
            }
        }

        /*
         * Notifies the listeners of this task that the completion date of a sub task
         * without successors has changed.
         */
        public virtual void completionDateChanged(Task t, uint date)
        {
            completionDate = System.Math.Max(completionDate, date);
            IEnumerator<Task> ie = getInverseDependencies(t);
            if (ie.MoveNext())
            {
                // if t has successors,
                // updates the predecessor completion date of the successors of t
                {
                    Task s = ie.Current;
                    s.setPredecessorsCompletionDate(date);
                } while (ie.MoveNext()) ;
            }
            else
            {
                // if t does not have predecessors, notifies the listeners of
                // this taskgraph that its completion date has changed
                for (int i = 0; i < listeners.Count; ++i)
                {
                    listeners[i].completionDateChanged(this, date);
                }
            }
        }


        /*
         * Clears the temporary data structures in this task graph that are used by
         * schedulers.
         */
        protected void cleanup()
        {
            flattenedFirstTasks.Clear();
            flattenedLastTasks.Clear();
        }

        /*
         * Initializes a set of tasks. In order to support the modification of a task
         * graph during the initialization of its sub tasks, this method is recursive
         * and initializes the tasks after all the iterator elements have been found.
         * Without this the iterator over the sub tasks could become invalid if the set
         * of sub tasks is modified during the iteration (by the init method of one of
         * the sub tasks).
         */
        private void initTasks(IEnumerable<Task> i, ISet<Task> initialized)
        {
            IEnumerator<Task> tasks = i.GetEnumerator();
            initTasks(tasks, initialized);
        }
        private void initTasks(IEnumerator<Task> i, ISet<Task> initialized)
        {
            if (i.MoveNext())
            {
                Task t = i.Current;
                initTasks(i, initialized);
                t.init(initialized);
            }
        }
        internal ISet<Task> allTasks = new HashSet<Task>(); ///< all the tasks of this graph

        internal ISet<Task> firstTasks = new HashSet<Task>(); ///< the tasks without predecessors

        internal ISet<Task> lastTasks = new HashSet<Task>(); ///< the tasks without successors

        internal ISet<Task> flattenedFirstTasks = new HashSet<Task>(); ///< the primitive tasks without predecessors

        internal ISet<Task> flattenedLastTasks = new HashSet<Task>(); ///< the primitive tasks without successors

        /*
         * The predecessors of the sub tasks of this graph.
         * Maps each task to its set of predecessors.
         */
        private IDictionary<Task, ISet<Task>> dependencies = new Dictionary<Task, ISet<Task>>();

        /*
         * The successors of the sub tasks of this graph.
         * Maps each task to its set of successors.
         */
        private IDictionary<Task, ISet<Task>> inverseDependencies = new Dictionary<Task, ISet<Task>>();
    }
}
