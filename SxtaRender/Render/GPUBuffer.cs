// glMapBuffer and glUnmapBuffer seem inefficient.
// This option avoids theses calls by using a copy of
// the buffer data on CPU.

#define NOT_CUSTOM_MAP_BUFFER

using log4net;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sxta.Render
{
    /// <summary>
    /// A Buffer whose data is on the GPU.
    /// </summary>
    public class GPUBuffer : Buffer, IDisposable //where T : struct
    {
        // For a tutorial about OpenTK buffers, see
        // http://www.opentk.com/doc/graphics/geometry/vertex-buffer-objects
        //

     
		/// <summary>
		/// Creates a new GPU buffer with no associated data.
		/// Initializes a new instance of the <see cref="Sxta.Render.GPUBuffer"/> class.
		/// </summary>
        public GPUBuffer()
        {
            size = 0;
            mappedData = IntPtr.Zero;
            cpuData = null;
            currentUniformUnit = -1;

            if (UNIFORM_BUFFER_MANAGER == null)
            {
                UNIFORM_BUFFER_MANAGER = new UniformBufferManager();
            }
#if OPENTK
            GL.GenBuffers(1, out bufferId);
#else
            glGenBuffers(1, &bufferId);
#endif
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
        }


        internal GPUBuffer(IntPtr ptr)
        {
            throw new NotImplementedException();
        }

      
		/// <summary>
		/// Destroys this GPU buffer. The buffer data itself is also destroyed.
		/// Releases unmanaged resources and performs other cleanup operations before the <see cref="Sxta.Render.GPUBuffer"/>
		/// is reclaimed by garbage collection.
		/// </summary>
        ~GPUBuffer()
        {
            // Do not re-create Dispose clean-up code here. 
            // Calling Dispose(false) is optimal in terms of 
            // readability and maintainability.
            Dispose(false);
        }


		/// <summary>
		/// Returns the id of this buffer.
		/// </summary>
		/// <returns>
		/// The identifier.
		/// </returns>
        public uint getId()
        {
            return bufferId;
        }

     
		/// <summary>
		/// Returns the size of this buffer.
		/// </summary>
		/// <returns>
		/// The size.
		/// </returns>
        public int getSize()
        {
            return size;
        }


		/// <summary>
		/// Sets the content of this buffer. The previous content is erased and
        /// replaced by the new one.
		/// </summary>
		/// <returns>
		/// The data.
		/// </returns>
		/// <param name='size'>
		/// Size number of bytes in 'data'.
		/// </param>
		/// <param name='data'>
		/// Data the new buffer data. May be NULL.
		/// </param>
		/// <param name='u'>
		/// U how this buffer will be used.
		/// </param>
		/// <typeparam name='T'>
		/// The 1st type parameter.
		/// </typeparam>
        public void setData<T>(int size, T[] data, BufferUsage u) where T : struct
        {
            Debug.Assert(mappedData == IntPtr.Zero);
            this.size = size;
#if OPENTK
            GL.BindBuffer(BufferTarget.CopyWriteBuffer, bufferId);
            GL.BufferData(BufferTarget.CopyWriteBuffer, (IntPtr)size, data, EnumConversion.getBufferUsage(u));
            GL.BindBuffer(BufferTarget.CopyWriteBuffer, 0);
#else
            glBindBuffer(GL_COPY_WRITE_BUFFER, bufferId);
            glBufferData(GL_COPY_WRITE_BUFFER, size, data, getBufferUsage(u));
            glBindBuffer(GL_COPY_WRITE_BUFFER, 0);
#endif
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);

            if (cpuData != null)
            {
                cpuData = null;
            }
#if  CUSTOM_MAP_BUFFER
            if (size < 1024)
            {
                cpuData = new byte[size];
                if (data != null)
                {
                    Array.Copy(data, cpuData, size);
                }
            }
#endif
        }
        public void setData<T>(int size, T data, BufferUsage u) where T : struct
        {
            Debug.Assert(mappedData == IntPtr.Zero);
            this.size = size;
#if OPENTK
            GL.BindBuffer(BufferTarget.CopyWriteBuffer, bufferId);
            GL.BufferData(BufferTarget.CopyWriteBuffer, (IntPtr)size, ref data, EnumConversion.getBufferUsage(u));
            GL.BindBuffer(BufferTarget.CopyWriteBuffer, 0);
#else
            glBindBuffer(GL_COPY_WRITE_BUFFER, bufferId);
            glBufferData(GL_COPY_WRITE_BUFFER, size, data, getBufferUsage(u));
            glBindBuffer(GL_COPY_WRITE_BUFFER, 0);
#endif
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);

            if (cpuData != null)
            {
                cpuData = null;
            }
#if  CUSTOM_MAP_BUFFER
            if (size < 1024)
            {
                cpuData = new byte[size];
                if (data != null)
                {
                    Array.Copy(data, cpuData, size);
                }
            }
#endif
        }
        public void setData(int size, IntPtr data, BufferUsage u)
        {
            Debug.Assert(mappedData == IntPtr.Zero);
            this.size = size;
#if OPENTK
            GL.BindBuffer(BufferTarget.CopyWriteBuffer, bufferId);
            GL.BufferData(BufferTarget.CopyWriteBuffer, (IntPtr)size, data, EnumConversion.getBufferUsage(u));
            GL.BindBuffer(BufferTarget.CopyWriteBuffer, 0);
#else
            glBindBuffer(GL_COPY_WRITE_BUFFER, bufferId);
            glBufferData(GL_COPY_WRITE_BUFFER, size, data, getBufferUsage(u));
            glBindBuffer(GL_COPY_WRITE_BUFFER, 0);
#endif
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);

            if (cpuData != null)
            {
                cpuData = null;
            }
