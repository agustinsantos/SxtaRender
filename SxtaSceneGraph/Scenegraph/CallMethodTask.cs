using log4net;
using Sxta.Core;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sxta.Render.Scenegraph
{
    /**
     * An AbstractTask to execute a Method on a SceneNode.
     * @ingroup scenegraph
     */
    public class CallMethodTask : AbstractTask, ISwappable<CallMethodTask>
    {

        /**
         * Creates a new CallMethodTask.
         *
         * @param method a "node.method" qualified name. The first part specifies
         *      the scene node on which the method must be called. The second part
         *      specifies the name of the method that must be called.
         */
        public CallMethodTask(QualifiedName method)
            : base("CallMethodTask")
        {
            init(method);
        }


        /**
         * Deletes this CallMethodTask.
         */
        //TODO ~CallMethodTask() {}

        public override Task getTask(Object context)
        {
            SceneNode n = ((Method)context).getOwner();
            SceneNode target = method.getTarget(n);
            if (target != null)
            {
                Method m = target.getMethod(method.name);
                if (m != null)
                {
                    if (m.isEnabled())
                    {
                        return m.getTask();
                    }
                    else
                    {
                        return new TaskGraph();
                    }
                }
            }
            if (log.IsErrorEnabled)
            {
                log.Error("CallMethod: cannot find method '" + method.target + "." + method.name + "'");
            }
            throw new Exception("CallMethod: cannot find method '" + method.target + "." + method.name + "'");
        }


        /**
         * Creates an empty CallMethodTask.
         */
        public CallMethodTask()
            : base("CallMethodTask")
        {
        }

        /**
         * Initializes this CallMethodTask.
         *
         * @param method a "node.method" qualified name. The first part specifies
         *      the scene node on which the method must be called. The second part
         *      specifies the name of the method that must be called.
         */
        public void init(QualifiedName method)
        {
            this.method = method;
        }


        /**
         * Swaps this CallMethodTask with the given one.
         *
         * @param t a CallMethodTask.
         */
        public void swap(CallMethodTask t)
        {
            Std.Swap(ref method, ref t.method);
        }



        /**
         * A "node.method" qualified name. The first part specifies the scene node
         * on which the method must be called. The second part specifies the name of
         * the method that must be called.
         */
        private QualifiedName method;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    }
}
