using log4net;
using Sxta.Core;
using Sxta.Math;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.Render.Scenegraph
{
    /**
     * An AbstractTask to set the attachments of a framebuffer.
     * @ingroup scenegraph
     */
    public class SetTargetTask : AbstractTask, ISwappable<SetTargetTask>
    {

        /**
         * A framebuffer attachment specification.
         */
        public struct Target
        {
            /**
             * A framebuffer attachment point.
             */
            public BufferId buffer;

            /**
             * The texture to be attached to #buffer. Each texture is specified by a
             * "node.uniform" or "node.module:uniform" qualified name. The first
             * part specifies the scene node that contains the texture. The second
             * part specifies the name of the uniform that refers to the texture
             * (either directly or via a module).
             */
            public QualifiedName texture;

            /**
             * The mipmap level of #texture to be attached.
             */
            public int level;

            /**
             * The layer, z slice or cube face of #texture to be attached.
             */
            public int layer;
        }

        /**
         * Creates a new SetTargetTask.
         *
         * @param targets the framebuffer attachments to be set.
         * @param autoResize true to automatically resize the target textures to
         *      the default framebuffer viewport size.
         */
        public SetTargetTask(List<Target> targets, bool autoResize)
            : base("SetTargetTask")
        {
            init(targets, autoResize);
        }

        /**
         * Deletes this SetTargetTask.
         */
        //~SetTargetTask();

        public override Task getTask(Object context)
        {
            List<Texture> textures = new List<Texture>();
            SceneNode n = ((Method)context).getOwner();
            try
            {
                for (int i = 0; i < targets.Count; ++i)
                {
                    Target target = targets[i];
                    string name = target.texture.name;
                    SceneNode owner = target.texture.getTarget(n);
                    if (owner != null)
                    {
                        //                ptr<Uniform> u = NULL;
                        int index = name.IndexOf(':');
                        Texture t = null;
                        if (index == -1)
                        {
#if TODO
                            t = (ValueSampler)(owner.getValue(name)) ;
 
#endif
                            throw new NotImplementedException();
                        }
                        else
                        {
                            Module module = owner.getModule(name.Substring(0, index));
                            ISet<Program> progs = module.getUsers();
                            t = ( progs.First()).getUniformSampler(name.Substring(index + 1)).get();
                        }
                        textures.Add(t);
                    }
                    else
                    {
                        textures.Add(n.getOwner().getResourceManager().loadResource(name).get() as Texture);
                    }
                    if (textures[i] == null)
                    {
                        throw new Exception();
                    }
                    if (autoResize)
                    {
                        Vector4i viewport = FrameBuffer.getDefault().getViewport();
                        Texture2D t = (Texture2D)textures[i];
                        Debug.Assert(t != null);
                        if (t.getWidth() != viewport.Z || t.getHeight() != viewport.W)
                        {
                            t.setImage(viewport.Z, viewport.W, t.getFormat(), PixelType.FLOAT, new CPUBuffer<byte>(null));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //if (Logger::ERROR_LOGGER != NULL) {
                //    Logger::ERROR_LOGGER->log("SCENEGRAPH", "SetTarget: cannot find attachment textures");
                //}
                throw ex;
            }

            return new Impl(this, textures);
        }


        /**
         * Creates an uninitialized SetTargetTask.
         */
        public SetTargetTask()
            : base("SetTargetTask")
        {
        }

        /**
         * Initializes this SetTargetTask.
         *
         * @param targets the framebuffer attachments to be set.
         * @param autoResize true to automatically resize the target textures to
         *      the default framebuffer viewport size.
         */
        public void init(List<Target> targets, bool autoResize)
        {
            this.targets = targets;
            this.autoResize = autoResize;
        }

        /**
         * Swaps this SetTargetTask with the given one.
         *
         * @param t a SetTargetTask.
         */
        public void swap(SetTargetTask t)
        {
            Std.Swap(ref targets, ref t.targets);
        }


        /**
         * An offscreen framebuffer for use with SetTargetTask.
         */
        private static FrameBuffer TARGET_BUFFER;

        /**
         * The framebuffer attachments to be set.
         */
        private List<Target> targets;

        /**
         * True to automatically resize the target textures to the default
         * framebuffer viewport size.
         */
        private bool autoResize;

        /**
         * Returns an offscreen framebuffer for use with SetTargetTask.
         */
        private static FrameBuffer getTargetBuffer()
        {
            if (TARGET_BUFFER == null)
            {
                TARGET_BUFFER = new FrameBuffer();
                TARGET_BUFFER.setReadBuffer(BufferId.DEFAULT);
                TARGET_BUFFER.setDrawBuffer(BufferId.DEFAULT);
            }
            return TARGET_BUFFER;
        }

        /**
         * An ork::Task to set the attachments of a framebuffer.
         */
        private class Impl : Task
        {

            /**
             * The SetTargetTask that created this task.
             */
            public SetTargetTask source;

            /**
             * The textures to be set to the framebuffer attachment points.
             */
            public List<Texture> textures;

            /**
             * Creates a new SetTargetTask::Impl.
             *
             * @param source the SetTargetTask that created this task.
             * @param textures the textures to be set to the framebuffer attachment
             *      points.
             */
            public Impl(SetTargetTask source, List<Texture> textures)
                : base("SetTarget", true, 0)
            {
                this.source = source;
                this.textures = textures;
            }


            /**
             * Deletes this SetTargetTask::Impl.
             */
            // ~Impl();

            public override bool run()
            {
                if (log.IsDebugEnabled)
                {
                    string os = "SetTarget";
                    for (int i = 0; i < textures.Count; ++i)
                    {
                        BufferId b = source.targets[i].buffer;
                        switch (b)
                        {
                            case BufferId.COLOR0:
                                os += " COLOR0";
                                break;
                            case BufferId.COLOR1:
                                os += " COLOR1";
                                break;
                            case BufferId.COLOR2:
                                os += " COLOR2";
                                break;
                            case BufferId.COLOR3:
                                os += " COLOR3";
                                break;
                            case BufferId.COLOR4:
                                os += " COLOR4";
                                break;
                            case BufferId.COLOR5:
                                os += " COLOR5";
                                break;
                            case BufferId.COLOR6:
                                os += " COLOR6";
                                break;
                            case BufferId.COLOR7:
                                os += " COLOR7";
                                break;
                            case BufferId.STENCIL:
                                os += " STENCIL";
                                break;
                            case BufferId.DEPTH:
                                os += " DEPTH";
                                break;
                        }
                         if (textures[i] != null)
                        {
                            os += " '" + textures[i].Name + "'";
                        }
                      }
                    if (textures.Count == 0)
                    {
                        os += " default framebuffer";
                    }
                    log.Debug(os);
                }

                FrameBuffer fb = getTargetBuffer();
                if (textures.Count == 0)
                {
                    BufferId[] bufs = new BufferId[10]{
                                                        BufferId.COLOR0,
                                                        BufferId.COLOR1,
                                                        BufferId.COLOR2,
                                                        BufferId.COLOR3,
                                                        BufferId.COLOR4,
                                                        BufferId.COLOR5,
                                                        BufferId.COLOR6,
                                                        BufferId.COLOR7,
                                                        BufferId.STENCIL,
                                                       BufferId. DEPTH
                                                    };
                    for (int i = 0; i < 10; ++i)
                    {
                        Texture t = fb.getTextureBuffer(bufs[i]);
                        if (t != null)
                        {
                            t.generateMipMap();
                        }
                    }
                    SceneManager.setCurrentFrameBuffer(FrameBuffer.getDefault());
                    return true;
                }
                int w = 0, h = 0;

                for (int i = 0; i < textures.Count; ++i)
                {
                    Target target = source.targets[i];
                    Texture texture = textures[i];
                    if (texture is Texture2D)
                    {
                        Texture2D t = texture as Texture2D;
                        fb.setTextureBuffer(target.buffer, t, target.level);
                        //a.setColorBuffer(target->buffer, t, target->level);
                        w = t.getWidth();
                        h = t.getHeight();
                    }
                    else if (texture is Texture2DArray)
                    {
                        Texture2DArray t = texture as Texture2DArray;
                        fb.setTextureBuffer(target.buffer, t, target.level, target.layer);
                        //a.setColorBuffer(target->buffer, t, target->level, target->layer);
                        w = t.getWidth();
                        h = t.getHeight();
                    }
                    else if (texture is TextureCube)
                    {
                        TextureCube t = texture as TextureCube;
                        fb.setTextureBuffer(target.buffer, t, target.level, (CubeFace)(target.layer));
                        //a.setColorBuffer(target->buffer, t, target->level, TextureCube::face(target->layer));
                        w = t.getWidth();
                        h = t.getHeight();
                    }
                    else if (texture is Texture3D)
                    {
                        Texture3D t = texture as Texture3D;
                        fb.setTextureBuffer(target.buffer, t, target.level, target.layer);
                        //a.setColorBuffer(target->buffer, t, target->level, target->layer);
                        w = t.getWidth();
                        h = t.getHeight();
                    }
                    else
                    {
                        Texture1D t = texture as Texture1D;
                        fb.setTextureBuffer(target.buffer, t, target.level);
                        //a.setColorBuffer(target->buffer, t, target->level);
                        w = t.getWidth();
                        h = 1;
                    }
                }
                SceneManager.setCurrentFrameBuffer(fb);
                fb.setViewport(new Vector4i(0, 0, w, h));
                return true;
            }

            private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        }
    }

}
