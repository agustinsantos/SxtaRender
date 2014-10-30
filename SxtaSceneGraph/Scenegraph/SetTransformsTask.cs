using log4net;
using Sxta.Core;
using Sxta.Math;
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
     * An AbstractTask to set transformation matrices in programs.
     * @ingroup scenegraph
     */
    public class SetTransformsTask : AbstractTask, ISwappable<SetTransformsTask>
    {

        /**
         * Creates a new SetTransformsTask.
         *
         * @param screen the "screen" node to be used for transformation involving
         *     the "screen" space. An empty name means the "real" screen space of
         *     the camera node.
         * @param m a "node.module" qualified name. The first part specifies the
         *      scene node that contains the module. The second part specifies the
         *      name of a module in this node. This module is used to find the
         *      uniforms that this task must set.
         * @param t the vec2 uniform that contains time of current frame and time
         *      elapsed since last frame.
         * @param ltow the mat4 uniform to be set to the local to world
         *      transformation.
         * @param ltos the mat4 uniform to be set to the local to screen
         *      transformation.
         * @param ctow the mat4 uniform to be set to the camera to world
         *      transformation.
         * @param ctos the mat4 uniform to be set to the camera to screen
         *      transformation.
         * @param stoc the mat4 uniform to be set to the screen to camera
         *      transformation.
         * @param wtos the mat4 uniform to be set to the world to screen
         *      transformation.
         * @param wp the vec3 uniform to be set to the world coordinates
         *      of the origin of the local frame.
         * @param wd the vec3 uniform to be set to the world coordinates
         *      of the unit z vector of the local frame.
         */
        public SetTransformsTask(string screen, QualifiedName m,
              string t, string ltow, string ltos,
              string ctow, string ctos, string stoc,
              string wtos, string wp, string wd)
            : base("SetTransformsTask")
        {
            init(screen, m, t, ltow, ltos, ctow, ctos, stoc, wtos, wp, wd);
        }


        /**
         * Deletes this SetTransformsTask.
         */
        //~SetTransformsTask() {}

        public override Task getTask(Object context)
        {
            SceneNode n = ((Method)context).getOwner();
            SceneNode screenNode = null;
            if (ltos == null || wtos == null)
            {
                if (!string.IsNullOrWhiteSpace(screen.target))
                {
                    screenNode = screen.getTarget(n);
                    if (screenNode == null)
                    {
                        if (log.IsErrorEnabled)
                        {
                            log.Error("SetTransforms: cannot find screen node");
                        }
                        throw new ArgumentException();
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(m.target) && module == null)
            {
                module = m.getTarget(n).getModule(m.name);
                if (module == null)
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error("SetTransforms: cannot find " + m.target + "." + m.name + " module");
                    }
                    throw new ArgumentException();
                }
            }
            else if (!string.IsNullOrWhiteSpace(m.name) && module == null)
            {
                module = n.getOwner().getResourceManager().loadResource(m.name).get() as Module;
                if (module == null)
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error("SetTransforms: cannot find " + m.name + " module");
                    }
                    throw new ArgumentException();
                }
            }

            return new Impl(screenNode, n, this);
        }

        /**
         * Creates an uninitialized SetTransformsTask.
         */

        public SetTransformsTask()
            : base("ShowInfoTask")
        {
        }

        /**
         * Initializes this SetTransformsTask.
         *
         * See #SetTransformsTask.
         */
        public void init(string screen, QualifiedName m,
          string t, string ltow, string ltos,
          string ctow, string ctos, string stoc,
          string wtos, string wp, string wd)
        {
            this.screen = new QualifiedName(screen + ".");
            this.m = m;
            this.t = t;
            this.ltow = ltow;
            this.ltos = ltos;
            this.ctow = ctow;
            this.ctos = ctos;
            this.stoc = stoc;
            this.wtos = wtos;
            this.wp = wp;
            this.wd = wd;
            this.time = null;
            this.localToWorld = null;
            this.localToScreen = null;
            this.cameraToWorld = null;
            this.cameraToScreen = null;
            this.screenToCamera = null;
            this.worldToScreen = null;
            this.worldPos = null;
            this.worldDir = null;
        }



        /**
         * Swaps this SetTransformsTask with the given one.
         *
         * @param t a SetTransformsTask.
         */
        public void swap(SetTransformsTask t)
        {
            Std.Swap(ref screen, ref t.screen);
            Std.Swap(ref module, ref t.module);
            Std.Swap(ref m, ref t.m);
            Std.Swap(ref this.t, ref t.t);
            Std.Swap(ref ltow, ref t.ltow);
            Std.Swap(ref ltos, ref t.ltos);
            Std.Swap(ref ctow, ref t.ctow);
            Std.Swap(ref ctos, ref t.ctos);
            Std.Swap(ref stoc, ref t.stoc);
            Std.Swap(ref wp, ref t.wp);
            Std.Swap(ref wd, ref t.wd);

            if (lastProg != null)
            {
                time = this.t == null ? null : lastProg.getUniform2f(this.t);
                localToWorld = ltow == null ? null : lastProg.getUniformMatrix4f(ltow);
                localToScreen = ltos == null ? null : lastProg.getUniformMatrix4f(ltos);
                cameraToWorld = ctow == null ? null : lastProg.getUniformMatrix4f(ctow);
                cameraToScreen = ctos == null ? null : lastProg.getUniformMatrix4f(ctos);
                screenToCamera = stoc == null ? null : lastProg.getUniformMatrix4f(stoc);
                worldToScreen = wtos == null ? null : lastProg.getUniformMatrix4f(wtos);
                worldPos = wp == null ? null : lastProg.getUniform3f(wp);
                worldDir = wd == null ? null : lastProg.getUniform3f(wd);
            }
        }



        /**
         * The "screen" node to be used for transformation involving the "screen"
         * space. An empty name means the "real" screen space of the camera node.
         */
        private QualifiedName screen;

        private QualifiedName m;

        private Module module;

        private Program lastProg;

        private Uniform2f time;

        private UniformMatrix4f localToWorld;

        private UniformMatrix4f localToScreen;

        private UniformMatrix4f cameraToWorld;

        private UniformMatrix4f cameraToScreen;

        private UniformMatrix4f screenToCamera;

        private UniformMatrix4f worldToScreen;

        private Uniform3f worldPos;

        private Uniform3f worldDir;

        private string t;

        private string ltow;

        private string ltos;

        private string ctow;

        private string ctos;

        private string stoc;

        private string wtos;

        private string wp;

        private string wd;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /**
         * An ork.Task to set transformation matrices in programs.
         */
        private class Impl : Task
        {

            /**
             * The scene node corresponding to the "screen" space.
             */
            public SceneNode screenNode;

            /**
             * Creates a new SetTransformsTask.Task.
             *
             * @param screenNode the SceneNode corresponding to the "screen" space.
             * @param context the SceneNode that contains the Method to which
             *      'source' belongs.
             * @param source the SetTransformsTask that created this task.
             */
            public Impl(SceneNode screenNode, SceneNode context, SetTransformsTask source)
                : base("SetTransforms", true, 0)
            {
                this.screenNode = screenNode;
                this.context = context;
                this.source = source;
            }


            /**
             * Deletes this SetTransformsTask.Task
             */
            // ~Impl() {}

            public override bool run()
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("SetTransforms");
                }

                Program prog = null;
                if (source.module != null && source.module.getUsers().Count > 0)
                {
                    prog = source.module.getUsers().First();
                }
                else
                {
                    prog = SceneManager.getCurrentProgram();
                }

                if (prog == null)
                {
                    return true;
                }
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("SetTransforms {0}", prog.Name);
                }

                if (prog != source.lastProg)
                {
                    source.time = string.IsNullOrWhiteSpace(source.t) ? null : prog.getUniform2f(source.t);
                    source.localToWorld = string.IsNullOrWhiteSpace(source.ltow) ? null : prog.getUniformMatrix4f(source.ltow);
                    source.localToScreen = string.IsNullOrWhiteSpace(source.ltos) ? null : prog.getUniformMatrix4f(source.ltos);
                    source.cameraToWorld = string.IsNullOrWhiteSpace(source.ctow) ? null : prog.getUniformMatrix4f(source.ctow);
                    source.cameraToScreen = string.IsNullOrWhiteSpace(source.ctos) ? null : prog.getUniformMatrix4f(source.ctos);
                    source.screenToCamera = string.IsNullOrWhiteSpace(source.stoc) ? null : prog.getUniformMatrix4f(source.stoc);
                    source.worldToScreen = string.IsNullOrWhiteSpace(source.wtos) ? null : prog.getUniformMatrix4f(source.wtos);
                    source.worldPos = string.IsNullOrWhiteSpace(source.wp) ? null : prog.getUniform3f(source.wp);
                    source.worldDir = string.IsNullOrWhiteSpace(source.wd) ? null : prog.getUniform3f(source.wd);
                    source.lastProg = prog;
                }

                if (source.time != null)
                {
                    source.time.set(new Vector2f((float)context.getOwner().getTime(), (float)context.getOwner().getElapsedTime()));
                }

                if (source.localToWorld != null)
                {
                    log.DebugFormat("Setting localToWorld matrix {0} for context {1}", context.getLocalToWorld(), context.getFlags().First());
                    source.localToWorld.setMatrix((Matrix4f)context.getLocalToWorld());
                }

                if (source.localToScreen != null)
                {
                    if (string.IsNullOrWhiteSpace(source.screen.target))
                    {
                        log.DebugFormat("Setting localToScreen matrix {0} for context {1}", context.getLocalToScreen(), context.getFlags().First());
                        source.localToScreen.setMatrix((Matrix4f)context.getLocalToScreen());
                    }
                    else
                    {
                        Matrix4d ltow = context.getLocalToWorld();
                        Matrix4d wtos = screenNode.getWorldToLocal();
                        Matrix4f ltos = (Matrix4f)(wtos * ltow);
                        source.localToScreen.setMatrix(ltos);
                    }
                }

                if (source.cameraToWorld != null)
                {
                    Matrix4d ctow = context.getOwner().getCameraNode().getLocalToWorld();
                    source.cameraToWorld.setMatrix((Matrix4f)ctow);
                }

                if (source.cameraToScreen != null)
                {
                    Matrix4d ctos = context.getOwner().getCameraToScreen();
                    source.cameraToScreen.setMatrix((Matrix4f)ctos);
                }

                if (source.screenToCamera != null)
                {
                    Matrix4d ctos = context.getOwner().getCameraToScreen();
                    ctos.Invert();
                    source.screenToCamera.setMatrix((Matrix4f)ctos);
                }

                if (source.worldToScreen != null)
                {
                    if (string.IsNullOrWhiteSpace(source.screen.target))
                    {
                        source.worldToScreen.setMatrix((Matrix4f)context.getOwner().getWorldToScreen());
                    }
                    else
                    {
                        Matrix4f wtos = (Matrix4f)screenNode.getWorldToLocal();
                        source.worldToScreen.setMatrix(wtos);
                    }
                }

                if (source.worldPos != null)
                {
                    source.worldPos.set((Vector3f)context.getWorldPos());
                    log.DebugFormat("Setting worldPos vector {0} for context {1}", context.getWorldPos(), context.getFlags().First());
                }

                if (source.worldDir != null)
                {
                    Vector4d d = context.getLocalToWorld() * Vector4d.UnitZ;
                    source.worldDir.set(new Vector3f((float)-d.X, (float)-d.Y, (float)-d.Z));
                    log.DebugFormat("Setting worldDir vector {0} for context {1}", d, context.getFlags().First());
                }

                return true;
            }

            /**
            * The SceneNode that contains the Method to which #source belongs.
            */
            private SceneNode context;

            /**
             * The SetTransformsTask that created this task.
             */
            private SetTransformsTask source;

            private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        }

    }
}
