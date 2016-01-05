using System;
using System.IO;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace Sxta.Render.OpenGLExt
{
    public class TextureDDS : Texture
    {
        ImageDDS imgdds;

        public override int Height
        {
            get
            {
               return imgdds.Width;
            }
        }

        public override int Width
        {
            get
            {
                return imgdds.Height;
            }
        }

        internal void init(ImageDDS img, Parameters params_)
        {
            this.imgdds = img;

            this.textureTarget = img.TextureTarget;
            base.init((TextureInternalFormat)imgdds.InternalFormat, params_);

            // foreach face in the cubemap, get all it's mipmaps levels. Only one iteration for Texture2D
            for (int Slices = 0; Slices < imgdds.Depth; Slices++)
            {
                int trueMipMapCount = imgdds.NumberOfMipmapLevels - 1; // TODO: triplecheck correctness
                int Width = imgdds.Width;
                int Height = imgdds.Height;
                for (int Level = 0; Level < imgdds.NumberOfMipmapLevels; Level++) // start at base image
                {
                    if (Width > 2 && Height > 2)
                    { // Note: there could be a potential problem with non-power-of-two cube maps

#region Create TexImage
                        switch (imgdds.TextureTarget)
                        {
                            case TextureTarget.Texture2D:
                                if (img.IsCompressed)
                                {
                                    GL.CompressedTexImage2D(TextureTarget.Texture2D,
                                                         Level,
                                                         imgdds.InternalFormat,
                                                         Width,
                                                         Height,
                                                         0, //Compressed formats must have a border of 0, so this is constant.
                                                         imgdds[Level, Slices].Length,
                                                         imgdds[Level, Slices]);
                                }
                                else
                                {
                                    //GL.TexImage2D((TextureTarget.Texture2D, (int)Level,
                                    //                image.InternalFormat, Width, Height, 0, image.InternalFormat, imgktx.PixelType, img[Level, Slices]);
                                }
                                break;
                            case TextureTarget.TextureCubeMap:
                                GL.CompressedTexImage2D(TextureTarget.TextureCubeMapPositiveX + Slices,
                                                         Level,
                                                         imgdds.InternalFormat,
                                                         Width,
                                                         Height,
                                                         0, //Compressed formats must have a border of 0, so this is constant.
                                                         imgdds[Level, Slices].Length,
                                                         imgdds[Level, Slices]);
                                break;
                            case TextureTarget.Texture1D: // Untested
                            case TextureTarget.Texture3D: // Untested
                            default:
                                throw new ArgumentException("ERROR: Use DXT for 2D Images only. Cannot evaluate " + imgdds.TextureTarget);
                        }
                        GL.Finish();
#endregion Create TexImage

#region Query Success
                        //int width, height, internalformat, compressed;
                        //switch (image.TextureTarget)
                        //{
                        //    case TextureTarget.Texture1D:
                        //    case TextureTarget.Texture2D:
                        //    case TextureTarget.Texture3D:
                        //        GL.GetTexLevelParameter(image.TextureTarget, Level, GetTextureParameter.TextureWidth, out width);
                        //        GL.GetTexLevelParameter(image.TextureTarget, Level, GetTextureParameter.TextureHeight, out height);
                        //        GL.GetTexLevelParameter(image.TextureTarget, Level, GetTextureParameter.TextureInternalFormat, out internalformat);
                        //        GL.GetTexLevelParameter(image.TextureTarget, Level, GetTextureParameter.TextureCompressed, out compressed);
                        //        break;
                        //    case TextureTarget.TextureCubeMap:
                        //        GL.GetTexLevelParameter(TextureTarget.TextureCubeMapPositiveX + Slices, Level, GetTextureParameter.TextureWidth, out width);
                        //        GL.GetTexLevelParameter(TextureTarget.TextureCubeMapPositiveX + Slices, Level, GetTextureParameter.TextureHeight, out height);
                        //        GL.GetTexLevelParameter(TextureTarget.TextureCubeMapPositiveX + Slices, Level, GetTextureParameter.TextureInternalFormat, out internalformat);
                        //        GL.GetTexLevelParameter(TextureTarget.TextureCubeMapPositiveX + Slices, Level, GetTextureParameter.TextureCompressed, out compressed);
                        //        break;
                        //    default:
                        //        throw new NotImplementedException();
                        //}
                        Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
#endregion Query Success
                    }
#region Prepare the next MipMap level
                    Width /= 2;
                    if (Width < 1)
                        Width = 1;
                    Height /= 2;
                    if (Height < 1)
                        Height = 1;
#endregion Prepare the next MipMap level
                }
            }
        }
    }


    public class ImageDDS
    {

#region Header and Enums
        [Flags] // Surface Description
        private enum eDDSD : uint
        {
            CAPS = 0x00000001, // is always present
            HEIGHT = 0x00000002, // is always present
            WIDTH = 0x00000004, // is always present
            PITCH = 0x00000008, // is set if the image is uncompressed
            PIXELFORMAT = 0x00001000, // is always present
            MIPMAPCOUNT = 0x00020000, // is set if the image contains MipMaps
            LINEARSIZE = 0x00080000, // is set if the image is compressed
            DEPTH = 0x00800000 // is set for 3D Volume Textures
        }

        [Flags] // Pixelformat 
        private enum eDDPF : uint
        {
            NONE = 0x00000000, // not part of DX, added for convenience
            ALPHAPIXELS = 0x00000001,
            FOURCC = 0x00000004,
            RGB = 0x00000040,
            RGBA = 0x00000041
        }

        /// <summary>This list was derived from nVidia OpenGL SDK</summary>
        [Flags] // Texture types
        private enum eFOURCC : uint
        {
            UNKNOWN = 0,
            R8G8B8 = 20,
            A8R8G8B8 = 21,
            X8R8G8B8 = 22,
            R5G6B5 = 23,
            X1R5G5B5 = 24,
            A1R5G5B5 = 25,
            A4R4G4B4 = 26,
            R3G3B2 = 27,
            A8 = 28,
            A8R3G3B2 = 29,
            X4R4G4B4 = 30,
            A2B10G10R10 = 31,
            A8B8G8R8 = 32,
            X8B8G8R8 = 33,
            G16R16 = 34,
            A2R10G10B10 = 35,
            A16B16G16R16 = 36,

            L8 = 50,
            A8L8 = 51,
            A4L4 = 52,

            D16_LOCKABLE = 70,
            D32 = 71,
            D24X8 = 77,
            D16 = 80,

            D32F_LOCKABLE = 82,
            L16 = 81,

            // s10e5 formats (16-bits per channel)
            R16F = 111,
            G16R16F = 112,
            A16B16G16R16F = 113,

            // IEEE s23e8 formats (32-bits per channel)
            R32F = 114,
            G32R32F = 115,
            A32B32G32R32F = 116,
            DXT1 = 0x31545844,
            DXT2 = 0x32545844,
            DXT3 = 0x33545844,
            DXT4 = 0x34545844,
            DXT5 = 0x35545844,

            FOURCC_ATI1 = 0x31495441,
            FOURCC_ATI2 = 0x32495441,
            FOURCC_DX10 = 0x30315844

        }

        [Flags] // dwCaps1
        private enum eDDSCAPS : uint
        {
            NONE = 0x00000000, // not part of DX, added for convenience
            COMPLEX = 0x00000008, // should be set for any DDS file with more than one main surface
            TEXTURE = 0x00001000, // should always be set
            MIPMAP = 0x00400000 // only for files with MipMaps
        }

        [Flags]  // dwCaps2
        private enum ddsCubemapflag : uint
        {
            NONE = 0x00000000, // not part of DX, added for convenience
            DDSCAPS2_CUBEMAP = 0x00000200,
            DDSCAPS2_CUBEMAP_POSITIVEX = 0x00000400,
            DDSCAPS2_CUBEMAP_NEGATIVEX = 0x00000800,
            DDSCAPS2_CUBEMAP_POSITIVEY = 0x00001000,
            DDSCAPS2_CUBEMAP_NEGATIVEY = 0x00002000,
            DDSCAPS2_CUBEMAP_POSITIVEZ = 0x00004000,
            DDSCAPS2_CUBEMAP_NEGATIVEZ = 0x00008000,
            DDSCAPS2_VOLUME = 0x00200000 // for 3D Textures
        }
        private const ddsCubemapflag DDSCAPS2_CUBEMAP_ALLFACES = ddsCubemapflag.DDSCAPS2_CUBEMAP_POSITIVEX |
                                                                ddsCubemapflag.DDSCAPS2_CUBEMAP_NEGATIVEX |
                                                                ddsCubemapflag.DDSCAPS2_CUBEMAP_POSITIVEY |
                                                                ddsCubemapflag.DDSCAPS2_CUBEMAP_NEGATIVEY |
                                                                ddsCubemapflag.DDSCAPS2_CUBEMAP_POSITIVEZ |
                                                                ddsCubemapflag.DDSCAPS2_CUBEMAP_NEGATIVEZ;


        [Flags] // Surface Description
        private enum ddsFlag : uint
        {
            DDSD_CAPS = 0x00000001, // is always present
            DDSD_HEIGHT = 0x00000002, // is always present
            DDSD_WIDTH = 0x00000004, // is always present
            DDSD_PITCH = 0x00000008, // is set if the image is uncompressed
            DDSD_PIXELFORMAT = 0x00001000, // is always present
            DDSD_MIPMAPCOUNT = 0x00020000, // is set if the image contains MipMaps
            DDSD_LINEARSIZE = 0x00080000, // is set if the image is compressed
            DDSD_DEPTH = 0x00800000 // is set for 3D Volume Textures
        }

        [Flags] // dwCaps1
        private enum ddsSurfaceflag : uint
        {
            NONE = 0x00000000, // not part of DX, added for convenience
            DDSCAPS_COMPLEX = 0x00000008, // should be set for any DDS file with more than one main surface
            DDSCAPS_TEXTURE = 0x00001000, // should always be set
            DDSCAPS_MIPMAP = 0x00400000 // only for files with MipMaps
        }

        [Flags] // Pixelformat 
        private enum DDPF : uint
        {
            NONE = 0x00000000, // not part of DX, added for convenience
            DDPF_ALPHAPIXELS = 0x1,
            DDPF_ALPHA = 0x2,
            DDPF_FOURCC = 0x4,
            DDPF_RGB = 0x40,
            DDPF_YUV = 0x200,
            DDPF_LUMINANCE = 0x20000,
            DDPF_LUMINANCE_ALPHA = DDPF_LUMINANCE | DDPF_ALPHA,
            DDPF_FOURCC_ALPHAPIXELS = DDPF_FOURCC | DDPF_ALPHAPIXELS,
            DDPF_RGBAPIXELS = DDPF_RGB | DDPF_ALPHAPIXELS,
            DDPF_RGBA = DDPF_RGB | DDPF_ALPHA,
            DDPF_LUMINANCE_ALPHAPIXELS = DDPF_LUMINANCE | DDPF_ALPHAPIXELS,

        }

        [Flags] // Texture types
        private enum D3DFORMAT : uint
        {
            D3DFMT_UNKNOWN = 0,

            D3DFMT_R8G8B8 = 20,
            D3DFMT_A8R8G8B8 = 21,
            D3DFMT_X8R8G8B8 = 22,
            D3DFMT_R5G6B5 = 23,
            D3DFMT_X1R5G5B5 = 24,
            D3DFMT_A1R5G5B5 = 25,
            D3DFMT_A4R4G4B4 = 26,
            D3DFMT_R3G3B2 = 27,
            D3DFMT_A8 = 28,
            D3DFMT_A8R3G3B2 = 29,
            D3DFMT_X4R4G4B4 = 30,
            D3DFMT_A2B10G10R10 = 31,
            D3DFMT_A8B8G8R8 = 32,
            D3DFMT_X8B8G8R8 = 33,
            D3DFMT_G16R16 = 34,
            D3DFMT_A2R10G10B10 = 35,
            D3DFMT_A16B16G16R16 = 36,

            D3DFMT_A8P8 = 40,
            D3DFMT_P8 = 41,

            D3DFMT_L8 = 50,
            D3DFMT_A8L8 = 51,
            D3DFMT_A4L4 = 52,

            D3DFMT_V8U8 = 60,
            D3DFMT_L6V5U5 = 61,
            D3DFMT_X8L8V8U8 = 62,
            D3DFMT_Q8W8V8U8 = 63,
            D3DFMT_V16U16 = 64,
            D3DFMT_A2W10V10U10 = 67,

            D3DFMT_UYVY = 0x59565955,
            D3DFMT_R8G8_B8G8 = 0x47424752,
            D3DFMT_YUY2 = 0x32595559,
            D3DFMT_G8R8_G8B8 = 0x42475247,
            D3DFMT_DXT1 = 0x31545844,
            D3DFMT_DXT2 = 0x32545844,
            D3DFMT_DXT3 = 0x33545844,
            D3DFMT_DXT4 = 0x34545844,
            D3DFMT_DXT5 = 0x35545844,

            D3DFMT_ATI1 = 0x31495441,
            D3DFMT_AT1N = 0x4E315441, // GLI_MAKEFOURCC('A', 'T', '1', 'N'),
            D3DFMT_ATI2 = 0x32495441, // GLI_MAKEFOURCC('A', 'T', 'I', '2'),
            D3DFMT_AT2N = 0x4E325441, // GLI_MAKEFOURCC('A', 'T', '2', 'N'),

            D3DFMT_ETC = 541283397, //  GLI_MAKEFOURCC('E', 'T', 'C', ' '),
            D3DFMT_ETC1 = 826496069, // GLI_MAKEFOURCC('E', 'T', 'C', '1'),
            D3DFMT_ATC = 541283393, // GLI_MAKEFOURCC('A', 'T', 'C', ' '),
            D3DFMT_ATCA = 1094931521, // GLI_MAKEFOURCC('A', 'T', 'C', 'A'),
            D3DFMT_ATCI = 1229149249, // GLI_MAKEFOURCC('A', 'T', 'C', 'I'),

            D3DFMT_POWERVR_2BPP = 843273296, // GLI_MAKEFOURCC('P', 'T', 'C', '2'),
            D3DFMT_POWERVR_4BPP = 876827728, // GLI_MAKEFOURCC('P', 'T', 'C', '4'),

            D3DFMT_D16_LOCKABLE = 70,
            D3DFMT_D32 = 71,
            D3DFMT_D15S1 = 73,
            D3DFMT_D24S8 = 75,
            D3DFMT_D24X8 = 77,
            D3DFMT_D24X4S4 = 79,
            D3DFMT_D16 = 80,

            D3DFMT_D32F_LOCKABLE = 82,
            D3DFMT_D24FS8 = 83,

            D3DFMT_L16 = 81,

            D3DFMT_VERTEXDATA = 100,
            D3DFMT_INDEX16 = 101,
            D3DFMT_INDEX32 = 102,

            D3DFMT_Q16W16V16U16 = 110,

            D3DFMT_MULTI2_ARGB8 = 827606349, //GLI_MAKEFOURCC('M', 'E', 'T', '1'),

            D3DFMT_R16F = 111,
            D3DFMT_G16R16F = 112,
            D3DFMT_A16B16G16R16F = 113,

            D3DFMT_R32F = 114,
            D3DFMT_G32R32F = 115,
            D3DFMT_A32B32G32R32F = 116,

            D3DFMT_CxV8U8 = 117,

            D3DFMT_DX10 = 0x30315844,

            D3DFMT_GLI1 = 0x31494C47, //GLI_MAKEFOURCC('G', 'L', 'I', '1'),

            D3DFMT_FORCE_DWORD = 0x7fffffff
        }

        private struct ddsPixelFormat
        {
            public UInt32 size; // Size of Pixelformat structure. This member must be set to 32.
            public DDPF flags; // Flags to indicate valid fields.
            public D3DFORMAT fourCC; // This is the four-character code for compressed formats.
            public UInt32 bpp;
            public UInt32 pfRBitMask; // For RGB formats, these three fields contain the masks for the red, green, and blue channels. For A8R8G8B8, these values would be 0x00ff0000, 0x0000ff00, and 0x000000ff respectively.
            public UInt32 pfGBitMask; // ..
            public UInt32 pfBBitMask; // ..
            public UInt32 pfABitMask; // For RGB formats, this contains the mask for the alpha channel, if any. dwFlags should include DDpf_ALPHAPIXELS in this case. For A8R8G8B8, this value would be 0xff000000.

            public bool MaskAreEquals(UInt32 r, UInt32 g, UInt32 b, UInt32 a)
            {
                return this.pfRBitMask == r && this.pfGBitMask == g && this.pfBBitMask == b && this.pfABitMask == a;
            }
            public bool MaskAreEquals(GXformat f)
            {
                return this.pfRBitMask == f.Mask_r && this.pfGBitMask == f.Mask_g && this.pfBBitMask == f.Mask_b && this.pfABitMask == f.Mask_a;
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct ddsHeader
        {
            public byte identifier0;
            public byte identifier1;
            public byte identifier2;
            public byte identifier3;
            public UInt32 Size; // Size of structure is 124 bytes, 128 including all sub-structs and the header
            public ddsFlag Flags; // Flags to indicate valid fields.
            public UInt32 Height; // Height of the main image in pixels
            public UInt32 Width; // Width of the main image in pixels
            public UInt32 Pitch; // For compressed formats, this is the total number of bytes for the main image.
            public UInt32 Depth; // For volume textures, this is the depth of the volume.
            public UInt32 MipMapLevels; // total number of levels in the mipmap chain of the main image.
            public UInt32 R1_1, R1_2, R1_3, R1_4, R1_5, R1_6, R1_7, R1_8, R1_9, R1_10, R1_11; // 11 Reserved1 UInt32s
            public ddsPixelFormat Format;
            public UInt32 SurfaceFlags;
            public ddsCubemapflag CubemapFlags;
            public UInt32 R2_1, R3_2, R3_3;// Reserved2 3 = 2 + 1 UInt32


            private const int DDS_HEADER_SIZE = 124 + 4;
            private static readonly char[] FOURCC_DDS = new char[] { 'D', 'D', 'S', ' ' };

            public static ddsHeader ReadHeader(Stream stream)
            {

                byte[] buff = new byte[DDS_HEADER_SIZE];
                stream.Read(buff, 0, DDS_HEADER_SIZE);
                ddsHeader header = ReadUsingPointer(buff);
                // cheack FOURCC_DDS
                if (header.identifier0 != (char)'D' || header.identifier1 != (char)'D' || header.identifier2 != (char)'S' || header.identifier3 != (char)' ')
                {
                    throw new Exception("Invalid file format");
                }
                return header;
            }

            public static unsafe void WriteHeader(Stream stream, ddsHeader header)
            {
                byte[] buff = new byte[DDS_HEADER_SIZE];
                fixed (byte* pbuff = buff)
                {
                    *(ddsHeader*)pbuff = *((ddsHeader*)&header);
                    stream.Write(buff, 0, DDS_HEADER_SIZE);
                }
            }

            /// <summary>
            /// This method reads unmanaged data into a header structure 
            /// This uses a technique described in 
            /// http://www.codeproject.com/Articles/25896/Reading-Unmanaged-Data-Into-Structures
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            private static unsafe ddsHeader ReadUsingPointer(byte[] data)
            {
                fixed (byte* header = &data[0])
                {
                    return *(ddsHeader*)header;
                }
            }
        }

        private enum D3D10_RESOURCE_DIMENSION : uint
        {
            D3D10_RESOURCE_DIMENSION_UNKNOWN = 0,
            D3D10_RESOURCE_DIMENSION_BUFFER = 1,
            D3D10_RESOURCE_DIMENSION_TEXTURE1D = 2,
            D3D10_RESOURCE_DIMENSION_TEXTURE2D = 3,
            D3D10_RESOURCE_DIMENSION_TEXTURE3D = 4
        }

        private enum D3D10_RESOURCE_MISC_FLAG : uint
        {
            D3D10_RESOURCE_MISC_GENERATE_MIPS = 0x01,
            D3D10_RESOURCE_MISC_SHARED = 0x02,
            D3D10_RESOURCE_MISC_TEXTURECUBE = 0x04,
            D3D10_RESOURCE_MISC_SHARED_KEYEDMUTEX = 0x10,
            D3D10_RESOURCE_MISC_GDI_COMPATIBLE = 0x20,
        }

        private enum DDS_ALPHA : uint
        {
            DDS_ALPHA_MODE_UNKNOWN = 0x0,
            DDS_ALPHA_MODE_STRAIGHT = 0x1,
            DDS_ALPHA_MODE_PREMULTIPLIED = 0x2,
            DDS_ALPHA_MODE_OPAQUE = 0x3,
            DDS_ALPHA_MODE_CUSTOM = 0x4
        }
        private enum dxgiFormat : uint
        {
            DXGI_FORMAT_UNKNOWN = 0,
            DXGI_FORMAT_R32G32B32A32_TYPELESS = 1,
            DXGI_FORMAT_R32G32B32A32_FLOAT = 2,
            DXGI_FORMAT_R32G32B32A32_UINT = 3,
            DXGI_FORMAT_R32G32B32A32_SINT = 4,
            DXGI_FORMAT_R32G32B32_TYPELESS = 5,
            DXGI_FORMAT_R32G32B32_FLOAT = 6,
            DXGI_FORMAT_R32G32B32_UINT = 7,
            DXGI_FORMAT_R32G32B32_SINT = 8,
            DXGI_FORMAT_R16G16B16A16_TYPELESS = 9,
            DXGI_FORMAT_R16G16B16A16_FLOAT = 10,
            DXGI_FORMAT_R16G16B16A16_UNORM = 11,
            DXGI_FORMAT_R16G16B16A16_UINT = 12,
            DXGI_FORMAT_R16G16B16A16_SNORM = 13,
            DXGI_FORMAT_R16G16B16A16_SINT = 14,
            DXGI_FORMAT_R32G32_TYPELESS = 15,
            DXGI_FORMAT_R32G32_FLOAT = 16,
            DXGI_FORMAT_R32G32_UINT = 17,
            DXGI_FORMAT_R32G32_SINT = 18,
            DXGI_FORMAT_R32G8X24_TYPELESS = 19,
            DXGI_FORMAT_D32_FLOAT_S8X24_UINT = 20,
            DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS = 21,
            DXGI_FORMAT_X32_TYPELESS_G8X24_UINT = 22,
            DXGI_FORMAT_R10G10B10A2_TYPELESS = 23,
            DXGI_FORMAT_R10G10B10A2_UNORM = 24,
            DXGI_FORMAT_R10G10B10A2_UINT = 25,
            DXGI_FORMAT_R11G11B10_FLOAT = 26,
            DXGI_FORMAT_R8G8B8A8_TYPELESS = 27,
            DXGI_FORMAT_R8G8B8A8_UNORM = 28,
            DXGI_FORMAT_R8G8B8A8_UNORM_SRGB = 29,
            DXGI_FORMAT_R8G8B8A8_UINT = 30,
            DXGI_FORMAT_R8G8B8A8_SNORM = 31,
            DXGI_FORMAT_R8G8B8A8_SINT = 32,
            DXGI_FORMAT_R16G16_TYPELESS = 33,
            DXGI_FORMAT_R16G16_FLOAT = 34,
            DXGI_FORMAT_R16G16_UNORM = 35,
            DXGI_FORMAT_R16G16_UINT = 36,
            DXGI_FORMAT_R16G16_SNORM = 37,
            DXGI_FORMAT_R16G16_SINT = 38,
            DXGI_FORMAT_R32_TYPELESS = 39,
            DXGI_FORMAT_D32_FLOAT = 40,
            DXGI_FORMAT_R32_FLOAT = 41,
            DXGI_FORMAT_R32_UINT = 42,
            DXGI_FORMAT_R32_SINT = 43,
            DXGI_FORMAT_R24G8_TYPELESS = 44,
            DXGI_FORMAT_D24_UNORM_S8_UINT = 45,
            DXGI_FORMAT_R24_UNORM_X8_TYPELESS = 46,
            DXGI_FORMAT_X24_TYPELESS_G8_UINT = 47,
            DXGI_FORMAT_R8G8_TYPELESS = 48,
            DXGI_FORMAT_R8G8_UNORM = 49,
            DXGI_FORMAT_R8G8_UINT = 50,
            DXGI_FORMAT_R8G8_SNORM = 51,
            DXGI_FORMAT_R8G8_SINT = 52,
            DXGI_FORMAT_R16_TYPELESS = 53,
            DXGI_FORMAT_R16_FLOAT = 54,
            DXGI_FORMAT_D16_UNORM = 55,
            DXGI_FORMAT_R16_UNORM = 56,
            DXGI_FORMAT_R16_UINT = 57,
            DXGI_FORMAT_R16_SNORM = 58,
            DXGI_FORMAT_R16_SINT = 59,
            DXGI_FORMAT_R8_TYPELESS = 60,
            DXGI_FORMAT_R8_UNORM = 61,
            DXGI_FORMAT_R8_UINT = 62,
            DXGI_FORMAT_R8_SNORM = 63,
            DXGI_FORMAT_R8_SINT = 64,
            DXGI_FORMAT_A8_UNORM = 65,
            DXGI_FORMAT_R1_UNORM = 66,
            DXGI_FORMAT_R9G9B9E5_SHAREDEXP = 67,
            DXGI_FORMAT_R8G8_B8G8_UNORM = 68,
            DXGI_FORMAT_G8R8_G8B8_UNORM = 69,
            DXGI_FORMAT_BC1_TYPELESS = 70,
            DXGI_FORMAT_BC1_UNORM = 71,
            DXGI_FORMAT_BC1_UNORM_SRGB = 72,
            DXGI_FORMAT_BC2_TYPELESS = 73,
            DXGI_FORMAT_BC2_UNORM = 74,
            DXGI_FORMAT_BC2_UNORM_SRGB = 75,
            DXGI_FORMAT_BC3_TYPELESS = 76,
            DXGI_FORMAT_BC3_UNORM = 77,
            DXGI_FORMAT_BC3_UNORM_SRGB = 78,
            DXGI_FORMAT_BC4_TYPELESS = 79,
            DXGI_FORMAT_BC4_UNORM = 80,
            DXGI_FORMAT_BC4_SNORM = 81,
            DXGI_FORMAT_BC5_TYPELESS = 82,
            DXGI_FORMAT_BC5_UNORM = 83,
            DXGI_FORMAT_BC5_SNORM = 84,
            DXGI_FORMAT_B5G6R5_UNORM = 85,
            DXGI_FORMAT_B5G5R5A1_UNORM = 86,
            DXGI_FORMAT_B8G8R8A8_UNORM = 87,
            DXGI_FORMAT_B8G8R8X8_UNORM = 88,
            DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM = 89,
            DXGI_FORMAT_B8G8R8A8_TYPELESS = 90,
            DXGI_FORMAT_B8G8R8A8_UNORM_SRGB = 91,
            DXGI_FORMAT_B8G8R8X8_TYPELESS = 92,
            DXGI_FORMAT_B8G8R8X8_UNORM_SRGB = 93,
            DXGI_FORMAT_BC6H_TYPELESS = 94,
            DXGI_FORMAT_BC6H_UF16 = 95,
            DXGI_FORMAT_BC6H_SF16 = 96,
            DXGI_FORMAT_BC7_TYPELESS = 97,
            DXGI_FORMAT_BC7_UNORM = 98,
            DXGI_FORMAT_BC7_UNORM_SRGB = 99,
            DXGI_FORMAT_AYUV = 100,
            DXGI_FORMAT_Y410 = 101,
            DXGI_FORMAT_Y416 = 102,
            DXGI_FORMAT_NV12 = 103,
            DXGI_FORMAT_P010 = 104,
            DXGI_FORMAT_P016 = 105,
            DXGI_FORMAT_420_OPAQUE = 106,
            DXGI_FORMAT_YUY2 = 107,
            DXGI_FORMAT_Y210 = 108,
            DXGI_FORMAT_Y216 = 109,
            DXGI_FORMAT_NV11 = 110,
            DXGI_FORMAT_AI44 = 111,
            DXGI_FORMAT_IA44 = 112,
            DXGI_FORMAT_P8 = 113,
            DXGI_FORMAT_A8P8 = 114,
            DXGI_FORMAT_B4G4R4A4_UNORM = 115,

            DXGI_FORMAT_P208 = 130,
            DXGI_FORMAT_V208 = 131,
            DXGI_FORMAT_V408 = 132,
            DXGI_FORMAT_ASTC_4X4_TYPELESS = 133,
            DXGI_FORMAT_ASTC_4X4_UNORM = 134,
            DXGI_FORMAT_ASTC_4X4_UNORM_SRGB = 135,
            DXGI_FORMAT_ASTC_5X4_TYPELESS = 137,
            DXGI_FORMAT_ASTC_5X4_UNORM = 138,
            DXGI_FORMAT_ASTC_5X4_UNORM_SRGB = 139,
            DXGI_FORMAT_ASTC_5X5_TYPELESS = 141,
            DXGI_FORMAT_ASTC_5X5_UNORM = 142,
            DXGI_FORMAT_ASTC_5X5_UNORM_SRGB = 143,
            DXGI_FORMAT_ASTC_6X5_TYPELESS = 145,
            DXGI_FORMAT_ASTC_6X5_UNORM = 146,
            DXGI_FORMAT_ASTC_6X5_UNORM_SRGB = 147,
            DXGI_FORMAT_ASTC_6X6_TYPELESS = 149,
            DXGI_FORMAT_ASTC_6X6_UNORM = 150,
            DXGI_FORMAT_ASTC_6X6_UNORM_SRGB = 151,
            DXGI_FORMAT_ASTC_8X5_TYPELESS = 153,
            DXGI_FORMAT_ASTC_8X5_UNORM = 154,
            DXGI_FORMAT_ASTC_8X5_UNORM_SRGB = 155,
            DXGI_FORMAT_ASTC_8X6_TYPELESS = 157,
            DXGI_FORMAT_ASTC_8X6_UNORM = 158,
            DXGI_FORMAT_ASTC_8X6_UNORM_SRGB = 159,
            DXGI_FORMAT_ASTC_8X8_TYPELESS = 161,
            DXGI_FORMAT_ASTC_8X8_UNORM = 162,
            DXGI_FORMAT_ASTC_8X8_UNORM_SRGB = 163,
            DXGI_FORMAT_ASTC_10X5_TYPELESS = 165,
            DXGI_FORMAT_ASTC_10X5_UNORM = 166,
            DXGI_FORMAT_ASTC_10X5_UNORM_SRGB = 167,
            DXGI_FORMAT_ASTC_10X6_TYPELESS = 169,
            DXGI_FORMAT_ASTC_10X6_UNORM = 170,
            DXGI_FORMAT_ASTC_10X6_UNORM_SRGB = 171,
            DXGI_FORMAT_ASTC_10X8_TYPELESS = 173,
            DXGI_FORMAT_ASTC_10X8_UNORM = 174,
            DXGI_FORMAT_ASTC_10X8_UNORM_SRGB = 175,
            DXGI_FORMAT_ASTC_10X10_TYPELESS = 177,
            DXGI_FORMAT_ASTC_10X10_UNORM = 178,
            DXGI_FORMAT_ASTC_10X10_UNORM_SRGB = 179,
            DXGI_FORMAT_ASTC_12X10_TYPELESS = 181,
            DXGI_FORMAT_ASTC_12X10_UNORM = 182,
            DXGI_FORMAT_ASTC_12X10_UNORM_SRGB = 183,
            DXGI_FORMAT_ASTC_12X12_TYPELESS = 185,
            DXGI_FORMAT_ASTC_12X12_UNORM = 186,
            DXGI_FORMAT_ASTC_12X12_UNORM_SRGB = 187,
            DXGI_FORMAT_R64_FLOAT_GLI,
            DXGI_FORMAT_R64G64_FLOAT_GLI,
            DXGI_FORMAT_R64G64B64_FLOAT_GLI,
            DXGI_FORMAT_R64G64B64A64_FLOAT_GLI,

            DXGI_FORMAT_LAST = DXGI_FORMAT_R64G64B64A64_FLOAT_GLI,
            DXGI_FORMAT_FORCE_UINT = 0xffffffffU
        }

        [StructLayout(LayoutKind.Sequential)]

        private struct ddsHeader10
        {
            public dxgiFormat Format;
            public D3D10_RESOURCE_DIMENSION ResourceDimension;
            public UInt32 MiscFlag; // D3D10_RESOURCE_MISC_GENERATE_MIPS
            public UInt32 ArraySize;
            public UInt32 Reserved;
            private const int DDS_HEADER10_SIZE = 20;

            public static ddsHeader10 ReadHeader(Stream stream)
            {

                byte[] buff = new byte[DDS_HEADER10_SIZE];
                stream.Read(buff, 0, DDS_HEADER10_SIZE);
                ddsHeader10 header = ReadUsingPointer(buff);
                return header;
            }

            public static unsafe void WriteHeader(Stream stream, ddsHeader10 header)
            {
                byte[] buff = new byte[DDS_HEADER10_SIZE];
                fixed (byte* pbuff = buff)
                {
                    *(ddsHeader10*)pbuff = *((ddsHeader10*)&header);
                    stream.Write(buff, 0, DDS_HEADER10_SIZE);
                }
            }

            /// <summary>
            /// This method reads unmanaged data into a header structure 
            /// This uses a technique described in 
            /// http://www.codeproject.com/Articles/25896/Reading-Unmanaged-Data-Into-Structures
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            private static unsafe ddsHeader10 ReadUsingPointer(byte[] data)
            {
                fixed (byte* header = &data[0])
                {
                    return *(ddsHeader10*)header;
                }
            }
        }

        private struct GXformat
        {
            public DDPF DDPixelFormat;
            public D3DFORMAT D3DFormat;
            public dxgiFormat DXGIFormat;
            public uint Mask_r;
            public uint Mask_g;
            public uint Mask_b;
            public uint Mask_a;

            public GXformat(DDPF pf, D3DFORMAT f, dxgiFormat df, uint m0, uint m1, uint m2, uint m3)
            {
                DDPixelFormat = pf;
                D3DFormat = f;
                DXGIFormat = df;
                Mask_r = m0;
                Mask_g = m1;
                Mask_b = m2;
                Mask_a = m3;
            }
        }
        

        static readonly GXformat[] ConversionTable = new GXformat[]
        {
            new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_RG4_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_RG4_USCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_A4R4G4B4, dxgiFormat.DXGI_FORMAT_B4G4R4A4_UNORM,  0x0F00, 0x00F0, 0x000F, 0xF000),					//FORMAT_RGBA4_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_A4R4G4B4, dxgiFormat.DXGI_FORMAT_B4G4R4A4_UNORM,  0x0F00, 0x00F0, 0x000F, 0xF000),					//FORMAT_RGBA4_USCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_B5G6R5_UNORM,  0xf800, 0x07e0, 0x001f, 0x0000),							//FORMAT_R5G6B5_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_B5G6R5_UNORM,  0xf800, 0x07e0, 0x001f, 0x0000),							//FORMAT_R5G6B5_USCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_B5G5R5A1_UNORM,  0x7c00, 0x03e0, 0x001f, 0x8000),						//FORMAT_RGB5A1_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_B5G5R5A1_UNORM,  0x7c00, 0x03e0, 0x001f, 0x8000),						//FORMAT_RGB5A1_USCALED,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R8_UNORM,  0x00FF0000, 0x00000000, 0x00000000, 0x00000000),				//FORMAT_R8_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R8_SNORM, 0,0,0,0),															//FORMAT_R8_SNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R8_UNORM,  0x00FF0000, 0x00000000, 0x00000000, 0x00000000),				//FORMAT_R8_USCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R8_SNORM, 0,0,0,0),															//FORMAT_R8_SSCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R8_UINT, 0,0,0,0),															//FORMAT_R8_UINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R8_SINT, 0,0,0,0),															//FORMAT_R8_SINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_R8_SRGB,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R8G8_UNORM,  0x00FF0000, 0x0000FF00, 0x00000000, 0x00000000),			//FORMAT_RG8_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R8G8_SNORM, 0,0,0,0),														//FORMAT_RG8_SNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R8G8_UNORM,  0x00FF0000, 0x0000FF00, 0x00000000, 0x00000000),			//FORMAT_RG8_USCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R8G8_SNORM, 0,0,0,0),														//FORMAT_RG8_SSCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R8G8_UINT, 0,0,0,0),															//FORMAT_RG8_UINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R8G8_SINT, 0,0,0,0),															//FORMAT_RG8_SINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_RG8_SRGB,

			new GXformat(DDPF.DDPF_RGB, D3DFORMAT.D3DFMT_R8G8B8, dxgiFormat.DXGI_FORMAT_UNKNOWN,  0x00FF0000, 0x0000FF00, 0x000000FF, 0x00000000),				//FORMAT_RGB8_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_RGB8_SNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_RGB8_USCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_RGB8_SSCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_RGB8_UINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_RGB8_SINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_B8G8R8X8_UNORM_SRGB, 0,0,0,0),												//FORMAT_RGB8_SRGB,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R8G8B8A8_UNORM,  0x000000FF, 0x0000FF00, 0x00FF0000, 0xFF000000),		//FORMAT_RGBA8_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R8G8B8A8_SNORM, 0,0,0,0),													//FORMAT_RGBA8_SNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_RGBA8_USCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_RGBA8_SSCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R8G8B8A8_UINT, 0,0,0,0),														//FORMAT_RGBA8_UINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R8G8B8A8_SINT, 0,0,0,0),														//FORMAT_RGBA8_SINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB, 0,0,0,0),												//FORMAT_RGBA8_SRGB,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R10G10B10A2_UNORM,  0x3FF00000, 0x000FFC00, 0x000003FF, 0xC0000000),	//FORMAT_RGB10A2_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN,  0x3FF00000, 0x000FFC00, 0x000003FF, 0xC0000000),				//FORMAT_RGB10A2_SNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN,  0x3FF00000, 0x000FFC00, 0x000003FF, 0xC0000000),				//FORMAT_RGB10A2_USCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN,  0x3FF00000, 0x000FFC00, 0x000003FF, 0xC0000000),				//FORMAT_RGB10A2_SSCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R10G10B10A2_UINT,  0x3FF00000, 0x000FFC00, 0x000003FF, 0xC0000000),		//FORMAT_RGB10A2_UINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN,  0x3FF00000, 0x000FFC00, 0x000003FF, 0xC0000000),				//FORMAT_RGB10A2_SINT,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R16_UNORM,  0x0000FFFF, 0x00000000, 0x00000000, 0x00000000),			//FORMAT_R16_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R16_SNORM,  0x0000FFFF, 0x00000000, 0x00000000, 0x00000000),			//FORMAT_R16_SNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN,  0x0000FFFF, 0x00000000, 0x00000000, 0x00000000),				//FORMAT_R16_USCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN,  0x0000FFFF, 0x00000000, 0x00000000, 0x00000000),				//FORMAT_R16_SSCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R16_UINT,  0x0000FFFF, 0x00000000, 0x00000000, 0x0000000),				//FORMAT_R16_UINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R16_SINT,  0x0000FFFF, 0x00000000, 0x00000000, 0x0000000),				//FORMAT_R16_SINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_R16F, dxgiFormat.DXGI_FORMAT_R16_FLOAT,  0x0000FFFF, 0x00000000, 0x00000000, 0x0000000),				//FORMAT_R16_SFLOAT,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_G16R16, dxgiFormat.DXGI_FORMAT_R16G16_UNORM,  0x0000FFFF, 0xFFFF0000, 0x00000000, 0x00000000),		//FORMAT_RG16_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R16G16_SNORM,  0x0000FFFF, 0xFFFF0000, 0x00000000, 0x00000000),			//FORMAT_RG16_SNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN,  0x0000FFFF, 0xFFFF0000, 0x00000000, 0x00000000),				//FORMAT_RG16_USCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN,  0x0000FFFF, 0xFFFF0000, 0x00000000, 0x00000000),				//FORMAT_RG16_SSCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R16G16_UINT,  0x0000FFFF, 0xFFFF0000, 0x00000000, 0x00000000),			//FORMAT_RG16_UINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R16G16_SINT,  0x0000FFFF, 0xFFFF0000, 0x00000000, 0x00000000),			//FORMAT_RG16_SINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_G16R16F, dxgiFormat.DXGI_FORMAT_R16G16_FLOAT,  0x0000FFFF, 0xFFFF0000, 0x00000000, 0x00000000),		//FORMAT_RG16_SFLOAT,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_RGB16_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_RGB16_SNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_RGB16_USCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_RGB16_SSCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_RGB16_UINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_RGB16_SINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_RGB16_SFLOAT,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_A16B16G16R16, dxgiFormat.DXGI_FORMAT_R16G16B16A16_UNORM, 0,0,0,0),										//FORMAT_RGBA16_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R16G16B16A16_SNORM, 0,0,0,0),												//FORMAT_RGBA16_SNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_RGBA16_USCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_RGBA16_SSCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R16G16B16A16_UINT, 0,0,0,0),													//FORMAT_RGBA16_UINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R16G16B16A16_SINT, 0,0,0,0),													//FORMAT_RGBA16_SINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_A16B16G16R16F, dxgiFormat.DXGI_FORMAT_R16G16B16A16_FLOAT, 0,0,0,0),										//FORMAT_RGBA16_SFLOAT,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R32_UINT, 0,0,0,0),															//FORMAT_R32_UINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R32_SINT, 0,0,0,0),															//FORMAT_R32_SINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_R32F, dxgiFormat.DXGI_FORMAT_R32_FLOAT,  0xFFFFFFFF, 0x0000000, 0x0000000, 0x0000000),				//FORMAT_R32_SFLOAT,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R32G32_UINT, 0,0,0,0),														//FORMAT_RG32_UINT
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R32G32_SINT, 0,0,0,0),														//FORMAT_RG32_SINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_G32R32F, dxgiFormat.DXGI_FORMAT_R32G32_FLOAT, 0,0,0,0),													//FORMAT_RG32_SFLOAT,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R32G32B32_UINT, 0,0,0,0),													//FORMAT_RGB32_UINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R32G32B32_SINT, 0,0,0,0),													//FORMAT_RGB32_SINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R32G32B32_FLOAT, 0,0,0,0),													//FORMAT_RGB32_SFLOAT,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R32G32B32A32_UINT, 0,0,0,0),													//FORMAT_RGBA32_UINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R32G32B32A32_SINT, 0,0,0,0),													//FORMAT_RGBA32_SINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_A32B32G32R32F, dxgiFormat.DXGI_FORMAT_R32G32B32A32_FLOAT, 0,0,0,0),										//FORMAT_RGBA32_SFLOAT,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_GLI1, dxgiFormat.DXGI_FORMAT_R64_FLOAT_GLI, 0,0,0,0),														//FORMAT_R64_SFLOAT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_GLI1, dxgiFormat.DXGI_FORMAT_R64G64_FLOAT_GLI, 0,0,0,0),													//FORMAT_RG64_SFLOAT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_GLI1, dxgiFormat.DXGI_FORMAT_R64G64B64_FLOAT_GLI, 0,0,0,0),												//FORMAT_RGB64_SFLOAT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_GLI1, dxgiFormat.DXGI_FORMAT_R64G64B64A64_FLOAT_GLI, 0,0,0,0),											//FORMAT_RGBA64_SFLOAT,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R11G11B10_FLOAT, 0,0,0,0),													//FORMAT_RG11B10_UFLOAT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R9G9B9E5_SHAREDEXP, 0,0,0,0),												//FORMAT_RGB9E5_UFLOAT,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_D16_UNORM, 0,0,0,0),						//FORMAT_D16_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),						//FORMAT_D24_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_D32_FLOAT, 0,0,0,0),						//FORMAT_D32_SFLOAT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),						//FORMAT_S8_UINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),						//FORMAT_D16_UNORM_S8_UINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_D24_UNORM_S8_UINT, 0,0,0,0),				//FORMAT_D24_UNORM_S8_UINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_D32_FLOAT_S8X24_UINT, 0,0,0,0),			//FORMAT_D32_SFLOAT_S8_UINT,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DXT1, dxgiFormat.DXGI_FORMAT_BC1_UNORM, 0,0,0,0),						//FORMAT_RGB_DXT1_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_BC1_UNORM_SRGB, 0,0,0,0),				//FORMAT_RGB_DXT1_SRGB,
			new GXformat(DDPF.DDPF_FOURCC_ALPHAPIXELS, D3DFORMAT.D3DFMT_DXT1, dxgiFormat.DXGI_FORMAT_BC1_UNORM, 0,0,0,0),			//FORMAT_RGBA_DXT1_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_BC1_UNORM_SRGB, 0,0,0,0),				//FORMAT_RGBA_DXT1_SRGB,
			new GXformat(DDPF.DDPF_FOURCC_ALPHAPIXELS, D3DFORMAT.D3DFMT_DXT3, dxgiFormat.DXGI_FORMAT_BC2_UNORM, 0,0,0,0),			//FORMAT_RGBA_DXT3_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_BC2_UNORM_SRGB, 0,0,0,0),				//FORMAT_RGBA_DXT3_SRGB,
			new GXformat(DDPF.DDPF_FOURCC_ALPHAPIXELS, D3DFORMAT.D3DFMT_DXT5, dxgiFormat.DXGI_FORMAT_BC3_UNORM, 0,0,0,0),			//FORMAT_RGBA_DXT5_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_BC3_UNORM_SRGB, 0,0,0,0),				//FORMAT_RGBA_DXT5_SRGB,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_ATI1, dxgiFormat.DXGI_FORMAT_BC4_UNORM, 0,0,0,0),						//FORMAT_R_ATI1N_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_AT1N, dxgiFormat.DXGI_FORMAT_BC4_SNORM, 0,0,0,0),						//FORMAT_R_ATI1N_SNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_ATI2, dxgiFormat.DXGI_FORMAT_BC5_UNORM, 0,0,0,0),						//FORMAT_RG_ATI2N_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_AT2N, dxgiFormat.DXGI_FORMAT_BC5_SNORM, 0,0,0,0),						//FORMAT_RG_ATI2N_SNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_BC6H_UF16, 0,0,0,0),						//FORMAT_RGB_BP_UFLOAT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_BC6H_SF16, 0,0,0,0),						//FORMAT_RGB_BP_SFLOAT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_BC7_UNORM, 0,0,0,0),						//FORMAT_RGB_BP_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_BC7_UNORM_SRGB, 0,0,0,0),				//FORMAT_RGB_BP_SRGB,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),						//FORMAT_RGB_ETC2_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),						//FORMAT_RGB_ETC2_SRGB,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),						//FORMAT_RGBA_ETC2_A1_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),						//FORMAT_RGBA_ETC2_A1_SRGB,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),						//FORMAT_RGBA_ETC2_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),						//FORMAT_RGBA_ETC2_SRGB,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),						//R11_EAC_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),						//R11_EAC_SNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),						//RG11_EAC_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),						//RG11_EAC_SNORM,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_4X4_UNORM, 0,0,0,0),				//FORMAT_ASTC_4x4_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_4X4_UNORM_SRGB, 0,0,0,0),			//FORMAT_ASTC_4x4_SRGB,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_5X4_UNORM, 0,0,0,0),				//RGBA_ASTC_5x4,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_5X4_UNORM_SRGB, 0,0,0,0),			//SRGB_ALPHA_ASTC_5x4,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_5X5_UNORM, 0,0,0,0),				//RGBA_ASTC_5x5,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_5X5_UNORM_SRGB, 0,0,0,0),			//SRGB_ALPHA_ASTC_5x5,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_6X5_UNORM, 0,0,0,0),				//RGBA_ASTC_6x5,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_6X5_UNORM_SRGB, 0,0,0,0),			//SRGB_ALPHA_ASTC_6x5,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_6X6_UNORM, 0,0,0,0),				//RGBA_ASTC_6x6,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_6X6_UNORM_SRGB, 0,0,0,0),			//SRGB_ALPHA_ASTC_6x6,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_8X5_UNORM, 0,0,0,0),				//RGBA_ASTC_8x5,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_8X5_UNORM_SRGB, 0,0,0,0),			//SRGB_ALPHA_ASTC_8x5,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_8X6_UNORM, 0,0,0,0),				//RGBA_ASTC_8x6,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_8X6_UNORM_SRGB, 0,0,0,0),			//SRGB_ALPHA_ASTC_8x6,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_8X8_UNORM, 0,0,0,0),				//RGBA_ASTC_8x8,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_8X8_UNORM_SRGB, 0,0,0,0),			//SRGB_ALPHA_ASTC_8x8,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_10X5_UNORM, 0,0,0,0),				//RGBA_ASTC_10x5,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_10X5_UNORM_SRGB, 0,0,0,0),			//SRGB_ALPHA_ASTC_10x5,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_10X6_UNORM, 0,0,0,0),				//RGBA_ASTC_10x6,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_10X6_UNORM_SRGB, 0,0,0,0),			//SRGB_ALPHA_ASTC_10x6,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_10X8_UNORM, 0,0,0,0),				//RGBA_ASTC_10x8,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_10X8_UNORM_SRGB, 0,0,0,0),			//SRGB_ALPHA_ASTC_10x8,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_10X10_UNORM_SRGB, 0,0,0,0),			//SRGB_ALPHA_ASTC_10x10,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_10X10_UNORM, 0,0,0,0),				//RGBA_ASTC_10x10,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_12X10_UNORM, 0,0,0,0),				//RGBA_ASTC_12x10,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_12X10_UNORM_SRGB, 0,0,0,0),			//SRGB_ALPHA_ASTC_12x10,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_12X12_UNORM, 0,0,0,0),				//RGBA_ASTC_12x12,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_ASTC_12X12_UNORM_SRGB, 0,0,0,0),			//SRGB_ALPHA_ASTC_12x12,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_A4R4G4B4, dxgiFormat.DXGI_FORMAT_B4G4R4A4_UNORM,  0x0F00, 0x00F0, 0x000F, 0xF000),					//FORMAT_BGRA4_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_A4R4G4B4, dxgiFormat.DXGI_FORMAT_B4G4R4A4_UNORM,  0x0F00, 0x00F0, 0x000F, 0xF000),					//FORMAT_BGRA4_USCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_B5G6R5_UNORM,  0xf800, 0x07e0, 0x001f, 0x0000),							//FORMAT_B5G6R5_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_B5G6R5_UNORM,  0xf800, 0x07e0, 0x001f, 0x0000),							//FORMAT_B5G6R5_USCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_B5G5R5A1_UNORM,  0x7c00, 0x03e0, 0x001f, 0x8000),						//FORMAT_BGR5A1_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_B5G5R5A1_UNORM,  0x7c00, 0x03e0, 0x001f, 0x8000),						//FORMAT_BGR5A1_USCALED,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN,  0x00FF0000, 0x0000FF00, 0x000000FF, 0x00000000),				//FORMAT_BGR8_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_BGR8_SNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_BGR8_USCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_BGR8_SSCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_BGR8_UINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_BGR8_SINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_BGR8_SRGB,

			new GXformat(DDPF.DDPF_RGBA, D3DFORMAT.D3DFMT_A8R8G8B8, dxgiFormat.DXGI_FORMAT_B8G8R8A8_UNORM,  0x00FF0000, 0x0000FF00, 0x000000FF, 0xFF000000),		//FORMAT_BGRA8_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R8G8B8A8_SNORM, 0,0,0,0),													//FORMAT_BGRA8_SNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_BGRA8_USCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),															//FORMAT_BGRA8_SSCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R8G8B8A8_UINT, 0,0,0,0),														//FORMAT_BGRA8_UINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R8G8B8A8_SINT, 0,0,0,0),														//FORMAT_BGRA8_SINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_B8G8R8A8_UNORM_SRGB,  0x00FF0000, 0x0000FF00, 0x000000FF, 0xFF000000),	//FORMAT_BGRA8_SRGB,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R10G10B10A2_UNORM,  0x3FF00000, 0x000FFC00, 0x000003FF, 0xC0000000),	//FORMAT_RGB10A2_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN,  0x3FF00000, 0x000FFC00, 0x000003FF, 0xC0000000),				//FORMAT_RGB10A2_SNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN,  0x3FF00000, 0x000FFC00, 0x000003FF, 0xC0000000),				//FORMAT_RGB10A2_USCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN,  0x3FF00000, 0x000FFC00, 0x000003FF, 0xC0000000),				//FORMAT_RGB10A2_SSCALED,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_R10G10B10A2_UINT,  0x3FF00000, 0x000FFC00, 0x000003FF, 0xC0000000),		//FORMAT_RGB10A2_UINT,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN,  0x3FF00000, 0x000FFC00, 0x000003FF, 0xC0000000),				//FORMAT_RGB10A2_SINT,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_DX10, dxgiFormat.DXGI_FORMAT_UNKNOWN,  0x70, 0x38, 0xC0, 0x00),										//FORMAT_RG3B2_UNORM,
			new GXformat(DDPF.DDPF_RGB, D3DFORMAT.D3DFMT_R8G8B8, dxgiFormat.DXGI_FORMAT_B8G8R8X8_UNORM,  0x00FF0000, 0x0000FF00, 0x000000FF, 0x00000000),		//FORMAT_BGRX8_UNORM,
			new GXformat(DDPF.DDPF_RGB, D3DFORMAT.D3DFMT_R8G8B8, dxgiFormat.DXGI_FORMAT_B8G8R8X8_UNORM_SRGB,  0x00FF0000, 0x0000FF00, 0x000000FF, 0x00000000),	//FORMAT_BGRX8_SRGB,

			new GXformat(DDPF.DDPF_LUMINANCE, D3DFORMAT.D3DFMT_L8, dxgiFormat.DXGI_FORMAT_R8_UNORM,  0x000000FF, 0x00000000, 0x00000000, 0x00000000),				//L8_UNORM,
			new GXformat(DDPF.DDPF_ALPHA, D3DFORMAT.D3DFMT_A8, dxgiFormat.DXGI_FORMAT_R8_UNORM,  0x00000000, 0x00000000, 0x00000000, 0x000000FF),					//A8_UNORM,
			new GXformat(DDPF.DDPF_LUMINANCE_ALPHA, D3DFORMAT.D3DFMT_A8L8, dxgiFormat.DXGI_FORMAT_R8G8_UNORM,  0x000000FF, 0x00000000, 0x00000000, 0x0000FF00),		//LA8_UNORM,
			new GXformat(DDPF.DDPF_LUMINANCE, D3DFORMAT.D3DFMT_L16, dxgiFormat.DXGI_FORMAT_R16_UNORM,  0x0000FFFF, 0x00000000, 0x00000000, 0x00000000),				//L16_UNORM,
			new GXformat(DDPF.DDPF_ALPHA, D3DFORMAT.D3DFMT_UNKNOWN, dxgiFormat.DXGI_FORMAT_R16_UNORM,  0x00000000, 0x00000000, 0x00000000, 0x0000FFFF),				//A16_UNORM,
			new GXformat(DDPF.DDPF_LUMINANCE_ALPHA, D3DFORMAT.D3DFMT_UNKNOWN, dxgiFormat.DXGI_FORMAT_R16_UNORM,  0x0000FFFF, 0x00000000, 0x00000000, 0xFFFF0000),	//LA16_UNORM,

			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_POWERVR_4BPP, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),				//FORMAT_RGB_PVRTC1_8X8_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_UNKNOWN, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),					//FORMAT_RGB_PVRTC1_8X8_SRGB,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_POWERVR_2BPP, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),				//FORMAT_RGB_PVRTC1_16X8_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_UNKNOWN, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),					//FORMAT_RGB_PVRTC1_16X8_SRGB,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_POWERVR_4BPP, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),				//FORMAT_RGBA_PVRTC1_8X8_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_UNKNOWN, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),					//FORMAT_RGBA_PVRTC1_8X8_SRGB,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_POWERVR_2BPP, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),				//FORMAT_RGBA_PVRTC1_16X8_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_UNKNOWN, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),					//FORMAT_RGBA_PVRTC1_16X8_SRGB,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_UNKNOWN, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),					//FORMAT_RGBA_PVRTC2_8X8_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_UNKNOWN, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),					//FORMAT_RGBA_PVRTC2_8X8_SRGB,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_UNKNOWN, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),					//FORMAT_RGBA_PVRTC2_16X8_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_UNKNOWN, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),					//FORMAT_RGBA_PVRTC2_16X8_SRGB,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_ETC, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),						//FORMAT_RGB_ETC_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_ATC, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),						//FORMAT_RGB_ATC_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_ATCA, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),						//FORMAT_RGBA_ATC_EXPLICIT_UNORM,
			new GXformat(DDPF.DDPF_FOURCC, D3DFORMAT.D3DFMT_ATCI, dxgiFormat.DXGI_FORMAT_UNKNOWN, 0,0,0,0),						//FORMAT_RGBA_ATC_INTERPOLATED_UNORM,
		};

        

#endregion Header and Enums
        private static target getTarget(ddsHeader Header, ddsHeader10 Header10)
        {
            if (Header.CubemapFlags.HasFlag(ddsCubemapflag.DDSCAPS2_CUBEMAP))
            {
                if (Header10.ArraySize > 1)
                    return target.TARGET_CUBE_ARRAY;
                else
                    return target.TARGET_CUBE;
            }
            else if (Header10.ArraySize > 1)
            {
                if (Header.Flags.HasFlag(ddsFlag.DDSD_HEIGHT))
                    return target.TARGET_2D_ARRAY;
                else
                    return target.TARGET_1D_ARRAY;
            }
            else if (Header10.ResourceDimension == D3D10_RESOURCE_DIMENSION.D3D10_RESOURCE_DIMENSION_TEXTURE1D)
                return target.TARGET_1D;
            else if (Header10.ResourceDimension == D3D10_RESOURCE_DIMENSION.D3D10_RESOURCE_DIMENSION_TEXTURE3D || Header.Flags.HasFlag(ddsFlag.DDSD_DEPTH))
                return target.TARGET_3D;
            else
                return target.TARGET_2D;
        }
        private static format find(D3DFORMAT FourCC)
        {
            for (int FormatIndex = (int)format.FORMAT_FIRST; FormatIndex <= (int)format.FORMAT_LAST; ++FormatIndex)
            {
                if (ConversionTable[FormatIndex - (int)format.FORMAT_FIRST].D3DFormat == FourCC) return (format)FormatIndex;
            }
            return format.FORMAT_INVALID;
        }
        private static format find(dxgiFormat Format)
        {
            for (int FormatIndex = (int)format.FORMAT_FIRST; FormatIndex <= (int)format.FORMAT_LAST; ++FormatIndex)
            {
                if (ConversionTable[FormatIndex - (int)format.FORMAT_FIRST].DXGIFormat == Format) return (format)FormatIndex;
            }
            return format.FORMAT_INVALID;
        }
        private static int NumberOfSetBits(int i)
        {
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }

        public static ImageDDS LoadFromFile(string filename)
        {
            ImageDDS image = new ImageDDS();
            if (string.IsNullOrWhiteSpace(filename) || !File.Exists(filename))
                throw new ArgumentException("Invalid file name.");
            using (FileStream stream = new FileStream(filename, FileMode.Open))
            {
                if (stream == null || !stream.CanRead || !stream.CanSeek)
                {
                    throw new ArgumentException("Invalid stream");
                }

                image.header = ddsHeader.ReadHeader(stream);
                image.header10 = new ddsHeader10();
                if (image.header.Format.flags.HasFlag(DDPF.DDPF_FOURCC) && image.header.Format.fourCC == D3DFORMAT.D3DFMT_DX10)
                {
                    image.header10 = ddsHeader10.ReadHeader(stream);
                }

                image.format_ = format.FORMAT_INVALID;
                if ((image.header.Format.flags.HasFlag(DDPF.DDPF_RGB | DDPF.DDPF_ALPHAPIXELS | DDPF.DDPF_ALPHA | DDPF.DDPF_YUV | DDPF.DDPF_LUMINANCE)) &&
                    (image.format_ == format.FORMAT_INVALID) &&
                     image.header.Format.flags != DDPF.DDPF_FOURCC_ALPHAPIXELS)
                {
                    switch (image.header.Format.bpp)
                    {
                        case 8:
                            {
                                if (image.header.Format.MaskAreEquals(ConversionTable[(int)format.FORMAT_L8_UNORM]))
                                    image.format_ = format.FORMAT_L8_UNORM;
                                else if (image.header.Format.MaskAreEquals(ConversionTable[(int)format.FORMAT_A8_UNORM]))
                                    image.format_ = format.FORMAT_A8_UNORM;
                                else if (image.header.Format.MaskAreEquals(ConversionTable[(int)format.FORMAT_R8_UNORM]))
                                    image.format_ = format.FORMAT_R8_UNORM;
                                else if (image.header.Format.MaskAreEquals(ConversionTable[(int)format.FORMAT_RG3B2_UNORM]))
                                    image.format_ = format.FORMAT_RG3B2_UNORM;
                                break;
                            }
                        case 16:
                            {
                                if (image.header.Format.MaskAreEquals(ConversionTable[(int)format.FORMAT_LA8_UNORM]))
                                    image.format_ = format.FORMAT_LA8_UNORM;
                                else if (image.header.Format.MaskAreEquals(ConversionTable[(int)format.FORMAT_RG8_UNORM]))
                                    image.format_ = format.FORMAT_RG8_UNORM;
                                else if (image.header.Format.MaskAreEquals(ConversionTable[(int)format.FORMAT_R5G6B5_UNORM]))
                                    image.format_ = format.FORMAT_R5G6B5_UNORM;
                                else if (image.header.Format.MaskAreEquals(ConversionTable[(int)format.FORMAT_L16_UNORM]))
                                    image.format_ = format.FORMAT_L16_UNORM;
                                else if (image.header.Format.MaskAreEquals(ConversionTable[(int)format.FORMAT_A16_UNORM]))
                                    image.format_ = format.FORMAT_A16_UNORM;
                                else if (image.header.Format.MaskAreEquals(ConversionTable[(int)format.FORMAT_R16_UNORM]))
                                    image.format_ = format.FORMAT_R16_UNORM;
                                else if (image.header.Format.MaskAreEquals(ConversionTable[(int)format.FORMAT_RGB5A1_UNORM]))
                                    image.format_ = format.FORMAT_RGB5A1_UNORM;
                                break;
                            }
                        case 24:
                            {
                                if (image.header.Format.MaskAreEquals(ConversionTable[(int)format.FORMAT_RGB8_UNORM]))
                                    image.format_ = format.FORMAT_RGB8_UNORM;
                                break;
                            }
                        case 32:
                            {
                                if (image.header.Format.MaskAreEquals(ConversionTable[(int)format.FORMAT_BGRX8_UNORM]))
                                    image.format_ = format.FORMAT_BGRX8_UNORM;
                                else if (image.header.Format.MaskAreEquals(ConversionTable[(int)format.FORMAT_BGRA8_UNORM]))
                                    image.format_ = format.FORMAT_BGRA8_UNORM;
                                else if (image.header.Format.MaskAreEquals(ConversionTable[(int)format.FORMAT_RGBA8_UNORM]))
                                    image.format_ = format.FORMAT_RGBA8_UNORM;
                                else if (image.header.Format.MaskAreEquals(ConversionTable[(int)format.FORMAT_RGB10A2_UNORM]))
                                    image.format_ = format.FORMAT_RGB10A2_UNORM;
                                else if (image.header.Format.MaskAreEquals(ConversionTable[(int)format.FORMAT_LA16_UNORM]))
                                    image.format_ = format.FORMAT_LA16_UNORM;
                                else if (image.header.Format.MaskAreEquals(ConversionTable[(int)format.FORMAT_RG16_UNORM]))
                                    image.format_ = format.FORMAT_RG16_UNORM;
                                else if (image.header.Format.MaskAreEquals(ConversionTable[(int)format.FORMAT_R32_SFLOAT]))
                                    image.format_ = format.FORMAT_R32_SFLOAT;
                            }
                            break;
                    }
                }
                else if ((image.header.Format.flags.HasFlag(DDPF.DDPF_FOURCC) && (image.header.Format.fourCC != D3DFORMAT.D3DFMT_DX10) && image.format_ == format.FORMAT_INVALID))
                    image.format_ = find(image.header.Format.fourCC);
                else if ((image.header.Format.fourCC == D3DFORMAT.D3DFMT_DX10) && (image.header10.Format != dxgiFormat.DXGI_FORMAT_UNKNOWN))
                    image.format_ = find(image.header10.Format);

                image.mipMapCount_ = (image.header.Flags.HasFlag(ddsFlag.DDSD_MIPMAPCOUNT)) ? (int)image.header.MipMapLevels : 1;
                image.faceCount_ = 1;
                if (image.header.CubemapFlags.HasFlag(ddsCubemapflag.DDSCAPS2_CUBEMAP))
                    image.faceCount_ = NumberOfSetBits((int)(image.header.CubemapFlags & DDSCAPS2_CUBEMAP_ALLFACES));

                image.depth_ = 1;
                if (image.header.CubemapFlags.HasFlag(ddsCubemapflag.DDSCAPS2_VOLUME))
                    image.depth_ = image.header.Depth;

                image.target_ = getTarget(image.header, image.header10);
                image.arraySize_ = System.Math.Max(image.header10.ArraySize, 1);
                //texture Texture(
                //    getTarget(Header, Header10), Format,
                //    texture::dim_type(Header.Width, Header.Height, DepthCount),
                //    std::max<std::size_t>(Header10.ArraySize, 1), FaceCount, MipMapCount);

                //std::size_t const SourceSize = Offset + Texture.size();
                //assert(SourceSize == Size);

                //std::memcpy(Texture.data(), Data + Offset, Texture.size());
                image.imgData = new byte[image.depth_][][];
                int bytesPerBlock = (int)GLImage.BlockSize(image.format_);
                // foreach face in the cubemap, get all it's mipmaps levels. Only one iteration for Texture2D
                for (int Slices = 0; Slices < image.Depth; Slices++)
                {
                    int trueMipMapCount = image.mipMapCount_ - 1; // TODO: triplecheck correctness
                    int Width = (int)image.header.Width;
                    int Height = (int)image.header.Height;
                    image.imgData[Slices] = new byte[image.mipMapCount_][];
                    for (int Level = 0; Level < image.mipMapCount_; Level++) // start at base image
                    {
                        //#region determine Dimensions
                        int BlocksPerRow = (Width + 3) >> 2;
                        int BlocksPerColumn = (Height + 3) >> 2;
                        int SurfaceBlockCount = BlocksPerRow * BlocksPerColumn; //   // DXTn stores Texels in 4x4 blocks, a Color block is 8 Bytes, an Alpha block is 8 Bytes for DXT3/5
                        int SurfaceSizeInBytes = (int)(SurfaceBlockCount * bytesPerBlock);

                        //// this check must evaluate to false for 2D and Cube maps, or it's impossible to determine MipMap sizes.
                        //if (TextureLoaderParameters.Verbose && Level == 0 && image.IsCompressed && image._BytesForMainSurface != SurfaceSizeInBytes)
                        //    Trace.WriteLine("Warning: Calculated byte-count of main image differs from what was read from file.");
                        //#endregion determine Dimensions

                        //// skip mipmaps smaller than a 4x4 Pixels block, which is the smallest DXTn unit.
                        if (Width > 2 && Height > 2)
                        { // Note: there could be a potential problem with non-power-of-two cube maps
                          //byte[] RawDataOfSurface = new byte[SurfaceSizeInBytes];
                            image.imgData[Slices][Level] = new byte[SurfaceSizeInBytes];
                            // no changes to the image, copy as is
                            stream.Read(image.imgData[Slices][Level], 0, SurfaceSizeInBytes);
                        }
                        else
                        {
                            if (trueMipMapCount > Level)
                                trueMipMapCount = Level - 1; // The current Level is invalid
                        }

#region Prepare the next MipMap level
                        Width /= 2;
                        if (Width < 1)
                            Width = 1;
                        Height /= 2;
                        if (Height < 1)
                            Height = 1;
#endregion Prepare the next MipMap level
                    }
                }
            }
            return image;
        }
        public TextureDDS BuildTexture(Texture.Parameters params_ = null)
        {
            if (params_ == null)
                params_ = new Texture.Parameters();
            TextureDDS texKtx = new TextureDDS();
            texKtx.init(this, params_);

            return texKtx;
        }
#region Data
        private ddsHeader header;
        private ddsHeader10 header10;
        private target target_;
        private format format_;
        private uint depth_;
        private uint arraySize_;
        private int faceCount_;
        private int mipMapCount_;
        private byte[][][] imgData;
#endregion

        public TextureTarget TextureTarget { get { return (TextureTarget)target_; } }
        public int Width { get { return (int)this.header.Width; } }
        public int Height { get { return (int)this.header.Height; } }
        public int Depth { get { return (int)this.depth_; } }
        public int NumberOfMipmapLevels { get { return (int)this.mipMapCount_; } }
        public PixelInternalFormat InternalFormat { get { return GLFormat.GetPixelInternalFormat(this.format_); } }
        public bool IsCompressed { get { return GLImage.IsCompressed(this.format_); } }
        public byte[] this[int mipmapLevel, int face]
        {
            get { return imgData[face][mipmapLevel]; }
        }
    }
}
