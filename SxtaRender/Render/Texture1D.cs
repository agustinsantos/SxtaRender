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
    /// A 1D texture.
    /// </summary>
    public class Texture1D : Texture
    {

        /// <summary>
        /// Creates a new 1D texture.
        /// Initializes a new instance of the <see cref="Sxta.Render.Texture1D"/> class.
        /// </summary>
        /// <param name='w'>
        /// W. the width of this texture in pixels.
        /// </param>
        /// <param name='tf'>
        /// Tf.  texture data format on GPU.
        /// </param>
        /// <param name='f'>
        /// F.  the texture components in 'pixels'.
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
        public Texture1D(int w, TextureInternalFormat tf, TextureFormat f, PixelType t,
                         Parameters @params, Buffer.Parameters s, Buffer pixels)
            : this()
        {
            init(w, tf, f, t, @params, s, pixels);
        }

        /*
         * Destroys this 1D texture.
         */
        //~Texture1D();


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
        /// Replaces a part of the content of this texture.
        /// </summary>
        /// <returns>
        /// The sub image.
        /// </returns>
        /// <param name='level'>
        /// Level. the LOD level to be changed.
        /// </param>
        /// <param name='x'>
        /// X. left border of the part to be replaced, in pixels.
        /// </param>
        /// <param name='w'>
        /// W. size of the part to be replaced, in pixels.
        /// </param>
        /// <param name='f'>
        /// F. texture components in 'pixels'.
        /// </param>
        /// <param name='t'>
        /// T. type of each component in 'pixels'.
        /// </param>
        /// <param name='s'>
        /// S.  optional pixel storage parameters for 'pixels'.
        /// </param>
        /// <param name='pixels'>
        /// Pixels. the pixels to be written into this texture LOD level.
        /// </param>/
        public void setSubImage(int level, int x, int w, TextureFormat f, PixelType t, Buffer.Parameters s, Buffer pixels)
        {
#if TODO
            bindToTextureUnit();
            pixels.bind(BufferTarget.PixelUnpackBuffer);
            s.set();
#if OPENTK
            GL.TexSubImage1D(this.textureTarget, level, x, w, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), ref pixels.data(0));
#else
            glTexSubImage1D(this.textureTarget, level, x, w, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
#endif
            s.unset();
            pixels.unbind(BufferTarget.PixelUnpackBuffer);
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);  
#endif
            throw new NotImplementedException();
        }


        /// <summary>
        /// Replaces a part of the content of this texture.
        /// Sets the compressed sub image.
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
        /// <param name='w'>
        /// W. the size of the part to be replaced, in pixels.
        /// </param>
        /// <param name='s'>
        /// S. the size of 'pixels' in bytes.
        /// </param>
        /// <param name='pixels'>
        /// Pixels.  the pixels to be written into this texture LOD level.
        /// </param>
        public void setCompressedSubImage(int level, int x, int w, int s, Buffer pixels)
        {
#if TODO
            bindToTextureUnit();
            pixels.bind(BufferTarget.PixelUnpackBuffer);
#if OPENTK
            GL.CompressedTexSubImage1D(TextureTarget.Texture1D, level, x, w, EnumConversion.getTextureInternalFormat(internalFormat), s, pixels.data(0));
#else
            glCompressedTexSubImage1D(this.textureTarget, level, x, w, EnumConversion.getTextureInternalFormat(internalFormat), s, pixels.data(0));
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
        /// Creates a new unitialized 1D texture.
        /// Initializes a new instance of the <see cref="Sxta.Render.Texture1D"/> class.
        /// </summary>
        protected Texture1D()
            : base("Texture1D", TextureTarget.Texture1D)
        { }


        /// <summary>
        /// Initializes this texture.
        /// Init the specified w, tf, f, t, params, s and pixels.
        /// </summary>
        /// <param name='w'>
        /// W. the width of this texture in pixels.
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
        public void init(int w, TextureInternalFormat tf, TextureFormat f, PixelType t,
            Parameters @params, Buffer.Parameters s, Buffer pixels)
        {
            base.init(tf, @params);
            this.w = w;

            pixels.bind(BufferTarget.PixelUnpackBuffer);
            if (isCompressed() && s.compressedSize() > 0)
            {
#if OPENTK
                GL.CompressedTexImage1D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, 0, s.compressedSize(), pixels.data(0));
#else
                glCompressedTexImage1D(this.textureTarget, 0, getTextureInternalFormat(internalFormat), w, 0, s.compressedSize(), pixels.data(0));
#endif
            }
            else
            {
                s.set();
#if OPENTK
                GL.TexImage1D(this.textureTarget, 0, EnumConversion.getTextureInternalFormat(internalFormat), w, 0, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
#else
                glTexImage1D(this.textureTarget, 0, getTextureInternalFormat(internalFormat), w, 0, getTextureFormat(f), getPixelType(t), pixels.data(0));
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
            Std.Swap<int>(ref w, ref ((Texture1D)t).w);
        }

    }
}
