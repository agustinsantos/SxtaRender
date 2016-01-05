using OpenTK.Graphics.OpenGL;
using Sxta.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Sxta.Render
{
    /// <summary>
    /// 
    /// </summary>
    public class Texture1DArray : Texture
    {


		/// <summary>
		/// Creates a new 1D array texture.
		/// Initializes a new instance of the <see cref="Sxta.Render.Texture1DArray"/> class.
		/// </summary>
		/// <param name='w'>
		/// W. the width of this texture in pixels.
		/// </param>
		/// <param name='l'>
		/// L. the number of layers of this texture.
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
        public Texture1DArray(int w, int l, TextureInternalFormat tf, TextureFormat f, PixelType t,
                              Parameters @params, Buffer.Parameters s, Buffer pixels)
            : this()
        {
            init(w, l, tf, f, t, @params, s, pixels);
        }

        /*
         * Destroys this 1D array texture.
         */
        // ~Texture1DArray();

        /*
         * 
         */
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

        public override int Width
        {
            get
            {
                return getWidth();
            }
        }

        public override int Height
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Returns the number of layers of this texture.
        /// </summary>
        /// <returns>
        /// The layers.
        /// </returns>
        public int getLayers()
        {
            return l;
        }


		/// <summary>
		/// Replaces a part of the content of this texture.
		/// </summary>
		/// <returns>
		/// The sub image.
		/// </returns>
		/// <param name='level'>
		/// Level. level the LOD level to be changed.
		/// </param>
		/// <param name='x'>
		/// X. the left border of the part to be replaced, in pixels.
		/// </param>
		/// <param name='l'>
		/// L. first layer of the part to be replaced.
		/// </param>
		/// <param name='w'>
		/// W. the size of the part to be replaced, in pixels.
		/// </param>
		/// <param name='d'>
		/// D. number of layers of the part to be replaced.
		/// </param>
		/// <param name='f'>
		/// F. the texture components in 'pixels'.
		/// </param>
		/// <param name='t'>
		/// T. the type of each component in 'pixels'.
		/// </param>
		/// <param name='s'>
		/// S.  optional pixel storage parameters for 'pixels'.
		/// </param>
		/// <param name='pixels'>
		/// Pixels. the pixels to be written into this texture LOD level.
		/// </param>
        public void setSubImage(int level, int x, int l, int w, int d, TextureFormat f, PixelType t, Buffer.Parameters s, Buffer pixels)
        {
#if TODO
            bindToTextureUnit();
            pixels.bind(BufferTarget.PixelUnpackBuffer);
            s.set();
#if OPENTK
            GL.TexSubImage2D(TextureTarget.Texture2D, level, x, l, w, d, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
#else
            glTexSubImage2D(TextureTarget.Texture2D, level, x, l, w, d, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
#endif
            s.unset();
            pixels.unbind(BufferTarget.PixelUnpackBuffer);
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);  
#endif
            throw new NotImplementedException();
        }

  
		/// <summary>
		///  Replaces a part of the content of this texture.
		/// </summary>
		/// <returns>
		/// The compressed sub image.
		/// </returns>
		/// <param name='level'>
		/// Level. the LOD level to be changed.
		/// </param>
		/// <param name='x'>
		/// X. the left border of the part to be replaced, in pixels.
		/// </param>
		/// <param name='l'>
		/// L. first layer of the part to be replaced.
		/// </param>
		/// <param name='w'>
		/// W.  the size of the part to be replaced, in pixels.
		/// </param>
		/// <param name='d'>
		/// D. number of layers of the part to be replaced.
		/// </param>
		/// <param name='s'>
		/// S. the size of 'pixels' in bytes.
		/// </param>
		/// <param name='pixels'>
		/// Pixels. the pixels to be written into this texture LOD level.
		/// </param>
        public void setCompressedSubImage(int level, int x, int l, int w, int d, int s, Buffer pixels)
        {
#if TODO
            bindToTextureUnit();
            pixels.bind(BufferTarget.PixelUnpackBuffer);
#if OPENTK
            GL.CompressedTexSubImage2D(TextureTarget.Texture2D, level, x, l, w, d, EnumConversion.getTextureInternalFormat(internalFormat), s, pixels.data(0));
#else
            glCompressedTexSubImage2D(TextureTarget.Texture2D, level, x, l, w, d, EnumConversion.getTextureInternalFormat(internalFormat), s, pixels.data(0));
#endif

            pixels.unbind(BufferTarget.PixelUnpackBuffer);
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);  
#endif
            throw new NotImplementedException();
        }


		/// <summary>
		///  The width of this texture.
		/// </summary>
        protected int w;

       
		/// <summary>
		/// The number of this layers of this texture.
		/// </summary>
        protected int l;

        
		/// <summary>
		/// Creates a new unitialized 1D texture.
		/// Initializes a new instance of the <see cref="Sxta.Render.Texture1DArray"/> class.
		/// </summary>
        protected Texture1DArray()
            : base("Texture1DArray", TextureTarget.Texture1DArray)
        {
        }

		/// <summary>
		/// Initializes this texture.
		/// </summary>
		/// <param name='w'>
		/// W. the width of this texture in pixels.
		/// </param>
		/// <param name='l'>
		/// L. the number of layers of this texture.
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
        public void init(int w, int l, TextureInternalFormat tf, TextureFormat f, PixelType t,
                            Parameters @params, Buffer.Parameters s, Buffer pixels)
        {
#if TODO
            base.init(tf, @params);
            this.w = w;
            this.l = l;

            pixels.bind(BufferTarget.PixelUnpackBuffer);
            if (isCompressed() && s.compressedSize() > 0)
            {
#if OPENTK
                GL.CompressedTexImage2D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, l, 0, s.compressedSize(), pixels.data(0));
#else
                glCompressedTexImage2D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, l, 0, s.compressedSize(), pixels.data(0));
#endif

            }
            else
            {
                s.set();
#if OPENTK
                GL.TexImage2D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, l, 0, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
#else
               glTexImage2D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, l, 0, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
#endif
                s.unset();
            }
            pixels.unbind(BufferTarget.PixelUnpackBuffer);

            generateMipMap();

            if (FrameBuffer.getError() != ErrorCode.NoError)
            {
                throw new Exception();
            }  
#endif
            throw new NotImplementedException();
        }

        public override void swap(Texture t)
        {
            base.swap(t);
            Std.Swap<int>(ref w, ref ((Texture1DArray)t).w);
            Std.Swap<int>(ref l, ref ((Texture1DArray)t).l);
        }

    }
}
