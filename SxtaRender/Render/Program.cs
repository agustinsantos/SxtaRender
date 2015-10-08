using log4net;
using OpenTK.Graphics.OpenGL;
using Sxta.Core;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Sxta.Render
{
    /// <summary>
    /// A GPU program. A GPU program can define vertex, tessellation, geometry and
    /// fragment programs. It is made of one or more Module, themselves made of one
    /// or more GLSL shaders.
    /// </summary>
    public class Program : ISwappable<Program>, IDisposable
    {
        
		/// <summary>
		/// Creates a new program.
		/// Initializes a new instance of the <see cref="Sxta.Render.Program"/> class.
		/// </summary>
		/// <param name='modules'>
		/// Modules the modules that will compose this program.
		/// </param>
        public Program(List<Module> modules)
        {
            init(modules);
        }


		/// <summary>
		/// Creates a new program.
		/// Initializes a new instance of the <see cref="Sxta.Render.Program"/> class.
		/// </summary>
		/// <param name='module'>
		/// module the single module that will compose this program
		/// </param>
        public Program(Module module)
        {
            List<Module> modules = new List<Module>() { module };
            init(modules);
        }

      
		/// <summary>
		/// Deletes this program.
		/// Releases unmanaged resources and performs other cleanup operations before the <see cref="Sxta.Render.Program"/> is
		/// reclaimed by garbage collection.
		/// </summary>
        ~Program()
        {
            // Do not re-create Dispose clean-up code here. 
            // Calling Dispose(false) is optimal in terms of 
            // readability and maintainability.
            Dispose(false);
        }

        public string Name { get; set; }

		/// <summary>
		/// Returns the id of this program.
		/// </summary>
		/// <returns>
		/// The identifier.
		/// </returns>
        public virtual int getId()
        {
            return programId;
        }

      
		/// <summary>
		/// Returns the number of Module objects in this program.
		/// </summary>
		/// <returns>
		/// The module count.
		/// </returns>
        public int getModuleCount()
        {
            return modules.Count;
        }

		/// <summary>
		/// Returns the Module of this program whose index is given.
		/// </summary>
		/// <returns>
		/// The module.
		/// </returns>
		/// <param name='index'>
		/// Index.
		/// </param>
        public Module getModule(int index)
        {
            return modules[index];
        }

       
		/// <summary>
		/// Returns the uniform of this program whose name is given.
		/// <returns>
		/// The uniform of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public Uniform getUniform(string name)
        {
            Uniform val;

            if (!uniforms.TryGetValue(name, out val))
            {
                log.Warn("Missing Uniform  " + name);
                return null;
            }
            return val;
        }

    
		/// <summary>
		/// Returns the uniform1f of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform1f.
		/// </returns>
		/// <param name='name'>
		/// name a GLSL uniform name.
		/// </param>
        public Uniform1f getUniform1f(string name)
        {
            return (Uniform1f)getUniform(name);
        }

		/// <summary>
		/// Returns the uniform1d of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform1d the uniform of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public Uniform1d getUniform1d(string name)
        {
            return (Uniform1d)getUniform(name);
        }

		/// <summary>
		/// Returns the uniform1i of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform1i of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public Uniform1i getUniform1i(string name)
        {
            return (Uniform1i)getUniform(name);
        }


		/// <summary>
		/// Returns the uniform1ui of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform1ui of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public Uniform1ui getUniform1ui(string name)
        {
            return (Uniform1ui)getUniform(name);
        }


		/// <summary>
		/// Returns the uniform1b of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform1b  the uniform of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name name a GLSL uniform name.
		/// </param>
        public Uniform1b getUniform1b(string name)
        {
            return (Uniform1b)getUniform(name);
        }

		/// <summary>
		/// Returns the uniform2f of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform2f the uniform of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public Uniform2f getUniform2f(string name)
        {
            return (Uniform2f)getUniform(name);
        }


		/// <summary>
		/// Returns the uniform2d of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform2d the uniform of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public Uniform2d getUniform2d(string name)
        {
            return (Uniform2d)getUniform(name);
        }

		/// <summary>
		/// Returns the uniform2i of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform2i  the uniform of this program whose name is given,
        ///  or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public Uniform2i getUniform2i(string name)
        {
            return (Uniform2i)getUniform(name);
        }

		/// <summary>
		/// Returns the uniform2ui of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform2ui of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public Uniform2ui getUniform2ui(string name)
        {
            return (Uniform2ui)getUniform(name);
        }

		/// <summary>
		/// Returns the uniform2b of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform2b the uniform of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public Uniform2b getUniform2b(string name)
        {
            return (Uniform2b)getUniform(name);
        }

		/// <summary>
		/// Returns the uniform3f of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform3f of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public Uniform3f getUniform3f(string name)
        {
            return (Uniform3f)getUniform(name);
        }

		/// <summary>
		/// Returns the uniform3d of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform3d of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public Uniform3d getUniform3d(string name)
        {
            return (Uniform3d)getUniform(name);
        }

		/// <summary>
		/// Returns the uniform3i of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform3i  of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public Uniform3i getUniform3i(string name)
        {
            return (Uniform3i)getUniform(name);
        }

        
		/// <summary>
		/// Returns the uniform3ui of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform3ui the uniform of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public Uniform3ui getUniform3ui(string name)
        {
            return (Uniform3ui)getUniform(name);
        }

		/// <summary>
		/// Returns the uniform3b of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform3b of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public Uniform3b getUniform3b(string name)
        {
            return (Uniform3b)getUniform(name);
        }

		/// <summary>
		/// Returns the uniform4f of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform4f of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public Uniform4f getUniform4f(string name)
        {
            return (Uniform4f)getUniform(name);
        }

		/// <summary>
		/// Returns the uniform4d of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform4d of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public Uniform4d getUniform4d(string name)
        {
            return (Uniform4d)getUniform(name);
        }

        
		/// <summary>
		/// Returns the uniform4i of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform4i of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public Uniform4i getUniform4i(string name)
        {
            return (Uniform4i)getUniform(name);
        }

     
		/// <summary>
		/// Returns the uniform4ui of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform4ui the uniform of this program whose name is given,
        ///  or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name  GLSL uniform name.
		/// </param>
        public Uniform4ui getUniform4ui(string name)
        {
            return (Uniform4ui)getUniform(name);
        }

  
		/// <summary>
		/// Returns the uniform4b of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform4b the uniform of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// name a GLSL uniform name.
		/// </param>
        public Uniform4b getUniform4b(string name)
        {
            return (Uniform4b)getUniform(name);
        }

		/// <summary>
		/// Returns the uniformMatrix2f of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix2f  the uniform of this program whose name is given,
        ///  or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// name a GLSL uniform name.
		/// </param>
        public UniformMatrix2f getUniformMatrix2f(string name)
        {
            return (UniformMatrix2f)getUniform(name);
        }

		/// <summary>
		/// Returns the uniformMatrix2d of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix2d the uniform of this program whose name is given,
        ///  or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// name a GLSL uniform name.
		/// </param>
        public UniformMatrix2d getUniformMatrix2d(string name)
        {
            return (UniformMatrix2d)getUniform(name);
        }

        
		/// <summary>
		/// Returns the uniformMatrix3f of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix3f  the uniform of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public UniformMatrix3f getUniformMatrix3f(string name)
        {
            return (UniformMatrix3f)getUniform(name);
        }

       
		/// <summary>
		/// Returns the uniformMatrix3d of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix3d f this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public UniformMatrix3d getUniformMatrix3d(string name)
        {
            return (UniformMatrix3d)getUniform(name);
        }

        
		/// <summary>
		///  Returns the uniformMatrix4f of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix4f the uniform of this program whose name is given,
        ///  or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public UniformMatrix4f getUniformMatrix4f(string name)
        {
            return (UniformMatrix4f)getUniform(name);
        }

      
		/// <summary>
		/// Returns the uniformMatrix4d of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix4d  of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name  a GLSL uniform name.
		/// </param>/
        public UniformMatrix4d getUniformMatrix4d(string name)
        {
            return (UniformMatrix4d)getUniform(name);
        }

		/// <summary>
		/// Returns the uniformMatrix2x3f of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix2x3f the uniform of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public UniformMatrix2x3f getUniformMatrix2x3f(string name)
        {
            return (UniformMatrix2x3f)getUniform(name);
        }

 
		/// <summary>
		///  Returns the uniformMatrix2x3d of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix2x3d of this program whose name is given,
        ///  or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public UniformMatrix2x3d getUniformMatrix2x3d(string name)
        {
            return (UniformMatrix2x3d)getUniform(name);
        }

        
		/// <summary>
		/// Gets the uniform matrix2x4f of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix2x4f of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name GLSL uniform name.
		/// </param>
        public UniformMatrix2x4f getUniformMatrix2x4f(string name)
        {
            return (UniformMatrix2x4f)getUniform(name);
        }

 
		/// <summary>
		/// Returns the uniformMatrix2x4d of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix2x4d of this program whose name is given,
        ///  or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public UniformMatrix2x4d getUniformMatrix2x4d(string name)
        {
            return (UniformMatrix2x4d)getUniform(name);
        }


		/// <summary>
		/// Returns the uniformMatrix3x2f of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix3x2f of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public UniformMatrix3x2f getUniformMatrix3x2f(string name)
        {
            return (UniformMatrix3x2f)getUniform(name);
        }


		/// <summary>
		/// Returns the uniformMatrix3x2d of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix3x2d f this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public UniformMatrix3x2d getUniformMatrix3x2d(string name)
        {
            return (UniformMatrix3x2d)getUniform(name);
        }


		/// <summary>
		/// Returns the uniformMatrix3x4f of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix3x4f of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public UniformMatrix3x4f getUniformMatrix3x4f(string name)
        {
            return (UniformMatrix3x4f)getUniform(name);
        }

  
		/// <summary>
		/// Returns the uniformMatrix3x4d of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix3x4d the uniform of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public UniformMatrix3x4d getUniformMatrix3x4d(string name)
        {
            return (UniformMatrix3x4d)getUniform(name);
        }

        
		/// <summary>
		/// Returns the uniformMatrix4x2f of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix4x2f  the uniform of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public UniformMatrix4x2f getUniformMatrix4x2f(string name)
        {
            return (UniformMatrix4x2f)getUniform(name);
        }

  
		/// <summary>
		/// Returns the uniformMatrix4x2d of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix4x2d  of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public UniformMatrix4x2d getUniformMatrix4x2d(string name)
        {
            return (UniformMatrix4x2d)getUniform(name);
        }

      
		/// <summary>
		/// Returns the uniformMatrix4x3f of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix4x3f  of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public UniformMatrix4x3f getUniformMatrix4x3f(string name)
        {
            return (UniformMatrix4x3f)getUniform(name);
        }

   
		/// <summary>
		/// Returns the uniformMatrix4x2d of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix4x3d of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public UniformMatrix4x3d getUniformMatrix4x3d(string name)
        {
            return (UniformMatrix4x3d)getUniform(name);
        }

  
		/// <summary>
		/// Returns the uniform sampler of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform sampler of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
        public UniformSampler getUniformSampler(string name)
        {
            return (UniformSampler)getUniform(name);
        }

    
		/// <summary>
		/// Returns the uniform block of this program whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform block of this program whose name is given,
        /// or null if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform block name.
		/// </param>
        public UniformBlock getUniformBlock(string name)
        {
            UniformBlock rst;
            if (!uniformBlocks.TryGetValue(name, out rst))
            {
                return null;
            }
            return rst;
        }


    
		/// <summary>
		/// The modules of this program.
		/// </summary>
        protected List<Module> modules;

		/// <summary>
		/// Creates an unitialized program.
		/// Initializes a new instance of the <see cref="Sxta.Render.Program"/> class.
		/// </summary>
        public Program() { }

  
		/// <summary>
		/// Initializes this program.
		/// </summary>
		/// <param name='modules'>
		/// Modules the modules that will compose this program.
		/// </param>
        public void init(List<Module> modules)
        {
            this.modules = modules;
            // creates the program
#if OPENTK
            programId = GL.CreateProgram();
#else
             programId = glCreateProgram();
#endif
            Debug.Assert(programId > 0);

            int feedbackVaryingCount = 0;

            // attach all the shader objects
            foreach (Module module in this.modules)
            {
                module.users.Add(this);
                if (module.vertexShaderId != -1)
                {
#if OPENTK
                    GL.AttachShader(programId, module.vertexShaderId);
#else
                   glAttachShader(programId, module.vertexShaderId);
#endif
                }
                if (module.tessControlShaderId != -1)
                {
#if OPENTK
                    GL.AttachShader(programId, module.tessControlShaderId);
#else
                    glAttachShader(programId, module.tessControlShaderId);
#endif
                }
                if (module.tessEvalShaderId != -1)
                {
#if OPENTK
                    GL.AttachShader(programId, module.tessEvalShaderId);
#else
                    glAttachShader(programId, module.tessEvalShaderId);
#endif
                }
                if (module.geometryShaderId != -1)
                {
#if OPENTK
                    GL.AttachShader(programId, module.geometryShaderId);
#else
                    glAttachShader(programId, module.geometryShaderId);
#endif
                }
                if (module.fragmentShaderId != -1)
                {
#if OPENTK
                    GL.AttachShader(programId, module.fragmentShaderId);
#else
                    glAttachShader(programId, module.fragmentShaderId);
#endif
                }
                feedbackVaryingCount += module.feedbackVaryings.Count;
            }

            // intializes the transform feedback varyings
            if (feedbackVaryingCount > 0)
            {
                int index = 0;
                int interleaved = 0;
                string[] varyings = new string[feedbackVaryingCount];
                foreach (Module module in this.modules)
                {
                    foreach (string j in module.feedbackVaryings)
                    {
                        varyings[index++] = j;
                    }
                    if (module.feedbackMode != 0)
                    {
                        if (interleaved == 0 || module.feedbackMode == interleaved)
                        {
                            interleaved = module.feedbackMode;
                        }
                        else
                        {
                            Debug.Assert(false);
                        }
                    }
                }
                //delete[] varyings = null;
                Debug.Assert(interleaved != 0);

#if OPENTK
                GL.TransformFeedbackVaryings(programId, feedbackVaryingCount, varyings,
                    interleaved == 1 ? TransformFeedbackMode.InterleavedAttribs : TransformFeedbackMode.SeparateAttribs);
#else
                glTransformFeedbackVaryings(programId, feedbackVaryingCount, varyings,
                    interleaved == 1 ? GL_INTERLEAVED_ATTRIBS : GL_SEPARATE_ATTRIBS);
#endif
            }

            // link everything together
#if OPENTK
            int linked;
            GL.LinkProgram(programId);
            GL.GetProgram(programId, ProgramParameter.LinkStatus, out linked);
            if (linked == 0)
            {
                // if a link error occured ...
                int logLength;
                GL.GetProgram(programId, ProgramParameter.InfoLogLength, out logLength);
                if (logLength > 0)
                {
                    int length;
                    StringBuilder logStr = new StringBuilder();
                    GL.GetProgramInfoLog(programId, logLength, out length, logStr);
                    log.Error(logStr.ToString());
                    Debug.Assert(false);
                }
                GL.DeleteProgram(programId);
                programId = -1;
                throw new Exception();
            }
            int maxNameLength;
            int maxBlockNameLength = 0;
            GL.GetProgram(programId, ProgramParameter.ActiveUniformMaxLength, out maxNameLength);
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
            GL.GetProgram(programId, ProgramParameter.ActiveUniformBlockMaxNameLength, out maxBlockNameLength);
#if DEBUG
            ErrorCode error = FrameBuffer.getError();
            if (error != ErrorCode.NoError)
                log.Warn("ActiveUniformBlockMaxNameLength produces error:" + error);
#endif
            maxNameLength = System.Math.Max(maxNameLength, maxBlockNameLength);

            StringBuilder buf = new StringBuilder(); // = new char[maxNameLength];

            int nUniforms = 0;
            GL.GetProgram(programId, ProgramParameter.ActiveUniforms, out nUniforms);
#else
            
            int linked;
            glLinkProgram(programId);
            glGetProgramiv(programId, GL_LINK_STATUS, &linked);
            if (linked == GL_FALSE)
            {
                // if a link error occured ...
                int logLength;
                glGetProgramiv(programId, GL_INFO_LOG_LENGTH, &logLength);
                if ((Logger.ERROR_LOGGER != null) && (logLength > 0))
                {
                    int length;
                    char* log = new char[logLength];
                    glGetProgramInfoLog(programId, logLength, &length, log);
                    log.Error(log);
                    delete[] log;
                    Debug.Assert(false);
                }
                glDeleteProgram(programId);
                programId = -1;
                throw exception();
            }


            int maxNameLength;
            int maxBlockNameLength;
            glGetProgramiv(programId, GL_ACTIVE_UNIFORM_MAX_LENGTH, &maxNameLength);
            glGetProgramiv(programId, GL_ACTIVE_UNIFORM_BLOCK_MAX_NAME_LENGTH, &maxBlockNameLength);
            maxNameLength = max(maxNameLength, maxBlockNameLength);

            string buf; // = new char[maxNameLength];

            int nUniforms;
            glGetProgramiv(programId, GL_ACTIVE_UNIFORMS, &nUniforms);
#endif


            ISet<string> newBlocks = new HashSet<string>();

            for (int i = 0; i < nUniforms; ++i)
            {
                int length;
                int type;
                int size;
                int blockIndex;
                int offset;
                int arrayStride;
                int matrixStride;
                int isRowMajor;
#if OPENTK
                GL.GetActiveUniformName(programId, i, maxNameLength, out length, buf);
                GL.GetActiveUniforms(programId, 1, ref i, ActiveUniformParameter.UniformType, out type);
                GL.GetActiveUniforms(programId, 1, ref i, ActiveUniformParameter.UniformSize, out size);
                GL.GetActiveUniforms(programId, 1, ref i, ActiveUniformParameter.UniformBlockIndex, out blockIndex);
                if (blockIndex == -1)
                {
                    offset = GL.GetUniformLocation(programId, buf.ToString());
                }
                else
                {
                    GL.GetActiveUniforms(programId, 1, ref i, ActiveUniformParameter.UniformOffset, out offset);
                }
                GL.GetActiveUniforms(programId, 1, ref i, ActiveUniformParameter.UniformArrayStride, out arrayStride);
                GL.GetActiveUniforms(programId, 1, ref i, ActiveUniformParameter.UniformMatrixStride, out matrixStride);
                GL.GetActiveUniforms(programId, 1, ref i, ActiveUniformParameter.UniformIsRowMajor, out isRowMajor);
                string name = buf.ToString();
#else
                glGetActiveUniformName(programId, i, maxNameLength, &length, buf);
                glGetActiveUniformsiv(programId, 1, &i, GL_UNIFORM_TYPE, &type);
                glGetActiveUniformsiv(programId, 1, &i, GL_UNIFORM_SIZE, &size);
                glGetActiveUniformsiv(programId, 1, &i, GL_UNIFORM_BLOCK_INDEX, &blockIndex);
                if (blockIndex == -1)
                {
                    offset = glGetUniformLocation(programId, buf);
                }
                else
                {
                    glGetActiveUniformsiv(programId, 1, &i, GL_UNIFORM_OFFSET, &offset);
                }
                glGetActiveUniformsiv(programId, 1, &i, GL_UNIFORM_ARRAY_STRIDE, &arrayStride);
                glGetActiveUniformsiv(programId, 1, &i, GL_UNIFORM_MATRIX_STRIDE, &matrixStride);
                glGetActiveUniformsiv(programId, 1, &i, GL_UNIFORM_IS_ROW_MAJOR, &isRowMajor);
                string name = buf;
#endif


                UniformBlock b = null;
                if (blockIndex != -1)
                {
#if OPENTK
                    GL.GetActiveUniformBlockName(programId, blockIndex, maxNameLength, out length, buf);
                    string blockName = buf.ToString();

                    if (!uniformBlocks.TryGetValue(blockName, out b))
                    {
                        int blockSize;
                        GL.GetActiveUniformBlock(programId, blockIndex, ActiveUniformBlockParameter.UniformBlockDataSize, out blockSize);
                        b = new UniformBlock(this, blockName, blockIndex, blockSize);
                        uniformBlocks.Add(blockName, b);
                    }
#else
                    glGetActiveUniformBlockName(programId, (uint)(blockIndex), maxNameLength, &length, buf);
                    string blockName = buf;

                    if (!uniformBlocks.TryGetValue(blockName, out b))
                    {
                        int blockSize;
                        glGetActiveUniformBlockiv(programId, (uint)(blockIndex), GL_UNIFORM_BLOCK_DATA_SIZE, &blockSize);
                        b = new UniformBlock(this, blockName, (uint)(blockIndex), (uint)(blockSize));
                        uniformBlocks.Add( blockName, b );
                    }
#endif
                }

                UniformBlock block = b;
                if (name.EndsWith("[0]"))
                    name = name.Substring(0, name.Length - 3);

                for (int j = 0; j < size; ++j)
                {
                    Uniform u = null;

                    string uname;
                    if (size == 1)
                    {
                        uname = name;
                    }
                    else
                    {
                        uname = name + "[" + j + "]";
                    }

                    int uoffset;
                    if (block == null)
                    {
#if OPENTK
                        uoffset = GL.GetUniformLocation(programId, uname);
#else
                        uoffset = glGetUniformLocation(programId, uname.c_str());
#endif
                    }
                    else
                    {
                        uoffset = offset + j * arrayStride;
                    }

                    switch ((ActiveUniformType)type)
                    {
                        case ActiveUniformType.Float:
                            u = new Uniform1f(this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.FloatVec2:
                            u = new Uniform2f(this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.FloatVec3:
                            u = new Uniform3f(this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.FloatVec4:
                            u = new Uniform4f(this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.Double:
                            u = new Uniform1d(this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.DoubleVec2:
                            u = new Uniform2d(this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.DoubleVec3:
                            u = new Uniform3d(this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.DoubleVec4:
                            u = new Uniform4d(this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.Int:
                            u = new Uniform1i(this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.IntVec2:
                            u = new Uniform2i(this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.IntVec3:
                            u = new Uniform3i(this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.IntVec4:
                            u = new Uniform4i(this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.UnsignedInt:
                            u = new Uniform1ui(this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.UnsignedIntVec2:
                            u = new Uniform2ui(this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.UnsignedIntVec3:
                            u = new Uniform3ui(this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.UnsignedIntVec4:
                            u = new Uniform4ui(this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.Bool:
                            u = new Uniform1b(this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.BoolVec2:
                            u = new Uniform2b(this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.BoolVec3:
                            u = new Uniform3b(this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.BoolVec4:
                            u = new Uniform4b(this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.FloatMat2:
                            u = new UniformMatrix2f(this, block, uname, uoffset, matrixStride, isRowMajor != 0);
                            break;
                        case ActiveUniformType.FloatMat3:
                            u = new UniformMatrix3f(this, block, uname, uoffset, matrixStride, isRowMajor != 0);
                            break;
                        case ActiveUniformType.FloatMat4:
                            u = new UniformMatrix4f(this, block, uname, uoffset, matrixStride, isRowMajor != 0);
                            break;
                        case ActiveUniformType.FloatMat2x3:
                            u = new UniformMatrix2x3f(this, block, uname, uoffset, matrixStride, isRowMajor);
                            break;
                        case ActiveUniformType.FloatMat2x4:
                            u = new UniformMatrix2x4f(this, block, uname, uoffset, matrixStride, isRowMajor);
                            break;
                        //case ActiveUniformType.FloatMat3x2:
                        //    u = new UniformMatrix3x2f(this, block, uname, uoffset, matrixStride, isRowMajor);
                        //    break;
                        case ActiveUniformType.FloatMat3x4:
                            u = new UniformMatrix3x4f(this, block, uname, uoffset, matrixStride, isRowMajor);
                            break;
                        case ActiveUniformType.FloatMat4x2:
                            u = new UniformMatrix4x2f(this, block, uname, uoffset, matrixStride, isRowMajor);
                            break;
                        case ActiveUniformType.FloatMat4x3:
                            u = new UniformMatrix4x3f(this, block, uname, uoffset, matrixStride, isRowMajor);
                            break;
#if TODO //TODO Agustin
                        case ActiveUniformType.DoubleMat2:
                            u = new UniformMatrix2d(this, block, uname, uoffset, matrixStride, isRowMajor != 0);
                            break;
                        case ActiveUniformType.DoubleMat3:
                            u = new UniformMatrix3d(this, block, uname, uoffset, matrixStride, isRowMajor != 0);
                            break;
                        case ActiveUniformType.DoubleMat4:
                            u = new UniformMatrix4d(this, block, uname, uoffset, matrixStride, isRowMajor != 0);
                            break;
                        case ActiveUniformType.DoubleMat2x3:
                            u = new UniformMatrix2x3d(this, block, uname, uoffset, matrixStride, isRowMajor);
                            break;
                        case ActiveUniformType.DoubleMat2x4:
                            u = new UniformMatrix2x4d(this, block, uname, uoffset, matrixStride, isRowMajor);
                            break;
                        case ActiveUniformType.DoubleMat3x2:
                            u = new UniformMatrix3x2d(this, block, uname, uoffset, matrixStride, isRowMajor);
                            break;
                        case ActiveUniformType.DoubleMat3x4:
                            u = new UniformMatrix3x4d(this, block, uname, uoffset, matrixStride, isRowMajor);
                            break;
                        case ActiveUniformType.DoubleMat4x2:
                            u = new UniformMatrix4x2d(this, block, uname, uoffset, matrixStride, isRowMajor);
                            break;
                        case ActiveUniformType.DoubleMat4x3:
                            u = new UniformMatrix4x3d(this, block, uname, uoffset, matrixStride, isRowMajor);
                            break;
#endif
                        case ActiveUniformType.Sampler1D:
                        case ActiveUniformType.Sampler1DShadow:
                            u = new UniformSampler(UniformType.SAMPLER_1D, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.Sampler2D:
                        case ActiveUniformType.Sampler2DShadow:
                            u = new UniformSampler(UniformType.SAMPLER_2D, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.Sampler3D:
                            u = new UniformSampler(UniformType.SAMPLER_3D, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.SamplerCube:
                        case ActiveUniformType.SamplerCubeShadow:
                            u = new UniformSampler(UniformType.SAMPLER_CUBE, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.Sampler1DArray:
                        case ActiveUniformType.Sampler1DArrayShadow:
                            u = new UniformSampler(UniformType.SAMPLER_1D_ARRAY, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.Sampler2DArray:
                        case ActiveUniformType.Sampler2DArrayShadow:
                            u = new UniformSampler(UniformType.SAMPLER_2D_ARRAY, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.SamplerCubeMapArray:
                        case ActiveUniformType.SamplerCubeMapArrayShadow:
                            u = new UniformSampler(UniformType.SAMPLER_CUBE_MAP_ARRAY, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.Sampler2DMultisample:
                            u = new UniformSampler(UniformType.SAMPLER_2D_MULTISAMPLE, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.Sampler2DMultisampleArray:
                            u = new UniformSampler(UniformType.SAMPLER_2D_MULTISAMPLE_ARRAY, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.SamplerBuffer:
                            u = new UniformSampler(UniformType.SAMPLER_BUFFER, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.Sampler2DRect:
                        case ActiveUniformType.Sampler2DRectShadow:
                            u = new UniformSampler(UniformType.SAMPLER_2D_RECT, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.IntSampler1D:
                            u = new UniformSampler(UniformType.INT_SAMPLER_1D, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.IntSampler2D:
                            u = new UniformSampler(UniformType.INT_SAMPLER_2D, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.IntSampler3D:
                            u = new UniformSampler(UniformType.INT_SAMPLER_3D, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.IntSamplerCube:
                            u = new UniformSampler(UniformType.INT_SAMPLER_CUBE, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.IntSampler1DArray:
                            u = new UniformSampler(UniformType.INT_SAMPLER_1D_ARRAY, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.IntSampler2DArray:
                            u = new UniformSampler(UniformType.INT_SAMPLER_2D_ARRAY, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.IntSamplerCubeMapArray:
                            u = new UniformSampler(UniformType.INT_SAMPLER_CUBE_MAP_ARRAY, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.IntSampler2DMultisample:
                            u = new UniformSampler(UniformType.INT_SAMPLER_2D_MULTISAMPLE, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.IntSampler2DMultisampleArray:
                            u = new UniformSampler(UniformType.INT_SAMPLER_2D_MULTISAMPLE_ARRAY, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.IntSamplerBuffer:
                            u = new UniformSampler(UniformType.INT_SAMPLER_BUFFER, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.IntSampler2DRect:
                            u = new UniformSampler(UniformType.INT_SAMPLER_2D_RECT, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.UnsignedIntSampler1D:
                            u = new UniformSampler(UniformType.UNSIGNED_INT_SAMPLER_1D, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.UnsignedIntSampler2D:
                            u = new UniformSampler(UniformType.UNSIGNED_INT_SAMPLER_2D, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.UnsignedIntSampler3D:
                            u = new UniformSampler(UniformType.UNSIGNED_INT_SAMPLER_3D, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.UnsignedIntSamplerCube:
                            u = new UniformSampler(UniformType.UNSIGNED_INT_SAMPLER_CUBE, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.UnsignedIntSampler1DArray:
                            u = new UniformSampler(UniformType.UNSIGNED_INT_SAMPLER_1D_ARRAY, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.UnsignedIntSampler2DArray:
                            u = new UniformSampler(UniformType.UNSIGNED_INT_SAMPLER_2D_ARRAY, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.UnsignedIntSamplerCubeMapArray:
                            u = new UniformSampler(UniformType.UNSIGNED_INT_SAMPLER_CUBE_MAP_ARRAY, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.UnsignedIntSampler2DMultisample:
                            u = new UniformSampler(UniformType.UNSIGNED_INT_SAMPLER_2D_MULTISAMPLE, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.UnsignedIntSampler2DMultisampleArray:
                            u = new UniformSampler(UniformType.UNSIGNED_INT_SAMPLER_2D_MULTISAMPLE_ARRAY, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.UnsignedIntSamplerBuffer:
                            u = new UniformSampler(UniformType.UNSIGNED_INT_SAMPLER_BUFFER, this, block, uname, uoffset);
                            break;
                        case ActiveUniformType.UnsignedIntSampler2DRect:
                            u = new UniformSampler(UniformType.UNSIGNED_INT_SAMPLER_2D_RECT, this, block, uname, uoffset);
                            break;
                        default:
                            Debug.Assert(false);
                            break;
                    }

                    uniforms.Add(uname, u);
                    if (b != null)
                    {
                        b.uniforms.Add(uname, u);
                    }
                    if (u is UniformSampler)
                    {
                        uniformSamplers.Add(u as UniformSampler);
                    }
                }
            }
            //delete[] buf;

            // finds GPUBuffer suitable for the blocks used in this Program
            foreach (UniformBlock u in uniformBlocks.Values)
            {
                string oss = u.getName() + "-" + u.size + "-" + u.uniforms.Count; //example : deformation-8-32
                GPUBuffer buffer = UniformBlock.buffers.Get(oss);
                if (buffer.getSize() == 0)
                {
                    buffer.setData<byte>(u.size, null, BufferUsage.DYNAMIC_DRAW);
                    newBlocks.Add(u.getName());
                }
                u.setBuffer(buffer);
            }

            // sets the initial values of the uniforms
            foreach (Module module in this.modules)
            {
                foreach (var kv in module.initialValues)
                {
                    Value v = kv.Value;
                    Uniform u = null;

                    uniforms.TryGetValue(kv.Key, out u);

                    if (u != null && u.block != null && !newBlocks.Contains(u.block.getName()))
                    {
                        // do not set initial values for uniforms in already existing
                        // uniform blocks, to avoid overriding the values of their uniforms
                        u = null;
                    }

                    if (u != null)
                    {
                        ValueSampler vs = v as ValueSampler;
                        UniformSampler us = u as UniformSampler;
                        Debug.Assert(u.getName() == v.getName());
                        if (u.getType() == v.getType() || (us != null && vs != null))
                        {
                            u.setValue(v);
                        }
                    }
                }
            }
        }

   
		/// <summary>
		/// Swaps this program with the given one.
		/// </summary>
		/// <param name='p'>
		/// P.
		/// </param>
        public virtual void swap(Program p)
        {

            updateTextureUsers(false);
            p.updateTextureUsers(false);

            updateUniformBlocks(false);
            p.updateUniformBlocks(false);

            Std.Swap(ref modules, ref p.modules);
            Std.Swap(ref programId, ref  p.programId);
            Std.Swap(ref uniforms, ref p.uniforms);
            Std.Swap(ref uniformBlocks, ref  p.uniformBlocks);
#if TODO
            map<string, Uniform>.iterator i = p.uniforms.begin();
            while (i != p.uniforms.end())
            {
                Uniform u = i.second;
                map<string, Uniform>.iterator j = uniforms.find(u.getName());
                if (j != uniforms.end())
                {
                    Uniform pu = j.second;
                    if (pu.getType() == u.getType())
                    {
                        std.swap(i.second, j.second);
                        std.swap(i.second.location, j.second.location);
                        j.second.program = this;
#if ORK_NO_GLPROGRAMUNIFORM
                j.second.dirty = true;
#else
                        j.second.setValue();
#endif
                    }
                }
                else
                {
                    // 'u' is no longer an uniform of this program; we store it in the
                    // oldUniforms map to reuse this object if this uniform becomes a
                    // member of this program again, in future versions
                    oldUniforms[u.getName()] = u;
                }
                ++i;
            }

            i = oldUniforms.begin();
            while (i != oldUniforms.end())
            {
                Uniform oldU = i.second;
                map<string, Uniform>.iterator j = uniforms.find(oldU.getName());
                if (j != uniforms.end())
                {
                    // if an uniform of this program corresponds to an old uniform object,
                    // we reuse the old uniform object (so that clients do not have to
                    // update their references to the uniforms of this program)
                    Uniform u = j.second;
                    if (u != oldU && u.getType() == oldU.getType())
                    {
                        Std.Swap<Uniform>(i.second, j.second);
                        Std.Swap<UniformBlock>(i.second.location, j.second.location);
                        j.second.program = this;
#if ORK_NO_GLPROGRAMUNIFORM
                j.second.dirty = true;
#else
                        j.second.setValue();
#endif
                    }
                    oldUniforms.erase(i++);
                }
                else
                {
                    ++i;
                }
            }

            map<string, UniformBlock>.iterator k = uniformBlocks.begin();
            while (k != uniformBlocks.end())
            {
                UniformBlock b = k.second;
                map<string, UniformBlock>.iterator i = p.uniformBlocks.find(b.getName());
                if (i != p.uniformBlocks.end())
                {
                    UniformBlock pb = i.second;
                    Std.Swap<UniformBlock>(k.second, i.second);
                }
                ++k;
            }

            updateUniforms(this);
            p.updateUniforms(&(*p));

            updateTextureUsers(true);
            p.updateTextureUsers(true);

            if (CURRENT == this)
            {
                CURRENT = null;
            }
 
#endif
            throw new NotImplementedException();
        }

    
		/// <summary>
		/// The id of this program.
		/// </summary>
        private int programId;

        
		/// <summary>
		/// The uniforms of this program. This includes all uniforms,
        /// whether outside or inside a uniform block, including uniform samplers.
		/// </summary>
        private IDictionary<string, Uniform> uniforms = new Dictionary<string, Uniform>();

        
		/// <summary>
		/// The uniforms of this program that were present in old versions,
        /// but are no longer present in the current one.
		/// </summary>
        private IDictionary<string, Uniform> oldUniforms = new Dictionary<string, Uniform>();

    
		/// <summary>
		/// The uniform samplers of this program.
		/// </summary>
        private List<UniformSampler> uniformSamplers = new List<UniformSampler>();

      
		/// <summary>
		/// The uniform blocks of this program.
		/// </summary>
        private IDictionary<string, UniformBlock> uniformBlocks = new Dictionary<string, UniformBlock>();

       
		/// <summary>
		/// The program currently in use.
		/// </summary>
        internal static Program CURRENT;

  
		/// <summary>
		/// Checks that each active program sampler is bound to a texture.
		/// </summary>
		/// <returns>
		/// true if all samplers are bound.
		/// </returns>
        internal bool checkSamplers()
        {
            for (int i = 0; i < uniformSamplers.Count; ++i)
            {
                UniformSampler u = uniformSamplers[i];
                if (u.location != -1 && u.get() == null)
                {
                    log.Error("Sampler not bound " + u.getName());
                    return false;
                }
            }
            return true;
        }

     
		/// <summary>
		/// Sets this program as the current program.
		/// </summary>
        public void set()
        {
            Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
            if (CURRENT != this)
            {
                log.Debug("Switching Program");
                CURRENT = this;
#if OPENTK
                GL.UseProgram(programId);
#else
                glUseProgram(programId);
#endif
                for (int i = 0; i < uniformSamplers.Count; ++i)
                {
                    uniformSamplers[i].setValue();
                }

                foreach (UniformBlock u in uniformBlocks.Values)
                {
                    int unit = u.buffer.bindToUniformBufferUnit(programId);
                    Debug.Assert(unit >= 0);
#if OPENTK
                    GL.UniformBlockBinding(programId, u.index, unit);
#else
                    glUniformBlockBinding(programId, u.index, (uint)(unit));
#endif
                }

                Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
            }


            if (log.IsDebugEnabled)
            {
                StringBuilder sb = new StringBuilder();
                int nBlocks = 0;
                foreach (var j in uniformBlocks)
                {
                    UniformBlock u = j.Value;
                    if (u.isMapped())
                    {
                        sb.Append(j.Key + ";");
                        ++nBlocks;
                    }
                }
                if (nBlocks > 0)
                {
                    log.DebugFormat("Updating {0} block(s) [{1}]", nBlocks, sb);
                }
            }

            foreach (UniformBlock u in uniformBlocks.Values)
            {
                if (u.isMapped())
                {
                    u.unmapBuffer();
                }
            }

#if ORK_NO_GLPROGRAMUNIFORM
    map<string, Uniform >.iterator i = uniforms.begin();
    while (i != uniforms.end()) {
        Uniform u = i.second;
        if (u.dirty) {
            u.setValue();
            u.dirty = false;
        }
        i++;
    }
#endif
        }

		/// <summary>
		/// Adds or removes this program as a user of the textures bound to
        /// the uniform samplers of this program.
		/// </summary>
		/// <returns>
		/// The texture users.
		/// </returns>
		/// <param name='add'>
		/// Add.
		/// </param>
        private void updateTextureUsers(bool add)
        {
            for (int i = 0; i < uniformSamplers.Count; ++i)
            {
                UniformSampler us = uniformSamplers[i];
                Texture t = us.get();
                if (t != null)
                {
                    if (add)
                    {
                        t.addUser(us.program.getId());
                    }
                    else
                    {
                        t.removeUser(us.program.getId());
                        us.unit = -1;
                    }
                }
            }
        }

       
		/// <summary>
		/// Adds to or removes from #uniforms the uniforms that are inside
        /// unifom blocks.
		/// </summary>
		/// <returns>
		/// The uniform blocks.
		/// </returns>
		/// <param name='add'>
		/// Add.
		/// </param>
        private void updateUniformBlocks(bool add)
        {
#if TODO
            if (add)
            {
                map<string, UniformBlock>.iterator j = uniformBlocks.begin();
                while (j != uniformBlocks.end())
                {
                    UniformBlock b = j.second;
                    map<string, Uniform>.iterator i = b.uniforms.begin();
                    while (i != b.uniforms.end())
                    {
                        uniforms.insert(make_pair(i.second.getName(), i.second));
                        ++i;
                    }
                    ++j;
                }

            }
            else
            {
                map<string, Uniform>.iterator i = uniforms.begin();
                while (i != uniforms.end())
                {
                    if (i.second.block != null)
                    {
                        uniforms.erase(i++);
                    }
                    else
                    {
                        ++i;
                    }
                }
            }
 
#endif
            throw new NotImplementedException();
        }

        
		/// <summary>
		/// Sets the owner program of the uniforms and uniform blocks of this
         /// program to the given valueC.
		/// Updates the uniforms.
		/// </summary>
		/// <returns>
		/// The uniforms.
		/// </returns>
		/// <param name='owner'>
		/// Owner the new owner for the uniforms of this program.
		/// </param>/
        private void updateUniforms(Program owner)
        {
            uniformSamplers.Clear();

            foreach (Uniform u in uniforms.Values)
            {
                u.program = owner;

                UniformSampler us = u as UniformSampler;
                if (us != null)
                {
                    uniformSamplers.Add(us);
                }
            }

            foreach (UniformBlock b in uniformBlocks.Values)
            {
                b.program = owner;
                if (b.buffer != null && b.isMapped())
                {
                    b.unmapBuffer();
                }
                foreach (Uniform uv in b.uniforms.Values)
                {
                    uv.program = owner;
                    uv.block = b;
                }
            }

            if (owner != null)
            {
                updateUniformBlocks(true);
            }
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
                    if (CURRENT == this)
                    {
                        CURRENT = null;
                    }
                    if (programId != -1)
                    {
                        updateTextureUsers(false);
                        updateUniforms(null);
                        foreach (UniformBlock b in uniformBlocks.Values)
                        {
                            b.Dispose();
                        }
                    }
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.

#if OPENTK
                GL.DeleteProgram(programId);
#else
                glDeleteProgram(programId);
#endif
                Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
                for (int i = 0; i < modules.Count; ++i)
                {
                    modules[i].users.Remove(this);
                    if (modules[i].users.Count == 0)
                        modules[i].Dispose();
                }
                Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
                // Note disposing has been done.
                disposed = true;

            }
        }
#endregion

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }

    
}
