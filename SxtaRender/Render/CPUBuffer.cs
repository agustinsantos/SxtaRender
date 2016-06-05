using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
            if (data != null)
                p = ToByteArray(data);
        }

        /*
         * Destroys this CPU buffer. The buffer data itself is NOT destroyed.
         */
        //  ~CPUBuffer() { Debugger.Break(); }

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
            if (p != null)
                unsafe
                {
                    fixed (byte* ptr = &p[0])
                    {
                        return (IntPtr)(ptr) + offset;
                    }
                }
            else
                return (IntPtr)offset;
        }

        internal override void unbind(BufferTarget target) { }

        /// <summary>
        /// code from http://stackoverflow.com/questions/25311361/copy-array-to-struct-array-as-fast-as-possible-in-c-sharp
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        private static byte[] ToByteArray(T[] source)
        {
            GCHandle handle = GCHandle.Alloc(source, GCHandleType.Pinned);
            try
            {
                IntPtr pointer = handle.AddrOfPinnedObject();
                byte[] destination = new byte[source.Length * Marshal.SizeOf(typeof(T))];
                Marshal.Copy(pointer, destination, 0, destination.Length);
                return destination;
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();
            }
        }

        private static T[] FromByteArray(byte[] source)
        {
            T[] destination = new T[source.Length / Marshal.SizeOf(typeof(T))];
            GCHandle handle = GCHandle.Alloc(destination, GCHandleType.Pinned);
            try
            {
                IntPtr pointer = handle.AddrOfPinnedObject();
                Marshal.Copy(source, 0, pointer, source.Length);
                return destination;
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();
            }
        }
        /// <summary>
        /// The p. The buffer data. May be NULL.
        /// </summary>
        private byte[] p;
    }
}
