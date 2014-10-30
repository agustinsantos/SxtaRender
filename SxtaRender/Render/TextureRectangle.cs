using OpenTK.Graphics.OpenGL;
using Sxta.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Sxta.Render
{
    /// <summary>
    ///  A rectangle texture.
    /// </summary>
    public class TextureRectangle : Texture
    {
        
		/// <summary>
		/// Creates a new rectangle texture.
		/// Initializes a new instance of the <see cref="Sxta.Render.TextureRectangle"/> class.
		/// </summary>
		/// <param name='w'>
		/// W the width of this texture in pixels.
		/// </param>
		/// <param name='h'>
		/// H the height of this texture in pixels.
		/// </param>
		/// <param name='tf'>
		/// Tf texture data format on GPU.
		/// </param>
		/// <param name='f'>
		/// F the texture components in 'pixels'.
		/// </param>
		/// <param name='t'>
		/// T the type of each component in 'pixels'.
		/// </param>
		/// <param name='params'>
		/// Parameters optional additional texture parameters.
		/// </param>
		/// <param name='s'>
		/// S optional pixel storage parameters for 'pixels'.
		/// </param>
		/// <param name='pixels'>
		/// Pixels the pixels to be written into this texture.
		/// </param>
        public TextureRectangle(int w, int h, TextureInternalFormat tf, TextureFormat f, PixelType t,
            Parameters @params, Buffer.Parameters s, Buffer pixels)
            : this()
        {
            init(w, h, tf, f, t, @params, s, pixels);
        }

        /*
         * Destroys this rectangle texture.
         */
        //~TextureRectangle();

       
		/// <summary>
		/// Returns the width of this texture.
		/// </summary>
		/// <returns>
		/// The width.
		/// </returns>
        public virtual int getWidth()
        {
            return w;
        }

       
		/// <summary>
		/// Returns the height of this texture.
		/// </summary>
		/// <returns>
		/// The height.
		/// </returns>
        public virtual int getHeight()
        {
            return h;
        }

      
		/// <summary>
		/// Replaces a part of the content of this texture.
		/// </summary>
		/// <returns>
		/// The sub image.
		/// </returns>
		/// <param name='level'>
		/// Level the LOD level to be changed.
		/// </param>
		/// <param name='x'>
		/// X lower left corner of the part to be replaced, in pixels.
		/// </param>
		/// <param name='y'>
		/// Y lower left corner of the part to be replaced, in pixels.
		/// </param>
		/// <param name='w'>
		/// W the width of the part to be replaced, in pixels.
		/// </param>
		/// <param name='h'>
		/// H the height of the part to be replaced, in pixels.
		/// </param>
		/// <param name='f'>
		/// F the texture components in 'pixels'.
		/// </param>
		/// <param name='t'>
		/// T the type of each component in 'pixels'.
		/// </param>
		/// <param name='s'>
		/// S optional pixel storage parameters for 'pixels'.
		/// </param>
		/// <param name='pixels'>
		/// Pixels the pixels to be written into this texture LOD level.
		/// </param>
        public virtual void setSubImage(int level, int x, int y, int w, int h, TextureFormat f, PixelType t, Buffer.Parameters s, Buffer pixels)
        {
#if TODO
            bindToTextureUnit();
            pixels.bind(BufferTarget.PixelUnpackBuffer);
            s.set();
#if OPENTK
            GL.TexSubImage2D(this.textureTarget, level, x, y, w, h, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
#else
            glTexSubImage2D(GL_TEXTURE_RECTANGLE, level, x, y, w, h, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
#endif
            s.unset();
            pixels.unbind(BufferTarget.PixelUnpackBuffer);
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
#endif
            throw new NotImplementedException();
        }


		/// <summary>
		/// Replaces a part of the content of this texture.
		/// </summary>
		/// <returns>
		/// The compressed sub image.
		/// </returns>
		/// <param name='level'>
		/// Level the LOD level to be changed.
		/// </param>
		/// <param name='x'>
		/// X lower left corner of the part to be replaced, in pixels.
		/// </param>
		/// <param name='y'>
		/// Y lower left corner of the part to be replaced, in pixels.
		/// </param>
		/// <param name='w'>
		/// W the width of the part to be replaced, in pixels.
		/// </param>
		/// <param name='h'>
		/// H the height of the part to be replaced, in pixels.
		/// </param>
		/// <param name='s'>
		/// S the size of 'pixels' in bytes.
		/// </param>
		/// <param name='pixels'>
		/// Pixels the pixels to be written into this texture LOD level.
		/// </param>
        public virtual void setCompressedSubImage(int level, int x, int y, int w, int h, int s, Buffer pixels)
        {
#if TODO
            bindToTextureUnit();
            pixels.bind(BufferTarget.PixelUnpackBuffer);
#if OPENTK
            GL.CompressedTexSubImage2D(this.textureTarget, level, x, y, w, h, EnumConversion.getTextureInternalFormat(internalFormat), s, pixels.data(0));
#else
            glCompressedTexSubImage2D(GL_TEXTURE_RECTANGLE, level, x, y, w, h, EnumConversion.getTextureInternalFormat(internalFormat), s, pixels.data(0));
#endif

            pixels.unbind(BufferTarget.PixelUnpackBuffer);
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);  
#endif
            throw new NotImplementedException();
        }


       
		/// <summary>
		/// The width of this texture.
		/// </summary>
        protected int w;

      
		/// <summary>
		/// The height of this texture.
		/// </summary>
        protected int h;

       
		/// <summary>
		/// Creates a new unitialized rectangle texture.
		/// Initializes a new instance of the <see cref="Sxta.Render.TextureRectangle"/> class.
		/// </summary>
        protected TextureRectangle()
            : base("TextureRectangle", TextureTarget.TextureRectangle)
        {
        }

		/// <summary>
		/// Initializes this texture.
		/// Init the specified w, h, tf, f, t, params, s and pixels.
		/// </summary>
		/// <param name='w'>
		/// W the width of this texture in pixels.
		/// </param>
		/// <param name='h'>
		/// H the height of this texture in pixels.
		/// </param>
		/// <param name='tf'>
		/// Tf texture data format on GPU.
		/// </param>
		/// <param name='f'>
		/// F  the texture components in 'pixels'.
		/// </param>
		/// <param name='t'>
		/// T the type of each component in 'pixels'.
		/// </param>
		/// <param name='params'>
		/// Parameters optional additional texture parameters.
		/// </param>
		/// <param name='s'>
		/// S optional pixel storage parameters for 'pixels'.
		/// </param>
		/// <param name='pixels'>
		/// Pixels the pixels to be written into this texture.
		/// </param>
        public void init(int w, int h, TextureInternalFormat tf, TextureFormat f, PixelType t,
            Parameters @params, Buffer.Parameters s, Buffer pixels)
        {
#if TODO
            base.init(tf, @params);
            this.w = w;
            this.h = h;

            pixels.bind(BufferTarget.PixelUnpackBuffer);
            bool needToGenerateMipmaps = true;
            if (isCompressed() && s.compressedSize() > 0)
            {
#if OPENTK
                GL.CompressedTexImage2D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, 0, s.compressedSize(), pixels.data(0));
#else
                glCompressedTexImage2D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, 0, s.compressedSize(), pixels.data(0));
#endif

            }
            else
            {
                s.set();
#if OPENTK
                GL.TexImage2D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, 0, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
#else
                glTexImage2D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, 0, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
#endif
                s.unset();

                int size = s.compressedSize(); // should work because size is retrieved from file descriptor.
                int pixelSize = EnumConversion.getFormatSize(f, t);
                if (size > w * h * pixelSize)
                {
                    // get the other levels from the same buffer
                    int offset = w * h * pixelSize;
                    int level = 0;
                    int wl = w;
                    int hl = h;
                    while (wl % 2 == 0 && hl % 2 == 0 && size - offset >= (wl * hl / 4) * pixelSize)
                    {
                        level += 1;
                        wl = wl / 2;
                        hl = hl / 2;
#if OPENTK
                        GL.TexImage2D(this.textureTarget, level, EnumConversion.getTextureInternalFormat(internalFormat), wl, hl, 0, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(offset));
#else
                        glTexImage2D(this.textureTarget, level, EnumConversion.getTextureInternalFormat(internalFormat), wl, hl, 0, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(offset));
#endif

                        offset += wl * hl * pixelSize;
                        needToGenerateMipmaps = false;
                    }
                    this.params_.lodMax(MathHelper.Clamp(@params.lodMax(), 0.0f, (float)level));
                }
            }
            pixels.unbind(BufferTarget.PixelUnpackBuffer);

            if (needToGenerateMipmaps)
            {
                generateMipMap();
            }

            if (FrameBuffer.getError() != ErrorCode.NoError)
            {
                throw new Exception();
            }  
#endif
            throw new NotImplementedException();
        }
    }
}
