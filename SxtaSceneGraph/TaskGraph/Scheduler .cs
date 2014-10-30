using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxta.Render.Scenegraph
{
    /// <summary>
    /// An abstract scheduler, sorts and executes tasks with one or more threads.
    /// </summary>
    public abstract class Scheduler
    {

        /*
         * Creates a new scheduler.
         *
         * @param type the type of this scheduler.
         */
        public Scheduler(string type) { }

        /*
         * Deletes this scheduler.
         */
        //public virtual ~Scheduler();

        /*
         * Returns true if this scheduler can execute CPU or GPU tasks whose
         * deadline is not immediate. This means tasks whose result will be needed
         * in the next few frames, but that are known in advance and could be
         * computed ahead of time to reduce the load of these coming frames.
         *
         * @param gpuTasks true to know if this scheduler can prefetch GPU tasks,
         *      or false to know if it can prefetch CPU tasks.
         * @return true if this scheduler can prefetch GPU (resp. CPU) tasks, if
         *      gpuTasks is true (resp. false).
         */
        public abstract bool supportsPrefetch(bool gpuTasks);

        /*
         * Adds a task whose deadline is not immediate. This method must not be
         * called if this scheduler does not support prefetch (see
         * #supportsPrefetch). Otherwise it adds this task and its sub tasks to the
         * list of tasks to be executed by this scheduler, and returns immediately
         * (i.e. before these tasks are executed).
         *
         * @param task a task or task graph whose deadline is not immediate.
         */
        public abstract void schedule( Task  task);

        /*
         * Forces the reexecution of the given task and of its sub tasks.
         *
         * @param task a task or task graph that must be reexecuted. This task is marked
         *      as undone (with Task#setIsDone) so that it will be reexecuted.
         * @param r the reason why the task must be reexecuted.
         * @param deadline the frame number before which this task must be
         *      reexecuted.
         */
        public abstract void reschedule(Task task, Task.reason r, uint deadline);

        /*
         * Executes the given tasks. This method does not return before all tasks
         * with an immediate deadline are completed.
         *
         * @param task a task or task graph to be executed.
         */
        public abstract void run(Task task);


        /*
         * Swaps this scheduler with the given one.
         */
        protected void swap(Scheduler s) { }
    }
}
