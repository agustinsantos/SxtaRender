using Sxta.Math;
using Sxta.Render;
using System;
using NVGcolor = Sxta.Math.Vector4f;

namespace NanoVG
{
    public struct Bound
    {
        public float xmin;
        public float ymin;
        public float xmax;
        public float ymax;
    }

    public struct NVGpaint
    {
        public float[] xform;// = new float[6];
        public float[] extent;// = new float[2];
        public float radius;
        public float feather;
        public NVGcolor innerColor;
        public NVGcolor outerColor;
        public int image;

        public void Init()
        {
            this.xform = new float[6] { 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f }; //nvgTransformIdentity(p.xform);
            this.extent = new float[2];
        }

        public void SetPaintColor(NVGcolor color)
        {
            this.xform = new float[6] { 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f }; //nvgTransformIdentity(p.xform);
            this.extent = new float[2];
            this.radius = 0.0f;
            this.feather = 1.0f;
            this.innerColor = color;
            this.outerColor = color;
        }
    }


    public enum NVGwinding
    {
        NVG_CCW = 1,			// Winding for solid shapes
        NVG_CW = 2,				// Winding for holes
    }

    public enum NVGsolidity
    {
        NVG_SOLID = 1,			// CCW
        NVG_HOLE = 2,			// CW
    }

    /// <summary>
    /// The lineCap property sets or returns the style of the end caps for a line.
    /// </summary>
    public enum NVGlineCap
    {
        /// <summary>
        /// Default. A flat edge is added to each end of the line
        /// </summary>
        NVG_BUTT,
        /// <summary>
        /// A rounded end cap is added to each end of the line
        /// </summary>
        NVG_ROUND,
        /// <summary>
        /// A square end cap is added to each end of the line
        /// </summary>
        NVG_SQUARE
    }

    /// <summary>
    /// The lineJoin property sets or returns the type of corner created, when two lines meet.
    /// </summary>
    public enum NVGlineJoin
    {
        /// <summary>
        /// Default. Creates a sharp corner
        /// </summary>
        NVG_MITER,
        /// <summary>
        /// Creates a rounded corner
        /// </summary>
        NVG_ROUND,
        /// <summary>
        /// Creates a beveled corner
        /// </summary>
        NVG_BEVEL
    }

    [Flags]
    public enum NVGalign
    {
        // Horizontal align
        NVG_ALIGN_LEFT = 1 << 0,	// Default, align text horizontally to left.
        NVG_ALIGN_CENTER = 1 << 1,	// Align text horizontally to center.
        NVG_ALIGN_RIGHT = 1 << 2,	// Align text horizontally to right.
        // Vertical align
        NVG_ALIGN_TOP = 1 << 3,	// Align text vertically to top.
        NVG_ALIGN_MIDDLE = 1 << 4,	// Align text vertically to middle.
        NVG_ALIGN_BOTTOM = 1 << 5,	// Align text vertically to bottom. 
        NVG_ALIGN_BASELINE = 1 << 6, // Default, align text vertically to baseline. 
    }

    public struct NVGglyphPosition
    {
        public int str;	        // Position of the glyph in the input string.
        public float x;         // The x-coordinate of the logical glyph position.
        public float minx, maxx;	// The bounds of the glyph shape.
    }


    public struct NVGtextRow
    {
        public int start;   // Pointer to the input text where the row starts.
        public int end; // Pointer to the input text where the row ends (one past the last character).
        public int next;    // Pointer to the beginning of the next row.
        public float width;		// Logical width of the row.
        public float minx, maxx;	// Actual bounds of the row. Logical with and bounds can differ because of kerning and some parts over extending.
    }

    [Flags]
    public enum NVGimageFlags
    {
        NVG_IMAGE_GENERATE_MIPMAPS = 1 << 0,     // Generate mipmaps during creation of the image.
        NVG_IMAGE_REPEATX = 1 << 1,		// Repeat image in X direction.
        NVG_IMAGE_REPEATY = 1 << 2,		// Repeat image in Y direction.
        NVG_IMAGE_FLIPY = 1 << 3,		// Flips (inverses) image in Y direction when rendered.
        NVG_IMAGE_PREMULTIPLIED = 1 << 4,		// Image data has premultiplied alpha.
    }



    public class Nvg
    {

        public static NVGcontext CreateContext(FrameBuffer fb, NVGcreateFlags flags = NVGcreateFlags.NVG_NONE, object userData = null)
        {
            NVGparams params_ = new NVGparams();
            params_.UserPtr = userData;
            params_.EdgeAntiAlias = flags.HasFlag(NVGcreateFlags.NVG_ANTIALIAS);

            NVGcontext ctx = new NVGcontext(fb, flags, params_);
            return ctx;
        }

        /// <summary>
        /// Begin drawing a new frame
        /// Calls to nanovg drawing API should be wrapped in nvgBeginFrame() & nvgEndFrame()
        /// nvgBeginFrame() defines the size of the window to render to in relation currently
        /// set viewport (i.e. glViewport on GL backends). Device pixel ration allows to
        /// control the rendering on Hi-DPI devices.
        /// For example, GLFW returns two dimension for an opened window: window size and
        /// frame buffer size. In that case you would set windowWidth/Height to the window size
        /// devicePixelRatio to: frameBufferWidth / windowWidth.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        /// <param name="devicePixelRatio"></param>
        public static void BeginFrame(NVGcontext ctx, int windowWidth, int windowHeight, float devicePixelRatio)
        {
            ctx.nstates = 0;
            ctx.SaveState();
            ctx.Reset();

            SetDevicePixelRatio(ctx, devicePixelRatio);

            ctx.RenderViewport(windowWidth, windowHeight);

            ctx.drawCallCount = 0;
            ctx.fillTriCount = 0;
            ctx.strokeTriCount = 0;
            ctx.textTriCount = 0;
        }

        /// <summary>
        /// Cancels drawing the current frame.
        /// </summary>
        /// <param name="ctx"></param>
        public void CancelFrame(NVGcontext ctx)
        {
            ctx.glctx.RenderCancel();
        }

