using Sxta.Core;
using Sxta.Math;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.Render.Scenegraph
{
    /**
     * An AbstractTask to set the state of a framebuffer.
     * @ingroup scenegraph
     */
    public class SetStateTask : AbstractTask, ISwappable<SetStateTask>
    {

        /**
         * A 'subtask' of this task.
         */
        public interface Runnable
        {
            /**
             * Deletes this 'subtask'.
             */
            //~Runnable();

            /**
             * Runs this 'subtask'.
             *
             * @param fb the framebuffer to use for this 'subtask'.
             */
            void run(FrameBuffer fb);
        }

        /**
         * Creates an empty SetStateTask.
         */
        public SetStateTask()
            : base("SetStateTask")
        {
        }


        /**
         * Deletes this SetStateTask.
         */
        ~SetStateTask()
        {
            for (int i = 0; i < runnables.Count; ++i)
            {
                //TODO delete runnables[i];
            }
            runnables.Clear();
        }

        /**
         * Adds a 'subtask' to this task.
         *
         * @param r a 'subtask' to be added to this task.
         */
        public void addRunnable(Runnable r)
        {
            runnables.Add(r);
        }

        /**
         * Sets the viewport (up, down, left and right planes).
         *
         * @param viewport the new viewport.
         */
        public void setViewport(Vector4i viewport)
        {
            addRunnable(new SetViewport(viewport));
        }

        /**
         * Sets the depth range (near and far planes).
         *
         * @param n near plane.
         * @param f far plane.
         */
        public void setDepthRange(float n, float f)
        {
            addRunnable(new SetDepthRange(n, f));
        }

        /**
         * Sets the clipping bits, used to determine which planes will be used for clipping.
         */
        public void setClipDistances(int d)
        {
            addRunnable(new SetClipDistances(d));
        }

        /**
         * Sets the color used to clear the current draw buffer.
         */
        public void setClearColor(Vector4f c)
        {
            addRunnable(new SetClearColor(c));
        }

        /**
         * Sets the depth used to clear the current depth buffer.
         */
        public void setClearDepth(float clearDepth)
        {
            addRunnable(new SetClearDepth(clearDepth));
        }

        /**
         * Sets the stencil used to clear the current stencil buffer.
         */
        public void setClearStencil(int clearStencil)
        {
            addRunnable(new SetClearStencil(clearStencil));
        }

        /**
         * Sets the point primitive's size.
         */
        public void setPointSize(float pointSize)
        {
            addRunnable(new SetPointSize(pointSize));
        }

        /**
         * Sets the maximum size of a point.
         */
        public void setPointFadeThresholdSize(float tSize)
        {
            addRunnable(new SetPointFadeThresholdSize(tSize));
        }

        /**
         * Sets the origin corner of a point.
         */
        public void setPointLowerLeftOrigin(bool pointLowerLeftOrigin)
        {
            addRunnable(new SetPointLowerLeftOrigin(pointLowerLeftOrigin));
        }

        /**
         * Sets the line primitive's width.
         */
        public void setLineWidth(float lineWidth)
        {
            addRunnable(new SetLineWidth(lineWidth));
        }

        /**
         * Enables or disables the AA on line drawing.
         */
        public void setLineSmooth(bool lineSmooth)
        {
            addRunnable(new SetLineSmooth(lineSmooth));
        }

        /**
         * Determines the orientation of front faces.
         *
         * @param frontFaceCW if true, clockwise faces will be front faces.
         */
        public void setFrontFaceCW(bool frontFaceCW)
        {
            addRunnable(new SetFrontFaceCW(frontFaceCW));
        }

        /**
         * Sets the polygon mode for front and back faces.
         *
         * @param polygonFront drawing mode for front faces.
         * @param polygonBack drawing mode for back faces.
         */
        public void setPolygonMode(PolygonMode polygonFront, PolygonMode polygonBack)
        {
            addRunnable(new SetPolygonMode(polygonFront, polygonBack));
        }

        /**
         * Enables or disables AA on polygon drawing.
         */
        public void setPolygonSmooth(bool polygonSmooth)
        {
            addRunnable(new SetPolygonSmooth(polygonSmooth));
        }

        /**
         * Sets the factor and units when using offsets on primitives.
         */
        public void setPolygonOffset(float factor, float units)
        {
            addRunnable(new SetPolygonOffset(factor, units));
        }

        /**
         * Determines which primitives should be offset.
         */
        public void setPolygonOffset(bool pointOffset, bool lineOffset, bool polygonOffset)
        {
            addRunnable(new SetPolygonOffsets(pointOffset, lineOffset, polygonOffset));
        }

        /**
         * Enables or disables multisampling.
         */
        public void setMultisample(bool multiSample)
        {
            addRunnable(new SetMultisample(multiSample));
        }

        /**
         * Sets sampling filter options.
         */
        public void setSampleAlpha(bool sampleAlphaToCoverage, bool sampleAlphaToOne)
        {
            addRunnable(new SetSampleAlpha(sampleAlphaToCoverage, sampleAlphaToOne));
        }

        /**
         * Sets sampling coverage.
         */
        public void setSampleCoverage(float sampleCoverage)
        {
            addRunnable(new SetSampleCoverage(sampleCoverage));
        }

        /**
         * Sets sampling mask.
         */
        public void setSampleMask(uint sampleMask)
        {
            addRunnable(new SetSampleMask(sampleMask));
        }

        /**
         * Enables or disables AA on multisampling.
         */
        public void setSampleShading(bool sampleShading, float minSamples)
        {
            addRunnable(new SetSampleShading(sampleShading, minSamples));
        }

        /**
         * Adds an occlusion query.
         */
        public void setOcclusionTest(Query occlusionQuery, QueryMode occlusionMode)
        {
            addRunnable(new SetOcclusionTest(occlusionQuery, occlusionMode));
        }

        /**
         * Enables or disables scissor test.
         */
        public void setScissorTest(bool enableScissor)
        {
            addRunnable(new SetScissorTest(enableScissor));
        }

        /**
         * Enables or disables scissor test.
         */
        public void setScissorTest(bool enableScissor, Vector4i scissor)
        {
            addRunnable(new SetScissorTestValue(enableScissor, scissor));
        }

        /**
         * Enables or disables stencil test.
         *
         * @param enableStencil true to enable the stencil test.
         * @param ff the front face function.
         * @param fref the front face reference value.
         * @param fmask the front face stencil mask.
         * @param ffail the stencil operation used when failing stencil test on front faces.
         * @param fdpfail the stencil operation used when passing stencil test but failing depth test on front faces.
         * @param fdppass the stencil operation used when passing both stencil and depth tests on front faces.
         * @param bf the back face function.
         * @param bref the back face reference value.
         * @param bmask the back face stencil mask.
         * @param bfail the stencil operation used when failing stencil test on back faces.
         * @param bdpfail the stencil operation used when passing stencil test but failing depth test on back faces.
         * @param bdppass the stencil operation used when passing both stencil and depth tests on back faces.
         */
        public void setStencilTest(bool enableStencil,
            Function ff = (Function)(-1), int fref = -1, uint fmask = 0, StencilOperation ffail = (StencilOperation)(-1), StencilOperation fdpfail = (StencilOperation)(-1), StencilOperation fdppass = (StencilOperation)(-1),
            Function bf = (Function)(-1), int bref = -1, uint bmask = 0, StencilOperation bfail = (StencilOperation)(-1), StencilOperation bdpfail = (StencilOperation)(-1), StencilOperation bdppass = (StencilOperation)(-1))
        {
            addRunnable(new SetStencilTest(enableStencil,
                ff, fref, fmask, ffail, fdpfail, fdppass,
                bf, bref, bmask, bfail, bdpfail, bdppass));
        }

        /**
         * Enables or disables depth test.
         */
        public void setDepthTest(bool enableDepth, Function depth = (Function) (-1))
        {
            addRunnable(new SetDepthTest(enableDepth, depth));
        }

        /**
         * Enables or disables blending.
         *
         * @param buffer the buffer whose blending options must be changed.
         * @param enableBlend true to enable blending.
         * @param rgb the color blending equation.
         * @param srgb the source color blending argument.
         * @param drgb the destination color blending argument.
         * @param alpha the alpha blending equation.
         * @param salpha the source alpha blending argument.
         * @param dalpha the destination alpha blending argument.
         */
        public void setBlend(BufferId buffer, bool enableBlend, BlendEquation rgb = (BlendEquation)(-1), BlendArgument src = (BlendArgument)(-1), BlendArgument dst = (BlendArgument)(-1),
            BlendEquation eAlpha = (BlendEquation)(-1), BlendArgument srcAlpha = (BlendArgument)(-1), BlendArgument dstAlpha = (BlendArgument)(-1))
        {
            addRunnable(new SetBlend(buffer, enableBlend, rgb, src, dst, eAlpha, srcAlpha, dstAlpha));
        }


        /**
         * Sets Blend color parameter.
         */
        public void setBlendColor(Vector4f color)
        {
            addRunnable(new SetBlendColor(color));
        }

        /**
         * Enables or disables dithering.
         */
        public void setDither(bool enableDither)
        {
            addRunnable(new SetDither(enableDither));
        }

        /**
         * Enables or disables logical operation.
         */
        public void setLogicOp(bool enableLogic, LogicOperation logicOp = (LogicOperation)(-1))
        {
            addRunnable(new SetLogicOp(enableLogic, logicOp));
        }

        /**
         * Sets color buffer's writing mask.
         */
        public void setColorMask(BufferId buffer, bool r, bool g, bool b, bool a)
        {
            addRunnable(new SetColorMask(buffer, r, g, b, a));
        }

        /**
         * Sets depth buffer's writing mask.
         */
        public void setDepthMask(bool d)
        {
            addRunnable(new SetDepthMask(d));
        }

        /**
         * Sets stencil buffer's writing mask.
         */
        public void setStencilMask(uint frontMask, uint backMask)
        {
            addRunnable(new SetStencilMask(frontMask, backMask));
        }

        /**
         * Sets the color, stencil and depth used to clear the current draw buffer.
         */
        public void setClearState(bool color, bool stencil, bool depth)
        {
            addRunnable(new SetClearState(color, stencil, depth));
        }

        /**
         * Sets the read and draw buffers.
         */
        public void setBuffers(BufferId rb, BufferId db)
        {
            addRunnable(new SetBuffers(rb, db));
        }

        public override Task getTask(Object context)
        {
            return new Impl(this);
        }


        /**
         * Swaps this SetStateTask with the given one.
         *
         * @param t a SetStateTask.
         */
        public void swap(SetStateTask t)
        {
            Std.Swap(ref runnables, ref t.runnables);
        }


        /**
         * The 'subtasks' to do in this task.
         */
        private List<Runnable> runnables = new List<Runnable>();

        /**
         * Runs each 'subtask' in #runnables.
         */
        private void run()
        {
            FrameBuffer fb = SceneManager.getCurrentFrameBuffer();
            for (int i = 0; i < runnables.Count; ++i)
            {
                runnables[i].run(fb);
            }
        }

        /**
         * A ork.Task to set the state of a framebuffer.
         */
        private class Impl : Task
        {

            /**
             * The SetStateTask that created this task.
             */
            public SetStateTask source;

            /**
             * Creates a new SetStateTask.Impl.
             *
             * @param source the SetStateTask that created this task.
             */
            public Impl(SetStateTask source)
                : base("SetState", true, 0)
            {
                this.source = source;
            }

            /**
             * Deletes this SetStateTask.Impl.
             */
            //~Impl()
            //{
            //}

            public override bool run()
            {
                source.run();
                return true;
            }
        }

        

        internal sealed class SetViewport : SetStateTask.Runnable
        {

            public SetViewport(Vector4i vp)
            {
                this.viewport = vp;
            }

            public void run(FrameBuffer fb)
            {
                fb.setViewport(viewport);
            }


            private Vector4i viewport;
        }


        internal sealed class SetDepthRange : SetStateTask.Runnable
        {
            public SetDepthRange(float n, float f)
            {
                this.near = n;
                this.far = f;
            }

            public void run(FrameBuffer fb)
            {
                fb.setDepthRange(near, far);
            }


            private float near;

            private float far;
        }

        internal sealed class SetClipDistances : SetStateTask.Runnable
        {

            public SetClipDistances(int d)
            {
                this.clipDistances = d;
            }

            public void run(FrameBuffer fb)
            {
                fb.setClipDistances(clipDistances);
            }


            private int clipDistances;
        }

        internal sealed class SetClearColor : SetStateTask.Runnable
        {

            public SetClearColor(Vector4f c)
            {
                this.color = c;
            }

            public void run(FrameBuffer fb)
            {
                fb.setClearColor(color);
            }


            private Vector4f color;
        }

        internal sealed class SetClearDepth : SetStateTask.Runnable
        {

            public SetClearDepth(float clearDepth)
            {
                depth = clearDepth;
            }

            public void run(FrameBuffer fb)
            {
                fb.setClearDepth(depth);
            }


            private float depth;
        }

        internal sealed class SetClearStencil : SetStateTask.Runnable
        {

            public SetClearStencil(int clearStencil)
            {
                stencil = clearStencil;
            }

            public void run(FrameBuffer fb)
            {
                fb.setClearStencil(stencil);
            }


            private int stencil;
        }

        internal sealed class SetPointSize : SetStateTask.Runnable
        {

            public SetPointSize(float pointSize)
            {
                size = pointSize;
            }
            public void run(FrameBuffer fb)
            {
                fb.setPointSize(size);
            }


            private float size;
        }

        internal sealed class SetPointFadeThresholdSize : SetStateTask.Runnable
        {

            public SetPointFadeThresholdSize(float tSize)
            {
                size = tSize;
            }

            public void run(FrameBuffer fb)
            {
                fb.setPointFadeThresholdSize(size);
            }


            private float size;
        }

        internal sealed class SetPointLowerLeftOrigin : SetStateTask.Runnable
        {

            public SetPointLowerLeftOrigin(bool pointLowerLeftOrigin)
            {
                origin = pointLowerLeftOrigin;
            }

            public void run(FrameBuffer fb)
            {
                fb.setPointLowerLeftOrigin(origin);
            }


            private bool origin;
        }

        internal sealed class SetLineWidth : SetStateTask.Runnable
        {

            public SetLineWidth(float lineWidth)
            {
                width = lineWidth;
            }

            public void run(FrameBuffer fb)
            {
                fb.setLineWidth(width);
            }


            private float width;
        }

        internal sealed class SetLineSmooth : SetStateTask.Runnable
        {

            public SetLineSmooth(bool lineSmooth)
            {
                smooth = lineSmooth;
            }

            public void run(FrameBuffer fb)
            {
                fb.setLineSmooth(smooth);
            }


            private bool smooth;
        }

        internal sealed class SetFrontFaceCW : SetStateTask.Runnable
        {

            public SetFrontFaceCW(bool frontFaceCW)
            {
                this.frontFaceCW = frontFaceCW;
            }

            public void run(FrameBuffer fb)
            {
                fb.setFrontFaceCW(frontFaceCW);
            }


            private bool frontFaceCW;
        }

        internal sealed class SetPolygonMode : SetStateTask.Runnable
        {

            public SetPolygonMode(PolygonMode polygonFront, PolygonMode polygonBack)
            {
                this.polygonFront = polygonFront;
                this.polygonBack = polygonBack;
            }

            public void run(FrameBuffer fb)
            {
                fb.setPolygonMode(polygonFront, polygonBack);
            }


            private PolygonMode polygonFront;
            private PolygonMode polygonBack;
        }

        internal sealed class SetPolygonSmooth : SetStateTask.Runnable
        {

            public SetPolygonSmooth(bool polygonSmooth)
            {
                this.polygonSmooth = polygonSmooth;
            }

            public void run(FrameBuffer fb)
            {
                fb.setPolygonSmooth(polygonSmooth);
            }


            private bool polygonSmooth;
        }

        internal sealed class SetPolygonOffset : SetStateTask.Runnable
        {
            public SetPolygonOffset(float factor, float units)
            {
                this.factor = factor;
                this.units = units;
            }

            public void run(FrameBuffer fb)
            {
                fb.setPolygonOffset(factor, units);
            }


            private float factor;
            private float units;
        }

        internal sealed class SetPolygonOffsets : SetStateTask.Runnable
        {
            public SetPolygonOffsets(bool pointOffset, bool lineOffset, bool polygonOffset)
            {
                this.pointOffset = pointOffset;
                this.lineOffset = lineOffset;
                this.polygonOffset = polygonOffset;
            }

            public void run(FrameBuffer fb)
            {
                fb.setPolygonOffset(pointOffset, lineOffset, polygonOffset);
            }


            private bool pointOffset;
            private bool lineOffset;
            private bool polygonOffset;
        }

        internal sealed class SetMultisample : SetStateTask.Runnable
        {
            public SetMultisample(bool multiSample)
            {
                this.multiSample = multiSample;
            }

            public void run(FrameBuffer fb)
            {
                fb.setMultisample(multiSample);
            }


            private bool multiSample;
        }

        internal sealed class SetSampleAlpha : SetStateTask.Runnable
        {
            public SetSampleAlpha(bool sampleAlphaToCoverage, bool sampleAlphaToOne)
            {
                this.sampleAlphaToCoverage = sampleAlphaToCoverage;
                this.sampleAlphaToOne = sampleAlphaToOne;
            }

            public void run(FrameBuffer fb)
            {
                fb.setSampleAlpha(sampleAlphaToCoverage, sampleAlphaToOne);
            }


            private bool sampleAlphaToCoverage;
            private bool sampleAlphaToOne;
        }

        internal sealed class SetSampleCoverage : SetStateTask.Runnable
        {
            public SetSampleCoverage(float sampleCoverage)
            {
                this.sampleCoverage = sampleCoverage;
            }

            public void run(FrameBuffer fb)
            {
                fb.setSampleCoverage(sampleCoverage);
            }


            private float sampleCoverage;
        }

        internal sealed class SetSampleMask : SetStateTask.Runnable
        {
            public SetSampleMask(uint sampleMask)
            {
                this.sampleMask = sampleMask;
            }

            public void run(FrameBuffer fb)
            {
                fb.setSampleMask(sampleMask);
            }


            private uint sampleMask;
        }

        internal sealed class SetSampleShading : SetStateTask.Runnable
        {
            public SetSampleShading(bool sampleShading, float minSamples)
            {
                this.sampleShading = sampleShading;
                this.minSamples = minSamples;
            }

            public void run(FrameBuffer fb)
            {
                fb.setSampleShading(sampleShading, minSamples);
            }


            private bool sampleShading;
            private float minSamples;
        }

        internal sealed class SetOcclusionTest : SetStateTask.Runnable
        {
            public SetOcclusionTest(Query occlusionQuery, QueryMode occlusionMode)
            {
                this.occlusionQuery = occlusionQuery;
                this.occlusionMode = occlusionMode;
            }

            public void run(FrameBuffer fb)
            {
                fb.setOcclusionTest(occlusionQuery, occlusionMode);
            }


            private Query occlusionQuery;
            private QueryMode occlusionMode;
        }

        internal sealed class SetScissorTest : SetStateTask.Runnable
        {
            public SetScissorTest(bool enableScissor)
            {
                this.enableScissor = enableScissor;
            }

            public void run(FrameBuffer fb)
            {
                fb.setScissorTest(enableScissor);
            }


            private bool enableScissor;
        }

        internal sealed class SetScissorTestValue : SetStateTask.Runnable
        {
            public SetScissorTestValue(bool enableScissor, Vector4i scissor)
            {
                this.enableScissor = enableScissor;
                this.scissor = scissor;
            }

            public void run(FrameBuffer fb)
            {
                fb.setScissorTest(enableScissor, scissor);
            }


            private bool enableScissor;
            private Vector4i scissor;
        }

        internal sealed class SetStencilTest : SetStateTask.Runnable
        {
            public SetStencilTest(bool enableStencil,
           Function ff = (Function)(-1), int fref = -1, uint fmask = 0, StencilOperation ffail = (StencilOperation)(-1), StencilOperation fdpfail = (StencilOperation)(-1), StencilOperation fdppass = (StencilOperation)(-1),
           Function bf = (Function)(-1), int bref = -1, uint bmask = 0, StencilOperation bfail = (StencilOperation)(-1), StencilOperation bdpfail = (StencilOperation)(-1), StencilOperation bdppass = (StencilOperation)(-1))
            {
                this.enableStencil = enableStencil;
                this.ff = ff;
                this.fref = fref;
                this.fmask = fmask;
                this.ffail = ffail;
                this.fdpfail = fdpfail;
                this.fdppass = fdppass;
                this.bf = bf;
                this.bref = bref;
                this.bmask = bmask;
                this.bfail = bfail;
                this.bdpfail = bdpfail;
                this.bdppass = bdppass;
                // checks that correct parameters are provided if a function is set
                Debug.Assert(ff == (Function)(-1) || (fref != -1 && ffail != (StencilOperation)(-1) && fdpfail != (StencilOperation)(-1) && fdppass != (StencilOperation)(-1)));
                Debug.Assert(bf == (Function)(-1) || (bref != -1 && bfail != (StencilOperation)(-1) && bdpfail != (StencilOperation)(-1) && bdppass != (StencilOperation)(-1)));
            }

            public void run(FrameBuffer fb)
            {
                if (ff == (Function)(-1) && bf == (Function)(-1))
                {
                    fb.setStencilTest(enableStencil);
                }
                else if (bf == (Function)(-1))
                {
                    fb.setStencilTest(enableStencil, ff, fref, fmask, ffail, fdpfail, fdppass);
                }
                else
                {
                    fb.setStencilTest(enableStencil, ff, fref, fmask, ffail, fdpfail, fdppass, bf, bref, bmask, bfail, bdpfail, bdppass);
                }
            }


            private bool enableStencil;

            private Function ff;

            private int fref;

            private uint fmask;

            private StencilOperation ffail;

            private StencilOperation fdpfail;

            private StencilOperation fdppass;

            private Function bf;

            private int bref;

            private uint bmask;

            private StencilOperation bfail;

            private StencilOperation bdpfail;

            private StencilOperation bdppass;
        }

        internal sealed class SetDepthTest : SetStateTask.Runnable
        {
            public SetDepthTest(bool enableDepth, Function depth = (Function) (-1))
            {
                this.enableDepth = enableDepth;
                this.depth = depth;
            }

            public void run(FrameBuffer fb)
            {
                if (depth == (Function)(-1))
                {
                    fb.setDepthTest(enableDepth);
                }
                else
                {
                    fb.setDepthTest(enableDepth, depth);
                }
            }


            private bool enableDepth;

            private Function depth;
        }

        internal sealed class SetBlend : SetStateTask.Runnable
        {
            public SetBlend(BufferId buffer, bool enableBlend, BlendEquation e = (BlendEquation)(-1), BlendArgument src = (BlendArgument)(-1), BlendArgument dst = (BlendArgument)(-1),
                BlendEquation eAlpha = (BlendEquation)(-1), BlendArgument srcAlpha = (BlendArgument)(-1), BlendArgument dstAlpha = (BlendArgument)(-1))
            {
                this.buffer = buffer;
                this.enableBlend = enableBlend;
                this.rgb = e;
                this.alpha = eAlpha;
                this.srgb = src;
                this.drgb = dst;
                this.salpha = srcAlpha;
                this.dalpha = dstAlpha;
                // checks that src & dst arguments are set when the equations are set.
                Debug.Assert(e == (BlendEquation)(-1) || (src != (BlendArgument)(-1) && dst != (BlendArgument)(-1)));
                Debug.Assert(eAlpha == (BlendEquation)(-1) || (srcAlpha != (BlendArgument)(-1) && dstAlpha != (BlendArgument)(-1)));
            }

            public void run(FrameBuffer fb)
            {
                if (rgb == (BlendEquation)(-1) && buffer == (BufferId)(-1))
                {
                    fb.setBlend(enableBlend);
                }
                else if (rgb == (BlendEquation)(-1))
                {
                    fb.setBlend(buffer, enableBlend);
                }
                else if (alpha == (BlendEquation)(-1) && buffer == (BufferId)(-1))
                {
                    fb.setBlend(enableBlend, rgb, srgb, drgb);
                }
                else if (alpha == (BlendEquation)(-1))
                {
                    fb.setBlend(buffer, enableBlend, rgb, srgb, drgb);
                }
                else if (buffer == (BufferId)(-1))
                {
                    fb.setBlend(enableBlend, rgb, srgb, drgb, alpha, salpha, dalpha);
                }
                else
                {
                    fb.setBlend(buffer, enableBlend, rgb, srgb, drgb, alpha, salpha, dalpha);
                }
            }


            private BufferId buffer;

            private bool enableBlend;

            private BlendEquation rgb;

            private BlendEquation alpha;

            private BlendArgument srgb;

            private BlendArgument drgb;

            private BlendArgument salpha;

            private BlendArgument dalpha;
        }

        internal sealed class SetBlendColor : SetStateTask.Runnable
        {
            public SetBlendColor(Vector4f color)
            {
                this.color = color;
            }

            public void run(FrameBuffer fb)
            {
                fb.setBlendColor(color);
            }

            private Vector4f color;
        }

        internal sealed class SetDither : SetStateTask.Runnable
        {
            public SetDither(bool enableDither)
            {
                this.enableDither = enableDither;
            }

            public void run(FrameBuffer fb)
            {
                fb.setDither(enableDither);
            }


            private bool enableDither;
        }

        internal sealed class SetLogicOp : SetStateTask.Runnable
        {
            public SetLogicOp(bool enableLogic, LogicOperation logicOp = (LogicOperation)(-1))
            {
                this.enableLogic = enableLogic;
                this.logicOp = logicOp;
            }

            public void run(FrameBuffer fb)
            {
                if (logicOp == (LogicOperation)(-1))
                {
                    fb.setLogicOp(enableLogic);
                }
                else
                {
                    fb.setLogicOp(enableLogic, logicOp);
                }
            }


            private bool enableLogic;

            private LogicOperation logicOp;
        }

        internal sealed class SetColorMask : SetStateTask.Runnable
        {
            public SetColorMask(BufferId buffer, bool r, bool g, bool b, bool a)
            {
                this.buffer = buffer;
                this.r = r;
                this.g = g;
                this.b = b;
                this.a = a;
            }

            public void run(FrameBuffer fb)
            {
                if (buffer != (BufferId)(-1))
                {
                    fb.setColorMask(buffer, r, g, b, a);
                }
                else
                {
                    fb.setColorMask(r, g, b, a);
                }
            }


            private BufferId buffer;

            private bool r;

            private bool g;

            private bool b;

            private bool a;
        }

        internal sealed class SetDepthMask : SetStateTask.Runnable
        {
            public SetDepthMask(bool d)
            {
                this.d = d;
            }

            public void run(FrameBuffer fb)
            {
                fb.setDepthMask(d);
            }


            private bool d;
        }

        internal sealed class SetStencilMask : SetStateTask.Runnable
        {
            public SetStencilMask(uint frontMask, uint backMask)
            {
                this.frontMask = frontMask;
                this.backMask = backMask;
            }

            public void run(FrameBuffer fb)
            {
                fb.setStencilMask(frontMask, backMask);
            }


            private uint frontMask;

            private uint backMask;
        }

        internal sealed class SetClearState : SetStateTask.Runnable
        {
            public SetClearState(bool color, bool stencil, bool depth)
            {
                this.color = color;
                this.stencil = stencil;
                this.depth = depth;
            }

            public void run(FrameBuffer fb)
            {
                fb.clear(color, stencil, depth);
            }

            private bool color;

            private bool stencil;

            private bool depth;
        }

        public sealed class SetBuffers : SetStateTask.Runnable
        {
            public SetBuffers(BufferId rb, BufferId db)
            {
                this.rb = rb;
                this.db = db;
            }

            public void run(FrameBuffer fb)
            {
                if (rb != (BufferId)(-1))
                {
                    fb.setReadBuffer(rb);
                }
                if (db != (BufferId)(-1))
                {
                    if (db == BufferId.COLOR0 || db == BufferId.COLOR1 || db == BufferId.COLOR2 || db == BufferId.COLOR3 ||
                        db == BufferId.COLOR4 || db == BufferId.COLOR5 || db == BufferId.COLOR6 || db == BufferId.COLOR7)
                    {
                        fb.setDrawBuffer(db);
                    }
                    else
                    {
                        fb.setDrawBuffers(db);
                    }
                }
            }


            private BufferId rb;

            private BufferId db;
        }

    }
}
