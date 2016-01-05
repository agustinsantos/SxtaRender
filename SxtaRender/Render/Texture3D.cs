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
    /// A 3D texture.
    /// </summary>
    public class Texture3D : Texture
    {

   
		/// <summary>
		/// Creates a new 3D texture.
		/// Initializes a new instance of the <see cref="Sxta.Render.Texture3D"/> class.
		/// </summary>
		/// <param name='w'>
		/// W. the width of this texture in pixels.
		/// </param>
		/// <param name='h'>
		/// H. the height of this texture in pixels.
		/// </param>
		/// <param name='d'>
		/// D. the depth of this texture in pixels.
		/// </param>
		/// <param name='tf'>
		/// Tf.  texture data format on GPU.
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
        public Texture3D(int w, int h, int d, TextureInternalFormat tf, TextureFormat f, PixelType t,
                         Parameters @params, Buffer.Parameters s, Buffer pixels)
            : this()
        {
            init(w, h, d, tf, f, t, @params, s, pixels);
        }

        /*
         * Destroys this 3D texture.
         */
        // ~Texture3D();

       
		/// <summary>
		///  Returns the width of this texture.
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

        public override int Width
        {
            get
            {
                return w;
            }
        }

        public override int Height
        {
            get
            {
                return h;
            }
        }

        /// <summary>
        /// Returns the depth of this texture.
        /// </summary>
        /// <returns>
        /// The depth.
        /// </returns>
        public int getDepth()
        {
            return d;
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
		/// X. lower left corner of the part to be replaced, in pixels.
		/// </param>
		/// <param name='y'>
		/// Y. lower left corner of the part to be replaced, in pixels.
		/// </param>
		/// <param name='z'>
		/// Z. lower left corner of the part to be replaced, in pixels.
		/// </param>
		/// <param name='w'>
		/// W. the width of the part to be replaced, in pixels.
		/// </param>
		/// <param name='h'>
		/// H. the height of the part to be replaced, in pixels.
		/// </param>
		/// <param name='d'>
		/// D. the depth of the part to be replaced, in pixels.
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
        public void setSubImage(int level, int x, int y, int z, int w, int h, int d, TextureFormat f, PixelType t, Buffer.Parameters s, Buffer pixels)
        {
#if TODO
            bindToTextureUnit();
            pixels.bind(BufferTarget.PixelUnpackBuffer);
            s.set();
#if OPENTK
            GL.TexSubImage3D(TextureTarget.ProxyTexture3D, level, x, y, z, w, h, d, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
#else
            glTexSubImage3D(GL_TEXTURE_3D, level, x, y, z, w, h, d, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
#endif

            s.unset();
            pixels.unbind(BufferTarget.PixelUnpackBuffer);
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);  
#endif
            //throw new NotImplementedException();
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
		/// X  lower left corner of the part to be replaced, in pixels.
		/// </param>
		/// <param name='y'>
		/// Y lower left corner of the part to be replaced, in pixels.
		/// </param>
		/// <param name='z'>
		/// Z lower left corner of the part to be replaced, in pixels.
		/// </param>
		/// <param name='w'>
		/// W the width of the part to be replaced, in pixels.
		/// </param>
		/// <param name='h'>
		/// H the height of the part to be replaced, in pixels.
		/// </param>
		/// <param name='d'>
		/// D the depth of the part to be replaced, in pixels.
		/// </param>
		/// <param name='s'>
		/// S the size of 'pixels' in bytes.
		/// </param>
		/// <param name='pixels'>
		/// Pixels the pixels to be written into this texture LOD level.
		/// </param>
        public void setCompressedSubImage(int level, int x, int y, int z, int w, int h, int d, int s, Buffer pixels)
        {
#if TODO
            bindToTextureUnit();
            pixels.bind(BufferTarget.PixelUnpackBuffer);
#if OPENTK
            GL.TexSubImage3D(TextureTarget.Texture3D, level, x, y, z, w, h, d, EnumConversion.getTextureInternalFormat(internalFormat), s, pixels.data(0));
#else
            glTexSubImage3D(GL_TEXTURE_3D, level, x, y, z, w, h, d, EnumConversion.getTextureInternalFormat(internalFormat), s, pixels.data(0));
#endif

            pixels.unbind(BufferTarget.PixelUnpackBuffer);
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);  
#endif
           // throw new NotImplementedException();
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
		/// The depth of this texture.
		/// </summary>
        protected int d;

      
		/// <summary>
		/// Creates a new unitialized 3D texture.
		/// Initializes a new instance of the <see cref="Sxta.Render.Texture3D"/> class.
		/// </summary>
        protected Texture3D() : base("Texture3D", TextureTarget.Texture3D) { }

      
		/// <summary>
		/// Initializes this texture.
		/// Init the specified w, h, d, tf, f, t, params, s and pixels.
		/// </summary>
		/// <param name='w'>
		/// W the width of this texture in pixels.
		/// </param>
		/// <param name='h'>
		/// H the height of this texture in pixels.
		/// </param>
		/// <param name='d'>
		/// D the depth of this texture in pixels.
		/// </param>
		/// <param name='tf'>
		/// Tf texture data format on GPU.
		/// </param>
		/// <param name='f'>
		/// F the texture components in 'pixels'.
		/// </param>
		/// <param name='t'>
		/// T  the type of each component in 'pixels'.
		/// </param>
		/// <param name='params'>
		/// Parameters optional additional texture parameters.
		/// </param>
		/// <param name='s'>
		/// S pixel storage parameters for 'pixels'.
		/// </param>
		/// <param name='pixels'>
		/// Pixel the pixels to be written into this texture.
		/// </param>
        public void init(int w, int h, int d, TextureInternalFormat tf, TextureFormat f, PixelType t,
                            Parameters @params, Buffer.Parameters s, Buffer pixels)
        {
#if TODO
            base.init(tf, @params);
            this.w = w;
            this.h = h;
            this.d = d;

            bindToTextureUnit();

            pixels.bind(BufferTarget.PixelUnpackBuffer);
            if (isCompressed() && s.compressedSize() > 0)
            {
#if OPENTK
                GL.CompressedTexImage3D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, d, 0, s.compressedSize(), pixels.data(0));
#else
                glCompressedTexImage3D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, d, 0, s.compressedSize(), pixels.data(0));
#endif

            }
            else
            {
                s.set();
#if OPENTK
                GL.TexImage3D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, d, 0, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
#else
                glTexImage3D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, d, 0, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
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
          //  throw new NotImplementedException();
        }

        public override void swap(Texture t)
        {
            base.swap(t);
            Std.Swap<int>(ref w, ref ((Texture3D)t).w);
            Std.Swap<int>(ref h, ref ((Texture3D)t).h);
            Std.Swap<int>(ref d, ref ((Texture3D)t).d);
        }

    }
}
