using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxta.Render
{
    /// <summary>
    /// An asynchronous GPU query. A query measures some valueC, depending on its type,
    /// between the calls to #begin() and #end(). After #end() has been called, the
    /// result is available asynchronously. Its availability can be tested with
    /// #available(), and its valueC with #getResult().
    /// </summary>
    public class Query
    {
        /// <summary>
        /// Creates a new Query of the given type.
        /// </summary>
        /// <param name="type">the query type</param>
        public Query(QueryType type)
        {
            this.type = type;
            resultAvailable = false;
            resultRead = false;
#if OPENGL
            glGenQueries(1, &id);
#else
            GL.GenQueries(1, out id);
#endif
            switch (type)
            {
                case QueryType.PRIMITIVES_GENERATED:
                    target = QueryTarget.PrimitivesGenerated;
                    break;
                case QueryType.TRANSFORM_FEEDBACK_PRIMITIVES_WRITTEN:
                    target = QueryTarget.TransformFeedbackPrimitivesWritten;
                    break;
                case QueryType.SAMPLES_PASSED:
                    target = QueryTarget.SamplesPassed;
                    break;
                case QueryType.ANY_SAMPLES_PASSED:
                    target = QueryTarget.AnySamplesPassed;
                    break;
                case QueryType.TIME_STAMP:
                    target = QueryTarget.TimeElapsed;
                    break;
            }
        }

     
		/// <summary>
		/// Deletes this query.
		/// Releases unmanaged resources and performs other cleanup operations before the <see cref="Sxta.Render.Query"/> is
		/// reclaimed by garbage collection.
		/// </summary>
         ~Query()
        {
#if OPENGL
            glDeleteQueries(1, &id);
#else
            GL.DeleteQueries(1, ref id);
#endif
        }

      
		/// <summary>
		/// Returns the type of this query.
		/// </summary>
		/// <returns>
		/// The type.
		/// </returns>
        public QueryType getType()
        {
            return type;
        }

   
		/// <summary>
		/// Returns the id of this query.
		/// Gets the identifier.
		/// </summary>
		/// <returns>
		/// The identifier.
		/// </returns>
        public uint getId()
        {
            return id;
        }

       
		/// <summary>
		/// Starts this query.
		/// </summary>
        public void begin()
        {
#if OPENGL
            glBeginQuery(target, id);
            if (target == GL_TIME_ELAPSED)
            {
                glQueryCounter(id, GL_TIMESTAMP);
            }
#else
            GL.BeginQuery(target, id);
            if (target == QueryTarget.TimeElapsed)
            {
                GL.QueryCounter(id, QueryCounterTarget.Timestamp);
            }
#endif
        }

      
		/// <summary>
		/// End this query.
		/// </summary>
        public void end()
        {
#if OPENGL
            glEndQuery(target);
#else
            GL.EndQuery(target);
#endif
        }

    
		/// <summary>
		/// Returns true if the result of this query is available.
		/// </summary>
        public bool available()
        {
            if (!resultAvailable)
            {
                uint result;
#if OPENGL
                glGetQueryObjectuiv(id, GL_QUERY_RESULT_AVAILABLE, &result);
#else
                GL.GetQueryObject(id, GetQueryObjectParam.QueryResultAvailable, out result);
#endif
                resultAvailable = result != 0;
            }
            return resultAvailable;
        }

        
		/// <summary>
		/// Returns the result of this query. This may block the caller
        ///  until the result is available.
		/// Gets the result.
		/// </summary>
		/// <returns>
		/// The result.
		/// </returns>
        public ulong getResult()
        {
            if (!resultRead)
            {
#if OPENGL
                glGetQueryObjectui64v(id, GL_QUERY_RESULT, &result);
#else
                GL.GetQueryObject(id, GetQueryObjectParam.QueryResult, out result);
#endif
                resultRead = true;
            }
            return result;
        }


      
		/// <summary>
		/// The type of this query.
		/// </summary>
        private QueryType type;

		/// <summary>
		///The OpenGL target for this query.
		/// </summary>
        private QueryTarget target;

      
		/// <summary>
		/// The id of this query.
		/// </summary>
        private uint id;

      
		/// <summary>
		/// TTrue if the result of this query is available.
		/// </summary>
        private bool resultAvailable;

      
		/// <summary>
		/// True if the result of this query had been read.
		/// </summary>
        private bool resultRead;

        
		/// <summary>
		/// The result of this query.
		/// </summary>
        private ulong result;
    }
}
