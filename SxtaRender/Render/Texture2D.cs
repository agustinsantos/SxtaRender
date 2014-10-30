using OpenTK.Graphics.OpenGL;
using Sxta.Core;
using System;

namespace Sxta.Render
{
    /// <summary>
    /// A 2D texture.
    /// </summary>
    public class Texture2D : Texture
    {
       
		/// <summary>
		/// Creates a new 2D texture.
		/// Initializes a new instance of the <see cref="Sxta.Render.Texture2D"/> class.
		/// </summary>
		/// <param name='w'>
		/// W. the width of this texture in pixels.
		/// </param>
		/// <param name='h'>
		/// H. the height of this texture in pixels.
		/// </param>
		/// <param name='tf'>
		/// Tf. texture data format on GPU.
		/// </param>
		/// <param name='f'>
		/// F. the texture components in 'pixels'.
		/// </param>
		/// <param name='t'>
		/// T. the type of each component in 'pixels'.
		/// </param>
		/// <param name='params'>
		/// Parameters. optional additional texture parameters.
		/// </param>
		/// <param name='s'>
		/// S. optional pixel storage parameters for 'pixels'.
		/// </param>
		/// <param name='pixels'>
		/// Pixels. the pixels to be written into this texture.
		/// </param>
        public Texture2D(int w, int h, TextureInternalFormat tf, TextureFormat f, PixelType t,
                  Parameters @params, Buffer.Parameters s, Buffer pixels)
            : this()
        {
            init(w, h, tf, f, t, @params, s, pixels);
        }

        /*
         * Destroys this 2D texture.
         */
        //~Texture2D();

      
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
		/// Returns the height of this texture.
		/// </summary>
		/// <returns>
		/// The height.
		/// </returns>
        public int getHeight()
        {
            return h;
        }

    
		/// <summary>
		/// Replaces the content of this texture.
		/// </summary>
		/// <returns>
		/// The image.
		/// </returns>
		/// <param name='w'>
		/// W. the width of the new texture content, in pixels.
		/// </param>
		/// <param name='h'>
		/// H. the height of the new texture content, in pixels.
		/// </param>
		/// <param name='f'>
		/// F. the texture components in 'pixels'.
		/// </param>
		/// <param name='t'>
		/// T. the type of each component in 'pixels'.
		/// </param>
		/// <param name='pixels'>
		/// Pixels. the pixels to be written into this texture.
		/// </param>
        public void setImage(int w, int h, TextureFormat f, PixelType t, Buffer pixels)
        {
#if TODO
            this.w = w;
            this.h = h;
            bindToTextureUnit();
            pixels.bind(BufferTarget.PixelUnpackBuffer);
#if OPENTK
            GL.TexImage2D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, 0, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
#else
            glTexImage2D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, 0, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
#endif

            pixels.unbind(BufferTarget.PixelUnpackBuffer);
            generateMipMap();

            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);  
#endif
            throw new NotImplementedException();
        }


