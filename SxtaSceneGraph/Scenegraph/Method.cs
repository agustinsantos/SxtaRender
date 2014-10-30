using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxta.Render.Scenegraph
{
    /// <summary>
    ///  A SceneNode method. A Method defines a behavior of a scene node. It can be a
    /// basic task or a combination of basic tasks using sequences, loops or method
    /// calls. The <i>body</i> of a method is TaskFactory that can be shared between
    /// several methods. This TaskFactory is used to get the tasks to be run to
    /// execute the method, depending on the context (the context passed to
    /// TaskFactory#getTask being the Method itself, from which the owner SceneNode
    /// can be found, and then then owner SceneManager).
    /// </summary>
    public class Method
    {

        /*
         * Creates a new Method using the given basic or compound task.
         *
         * @param body the method definition.
         */
        public Method(TaskFactory body)
        {
            enabled = true;
            taskFactory = body;
        }


        /*
         * Deletes this method.
         */
        //TODO public virtual ~Method();

        /*
         * Returns the SceneNode to which this Method belongs.
         * See SceneNode#getMethod.
         */
        public SceneNode getOwner()
        {
            return owner;
        }

        /*
         * Returns true if this method is enabled. A call to disabled method is
         * skipped. A method is enabled by default.
         */
        public bool isEnabled()
        {
            return enabled;
        }

        /*
         * Enables or disables this method. A call to disabled method is skipped.
         *
         * @param enabled true to enable this method, false to disable it.
         */
        public void setIsEnabled(bool enabled)
        {
            this.enabled = enabled;
        }

        /*
         * Returns the body of this method.
         */
        public TaskFactory getTaskFactory()
        {
            return taskFactory;
        }

        /*
         * Sets the body of this method.
         *
         * @param taskFactory the new method body.
         */
        public void setTaskFactory(TaskFactory taskFactory)
        {
            this.taskFactory = taskFactory;
        }

        /*
         * Returns the Task to be run to execute this method.
         */
        public Task getTask()
        {
            return taskFactory.getTask(this);
        }



        /*
         * The SceneNode to which this Method belongs.
         */
        internal SceneNode owner;

        /*
         * True if this method is enabled.
         */
        private bool enabled;

        /*
         * The body of this method.
         */
        private TaskFactory taskFactory;
    }
}