#if  CUSTOM_MAP_BUFFER
            if (size < 1024)
            {
                cpuData = new byte[size];
                if (data != null)
                {
                    Array.Copy(data, cpuData, size);
                }
            }
#endif
        }


		/// <summary>
		/// Replaces a part of the content of this buffer.
		/// </summary>
		/// <returns>
		/// The sub data.
		/// </returns>
		/// <param name='target'>
		/// Target the target to bind to (use 0 for default).
		/// </param>
		/// <param name='offset'>
		/// Offset index of the first byte to be replaced.
		/// </param>
		/// <param name='size'>
		/// Size number of bytes in 'data'.
		/// </param>
		/// <param name='data'>
		/// Data the new buffer data.
		/// </param>
        public void setSubData(int target, int offset, int size, byte[] data)
        {
            Debug.Assert(mappedData == IntPtr.Zero);
#if OPENTK
            GL.BindBuffer(BufferTarget.CopyWriteBuffer, bufferId);
            GL.BufferSubData(BufferTarget.CopyWriteBuffer, (IntPtr)offset, (IntPtr)size, data);
            GL.BindBuffer(BufferTarget.CopyWriteBuffer, 0);
#else
            glBindBuffer(GL_COPY_WRITE_BUFFER, bufferId);
            glBufferSubData(GL_COPY_WRITE_BUFFER, offset, size, data);
            glBindBuffer(GL_COPY_WRITE_BUFFER, 0);
#endif

            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);

            if (cpuData != null)
            {
                // memcpy(cpuData + offset, (unsigned char*) data, size);
            }
        }

        public void setSubData<T>(int offset, int size, T data) where T : struct
        {
            Debug.Assert(mappedData == IntPtr.Zero);
#if OPENTK
            GL.BindBuffer(BufferTarget.CopyWriteBuffer, bufferId);
            GL.BufferSubData(BufferTarget.CopyWriteBuffer, (IntPtr)offset, (IntPtr)size, ref data);
            GL.BindBuffer(BufferTarget.CopyWriteBuffer, 0);
#else
            glBindBuffer(GL_COPY_WRITE_BUFFER, bufferId);
            glBufferSubData(GL_COPY_WRITE_BUFFER, offset, size, data);
            glBindBuffer(GL_COPY_WRITE_BUFFER, 0);
#endif

            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);

            if (cpuData != null)
            {
                // memcpy(cpuData + offset, (unsigned char*) data, size);
            }
        }

        
		/// <summary>
		/// Gets a part of the content of this buffer.
		/// </summary>
		/// <returns>
		/// The sub data.
		/// </returns>
		/// <param name='target'>
		/// Target the target to bind to (use 0 for default).
		/// </param>
		/// <param name='offset'>
		/// Offset index of the first byte to be replaced.
		/// </param>
		/// <param name='size'>
		/// Size number of bytes in 'data'.
		/// </param>
		/// <param name='data'>
		/// Data he new buffer data.
		/// </param>
        public void getSubData(int target, int offset, int size, byte[] data)
        {
            Debug.Assert(mappedData == IntPtr.Zero);
#if OPENTK
            GL.BindBuffer(BufferTarget.CopyReadBuffer, bufferId);
            GL.BufferSubData(BufferTarget.CopyReadBuffer, (IntPtr)offset, (IntPtr)size, data);
            GL.BindBuffer(BufferTarget.CopyReadBuffer, 0);
#else
            glBindBuffer(GL_COPY_READ_BUFFER, bufferId);
            glGetBufferSubData(GL_COPY_READ_BUFFER, offset, size, data);
            glBindBuffer(GL_COPY_READ_BUFFER, 0);
#endif
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
        }

		/// <summary>
		/// Maps this buffer into CPU memory and returns a pointer to it. If the
        /// access mode is not READ_ONLY, changes made to the mapped buffer in CPU
        /// memory are reflected on GPU when the buffer is unmapped.
		/// </summary>
		/// <param name='a'>
		/// A the read and write permissions for this mapped memory region.
		/// </param>
        public IntPtr map(BufferAccess a)
        {
            Debug.Assert(mappedData == IntPtr.Zero);

            if (cpuData != null)
            {
                // TODO Convert cpuData to mappedData
                //mappedData = cpuData;
                throw new NotImplementedException();
            }
            else
            {
#if OPENTK
                GL.BindBuffer(BufferTarget.CopyReadBuffer, bufferId);
                IntPtr mapData = GL.MapBuffer(BufferTarget.CopyReadBuffer, EnumConversion.getBufferAccess(a));
                GL.BindBuffer(BufferTarget.CopyReadBuffer, 0);
#else
                glBindBuffer(GL_COPY_READ_BUFFER, bufferId);
                mappedData = glMapBuffer(GL_COPY_READ_BUFFER, getBufferAccess(a));
                glBindBuffer(GL_COPY_READ_BUFFER, 0);
#endif
                Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
            }

            return mappedData;
        }

		/// <summary>
		/// Returns the mapped data of this buffer, or NULL if it is currently unmapped.
		/// </summary>
		/// <returns>
		/// The mapped data.
		/// </returns>
        public IntPtr getMappedData()
        {
            return mappedData;
        }

     
		/// <summary>
		/// Unnmaps this buffer from CPU memory.
		/// </summary>
        public void unmap()
        {
            Debug.Assert(mappedData != IntPtr.Zero);

            if (cpuData != null)
            {
#if OPENTK
                GL.BindBuffer(BufferTarget.CopyWriteBuffer, bufferId);
                GL.BufferSubData(BufferTarget.CopyWriteBuffer, IntPtr.Zero, (IntPtr)size, cpuData);
                GL.BindBuffer(BufferTarget.CopyWriteBuffer, 0);
#else
                glBindBuffer(GL_COPY_WRITE_BUFFER, bufferId);
                glBufferSubData(GL_COPY_WRITE_BUFFER, 0, size, cpuData);
                glBindBuffer(GL_COPY_WRITE_BUFFER, 0);
#endif

                Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
            }
            else
            {
#if OPENTK
                GL.BindBuffer(BufferTarget.CopyReadBuffer, bufferId);
                GL.UnmapBuffer(BufferTarget.CopyReadBuffer);
                GL.BindBuffer(BufferTarget.CopyReadBuffer, 0);
#else
                glBindBuffer(GL_COPY_READ_BUFFER, bufferId);
                glUnmapBuffer(GL_COPY_READ_BUFFER);
                glBindBuffer(GL_COPY_READ_BUFFER, 0);
#endif

                Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
            }

            mappedData = IntPtr.Zero;
        }


        internal override void bind(BufferTarget target)
        {
#if OPENTK
            GL.BindBuffer(target, bufferId);
#else
            glBindBuffer(target, bufferId);
#endif
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
        }

     
		/// <summary>
		/// Returns (void*) offset. 
		/// </summary>
		/// <param name='offset'>
		///  Offset an offset from the start of this buffer, in bytes. 
		/// </param>
        internal override IntPtr data(int offset)
        {
            return (IntPtr)offset;
        }

        internal override void unbind(BufferTarget target)
        {
#if OPENTK
            GL.BindBuffer(target, 0);
#else
            glBindBuffer(target, 0);
#endif
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
        }


		/// <summary>
		/// The OpenGL buffer identifier of this buffer (as returned by glGenBuffers).
		/// </summary>
        private uint bufferId;

     
		/// <summary>
		/// The size of this buffer.
		/// </summary>
        internal int size;

    
		/// <summary>
		/// The mapped data of this buffer. NULL if the buffer is currently unmapped.
		/// </summary>
        private volatile IntPtr mappedData;

      
		/// <summary>
		/// Optional copy of buffer data on CPU.
		/// </summary>
        private byte[] cpuData;

		/// <summary>
		/// The uniform block binding unit to which this buffer is currently bound,
        /// or -1 if it is not bound to any uniform block binding unit.
		/// </summary>
        internal int currentUniformUnit;

    
		/// <summary>
		/// Identifiers of the programs that use this buffer as a uniform block.
		/// </summary>
        private List<uint> programIds = new List<uint>();

      
		/// <summary>
		/// Adds the given program as a user of this buffer as a uniform block.
		/// </summary>
		/// <returns>
		/// The user.
		/// </returns>
		/// <param name='programId'>
		/// Program identifier.
		/// </param>
        private void addUser(uint programId)
        {
            Debug.Assert(!isUsedBy(programId));
            programIds.Add(programId);
        }


		/// <summary>
		/// Removes the given program as a user of this buffer as a uniform block.
		/// </summary>
		/// <returns>
		/// The user.
		/// </returns>
		/// <param name='programId'>
		/// Program identifier.
		/// </param>
        private void removeUser(uint programId)
        {
            Debug.Assert(isUsedBy(programId));
            programIds.Remove(programId);
        }

      
		/// <summary>
		/// Returns true if the given program uses this buffer as a uniform block.
		/// </summary>
		/// <returns>
		/// The used by.
		/// </returns>
		/// <param name='programId'>
		/// Program identifier.
		/// </param>
        internal bool isUsedBy(uint programId)
        {
            return programIds.Contains(programId);
        }

 
		/// <summary>
		/// Binds this buffer to a uniform block binding unit not currently used
        /// by the given program. If all the uniform block binding units are
        /// currently bound, a unit not used by the given program will be unbound
        /// and reused to bind this buffer.
		/// </summary>
		/// <returns>
		/// the uniform block binding unit to which this buffer has been
        /// bound, or -1 if no unit was available (meaning that the program
        /// uses too much uniform blocks).
		/// The to uniform buffer unit.
		/// </returns>
		/// <param name='programId'>
		/// Program identifier  the id of a program that must use this buffer as a uniform block.
		/// </param>
        internal int bindToUniformBufferUnit(int programId)
        {
            Debug.Assert(programId != 0);

            int unit = currentUniformUnit;
            if (unit == -1)
            {
                unit = UNIFORM_BUFFER_MANAGER.findFreeUnit((uint)programId);
            }

            UNIFORM_BUFFER_MANAGER.bind((uint)(unit), this);

            return unit;
        }

        private static UniformBufferManager UNIFORM_BUFFER_MANAGER = null;

        // Track whether Dispose has been called. 
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly 
        // or indirectly by a user's code. Managed and unmanaged resources 
        // can be disposed. 
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
                    cpuData = null;
                }
                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.

                UNIFORM_BUFFER_MANAGER.unbind(this);
