using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.Render.OpenGLExt
{
    public struct Vec3
    {
        public byte X;
        public byte Y;
        public byte Z;
        public Vec3(byte x, byte y, byte z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
    public enum Swizzle : uint
    {
        SWIZZLE_RED = 0x1903,       //GL_RED
        SWIZZLE_GREEN = 0x1904,     //GL_GREEN
        SWIZZLE_BLUE = 0x1905,      //GL_BLUE
        SWIZZLE_ALPHA = 0x1906,     //GL_ALPHA
        SWIZZLE_ZERO = 0x0000,      //GL_ZERO
        SWIZZLE_ONE = 0x0001,       //GL_ONE
    }

    public enum format
    {
        FORMAT_INVALID = -1,
        FORMAT_UNDEFINED = 0,
        FORMAT_RG4_UNORM = 1,
        FORMAT_FIRST = FORMAT_RG4_UNORM,
        FORMAT_RG4_USCALED = 2,
        FORMAT_RGBA4_UNORM = 3,
        FORMAT_RGBA4_USCALED = 4,
        FORMAT_R5G6B5_UNORM = 5,
        FORMAT_R5G6B5_USCALED = 6,
        FORMAT_RGB5A1_UNORM = 7,
        FORMAT_RGB5A1_USCALED = 8,

        FORMAT_R8_UNORM = 9,
        FORMAT_R8_SNORM = 10,
        FORMAT_R8_USCALED = 11,
        FORMAT_R8_SSCALED = 12,
        FORMAT_R8_UINT = 13,
        FORMAT_R8_SINT = 14,
        FORMAT_R8_SRGB = 15,

        FORMAT_RG8_UNORM = 16,
        FORMAT_RG8_SNORM = 17,
        FORMAT_RG8_USCALED = 18,
        FORMAT_RG8_SSCALED = 19,
        FORMAT_RG8_UINT = 20,
        FORMAT_RG8_SINT = 21,
        FORMAT_RG8_SRGB = 22,

        FORMAT_RGB8_UNORM = 23,
        FORMAT_RGB8_SNORM = 24,
        FORMAT_RGB8_USCALED = 25,
        FORMAT_RGB8_SSCALED = 26,
        FORMAT_RGB8_UINT = 27,
        FORMAT_RGB8_SINT = 28,
        FORMAT_RGB8_SRGB = 29,

        FORMAT_RGBA8_UNORM = 30,
        FORMAT_RGBA8_SNORM = 31,
        FORMAT_RGBA8_USCALED = 32,
        FORMAT_RGBA8_SSCALED = 33,
        FORMAT_RGBA8_UINT = 34,
        FORMAT_RGBA8_SINT = 35,
        FORMAT_RGBA8_SRGB = 36,

        FORMAT_RGB10A2_UNORM = 37,
        FORMAT_RGB10A2_SNORM = 38,
        FORMAT_RGB10A2_USCALED = 39,
        FORMAT_RGB10A2_SSCALED = 40,
        FORMAT_RGB10A2_UINT = 41,
        FORMAT_RGB10A2_SINT = 42,

        FORMAT_R16_UNORM = 43,
        FORMAT_R16_SNORM = 44,
        FORMAT_R16_USCALED = 45,
        FORMAT_R16_SSCALED = 46,
        FORMAT_R16_UINT = 47,
        FORMAT_R16_SINT = 48,
        FORMAT_R16_SFLOAT = 49,

        FORMAT_RG16_UNORM = 50,
        FORMAT_RG16_SNORM = 51,
        FORMAT_RG16_USCALED = 52,
        FORMAT_RG16_SSCALED = 53,
        FORMAT_RG16_UINT = 54,
        FORMAT_RG16_SINT = 55,
        FORMAT_RG16_SFLOAT = 56,

        FORMAT_RGB16_UNORM = 57,
        FORMAT_RGB16_SNORM = 58,
        FORMAT_RGB16_USCALED = 59,
        FORMAT_RGB16_SSCALED = 60,
        FORMAT_RGB16_UINT = 61,
        FORMAT_RGB16_SINT = 62,
        FORMAT_RGB16_SFLOAT = 63,

        FORMAT_RGBA16_UNORM = 64,
        FORMAT_RGBA16_SNORM = 65,
        FORMAT_RGBA16_USCALED = 66,
        FORMAT_RGBA16_SSCALED = 67,
        FORMAT_RGBA16_UINT = 68,
        FORMAT_RGBA16_SINT = 69,
        FORMAT_RGBA16_SFLOAT = 70,

        FORMAT_R32_UINT = 71,
        FORMAT_R32_SINT = 72,
        FORMAT_R32_SFLOAT = 73,
        FORMAT_RG32_UINT = 74,
        FORMAT_RG32_SINT = 75,
        FORMAT_RG32_SFLOAT = 76,
        FORMAT_RGB32_UINT = 77,
        FORMAT_RGB32_SINT = 78,
        FORMAT_RGB32_SFLOAT = 79,
        FORMAT_RGBA32_UINT = 80,
        FORMAT_RGBA32_SINT = 81,
        FORMAT_RGBA32_SFLOAT = 82,

        FORMAT_R64_SFLOAT = 83,
        FORMAT_RG64_SFLOAT = 84,
        FORMAT_RGB64_SFLOAT = 85,
        FORMAT_RGBA64_SFLOAT = 86,

        FORMAT_RG11B10_UFLOAT = 87,
        FORMAT_RGB9E5_UFLOAT = 88,

        FORMAT_D16_UNORM = 89,
        FORMAT_D24_UNORM = 90,
        FORMAT_D32_SFLOAT = 91,
        FORMAT_S8_UINT = 92,
        FORMAT_D16_UNORM_S8_UINT = 93,
        FORMAT_D24_UNORM_S8_UINT = 94,
        FORMAT_D32_SFLOAT_S8_UINT = 95,

        FORMAT_RGB_DXT1_UNORM,
        FORMAT_RGB_DXT1_SRGB,
        FORMAT_RGBA_DXT1_UNORM,
        FORMAT_RGBA_DXT1_SRGB,
        FORMAT_RGBA_DXT3_UNORM,
        FORMAT_RGBA_DXT3_SRGB,
        FORMAT_RGBA_DXT5_UNORM,
        FORMAT_RGBA_DXT5_SRGB,
        FORMAT_R_ATI1N_UNORM,
        FORMAT_R_ATI1N_SNORM,
        FORMAT_RG_ATI2N_UNORM,
        FORMAT_RG_ATI2N_SNORM,
        FORMAT_RGB_BP_UFLOAT,
        FORMAT_RGB_BP_SFLOAT,
        FORMAT_RGBA_BP_UNORM,
        FORMAT_RGBA_BP_SRGB,

        FORMAT_RGB_ETC2_UNORM = 112,
        FORMAT_RGB_ETC2_SRGB = 113,
        FORMAT_RGBA_ETC2_A1_UNORM = 114,
        FORMAT_RGBA_ETC2_A1_SRGB = 115,
        FORMAT_RGBA_ETC2_UNORM = 116,
        FORMAT_RGBA_ETC2_SRGB = 117,
        FORMAT_R_EAC_UNORM = 118,
        FORMAT_R_EAC_SNORM = 119,
        FORMAT_RG_EAC_UNORM = 120,
        FORMAT_RG_EAC_SNORM = 121,
        FORMAT_ASTC_4x4_UNORM = 122,
        FORMAT_ASTC_4x4_SRGB = 123,
        FORMAT_ASTC_5x4_UNORM = 124,
        FORMAT_ASTC_5x4_SRGB = 125,
        FORMAT_ASTC_5x5_UNORM = 126,
        FORMAT_ASTC_5x5_SRGB = 127,
        FORMAT_ASTC_6x5_UNORM = 128,
        FORMAT_ASTC_6x5_SRGB = 129,
        FORMAT_ASTC_6x6_UNORM = 130,
        FORMAT_ASTC_6x6_SRGB = 131,
        FORMAT_ASTC_8x5_UNORM = 132,
        FORMAT_ASTC_8x5_SRGB = 133,
        FORMAT_ASTC_8x6_UNORM = 134,
        FORMAT_ASTC_8x6_SRGB = 135,
        FORMAT_ASTC_8x8_UNORM = 136,
        FORMAT_ASTC_8x8_SRGB = 137,
        FORMAT_ASTC_10x5_UNORM = 138,
        FORMAT_ASTC_10x5_SRGB = 139,
        FORMAT_ASTC_10x6_UNORM = 140,
        FORMAT_ASTC_10x6_SRGB = 141,
        FORMAT_ASTC_10x8_UNORM = 142,
        FORMAT_ASTC_10x8_SRGB = 143,
        FORMAT_ASTC_10x10_UNORM = 144,
        FORMAT_ASTC_10x10_SRGB = 145,
        FORMAT_ASTC_12x10_UNORM = 146,
        FORMAT_ASTC_12x10_SRGB = 147,
        FORMAT_ASTC_12x12_UNORM = 148,
        FORMAT_ASTC_12x12_SRGB = 149,

        FORMAT_BGRA4_UNORM = 150,
        FORMAT_BGRA4_USCALED = 151,
        FORMAT_B5G6R5_UNORM = 152,
        FORMAT_B5G6R5_USCALED = 153,
        FORMAT_BGR5A1_UNORM = 154,
        FORMAT_BGR5A1_USCALED = 155,

        FORMAT_BGR8_UNORM = 156,
        FORMAT_BGR8_SNORM = 157,
        FORMAT_BGR8_USCALED = 158,
        FORMAT_BGR8_SSCALED = 159,
        FORMAT_BGR8_UINT = 160,
        FORMAT_BGR8_SINT = 161,
        FORMAT_BGR8_SRGB = 162,

        FORMAT_BGRA8_UNORM = 163,
        FORMAT_BGRA8_SNORM = 164,
        FORMAT_BGRA8_USCALED = 165,
        FORMAT_BGRA8_SSCALED = 166,
        FORMAT_BGRA8_UINT = 167,
        FORMAT_BGRA8_SINT = 168,
        FORMAT_BGRA8_SRGB = 169,

        FORMAT_BGR10A2_UNORM = 170,
        FORMAT_BGR10A2_SNORM = 171,
        FORMAT_BGR10A2_USCALED = 172,
        FORMAT_BGR10A2_SSCALED = 173,
        FORMAT_BGR10A2_UINT = 174,
        FORMAT_BGR10A2_SINT = 175,

        FORMAT_RG3B2_UNORM,
        FORMAT_BGRX8_UNORM,
        FORMAT_BGRX8_SRGB,

        FORMAT_L8_UNORM,
        FORMAT_A8_UNORM,
        FORMAT_LA8_UNORM,
        FORMAT_L16_UNORM,
        FORMAT_A16_UNORM,
        FORMAT_LA16_UNORM,

        FORMAT_RGB_PVRTC1_8X8_UNORM,
        FORMAT_RGB_PVRTC1_8X8_SRGB,
        FORMAT_RGB_PVRTC1_16X8_UNORM,
        FORMAT_RGB_PVRTC1_16X8_SRGB,
        FORMAT_RGBA_PVRTC1_8X8_UNORM,
        FORMAT_RGBA_PVRTC1_8X8_SRGB,
        FORMAT_RGBA_PVRTC1_16X8_UNORM,
        FORMAT_RGBA_PVRTC1_16X8_SRGB,
        FORMAT_RGBA_PVRTC2_4X4_UNORM,
        FORMAT_RGBA_PVRTC2_4X4_SRGB,
        FORMAT_RGBA_PVRTC2_8X4_UNORM,
        FORMAT_RGBA_PVRTC2_8X4_SRGB,

        FORMAT_RGB_ETC_UNORM,
        FORMAT_RGB_ATC_UNORM,
        FORMAT_RGBA_ATC_EXPLICIT_UNORM,
        FORMAT_RGBA_ATC_INTERPOLATED_UNORM, FORMAT_LAST = FORMAT_RGBA_ATC_INTERPOLATED_UNORM
    }
    public enum internalFormat
    {
        // unorm formats
        INTERNAL_R8_UNORM = 0x8229,         //GL_R8
        INTERNAL_RG8_UNORM = 0x822B,        //GL_RG8
        INTERNAL_RGB8_UNORM = 0x8051,       //GL_RGB8
        INTERNAL_RGBA8_UNORM = 0x8058,      //GL_RGBA8

        INTERNAL_R16_UNORM = 0x822A,        //GL_R16
        INTERNAL_RG16_UNORM = 0x822C,       //GL_RG16
        INTERNAL_RGB16_UNORM = 0x8054,      //GL_RGB16
        INTERNAL_RGBA16_UNORM = 0x805B,     //GL_RGBA16

        INTERNAL_RGB10A2_UNORM = 0x8059,    //GL_RGB10_A2
        INTERNAL_RGB10A2_SNORM_EXT = 0xFFFC,

        // snorm formats
        INTERNAL_R8_SNORM = 0x8F94,         //GL_R8_SNORM
        INTERNAL_RG8_SNORM = 0x8F95,        //GL_RG8_SNORM
        INTERNAL_RGB8_SNORM = 0x8F96,       //GL_RGB8_SNORM
        INTERNAL_RGBA8_SNORM = 0x8F97,      //GL_RGBA8_SNORM

        INTERNAL_R16_SNORM = 0x8F98,        //GL_R16_SNORM
        INTERNAL_RG16_SNORM = 0x8F99,       //GL_RG16_SNORM
        INTERNAL_RGB16_SNORM = 0x8F9A,      //GL_RGB16_SNORM
        INTERNAL_RGBA16_SNORM = 0x8F9B,     //GL_RGBA16_SNORM

        // unsigned integer formats
        INTERNAL_R8U = 0x8232,              //GL_R8UI
        INTERNAL_RG8U = 0x8238,             //GL_RG8UI
        INTERNAL_RGB8U = 0x8D7D,            //GL_RGB8UI
        INTERNAL_RGBA8U = 0x8D7C,           //GL_RGBA8UI

        INTERNAL_R16U = 0x8234,             //GL_R16UI
        INTERNAL_RG16U = 0x823A,            //GL_RG16UI
        INTERNAL_RGB16U = 0x8D77,           //GL_RGB16UI
        INTERNAL_RGBA16U = 0x8D76,          //GL_RGBA16UI

        INTERNAL_R32U = 0x8236,             //GL_R32UI
        INTERNAL_RG32U = 0x823C,            //GL_RG32UI
        INTERNAL_RGB32U = 0x8D71,           //GL_RGB32UI
        INTERNAL_RGBA32U = 0x8D70,          //GL_RGBA32UI

        INTERNAL_RGB10A2U = 0x906F,         //GL_RGB10_A2UI
        INTERNAL_RGB10A2I_EXT = 0xFFFB,

        // signed integer formats
        INTERNAL_R8I = 0x8231,              //GL_R8I
        INTERNAL_RG8I = 0x8237,             //GL_RG8I
        INTERNAL_RGB8I = 0x8D8F,            //GL_RGB8I
        INTERNAL_RGBA8I = 0x8D8E,           //GL_RGBA8I

        INTERNAL_R16I = 0x8233,             //GL_R16I
        INTERNAL_RG16I = 0x8239,            //GL_RG16I
        INTERNAL_RGB16I = 0x8D89,           //GL_RGB16I
        INTERNAL_RGBA16I = 0x8D88,          //GL_RGBA16I

        INTERNAL_R32I = 0x8235,             //GL_R32I
        INTERNAL_RG32I = 0x823B,            //GL_RG32I
        INTERNAL_RGB32I = 0x8D83,           //GL_RGB32I
        INTERNAL_RGBA32I = 0x8D82,          //GL_RGBA32I

        // Floating formats
        INTERNAL_R16F = 0x822D,             //GL_R16F
        INTERNAL_RG16F = 0x822F,            //GL_RG16F
        INTERNAL_RGB16F = 0x881B,           //GL_RGB16F
        INTERNAL_RGBA16F = 0x881A,          //GL_RGBA16F

        INTERNAL_R32F = 0x822E,             //GL_R32F
        INTERNAL_RG32F = 0x8230,            //GL_RG32F
        INTERNAL_RGB32F = 0x8815,           //GL_RGB32F
        INTERNAL_RGBA32F = 0x8814,          //GL_RGBA32F

        INTERNAL_R64F_EXT = 0xFFFA,         //GL_R64F
        INTERNAL_RG64F_EXT = 0xFFF9,        //GL_RG64F
        INTERNAL_RGB64F_EXT = 0xFFF8,       //GL_RGB64F
        INTERNAL_RGBA64F_EXT = 0xFFF7,      //GL_RGBA64F

        // sRGB formats
        INTERNAL_SR8 = 0x8FBD,              //GL_SR8_EXT
        INTERNAL_SRG8 = 0x8FBE,             //GL_SRG8_EXT
        INTERNAL_SRGB8 = 0x8C41,            //GL_SRGB8
        INTERNAL_SRGB8_ALPHA8 = 0x8C43,     //GL_SRGB8_ALPHA8

        // Packed formats
        INTERNAL_RGB9E5 = 0x8C3D,           //GL_RGB9_E5
        INTERNAL_RG11B10F = 0x8C3A,         //GL_R11F_G11F_B10F
        INTERNAL_RG3B2 = 0x2A10,            //GL_R3_G3_B2
        INTERNAL_R5G6B5 = 0x8D62,           //GL_RGB565
        INTERNAL_RGB5A1 = 0x8057,           //GL_RGB5_A1
        INTERNAL_RGBA4 = 0x8056,            //GL_RGBA4

        INTERNAL_RG4_EXT = 0xFFFE,

        // Luminance Alpha formats
        INTERNAL_LA4 = 0x8043,              //GL_LUMINANCE4_ALPHA4
        INTERNAL_L8 = 0x8040,               //GL_LUMINANCE8
        INTERNAL_A8 = 0x803C,               //GL_ALPHA8
        INTERNAL_LA8 = 0x8045,              //GL_LUMINANCE8_ALPHA8
        INTERNAL_L16 = 0x8042,              //GL_LUMINANCE16
        INTERNAL_A16 = 0x803E,              //GL_ALPHA16
        INTERNAL_LA16 = 0x8048,             //GL_LUMINANCE16_ALPHA16

        // Depth formats
        INTERNAL_D16 = 0x81A5,              //GL_DEPTH_COMPONENT16
        INTERNAL_D24 = 0x81A6,              //GL_DEPTH_COMPONENT24
        INTERNAL_D16S8_EXT = 0xFFF6,
        INTERNAL_D24S8 = 0x88F0,            //GL_DEPTH24_STENCIL8
        INTERNAL_D32 = 0x81A7,              //GL_DEPTH_COMPONENT32
        INTERNAL_D32F = 0x8CAC,             //GL_DEPTH_COMPONENT32F
        INTERNAL_D32FS8X24 = 0x8CAD,        //GL_DEPTH32F_STENCIL8
        INTERNAL_S8_EXT = 0x8D48,           //GL_STENCIL_INDEX8

        // Compressed formats
        INTERNAL_RGB_DXT1 = 0x83F0,                     //GL_COMPRESSED_RGB_S3TC_DXT1_EXT
        INTERNAL_RGBA_DXT1 = 0x83F1,                    //GL_COMPRESSED_RGBA_S3TC_DXT1_EXT
        INTERNAL_RGBA_DXT3 = 0x83F2,                    //GL_COMPRESSED_RGBA_S3TC_DXT3_EXT
        INTERNAL_RGBA_DXT5 = 0x83F3,                    //GL_COMPRESSED_RGBA_S3TC_DXT5_EXT
        INTERNAL_R_ATI1N_UNORM = 0x8DBB,                //GL_COMPRESSED_RED_RGTC1
        INTERNAL_R_ATI1N_SNORM = 0x8DBC,                //GL_COMPRESSED_SIGNED_RED_RGTC1
        INTERNAL_RG_ATI2N_UNORM = 0x8DBD,               //GL_COMPRESSED_RG_RGTC2
        INTERNAL_RG_ATI2N_SNORM = 0x8DBE,               //GL_COMPRESSED_SIGNED_RG_RGTC2
        INTERNAL_RGB_BP_UNSIGNED_FLOAT = 0x8E8F,        //GL_COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT
        INTERNAL_RGB_BP_SIGNED_FLOAT = 0x8E8E,          //GL_COMPRESSED_RGB_BPTC_SIGNED_FLOAT
        INTERNAL_RGB_BP_UNORM = 0x8E8C,                 //GL_COMPRESSED_RGBA_BPTC_UNORM
        INTERNAL_RGB_PVRTC_4BPPV1 = 0x8C00,             //GL_COMPRESSED_RGB_PVRTC_4BPPV1_IMG
        INTERNAL_RGB_PVRTC_2BPPV1 = 0x8C01,             //GL_COMPRESSED_RGB_PVRTC_2BPPV1_IMG
        INTERNAL_RGBA_PVRTC_4BPPV1 = 0x8C02,            //GL_COMPRESSED_RGBA_PVRTC_4BPPV1_IMG
        INTERNAL_RGBA_PVRTC_2BPPV1 = 0x8C03,            //GL_COMPRESSED_RGBA_PVRTC_2BPPV1_IMG
        INTERNAL_RGBA_PVRTC_4BPPV2 = 0x9137,            //GL_COMPRESSED_RGBA_PVRTC_4BPPV2_IMG
        INTERNAL_RGBA_PVRTC_2BPPV2 = 0x9138,            //GL_COMPRESSED_RGBA_PVRTC_2BPPV2_IMG
        INTERNAL_ATC_RGB = 0x8C92,                      //GL_ATC_RGB_AMD
        INTERNAL_ATC_RGBA_EXPLICIT_ALPHA = 0x8C93,      //GL_ATC_RGBA_EXPLICIT_ALPHA_AMD
        INTERNAL_ATC_RGBA_INTERPOLATED_ALPHA = 0x87EE,  //GL_ATC_RGBA_INTERPOLATED_ALPHA_AMD

        INTERNAL_RGB_ETC = 0x8D64,                      //GL_COMPRESSED_RGB8_ETC1
        INTERNAL_RGB_ETC2 = 0x9274,                     //GL_COMPRESSED_RGB8_ETC2
        INTERNAL_RGBA_PUNCHTHROUGH_ETC2 = 0x9276,       //GL_COMPRESSED_RGB8_PUNCHTHROUGH_ALPHA1_ETC2
        INTERNAL_RGBA_ETC2 = 0x9278,                    //GL_COMPRESSED_RGBA8_ETC2_EAC
        INTERNAL_R11_EAC = 0x9270,                      //GL_COMPRESSED_R11_EAC
        INTERNAL_SIGNED_R11_EAC = 0x9271,               //GL_COMPRESSED_SIGNED_R11_EAC
        INTERNAL_RG11_EAC = 0x9272,                     //GL_COMPRESSED_RG11_EAC
        INTERNAL_SIGNED_RG11_EAC = 0x9273,              //GL_COMPRESSED_SIGNED_RG11_EAC

        INTERNAL_RGBA_ASTC_4x4 = 0x93B0,                //GL_COMPRESSED_RGBA_ASTC_4x4_KHR
        INTERNAL_RGBA_ASTC_5x4 = 0x93B1,                //GL_COMPRESSED_RGBA_ASTC_5x4_KHR
        INTERNAL_RGBA_ASTC_5x5 = 0x93B2,                //GL_COMPRESSED_RGBA_ASTC_5x5_KHR
        INTERNAL_RGBA_ASTC_6x5 = 0x93B3,                //GL_COMPRESSED_RGBA_ASTC_6x5_KHR
        INTERNAL_RGBA_ASTC_6x6 = 0x93B4,                //GL_COMPRESSED_RGBA_ASTC_6x6_KHR
        INTERNAL_RGBA_ASTC_8x5 = 0x93B5,                //GL_COMPRESSED_RGBA_ASTC_8x5_KHR
        INTERNAL_RGBA_ASTC_8x6 = 0x93B6,                //GL_COMPRESSED_RGBA_ASTC_8x6_KHR
        INTERNAL_RGBA_ASTC_8x8 = 0x93B7,                //GL_COMPRESSED_RGBA_ASTC_8x8_KHR
        INTERNAL_RGBA_ASTC_10x5 = 0x93B8,               //GL_COMPRESSED_RGBA_ASTC_10x5_KHR
        INTERNAL_RGBA_ASTC_10x6 = 0x93B9,               //GL_COMPRESSED_RGBA_ASTC_10x6_KHR
        INTERNAL_RGBA_ASTC_10x8 = 0x93BA,               //GL_COMPRESSED_RGBA_ASTC_10x8_KHR
        INTERNAL_RGBA_ASTC_10x10 = 0x93BB,              //GL_COMPRESSED_RGBA_ASTC_10x10_KHR
        INTERNAL_RGBA_ASTC_12x10 = 0x93BC,              //GL_COMPRESSED_RGBA_ASTC_12x10_KHR
        INTERNAL_RGBA_ASTC_12x12 = 0x93BD,              //GL_COMPRESSED_RGBA_ASTC_12x12_KHR

        // sRGB formats
        INTERNAL_SRGB_DXT1 = 0x8C4C,                    //GL_COMPRESSED_SRGB_S3TC_DXT1_EXT
        INTERNAL_SRGB_ALPHA_DXT1 = 0x8C4C,              //GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT1_EXT
        INTERNAL_SRGB_ALPHA_DXT3 = 0x8C4E,              //GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT3_EXT
        INTERNAL_SRGB_ALPHA_DXT5 = 0x8C4F,              //GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT5_EXT
        INTERNAL_SRGB_BP_UNORM = 0x8E8D,                //GL_COMPRESSED_SRGB_ALPHA_BPTC_UNORM
        INTERNAL_SRGB_PVRTC_2BPPV1 = 0x8A54,            //GL_COMPRESSED_SRGB_PVRTC_2BPPV1_EXT
        INTERNAL_SRGB_PVRTC_4BPPV1 = 0x8A55,            //GL_COMPRESSED_SRGB_PVRTC_4BPPV1_EXT
        INTERNAL_SRGB_ALPHA_PVRTC_2BPPV1 = 0x8A56,      //GL_COMPRESSED_SRGB_ALPHA_PVRTC_2BPPV1_EXT
        INTERNAL_SRGB_ALPHA_PVRTC_4BPPV1 = 0x8A57,      //GL_COMPRESSED_SRGB_ALPHA_PVRTC_4BPPV1_EXT
        INTERNAL_SRGB_ALPHA_PVRTC_2BPPV2 = 0x93F0,      //COMPRESSED_SRGB_ALPHA_PVRTC_2BPPV2_IMG
        INTERNAL_SRGB_ALPHA_PVRTC_4BPPV2 = 0x93F1,      //GL_COMPRESSED_SRGB_ALPHA_PVRTC_4BPPV2_IMG
        INTERNAL_SRGB8_ETC2 = 0x9275,                       //GL_COMPRESSED_SRGB8_ETC2
        INTERNAL_SRGB8_PUNCHTHROUGH_ALPHA1_ETC2 = 0x9277,   //GL_COMPRESSED_SRGB8_PUNCHTHROUGH_ALPHA1_ETC2
        INTERNAL_SRGB8_ALPHA8_ETC2_EAC = 0x9279,            //GL_COMPRESSED_SRGB8_ALPHA8_ETC2_EAC
        INTERNAL_SRGB8_ALPHA8_ASTC_4x4 = 0x93D0,        //GL_COMPRESSED_SRGB8_ALPHA8_ASTC_4x4_KHR
        INTERNAL_SRGB8_ALPHA8_ASTC_5x4 = 0x93D1,        //GL_COMPRESSED_SRGB8_ALPHA8_ASTC_5x4_KHR
        INTERNAL_SRGB8_ALPHA8_ASTC_5x5 = 0x93D2,        //GL_COMPRESSED_SRGB8_ALPHA8_ASTC_5x5_KHR
        INTERNAL_SRGB8_ALPHA8_ASTC_6x5 = 0x93D3,        //GL_COMPRESSED_SRGB8_ALPHA8_ASTC_6x5_KHR
        INTERNAL_SRGB8_ALPHA8_ASTC_6x6 = 0x93D4,        //GL_COMPRESSED_SRGB8_ALPHA8_ASTC_6x6_KHR
        INTERNAL_SRGB8_ALPHA8_ASTC_8x5 = 0x93D5,        //GL_COMPRESSED_SRGB8_ALPHA8_ASTC_8x5_KHR
        INTERNAL_SRGB8_ALPHA8_ASTC_8x6 = 0x93D6,        //GL_COMPRESSED_SRGB8_ALPHA8_ASTC_8x6_KHR
        INTERNAL_SRGB8_ALPHA8_ASTC_8x8 = 0x93D7,        //GL_COMPRESSED_SRGB8_ALPHA8_ASTC_8x8_KHR
        INTERNAL_SRGB8_ALPHA8_ASTC_10x5 = 0x93D8,       //GL_COMPRESSED_SRGB8_ALPHA8_ASTC_10x5_KHR
        INTERNAL_SRGB8_ALPHA8_ASTC_10x6 = 0x93D9,       //GL_COMPRESSED_SRGB8_ALPHA8_ASTC_10x6_KHR
        INTERNAL_SRGB8_ALPHA8_ASTC_10x8 = 0x93DA,       //GL_COMPRESSED_SRGB8_ALPHA8_ASTC_10x8_KHR
        INTERNAL_SRGB8_ALPHA8_ASTC_10x10 = 0x93DB,      //GL_COMPRESSED_SRGB8_ALPHA8_ASTC_10x10_KHR
        INTERNAL_SRGB8_ALPHA8_ASTC_12x10 = 0x93DC,      //GL_COMPRESSED_SRGB8_ALPHA8_ASTC_12x10_KHR
        INTERNAL_SRGB8_ALPHA8_ASTC_12x12 = 0x93DD       //GL_COMPRESSED_SRGB8_ALPHA8_ASTC_12x12_KHR
    };

    public enum externalFormat
    {
        EXTERNAL_NONE = 0,                  //GL_NONE
        EXTERNAL_RED = 0x1903,              //GL_RED
        EXTERNAL_RG = 0x8227,               //GL_RG
        EXTERNAL_RGB = 0x1907,              //GL_RGB
        EXTERNAL_BGR = 0x80E0,              //GL_BGR
        EXTERNAL_RGBA = 0x1908,             //GL_RGBA
        EXTERNAL_BGRA = 0x80E1,             //GL_BGRA
        EXTERNAL_RED_INTEGER = 0x8D94,      //GL_RED_INTEGER
        EXTERNAL_RG_INTEGER = 0x8228,       //GL_RG_INTEGER
        EXTERNAL_RGB_INTEGER = 0x8D98,      //GL_RGB_INTEGER
        EXTERNAL_BGR_INTEGER = 0x8D9A,      //GL_BGR_INTEGER
        EXTERNAL_RGBA_INTEGER = 0x8D99,     //GL_RGBA_INTEGER
        EXTERNAL_BGRA_INTEGER = 0x8D9B,     //GL_BGRA_INTEGER
        EXTERNAL_DEPTH = 0x1902,            //GL_DEPTH_COMPONENT
        EXTERNAL_DEPTH_STENCIL = 0x84F9,    //GL_DEPTH_STENCIL
        EXTERNAL_STENCIL = 0x1901,          //GL_STENCIL_INDEX

        EXTERNAL_LUMINANCE = 0x1909,                //GL_LUMINANCE
        EXTERNAL_ALPHA = 0x1906,                    //GL_ALPHA
        EXTERNAL_LUMINANCE_ALPHA = 0x190A,          //GL_LUMINANCE_ALPHA
    };

    public enum typeFormat
    {
        TYPE_NONE = 0,                      //GL_NONE
        TYPE_I8 = 0x1400,                   //GL_BYTE
        TYPE_U8 = 0x1401,                   //GL_UNSIGNED_BYTE
        TYPE_I16 = 0x1402,                  //GL_SHORT
        TYPE_U16 = 0x1403,                  //GL_UNSIGNED_SHORT
        TYPE_I32 = 0x1404,                  //GL_INT
        TYPE_U32 = 0x1405,                  //GL_UNSIGNED_INT
        TYPE_F16 = 0x140B,                  //GL_HALF_FLOAT
        TYPE_F32 = 0x1406,                  //GL_FLOAT
        TYPE_UINT32_RGB9_E5 = 0x8C3E,       //GL_UNSIGNED_INT_5_9_9_9_REV
        TYPE_UINT32_RG11B10F = 0x8C3B,      //GL_UNSIGNED_INT_10F_11F_11F_REV
        TYPE_UINT8_RG3B2 = 0x8032,          //GL_UNSIGNED_BYTE_3_3_2
        TYPE_UINT8_RG3B2_REV = 0x8362,      //GL_UNSIGNED_BYTE_2_3_3_REV
        TYPE_UINT16_RGB5A1 = 0x8034,        //GL_UNSIGNED_SHORT_5_5_5_1
        TYPE_UINT16_RGB5A1_REV = 0x8366,    //GL_UNSIGNED_SHORT_1_5_5_5_REV
        TYPE_UINT16_R5G6B5 = 0x8363,        //GL_UNSIGNED_SHORT_5_6_5
        TYPE_UINT16_R5G6B5_REV = 0x8364,    //GL_UNSIGNED_SHORT_5_6_5_REV
        TYPE_UINT16_RGBA4 = 0x8033,         //GL_UNSIGNED_SHORT_4_4_4_4
        TYPE_UINT16_RGBA4_REV = 0x8365,     //GL_UNSIGNED_SHORT_4_4_4_4_REV
        TYPE_UINT32_RGB10A2 = 0x8036,       //GL_UNSIGNED_INT_10_10_10_2
        TYPE_UINT32_RGB10A2_REV = 0x8368,   //GL_UNSIGNED_INT_2_10_10_10_REV

        TYPE_UINT8_RG4_EXT = 0xFFFD,
        TYPE_F64_EXT = 0xFFFB
    };

    
    public static class GLImage
    {
        private enum CAP : ushort
        {
            CAP_COMPRESSED_BIT = (1 << 0),
            CAP_PACKED_BIT = (1 << 1),
            CAP_NORMALIZED_BIT = (1 << 2),
            CAP_SCALED_BIT = (1 << 3),
            CAP_UNSIGNED_BIT = (1 << 4),
            CAP_SIGNED_BIT = (1 << 5),
            CAP_INTEGER_BIT = (1 << 6),
            CAP_FLOAT_BIT = (1 << 7),
            CAP_DEPTH_BIT = (1 << 8),
            CAP_STENCIL_BIT = (1 << 9),
            CAP_COLORSPACE_SRGB_BIT = (1 << 10),
            CAP_SWIZZLE_BIT = (1 << 11),
            CAP_LUMINANCE_ALPHA_BIT = (1 << 12)
        };


        private struct Swizzles
        {
            public Swizzle R;
            public Swizzle G;
            public Swizzle B;
            public Swizzle A;
            public Swizzles(Swizzle r, Swizzle g, Swizzle b, Swizzle a)
            {
                R = r;
                G = g;
                B = b;
                A = a;
            }
        }
        
        private struct FormatInfo
        {
            public byte BlockSize;
            public Vec3 BlockDimensions;
            public byte Component;
            public Swizzles Swizzles;
            public CAP Flags;

            public FormatInfo(byte bs, Vec3 bd, byte c, Swizzles s, CAP f)
            {
                BlockSize = bs;
                BlockDimensions = bd;
                Component = c;
                Swizzles = s;
                Flags = f;
            }
        }

        private static FormatInfo[] formatsTable = new FormatInfo[]
            {
               new FormatInfo(  1, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_PACKED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),	//FORMAT_R4G4_UNORM,
			new FormatInfo(  1, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_PACKED_BIT | CAP.CAP_SCALED_BIT | CAP.CAP_UNSIGNED_BIT),					//FORMAT_R4G4_USCALED,
			new FormatInfo(  2, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_PACKED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),				//FORMAT_R4G4B4A4_UNORM,
			new FormatInfo(  2, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_PACKED_BIT | CAP.CAP_SCALED_BIT | CAP.CAP_UNSIGNED_BIT),					//FORMAT_R4G4B4A4_USCALED,
			new FormatInfo(  2, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_PACKED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),				//FORMAT_R5G6B5_UNORM,
			new FormatInfo(  2, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_PACKED_BIT | CAP.CAP_SCALED_BIT | CAP.CAP_UNSIGNED_BIT),					//FORMAT_R5G6B5_USCALED,
			new FormatInfo(  2, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_PACKED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),				//FORMAT_R5G5B5A1_UNORM,
			new FormatInfo(  2, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_PACKED_BIT | CAP.CAP_SCALED_BIT | CAP.CAP_UNSIGNED_BIT),					//FORMAT_R5G5B5A1_USCALED,

			new FormatInfo(  1, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_R8_UNORM,
			new FormatInfo(  1, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_NORMALIZED_BIT | CAP.CAP_SIGNED_BIT),									//FORMAT_R8_SNORM,
			new FormatInfo(  1, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_SCALED_BIT | CAP.CAP_UNSIGNED_BIT),										//FORMAT_R8_USCALED,
			new FormatInfo(  1, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_SCALED_BIT | CAP.CAP_SIGNED_BIT),										//FORMAT_R8_SSCALED,
			new FormatInfo(  1, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_INTEGER_BIT | CAP.CAP_UNSIGNED_BIT),										//FORMAT_R8_UINT,
			new FormatInfo(  1, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_INTEGER_BIT | CAP.CAP_SIGNED_BIT),										//FORMAT_R8_SINT,
			new FormatInfo(  1, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT),		//FORMAT_R8_SRGB,

			new FormatInfo(  2, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RG8_UNORM,
			new FormatInfo(  2, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_NORMALIZED_BIT | CAP.CAP_SIGNED_BIT),									//FORMAT_RG8_SNORM,
			new FormatInfo(  2, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_SCALED_BIT | CAP.CAP_UNSIGNED_BIT),										//FORMAT_RG8_USCALED,
			new FormatInfo(  2, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_SCALED_BIT | CAP.CAP_SIGNED_BIT),										//FORMAT_RG8_SSCALED,
			new FormatInfo(  2, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_INTEGER_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RG8_UINT,
			new FormatInfo(  2, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_INTEGER_BIT | CAP.CAP_SIGNED_BIT),										//FORMAT_RG8_SINT,
			new FormatInfo(  2, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_NORMALIZED_BIT | CAP.CAP_SIGNED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT),			//FORMAT_RG8_SRGB,

			new FormatInfo(  3, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGB8_UNORM,
			new FormatInfo(  3, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_NORMALIZED_BIT | CAP.CAP_SIGNED_BIT),									//FORMAT_RGB8_SNORM,
			new FormatInfo(  3, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_SCALED_BIT | CAP.CAP_UNSIGNED_BIT),										//FORMAT_RGB8_USCALED,
			new FormatInfo(  3, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_SCALED_BIT | CAP.CAP_SIGNED_BIT),										//FORMAT_RGB8_SSCALED,
			new FormatInfo(  3, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_INTEGER_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGB8_UINT,
			new FormatInfo(  3, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_INTEGER_BIT | CAP.CAP_SIGNED_BIT),										//FORMAT_RGB8_SINT,
			new FormatInfo(  3, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT),		//FORMAT_RGB8_SRGB,

			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),								//FORMAT_RGBA8_UNORM,
			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_NORMALIZED_BIT | CAP.CAP_SIGNED_BIT),									//FORMAT_RGBA8_SNORM,
			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_SCALED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGBA8_USCALED,
			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_SCALED_BIT | CAP.CAP_SIGNED_BIT),										//FORMAT_RGBA8_SSCALED,
			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_INTEGER_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGBA8_UINT,
			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_INTEGER_BIT | CAP.CAP_SIGNED_BIT),									//FORMAT_RGBA8_SINT,
			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT),		//FORMAT_RGBA8_SRGB,

			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_PACKED_BIT),				//FORMAT_RGB10A2_UNORM,
			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_NORMALIZED_BIT | CAP.CAP_SIGNED_BIT | CAP.CAP_PACKED_BIT),				//FORMAT_RGB10A2_SNORM,
			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_SCALED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_PACKED_BIT),					//FORMAT_RGB10A2_USCALE,
			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_SCALED_BIT | CAP.CAP_SIGNED_BIT | CAP.CAP_PACKED_BIT),					//FORMAT_RGB10A2_SSCALE,
			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_INTEGER_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_PACKED_BIT),					//FORMAT_RGB10A2_UINT,
			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_INTEGER_BIT | CAP.CAP_SIGNED_BIT | CAP.CAP_PACKED_BIT),					//FORMAT_RGB10A2_SINT,

			new FormatInfo(  2, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_R16_UNORM,
			new FormatInfo(  2, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_NORMALIZED_BIT | CAP.CAP_SIGNED_BIT),									//FORMAT_R16_SNORM,
			new FormatInfo(  2, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_SCALED_BIT | CAP.CAP_UNSIGNED_BIT),										//FORMAT_R16_USCALE,
			new FormatInfo(  2, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_SCALED_BIT | CAP.CAP_SIGNED_BIT),										//FORMAT_R16_SSCALE,
			new FormatInfo(  2, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_INTEGER_BIT | CAP.CAP_UNSIGNED_BIT),										//FORMAT_R16_UINT,
			new FormatInfo(  2, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_INTEGER_BIT | CAP.CAP_SIGNED_BIT),										//FORMAT_R16_SINT,
			new FormatInfo(  2, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_FLOAT_BIT | CAP.CAP_SIGNED_BIT),											//FORMAT_R16_SFLOAT,

			new FormatInfo(  4, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RG16_UNORM,
			new FormatInfo(  4, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_NORMALIZED_BIT | CAP.CAP_SIGNED_BIT),									//FORMAT_RG16_SNORM,
			new FormatInfo(  4, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_SCALED_BIT | CAP.CAP_UNSIGNED_BIT),										//FORMAT_RG16_USCALE,
			new FormatInfo(  4, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_SCALED_BIT | CAP.CAP_SIGNED_BIT),										//FORMAT_RG16_SSCALE,
			new FormatInfo(  4, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_INTEGER_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RG16_UINT,
			new FormatInfo(  4, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_INTEGER_BIT | CAP.CAP_SIGNED_BIT),										//FORMAT_RG16_SINT,
			new FormatInfo(  4, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_FLOAT_BIT | CAP.CAP_SIGNED_BIT),										//FORMAT_RG16_SFLOAT,

			new FormatInfo(  6, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGB16_UNORM,
			new FormatInfo(  6, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_NORMALIZED_BIT | CAP.CAP_SIGNED_BIT),		//FORMAT_RGB16_SNORM,
			new FormatInfo(  6, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_SCALED_BIT | CAP.CAP_UNSIGNED_BIT),			//FORMAT_RGB16_USCALE,
			new FormatInfo(  6, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_SCALED_BIT | CAP.CAP_SIGNED_BIT),			//FORMAT_RGB16_SSCALE,
			new FormatInfo(  6, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_INTEGER_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGB16_UINT,
			new FormatInfo(  6, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_INTEGER_BIT | CAP.CAP_SIGNED_BIT),			//FORMAT_RGB16_SINT,
			new FormatInfo(  6, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_FLOAT_BIT | CAP.CAP_SIGNED_BIT),			//FORMAT_RGB16_SFLOAT,

			new FormatInfo(  8, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),	//FORMAT_RGBA16_UNORM,
			new FormatInfo(  8, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_NORMALIZED_BIT | CAP.CAP_SIGNED_BIT),		//FORMAT_RGBA16_SNORM,
			new FormatInfo(  8, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_SCALED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGBA16_USCALE,
			new FormatInfo(  8, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_SCALED_BIT | CAP.CAP_SIGNED_BIT),			//FORMAT_RGBA16_SSCALE,
			new FormatInfo(  8, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_INTEGER_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGBA16_UINT,
			new FormatInfo(  8, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_INTEGER_BIT | CAP.CAP_SIGNED_BIT),		//FORMAT_RGBA16_SINT,
			new FormatInfo(  8, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_FLOAT_BIT | CAP.CAP_SIGNED_BIT),			//FORMAT_RGBA16_SFLOAT,

			new FormatInfo(  4, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_INTEGER_BIT | CAP.CAP_UNSIGNED_BIT),			//FORMAT_R32_UINT,
			new FormatInfo(  4, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_INTEGER_BIT | CAP.CAP_SIGNED_BIT),			//FORMAT_R32_SINT,
			new FormatInfo(  4, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_FLOAT_BIT | CAP.CAP_SIGNED_BIT),				//FORMAT_R32_SFLOAT,

			new FormatInfo(  8, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_INTEGER_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RG32_UINT,
			new FormatInfo(  8, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_INTEGER_BIT | CAP.CAP_SIGNED_BIT),			//FORMAT_RG32_SINT,
			new FormatInfo(  8, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_FLOAT_BIT | CAP.CAP_SIGNED_BIT),			//FORMAT_RG32_SFLOAT,

			new FormatInfo( 12, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_INTEGER_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGB32_UINT,
			new FormatInfo( 12, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_INTEGER_BIT | CAP.CAP_SIGNED_BIT),			//FORMAT_RGB32_SINT,
			new FormatInfo( 12, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_FLOAT_BIT | CAP.CAP_SIGNED_BIT),			//FORMAT_RGB32_SFLOAT,

			new FormatInfo( 16, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_INTEGER_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGBA32_UINT,
			new FormatInfo( 16, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_INTEGER_BIT | CAP.CAP_SIGNED_BIT),		//FORMAT_RGBA32_SINT,
			new FormatInfo( 16, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_FLOAT_BIT | CAP.CAP_SIGNED_BIT),			//FORMAT_RGBA32_SFLOAT,

			new FormatInfo(  8, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_FLOAT_BIT | CAP.CAP_SIGNED_BIT),				//FORMAT_R64_SFLOAT,
			new FormatInfo( 16, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_FLOAT_BIT | CAP.CAP_SIGNED_BIT),			//FORMAT_RG64_SFLOAT,
			new FormatInfo( 24, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_FLOAT_BIT | CAP.CAP_SIGNED_BIT),			//FORMAT_RGB64_SFLOAT,
			new FormatInfo( 32, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_FLOAT_BIT | CAP.CAP_SIGNED_BIT),			//FORMAT_RGBA64_SFLOAT,

			new FormatInfo(  4, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_PACKED_BIT | CAP.CAP_FLOAT_BIT | CAP.CAP_SIGNED_BIT),		//FORMAT_RG11B10_UFLOAT,
			new FormatInfo(  4, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_PACKED_BIT | CAP.CAP_FLOAT_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGB9E5_UFLOAT,

			new FormatInfo(  2, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_DEPTH_BIT | CAP.CAP_INTEGER_BIT),						//FORMAT_D16_UNORM,
			new FormatInfo(  3, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_DEPTH_BIT | CAP.CAP_INTEGER_BIT),						//FORMAT_D24_UNORM,
			new FormatInfo(  4, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_DEPTH_BIT | CAP.CAP_FLOAT_BIT),						//FORMAT_D32_UFLOAT,
			new FormatInfo(  1, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_DEPTH_BIT | CAP.CAP_STENCIL_BIT),						//FORMAT_S8_UNORM,
			new FormatInfo(  3, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_DEPTH_BIT | CAP.CAP_INTEGER_BIT | CAP.CAP_STENCIL_BIT),	//FORMAT_D16_UNORM_S8_UINT,
			new FormatInfo(  4, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_DEPTH_BIT | CAP.CAP_INTEGER_BIT | CAP.CAP_STENCIL_BIT),	//FORMAT_D24_UNORM_S8_UINT,
			new FormatInfo(  5, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_DEPTH_BIT | CAP.CAP_FLOAT_BIT | CAP.CAP_STENCIL_BIT),		//FORMAT_D32_SFLOAT_S8_UINT,

			new FormatInfo(  8, new Vec3(4, 4, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGB_DXT1_UNORM,
			new FormatInfo(  8, new Vec3(4, 4, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),			//FORMAT_RGB_DXT1_SRGB,
			new FormatInfo(  8, new Vec3(4, 4, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGBA_DXT1_UNORM,
			new FormatInfo(  8, new Vec3(4, 4, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGBA_DXT1_SRGB,
			new FormatInfo( 16, new Vec3(4, 4, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGBA_DXT3_UNORM,
			new FormatInfo( 16, new Vec3(4, 4, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGBA_DXT3_SRGB,
			new FormatInfo( 16, new Vec3(4, 4, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGBA_DXT5_UNORM,
			new FormatInfo( 16, new Vec3(4, 4, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGBA_DXT5_SRGB,
			new FormatInfo(  8, new Vec3(4, 4, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),										//FORMAT_R_ATI1N_UNORM,
			new FormatInfo(  8, new Vec3(4, 4, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_SIGNED_BIT),										//FORMAT_R_ATI1N_SNORM,
			new FormatInfo( 16, new Vec3(4, 4, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),										//FORMAT_RG_ATI2N_UNORM,
			new FormatInfo( 16, new Vec3(4, 4, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_SIGNED_BIT),										//FORMAT_RG_ATI2N_SNORM,
			new FormatInfo( 16, new Vec3(4, 4, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_COMPRESSED_BIT | CAP.CAP_FLOAT_BIT | CAP.CAP_UNSIGNED_BIT),											//FORMAT_RGB_BP_UFLOAT,
			new FormatInfo( 16, new Vec3(4, 4, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_COMPRESSED_BIT | CAP.CAP_FLOAT_BIT | CAP.CAP_SIGNED_BIT),											//FORMAT_RGB_BP_SFLOAT,
			new FormatInfo( 16, new Vec3(4, 4, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGB_BP_UNORM,
			new FormatInfo( 16, new Vec3(4, 4, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),			//FORMAT_RGB_BP_SRGB,

			new FormatInfo(  8, new Vec3(4, 4, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGB_ETC2_UNORM,
			new FormatInfo(  8, new Vec3(4, 4, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGB_ETC2_SRGB,
			new FormatInfo(  8, new Vec3(4, 4, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGBA_ETC2_PUNCHTHROUGH_UNORM,
			new FormatInfo(  8, new Vec3(4, 4, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGBA_ETC2_PUNCHTHROUGH_SRGB,
			new FormatInfo( 16, new Vec3(4, 4, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGBA_ETC2_UNORM,
			new FormatInfo( 16, new Vec3(4, 4, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGBA_ETC2_SRGB,
			new FormatInfo(  8, new Vec3(4, 4, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_R11_EAC_UNORM,
			new FormatInfo(  8, new Vec3(4, 4, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_SIGNED_BIT),									//FORMAT_R11_EAC_SNORM,
			new FormatInfo( 16, new Vec3(4, 4, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RG11_EAC_UNORM,
			new FormatInfo( 16, new Vec3(4, 4, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_SIGNED_BIT),									//FORMAT_RG11_EAC_SNORM,

			new FormatInfo( 16, new Vec3(4, 4, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGBA_ASTC4X4_UNORM,
			new FormatInfo( 16, new Vec3(4, 4, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGBA_ASTC4X4_SRGB,
			new FormatInfo( 16, new Vec3(5, 4, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGBA_ASTC5X4_UNORM,
			new FormatInfo( 16, new Vec3(5, 4, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGBA_ASTC5X4_SRGB,
			new FormatInfo( 16, new Vec3(5, 5, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGBA_ASTC5X5_UNORM,
			new FormatInfo( 16, new Vec3(5, 5, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGBA_ASTC5X5_SRGB,
			new FormatInfo( 16, new Vec3(6, 5, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGBA_ASTC6X5_UNORM,
			new FormatInfo( 16, new Vec3(6, 5, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGBA_ASTC6X5_SRGB,
			new FormatInfo( 16, new Vec3(6, 6, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGBA_ASTC6X6_UNORM,
			new FormatInfo( 16, new Vec3(6, 6, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGBA_ASTC6X6_SRGB,
			new FormatInfo( 16, new Vec3(8, 5, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGBA_ASTC8X5_UNORM,
			new FormatInfo( 16, new Vec3(8, 5, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGBA_ASTC8X5_SRGB,
			new FormatInfo( 16, new Vec3(8, 6, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGBA_ASTC8X6_UNORM,
			new FormatInfo( 16, new Vec3(8, 6, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGBA_ASTC8X6_SRGB,
			new FormatInfo( 16, new Vec3(8, 8, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGBA_ASTC8X8_UNORM,
			new FormatInfo( 16, new Vec3(8, 8, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGBA_ASTC8X8_SRGB,
			new FormatInfo( 16, new Vec3(10, 5, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGBA_ASTC10X5_UNORM,
			new FormatInfo( 16, new Vec3(10, 5, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGBA_ASTC10X5_SRGB,
			new FormatInfo( 16, new Vec3(10, 6, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGBA_ASTC10X6_UNORM,
			new FormatInfo( 16, new Vec3(10, 6, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGBA_ASTC10X6_SRGB,
			new FormatInfo( 16, new Vec3(10, 8, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RGBA_ASTC10X8_UNORM,
			new FormatInfo( 16, new Vec3(10, 8, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGBA_ASTC10X8_SRGB,
			new FormatInfo( 16, new Vec3(10, 10, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),								//FORMAT_RGBA_ASTC10X10_UNORM,
			new FormatInfo( 16, new Vec3(10, 10, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGBA_ASTC10X10_SRGB,
			new FormatInfo( 16, new Vec3(12, 10, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),								//FORMAT_RGBA_ASTC12X10_UNORM,
			new FormatInfo( 16, new Vec3(12, 10, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGBA_ASTC12X10_SRGB,
			new FormatInfo( 16, new Vec3(12, 12, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),								//FORMAT_RGBA_ASTC12X12_UNORM,
			new FormatInfo( 16, new Vec3(12, 12, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGBA_ASTC12X12_SRGB,

			new FormatInfo(  2, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ALPHA), CAP.CAP_PACKED_BIT| CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_SWIZZLE_BIT),					//FORMAT_BGRA4_UNORM,
			new FormatInfo(  2, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ALPHA), CAP.CAP_PACKED_BIT| CAP.CAP_SCALED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_SWIZZLE_BIT),						//FORMAT_BGRA4_USCALED,
			new FormatInfo(  2, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ONE), CAP.CAP_PACKED_BIT| CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_SWIZZLE_BIT),					//FORMAT_B5G6R5_UNORM,
			new FormatInfo(  2, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ONE), CAP.CAP_PACKED_BIT| CAP.CAP_SCALED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_SWIZZLE_BIT),						//FORMAT_B5G6R5_USCALED,
			new FormatInfo(  2, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ALPHA), CAP.CAP_PACKED_BIT| CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_SWIZZLE_BIT),					//FORMAT_BGR5A1_UNORM,
			new FormatInfo(  2, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ALPHA), CAP.CAP_PACKED_BIT| CAP.CAP_SCALED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_SWIZZLE_BIT),						//FORMAT_BGRA1_USCALED,

			new FormatInfo(  3, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ONE), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_SWIZZLE_BIT),									//FORMAT_BGR8_UNORM,
			new FormatInfo(  3, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ONE), CAP.CAP_NORMALIZED_BIT | CAP.CAP_SIGNED_BIT | CAP.CAP_SWIZZLE_BIT),										//FORMAT_BGR8_SNORM,
			new FormatInfo(  3, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ONE), CAP.CAP_SCALED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_SWIZZLE_BIT),										//FORMAT_BGR8_USCALED,
			new FormatInfo(  3, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ONE), CAP.CAP_SCALED_BIT | CAP.CAP_SIGNED_BIT | CAP.CAP_SWIZZLE_BIT),											//FORMAT_BGR8_SSCALED,
			new FormatInfo(  3, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ONE), CAP.CAP_INTEGER_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_SWIZZLE_BIT),										//FORMAT_BGR8_UINT,
			new FormatInfo(  3, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ONE), CAP.CAP_INTEGER_BIT | CAP.CAP_SIGNED_BIT | CAP.CAP_SWIZZLE_BIT),										//FORMAT_BGR8_SINT,
			new FormatInfo(  3, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ONE), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_SWIZZLE_BIT),			//FORMAT_BGR8_SRGB,

			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ALPHA), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_SWIZZLE_BIT),									//FORMAT_BGRA8_UNORM,
			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ALPHA), CAP.CAP_NORMALIZED_BIT | CAP.CAP_SIGNED_BIT | CAP.CAP_SWIZZLE_BIT),									//FORMAT_BGRA8_SNORM,
			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ALPHA), CAP.CAP_SCALED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_SWIZZLE_BIT),										//FORMAT_BGRA8_USCALED,
			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ALPHA), CAP.CAP_SCALED_BIT | CAP.CAP_SIGNED_BIT | CAP.CAP_SWIZZLE_BIT),										//FORMAT_BGRA8_SSCALED,
			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ALPHA), CAP.CAP_INTEGER_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_SWIZZLE_BIT),									//FORMAT_BGRA8_UINT,
			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ALPHA), CAP.CAP_INTEGER_BIT | CAP.CAP_SIGNED_BIT | CAP.CAP_SWIZZLE_BIT),										//FORMAT_BGRA8_SINT,
			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ALPHA), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_SWIZZLE_BIT),		//FORMAT_BGRA8_SRGB,

			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ALPHA), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_PACKED_BIT | CAP.CAP_SWIZZLE_BIT),				//FORMAT_BGR10A2_UNORM,
			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ALPHA), CAP.CAP_NORMALIZED_BIT | CAP.CAP_SIGNED_BIT | CAP.CAP_PACKED_BIT | CAP.CAP_SWIZZLE_BIT),					//FORMAT_BGR10A2_SNORM,
			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ALPHA), CAP.CAP_SCALED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_PACKED_BIT | CAP.CAP_SWIZZLE_BIT),					//FORMAT_BGR10A2_USCALE,
			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ALPHA), CAP.CAP_SCALED_BIT | CAP.CAP_SIGNED_BIT | CAP.CAP_PACKED_BIT | CAP.CAP_SWIZZLE_BIT),						//FORMAT_BGR10A2_SSCALE,
			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ALPHA), CAP.CAP_INTEGER_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_PACKED_BIT | CAP.CAP_SWIZZLE_BIT),					//FORMAT_BGR10A2_UINT,
			new FormatInfo(  4, new Vec3(1, 1, 1), 4, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ALPHA), CAP.CAP_INTEGER_BIT | CAP.CAP_SIGNED_BIT | CAP.CAP_PACKED_BIT | CAP.CAP_SWIZZLE_BIT),						//FORMAT_BGR10A2_SINT,

			new FormatInfo(  1, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_PACKED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),									//FORMAT_RG3B2_UNORM,
			new FormatInfo(  4, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ONE), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_SWIZZLE_BIT),									//FORMAT_BGRX8_UNORM,
			new FormatInfo(  4, new Vec3(1, 1, 1), 3, new Swizzles(Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ONE), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_SWIZZLE_BIT | CAP.CAP_COLORSPACE_SRGB_BIT),			//FORMAT_BGRX8_SRGB,

			new FormatInfo(  1, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_LUMINANCE_ALPHA_BIT),							//FORMAT_L8_UNORM,
			new FormatInfo(  1, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_RED), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_LUMINANCE_ALPHA_BIT),							//FORMAT_A8_UNORM,
			new FormatInfo(  2, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_GREEN), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_LUMINANCE_ALPHA_BIT),							//FORMAT_LA8_UNORM,
			new FormatInfo(  2, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ONE), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_LUMINANCE_ALPHA_BIT),							//FORMAT_L16_UNORM,
			new FormatInfo(  2, new Vec3(1, 1, 1), 1, new Swizzles(Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_RED), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_LUMINANCE_ALPHA_BIT),							//FORMAT_A16_UNORM,
			new FormatInfo(  4, new Vec3(1, 1, 1), 2, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_ZERO, Swizzle.SWIZZLE_GREEN), CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT | CAP.CAP_LUMINANCE_ALPHA_BIT),							//FORMAT_LA16_UNORM,

			new FormatInfo( 32, new Vec3(8, 8, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),								//FORMAT_RGB_PVRTC1_8X8_UNORM,
			new FormatInfo( 32, new Vec3(8, 8, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGB_PVRTC1_8X8_SRGB,
			new FormatInfo( 32, new Vec3(16, 8, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),								//FORMAT_RGB_PVRTC1_16X8_UNORM,
			new FormatInfo( 32, new Vec3(16, 8, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),		//FORMAT_RGB_PVRTC1_16X8_SRGB,
			new FormatInfo( 32, new Vec3(8, 8, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),								//FORMAT_RGBA_PVRTC1_8X8_UNORM,
			new FormatInfo( 32, new Vec3(8, 8, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),	//FORMAT_RGBA_PVRTC1_8X8_SRGB,
			new FormatInfo( 32, new Vec3(16, 8, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),								//FORMAT_RGBA_PVRTC1_16X8_UNORM,
			new FormatInfo( 32, new Vec3(16, 8, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),	//FORMAT_RGBA_PVRTC1_16X8_SRGB,
			new FormatInfo(  8, new Vec3(4, 4, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),								//FORMAT_RGBA_PVRTC2_4X4_UNORM,
			new FormatInfo(  8, new Vec3(4, 4, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),	//FORMAT_RGBA_PVRTC2_4X4_SRGB,
			new FormatInfo(  8, new Vec3(8, 4, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),								//FORMAT_RGBA_PVRTC2_8X4_UNORM,
			new FormatInfo(  8, new Vec3(8, 4, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_COLORSPACE_SRGB_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),	//FORMAT_RGBA_PVRTC2_8X4_SRGB,

			new FormatInfo(  8, new Vec3(4, 4, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),								//FORMAT_RGB_ETC_UNORM,
			new FormatInfo(  8, new Vec3(4, 4, 1), 3, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ONE), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),								//FORMAT_RGB_ATC_UNORM,
			new FormatInfo( 16, new Vec3(4, 4, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),								//FORMAT_RGBA_ATC_EXPLICIT_UNORM,
			new FormatInfo( 16, new Vec3(4, 4, 1), 4, new Swizzles(Swizzle.SWIZZLE_RED, Swizzle.SWIZZLE_GREEN, Swizzle.SWIZZLE_BLUE, Swizzle.SWIZZLE_ALPHA), CAP.CAP_COMPRESSED_BIT | CAP.CAP_NORMALIZED_BIT | CAP.CAP_UNSIGNED_BIT),								//FORMAT_RGBA_ATC_INTERPOLATED_UNORM,
 };

        private static FormatInfo GetFormatInfo(format format)
        {
            return formatsTable[(int)format-(int)(format.FORMAT_FIRST)];
        }

        public static int BitsPerPixel(format format)
        {
            FormatInfo info = GetFormatInfo(format);

            return info.BlockSize * 8 / (info.BlockDimensions.X * info.BlockDimensions.Y * info.BlockDimensions.Z);
        }

        public static bool IsCompressed(format format)
        {
            return GetFormatInfo(format).Flags.HasFlag(CAP.CAP_COMPRESSED_BIT);
        }

        public static bool IsSrgb(format format)
        {
            return GetFormatInfo(format).Flags.HasFlag(CAP.CAP_COLORSPACE_SRGB_BIT);
        }

        public static uint BlockSize(format format)
        {
            return GetFormatInfo(format).BlockSize;
        }

        public static Vec3 BlockDimensions(format format)
        {
            return GetFormatInfo(format).BlockDimensions;
        }

        public static uint ComponentCount(format format)
        {
            return GetFormatInfo(format).Component;
        }
    }

    public enum target
    {
        TARGET_1D = TextureTarget.Texture1D, TARGET_FIRST = TARGET_1D,
        TARGET_1D_ARRAY = TextureTarget.Texture1DArray,
        TARGET_2D = TextureTarget.Texture2D,
        TARGET_2D_ARRAY = TextureTarget.Texture2DArray,
        TARGET_3D = TextureTarget.Texture3D,
        TARGET_RECT = TextureTarget.TextureRectangle,
        TARGET_RECT_ARRAY = TextureTarget.TextureRectangle, // Not supported by OpenGL
        TARGET_CUBE = TextureTarget.TextureCubeMap,
        TARGET_CUBE_ARRAY = TextureTarget.TextureCubeMapArray, TARGET_LAST = TARGET_CUBE_ARRAY
    }

    public static class GLFormat
    {
        private struct Glformat
        {
            public internalFormat Internal;
            public externalFormat External;
            public typeFormat Type;
            public Swizzle Swizzles_R;
            public Swizzle Swizzles_G;
            public Swizzle Swizzles_B;
            public Swizzle Swizzles_A;

            public Glformat(internalFormat i, externalFormat e, typeFormat t, uint r, uint g, uint b, uint a)
            {
                Internal = i;
                External = e;
                Type = t;
                Swizzles_R = (Swizzle)r;
                Swizzles_G = (Swizzle)g;
                Swizzles_B = (Swizzle)r;
                Swizzles_A = (Swizzle)g;
            }
            public Glformat(internalFormat i, externalFormat e, typeFormat t)
                : this(i,e,t,0,0,0,0)
            {
              }
        }

        private static Glformat[] ConversionTable = new Glformat[]
        {
            new Glformat(internalFormat.INTERNAL_RG4_EXT, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_UINT8_RG4_EXT),			//FORMAT_R4G4_UNORM,
			new Glformat(internalFormat.INTERNAL_RG4_EXT, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_UINT8_RG4_EXT),			//FORMAT_R4G4_USCALED,
			new Glformat(internalFormat.INTERNAL_RGBA4, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_UINT16_RGBA4),				//FORMAT_R4G4B4A4_UNORM,
			new Glformat(internalFormat.INTERNAL_RGBA4, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_UINT16_RGBA4),				//FORMAT_R4G4B4A4_USCALED,
			new Glformat(internalFormat.INTERNAL_R5G6B5, externalFormat.EXTERNAL_RGB, typeFormat.TYPE_UINT16_R5G6B5),			//FORMAT_R5G6B5_UNORM,
			new Glformat(internalFormat.INTERNAL_R5G6B5, externalFormat.EXTERNAL_RGB, typeFormat.TYPE_UINT16_R5G6B5),			//FORMAT_R5G6B5_USCALED,
			new Glformat(internalFormat.INTERNAL_RGB5A1, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_UINT16_RGB5A1),			//FORMAT_R5G5B5A1_UNORM,
			new Glformat(internalFormat.INTERNAL_RGB5A1, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_UINT16_RGB5A1),			//FORMAT_R5G5B5A1_USCALED,

			new Glformat(internalFormat.INTERNAL_R8_UNORM, externalFormat.EXTERNAL_RED, typeFormat.TYPE_U8),						//FORMAT_R8_UNORM,
			new Glformat(internalFormat.INTERNAL_R8_SNORM, externalFormat.EXTERNAL_RED, typeFormat.TYPE_I8),						//FORMAT_R8_SNORM,
			new Glformat(internalFormat.INTERNAL_R8_UNORM, externalFormat.EXTERNAL_RED, typeFormat.TYPE_U8),						//FORMAT_R8_USCALED,
			new Glformat(internalFormat.INTERNAL_R8_SNORM, externalFormat.EXTERNAL_RED, typeFormat.TYPE_I8),						//FORMAT_R8_SSCALED,
			new Glformat(internalFormat.INTERNAL_R8U, externalFormat.EXTERNAL_RED_INTEGER, typeFormat.TYPE_U8),					//FORMAT_R8_UINT,
			new Glformat(internalFormat.INTERNAL_R8I, externalFormat.EXTERNAL_RED_INTEGER, typeFormat.TYPE_I8),					//FORMAT_R8_SINT,
			new Glformat(internalFormat.INTERNAL_SR8, externalFormat.EXTERNAL_RED, typeFormat.TYPE_U8),							//FORMAT_R8_SRGB,

			new Glformat(internalFormat.INTERNAL_RG8_UNORM, externalFormat.EXTERNAL_RG, typeFormat.TYPE_U8),						//FORMAT_RG8_UNORM,
			new Glformat(internalFormat.INTERNAL_RG8_SNORM, externalFormat.EXTERNAL_RG, typeFormat.TYPE_I8),						//FORMAT_RG8_SNORM,
			new Glformat(internalFormat.INTERNAL_RG8_UNORM, externalFormat.EXTERNAL_RG, typeFormat.TYPE_U8),						//FORMAT_RG8_USCALED,
			new Glformat(internalFormat.INTERNAL_RG8_SNORM, externalFormat.EXTERNAL_RG, typeFormat.TYPE_I8),						//FORMAT_RG8_SSCALED,
			new Glformat(internalFormat.INTERNAL_RG8U, externalFormat.EXTERNAL_RG_INTEGER, typeFormat.TYPE_U8),					//FORMAT_RG8_UINT,
			new Glformat(internalFormat.INTERNAL_RG8I, externalFormat.EXTERNAL_RG_INTEGER, typeFormat.TYPE_I8),					//FORMAT_RG8_SINT,
			new Glformat(internalFormat.INTERNAL_SRG8, externalFormat.EXTERNAL_RG, typeFormat.TYPE_U8),							//FORMAT_RG8_SRGB,

			new Glformat(internalFormat.INTERNAL_RGB8_UNORM, externalFormat.EXTERNAL_RGB, typeFormat.TYPE_U8),					//FORMAT_RGB8_UNORM,
			new Glformat(internalFormat.INTERNAL_RGB8_SNORM, externalFormat.EXTERNAL_RGB, typeFormat.TYPE_I8),					//FORMAT_RGB8_SNORM,
			new Glformat(internalFormat.INTERNAL_RGB8_UNORM, externalFormat.EXTERNAL_RGB, typeFormat.TYPE_U8),					//FORMAT_RGB8_USCALED,
			new Glformat(internalFormat.INTERNAL_RGB8_SNORM, externalFormat.EXTERNAL_RGB, typeFormat.TYPE_I8),					//FORMAT_RGB8_SSCALED,
			new Glformat(internalFormat.INTERNAL_RGB8U, externalFormat.EXTERNAL_RGB_INTEGER, typeFormat.TYPE_U8),				//FORMAT_RGB8_UINT,
			new Glformat(internalFormat.INTERNAL_RGB8I, externalFormat.EXTERNAL_RGB_INTEGER, typeFormat.TYPE_I8),				//FORMAT_RGB8_SINT,
			new Glformat(internalFormat.INTERNAL_SRGB8, externalFormat.EXTERNAL_RGB, typeFormat.TYPE_U8),						//FORMAT_RGB8_SRGB,

			new Glformat(internalFormat.INTERNAL_RGBA8_UNORM, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_U8),					//FORMAT_RGBA8_UNORM,
			new Glformat(internalFormat.INTERNAL_RGBA8_SNORM, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_I8),					//FORMAT_RGBA8_SNORM,
			new Glformat(internalFormat.INTERNAL_RGBA8_UNORM, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_U8),					//FORMAT_RGBA8_USCALED,
			new Glformat(internalFormat.INTERNAL_RGBA8_SNORM, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_I8),					//FORMAT_RGBA8_SSCALED,
			new Glformat(internalFormat.INTERNAL_RGBA8U, externalFormat.EXTERNAL_RGBA_INTEGER, typeFormat.TYPE_U8),				//FORMAT_RGBA8_UINT,
			new Glformat(internalFormat.INTERNAL_RGBA8I, externalFormat.EXTERNAL_RGBA_INTEGER, typeFormat.TYPE_I8),				//FORMAT_RGBA8_SINT,
			new Glformat(internalFormat.INTERNAL_SRGB8_ALPHA8, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_U8),				//FORMAT_RGBA8_SRGB,

			new Glformat(internalFormat.INTERNAL_RGB10A2_UNORM, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_UINT32_RGB10A2),			//FORMAT_RGB10A2_UNORM,
			new Glformat(internalFormat.INTERNAL_RGB10A2_SNORM_EXT, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_UINT32_RGB10A2),		//FORMAT_RGB10A2_SNORM,
			new Glformat(internalFormat.INTERNAL_RGB10A2_UNORM, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_UINT32_RGB10A2),			//FORMAT_RGB10A2_USCALE,
			new Glformat(internalFormat.INTERNAL_RGB10A2_SNORM_EXT, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_UINT32_RGB10A2),		//FORMAT_RGB10A2_SSCALE,
			new Glformat(internalFormat.INTERNAL_RGB10A2U, externalFormat.EXTERNAL_RGBA_INTEGER, typeFormat.TYPE_UINT32_RGB10A2),		//FORMAT_RGB10A2_UINT,
			new Glformat(internalFormat.INTERNAL_RGB10A2I_EXT, externalFormat.EXTERNAL_RGBA_INTEGER, typeFormat.TYPE_UINT32_RGB10A2),	//FORMAT_RGB10A2_SINT,

			new Glformat(internalFormat.INTERNAL_R16_UNORM, externalFormat.EXTERNAL_RED, typeFormat.TYPE_U16),					//FORMAT_R16_UNORM,
			new Glformat(internalFormat.INTERNAL_R16_SNORM, externalFormat.EXTERNAL_RED, typeFormat.TYPE_I16),					//FORMAT_R16_SNORM,
			new Glformat(internalFormat.INTERNAL_R16_UNORM, externalFormat.EXTERNAL_RED, typeFormat.TYPE_U16),					//FORMAT_R16_UNORM,
			new Glformat(internalFormat.INTERNAL_R16_SNORM, externalFormat.EXTERNAL_RED, typeFormat.TYPE_I16),					//FORMAT_R16_SNORM,
			new Glformat(internalFormat.INTERNAL_R16U, externalFormat.EXTERNAL_RED_INTEGER, typeFormat.TYPE_U16),				//FORMAT_R16_UINT,
			new Glformat(internalFormat.INTERNAL_R16I, externalFormat.EXTERNAL_RED_INTEGER, typeFormat.TYPE_I16),				//FORMAT_R16_SINT,
			new Glformat(internalFormat.INTERNAL_R16F, externalFormat.EXTERNAL_RED, typeFormat.TYPE_F16),						//FORMAT_R16_SFLOAT,

			new Glformat(internalFormat.INTERNAL_RG16_UNORM, externalFormat.EXTERNAL_RG, typeFormat.TYPE_U16),					//FORMAT_RG16_UNORM,
			new Glformat(internalFormat.INTERNAL_RG16_SNORM, externalFormat.EXTERNAL_RG, typeFormat.TYPE_I16),					//FORMAT_RG16_SNORM,
			new Glformat(internalFormat.INTERNAL_RG16_UNORM, externalFormat.EXTERNAL_RG, typeFormat.TYPE_U16),					//FORMAT_RG16_USCALE,
			new Glformat(internalFormat.INTERNAL_RG16_SNORM, externalFormat.EXTERNAL_RG, typeFormat.TYPE_I16),					//FORMAT_RG16_SSCALE,
			new Glformat(internalFormat.INTERNAL_RG16U, externalFormat.EXTERNAL_RG_INTEGER, typeFormat.TYPE_U16),				//FORMAT_RG16_UINT,
			new Glformat(internalFormat.INTERNAL_RG16I, externalFormat.EXTERNAL_RG_INTEGER, typeFormat.TYPE_I16),				//FORMAT_RG16_SINT,
			new Glformat(internalFormat.INTERNAL_RG16F, externalFormat.EXTERNAL_RG, typeFormat.TYPE_F16),						//FORMAT_RG16_SFLOAT,

			new Glformat(internalFormat.INTERNAL_RGB16_UNORM, externalFormat.EXTERNAL_RGB, typeFormat.TYPE_U16),					//FORMAT_RGB16_UNORM,
			new Glformat(internalFormat.INTERNAL_RGB16_SNORM, externalFormat.EXTERNAL_RGB, typeFormat.TYPE_I16),					//FORMAT_RGB16_SNORM,
			new Glformat(internalFormat.INTERNAL_RGB16_UNORM, externalFormat.EXTERNAL_RGB, typeFormat.TYPE_U16),					//FORMAT_RGB16_USCALE,
			new Glformat(internalFormat.INTERNAL_RGB16_SNORM, externalFormat.EXTERNAL_RGB, typeFormat.TYPE_I16),					//FORMAT_RGB16_USCALE,
			new Glformat(internalFormat.INTERNAL_RGB16U, externalFormat.EXTERNAL_RGB_INTEGER, typeFormat.TYPE_U16),				//FORMAT_RGB16_UINT,
			new Glformat(internalFormat.INTERNAL_RGB16I, externalFormat.EXTERNAL_RGB_INTEGER, typeFormat.TYPE_I16),				//FORMAT_RGB16_SINT,
			new Glformat(internalFormat.INTERNAL_RGB16F, externalFormat.EXTERNAL_RGB, typeFormat.TYPE_F16),						//FORMAT_RGB16_SFLOAT,

			new Glformat(internalFormat.INTERNAL_RGBA16_UNORM, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_U16),				//FORMAT_RGBA16_UNORM,
			new Glformat(internalFormat.INTERNAL_RGBA16_SNORM, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_I16),				//FORMAT_RGBA16_SNORM,
			new Glformat(internalFormat.INTERNAL_RGBA16_UNORM, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_U16),				//FORMAT_RGBA16_USCALE,
			new Glformat(internalFormat.INTERNAL_RGBA16_SNORM, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_I16),				//FORMAT_RGBA16_SSCALE,
			new Glformat(internalFormat.INTERNAL_RGBA16U, externalFormat.EXTERNAL_RGBA_INTEGER, typeFormat.TYPE_U16),			//FORMAT_RGBA16_UINT,
			new Glformat(internalFormat.INTERNAL_RGBA16I, externalFormat.EXTERNAL_RGBA_INTEGER, typeFormat.TYPE_I16),			//FORMAT_RGBA16_SINT,
			new Glformat(internalFormat.INTERNAL_RGBA16F, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_F16),					//FORMAT_RGBA16_SFLOAT,

			new Glformat(internalFormat.INTERNAL_R32U, externalFormat.EXTERNAL_RED_INTEGER, typeFormat.TYPE_U32),				//FORMAT_R32_UINT,
			new Glformat(internalFormat.INTERNAL_R32I, externalFormat.EXTERNAL_RED_INTEGER, typeFormat.TYPE_I32),				//FORMAT_R32_SINT,
			new Glformat(internalFormat.INTERNAL_R32F, externalFormat.EXTERNAL_RED, typeFormat.TYPE_F32),						//FORMAT_R32_SFLOAT,

			new Glformat(internalFormat.INTERNAL_RG32U, externalFormat.EXTERNAL_RG_INTEGER, typeFormat.TYPE_U32),				//FORMAT_RG32_UINT,
			new Glformat(internalFormat.INTERNAL_RG32I, externalFormat.EXTERNAL_RG_INTEGER, typeFormat.TYPE_I32),				//FORMAT_RG32_SINT,
			new Glformat(internalFormat.INTERNAL_RG32F, externalFormat.EXTERNAL_RG, typeFormat.TYPE_F32),						//FORMAT_RG32_SFLOAT,

			new Glformat(internalFormat.INTERNAL_RGB32U, externalFormat.EXTERNAL_RGB_INTEGER, typeFormat.TYPE_U32),				//FORMAT_RGB32_UINT,
			new Glformat(internalFormat.INTERNAL_RGB32I, externalFormat.EXTERNAL_RGB_INTEGER, typeFormat.TYPE_I32),				//FORMAT_RGB32_SINT,
			new Glformat(internalFormat.INTERNAL_RGB32F, externalFormat.EXTERNAL_RGB, typeFormat.TYPE_F32),						//FORMAT_RGB32_SFLOAT,

			new Glformat(internalFormat.INTERNAL_RGBA32U, externalFormat.EXTERNAL_RGBA_INTEGER, typeFormat.TYPE_U32),			//FORMAT_RGBA32_UINT,
			new Glformat(internalFormat.INTERNAL_RGBA32I, externalFormat.EXTERNAL_RGBA_INTEGER, typeFormat.TYPE_I32),			//FORMAT_RGBA32_SINT,
			new Glformat(internalFormat.INTERNAL_RGBA32F, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_F32),					//FORMAT_RGBA32_SFLOAT,

			new Glformat(internalFormat.INTERNAL_R64F_EXT, externalFormat.EXTERNAL_RED, typeFormat.TYPE_F64_EXT),				//FORMAT_R64_SFLOAT,
			new Glformat(internalFormat.INTERNAL_RG64F_EXT, externalFormat.EXTERNAL_RG, typeFormat.TYPE_F64_EXT),				//FORMAT_RG64_SFLOAT,
			new Glformat(internalFormat.INTERNAL_RGB64F_EXT, externalFormat.EXTERNAL_RGB, typeFormat.TYPE_F64_EXT),				//FORMAT_RGB64_SFLOAT,
			new Glformat(internalFormat.INTERNAL_RGBA64F_EXT, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_F64_EXT),			//FORMAT_RGBA64_SFLOAT,

			new Glformat(internalFormat.INTERNAL_RG11B10F, externalFormat.EXTERNAL_RGB, typeFormat.TYPE_UINT32_RG11B10F),		//FORMAT_RG11B10_UFLOAT,
			new Glformat(internalFormat.INTERNAL_RGB9E5, externalFormat.EXTERNAL_RGB, typeFormat.TYPE_UINT32_RGB9_E5),			//FORMAT_RGB9E5_UFLOAT,

			new Glformat(internalFormat.INTERNAL_D16, externalFormat.EXTERNAL_DEPTH, typeFormat.TYPE_NONE),								//FORMAT_D16_UNORM,
			new Glformat(internalFormat.INTERNAL_D24, externalFormat.EXTERNAL_DEPTH, typeFormat.TYPE_NONE),								//FORMAT_D24_UNORM,
			new Glformat(internalFormat.INTERNAL_D32F, externalFormat.EXTERNAL_DEPTH, typeFormat.TYPE_NONE),								//FORMAT_D32_UFLOAT,
			new Glformat(internalFormat.INTERNAL_S8_EXT, externalFormat.EXTERNAL_STENCIL, typeFormat.TYPE_NONE),							//FORMAT_S8_UNORM,
			new Glformat(internalFormat.INTERNAL_D16S8_EXT, externalFormat.EXTERNAL_DEPTH, typeFormat.TYPE_NONE),						//FORMAT_D16_UNORM_S8_UINT,
			new Glformat(internalFormat.INTERNAL_D24S8, externalFormat.EXTERNAL_DEPTH_STENCIL, typeFormat.TYPE_NONE),					//FORMAT_D24_UNORM_S8_UINT,
			new Glformat(internalFormat.INTERNAL_D32FS8X24, externalFormat.EXTERNAL_DEPTH_STENCIL, typeFormat.TYPE_NONE),				//FORMAT_D32_SFLOAT_S8_UINT,

			new Glformat(internalFormat.INTERNAL_RGB_DXT1, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),							//FORMAT_RGB_DXT1_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB_DXT1, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),							//FORMAT_RGB_DXT1_SRGB,
			new Glformat(internalFormat.INTERNAL_RGBA_DXT1, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),							//FORMAT_RGBA_DXT1_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB_ALPHA_DXT1, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),					//FORMAT_RGBA_DXT1_SRGB,
			new Glformat(internalFormat.INTERNAL_RGBA_DXT3, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),							//FORMAT_RGBA_DXT3_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB_ALPHA_DXT3, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),					//FORMAT_RGBA_DXT3_SRGB,
			new Glformat(internalFormat.INTERNAL_RGBA_DXT5, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),							//FORMAT_RGBA_DXT5_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB_ALPHA_DXT5, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),					//FORMAT_RGBA_DXT5_SRGB,
			new Glformat(internalFormat.INTERNAL_R_ATI1N_UNORM, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),						//FORMAT_R_ATI1N_UNORM,
			new Glformat(internalFormat.INTERNAL_R_ATI1N_SNORM, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),						//FORMAT_R_ATI1N_SNORM,
			new Glformat(internalFormat.INTERNAL_RG_ATI2N_UNORM, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),					//FORMAT_RG_ATI2N_UNORM,
			new Glformat(internalFormat.INTERNAL_RG_ATI2N_SNORM, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),					//FORMAT_RG_ATI2N_SNORM,
			new Glformat(internalFormat.INTERNAL_RGB_BP_UNSIGNED_FLOAT, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),				//FORMAT_RGB_BP_UFLOAT,
			new Glformat(internalFormat.INTERNAL_RGB_BP_SIGNED_FLOAT, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),				//FORMAT_RGB_BP_SFLOAT,
			new Glformat(internalFormat.INTERNAL_RGB_BP_UNORM, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),						//FORMAT_RGB_BP_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB_BP_UNORM, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),						//FORMAT_RGB_BP_SRGB,

			new Glformat(internalFormat.INTERNAL_RGB_ETC2, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),							//FORMAT_RGB_ETC2_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB8_ETC2, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),						//FORMAT_RGB_ETC2_SRGB,
			new Glformat(internalFormat.INTERNAL_RGBA_PUNCHTHROUGH_ETC2, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),			//FORMAT_RGBA_ETC2_PUNCHTHROUGH_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB8_PUNCHTHROUGH_ALPHA1_ETC2, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),	//FORMAT_RGBA_ETC2_PUNCHTHROUGH_SRGB,
			new Glformat(internalFormat.INTERNAL_RGBA_ETC2, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),							//FORMAT_RGBA_ETC2_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB8_ALPHA8_ETC2_EAC, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),				//FORMAT_RGBA_ETC2_SRGB,
			new Glformat(internalFormat.INTERNAL_R11_EAC, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),							//FORMAT_R11_EAC_UNORM,
			new Glformat(internalFormat.INTERNAL_SIGNED_R11_EAC, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),					//FORMAT_R11_EAC_SNORM,
			new Glformat(internalFormat.INTERNAL_RG11_EAC, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),							//FORMAT_RG11_EAC_UNORM,
			new Glformat(internalFormat.INTERNAL_SIGNED_RG11_EAC, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),					//FORMAT_RG11_EAC_SNORM,

			new Glformat(internalFormat.INTERNAL_RGBA_ASTC_4x4, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),						//FORMAT_RGBA_ASTC4X4_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB8_ALPHA8_ASTC_4x4, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),				//FORMAT_RGBA_ASTC4X4_SRGB,
			new Glformat(internalFormat.INTERNAL_RGBA_ASTC_5x4, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),						//FORMAT_RGBA_ASTC5X4_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB8_ALPHA8_ASTC_5x4, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),				//FORMAT_RGBA_ASTC5X4_SRGB,
			new Glformat(internalFormat.INTERNAL_RGBA_ASTC_5x5, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),						//FORMAT_RGBA_ASTC5X5_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB8_ALPHA8_ASTC_5x5, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),				//FORMAT_RGBA_ASTC5X5_SRGB,
			new Glformat(internalFormat.INTERNAL_RGBA_ASTC_6x5, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),						//FORMAT_RGBA_ASTC6X5_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB8_ALPHA8_ASTC_6x5, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),				//FORMAT_RGBA_ASTC6X5_SRGB,
			new Glformat(internalFormat.INTERNAL_RGBA_ASTC_6x6, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),						//FORMAT_RGBA_ASTC6X6_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB8_ALPHA8_ASTC_6x6, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),				//FORMAT_RGBA_ASTC6X6_SRGB,
			new Glformat(internalFormat.INTERNAL_RGBA_ASTC_8x5, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),						//FORMAT_RGBA_ASTC8X5_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB8_ALPHA8_ASTC_8x5, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),				//FORMAT_RGBA_ASTC8X5_SRGB,
			new Glformat(internalFormat.INTERNAL_RGBA_ASTC_8x6, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),						//FORMAT_RGBA_ASTC8X6_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB8_ALPHA8_ASTC_8x6, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),				//FORMAT_RGBA_ASTC8X6_SRGB,
			new Glformat(internalFormat.INTERNAL_RGBA_ASTC_8x8, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),						//FORMAT_RGBA_ASTC8X8_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB8_ALPHA8_ASTC_8x8, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),				//FORMAT_RGBA_ASTC8X8_SRGB,
			new Glformat(internalFormat.INTERNAL_RGBA_ASTC_10x5, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),					//FORMAT_RGBA_ASTC10X5_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB8_ALPHA8_ASTC_10x5, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),			//FORMAT_RGBA_ASTC10X5_SRGB,
			new Glformat(internalFormat.INTERNAL_RGBA_ASTC_10x6, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),					//FORMAT_RGBA_ASTC10X6_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB8_ALPHA8_ASTC_10x6, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),			//FORMAT_RGBA_ASTC10X6_SRGB,
			new Glformat(internalFormat.INTERNAL_RGBA_ASTC_10x8, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),					//FORMAT_RGBA_ASTC10X8_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB8_ALPHA8_ASTC_10x8, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),			//FORMAT_RGBA_ASTC10X8_SRGB,
			new Glformat(internalFormat.INTERNAL_RGBA_ASTC_10x10, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),					//FORMAT_RGBA_ASTC10X10_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB8_ALPHA8_ASTC_10x10, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),			//FORMAT_RGBA_ASTC10X10_SRGB,
			new Glformat(internalFormat.INTERNAL_RGBA_ASTC_12x10, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),					//FORMAT_RGBA_ASTC12X10_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB8_ALPHA8_ASTC_12x10, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),			//FORMAT_RGBA_ASTC12X10_SRGB,
			new Glformat(internalFormat.INTERNAL_RGBA_ASTC_12x12, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),					//FORMAT_RGBA_ASTC12X12_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB8_ALPHA8_ASTC_12x12, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),			//FORMAT_RGBA_ASTC12X12_SRGB,

			new Glformat(internalFormat.INTERNAL_RGBA4, externalFormat.EXTERNAL_BGRA, typeFormat.TYPE_UINT16_RGBA4),						//FORMAT_BGRA4_UNORM,
			new Glformat(internalFormat.INTERNAL_RGBA4, externalFormat.EXTERNAL_BGRA, typeFormat.TYPE_UINT16_RGBA4),						//FORMAT_BGRA4_USCALED,
			new Glformat(internalFormat.INTERNAL_R5G6B5, externalFormat.EXTERNAL_BGR, typeFormat.TYPE_UINT16_R5G6B5),					//FORMAT_B5G6R5_UNORM,
			new Glformat(internalFormat.INTERNAL_R5G6B5, externalFormat.EXTERNAL_BGR, typeFormat.TYPE_UINT16_R5G6B5),					//FORMAT_B5G6R5_USCALED,
			new Glformat(internalFormat.INTERNAL_RGB5A1, externalFormat.EXTERNAL_BGRA, typeFormat.TYPE_UINT16_RGB5A1),					//FORMAT_BGR5A1_UNORM,
			new Glformat(internalFormat.INTERNAL_RGB5A1, externalFormat.EXTERNAL_BGRA, typeFormat.TYPE_UINT16_RGB5A1),					//FORMAT_BGRA1_USCALED,

			new Glformat(internalFormat.INTERNAL_RGB8_UNORM, externalFormat.EXTERNAL_BGR, typeFormat.TYPE_U8),							//FORMAT_BGR8_UNORM,
			new Glformat(internalFormat.INTERNAL_RGB8_SNORM, externalFormat.EXTERNAL_BGR, typeFormat.TYPE_I8),							//FORMAT_BGR8_SNORM,
			new Glformat(internalFormat.INTERNAL_RGB8_UNORM, externalFormat.EXTERNAL_BGR, typeFormat.TYPE_U8),							//FORMAT_BGR8_USCALED,
			new Glformat(internalFormat.INTERNAL_RGB8_SNORM, externalFormat.EXTERNAL_BGR, typeFormat.TYPE_I8),							//FORMAT_BGR8_SSCALED,
			new Glformat(internalFormat.INTERNAL_RGB8U, externalFormat.EXTERNAL_BGR_INTEGER, typeFormat.TYPE_U8),						//FORMAT_BGR8_UINT,
			new Glformat(internalFormat.INTERNAL_RGB8I, externalFormat.EXTERNAL_BGR_INTEGER, typeFormat.TYPE_I8),						//FORMAT_BGR8_SINT,
			new Glformat(internalFormat.INTERNAL_SRGB8, externalFormat.EXTERNAL_BGR, typeFormat.TYPE_U8),								//FORMAT_BGR8_SRGB,

			new Glformat(internalFormat.INTERNAL_RGBA8_UNORM, externalFormat.EXTERNAL_BGRA, typeFormat.TYPE_U8),							//FORMAT_BGRA8_UNORM,
			new Glformat(internalFormat.INTERNAL_RGBA8_SNORM, externalFormat.EXTERNAL_BGRA, typeFormat.TYPE_I8),							//FORMAT_BGRA8_SNORM,
			new Glformat(internalFormat.INTERNAL_RGBA8_UNORM, externalFormat.EXTERNAL_BGRA, typeFormat.TYPE_U8),							//FORMAT_BGRA8_USCALED,
			new Glformat(internalFormat.INTERNAL_RGBA8_SNORM, externalFormat.EXTERNAL_BGRA, typeFormat.TYPE_I8),							//FORMAT_BGRA8_SSCALED,
			new Glformat(internalFormat.INTERNAL_RGBA8U, externalFormat.EXTERNAL_BGRA_INTEGER, typeFormat.TYPE_U8),						//FORMAT_BGRA8_UINT,
			new Glformat(internalFormat.INTERNAL_RGBA8I, externalFormat.EXTERNAL_BGRA_INTEGER, typeFormat.TYPE_I8),						//FORMAT_BGRA8_SINT,
			new Glformat(internalFormat.INTERNAL_SRGB8_ALPHA8, externalFormat.EXTERNAL_BGRA, typeFormat.TYPE_U8),						//FORMAT_BGRA8_SRGB,

			new Glformat(internalFormat.INTERNAL_RGB10A2_UNORM, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_UINT32_RGB10A2),			//FORMAT_BGR10A2_UNORM,
			new Glformat(internalFormat.INTERNAL_RGB10A2_SNORM_EXT, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_UINT32_RGB10A2),		//FORMAT_BGR10A2_SNORM,
			new Glformat(internalFormat.INTERNAL_RGB10A2_UNORM, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_UINT32_RGB10A2),			//FORMAT_BGR10A2_USCALE,
			new Glformat(internalFormat.INTERNAL_RGB10A2_SNORM_EXT, externalFormat.EXTERNAL_RGBA, typeFormat.TYPE_UINT32_RGB10A2),		//FORMAT_BGR10A2_SSCALE,
			new Glformat(internalFormat.INTERNAL_RGB10A2U, externalFormat.EXTERNAL_RGBA_INTEGER, typeFormat.TYPE_UINT32_RGB10A2),		//FORMAT_BGR10A2_UINT,
			new Glformat(internalFormat.INTERNAL_RGB10A2I_EXT, externalFormat.EXTERNAL_RGBA_INTEGER, typeFormat.TYPE_UINT32_RGB10A2),	//FORMAT_BGR10A2_SINT,

			new Glformat(internalFormat.INTERNAL_RG3B2, externalFormat.EXTERNAL_RGB, typeFormat.TYPE_UINT8_RG3B2),						//FORMAT_RG3B2_UNORM,
			new Glformat(internalFormat.INTERNAL_RGB8_UNORM, externalFormat.EXTERNAL_BGRA, typeFormat.TYPE_U8),							//FORMAT_BGRX8_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB8, externalFormat.EXTERNAL_BGRA, typeFormat.TYPE_U8),								//FORMAT_BGRX8_SRGB,

			new Glformat(internalFormat.INTERNAL_R8_UNORM, externalFormat.EXTERNAL_RED, typeFormat.TYPE_U8),								//FORMAT_L8_UNORM,
			new Glformat(internalFormat.INTERNAL_R8_UNORM, externalFormat.EXTERNAL_RED, typeFormat.TYPE_U8),								//FORMAT_A8_UNORM,
			new Glformat(internalFormat.INTERNAL_RG8_UNORM, externalFormat.EXTERNAL_RG, typeFormat.TYPE_U8),								//FORMAT_LA8_UNORM,
			new Glformat(internalFormat.INTERNAL_R16_UNORM, externalFormat.EXTERNAL_RED, typeFormat.TYPE_U16),							//FORMAT_L16_UNORM,
			new Glformat(internalFormat.INTERNAL_R16_UNORM, externalFormat.EXTERNAL_RED, typeFormat.TYPE_U16),							//FORMAT_A16_UNORM,
			new Glformat(internalFormat.INTERNAL_RG16_UNORM, externalFormat.EXTERNAL_RG, typeFormat.TYPE_U16),							//FORMAT_LA16_UNORM,

			new Glformat(internalFormat.INTERNAL_RGB_PVRTC_4BPPV1, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),					//FORMAT_RGB_PVRTC1_8X8_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB_PVRTC_2BPPV1, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),					//FORMAT_RGB_PVRTC1_8X8_SRGB,
			new Glformat(internalFormat.INTERNAL_RGB_PVRTC_2BPPV1, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),					//FORMAT_RGB_PVRTC1_16X8_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB_PVRTC_4BPPV1, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),					//FORMAT_RGB_PVRTC1_16X8_SRGB,
			new Glformat(internalFormat.INTERNAL_RGBA_PVRTC_4BPPV1, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),					//FORMAT_RGBA_PVRTC1_8X8_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB_ALPHA_PVRTC_2BPPV1, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),			//FORMAT_RGBA_PVRTC1_8X8_SRGB,
			new Glformat(internalFormat.INTERNAL_RGBA_PVRTC_2BPPV1, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),					//FORMAT_RGBA_PVRTC1_16X8_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB_ALPHA_PVRTC_4BPPV1, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),			//FORMAT_RGBA_PVRTC1_16X8_SRGB,
			new Glformat(internalFormat.INTERNAL_RGBA_PVRTC_4BPPV2, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),					//FORMAT_RGBA_PVRTC2_4X4_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB_ALPHA_PVRTC_4BPPV2, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),			//FORMAT_RGBA_PVRTC2_4X4_SRGB,
			new Glformat(internalFormat.INTERNAL_RGBA_PVRTC_2BPPV2, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),					//FORMAT_RGBA_PVRTC2_8X4_UNORM,
			new Glformat(internalFormat.INTERNAL_SRGB_ALPHA_PVRTC_2BPPV2, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),			//FORMAT_RGBA_PVRTC2_8X4_SRGB,

			new Glformat(internalFormat.INTERNAL_RGB_ETC, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),							//FORMAT_RGB_ETC_UNORM,
			new Glformat(internalFormat.INTERNAL_ATC_RGB, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),							//FORMAT_RGB_ATC_UNORM,
			new Glformat(internalFormat.INTERNAL_ATC_RGBA_EXPLICIT_ALPHA, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),			//FORMAT_RGBA_ATC_EXPLICIT_UNORM,
			new Glformat(internalFormat.INTERNAL_ATC_RGBA_INTERPOLATED_ALPHA, externalFormat.EXTERNAL_NONE, typeFormat.TYPE_NONE),		//FORMAT_RGBA_ATC_INTERPOLATED_UNORM,
		};

        public static PixelInternalFormat GetPixelInternalFormat(format format)
        {
            return (PixelInternalFormat)ConversionTable[(int)format - (int)format.FORMAT_FIRST].Internal;
        }
    }
}
