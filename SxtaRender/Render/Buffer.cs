using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sxta.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxta.Render
{
    /// <summary>
    /// An abstract data buffer.
    /// </summary>
    public abstract class Buffer
    {

	/// <summary>
    /// A Buffer layout in client memory for transferring pixels to or from GPU.
    /// </summary>
	public class Parameters
        {

			/// <summary>
			/// Creates a new buffer layout with default parameter values.
			/// Initializes a new instance of the <see cref="Sxta.Render.Buffer+Parameters"/> class.
			/// </summary>
            public Parameters()
            {
                _swapBytes = false;
                _leastSignificantBitFirst = false;
                _alignment = 4;
                _compressedSize = 0;
                _subImage2D = new Vector3i(0, 0, 0);
                _subImage3D = new Vector2i(0, 0);
                modified = false;

            }

			/// <summary>
			/// Swaps the bytes.
			///  Returns true if little endian mode is used. Default is false.
			/// </summary>
			/// <returns>
			/// The bytes.
			/// </returns>
            public bool swapBytes()
            {
                return _swapBytes;
            }


			/// <summary>
			/// Leasts the significant bit first.
			/// Returns true if least significant bit is first. Default is false.
			/// </summary>
			/// <returns>
			/// The significant bit first.
			/// </returns>
            public bool leastSignificantBitFirst()
            {
                return _leastSignificantBitFirst;
            }


			/// <summary>
			/// Alignment this instance.
			/// Returns the alignment of data in memory. Default is 4 bytes.
			/// </summary>
            public int alignment()
            {
                return _alignment;
            }


            /// <summary>
            /// Compresseds the size.
			/// Returns the compressed size in bytes of the pixels.
            /// Default is 0, meaning that the buffer contains uncompressed data.
            /// </summary>
            /// <returns>
            /// The size.
            /// </returns>
			public int compressedSize()
            {
                return _compressedSize;
            }


			/// <summary>
			/// Subs the image2 d.
			/// Returns the 2D subpart of the buffer that must be used for
            /// transferring pixels. Default is whole buffer.
			/// </summary>
			/// <returns>
			/// A vector containing the x and y origin of the subpart to
            /// transfer, and the total width of the image.
			/// The image2 d.
			/// </returns>
            public Vector3i subImage2D()
            {
                return _subImage2D;
            }

			/// <summary>
			/// Returns the 3D subpart of the buffer that must be used for
            /// transferring pixels. Default is whole buffer.
			/// Subs the image3 d.
			/// </summary>
			/// <returns>
			/// A vector containing the z origin of the subpart to
            /// transfer, and the total height of the image.
			/// </returns>
            public Vector2i subImage3D()
            {
                return _subImage3D;
            }


       
			/// <summary>
			/// Sets the endianness mode to use.
			/// Swaps the bytes.
			/// </summary>
			/// <returns>
			/// The bytes.
			/// </returns>
			/// <param name='swapBytes'>
			/// Swap bytestrue to use little endian mode.
			/// </param>
            public Parameters swapBytes(bool swapBytes)
            {
                _swapBytes = swapBytes;
                modified = true;
                return this;
            }

			/// <summary>
			/// Sets the bit ordering mode to use.
			/// </summary>
			/// <returns>
			/// The significant bit first.
			/// </returns>
			/// <param name='leastSignificantBitFirst'>
			/// leastSignificantBitFirst true if least significant bit is first.
			/// </param>
            public Parameters leastSignificantBitFirst(bool leastSignificantBitFirst)
            {
                _leastSignificantBitFirst = leastSignificantBitFirst;
                modified = true;
                return this;
            }

			/// <summary>
			/// Sets the alignment of data in memory.
			/// </summary>
			/// <param name='alignment'>
			/// Alignment the alignment of data in memory.
			/// </param>
            public Parameters alignment(int alignment)
            {
                _alignment = alignment;
                modified = true;
                return this;
            }

			/// <summary>
			/// Returns the size in bytes of the compressed pixels.
			/// </summary>
			/// <returns>
			/// The size.
			/// </returns>
			/// <param name='compressedSize'>
			/// CompressedSize the size in bytes of the compressed pixels,
            ///   or 0 if the buffer data is not compressed.
			/// </param>
            public Parameters compressedSize(int compressedSize)
            {
                _compressedSize = compressedSize;
                // do NOT set modified to true!
                return this;
            }

			/// <summary>
			/// Sets the 2D subpart of the buffer that must be used for
            /// transferring pixels.
			/// </summary>
			/// <returns>
			/// The image2 d.
			/// </returns>
			/// <param name='skipPixels'>
			/// Skip pixels the x origin of the subpart to transfer.
			/// </param>
			/// <param name='skipRows'>
			/// Skip rows the y origin of the subpart to transfer.
			/// </param>
			/// <param name='rowLength'>
			/// Row length the total width of the image.
			/// </param>
            public Parameters subImage2D(int skipPixels, int skipRows, int rowLength)
            {
                _subImage2D = new Vector3i(skipPixels, skipRows, rowLength);
                modified = true;
                return this;
            }

     
			/// <summary>
			/// Sets the 3D subpart of the buffer that must be used for
            /// transferring pixels.
			/// </summary>
			/// <returns>
			/// The image3 d.
			/// </returns>
			/// <param name='skipImages'>
			/// Skip images the z origin of the subpart to transfer.
			/// </param>
			/// <param name='imageHeight'>
			/// Image height, the total height of the image.
			/// </param>
            public Parameters subImage3D(int skipImages, int imageHeight)
            {
                _subImage3D = new Vector2i(skipImages, imageHeight);
                modified = true;
                return this;
            }



            private bool _swapBytes;

            private bool _leastSignificantBitFirst;

            private int _alignment;

            private int _compressedSize;

            private Vector3i _subImage2D;

            private Vector2i _subImage3D;

			/// <summary>
			/// True if the parameters are not equal to their default values.
			/// </summary>
            private bool modified;

       
			/// <summary>
			/// Sets the OpenGL state corresponding to these parameters.
			/// </summary>
            internal void set()
            {
                if (modified)
                {
#if OPENTK
                    GL.PixelStore(PixelStoreParameter.UnpackSwapBytes, (_swapBytes ? 1 : 0));
                    GL.PixelStore(PixelStoreParameter.UnpackLsbFirst, (_leastSignificantBitFirst ? 1 : 0));
                    GL.PixelStore(PixelStoreParameter.UnpackAlignment, _alignment);
                    GL.PixelStore(PixelStoreParameter.UnpackSkipPixels, _subImage2D.X);
                    GL.PixelStore(PixelStoreParameter.UnpackSkipRows, _subImage2D.Y);
                    GL.PixelStore(PixelStoreParameter.UnpackRowLength, _subImage2D.Z);
                    GL.PixelStore(PixelStoreParameter.UnpackSkipImages, _subImage3D.X);
                    GL.PixelStore(PixelStoreParameter.UnpackImageHeight, _subImage3D.Y);
#else
                    glPixelStorei(GL_UNPACK_SWAP_BYTES, _swapBytes);
                    glPixelStorei(GL_UNPACK_LSB_FIRST, _leastSignificantBitFirst);
                    glPixelStorei(GL_UNPACK_ALIGNMENT, _alignment);
                    glPixelStorei(GL_UNPACK_SKIP_PIXELS, _subImage2D.x);
                    glPixelStorei(GL_UNPACK_SKIP_ROWS, _subImage2D.y);
                    glPixelStorei(GL_UNPACK_ROW_LENGTH, _subImage2D.z);
                    glPixelStorei(GL_UNPACK_SKIP_IMAGES, _subImage3D.x);
                    glPixelStorei(GL_UNPACK_IMAGE_HEIGHT, _subImage3D.y);   
#endif
                }
            }


			/// <summary>
			/// Sets the default OpenGL state corresponding to these parameters.
			/// </summary>
            internal void unset()
            {
                if (modified)
                {
#if OPENTK
                    GL.PixelStore(PixelStoreParameter.UnpackSwapBytes, 0);
                    GL.PixelStore(PixelStoreParameter.UnpackLsbFirst, 0);
                    GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);
                    GL.PixelStore(PixelStoreParameter.UnpackSkipPixels, 0);
                    GL.PixelStore(PixelStoreParameter.UnpackSkipRows, 0);
                    GL.PixelStore(PixelStoreParameter.UnpackRowLength, 0);
                    GL.PixelStore(PixelStoreParameter.UnpackSkipImages, 0);
                    GL.PixelStore(PixelStoreParameter.UnpackImageHeight, 0);

#else
                    glPixelStorei(GL_UNPACK_SWAP_BYTES, false);
                    glPixelStorei(GL_UNPACK_LSB_FIRST, false);
                    glPixelStorei(GL_UNPACK_ALIGNMENT, 4);
                    glPixelStorei(GL_UNPACK_SKIP_PIXELS, 0);
                    glPixelStorei(GL_UNPACK_SKIP_ROWS, 0);
                    glPixelStorei(GL_UNPACK_ROW_LENGTH, 0);
                    glPixelStorei(GL_UNPACK_SKIP_IMAGES, 0);
                    glPixelStorei(GL_UNPACK_IMAGE_HEIGHT, 0);
#endif
                }
            }
        }

		/// <summary>
		/// Creates a new buffer.
		/// Initializes a new instance of the <see cref="Sxta.Render.Buffer"/> class.
		/// </summary>
        internal Buffer() { }


        
         /// Destroys this buffer.

        // virtual ~Buffer() {}


		/// <summary>
		/// Binds this buffer to the given target.
		/// </summary>
		/// <param name='target'>
		/// Target an OpenGL buffer target (GL_ARRAY_BUFFER, etc).
		/// </param>
        internal abstract void bind(BufferTarget target);

		/// <summary>
		/// Returns a pointer to the given offset in this data buffer.
		/// </summary>
		/// <param name='offset'>
		/// Offset an offset from the start of this buffer, in bytes.
		/// </param>
        internal abstract IntPtr data(int offset);

		/// <summary>
		/// Unbinds this buffer from the given target.
		/// </summary>
		/// <param name='target'>
		/// Target an OpenGL buffer target (GL_ARRAY_BUFFER, etc).
		/// </param>
        internal abstract void unbind(BufferTarget target);
    }
}