#if OPENTK
                GL.DeleteBuffers(1, ref bufferId);
#else
                glDeleteBuffers(1, &bufferId);
#endif
                Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);

                // Note disposing has been done.
                disposed = true;

            }
        }
    }

    internal class UniformBufferManager
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public UniformBufferManager()
        {
            time = 0;
            uint maxUnits = getMaxUniformBufferUnits();
            for (uint i = 0; i < maxUnits; ++i)
            {
                units[i] = new UniformBufferUnit(i);
            }
        }

        public int findFreeUnit(uint programId)
        {
            for (int i = 0; i < maxUnits; ++i)
            {
                if (units[i].isFree())
                {
                    return i;
                }
            }

            int bestUnit = -1;
            uint oldestBindingTime = time;

            for (int i = 0; i < maxUnits; ++i)
            {
                GPUBuffer buffer = units[i].getCurrentBufferBinding();
                if (!buffer.isUsedBy(programId))
                {
                    uint bindingTime = units[i].getLastBindingTime();
                    if (bestUnit == -1 || bindingTime < oldestBindingTime)
                    {
                        bestUnit = i;
                        oldestBindingTime = bindingTime;
                    }
                }
            }

            Debug.Assert(bestUnit != -1);
            return bestUnit;
        }

        public void bind(uint i, GPUBuffer buffer)
        {
            units[i].bind(buffer, time++);
        }

        public void unbind(GPUBuffer buffer)
        {
            for (uint i = 0; i < maxUnits; ++i)
            {
                if (units[i].getCurrentBufferBinding() == buffer)
                {
                    units[i].bind(null, time++);
                }
            }
        }

        public void unbindAll()
        {
            for (uint i = 0; i < maxUnits; ++i)
            {
                units[i].bind(null, 0);
            }
            time = 0;
        }

        public static uint getMaxUniformBufferUnits()
        {
            if (maxUnits == 0)
            {
                int maxUniformBufferBindings;
                int v, f, g, h;
#if OPENTK
                GL.GetInteger(GetPName.MaxUniformBufferBindings, out maxUniformBufferBindings);
                GL.GetInteger(GetPName.MaxVertexUniformBlocks, out v);
                GL.GetInteger(GetPName.MaxGeometryUniformBlocks, out f);
                GL.GetInteger(GetPName.MaxFragmentUniformBlocks, out g);
                GL.GetInteger(GetPName.MaxCombinedUniformBlocks, out h);
#else
                glGetIntegerv(GL_MAX_UNIFORM_BUFFER_BINDINGS, &maxUniformBufferBindings);
                glGetIntegerv(GL_MAX_VERTEX_UNIFORM_BLOCKS, &v);
                glGetIntegerv(GL_MAX_GEOMETRY_UNIFORM_BLOCKS, &f);
                glGetIntegerv(GL_MAX_FRAGMENT_UNIFORM_BLOCKS, &g);
                glGetIntegerv(GL_MAX_COMBINED_UNIFORM_BLOCKS, &h);
#endif
                maxUnits = System.Math.Min((uint)maxUniformBufferBindings, MAX_UNIFORM_BUFFER_UNITS);
                maxUnits = System.Math.Min(maxUnits, (uint)v);
                maxUnits = System.Math.Min(maxUnits, (uint)f);
                maxUnits = System.Math.Min(maxUnits, (uint)g);
                maxUnits = System.Math.Min(maxUnits, (uint)h);

                log.Debug("OPENGL MAX_UNIFORM_BUFFER_BINDINGS = " + maxUniformBufferBindings);

            }
            return maxUnits;
        }


        private UniformBufferUnit[] units = new UniformBufferUnit[MAX_UNIFORM_BUFFER_UNITS];

        private uint time;

        private static uint maxUnits = 0;

        internal const uint MAX_UNIFORM_BUFFER_UNITS = 64;

    }


	/// <summary>
	/// A uniform buffer unit.
    /// Used to bind buffers used as uniform blocks in programs.
	/// </summary>
    internal class UniformBufferUnit
    {
        public UniformBufferUnit(uint unit)
        {
            this.unit = unit;
            lastBindingTime = 0;
            currentBufferBinding = null;
        }

        public void bind(GPUBuffer buffer, uint time)
        {
            lastBindingTime = time;

            if (currentBufferBinding != null)
            {
                currentBufferBinding.currentUniformUnit = -1;
            }
            currentBufferBinding = buffer;
            if (currentBufferBinding != null)
            {
                currentBufferBinding.currentUniformUnit = (int)unit;
            }

            if (buffer == null)
            {
#if OPENTK
                GL.BindBufferBase(BufferTarget.UniformBuffer, unit, 0);
#else
                glBindBufferBase(GL_UNIFORM_BUFFER, unit, 0);
#endif
            }
            else
            {
#if OPENGL
                // TODO add support for glBindBufferRange
                //glBindBufferRange(GL_UNIFORM_BUFFER, unit, buffer->getId(), offset, size);
                glBindBufferBase(GL_UNIFORM_BUFFER, unit, buffer.getId());
#else
                GL.BindBufferRange((BufferRangeTarget)BufferTarget.UniformBuffer, (int)unit, (int)buffer.getId(), (IntPtr)0, (IntPtr)buffer.size);
                //GL.BindBufferBase(BufferTarget.UniformBuffer, unit, buffer.getId());
#endif
            }
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
        }

        public uint getLastBindingTime()
        {
            return lastBindingTime;
        }

        public bool isFree()
        {
            return currentBufferBinding == null;
        }

        public GPUBuffer getCurrentBufferBinding()
        {
            return currentBufferBinding;
        }


        private uint unit;

        private uint lastBindingTime;

        private GPUBuffer currentBufferBinding;
    }

}
