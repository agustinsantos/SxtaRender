using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Sxta.Render
{

    /// <summary>
    /// A GPU buffer usage.
    /// </summary>
    public enum BufferUsage
    {
        STREAM_DRAW,
        STREAM_READ,
        STREAM_COPY,
        STATIC_DRAW,
        STATIC_READ,
        STATIC_COPY,
        DYNAMIC_DRAW,
        DYNAMIC_READ,
        DYNAMIC_COPY
    }


    /// <summary>
    /// A GPU buffer access mode.
    /// </summary>
    public enum BufferAccess
    {
        READ_ONLY,
        WRITE_ONLY,
        READ_WRITE
    }


    /// <summary>
    /// A vertex attribute component type.
    /// </summary>
    public enum AttributeType
    {
        A8I,
        A8UI,
        A16I,
        A16UI,
        A32I,
        A32UI,
        A16F,
        A32F,
        A64F,
        A32I_2_10_10_10_REV,
        A32UI_2_10_10_10_REV,
        A32I_FIXED
    }


    /// <summary>
    /// A mesh topology.
    /// </summary>
    public enum MeshMode
    {
        POINTS,
        LINE_STRIP,
        LINE_LOOP,
        LINES,
        TRIANGLE_STRIP,
        TRIANGLE_FAN,
        TRIANGLES,
        LINES_ADJACENCY,
        LINE_STRIP_ADJACENCY,
        TRIANGLES_ADJACENCY,
        TRIANGLE_STRIP_ADJACENCY,
        PATCHES
    }


    /// <summary>
    /// A mesh usage.
    /// </summary>
    public enum MeshUsage
    {
        CPU,
        GPU_STATIC,
        GPU_DYNAMIC,
        GPU_STREAM
    }


    /// <summary>
    /// A cube face identifier.
    /// </summary>
    public enum CubeFace
    {
        POSITIVE_X = 0,
        NEGATIVE_X = 1,
        POSITIVE_Y = 2,
        NEGATIVE_Y = 3,
        POSITIVE_Z = 4,
        NEGATIVE_Z = 5
    }


    /// <summary>
    /// A texture internal format on GPU. Specifies pixel components and their type.
    /// </summary>
    public enum TextureInternalFormat
    {
        R8 = PixelInternalFormat.R8,
        R8_SNORM = PixelInternalFormat.R8Snorm,
        R16 = PixelInternalFormat.R16,
        R16_SNORM = PixelInternalFormat.R16Snorm,
        RG8 = PixelInternalFormat.Rg8,
        RG8_SNORM = PixelInternalFormat.Rg8Snorm,
        RG16 = PixelInternalFormat.Rg16,
        RG16_SNORM = PixelInternalFormat.Rg16Snorm,
        R3_G3_B2 = PixelInternalFormat.R3G3B2,
        RGB4 = PixelInternalFormat.Rgb4,
        RGB5 = PixelInternalFormat.Rgb5,
        RGB8 = PixelInternalFormat.Rgb8,
        RGB8_SNORM = PixelInternalFormat.Rgb8Snorm,
        RGB10 = PixelInternalFormat.Rgb10,
        RGB12 = PixelInternalFormat.Rgb12,
        RGB16 = PixelInternalFormat.Rgb16,
        RGB16_SNORM = PixelInternalFormat.Rgb16Snorm,
        RGBA2 = PixelInternalFormat.Rgba2,
        RGBA4 = PixelInternalFormat.Rgba4,
        RGB5_A1 = PixelInternalFormat.Rgb5A1,
        RGBA8 = PixelInternalFormat.Rgba8,
        RGBA8_SNORM = PixelInternalFormat.Rgba8Snorm,
        RGB10_A2 = PixelInternalFormat.Rgb10A2,
        RGB10_A2UI = PixelInternalFormat.Rgb10A2ui,
        RGBA12 = PixelInternalFormat.Rgba12,
        RGBA16 = PixelInternalFormat.Rgba16,
        RGBA16_SNORM = PixelInternalFormat.Rgba16Snorm,
        SRGB8 = PixelInternalFormat.Srgb8,
        SRGB8_ALPHA8 = PixelInternalFormat.Srgb8Alpha8,
        R16F = PixelInternalFormat.R16f,
        RG16F = PixelInternalFormat.Rg16f,
        RGB16F = PixelInternalFormat.Rgb16f,
        RGBA16F = PixelInternalFormat.Rgba16f,
        R32F = PixelInternalFormat.R32f,
        RG32F = PixelInternalFormat.Rg32f,
        RGB32F = PixelInternalFormat.Rgb32f,
        RGBA32F = PixelInternalFormat.Rgba32f,
        R11F_G11F_B10F = PixelInternalFormat.R11fG11fB10f,
        RGB9_E5 = PixelInternalFormat.Rgb9E5,
        R8I = PixelInternalFormat.R8i,
        R8UI = PixelInternalFormat.R8ui,
        R16I = PixelInternalFormat.R16i,
        R16UI = PixelInternalFormat.R16ui,
        R32I = PixelInternalFormat.R32i,
        R32UI = PixelInternalFormat.R32ui,
        RG8I = PixelInternalFormat.Rg8i,
        RG8UI = PixelInternalFormat.Rg8ui,
        RG16I = PixelInternalFormat.Rg16i,
        RG16UI = PixelInternalFormat.Rg16ui,
        RG32I = PixelInternalFormat.Rg32i,
        RG32UI = PixelInternalFormat.Rg32ui,
        RGB8I = PixelInternalFormat.Rgb8i,
        RGB8UI = PixelInternalFormat.Rgb8ui,
        RGB16I = PixelInternalFormat.Rgb16i,
        RGB16UI = PixelInternalFormat.Rgb16ui,
        RGB32I = PixelInternalFormat.Rgb32i,
        RGB32UI = PixelInternalFormat.Rgb32ui,
        RGBA8I = PixelInternalFormat.Rgba8i,
        RGBA8UI = PixelInternalFormat.Rgba8ui,
        RGBA16I = PixelInternalFormat.Rgba16i,
        RGBA16UI = PixelInternalFormat.Rgba16ui,
        RGBA32I = PixelInternalFormat.Rgba32i,
        RGBA32UI = PixelInternalFormat.Rgba32ui,
        DEPTH_COMPONENT16 = PixelInternalFormat.DepthComponent16,
        DEPTH_COMPONENT24 = PixelInternalFormat.DepthComponent24,
        DEPTH_COMPONENT32F = PixelInternalFormat.DepthComponent32f,
        DEPTH32F_STENCIL8 = PixelInternalFormat.Depth32fStencil8,
        DEPTH24_STENCIL8 = PixelInternalFormat.Depth24Stencil8,
        COMPRESSED_RED = PixelInternalFormat.CompressedRed,
        COMPRESSED_RG = PixelInternalFormat.CompressedRg,
        COMPRESSED_RGB = PixelInternalFormat.CompressedRgb,
        COMPRESSED_RGBA = PixelInternalFormat.CompressedRgba,
        COMPRESSED_SRGB = PixelInternalFormat.CompressedSrgb,
        COMPRESSED_RED_RGTC1 = PixelInternalFormat.CompressedRedRgtc1,
        COMPRESSED_SIGNED_RED_RGTC1 = PixelInternalFormat.CompressedSignedRedRgtc1,
        COMPRESSED_SIGNED_RG_RGTC2 = PixelInternalFormat.CompressedSignedRgRgtc2,
        COMPRESSED_RG_RGTC2 = PixelInternalFormat.CompressedRgRgtc2,
        COMPRESSED_RGBA_BPTC_UNORM_ARB = PixelInternalFormat.CompressedRgbaBptcUnorm,
        COMPRESSED_SRGB_ALPHA_BPTC_UNORM_ARB = PixelInternalFormat.CompressedSrgbAlpha,
        COMPRESSED_RGB_BPTC_SIGNED_FLOAT_ARB = PixelInternalFormat.CompressedRgbBptcSignedFloat,
        COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT_ARB = PixelInternalFormat.CompressedRgbBptcUnsignedFloat,
        COMPRESSED_RGB_S3TC_DXT1_EXT = PixelInternalFormat.CompressedRgbS3tcDxt1Ext,
        COMPRESSED_RGBA_S3TC_DXT1_EXT = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext,
        COMPRESSED_RGBA_S3TC_DXT3_EXT = PixelInternalFormat.CompressedRgbaS3tcDxt3Ext,
        COMPRESSED_RGBA_S3TC_DXT5_EXT = PixelInternalFormat.CompressedRgbaS3tcDxt5Ext
    }


    /// <summary>
    /// A texture format. Specifies texture components to be read or written.
    /// </summary>
    public enum TextureFormat
    {
        COLOR_INDEX = PixelFormat.ColorIndex,
        STENCIL_INDEX = PixelFormat.StencilIndex,
        DEPTH_COMPONENT = PixelFormat.DepthComponent,
        DEPTH_STENCIL = PixelFormat.DepthStencil,
        RED = PixelFormat.Red,
        GREEN = PixelFormat.Green,
        BLUE = PixelFormat.Blue,
        RG = PixelFormat.Rg,
        RGB = PixelFormat.Rgb,
        RGBA = PixelFormat.Rgba,
        BGR = PixelFormat.Bgr,
        BGRA = PixelFormat.Bgra,
        RED_INTEGER = PixelFormat.RedInteger,
        BLUE_INTEGER = PixelFormat.BlueInteger,
        GREEN_INTEGER = PixelFormat.GreenInteger,
        RG_INTEGER = PixelFormat.RgInteger,
        RGB_INTEGER = PixelFormat.RgbInteger,
        RGBA_INTEGER = PixelFormat.RgbaInteger,
        BGR_INTEGER = PixelFormat.BgrInteger,
        BGRA_INTEGER = PixelFormat.BgraInteger
    }


    /// <summary>
    /// A pixel component type. Specifies a type for reading or writing texture data.
    /// </summary>
    public enum PixelType : uint
    {
        UNSIGNED_BYTE, /// 
		BYTE,
        UNSIGNED_SHORT,
        SHORT,
        UNSIGNED_INT,
        INT,
        HALF,
        FLOAT,
        UNSIGNED_BYTE_3_3_2,
        UNSIGNED_BYTE_2_3_3_REV,
        UNSIGNED_SHORT_5_6_5,
        UNSIGNED_SHORT_5_6_5_REV,
        UNSIGNED_SHORT_4_4_4_4,
        UNSIGNED_SHORT_4_4_4_4_REV,
        UNSIGNED_SHORT_5_5_5_1,
        UNSIGNED_SHORT_1_5_5_5_REV,
        UNSIGNED_INT_8_8_8_8,
        UNSIGNED_INT_8_8_8_8_REV,
        UNSIGNED_INT_10_10_10_2,
        UNSIGNED_INT_2_10_10_10_REV,
        UNSIGNED_INT_24_8,
        UNSIGNED_INT_10F_11F_11F_REV,
        UNSIGNED_INT_5_9_9_9_REV,
        FLOAT_32_UNSIGNED_INT_24_8_REV
    }


    /// <summary>
    /// A texture wrap mode for texture access.
    /// </summary>
    public enum TextureWrap
    {
        CLAMP_TO_EDGE,
        CLAMP_TO_BORDER,
        REPEAT,
        MIRRORED_REPEAT
    }


    /// <summary>
    /// A texture filter mode for texture minification and magnification.
    /// </summary>
    public enum TextureFilter
    {
        NEAREST,
        LINEAR,
        NEAREST_MIPMAP_NEAREST,
        NEAREST_MIPMAP_LINEAR,
        LINEAR_MIPMAP_NEAREST,
        LINEAR_MIPMAP_LINEAR
    }
    ;


    /// <summary>
    /// A comparison function for texture compare, stencil test or depth test.
    /// </summary>
    public enum Function
    {
        LEQUAL,
        GEQUAL,
        LESS,
        GREATER,
        EQUAL,
        NOTEQUAL,
        ALWAYS,
        NEVER
    }
    ;


    /// <summary>
    /// A framebuffer attachment point.
    /// </summary>
    [Flags]
    public enum BufferId
    {
        NOVALUE = -1,
        DEFAULT = 0,
        COLOR0 = 1,
        COLOR1 = 2,
        COLOR2 = 4,
        COLOR3 = 8,
        COLOR4 = 16,
        COLOR5 = 32,
        COLOR6 = 64,
        COLOR7 = 128,
        STENCIL = 256,
        DEPTH = 512 ///< depth
    }
    ;


    /// <summary>
    /// A polygon drawing mode.
    /// </summary>
    public enum PolygonMode
    {
        CULL,
        POINT,
        LINE,
        FILL
    }
    ;


    /// <summary>
    /// A stencil operation to perform if the stencil test passes or fails.
    /// </summary>
    public enum StencilOperation
    {
        KEEP,
        RESET,
        REPLACE,
        INCR,
        DECR,
        INVERT,
        INCR_WRAP,
        DECR_WRAP
    }
    ;


    /// <summary>
    /// An equation to blend the fragment colors with the framebuffer colors.
    /// </summary>
    public enum BlendEquation
    {
        ADD,
        SUBTRACT,
        REVERSE_SUBTRACT,
        MIN,
        MAX
    }
    ;


    /// <summary>
    /// A blend equation argument for blending pixels.
    /// </summary>
    public enum BlendArgument
    {
        ZERO,
        ONE,
        SRC_COLOR,
        ONE_MINUS_SRC_COLOR,
        DST_COLOR,
        ONE_MINUS_DST_COLOR,
        SRC_ALPHA,
        ONE_MINUS_SRC_ALPHA,
        DST_ALPHA,
        ONE_MINUS_DST_ALPHA,
        CONSTANT_COLOR,
        ONE_MINUS_CONSTANT_COLOR,
        CONSTANT_ALPHA,
        ONE_MINUS_CONSTANT_ALPHA
    }
    ;


    /// <summary>
    /// A logical operation to combine fragment colors with the framebuffer colors.
    /// </summary>
    public enum LogicOperation
    {
        CLEAR,
        AND,
        AND_REVERSE,
        COPY,
        AND_INVERTED,
        NOOP,
        XOR,
        OR,
        NOR,
        EQUIV,
        NOT,
        OR_REVERSE,
        COPY_INVERTED,
        OR_INVERTED,
        NAND,
        SET
    }
    ;


    /// <summary>
    /// A shader type.
    /// </summary>
    public enum Stage
    {
        VERTEX,
        TESSELATION_CONTROL,
        TESSELATION_EVALUATION,
        GEOMETRY,
        FRAGMENT
    }
    ;


    /// <summary>
    /// A uniform type.
    /// </summary>
    public enum UniformType
    {
        VEC1F,
        VEC2F,
        VEC3F,
        VEC4F,
        VEC1D,
        VEC2D,
        VEC3D,
        VEC4D,
        VEC1I,
        VEC2I,
        VEC3I,
        VEC4I,
        VEC1UI,
        VEC2UI,
        VEC3UI,
        VEC4UI,
        VEC1B,
        VEC2B,
        VEC3B,
        VEC4B,
        MAT2F,
        MAT3F,
        MAT4F,
        MAT2x3F,
        MAT2x4F,
        MAT3x2F,
        MAT3x4F,
        MAT4x2F,
        MAT4x3F,
        MAT2D,
        MAT3D,
        MAT4D,
        MAT2x3D,
        MAT2x4D,
        MAT3x2D,
        MAT3x4D,
        MAT4x2D,
        MAT4x3D,
        SAMPLER_1D,
        SAMPLER_2D,
        SAMPLER_3D,
        SAMPLER_CUBE,
        SAMPLER_1D_ARRAY,
        SAMPLER_2D_ARRAY,
        SAMPLER_CUBE_MAP_ARRAY,
        SAMPLER_2D_MULTISAMPLE,
        SAMPLER_2D_MULTISAMPLE_ARRAY,
        SAMPLER_BUFFER,
        SAMPLER_2D_RECT,
        INT_SAMPLER_1D,
        INT_SAMPLER_2D,
        INT_SAMPLER_3D,
        INT_SAMPLER_CUBE,
        INT_SAMPLER_1D_ARRAY,
        INT_SAMPLER_2D_ARRAY,
        INT_SAMPLER_CUBE_MAP_ARRAY,
        INT_SAMPLER_2D_MULTISAMPLE,
        INT_SAMPLER_2D_MULTISAMPLE_ARRAY,
        INT_SAMPLER_BUFFER,
        INT_SAMPLER_2D_RECT,
        UNSIGNED_INT_SAMPLER_1D,
        UNSIGNED_INT_SAMPLER_2D,
        UNSIGNED_INT_SAMPLER_3D,
        UNSIGNED_INT_SAMPLER_CUBE,
        UNSIGNED_INT_SAMPLER_1D_ARRAY,
        UNSIGNED_INT_SAMPLER_2D_ARRAY,
        UNSIGNED_INT_SAMPLER_CUBE_MAP_ARRAY,
        UNSIGNED_INT_SAMPLER_2D_MULTISAMPLE,
        UNSIGNED_INT_SAMPLER_2D_MULTISAMPLE_ARRAY,
        UNSIGNED_INT_SAMPLER_BUFFER,
        UNSIGNED_INT_SAMPLER_2D_RECT
    }
    ;

    /*
     * An asynchronous query type.
     */
    public enum QueryType
    {
        PRIMITIVES_GENERATED,
        TRANSFORM_FEEDBACK_PRIMITIVES_WRITTEN,
        SAMPLES_PASSED,
        ANY_SAMPLES_PASSED,
        TIME_STAMP
    }
    ;


    /// <summary>
    /// An asynchronous query mode.
    /// </summary>
    public enum QueryMode
    {
        WAIT,
        NO_WAIT,
        REGION_WAIT,
        REGION_NO_WAIT
    }

    /// <summary>
    /// Provides utility methods to work with OpenTK enumerations.
    /// </summary>
    public static class EnumConversion
    {
        public static BufferUsageHint getBufferUsage(BufferUsage u)
        {
            switch (u)
            {
                case BufferUsage.STREAM_DRAW:
                    return BufferUsageHint.StreamDraw;
                case BufferUsage.STREAM_READ:
                    return BufferUsageHint.StreamRead;
                case BufferUsage.STREAM_COPY:
                    return BufferUsageHint.StreamCopy;
                case BufferUsage.STATIC_DRAW:
                    return BufferUsageHint.StaticDraw;
                case BufferUsage.STATIC_READ:
                    return BufferUsageHint.StaticRead;
                case BufferUsage.STATIC_COPY:
                    return BufferUsageHint.StaticCopy;
                case BufferUsage.DYNAMIC_DRAW:
                    return BufferUsageHint.DynamicDraw;
                case BufferUsage.DYNAMIC_READ:
                    return BufferUsageHint.DynamicRead;
                case BufferUsage.DYNAMIC_COPY:
                    return BufferUsageHint.DynamicCopy;
            }
            Debug.Assert(false);
            throw new Exception("Wrong BufferUsage" + u);
        }

        public static OpenTK.Graphics.OpenGL.BufferAccess getBufferAccess(BufferAccess a)
        {
            switch (a)
            {
                case BufferAccess.READ_ONLY:
                    return OpenTK.Graphics.OpenGL.BufferAccess.ReadOnly;
                case BufferAccess.WRITE_ONLY:
                    return OpenTK.Graphics.OpenGL.BufferAccess.WriteOnly;
                case BufferAccess.READ_WRITE:
                    return OpenTK.Graphics.OpenGL.BufferAccess.ReadWrite;
            }
            Debug.Assert(false);
            throw new Exception("Wrong BufferAccess" + a);
        }

        /// <summary>
		/// Returns the size of a type (which depends on its type: byte, int, float, etc).
        /// Except for the packed
        /// formats A32I_2_10_10_10_REV and A32UI_2_10_10_10_REV.
		/// </summary>
		/// <returns>
		/// The attribute size.
		/// </returns>
        public static int getAttributeSize(AttributeType type)
        {
            int size = 0;
            switch (type)
            {
                case AttributeType.A8I:
                case AttributeType.A8UI:
                    size = 1;
                    break;
                case AttributeType.A16I:
                case AttributeType.A16UI:
                case AttributeType.A16F:
                    size = 2;
                    break;
                case AttributeType.A32I:
                case AttributeType.A32UI:
                case AttributeType.A32F:
                    size = 4;
                    break;
                case AttributeType.A64F:
                    size = 8;
                    break;
                case AttributeType.A32I_2_10_10_10_REV:
                case AttributeType.A32UI_2_10_10_10_REV:
                    return 4;
            }
            return size;
        }
        public static DrawElementsType getDrawElementType(AttributeType t)
        {
            switch (t)
            {
                case AttributeType.A8UI:
                    return DrawElementsType.UnsignedByte;
                case AttributeType.A16UI:
                    return DrawElementsType.UnsignedShort;
                case AttributeType.A32UI:
                    return DrawElementsType.UnsignedInt;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static VertexAttribPointerType getAttributeType(AttributeType t)
        {
            switch (t)
            {
                case AttributeType.A8I:
                    return VertexAttribPointerType.Byte;
                case AttributeType.A8UI:
                    return VertexAttribPointerType.UnsignedByte;
                case AttributeType.A16I:
                    return VertexAttribPointerType.Short;
                case AttributeType.A16UI:
                    return VertexAttribPointerType.UnsignedShort;
                case AttributeType.A32I:
                    return VertexAttribPointerType.Int;
                case AttributeType.A32UI:
                    return VertexAttribPointerType.UnsignedInt;

                case AttributeType.A16F:
                    return VertexAttribPointerType.HalfFloat;
                case AttributeType.A32F:
                    return VertexAttribPointerType.Float;
                case AttributeType.A64F:
                    return VertexAttribPointerType.Double;
                case AttributeType.A32I_2_10_10_10_REV:
                    return VertexAttribPointerType.Int2101010Rev;
                case AttributeType.A32UI_2_10_10_10_REV:
                    return VertexAttribPointerType.UnsignedInt2101010Rev;
                case AttributeType.A32I_FIXED:
                    return VertexAttribPointerType.Fixed;

            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static VertexAttribIPointerType getIAttributeType(AttributeType t)
        {
            switch (t)
            {
                case AttributeType.A8I:
                    return VertexAttribIPointerType.Byte;
                case AttributeType.A8UI:
                    return VertexAttribIPointerType.UnsignedByte;
                case AttributeType.A16I:
                    return VertexAttribIPointerType.Short;
                case AttributeType.A16UI:
                    return VertexAttribIPointerType.UnsignedShort;
                case AttributeType.A32I:
                    return VertexAttribIPointerType.Int;
                case AttributeType.A32UI:
                    return VertexAttribIPointerType.UnsignedInt;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static VertexAttribDPointerType getDAttributeType(AttributeType t)
        {
            switch (t)
            {
                case AttributeType.A64F:
                    return VertexAttribDPointerType.Double;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static BeginMode getMeshMode(MeshMode m)
        {
            switch (m)
            {
                case MeshMode.POINTS:
                    return BeginMode.Points;
                case MeshMode.LINE_STRIP:
                    return BeginMode.LineStrip;
                case MeshMode.LINE_LOOP:
                    return BeginMode.LineLoop;
                case MeshMode.LINES:
                    return BeginMode.Lines;
                case MeshMode.TRIANGLE_STRIP:
                    return BeginMode.TriangleStrip;
                case MeshMode.TRIANGLE_FAN:
                    return BeginMode.TriangleFan;
                case MeshMode.TRIANGLES:
                    return BeginMode.Triangles;
                case MeshMode.LINES_ADJACENCY:
                    return BeginMode.LinesAdjacency;
                case MeshMode.LINE_STRIP_ADJACENCY:
                    return BeginMode.LineStripAdjacency;
                case MeshMode.TRIANGLES_ADJACENCY:
                    return BeginMode.TrianglesAdjacency;
                case MeshMode.TRIANGLE_STRIP_ADJACENCY:
                    return BeginMode.TriangleStripAdjacency;
                case MeshMode.PATCHES:
                    return BeginMode.Patches;
            }
            Debug.Assert(false);
            throw new Exception();
        }


        public static PrimitiveType getPrimitiveType(MeshMode m)
        {
            switch (m)
            {
                case MeshMode.POINTS:
                    return PrimitiveType.Points;
                case MeshMode.LINE_STRIP:
                    return PrimitiveType.LineStrip;
                case MeshMode.LINE_LOOP:
                    return PrimitiveType.LineLoop;
                case MeshMode.LINES:
                    return PrimitiveType.Lines;
                case MeshMode.TRIANGLE_STRIP:
                    return PrimitiveType.TriangleStrip;
                case MeshMode.TRIANGLE_FAN:
                    return PrimitiveType.TriangleFan;
                case MeshMode.TRIANGLES:
                    return PrimitiveType.Triangles;
                case MeshMode.LINES_ADJACENCY:
                    return PrimitiveType.LinesAdjacency;
                case MeshMode.LINE_STRIP_ADJACENCY:
                    return PrimitiveType.LineStripAdjacency;
                case MeshMode.TRIANGLES_ADJACENCY:
                    return PrimitiveType.TrianglesAdjacency;
                case MeshMode.TRIANGLE_STRIP_ADJACENCY:
                    return PrimitiveType.TriangleStripAdjacency;
                case MeshMode.PATCHES:
                    return PrimitiveType.Patches;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static TextureTarget getCubeFace(CubeFace f)
        {
            switch (f)
            {
                case CubeFace.POSITIVE_X:
                    return TextureTarget.TextureCubeMapPositiveX;
                case CubeFace.NEGATIVE_X:
                    return TextureTarget.TextureCubeMapNegativeX;
                case CubeFace.POSITIVE_Y:
                    return TextureTarget.TextureCubeMapPositiveY;
                case CubeFace.NEGATIVE_Y:
                    return TextureTarget.TextureCubeMapNegativeY;
                case CubeFace.POSITIVE_Z:
                    return TextureTarget.TextureCubeMapPositiveZ;
                case CubeFace.NEGATIVE_Z:
                    return TextureTarget.TextureCubeMapNegativeZ;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static string getTextureInternalFormatName(TextureInternalFormat f)
        {
            switch (f)
            {
                case TextureInternalFormat.R8:
                    return "R8";
                case TextureInternalFormat.R8_SNORM:
                    return "R8_SNORM";
                case TextureInternalFormat.R16:
                    return "R16";
                case TextureInternalFormat.R16_SNORM:
                    return "R16_SNORM";
                case TextureInternalFormat.RG8:
                    return "RG8";
                case TextureInternalFormat.RG8_SNORM:
                    return "RG8_SNORM";
                case TextureInternalFormat.RG16:
                    return "RG16";
                case TextureInternalFormat.RG16_SNORM:
                    return "RG16_SNORM";
                case TextureInternalFormat.R3_G3_B2:
                    return "R3_G3_B2";
                case TextureInternalFormat.RGB4:
                    return "RGB4";
                case TextureInternalFormat.RGB5:
                    return "RGB5";
                case TextureInternalFormat.RGB8:
                    return "RGB8";
                case TextureInternalFormat.RGB8_SNORM:
                    return "RGB8_SNORM";
                case TextureInternalFormat.RGB10:
                    return "RGB10";
                case TextureInternalFormat.RGB12:
                    return "RGB12";
                case TextureInternalFormat.RGB16:
                    return "RGB16";
                case TextureInternalFormat.RGB16_SNORM:
                    return "RGB16_SNORM";
                case TextureInternalFormat.RGBA2:
                    return "RGBA2";
                case TextureInternalFormat.RGBA4:
                    return "RGBA4";
                case TextureInternalFormat.RGB5_A1:
                    return "RGB5_A1";
                case TextureInternalFormat.RGBA8:
                    return "RGBA8";
                case TextureInternalFormat.RGBA8_SNORM:
                    return "RGBA8_SNORM";
                case TextureInternalFormat.RGB10_A2:
                    return "RGB10_A2";
                case TextureInternalFormat.RGB10_A2UI:
                    return "RGB10_A2UI";
                case TextureInternalFormat.RGBA12:
                    return "RGBA12";
                case TextureInternalFormat.RGBA16:
                    return "RGBA16";
                case TextureInternalFormat.RGBA16_SNORM:
                    return "RGBA16_SNORM";
                case TextureInternalFormat.SRGB8:
                    return "SRGB8";
                case TextureInternalFormat.SRGB8_ALPHA8:
                    return "SRGB8_ALPHA8";
                case TextureInternalFormat.R16F:
                    return "R16F";
                case TextureInternalFormat.RG16F:
                    return "RG16F";
                case TextureInternalFormat.RGB16F:
                    return "RGB16F";
                case TextureInternalFormat.RGBA16F:
                    return "RGBA16F";
                case TextureInternalFormat.R32F:
                    return "R32F";
                case TextureInternalFormat.RG32F:
                    return "RG32F";
                case TextureInternalFormat.RGB32F:
                    return "RGB32F";
                case TextureInternalFormat.RGBA32F:
                    return "RGBA32F";
                case TextureInternalFormat.R11F_G11F_B10F:
                    return "R11F_G11F_B10F";
                case TextureInternalFormat.RGB9_E5:
                    return "RGB9_E5";
                case TextureInternalFormat.R8I:
                    return "R8I";
                case TextureInternalFormat.R8UI:
                    return "R8UI";
                case TextureInternalFormat.R16I:
                    return "R16I";
                case TextureInternalFormat.R16UI:
                    return "R16UI";
                case TextureInternalFormat.R32I:
                    return "R32I";
                case TextureInternalFormat.R32UI:
                    return "R32UI";
                case TextureInternalFormat.RG8I:
                    return "RG8I";
                case TextureInternalFormat.RG8UI:
                    return "RG8UI";
                case TextureInternalFormat.RG16I:
                    return "RG16I";
                case TextureInternalFormat.RG16UI:
                    return "RG16UI";
                case TextureInternalFormat.RG32I:
                    return "RG32I";
                case TextureInternalFormat.RG32UI:
                    return "RG32UI";
                case TextureInternalFormat.RGB8I:
                    return "RGB8I";
                case TextureInternalFormat.RGB8UI:
                    return "RGB8UI";
                case TextureInternalFormat.RGB16I:
                    return "RGB16I";
                case TextureInternalFormat.RGB16UI:
                    return "RGB16UI";
                case TextureInternalFormat.RGB32I:
                    return "RGB32I";
                case TextureInternalFormat.RGB32UI:
                    return "RGB32UI";
                case TextureInternalFormat.RGBA8I:
                    return "RGBA8I";
                case TextureInternalFormat.RGBA8UI:
                    return "RGBA8UI";
                case TextureInternalFormat.RGBA16I:
                    return "RGBA16I";
                case TextureInternalFormat.RGBA16UI:
                    return "RGBA16UI";
                case TextureInternalFormat.RGBA32I:
                    return "RGBA32I";
                case TextureInternalFormat.RGBA32UI:
                    return "RGBA32UI";
                case TextureInternalFormat.DEPTH_COMPONENT16:
                    return "DEPTH_COMPONENT16";
                case TextureInternalFormat.DEPTH_COMPONENT24:
                    return "DEPTH_COMPONENT24";
                case TextureInternalFormat.DEPTH_COMPONENT32F:
                    return "DEPTH_COMPONENT32F";
                case TextureInternalFormat.DEPTH24_STENCIL8:
                    return "DEPTH24_STENCIL8";
                case TextureInternalFormat.DEPTH32F_STENCIL8:
                    return "DEPTH32F_STENCIL8";
                case TextureInternalFormat.COMPRESSED_RED:
                    return "COMPRESSED_RED";
                case TextureInternalFormat.COMPRESSED_RG:
                    return "COMPRESSED_RG";
                case TextureInternalFormat.COMPRESSED_RGB:
                    return "COMPRESSED_RGB";
                case TextureInternalFormat.COMPRESSED_RGBA:
                    return "COMPRESSED_RGBA";
                case TextureInternalFormat.COMPRESSED_SRGB:
                    return "COMPRESSED_SRGB";
                case TextureInternalFormat.COMPRESSED_RED_RGTC1:
                    return "COMPRESSED_RED_RGTC1";
                case TextureInternalFormat.COMPRESSED_SIGNED_RED_RGTC1:
                    return "COMPRESSED_SIGNED_RED_RGTC1";
                case TextureInternalFormat.COMPRESSED_RG_RGTC2:
                    return "COMPRESSED_RG_RGTC2";
                case TextureInternalFormat.COMPRESSED_SIGNED_RG_RGTC2:
                    return "COMPRESSED_SIGNED_RG_RGTC2";
                case TextureInternalFormat.COMPRESSED_RGBA_BPTC_UNORM_ARB:
                    return "COMPRESSED_RGBA_BPTC_UNORM_ARB";
                case TextureInternalFormat.COMPRESSED_SRGB_ALPHA_BPTC_UNORM_ARB:
                    return "COMPRESSED_SRGB_ALPHA_BPTC_UNORM_ARB";
                case TextureInternalFormat.COMPRESSED_RGB_BPTC_SIGNED_FLOAT_ARB:
                    return "COMPRESSED_RGB_BPTC_SIGNED_FLOAT_ARB";
                case TextureInternalFormat.COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT_ARB:
                    return "COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT_ARB";
                case TextureInternalFormat.COMPRESSED_RGB_S3TC_DXT1_EXT:
                    return "COMPRESSED_RGB_S3TC_DXT1_EXT";
                case TextureInternalFormat.COMPRESSED_RGBA_S3TC_DXT1_EXT:
                    return "COMPRESSED_RGBA_S3TC_DXT1_EXT";
                case TextureInternalFormat.COMPRESSED_RGBA_S3TC_DXT3_EXT:
                    return "COMPRESSED_RGBA_S3TC_DXT3_EXT";
                case TextureInternalFormat.COMPRESSED_RGBA_S3TC_DXT5_EXT:
                    return "COMPRESSED_RGBA_S3TC_DXT5_EXT";
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static PixelInternalFormat getTextureInternalFormat(TextureInternalFormat f)
        {
            switch (f)
            {
                case TextureInternalFormat.R8:
                    return PixelInternalFormat.R8;
                case TextureInternalFormat.R16:
                    return PixelInternalFormat.R16;
                case TextureInternalFormat.RG8:
                    return PixelInternalFormat.Rg8;
                case TextureInternalFormat.RG16:
                    return PixelInternalFormat.Rg16;
#if TODO
                case TextureInternalFormat.R8_SNORM:
                    return PixelInternalFormat.GL_R8_SNORM;
                case TextureInternalFormat.R16_SNORM:
                    return PixelInternalFormat.GL_R16_SNORM;
                case TextureInternalFormat.RG8_SNORM:
                    return PixelInternalFormat.GL_RG8_SNORM;
                case TextureInternalFormat.RG16_SNORM:
                    return PixelInternalFormat.GL_RG16_SNORM;
                case TextureInternalFormat.RGB8_SNORM:
                    return PixelInternalFormat.GL_RGB8_SNORM;
                case TextureInternalFormat.RGB16_SNORM:
                    return GL_RGB16_SNORM;
                case TextureInternalFormat.RGBA8_SNORM:
                    return GL_RGBA8_SNORM;
                case TextureInternalFormat.RGBA16_SNORM:
                    return GL_RGBA16_SNORM;
#endif
                case TextureInternalFormat.R3_G3_B2:
                    return PixelInternalFormat.R3G3B2;
                case TextureInternalFormat.RGB4:
                    return PixelInternalFormat.Rgb4;
                case TextureInternalFormat.RGB5:
                    return PixelInternalFormat.Rgb5;
                case TextureInternalFormat.RGB8:
                    return PixelInternalFormat.Rgb8;
                case TextureInternalFormat.RGB10:
                    return PixelInternalFormat.Rgb10;
                case TextureInternalFormat.RGB12:
                    return PixelInternalFormat.Rgb12;
                case TextureInternalFormat.RGB16:
                    return PixelInternalFormat.Rgb16;
                case TextureInternalFormat.RGBA2:
                    return PixelInternalFormat.Rgba2;
                case TextureInternalFormat.RGBA4:
                    return PixelInternalFormat.Rgba4;
                case TextureInternalFormat.RGB5_A1:
                    return PixelInternalFormat.Rgb5A1;
                case TextureInternalFormat.RGBA8:
                    return PixelInternalFormat.Rgba8;
                case TextureInternalFormat.RGB10_A2:
                    return PixelInternalFormat.Rgb10A2;
                case TextureInternalFormat.RGB10_A2UI:
                    return PixelInternalFormat.Rgb10A2ui;
                case TextureInternalFormat.RGBA12:
                    return PixelInternalFormat.Rgba12;
                case TextureInternalFormat.RGBA16:
                    return PixelInternalFormat.Rgba16;
                case TextureInternalFormat.SRGB8:
                    return PixelInternalFormat.Rgb8;
                case TextureInternalFormat.SRGB8_ALPHA8:
                    return PixelInternalFormat.Srgb8Alpha8;
                case TextureInternalFormat.R16F:
                    return PixelInternalFormat.R16f;
                case TextureInternalFormat.RG16F:
                    return PixelInternalFormat.Rg16f;
                case TextureInternalFormat.RGB16F:
                    return PixelInternalFormat.Rgb16f;
                case TextureInternalFormat.RGBA16F:
                    return PixelInternalFormat.Rgba16f;
                case TextureInternalFormat.R32F:
                    return PixelInternalFormat.R32f;
                case TextureInternalFormat.RG32F:
                    return PixelInternalFormat.Rg32f;
                case TextureInternalFormat.RGB32F:
                    return PixelInternalFormat.Rgb32f;
                case TextureInternalFormat.RGBA32F:
                    return PixelInternalFormat.Rgba32f;
                case TextureInternalFormat.R11F_G11F_B10F:
                    return PixelInternalFormat.R11fG11fB10f;
                case TextureInternalFormat.RGB9_E5:
                    return PixelInternalFormat.Rgb9E5;
                case TextureInternalFormat.R8I:
                    return PixelInternalFormat.R8i;
                case TextureInternalFormat.R8UI:
                    return PixelInternalFormat.R8ui;
                case TextureInternalFormat.R16I:
                    return PixelInternalFormat.R16i;
                case TextureInternalFormat.R16UI:
                    return PixelInternalFormat.R16ui;
                case TextureInternalFormat.R32I:
                    return PixelInternalFormat.R32i;
                case TextureInternalFormat.R32UI:
                    return PixelInternalFormat.R32ui;
                case TextureInternalFormat.RG8I:
                    return PixelInternalFormat.Rg8i;
                case TextureInternalFormat.RG8UI:
                    return PixelInternalFormat.Rg8ui;
                case TextureInternalFormat.RG16I:
                    return PixelInternalFormat.Rg16i;
                case TextureInternalFormat.RG16UI:
                    return PixelInternalFormat.Rg16ui;
                case TextureInternalFormat.RG32I:
                    return PixelInternalFormat.Rg32i;
                case TextureInternalFormat.RG32UI:
                    return PixelInternalFormat.Rg32ui;
                case TextureInternalFormat.RGB8I:
                    return PixelInternalFormat.Rgb8i;
                case TextureInternalFormat.RGB8UI:
                    return PixelInternalFormat.Rgb8ui;
                case TextureInternalFormat.RGB16I:
                    return PixelInternalFormat.Rgb16i;
                case TextureInternalFormat.RGB16UI:
                    return PixelInternalFormat.Rgb16ui;
                case TextureInternalFormat.RGB32I:
                    return PixelInternalFormat.Rgb32i;
                case TextureInternalFormat.RGB32UI:
                    return PixelInternalFormat.Rgb32ui;
                case TextureInternalFormat.RGBA8I:
                    return PixelInternalFormat.Rgba8i;
                case TextureInternalFormat.RGBA8UI:
                    return PixelInternalFormat.Rgba8ui;
                case TextureInternalFormat.RGBA16I:
                    return PixelInternalFormat.Rgba16i;
                case TextureInternalFormat.RGBA16UI:
                    return PixelInternalFormat.Rgba16ui;
                case TextureInternalFormat.RGBA32I:
                    return PixelInternalFormat.Rgba32i;
                case TextureInternalFormat.RGBA32UI:
                    return PixelInternalFormat.Rgba32ui;
                case TextureInternalFormat.DEPTH_COMPONENT16:
                    return PixelInternalFormat.DepthComponent16;
                case TextureInternalFormat.DEPTH_COMPONENT24:
                    return PixelInternalFormat.DepthComponent24;
                case TextureInternalFormat.DEPTH_COMPONENT32F:
                    return PixelInternalFormat.DepthComponent32f;
                case TextureInternalFormat.DEPTH32F_STENCIL8:
                    return PixelInternalFormat.Depth32fStencil8;
                case TextureInternalFormat.DEPTH24_STENCIL8:
                    return PixelInternalFormat.Depth24Stencil8;
                case TextureInternalFormat.COMPRESSED_RED:
                    return PixelInternalFormat.CompressedRed;
                case TextureInternalFormat.COMPRESSED_RG:
                    return PixelInternalFormat.CompressedRg;
                case TextureInternalFormat.COMPRESSED_RGB:
                    return PixelInternalFormat.CompressedRgb;
                case TextureInternalFormat.COMPRESSED_RGBA:
                    return PixelInternalFormat.CompressedRgba;
                case TextureInternalFormat.COMPRESSED_SRGB:
                    return PixelInternalFormat.Srgb;
                case TextureInternalFormat.COMPRESSED_RED_RGTC1:
                    return PixelInternalFormat.CompressedRedRgtc1;
                case TextureInternalFormat.COMPRESSED_SIGNED_RED_RGTC1:
                    return PixelInternalFormat.CompressedSignedRedRgtc1;
                case TextureInternalFormat.COMPRESSED_RG_RGTC2:
                    return PixelInternalFormat.CompressedRgRgtc2;
                case TextureInternalFormat.COMPRESSED_SIGNED_RG_RGTC2:
                    return PixelInternalFormat.CompressedSignedRgRgtc2;
#if TODO
                case TextureInternalFormat.COMPRESSED_RGBA_BPTC_UNORM_ARB:
                    return PixelInternalFormat.GL_COMPRESSED_RGBA_BPTC_UNORM_ARB;
                case TextureInternalFormat.COMPRESSED_SRGB_ALPHA_BPTC_UNORM_ARB:
                    return PixelInternalFormat.GL_COMPRESSED_SRGB_ALPHA_BPTC_UNORM_ARB;
                case TextureInternalFormat.COMPRESSED_RGB_BPTC_SIGNED_FLOAT_ARB:
                    return PixelInternalFormat.GL_COMPRESSED_RGB_BPTC_SIGNED_FLOAT_ARB;
                case TextureInternalFormat.COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT_ARB:
                    return PixelInternalFormat.GL_COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT_ARB;
#endif
                case TextureInternalFormat.COMPRESSED_RGB_S3TC_DXT1_EXT:
                    return PixelInternalFormat.CompressedRgbS3tcDxt1Ext;
                case TextureInternalFormat.COMPRESSED_RGBA_S3TC_DXT1_EXT:
                    return PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
                case TextureInternalFormat.COMPRESSED_RGBA_S3TC_DXT3_EXT:
                    return PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;
                case TextureInternalFormat.COMPRESSED_RGBA_S3TC_DXT5_EXT:
                    return PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static SizedInternalFormat getSizedInternalFormat(TextureInternalFormat f)
        {
            switch (f)
            {
                case TextureInternalFormat.R8:
                    return SizedInternalFormat.R8;
                case TextureInternalFormat.R16:
                    return SizedInternalFormat.R16;
                case TextureInternalFormat.RG8:
                    return SizedInternalFormat.Rg8;
                case TextureInternalFormat.RG16:
                    return SizedInternalFormat.Rg16;
                case TextureInternalFormat.RGBA8:
                    return SizedInternalFormat.Rgba8;
                case TextureInternalFormat.R16F:
                    return SizedInternalFormat.R16f;
                case TextureInternalFormat.RG16F:
                    return SizedInternalFormat.Rg16f;
                case TextureInternalFormat.RGBA16F:
                    return SizedInternalFormat.Rgba16f;
                case TextureInternalFormat.R32F:
                    return SizedInternalFormat.R32f;
                case TextureInternalFormat.RG32F:
                    return SizedInternalFormat.Rg32f;
                case TextureInternalFormat.RGBA32F:
                    return SizedInternalFormat.Rgba32f;
                case TextureInternalFormat.R8I:
                    return SizedInternalFormat.R8i;
                case TextureInternalFormat.R8UI:
                    return SizedInternalFormat.R8ui;
                case TextureInternalFormat.R16I:
                    return SizedInternalFormat.R16i;
                case TextureInternalFormat.R16UI:
                    return SizedInternalFormat.R16ui;
                case TextureInternalFormat.R32I:
                    return SizedInternalFormat.R32i;
                case TextureInternalFormat.R32UI:
                    return SizedInternalFormat.R32ui;
                case TextureInternalFormat.RG8I:
                    return SizedInternalFormat.Rg8i;
                case TextureInternalFormat.RG8UI:
                    return SizedInternalFormat.Rg8ui;
                case TextureInternalFormat.RG16I:
                    return SizedInternalFormat.Rg16i;
                case TextureInternalFormat.RG16UI:
                    return SizedInternalFormat.Rg16ui;
                case TextureInternalFormat.RG32I:
                    return SizedInternalFormat.Rg32i;
                case TextureInternalFormat.RG32UI:
                    return SizedInternalFormat.Rg32ui;
                case TextureInternalFormat.RGBA8I:
                    return SizedInternalFormat.Rgba8i;
                case TextureInternalFormat.RGBA8UI:
                    return SizedInternalFormat.Rgba8ui;
                case TextureInternalFormat.RGBA16I:
                    return SizedInternalFormat.Rgba16i;
                case TextureInternalFormat.RGBA16UI:
                    return SizedInternalFormat.Rgba16ui;
                case TextureInternalFormat.RGBA32I:
                    return SizedInternalFormat.Rgba32i;
                case TextureInternalFormat.RGBA32UI:
                    return SizedInternalFormat.Rgba32ui;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static PixelFormat getTextureFormat(TextureFormat f)
        {
            switch (f)
            {
                case TextureFormat.COLOR_INDEX:
                    return PixelFormat.ColorIndex;
                case TextureFormat.STENCIL_INDEX:
                    return PixelFormat.StencilIndex;
                case TextureFormat.DEPTH_COMPONENT:
                    return PixelFormat.DepthComponent;
                case TextureFormat.DEPTH_STENCIL:
                    return PixelFormat.DepthStencil;
                case TextureFormat.RED:
                    return PixelFormat.Red;
                case TextureFormat.GREEN:
                    return PixelFormat.Green;
                case TextureFormat.BLUE:
                    return PixelFormat.Blue;
                case TextureFormat.RG:
                    return PixelFormat.Rg;
                case TextureFormat.RGB:
                    return PixelFormat.Rgb;
                case TextureFormat.RGBA:
                    return PixelFormat.Rgba;
                case TextureFormat.BGR:
                    return PixelFormat.Bgr;
                case TextureFormat.BGRA:
                    return PixelFormat.Bgra;
                case TextureFormat.RED_INTEGER:
                    return PixelFormat.RedInteger;
                case TextureFormat.BLUE_INTEGER:
                    return PixelFormat.BlueInteger;
                case TextureFormat.GREEN_INTEGER:
                    return PixelFormat.GreenInteger;
                case TextureFormat.RG_INTEGER:
                    return PixelFormat.RgInteger;
                case TextureFormat.RGB_INTEGER:
                    return PixelFormat.RgbInteger;
                case TextureFormat.RGBA_INTEGER:
                    return PixelFormat.RgbaInteger;
                case TextureFormat.BGR_INTEGER:
                    return PixelFormat.BgrInteger;
                case TextureFormat.BGRA_INTEGER:
                    return PixelFormat.BgraInteger;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static OpenTK.Graphics.OpenGL.PixelType getPixelType(PixelType t)
        {
            switch (t)
            {
                case PixelType.UNSIGNED_BYTE:
                    return OpenTK.Graphics.OpenGL.PixelType.UnsignedByte;
                case PixelType.BYTE:
                    return OpenTK.Graphics.OpenGL.PixelType.Byte;
                case PixelType.UNSIGNED_SHORT:
                    return OpenTK.Graphics.OpenGL.PixelType.UnsignedShort;
                case PixelType.SHORT:
                    return OpenTK.Graphics.OpenGL.PixelType.Short;
                case PixelType.UNSIGNED_INT:
                    return OpenTK.Graphics.OpenGL.PixelType.UnsignedInt;
                case PixelType.INT:
                    return OpenTK.Graphics.OpenGL.PixelType.Int;
                case PixelType.HALF:
                    return OpenTK.Graphics.OpenGL.PixelType.HalfFloat;
                case PixelType.FLOAT:
                    return OpenTK.Graphics.OpenGL.PixelType.Float;
                case PixelType.UNSIGNED_BYTE_3_3_2:
                    return OpenTK.Graphics.OpenGL.PixelType.UnsignedByte332;
                case PixelType.UNSIGNED_BYTE_2_3_3_REV:
                    return OpenTK.Graphics.OpenGL.PixelType.UnsignedByte233Reversed;
                case PixelType.UNSIGNED_SHORT_5_6_5:
                    return OpenTK.Graphics.OpenGL.PixelType.UnsignedShort565;
                case PixelType.UNSIGNED_SHORT_5_6_5_REV:
                    return OpenTK.Graphics.OpenGL.PixelType.UnsignedShort565Reversed;
                case PixelType.UNSIGNED_SHORT_4_4_4_4:
                    return OpenTK.Graphics.OpenGL.PixelType.UnsignedShort4444;
                case PixelType.UNSIGNED_SHORT_4_4_4_4_REV:
                    return OpenTK.Graphics.OpenGL.PixelType.UnsignedShort4444Reversed;
                case PixelType.UNSIGNED_SHORT_5_5_5_1:
                    return OpenTK.Graphics.OpenGL.PixelType.UnsignedShort5551;
                case PixelType.UNSIGNED_SHORT_1_5_5_5_REV:
                    return OpenTK.Graphics.OpenGL.PixelType.UnsignedShort1555Reversed;
                case PixelType.UNSIGNED_INT_8_8_8_8:
                    return OpenTK.Graphics.OpenGL.PixelType.UnsignedInt8888;
                case PixelType.UNSIGNED_INT_8_8_8_8_REV:
                    return OpenTK.Graphics.OpenGL.PixelType.UnsignedInt8888Reversed;
                case PixelType.UNSIGNED_INT_10_10_10_2:
                    return OpenTK.Graphics.OpenGL.PixelType.UnsignedInt1010102;
                case PixelType.UNSIGNED_INT_2_10_10_10_REV:
                    return OpenTK.Graphics.OpenGL.PixelType.UnsignedInt10F11F11FRev;
                case PixelType.UNSIGNED_INT_24_8:
                    return OpenTK.Graphics.OpenGL.PixelType.UnsignedInt248;
                case PixelType.UNSIGNED_INT_10F_11F_11F_REV:
                    return OpenTK.Graphics.OpenGL.PixelType.UnsignedInt10F11F11FRev;
                case PixelType.UNSIGNED_INT_5_9_9_9_REV:
                    return OpenTK.Graphics.OpenGL.PixelType.UnsignedInt5999Rev;
                case PixelType.FLOAT_32_UNSIGNED_INT_24_8_REV:
                    return OpenTK.Graphics.OpenGL.PixelType.UnsignedInt248;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static int getTextureComponents(TextureFormat f)
        {
            switch (f)
            {
                case TextureFormat.STENCIL_INDEX:
                case TextureFormat.DEPTH_COMPONENT:
                case TextureFormat.RED:
                case TextureFormat.GREEN:
                case TextureFormat.BLUE:
                case TextureFormat.RED_INTEGER:
                case TextureFormat.BLUE_INTEGER:
                case TextureFormat.GREEN_INTEGER:
                    return 1;
                case TextureFormat.DEPTH_STENCIL:
                case TextureFormat.RG:
                case TextureFormat.RG_INTEGER:
                    return 2;
                case TextureFormat.RGB:
                case TextureFormat.BGR:
                case TextureFormat.RGB_INTEGER:
                case TextureFormat.BGR_INTEGER:
                    return 3;
                case TextureFormat.RGBA:
                case TextureFormat.BGRA:
                case TextureFormat.RGBA_INTEGER:
                case TextureFormat.BGRA_INTEGER:
                    return 4;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static int getFormatSize(TextureFormat f, PixelType t)
        {
            int components = getTextureComponents(f);

            switch (t)
            {
                case PixelType.UNSIGNED_BYTE:
                case PixelType.BYTE:
                    return components;
                case PixelType.UNSIGNED_SHORT:
                case PixelType.SHORT:
                case PixelType.HALF:
                    return 2 * components;
                case PixelType.UNSIGNED_INT:
                case PixelType.INT:
                case PixelType.FLOAT:
                    return 4 * components;
                case PixelType.UNSIGNED_BYTE_3_3_2:
                case PixelType.UNSIGNED_BYTE_2_3_3_REV:
                    return 1;
                case PixelType.UNSIGNED_SHORT_5_6_5:
                case PixelType.UNSIGNED_SHORT_5_6_5_REV:
                case PixelType.UNSIGNED_SHORT_4_4_4_4:
                case PixelType.UNSIGNED_SHORT_4_4_4_4_REV:
                case PixelType.UNSIGNED_SHORT_5_5_5_1:
                case PixelType.UNSIGNED_SHORT_1_5_5_5_REV:
                    return 2;
                case PixelType.UNSIGNED_INT_8_8_8_8:
                case PixelType.UNSIGNED_INT_8_8_8_8_REV:
                case PixelType.UNSIGNED_INT_10_10_10_2:
                case PixelType.UNSIGNED_INT_2_10_10_10_REV:
                case PixelType.UNSIGNED_INT_24_8:
                case PixelType.UNSIGNED_INT_10F_11F_11F_REV:
                case PixelType.UNSIGNED_INT_5_9_9_9_REV:
                    return 4;
                case PixelType.FLOAT_32_UNSIGNED_INT_24_8_REV:
                    return 8;
            }

            Debug.Assert(false);
            throw new Exception();
        }

        public static int getTextureSwizzle(char c)
        {
            switch (c)
            {
                case 'r':
                    return (int)PixelFormat.Red;
                case 'g':
                    return (int)PixelFormat.Green;
                case 'b':
                    return (int)PixelFormat.Blue;
                case 'a':
                    return (int)PixelFormat.Alpha;
                case '0':
                    return (int)BlendingFactorSrc.Zero;
                case '1':
                    return (int)BlendingFactorSrc.One;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static int getTextureWrap(TextureWrap w)
        {
            switch (w)
            {
                case TextureWrap.CLAMP_TO_EDGE:
                    return (int)TextureWrapMode.ClampToEdge;
                case TextureWrap.CLAMP_TO_BORDER:
                    return (int)TextureWrapMode.ClampToBorder;
                case TextureWrap.REPEAT:
                    return (int)TextureWrapMode.Repeat;
                case TextureWrap.MIRRORED_REPEAT:
                    return (int)TextureWrapMode.MirroredRepeat;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static int getTextureMinFilter(TextureFilter f)
        {
            switch (f)
            {
                case TextureFilter.NEAREST:
                    return (int)TextureMinFilter.Nearest;
                case TextureFilter.LINEAR:
                    return (int)TextureMinFilter.Linear;
                case TextureFilter.NEAREST_MIPMAP_NEAREST:
                    return (int)TextureMinFilter.NearestMipmapNearest;
                case TextureFilter.NEAREST_MIPMAP_LINEAR:
                    return (int)TextureMinFilter.NearestMipmapLinear;
                case TextureFilter.LINEAR_MIPMAP_NEAREST:
                    return (int)TextureMinFilter.LinearMipmapNearest;
                case TextureFilter.LINEAR_MIPMAP_LINEAR:
                    return (int)TextureMinFilter.LinearMipmapLinear;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static int getTextureMagFilter(TextureFilter f)
        {
            switch (f)
            {
                case TextureFilter.NEAREST:
                    return (int)TextureMagFilter.Nearest;
                case TextureFilter.LINEAR:
                    return (int)TextureMagFilter.Linear;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static StencilFunction getStencilFunction(Function f)
        {
            switch (f)
            {
                case Function.LEQUAL:
                    return StencilFunction.Lequal;
                case Function.GEQUAL:
                    return StencilFunction.Gequal;
                case Function.LESS:
                    return StencilFunction.Less;
                case Function.GREATER:
                    return StencilFunction.Greater;
                case Function.EQUAL:
                    return StencilFunction.Equal;
                case Function.NOTEQUAL:
                    return StencilFunction.Notequal;
                case Function.ALWAYS:
                    return StencilFunction.Always;
                case Function.NEVER:
                    return StencilFunction.Never;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static DepthFunction getDepthFunction(Function f)
        {
            switch (f)
            {
                case Function.LEQUAL:
                    return DepthFunction.Lequal;
                case Function.GEQUAL:
                    return DepthFunction.Gequal;
                case Function.LESS:
                    return DepthFunction.Less;
                case Function.GREATER:
                    return DepthFunction.Greater;
                case Function.EQUAL:
                    return DepthFunction.Equal;
                case Function.NOTEQUAL:
                    return DepthFunction.Notequal;
                case Function.ALWAYS:
                    return DepthFunction.Always;
                case Function.NEVER:
                    return DepthFunction.Never;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static int getBufferId(BufferId b)
        {
            switch (b)
            {
                case BufferId.COLOR0:
                    return 0;
                case BufferId.COLOR1:
                    return 1;
                case BufferId.COLOR2:
                    return 2;
                case BufferId.COLOR3:
                    return 3;
                case BufferId.STENCIL:
                    return 4;
                case BufferId.DEPTH:
                    return 5;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static StencilOp getStencilOperation(StencilOperation o)
        {
            switch (o)
            {
                case StencilOperation.KEEP:
                    return StencilOp.Keep;
                case StencilOperation.RESET:
                    return StencilOp.Zero;
                case StencilOperation.REPLACE:
                    return StencilOp.Replace;
                case StencilOperation.INVERT:
                    return StencilOp.Invert;
                case StencilOperation.INCR:
                    return StencilOp.Incr;
                case StencilOperation.DECR:
                    return StencilOp.Decr;
                case StencilOperation.INCR_WRAP:
                    return StencilOp.IncrWrap;
                case StencilOperation.DECR_WRAP:
                    return StencilOp.DecrWrap;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static BlendEquationMode getBlendEquation(BlendEquation e)
        {
            switch (e)
            {
                case BlendEquation.ADD:
                    return BlendEquationMode.FuncAdd;
                case BlendEquation.SUBTRACT:
                    return BlendEquationMode.FuncSubtract;
                case BlendEquation.REVERSE_SUBTRACT:
                    return BlendEquationMode.FuncReverseSubtract;
                case BlendEquation.MIN:
                    return BlendEquationMode.Min;
                case BlendEquation.MAX:
                    return BlendEquationMode.Max;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static BlendingFactorSrc getBlendSrcArgument(BlendArgument a)
        {
            switch (a)
            {
                case BlendArgument.ZERO:
                    return BlendingFactorSrc.Zero;
                case BlendArgument.ONE:
                    return BlendingFactorSrc.One;
                case BlendArgument.SRC_COLOR:
                    return BlendingFactorSrc.Src1Color;
                case BlendArgument.ONE_MINUS_SRC_COLOR:
                    return BlendingFactorSrc.OneMinusSrc1Color;
                case BlendArgument.DST_COLOR:
                    return BlendingFactorSrc.DstColor;
                case BlendArgument.ONE_MINUS_DST_COLOR:
                    return BlendingFactorSrc.OneMinusDstColor;
                case BlendArgument.SRC_ALPHA:
                    return BlendingFactorSrc.SrcAlpha;
                case BlendArgument.ONE_MINUS_SRC_ALPHA:
                    return BlendingFactorSrc.OneMinusSrcAlpha;
                case BlendArgument.DST_ALPHA:
                    return BlendingFactorSrc.DstAlpha;
                case BlendArgument.ONE_MINUS_DST_ALPHA:
                    return BlendingFactorSrc.OneMinusDstAlpha;
                case BlendArgument.CONSTANT_COLOR:
                    return BlendingFactorSrc.ConstantColor;
                case BlendArgument.ONE_MINUS_CONSTANT_COLOR:
                    return BlendingFactorSrc.OneMinusConstantColor;
                case BlendArgument.CONSTANT_ALPHA:
                    return BlendingFactorSrc.ConstantAlpha;
                case BlendArgument.ONE_MINUS_CONSTANT_ALPHA:
                    return BlendingFactorSrc.OneMinusConstantAlpha;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static BlendingFactorDest getBlendDestArgument(BlendArgument a)
        {
            switch (a)
            {
                case BlendArgument.ZERO:
                    return BlendingFactorDest.Zero;
                case BlendArgument.ONE:
                    return BlendingFactorDest.One;
                case BlendArgument.SRC_COLOR:
                    return BlendingFactorDest.Src1Color;
                case BlendArgument.ONE_MINUS_SRC_COLOR:
                    return BlendingFactorDest.OneMinusSrc1Color;
                case BlendArgument.SRC_ALPHA:
                    return BlendingFactorDest.SrcAlpha;
                case BlendArgument.ONE_MINUS_SRC_ALPHA:
                    return BlendingFactorDest.OneMinusSrcAlpha;
                case BlendArgument.DST_ALPHA:
                    return BlendingFactorDest.DstAlpha;
                case BlendArgument.ONE_MINUS_DST_ALPHA:
                    return BlendingFactorDest.OneMinusDstAlpha;
                case BlendArgument.CONSTANT_COLOR:
                    return BlendingFactorDest.ConstantColor;
                case BlendArgument.ONE_MINUS_CONSTANT_COLOR:
                    return BlendingFactorDest.OneMinusConstantColor;
                case BlendArgument.CONSTANT_ALPHA:
                    return BlendingFactorDest.ConstantAlpha;
                case BlendArgument.ONE_MINUS_CONSTANT_ALPHA:
                    return BlendingFactorDest.OneMinusConstantAlpha;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static LogicOp getLogicOperation(LogicOperation o)
        {
            switch (o)
            {
                case LogicOperation.CLEAR:
                    return LogicOp.Clear;
                case LogicOperation.AND:
                    return LogicOp.And;
                case LogicOperation.AND_REVERSE:
                    return LogicOp.AndReverse;
                case LogicOperation.COPY:
                    return LogicOp.Copy;
                case LogicOperation.AND_INVERTED:
                    return LogicOp.AndInverted;
                case LogicOperation.NOOP:
                    return LogicOp.Noop;
                case LogicOperation.XOR:
                    return LogicOp.Xor;
                case LogicOperation.OR:
                    return LogicOp.Or;
                case LogicOperation.NOR:
                    return LogicOp.Nor;
                case LogicOperation.EQUIV:
                    return LogicOp.Equiv;
                case LogicOperation.NOT:
                    return LogicOp.Invert;
                case LogicOperation.OR_REVERSE:
                    return LogicOp.OrReverse;
                case LogicOperation.COPY_INVERTED:
                    return LogicOp.CopyInverted;
                case LogicOperation.OR_INVERTED:
                    return LogicOp.OrInverted;
                case LogicOperation.NAND:
                    return LogicOp.Nand;
                case LogicOperation.SET:
                    return LogicOp.Set;
            }
            Debug.Assert(false);
            throw new Exception();
        }
#if TODO
        GLenum getStage(Stage s)
        {
            switch (s) {
            case VERTEX:
                return GL_VERTEX_SHADER;
            case TESSELATION_CONTROL:
                return GL_TESS_CONTROL_SHADER;
            case TESSELATION_EVALUATION:
                return GL_TESS_EVALUATION_SHADER;
            case GEOMETRY:
                return GL_GEOMETRY_SHADER;
            case FRAGMENT:
                return GL_FRAGMENT_SHADER;
            }
            Debug.Assert(false);
            throw new Exception();
        }
#endif
        public static ConditionalRenderType getQueryMode(QueryMode m)
        {
            switch (m)
            {
                case QueryMode.WAIT:
                    return ConditionalRenderType.QueryWait;
                case QueryMode.NO_WAIT:
                    return ConditionalRenderType.QueryNoWait;
                case QueryMode.REGION_WAIT:
                    return ConditionalRenderType.QueryByRegionWait;
                case QueryMode.REGION_NO_WAIT:
                    return ConditionalRenderType.QueryByRegionNoWait;
            }
            Debug.Assert(false);
            throw new Exception();
        }

        public static void ConvertPixelFormat(System.Drawing.Imaging.PixelFormat pixelFormat,
                       out TextureInternalFormat pif, out TextureFormat pf, out Sxta.Render.PixelType pt, out int size)
        {
            const int pixelFormat32bppCMYK = 0x200F;
            switch (pixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed: // misses glColorTable setup
                    pif = TextureInternalFormat.RGB8;
                    pf = TextureFormat.COLOR_INDEX;
                    pt = Sxta.Render.PixelType.UNSIGNED_BYTE;
                    size = 1;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format16bppArgb1555:
                case System.Drawing.Imaging.PixelFormat.Format16bppRgb555: // does not work
                    pif = TextureInternalFormat.RGB5_A1;
                    pf = TextureFormat.BGR;
                    pt = Sxta.Render.PixelType.UNSIGNED_SHORT_5_5_5_1;
                    size = 2;
                    break;
                /*  case System.Drawing.Imaging.PixelFormat.Format16bppRgb565:
                          pif = OpenTK.Graphics.OpenGL.PixelInternalFormat.R5G6B5IccSgix;
                          pf = OpenTK.Graphics.OpenGL.PixelFormat.R5G6B5IccSgix;
                          pt = OpenTK.Graphics.OpenGL.PixelType.UnsignedByte;
                          break;
                     */


                case System.Drawing.Imaging.PixelFormat.Format24bppRgb: // works
                    pif = TextureInternalFormat.RGB8;
                    pf = TextureFormat.BGR;
                    pt = Sxta.Render.PixelType.UNSIGNED_BYTE;
                    size = 3;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb: // has alpha too? wtf?
                case System.Drawing.Imaging.PixelFormat.Canonical:
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb: // works
                    pif = TextureInternalFormat.RGBA8;
                    pf = TextureFormat.BGRA;
                    pt = Sxta.Render.PixelType.UNSIGNED_BYTE;
                    size = 4;
                    break;
                case (System.Drawing.Imaging.PixelFormat)pixelFormat32bppCMYK:
                default:
                    throw new ArgumentException("ERROR: Unsupported Pixel Format " + pixelFormat);
            }
        }
    }
}