        /// <summary>
        /// Ends drawing flushing remaining render state.
        /// </summary>
        /// <param name="ctx"></param>
        public static void EndFrame(NVGcontext ctx)
        {
            ctx.glctx.RenderFlush();
            if (ctx.fontImageIdx != 0)
            {
                int fontImage = ctx.fontImages[ctx.fontImageIdx];
                int i, j, iw, ih;
                // delete images that smaller than current one
                if (fontImage == 0)
                    return;
                nvgImageSize(ctx, fontImage, out iw, out ih);
                for (i = j = 0; i < ctx.fontImageIdx; i++)
                {
                    if (ctx.fontImages[i] != 0)
                    {
                        int nw, nh;
                        nvgImageSize(ctx, ctx.fontImages[i], out nw, out nh);
                        if (nw < iw || nh < ih)
                            nvgDeleteImage(ctx, ctx.fontImages[i]);
                        else
                            ctx.fontImages[j++] = ctx.fontImages[i];
                    }
                }
                // make current font image to first
                ctx.fontImages[j++] = ctx.fontImages[0];
                ctx.fontImages[0] = fontImage;
                ctx.fontImageIdx = 0;
                // clear all images after j
                for (i = j; i < NVGcontext.NVG_MAX_FONTIMAGES; i++)
                    ctx.fontImages[i] = 0;
            }
        }

        //
        // Color utils
        //

        /// <summary>
        /// Returns a color value from red, green, blue values. Alpha will be set to 255 (1.0f).
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static NVGcolor RGB(byte r, byte g, byte b)
        {
            return RGBA(r, g, b, 255);
        }

        /// <summary>
        /// Returns a color value from red, green, blue values. Alpha will be set to 1.0f.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static NVGcolor RGBf(float r, float g, float b)
        {
            return RGBAf(r, g, b, 1.0f);
        }

