using log4net;
using OpenTK.Graphics.OpenGL;
using Sxta.Core;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sxta.Render
{
    /// <summary>
    /// An abstract texture.
    /// </summary>
    public abstract class Texture : ISwappable<Texture>, IDisposable
    {
        // conservative estimation of the true maximum number of texture units,
        // used to allocate static arrays for storing texture unit states
        public const int MAX_TEXTURE_UNITS = 64;

        public class Parameters : Sampler.Parameters
        {

            /// <summary>
            /// Creates a new set of texture parameters with default values.
            /// Initializes a new instance of the <see cref="Sxta.Render.Texture+Parameters"/> class.
            /// </summary>
            public Parameters()
                : base()
            {
                _minLevel = 0;
                _maxLevel = 1000;

                _swizzle = "rgba";
            }

            /*
             * Deletes this set of texture parameters.
             */
            //public virtual ~Parameters() { }

            /// <summary>
            /// Returns the current texture swizzling order.
            /// </summary>
            public string swizzle()
            {
                return _swizzle;
            }


            /// <summary>
            /// Returns the index of the lowest defined mipmap level.
            /// </summary>
            /// <returns>
            /// The level.
            /// </returns>
            public int minLevel()
            {
                return _minLevel;
            }


            /// <summary>
            /// Returns the index of the highest defined mipmap level.
            /// </summary>
            /// <returns>
            /// The level.
            /// </returns>
            public int maxLevel()
            {
                return _maxLevel;
            }

            public new Parameters wrapS(TextureWrap wrapS)
            {
                base.wrapS(wrapS);
                return this;
            }

            public new Parameters wrapT(TextureWrap wrapT)
            {
                base.wrapT(wrapT);
                return this;
            }

            public new Parameters wrapR(TextureWrap wrapR)
            {
                base.wrapR(wrapR);
                return this;
            }

            public new Parameters min(TextureFilter min)
            {
                base.min(min);
                return this;
            }

            public new Parameters mag(TextureFilter mag)
            {
                base.mag(mag);
                return this;
            }

            public override Sampler.Parameters borderi(int r, int g, int b, int a = 0)
            {
                base.borderi(r, g, b, a);
                return this;
            }

            public override Sampler.Parameters borderf(float r, float g, float b, float a = 0.0f)
            {
                base.borderf(r, g, b, a);
                return this;
            }

            public override Sampler.Parameters borderIi(int r, int g, int b, int a = 0)
            {
                base.borderIi(r, g, b, a);
                return this;
            }

            public override Sampler.Parameters borderIui(uint r, uint g, uint b, uint a = 0)
            {
                base.borderIui(r, g, b, a);
                return this;
            }

            public override Sampler.Parameters lodMin(float lodMin)
            {
                base.lodMin(lodMin);
                return this;
            }

            public override Sampler.Parameters lodMax(float lodMax)
            {
                base.lodMax(lodMax);
                return this;
            }

            public override Sampler.Parameters lodBias(float lodBias)
            {
                base.lodBias(lodBias);
                return this;
            }

            public override Sampler.Parameters compareFunc(Function compareFunc)
            {
                base.compareFunc(compareFunc);
                return this;
            }

            public override Sampler.Parameters maxAnisotropyEXT(float maxAnisotropy)
            {
                base.maxAnisotropyEXT(maxAnisotropy);
                return this;
            }


            /// <summary>
            /// Sets the swizzling order for this Texture.
            /// Enables the user to reorder the components of a texture before it is sent to the GPU.
            /// Each parameter can be either 'r', 'g', 'b', or 'a'.
            /// </summary>
            /// <param name='r'>
            /// R. new component for red channel.
            /// </param>
            /// <param name='g'>
            /// G.new component for green channel.
            /// </param>
            /// <param name='b'>
            /// B. new component for blue channel.
            /// </param>
            /// <param name='a'>
            /// A. new component for alpha channel.
            /// </param>
            public virtual Parameters swizzle(char r, char g, char b, char a)
            {
                // Combine chars into array
                char[] arr = new char[4];
                arr[0] = r;
                arr[1] = g;
                arr[2] = b;
                arr[3] = a;
                // Return new string key
                _swizzle = new string(arr);
                return this;
            }

            /// <summary>
            /// MSets the index of the lowest defined mipmap level.
            /// </summary>
            /// <returns>
            /// The level.
            /// </returns>
            /// <param name='minLevel'>
            /// Minimum level.
            /// </param>
            public virtual Parameters minLevel(int minLevel)
            {
                _minLevel = minLevel;
                return this;
            }


            /// <summary>
            /// Sets the index of the highest defined mipmap level.
            /// </summary>
            /// <returns>
            /// The level.
            /// </returns>
            /// <param name='maxLevel'>
            /// Max level.
            /// </param>
            public virtual Parameters maxLevel(int maxLevel)
            {
                _maxLevel = maxLevel;
                return this;
            }


            private string _swizzle;

            private int _minLevel;

            private int _maxLevel;
        }

        /// <summary>
        /// Destroys this texture.
        /// Releases unmanaged resources and performs other cleanup operations before the <see cref="Sxta.Render.Texture"/> is
        /// reclaimed by garbage collection.
        /// </summary>
        ~Texture()
        {
            // Do not re-create Dispose clean-up code here. 
            // Calling Dispose(false) is optimal in terms of 
            // readability and maintainability.
            Dispose(false);
        }

        public string Name { get; set; }


        /// <summary>
        /// Returns the identifier of this texture.
        /// </summary>
        /// <returns>
        /// The identifier.
        /// </returns>
        public uint getId()
        {
            return textureId;
        }


        /// <summary>
        /// Returns the number of components in the texture's internal format.
        /// </summary>
        /// <returns>
        /// The components.
        /// </returns>
        public int getComponents()
        {
            return EnumConversion.getTextureComponents(getFormat());
        }

        public abstract int Width { get; }
        public abstract int Height { get; }


        /// <summary>
        /// Returns the name of the texture's internal format.
        /// </summary>
        /// <returns>
        /// The internal format name.
        /// </returns>
        public string getInternalFormatName()
        {
            return EnumConversion.getTextureInternalFormatName(internalFormat);
        }


        /// <summary>
        /// Returns the internal format of this texture.
        /// </summary>
        /// <returns>
        /// The internal format.
        /// </returns>
        public TextureInternalFormat getInternalFormat()
        {
            return internalFormat;
        }


        /// <summary>
        /// Returns a Format compatible with the internal format of this Texture.
        /// </summary>
        /// <returns>
        /// The format.
        /// </returns>
        public TextureFormat getFormat()
        {
            switch (internalFormat)
            {
                case TextureInternalFormat.R8:
                case TextureInternalFormat.R8_SNORM:
                case TextureInternalFormat.R16:
                case TextureInternalFormat.R16_SNORM:
                case TextureInternalFormat.COMPRESSED_RED:
                case TextureInternalFormat.R16F:
                case TextureInternalFormat.R32F:
                case TextureInternalFormat.COMPRESSED_RED_RGTC1:
                case TextureInternalFormat.COMPRESSED_SIGNED_RED_RGTC1:
                    return TextureFormat.RED;
                case TextureInternalFormat.R8I:
                case TextureInternalFormat.R8UI:
                case TextureInternalFormat.R16I:
                case TextureInternalFormat.R16UI:
                case TextureInternalFormat.R32I:
                case TextureInternalFormat.R32UI:
                    return TextureFormat.RED_INTEGER;
                case TextureInternalFormat.RG8:
                case TextureInternalFormat.RG8_SNORM:
                case TextureInternalFormat.RG16:
                case TextureInternalFormat.RG16_SNORM:
                case TextureInternalFormat.RG16F:
                case TextureInternalFormat.RG32F:
                case TextureInternalFormat.COMPRESSED_RG:
                case TextureInternalFormat.COMPRESSED_RG_RGTC2:
                case TextureInternalFormat.COMPRESSED_SIGNED_RG_RGTC2:
                    return TextureFormat.RG;
                case TextureInternalFormat.RG8I:
                case TextureInternalFormat.RG8UI:
                case TextureInternalFormat.RG16I:
                case TextureInternalFormat.RG16UI:
                case TextureInternalFormat.RG32I:
                case TextureInternalFormat.RG32UI:
                    return TextureFormat.RG_INTEGER;
                case TextureInternalFormat.R3_G3_B2:
                case TextureInternalFormat.RGB4:
                case TextureInternalFormat.RGB5:
                case TextureInternalFormat.RGB8:
                case TextureInternalFormat.RGB8_SNORM:
                case TextureInternalFormat.RGB10:
                case TextureInternalFormat.RGB12:
                case TextureInternalFormat.RGB16:
                case TextureInternalFormat.RGB16_SNORM:
                case TextureInternalFormat.SRGB8:
                case TextureInternalFormat.RGB16F:
                case TextureInternalFormat.RGB32F:
                case TextureInternalFormat.R11F_G11F_B10F:
                case TextureInternalFormat.RGB9_E5:
                case TextureInternalFormat.COMPRESSED_RGB:
                case TextureInternalFormat.COMPRESSED_RGB_BPTC_SIGNED_FLOAT_ARB:
                case TextureInternalFormat.COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT_ARB:
                case TextureInternalFormat.COMPRESSED_RGB_S3TC_DXT1_EXT:
                    return TextureFormat.RGB;
                case TextureInternalFormat.RGB8I:
                case TextureInternalFormat.RGB8UI:
                case TextureInternalFormat.RGB16I:
                case TextureInternalFormat.RGB16UI:
                case TextureInternalFormat.RGB32I:
                case TextureInternalFormat.RGB32UI:
                    return TextureFormat.RGB_INTEGER;
                case TextureInternalFormat.RGB5_A1:
                case TextureInternalFormat.RGBA2:
                case TextureInternalFormat.RGBA4:
                case TextureInternalFormat.RGBA8:
                case TextureInternalFormat.RGBA8_SNORM:
                case TextureInternalFormat.RGB10_A2:
                case TextureInternalFormat.RGB10_A2UI:
                case TextureInternalFormat.RGBA12:
                case TextureInternalFormat.RGBA16:
                case TextureInternalFormat.RGBA16_SNORM:
                case TextureInternalFormat.SRGB8_ALPHA8:
                case TextureInternalFormat.RGBA16F:
                case TextureInternalFormat.RGBA32F:
                    return TextureFormat.RGBA;
                case TextureInternalFormat.DEPTH_COMPONENT16:
                case TextureInternalFormat.DEPTH_COMPONENT24:
                case TextureInternalFormat.DEPTH_COMPONENT32F:
                    return TextureFormat.DEPTH_COMPONENT;
                case TextureInternalFormat.DEPTH24_STENCIL8:
                case TextureInternalFormat.DEPTH32F_STENCIL8:
                    return TextureFormat.DEPTH_STENCIL;
                case TextureInternalFormat.COMPRESSED_RGBA:
                case TextureInternalFormat.COMPRESSED_SRGB:
                case TextureInternalFormat.COMPRESSED_RGBA_BPTC_UNORM_ARB:
                case TextureInternalFormat.COMPRESSED_SRGB_ALPHA_BPTC_UNORM_ARB:
                case TextureInternalFormat.COMPRESSED_RGBA_S3TC_DXT1_EXT:
                case TextureInternalFormat.COMPRESSED_RGBA_S3TC_DXT3_EXT:
                case TextureInternalFormat.COMPRESSED_RGBA_S3TC_DXT5_EXT:
                    return TextureFormat.RGBA;
                case TextureInternalFormat.RGBA8I:
                case TextureInternalFormat.RGBA8UI:
                case TextureInternalFormat.RGBA16I:
                case TextureInternalFormat.RGBA16UI:
                case TextureInternalFormat.RGBA32I:
                case TextureInternalFormat.RGBA32UI:
                    return TextureFormat.RGBA_INTEGER;
            }
            Debug.Assert(false);
            throw new Exception();
        }


        /// <summary>
        /// True if this texture has any mipmap level above 0.
        /// False if this texture has only a base level.
        /// </summary>
        /// <returns>
        /// The mipmaps.
        /// </returns>
        public bool hasMipmaps()
        {
            return params_.min() != TextureFilter.NEAREST && params_.min() != TextureFilter.LINEAR;
        }



        /// <summary>
        /// Returns true if this texture is in a compressed format on GPU.
        /// </summary>
        /// <returns>
        /// The compressed.
        /// </returns>
        public bool isCompressed()
        {
            switch (internalFormat)
            {
                case TextureInternalFormat.R8:
                case TextureInternalFormat.R8_SNORM:
                case TextureInternalFormat.R16:
                case TextureInternalFormat.R16_SNORM:
                case TextureInternalFormat.RG8:
                case TextureInternalFormat.RG8_SNORM:
                case TextureInternalFormat.RG16:
                case TextureInternalFormat.RG16_SNORM:
                case TextureInternalFormat.R3_G3_B2:
                case TextureInternalFormat.RGB4:
                case TextureInternalFormat.RGB5:
                case TextureInternalFormat.RGB8:
                case TextureInternalFormat.RGB8_SNORM:
                case TextureInternalFormat.RGB10:
                case TextureInternalFormat.RGB12:
                case TextureInternalFormat.RGB16:
                case TextureInternalFormat.RGB16_SNORM:
                case TextureInternalFormat.RGBA2:
                case TextureInternalFormat.RGBA4:
                case TextureInternalFormat.RGB5_A1:
                case TextureInternalFormat.RGBA8:
                case TextureInternalFormat.RGBA8_SNORM:
                case TextureInternalFormat.RGB10_A2:
                case TextureInternalFormat.RGB10_A2UI:
                case TextureInternalFormat.RGBA12:
                case TextureInternalFormat.RGBA16:
                case TextureInternalFormat.RGBA16_SNORM:
                case TextureInternalFormat.SRGB8:
                case TextureInternalFormat.SRGB8_ALPHA8:
                case TextureInternalFormat.R16F:
                case TextureInternalFormat.RG16F:
                case TextureInternalFormat.RGB16F:
                case TextureInternalFormat.RGBA16F:
                case TextureInternalFormat.R32F:
                case TextureInternalFormat.RG32F:
                case TextureInternalFormat.RGB32F:
                case TextureInternalFormat.RGBA32F:
                case TextureInternalFormat.R11F_G11F_B10F:
                case TextureInternalFormat.RGB9_E5:
                case TextureInternalFormat.R8I:
                case TextureInternalFormat.R8UI:
                case TextureInternalFormat.R16I:
                case TextureInternalFormat.R16UI:
                case TextureInternalFormat.R32I:
                case TextureInternalFormat.R32UI:
                case TextureInternalFormat.RG8I:
                case TextureInternalFormat.RG8UI:
                case TextureInternalFormat.RG16I:
                case TextureInternalFormat.RG16UI:
                case TextureInternalFormat.RG32I:
                case TextureInternalFormat.RG32UI:
                case TextureInternalFormat.RGB8I:
                case TextureInternalFormat.RGB8UI:
                case TextureInternalFormat.RGB16I:
                case TextureInternalFormat.RGB16UI:
                case TextureInternalFormat.RGB32I:
                case TextureInternalFormat.RGB32UI:
                case TextureInternalFormat.RGBA8I:
                case TextureInternalFormat.RGBA8UI:
                case TextureInternalFormat.RGBA16I:
                case TextureInternalFormat.RGBA16UI:
                case TextureInternalFormat.RGBA32I:
                case TextureInternalFormat.RGBA32UI:
                case TextureInternalFormat.DEPTH_COMPONENT16:
                case TextureInternalFormat.DEPTH_COMPONENT24:
                case TextureInternalFormat.DEPTH_COMPONENT32F:
                case TextureInternalFormat.DEPTH24_STENCIL8:
                case TextureInternalFormat.DEPTH32F_STENCIL8:
                    return false;
                case TextureInternalFormat.COMPRESSED_RED:
                case TextureInternalFormat.COMPRESSED_RG:
                case TextureInternalFormat.COMPRESSED_RGB:
                case TextureInternalFormat.COMPRESSED_RGBA:
                case TextureInternalFormat.COMPRESSED_SRGB:
                case TextureInternalFormat.COMPRESSED_RED_RGTC1:
                case TextureInternalFormat.COMPRESSED_SIGNED_RED_RGTC1:
                case TextureInternalFormat.COMPRESSED_RG_RGTC2:
                case TextureInternalFormat.COMPRESSED_SIGNED_RG_RGTC2:
                case TextureInternalFormat.COMPRESSED_RGBA_BPTC_UNORM_ARB:
                case TextureInternalFormat.COMPRESSED_SRGB_ALPHA_BPTC_UNORM_ARB:
                case TextureInternalFormat.COMPRESSED_RGB_BPTC_SIGNED_FLOAT_ARB:
                case TextureInternalFormat.COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT_ARB:
                case TextureInternalFormat.COMPRESSED_RGB_S3TC_DXT1_EXT:
                case TextureInternalFormat.COMPRESSED_RGBA_S3TC_DXT1_EXT:
                case TextureInternalFormat.COMPRESSED_RGBA_S3TC_DXT3_EXT:
                case TextureInternalFormat.COMPRESSED_RGBA_S3TC_DXT5_EXT:
                    return true;
            }
            Debug.Assert(false);
            throw new Exception();
        }


        /// <summary>
        /// Returns the size of the compressed data of this texture. Must be used
        /// only for a compressed texture (see #isCompressed).
        /// </summary>
        /// <returns>
        /// The compressed size.
        /// </returns>
        /// <param name='level'>
        /// Level.
        /// </param>
        public int getCompressedSize(int level)
        {
            int size;
            bindToTextureUnit();
#if OPENTK
            GL.GetTexLevelParameter(textureTarget, level, GetTextureParameter.TextureCompressedImageSize, out size);
#else
            glGetTexLevelParameteriv(textureTarget, level, GL_TEXTURE_COMPRESSED_IMAGE_SIZE, &size);
#endif
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
            return size;
        }


        /// <summary>
        /// Returns the texture pixels in the specified format.
        /// </summary>
        /// <returns>
        /// The image.
        /// </returns>
        /// <param name='level'>
        /// Level. the texture LOD level to be read.
        /// </param>
        /// <param name='f'>
        /// F. the format in which data must be returned.
        /// </param>
        /// <param name='t'>
        /// T. the type in which pixel components must be returned.
        /// </param>
        /// <param name='pixels'>
        /// Pixels pixels the returned data.
        /// </param>
        public void getImage(int level, TextureFormat f, PixelType t, out byte[] pixels)
        {
            bindToTextureUnit();
#if OPENTK
            throw new NotImplementedException();
            pixels = new byte[EnumConversion.getTextureComponents(f)]; // TODO determine the size of the array???
            GL.GetTexImage<byte>(textureTarget, level, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels);
#else
            glGetTexImage(textureTarget, level, getTextureFormat(f), getPixelType(t), pixels);
#endif
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
        }


        /// <summary>
        /// Returns the compressed data of this texture. Must be used only for a
        /// compressed texture (see #isCompressed).
        /// </summary>
        /// <returns>
        /// The compressed image.
        /// </returns>
        /// <param name='level'>
        /// Level the texture LOD level to be read.
        /// </param>
        /// <param name='pixels'>
        /// Pixels. the returned compressed data.
        /// </param>
        public void getCompressedImage(int level, out byte[] pixels)
        {
            bindToTextureUnit();
#if OPENTK
            pixels = new byte[getCompressedSize(level)];
            GL.GetCompressedTexImage<byte>(textureTarget, level, pixels);
#else
            glGetCompressedTexImage(textureTarget, level, pixels);
#endif
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
        }


        /// <summary>
        /// Generates the mipmap levels for this texture.
        /// </summary>
        /// <returns>
        /// The mip map.
        /// </returns>
        public void generateMipMap()
        {
            if (hasMipmaps())
            {
                bindToTextureUnit();
#if OPENTK
                GL.GenerateMipmap((GenerateMipmapTarget)textureTarget);
#else
                glGenerateMipmap(textureTarget);
#endif
                Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
            }
            else
            {
                // do nothing, but this could be an error
            }
        }




        /// <summary>
        /// Creates a new unitialized texture.
        /// Initializes a new instance of the <see cref="Sxta.Render.Texture"/> class.
        /// </summary>
        /// <param name='type'>
        /// Type. the texture class name.
        /// </param>
        /// <param name='target'>
        /// Target. a texture type (1D, 2D, 3D, etc).
        /// </param>
        protected Texture(string type, TextureTarget target)
        {
            textureTarget = target;
        }

        /// <summary>
        /// Creates a new unitialized texture.
        /// Initializes a new instance of the <see cref="Sxta.Render.Texture"/> class.
        /// </summary>
        protected Texture()
        {
        }

        /// <summary>
        /// Initializes this texture.
        /// </summary>
        /// <param name='tf'>
        /// Tf. texture data format on GPU.
        /// </param>
        /// <param name='params_'>
        /// Params_. the texture parameters.
        /// </param>
        protected void init(TextureInternalFormat tf, Parameters params_)
        {
#if OPENTK
            GL.GenTextures(1, out textureId);
#else
            glGenTextures(1, &textureId);
#endif
            Debug.Assert(textureId > 0);

            this.internalFormat = tf;
            this.params_ = params_;

            bindToTextureUnit();

            if (textureTarget == TextureTarget.TextureBuffer)
            {
                Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
                return;
            }

#if OPENTK
            GL.TexParameter(textureTarget, TextureParameterName.TextureWrapS, EnumConversion.getTextureWrap(params_.wrapS()));
            GL.TexParameter(textureTarget, TextureParameterName.TextureWrapT, EnumConversion.getTextureWrap(params_.wrapT()));
            GL.TexParameter(textureTarget, TextureParameterName.TextureWrapR, EnumConversion.getTextureWrap(params_.wrapR()));
            GL.TexParameter(textureTarget, TextureParameterName.TextureMinFilter, EnumConversion.getTextureMinFilter(params_.min()));
            GL.TexParameter(textureTarget, TextureParameterName.TextureMagFilter, EnumConversion.getTextureMagFilter(params_.mag()));
            switch (params_.borderType())
            {
                case 0: // i
                    GL.TexParameter(textureTarget, TextureParameterName.TextureBorderColor, params_.borderi());
                    break;
                case 1: // f
                    GL.TexParameter(textureTarget, TextureParameterName.TextureBorderColor, params_.borderf());
                    break;
                case 2: // Ii
                    GL.TexParameterI(textureTarget, TextureParameterName.TextureBorderColor, params_.borderIi());
                    break;
                case 3: // Iui
                    GL.TexParameterI(textureTarget, TextureParameterName.TextureBorderColor, params_.borderIui());
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
            if (textureTarget != TextureTarget.TextureRectangle)
            {
                GL.TexParameter(textureTarget, TextureParameterName.TextureMinLod, params_.lodMin());
                GL.TexParameter(textureTarget, TextureParameterName.TextureMaxLod, params_.lodMax());
            }

            GL.TexParameter(textureTarget, TextureParameterName.TextureLodBias, params_.lodBias());
            if (params_.compareFunc() != Function.ALWAYS)
            {
#if TODO
                GL.TexParameter(textureTarget, TextureParameterName.TextureCompareMode, GL_COMPARE_REF_TO_TEXTURE);
                GL.TexParameter(textureTarget, TextureParameterName.TextureCompareFunc, EnumConversion.getFunction(params_.compareFunc()));
#endif
                throw new NotImplementedException();
            }
#if TODO
            GL.TexParameter(textureTarget, TextureParameterName.GL_TEXTURE_MAX_ANISOTROPY_EXT, params_.maxAnisotropyEXT());
#endif
            GL.TexParameter(textureTarget, TextureParameterName.TextureSwizzleR, EnumConversion.getTextureSwizzle(params_.swizzle()[0]));
            GL.TexParameter(textureTarget, TextureParameterName.TextureSwizzleG, EnumConversion.getTextureSwizzle(params_.swizzle()[1]));
            GL.TexParameter(textureTarget, TextureParameterName.TextureSwizzleB, EnumConversion.getTextureSwizzle(params_.swizzle()[2]));
            GL.TexParameter(textureTarget, TextureParameterName.TextureSwizzleA, EnumConversion.getTextureSwizzle(params_.swizzle()[3]));
            if (textureTarget != TextureTarget.TextureRectangle)
            {
                GL.TexParameter(textureTarget, TextureParameterName.TextureBaseLevel, params_.minLevel());
                GL.TexParameter(textureTarget, TextureParameterName.TextureMaxLevel, params_.maxLevel());
            }
#else
            glTexParameteri(textureTarget, GL_TEXTURE_WRAP_S, getTextureWrap(params_.wrapS()));
            glTexParameteri(textureTarget, GL_TEXTURE_WRAP_T, getTextureWrap(params_.wrapT()));
            glTexParameteri(textureTarget, GL_TEXTURE_WRAP_R, getTextureWrap(params_.wrapR()));
            glTexParameteri(textureTarget, GL_TEXTURE_MIN_FILTER, getTextureFilter(params_.min()));
            glTexParameteri(textureTarget, GL_TEXTURE_MAG_FILTER, getTextureFilter(params_.mag()));
            switch (params_.borderType())
            {
                case 0: // i
                    glTexParameteriv(textureTarget, GL_TEXTURE_BORDER_COLOR, params_.borderi());
                    break;
                case 1: // f
                    glTexParameterfv(textureTarget, GL_TEXTURE_BORDER_COLOR, params_.borderf());
                    break;
                case 2: // Ii
                    glTexParameterIiv(textureTarget, GL_TEXTURE_BORDER_COLOR, params_.borderIi());
                    break;
                case 3: // Iui
                    glTexParameterIuiv(textureTarget, GL_TEXTURE_BORDER_COLOR, params_.borderIui());
                    break;
                default:
                    Debug.Assert(false);
            }
            if (textureTarget != GL_TEXTURE_RECTANGLE)
            {
                glTexParameterf(textureTarget, GL_TEXTURE_MIN_LOD, params_.lodMin());
                glTexParameterf(textureTarget, GL_TEXTURE_MAX_LOD, params_.lodMax());
            }

            glTexParameterf(textureTarget, GL_TEXTURE_LOD_BIAS, params_.lodBias());
            if (params_.compareFunc() != ALWAYS)
            {
                glTexParameteri(textureTarget, GL_TEXTURE_COMPARE_MODE, GL_COMPARE_REF_TO_TEXTURE);
                glTexParameteri(textureTarget, GL_TEXTURE_COMPARE_FUNC, getFunction(params_.compareFunc()));
            }
            glTexParameterf(textureTarget, GL_TEXTURE_MAX_ANISOTROPY_EXT, params_.maxAnisotropyEXT());
            glTexParameteri(textureTarget, GL_TEXTURE_SWIZZLE_R, getTextureSwizzle(params_.swizzle()[0]));
            glTexParameteri(textureTarget, GL_TEXTURE_SWIZZLE_G, getTextureSwizzle(params_.swizzle()[1]));
            glTexParameteri(textureTarget, GL_TEXTURE_SWIZZLE_B, getTextureSwizzle(params_.swizzle()[2]));
            glTexParameteri(textureTarget, GL_TEXTURE_SWIZZLE_A, getTextureSwizzle(params_.swizzle()[3]));
            if (textureTarget != GL_TEXTURE_RECTANGLE)
            {
                glTexParameteri(textureTarget, GL_TEXTURE_BASE_LEVEL, params_.minLevel());
                glTexParameteri(textureTarget, GL_TEXTURE_MAX_LEVEL, params_.maxLevel());
            }
#endif


            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
        }


        /// <summary>
        /// Binds this texture and the given sampler to a texture unit, for the
        /// given program. If there is a texture unit to which no texture is
        /// currently bound the texture is bound to this unit. Otherwise the
        /// least recently used texture unit that is not used by the given program
        /// is unbound, and this texture is bound instead.
        /// </summary>
        /// <returns>
        /// The to texture unit. to which the texture has been bound, or -1
        /// </returns>
        /// <param name='sampler'>
        /// Sampler. s a sampler object to sample this texture. May be null.
        /// </param>
        /// <param name='programId'>
        /// Program identifier. the program for which this texture must be bound.
        /// </param>
        internal protected int bindToTextureUnit(Sampler sampler, uint programId)
        {
            Debug.Assert(programId != 0);

            uint samplerId = sampler == null ? 0 : sampler.getId();
            uint val;
            uint unit;
            if (!currentTextureUnits.TryGetValue(samplerId, out val))
            {
                unit = (uint)TextureUnitManager.TEXTURE_UNIT_MANAGER.findFreeTextureUnit((int)programId);
            }
            else
            {
                unit = val;
            }

            TextureUnitManager.TEXTURE_UNIT_MANAGER.bind(unit, sampler, this);

            return (int)unit;
        }

        /// <summary>
        /// Binds this texture to a texture unit. If there is a texture unit
        /// to which no texture is currently bound the texture is bound to this
        /// unit. Otherwise the least recently used texture unit is unbound, and
        /// this texture is bound instead.
        /// </summary>
        /// <returns>
        /// The to texture unit.
        /// </returns>
        internal protected int bindToTextureUnit()
        {
            if (currentTextureUnits.Count == 0)
            {
                int programId = 0;
                if (Program.CURRENT != null)
                {
                    programId = Program.CURRENT.getId();
                }
                int unit = TextureUnitManager.TEXTURE_UNIT_MANAGER.findFreeTextureUnit(programId);
                TextureUnitManager.TEXTURE_UNIT_MANAGER.bind((uint)unit, null, this);
                return unit;
            }
            else
            {
                int unit = (int)currentTextureUnits[0];
#if OPENTK
                GL.ActiveTexture((OpenTK.Graphics.OpenGL.TextureUnit)((int)(OpenTK.Graphics.OpenGL.TextureUnit.Texture0) + unit));
#else
                glActiveTexture(GL_TEXTURE0 + unit);
#endif
                return unit;
            }
            // Visual Studio says this code is not reachable so I
            // commented it.
            // return -1;
        }


        /// <summary>
        /// Swaps this texture with the given one.
        /// </summary>
        /// <param name='t'>
        /// T.
        /// </param>
        public virtual void swap(Texture t)
        {
            TextureUnitManager.TEXTURE_UNIT_MANAGER.unbind(this);
            TextureUnitManager.TEXTURE_UNIT_MANAGER.unbind(t);
            if (Program.CURRENT != null)
            {
                uint programId = (uint)Program.CURRENT.getId();
                for (int i = 0; i < programIds.Count; ++i)
                {
                    if (programIds[i] == programId)
                    {
                        Program.CURRENT = null;
                    }
                }
            }
            Debug.Assert(textureTarget == t.textureTarget);
            Std.Swap<uint>(ref textureId, ref t.textureId);
            Std.Swap<TextureInternalFormat>(ref internalFormat, ref t.internalFormat);
            Std.Swap<Parameters>(ref params_, ref t.params_);
        }



        /// <summary>
        /// The OpenGL texture identifier (as returned by glGenTextures).
        /// </summary>
        internal uint textureId;


        /// <summary>
        /// The OpenGL texture target type of this texture (GL_TEXTURE_1D, GL_TEXTURE_2D, etc...).
        /// </summary>
        internal TextureTarget textureTarget;


        /// <summary>
        /// The OpenGL texture internal format (GL_RGB8, GL_RGBA8, etc).
        /// </summary>
        internal TextureInternalFormat internalFormat;


        /// <summary>
        /// The texture parameters.
        /// </summary>
        internal Parameters params_;


        /// <summary>
        /// The OpenGL texture units where this texture is currently bound.
        /// There is one possible binding per sampler object (a texture can be
        /// bound to several units with different sampler objects).
        /// </summary>
        internal IDictionary<uint, uint> currentTextureUnits = new Dictionary<uint, uint>();


        /// <summary>
        /// Identifiers of the programs that use this texture.
        /// </summary>
        internal List<int> programIds = new List<int>();


        /// <summary>
        /// Adds the given program as a user of this texture.
        /// </summary>
        /// <returns>
        /// The user.
        /// </returns>
        /// <param name='programId'>
        /// Program identifier.
        /// </param>
        internal void addUser(int programId)
        {
            Debug.Assert(!isUsedBy(programId));
            programIds.Add(programId);
        }

        /// <summary>
        /// Removes the given program as a user of this texture.
        /// </summary>
        /// <returns>
        /// The user.
        /// </returns>
        /// <param name='programId'>
        /// Program identifier.
        /// </param>
        internal void removeUser(int programId)
        {
            bool contains = programIds.Contains(programId);
            Debug.Assert(contains);
            programIds.Remove(programId);
        }


        /// <summary>
        ///  Returns true if the given program uses this texture.
        /// </summary>
        /// <returns>
        /// The used by.
        /// </returns>
        /// <param name='programId'>
        /// Program identifier.
        /// </param>
        internal bool isUsedBy(int programId)
        {
            return programIds.Contains(programId);
        }


        /// <summary>
        /// Returns the actual maximum number of texture units.
        /// </summary>
        /// <returns>
        /// The max texture units.
        /// </returns>
        private static uint getMaxTextureUnits()
        {
            return TextureUnitManager.getMaxTextureUnits();
        }


        /// <summary>
        /// Unbinds the texture units using the given Sampler.
        /// </summary>
        /// <returns>
        /// The sampler.
        /// </returns>
        /// <param name='sampler'>
        /// Sampler.
        /// </param>
        internal static void unbindSampler(Sampler sampler)
        {
            TextureUnitManager.TEXTURE_UNIT_MANAGER.unbind(sampler);
        }


        /// <summary>
        /// Unbinds all the texture units.
        /// </summary>
        /// <returns>
        /// The all.
        /// </returns>
        internal static void unbindAll()
        {
            TextureUnitManager.TEXTURE_UNIT_MANAGER.unbindAll();
        }

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
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.

                TextureUnitManager.TEXTURE_UNIT_MANAGER.unbind(this);
#if OPENTK
                GL.DeleteTextures(1, ref textureId);
#else
                glDeleteTextures(1, &textureId);
#endif
                Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);

                // Note disposing has been done.
                disposed = true;

            }
        }
    }


    /// <summary>
    /// A texture unit.
    /// </summary>
    internal class TextureUnit
    {


        /// <summary>
        /// Creates a new texture unit.
        /// Initializes a new instance of the <see cref="Sxta.Render.TextureUnit"/> class.
        /// </summary>
        /// <param name='unit'>
        /// Unit. the index of this texture unit.
        /// </param>
        public TextureUnit(uint unit)
        {
            this.unit = unit;
            lastBindingTime = 0;
            currentSamplerBinding = null;
            currentTextureBinding = null;
        }

        /*
         * Binds a sampler and a texture to this texture unit. This unbinds the
         * previously bound sampler and texture.
         *
         * @param sampler the Sampler to bind to this unit.
         * @param tex the Texture to bind to this unit.
         * @param time the current time.
         */
        public void bind(Sampler sampler, Texture tex, uint time)
        {
            lastBindingTime = time; // always update time, or LRU won't work

            uint currentSamplerId = currentSamplerBinding == null ? 0 : currentSamplerBinding.getId();
            uint samplerId = sampler == null ? 0 : sampler.getId();

#if OPENTK
            GL.ActiveTexture((OpenTK.Graphics.OpenGL.TextureUnit)((int)OpenTK.Graphics.OpenGL.TextureUnit.Texture0 + unit));

            if (sampler != currentSamplerBinding)
            {
                GL.BindSampler(unit, samplerId);
                currentSamplerBinding = sampler;
            }
#else
            glActiveTexture(GL_TEXTURE0 + unit);

            if (sampler != currentSamplerBinding)
            {
                glBindSampler(unit, samplerId);
                currentSamplerBinding = sampler;
            }
#endif
            if (tex != currentTextureBinding)
            {
                if (currentTextureBinding != null)
                {
                    bool found = currentTextureBinding.currentTextureUnits.ContainsKey(currentSamplerId);
                    Debug.Assert(found);
                    currentTextureBinding.currentTextureUnits.Remove(currentSamplerId);
                    if (tex == null || currentTextureBinding.textureTarget != tex.textureTarget)
                    {
#if OPENTK
                        GL.BindTexture(currentTextureBinding.textureTarget, 0);
#else
                        glBindTexture(currentTextureBinding.textureTarget, 0);
#endif
                    }
                }
                if (tex != null)
                {
                    tex.currentTextureUnits.Add(samplerId, unit);
#if OPENTK
                    GL.BindTexture(tex.textureTarget, tex.textureId);
#else
                    glBindTexture(tex.textureTarget, tex.textureId);
#endif
                }
                currentTextureBinding = tex;
            }

            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
        }

        public uint getLastBindingTime()
        {
            return lastBindingTime;
        }

        public bool isFree()
        {
            return currentTextureBinding == null;
        }

        public Sampler getCurrentSamplerBinding()
        {
            return currentSamplerBinding;
        }

        public Texture getCurrentTextureBinding()
        {
            return currentTextureBinding;
        }


        /// <summary>
        /// The index of this texture unit (between 0 ... MAX_TEXTURE_UNITS).
        /// </summary>
        private uint unit;


        /// <summary>
        /// The 'time' at which the textures currently bound to the OpenGL texture
        /// unit were bound. This time is used to find the last recently bound
        /// texture when a texture must be unbound to bind a new one.
        /// </summary>
        private uint lastBindingTime;

        /// <summary>
        /// The sampler currently bound to this texture unit.
        /// </summary>
        private Sampler currentSamplerBinding;


        /// <summary>
        /// The texture currently bound to this texture unit.
        /// </summary>
        private Texture currentTextureBinding;
    }


    /// <summary>
    /// Manages texture units.
    /// </summary>
    internal class TextureUnitManager
    {

        public static TextureUnitManager instance = null;

        public static TextureUnitManager TEXTURE_UNIT_MANAGER
        {
            get
            {
                if (instance == null)
                    instance = new TextureUnitManager();
                return instance;
            }
        }


        /// <summary>
        /// Creates a new TextureUnitManager.
        /// Initializes a new instance of the <see cref="Sxta.Render.TextureUnitManager"/> class.
        /// </summary>
        private TextureUnitManager()
        {
            uint maxUnits = getMaxTextureUnits();
            for (uint i = 0; i < maxUnits; ++i)
            {
                units[i] = new TextureUnit(i);
            }

            time = 0;
        }


        /// <summary>
        /// Finds a free texture unit and return its index. If no texture unit is
        /// free, evicts the least recently bound texture not used in the given
        /// program.
        /// </summary>
        /// <returns>
        /// The free texture unit. or -1 if all units are used by the given
        ///  program.
        /// </returns>
        /// <param name='programId'>
        /// Program identifier.
        /// </param>
        public int findFreeTextureUnit(int programId)
        {
            // we first try to find an unused texture unit
            for (int i = 0; i < maxUnits; ++i)
            {
                if (units[i].isFree())
                {
                    return i;
                }
            }

            // if all the texture units are used we must unbind a texture to free
            // a texture unit; we choose the least recently used unit that is not
            // used by the current program

            int bestUnit = -1;
            uint oldestBindingTime = time;

            for (int i = 0; i < maxUnits; ++i)
            {
                Texture t = units[i].getCurrentTextureBinding();
                if (!t.isUsedBy(programId))
                {
                    uint bindingTime = units[i].getLastBindingTime();
                    if (bestUnit == -1 || bindingTime < oldestBindingTime)
                    {
                        bestUnit = i;
                        oldestBindingTime = bindingTime;
                    }
                }
            }

            // if you fail here, there is no more texture unit available
            Debug.Assert(bestUnit != -1);

            return bestUnit;
        }


        /// <summary>
        /// Binds a sampler and a texture to a given texture unit.
        /// </summary>
        /// <param name='i'>
        /// I.
        /// </param>
        /// <param name='sampler'>
        /// Sampler.
        /// </param>
        /// <param name='tex'>
        /// Tex.
        /// </param>
        public void bind(uint i, Sampler sampler, Texture tex)
        {
            units[i].bind(sampler, tex, time++);
        }

        public void unbind(Texture tex)
        {
            for (uint i = 0; i < maxUnits; ++i)
            {
                if (units[i].getCurrentTextureBinding() == tex)
                {
                    units[i].bind(null, null, time++);
                }
            }
        }

        public void unbind(Sampler sampler)
        {
            for (uint i = 0; i < maxUnits; ++i)
            {
                if (units[i].getCurrentSamplerBinding() == sampler)
                {
                    units[i].bind(null, null, time++);
                }
            }
        }

        public void unbindAll()
        {
            for (uint i = 0; i < maxUnits; ++i)
            {
                units[i].bind(null, null, 0);
            }
            time = 0;
        }

        public static uint getMaxTextureUnits()
        {
            if (maxUnits == 0)
            {
                int maxVertexTextureImageUnits;
                int maxFragmentTextureImageUnits;
                int maxCombinedTextureImageUnits;

#if OPENTK
                GL.GetInteger(GetPName.MaxVertexTextureImageUnits, out maxVertexTextureImageUnits);
                GL.GetInteger(GetPName.MaxTextureImageUnits, out maxFragmentTextureImageUnits);
                GL.GetInteger(GetPName.MaxCombinedTextureImageUnits, out maxCombinedTextureImageUnits);
#else
                glGetIntegerv(GL_MAX_VERTEX_TEXTURE_IMAGE_UNITS, &maxVertexTextureImageUnits);
                glGetIntegerv(GL_MAX_TEXTURE_IMAGE_UNITS, &maxFragmentTextureImageUnits);
                glGetIntegerv(GL_MAX_COMBINED_TEXTURE_IMAGE_UNITS, &maxCombinedTextureImageUnits);
#endif
                // Sxta.Render does not support more than MAX_TEXTURE_UNITS units,
                // Because this state is stored in static array of size MAX_TEXTURE_UNITS.
                maxUnits = System.Math.Min(maxCombinedTextureImageUnits, Texture.MAX_TEXTURE_UNITS);

                log.Debug("MAX_VERTEX_TEXTURE_IMAGE_UNITS = " + maxVertexTextureImageUnits);
                log.Debug("MAX_FRAGEMNT_TEXTURE_IMAGE_UNITS = " + maxFragmentTextureImageUnits);
                log.Debug("MAX_COMBINED_TEXTURE_IMAGE_UNITS = " + maxCombinedTextureImageUnits);
            }
            return (uint)maxUnits;
        }



        /// <summary>
        /// The state of all texture units maxUnits elements.
        /// </summary>
        private TextureUnit[] units = new TextureUnit[Texture.MAX_TEXTURE_UNITS];


        /// <summary>
        /// The 'time' used to measure the texture binding times. This abstract time
        /// is an integer that is incremented each time a texture is bound.
        /// </summary>
        private uint time;


        /// <summary>
        /// Maximum number of texture units on the current graphics card.
        /// </summary>
        private static int maxUnits = 0;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
