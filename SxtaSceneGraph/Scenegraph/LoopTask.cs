using Sxta.Core;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.Render.Scenegraph
{
    /**
     * An AbstractTask to execute a task on a set of scene nodes.
     * @ingroup scenegraph
     */
    public class LoopTask : AbstractTask, ISwappable<LoopTask>
    {

        /**
         * Creates a new LoopTask.
         *
         * @param var the loop variable name.
         * @param flag a flag that specifies the scene nodes to which the loop must
         *      be applied.
         * @param cull true to apply the loop only on the visible scene nodes.
         * @param parallel true the apply the loop to all scene nodes in parallel.
         * @param subtask the task that must be executed on each SceneNode.
         */
        public LoopTask(string var, string flag, bool cull, bool parallel, TaskFactory subtask)
            : base("LoopTask")
        {
            init(var, flag, cull, parallel, subtask);
        }

        /**
         * Deletes this LoopTask.
         */
        //~LoopTask();

        public override Task getTask(Object context)
        {
            SceneManager manager = ((Method)context).getOwner().getOwner();

            List<SceneNode> nodes = new List<SceneNode>();
             foreach (SceneNode n in manager.getNodes(flag))
            {
                 if (!cull || n.isVisible)
                {
                    nodes.Add(n);
                }
            }

            if (nodes.Count == 1)
            {
                manager.setNodeVar(var, nodes[0]);
                return subtask.getTask(context);
            }
            else
            {
                TaskGraph result = new TaskGraph();
                Task prev = null;
                for (int i = 0; i < nodes.Count; ++i)
                {
                    manager.setNodeVar(var, nodes[i]);
                    try
                    {
                        Task next = subtask.getTask(context);
                        if (!(next is TaskGraph) || !((TaskGraph)next).isEmpty())
                        {
                            result.addTask(next);
                            if (!parallel && prev != null)
                            {
                                result.addDependency(next, prev);
                            }
                            prev = next;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                return result;
            }
        }


        /**
         * Creates an empty LoopTask.
         */
        public LoopTask()
            : base("LoopTask")
        {
        }

        /**
         * Initializes this LoopTask.
         *
         * @param var the loo variable name.
         * @param flag a flag that specifies the scene nodes to which the loop must
         *      be applied.
         * @param cull true to apply the loop only on the visible scene nodes.
         * @param parallel true the apply the loop to all scene nodes in parallel.
         * @param subtask the task that must be executed on each SceneNode.
         */
        public void init(string var, string flag, bool cull, bool parallel, TaskFactory subtask)
        {
            this.var = var;
            this.flag = flag;
            this.cull = cull;
            this.parallel = parallel;
            this.subtask = subtask;
        }

        /**
         * Swaps this LoopTask with the given one.
         *
         * @param t a LoopTask.
         */
        public void swap(LoopTask t)
        {
            Std.Swap(ref var, ref t.var);
            Std.Swap(ref flag, ref t.flag);
            Std.Swap(ref cull, ref t.cull);
            Std.Swap(ref subtask, ref t.subtask);
        }


        /**
         * The loop variable name.
         */
        private string var;

        /**
         * The flag thatt specifies the scene nodes to which the loop must be applied.
         */
        private string flag;

        /**
         * True to apply the loop to all scene nodes in parallel.
         */
        private bool parallel;

        /**
         * True to apply the loop only on the visible scene nodes.
         */
        private bool cull;

        /**
         * The task that must be executed on each scene node.
         */
        private TaskFactory subtask;
    }
}
