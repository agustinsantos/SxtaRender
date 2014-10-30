using OpenTK.Graphics.OpenGL;
using Sxta.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxta.Render
{
    /// <summary>
    /// A cube array texture.
    /// </summary>
    public class TextureCubeArray : Texture
    {
        /*
         * 
         *
         * @param w 
         * @param h 
         * @param l 
         * @param tf 
         * @param f 
         * @param t 
         * @param params 
         * @param s 
         * @param 
         * 	
         */
		/// <summary>
		/// Creates a new cube texture.
		/// Initializes a new instance of the <see cref="Sxta.Render.TextureCubeArray"/> class.
		/// </summary>
		/// <param name='w'>
		/// W the width of this texture in pixels.
		/// </param>
		/// <param name='h'>
		/// H the height of this texture in pixels.
		/// </param>
		/// <param name='l'>
		/// L the number of layers of this texture.
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
		/// Pixels
		/// pixels the pixels to be written into this texture. The pixels
        /// must be specified as in a 3D texture, with one face per layer.
        /// The faces must be specified in the following order: POSITIVE_X,
        /// NEGATIVE_X, POSITIVE_Y, NEGATIVE_Y, POSITIVE_Z, and NEGATIVE_Z of
        /// the first cube map layer, then the six faces of the second layer,
        /// in the same order, and so on for all layers.
		/// </param>
        public TextureCubeArray(int w, int h, int l, TextureInternalFormat tf, TextureFormat f, PixelType t,
             Parameters @params, Buffer.Parameters s, Buffer pixels)
            : this()
        {
            init(w, h, l, tf, f, t, @params, s, pixels);
        }


        /*
         * Destroys this cube texture.
         */
        //~TextureCubeArray();

        
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
		/// The width of this texture.
		/// </summary>
        protected int w;

        
		/// <summary>
		/// The height of this texture.
		/// </summary>
        protected int h;

       
		/// <summary>
		/// The number of layers of this texture.
		/// </summary>
        protected int l;

        
		/// <summary>
		/// Creates a new unitialized Cube texture.
		/// Initializes a new instance of the <see cref="Sxta.Render.TextureCubeArray"/> class.
		/// </summary>
        protected TextureCubeArray()
            : base("TextureCube", TextureTarget.TextureCubeMapArray)
        {
        }

        /*
         * 
         *
         * @param w 
         * @param h 
         * @param l 
         * @param tf 
         * @param f 
         * @param t 
         * @param params 
         * @param s 
         * @param pixels the pixels to be written into this texture. The pixels
         *      must be specified as in a 3D texture, with one face per layer.
         *      The faces must be specified in the following order: POSITIVE_X,
         *      NEGATIVE_X, POSITIVE_Y, NEGATIVE_Y, POSITIVE_Z, and NEGATIVE_Z of
         *      the first cube map layer, then the six faces of the second layer,
         *      in the same order, and so on for all layers.
         */
		/// <summary>
		/// Initializes this texture.
		/// Init the specified w, h, l, tf, f, t, params, s and pixels.
		/// </summary>
		/// <param name='w'>
		/// W the width of this texture in pixels.
		/// </param>
		/// <param name='h'>
		/// H the height of this texture in pixels.
		/// </param>
		/// <param name='l'>
		/// L the number of layers of this texture.
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
		/// Pixels the pixels to be written into this texture. The pixels
        /// must be specified as in a 3D texture, with one face per layer.
        /// The faces must be specified in the following order: POSITIVE_X,
        /// NEGATIVE_X, POSITIVE_Y, NEGATIVE_Y, POSITIVE_Z, and NEGATIVE_Z of
        /// the first cube map layer, then the six faces of the second layer,
        /// in the same order, and so on for all layers.
		/// </param>
        public void init(int w, int h, int l, TextureInternalFormat tf, TextureFormat f, PixelType t,
               Parameters @params, Buffer.Parameters s, Buffer pixels)
        {
#if TODO
            base.init(tf, @params);
            this.w = w;
            this.h = h;
            this.l = l;

            pixels.bind(BufferTarget.PixelUnpackBuffer);
            if (isCompressed())
            {
#if OPENTK
                GL.CompressedTexImage3D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, 6 * l, 0, s.compressedSize(), pixels.data(0));
#else
                glCompressedTexImage3D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, 6 * l, 0, s.compressedSize(), pixels.data(0));
#endif
            }
            else
            {
                s.set();
#if OPENTK
                GL.TexImage3D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, 6 * l, 0, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
#else
                glTexImage3D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, 6 * l, 0, EnumConversion.getTextureFormat(f), getPixelType(t), pixels.data(0));
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

        protected virtual void swap(Texture t)
        {
            base.swap(t);
            Std.Swap<int>(ref w, ref ((TextureCubeArray)t).w);
            Std.Swap<int>(ref h, ref ((TextureCubeArray)t).h);
            Std.Swap<int>(ref l, ref ((TextureCubeArray)t).l);
        }

    }
}
