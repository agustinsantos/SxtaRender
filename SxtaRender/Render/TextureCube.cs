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
    /// A cube texture.
    /// </summary>
    public class TextureCube : Texture
    {
        /*
         * 
         *
         * @param w t
         * @param h 
         * @param tf 
         * @param f 
         * @param t 
         * @param params
         * @param s 
         * @param pixels 
         */
		/// <summary>
		/// Creates a new cube texture.
		/// Initializes a new instance of the <see cref="Sxta.Render.TextureCube"/> class.
		/// </summary>
		/// <param name='w'>
		/// W he width of this texture in pixels.
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
		/// Parameters  optional additional texture parameters.
		/// </param>
		/// <param name='s'>
		/// S optional pixel storage parameters for 'pixels'.
		/// </param>
		/// <param name='pixels'>
		/// Pixels
		/// the pixels to be written into this texture. The cube
        /// faces must be specified in the following order: POSITIVE_X,
        /// NEGATIVE_X, POSITIVE_Y, NEGATIVE_Y, POSITIVE_Z, NEGATIVE_Z.
		/// </param>
        public TextureCube(int w, int h, TextureInternalFormat tf, TextureFormat f, PixelType t,
            Parameters @params, Buffer.Parameters[] s, Buffer[] pixels)
            : this()
        {
            init(w, h, tf, f, t, @params, s, pixels);
        }


        /*
         * Destroys this cube texture.
         */
        // ~TextureCube();

       
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
        /// Replaces a part of one face of this cube texture.
        /// </summary>
        /// <returns>
        /// The sub image.
        /// </returns>
        /// <param name='cf'>
        /// Cf the cube face to be set.
        /// </param>
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
        public void setSubImage(CubeFace cf, int level, int x, int y, int w, int h, TextureFormat f, PixelType t, Buffer.Parameters s, Buffer pixels)
        {
#if TODO
           bindToTextureUnit();
            pixels.bind(BufferTarget.PixelUnpackBuffer);
            s.set();
#if OPENTK
            GL.TexSubImage2D(EnumConversion.getCubeFace(cf), level, x, y, w, h, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
#else
            glTexSubImage2D(EnumConversion.getCubeFace(cf), level, x, y, w, h, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels.data(0));
#endif
            s.unset();
            pixels.unbind(BufferTarget.PixelUnpackBuffer);
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);  
#endif
            //throw new NotImplementedException();
        }

 
		/// <summary>
		/// Replaces a part of one face of this cube texture.
		/// </summary>
		/// <returns>
		/// The compressed sub image.
		/// </returns>
		/// <param name='cf'>
		/// Cf the cube face to be set.
		/// </param>
		/// <param name='level'>
		/// Level  the LOD level to be changed.
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
        public void setCompressedSubImage(CubeFace cf, int level, int x, int y, int w, int h, int s, Buffer pixels)
        {
#if TODO
            bindToTextureUnit();
            pixels.bind(BufferTarget.PixelUnpackBuffer);
#if OPENTK
            GL.CompressedTexSubImage2D(EnumConversion.getCubeFace(cf), level, x, y, w, h, EnumConversion.getTextureInternalFormat(internalFormat), s, pixels.data(0));
#else
            glCompressedTexSubImage2D(EnumConversion.getCubeFace(cf), level, x, y, w, h, EnumConversion.getTextureInternalFormat(internalFormat), s, pixels.data(0));
#endif
            pixels.unbind(BufferTarget.PixelUnpackBuffer);
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);  
#endif
           
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
		/// Creates a new unitialized Cube texture.
		/// Initializes a new instance of the <see cref="Sxta.Render.TextureCube"/> class.
		/// </summary>
        protected TextureCube() : base("TextureCube", TextureTarget.TextureCubeMap) { }

 
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
		/// Pixels  the pixels to be written into this texture. The cube
        /// faces must be specified in the following order: POSITIVE_X,
        ///  NEGATIVE_X, POSITIVE_Y, NEGATIVE_Y, POSITIVE_Z, NEGATIVE_Z.
		/// </param>
        public void init(int w, int h, TextureInternalFormat tf, TextureFormat f, PixelType t,
            Parameters @params, Buffer.Parameters[] s, Buffer[] pixels)
        {
 #if TODO
            base.init(tf, @params);
            this.w = w;
            this.h = h;


            if (isCompressed())
            {
                for (int i = 0; i < 6; ++i)
                {
                    pixels[i].bind(BufferTarget.PixelUnpackBuffer);
                    if (s[i].compressedSize() > 0)
                    {
#if OPENTK
                        GL.CompressedTexImage2D(FACES[i], 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, 0, s[i].compressedSize(), pixels[i].data(0));
#else
                        glCompressedTexImage2D(FACES[i], 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, 0, s[i].compressedSize(), pixels[i].data(0));
#endif
                    }
                    else
                    {
                        s[i].set();
#if OPENTK
                        GL.TexImage2D(FACES[i], 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, 0, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels[i].data(0));
#else
                        glTexImage2D(FACES[i], 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, 0, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels[i].data(0));
#endif
                        s[i].unset();
                    }
                    pixels[i].unbind(BufferTarget.PixelUnpackBuffer);
                }
            }
            else
            {
                for (int i = 0; i < 6; ++i)
                {
                    pixels[i].bind(BufferTarget.PixelUnpackBuffer);
                    s[i].set();
#if OPENTK
                    GL.TexImage2D(FACES[i], 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, 0, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels[i].data(0));
#else
                    glTexImage2D(FACES[i], 0, EnumConversion.getTextureInternalFormat(internalFormat), w, h, 0, EnumConversion.getTextureFormat(f), EnumConversion.getPixelType(t), pixels[i].data(0));
#endif
                    s[i].unset();
                    pixels[i].unbind(BufferTarget.PixelUnpackBuffer);
                }
            }

            generateMipMap();

            if (FrameBuffer.getError() != ErrorCode.NoError)
            {
                throw new Exception();
            }
#endif
           
        }

        static readonly TextureTarget[] FACES = new TextureTarget[6]{
                                                            TextureTarget.TextureCubeMapPositiveX,
                                                            TextureTarget.TextureCubeMapNegativeX,
                                                            TextureTarget.TextureCubeMapPositiveY,
                                                            TextureTarget.TextureCubeMapNegativeY,
                                                            TextureTarget.TextureCubeMapPositiveZ,
                                                            TextureTarget.TextureCubeMapNegativeZ
                                                        };

        public override void swap(Texture t)
        {
            base.swap(t);
            Std.Swap<int>(ref w, ref ((TextureCube)t).w);
            Std.Swap<int>(ref h, ref ((TextureCube)t).h);
        }
    }
}
