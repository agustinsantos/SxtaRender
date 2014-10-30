using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxta.Render.Scenegraph
{
    /// <summary>
    /// An object that can create Task.
    /// </summary>
    public abstract class TaskFactory
    {

        /*
         * Creates a new task factory.
         *
         * @param type the type of this factory.
         */
        public TaskFactory(string type)
        { }

        /*
         * Deletes this task factory.
         */
        //TODO public virtual ~TaskFactory();

        /*
         * Creates a new task.
         *
         * @param context an optional parameter to control the task creation.
         * @return the created task.
         */
        public abstract Task getTask(Object context);
    }
}
