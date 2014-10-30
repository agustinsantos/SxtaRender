using log4net;
using Sxta.Core;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.Render.Scenegraph
{
    /**
     * An AbstractTask to set a program.
     * @ingroup scenegraph
     */
    public class SetProgramTask : AbstractTask, ISwappable<SetProgramTask>
    {

        /**
         * Creates a SetProgramTask.
         *
         * @param modules the modules of the program to be set. Each module is
         *      specified by a "node.module" qualified name. The first part
         *      specifies the scene node that contains the module. The second part
         *      specifies the name of the module in this node.
         * @param setUniforms true to set the uniforms of the program, using the
         *      values defined in the scene node from which this task is called.
         */
        public SetProgramTask(List<QualifiedName> modules, bool setUniforms)
            : base("SetProgramTask")
        {
            init(modules, setUniforms);
        }


        /**
         * Deletes this SetProgramTask.
         */
        // ~SetProgramTask() {}

        public override Task getTask(Object context)
        {
            string name = "";
            SceneNode n = ((Method)context).getOwner();
            SceneManager m = n.getOwner();
            Program p;
            try
            {
                for (int i = 0; i < modules.Count; ++i)
                {
                    SceneNode target = modules[i].getTarget(n);
                    if (target == null)
                    {
                        name = name + modules[i].name + ";";
                    }
                    else
                    {
                        Module s = target.getModule(modules[i].name);
                        name = name + s.Name + ";";
                    }
                }
                // TODO sort name components!!!
                p = m.getResourceManager().loadResource(name).get() as Program;
                p.Name = name;
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("SetProgram: cannot find program " + ex);
                }
                throw ex;
            }
             
            return new Impl(p, setUniforms ? n : null);
        }



        /**
         * Creates an uninitialized SetProgramTask.
         */
        public SetProgramTask()
            : base("SetProgramTask")
        { }

        /**
         * Initializes this SetProgramTask.
         *
         * @param modules the modules of the program to be set. Each module is
         *      specified by a "node.module" qualified name. The first part
         *      specifies the scene node that contains the module. The second part
         *      specifies the name of the module in this node.
         * @param setUniforms true to set the uniforms of the program, using the
         *      values defined in the scene node from which this task is called.
         */
        public void init(List<QualifiedName> modules, bool setUniforms)
        {
            this.modules = modules;
            this.setUniforms = setUniforms;
        }


        /**
         * Swaps this SetProgramTask with the given one.
         *
         * @param t a SetProgramTask.
         */
        public void swap(SetProgramTask t)
        {
            Std.Swap(ref modules, ref t.modules);
        }



        /**
         * The modules of the program to be set. Each module is specified by a
         * "node.module" qualified name. The first part specifies the scene node
         * that contains the module. The second part specifies the name of the
         * module in this node.
         */
        private List<QualifiedName> modules;

        /**
         * True to set the uniforms of the program, using the values defined
         * in the scene node from which this task is called.
         */
        private bool setUniforms;

        /**
         * A ork::Task to set a program.
         */
        private class Impl : Task
        {

            /**
             * The program to be set.
             */
            public Program p;

            /**
             * The scene node whose uniforms must be in #p.
             */
            public SceneNode n;

            /**
             * Creates a new SetProgramTask:Impl.
             *
             * @param p the program to be set.
             * @param n the scene node whose uniforms must be in #p.
             */
            public Impl(Program p, SceneNode n)
                : base("SetProgram", true, 0)
            {

                this.p = p;
                this.n = n;
            }

            /**
             * Deletes this SetProgramTask::Impl.
             */
            //~Impl();

            public override bool run()
            {
                if (p != null)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("SetProgram " + p.Name);
                    }
                    if (n != null)
                    {
                        foreach (Value v in n.getValues().Values)
                        {
                            string name = v.getName();
                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                Uniform u = p.getUniform(name);
                                if (u != null)
                                {
                                    u.setValue(v);
                                }
                            }
                        }
                    }
                    SceneManager.setCurrentProgram(p);
                }
                return true;
            }
            private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
