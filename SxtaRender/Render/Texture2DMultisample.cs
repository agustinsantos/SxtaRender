using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Sxta.Render
{
    /// <summary>
    /// A 2D texture with multiple samples per pixel.
    /// </summary>
    public class Texture2DMultisample : Texture
    {
		/// <summary>
		/// Creates a new 2D texture with multiple samples per pixel.
		/// Initializes a new instance of the <see cref="Sxta.Render.Texture2DMultisample"/> class.
		/// </summary>
		/// <param name='w'>
		/// W. the width of this texture in pixels.
		/// </param>
		/// <param name='h'>
		/// H. the height of this texture in pixels.
		/// </param>
		/// <param name='samples'>
		/// Samples. the number of samples per pixel.
		/// </param>
		/// <param name='tf'>
		/// Tf. texture data format on GPU.
		/// </param>
		/// <param name='fixedLocations'>
		/// Fixed locations. true to use fixed sample locations for all pixels.
		/// </param>
        public Texture2DMultisample(int w, int h, int samples, TextureInternalFormat tf, bool fixedLocations)
            : base("Texture2DMultisample", TextureTarget.Texture2DMultisample)
        {
#if TODO
#if OPENTK
            GL.GenTextures(1, out textureId);
#else
            glGenTextures(1, &textureId);
#endif

            Debug.Assert(textureId > 0);

            this.internalFormat = tf;
            this.w = w;
            this.h = h;
            this.samples = samples;

            bindToTextureUnit();

#if OPENTK
            GL.TexImage2DMultisample(this.textureTarget, samples, EnumConversion.getTextureInternalFormat(tf), w, h, fixedLocations);
#else
            glTexImage2DMultisample(this.textureTarget, samples, EnumConversion.getTextureInternalFormat(tf), w, h, fixedLocations);
#endif
            if (FrameBuffer.getError() != ErrorCode.NoError)
            {
                throw new Exception();
            }  
#endif
            throw new NotImplementedException();
        }

        /*
         * Destroys this 2D texture with multiple samples per pixel.
         */
        //~Texture2DMultisample();

       
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
		/// Returns the number of samples per pixel of this texture.
		/// </summary>
		/// <returns>
		/// The samples.
		/// </returns>
        public int getSamples()
        {
            return samples;
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
		/// The number of samples per pixel of this texture.
		/// </summary>
        protected int samples;
    }
}
