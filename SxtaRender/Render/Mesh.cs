using log4net;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Sxta.Render
{
    public class Mesh<vertex, index>
        where vertex : struct
        where index : struct
    {


        /// <summary>
        /// Creates a new mesh.
        /// Initializes a new instance of the <see cref="Sxta.Render.Mesh`2"/> class.
        /// </summary>
        /// <param name='vertexsize'>
        /// Vertexsize.
        /// </param>
        /// <param name='m'>
        /// M how the list of vertices of this mesh must be interpreted.
        /// </param>
        /// <param name='usage'>
        /// Usage how the data should be handled.
        /// </param>
        /// <param name='vertexCount'>
        /// Vertex count the initial capacity of the vertex array.
        /// </param>
        /// <param name='indiceCount'>
        /// Indice count the initial capacity of the indice array.
        /// </param>
        public Mesh(int vertexsize, MeshMode m, MeshUsage usage, int vertexCount = 4, int indiceCount = 4)
            : this(vertexsize, 0, m, usage, vertexCount, indiceCount)
        {
        }
        public Mesh(int vertexsize, int indexSize, MeshMode m, MeshUsage usage, int vertexCount = 4, int indiceCount = 4)
        {
            this.vertexSize = vertexsize;
            this.indexSize = indexSize;
            this.usage = usage;
            this.vertexBuffer = null;
            this.indexBuffer = null;
            this.created = false;
            this.m = m;
            this.buffers = new MeshBuffers();
            vertices = new vertex[vertexCount];
            verticesLength = vertexCount;
            verticesCount = 0;
            indices = new index[indiceCount];
            indicesLength = indiceCount;
            indicesCount = 0;
            primitiveRestart = -1;
            patchVertices = 0;
            vertexDataHasChanged = true;
            indexDataHasChanged = true;
        }


        /// <summary>
        /// Creates a new mesh.
        /// Initializes a new instance of the <see cref="Sxta.Render.Mesh`2"/> class.
        /// </summary>
        /// <param name='vertexsize'>
        /// Vertexsize.
        /// </param>
        /// <param name='target'>
        /// Target the mesh buffers wrapped by this mesh.
        /// </param>
        /// <param name='m'>
        /// M how the list of vertices of this mesh must be interpreted.
        /// </param>
        /// <param name='usage'>
        /// Usage how the data should be handled.
        /// </param>
        /// <param name='vertexCount'>
        /// Vertex count the initial capacity of the vertex array.
        /// </param>
        /// <param name='indiceCount'>
        /// Indice count the initial capacity of the indice array.
        /// </param>
        public Mesh(int vertexsize, MeshBuffers target, MeshMode m, MeshUsage usage, int vertexCount = 4, int indiceCount = 4)
        {
            this.vertexSize = vertexsize;
            this.usage = usage;
            this.created = false;
            this.m = m;
            this.buffers = target;
            vertices = new vertex[vertexCount];
            verticesLength = vertexCount;
            verticesCount = 0;

            indices = new index[indiceCount];
            indicesLength = indiceCount;
            indicesCount = 0;

            primitiveRestart = -1;
            patchVertices = 0;
            vertexDataHasChanged = true;
            indexDataHasChanged = true;
        }


        /// <summary>
        /// Deletes this mesh.
        /// Releases unmanaged resources and performs other cleanup operations before the <see cref="Sxta.Render.Mesh`2"/> is
        /// reclaimed by garbage collection.
        /// </summary>
        ~Mesh()
        {
            // Do not re-create Dispose clean-up code here. 
            // Calling Dispose(false) is optimal in terms of 
            // readability and maintainability.
            Dispose(false);
        }

        /// <summary>
        /// Returns the interpretation mode of the vertices of this mesh.
        /// Gets the mode.
        /// </summary>
        /// <returns>
        /// The mode.
        /// </returns>
        public MeshMode getMode()
        {
            return m;
        }


        /// <summary>
        /// Gets the vertex count.
        /// </summary>
        /// <returns>
        /// Returns the number of vertices in this mesh.
        /// The vertex count.
        /// </returns>
        public int getVertexCount()
        {
            return verticesCount;
        }

        /// <summary>
        /// eturns a vertex of this mesh.
        /// </summary>
        /// <returns>
        /// The vertex.
        /// </returns>
        /// <param name='i'>
        /// I a vertex index.
        /// </param>
        public vertex getVertex(int i)
        {
            return vertices[i];
        }

        /// <summary>
        /// Returns the number of indices of this mesh.
        /// Gets the indice count.
        /// </summary>
        /// <returns>
        /// The indice count.
        /// </returns>
        public int getIndiceCount()
        {
            return indicesCount;
        }


        /// <summary>
        ///  Returns the vertex index used for primitive restart. -1 means no restart.
        /// Gets the primitive restart.
        /// </summary>
        /// <returns>
        /// The primitive restart.
        /// </returns>
        public int getPrimitiveRestart()
        {
            return primitiveRestart;
        }

        /// <summary> 
        /// Returns the number of vertices per patch in this mesh, if #getMode() is PATCHES.
        /// Gets the patch vertices.
        /// </summary>
        /// <returns>
        /// The patch vertices.
        /// </returns>
        public int getPatchVertices()
        {
            return patchVertices;
        }



        /// <summary>
        /// Returns the MeshBuffers wrapped by this Mesh instance.
        /// </summary>
        /// <returns>
        /// The buffers.
        /// </returns>
        public MeshBuffers getBuffers()
        {
            if (!created)
            {
                createBuffers();
            }

            if ((usage == MeshUsage.GPU_DYNAMIC) || (usage == MeshUsage.GPU_STREAM))
            { // upload data to GPU if needed
                BufferUsage u = (usage == MeshUsage.GPU_DYNAMIC ? BufferUsage.DYNAMIC_DRAW : BufferUsage.STREAM_DRAW);
                if (vertexDataHasChanged)
                {
                    uploadVertexDataToGPU(u);
                }
                if ((indicesCount != 0) && indexDataHasChanged)
                {
                    uploadIndexDataToGPU(u);
                }
            }

            buffers.primitiveRestart = primitiveRestart;
            buffers.patchVertices = patchVertices;

            return buffers;
        }


        /// <summary>
        /// Declares an attribute of the vertices of this mesh.
        /// </summary>
        /// <returns>
        /// The attribute type.
        /// </returns>
        /// <param name='id'>
        /// Identifier  a vertex attribute index.
        /// </param>
        /// <param name='size'>
        /// Size the number of components in attributes of this kind.
        /// </param>
        /// <param name='type'>
        /// Type the type of each component in attributes of this kind.
        /// </param>
        /// <param name='norm'>
        /// Norm if the attribute components must be normalized to 0..1.
        /// </param>
        public void addAttributeType(int id, int size, AttributeType type, bool norm)
        {
            buffers.addAttributeBuffer(id, size, vertexSize, type, norm);
        }



        /// <summary>
        /// Sets the capacity of the vertex and indice array of this mesh. Does
        /// nothing if the provided sizes are smaller than the current ones.
        /// Sets the capacity.
        /// </summary>
        /// <returns>
        /// The capacity.
        /// </returns>
        /// <param name='vertexCount'>
        /// Vertex count the new vertex array capacity.
        /// </param>
        /// <param name='indiceCount'>
        /// Indice count the new indice array capacity.
        /// </param>
        public void setCapacity(int vertexCount, int indiceCount)
        {
            if (verticesCount < vertexCount)
            {
                resizeVertices(vertexCount);
            }
            if (indicesCount < indiceCount)
            {
                resizeIndices(indiceCount);
            }
        }


        /// <summary>
        /// Adds a vertex to this mesh.
        /// </summary>
        /// <returns>
        /// The vertex.
        /// </returns>
        /// <param name='v'>
        /// V v a vertex.
        /// </param>
        public void addVertex(vertex v)
        {
            if (verticesCount == verticesLength)
            {
                resizeVertices(2 * verticesLength);
            }
            vertices[verticesCount++] = v;
            vertexDataHasChanged = true;
        }


        /// <summary>
        /// Adds vertices this mesh.
        /// </summary>
        /// <returns>
        /// The vertices.
        /// </returns>
        /// <param name='v'>
        /// V a pointer to a vertex array.
        /// </param>
        /// <param name='count'>
        /// Count count number of vertices
        /// </param>
        public void addVertices(vertex[] v, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                addVertex(v[i]);
            }
        }


        /// <summary>
        /// Adds an indice to this mesh.
        /// </summary>
        /// <returns>
        /// The indice.
        /// </returns>
        /// <param name='i'>
        /// I a vertex index.
        /// </param>
        public void addIndice(index i)
        {
            if (indicesCount == indicesLength)
            {
                resizeIndices(2 * indicesLength);
            }
            indices[indicesCount++] = i;
            indexDataHasChanged = true;
        }

        /// <summary>
		/// Adds an indice to this mesh.
		/// </summary>
		/// <returns>
		/// The indice.
		/// </returns>
		/// <param name='i'>
		/// I a vertex index.
		/// </param>
        public void addIndices(index[] arr)
        {
            int intitialCnt = indicesCount;
            resizeIndices(indicesCount + arr.Length);
            Array.Copy(arr, 0, indices, intitialCnt, arr.Length);
            indicesCount = intitialCnt + arr.Length;
            indexDataHasChanged = true;
        }

        /// <summary>
        /// Sets the interpretation mode of the vertices of this mesh.
        /// </summary>
        /// <returns>
        /// The mode.
        /// </returns>
        /// <param name='mode'>
        /// Mode.
        /// </param>
        public void setMode(MeshMode mode)
        {
            m = mode;
        }


        /// <summary>
        /// Changes a vertex of this mesh.
        /// </summary>
        /// <returns>
        /// The vertex.
        /// </returns>
        /// <param name='i'>
        /// I.
        /// </param>
        /// <param name='v'>
        /// V.
        /// </param>
        public void setVertex(int i, vertex v)
        {
            vertices[i] = v;
            vertexDataHasChanged = true;
        }

        /// <summary>
        /// Changes an indice of this mesh.
        /// </summary>
        /// <returns>
        /// The indice.
        /// </returns>
        /// <param name='i'>
        /// I.
        /// </param>
        /// <param name='ind'>
        /// Ind.
        /// </param>
        public void setIndice(int i, index ind)
        {
            indices[i] = ind;
            indexDataHasChanged = true;
        }


        /// <summary>
        /// Sets the vertex index used for primitive restart. -1 means no restart.
        /// </summary>
        /// <returns>
        /// The primitive restart.
        /// </returns>
        /// <param name='restart'>
        /// Restart.
        /// </param>
        public void setPrimitiveRestart(int restart)
        {
            primitiveRestart = restart;
        }



        /// <summary>
        /// Sets the number of vertices per patch in this mesh, if #getMode() is PATCHES.
        /// </summary>
        /// <returns>
        /// The patch vertices.
        /// </returns>
        /// <param name='vertices'>
        /// Vertices.
        /// </param>
        public void setPatchVertices(int vertices)
        {
            patchVertices = vertices;
        }


        /// <summary>
        /// Removes all the vertices and indices of this mesh.
        /// Clear this instance.
        /// </summary>
        public void clear()
        {
            verticesCount = 0;
            indicesCount = 0;
            vertexDataHasChanged = true;
            indexDataHasChanged = true;
            if (created)
            {
                buffers.reset();
                buffers.setIndicesBuffer(null);
                created = false;
            }
        }


        /// <summary>
        /// Clears the MeshBuffers.
        /// </summary>
        /// <returns>
        /// The buffers.
        /// </returns>
        public void clearBuffers()
        {
            if (created)
            {
                buffers.reset();
                created = false;
            }
        }


        /// <summary>
        /// The usage of this mesh.
        /// </summary>
        private MeshUsage usage;

        /// <summary>
        /// The Buffer containing the vertices data.
        /// </summary>
        private Buffer vertexBuffer;


        /// <summary>
        /// The Buffer containing the indices data.
        /// </summary>
        private Buffer indexBuffer;


        /// <summary>
        /// True if the vertex data has changed since last call to #uploadVertexDataToGPU.
        /// </summary>
        private bool vertexDataHasChanged;


        /// <summary>
        /// True if the index data has changed since last call to #uploadIndexDataToGPU.
        /// </summary>
        private bool indexDataHasChanged;

        /// <summary>
        ///  True if the CPU or GPU mesh buffers have been created.
        /// </summary>
        private bool created;


        /// <summary>
        ///  How the list of vertices of this mesh must be interpreted.
        /// </summary>
        private MeshMode m;


        /// <summary>
        /// The vertices of this mesh.
        /// </summary>
        private vertex[] vertices;


        /// <summary>
        ///  The capacity of the vertex array.
        /// </summary>
        private int verticesLength;


        /// <summary>
        /// The actual number of vertices.
        /// </summary>
        private int verticesCount;

        /// <summary>
        /// The indices of this mesh.
        /// </summary>
        private index[] indices;


        /// <summary>
        /// The capacity of the indice array.
        /// </summary>
        private int indicesLength;

        /// <summary>
        /// The actual number of indices.
        /// </summary>
        private int indicesCount;


        /// <summary>
        /// The vertex index used for primitive restart. -1 means no restart.
        /// </summary>
        private int primitiveRestart;


        /// <summary>
        /// The number of vertices per patch in this mesh, if #getMode() is PATCHES.
        /// </summary>
        private int patchVertices;


        /// <summary>
        /// The MeshBuffers wrapped by this Mesh.
        /// </summary>
        private MeshBuffers buffers;

        private readonly int vertexSize;
        private readonly int indexSize;


        /// <summary>
        /// Resizes the vertex array to expand its capacity.
        /// </summary>
        /// <returns>
        /// The vertices.
        /// </returns>
        /// <param name='newSize'>
        /// New size.
        /// </param>
        private void resizeVertices(int newSize)
        {
#if DEBUG
            log.Debug("Resize vertex array to new size = " + newSize);
#endif
            vertex[] newVertices = new vertex[newSize];
            Array.Copy(vertices, newVertices, verticesLength);
            vertices = newVertices;
            verticesLength = newSize;
            if (created)
            {
                buffers.reset();
                created = false;
            }
        }


        /// <summary>
        /// Resizes the indice array to expand its capacity.
        /// </summary>
        /// <returns>
        /// The indices.
        /// </returns>
        /// <param name='newSize'>
        /// New size.
        /// </param>
        private void resizeIndices(int newSize)
        {
#if DEBUG
            log.Debug("Resize index array to new size = " + newSize);
#endif
            index[] newIndices = new index[newSize];
            Array.Copy(indices, newIndices, indicesLength);
            indices = newIndices;
            indicesLength = newSize;
            if (created)
            {
                buffers.reset();
                created = false;
            }
        }



        /// <summary>
        /// Creates the CPU of GPU buffers based on the current content of the
        /// vertex and indice arrays.
        /// </summary>
        /// <returns>
        /// The buffers.
        /// </returns>
        private void createBuffers()
        {
            if (usage == MeshUsage.GPU_STATIC || usage == MeshUsage.GPU_DYNAMIC || usage == MeshUsage.GPU_STREAM)
            {
                vertexBuffer = new GPUBuffer();
                if (usage == MeshUsage.GPU_STATIC)
                {
                    uploadVertexDataToGPU(BufferUsage.STATIC_DRAW);
                }
            }
            else if (usage == MeshUsage.CPU)
            {
                vertexBuffer = new CPUBuffer<vertex>(vertices);

            }

            Debug.Assert(buffers.getAttributeCount() > 0);
            for (int i = 0; i < buffers.getAttributeCount(); ++i)
            {
                buffers.getAttributeBuffer(i).setBuffer(vertexBuffer);
            }

            if (indicesCount != 0)
            {
                if (usage == MeshUsage.GPU_STATIC || usage == MeshUsage.GPU_DYNAMIC || usage == MeshUsage.GPU_STREAM)
                {
                    indexBuffer = new GPUBuffer();
                    if (usage == MeshUsage.GPU_STATIC)
                    {
                        uploadIndexDataToGPU(BufferUsage.STATIC_DRAW);
                    }
                }
                else if (usage == MeshUsage.CPU)
                {
                    indexBuffer = new CPUBuffer<index>(indices);

                }

                AttributeType type;
                switch (indexSize)
                {
                    case 1:
                        type = AttributeType.A8UI;
                        break;
                    case 2:
                        type = AttributeType.A16UI;
                        break;
                    default:
                        type = AttributeType.A32UI;
                        break;
                }
                buffers.setIndicesBuffer(new AttributeBuffer(0, 1, type, false, indexBuffer));
            }
            buffers.mode = m;
            buffers.nvertices = verticesCount;
            buffers.nindices = indicesCount;
            created = true;
        }


        /// <summary>
        /// Send the vertices to the GPU.
        /// </summary>
        /// <returns>
        /// The vertex data to GP.
        /// </returns>
        /// <param name='u'>
        /// U.
        /// </param>
        private void uploadVertexDataToGPU(BufferUsage u)
        {
            GPUBuffer vb = vertexBuffer as GPUBuffer;
            Debug.Assert(vb != null); // check it's a GPU mesh
            vb.setData(verticesCount * vertexSize, vertices, u);
            vertexDataHasChanged = false;

        }

        /// <summary>
        /// Send the indices to the GPU.
        /// </summary>
        /// <returns>
        /// The index data to GP.
        /// </returns>
        /// <param name='u'>
        /// U.
        /// </param>
        private void uploadIndexDataToGPU(BufferUsage u)
        {
            GPUBuffer ib = indexBuffer as GPUBuffer;
            Debug.Assert(ib != null);
            ib.setData(indicesCount * indexSize, indices, u);
            indexDataHasChanged = false;
        }


        #region Dispose

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
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.
                if (buffers != null)
                    buffers.Dispose();
                if (vertexBuffer != null && vertexBuffer is GPUBuffer)
                    ((GPUBuffer)vertexBuffer).Dispose();
                if (indexBuffer != null && indexBuffer is GPUBuffer)
                    ((GPUBuffer)indexBuffer).Dispose();

                Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);

                // Note disposing has been done.
                disposed = true;

            }
        }
        #endregion

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
