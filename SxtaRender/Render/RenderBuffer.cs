using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Sxta.Render
{
    /// <summary>
    /// A buffer for offscreen rendering.
    /// </summary>
    public class RenderBuffer
    {

        

		/// <summary>
    	/// A pixel format for a RenderBuffer.
    	/// </summary>
        public enum RenderBufferFormat
        {
            R8, ///< &nbsp;
            R8_SNORM, ///< &nbsp;
            R16, ///< &nbsp;
            R16_SNORM, ///< &nbsp;
            RG8, ///< &nbsp;
            RG8_SNORM, ///< &nbsp;
            RG16, ///< &nbsp;
            RG16_SNORM, ///< &nbsp;
            R3_G3_B2, ///< &nbsp;
            RGB4, ///< &nbsp;
            RGB5, ///< &nbsp;
            RGB8, ///< &nbsp;
            RGB8_SNORM, ///< &nbsp;
            RGB10, ///< &nbsp;
            RGB12, ///< &nbsp;
            RGB16, ///< &nbsp;
            RGB16_SNORM, ///< &nbsp;
            RGBA2, ///< &nbsp;
            RGBA4, ///< &nbsp;
            RGB5_A1, ///< &nbsp;
            RGBA8, ///< &nbsp;
            RGBA8_SNORM, ///< &nbsp;
            RGB10_A2, ///< &nbsp;
            RGB10_A2UI, ///< &nbsp;
            RGBA12, ///< &nbsp;
            RGBA16, ///< &nbsp;
            RGBA16_SNORM, ///< &nbsp;
            SRGB8, ///< &nbsp;
            SRGB8_ALPHA8, ///< &nbsp;
            R16F, ///< &nbsp;
            RG16F, ///< &nbsp;
            RGB16F, ///< &nbsp;
            RGBA16F, ///< &nbsp;
            R32F, ///< &nbsp;
            RG32F, ///< &nbsp;
            RGB32F, ///< &nbsp;
            RGBA32F, ///< &nbsp;
            R11F_G11F_B10F, ///< &nbsp;
            RGB9_E5, ///< &nbsp;
            R8I, ///< &nbsp;
            R8UI, ///< &nbsp;
            R16I, ///< &nbsp;
            R16UI, ///< &nbsp;
            R32I, ///< &nbsp;
            R32UI, ///< &nbsp;
            RG8I, ///< &nbsp;
            RG8UI, ///< &nbsp;
            RG16I, ///< &nbsp;
            RG16UI, ///< &nbsp;
            RG32I, ///< &nbsp;
            RG32UI, ///< &nbsp;
            RGB8I, ///< &nbsp;
            RGB8UI, ///< &nbsp;
            RGB16I, ///< &nbsp;
            RGB16UI, ///< &nbsp;
            RGB32I, ///< &nbsp;
            RGB32UI, ///< &nbsp;
            RGBA8I, ///< &nbsp;
            RGBA8UI, ///< &nbsp;
            RGBA16I, ///< &nbsp;
            RGBA16UI, ///< &nbsp;
            RGBA32I, ///< &nbsp;
            RGBA32UI, ///< &nbsp;
            DEPTH_COMPONENT, ///< &nbsp;
            DEPTH_COMPONENT16, ///< &nbsp;
            DEPTH_COMPONENT24, ///< &nbsp;
            DEPTH_COMPONENT32, ///< &nbsp;
            DEPTH_COMPONENT32F, ///< &nbsp;
            DEPTH24_STENCIL8, ///< &nbsp;
            DEPTH32F_STENCIL8, ///< &nbsp;
            STENCIL_INDEX, ///< &nbsp;
            STENCIL_INDEX1, ///< &nbsp;
            STENCIL_INDEX4, ///< &nbsp;
            STENCIL_INDEX8, ///< &nbsp;
            STENCIL_INDEX16 ///< &nbsp;
        }


		/// <summary>
		/// Creates a new offscreen render buffer.
		/// Initializes a new instance of the <see cref="Sxta.Render.RenderBuffer"/> class.
		/// </summary>
		/// <param name='type'>
		/// Type the pixel format for this buffer.
		/// </param>
		/// <param name='width'>
		/// Width of the buffer.
		/// </param>
		/// <param name='height'>
		/// Height of the buffer.
		/// </param>
		/// <param name='samples'>
		/// Samples  the number of samples per pixel,or 0 if multisampling is not needed.      
		/// </param>
        public RenderBuffer(RenderBufferFormat type, int width, int height, int samples = 0)
        {
            RenderbufferStorage format = 0;
            switch (type)
            {
                case RenderBufferFormat.R8:
                    format = RenderbufferStorage.R8;
                    break;
                case RenderBufferFormat.R16:
                    format = RenderbufferStorage.R16;
                    break;
                case RenderBufferFormat.R16_SNORM:
#if TODO
                    format = RenderbufferStorage.GL_R16_SNORM;
                    break;  
#endif
                    throw new NotImplementedException();
                case RenderBufferFormat.RG8:
                    format = RenderbufferStorage.Rg8;
                    break;
                case RenderBufferFormat.RG8_SNORM:
#if TODO
                    format = RenderbufferStorage.GL_RG8_SNORM;
                    break;
 
#endif
                    throw new NotImplementedException();
                case RenderBufferFormat.RG16:
                    format = RenderbufferStorage.Rg16;
                    break;
                case RenderBufferFormat.RG16_SNORM:
#if TODO
                    format = RenderbufferStorage.GL_RG16_SNORM;
                    break;
 
#endif
                    throw new NotImplementedException();
                case RenderBufferFormat.R3_G3_B2:
                    format = RenderbufferStorage.R3G3B2;
                    break;
                case RenderBufferFormat.RGB4:
                    format = RenderbufferStorage.Rgb4;
                    break;
                case RenderBufferFormat.RGB5:
                    format = RenderbufferStorage.Rgb5;
                    break;
                case RenderBufferFormat.RGB8:
                    format = RenderbufferStorage.Rgb8;
                    break;
                case RenderBufferFormat.RGB8_SNORM:
#if TODO
                    format = RenderbufferStorage.GL_RGB8_SNORM;
                    break;
 
#endif
                    throw new NotImplementedException();
                case RenderBufferFormat.RGB10:
                    format = RenderbufferStorage.Rgb10;
                    break;
                case RenderBufferFormat.RGB12:
                    format = RenderbufferStorage.Rgb12;
                    break;
                case RenderBufferFormat.RGB16:
                    format = RenderbufferStorage.Rgb16;
                    break;
                case RenderBufferFormat.RGB16_SNORM:
#if TODO
                    format = RenderbufferStorage.GL_RGB16_SNORM;
                    break;
 
#endif
                    throw new NotImplementedException();
                case RenderBufferFormat.RGBA2:
                    format = RenderbufferStorage.Rgba2;
                    break;
                case RenderBufferFormat.RGBA4:
                    format = RenderbufferStorage.Rgba4;
                    break;
                case RenderBufferFormat.RGB5_A1:
#if TODO
                    format = RenderbufferStorage.Rgb5a1;
                    break;
 
#endif
                    throw new NotImplementedException();
                case RenderBufferFormat.RGBA8:
                    format = RenderbufferStorage.Rgba8;
                    break;
                case RenderBufferFormat.RGBA8_SNORM:
#if TODO
                    format = RenderbufferStorage.GL_RGBA8_SNORM;
                    break;
 
#endif
                    throw new NotImplementedException();
                case RenderBufferFormat.RGB10_A2:
                    format = RenderbufferStorage.Rgb10A2;
                    break;
                case RenderBufferFormat.RGB10_A2UI:
                    format = RenderbufferStorage.Rgb10A2ui;
                    break;
                case RenderBufferFormat.RGBA12:
                    format = RenderbufferStorage.Rgba12;
                    break;
                case RenderBufferFormat.RGBA16:
                    format = RenderbufferStorage.Rgba16;
                    break;
                case RenderBufferFormat.RGBA16_SNORM:
#if TODO
                    format = RenderbufferStorage.GL_RGBA16_SNORM;
                    break;
 
#endif
                    throw new NotImplementedException();
                case RenderBufferFormat.SRGB8:
                    format = RenderbufferStorage.Srgb8;
                    break;
                case RenderBufferFormat.SRGB8_ALPHA8:
                    format = RenderbufferStorage.Srgb8Alpha8;
                    break;
                case RenderBufferFormat.R16F:
                    format = RenderbufferStorage.R16f;
                    break;
                case RenderBufferFormat.RG16F:
                    format = RenderbufferStorage.Rg16f;
                    break;
                case RenderBufferFormat.RGB16F:
                    format = RenderbufferStorage.Rgb16f;
                    break;
                case RenderBufferFormat.RGBA16F:
                    format = RenderbufferStorage.Rgb16f;
                    break;
                case RenderBufferFormat.R32F:
                    format = RenderbufferStorage.R32f;
                    break;
                case RenderBufferFormat.RG32F:
                    format = RenderbufferStorage.Rg32f;
                    break;
                case RenderBufferFormat.RGB32F:
                    format = RenderbufferStorage.Rgb32f;
                    break;
                case RenderBufferFormat.RGBA32F:
                    format = RenderbufferStorage.Rgba32f;
                    break;
                case RenderBufferFormat.R11F_G11F_B10F:
                    format = RenderbufferStorage.R11fG11fB10f;
                    break;
                case RenderBufferFormat.RGB9_E5:
                    format = RenderbufferStorage.Rgb9E5;
                    break;
                case RenderBufferFormat.R8I:
                    format = RenderbufferStorage.R8i;
                    break;
                case RenderBufferFormat.R8UI:
                    format = RenderbufferStorage.R8ui;
                    break;
                case RenderBufferFormat.R16I:
                    format = RenderbufferStorage.R16i;
                    break;
                case RenderBufferFormat.R16UI:
                    format = RenderbufferStorage.R16ui;
                    break;
                case RenderBufferFormat.R32I:
                    format = RenderbufferStorage.R32i;
                    break;
                case RenderBufferFormat.R32UI:
                    format = RenderbufferStorage.R32ui;
                    break;
                case RenderBufferFormat.RG8I:
                    format = RenderbufferStorage.Rg8i;
                    break;
                case RenderBufferFormat.RG8UI:
                    format = RenderbufferStorage.Rg8ui;
                    break;
                case RenderBufferFormat.RG16I:
                    format = RenderbufferStorage.Rg16i;
                    break;
                case RenderBufferFormat.RG16UI:
                    format = RenderbufferStorage.Rg16ui;
                    break;
                case RenderBufferFormat.RG32I:
                    format = RenderbufferStorage.Rg32i;
                    break;
                case RenderBufferFormat.RG32UI:
                    format = RenderbufferStorage.Rg32ui;
                    break;
                case RenderBufferFormat.RGB8I:
                    format = RenderbufferStorage.Rg8i;
                    break;
                case RenderBufferFormat.RGB8UI:
                    format = RenderbufferStorage.Rgb8ui;
                    break;
                case RenderBufferFormat.RGB16I:
                    format = RenderbufferStorage.Rgb16i;
                    break;
                case RenderBufferFormat.RGB16UI:
                    format = RenderbufferStorage.Rgb16ui;
                    break;
                case RenderBufferFormat.RGB32I:
                    format = RenderbufferStorage.Rgb32i;
                    break;
                case RenderBufferFormat.RGB32UI:
                    format = RenderbufferStorage.Rgb32ui;
                    break;
                case RenderBufferFormat.RGBA8I:
                    format = RenderbufferStorage.Rgba8i;
                    break;
                case RenderBufferFormat.RGBA8UI:
                    format = RenderbufferStorage.Rgba8ui;
                    break;
                case RenderBufferFormat.RGBA16I:
                    format = RenderbufferStorage.Rgba16i;
                    break;
                case RenderBufferFormat.RGBA16UI:
                    format = RenderbufferStorage.Rgba16ui;
                    break;
                case RenderBufferFormat.RGBA32I:
                    format = RenderbufferStorage.Rgba32i;
                    break;
                case RenderBufferFormat.RGBA32UI:
                    format = RenderbufferStorage.Rgba32ui;
                    break;
                case RenderBufferFormat.DEPTH_COMPONENT:
                    format = RenderbufferStorage.DepthComponent16;
                    break;
                case RenderBufferFormat.DEPTH_COMPONENT16:
                    format = RenderbufferStorage.DepthComponent16;
                    break;
                case RenderBufferFormat.DEPTH_COMPONENT24:
                    format = RenderbufferStorage.DepthComponent24;
                    break;
                case RenderBufferFormat.DEPTH_COMPONENT32:
                    format = RenderbufferStorage.DepthComponent32;
                    break;
                case RenderBufferFormat.DEPTH_COMPONENT32F:
                    format = RenderbufferStorage.DepthComponent32f;
                    break;
                case RenderBufferFormat.DEPTH24_STENCIL8:
                    format = RenderbufferStorage.Depth24Stencil8;
                    break;
                case RenderBufferFormat.DEPTH32F_STENCIL8:
                    format = RenderbufferStorage.Depth32fStencil8;
                    break;
                case RenderBufferFormat.STENCIL_INDEX:
                    format = RenderbufferStorage.StencilIndex1;
                    break;
                case RenderBufferFormat.STENCIL_INDEX1:
                    format = RenderbufferStorage.StencilIndex1;
                    break;
                case RenderBufferFormat.STENCIL_INDEX4:
                    format = RenderbufferStorage.StencilIndex4;
                    break;
                case RenderBufferFormat.STENCIL_INDEX8:
                    format = RenderbufferStorage.StencilIndex8;
                    break;
                case RenderBufferFormat.STENCIL_INDEX16:
                    format = RenderbufferStorage.StencilIndex16;
                    break;
                default:
                    Debug.Assert(false); // unsupported format
                    break;
            }
#if OPENGL
            glGenRenderbuffers(1, &bufferId);
            glBindRenderbuffer(GL_RENDERBUFFER, bufferId);
            if (samples == 0) {
                glRenderbufferStorage(GL_RENDERBUFFER, format, width, height);
            } else {
                glRenderbufferStorageMultisample(GL_RENDERBUFFER, samples, format, width, height);
            }
#else
            GL.GenRenderbuffers(1, out bufferId);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, bufferId);
            if (samples == 0)
            {
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, format, width, height);
            }
            else
            {
                GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, samples, format, width, height);
            }
#endif
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
        }

     
		/// <summary>
		///  Deletes this render buffer.
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="Sxta.Render.RenderBuffer"/> is reclaimed by garbage collection.
		/// </summary>
        ~RenderBuffer()
        {
#if OPENGL
            glDeleteRenderbuffers(1, &bufferId);
#else
            GL.DeleteRenderbuffers(1, ref bufferId);
#endif
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
        }

       
		/// <summary>
		/// Returns the id of this render buffer.
		/// </summary>
		/// <returns>
		/// The identifier.
		/// </returns>
        public uint getId()
        {
            return bufferId;
        }

     
		/// <summary>
		/// The id of this render buffer.
		/// </summary>
        private uint bufferId;
    }
}
