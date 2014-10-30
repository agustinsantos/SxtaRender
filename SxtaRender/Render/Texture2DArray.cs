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
    /// A 2D array texture.
    /// </summary>
    public class Texture2DArray : Texture
    {
       
		/// <summary>
		/// Creates a new 2D texture array.
		/// Initializes a new instance of the <see cref="Sxta.Render.Texture2DArray"/> class.
		/// </summary>
		/// <param name='w'>
		/// W. the width of this texture in pixels.
		/// </param>
		/// <param name='h'>
		/// H. h the height of this texture in pixels.
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
		/// S.  optional pixel storage parameters for 'pixels'.
		/// </param>
		/// <param name='pixels'>
		/// Pixels. the pixels to be written into this texture.
		/// </param>
        public Texture2DArray(int w, int h, int l, TextureInternalFormat tf, TextureFormat f, PixelType t,
                                  Parameters @params, Buffer.Parameters s, Buffer pixels)
            : this()
        {
            init(w, h, l, tf, f, t, @params, s, pixels);
        }


        /*
         * Destroys this 2D texture.
         */
        // ~Texture2DArray();

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
		/// Level  the LOD level to be changed.
		/// </param>
		/// <param name='x'>
		/// X. lower left corner of the part to be replaced, in pixels.
		/// </param>
		/// <param name='y'>
		/// Y. lower left corner of the part to be replaced, in pixels.
		/// </param>
		/// <param name='l'>
		/// L. first layer of the part to be replaced.
		/// </param>
		/// <param name='w'>
		/// W. the width of the part to be replaced, in pixels.
		/// </param>
		/// <param name='h'>
		/// H. the height of the part to be replaced, in pixels.
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
		/// S. optional pixel storage parameters for 'pixels'.
		/// </param>
		/// <param name='pixels'>
		/// Pixels. the pixels to be written into this texture LOD level.
		/// </param>
        public void setSubImage(int level, int x, int y, int l, int w, int h, int d, TextureFormat f, PixelType t, Buffer.Parameters s, Buffer pixels)
        {
            bindToTextureUnit();
            pixels.bind(BufferTarget.PixelUnpackBuffer);
            s.set();

#if OPENTK
#if TODO
            GL.TexSubImage3D(TextureTarget.ProxyTexture2DArray, level, x, y, l, w, h, d, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
#endif
            throw new NotImplementedException();
#else
            glTexSubImage3D(GL_TEXTURE_2D_ARRAY, level, x, y, l, w, h, d, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
#endif

            s.unset();
            pixels.unbind(BufferTarget.PixelUnpackBuffer);
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);

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
		/// <param name='l'>
		/// L. first layer of the part to be replaced.
		/// </param>
		/// <param name='w'>
		/// W. the width of the part to be replaced, in pixels.
		/// </param>
		/// <param name='h'>
		/// H. the height of the part to be replaced, in pixels.
		/// </param>
		/// <param name='d'>
		/// D.  number of layers of the part to be replaced.
		/// </param>
		/// <param name='s'>
		/// S. the size of 'pixels' in bytes.
		/// </param>
		/// <param name='pixels'>
		/// Pixels. the pixels to be written into this texture LOD level.
		/// </param>
        public void setCompressedSubImage(int level, int x, int y, int l, int w, int h, int d, int s, Buffer pixels)
        {

            bindToTextureUnit();
            pixels.bind(BufferTarget.PixelUnpackBuffer);
#if OPENTK
#if TODO
            GL.CompressedTexSubImage3D(TextureTarget.ProxyTexture2DArray, level, x, y, l, w, h, d, EnumConversion.getTextureInternalFormat(internalFormat), s, pixels.data(0));
#endif
            throw new NotImplementedException();
#else
            glCompressedTexSubImage3D(GL_TEXTURE_2D_ARRAY, level, x, y, l, w, h, d, EnumConversion.getTextureInternalFormat(internalFormat), s, pixels.data(0));
#endif
            pixels.unbind(BufferTarget.PixelUnpackBuffer);
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
        }



        /// <summary>
        /// Returns the maximum number of layers for array textures.
        /// </summary>
        /// <returns></returns>
        public static int getMaxLayers()
        {
            int l;
#if OPENTK
            GL.GetInteger(GetPName.MaxArrayTextureLayers, out l);
#else
            glGetIntegerv(GL_MAX_ARRAY_TEXTURE_LAYERS, &l);
#endif
            return l;
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
		/// The number of this layers of this texture.
		/// </summary>
        protected int l;

        
		/// <summary>
		/// Creates a new unitialized 2DArray texture.
		/// Initializes a new instance of the <see cref="Sxta.Render.Texture2DArray"/> class.
		/// </summary>
        protected Texture2DArray() : base("Texture2DArray", TextureTarget.Texture2DArray) { }

     
		/// <summary>
		/// Initializes this texture.
		/// Init the specified w, h, l, tf, f, t, params, s and pixels.
		/// </summary>
		/// <param name='w'>
		/// W. the width of this texture in pixels.
		/// </param>
		/// <param name='h'>
		/// H. the height of this texture in pixels.
		/// </param>
		/// <param name='l'>
		/// L.  the number of layers of this texture.
		/// </param>
		/// <param name='tf'>
		/// Tf. texture data format on GPU.
		/// </param>
		/// <param name='f'>
		/// F.the texture components in 'pixels'.
		/// </param>
		/// <param name='t'>
		/// T. the type of each component in 'pixels'.
		/// </param>
		/// <param name='params'>
		/// Parameters.  optional additional texture parameters.
		/// </param>
		/// <param name='s'>
		/// S.  optional pixel storage parameters for 'pixels'.
		/// </param>
		/// <param name='pixels'>
		/// Pixels. the pixels to be written into this texture.
		/// </param>
        public void init(int w, int h, int l, TextureInternalFormat tf, TextureFormat f, PixelType t,
                              Parameters @params, Buffer.Parameters s, Buffer pixels)
        {
            base.init(tf, @params);
            this.w = w;
            this.h = h;
            this.l = l;
            pixels.bind(BufferTarget.PixelUnpackBuffer);
#if TODO
            if (isCompressed() && s.compressedSize() > 0)
            {
#if OPENTK
                GL.CompressedTexImage3D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, l, 0, s.compressedSize(), pixels.data(0));
#else
                glCompressedTexImage3D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, l, 0, s.compressedSize(), pixels.data(0));
#endif
            }
            else
            {
                s.set();
#if OPENTK
                GL.TexImage3D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, l, 0, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
#else
                glTexImage3D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, l, 0, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
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
            Std.Swap<int>(ref l, ref ((Texture2DArray)t).l);
        }

    }
}
