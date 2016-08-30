using log4net;
using OpenTK.Graphics.OpenGL;
using Sxta.Core;
using Sxta.Math;
using Sxta.Render.Core;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sxta.Render
{
    /// <summary>
    ///  A set of AttributeBuffer that represent the vertices and indices of a mesh.
    /// Each attribute buffer represents an attribute (position, normal, color, etc)
    /// of the vertices of the mesh. A mesh can have an indices array that allows
    /// vertices shared between adjacent primitives to be shared in memory. If there
    /// is no indices array shared vertices must be duplicated in the vertices array.
    /// </summary>
    public class MeshBuffers : ISwappable<MeshBuffers>, IDisposable
    {
//#if DEBUG
//        TraceOpenTKDisposableObject traceDisposable;
//#endif


        /// <summary>
        /// How the list of vertices of this mesh must be interpreted.
        /// </summary>
        public MeshMode mode;

        /// <summary>
        /// The number of vertices in this mesh.
        /// </summary>
        public int nvertices;


        /// <summary>
        /// The number of indices in this mesh.
        /// </summary>
        public int nindices;


        /// <summary>
        /// The bounding box of this mesh.
        /// </summary>
        public Box3f bounds;


        /// <summary>
        /// The vertex index used for primitive restart. -1 means no restart.
        /// </summary>
        public int primitiveRestart;


        /// <summary>
        /// The number of vertices per patch of this mesh, if mode is PATCHES.
        /// </summary>
        public int patchVertices;


        /// <summary>
        /// Creates a new mesh without any AttributeBuffer.
        /// Initializes a new instance of the <see cref="Sxta.Render.MeshBuffers"/> class.
        /// </summary>
        public MeshBuffers()
        {
//#if DEBUG
//            traceDisposable = new TraceOpenTKDisposableObject();
//#endif
            mode = MeshMode.POINTS;
            nvertices = 0;
            nindices = 0;
            primitiveRestart = -1;
            patchVertices = 0;
        }



        /// <summary>
        /// Deletes this mesh.
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Sxta.Render.MeshBuffers"/> is reclaimed by garbage collection.
        /// </summary>
        ~MeshBuffers()
        {
            // Do not re-create Dispose clean-up code here. 
            // Calling Dispose(false) is optimal in terms of 
            // readability and maintainability.
            Dispose(false);
        }


        /// <summary>
        /// Returns the number of AttributeBuffer in this mesh. This is the
        /// number of attributes per vertex.
        /// </summary>
        /// <returns>
        /// The attribute count.
        /// </returns>
        public int getAttributeCount()
        {
            return attributeBuffers.Count;
        }


        /// <summary>
        /// Returns the AttributeBuffer whose index is given.
        /// </summary>
        /// <returns>
        /// The attribute buffer.
        /// </returns>
        /// <param name='index'>
        /// Index index an index between 0 and #getAttributeCount (exclusive).
        /// </param>
        public AttributeBuffer getAttributeBuffer(int index)
        {
            return attributeBuffers[index];
        }


        /// <summary>
        /// Returns the indices buffer of this mesh.
        /// </summary>
        /// <returns>
        /// The indice buffer.
        /// </returns>
        public AttributeBuffer getIndiceBuffer()
        {
            return indicesBuffer;
        }


        /// <summary>
        /// Adds a vertex attribute buffer to this mesh. This method assumes that
        /// this vertex attribute is stored in its own buffer.
        /// </summary>
        /// <returns>
        /// The attribute buffer.
        /// </returns>
        /// <param name='index'>
        /// Index  a vertex attribute index.
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
        public void addAttributeBuffer(int index, int size, AttributeType type, bool norm)
        {
            AttributeBuffer a = new AttributeBuffer(index, size, type, norm, null);
            attributeBuffers.Add(a);
        }


        /// <summary>
        /// Adds a vertex attribute buffer to this mesh. This method assumes that
        /// this vertex attribute is stored interleaved with others in a shared buffer.
        /// For instance, for a mesh with a position, normal and color attributes,
        /// the data layout is position, normal and color of first vertex, position,
        /// normal and color of second vertex, and so on for other vertices.
        /// </summary>
        /// <returns>
        /// The attribute buffer.
        /// </returns>
        /// <param name='index'>
        /// Index  a vertex attribute index.
        /// </param>
        /// <param name='size'>
        /// Size the number of components in attributes of this kind.
        /// </param>
        /// <param name='vertexsize'>
        /// Vertexsize the total size of all the vertex attributes.
        /// </param>
        /// <param name='type'>
        /// Type the type of each component in attributes of this kind.
        /// </param>
        /// <param name='norm'>
        /// Norm if the attribute components must be normalized to 0..1.
        /// </param>
        public void addAttributeBuffer(int index, int size, int vertexsize, AttributeType type, bool norm)
        {
            int offset;
            if (attributeBuffers.Count > 0)
            {
                AttributeBuffer ab = attributeBuffers[attributeBuffers.Count - 1];
                offset = ab.getOffset() + ab.getAttributeSize();
            }
            else
            {
                offset = 0;
            }
            AttributeBuffer a = new AttributeBuffer(index, size, type, norm, null, vertexsize, offset);
            attributeBuffers.Add(a);
        }

        public void addAttributeBuffer(IEnumerable<AttributeBuffer> attrbs)
        {
            attributeBuffers.AddRange(attrbs);
        }

        /// <summary>
        /// Sets the indices array buffer of this mesh.
        /// </summary>
        /// <returns>
        /// The indices buffer.
        /// </returns>
        /// <param name='indices'>
        /// Indices.
        /// </param>
        public void setIndicesBuffer(AttributeBuffer indices)
        {
            indicesBuffer = indices;
        }


        /// <summary>
        /// Resets the internal %state associated with this mesh. For internal use only.
        /// </summary>
        public void reset()
        {
            if (CURRENT == this)
            {
                unbind();
                CURRENT = null;
            }
        }


        /// <summary>
        /// Sets the default valueC for the given attribute when a MeshBuffers does
        /// not specify any Buffer for this attribute.
        /// </summary>
        /// <returns>
        /// The default attribute.
        /// </returns>
        /// <param name='index'>
        /// Index  a vertex attribute index. This attribute must be declared with a floating point type in the program.
        /// </param>
        /// <param name='defaultValue'>
        /// Default value  the default valueC to use for this attribute.
        /// </param>
        /// <typeparam name='T'>
        /// The 1st type parameter.
        /// </typeparam>
        public static void setDefaultAttribute<T>(uint index, T defaultValue)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Sets the default valueC for the given attribute when a MeshBuffers does
        /// not specify any Buffer for this attribute.
        /// </summary>
        /// <returns>
        /// The default attribute.
        /// </returns>
        /// <param name='index'>
        /// index a vertex attribute index. This attribute must be declared with
        /// floating point type in the program.
        /// </param>
        /// <param name='count'>
        /// Count the number of elements in the 'defaultValue' array.
        /// </param>
        /// <param name='defaultValue'>
        /// Default value the default valueC to use for this attribute.
        /// </param>
        /// <param name='normalize'>
        /// Normalize  true to normalize the components of 'defaultValue'.
        /// </param>
        /// <typeparam name='T'>
        /// The 1st type parameter.
        /// </typeparam>
        public static void setDefaultAttribute<T>(uint index, int count, T defaultValue, bool normalize = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the default valueC for the given attribute when a MeshBuffers does
        /// not specify any Buffer for this attribute.
        /// </summary>
        /// <returns>
        /// The default attribute i.
        /// </returns>
        /// <param name='index'>
        /// a vertex attribute index. This attribute must be declared with
        /// an integer or unsigned integer type in the program.
        /// </param>
        /// <param name='defaultValue'>
        /// Default value the default valueC to use for this attribute.	
        /// </param>
        /// <typeparam name='T'>
        /// The 1st type parameter.
        /// </typeparam>
        public static void setDefaultAttributeI<T>(uint index, T defaultValue)
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// Sets the default valueC for the given attribute when a MeshBuffers does
        /// not specify any Buffer for this attribute.
        /// </summary>
        /// <returns>
        /// The default attribute i.
        /// </returns>
        /// <param name='index'>
        /// Index a vertex attribute index. This attribute must be declared with
        /// an integer or unsigned integer type in the program.
        /// </param>
        /// <param name='count'>
        /// Count the number of elements in the 'defaultValue' array.
        /// </param>
        /// <param name='defaultValue'>
        /// Default value the default valueC to use for this attribute.
        /// </param>
        /// <typeparam name='T'>
        /// The 1st type parameter.
        /// </typeparam>
        public static void setDefaultAttributeI<T>(uint index, int count, T defaultValue)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Sets the default valueC for the given attribute when a MeshBuffers does
        /// not specify any Buffer for this attribute.
        /// </summary>
        /// <returns>
        /// The default attribute l.
        /// </returns>
        /// <param name='index'>
        /// Index a vertex attribute index. This attribute must be declared with
        /// a double type in the program.
        /// </param>
        /// <param name='defaultValue'>
        /// Default value the default valueC to use for this attribute.
        /// </param>
        /// <typeparam name='T'>
        /// The 1st type parameter.
        /// </typeparam>
        public static void setDefaultAttributeL<T>(uint index, T defaultValue)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Sets the default valueC for the given attribute when a MeshBuffers does
        /// not specify any Buffer for this attribute.
        /// </summary>
        /// <returns>
        /// The default attribute l.
        /// </returns>
        /// <param name='index'>
        /// Indexa vertex attribute index. This attribute must be declared with
        /// a double point type in the program.
        /// </param>
        /// <param name='count'>
        /// Count the number of elements in the 'defaultValue' array.
        /// </param>
        /// <param name='defaultValue'>
        /// Default value the default valueC to use for this attribute.
        /// </param>
        /// <typeparam name='T'>
        /// The 1st type parameter.
        /// </typeparam>
        public static void setDefaultAttributeL<T>(uint index, int count, T defaultValue)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Sets the default valueC for the given attribute when a MeshBuffers does
        /// not specify any Buffer for this attribute. This method only sets the first
        /// component of the specified attribute.
        /// </summary>
        /// <returns>
        /// The default attribute p1.
        /// </returns>
        /// <param name='index'>
        /// Index a vertex attribute index. This attribute must be declared with
        /// a floating point type in the program.
        /// </param>
        /// <param name='defaultValue'>
        /// Default value  the default valueC to use for this attribute,
        /// in packed format.
        /// </param>
        /// <param name='isSigned'>
        /// isSigned true to use the signed packed format, false to use the
        /// unsigned packed format.
        /// </param>
        /// <param name='normalize'>
        /// Normalize 
        /// </param>
        public static void setDefaultAttributeP1(uint index, uint defaultValue, bool isSigned, bool normalize = false)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Sets the default valueC for the given attribute when a MeshBuffers does
        ///  not specify any Buffer for this attribute. This method only sets the first
        ///  two components of the specified attribute.
        /// </summary>
        /// <returns>
        /// The default attribute p2.
        /// </returns>
        /// <param name='index'>
        /// Indexa vertex attribute index. This attribute must be declared with
        ///  a floating point type in the program.
        /// </param>
        /// <param name='defaultValue'>
        /// DefaultValue the default valueC to use for this attribute,
        ///     in packed format.
        /// </param>
        /// <param name='isSigned'>
        /// Is signed  true to use the signed packed format, false to use the
        /// unsigned packed format.
        /// </param>
        /// <param name='normalize'>
        /// Normalize true to normalize the components of 'defaultValue'.
        /// </param>
        public static void setDefaultAttributeP2(uint index, uint defaultValue, bool isSigned, bool normalize = false)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Sets the default valueC for the given attribute when a MeshBuffers does
        /// not specify any Buffer for this attribute. This method only sets the first
        /// three components of the specified attribute.
        /// </summary>
        /// <returns>
        /// The default attribute p3.
        /// </returns>
        /// <param name='index'>
        /// Index a vertex attribute index. This attribute must be declared with
        /// a floating point type in the program.
        /// </param>
        /// <param name='defaultValue'>
        /// Default valuethe default valueC to use for this attribute,
        /// in packed format.
        /// </param>
        /// <param name='isSigned'>
        /// Is signed true to use the signed packed format, false to use the
        ///  unsigned packed format.
        /// </param>
        /// <param name='normalize'>
        /// Normalize true to normalize the components of 'defaultValue'.
        /// </param>
        public static void setDefaultAttributeP3(uint index, uint defaultValue, bool isSigned, bool normalize = false)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Sets the default valueC for the given attribute when a MeshBuffers does
        /// not specify any Buffer for this attribute. This method sets the four
        ///  components of the specified attribute.
        /// </summary>
        /// <returns>
        /// The default attribute p4.
        /// </returns>
        /// <param name='index'>
        /// Index a vertex attribute index. This attribute must be declared with
        /// a floating point type in the program.
        /// </param>
        /// <param name='defaultValue'>
        /// Default value the default valueC to use for this attribute,
        /// in packed format.
        /// </param>
        /// <param name='isSigned'>
        /// Is signed true to use the signed packed format, false to use the
        ///  unsigned packed format.
        /// </param>
        /// <param name='normalize'>
        /// Normalize true to normalize the components of 'defaultValue'.
        /// </param>
        public static void setDefaultAttributeP4(uint index, uint defaultValue, bool isSigned, bool normalize = false)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Sets the default valueC for the given attribute when a MeshBuffers does
        /// not specify any Buffer for this attribute. This method only sets the first
        /// 'count' components of the specified attribute.
        /// </summary>
        /// <returns>
        /// The default attribute p.
        /// </returns>
        /// <param name='index'>
        /// Index a vertex attribute index. This attribute must be declared with
        ///a floating point type in the program.
        /// </param>
        /// <param name='count'>
        /// Count the number of components of the attribute to set.
        /// </param>
        /// <param name='defaultValue'>
        /// Default value the default valueC to use for this attribute,
        ///   in packed format.
        /// </param>
        /// <param name='isSigned'>
        /// Is signed isSigned true to use the signed packed format, false to use the
        /// unsigned packed format.
        /// </param>
        /// <param name='normalize'>
        /// Normalize true to normalize the components of 'defaultValue'.
        /// </param>
        public static void setDefaultAttributeP(uint index, int count, uint[] defaultValue, bool isSigned, bool normalize = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Swaps this mesh with the given one.
        /// </summary>
        /// <param name='buffers'>
        /// Buffers.
        /// </param>
        public virtual void swap(MeshBuffers buffers)
        {
            Std.Swap<MeshMode>(ref mode, ref buffers.mode);
            Std.Swap<int>(ref nvertices, ref buffers.nvertices);
            Std.Swap<int>(ref nindices, ref buffers.nindices);
            Std.Swap(ref bounds, ref buffers.bounds);
            Std.Swap(ref attributeBuffers, ref buffers.attributeBuffers);
            Std.Swap<AttributeBuffer>(ref indicesBuffer, ref buffers.indicesBuffer);
        }

        /// <summary>
        /// Draws a part of this mesh one or more times.
        /// </summary>
        /// <param name='m'>
        /// M how the mesh vertices must be interpreted.
        /// </param>
        /// <param name='first'>
        /// First the first vertex to draw, or the first indice to draw if
        /// this mesh has indices.
        /// </param>
        /// <param name='count'>
        /// Count the number of vertices to draw, or the number of indices
        /// to draw if this mesh has indices.
        /// </param>
        /// <param name='primCount'>
        /// Prim count the number of times this mesh must be drawn (with
        ///  geometry instancing).
        /// </param>
        /// <param name='basevertex'>
        /// Basevertex the base vertex to use. Only used for meshes with indices..
        /// </param>
        public void draw(MeshMode m, int first, int count, int primCount = 1, int basevertex = 0)
        {
            ErrorCode err1 = FrameBuffer.getError();

#if OPENTK
            if (CURRENT != this)
            {
                set();
            }

            if (primitiveRestart != CURRENT_RESTART_INDEX)
            {
                if (primitiveRestart >= 0)
                {
                    GL.Enable(EnableCap.PrimitiveRestart);
                    GL.PrimitiveRestartIndex((uint)(primitiveRestart));
                }
                else
                {
                    GL.Disable(EnableCap.PrimitiveRestart);
                }
                CURRENT_RESTART_INDEX = primitiveRestart;
            }
            if (patchVertices > 0 && patchVertices != CURRENT_PATCH_VERTICES)
            {
                GL.PatchParameter(PatchParameterInt.PatchVertices, patchVertices);
            }

            if (indicesBuffer == null)
            {
                if (primCount == 1)
                {
                    GL.DrawArrays(EnumConversion.getPrimitiveType(m), first, count);
                    ErrorCode err2 = FrameBuffer.getError();
                }
                else
                {
                    GL.DrawArraysInstanced(EnumConversion.getPrimitiveType(m), first, count, primCount);
                }
            }
            else
            {
                int indices = offset;
                if (first > 0)
                {
                    indices = offset + first * indicesBuffer.getAttributeSize();
                }
                if (basevertex == 0)
                {
                    if (primCount == 1)
                    {
                        GL.DrawElements(EnumConversion.getMeshMode(m), count, EnumConversion.getDrawElementType(type), indices);
                    }
                    else
                    {
                        GL.DrawElementsInstancedBaseInstance(EnumConversion.getPrimitiveType(m), count, EnumConversion.getDrawElementType(type), (IntPtr)indices, primCount, 0);
                    }
                }
                else
                {
                    if (primCount == 1)
                    {
                        GL.DrawElementsBaseVertex(EnumConversion.getPrimitiveType(m), count, EnumConversion.getDrawElementType(type), (IntPtr)indices, basevertex);
                    }
                    else
                    {
                        GL.DrawElementsInstancedBaseVertex(EnumConversion.getPrimitiveType(m), count, EnumConversion.getDrawElementType(type), (IntPtr)indices, primCount, basevertex);
                    }
                }
            }
#else
           if (CURRENT != this)
            {
                set();
            }

            if (primitiveRestart != CURRENT_RESTART_INDEX)
            {
                if (primitiveRestart >= 0)
                {
                    glEnable(GL_PRIMITIVE_RESTART);
                    glPrimitiveRestartIndex(GLuint(primitiveRestart));
                }
                else
                {
                    glDisable(GL_PRIMITIVE_RESTART);
                }
                CURRENT_RESTART_INDEX = primitiveRestart;
            }
            if (patchVertices > 0 && patchVertices != CURRENT_PATCH_VERTICES)
            {
                glPatchParameteri(GL_PATCH_VERTICES, patchVertices);
            }

            if (indicesBuffer == null)
            {
                if (primCount == 1)
                {
                    glDrawArrays(getMeshMode(m), first, count);
                }
                else
                {
                    glDrawArraysInstanced(getMeshMode(m), first, count, primCount);
                }
            }
            else
            {
                int indices = offset;
                if (first > 0)
                {
                    indices = offset + first * indicesBuffer.getAttributeSize();
                }
                if (basevertex == 0)
                {
                    if (primCount == 1)
                    {
                        glDrawElements(getMeshMode(m), count, getAttributeType(type), indices);
                    }
                    else
                    {
                        glDrawElementsInstanced(getMeshMode(m), count, getAttributeType(type), indices, primCount);
                    }
                }
                else
                {
                    if (primCount == 1)
                    {
                        glDrawElementsBaseVertex(getMeshMode(m), count, getAttributeType(type), indices, basevertex);
                    }
                    else
                    {
                        glDrawElementsInstancedBaseVertex(getMeshMode(m), count, getAttributeType(type), indices, primCount, basevertex);
                    }
                }
            }
#endif
#if  DEBUG
            ErrorCode err = FrameBuffer.getError();
            if (err != ErrorCode.NoError)
            {
                if (Program.CURRENT == null || Program.CURRENT.checkSamplers())
                {
                    log.Error("OpenGL error " + err);
                    Debug.Assert(err == 0);
                }
            }
#endif
        }




        /// <summary>
        /// Draws several parts of this mesh. Each part is specified with a first
        /// and count parameter as in #draw(). These values are passed in arrays
        /// of primCount values.
        /// </summary>
        /// <returns>
        /// The draw.
        /// </returns>
        /// <param name='m'>
        /// M  how the mesh vertices must be interpreted.
        /// </param>
        /// <param name='firsts'>
        /// Firsts an array of primCount 'first vertex' to draw, or an array
        ///     of 'first indice' to draw if this mesh has indices.
        /// </param>
        /// <param name='counts'>
        /// Counts an array of number of vertices to draw, or an array of
        ///number of indices to draw if this mesh has indices.
        /// </param>
        /// <param name='primCount'>
        /// Prim count  the number of parts of this mesh to draw.
        /// </param>
        /// <param name='bases'>
        /// Bases the base vertices to use. Only used for meshes with indices.
        /// </param>
        internal protected void multiDraw(MeshMode m, int[] firsts, int[] counts, int primCount, int[] bases = null)
        {
            if (CURRENT != this)
            {
                set();
            }
#if OPENTK
            if (primitiveRestart != CURRENT_RESTART_INDEX)
            {
                if (primitiveRestart >= 0)
                {
                    GL.Enable(EnableCap.PrimitiveRestart);
                    GL.PrimitiveRestartIndex((uint)(primitiveRestart));
                }
                else
                {
                    GL.Disable(EnableCap.PrimitiveRestart);
                }
                CURRENT_RESTART_INDEX = primitiveRestart;
            }
            if (patchVertices > 0 && patchVertices != CURRENT_PATCH_VERTICES)
            {
                GL.PatchParameter(PatchParameterInt.PatchVertices, patchVertices);
            }

            if (indicesBuffer == null)
            {
                GL.MultiDrawArrays(EnumConversion.getPrimitiveType(m), firsts, counts, primCount);
            }
            else
            {
                int size = indicesBuffer.getAttributeSize();
                uint[] indices = new uint[primCount];
                for (int i = 0; i < primCount; ++i)
                {
                    indices[i] = (uint)(firsts[i] * size + offset);
                }
                if (bases == null)
                {
                    GL.MultiDrawElements(EnumConversion.getPrimitiveType(m), counts, DrawElementsType.UnsignedInt, indices, primCount);
                }
                else
                {
                    GL.MultiDrawElementsBaseVertex(EnumConversion.getPrimitiveType(m), counts, DrawElementsType.UnsignedInt, indices, primCount, bases);
                }
            }
#else
                if (primitiveRestart != CURRENT_RESTART_INDEX)
            {
                if (primitiveRestart >= 0)
                {
                    glEnable(GL_PRIMITIVE_RESTART);
                    glPrimitiveRestartIndex(GLuint(primitiveRestart));
                }
                else
                {
                    glDisable(GL_PRIMITIVE_RESTART);
                }
                CURRENT_RESTART_INDEX = primitiveRestart;
            }
            if (patchVertices > 0 && patchVertices != CURRENT_PATCH_VERTICES)
            {
                glPatchParameteri(GL_PATCH_VERTICES, patchVertices);
            }

            if (indicesBuffer == null)
            {
                glMultiDrawArrays(getMeshMode(m), firsts, counts, primCount);
            }
            else
            {
#if TODO
            int size = indicesBuffer.getAttributeSize();
            GLvoid **indices = new void*[primCount];
            for (int i = 0; i < primCount; ++i) {
                indices[i] = (void*) (((unsigned char*) offset) + firsts[i] * size);
            }
            if (bases == null) {
                glMultiDrawElements(getMeshMode(m), counts, getAttributeType(type), (const GLvoid**) indices, primCount);
            } else {
                glMultiDrawElementsBaseVertex(getMeshMode(m), counts, getAttributeType(type), indices, primCount, bases);
            }
            //delete[] indices;
#endif
            }
#endif
#if  DEBUG
            ErrorCode err = FrameBuffer.getError();
            if (err != ErrorCode.NoError)
            {
                if (Program.CURRENT == null || Program.CURRENT.checkSamplers())
                {
                    log.Error("OpenGL error " + err);
                    Debug.Assert(err == 0);
                }
            }
#endif
        }


        /// <summary>
        ///  Draws a part of this mesh one or more times.
        /// </summary>
        /// <returns>
        /// The indirect.
        /// </returns>
        /// <param name='m'>
        /// M how the mesh vertices must be interpreted.
        /// </param>
        /// <param name='buf'>
        /// Buffer buf a CPU or GPU buffer containing the 'count', 'primCount',
        ///'first' and 'base' parameters, in this order, as 32 bit integers.
        /// </param>
        internal protected void drawIndirect(MeshMode m, Buffer buf)
        {
            if (CURRENT != this)
            {
                set();
            }

            if (primitiveRestart != CURRENT_RESTART_INDEX)
            {
                if (primitiveRestart >= 0)
                {
                    GL.Enable(EnableCap.PrimitiveRestart);
                    GL.PrimitiveRestartIndex((uint)(primitiveRestart));
                }
                else
                {
                    GL.Disable(EnableCap.PrimitiveRestart);
                }
                CURRENT_RESTART_INDEX = primitiveRestart;
            }
            if (patchVertices > 0 && patchVertices != CURRENT_PATCH_VERTICES)
            {
                GL.PatchParameter(PatchParameterInt.PatchVertices, patchVertices);
            }

            buf.bind(BufferTarget.DrawIndirectBuffer);
            if (indicesBuffer == null)
            {
                GL.DrawArraysIndirect(EnumConversion.getPrimitiveType(m), buf.data(0));
            }
            else
            {
                GL.DrawElementsIndirect(EnumConversion.getPrimitiveType(m), (All)EnumConversion.getDrawElementType(type), buf.data(0));
            }
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
            buf.unbind(BufferTarget.DrawIndirectBuffer);

            if (GL.GetError() != ErrorCode.NoError)
            {
                Debug.Assert(false);
                throw new Exception();
            }
        }

        /// <summary>
        /// The AttributeBuffer of this mesh.
        /// </summary>
        protected List<AttributeBuffer> attributeBuffers = new List<AttributeBuffer>();


        /// <summary>
        /// The indices buffer of this mesh.
        /// </summary>
        private AttributeBuffer indicesBuffer;


        /// <summary>
        /// The currently bound mesh buffers. The buffers of a mesh must be bound
        /// before it can be drawn.
        /// </summary>
        internal static MeshBuffers CURRENT;


        /// <summary>
        /// The current valueC of the primitive restart index.
        /// </summary>
        private static int CURRENT_RESTART_INDEX;


        /// <summary>
        /// The current valueC of the patch vertices parameter.
        /// </summary>
        private static int CURRENT_PATCH_VERTICES;


        /// <summary>
        /// The type of the indices of the currently bound mesh.
        /// </summary>
        private static AttributeType type;

        /// <summary>
        /// The offset of the indices of the currently bound mesh in its indices buffer.
        /// </summary>
        private static int offset;

        /// <summary>
        /// Binds the buffers of this mesh, so that it is ready to be drawn.
        /// </summary>
        private void bind()
        {
            Debug.Assert(attributeBuffers.Count > 0);
            // binds the attribute buffers for each attribute
            for (int i = (int)attributeBuffers.Count - 1; i >= 0; --i)
            {
                AttributeBuffer a = attributeBuffers[i];
                Buffer b = a.b;
                b.bind(BufferTarget.ArrayBuffer);
                int index = a.index;
#if OPENTK
                if (a.I)
                {
                    GL.VertexAttribIPointer(index, a.size, EnumConversion.getIAttributeType(a.type), a.stride, b.data(a.offset));
                }
                else if (a.L)
                {
                    GL.VertexAttribLPointer(index, a.size, EnumConversion.getDAttributeType(a.type), a.stride, b.data(a.offset));
                }
                else
                {
                    GL.VertexAttribPointer(index, a.size, EnumConversion.getAttributeType(a.type), a.norm, a.stride, a.offset);
                }
                GL.VertexAttribDivisor(index, a.divisor);
                GL.EnableVertexAttribArray(index);
#else
                if (a.I)
                {
                    glVertexAttribIPointer(index, a.size, getAttributeType(a.type), a.stride, b.data(a.offset));
                }
                else if (a.L)
                {
                    glVertexAttribLPointer(index, a.size, getAttributeType(a.type), a.stride, b.data(a.offset));
                }
                else
                {
                    glVertexAttribPointer(index, a.size, getAttributeType(a.type), a.norm, a.stride, b.data(a.offset));
                }
                glVertexAttribDivisor(index, a.divisor);
                glEnableVertexAttribArray(index);
#endif
            }
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
            // binds the indices buffer, if any
            if (indicesBuffer != null)
            {
                Buffer b = indicesBuffer.b;
                b.bind(BufferTarget.ElementArrayBuffer);
                type = indicesBuffer.type;
                //ORIGINAL CODE IN C++ offset = b.data(indicesBuffer.offset);
                offset = indicesBuffer.offset;
            }
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
        }


        /// <summary>
        /// Unbinds the buffers of this mesh, so that another mesh can be bound instead.
        /// </summary>
        private void unbind()
        {
            for (int i = attributeBuffers.Count - 1; i >= 0; --i)
            {
                AttributeBuffer a = attributeBuffers[i];
                int index = a.index;
#if OPENTK
                GL.DisableVertexAttribArray(index);
#else
                glDisableVertexAttribArray(index);
#endif
            }
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
        }


        /// <summary>
        /// Sets this mesh as the current one. Unbinds the currently bound mesh if
        /// necessary.
        /// </summary>
        private void set()
        {
            if (CURRENT != null)
            {
                CURRENT.unbind();
            }
            bind();
            CURRENT = this;
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
//#if DEBUG
//                traceDisposable.CheckDispose();
//#endif
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
                if (CURRENT == this)
                {
                    unbind();
                    CURRENT = null;
                }
                //Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);

                // Note disposing has been done.
                disposed = true;

            }
        }
        #endregion

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
