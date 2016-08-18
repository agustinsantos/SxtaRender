using log4net;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Sxta.Core;
using Sxta.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sxta.Render
{
    /// <summary>
    ///  A framebuffer, either the default one or a frame buffer object. Each
    /// framebuffer has its own state, made of attachments and fixed functions
    /// parameters.
    /// </summary>
    public class FrameBuffer : IDisposable
    {

        /// <summary>
        /// The state of a FrameBuffer.
        /// </summary>
        public class Parameters : IDeepCloneable<Parameters>
        {

            /// <summary>
            /// Creates a new framebuffer state with default parameter values.
            /// </summary>
            public Parameters()
            {
                // --------------- parameters related with transform
                transformId = 0;
                viewport = new Vector4i(0, 0, 0, 0);
                depthRange = new Vector2f(0.0f, 1.0f);
                clipDistances = 0;

                // --------------- parameters related with clear
                clearId = 0;
                clearColor = new Vector4f(0.0f, 0.0f, 0.0f, 0.0f);
                clearDepth = 1.0f;
                clearStencil = 0;

                // --------------- parameters related with point
                pointId = 0;
                pointSize = 1.0f;
                pointFadeThresholdSize = 1.0f;
                pointLowerLeftOrigin = false;

                // --------------- parameters related with polygon
                polygonId = 0;
                lineWidth = 1.0f;
                lineSmooth = false;
                frontFaceCW = false;
                polygonFront = PolygonMode.FILL;
                polygonBack = PolygonMode.FILL;
                polygonSmooth = false;
                polygonOffset = new Vector2f(0.0f, 0.0f);
                polygonOffsets = new Vector3b(false, false, false);

                // --------------- parameters related with sample
                multiSampleId = 0;
                multiSample = true;
                sampleAlphaToCoverage = false;
                sampleAlphaToOne = false;
                sampleCoverage = 1.0f;
                sampleMask = 0xFFFFFFFF;
                sampleShading = false;
                samplesMin = 0;

                // TODO Posible bug. No ID is associated with these parameters
                occlusionQuery = null;
                occlusionMode = QueryMode.WAIT;

                // TODO Posible bug. No ID is associated with these parameters
                enableScissor = false;
                scissor = new Vector4i(0, 0, 0, 0);

                // --------------- parameters related with stencil
                stencilId = 0;
                enableStencil = false;
                ffunc = Function.ALWAYS;
                fref = 0;
                fmask = 0xFFFFFFFF;
                ffail = StencilOperation.KEEP;
                fdpfail = StencilOperation.KEEP;
                fdppass = StencilOperation.KEEP;
                bfunc = Function.ALWAYS;
                bref = 0;
                bmask = 0xFFFFFFFF;
                bfail = StencilOperation.KEEP;
                bdpfail = StencilOperation.KEEP;
                bdppass = StencilOperation.KEEP;

                // --------------- parameters related with depth test
                depthId = 0;
                enableDepth = false;
                depth = Function.LESS;

                // --------------- parameters related with blend
                blendId = 0;
                multiBlendEnable = false;
                multiBlendEq = false;
                for (int i = 0; i < 4; ++i)
                {
                    enableBlend[i] = false;
                    rgb[i] = BlendEquation.ADD;
                    srgb[i] = BlendArgument.ONE;
                    drgb[i] = BlendArgument.ZERO;
                    alpha[i] = BlendEquation.ADD;
                    salpha[i] = BlendArgument.ONE;
                    dalpha[i] = BlendArgument.ZERO;
                    colorMask[i] = new Vector4b(true, true, true, true);
                }

                // TODO Posible bug. No ID is associated with these parameters
                color = new Vector4f(0.0f, 0.0f, 0.0f, 0.0f);
                enableDither = false;
                enableLogic = false;
                logicOp = LogicOperation.COPY;

                // --------------- parameters related with mask
                maskId = 0;
                multiColorMask = false;
                for (int i = 0; i < 4; ++i)
                {
                    colorMask[i] = new Vector4b(true, true, true, true);
                }
                depthMask = true;
                stencilMaskFront = 0xFFFFFFFF;
                stencilMaskBack = 0xFFFFFFFF;

            }

            /// <summary>
            /// The viewport that defines the destination area for FrameBuffer#draw.
            /// This valueC is specific to this framebuffer instance and is
            /// automatically updated when the framebuffer is activated with
            /// FrameBuffer##set.
            /// (This corresponds to up, down, left and right planes).
            /// </summary>
            internal Vector4i viewport;

            /// <summary>
            /// The depth range that defines the destination area for #draw.
            /// Contains far and near planes.
            /// </summary>
            internal Vector2f depthRange;

            /// <summary>
            /// Defines which planes must be used for clipping tests.
            /// Each bit of clipDistances corresponds to a given plane.
            /// Default is all activated.
            /// </summary>
            internal int clipDistances;

            /// <summary>
            /// A unique Id incremented each time viewport, depthrange or clipDistances change.
            /// </summary>/
            internal int transformId;

            // -------------

            /// <summary>
            /// The color used to refill the framebuffer when calling glClear.
            /// </summary>
            internal Vector4f clearColor;


            /// <summary>
            /// The depth valueC used to refill the depthbuffer when calling glClear.
            /// </summary>
            internal double clearDepth;

            /// <summary>
            /// The stencil valueC used to refill the stencilbuffer when calling glClear.
            /// </summary>
            internal int clearStencil;

            /// <summary>
            /// A unique ID incremented each time clearColor, clearDepth or clearStencil
            /// change.
            /// </summary>
            internal int clearId;

            // -------------

            /// <summary>
            /// Defines the size of a drawn 'point' primitive.
            /// </summary>
            internal float pointSize; // <= 0.0 means controlled by program

            /// <summary>
            /// Defines the size above which point sizes are clamped.
            /// </summary>
            internal float pointFadeThresholdSize;


            /// <summary>
            /// Defines the origin of a drawn point primitive.
            /// If true, will be the lower left corner.
            /// Otherwise, will be the upper left corner. Default is false.
            /// </summary>
            internal bool pointLowerLeftOrigin;

            /// <summary>
            /// A unique ID incremented each time pointSize, pointFadeThresholdSize or
            /// pointLowerLeftOrigin change.
            /// </summary>
            internal int pointId;

            // -------------
            internal int lineId;


            /// <summary>
            /// Defines rasterized width of both aliased and antialiased lines.
            /// </summary>
            internal float lineWidth;

            /// <summary>
            /// If true, antialiasing will be enabled when drawing lines.
            /// </summary>
            internal bool lineSmooth;

            /// <summary>
            /// Specifies the orientation of front-facing polygons.
            /// If false, will be Counter-clockwise. Clockwise otherwise.
            /// </summary>
            internal bool frontFaceCW;

            /// <summary>
            /// Specifies how front-facing polygons will be rasterized.
            /// </summary>
            internal PolygonMode polygonFront;

            /// <summary>
            /// Specifies how back-facing polygons will be rasterized.
            /// </summary>
            internal PolygonMode polygonBack;


            /// <summary>
            /// If true, antialiasing will be enabled when drawing polygons.
            /// </summary>
            internal bool polygonSmooth;

            /// <summary>
            /// The scale and units used to calculate depth values.
            /// The x coordinate specifies a scale factor that is used
            /// to create a variable depth offset for each polygon.
            /// The y coordinate is multiplied by an implementation-specific
            /// valueC to create a constant depth offset.
            /// </summary>
            internal Vector2f polygonOffset;

            /// <summary>
            /// Determiens if fragment's depth valueC will be offset after it is
            /// interpolated from the depth values of the appropriate vertices.
            /// x is for points, y for lines and z for polygons.
            /// </summary
            internal Vector3b polygonOffsets;

            /// <summary>
            /// Modified each time frontFaceCW, polygonFront, polygonBack
            /// polygonSmooth, polygonOffset or polygonOffsets change.
            /// </summary>
            internal int polygonId;

            /// <summary>
            /// If enabled, use multiple fragment samples in computing the final
            /// color of a pixel.
            /// </summary>
            internal bool multiSample;

            /// <summary>
            /// If enabled, computes a temporary coverage valueC where each bit is
            /// determined by the alpha valueC at the corresponding sample location.
            /// The temporary coverage valueC is then ANDed with the fragment
            /// coverage valueC.
            /// The sample alpha to coverage.
            /// </summary>
            internal bool sampleAlphaToCoverage;

            /// <summary>
            /// If enabled, each sample alpha valueC is replaced by the maximum
            /// representable alpha valueC.
            /// </summary>
            internal bool sampleAlphaToOne;


            /// <summary>
            /// If enabled, the fragment's coverage is ANDed with the temporary
            /// coverage valueC. If set to 1.0f, will disable sampleCoverage.
            /// Negative values will invert coverage valueC.
            /// </summary>
            internal float sampleCoverage;

            /// <summary>
            /// Used to change the coverage of a sample or to exclude some samples
            /// from further fragment processing. Will never enable uncovered samples.
            /// </summary>
            internal uint sampleMask; // 0xFFFFFFFF

            /// <summary>
            /// If enabled, this will explicitly request that an implementation uses
            /// a minimum number of unique set of fragment computation inputs when
            /// multisampling a pixel (i.e. different samples). This is used to avoid
            /// aliasing.
            /// </summary>
            internal bool sampleShading;

            /// <summary>
            /// Minimum number of unique set of fragment computation inputs when
            /// multisampling a pixel (i.e. different samples). Only used if
            /// #sampleShading is enabled.
            /// </summary>
            internal float samplesMin;

            /// <summary>
            /// Modified each time multiSample, sampleAlphaToCoverage, sampleAlphaToOne
            /// sampleCoverage, sampleMask, sampleShading or samplesMin change.
            /// </summary>
            internal int multiSampleId;

            // -------------

            /// <summary>
            /// Occlusion queries use query objects to track the number of fragments
            /// or samples that pass the depth test.
            /// The occlusion query.
            /// </summary>
            internal Query occlusionQuery;

            /// <summary>
            /// Defines the comparison mode used for the depth tests when using occlusion
            /// queries.
            /// </summary>
            internal QueryMode occlusionMode;

            // -------------

            /// <summary>
            /// If enabled, only the fragments inside #scissor will not be discarded.
            /// If disabled, the scissor test always passes.
            /// </summary>
            internal bool enableScissor;


            /// <summary>
            /// The viewport of the scissor test.
            /// </summary>
            internal Vector4i scissor;

            /// <summary>
            /// If enabled, the stencil test conditionnaly discards a fragment based on
            /// the outcome of a comparison between the valueC in the stencil buffer at
            /// the fragment's position and a reference valueC. Different test functions
            /// may be used, and front-faces and back-faces are treated separatly.
            /// </summary>
            internal bool enableStencil;


            /// <summary>
            /// TThe stencil test function for front faces.
            /// </summary>
            internal Function ffunc;


            /// <summary>
            ///  The stencil reference valueC for stencil tests of front faces.
            /// </summary>
            internal int fref;

            /// <summary>
            /// The stencil mask valueC for stencil tests of front faces.
            /// </summary>
            internal uint fmask;

            /// <summary>
            /// The action to use when the stencil test fails for front faces.
            /// </summary>
            internal StencilOperation ffail;


            /// <summary>
            /// The action to use when the depth test fails for front faces.
            /// </summary>
            internal StencilOperation fdpfail;

            /// <summary>
            /// The action to use when the depth test passes for front faces.
            /// </summary>
            internal StencilOperation fdppass;

            /// <summary>
            /// The stencil test function for back faces.
            /// </summary>
            internal Function bfunc;

            /// <summary>
            /// The stencil reference valueC for stencil tests of back faces.
            /// </summary>
            internal int bref;

            /// <summary>
            /// The stencil mask valueC for stencil tests of back faces.
            /// </summary>
            internal uint bmask;

            /// <summary>
            /// The action to use when the stencil test fails for back faces.
            /// </summary>
            internal StencilOperation bfail;

            /// <summary>
            /// The action to use when the depth test fails for back faces.
            /// </summary>
            internal StencilOperation bdpfail;

            /// <summary>
            /// The action to use when the depth test passes for back faces.
            /// </summary>
            internal StencilOperation bdppass;

            /// <summary>
            /// A unique ID incremented each time enableStencil, ffunc, fref, fmask, ffail, fdpfail, fdppass,
            /// bfunc, bref, bmask, bfail, bdpfail, bdppass change.
            /// The stencil identifier.
            /// </summary>
            internal int stencilId;

            // -------------

            /// <summary>
            /// A unique Id incremented each time enableDepth, or depth change.
            /// </summary>/
            internal int depthId;

            /// <summary>
            /// If enabled, the depth buffer test discards the incoming fragment
            /// if a depth comparison fails. The comparison is specified with #depth.
            /// </summary>
            internal bool enableDepth;

            /// <summary>
            /// Depth comparison function.
            /// </summary>
            internal Function depth;

            /// <summary>
            /// If enabled, and if multiple draw buffers are set, blending will be
            /// enabled for all of them.
            /// The multi blend enable.
            /// </summary>
            internal bool multiBlendEnable;

            /// <summary>
            /// If enabled, and if multiple draw buffers are set, different blending
            /// equations will be used.
            /// </summary>
            internal bool multiBlendEq;

            /// <summary>
            /// Determines for which draw buffers the blending should be enabled.
            /// </summary>
            internal bool[] enableBlend = new bool[4];

            /// <summary>
            /// The blending equation to use for color channels.
            /// </summary>
            internal BlendEquation[] rgb = new BlendEquation[4];

            /// <summary>
            /// The source color to be blended.
            /// </summary>
            internal BlendArgument[] srgb = new BlendArgument[4];

            /// <summary>
            /// The destination color with which the source color must be blended.
            /// </summary>
            internal BlendArgument[] drgb = new BlendArgument[4];

            /// <summary>
            /// The blending equation to use for alpha channel.
            /// </summary>
            internal BlendEquation[] alpha = new BlendEquation[4];

            /// <summary>
            /// The source alpha to be blended.
            /// </summary>
            internal BlendArgument[] salpha = new BlendArgument[4];

            /// <summary>
            /// The destination alpha with which the source alpha must be blended.
            /// </summary>
            internal BlendArgument[] dalpha = new BlendArgument[4];

            /// <summary>
            /// The color used in the blend equations. Only used if any of the equation has CONSTANT_COLOR inside it.
            /// </summary>
            internal Vector4f color;

            /// <summary>
            /// Modified each time multiBlendEnable, multiBlendeq, enableBlend, rgb, srgb, drgb,
            /// alpha, salpha or dalpha change.
            /// </summary>
            internal int blendId;


            /// <summary>
            /// If enabled, dithering selects between two representable color values or indices.
            /// A representable valueC is a valueC that has an exact representation in the color buffer.
            /// Dithering selects, for each color component, either the largest representable color
            /// valueC (for that particular color component) that is less than or equal to the incoming
            /// color component valueC, c, or the smallest representable color valueC that is greater
            /// than or equal to c.
            /// The enable dither.
            /// </summary>
            internal bool enableDither;

            // -------------


            /// <summary>
            /// If enabled, a logic operation is applied between the incoming fragment's color valueC
            /// and the color values stored at the corresponding location in the framebuffer.
            /// The enable logic.
            /// </summary>
            internal bool enableLogic;

            /// <summary>
            /// The logical operation used if logical operations are enabled.
            /// The logic op.
            /// </summary>
            internal LogicOperation logicOp;

            /// <summary>
            /// If enabled, each draw buffer will have its own color mask. Color mask will determine
            /// which color(s) may be written to.
            /// The multi color mask.
            /// </summary>
            internal bool multiColorMask;


            /// <summary>
            /// The color masks for each draw buffers.
            /// </summary>
            internal Vector4b[] colorMask = new Vector4b[4];


            /// <summary>
            ///  If enabled, depth buffer can be written to.
            /// </summary>
            internal bool depthMask;


            /// <summary>
            /// If enabled, stencil mask can be written to for front faces.
            /// </summary>
            internal uint stencilMaskFront;

            /// <summary>
            /// If enabled, stencil mask can be written to for back faces.
            /// </summary>
            internal uint stencilMaskBack;


            /// <summary>
            /// The mask identifier.
            /// </summary>
            internal int maskId;

            /// A unique ID incremented each time multiColorMask, colorMask, depthMask,
            /// stencilMaskFront or stencilMaskBack change.
            internal Parameters set(Parameters p)
            {
#if DEBUG
                if (log.IsDebugEnabled)
                    log.Debug("Set FrameBuffer Parameters");
#endif
                int version = 0;
#if OPENTK
                GL.GetInteger(GetPName.MajorVersion, out version);
#else
                glGetIntegerv(GL_MAJOR_VERSION, &version);
#endif
                // TRANSFORM -------------
                if (transformId != p.transformId)
                {
#if OPENTK
                    GL.Viewport(p.viewport.X, p.viewport.Y, p.viewport.Z, p.viewport.W);
                    GL.DepthRange((double)p.depthRange.X, (double)p.depthRange.Y);
                    for (int i = 0; i < 6; ++i)
                    {
                        glEnable((EnableCap)(EnableCap.ClipPlane0 + i), (p.clipDistances & (1 << i)) != 0);
                    }
#else
                    glViewport(p.viewport.x, p.viewport.y, p.viewport.z, p.viewport.w);
                    glDepthRange(p.depthRange.x, p.depthRange.y);
                    for (int i = 0; i < 6; ++i) {
                        glEnable(GL_CLIP_DISTANCE0 + i, (p.clipDistances & (1 << i)) != 0);
                    }
#endif

                }
                // CLEAR -------------
                if (clearId != p.clearId)
                {
#if OPENTK
                    GL.ClearColor(p.clearColor.X, p.clearColor.Y, p.clearColor.Z, p.clearColor.W);
                    GL.ClearDepth(p.clearDepth);
                    GL.ClearStencil(p.clearStencil);
#else
                    glClearColor(p.clearColor.x, p.clearColor.y, p.clearColor.z, p.clearColor.w);
                    glClearDepth(p.clearDepth);
                    glClearStencil(p.clearStencil);
#endif
                }
                // POINTS -------------
                if (pointId != p.pointId)
                {
#if OPENTK
                    glEnable(EnableCap.ProgramPointSize, p.pointSize <= 0.0f);
                    GL.PointSize(p.pointSize);
                    GL.PointParameter(PointParameterName.PointFadeThresholdSize, p.pointFadeThresholdSize);
                    GL.PointParameter(PointParameterName.PointSpriteCoordOrigin, (int)(p.pointLowerLeftOrigin ? PointSpriteCoordOriginParameter.LowerLeft : PointSpriteCoordOriginParameter.UpperLeft));
#else
                    glEnable(GL_PROGRAM_POINT_SIZE, p.pointSize <= 0.0f);
                    glPointSize(p.pointSize);
                    glPointParameterf(GL_POINT_FADE_THRESHOLD_SIZE, p.pointFadeThresholdSize);
                    glPointParameteri(GL_POINT_SPRITE_COORD_ORIGIN, p.pointLowerLeftOrigin ? GL_LOWER_LEFT : GL_UPPER_LEFT);
#endif
                }

                // LINES -------------
                if (lineId != p.lineId)
                {
#if OPENTK
                    glEnable(EnableCap.LineSmooth, p.lineSmooth);
                    GL.LineWidth(p.lineWidth);
#else
                    glEnable(GL_LINE_SMOOTH, p.lineSmooth);
                    glLineWidth(p.lineWidth);
#endif
                }

                // POLYGONS -------------
                if (polygonId != p.polygonId)
                {
#if OPENTK
                    GL.FrontFace(p.frontFaceCW ? FrontFaceDirection.Cw : FrontFaceDirection.Ccw);

                    if (p.polygonFront == PolygonMode.CULL || p.polygonBack == PolygonMode.CULL)
                    {
                        GL.Enable(EnableCap.CullFace);
                        if (p.polygonFront == PolygonMode.CULL && p.polygonBack == PolygonMode.CULL)
                        {
                            GL.CullFace(CullFaceMode.FrontAndBack);
                        }
                        else if (p.polygonFront == PolygonMode.CULL)
                        {
                            GL.CullFace(CullFaceMode.Front);
                        }
                        else
                        {
                            GL.CullFace(CullFaceMode.Back);
                        }
                    }
                    else
                    {
                        GL.Disable(EnableCap.CullFace);
                    }
                    switch (p.polygonFront)
                    {
                        case PolygonMode.CULL:
                            switch (p.polygonBack)
                            {
                                case PolygonMode.CULL:
                                    break;
                                case PolygonMode.POINT:
                                    GL.PolygonMode(MaterialFace.FrontAndBack, OpenTK.Graphics.OpenGL.PolygonMode.Point);
                                    break;
                                case PolygonMode.LINE:
                                    GL.PolygonMode(MaterialFace.FrontAndBack, OpenTK.Graphics.OpenGL.PolygonMode.Line);
                                    break;
                                case PolygonMode.FILL:
                                    GL.PolygonMode(MaterialFace.FrontAndBack, OpenTK.Graphics.OpenGL.PolygonMode.Fill);
                                    break;
                            }
                            break;
                        case PolygonMode.POINT:
                            GL.PolygonMode(MaterialFace.FrontAndBack, OpenTK.Graphics.OpenGL.PolygonMode.Point);
                            break;
                        case PolygonMode.LINE:
                            GL.PolygonMode(MaterialFace.FrontAndBack, OpenTK.Graphics.OpenGL.PolygonMode.Line);
                            break;
                        case PolygonMode.FILL:
                            GL.PolygonMode(MaterialFace.FrontAndBack, OpenTK.Graphics.OpenGL.PolygonMode.Fill);
                            break;
                    }
                    Debug.Assert(getError() == ErrorCode.NoError);
                    glEnable(EnableCap.PolygonSmooth, p.polygonSmooth);
                    GL.PolygonOffset(p.polygonOffset.X, p.polygonOffset.Y);
                    glEnable(EnableCap.PolygonOffsetPoint, p.polygonOffsets.X);
                    glEnable(EnableCap.PolygonOffsetLine, p.polygonOffsets.Y);
                    glEnable(EnableCap.PolygonOffsetFill, p.polygonOffsets.Z);
#else
                    glFrontFace(p.frontFaceCW ? GL_CW : GL_CCW);
                    if (p.polygonFront == CULL || p.polygonBack == CULL)
                    {
                        glEnable(GL_CULL_FACE);
                        if (p.polygonFront == CULL && p.polygonBack == CULL)
                        {
                            glCullFace(GL_FRONT_AND_BACK);
                        }
                        else if (p.polygonFront == CULL)
                        {
                            glCullFace(GL_FRONT);
                        }
                        else
                        {
                            glCullFace(GL_BACK);
                        }
                    }
                    else
                    {
                        glDisable(GL_CULL_FACE);
                    }
                    switch (p.polygonFront)
                    {
                        case CULL:
                            switch (p.polygonBack)
                            {
                                case CULL:
                                    break;
                                case POINT:
                                    glPolygonMode(GL_FRONT_AND_BACK, GL_POINT);
                                    break;
                                case LINE:
                                    glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);
                                    break;
                                case FILL:
                                    glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
                                    break;
                            }
                            break;
                        case POINT:
                            glPolygonMode(GL_FRONT_AND_BACK, GL_POINT);
                            break;
                        case LINE:
                            glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);
                            break;
                        case FILL:
                            glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
                            break;
                    }
                    Debug.Assert(getError() == ErrorCode.NoError);
                    glEnable(GL_POLYGON_SMOOTH, p.polygonSmooth);
                    glPolygonOffset(p.polygonOffset.x, p.polygonOffset.y);
                    glEnable(GL_POLYGON_OFFSET_POINT, p.polygonOffsets.x);
                    glEnable(GL_POLYGON_OFFSET_LINE, p.polygonOffsets.y);
                    glEnable(GL_POLYGON_OFFSET_FILL, p.polygonOffsets.z);
#endif
                }
                // MULTISAMPLING -------------
                if (multiSampleId != p.multiSampleId)
                {
#if OPENTK
                    glEnable(EnableCap.Multisample, p.multiSample);
                    glEnable(EnableCap.SampleAlphaToCoverage, p.sampleAlphaToCoverage);
                    glEnable(EnableCap.SampleAlphaToOne, p.sampleAlphaToOne);
                    glEnable(EnableCap.SampleCoverage, p.sampleCoverage < 1.0f);
                    GL.SampleCoverage(System.Math.Abs(p.sampleCoverage), p.sampleCoverage < 0.0f);
                    glEnable(EnableCap.SampleMask, p.sampleMask != (uint)0xFFFFFFFF);
                    GL.SampleMask(0, p.sampleMask);
                    if (version >= 4)
                    {
                        glEnable(EnableCap.SampleShading, p.sampleShading);
                        GL.MinSampleShading(p.samplesMin);
                    }
#else
                    glEnable(GL_MULTISAMPLE, p.multiSample);
                    glEnable(GL_SAMPLE_ALPHA_TO_COVERAGE, p.sampleAlphaToCoverage);
                    glEnable(GL_SAMPLE_ALPHA_TO_ONE, p.sampleAlphaToOne);
                    glEnable(GL_SAMPLE_COVERAGE, p.sampleCoverage < 1.0f);
                    glSampleCoverage(abs(p.sampleCoverage), p.sampleCoverage < 0.0f);
                    glEnable(GL_SAMPLE_MASK, p.sampleMask != (GLuint)0xFFFFFFFF);
                    glSampleMaski(0, p.sampleMask);
                    if (version >= 4)
                    {
                        glEnable(GL_SAMPLE_SHADING, p.sampleShading);
                        glMinSampleShading(p.samplesMin);
                    }
#endif
                }
                // SCISSOR TEST -------------
                if (enableScissor != p.enableScissor ||
                    scissor != p.scissor)
                {
#if OPENTK
                    glEnable(EnableCap.ScissorTest, p.enableScissor);
                    GL.Scissor(p.scissor.X, p.scissor.Y, p.scissor.Z, p.scissor.W);
#else
                    glEnable(GL_SCISSOR_TEST, p.enableScissor);
                    glScissor(p.scissor.x, p.scissor.y, p.scissor.z, p.scissor.w);
#endif
                }
                // STENCIL TEST -------------
                if (stencilId != p.stencilId)
                {
#if OPENGL
                    glEnable(GL_STENCIL_TEST, p.enableStencil);
                    glStencilFuncSeparate(GL_FRONT, EnumConversion.getFunction(p.ffunc), p.fref, p.fmask);
                    glStencilFuncSeparate(GL_BACK, EnumConversion.getFunction(p.bfunc), p.bref, p.bmask);
                    glStencilOpSeparate(GL_FRONT, getStencilOperation(p.ffail), getStencilOperation(p.fdpfail), getStencilOperation(p.fdppass));
                    glStencilOpSeparate(GL_BACK, getStencilOperation(p.bfail), getStencilOperation(p.bdpfail), getStencilOperation(p.bdppass));
#else
                    glEnable(EnableCap.StencilTest, p.enableStencil);
                    GL.StencilFuncSeparate(StencilFace.Front, EnumConversion.getStencilFunction(p.ffunc), p.fref, p.fmask);
                    GL.StencilFuncSeparate(StencilFace.Back, EnumConversion.getStencilFunction(p.bfunc), p.bref, p.bmask);
                    GL.StencilOpSeparate(StencilFace.Front, EnumConversion.getStencilOperation(p.ffail), EnumConversion.getStencilOperation(p.fdpfail), EnumConversion.getStencilOperation(p.fdppass));
                    GL.StencilOpSeparate(StencilFace.Back, EnumConversion.getStencilOperation(p.bfail), EnumConversion.getStencilOperation(p.bdpfail), EnumConversion.getStencilOperation(p.bdppass));
#endif
                }
                // DEPTH TEST -------------
                if (depthId != p.depthId)
                {
#if OPENTK
                    glEnable(EnableCap.DepthTest, p.enableDepth);
                    GL.DepthFunc(EnumConversion.getDepthFunction(p.depth));
#else
                    glEnable(GL_DEPTH_TEST, p.enableDepth);
                    glDepthFunc(EnumConversion.getFunction(p.depth));
#endif
                }

                // BLENDING --------------
                if (blendId != p.blendId)
                {
#if OPENTK
                    if (p.multiBlendEnable)
                    {
                        for (int i = 0; i < 4; ++i)
                        {
                            if (p.enableBlend[i])
                            {
                                GL.Enable(IndexedEnableCap.Blend, i);
                            }
                            else
                            {
                                GL.Disable(IndexedEnableCap.Blend, i);
                            }
                        }
                    }
                    else
                    {
                        glEnable(EnableCap.Blend, p.enableBlend[0]);
                    }
                    if (p.multiBlendEq && version >= 4)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            GL.BlendEquationSeparate(i, EnumConversion.getBlendEquation(p.rgb[i]), EnumConversion.getBlendEquation(p.alpha[i]));

                            GL.BlendFuncSeparate(i, (Version40)EnumConversion.getBlendSrcArgument(p.srgb[i]), (Version40)EnumConversion.getBlendDestArgument(p.drgb[i]), (Version40)EnumConversion.getBlendSrcArgument(p.salpha[i]), (Version40)EnumConversion.getBlendDestArgument(p.dalpha[i]));
                            throw new NotImplementedException("Am I using BlendFuncSeparate correctly??");
                        }
                    }
                    else
                    {
                        GL.BlendEquationSeparate(EnumConversion.getBlendEquation(p.rgb[0]), EnumConversion.getBlendEquation(p.alpha[0]));

                        GL.BlendFuncSeparate(EnumConversion.getBlendSrcArgument(p.srgb[0]), EnumConversion.getBlendDestArgument(p.drgb[0]), EnumConversion.getBlendSrcArgument(p.salpha[0]), EnumConversion.getBlendDestArgument(p.dalpha[0]));
                    }
                    GL.BlendColor(p.color.X, p.color.Y, p.color.Z, p.color.W);
#else
                    if (p.multiBlendEnable)
                    {
                        for (int i = 0; i < 4; ++i)
                        {
                            if (p.enableBlend[i])
                            {
                                glEnablei(GL_BLEND, i);
                            }
                            else
                            {
                                glDisablei(GL_BLEND, i);
                            }
                        }
                    }
                    else
                    {
                        glEnable(GL_BLEND, p.enableBlend[0]);
                    }
                    if (p.multiBlendEq && version >= 4)
                    {
                        for (int i = 0; i < 4; ++i)
                        {
                            glBlendEquationSeparatei(i, EnumConversion.getBlendEquation(p.rgb[i]), EnumConversion.getBlendEquation(p.alpha[i]));
                            glBlendFuncSeparatei(i, EnumConversion.getBlendArgument(p.srgb[i]), EnumConversion.getBlendArgument(p.drgb[i]), EnumConversion.getBlendArgument(p.salpha[i]), EnumConversion.getBlendArgument(p.dalpha[i]));
                        }
                    }
                    else
                    {
                        glBlendEquationSeparate(EnumConversion.getBlendEquation(p.rgb[0]), EnumConversion.getBlendEquation(p.alpha[0]));
                        glBlendFuncSeparate(EnumConversion.getBlendArgument(p.srgb[0]), EnumConversion.getBlendArgument(p.drgb[0]), EnumConversion.getBlendArgument(p.salpha[0]), EnumConversion.getBlendArgument(p.dalpha[0]));
                    }
                    glBlendColor(p.color.x, p.color.y, p.color.z, p.color.w);
#endif
                }
                // DITHERING --------------
                if (enableDither != p.enableDither)
                {
#if OPENTK
                    glEnable(EnableCap.Dither, p.enableDither);
#else
                    glEnable(GL_DITHER, p.enableDither);
#endif
                }
                // LOGIC OP --------------
                if (enableLogic != p.enableLogic ||
                    logicOp != p.logicOp)
                {
#if OPENTK
                    glEnable(EnableCap.ColorLogicOp, p.enableDither);
                    GL.LogicOp(EnumConversion.getLogicOperation(p.logicOp));
#else
                    glEnable(GL_COLOR_LOGIC_OP, p.enableDither);
                    glLogicOp(getLogicOperation(p.logicOp));
#endif
                }
                // WRITE MASKS --------------
                if (maskId != p.maskId)
                {
#if OPENTK
                    if (p.multiColorMask)
                    {
                        for (int i = 0; i < 4; ++i)
                        {
                            GL.ColorMask(i, p.colorMask[i].X, p.colorMask[i].Y, p.colorMask[i].Z, p.colorMask[i].W);
                        }
                    }
                    else
                    {
                        GL.ColorMask(p.colorMask[0].X, p.colorMask[0].Y, p.colorMask[0].Z, p.colorMask[0].W);
                    }
                    GL.DepthMask(p.depthMask);
                    GL.StencilMaskSeparate(StencilFace.Front, p.stencilMaskFront);
                    GL.StencilMaskSeparate(StencilFace.Back, p.stencilMaskBack);
#else
                    if (p.multiColorMask)
                    {
                        for (int i = 0; i < 4; ++i)
                        {
                            glColorMaski(i, p.colorMask[i].x, p.colorMask[i].y, p.colorMask[i].z, p.colorMask[i].w);
                        }
                    }
                    else
                    {
                        glColorMask(p.colorMask[0].x, p.colorMask[0].y, p.colorMask[0].z, p.colorMask[0].w);
                    e
                    glDepthMask(p.depthMask);
                    glStencilMaskSeparate(GL_FRONT, p.stencilMaskFront);
                    glStencilMaskSeparate(GL_BACK, p.stencilMaskBack);
#endif
                }
                Debug.Assert(getError() == ErrorCode.NoError);
                // *this = p;
                return p.DeepClone();
            }

            private void glEnable(EnableCap p, bool b)
            {
#if OPENTK
                if (b)
                    GL.Enable(p);
                else
                    GL.Disable(p);

#else
               if (b) {
                    glEnable(p);
                } else {
                    glDisable(p);
                }
#endif
            }

            private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            object IDeepCloneable.DeepClone()
            {
                return this.DeepClone();
            }
            public Parameters DeepClone()
            {
                Parameters parameters = (Parameters)this.MemberwiseClone();
                parameters.enableBlend = new bool[4];
                parameters.rgb = new BlendEquation[4];
                parameters.srgb = new BlendArgument[4];
                parameters.drgb = new BlendArgument[4];
                parameters.alpha = new BlendEquation[4];
                parameters.salpha = new BlendArgument[4];
                parameters.dalpha = new BlendArgument[4];

                Array.Copy(this.enableBlend, parameters.enableBlend, 4);
                Array.Copy(this.rgb, parameters.rgb, 4);
                Array.Copy(this.srgb, parameters.srgb, 4);
                Array.Copy(this.drgb, parameters.drgb, 4);
                Array.Copy(this.alpha, parameters.alpha, 4);
                Array.Copy(this.salpha, parameters.salpha, 4);
                Array.Copy(this.dalpha, parameters.dalpha, 4);

                return parameters;
            }
        }


        /// <summary>
        /// Creates a new frambuffer. This creates an offsreen framebuffer. The
        /// default framebuffer can be retrieved with #getDefault.
        /// Initializes a new instance of the <see cref="Sxta.Render.FrameBuffer"/> class.
        /// </summary>
        public FrameBuffer()
        {
            attachmentsChanged = false;
            readDrawChanged = false;
            parametersChanged = false;
            checkExtensions();
#if OPENTK
            GL.GenFramebuffers(1, out framebufferId);
#else
            glGenFramebuffers(1, &framebufferId);
#endif
            Debug.Assert(getError() == ErrorCode.NoError);

            for (int i = 0; i < 6; ++i)
            {
                textures[i] = null;
                levels[i] = 0;
                layers[i] = 0;
            }

            readBuffer = BufferId.COLOR0;
            drawBufferCount = 1;
            drawBuffers[0] = BufferId.COLOR0;
        }

        /// <summary>
        /// Creates a new framebuffer.
        /// Initializes a new instance of the <see cref="Sxta.Render.FrameBuffer"/> class.
        /// </summary>
        /// <param name='main'>
        /// Main true for the default framebuffer.
        /// </param>
        public FrameBuffer(bool main)
        {
            attachmentsChanged = false;
            readDrawChanged = false;
            parametersChanged = false;
            checkExtensions();
            if (main)
            {
                framebufferId = 0;
            }
            else
            {
#if OPENTK
                GL.GenFramebuffers(1, out framebufferId);
#else
                glGenFramebuffers(1, &framebufferId);
#endif
                Debug.Assert(getError() == ErrorCode.NoError);
            }
            readBuffer = BufferId.COLOR0;
            drawBufferCount = 1;
            drawBuffers[0] = BufferId.COLOR0;
        }

        /// <summary>
        /// Deletes this framebuffer.
        /// Releases unmanaged resources and performs other cleanup operations before the 
        /// <see cref="Sxta.Render.FrameBuffer"/> is reclaimed by garbage collection.
        /// </summary>
        ~FrameBuffer()
        {
            // Do not re-create Dispose clean-up code here. 
            // Calling Dispose(false) is optimal in terms of 
            // readability and maintainability.
            Dispose(false);
        }

        /// <summary>
        /// Gets the default.
        /// </summary>
        /// <returns>
        /// The default, onscreen framebuffer.
        /// </returns>
        public static FrameBuffer getDefault()
        {
            IGraphicsContext context;
#if OPENTK
            context = GraphicsContext.CurrentContext;
#else
            context = wglGetCurrentContext();
#endif
            FrameBuffer fb;
            if (!DEFAULT.buffers.TryGetValue(context, out fb))
            {
                fb = new FrameBuffer(true);
                DEFAULT.buffers.Add(context, fb);
            }
            return fb;
        }


        /// <summary>
        /// Returns the render buffer attached to the given attachment point.
        /// </summary>
        /// <returns>
        /// the render buffer attached to the given attachment point,
        /// or null if there is no renderbuffer bound to this attachment
        /// point (either because nothing is attached, or because a texture
        /// is attached).
        /// </returns>
        /// <param name='b'>
        /// B.
        /// </param>
        public RenderBuffer getRenderBuffer(BufferId b)
        {
            return (RenderBuffer)textures[EnumConversion.getBufferId(b)];
        }

        /// <summary>
        /// the texture attached to the given attachment point,
        /// or null if there is no texture bound to this attachment
        /// point (either because nothing is attached, or because a render
        /// buffer is attached).
        /// </summary>
        /// <returns>
        /// The texture buffer.
        /// </returns>
        /// <param name='b'>
        /// B.
        /// </param>
        public Texture getTextureBuffer(BufferId b)
        {
            return (Texture)textures[EnumConversion.getBufferId(b)];
        }

        /// <summary>
        /// Sets an attachment of this framebuffer using a render buffer.
        /// </summary>
        /// <returns>
        /// The render buffer.
        /// </returns>
        /// <param name='b'>
        /// B an attachment point.
        /// </param>
        /// <param name='t'>
        /// T e render buffer to be bound to this attachment point.
        /// </param>
        public void setRenderBuffer(BufferId b, RenderBuffer t)
        {
            Debug.Assert(framebufferId != 0);
            int i = EnumConversion.getBufferId(b);
            textures[i] = t;
            attachmentsChanged = true;
        }

        /// <summary>
        /// Sets an attachment of this framebuffer using a texture.
        /// </summary>
        /// <returns>
        /// The texture buffer.
        /// </returns>
        /// <param name='b'>
        /// B an attachment point.
        /// </param>
        /// <param name='t'>
        /// T the texture to be bound to this attachment point.
        /// </param>
        /// <param name='level'>
        /// Level evel the LOD level to be attached.
        /// </param>
        public void setTextureBuffer(BufferId b, Texture1D t, int level)
        {
            Debug.Assert(framebufferId != 0);
            int i = EnumConversion.getBufferId(b);
            textures[i] = t;
            levels[i] = level;
            attachmentsChanged = true;
        }


        /// <summary>
        /// Sets an attachment of this framebuffer using a texture.
        /// </summary>
        /// <returns>
        /// The texture buffer.
        /// </returns>
        /// <param name='b'>
        /// B an attachment point.
        /// </param>
        /// <param name='t'>
        /// T the texture to be bound to this attachment point.
        /// </param>
        /// <param name='level'>
        /// Level the LOD level to be attached.
        /// </param>
        /// <param name='layer'>
        /// Layer the layer to be attached (-1 to attach all layers).
        /// </param>
        public void setTextureBuffer(BufferId b, Texture1DArray t, int level, int layer)
        {
            Debug.Assert(framebufferId != 0);
            int i = EnumConversion.getBufferId(b);
            textures[i] = t;
            levels[i] = level;
            layers[i] = layer;
            attachmentsChanged = true;
        }


        /// <summary>
        /// Sets an attachment of this framebuffer using a texture.
        /// </summary>
        /// <returns>
        /// The texture buffer.
        /// </returns>
        /// <param name='b'>
        /// B an attachment point.
        /// </param>
        /// <param name='t'>
        /// T the texture to be bound to this attachment point.
        /// </param>
        /// <param name='level'>
        /// Level the LOD level to be attached.
        /// </param>
        public void setTextureBuffer(BufferId b, Texture2D t, int level)
        {
            Debug.Assert(framebufferId != 0);
            int i = EnumConversion.getBufferId(b);
            textures[i] = t;
            levels[i] = level;
            attachmentsChanged = true;
        }

        /// <summary>
        /// Sets an attachment of this framebuffer using a texture.
        /// </summary>
        /// <returns>
        /// The texture buffer.
        /// </returns>
        /// <param name='b'>
        /// B an attachment point.
        /// </param>
        /// <param name='t'>
        /// T the texture to be bound to this attachment point.
        /// </param>
        /// <param name='level'>
        /// Level the LOD level to be attached.
        /// </param>
        public void setTextureBuffer(BufferId b, TextureRectangle t, int level)
        {
            Debug.Assert(framebufferId != 0);
            int i = EnumConversion.getBufferId(b);
            textures[i] = t;
            levels[i] = level;
            attachmentsChanged = true;
        }

        /// <summary>
        /// Sets an attachment of this framebuffer using a texture.
        /// </summary>
        /// <returns>
        /// The texture buffer.
        /// </returns>
        /// <param name='b'>
        /// B an attachment point.
        /// </param>
        /// <param name='t'>
        /// T the texture to be bound to this attachment point.
        /// </param>
        public void setTextureBuffer(BufferId b, Texture2DMultisample t)
        {
            Debug.Assert(framebufferId != 0);
            int i = EnumConversion.getBufferId(b);
            textures[i] = t;
            levels[i] = 0;
            attachmentsChanged = true;
        }


        /// <summary>
        /// Sets an attachment of this framebuffer using a texture.
        /// </summary>
        /// <returns>
        /// The texture buffer.
        /// </returns>
        /// <param name='b'>
        /// B an attachment point.
        /// </param>
        /// <param name='t'>
        /// T the texture to be bound to this attachment point.
        /// </param>
        /// <param name='level'>
        /// Level the LOD level to be attached.
        /// </param>
        /// <param name='layer'>
        /// Layer the layer to be attached (-1 to attach all layers).
        /// </param>
        public void setTextureBuffer(BufferId b, Texture2DArray t, int level, int layer)
        {
            Debug.Assert(framebufferId != 0);
            int i = EnumConversion.getBufferId(b);
            textures[i] = t;
            levels[i] = level;
            layers[i] = layer;
            attachmentsChanged = true;
        }


        /// <summary>
        /// Sets an attachment of this framebuffer using a texture.
        /// </summary>
        /// <returns>
        /// The texture buffer.
        /// </returns>
        /// <param name='b'>
        /// B an attachment point.
        /// </param>
        /// <param name='t'>
        /// T the texture to be bound to this attachment point.
        /// </param>
        /// <param name='layer'>
        /// Layer the layer to be attached (-1 to attach all layers).
        /// </param>
        public void setTextureBuffer(BufferId b, Texture2DMultisampleArray t, int layer)
        {
            Debug.Assert(framebufferId != 0);
            int i = EnumConversion.getBufferId(b);
            textures[i] = t;
            levels[i] = 0;
            layers[i] = layer;
            attachmentsChanged = true;
        }

        /// <summary>
        /// Sets an attachment of this framebuffer using a texture.
        /// </summary>
        /// <returns>
        /// The texture buffer.
        /// </returns>
        /// <param name='b'>
        /// B an attachment point.
        /// </param>
        /// <param name='t'>
        /// T. the texture to be bound to this attachment point.
        /// </param>
        /// <param name='level'>
        /// Level the LOD level to be attached.
        /// </param>
        /// <param name='layer'>
        /// Layer the z slice to be attached (-1 to attach all the slices).
        /// </param>
        public void setTextureBuffer(BufferId b, Texture3D t, int level, int layer)
        {
            Debug.Assert(framebufferId != 0);
            int i = EnumConversion.getBufferId(b);
            textures[i] = t;
            levels[i] = level;
            layers[i] = layer;
            attachmentsChanged = true;
        }


        /// <summary>
        /// Sets an attachment of this framebuffer using a texture.
        /// </summary>
        /// <returns>
        /// The texture buffer.
        /// </returns>
        /// <param name='b'>
        /// B an attachment point.
        /// </param>
        /// <param name='t'>
        /// T. the texture to be bound to this attachment point.
        /// </param>
        /// <param name='level'>
        /// Level the LOD level to be attached.
        /// </param>
        /// <param name='cf'>
        /// Cf. the cube face to be attached (-1 to attach all the faces).
        /// </param>
        public void setTextureBuffer(BufferId b, TextureCube t, int level, CubeFace cf)
        {
            Debug.Assert(framebufferId != 0);
            int i = EnumConversion.getBufferId(b);
            textures[i] = t;
            levels[i] = level;
            layers[i] = (int)cf;
            attachmentsChanged = true;
        }

        /// <summary>
        /// Sets an attachment of this framebuffer using a texture.
        /// </summary>
        /// <returns>
        /// The texture buffer.
        /// </returns>
        /// <param name='b'>
        /// B. an attachment point.
        /// </param>
        /// <param name='t'>
        /// T the texture to be bound to this attachment point.
        /// </param>
        /// <param name='level'>
        /// Level the LOD level to be attached.
        /// </param>
        /// <param name='cf'>
        /// Cf the cube face to be attached (all the layers for this face
        ///  will be attached at once).
        /// </param>
        public void setTextureBuffer(BufferId b, TextureCubeArray t, int level, CubeFace cf)
        {
            Debug.Assert(framebufferId != 0);
            int i = EnumConversion.getBufferId(b);
            textures[i] = t;
            levels[i] = level;
            layers[i] = (int)cf;
            attachmentsChanged = true;
        }


        /// <summary>
        /// Sets the read buffer for #readPixels and #copyPixels methods.
        /// </summary>
        /// <returns>
        /// The read buffer.
        /// </returns>
        /// <param name='b'>
        /// B one of the color buffer.
        /// </param>
        public void setReadBuffer(BufferId b)
        {
            readBuffer = b;
            readDrawChanged = true;
        }

        /// <summary>
        /// Sets the draw buffer for #clear and #draw methods.
        /// </summary>
        /// <returns>
        /// The draw buffer.
        /// </returns>
        /// <param name='b'>
        /// B  one of the color buffer.
        /// </param>
        public void setDrawBuffer(BufferId b)
        {
            drawBufferCount = 1;
            drawBuffers[0] = b;
            readDrawChanged = true;
        }

        /// <summary>
        /// Sets the draw buffers for #clear and #draw methods.
        /// </summary>
        /// <returns>
        /// The draw buffers.
        /// </returns>
        /// <param name='b'>
        /// B a set of color buffers (ORed with each other).
        /// </param>
        public void setDrawBuffers(BufferId b)
        {
            drawBufferCount = 0;
            if ((b & BufferId.COLOR0) != 0)
            {
                drawBuffers[drawBufferCount++] = BufferId.COLOR0;
            }
            if ((b & BufferId.COLOR1) != 0)
            {
                drawBuffers[drawBufferCount++] = BufferId.COLOR1;
            }
            if ((b & BufferId.COLOR2) != 0)
            {
                drawBuffers[drawBufferCount++] = BufferId.COLOR2;
            }
            if ((b & BufferId.COLOR3) != 0)
            {
                drawBuffers[drawBufferCount++] = BufferId.COLOR3;
            }
            readDrawChanged = true;
        }

        /// <summary>
        ///  Returns the current parameters of this framebuffer.
        /// </summary>
        /// <returns>
        /// The parameters.
        /// </returns>
        public Parameters getParameters()
        {
            return parameters;
        }

        /// <summary>
        /// Returns this framebuffer's viewport.
        /// </summary>
        /// <returns>
        /// The viewport.
        /// </returns>
        public Vector4i getViewport()
        {
            return parameters.viewport;
        }


        /// <summary>
        /// Returns this framebuffer's depth range.
        /// </summary>
        /// <returns>
        /// The depth range.
        /// </returns>
        public Vector2f getDepthRange()
        {
            return parameters.depthRange;
        }


        /// <summary>
        /// Returns this framebuffer's clip distances mask.
        /// </summary>
        /// <returns>
        /// The clip distances.
        /// </returns>
        public int getClipDistances()
        {
            return parameters.clipDistances;
        }

        /// <summary>
        /// Returns the valueC used to clear the color buffer.
        /// </summary>
        /// <returns>
        /// The clear color.
        /// </returns>
        public Vector4f getClearColor()
        {
            return parameters.clearColor;
        }


        /// <summary>
        ///  Returns the valueC used to clear the depth buffer.
        /// </summary>
        /// <returns>
        /// The clear depth.
        /// </returns>
        public double getClearDepth()
        {
            return parameters.clearDepth;
        }


        /// <summary>
        /// Returns the valueC used to clear the stencil buffer.
        /// </summary>
        /// <returns>
        /// The clear stencil.
        /// </returns>
        public int getClearStencil()
        {
            return parameters.clearStencil;
        }


        /// <summary>
        /// Returns the point primitive's size.
        /// </summary>
        /// <returns>
        /// The point size.
        /// </returns>
        public float getPointSize()
        {
            return parameters.pointSize;
        }

        /// <summary>
        /// Returns the point's fade threshold size
        /// </summary>
        /// <returns>
        /// The point fade threshold size.
        /// </returns>
        public float getPointFadeThresholdSize()
        {
            return parameters.pointFadeThresholdSize;
        }


        /// <summary>
        /// Returns the left origin of the point primitive.
        /// </summary>
        /// <returns>
        /// The point lower left origin.
        /// </returns>
        public bool getPointLowerLeftOrigin()
        {
            return parameters.pointLowerLeftOrigin;
        }


        /// <summary>
        /// Returns the line primitive's width.
        /// </summary>
        /// <returns>
        /// The line width.
        /// </returns>
        public float getLineWidth()
        {
            return parameters.lineWidth;
        }


        /// <summary>
        /// Retursn true if AA is enabled for lines.
        /// </summary>
        /// <returns>
        /// The line smooth.
        /// </returns>
        public bool getLineSmooth()
        {
            return parameters.lineSmooth;
        }


        /// <summary>
        /// Returns true if front faces are clockwise.
        /// </summary>
        /// <returns>
        /// The front face C.
        /// </returns>
        public bool getFrontFaceCW()
        {
            return parameters.frontFaceCW;
        }

        /// <summary>
        /// Returns the polygon mode for front and back faces cull state.
        /// </summary>
        /// <returns>
        /// The polygon mode.
        /// </returns>
        public Vec2<PolygonMode> getPolygonMode()
        {
            return new Vec2<PolygonMode>(parameters.polygonFront, parameters.polygonBack);
        }


        /// <summary>
        /// Returns true if AA is enabled for polygons.
        /// </summary>
        /// <returns>
        /// The polygon smooth.
        /// </returns>
        public bool getPolygonSmooth()
        {
            return parameters.polygonSmooth;
        }


        /// <summary>
        /// Returns factor and units used to offset the depth valueC.
        /// </summary>
        /// <returns>
        /// The polygon offset.
        /// </returns>
        public Vector2f getPolygonOffset()
        {
            return parameters.polygonOffset;
        }


        /// <summary>
        /// Returns the types of primitives that must be offset (points, lines, and polygons).
        /// </summary>
        /// <returns>
        /// The polygon offsets.
        /// </returns>
        public Vector3b getPolygonOffsets()
        {
            return parameters.polygonOffsets;
        }


        /// <summary>
        /// Returns true if multisampling is enabled.
        /// </summary>
        /// <returns>
        /// The multi sample.
        /// </returns>
        public bool getMultiSample()
        {
            return parameters.multiSample;
        }


        /// <summary>
        /// Returns the values for alpha sampling.
        /// </summary>
        /// <returns>
        /// The sample alpha.
        /// </returns>
        public Vector2b getSampleAlpha()
        {
            return new Vector2b(parameters.sampleAlphaToCoverage, parameters.sampleAlphaToOne);
        }


        /// <summary>
        /// Returns the sample coverage.
        /// </summary>
        /// <returns>
        /// The sample coverage.
        /// </returns>
        public float getSampleCoverage()
        {
            return parameters.sampleCoverage;
        }


        /// <summary>
        /// Returns the sample mask.
        /// </summary>
        /// <returns>
        /// The sample mask.
        /// </returns>
        public uint getSampleMask()
        {
            return parameters.sampleMask;
        }


        /// <summary>
        ///  Returns true if AA is enabled on multi-sampling.
        /// </summary>
        /// <returns>
        /// The sample shading.
        /// </returns>
        /// <param name='minSamples'>
        /// Minimum samples.
        /// </param>
        public bool getSampleShading(out float minSamples)
        {
            minSamples = parameters.samplesMin;
            return parameters.sampleShading;
        }


        /// <summary>
        /// Returns the occlusion test and its mode.
        /// </summary>
        /// <returns>
        /// The occlusion test.
        /// </returns>
        /// <param name='occlusionMode'>
        /// Occlusion mode.
        /// </param>
        public Query getOcclusionTest(out QueryMode occlusionMode)
        {
            occlusionMode = parameters.occlusionMode;
            return parameters.occlusionQuery;
        }

        /// <summary>
        /// Returns True if Scissor test is enabled.
        /// </summary>
        /// <returns>
        /// The scissor test.
        /// </returns>
        public bool getScissorTest()
        {
            return parameters.enableScissor;
        }

        /// <summary>
        /// Returns True if Scissor test is enabled.
        /// </summary>
        /// <returns>
        /// The scissor test.
        /// </returns>
        /// <param name='scissor'>
        /// Scissor the current scissor test viewport.
        /// </param>
        public bool getScissorTest(out Vector4i scissor)
        {
            scissor = parameters.scissor;
            return parameters.enableScissor;
        }


        /// <summary>
        /// Returns true if stencil test is enabled.
        /// </summary>
        /// <returns>
        /// The stencil test.
        /// </returns>
        public bool getStencilTest()
        {
            return parameters.enableStencil;
        }

        /// <summary>
        /// Returns true if stencil test is enabled.
        /// </summary>
        /// <returns>
        /// The stencil test.
        /// </returns>
        /// <param name='ff'>
        /// Ff the current front face function.
        /// </param>
        /// <param name='fref'>
        /// Fref e current front face reference valueC.
        /// </param>
        /// <param name='fmask'>
        /// Fmask the current front face stencil mask.
        /// </param>
        /// <param name='ffail'>
        /// Ffail the current stencil operation used when failing stencil test on front faces.
        /// </param>
        /// <param name='fdpfail'>
        /// Fdpfail the current stencil operation used when passing stencil test but failing depth test on front faces.
        /// </param>
        /// <param name='fdppass'>
        /// Fdppass the current stencil operation used when passing both stencil and depth tests on front faces.
        /// </param>
        public bool getStencilTest(out Function ff, out int fref, out uint fmask, out StencilOperation ffail,
                                   out StencilOperation fdpfail, out StencilOperation fdppass)
        {
            ff = parameters.ffunc;
            fref = parameters.fref;
            fmask = parameters.fmask;
            ffail = parameters.ffail;
            fdpfail = parameters.fdpfail;
            fdppass = parameters.fdppass;

            return parameters.enableStencil;
        }


        /// <summary>
        /// Returns true if stencil test is enabled.
        /// </summary>
        /// <returns>
        /// The stencil test.
        /// </returns>
        /// <param name='ff'>
        /// Ff the current front face function.
        /// </param>
        /// <param name='fref'>
        /// Fref the current front face reference valueC.
        /// </param>
        /// <param name='fmask'>
        /// Fmask the current front face stencil mask.
        /// </param>
        /// <param name='ffail'>
        /// Ffail the current stencil operation used when failing stencil test on front faces.
        /// </param>
        /// <param name='fdpfail'>
        /// Fdpfail the current stencil operation used when passing stencil test but failing depth test on front faces.
        /// </param>
        /// <param name='fdppass'>
        /// Fdppass the current stencil operation used when passing both stencil and depth tests on front faces.
        /// </param>
        /// <param name='bf'>
        /// Bf the current back face function.
        /// </param>
        /// <param name='bref'>
        /// Bref the current back face reference valueC.
        /// </param>
        /// <param name='bmask'>
        /// Bmask the current back face stencil mask.
        /// </param>
        /// <param name='bfail'>
        /// Bfail  the current stencil operation used when failing stencil test on back faces.
        /// </param>
        /// <param name='bdpfail'>
        /// Bdpfail the current stencil operation used when passing stencil test but failing depth test on back faces.
        /// </param>
        /// <param name='bdppass'>
        /// Bdppass the current stencil operation used when passing both stencil and depth tests on back faces.
        /// </param>
        public bool getStencilTest(out Function ff, out int fref, out uint fmask, out StencilOperation ffail,
                                   out StencilOperation fdpfail, out StencilOperation fdppass,
                                   out Function bf, out int bref, out uint bmask, out StencilOperation bfail,
                                   out StencilOperation bdpfail, out StencilOperation bdppass)
        {
            ff = parameters.ffunc;
            fref = parameters.fref;
            fmask = parameters.fmask;
            ffail = parameters.ffail;
            fdpfail = parameters.fdpfail;
            fdppass = parameters.fdppass;
            bf = parameters.bfunc;
            bref = parameters.bref;
            bmask = parameters.bmask;
            bfail = parameters.bfail;
            bdpfail = parameters.bdpfail;
            bdppass = parameters.bdppass;

            return parameters.enableStencil;
        }

        /// <summary>
        /// Returns true if depth test is enabled.
        /// </summary>
        /// <returns>
        /// The depth test.
        /// </returns>
        public bool getDepthTest()
        {
            return parameters.enableDepth;
        }


        /// <summary>
        /// Returns true if depth test is enabled.
        /// </summary>
        /// <returns>
        /// The depth test.
        /// </returns>
        /// <param nhe current depth test function.ame='depth'>
        /// Depth 
        /// </param>
        public bool getDepthTest(out Function depth)
        {
            depth = parameters.depth;
            return parameters.enableDepth;
        }

        /// <summary>
        /// Returns true if blending is enabled for specified buffer.
        /// If no buffer is specified, will use default buffer.
        /// </summary>
        /// <returns>
        /// The blend.
        /// </returns>
        /// <param name='buffer'>
        /// Buffer  an optionnal buffer id.
        /// </param>
        public bool getBlend(BufferId buffer = BufferId.NOVALUE)
        {
            int id = buffer < 0 ? 0 : EnumConversion.getBufferId(buffer);
            return parameters.enableBlend[id];
        }


        /// <summary>
        /// Returns true if blending is enabled for specified buffer.
        /// If no buffer is specified, will use default buffer.
        /// </summary>
        /// <returns>
        /// The blend.
        /// </returns>
        /// <param name='buffer'>
        /// Buffer an optionnal buffer id.
        /// </param>
        /// <param name='rgb'>
        /// Rgb the current color blending equation.
        /// </param>
        /// <param name='srgb'>
        /// Srgb the current source color blending argument.
        /// </param>
        /// <param name='drgb'>
        /// Drgb the current destination color blending argument.
        /// </param>
        public bool getBlend(BufferId buffer, out BlendEquation rgb, out BlendArgument srgb, out BlendArgument drgb)
        {
            int id = buffer < 0 ? 0 : EnumConversion.getBufferId(buffer);
            rgb = parameters.rgb[id];
            srgb = parameters.srgb[id];
            drgb = parameters.drgb[id];
            return parameters.enableBlend[id];
        }


        /// <summary>
        /// Returns true if blending is enabled for specified buffer.
        /// If no buffer is specified, will use default buffer.
        /// </summary>
        /// <returns>
        /// The blend.
        /// </returns>
        /// <param name='buffer'>
        /// Buffer an optionnal buffer id.
        /// </param>
        /// <param name='rgb'>
        /// Rgb the current color blending equation.
        /// </param>
        /// <param name='srgb'>
        /// Srgb the current source color blending argument.
        /// </param>
        /// <param name='drgb'>
        /// Drgb the current destination color blending argument.
        /// </param>
        /// <param name='alpha'>
        /// Alpha the current alpha blending equation.
        /// </param>
        /// <param name='salpha'>
        /// Salpha the current source alpha blending argument.
        /// </param>
        /// <param name='dalpha'>
        /// Dalpha the current destination alpha blending argument.
        /// </param>
        public bool getBlend(BufferId buffer, out BlendEquation rgb, out BlendArgument srgb, out BlendArgument drgb,
                                        out BlendEquation alpha, out BlendArgument salpha, out BlendArgument dalpha)
        {
            int id = buffer < 0 ? 0 : EnumConversion.getBufferId(buffer);
            rgb = parameters.rgb[id];
            srgb = parameters.srgb[id];
            drgb = parameters.drgb[id];
            alpha = parameters.alpha[id];
            salpha = parameters.salpha[id];
            dalpha = parameters.dalpha[id];
            return parameters.enableBlend[id];
        }

        /// <summary>
        /// Returns the current blending color parameter.
        /// </summary>
        /// <returns>
        /// The blend color.
        /// </returns>
        public Vector4f getBlendColor()
        {
            return parameters.color;
        }


        /// <summary>
        /// Returns true if dithering is enabled.
        /// </summary>
        /// <returns>
        /// The dither.
        /// </returns>
        public bool getDither()
        {
            return parameters.enableDither;
        }


        /// <summary>
        /// Returns true if logical operation is enabled.
        /// </summary>
        /// <returns>
        /// The logic op.
        /// </returns>
        public bool getLogicOp()
        {
            return parameters.enableLogic;
        }


        /// <summary>
        /// Returns true if logical operation is enabled.
        /// </summary>
        /// <returns>
        /// The logic op.
        /// </returns>
        /// <param name='logicOp'>
        /// Logic op the current logical operation.
        /// </param>
        public bool getLogicOp(out LogicOperation logicOp)
        {
            logicOp = parameters.logicOp;
            return parameters.enableLogic;
        }


        /// <summary>
        /// Returns the color writing mask for the given buffer.
        /// </summary>
        /// <returns>
        /// The color mask.
        /// </returns>
        /// <param name='buffer'>
        /// Buffer.
        /// </param>
        public Vector4b getColorMask(BufferId buffer = BufferId.NOVALUE)
        {
            return parameters.colorMask[EnumConversion.getBufferId(buffer)];
        }


        /// <summary>
        /// Returns the depth buffer writing mask.
        /// </summary>
        /// <returns>
        /// The depth mask.
        /// </returns>
        public bool getDepthMask()
        {
            return parameters.depthMask;
        }


        /// <summary>
        /// Returns the stencil buffer writing mask.
        /// </summary>
        /// <returns>
        /// The stencil mask.
        /// </returns>
        public Vector2i getStencilMask()
        {
            return new Vector2i((int)parameters.stencilMaskFront, (int)parameters.stencilMaskBack);
        }


        /// <summary>
        /// Sets all the parameters of this framebuffer at once.
        /// </summary>
        /// <returns>
        /// The parameters.
        /// </returns>
        /// <param name='p'>
        /// P a set of framebuffer parameters obtained via #getParameters().
        /// </param>
        public void setParameters(Parameters p)
        {
            parameters = p;
            parametersChanged = true;
        }

        /// <summary>
        /// Sets the viewport for this framebuffer (up, down, left and right planes).
        /// </summary>
        /// <returns>
        /// The viewport.
        /// </returns>
        /// <param name='viewport'>
        /// Viewport the new viewport.
        /// </param>
        public void setViewport(Vector4i viewport)
        {
            parameters.viewport = viewport;
            parameters.transformId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Sets the depth range for this framebuffer (near and far planes).
        /// </summary>
        /// <returns>
        /// The depth range.
        /// </returns>
        /// <param name='n'>
        /// N  near plane.
        /// </param>
        /// <param name='f'>
        /// F  far plane.
        /// </param>
        public void setDepthRange(float n, float f)
        {
            parameters.depthRange = new Vector2f(n, f);
            parameters.transformId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Sets the clipping bit, used to determine which planes will be used for clipping.
        /// </summary>
        /// <returns>
        /// The clip distances.
        /// </returns>
        /// <param name='clipDistances'>
        /// Clip distances.
        /// </param>
        public void setClipDistances(int clipDistances)
        {
            parameters.clipDistances = clipDistances;
            parameters.transformId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Sets the color used to clear the current draw buffer.
        /// </summary>
        /// <returns>
        /// The clear color.
        /// </returns>
        /// <param name='clearColor'>
        /// Clear color.
        /// </param>
        public void setClearColor(Vector4f clearColor)
        {
            parameters.clearColor = clearColor;
            parameters.clearId = ++PARAMETER_ID;
            parametersChanged = true;
        }
        public void setClearColor(System.Drawing.Color color)
        {
            setClearColor(new Vector4f(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f));
        }


        /// <summary>
        /// Sets the depth used to clear the current depth buffer.
        /// </summary>
        /// <returns>
        /// The clear depth.
        /// </returns>
        /// <param name='clearDepth'>
        /// Clear depth.
        /// </param>
        public void setClearDepth(float clearDepth)
        {
            parameters.clearDepth = clearDepth;
            parameters.clearId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Sets the stencil used to clear the current stencil buffer.
        /// </summary>
        /// <returns>
        /// The clear stencil.
        /// </returns>
        /// <param name='clearStencil'>
        /// Clear stencil.
        /// </param>
        public void setClearStencil(int clearStencil)
        {
            parameters.clearStencil = clearStencil;
            parameters.clearId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Sets the point primitive's size.
        /// </summary>
        /// <returns>
        /// The point size.
        /// </returns>
        /// <param name='pointSize'>
        /// Point size.
        /// </param>
        public void setPointSize(float pointSize)
        {
            parameters.pointSize = pointSize;
            parameters.pointId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Sets the maximum size of a point.
        /// </summary>
        /// <returns>
        /// The point fade threshold size.
        /// </returns>
        /// <param name='pointFadeThresholdSize'>
        /// Point fade threshold size.
        /// </param>
        public void setPointFadeThresholdSize(float pointFadeThresholdSize)
        {
            parameters.pointFadeThresholdSize = pointFadeThresholdSize;
            parameters.pointId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Sets the origin corner of a point.
        /// </summary>
        /// <returns>
        /// The point lower left origin.
        /// </returns>
        /// <param name='pointLowerLeftOrigin'>
        /// Point lower left origin.
        /// </param>
        public void setPointLowerLeftOrigin(bool pointLowerLeftOrigin)
        {
            parameters.pointLowerLeftOrigin = pointLowerLeftOrigin;
            parameters.pointId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Sets the line primitive's width.
        /// </summary>
        /// <returns>
        /// The line width.
        /// </returns>
        /// <param name='lineWidth'>
        /// Line width.
        /// </param>
        public void setLineWidth(float lineWidth)
        {
            parameters.lineId = ++PARAMETER_ID;
            parameters.lineWidth = lineWidth;
            parametersChanged = true;
        }

        /// <summary>
        ///  Enables or disables the AA on line drawing.
        /// </summary>
        /// <returns>
        /// The line smooth.
        /// </returns>
        /// <param name='lineSmooth'>
        /// Line smooth.
        /// </param>
        public void setLineSmooth(bool lineSmooth)
        {
            parameters.lineId = ++PARAMETER_ID;
            parameters.lineSmooth = lineSmooth;
            parametersChanged = true;
        }


        /// <summary>
        /// termines the orientation of front faces.
        /// </summary>
        /// <returns>
        /// The front face C.
        /// </returns>
        /// <param name='frontFaceCW'>
        /// Front face C if true, clockwise faces will be front faces.
        /// </param>
        public void setFrontFaceCW(bool frontFaceCW)
        {
            parameters.frontFaceCW = frontFaceCW;
            parameters.polygonId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Sets the polygon mode for front and back faces.
        /// </summary>
        /// <returns>
        /// The polygon mode.
        /// </returns>
        /// <param name='polygonFront'>
        /// Polygon front drawing mode for front faces.
        /// </param>
        /// <param name='polygonBack'>
        /// Polygon back drawing mode for back faces.
        /// </param>
        public void setPolygonMode(PolygonMode polygonFront, PolygonMode polygonBack)
        {
            parameters.polygonFront = polygonFront;
            parameters.polygonBack = polygonBack;
            parameters.polygonId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Enables or disables AA on polygon drawing.
        /// </summary>
        /// <returns>
        /// The polygon smooth.
        /// </returns>
        /// <param name='polygonSmooth'>
        /// Polygon smooth.
        /// </param>
        public void setPolygonSmooth(bool polygonSmooth)
        {
            parameters.polygonSmooth = polygonSmooth;
            parameters.polygonId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Sets the factor and units when using offsets on primitives.
        /// </summary>
        /// <returns>
        /// The polygon offset.
        /// </returns>
        /// <param name='factor'>
        /// Factor.
        /// </param>
        /// <param name='units'>
        /// Units.
        /// </param>
        public void setPolygonOffset(float factor, float units)
        {
            parameters.polygonOffset = new Vector2f(factor, units);
            parameters.polygonId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Determines which primitives should be offset.
        /// </summary>
        /// <returns>
        /// The polygon offset.
        /// </returns>
        /// <param name='pointOffset'>
        /// Point offset.
        /// </param>
        /// <param name='lineOffset'>
        /// Line offset.
        /// </param>
        /// <param name='polygonOffset'>
        /// Polygon offset.
        /// </param>
        public void setPolygonOffset(bool pointOffset, bool lineOffset, bool polygonOffset)
        {
            parameters.polygonOffsets = new Vector3b(pointOffset, lineOffset, polygonOffset);
            parameters.polygonId = ++PARAMETER_ID;
            parametersChanged = true;
        }

        /// <summary>
        /// Enables or disables multisampling.
        /// </summary>
        /// <returns>
        /// The multisample.
        /// </returns>
        /// <param name='multiSample'>
        /// Multi sample.
        /// </param>
        public void setMultisample(bool multiSample)
        {
            parameters.multiSample = multiSample;
            parameters.multiSampleId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Sets sampling filter options.
        /// </summary>
        /// <returns>
        /// The sample alpha.
        /// </returns>
        /// <param name='sampleAlphaToCoverage'>
        /// Sample alpha to coverage.
        /// </param>
        /// <param name='sampleAlphaToOne'>
        /// Sample alpha to one.
        /// </param>
        public void setSampleAlpha(bool sampleAlphaToCoverage, bool sampleAlphaToOne)
        {
            parameters.sampleAlphaToCoverage = sampleAlphaToCoverage;
            parameters.sampleAlphaToOne = sampleAlphaToOne;
            parameters.multiSampleId = ++PARAMETER_ID;
            parametersChanged = true;
        }

        /// <summary>
        /// Sets sampling coverage.
        /// </summary>
        /// <returns>
        /// The sample coverage.
        /// </returns>
        /// <param name='sampleCoverage'>
        /// Sample coverage.
        /// </param>
        public void setSampleCoverage(float sampleCoverage)
        {
            parameters.sampleCoverage = sampleCoverage;
            parameters.multiSampleId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Sets sampling mask.
        /// </summary>
        /// <returns>
        /// The sample mask.
        /// </returns>
        /// <param name='sampleMask'>
        /// Sample mask.
        /// </param>
        public void setSampleMask(uint sampleMask)
        {
            parameters.sampleMask = sampleMask;
            parameters.multiSampleId = ++PARAMETER_ID;
            parametersChanged = true;
        }

        /// <summary>
        /// Enables or disables AA on multisampling.
        /// </summary>
        /// <returns>
        /// The sample shading.
        /// </returns>
        /// <param name='sampleShading'>
        /// Sample shading.
        /// </param>
        /// <param name='minSamples'>
        /// Minimum samples.
        /// </param>
        public void setSampleShading(bool sampleShading, float minSamples)
        {
            parameters.sampleShading = sampleShading;
            parameters.samplesMin = minSamples;
            parameters.multiSampleId = ++PARAMETER_ID;
            parametersChanged = true;
        }

        /// <summary>
        /// Adds an occlusion query.
        /// </summary>
        /// <returns>
        /// The occlusion test.
        /// </returns>
        /// <param name='occlusionQuery'>
        /// Occlusion query.
        /// </param>
        /// <param name='occlusionMode'>
        /// Occlusion mode.
        /// </param>
        public void setOcclusionTest(Query occlusionQuery, QueryMode occlusionMode)
        {
            Debug.Assert(occlusionQuery == null ||
                occlusionQuery.getType() == QueryType.SAMPLES_PASSED ||
                occlusionQuery.getType() == QueryType.ANY_SAMPLES_PASSED);
            parameters.occlusionQuery = occlusionQuery;
            parameters.occlusionMode = occlusionMode;
            parametersChanged = true;
        }


        /// <summary>
        /// Enables or disables scissor test.
        /// </summary>
        /// <returns>
        /// The scissor test.
        /// </returns>
        /// <param name='enableScissor'>
        /// Enable scissor.
        /// </param>
        public void setScissorTest(bool enableScissor)
        {
            parameters.enableScissor = enableScissor;
            parametersChanged = true;
        }


        /// <summary>
        /// Enables or disables scissor test.
        /// </summary>
        /// <returns>
        /// The scissor test.
        /// </returns>
        /// <param name='enableScissor'>
        /// Enable scissor.
        /// </param>
        /// <param name='scissor'>
        /// Scissor.
        /// </param>
        public void setScissorTest(bool enableScissor, Vector4i scissor)
        {
            parameters.enableScissor = enableScissor;
            parameters.scissor = scissor;
            parametersChanged = true;
        }


        /// <summary>
        /// Enables or disables stencil test.
        /// </summary>
        /// <returns>
        /// The stencil test.
        /// </returns>
        /// <param name='enableStencil'>
        /// Enable stencil true to enable the stencil test.
        /// </param>
        public void setStencilTest(bool enableStencil)
        {
            parameters.enableStencil = enableStencil;
            parameters.stencilId = ++PARAMETER_ID;
            parametersChanged = true;
        }

        /// <summary>
        /// Enables or disables stencil test.
        /// </summary>
        /// <returns>
        /// The stencil test.
        /// </returns>
        /// <param name='enableStencil'>
        /// Enable stencil. enableStencil true to enable the stencil test.
        /// </param>
        /// <param name='f'>
        /// F. the front and back face function.
        /// </param>
        /// <param name='ref_'>
        /// Ref_. the front and back face reference valueC.
        /// </param>
        /// <param name='mask'>
        /// Mask the front and back face stencil mask.
        /// </param>
        /// <param name='sfail'>
        /// Sfail the stencil operation used when failing stencil test on front or back faces.
        /// </param>
        /// <param name='dpfail'>
        /// Dpfail the stencil operation used when passing stencil test but failing depth test on front or back faces.
        /// </param>
        /// <param name='dppass'>
        /// Dppass the stencil operation used when passing both stencil and depth tests on front or back faces.
        /// </param>
        public void setStencilTest(bool enableStencil,
             Function f, int ref_, uint mask, StencilOperation sfail, StencilOperation dpfail, StencilOperation dppass)
        {
            parameters.enableStencil = enableStencil;
            parameters.ffunc = f;
            parameters.fref = ref_;
            parameters.fmask = mask;
            parameters.ffail = sfail;
            parameters.fdpfail = dpfail;
            parameters.fdppass = dppass;
            parameters.bfunc = f;
            parameters.bref = ref_;
            parameters.bmask = mask;
            parameters.bfail = sfail;
            parameters.bdpfail = dpfail;
            parameters.bdppass = dppass;
            parameters.stencilId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Enables or disables stencil test.
        /// </summary>
        /// <returns>
        /// The stencil test.
        /// </returns>
        /// <param name='enableStencil'>
        /// Enable stencil true to enable the stencil test.
        /// </param>
        /// <param name='ff'>
        /// Ff the front face function.
        /// </param>
        /// <param name='fref'>
        /// Fref the front face reference valueC.
        /// </param>
        /// <param name='fmask'>
        /// Fmask the front face stencil mask.
        /// </param>
        /// <param name='ffail'>
        /// Ffail the stencil operation used when failing stencil test on front faces.
        /// </param>
        /// <param name='fdpfail'>
        /// Fdpfail the stencil operation used when passing stencil test but failing depth test on front faces.
        /// </param>
        /// <param name='fdppass'>
        /// Fdppass the stencil operation used when passing both stencil and depth tests on front faces.
        /// </param>
        /// <param name='bf'>
        /// Bf the back face function.
        /// </param>
        /// <param name='bref'>
        /// Bref the back face reference valueC.
        /// </param>
        /// <param name='bmask'>
        /// Bmask the back face stencil mask.
        /// </param>
        /// <param name='bfail'>
        /// Bfail the stencil operation used when failing stencil test on back faces.
        /// </param>
        /// <param name='bdpfail'>
        /// Bdpfail the stencil operation used when passing stencil test but failing depth test on back faces.
        /// </param>
        /// <param name='bdppass'>
        /// Bdppass the stencil operation used when passing both stencil and depth tests on back faces.
        /// </param>
        public void setStencilTest(bool enableStencil,
            Function ff, int fref, uint fmask, StencilOperation ffail, StencilOperation fdpfail, StencilOperation fdppass,
            Function bf, int bref, uint bmask, StencilOperation bfail, StencilOperation bdpfail, StencilOperation bdppass)
        {
            parameters.enableStencil = enableStencil;
            parameters.ffunc = ff;
            parameters.fref = fref;
            parameters.fmask = fmask;
            parameters.ffail = ffail;
            parameters.fdpfail = fdpfail;
            parameters.fdppass = fdppass;
            parameters.bfunc = bf;
            parameters.bref = bref;
            parameters.bmask = bmask;
            parameters.bfail = bfail;
            parameters.bdpfail = bdpfail;
            parameters.bdppass = bdppass;
            parameters.stencilId = ++PARAMETER_ID;
            parametersChanged = true;
        }

        /// <summary>
        /// Enables or disables depth test.
        /// </summary>
        /// <returns>
        /// The depth test.
        /// </returns>
        /// <param name='enableDepth'>
        /// Enable depth.
        /// </param>
        public void setDepthTest(bool enableDepth)
        {
            parameters.enableDepth = enableDepth;
            parameters.depthId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Enables or disables depth test.
        /// </summary>
        /// <returns>
        /// The depth test.
        /// </returns>
        /// <param name='enableDepth'>
        /// Enable depth.
        /// </param>
        /// <param name='depth'>
        /// Depth.
        /// </param>
        public void setDepthTest(bool enableDepth, Function depth)
        {
            parameters.enableDepth = enableDepth;
            parameters.depth = depth;
            parameters.depthId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Enables or disables blending.
        /// </summary>
        /// <returns>
        /// The blend.
        /// </returns>
        /// <param name='enableBlend'>
        /// Enable blend.
        /// </param>
        public void setBlend(bool enableBlend)
        {
            parameters.multiBlendEnable = false;
            parameters.enableBlend[0] = enableBlend;
            parameters.blendId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Enables or disables blending for a given buffer.
        /// </summary>
        /// <returns>
        /// The blend.
        /// </returns>
        /// <param name='buffer'>
        /// Buffer.
        /// </param>
        /// <param name='enableBlend'>
        /// Enable blend.
        /// </param>
        public void setBlend(BufferId buffer, bool enableBlend)
        {
            parameters.multiBlendEnable = true;
            parameters.enableBlend[EnumConversion.getBufferId(buffer)] = enableBlend;
            parameters.blendId = ++PARAMETER_ID;
            parametersChanged = true;
        }

        /// <summary>
        /// Enables or disables blending.
        /// </summary>
        /// <returns>
        /// enableBlend 
        /// </returns>
        /// <param name='enableBlend'>
        /// Enable blend true to enable blending.
        /// </param>
        /// <param name='e'>
        /// E the color and alpha blending equation.
        /// </param>
        /// <param name='src'>
        /// Source the source color and alpha blending argument.
        /// </param>
        /// <param name='dst'>
        /// Dst the destination color and alpha blending argument.
        /// </param>
        public void setBlend(bool enableBlend,
             BlendEquation e, BlendArgument src, BlendArgument dst)
        {
            parameters.multiBlendEnable = false;
            parameters.multiBlendEq = false;
            parameters.enableBlend[0] = enableBlend;
            parameters.rgb[0] = e;
            parameters.srgb[0] = src;
            parameters.drgb[0] = dst;
            parameters.alpha[0] = e;
            parameters.salpha[0] = src;
            parameters.dalpha[0] = dst;
            parameters.blendId = ++PARAMETER_ID;
            parametersChanged = true;
        }

        /// <summary>
        /// Enables or disables blending for a given buffer.
        /// </summary>
        /// <returns>
        /// The blend.
        /// </returns>
        /// <param name='buffer'>
        /// Buffer  the buffer whose blending options must be changed.
        /// </param>
        /// <param name='enableBlend'>
        /// Enable blend true to enable blending.
        /// </param>
        /// <param name='e'>
        /// E the color and alpha blending equation.
        /// </param>
        /// <param name='src'>
        /// Source  the source color and alpha blending argument.
        /// </param>
        /// <param name='dst'>
        /// Dst the destination color and alpha blending argument.
        /// </param>
        public void setBlend(BufferId buffer, bool enableBlend,
             BlendEquation e, BlendArgument src, BlendArgument dst)
        {
            int b = EnumConversion.getBufferId(buffer);
            parameters.multiBlendEnable = false;
            parameters.multiBlendEq = false;
            parameters.enableBlend[b] = enableBlend;
            parameters.rgb[b] = e;
            parameters.srgb[b] = src;
            parameters.drgb[b] = dst;
            parameters.alpha[b] = e;
            parameters.salpha[b] = src;
            parameters.dalpha[b] = dst;
            parameters.blendId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Enables or disables blending.
        /// </summary>
        /// <returns>
        /// The blend.
        /// </returns>
        /// <param name='enableBlend'>
        /// Enable blend true to enable blending.
        /// </param>
        /// <param name='rgb'>
        /// Rgb the color blending equation.
        /// </param>
        /// <param name='srgb'>
        /// Srgb the source color blending argument.
        /// </param>
        /// <param name='drgb'>
        /// Drgb the destination color blending argument.
        /// </param>
        /// <param name='alpha'>
        /// Alpha the alpha blending equation.
        /// </param>
        /// <param name='salpha'>
        /// Salpha the source alpha blending argument.
        /// </param>
        /// <param name='dalpha'>
        /// Dalpha the destination alpha blending argument.
        /// </param>
        public void setBlend(bool enableBlend,
                             BlendEquation rgb, BlendArgument srgb, BlendArgument drgb,
                             BlendEquation alpha, BlendArgument salpha, BlendArgument dalpha)
        {
            parameters.multiBlendEnable = false;
            parameters.multiBlendEq = false;
            parameters.enableBlend[0] = enableBlend;
            parameters.rgb[0] = rgb;
            parameters.srgb[0] = srgb;
            parameters.drgb[0] = drgb;
            parameters.alpha[0] = alpha;
            parameters.salpha[0] = salpha;
            parameters.dalpha[0] = dalpha;
            parameters.blendId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Enables or disables blending for a given buffer.
        /// </summary>
        /// <returns>
        /// The blend.
        /// </returns>
        /// <param name='buffer'>
        /// Buffer the buffer whose blending options must be changed.
        /// </param>
        /// <param name='enableBlend'>
        /// Enable blend  enableBlend true to enable blending.
        /// </param>
        /// <param name='rgb'>
        /// Rgb he color blending equation.
        /// </param>
        /// <param name='srgb'>
        /// Srgb the source color blending argument.
        /// </param>
        /// <param name='drgb'>
        /// Drgb the destination color blending argument.
        /// </param>
        /// <param name='alpha'>
        /// Alpha  the alpha blending equation.
        /// </param>
        /// <param name='salpha'>
        /// Salpha the source alpha blending argument.
        /// </param>
        /// <param name='dalpha'>
        /// Dalpha  the destination alpha blending argument.
        /// </param>
        public void setBlend(BufferId buffer, bool enableBlend,
             BlendEquation rgb, BlendArgument srgb, BlendArgument drgb,
             BlendEquation alpha, BlendArgument salpha, BlendArgument dalpha)
        {
            int b = EnumConversion.getBufferId(buffer);
            parameters.multiBlendEnable = true;
            parameters.multiBlendEq = true;
            parameters.enableBlend[b] = enableBlend;
            parameters.rgb[b] = rgb;
            parameters.srgb[b] = srgb;
            parameters.drgb[b] = drgb;
            parameters.alpha[b] = alpha;
            parameters.salpha[b] = salpha;
            parameters.dalpha[b] = dalpha;
            parameters.blendId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Sets Blend color parameter.
        /// </summary>
        /// <returns>
        /// The blend color.
        /// </returns>
        /// <param name='color'>
        /// Color.
        /// </param>
        public void setBlendColor(Vector4f color)
        {
            parameters.color = color;
            parameters.blendId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        ///  Enables or disables dithering.
        /// </summary>
        /// <returns>
        /// The dither.
        /// </returns>
        /// <param name='enableDither'>
        /// Enable dither.
        /// </param>
        public void setDither(bool enableDither)
        {
            parameters.enableDither = enableDither;
            parametersChanged = true;
        }

        /// <summary>
        /// Enables or disables logical operation.
        /// </summary>
        /// <returns>
        /// The logic op.
        /// </returns>
        /// <param name='enableLogic'>
        /// Enable logic.
        /// </param>
        public void setLogicOp(bool enableLogic)
        {
            parameters.enableLogic = enableLogic;
            parametersChanged = true;
        }


        /// <summary>
        /// Enables or disables logical operation.
        /// </summary>
        /// <returns>
        /// The logic op.
        /// </returns>
        /// <param name='enableLogic'>
        /// Enable logic.
        /// </param>
        /// <param name='logicOp'>
        /// Logic op.
        /// </param>
        public void setLogicOp(bool enableLogic, LogicOperation logicOp)
        {
            parameters.enableLogic = enableLogic;
            parameters.logicOp = logicOp;
            parametersChanged = true;
        }


        /// <summary>
        /// Sets color buffer's writing mask.
        /// </summary>
        /// <returns>
        /// The color mask.
        /// </returns>
        /// <param name='r'>
        /// R.
        /// </param>
        /// <param name='g'>
        /// G.
        /// </param>
        /// <param name='b'>
        /// B.
        /// </param>
        /// <param name='a'>
        /// A.
        /// </param>
        public void setColorMask(bool r, bool g, bool b, bool a)
        {
            parameters.multiColorMask = false;
            parameters.colorMask[0] = new Vector4b(r, g, b, a);
            parameters.maskId = ++PARAMETER_ID;
            parametersChanged = true;
        }

        /// <summary>
        /// Sets color buffer's writing mask.
        /// </summary>
        /// <returns>
        /// The color mask.
        /// </returns>
        /// <param name='buffer'>
        /// Buffer.
        /// </param>
        /// <param name='r'>
        /// R.
        /// </param>
        /// <param name='g'>
        /// G.
        /// </param>
        /// <param name='b'>
        /// B.
        /// </param>
        /// <param name='a'>
        /// A.
        /// </param>
        public void setColorMask(BufferId buffer, bool r, bool g, bool b, bool a)
        {
            parameters.multiColorMask = true;
            parameters.colorMask[EnumConversion.getBufferId(buffer)] = new Vector4b(r, g, b, a);
            parameters.maskId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Sets depth buffer's writing mask.
        /// </summary>
        /// <returns>
        /// The depth mask.
        /// </returns>
        /// <param name='d'>
        /// D.
        /// </param>
        public void setDepthMask(bool d)
        {
            parameters.depthMask = d;
            parameters.maskId = ++PARAMETER_ID;
            parametersChanged = true;
        }


        /// <summary>
        /// Sets stencil buffer's writing mask.
        /// setStencilMask controls the writing of individual bits in the stencil planes. The least significant n bits of mask, where
        /// n is the number of bits in the stencil buffer, specify a mask. Where a 1 appears in the mask, it's possible to write to the 
        /// corresponding bit in the stencil buffer. Where a 0 appears, the corresponding bit is write-protected. Initially, all bits 
        /// are enabled for writing. 
        /// There can be two separate mask writemasks; one affects back-facing polygons, and the other affects front-facing polygons as 
        /// well as other non-polygon primitives. setStencilMask with one parameter sets both front and back stencil writemasks to the same values.
        /// </summary>
        /// <returns>
        /// The stencil mask.
        /// </returns>
        /// <param name='frontMask'>
        /// Front mask.
        /// </param>
        /// <param name='backMask'>
        /// Back mask.
        /// </param>
        public void setStencilMask(uint frontMask, uint backMask)
        {
            parameters.stencilMaskFront = frontMask;
            parameters.stencilMaskBack = backMask;
            parameters.maskId = ++PARAMETER_ID;
            parametersChanged = true;
        }

        /// <summary>
        /// Sets stencil buffer's writing mask.
        /// setStencilMask controls the writing of individual bits in the stencil planes. The least significant n bits of mask, where
        /// n is the number of bits in the stencil buffer, specify a mask. Where a 1 appears in the mask, it's possible to write to the 
        /// corresponding bit in the stencil buffer. Where a 0 appears, the corresponding bit is write-protected. Initially, all bits 
        /// are enabled for writing. 
        /// There can be two separate mask writemasks; one affects back-facing polygons, and the other affects front-facing polygons as 
        /// well as other non-polygon primitives. setStencilMask with one parameter sets both front and back stencil writemasks to the same values.
        /// </summary>
        /// <returns>
        /// The stencil mask.
        /// </returns>
        /// <param name='frontMask'>
        /// Front mask.
        /// </param>
        /// <param name='backMask'>
        /// Back mask.
        /// </param>
        public void setStencilMask(uint mask)
        {
            parameters.stencilMaskFront = mask;
            parameters.stencilMaskBack = mask;
            parameters.maskId = ++PARAMETER_ID;
            parametersChanged = true;
        }

        /// <summary>
        /// Clear the buffers attached to this framebuffer.
        /// </summary>
        /// <param name='color'>
        /// Color true to clear the attached color buffers.
        /// </param>
        /// <param name='stencil'>
        /// Stencil true to clear the attached stencil buffer.
        /// </param>
        /// <param name='depth'>
        /// Depth true to clear the attached depth buffer.
        /// </param>
        public void clear(bool color, bool stencil, bool depth)
        {
#if DEBUG
            if (log.IsDebugEnabled)
                log.Debug("Clear FrameBuffer");
#endif
            set();
#if OPENTK
            ClearBufferMask buffers = 0;
            if (color)
            {
                buffers |= ClearBufferMask.ColorBufferBit;
            }
            if (stencil)
            {
                buffers |= ClearBufferMask.StencilBufferBit;
            }
            if (depth)
            {
                buffers |= ClearBufferMask.DepthBufferBit;
            }
            beginConditionalRender();
            Debug.Assert(getError() == ErrorCode.NoError);
            GL.Clear(buffers);
            Debug.Assert(getError() == ErrorCode.NoError);
#else
            int buffers = 0;
            if (color)
            {
                buffers |= GL_COLOR_BUFFER_BIT;
            }
            if (stencil)
            {
                buffers |= GL_STENCIL_BUFFER_BIT;
            }
            if (depth)
            {
                buffers |= GL_DEPTH_BUFFER_BIT;
            }
            beginConditionalRender();
            glClear(buffers);
#endif
            endConditionalRender();
            Debug.Assert(getError() == ErrorCode.NoError);
        }


        /// <summary>
        ///Draws the given mesh.
        /// </summary>
        /// <param name='p'>
        /// P the program to use to draw the mesh.
        /// </param>
        /// <param name='mesh'>
        /// Mesh mesh the mesh to draw.
        /// </param>
        /// <param name='primCount'>
        /// Prim count the number of times this mesh must be instanced.
        /// </param>
        /// <typeparam name='vertex'>
        /// The 1st type parameter.
        /// </typeparam>
        /// <typeparam name='index'>
        /// The 2nd type parameter.
        /// </typeparam>
        public void draw<vertex, index>(Program p, Mesh<vertex, index> mesh, int primCount = 1)
            where vertex : struct
            where index : struct
        {
            Debug.Assert(TransformFeedback.TRANSFORM == null);
            set();
            p.set();
            beginConditionalRender();
            mesh.getBuffers().draw(mesh.getMode(), 0, mesh.getIndiceCount() == 0 ? mesh.getVertexCount() : mesh.getIndiceCount(), primCount, 0);
            endConditionalRender();
        }

        /// <summary>
        /// Draw  a part of a mesh one or more times.
        /// </summary>
        /// <param name='p'>
        /// P  the program to use to draw the mesh.
        /// </param>
        /// <param name='mesh'>
        /// Mesh the mesh to draw.
        /// </param>
        /// <param name='m'>
        /// M  how the mesh vertices must be interpreted.
        /// </param>
        /// <param name='first'>
        /// First the first vertex to draw, or the first indice to draw if this mesh has indices.
        /// </param>
        /// <param name='count'>
        /// Count the number of vertices to draw, or the number of indices to draw if this mesh has indices.
        /// </param>
        /// <param name='primCount'>
        /// Prim count the number of times this mesh must be drawn (with  geometry instancing).
        /// </param>
        /// <param name='base_'>
        /// Base the base vertex to use. Only used for meshes with indices.
        /// </param>
        public void draw(Program p, MeshBuffers mesh, MeshMode m, int first, int count, int primCount = 1, int base_ = 0)
        {
            Debug.Assert(TransformFeedback.TRANSFORM == null);
            set();
            p.set();
#if DEBUG
            if (log.IsDebugEnabled)
                log.DebugFormat("Draw Mesh ({0} vertices)", count);
#endif
            beginConditionalRender();
            mesh.draw(m, first, count, primCount, base_);
            endConditionalRender();
        }


        /// <summary>
        /// Draws several parts of a mesh. Each part is specified with a first
        /// and count parameter as in #draw(). These values are passed in arrays
        /// of primCount values.
        /// </summary>
        /// <returns>
        /// The draw.
        /// </returns>
        /// <param name='p'>
        /// P the program to use to draw the mesh.
        /// </param>
        /// <param name='mesh'>
        /// Mesh the mesh to draw.
        /// </param>
        /// <param name='m'>
        /// M how the mesh vertices must be interpreted.
        /// </param>
        /// <param name='firsts'>
        /// Firsts an array of primCount 'first vertex' to draw, or an array of 'first indice' to draw if this mesh has indices.
        /// </param>
        /// <param name='counts'>
        /// Counts an array of number of vertices to draw, or an array of  number of indices to draw if this mesh has indices.
        /// </param>
        /// <param name='primCount'>
        /// Prim count the number of parts of this mesh to draw.
        /// </param>
        /// <param name='bases'>
        /// Bases the base vertices to use. Only used for meshes with indices.
        /// </param>
        public void multiDraw(Program p, MeshBuffers mesh, MeshMode m, int[] firsts, int[] counts, int primCount = 1, int[] bases = null)
        {
            Debug.Assert(TransformFeedback.TRANSFORM == null);
            set();
            p.set();
#if DEBUG
            if (log.IsDebugEnabled)
                log.DebugFormat("MultiDraw ({0} instances)", primCount);
#endif
            beginConditionalRender();
            mesh.multiDraw(m, firsts, counts, primCount, bases);
            endConditionalRender();
        }


        /// <summary>
        /// Draws a part of a mesh one or more times.
        /// Only available with OpenGL 4.0 or more.
        /// </summary>
        /// <returns>
        /// The indirect.
        /// </returns>
        /// <param name='p'>
        /// P the program to use to draw the mesh.
        /// </param>
        /// <param name='mesh'>
        /// Mesh the mesh to draw.
        /// </param>
        /// <param name='m'>
        /// M how the mesh vertices must be interpreted.
        /// </param>
        /// <param name='buf'>
        /// buf a CPU or GPU buffer containing the 'count', 'primCount',
        /// 'first' and 'base' parameters, in this order, followed by '0',
        /// as 32 bit integers.
        /// </param>
        public void drawIndirect(Program p, MeshBuffers mesh, MeshMode m, Buffer buf)
        {
            Debug.Assert(TransformFeedback.TRANSFORM == null);
            set();
            p.set();
#if DEBUG
            if (log.IsDebugEnabled)
                log.DebugFormat("DrawIndirect");
#endif
            beginConditionalRender();
            mesh.drawIndirect(m, buf);
            endConditionalRender();
        }

        /// <summary>
        /// Draws the mesh resulting from a transform feedback session.
        /// Only available with OpenGL 4.0 or more.
        /// </summary>
        /// <returns>
        /// The feedback.
        /// </returns>
        /// <param name='p'>
        /// P the program to use to draw the mesh.
        /// </param>
        /// <param name='m'>
        /// M how the mesh vertices must be interpreted.
        /// </param>
        /// <param name='tfb'>
        /// Tfb a TransformFeedback containing the results of a transform feedback session.
        /// </param>
        /// <param name='stream'>
        /// Stream the stream to draw.
        /// </param>
        public void drawFeedback(Program p, MeshMode m, TransformFeedback tfb, int stream = 0)
        {
            Debug.Assert(TransformFeedback.TRANSFORM == null);
            set();
            p.set();
#if DEBUG
            if (log.IsDebugEnabled)
                log.DebugFormat("DrawFeedBack");
#endif
            beginConditionalRender();
#if OPENTK
            GL.DrawTransformFeedbackStream(EnumConversion.getMeshMode(m), tfb.id, (int)stream);
#else
            glDrawTransformFeedbackStream(getMeshMode(m), tfb.id, stream);
#endif
            endConditionalRender();
        }

        /// <summary>
        /// Draws a quad mesh. This mesh has a position attribute made of four
        /// floats. xy coordinates vary between -1 and 1, while zw coordinates
        /// vary between 0 and 1.
        /// </summary>
        /// <returns>
        /// The quad.
        /// </returns>
        /// <param name='p'>
        /// P.
        /// </param>
        public void drawQuad(Program p)
        {
            if (QUAD == null)
            {
                Mesh<Vector4f, uint> quad = new Mesh<Vector4f, uint>(Vector4f.SizeInBytes, MeshMode.TRIANGLE_STRIP, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 4, AttributeType.A32F, false);
                quad.addVertex(new Vector4f(-1, -1, 0, 1));
                quad.addVertex(new Vector4f(1, -1, 0, 1));
                quad.addVertex(new Vector4f(-1, 1, 0, 1));
                quad.addVertex(new Vector4f(1, 1, 0, 1));
                QUAD = quad;
            }
            draw(p, QUAD);
        }


        /// <summary>
        /// Reads pixels from the attached color buffers into the given buffer.
        /// </summary>
        /// <returns>
        /// The pixels.
        /// </returns>
        /// <param name='x'>
        /// X  lower left corner of the area where the pixels must be read.
        /// </param>
        /// <param name='y'>
        /// Y  lower left corner of the area where the pixels must be read.
        /// </param>
        /// <param name='w'>
        /// W width of the area where the pixels must be read.
        /// </param>
        /// <param name='h'>
        /// H height of the area where the pixels must be read.
        /// </param>
        /// <param name='f'>
        /// F the format of pixels to be stored in dstBuf.
        /// </param>
        /// <param name='t'>
        /// T the type of pixel components in dstBuf.
        /// </param>
        /// <param name='s'>
        /// S optional pixel storage parameters for dstBuf.
        /// </param>
        /// <param name='dstBuf'>
        /// Dst buffer destination buffer for the read pixels.
        /// </param>
        /// <param name='clamp'>
        /// Clamp true to clamp read colors to 0..1.
        /// </param>
        public void readPixels(int x, int y, int w, int h, TextureFormat f, PixelType t, Buffer.Parameters s, Buffer dstBuf, bool clamp = false)
        {
#if DEBUG
            if (log.IsDebugEnabled)
                log.DebugFormat("read {0} pixels", w * h);
#endif


            set();
            dstBuf.bind(BufferTarget.PixelPackBuffer);
            s.set();
#if OPENTK
            GL.ClampColor(ClampColorTarget.ClampReadColor, clamp ? ClampColorMode.True : ClampColorMode.False);
            GL.ReadPixels(x, y, w, h, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), (IntPtr)dstBuf.data(0));
#else
            glClampColor(GL_CLAMP_READ_COLOR, clamp ? GL_TRUE : GL_FALSE);
            glReadPixels(x, y, w, h, getTextureFormat(f), getPixelType(t), dstBuf.data(0));
#endif
            s.unset();
            dstBuf.unbind(BufferTarget.PixelPackBuffer);
            Debug.Assert(getError() == ErrorCode.NoError);

            //throw new NotImplementedException();
        }


        /// <summary>
        /// Copies pixels from the attached color buffers into the given texture.
        /// </summary>
        /// <returns>
        /// The pixels.
        /// </returns>
        /// <param name='xoff'>
        /// Xoff x offset in the destination texture.
        /// </param>
        /// <param name='x'>
        /// X  lower left corner of the area where the pixels must be read.
        /// </param>
        /// <param name='y'>
        /// Y lower left corner of the area where the pixels must be read.
        /// </param>
        /// <param name='w'>
        /// W width of the area where the pixels must be read.
        /// </param>
        /// <param name='dst'>
        /// Dst estination texture for the read pixels.
        /// </param>
        /// <param name='level'>
        /// Level destination LOD level for the read pixels.
        /// </param>
        public void copyPixels(int xoff, int x, int y, int w, Texture1D dst, int level)
        {
            set();
            dst.bindToTextureUnit();
#if OPENTK
            GL.CopyTexSubImage1D(TextureTarget.Texture1D, level, xoff, x, y, w);
#else
            glCopyTexSubImage1D(GL_TEXTURE_1D, level, xoff, x, y, w);
#endif
            Debug.Assert(getError() == ErrorCode.NoError);
        }


        /// <summary>
        /// Copies the pixels  from the attached color buffers into the given texture.
        /// </summary>
        /// <returns>
        /// The pixels.
        /// </returns>
        /// <param name='xoff'>
        /// Xoff x offset in the destination texture.
        /// </param>
        /// <param name='layer'>
        /// Layer  destination layer in the destination texture.
        /// </param>
        /// <param name='x'>
        /// X lower left corner of the area where the pixels must be read.
        /// </param>
        /// <param name='y'>
        /// Y  lower left corner of the area where the pixels must be read.
        /// </param>
        /// <param name='w'>
        /// W width of the area where the pixels must be read.
        /// </param>
        /// <param name='d'>
        /// D number of layers to read.
        /// </param>
        /// <param name='dst'>
        /// Dst destination texture for the read pixels.
        /// </param>
        /// <param name='level'>
        /// Level  destination LOD level for the read pixels.
        /// </param>
        public void copyPixels(int xoff, int layer, int x, int y, int w, int d, Texture1DArray dst, int level)
        {
            set();
            dst.bindToTextureUnit();
#if OPENTK
            GL.CopyTexSubImage2D(TextureTarget.Texture1D, level, xoff, layer, x, y, w, d);
#else
            glCopyTexSubImage2D(GL_TEXTURE_1D, level, xoff, layer, x, y, w, d);
#endif
            Debug.Assert(getError() == ErrorCode.NoError);
        }


        /// <summary>
        /// Copies pixels from the attached color buffers into the given texture.
        /// </summary>
        /// <returns>
        /// The pixels.
        /// </returns>
        /// <param name='xoff'>
        /// Xoff  x offset in the destination texture.
        /// </param>
        /// <param name='yoff'>
        /// Yoff  y offset in the destination texture.
        /// </param>
        /// <param name='x'>
        /// X lower left corner of the area where the pixels must be read.
        /// </param>
        /// <param name='y'>
        /// Y lower left corner of the area where the pixels must be read.
        /// </param>
        /// <param name='w'>
        /// W width of the area where the pixels must be read.
        /// </param>
        /// <param name='h'>
        /// H height of the area where the pixels must be read.
        /// </param>
        /// <param name='dst'>
        /// Dst destination texture for the read pixels.
        /// </param>
        /// <param name='level'>
        /// Level destination LOD level for the read pixels.
        /// </param>
        public void copyPixels(int xoff, int yoff, int x, int y, int w, int h, Texture2D dst, int level)
        {
            set();
            dst.bindToTextureUnit();
#if OPENTK
            GL.CopyTexSubImage2D(TextureTarget.Texture2D, level, xoff, yoff, x, y, w, h);
#else
            glCopyTexSubImage2D(GL_TEXTURE_2D, level, xoff, yoff, x, y, w, h);
#endif
            Debug.Assert(getError() == ErrorCode.NoError);
        }


        /// <summary>
        /// Copies pixels from the attached color buffers into the given texture.
        /// </summary>
        /// <returns>
        /// The pixels.
        /// </returns>
        /// <param name='xoff'>
        /// Xoff x offset in the destination texture.
        /// </param>
        /// <param name='yoff'>
        /// Yoff  y offset in the destination texture.
        /// </param>
        /// <param name='layer'>
        /// Layer destination layer in the destination texture.
        /// </param>
        /// <param name='x'>
        /// X lower left corner of the area where the pixels must be read.
        /// </param>
        /// <param name='y'>
        /// Y lower left corner of the area where the pixels must be read.
        /// </param>
        /// <param name='w'>
        /// W width of the area where the pixels must be read.
        /// </param>
        /// <param name='h'>
        /// H height of the area where the pixels must be read.
        /// </param>
        /// <param name='dst'>
        /// Dst destination texture for the read pixels.
        /// </param>
        /// <param name='level'>
        /// Level destination LOD level for the read pixels.
        /// </param>
        public void copyPixels(int xoff, int yoff, int layer, int x, int y, int w, int h, Texture2DArray dst, int level)
        {
            set();
            dst.bindToTextureUnit();
#if OPENTK
            GL.CopyTexSubImage3D(TextureTarget.Texture2DArray, level, xoff, yoff, layer, x, y, w, h);
#else
            glCopyTexSubImage3D(GL_TEXTURE_2D_ARRAY, level, xoff, yoff, layer, x, y, w, h);
#endif
            Debug.Assert(getError() == ErrorCode.NoError);
        }


        /// <summary>
        /// Copies pixels from the attached color buffers into the given texture.
        /// </summary>
        /// <returns>
        /// The pixels.
        /// </returns>
        /// <param name='xoff'>
        /// Xoff x offset in the destination texture.
        /// </param>
        /// <param name='yoff'>
        /// Yoff y offset in the destination texture.
        /// </param>
        /// <param name='zoff'>
        /// Zoff  z offset in the destination texture.
        /// </param>
        /// <param name='x'>
        /// X lower left corner of the area where the pixels must be read.
        /// </param>
        /// <param name='y'>
        /// Y lower left corner of the area where the pixels must be read.
        /// </param>
        /// <param name='w'>
        /// W width of the area where the pixels must be read.
        /// </param>
        /// <param name='h'>
        /// H height of the area where the pixels must be read.
        /// </param>
        /// <param name='dst'>
        /// Dst destination texture for the read pixels.
        /// </param>
        /// <param name='level'>
        /// Level destination LOD level for the read pixels.
        /// </param>
        public void copyPixels(int xoff, int yoff, int zoff, int x, int y, int w, int h, Texture3D dst, int level)
        {
            set();
            dst.bindToTextureUnit();
#if OPENTK
            GL.CopyTexSubImage3D(TextureTarget.Texture3D, level, xoff, yoff, zoff, x, y, w, h);
#else
            glCopyTexSubImage3D(GL_TEXTURE_3D, level, xoff, yoff, zoff, x, y, w, h);
#endif
            Debug.Assert(getError() == ErrorCode.NoError);
        }


        /// <summary>
        /// Copies pixels from the attached color buffers into the given texture.
        /// </summary>
        /// <returns>
        /// The pixels.
        /// </returns>
        /// <param name='xoff'>
        /// Xoff x offset in the destination texture.
        /// </param>
        /// <param name='yoff'>
        /// Yoff  y offset in the destination texture.
        /// </param>
        /// <param name='x'>
        /// X lower left corner of the area where the pixels must be read.
        /// </param>
        /// <param name='y'>
        /// Y  lower left corner of the area where the pixels must be read.
        /// </param>
        /// <param name='w'>
        /// W  width of the area where the pixels must be read.
        /// </param>
        /// <param name='h'>
        /// H height of the area where the pixels must be read.
        /// </param>
        /// <param name='dst'>
        /// Dst destination texture for the read pixels.
        /// </param>
        /// <param name='level'>
        /// Level destination LOD level for the read pixels.
        /// </param>
        /// <param name='cf'>
        /// Cf destination face for the read pixels.
        /// </param>
        public void copyPixels(int xoff, int yoff, int x, int y, int w, int h, TextureCube dst, int level, CubeFace cf)
        {
            set();
            dst.bindToTextureUnit();
#if OPENTK
            GL.CopyTexSubImage2D(EnumConversion.getCubeFace(cf), level, xoff, yoff, x, y, w, h);
#else
            glCopyTexSubImage2D(cf, level, xoff, yoff, x, y, w, h);
#endif
            Debug.Assert(getError() == ErrorCode.NoError);
        }



        /// <summary>
        /// Copies pixels from the attached color buffers into the given texture.
        /// </summary>
        /// <returns>
        /// The pixels.
        /// </returns>
        /// <param name='xoff'>
        /// Xoff x offset in the destination texture.
        /// </param>
        /// <param name='yoff'>
        /// Yoff  y offset in the destination texture.
        /// </param>
        /// <param name='layer'>
        /// Layer destination layer in the destination texture.
        /// </param>
        /// <param name='x'>
        /// X  lower left corner of the area where the pixels must be read.
        /// </param>
        /// <param name='y'>
        /// Y lower left corner of the area where the pixels must be read.
        /// </param>
        /// <param name='w'>
        /// W width of the area where the pixels must be read.
        /// </param>
        /// <param name='h'>
        /// H height of the area where the pixels must be read.
        /// </param>
        /// <param name='dst'>
        /// Dst destination texture for the read pixels.
        /// </param>
        /// <param name='level'>
        /// Level destination LOD level for the read pixels.
        /// </param>
        /// <param name='cf'>
        /// Cf destination face for the read pixels.
        /// </param>
        public void copyPixels(int xoff, int yoff, int layer, int x, int y, int w, int h, TextureCubeArray dst, int level, CubeFace cf)
        {
            set();
            dst.bindToTextureUnit();
#if OPENTK
            GL.CopyTexSubImage3D(EnumConversion.getCubeFace(cf), level, xoff, yoff, layer, x, y, w, h);
#else
            glCopyTexSubImage3D(cf, level, xoff, yoff, layer, x, y, w, h);
#endif
            Debug.Assert(getError() == ErrorCode.NoError);
        }

        /// <summary>
        /// Copies pixels from the attached color buffers into the given texture.
        /// </summary>
        /// <returns>
        /// The pixels.
        /// </returns>
        /// <param name='xoff'>
        /// Xoff x offset in the destination texture.
        /// </param>
        /// <param name='yoff'>
        /// Yoff y offset in the destination texture.
        /// </param>
        /// <param name='x'>
        /// X lower left corner of the area where the pixels must be read.
        /// </param>
        /// <param name='y'>
        /// Y lower left corner of the area where the pixels must be read.
        /// </param>
        /// <param name='w'>
        /// W width of the area where the pixels must be read.
        /// </param>
        /// <param name='h'>
        /// H height of the area where the pixels must be read.
        /// </param>
        /// <param name='dst'>
        /// Dst destination texture for the read pixels.
        /// </param>
        /// <param name='level'>
        /// Level destination LOD level for the read pixels.
        /// </param>
        public void copyPixels(int xoff, int yoff, int x, int y, int w, int h, TextureRectangle dst, int level)
        {
            set();
            dst.bindToTextureUnit();
#if OPENTK
            GL.CopyTexSubImage2D(TextureTarget.TextureRectangle, level, xoff, yoff, x, y, w, h);
#else
            glCopyTexSubImage2D(GL_TEXTURE_RECTANGLE, level, xoff, yoff, x, y, w, h);
#endif
            Debug.Assert(getError() == ErrorCode.NoError);
        }


        /// <summary>
        /// Returns the OpenGL major version.
        /// </summary>
        /// <returns>
        /// The major version.
        /// </returns>
        public static int getMajorVersion()
        {
            int v;
#if OPENTK
            GL.GetInteger(GetPName.MajorVersion, out v);
#else
            glGetIntegerv(GL_MAJOR_VERSION, &v);
#endif
            return v;
        }


        /// <summary>
        /// Returns the OpenGL minor version.
        /// </summary>
        /// <returns>
        /// The minor version.
        /// </returns>
        public static int getMinorVersion()
        {
            int v;
#if OPENTK
            GL.GetInteger(GetPName.MinorVersion, out v);
#else
            glGetIntegerv(GL_MINOR_VERSION, &v);
#endif
            return v;
        }


        /// <summary>
        /// Returns the OpenGL state.
        /// </summary>
        /// <returns>
        /// the error code of the last invalid operation, or 0 if all
        /// operations executed normally.
        /// </returns>
        public static ErrorCode getError()
        {
#if DEBUG
            // This situation usually occurs when the GraphicsContext is disposed and there are still some OpenGL objects
            // pending to be disposed. Usually the error is due to a wrong dispose policy. Make sure that every OpenGL object
            // is disposed before you end your main program (close your window and therefore the GraphicsContext associated with it)
            // OpenGL only works with an active graphics context.
            if (GraphicsContext.CurrentContext == null || 
                GraphicsContext.CurrentContext.IsDisposed || 
                !GraphicsContext.CurrentContext.IsCurrent)
                Debugger.Break();
#endif
            ErrorCode error = GL.GetError();
            if (error != ErrorCode.NoError && log.IsErrorEnabled)
            {
                log.Error("OpenGL error code " + error);
            }
            return error;
        }

        /// <summary>
        /// Resets all the internal Sxta.Render specific state.
        /// Call this method before and after using OpenGL API directly.
        /// </summary>
        /// <returns>
        /// The all states.
        /// </returns>
        public static void resetAllStates()
        {
#if DEBUG
            if (log.IsDebugEnabled)
                log.Debug("Reset OpenGL STATES");
#endif
            if (MeshBuffers.CURRENT != null)
            {
                MeshBuffers.CURRENT.reset();
            }
#if OPENTK
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
#else
            glBindBuffer(GL_ARRAY_BUFFER, 0);
            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);
#endif
            FrameBuffer.CURRENT = null;
            Program.CURRENT = null;

            Texture.unbindAll();
        }

        public static void LogOpenGLInfo()
        {
            if (log.IsInfoEnabled)
            {
#if OPENTK
                string vendor = GL.GetString(StringName.Vendor);
                string renderer = GL.GetString(StringName.Renderer);
                string version = GL.GetString(StringName.Version);
                string shadingLanguageVersion = GL.GetString(StringName.ShadingLanguageVersion);
                string extensions = GL.GetString(StringName.Extensions);
#else
#endif
                log.InfoFormat("OpenGL Vendor:{0}", vendor);
                log.InfoFormat("OpenGL Renderer:{0}", renderer);
                log.InfoFormat("OpenGL Version:{0}", version);
                log.InfoFormat("OpenGL Shading Language Version:{0}", shadingLanguageVersion);
                log.DebugFormat("OpenGL Extensions:{0}", extensions);
                log.InfoFormat("Assembly Full Name:{0}", Assembly.GetExecutingAssembly().FullName);
            }
        }

        public static void Flush()
        {
            GL.Flush();
        }
        public static void Finish()
        {
            GL.Finish();
        }

        private class FrameBufferMap
        {
            public IDictionary<object, FrameBuffer> buffers = new Dictionary<object, FrameBuffer>();
        }


        /// <summary>
        /// The id of this framebuffer object. 0 for the default one.
        /// </summary>
        private uint framebufferId;


        /// <summary>
        /// The attachments of this framebuffer.
        /// </summary>
        private Object[] textures = new object[6];

        /// <summary>
        /// The levels specified for each attachments of this framebuffer.
        /// </summary>
        private int[] levels = new int[6];


        /// <summary>
        /// The layers specified for each attachments of this framebuffer. Only used for
        /// Texture arrays, Texture Cube, and Texture 3D.
        /// </summary>
        private int[] layers = new int[6];


        /// <summary>
        /// True if #textures, #levels or #layers has changed since the last call to #set().
        /// </summary>
        private bool attachmentsChanged;

        /// <summary>
        /// The read buffer.
        /// </summary>
        private BufferId readBuffer;


        /// <summary>
        /// The number of draw buffers.
        /// </summary>
        private int drawBufferCount;


        /// <summary>
        /// The draw buffers.
        /// </summary>
        private BufferId[] drawBuffers = new BufferId[4];

        /// <summary>
        /// True if #readBuffer, #drawBufferCount or #drawBuffers has changed since
        ///  the last call to #set().
        /// </summary>
        private bool readDrawChanged;


        /// <summary>
        /// The parameters of this framebuffer.
        /// </summary>
        private Parameters parameters = new Parameters();


        /// <summary>
        /// True if #parameters has changed since the last call to #set().
        /// </summary>
        private bool parametersChanged;


        /// <summary>
        /// The default, onscreen framebuffer (one per OpenGL context).
        /// </summary>
        private static FrameBufferMap DEFAULT = new FrameBuffer.FrameBufferMap();


        /// <summary>
        /// The current framebuffer.
        /// </summary>
        private static FrameBuffer CURRENT = null;


        /// <summary>
        /// The current framebuffer parameters.
        /// </summary>
        private static Parameters PARAMETERS = new Parameters();

        /// <summary>
        /// A quad mesh.
        /// </summary>
        private Mesh<Vector4f, uint> QUAD = null;


        /// <summary>
        /// Sets this framebuffer as the current framebuffer.
        /// </summary>
        internal void set()
        {
            bool framebufferChanged = false;
            if (CURRENT != this)
            {
#if DEBUG
                if (log.IsDebugEnabled)
                    log.Debug("Changing Current Framebuffer");
#endif
#if OPENTK
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferId);
#else
                glBindFramebuffer(GL_FRAMEBUFFER, framebufferId);
#endif
                CURRENT = this;
                framebufferChanged = true;
            }
            if (framebufferChanged || parametersChanged)
            {
                PARAMETERS = PARAMETERS.set(parameters);
                parametersChanged = false;
            }
            if (framebufferId != 0 && attachmentsChanged)
            {
                setAttachments();
                checkAttachments();
                attachmentsChanged = false;
            }
            if (framebufferId != 0 && readDrawChanged)
            {
#if OPENTK
                GL.ReadBuffer((ReadBufferMode)getBuffer(readBuffer));
#else
                glReadBuffer(getBuffer(readBuffer));
#endif
                if (drawBufferCount == 1)
                {
#if OPENTK
                    GL.DrawBuffer((DrawBufferMode)getBuffer(drawBuffers[0]));
#else
                    glDrawBuffer(getBuffer(drawBuffers[0]));
#endif
                }
                else
                {
                    DrawBuffersEnum[] drawBufs = new DrawBuffersEnum[4];
                    for (int i = 0; i < drawBufferCount; ++i)
                    {
                        drawBufs[i] = getBuffer(drawBuffers[i]);
                    }
#if OPENTK
                    GL.DrawBuffers(drawBufferCount, drawBufs);
#else
                    glDrawBuffers(drawBufferCount, drawBufs);
#endif
                }
                readDrawChanged = false;
                Debug.Assert(getError() == ErrorCode.NoError);
            }
        }


        /// <summary>
        ///  Sets the attachments of this framebuffer.
        /// </summary>
        /// <returns>
        /// The attachments.
        /// </returns>
        private void setAttachments()
        {
#if DEBUG
            if (log.IsDebugEnabled)
                log.Debug("Setting Framebuffer attachments");
#endif
            FramebufferAttachment[] ATTACHMENTS = new FramebufferAttachment[]{
                                            FramebufferAttachment.ColorAttachment0,
                                            FramebufferAttachment.ColorAttachment1,
                                            FramebufferAttachment.ColorAttachment2,
                                            FramebufferAttachment.ColorAttachment3,
                                            FramebufferAttachment.StencilAttachment,
                                            FramebufferAttachment.DepthAttachment
                                        };
            uint id;
            for (int i = 0; i < 6; ++i)
            {
                if (textures[i] == null)
                {
#if OPENTK
                    GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, ATTACHMENTS[i], RenderbufferTarget.Renderbuffer, 0);
#else
                    glFramebufferRenderbuffer(GL_FRAMEBUFFER, ATTACHMENTS[i], GL_RENDERBUFFER, 0);
#endif
                    continue;
                }
                if (textures[i] is RenderBuffer)
                {
                    id = ((RenderBuffer)textures[i]).getId();
#if OPENTK
                    GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, ATTACHMENTS[i], RenderbufferTarget.Renderbuffer, id);
#else
                    glFramebufferRenderbuffer(GL_FRAMEBUFFER, ATTACHMENTS[i], GL_RENDERBUFFER, id);
#endif
                    continue;
                }

                id = ((Texture)textures[i]).getId();
                if (textures[i] is Texture1DArray ||
                    textures[i] is Texture2DArray ||
                    textures[i] is Texture2DMultisampleArray ||
                    textures[i] is Texture3D)
                {
                    if (layers[i] == -1)
                    {
#if OPENTK
                        GL.FramebufferTexture(FramebufferTarget.Framebuffer, ATTACHMENTS[i], id, levels[i]);
#else
                        glFramebufferTexture(GL_FRAMEBUFFER, ATTACHMENTS[i], id, levels[i]);
#endif
                    }
                    else
                    {
#if OPENTK
                        GL.FramebufferTextureLayer(FramebufferTarget.Framebuffer, ATTACHMENTS[i], id, levels[i], layers[i]);
#else
                        glFramebufferTextureLayer(GL_FRAMEBUFFER, ATTACHMENTS[i], id, levels[i], layers[i]);
#endif
                    }
                }
                else if (textures[i] is Texture1D)
                {
#if OPENTK
                    GL.FramebufferTexture1D(FramebufferTarget.Framebuffer, ATTACHMENTS[i], TextureTarget.Texture1D, id, levels[i]);
#else
                    glFramebufferTexture1D(GL_FRAMEBUFFER, ATTACHMENTS[i], GL_TEXTURE_1D, id, levels[i]);
#endif
                }
                else if (textures[i] is Texture2D)
                {
#if OPENTK
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, ATTACHMENTS[i], TextureTarget.Texture2D, id, levels[i]);
#else
                    glFramebufferTexture2D(GL_FRAMEBUFFER, ATTACHMENTS[i], GL_TEXTURE_2D, id, levels[i]);
#endif
                }
                else if (textures[i] is TextureRectangle)
                {
#if OPENTK
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, ATTACHMENTS[i], TextureTarget.TextureRectangle, id, levels[i]);
#else
                    glFramebufferTexture2D(GL_FRAMEBUFFER, ATTACHMENTS[i], GL_TEXTURE_RECTANGLE, id, levels[i]);
#endif
                }
                else if (textures[i] is Texture2DMultisample)
                {
#if OPENTK
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, ATTACHMENTS[i], TextureTarget.Texture2DMultisample, id, levels[i]);
#else
                    glFramebufferTexture2D(GL_FRAMEBUFFER, ATTACHMENTS[i], GL_TEXTURE_2D_MULTISAMPLE, id, levels[i]);
#endif
                }
                else if (textures[i] is TextureCube)
                {
                    if (layers[i] == -1)
                    {
#if OPENTK
                        GL.FramebufferTexture(FramebufferTarget.Framebuffer, ATTACHMENTS[i], id, levels[i]);
#else
                        glFramebufferTexture(GL_FRAMEBUFFER, ATTACHMENTS[i], id, levels[i]);
#endif
                    }
                    else
                    {
#if OPENTK
                        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, ATTACHMENTS[i], EnumConversion.getCubeFace((CubeFace)layers[i]), id, levels[i]);
#else
                        glFramebufferTexture2D(GL_FRAMEBUFFER, ATTACHMENTS[i], getCubeFace((CubeFace)layers[i]), id, levels[i]);
#endif
                    }
                }
                else if (textures[i] is TextureCubeArray)
                {
#if OPENTK
                    GL.FramebufferTextureLayer(FramebufferTarget.Framebuffer, ATTACHMENTS[i], id, levels[i], layers[i]);
#else
                    glFramebufferTextureLayer(GL_FRAMEBUFFER, ATTACHMENTS[i], id, levels[i], layers[i]);
#endif
                }
                Debug.Assert(getError() == ErrorCode.NoError);
            }
        }


        /// <summary>
        /// Checks the attachments of this framebuffer.
        /// </summary>
        /// <returns>
        /// The attachments.
        /// </returns>
        private void checkAttachments()
        {
#if OPENTK
            FramebufferErrorCode status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
#else
            GLenum status = glCheckFramebufferStatus(GL_FRAMEBUFFER);
#endif
            switch (status)
            {
                case FramebufferErrorCode.FramebufferComplete:
                    return;
                case FramebufferErrorCode.FramebufferUndefined:
                    log.Error("Framebuffer object: undefined");
                    break;
                case FramebufferErrorCode.FramebufferIncompleteAttachment:
                    log.Error("Framebuffer object: incomplete attachement");
                    break;
                case FramebufferErrorCode.FramebufferIncompleteMissingAttachment:
                    log.Error("Framebuffer object: incomplete missing attachement");
                    break;
                case FramebufferErrorCode.FramebufferIncompleteDrawBuffer:
                    log.Error("Framebuffer object: incomplete draw buffer");
                    break;
                case FramebufferErrorCode.FramebufferIncompleteReadBuffer:
                    log.Error("Framebuffer object: incomplete read buffer");
                    break;
                case FramebufferErrorCode.FramebufferUnsupported:
                    log.Error("Framebuffer object: unsupported");
                    break;
                case FramebufferErrorCode.FramebufferIncompleteMultisample:
                    log.Error("Framebuffer object: incomplete multisample");
                    break;
                case FramebufferErrorCode.FramebufferIncompleteLayerTargets:
                    log.Error("Framebuffer object: incomplete layer targets");
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
            Debug.Assert(false);
        }


        /// <summary>
        /// Enables the conditional rendering mode.
        /// </summary>
        /// <returns>
        /// The conditional render.
        /// </returns>
        internal void beginConditionalRender()
        {
            if (parameters.occlusionQuery != null)
            {
#if OPENTK
                GL.BeginConditionalRender(parameters.occlusionQuery.getId(), EnumConversion.getQueryMode(parameters.occlusionMode));
#else
                glBeginConditionalRender(parameters.occlusionQuery.getId(), getQueryMode(parameters.occlusionMode));
#endif
            }
        }


        /// <summary>
        /// Disables the conditional rendering mode.
        /// </summary>
        /// <returns>
        /// The conditional render.
        /// </returns>
        internal void endConditionalRender()
        {
            if (parameters.occlusionQuery != null)
            {
#if OPENTK
                GL.EndConditionalRender();
#else
                glEndConditionalRender();
#endif
            }
        }

        /// <summary>
        /// Returns the id of the given framebuffer attachment point.
        /// </summary>
        /// <returns>
        /// The buffer.
        /// </returns>
        /// <param name='b'>
        /// B.
        /// </param>
        private DrawBuffersEnum getBuffer(BufferId b)
        {
            switch (b & (BufferId.COLOR0 | BufferId.COLOR1 | BufferId.COLOR2 | BufferId.COLOR3))
            {
                case 0:
                    return DrawBuffersEnum.None;
                case BufferId.COLOR0:
                    return framebufferId == 0 ? DrawBuffersEnum.FrontLeft : DrawBuffersEnum.ColorAttachment0;
                case BufferId.COLOR1:
                    return framebufferId == 0 ? DrawBuffersEnum.FrontRight : DrawBuffersEnum.ColorAttachment1;
                case BufferId.COLOR2:
                    return framebufferId == 0 ? DrawBuffersEnum.BackLeft : DrawBuffersEnum.ColorAttachment2;
                case BufferId.COLOR3:
                    return framebufferId == 0 ? DrawBuffersEnum.BackRight : DrawBuffersEnum.ColorAttachment3;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        private static bool checked_ = false;

        private static void checkExtensions()
        {
            if (!checked_)
            {
#if OPENTK
                // TODO
                int version;
                GL.GetInteger(GetPName.MajorVersion, out version);
                if (version < 2)
                {
                    throw new ApplicationException("Unsuported OPENGL version ");
                }
#else
                // if one of these Debug.Assertions fails, try with adding EXT to all
                // glProgramUniform calls (including in Uniform.cpp).
                // if this fails too, use the ORK_NO_GLPROGRAMUNIFORM preprocessor
                // directive (little slower at runtime).
#if !ORK_NO_GLPROGRAMUNIFORM
                Debug.Assert(glProgramUniform1iEXT != null);
                Debug.Assert(glProgramUniform1fEXT != null);
                Debug.Assert(glProgramUniform1d != null);
                Debug.Assert(glProgramUniform2iEXT != null);
                Debug.Assert(glProgramUniform2fEXT != null);
                Debug.Assert(glProgramUniform2d != null);
                Debug.Assert(glProgramUniform3iEXT != null);
                Debug.Assert(glProgramUniform3fEXT != null);
                Debug.Assert(glProgramUniform3d != null);
                Debug.Assert(glProgramUniform4iEXT != null);
                Debug.Assert(glProgramUniform4fEXT != null);
                Debug.Assert(glProgramUniform4d != null);
                Debug.Assert(glProgramUniform1uiEXT != null);
                Debug.Assert(glProgramUniform2uiEXT != null);
                Debug.Assert(glProgramUniform3uiEXT != null);
                Debug.Assert(glProgramUniform4uiEXT != null);
                Debug.Assert(glProgramUniformMatrix2fvEXT != null);
                Debug.Assert(glProgramUniformMatrix2dv != null);
                Debug.Assert(glProgramUniformMatrix3fvEXT != null);
                Debug.Assert(glProgramUniformMatrix3dv != null);
                Debug.Assert(glProgramUniformMatrix4fvEXT != null);
                Debug.Assert(glProgramUniformMatrix4dv != null);
                Debug.Assert(glProgramUniformMatrix2x3fvEXT != null);
                Debug.Assert(glProgramUniformMatrix2x3dv != null);
                Debug.Assert(glProgramUniformMatrix2x4fvEXT != null);
                Debug.Assert(glProgramUniformMatrix2x4dv != null);
                Debug.Assert(glProgramUniformMatrix3x2fvEXT != null);
                Debug.Assert(glProgramUniformMatrix3x2dv != null);
                Debug.Assert(glProgramUniformMatrix3x4fvEXT != null);
                Debug.Assert(glProgramUniformMatrix3x4dv != null);
                Debug.Assert(glProgramUniformMatrix4x2fvEXT != null);
                Debug.Assert(glProgramUniformMatrix4x2dv != null);
                Debug.Assert(glProgramUniformMatrix4x3fvEXT != null);
                Debug.Assert(glProgramUniformMatrix4x3dv != null);
#else
                Debug.Assert(glUniform1i != null);
                Debug.Assert(glUniform1f != null);
                Debug.Assert(glUniform1d != null);
                Debug.Assert(glUniform2i != null);
                Debug.Assert(glUniform2f != null);
                Debug.Assert(glUniform2d != null);
                Debug.Assert(glUniform3i != null);
                Debug.Assert(glUniform3f != null);
                Debug.Assert(glUniform3d != null);
                Debug.Assert(glUniform4i != null);
                Debug.Assert(glUniform4f != null);
                Debug.Assert(glUniform4d != null);
                Debug.Assert(glUniform1ui != null);
                Debug.Assert(glUniform2ui != null);
                Debug.Assert(glUniform3ui != null);
                Debug.Assert(glUniform4ui != null);
                Debug.Assert(glUniformMatrix2fv != null);
                Debug.Assert(glUniformMatrix2dv != null);
                Debug.Assert(glUniformMatrix3fv != null);
                Debug.Assert(glUniformMatrix3dv != null);
                Debug.Assert(glUniformMatrix4fv != null);
                Debug.Assert(glUniformMatrix4dv != null);
                Debug.Assert(glUniformMatrix2x3fv != null);
                Debug.Assert(glUniformMatrix2x3dv != null);
                Debug.Assert(glUniformMatrix2x4fv != null);
                Debug.Assert(glUniformMatrix2x4dv != null);
                Debug.Assert(glUniformMatrix3x2fv != null);
                Debug.Assert(glUniformMatrix3x2dv != null);
                Debug.Assert(glUniformMatrix3x4fv != null);
                Debug.Assert(glUniformMatrix3x4dv != null);
                Debug.Assert(glUniformMatrix4x2fv != null);
                Debug.Assert(glUniformMatrix4x2dv != null);
                Debug.Assert(glUniformMatrix4x3fv != null);
                Debug.Assert(glUniformMatrix4x3dv != null);
#endif
#endif
                checked_ = true;
            }


        }
        private static int PARAMETER_ID = 0;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        #region Dispose

        // Track whether Dispose has been called. 
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly 
        // or indirectly by a user's code. Managed and unmanaged resources 
        // can be disposed. 
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
                    if (CURRENT == this)
                    {
                        CURRENT = null;
                    }
                    if (QUAD != null)
                    {
                        QUAD.Dispose();
                        QUAD = null;
                    }
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.

                if (framebufferId != 0)
                {
#if OPENTK
                    GL.DeleteFramebuffers(1, ref framebufferId);
#else 
                    glDeleteFramebuffers(1, &framebufferId);
#endif
                }
                Debug.Assert(getError() == ErrorCode.NoError);

                // Note disposing has been done.
                disposed = true;
            }
        }
        #endregion
    }
}
