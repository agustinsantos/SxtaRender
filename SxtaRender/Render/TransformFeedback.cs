using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Sxta.Render
{
    /// <summary>
    /// A set of Buffer objects to collect the result of a transform feedback.
    /// In OpenGL 3.3 only a single TransformFeedback instance can be used, the
    /// one returned by#getDefault. With OpenGL 4 other instances can be used,
    /// which can be created with the constructor. In any case, only one transform
    /// feedback can be performed at a time, with the static #begin, #transform
    /// and #end methods.
    /// </summary>
    public class TransformFeedback
    {
        
		/// <summary>
		/// Creates a new TransformFeedback object. Only works with OpenGL 4.0 or
		/// Initializes a new instance of the <see cref="Sxta.Render.TransformFeedback"/> class.
		/// </summary>
        public TransformFeedback()
        {
#if OPENTK
            id=GL.GenTransformFeedback();
#else
            glGenTransformFeedbacks(1, &id);
#endif
        }

        
		/// <summary>
		/// Deletes this TransformFeedback object.
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="Sxta.Render.TransformFeedback"/> is reclaimed by garbage collection.
		/// </summary>
        ~TransformFeedback()
        {
            reset();
            if (id != 0)
            {
#if OPENTK
                GL.DeleteTransformFeedback(id);
#else
                glDeleteTransformFeedbacks(1, &id);
#endif
            }
        }


       
		/// <summary>
		/// Returns the default TransformFeedback instance.
		/// Gets the default.
		/// </summary>
		/// <returns>
		/// The default.
		/// </returns>
        public static TransformFeedback getDefault()
        {
            if (DEFAULT == null)
            {
                DEFAULT = new TransformFeedback(true);
            }
            return DEFAULT;
        }


       
		/// <summary>
		/// Removes all the buffers associated with this object.
		/// </summary>
        public void reset()
        {
            int n;
#if OPENTK
            GL.GetInteger(GetPName.MaxTransformFeedbackSeparateAttribs, out n);
#else
            glGetIntegerv(GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_ATTRIBS, &n);
#endif
            bind(id);
            for (int i = 0; i < n; ++i)
            {
#if OPENTK
                GL.BindBufferBase(BufferTarget.TransformFeedbackBuffer, i, 0);
#else
                glBindBufferBase(GL_TRANSFORM_FEEDBACK_BUFFER, i, 0);
#endif
            }
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
        }

        
		/// <summary>
		/// Attachs the given GPUBuffer to collect the transformed output varying
        /// of the given index.
		/// </summary>
		/// <returns>
		/// The vertex buffer.
		/// </returns>
		/// <param name='index'>
		/// Index the index of a recorded output varying variable.
		/// </param>
		/// <param name='b'>
		/// Bthe GPUBuffer to use to store the recorded values of this varying.
		/// </param>
        public void setVertexBuffer(uint index, GPUBuffer b)
        {
            bind(id);
#if OPENTK
            GL.BindBufferBase(BufferTarget.TransformFeedbackBuffer, index, b.getId());
#else
            glBindBufferBase(GL_TRANSFORM_FEEDBACK_BUFFER, index, b.getId());
#endif
        }

     
		/// <summary>
		/// Attachs the given GPUBuffer to collect the transformed output varying
        /// of the given index.
		/// </summary>
		/// <returns>
		/// The vertex buffer.
		/// </returns>
		/// <param name='index'>
		/// Index the index of a recorded output varying variable.
		/// </param>
		/// <param name='b'>
		/// B the GPUBuffer to use to store the recorded values of this varying.
		/// </param>
		/// <param name='offset'>
		/// Offset offset the offset at which the first recorded value must be stored.
		/// </param>
		/// <param name='size'>
		/// Size the maximum size of the recorded values.
		/// </param>
        public void setVertexBuffer(uint index, GPUBuffer b, uint offset, uint size)
        {
            bind(id);
#if OPENTK
            GL.BindBufferRange(BufferTarget.TransformFeedbackBuffer, index, b.getId(), (IntPtr)offset, (IntPtr)size);
#else
            glBindBufferRange(GL_TRANSFORM_FEEDBACK_BUFFER, index, b.getId(), GLintptr(offset), GLsizeiptr(size));
#endif
        }


    
		/// <summary>
		/// Starts a transform feedback session. Actual transforms are performed
        /// with the #transform methods. The transformation is ended with the #end()
        /// method. In OpenGL 4.0, #pause and #resume can be called between #begin()
        /// and #end(), for instance to change the TransformFeedback instance used
        /// to record the varyings.
		/// </summary>
		/// <param name='fb'>
		/// Fb.
		/// </param>
		/// <param name='transform'>
		/// Transform.
		/// </param>
		/// <param name='m'>
		/// M.
		/// </param>
		/// <param name='tfb'>
		/// Tfb.
		/// </param>
		/// <param name='rasterize'>
		/// Rasterize.
		/// </param>
        public static void begin(FrameBuffer fb, Program transform, MeshMode m, TransformFeedback tfb, bool rasterize)
        {
            Debug.Assert(m == MeshMode.POINTS || m == MeshMode.LINES || m == MeshMode.TRIANGLES);
            TRANSFORMFEEDBACK_FRAMEBUFFER = fb;
            TRANSFORM = transform;
            MODE = m;
            TRANSFORMFEEDBACK_FRAMEBUFFER.set();
            TRANSFORM.set();
            bind(tfb.id);
#if OPENTK
            GL.BeginTransformFeedback((BeginFeedbackMode)EnumConversion.getMeshMode(m));
            if (!rasterize)
            {
                GL.Enable(EnableCap.RasterizerDiscard);
            }
#else
            glBeginTransformFeedback(getMeshMode(m));
            if (!rasterize)
            {
                glEnable(GL_RASTERIZER_DISCARD);
            }
#endif
        }

     
		/// <summary>
		/// Transforms a part of a mesh one or more times.
		/// </summary>
		/// <param name='mesh'>
		/// Mesh the mesh to transform.
		/// </param>
		/// <param name='first'>
		/// First first the first vertex to draw, or the first indice to draw if
        /// this mesh has indices.
		/// </param>
		/// <param name='count'>
		/// Countcount the number of vertices to draw, or the number of indices
        /// to draw if this mesh has indices.
		/// </param>
		/// <param name='primCount'>
		/// Prim count primCount the number of times this mesh must be drawn (with
        /// geometry instancing).
		/// </param>
		/// <param name='base'>
		/// Base the base vertex to use. Only used for meshes with indices.
		/// </param>
        public static void transform(MeshBuffers mesh, int first, int count, int primCount = 1, int @base = 0)
        {
            TRANSFORMFEEDBACK_FRAMEBUFFER.set();
            TRANSFORM.set();
            TRANSFORMFEEDBACK_FRAMEBUFFER.beginConditionalRender();
            mesh.draw(MODE, first, count, primCount, @base);
            TRANSFORMFEEDBACK_FRAMEBUFFER.endConditionalRender();
        }

       
		/// <summary>
		/// Transforms several parts of a mesh. Each part is specified with a first
        /// and count parameter as in #transform(). These values are passed in arrays
        /// of primCount values.
		/// </summary>
		/// <returns>
		/// The transform.
		/// </returns>
		/// <param name='mesh'>
		/// Mesh the mesh to transform.
		/// </param>
		/// <param name='firsts'>
		/// Firsts firsts an array of primCount 'first vertex' to draw, or an array
        /// of 'first indice' to draw if this mesh has indices.
		/// </param>
		/// <param name='counts'>
		/// Counts counts an array of number of vertices to draw, or an array of
        /// number of indices to draw if this mesh has indices.
		/// </param>
		/// <param name='primCount'>
		/// Prim count primCount the number of parts of this mesh to draw.
		/// </param>
		/// <param name='bases'>
		/// Bases the base vertices to use. Only used for meshes with indices.
		/// </param>
        public static void multiTransform(MeshBuffers mesh, int[] firsts, int[] counts, int primCount, int[] @bases = null)
        {
            TRANSFORMFEEDBACK_FRAMEBUFFER.set();
            TRANSFORM.set();
            TRANSFORMFEEDBACK_FRAMEBUFFER.beginConditionalRender();
            mesh.multiDraw(MODE, firsts, counts, primCount, bases);
            TRANSFORMFEEDBACK_FRAMEBUFFER.endConditionalRender();
        }

      
		/// <summary>
		/// Transforms a part of a mesh one or more times.
		/// </summary>
		/// <returns>
		/// The indirect.
		/// </returns>
		/// <param name='mesh'>
		/// Mesh the mesh to transform.
		/// </param>
		/// <param name='buf'>
		/// Buffer buf a CPU or GPU buffer containing the 'count', 'primCount',
        /// 'first' and 'base' parameters, in this order, as 32 bit integers.
		/// </param>
        public static void transformIndirect(MeshBuffers mesh, Buffer buf)
        {
            TRANSFORMFEEDBACK_FRAMEBUFFER.set();
            TRANSFORM.set();
            TRANSFORMFEEDBACK_FRAMEBUFFER.beginConditionalRender();
            mesh.drawIndirect(MODE, buf);
            TRANSFORMFEEDBACK_FRAMEBUFFER.endConditionalRender();
        }

      
		/// <summary>
		/// Retransforms a mesh resulting from a previous transform feedback session.
		/// </summary>
		/// <returns>
		/// The feedback Only available with OpenGL 4.0 or more.
		/// </returns>
		/// <param name='tfb'>
		/// Tfb tfb a TransformFeedback containing the results of a previous transform
        /// feedback session.
		/// </param>
		/// <param name='stream'>
		/// Stream stream the stream to draw.
		/// </param>
        public void transformFeedback(TransformFeedback tfb, int stream = 0)
        {
            TRANSFORMFEEDBACK_FRAMEBUFFER.set();
            TRANSFORM.set();
            TRANSFORMFEEDBACK_FRAMEBUFFER.beginConditionalRender();
#if OPENTK
            GL.DrawTransformFeedbackStream(EnumConversion.getMeshMode(MODE), tfb.id, stream);
#else
            glDrawTransformFeedbackStream(getMeshMode(MODE), tfb.id, stream);
#endif
            TRANSFORMFEEDBACK_FRAMEBUFFER.endConditionalRender();
        }


		/// <summary>
		/// Pauses the current transform feedback session.
        /// Only available with OpenGL 4.0 or more.
		/// </summary>
        public static void pause()
        {
#if OPENTK
            GL.PauseTransformFeedback();
#else
            glPauseTransformFeedback();
#endif
        }

		/// <summary>
		/// Resumes the current transform feedback session.
        /// Only available with OpenGL 4.0 or more.
		/// </summary>
		/// <param name='tfb'>
		/// Tfb  tfb the set of buffers to use to store the results of the session,
        /// i.e., the transformed output varying variables.
		/// </param>
        public static void resume(TransformFeedback tfb)
        {
            TRANSFORMFEEDBACK_FRAMEBUFFER.set();
            TRANSFORM.set();
            bind(tfb.id);
#if OPENTK
            GL.ResumeTransformFeedback();
#else
            glResumeTransformFeedback();
#endif
        }
       
		/// <summary>
		/// Ends the current transform feedback session.
		/// </summary>
        public static void end()
        {
#if OPENTK
            GL.Disable(EnableCap.RasterizerDiscard);
            GL.EndTransformFeedback();
#else
            glDisable(GL_RASTERIZER_DISCARD);
            glEndTransformFeedback();
#endif
            TRANSFORMFEEDBACK_FRAMEBUFFER = null;
            TRANSFORM = null;
        }

		/// <summary>
		/// The id of this transform feedback object.
		/// </summary>
        internal int id;

		/// <summary>
		/// The default transform feedback instance.
		/// </summary>
        private static TransformFeedback DEFAULT;

       
		/// <summary>
		/// The program to use for the current transform feedback session.
		/// </summary>
        internal static Program TRANSFORM;

		/// <summary>
		/// How the mesh vertices must be interpreted in #transform methods.
		/// </summary>
        private static MeshMode MODE;

        private static FrameBuffer TRANSFORMFEEDBACK_FRAMEBUFFER;

       
		/// <summary>
		/// Creates a new TransformFeedback object.
		/// Initializes a new instance of the <see cref="Sxta.Render.TransformFeedback"/> class.
		/// </summary>
		/// <param name='main'>
		/// Main true to create the default instance.
		/// </param>
        private TransformFeedback(bool main)
        {
            if (main)
            {
                id = 0;
            }
            else
            {
#if OPENTK
                id=GL.GenTransformFeedback();
#else
                glGenTransformFeedbacks(1, &id);
#endif
            }
        }

       
		/// <summary>
		/// Binds the transform feedback object whose id is given.
		/// </summary>
		/// <param name='id'>
		/// Identifier.
		/// </param>
        private static void bind(int id)
        {
#if OPENTK
            int v;
            GL.GetInteger(GetPName.MajorVersion, out v);
            if (v >= 4)
            {
                GL.BindTransformFeedback(TransformFeedbackTarget.TransformFeedback, id);
            }
#else
            int v;
            glGetIntegerv(GL_MAJOR_VERSION, &v);
            if (v >= 4)
            {
                glBindTransformFeedback(GL_TRANSFORM_FEEDBACK, id);
            }
#endif
        }
    }
}
