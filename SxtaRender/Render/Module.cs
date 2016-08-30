using log4net;
using OpenTK.Graphics.OpenGL;
using Sxta.Core;
using Sxta.Render.Core;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Sxta.Render
{
    /// <summary>
    /// A module made of a vertex, a tesselation, a geometry, and a fragment shader
    /// parts. All parts are optional. These parts must be defined either each in its
    /// own GLSL compilation unit, or all grouped in a single compilation unit but
    /// separated with the following preprocessor directives:
    ///@verbatim
    /// ... common code ...
    ///#ifdef _VERTEX_
    /// ... vertex shader code ...
    ///#endif
    ///#ifdef _TESS_CONTROL_
    /// ... tessellation control shader code ...
    ///#endif
    ///#ifdef _TESS_EVAL_
    /// ... tessellation evaluation shader code ...
    ///#endif
    ///#ifdef _GEOMETRY_
    /// ... geometry shader code ...
    ///#endif
    ///#ifdef _FRAGMENT_
    /// ... fragment shader code ...
    ///#endif
    ///@endverbatim
    /// A module can specify some initial values for its uniform variables, and
    /// can also specify which output varying variable must be recorded in transform
    /// feedback mode.
    /// </summary>
    public class Module : IDisposable, ISwappable<Module>
    {
//#if DEBUG
//        TraceOpenTKDisposableObject traceDisposable;
//#endif

        /// <summary>
        /// Creates a new module whose parts are all grouped in a single compilation
        /// unit, but separated with preprocessor directives (see #Module).
        /// Initializes a new instance of the <see cref="Sxta.Render.Module"/> class.
        /// </summary>
        /// <param name='version'>
        /// Version.
        /// </param>
        /// <param name='source'>
        /// Source.
        /// </param>
        public Module(int version, string source):this()
        {
            init(version, source);
        }

        /// <summary>
        /// Creates a new module whose parts are defined in separate compilation units.
        /// Initializes a new instance of the <see cref="Sxta.Render.Module"/> class.
        /// </summary>
        /// <param name='version'>
        /// Version the GLSL version used for the source code.
        /// </param>
        /// <param name='vertex'>
        /// Vertex shader source code (maybe null).
        /// </param>
        /// <param name='fragment'>
        /// Fragment the fragment shader source code (maybe null)
        /// </param>
        public Module(int version, string vertex, string fragment) : this()
        {
            init(version, null, vertex, null, null, null, null, null, null, null, fragment);
        }


        /// <summary>
        /// Creates a new module whose parts are defined in separate compilation units.
        /// Initializes a new instance of the <see cref="Sxta.Render.Module"/> class.
        /// </summary>
        /// <param name='version'>
        /// Version GLSL version used for the source code.
        /// </param>
        /// <param name='vertex'>
        /// Vertex shader source code (maybe null).
        /// </param>
        /// <param name='geometry'>
        /// Geometry  shader source code (maybe null).
        /// </param>
        /// <param name='fragment'>
        /// Fragment shader source code (maybe null).
        /// </param>
        public Module(int version, string vertex, string geometry, string fragment) : this()
        {
            init(version, null, vertex, null, null, null, null, null, geometry, null, fragment);
        }


        /// <summary>
        /// Creates a new module whose parts are defined in separate compilation units.
        /// Initializes a new instance of the <see cref="Sxta.Render.Module"/> class.
        /// </summary>
        /// <param name='version'>
        /// Version the GLSL version used for the source code.
        /// </param>
        /// <param name='vertex'>
        /// Vertex the vertex shader source code (maybe null).
        /// </param>
        /// <param name='tessControl'>
        /// Tess control the tessellation control shader source code (maybe null).
        /// </param>
        /// <param name='tessEvaluation'>
        /// Tess evaluation the tessellation evaluation shader source code (maybe null).
        /// </param>
        /// <param name='geometry'>
        /// Geometry the geometry shader source code (maybe null).
        /// </param>
        /// <param name='fragment'>
        /// Fragment  the fragment shader source code (maybe null).
        /// </param>
        public Module(int version, string vertex, string tessControl,
            string tessEvaluation, string geometry, string fragment) : this()
        {
            init(version, null, vertex, null, tessControl, null, tessEvaluation, null, geometry, null, fragment);
        }

        /*
         * Deletes this module.  ----?¿
         */

        ~Module()
        {
            // Do not re-create Dispose clean-up code here. 
            // Calling Dispose(false) is optimal in terms of 
            // readability and maintainability.
            Dispose(false);
        }

        //Resource name if any
        public string Name { get; set; }


        /// <summary>
        /// Returns the id of the vertex shader part of this module.
        /// Gets the vertex shader identifier.
        /// </summary>
        /// <returns>
        /// the id of the vertex shader part of this module, or -1
        /// if this module does not have a vertex shader.
        /// The vertex shader identifier.
        /// </returns>
        public int getVertexShaderId()
        {
            return vertexShaderId;
        }


        /// <summary>
        /// Returns the id of the tessellation control shader part of this shader.
        /// Gets the tess control shader identifier.
        /// </summary>
        /// <returns>
        /// The tess control shader identifier.
        /// </returns>
        public int getTessControlShaderId()
        {
            return tessControlShaderId;
        }


        /// <summary>
        /// Returns the id of the tessellation evaluation shader part of this shader.
        /// </summary>
        /// <returns>
        /// The tess eval shader identifier.
        /// </returns>
        public int getTessEvalShaderId()
        {
            return tessEvalShaderId;
        }


        /// <summary>
        /// Gets the geometry shader identifier.
        /// </summary>
        /// <returns>
        /// The geometry shader identifier.
        /// </returns>
        public int getGeometryShaderId()
        {
            return geometryShaderId;
        }


        /// <summary>
        /// Returns the id of the fragment shader part of this shader.
        /// </summary>
        /// <returns>
        /// The fragment shader identifier.
        /// </returns>
        public int getFragmentShaderId()
        {
            return fragmentShaderId;
        }


        /// <summary>
        /// Returns the programs that use this Module.
        /// </summary>
        /// <returns>
        /// The users.
        /// </returns>
        public ISet<Program> getUsers()
        {
            return users;
        }


        /// <summary>
        /// Sets the format to use when a Program using this module is
        /// used in transform feedback.
        /// </summary>
        /// <returns>
        /// The feedback mode.
        /// </returns>
        /// <param name='interleaved'>
        /// Interleaved true to interleave the recorded output varying
        /// variables in a single buffer, or false to record each output
        /// variable in a separate buffer.
        /// </param>
        public void setFeedbackMode(bool interleaved)
        {
            feedbackMode = interleaved ? 1 : 2;
        }

        /// <summary>
        /// Adds an output varying variable that must be recorded in transform
        /// feedback mode. The order of these variables is important: they are
        /// recorded in the same order as they as declared with this method.
        /// </summary>
        /// <returns>
        /// The feedback varying.
        /// </returns>
        /// <param name='name'>
        /// Name the name of an output varying variable to record.
        /// </param>
        public void addFeedbackVarying(string name)
        {
            feedbackVaryings.Add(name);
        }


        /// <summary>
        /// Adds an initial valueC for the given uniform variable.
        /// </summary>
        /// <returns>
        /// The initial value.
        /// </returns>
        /// <param name='value'>
        /// valueC an initial valueC for an uniform of this module.
        /// </param>
        public void addInitialValue(Value value)
        {
            initialValues.Add(value.getName(), value);
        }



        /// <summary>
        /// Creates an uninitialized module.
        /// Initializes a new instance of the <see cref="Sxta.Render.Module"/> class.
        /// </summary>
        public Module()
        {
//#if DEBUG
//            traceDisposable = new TraceOpenTKDisposableObject();
//#endif
        }


        /// <summary>
        /// Initializes this module, with parts that are all grouped in a single
        /// compilation unit, but separated with preprocessor directives (see #Module).
        /// </summary>
        /// <param name='version'>
        /// Version.
        /// </param>
        /// <param name='source'>
        /// Source.
        /// </param>
        internal protected void init(int version, string source)
        {
            init(version,
                "#define _VERTEX_\n", source.Contains("_VERTEX_") ? source : null,
                "#define _TESS_CONTROL_\n", source.Contains("_TESS_CONTROL_") ? source : null,
                "#define _TESS_EVAL_", source.Contains("_TESS_EVAL_") ? source : null,
                "#define _GEOMETRY_\n", source.Contains("_GEOMETRY_") ? source : null,
                "#define _FRAGMENT_\n", source.Contains("_FRAGMENT_") ? source : null);
        }

        /// <summary>
        /// Initializes this module.
        /// Init the specified version, vertexHeader, vertex, tessControlHeader, tessControl, tessEvaluationHeader,
        /// tessEvaluation, geometryHeader, geometry, fragmentHeader and fragment.
        /// </summary>
        /// <param name='version'>
        /// Version the GLSL version used for the source code.
        /// </param>
        /// <param name='vertexHeader'>
        /// Vertex header an optional header for the the vertex shader source
        /// code (maybe null).
        /// </param>
        /// <param name='vertex'>
        /// Vertex the vertex shader source code (maybe null).
        /// </param>
        /// <param name='tessControlHeader'>
        /// Tess control header  an optional header for the the tessellation
        /// control shader source code (maybe null).
        /// </param>
        /// <param name='tessControl'>
        /// Tess control the tessellation control shader source code (maybe null).
        /// </param>
        /// <param name='tessEvaluationHeader'>
        /// Tess evaluation header an optional header for the the tessellation
        /// evaluation shader source code (maybe null).
        /// </param>
        /// <param name='tessEvaluation'>
        /// Tess evaluation the tessellation evaluation shader source code (maybe null).
        /// </param>
        /// <param name='geometryHeader'>
        /// Geometry header geometryHeader an optional header for the the geometry shader
        /// source code (maybe null).
        /// </param>
        /// <param name='geometry'>
        /// Geometry, the geometry shader source code (maybe null).
        /// </param>
        /// <param name='fragmentHeader'>
        /// Fragment header an optional header for the the fragment shader
        /// source code (maybe null).
        /// </param>
        /// <param name='fragment'>
        /// Fragment the fragment shader source code (maybe null).
        /// </param>
        internal protected void init(int version,
            string vertexHeader, string vertex,
            string tessControlHeader, string tessControl,
            string tessEvaluationHeader, string tessEvaluation,
            string geometryHeader, string geometry,
            string fragmentHeader, string fragment)
        {
            int lineCount;
            string[] lines = new string[3];

            string versionLine = "#version " + version + "\n";

            lines[0] = versionLine;

            bool error;

            int glVersion;
#if OPENTK
            GL.GetInteger(GetPName.MajorVersion, out glVersion);
#else
            glGetIntegerv(GL_MAJOR_VERSION, &glVersion);
#endif
            //compiles and checks the vertex shader part
            if (vertex != null)
            {
                lineCount = vertexHeader != null ? 3 : 2;
                lines[1] = lineCount == 3 ? vertexHeader : vertex;
                lines[2] = vertex;
#if OPENTK
                string source = "";
                for (int i = 0; i < lineCount; i++)
                    source += lines[i];
                vertexShaderId = GL.CreateShader(ShaderType.VertexShader);
                GL.ShaderSource(vertexShaderId, source); //lineCount, lines, ref len);
                GL.CompileShader(vertexShaderId);
#else
                vertexShaderId = glCreateShader(GL_VERTEX_SHADER);
                glShaderSource(vertexShaderId, lineCount, lines, null);
                glCompileShader(vertexShaderId);
#endif
                error = !check(vertexShaderId);
                string shaderlog = printLog(vertexShaderId, lineCount, lines, error);
                if (error)
                {
#if OPENTK
                    GL.DeleteShader(vertexShaderId);
#else
                    // deletes already allocated objects
                    glDeleteShader(vertexShaderId);
#endif
                    vertexShaderId = -1;
                    Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
                    throw new Exception(shaderlog);
                }
            }
            else
            {
                vertexShaderId = -1;
            }

            //compiles and checks the tessellation control shader part
            if (tessControl != null && glVersion >= 4)
            {
                lineCount = tessControlHeader != null ? 3 : 2;
                lines[1] = lineCount == 3 ? tessControlHeader : tessControl;
                lines[2] = tessControl;
#if OPENTK
                string source = "";
                for (int i = 0; i < lineCount; i++)
                    source += lines[i];
                tessControlShaderId = GL.CreateShader(ShaderType.TessControlShader);
                GL.ShaderSource(tessControlShaderId, source);// lineCount, lines, ref len);
                GL.CompileShader(tessControlShaderId);
#else
                tessControlShaderId = glCreateShader(GL_TESS_CONTROL_SHADER);
                glShaderSource(tessControlShaderId, lineCount, lines, null);
                glCompileShader(tessControlShaderId);
#endif
                error = !check(tessControlShaderId);
                string shaderlog = printLog(tessControlShaderId, lineCount, lines, error);
                if (error)
                {
                    // deletes already allocated objects
#if OPENTK
                    if (vertexShaderId != -1)
                    {
                        GL.DeleteShader(vertexShaderId);
                        vertexShaderId = -1;
                    }
                    GL.DeleteShader(tessControlShaderId);
#else
                    if (vertexShaderId != -1)
                    {
                        glDeleteShader(vertexShaderId);
                        vertexShaderId = -1;
                    }
                    glDeleteShader(tessControlShaderId);
#endif
                    tessControlShaderId = -1;
                    Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
                    throw new Exception(shaderlog);
                }
            }
            else
            {
                tessControlShaderId = -1;
            }

            //compiles and checks the tessellation evaluation shader part
            if (tessEvaluation != null && glVersion >= 4)
            {
                lineCount = tessEvaluationHeader != null ? 3 : 2;
                lines[1] = lineCount == 3 ? tessEvaluationHeader : tessEvaluation;
                lines[2] = tessEvaluation;
#if OPENTK
                string source = "";
                for (int i = 0; i < lineCount; i++)
                    source += lines[i];
                tessEvalShaderId = GL.CreateShader(ShaderType.TessEvaluationShader);
                GL.ShaderSource(tessEvalShaderId, source); // lineCount, lines, ref len);
                GL.CompileShader(tessEvalShaderId);
#else
                tessEvalShaderId = glCreateShader(GL_TESS_EVALUATION_SHADER);
                glShaderSource(tessEvalShaderId, lineCount, lines, null);
                glCompileShader(tessEvalShaderId);
#endif
                error = !check(tessEvalShaderId);
                string shaderlog = printLog(tessEvalShaderId, lineCount, lines, error);
                if (error)
                {
#if OPENTK
                    // deletes already allocated objects
                    if (vertexShaderId != -1)
                    {
                        GL.DeleteShader(vertexShaderId);
                        vertexShaderId = -1;
                    }
                    if (tessControlShaderId != -1)
                    {
                        GL.DeleteShader(tessControlShaderId);
                        geometryShaderId = -1;
                    }
                    GL.DeleteShader(tessEvalShaderId);
#else
                    // deletes already allocated objects
                    if (vertexShaderId != -1)
                    {
                        glDeleteShader(vertexShaderId);
                        vertexShaderId = -1;
                    }
                    if (tessControlShaderId != -1)
                    {
                        glDeleteShader(tessControlShaderId);
                        geometryShaderId = -1;
                    }
                    glDeleteShader(tessEvalShaderId);
#endif
                    tessEvalShaderId = -1;
                    Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
                    throw new Exception(shaderlog);
                }
            }
            else
            {
                tessEvalShaderId = -1;
            }

            //compiles and checks the geometry shader part
            if (geometry != null)
            {
                lineCount = geometryHeader != null ? 3 : 2;
                lines[1] = lineCount == 3 ? geometryHeader : geometry;
                lines[2] = geometry;
#if OPENTK
                string source = "";
                for (int i = 0; i < lineCount; i++)
                    source += lines[i];
                geometryShaderId = GL.CreateShader(ShaderType.GeometryShader);
                GL.ShaderSource(geometryShaderId, source); // lineCount, lines, ref len);
                GL.CompileShader(geometryShaderId);
#else
                geometryShaderId = glCreateShader(GL_GEOMETRY_SHADER);
                glShaderSource(geometryShaderId, lineCount, lines, null);
                glCompileShader(geometryShaderId);
#endif
                error = !check(geometryShaderId);
                string shaderlog = printLog(geometryShaderId, lineCount, lines, error);
                if (error)
                {
#if OPENTK
                    // deletes already allocated objects
                    if (vertexShaderId != -1)
                    {
                        GL.DeleteShader(vertexShaderId);
                        vertexShaderId = -1;
                    }
                    if (tessControlShaderId != -1)
                    {
                        GL.DeleteShader(tessControlShaderId);
                        geometryShaderId = -1;
                    }
                    if (tessEvalShaderId != -1)
                    {
                        GL.DeleteShader(tessEvalShaderId);
                        tessEvalShaderId = -1;
                    }
                    GL.DeleteShader(geometryShaderId);
#else
                   // deletes already allocated objects
                    if (vertexShaderId != -1)
                    {
                        glDeleteShader(vertexShaderId);
                        vertexShaderId = -1;
                    }
                    if (tessControlShaderId != -1)
                    {
                        glDeleteShader(tessControlShaderId);
                        geometryShaderId = -1;
                    }
                    if (tessEvalShaderId != -1)
                    {
                        glDeleteShader(tessEvalShaderId);
                        tessEvalShaderId = -1;
                    }
                    glDeleteShader(geometryShaderId);
#endif
                    geometryShaderId = -1;
                    Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
                    throw new Exception(shaderlog);
                }
            }
            else
            {
                geometryShaderId = -1;
            }

            //and finally compiles and checks the fragment shader part
            if (fragment != null)
            {
                lineCount = fragmentHeader != null ? 3 : 2;
                lines[1] = lineCount == 3 ? fragmentHeader : fragment;
                lines[2] = fragment;
#if OPENTK
                string source = "";
                for (int i = 0; i < lineCount; i++)
                    source += lines[i];
                fragmentShaderId = GL.CreateShader(ShaderType.FragmentShader);
                GL.ShaderSource(fragmentShaderId, source);// lineCount, lines, ref len);
                GL.CompileShader(fragmentShaderId);
#else
                fragmentShaderId = glCreateShader(GL_FRAGMENT_SHADER);
                glShaderSource(fragmentShaderId, lineCount, lines, null);
                glCompileShader(fragmentShaderId);
#endif
                error = !check(fragmentShaderId);
                string shaderlog = printLog(fragmentShaderId, lineCount, lines, error);
                if (error)
                {
#if OPENTK
                    // deletes already allocated objects
                    if (vertexShaderId != -1)
                    {
                        GL.DeleteShader(vertexShaderId);
                        vertexShaderId = -1;
                    }
                    if (tessControlShaderId != -1)
                    {
                        GL.DeleteShader(tessControlShaderId);
                        geometryShaderId = -1;
                    }
                    if (tessEvalShaderId != -1)
                    {
                        GL.DeleteShader(tessEvalShaderId);
                        tessEvalShaderId = -1;
                    }
                    if (geometryShaderId != -1)
                    {
                        GL.DeleteShader(geometryShaderId);
                        geometryShaderId = -1;
                    }
                    GL.DeleteShader(fragmentShaderId);
#else
                   // deletes already allocated objects
                    if (vertexShaderId != -1)
                    {
                        glDeleteShader(vertexShaderId);
                        vertexShaderId = -1;
                    }
                    if (tessControlShaderId != -1)
                    {
                        glDeleteShader(tessControlShaderId);
                        geometryShaderId = -1;
                    }
                    if (tessEvalShaderId != -1)
                    {
                        glDeleteShader(tessEvalShaderId);
                        tessEvalShaderId = -1;
                    }
                    if (geometryShaderId != -1)
                    {
                        glDeleteShader(geometryShaderId);
                        geometryShaderId = -1;
                    }
                    glDeleteShader(fragmentShaderId);
#endif
                    fragmentShaderId = -1;
                    Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
                    throw new Exception(shaderlog);
                }
            }
            else
            {
                fragmentShaderId = -1;
            }

#if OPENTK
            if (GL.GetError() != ErrorCode.NoError)
#else
            if (glGetError() != 0)
#endif
            {
                Debug.Assert(false);
                throw new Exception();
            }

            feedbackMode = 0;
        }

        /*
         * Swaps this module with the given one.
         */
        public virtual void swap(Module s)
        {
            Std.Swap<int>(ref vertexShaderId, ref s.vertexShaderId);
            Std.Swap<int>(ref tessControlShaderId, ref s.tessControlShaderId);
            Std.Swap<int>(ref tessEvalShaderId, ref s.tessEvalShaderId);
            Std.Swap<int>(ref geometryShaderId, ref s.geometryShaderId);
            Std.Swap<int>(ref fragmentShaderId, ref s.fragmentShaderId);
            Std.Swap<IDictionary<string, Value>>(ref initialValues, ref s.initialValues);
        }


        /// <summary>
        /// The Program instances that use this Module.
        /// </summary>
        internal ISet<Program> users = new HashSet<Program>();


        /// <summary>
        /// The id of the vertex shader part of this shader.
        /// </summary>
        internal int vertexShaderId;


        /// <summary>
        /// The id of the tessellation control shader part of this shader.
        /// </summary>
        internal int tessControlShaderId;


        /// <summary>
        /// The id of the tessellation evaluation shader part of this shader.
        /// </summary>
        internal int tessEvalShaderId;


        /// <summary>
        /// The id of the geometry shader part of this shader.
        /// </summary>
        internal int geometryShaderId;


        /// <summary>
        /// The id of the fragment shader part of this shader.
        /// </summary>
        internal int fragmentShaderId;

        /// <summary>
        /// The transform feedback mode to use with this module.
        /// 0 means 'any mode', 1 means 'interleaved attribs', 2 means 'separate attribs'.
        /// </summary>
        internal int feedbackMode;


        /// <summary>
        /// The output varying variables of this module that must be recorded in
        /// transform feedback mode.
        /// </summary>
        internal List<string> feedbackVaryings = new List<string>();


        /// <summary>
        /// The initial values for the uniforms of the shaders of this module.
        /// </summary>
        internal IDictionary<string, Value> initialValues = new Dictionary<string, Value>();


        /// <summary>
        /// Checks if a shader part has been correctly compiled.
        /// </summary>
        /// <param name='shaderId'>
        /// Shader identifierthe id the shader part to check.
        /// </param>
        private bool check(int shaderId)
        {
            int compiled;
#if OPENTK
            GL.GetShader(shaderId, ShaderParameter.CompileStatus, out compiled);
#else
            glGetShaderiv(shaderId, GL_COMPILE_STATUS, &compiled);
#endif
            return compiled != 0;
        }


        /// <summary>
        /// Logs the shader compiler output.
        /// </summary>
        /// <returns>
        /// The log.
        /// </returns>
        /// <param name='shaderId'>
        /// Shader identifier the id of the vertex, geometry or fragment part.
        /// </param>
        /// <param name='nlines'>
        /// Nlines number of lines in the compiler output text.
        /// </param>
        /// <param name='lines'>
        /// Lines the compiler output text.
        /// </param>
        /// <param name='error'>
        /// Error true if the compiler found some errors.
        /// </param>
        private string printLog(int shaderId, int nlines, string[] lines, bool error)
        {
            string shaderlog;
#if OPENTK
            GL.GetShaderInfoLog(shaderId, out shaderlog);
#else
            int logLength;
            glGetShaderiv(shaderId, GL_INFO_LOG_LENGTH, &logLength);
            glGetShaderInfoLog(shaderId, GLsizei(logLength), &length, shaderlog);
#endif
            if (shaderlog.Length > 1)
            {
                string msg;
                int l = 1;
                msg = "\033" + "\n";
                msg += l + ": ";
                for (int i = 0; i < nlines; ++i)
                {
                    int j = 0;
                    while (j < lines[i].Length)
                    {
                        if (lines[i][j] != '\r')
                        {
                            msg += lines[i][j];
                        }
                        if (lines[i][j] == '\n')
                        {
                            ++l;
                            msg += l + ": ";
                        }
                        ++j;
                    }
                }
                msg += "\n" + shaderlog;

                msg += "\033";
                if (error)
                    log.Error(msg);
                else
                    log.Info(msg);
            }

            return shaderlog;
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
                    users.Clear();
                    feedbackVaryings.Clear();
                    initialValues.Clear();
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.

#if OPENTK
                if (vertexShaderId != -1)
                {
                    GL.DeleteShader(vertexShaderId);
                }
                if (tessControlShaderId != -1)
                {
                    GL.DeleteShader(tessControlShaderId);
                }
                if (tessEvalShaderId != -1)
                {
                    GL.DeleteShader(tessEvalShaderId);
                }
                if (geometryShaderId != -1)
                {
                    GL.DeleteShader(geometryShaderId);
                }
                if (fragmentShaderId != -1)
                {
                    GL.DeleteShader(fragmentShaderId);
                }
#else
            if (vertexShaderId != -1)
            {
                glDeleteShader(vertexShaderId);
            }
            if (tessControlShaderId != -1)
            {
                glDeleteShader(tessControlShaderId);
            }
            if (tessEvalShaderId != -1)
            {
                glDeleteShader(tessEvalShaderId);
            }
            if (geometryShaderId != -1)
            {
                glDeleteShader(geometryShaderId);
            }
            if (fragmentShaderId != -1)
            {
                glDeleteShader(fragmentShaderId);
            }
#endif
                Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);

                // Note disposing has been done.
                disposed = true;

            }
        }
        #endregion
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }


}
