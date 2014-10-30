using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Sxta.Render
{
    /// <summary>
    /// A Buffer whose data is on the CPU.
    /// </summary>
    public class CPUBuffer<T> : Buffer where T : struct
    {
		/// <summary>
		/// Creates a new CPU buffer with the given data.
		/// Initializes a new instance of the <see cref="Sxta.Render.CPUBuffer`1"/> class.
		/// </summary>
		/// <param name='data'>
		/// Data  the buffer data. May be NULL.
		/// </param>
        public CPUBuffer(T[] data = null)
        {
            p = data;
        }

        /*
         * Destroys this CPU buffer. The buffer data itself is NOT destroyed.
         */
        // public virtual ~CPUBuffer() { }

		/// <summary>
		///  Binds this buffer to the given target. 
		/// </summary>
		/// <param name='target'>
		///  Target an OpenGL buffer target (GL_ARRAY_BUFFER, etc). 
		/// </param>
        internal override void bind(BufferTarget target)
        {
#if OPENTK
            GL.BindBuffer(target, 0);
            Debug.Assert(GL.GetError() == ErrorCode.NoError);
#else
            glBindBuffer(target, 0);
            assert(FrameBuffer::getError() == GL_NO_ERROR);
#endif
        }


        internal override IntPtr data(int offset)
        {
            //return (void*)((char*)p + offset);
#if TODO
            return new ArraySegment<T>(p, offset, p.Length);
 
#endif
            throw new NotImplementedException();
        }

        internal override void unbind(BufferTarget target) { }


		/// <summary>
		/// The p. The buffer data. May be NULL.
		/// </summary>
        private T[] p;
    }
}
