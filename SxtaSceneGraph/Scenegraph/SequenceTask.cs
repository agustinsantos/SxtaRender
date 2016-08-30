using log4net;
using Sxta.Core;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sxta.Render.Scenegraph
{
     /// <summary>
    /// An AbstractTask to compose a list of tasks in a sequence.
    /// </summary>
    public class SequenceTask : AbstractTask, ISwappable<SequenceTask>
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///  Creates a SequenceTask with the given list of tasks.
        /// </summary>
        /// <param name="subtasks">the tasks that must be composed into a sequence.</param>
        public SequenceTask(List<TaskFactory> subtasks)
            : base("SequenceTask")
        {
            init(subtasks);
        }

        /*
         * Deletes this SequenceTask.
         */
        // ~SequenceTask(){}

        public override Task getTask(Object context)
        {
            if (subtasks.Count == 1)
            {
                return subtasks[0].getTask(context);
            }
            else
            {
                TaskGraph result = new TaskGraph();
                Task prev = null;
                for (int i = 0; i < subtasks.Count; ++i)
                {
                    try
                    {
                        Task next = subtasks[i].getTask(context);
                        if (!(next is TaskGraph) || !((TaskGraph)next).isEmpty())
                        {
                            result.addTask(next);
                            if (prev != null)
                            {
                                result.addDependency(next, prev);
                            }
                            prev = next;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("SequenceTask. Exception {0}.", ex);
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Creates an empty SequenceTask.
        /// </summary>
        public SequenceTask()
            : base("SequenceTask")
        {
        }

        /// <summary>
        /// Initializes this SequenceTask with the given list of tasks.
        /// </summary>
        /// <param name="subtasks">the tasks that must be composed into a sequence.</param>
        public void init(List<TaskFactory> subtasks)
        {
            this.subtasks = subtasks;
        }


        /**
         * Swaps this SequenceTask with another one.
         *
         * @param t a SequenceTask.
         */
        public void swap(SequenceTask t)
        {
            Std.Swap(ref subtasks, ref t.subtasks);
        }



        /**
         * The tasks that are composed sequentially by this task.
         */
        private List<TaskFactory> subtasks;
    }
}