        /// <summary>
        ///  Returns a color value from red, green, blue and alpha values.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static NVGcolor RGBA(byte r, byte g, byte b, byte a)
        {
            return new NVGcolor(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
        }

        /// <summary>
        /// Returns a color value from red, green, blue and alpha values.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static NVGcolor RGBAf(float r, float g, float b, float a)
        {
            return new NVGcolor(r, g, b, a);
        }

        /// <summary>
        /// Linearly interpolates from color c0 to c1, and returns resulting color value.
        /// </summary>
        /// <param name="c0"></param>
        /// <param name="c1"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public static NVGcolor LerpRGBA(NVGcolor c0, NVGcolor c1, float u)
        {
            float oneminu;

            u = Clampf(u, 0.0f, 1.0f);
            oneminu = 1.0f - u;
            NVGcolor cint = new NVGcolor(c0.X * oneminu + c1.X * u,
                                        c0.Y * oneminu + c1.Y * u,
                                        c0.Z * oneminu + c1.Z * u,
                                        c0.W * oneminu + c1.W * u);

            return cint;
        }

        /// <summary>
        /// Sets transparency of a color value.
        /// </summary>
        /// <param name="c0"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static NVGcolor TransRGBA(NVGcolor c0, byte a)
        {
            c0.W = (uint)(a / 255.0f);
            return c0;
        }

        /// <summary>
        /// Sets transparency of a color value.
        /// </summary>
        /// <param name="c0"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static NVGcolor TransRGBAf(NVGcolor c0, float a)
        {
            c0.W = a;
            return c0;
        }

        // Returns color value specified by hue, saturation and lightness.
        // HSL values are all in range [0..1], alpha will be set to 255.
        public Vector3f nvgHSL(float h, float s, float l)
        {
            throw new NotImplementedException();
        }

        // Returns color value specified by hue, saturation and lightness and alpha.
        // HSL values are all in range [0..1], alpha in range [0..255]
        public Vector3f nvgHSLA(float h, float s, float l, byte a)
        {
            throw new NotImplementedException();
        }

        //
        // State Handling
        //
        // NanoVG contains state which represents how paths will be rendered.
        // The state contains transform, fill and stroke styles, text and font styles,
        // and scissor clipping.

        // Pushes and saves the current render state into a state stack.
        // A matching nvgRestore() must be used to restore the state.
        public void nvgSave(NVGcontext ctx)
        {
            throw new NotImplementedException();
        }


        //
        // Render styles
        //
        // Fill and stroke render style can be either a solid color or a paint which is a gradient or a pattern.
        // Solid color is simply defined as a color value, different kinds of paints can be created
        // using nvgLinearGradient(), nvgBoxGradient(), nvgRadialGradient() and nvgImagePattern().
        //
        // Current render style can be saved and restored using nvgSave() and nvgRestore(). 

        /// <summary>
        ///  Sets current stroke style to a solid color.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="color"></param>
        public static void StrokeColor(NVGcontext ctx, NVGcolor color)
        {
            NVGstate state = ctx.GetState();
            state.stroke.SetPaintColor(color);
            ctx.SetState(state);
        }

        /// <summary>
        /// Sets current stroke style to a paint, which can be a one of the gradients or a pattern.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="paint"></param>
        public static void StrokePaint(NVGcontext ctx, NVGpaint paint)
        {
            NVGstate state = ctx.GetState();
            state.stroke = paint;
            TransformMultiply(state.stroke.xform, state.xform);
            ctx.SetState(state);
        }

        /// <summary>
        /// Sets current fill style to a solid color.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="color"></param>
        public static void FillColor(NVGcontext ctx, NVGcolor color)
        {
            NVGstate state = ctx.GetState();
            state.fill.SetPaintColor(color);
        }

        /// <summary>
        /// Sets current fill style to a paint, which can be a one of the gradients or a pattern. 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="paint"></param>
        public static void FillPaint(NVGcontext ctx, NVGpaint paint)
        {
            NVGstate state = ctx.GetState();
            state.fill = paint;
            TransformMultiply(state.fill.xform, state.xform);
            ctx.SetState(state);
        }


        /// <summary>
        /// Sets the miter limit of the stroke style.
        /// Miter limit controls when a sharp corner is beveled.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="limit"></param>
        public static void MiterLimit(NVGcontext ctx, float limit)
        {
            NVGstate state = ctx.GetState();
            state.miterLimit = limit;
            ctx.SetState(state);
        }

        /// <summary>
        /// Sets the stroke width of the stroke style.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="width"></param>
        public static void StrokeWidth(NVGcontext ctx, float width)
        {
            NVGstate state = ctx.GetState();
            state.strokeWidth = width;
            ctx.SetState(state);
        }

        /// <summary>
        /// Sets how the end of the line (cap) is drawn,
        /// Can be one of: NVG_BUTT (default), NVG_ROUND, NVG_SQUARE.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="cap"></param>
        public static void LineCap(NVGcontext ctx, NVGlineCap cap)
        {
            NVGstate state = ctx.GetState();
            state.lineCap = cap;
            ctx.SetState(state);
        }

        /// <summary>
        /// Sets how sharp path corners are drawn.
        /// Can be one of NVG_MITER (default), NVG_ROUND, NVG_BEVEL.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="join"></param>
        public static void LineJoin(NVGcontext ctx, NVGlineJoin join)
        {
            NVGstate state = ctx.GetState();
            state.lineJoin = join;
            ctx.SetState(state);
        }

        /// <summary>
        /// Sets the transparency applied to all rendered shapes.
        /// Already transparent paths will get proportionally more transparent as well.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="alpha"></param>
        public static void GlobalAlpha(NVGcontext ctx, float alpha)
        {
            NVGstate state = ctx.GetState();
            state.alpha = alpha;
            ctx.SetState(state);
        }

        //
        // Transforms
        //
        // The paths, gradients, patterns and scissor region are transformed by an transformation
        // matrix at the time when they are passed to the API.
        // The current transformation matrix is a affine matrix:
        //   [sx kx tx]
        //   [ky sy ty]
        //   [ 0  0  1]
        // Where: sx,sy define scaling, kx,ky skewing, and tx,ty translation.
        // The last row is assumed to be 0,0,1 and is not stored.
        //
        // Apart from nvgResetTransform(), each transformation function first creates
        // specific transformation matrix and pre-multiplies the current transformation by it.
        //
        // Current coordinate system (transformation) can be saved and restored using nvgSave() and nvgRestore(). 

        // Resets current transform to a identity matrix.
        public void nvgResetTransform(NVGcontext ctx)
        {
            throw new NotImplementedException();
        }

        // Premultiplies current coordinate system by specified matrix.
        // The parameters are interpreted as matrix as follows:
        //   [a c e]
        //   [b d f]
        //   [0 0 1]
        public void nvgTransform(NVGcontext ctx, float a, float b, float c, float d, float e, float f)
        {
            throw new NotImplementedException();
        }

        // Translates current coordinate system.
        public void nvgTranslate(NVGcontext ctx, float x, float y)
        {
            throw new NotImplementedException();
        }

        // Rotates current coordinate system. Angle is specified in radians.
        public void nvgRotate(NVGcontext ctx, float angle)
        {
            throw new NotImplementedException();
        }

        // Skews the current coordinate system along X axis. Angle is specified in radians.
        public void nvgSkewX(NVGcontext ctx, float angle)
        {
            throw new NotImplementedException();
        }

        // Skews the current coordinate system along Y axis. Angle is specified in radians.
        public void nvgSkewY(NVGcontext ctx, float angle)
        {
            throw new NotImplementedException();
        }

        // Scales the current coordinate system.
        public void nvgScale(NVGcontext ctx, float x, float y)
        {
            throw new NotImplementedException();
        }

        // Stores the top part (a-f) of the current transformation matrix in to the specified buffer.
        //   [a c e]
        //   [b d f]
        //   [0 0 1]
        // There should be space for 6 floats in the return buffer for the values a-f.
        public void nvgCurrentTransform(NVGcontext ctx, Matrix3f xform)
        {
            throw new NotImplementedException();
        }


        // The following functions can be used to make calculations on 2x3 transformation matrices.
        // A 2x3 matrix is represented as float[6].

        /// <summary>
        ///  Sets the transform to identity matrix.
        /// </summary>
        /// <param name="t"></param>
        public static void TransformIdentity(float[] t)
        {
            t[0] = 1.0f; t[1] = 0.0f;
            t[2] = 0.0f; t[3] = 1.0f;
            t[4] = 0.0f; t[5] = 0.0f;
        }

        // Sets the transform to translation matrix matrix.
        public void nvgTransformTranslate(float[] dst, float tx, float ty)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the transform to scale matrix.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        public static void TransformScale(float[] t, float sx, float sy)
        {
            t[0] = sx; t[1] = 0.0f;
            t[2] = 0.0f; t[3] = sy;
            t[4] = 0.0f; t[5] = 0.0f;
        }

        // Sets the transform to rotate matrix. Angle is specified in radians.
        public void nvgTransformRotate(float[] dst, float a)
        {
            throw new NotImplementedException();
        }

        // Sets the transform to skew-x matrix. Angle is specified in radians.
        public void nvgTransformSkewX(float[] dst, float a)
        {
            throw new NotImplementedException();
        }

        // Sets the transform to skew-y matrix. Angle is specified in radians.
        public void nvgTransformSkewY(float[] dst, float a)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the transform to the result of multiplication of two transforms, of A = A*B.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="s"></param>
        public static void TransformMultiply(float[] t, float[] s)
        {
            float t0 = t[0] * s[0] + t[1] * s[2];
            float t2 = t[2] * s[0] + t[3] * s[2];
            float t4 = t[4] * s[0] + t[5] * s[2] + s[4];
            t[1] = t[0] * s[1] + t[1] * s[3];
            t[3] = t[2] * s[1] + t[3] * s[3];
            t[5] = t[4] * s[1] + t[5] * s[3] + s[5];
            t[0] = t0;
            t[2] = t2;
            t[4] = t4;
        }

        /// <summary>
        /// Sets the transform to the result of multiplication of two transforms, of A = B*A.
        /// </summary>
        /// <param name="dst"></param>
        /// <param name="src"></param>
        public void nvgTransformPremultiply(float[] dst, float[] src)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the destination to inverse of specified transform.
        /// </summary>
        /// <param name="inv"></param>
        /// <param name="t"></param>
        public static void TransformInverse(float[] inv, float[] t)
        {
            double invdet, det = (double)t[0] * t[3] - (double)t[2] * t[1];
            if (det > -1e-6 && det < 1e-6)
            {
                Nvg.TransformIdentity(inv);
            }
            invdet = 1.0 / det;
            inv[0] = (float)(t[3] * invdet);
            inv[2] = (float)(-t[2] * invdet);
            inv[4] = (float)(((double)t[2] * t[5] - (double)t[3] * t[4]) * invdet);
            inv[1] = (float)(-t[1] * invdet);
            inv[3] = (float)(t[0] * invdet);
            inv[5] = (float)(((double)t[1] * t[4] - (double)t[0] * t[5]) * invdet);
        }

        /// <summary>
        /// Transform a point by given transform.
        /// </summary>
        /// <param name="dstx"></param>
        /// <param name="dsty"></param>
        /// <param name="t"></param>
        /// <param name="srcx"></param>
        /// <param name="srcy"></param>
        public static void TransformPoint(out float dstx, out float dsty, float[] t, float srcx, float srcy)
        {
            dstx = srcx * t[0] + srcy * t[2] + t[4];
            dsty = srcx * t[1] + srcy * t[3] + t[5];
        }

        /// <summary>
        /// Converts degrees to radians and vice versa.
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static float DegToRad(float deg)
        {
            return deg / 180.0f * (float)Math.PI;
        }

        public static float RadToDeg(float rad)
        {
            return rad / (float)Math.PI * 180.0f;
        }

        //
        // Images
        //
        // NanoVG allows you to load jpg, png, psd, tga, pic and gif files to be used for rendering.
        // In addition you can upload your own image. The image loading is provided by stb_image.
        // The parameter imageFlags is combination of flags defined in NVGimageFlags.

        // Creates image by loading it from the disk from specified file name.
        // Returns handle to the image.
        public int nvgCreateImage(NVGcontext ctx, string filename, int imageFlags)
        {
            throw new NotImplementedException();
        }

        // Creates image by loading it from the specified chunk of memory.
        // Returns handle to the image.
        public int nvgCreateImageMem(NVGcontext ctx, int imageFlags, byte[] data, int ndata)
        {
            throw new NotImplementedException();
        }

        // Creates image from specified image data.
        // Returns handle to the image.
        public int nvgCreateImageRGBA(NVGcontext ctx, int w, int h, int imageFlags, byte[] data)
        {
            throw new NotImplementedException();
        }

        // Updates image data specified by image handle.
        public void nvgUpdateImage(NVGcontext ctx, int image, byte[] data)
        {
            throw new NotImplementedException();
        }

        // Returns the dimensions of a created image.
        public static void nvgImageSize(NVGcontext ctx, int image, out int w, out int h)
        {
            ctx.glctx.RenderGetTextureSize(image, out w, out h);
        }

        // Deletes created image.
        public static void nvgDeleteImage(NVGcontext ctx, int image)
        {
            ctx.glctx.RenderDeleteTexture(image);
        }

        //
        // Paints
        //
        // NanoVG supports four types of paints: linear gradient, box gradient, radial gradient and image pattern.
        // These can be used as paints for strokes and fills.

        /// <summary>
        /// Creates and returns a linear gradient. Parameters (sx,sy)-(ex,ey) specify the start and end coordinates
        /// of the linear gradient, icol specifies the start color and ocol the end color.
        /// The gradient is transformed by the current transform when it is passed to FillPaint() or StrokePaint().
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        /// <param name="ex"></param>
        /// <param name="ey"></param>
        /// <param name="icol"></param>
        /// <param name="ocol"></param>
        /// <returns></returns>
        public static NVGpaint LinearGradient(NVGcontext ctx, float sx, float sy, float ex, float ey, NVGcolor icol, NVGcolor ocol)
        {
            NVGpaint p = new NVGpaint();
            p.Init();
            float dx, dy, d;
            const float large = 1e5f;

            // Calculate transform aligned to the line
            dx = ex - sx;
            dy = ey - sy;
            d = (float)Math.Sqrt(dx * dx + dy * dy);
            if (d > 0.0001f)
            {
                dx /= d;
                dy /= d;
            }
            else
            {
                dx = 0;
                dy = 1;
            }

            p.xform[0] = dy; p.xform[1] = -dx;
            p.xform[2] = dx; p.xform[3] = dy;
            p.xform[4] = sx - dx * large; p.xform[5] = sy - dy * large;

            p.extent[0] = large;
            p.extent[1] = large + d * 0.5f;

            p.radius = 0.0f;

            p.feather = Math.Max(1.0f, d);

            p.innerColor = icol;
            p.outerColor = ocol;

            return p;
        }

        /// <summary>
        /// Creates and returns a box gradient. Box gradient is a feathered rounded rectangle, it is useful for rendering
        /// drop shadows or highlights for boxes. Parameters (x,y) define the top-left corner of the rectangle,
        /// (w,h) define the size of the rectangle, r defines the corner radius, and f feather. Feather defines how blurry
        /// the border of the rectangle is. Parameter icol specifies the inner color and ocol the outer color of the gradient.
        /// The gradient is transformed by the current transform when it is passed to FillPaint() or StrokePaint().
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="r"></param>
        /// <param name="f"></param>
        /// <param name="icol"></param>
        /// <param name="ocol"></param>
        /// <returns></returns>
        public static NVGpaint BoxGradient(NVGcontext ctx, float x, float y, float w, float h, float r, float f, NVGcolor icol, NVGcolor ocol)
        {
            NVGpaint p = new NVGpaint();
            p.Init();

            p.xform[4] = x + w * 0.5f;
            p.xform[5] = y + h * 0.5f;

            p.extent[0] = w * 0.5f;
            p.extent[1] = h * 0.5f;

            p.radius = r;

            p.feather = Math.Max(1.0f, f);

            p.innerColor = icol;
            p.outerColor = ocol;

            return p;
        }


        /// <summary>
        /// Creates and returns a radial gradient. Parameters (cx,cy) specify the center, inr and outr specify
        /// the inner and outer radius of the gradient, icol specifies the start color and ocol the end color.
        /// The gradient is transformed by the current transform when it is passed to FillPaint() or StrokePaint().       
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="inr"></param>
        /// <param name="outr"></param>
        /// <param name="icol"></param>
        /// <param name="ocol"></param>
        /// <returns></returns>
        public static NVGpaint RadialGradient(NVGcontext ctx, float cx, float cy, float inr, float outr, NVGcolor icol, NVGcolor ocol)
        {
            NVGpaint p = new NVGpaint();
            p.Init();
            float r = (inr + outr) * 0.5f;
            float f = (outr - inr);
            p.xform[4] = cx;
            p.xform[5] = cy;

            p.extent[0] = r;
            p.extent[1] = r;

            p.radius = r;

            p.feather = Math.Max(1.0f, f);

            p.innerColor = icol;
            p.outerColor = ocol;

            return p;
        }

        // Creates and returns an image patter. Parameters (ox,oy) specify the left-top location of the image pattern,
        // (ex,ey) the size of one image, angle rotation around the top-left corner, image is handle to the image to render.
        // The gradient is transformed by the current transform when it is passed to nvgFillPaint() or nvgStrokePaint().
        public NVGpaint nvgImagePattern(NVGcontext ctx, float ox, float oy, float ex, float ey,
                                            float angle, int image, float alpha)
        {
            throw new NotImplementedException();
        }

        //
        // Scissoring
        //
        // Scissoring allows you to clip the rendering into a rectangle. This is useful for various
        // user interface cases like rendering a text edit or a timeline. 

        // Sets the current scissor rectangle.
        // The scissor rectangle is transformed by the current transform.
        public void nvgScissor(NVGcontext ctx, float x, float y, float w, float h)
        {
            throw new NotImplementedException();
        }

        // Intersects current scissor rectangle with the specified rectangle.
        // The scissor rectangle is transformed by the current transform.
        // Note: in case the rotation of previous scissor rect differs from
        // the current one, the intersection will be done between the specified
        // rectangle and the previous scissor rectangle transformed in the current
        // transform space. The resulting shape is always rectangle.
        public void nvgIntersectScissor(NVGcontext ctx, float x, float y, float w, float h)
        {
            throw new NotImplementedException();
        }

        // Reset and disables scissoring.
        public void nvgResetScissor(NVGcontext ctx)
        {
            throw new NotImplementedException();
        }

        //
        // Paths
        //
        // Drawing a new shape starts with nvgBeginPath(), it clears all the currently defined paths.
        // Then you define one or more paths and sub-paths which describe the shape. The are functions
        // to draw common shapes like rectangles and circles, and lower level step-by-step functions,
        // which allow to define a path curve by curve.
        //
        // NanoVG uses even-odd fill rule to draw the shapes. Solid shapes should have counter clockwise
        // winding and holes should have counter clockwise order. To specify winding of a path you can
        // call nvgPathWinding(). This is useful especially for the common shapes, which are drawn CCW.
        //
        // Finally you can fill the path using current fill style by calling nvgFill(), and stroke it
        // with current stroke style by calling nvgStroke().
        //
        // The curve segments and sub-paths are transformed by the current transform.

        /// <summary>
        /// Clears the current path and sub-paths.
        /// </summary>
        /// <param name="ctx"></param>
        public static void BeginPath(NVGcontext ctx)
        {
            ctx.ncommands = 0;
            ctx.ClearPathCache();
        }

        /// <summary>
        /// Starts new sub-path with specified point as first point.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void MoveTo(NVGcontext ctx, float x, float y)
        {
            float[] vals = { (float)NVGcommands.NVG_MOVETO, x, y };
            AppendCommands(ctx, vals);
        }

        /// <summary>
        /// Adds line segment from the last point in the path to the specified point.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void LineTo(NVGcontext ctx, float x, float y)
        {
            float[] vals = { (float)NVGcommands.NVG_LINETO, x, y };
            AppendCommands(ctx, vals);
        }

        /// <summary>
        /// Adds cubic bezier segment from last point in the path via two control points to the specified point.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="c1x"></param>
        /// <param name="c1y"></param>
        /// <param name="c2x"></param>
        /// <param name="c2y"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void BezierTo(NVGcontext ctx, float c1x, float c1y, float c2x, float c2y, float x, float y)
        {
            float[] vals = { (float)NVGcommands.NVG_BEZIERTO, c1x, c1y, c2x, c2y, x, y };
            AppendCommands(ctx, vals);
        }

        /// <summary>
        /// Adds quadratic bezier segment from last point in the path via a control point to the specified point.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void QuadTo(NVGcontext ctx, float cx, float cy, float x, float y)
        {
            float x0 = ctx.commandx;
            float y0 = ctx.commandy;
            float[] vals = { (float)NVGcommands.NVG_BEZIERTO,
                                x0 + 2.0f/3.0f*(cx - x0), y0 + 2.0f/3.0f*(cy - y0),
                                x + 2.0f/3.0f*(cx - x), y + 2.0f/3.0f*(cy - y),
                                x, y };
            AppendCommands(ctx, vals);
        }

        /// <summary>
        /// Adds an arc segment at the corner defined by the last path point, and two specified points.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="radius"></param>
        public static void ArcTo(NVGcontext ctx, float x1, float y1, float x2, float y2, float radius)
        {
            float x0 = ctx.commandx;
            float y0 = ctx.commandy;
            float dx0, dy0, dx1, dy1, a, d, cx, cy, a0, a1;
            NVGwinding dir;

            if (ctx.ncommands == 0)
            {
                return;
            }

            // Handle degenerate cases.
            if (NVGpoint.Equals(x0, y0, x1, y1, ctx.distTol) ||
                NVGpoint.Equals(x1, y1, x2, y2, ctx.distTol) ||
                NVGpoint.DistPtSeg(x1, y1, x0, y0, x2, y2) < ctx.distTol * ctx.distTol ||
                radius < ctx.distTol)
            {
                LineTo(ctx, x1, y1);
                return;
            }

            // Calculate tangential circle to lines (x0,y0)-(x1,y1) and (x1,y1)-(x2,y2).
            dx0 = x0 - x1;
            dy0 = y0 - y1;
            dx1 = x2 - x1;
            dy1 = y2 - y1;
            NVGpoint.Normalize(ref dx0, ref dy0);
            NVGpoint.Normalize(ref dx1, ref dy1);
            a = (float)Math.Acos(dx0 * dx1 + dy0 * dy1);
            d = radius / (float)Math.Tan(a / 2.0f);

            //	printf("a=%f° d=%f\n", a/NVG_PI*180.0f, d);

            if (d > 10000.0f)
            {
                LineTo(ctx, x1, y1);
                return;
            }

            if (Cross(dx0, dy0, dx1, dy1) > 0.0f)
            {
                cx = x1 + dx0 * d + dy0 * radius;
                cy = y1 + dy0 * d + -dx0 * radius;
                a0 = (float)Math.Atan2(dx0, -dy0);
                a1 = (float)Math.Atan2(-dx1, dy1);
                dir = NVGwinding.NVG_CW;
                //		printf("CW c=(%f, %f) a0=%f° a1=%f°\n", cx, cy, a0/NVG_PI*180.0f, a1/NVG_PI*180.0f);
            }
            else
            {
                cx = x1 + dx0 * d + -dy0 * radius;
                cy = y1 + dy0 * d + dx0 * radius;
                a0 = (float)Math.Atan2(-dx0, dy0);
                a1 = (float)Math.Atan2(dx1, -dy1);
                dir = NVGwinding.NVG_CCW;
                //		printf("CCW c=(%f, %f) a0=%f° a1=%f°\n", cx, cy, a0/NVG_PI*180.0f, a1/NVG_PI*180.0f);
            }

            Arc(ctx, cx, cy, radius, a0, a1, dir);
        }

        /// <summary>
        /// Closes current sub-path with a line segment.
        /// </summary>
        /// <param name="ctx"></param>
        public static void ClosePath(NVGcontext ctx)
        {
            float[] vals = { (float)NVGcommands.NVG_CLOSE };
            AppendCommands(ctx, vals);
        }

        /// <summary>
        /// Sets the current sub-path winding, see NVGwinding and NVGsolidity. 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dir"></param>
        public static void PathWinding(NVGcontext ctx, int dir)
        {
            float[] vals = { (float)NVGcommands.NVG_WINDING, (float)dir };
            AppendCommands(ctx, vals);
        }

        /// <summary>
        /// Creates new circle arc shaped sub-path. The arc center is at cx,cy, the arc radius is r,
        /// and the arc is drawn from angle a0 to a1, and swept in direction dir (NVG_CCW, or NVG_CW).
        /// Angles are specified in radians.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="r"></param>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="dir"></param>
        public static void Arc(NVGcontext ctx, float cx, float cy, float r, float a0, float a1, NVGwinding dir)
        {
            float a = 0, da = 0, hda = 0, kappa = 0;
            float dx = 0, dy = 0, x = 0, y = 0, tanx = 0, tany = 0;
            float px = 0, py = 0, ptanx = 0, ptany = 0;
            float[] vals = new float[3 + 5 * 7 + 100];
            int i, ndivs, nvals;
            NVGcommands move = ctx.ncommands > 0 ? NVGcommands.NVG_LINETO : NVGcommands.NVG_MOVETO;

            // Clamp angles
            da = a1 - a0;
            if (dir == NVGwinding.NVG_CW)
            {
                if (Math.Abs(da) >= Math.PI * 2)
                {
                    da = (float)(Math.PI * 2);
                }
                else
                {
                    while (da < 0.0f) da += (float)(Math.PI * 2);
                }
            }
            else
            {
                if (Math.Abs(da) >= Math.PI * 2)
                {
                    da = -(float)(Math.PI * 2);
                }
                else
                {
                    while (da > 0.0f) da -= (float)(Math.PI * 2);
                }
            }

            // Split arc into max 90 degree segments.
            ndivs = Math.Max(1, Math.Min((int)(Math.Abs(da) / (Math.PI * 0.5f) + 0.5f), 5));
            hda = (da / (float)ndivs) / 2.0f;
            kappa = (float)(Math.Abs(4.0f / 3.0f * (1.0f - Math.Cos(hda)) / Math.Sin(hda)));

            if (dir == NVGwinding.NVG_CCW)
                kappa = -kappa;

            nvals = 0;
            for (i = 0; i <= ndivs; i++)
            {
                a = a0 + da * (i / (float)ndivs);
                dx = (float)(Math.Cos(a));
                dy = (float)(Math.Sin(a));
                x = cx + dx * r;
                y = cy + dy * r;
                tanx = -dy * r * kappa;
                tany = dx * r * kappa;

                if (i == 0)
                {
                    vals[nvals++] = (float)move;
                    vals[nvals++] = x;
                    vals[nvals++] = y;
                }
                else
                {
                    vals[nvals++] = (float)NVGcommands.NVG_BEZIERTO;
                    vals[nvals++] = px + ptanx;
                    vals[nvals++] = py + ptany;
                    vals[nvals++] = x - tanx;
                    vals[nvals++] = y - tany;
                    vals[nvals++] = x;
                    vals[nvals++] = y;
                }
                px = x;
                py = y;
                ptanx = tanx;
                ptany = tany;
            }

            AppendCommands(ctx, vals, nvals);
        }

        /// <summary>
        /// Creates new rectangle shaped sub-path.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public static void Rect(NVGcontext ctx, float x, float y, float w, float h)
        {
            float[] vals = new float[]{
                    (float)NVGcommands.NVG_MOVETO, x,y,
                    (float)NVGcommands.NVG_LINETO, x,y+h,
                    (float)NVGcommands.NVG_LINETO, x+w,y+h,
                    (float)NVGcommands.NVG_LINETO, x+w,y,
                    (float)NVGcommands.NVG_CLOSE
            };
            AppendCommands(ctx, vals);
        }

        /// <summary>
        /// Creates new rounded rectangle shaped sub-path.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="r"></param>
        public static void RoundedRect(NVGcontext ctx, float x, float y, float w, float h, float r)
        {
            if (r < 0.1f)
            {
                Rect(ctx, x, y, w, h);
                return;
            }
            else
            {
                float rx = Math.Min(r, Math.Abs(w) * 0.5f) * Math.Sign(w), ry = Math.Min(r, Math.Abs(h) * 0.5f) * Math.Sign(h);
                float[] vals = new float[]{
                        (float)NVGcommands.NVG_MOVETO, x, y+ry,
                        (float)NVGcommands.NVG_LINETO, x, y+h-ry,
                        (float)NVGcommands.NVG_BEZIERTO, x, y+h-ry*(1-NVG_KAPPA90), x+rx*(1-NVG_KAPPA90), y+h, x+rx, y+h,
                        (float)NVGcommands.NVG_LINETO, x+w-rx, y+h,
                        (float)NVGcommands.NVG_BEZIERTO, x+w-rx*(1-NVG_KAPPA90), y+h, x+w, y+h-ry*(1-NVG_KAPPA90), x+w, y+h-ry,
                        (float)NVGcommands.NVG_LINETO, x+w, y+ry,
                        (float)NVGcommands.NVG_BEZIERTO, x+w, y+ry*(1-NVG_KAPPA90), x+w-rx*(1-NVG_KAPPA90), y, x+w-rx, y,
                        (float)NVGcommands.NVG_LINETO, x+rx, y,
                        (float)NVGcommands.NVG_BEZIERTO, x+rx*(1-NVG_KAPPA90), y, x, y+ry*(1-NVG_KAPPA90), x, y+ry,
                        (float)NVGcommands.NVG_CLOSE
                    };
                AppendCommands(ctx, vals);
            }
        }

        /// <summary>
        /// Creates new ellipse shaped sub-path.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="rx"></param>
        /// <param name="ry"></param>
        public static void Ellipse(NVGcontext ctx, float cx, float cy, float rx, float ry)
        {
            float[] vals = {
                            (float)NVGcommands.NVG_MOVETO, cx-rx, cy,
                            (float)NVGcommands.NVG_BEZIERTO, cx-rx, cy+ry*NVG_KAPPA90, cx-rx*NVG_KAPPA90, cy+ry, cx, cy+ry,
                            (float)NVGcommands.NVG_BEZIERTO, cx+rx*NVG_KAPPA90, cy+ry, cx+rx, cy+ry*NVG_KAPPA90, cx+rx, cy,
                            (float)NVGcommands.NVG_BEZIERTO, cx+rx, cy-ry*NVG_KAPPA90, cx+rx*NVG_KAPPA90, cy-ry, cx, cy-ry,
                            (float)NVGcommands.NVG_BEZIERTO, cx-rx*NVG_KAPPA90, cy-ry, cx-rx, cy-ry*NVG_KAPPA90, cx-rx, cy,
                            (float)NVGcommands.NVG_CLOSE
                        };
            AppendCommands(ctx, vals);
        }

        /// <summary>
        /// Creates new circle shaped sub-path. 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="r"></param>
        public static void Circle(NVGcontext ctx, float cx, float cy, float r)
        {
            Ellipse(ctx, cx, cy, r, r);
        }

        /// <summary>
        /// Fills the current path with current fill style.
        /// </summary>
        /// <param name="ctx"></param>
        public static void Fill(NVGcontext ctx)
        {
            NVGstate state = ctx.GetState();
            NVGpaint fillPaint = state.fill;

            ctx.FlattenPaths();
            if (ctx.params_.EdgeAntiAlias)
                ctx.ExpandFill(ctx.fringeWidth, NVGlineJoin.NVG_MITER, 2.4f);
            else
                ctx.ExpandFill(0.0f, NVGlineJoin.NVG_MITER, 2.4f);

            // Apply global alpha
            fillPaint.innerColor.W *= state.alpha;
            fillPaint.outerColor.W *= state.alpha;

            ctx.glctx.RenderFill(fillPaint, state.scissor, ctx.fringeWidth, ctx.cache.bounds, ctx.cache.paths, ctx.cache.npaths);

            // Count triangles
            for (int i = 0; i < ctx.cache.npaths; i++)
            {
                NVGpath path = ctx.cache.paths[i];
                ctx.fillTriCount += path.nfill - 2;
                ctx.fillTriCount += path.nstroke - 2;
                ctx.drawCallCount += 2;
            }
        }

        /// <summary>
        /// Fills the current path with current stroke style.
        /// </summary>
        /// <param name="ctx"></param>
        public static void Stroke(NVGcontext ctx)
        {
            NVGstate state = ctx.GetState();
            float scale = GetAverageScale(state.xform);
            float strokeWidth = Clampf(state.strokeWidth * scale, 0.0f, 200.0f);
            NVGpaint strokePaint = state.stroke;

            if (strokeWidth < ctx.fringeWidth)
            {
                // If the stroke width is less than pixel size, use alpha to emulate coverage.
                // Since coverage is area, scale by alpha*alpha.
                float alpha = Clampf(strokeWidth / ctx.fringeWidth, 0.0f, 1.0f);
                strokePaint.innerColor.Z *= alpha * alpha;
                strokePaint.outerColor.Z *= alpha * alpha;
                strokeWidth = ctx.fringeWidth;
            }

            // Apply global alpha
            strokePaint.innerColor.Z *= state.alpha;
            strokePaint.outerColor.Z *= state.alpha;

            ctx.FlattenPaths();

            if (ctx.params_.EdgeAntiAlias)
                ctx.ExpandStroke(strokeWidth * 0.5f + ctx.fringeWidth * 0.5f, state.lineCap, state.lineJoin, state.miterLimit);
            else
                ctx.ExpandStroke(strokeWidth * 0.5f, state.lineCap, state.lineJoin, state.miterLimit);

            ctx.glctx.RenderStroke(strokePaint, state.scissor, ctx.fringeWidth, strokeWidth, ctx.cache.paths);

            // Count triangles
            for (int i = 0; i < ctx.cache.npaths; i++)
            {
                NVGpath path = ctx.cache.paths[i];
                ctx.strokeTriCount += path.nstroke - 2;
                ctx.drawCallCount++;
            }
        }


        //
        // Text
        //
        // NanoVG allows you to load .ttf files and use the font to render text.
        //
        // The appearance of the text can be defined by setting the current text style
        // and by specifying the fill color. Common text and font settings such as
        // font size, letter spacing and text align are supported. Font blur allows you
        // to create simple text effects such as drop shadows.
        //
        // At render time the font face can be set based on the font handles or name.
        //
        // Font measure functions return values in local space, the calculations are
        // carried in the same resolution as the final rendering. This is done because
        // the text glyph positions are snapped to the nearest pixels sharp rendering.
        //
        // The local space means that values are not rotated or scale as per the current
        // transformation. For example if you set font size to 12, which would mean that
        // line height is 16, then regardless of the current scaling and rotation, the
        // returned line height is always 16. Some measures may vary because of the scaling
        // since aforementioned pixel snapping.
        //
        // While this may sound a little odd, the setup allows you to always render the
        // same way regardless of scaling. I.e. following works regardless of scaling:
        //
        //		const char* txt = "Text me up.";
        //		nvgTextBounds(vg, x,y, txt, NULL, bounds);
        //		nvgBeginPath(vg);
        //		nvgRoundedRect(vg, bounds[0],bounds[1], bounds[2]-bounds[0], bounds[3]-bounds[1]);
        //		nvgFill(vg);
        //
        // Note: currently only solid color fill is supported for text.

        // Creates font by loading it from the disk from specified file name.
        // Returns handle to the font.
        public int nvgCreateFont(NVGcontext ctx, string name, string filename)
        {
            throw new NotImplementedException();
        }

        // Creates image by loading it from the specified memory chunk.
        // Returns handle to the font.
        public int nvgCreateFontMem(NVGcontext ctx, string name, byte[] data, int ndata, int freeData)
        {
            throw new NotImplementedException();
        }

        // Finds a loaded font of specified name, and returns handle to it, or -1 if the font is not found.
        public int nvgFindFont(NVGcontext ctx, string name)
        {
            throw new NotImplementedException();
        }

        // Sets the font size of current text style.
        public void nvgFontSize(NVGcontext ctx, float size)
        {
            throw new NotImplementedException();
        }

        // Sets the blur of current text style.
        public void nvgFontBlur(NVGcontext ctx, float blur)
        {
            throw new NotImplementedException();
        }

        // Sets the letter spacing of current text style.
        public void nvgTextLetterSpacing(NVGcontext ctx, float spacing)
        {
            throw new NotImplementedException();
        }

        // Sets the proportional line height of current text style. The line height is specified as multiple of font size. 
        public void nvgTextLineHeight(NVGcontext ctx, float lineHeight)
        {
            throw new NotImplementedException();
        }

        // Sets the text align of current text style, see NVGalign for options.
        public void nvgTextAlign(NVGcontext ctx, int align)
        {
            throw new NotImplementedException();
        }

        // Sets the font face based on specified id of current text style.
        public void nvgFontFaceId(NVGcontext ctx, int font)
        {
            throw new NotImplementedException();
        }

        // Sets the font face based on specified name of current text style.
        public void nvgFontFace(NVGcontext ctx, string font)
        {
            throw new NotImplementedException();
        }

        // Draws text string at specified location. If end is specified only the sub-string up to the end is drawn.
        public float nvgText(NVGcontext ctx, float x, float y, string str, string end)
        {
            throw new NotImplementedException();
        }

        // Draws multi-line text string at specified location wrapped at the specified width. If end is specified only the sub-string up to the end is drawn.
        // White space is stripped at the beginning of the rows, the text is split at word boundaries or when new-line characters are encountered.
        // Words longer than the max width are slit at nearest character (i.e. no hyphenation).
        public void nvgTextBox(NVGcontext ctx, float x, float y, float breakRowWidth, string str, string end)
        {
            throw new NotImplementedException();
        }

        // Measures the specified text string. Parameter bounds should be a pointer to float[4],
        // if the bounding box of the text should be returned. The bounds value are [xmin,ymin, xmax,ymax]
        // Returns the horizontal advance of the measured text (i.e. where the next character should drawn).
        // Measured values are returned in local coordinate space.
        public float nvgTextBounds(NVGcontext ctx, float x, float y, string str, string end, Bound bounds)
        {
            throw new NotImplementedException();
        }

        // Measures the specified multi-text string. Parameter bounds should be a pointer to float[4],
        // if the bounding box of the text should be returned. The bounds value are [xmin,ymin, xmax,ymax]
        // Measured values are returned in local coordinate space.
        public void nvgTextBoxBounds(NVGcontext ctx, float x, float y, float breakRowWidth, string str, string end, Bound bounds)
        {
            throw new NotImplementedException();
        }

        // Calculates the glyph x positions of the specified text. If end is specified only the sub-string will be used.
        // Measured values are returned in local coordinate space.
        public int nvgTextGlyphPositions(NVGcontext ctx, float x, float y, string str, string end, NVGglyphPosition positions, int maxPositions)
        {
            throw new NotImplementedException();
        }

        // Returns the vertical metrics based on the current text style.
        // Measured values are returned in local coordinate space.
        public void nvgTextMetrics(NVGcontext ctx, out float ascender, out float descender, out float lineh)
        {
            throw new NotImplementedException();
        }

        // Breaks the specified text into lines. If end is specified only the sub-string will be used.
        // White space is stripped at the beginning of the rows, the text is split at word boundaries or when new-line characters are encountered.
        // Words longer than the max width are slit at nearest character (i.e. no hyphenation).
        public int nvgTextBreakLines(NVGcontext ctx, string str, string end, float breakRowWidth, NVGtextRow rows, int maxRows)
        {
            throw new NotImplementedException();
        }

        //
        // Internal Render API
        //



        // Constructor and destructor, called by the render back-end.
        internal NVGcontext nvgCreateInternal(NVGparams params_)
        {
            throw new NotImplementedException();
        }
        internal void nvgDeleteInternal(NVGcontext ctx)
        {
            throw new NotImplementedException();
        }

        internal NVGparams nvgInternalParams(NVGcontext ctx)
        {
            throw new NotImplementedException();
        }

        // Debug function to dump cached path data.
        internal void nvgDebugDumpPathCache(NVGcontext ctx)
        {
            throw new NotImplementedException();
        }

        private static void SetDevicePixelRatio(NVGcontext ctx, float ratio)
        {
            ctx.tessTol = 0.25f / ratio;
            ctx.distTol = 0.01f / ratio;
            ctx.fringeWidth = 1.0f / ratio;
            ctx.devicePxRatio = ratio;
        }
        private static void AppendCommands(NVGcontext ctx, float[] vals)
        {
            AppendCommands(ctx, vals, vals.Length);
        }

        private static void AppendCommands(NVGcontext ctx, float[] vals, int nvals)
        {
            NVGstate state = ctx.GetState();
            if (ctx.ncommands + nvals > ctx.ccommands)
            {
                int ccommands = ctx.ncommands + nvals + ctx.ccommands / 2;
                ctx.commands = NVGcontext.ResizeArray(ctx.commands, ccommands);
                ctx.ccommands = ccommands;
            }

            if ((NVGcommands)vals[0] != NVGcommands.NVG_CLOSE && (NVGcommands)vals[0] != NVGcommands.NVG_WINDING)
            {
                ctx.commandx = vals[nvals - 2];
                ctx.commandy = vals[nvals - 1];
            }

            // transform commands
            int i = 0;
            while (i < nvals)
            {
                int cmd = (int)vals[i];
                switch ((NVGcommands)cmd)
                {
                    case NVGcommands.NVG_MOVETO:
                        TransformPoint(out vals[i + 1], out vals[i + 2], state.xform, vals[i + 1], vals[i + 2]);
                        i += 3;
                        break;
                    case NVGcommands.NVG_LINETO:
                        TransformPoint(out vals[i + 1], out vals[i + 2], state.xform, vals[i + 1], vals[i + 2]);
                        i += 3;
                        break;
                    case NVGcommands.NVG_BEZIERTO:
                        TransformPoint(out vals[i + 1], out vals[i + 2], state.xform, vals[i + 1], vals[i + 2]);
                        TransformPoint(out vals[i + 3], out vals[i + 4], state.xform, vals[i + 3], vals[i + 4]);
                        TransformPoint(out vals[i + 5], out vals[i + 6], state.xform, vals[i + 5], vals[i + 6]);
                        i += 7;
                        break;
                    case NVGcommands.NVG_CLOSE:
                        i++;
                        break;
                    case NVGcommands.NVG_WINDING:
                        i += 2;
                        break;
                    default:
                        i++;
                        break;
                }
            }

            Array.Copy(vals, 0, ctx.commands, ctx.ncommands, nvals);
            ctx.ncommands += nvals;
        }

        private static float Clampf(float a, float mn, float mx) { return a < mn ? mn : (a > mx ? mx : a); }
        private static float Cross(float dx0, float dy0, float dx1, float dy1) { return dx1 * dy0 - dx0 * dy1; }
        static float GetAverageScale(float[] t)
        {
            float sx = (float)Math.Sqrt(t[0] * t[0] + t[2] * t[2]);
            float sy = (float)Math.Sqrt(t[1] * t[1] + t[3] * t[3]);
            return (sx + sy) * 0.5f;
        }

        private const int NVG_INIT_FONTIMAGE_SIZE = 512;
        private const int NVG_MAX_FONTIMAGE_SIZE = 2048;

        private const float NVG_KAPPA90 = 0.5522847493f;
    }
}
