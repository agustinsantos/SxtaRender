using Sxta.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoVG
{
    public struct Matrix23f
    { }

    public struct Bound
    {
        public float xmin;
        public float ymin;
        public float xmax;
        public float ymax;
    }

    public struct NVGpaint
    {
        float[] xform; // = new float[6];
        float[] extent; // = new float[2];
        float radius;
        float feather;
        Vector3f innerColor;
        Vector3f outerColor;
        int image;
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

    public enum NVGlineCap
    {
        NVG_BUTT,
        NVG_ROUND,
        NVG_SQUARE,
        NVG_BEVEL,
        NVG_MITER,
    }

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
        int str;	        // Position of the glyph in the input string.
        float x;			// The x-coordinate of the logical glyph position.
        float minx, maxx;	// The bounds of the glyph shape.
    }


    public struct NVGtextRow
    {
        int start;	// Pointer to the input text where the row starts.
        int end;	// Pointer to the input text where the row ends (one past the last character).
        int next;	// Pointer to the beginning of the next row.
        float width;		// Logical width of the row.
        float minx, maxx;	// Actual bounds of the row. Logical with and bounds can differ because of kerning and some parts over extending.
    }

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
        // Begin drawing a new frame
        // Calls to nanovg drawing API should be wrapped in nvgBeginFrame() & nvgEndFrame()
        // nvgBeginFrame() defines the size of the window to render to in relation currently
        // set viewport (i.e. glViewport on GL backends). Device pixel ration allows to
        // control the rendering on Hi-DPI devices.
        // For example, GLFW returns two dimension for an opened window: window size and
        // frame buffer size. In that case you would set windowWidth/Height to the window size
        // devicePixelRatio to: frameBufferWidth / windowWidth.
        public void nvgBeginFrame(NVGcontext ctx, int windowWidth, int windowHeight, float devicePixelRatio)
        {
            /*	printf("Tris: draws:%d  fill:%d  stroke:%d  text:%d  TOT:%d\n",
                    ctx.drawCallCount, ctx.fillTriCount, ctx.strokeTriCount, ctx.textTriCount,
                    ctx.fillTriCount+ctx.strokeTriCount+ctx.textTriCount);*/

            ctx.nstates = 0;
            nvgSave(ctx);
            nvgReset(ctx);

            SetDevicePixelRatio(ctx, devicePixelRatio);

            ctx.params_.renderViewport(ctx.params_.userPtr, windowWidth, windowHeight);

            ctx.drawCallCount = 0;
            ctx.fillTriCount = 0;
            ctx.strokeTriCount = 0;
            ctx.textTriCount = 0;
        }

        // Cancels drawing the current frame.
        public void nvgCancelFrame(NVGcontext ctx)
        {
            ctx.params_.renderCancel(ctx.params_.userPtr);
        }

        // Ends drawing flushing remaining render state.
        public void nvgEndFrame(NVGcontext ctx)
        {
            ctx.params_.renderFlush(ctx.params_.userPtr);
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
                for (i = j; i < NVG_MAX_FONTIMAGES; i++)
                    ctx.fontImages[i] = 0;
            }
        }

        //
        // Color utils
        //
        // Colors in NanoVG are stored as unsigned ints in ABGR format.

        // Returns a color value from red, green, blue values. Alpha will be set to 255 (1.0f).
        public Vector4ui nvgRGB(byte r, byte g, byte b)
        {
            return nvgRGBA(r, g, b, 255);
        }

        // Returns a color value from red, green, blue values. Alpha will be set to 1.0f.
        public Vector4ui nvgRGBf(float r, float g, float b)
        {
            return nvgRGBA(r, g, b, 1.0f);
        }


        // Returns a color value from red, green, blue and alpha values.
        public Vector4ui nvgRGBA(byte r, byte g, byte b, byte a)
        {
            return new Vector4ui(r, g, b, a);
        }


        // Returns a color value from red, green, blue and alpha values.
        public Vector4ui nvgRGBAf(float r, float g, float b, float a)
        {
            Vector4ui color;
            // Use longer initialization to suppress warning.
            color.X = (uint)(r / 255.0f);
            color.Y = (uint)(g / 255.0f);
            color.Z = (uint)(b / 255.0f);
            color.W = (uint)(a / 255.0f);
            return color;
        }

        // Linearly interpolates from color c0 to c1, and returns resulting color value.
        public Vector4ui nvgLerpRGBA(Vector3f c0, Vector3f c1, float u)
        {
            int i;
            float oneminu;
            Vector4ui cint;

            u = Clampf(u, 0.0f, 1.0f);
            oneminu = 1.0f - u;
            for (i = 0; i < 4; i++)
            {
                cint[i] = c0[i] * oneminu + c1[i] * u;
            }

            return cint;
        }

        // Sets transparency of a color value.
        public Vector4ui nvgTransRGBA(Vector4ui c0, byte a)
        {
            c0.W = (uint)(a / 255.0f);
            return c0;
        }

        // Sets transparency of a color value.
        public Vector3f nvgTransRGBAf(Vector3f c0, float a)
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

        // Pops and restores current render state.
        public void nvgRestore(NVGcontext ctx)
        {
            throw new NotImplementedException();
        }

        // Resets current render state to default values. Does not affect the render state stack.
        public void nvgReset(NVGcontext ctx)
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

        // Sets current stroke style to a solid color.
        public void nvgStrokeColor(NVGcontext ctx, Vector3f color)
        {
            throw new NotImplementedException();
        }

        // Sets current stroke style to a paint, which can be a one of the gradients or a pattern.
        public void nvgStrokePaint(NVGcontext ctx, NVGpaint paint)
        {
            throw new NotImplementedException();
        }

        // Sets current fill style to a solid color.
        public void nvgFillColor(NVGcontext ctx, Vector3f color)
        {
            throw new NotImplementedException();
        }

        // Sets current fill style to a paint, which can be a one of the gradients or a pattern.
        public void nvgFillPaint(NVGcontext ctx, NVGpaint paint)
        {
            throw new NotImplementedException();
        }

        // Sets the miter limit of the stroke style.
        // Miter limit controls when a sharp corner is beveled.
        public void nvgMiterLimit(NVGcontext ctx, float limit)
        {
            throw new NotImplementedException();
        }

        // Sets the stroke width of the stroke style.
        public void nvgStrokeWidth(NVGcontext ctx, float size)
        {
            throw new NotImplementedException();
        }

        // Sets how the end of the line (cap) is drawn,
        // Can be one of: NVG_BUTT (default), NVG_ROUND, NVG_SQUARE.
        public void nvgLineCap(NVGcontext ctx, int cap)
        {
            throw new NotImplementedException();
        }

        // Sets how sharp path corners are drawn.
        // Can be one of NVG_MITER (default), NVG_ROUND, NVG_BEVEL.
        public void nvgLineJoin(NVGcontext ctx, int join)
        {
            throw new NotImplementedException();
        }

        // Sets the transparency applied to all rendered shapes.
        // Already transparent paths will get proportionally more transparent as well.
        public void nvgGlobalAlpha(NVGcontext ctx, float alpha)
        {
            throw new NotImplementedException();
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

        // Sets the transform to identity matrix.
        public void nvgTransformIdentity(out Matrix23f dst)
        {
            throw new NotImplementedException();
        }

        // Sets the transform to translation matrix matrix.
        public void nvgTransformTranslate(out Matrix23f dst, float tx, float ty)
        {
            throw new NotImplementedException();
        }

        // Sets the transform to scale matrix.
        public void nvgTransformScale(out Matrix23f dst, float sx, float sy)
        {
            throw new NotImplementedException();
        }

        // Sets the transform to rotate matrix. Angle is specified in radians.
        public void nvgTransformRotate(out Matrix23f dst, float a)
        {
            throw new NotImplementedException();
        }

        // Sets the transform to skew-x matrix. Angle is specified in radians.
        public void nvgTransformSkewX(out Matrix23f dst, float a)
        {
            throw new NotImplementedException();
        }

        // Sets the transform to skew-y matrix. Angle is specified in radians.
        public void nvgTransformSkewY(out Matrix23f dst, float a)
        {
            throw new NotImplementedException();
        }

        // Sets the transform to the result of multiplication of two transforms, of A = A*B.
        public void nvgTransformMultiply(out Matrix23f dst, Matrix23f src)
        {
            throw new NotImplementedException();
        }

        // Sets the transform to the result of multiplication of two transforms, of A = B*A.
        public void nvgTransformPremultiply(out Matrix23f dst, Matrix23f src)
        {
            throw new NotImplementedException();
        }

        // Sets the destination to inverse of specified transform.
        // Returns 1 if the inverse could be calculated, else 0.
        public int nvgTransformInverse(out Matrix23f dst, Matrix23f src)
        {
            throw new NotImplementedException();
        }

        // Transform a point by given transform.
        public void nvgTransformPoint(out Matrix23f dstx, Matrix23f dsty, Matrix23f xform, float srcx, float srcy)
        {
            throw new NotImplementedException();
        }

        // Converts degrees to radians and vice versa.
        public float nvgDegToRad(float deg)
        {
            throw new NotImplementedException();
        }

        public float nvgRadToDeg(float rad)
        {
            throw new NotImplementedException();
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
        public void nvgImageSize(NVGcontext ctx, int image, out int w, out int h)
        {
            throw new NotImplementedException();
        }

        // Deletes created image.
        public void nvgDeleteImage(NVGcontext ctx, int image)
        {
            throw new NotImplementedException();
        }

        //
        // Paints
        //
        // NanoVG supports four types of paints: linear gradient, box gradient, radial gradient and image pattern.
        // These can be used as paints for strokes and fills.

        // Creates and returns a linear gradient. Parameters (sx,sy)-(ex,ey) specify the start and end coordinates
        // of the linear gradient, icol specifies the start color and ocol the end color.
        // The gradient is transformed by the current transform when it is passed to nvgFillPaint() or nvgStrokePaint().
        public NVGpaint nvgLinearGradient(NVGcontext ctx, float sx, float sy, float ex, float ey, Vector3f icol, Vector3f ocol)
        {
            throw new NotImplementedException();
        }

        // Creates and returns a box gradient. Box gradient is a feathered rounded rectangle, it is useful for rendering
        // drop shadows or highlights for boxes. Parameters (x,y) define the top-left corner of the rectangle,
        // (w,h) define the size of the rectangle, r defines the corner radius, and f feather. Feather defines how blurry
        // the border of the rectangle is. Parameter icol specifies the inner color and ocol the outer color of the gradient.
        // The gradient is transformed by the current transform when it is passed to nvgFillPaint() or nvgStrokePaint().
        public NVGpaint nvgBoxGradient(NVGcontext ctx, float x, float y, float w, float h, float r, float f, Vector3f icol, Vector3f ocol)
        {
            throw new NotImplementedException();
        }

        // Creates and returns a radial gradient. Parameters (cx,cy) specify the center, inr and outr specify
        // the inner and outer radius of the gradient, icol specifies the start color and ocol the end color.
        // The gradient is transformed by the current transform when it is passed to nvgFillPaint() or nvgStrokePaint().
        public NVGpaint nvgRadialGradient(NVGcontext ctx, float cx, float cy, float inr, float outr, Vector3f icol, Vector3f ocol)
        {
            throw new NotImplementedException();
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

        // Clears the current path and sub-paths.
        public void nvgBeginPath(NVGcontext ctx)
        {
            throw new NotImplementedException();
        }

        // Starts new sub-path with specified point as first point.
        public void nvgMoveTo(NVGcontext ctx, float x, float y)
        {
            throw new NotImplementedException();
        }

        // Adds line segment from the last point in the path to the specified point.
        public void nvgLineTo(NVGcontext ctx, float x, float y)
        {
            throw new NotImplementedException();
        }

        // Adds cubic bezier segment from last point in the path via two control points to the specified point.
        public void nvgBezierTo(NVGcontext ctx, float c1x, float c1y, float c2x, float c2y, float x, float y)
        {
            throw new NotImplementedException();
        }

        // Adds quadratic bezier segment from last point in the path via a control point to the specified point.
        public void nvgQuadTo(NVGcontext ctx, float cx, float cy, float x, float y)
        {
            throw new NotImplementedException();
        }

        // Adds an arc segment at the corner defined by the last path point, and two specified points.
        public void nvgArcTo(NVGcontext ctx, float x1, float y1, float x2, float y2, float radius)
        {
            throw new NotImplementedException();
        }

        // Closes current sub-path with a line segment.
        public void nvgClosePath(NVGcontext ctx)
        {
            throw new NotImplementedException();
        }

        // Sets the current sub-path winding, see NVGwinding and NVGsolidity. 
        public void nvgPathWinding(NVGcontext ctx, int dir)
        {
            throw new NotImplementedException();
        }

        // Creates new circle arc shaped sub-path. The arc center is at cx,cy, the arc radius is r,
        // and the arc is drawn from angle a0 to a1, and swept in direction dir (NVG_CCW, or NVG_CW).
        // Angles are specified in radians.
        public void nvgArc(NVGcontext ctx, float cx, float cy, float r, float a0, float a1, int dir)
        {
            throw new NotImplementedException();
        }

        // Creates new rectangle shaped sub-path.
        public void nvgRect(NVGcontext ctx, float x, float y, float w, float h)
        {
            throw new NotImplementedException();
        }

        // Creates new rounded rectangle shaped sub-path.
        public void nvgRoundedRect(NVGcontext ctx, float x, float y, float w, float h, float r)
        {
            throw new NotImplementedException();
        }

        // Creates new ellipse shaped sub-path.
        public void nvgEllipse(NVGcontext ctx, float cx, float cy, float rx, float ry)
        {
            throw new NotImplementedException();
        }

        // Creates new circle shaped sub-path. 
        public void nvgCircle(NVGcontext ctx, float cx, float cy, float r)
        {
            throw new NotImplementedException();
        }

        // Fills the current path with current fill style.
        public void nvgFill(NVGcontext ctx)
        {
            throw new NotImplementedException();
        }

        // Fills the current path with current stroke style.
        public void nvgStroke(NVGcontext ctx)
        {
            throw new NotImplementedException();
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

        private const int NVG_INIT_FONTIMAGE_SIZE = 512;
        private const int NVG_MAX_FONTIMAGE_SIZE = 2048;
        private const int NVG_MAX_FONTIMAGES = 4;

        private const int NVG_INIT_COMMANDS_SIZE = 256;
        private const int NVG_INIT_POINTS_SIZE = 128;
        private const int NVG_INIT_PATHS_SIZE = 16;
        private const int NVG_INIT_VERTS_SIZE = 256;
        private const int NVG_MAX_STATES = 32;

        private const float NVG_KAPPA90 = 0.5522847493f;
    }
}