		/// <summary>
		/// Replaces a part of the content of this texture.
		/// </summary>
		/// <returns>
		/// The sub image.
		/// </returns>
		/// <param name='level'>
		/// Level. the LOD level to be changed.
		/// </param>
		/// <param name='x'>
		/// X. lower left corner of the part to be replaced, in pixels.
		/// </param>
		/// <param name='y'>
		/// Y. lower left corner of the part to be replaced, in pixels.
		/// </param>
		/// <param name='w'>
		/// W. the width of the part to be replaced, in pixels.
		/// </param>
		/// <param name='h'>
		/// H. the height of the part to be replaced, in pixels.
		/// </param>
		/// <param name='f'>
		/// F. the texture components in 'pixels'.
		/// </param>
		/// <param name='t'>
		/// T. the type of each component in 'pixels'.
		/// </param>
		/// <param name='s'>
		/// S. optional pixel storage parameters for 'pixels'.
		/// </param>
		/// <param name='pixels'>
		/// Pixels. the pixels to be written into this texture LOD level.
		/// </param>
        public void setSubImage(int level, int x, int y, int w, int h, TextureFormat f, PixelType t, Buffer.Parameters s, Buffer pixels)
        {
#if TODO
            bindToTextureUnit();
            pixels.bind(BufferTarget.PixelUnpackBuffer);
            s.set();
#if OPENTK
            GL.TexSubImage2D(this.textureTarget, level, x, y, w, h, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
#else
            glTexSubImage2D(this.textureTarget, level, x, y, w, h, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
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
		/// Level. the LOD level to be changed.
		/// </param>
		/// <param name='x'>
		/// X. lower left corner of the part to be replaced, in pixels.
		/// </param>
		/// <param name='y'>
		/// Y. lower left corner of the part to be replaced, in pixels.
		/// </param>
		/// <param name='w'>
		/// W. the width of the part to be replaced, in pixels.
		/// </param>
		/// <param name='h'>
		/// H. the height of the part to be replaced, in pixels.
		/// </param>
		/// <param name='s'>
		/// S. the size of 'pixels' in bytes.
		/// </param>
		/// <param name='pixels'>
		/// Pixels.  pixels to be written into this texture LOD level.
		/// </param>
        public void setCompressedSubImage(int level, int x, int y, int w, int h, int s, Buffer pixels)
        {
#if TODO
            bindToTextureUnit();
            pixels.bind(BufferTarget.PixelUnpackBuffer);
#if OPENTK
            GL.CompressedTexSubImage2D(this.textureTarget, level, x, y, w, h, EnumConversion.getTextureInternalFormat(internalFormat), s, pixels.data(0));
#else
            glCompressedTexSubImage2D(this.textureTarget, level, x, y, w, h, EnumConversion.getTextureInternalFormat(internalFormat), s, pixels.data(0));
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
		/// Creates a new unitialized 2D texture.
		/// Initializes a new instance of the <see cref="Sxta.Render.Texture2D"/> class.
		/// </summary>
        public Texture2D() : base("Texture2D", TextureTarget.Texture2D) { }

        /*
         * Creates a new unintialized 2D texture.
         *
         * @param target a texture type (2D, 2DArray, etc).
         */
        // NOT IMPLEMENTED protected Texture2D(int target);

    
		/// <summary>
		/// Initializes this texture.
		/// Init the specified w, h, tf, f, t, params, s and pixels.
		/// </summary>
		/// <param name='w'>
		/// W. the width of this texture in pixels.
		/// </param>
		/// <param name='h'>
		/// H. the height of this texture in pixels.
		/// </param>
		/// <param name='tf'>
		/// Tf. texture data format on GPU.
		/// </param>
		/// <param name='f'>
		/// F. the texture components in 'pixels'.
		/// </param>
		/// <param name='t'>
		/// T. the type of each component in 'pixels'.
		/// </param>
		/// <param name='params'>
		/// Parameters. optional additional texture parameters.
		/// </param>
		/// <param name='s'>
		/// S. optional pixel storage parameters for 'pixels'.
		/// </param>
		/// <param name='pixels'>
		/// Pixels. the pixels to be written into this texture.
		/// </param>
        public void init(int w, int h, TextureInternalFormat tf, TextureFormat f, PixelType t,
                            Parameters @params, Buffer.Parameters s, Buffer pixels)
        {
            base.init(tf, @params);

            this.w = w;
            this.h = h;

            pixels.bind(BufferTarget.PixelUnpackBuffer);

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
            }
            pixels.unbind(BufferTarget.PixelUnpackBuffer);

            generateMipMap();

            if (FrameBuffer.getError() != ErrorCode.NoError)
            {
                throw new Exception();
            } 
        }

        public override void swap(Texture t)
        {
            base.swap(t);
            Std.Swap<int>(ref w, ref ((Texture2D)t).w);
            Std.Swap<int>(ref h, ref ((Texture2D)t).h);
        }
    }
}
