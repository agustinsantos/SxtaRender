using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxta.Render.Scenegraph
{
    /**
     * An abstract task for a Method. A method "task" is in fact a TaskFactory that
     * creates Task. Indeed a new Task is created at each method invocation.
     *
     * @ingroup scenegraph
     */
    public abstract class AbstractTask : TaskFactory
    {

        /**
         * Creates a new AbstractTask.
         *
         * @param type the task type.
         */
        public AbstractTask(string type) : base(type) { }

        /**
         * Deletes this AbstractTask.
         */
        //~AbstractTask();

        /**
         * A qualified name of the form <i>target</i>.<i>name</i>.
         */
        public struct QualifiedName
        {
            /**
             * The first part of this qualified name. The first part is optional.
             * It can be "this", "$v" or any scene node flag.
             */
            public string target;

            /**
             * The second part of this qualified name.
             */
            public string name;

            /**
             * Creates an empty qualified name.
             */
            //public QualifiedName();

            /**
             * Creates a qualified name.
             *
             * @param n a qualified name of the form <i>target</i>.<i>name</i> or
             *      <i>name</i>.
             */
            public QualifiedName(string n)
            {
                int i = n.IndexOf('.');
                if (i != -1)
                {
                    target = n.Substring(0, i);
                    name = n.Substring(i + 1);
                }
                else
                {
                    name = n;
                    target = null;
                }
            }

            /**
             * Returns the SceneNode designated by this qualified name.
             *
             * @param context the scene graph into which the target SceneNode must
             *      be looked for.
             */
            public SceneNode getTarget(SceneNode context)
            {
                if (string.IsNullOrWhiteSpace(target))
                {
                    return null;
                }
                else if (target == "this")
                {
                    return context;
                }
                else if (target[0] == '$')
                {
                    return context.getOwner().getNodeVar(target.Substring(1));
                }
                else
                {
                    HashSet<SceneNode> nodes = context.getOwner().getNodes(target);
                    return (nodes != null && nodes.Count > 0 ? nodes.First() : null);
                }
            }
        }
    }
}
