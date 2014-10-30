using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Sxta.Render
{
    
	/// <summary>
	/// A buffer texture. A buffer texture is similar to a one-dimensional texture.
    /// However, unlike other texture types, the texel array is not stored as part
    /// of the texture. Instead, a buffer object is attached to a buffer texture
    /// and the texel array is taken from the data store of an attached buffer
    /// object.  When the contents of a buffer object's data store are modified,
    /// those changes are reflected in the contents of any buffer texture to which
    /// the buffer object is attached.  Also unlike other textures, buffer
    /// textures do not have multiple image levels; only a single data store is
    /// available.
	/// </summary>
    public class TextureBuffer : Texture
    {
        
		/// <summary>
		/// Creates a new buffer texture.
		/// Initializes a new instance of the <see cref="Sxta.Render.TextureBuffer"/> class.
		/// </summary>
		/// <param name='tf'>
		/// Tf texture data format in 'pixels'.
		/// </param>
		/// <param name='pixels'>
		/// Pixels the buffer holding the texture's texel array.
		/// </param>
        public TextureBuffer(TextureInternalFormat tf, GPUBuffer pixels)
            : base("TextureBuffer", TextureTarget.TextureBuffer)
        {
            int formatSize;
            switch (tf)
            {
                case TextureInternalFormat.R8:
                case TextureInternalFormat.R8I:
                case TextureInternalFormat.R8UI:
                    formatSize = 1;
                    break;
                case TextureInternalFormat.R16:
                case TextureInternalFormat.R16I:
                case TextureInternalFormat.R16UI:
                case TextureInternalFormat.R16F:
                case TextureInternalFormat.RG8:
                case TextureInternalFormat.RG8I:
                case TextureInternalFormat.RG8UI:
                    formatSize = 2;
                    break;
                case TextureInternalFormat.R32I:
                case TextureInternalFormat.R32UI:
                case TextureInternalFormat.R32F:
                case TextureInternalFormat.RG16:
                case TextureInternalFormat.RG16I:
                case TextureInternalFormat.RG16UI:
                case TextureInternalFormat.RG16F:
                case TextureInternalFormat.RGBA8:
                case TextureInternalFormat.RGBA8I:
                case TextureInternalFormat.RGBA8UI:
                    formatSize = 4;
                    break;
                case TextureInternalFormat.RG32I:
                case TextureInternalFormat.RG32UI:
                case TextureInternalFormat.RG32F:
                case TextureInternalFormat.RGBA16:
                case TextureInternalFormat.RGBA16I:
                case TextureInternalFormat.RGBA16UI:
                case TextureInternalFormat.RGBA16F:
                    formatSize = 8;
                    break;
                case TextureInternalFormat.RGBA32I:
                case TextureInternalFormat.RGBA32UI:
                case TextureInternalFormat.RGBA32F:
                    formatSize = 16;
                    break;
                default:
                    Debug.Assert(false); // other formats not allowed for texture buffers
                    throw new ArgumentException();
            }

            Parameters @params = new Parameters();
            @params.wrapS(TextureWrap.CLAMP_TO_EDGE);
            @params.wrapT(TextureWrap.CLAMP_TO_EDGE);
            @params.wrapR(TextureWrap.CLAMP_TO_EDGE);
            @params.min(TextureFilter.NEAREST);
            @params.mag(TextureFilter.NEAREST);
            @params.maxLevel(0);

            base.init(tf, @params);
            this.w = pixels.getSize() / formatSize;
            this.b = pixels;
#if OPENTK
            GL.TexBuffer(TextureBufferTarget.TextureBuffer, EnumConversion.getSizedInternalFormat(internalFormat), pixels.getId());
#else
            glTexBuffer(textureTarget, getTextureInternalFormat(internalFormat), pixels.getId());
#endif
            if (FrameBuffer.getError() != ErrorCode.NoError)
            {
                throw new Exception();
            }
        }

        /*
         * Destroys this buffer texture.
         */
        //~TextureBuffer() { }

       
		/// <summary>
		/// Returns the width of this texture.
		/// </summary>
		/// <returns>
		/// The width.
		/// </returns>
        public int getWidth()
        {
            return w;
        }

       
		/// <summary>
		/// Returns the buffer holding the texture's texel array.
		/// </summary>
		/// <returns>
		/// The buffer.
		/// </returns>
        public GPUBuffer getBuffer()
        {
            return b;
        }


      
		/// <summary>
		/// The width of this texture.
		/// </summary>
        protected int w;

        
		/// <summary>
		/// The buffer holding the texture's texel array.
		/// </summary>
        protected GPUBuffer b;
    }
}
