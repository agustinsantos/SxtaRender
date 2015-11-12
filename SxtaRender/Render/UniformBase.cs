using OpenTK.Graphics.OpenGL;
using Sxta.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Sxta.Render
{
	/// <summary>
	/// An abstract uniform variable of a Program. The uniform valueC can be set at
	/// any time, and affects only the program that contains this uniform (unless
	/// this uniform belongs to a UniformBlock whose Buffer is shared between different
	/// programs, in which case setting the valueC of this uniform also changes its
	/// valueC for the other programs).
	/// </summary>
	public abstract class Uniform
	{
		/*
         * Deletes this uniform.
         */
		// ~Uniform();

        
		/// <summary>
		/// Returns the type of this uniform.
		/// </summary>
		/// <returns>
		/// The type.
		/// </returns>
		public abstract UniformType getType ();

        
		/// <summary>
		/// Returns the name of this uniform.
		/// </summary>
		/// <returns>
		/// The name.
		/// </returns>
		public string getName ()
		{
			return name;
		}

      
		/// <summary>
		/// Sets the valueC of this uniform.
		/// </summary>
		/// <returns>
		/// The value.
		/// </returns>
		/// <param name='v'>
		/// V the new valueC for this uniform. Must be of the same
		/// type as this Uniform.
		/// </param>
		public abstract void setValue (Value v);


       
		/// <summary>
		/// The Program to which this uniform belongs.
		/// </summary>
		internal protected Program program;

       
		/// <summary>
		/// The UniformBlock to which this uniform belongs. Maybe NULL.
		/// </summary>
		internal protected UniformBlock block;

        
		/// <summary>
		/// The name of this uniform.
		/// </summary>
		internal protected string name;

        
		/// <summary>
		/// The location of this uniform. For an uniform inside a block,
		/// this location is an offset inside the uniform block buffer.
		/// </summary>
		internal protected int location;

#if  ORK_NO_GLPROGRAMUNIFORM
        /*
         * True if the value of this uniform in its program is not up to date.
         */

        protected bool dirty;
#endif

     
		/// <summary>
		/// Creates a new uniform.
		/// Initializes a new instance of the <see cref="Sxta.Render.Uniform"/> class.
		/// </summary>
		/// <param name='type'>
		/// Type the type of this uniform.
		/// </param>
		/// <param name='program'>
		/// Program the Program to which this uniform belongs.
		/// </param>
		/// <param name='block'>
		/// Block UniformBlock to which this uniform belongs. Maybe NULL.
		/// </param>
		/// <param name='name'>
		/// Name the name of the uniform in the GLSL shader code.
		/// </param>
		/// <param name='location'>
		/// Location the location of this uniform. For an uniform inside a
		/// block, this location is an offset inside the uniform block buffer.
		/// </param>
		protected Uniform (string type, Program program, UniformBlock block, string name, int location)
		{
			this.program = program;
			this.block = block;
			this.name = name;
			this.location = location;
#if ORK_NO_GLPROGRAMUNIFORM
             dirty = false;
#endif
		}

#if  ORK_NO_GLPROGRAMUNIFORM
        /*
         * Sets this uniform in its program if this it is the current one.
         */
        protected void setValueIfCurrent()
        {
            if (block != null) {
                return;
            }
            if (program == Program.CURRENT) {
                setValue();
                dirty = false;
            } else {
                dirty = true;
            }
        }
#endif

        
		/// <summary>
		/// Sets this uniform in its program.
		/// </summary>
		/// <returns>
		/// The value.
		/// </returns>
		internal protected abstract void setValue ();

     
		/// <summary>
		///  Maps the GPUBuffer of the uniform block of this uniform into memory.
		/// </summary>
		/// <returns>
		/// The buffer.
		/// </returns>
		/// <param name='offset'>
		/// Offset.
		/// </param>
		protected IntPtr mapBuffer (int offset)
		{
			return block.mapBuffer (offset);
		}
	}


	/// <summary>
	/// A uniform holding a Texture valueC. In addition to a texture, a UniformSampler
	/// can also have a Sampler object to modify the default texture sampling
	/// parameters of the bound texture (like the minification of magnification
	/// filters).
	/// </summary>
	public class UniformSampler : Uniform
	{
		/*
        * Deletes this uniform.
        */
		//~UniformSampler();

		public override UniformType getType ()
		{
			return type;
		}

    
		/// <summary>
		/// Returns the sampler used to sample the texture bound to this uniform.
		/// </summary>
		/// <returns>
		/// The samplerused to sample the texture bound to this uniform.
		///  May be NULL.
		/// </returns>
		public Sampler getSampler ()
		{
			return sampler;
		}

     
		/// <summary>
		/// Sets the sampler used to sample the texture bound to this uniform.
		/// </summary>
		/// <returns>
		/// The sampler.
		/// </returns>
		/// <param name='sampler'>
		/// Sampler a sampler object. May be NULL.
		/// </param>
		public void setSampler (Sampler sampler)
		{
			this.sampler = sampler;
			if (program != null && program == Program.CURRENT) {
				setValue ();
			}
		}

       
		/// <summary>
		/// Returns the current valueC of this uniform.
		/// </summary>
		public Texture get ()
		{
			return value;
		}

        
		/// <summary>
		///Sets the valueC of this uniform.
		/// </summary>
		/// <param name='value'>
		/// valueC the new valueC for this uniform.
		/// </param>
		public void set (Texture value)
		{
			if (program != null) {
				if (this.value != null) {
					this.value.removeUser (program.getId ());
				}
				if (value != null) {
					value.addUser (program.getId ());
				}
			}
			this.value = value;
			if (program != null && program == Program.CURRENT) {
				setValue ();
			}
		}

		public override void setValue (Value v)
		{
			ValueSampler vs = v as ValueSampler;
			//setSampler(vs->getSampler());
			set (vs.get ());
		}


		/// <summary>
		/// Creates a new uniform.
		/// Initializes a new instance of the <see cref="Sxta.Render.UniformSampler"/> class.
		/// </summary>
		/// <param name='type'>
		/// Typethe type of this uniform.
		/// </param>
		/// <param name='program'>
		/// Program the Program to which this uniform belongs.
		/// </param>
		/// <param name='block'>
		/// Block UniformBlock to which this uniform belongs. Maybe NULL.
		/// </param>
		/// <param name='name'>
		/// Name the name of the uniform in the GLSL shader code.
		/// </param>
		/// <param name='location'>
		/// Location the location of this uniform. For an uniform inside a
		/// block, this location is an offset inside the uniform block buffer.
		/// </param>
		internal protected UniformSampler (UniformType type, Program program, UniformBlock block, string name, int location) :
            base("UniformSampler", program, block, name, location)
		{
			this.type = type;
			unit = -1;
		}

		internal protected override void setValue ()
		{
			if (value != null && location != -1) {
				int newUnit = value.bindToTextureUnit (sampler, (uint)program.getId ());
				Debug.Assert (newUnit >= 0);
				if (newUnit != unit) {
#if OPENTK
                    GL.Uniform1(location, newUnit);
#else
#if  ORK_NO_GLPROGRAMUNIFORM
                    glUniform1i(location, newUnit);
#else
					glProgramUniform1iEXT (program.getId (), location, newUnit);
#endif
#endif
					Debug.Assert (FrameBuffer.getError () == ErrorCode.NoError);
					unit = newUnit;
				}
			}
		}


     
		/// <summary>
		/// The type of this uniform.
		/// </summary>
		private UniformType type;

       
		/// <summary>
		///  The current sampler used to sample the texture bound to this uniform. May be NULL.
		/// </summary>
		private Sampler sampler;

       
		/// <summary>
		/// The current valueC of this uniform.
		/// </summary>
		private Texture value;

        
		/// <summary>
		/// The current texture unit valueC of this uniform.
		/// </summary>
		internal int unit;

	}

	/// <summary>
	/// A named block of uniforms. The values of the uniforms in a uniform block are
	/// stored in a GPUBuffer. Different Programs having identical uniform blocks have
	/// different UniformBlock objects, but these objects can share the same GPUBuffer
	/// to store their values. Hence, changing values inside this GPUBuffer changes the
	/// uniform values in all the programs that use this GPUBuffer.
	/// You don't have to manipulate the GPUBuffer content yourself to change the
	/// uniforms inside a uniform block: this is automatically managed by the Uniform
	/// and UniformBlock classes. In particular, these classes know the offset of each
	/// uniform in the buffer, and map and unmap this buffer in client memory when
	/// necessary.
	/// Initially the GPUBuffer associated with a UniformBlock is NULL. You must set it
	/// with #setBuffer() before using the uniforms of this block.
	/// </summary>
	public class UniformBlock : IDisposable
	{

       
		/// <summary>
		/// Deletes this uniform block.
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="Sxta.Render.UniformBlock"/> is reclaimed by garbage collection.
		/// </summary>
		~UniformBlock ()
		{
			// Do not re-create Dispose clean-up code here. 
			// Calling Dispose(false) is optimal in terms of 
			// readability and maintainability.
			Dispose (false);
		}


        
		/// <summary>
		///  Returns the name of this uniform block.
		/// </summary>
		/// <returns>
		/// The name.
		/// </returns>
		public string getName ()
		{
			return name;
		}

      
		/// <summary>
		/// Returns the GPUBuffer that stores the values of the uniforms of this block.
		/// </summary>
		/// <returns>
		/// The buffer.
		/// </returns>
		public GPUBuffer getBuffer ()
		{
			return buffer;
		}

      
		/// <summary>
		/// Returns the uniform of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniformof this block whose name is given, or NULL if there
		/// is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a uniform name.
		/// </param>
		public Uniform getUniform (string name)
		{
			Uniform value;
			if (!uniforms.TryGetValue (name, out value)) {
				if (!uniforms.TryGetValue (getName () + "." + name, out value)) {
					return null;
				}
			}
			return value;
		}

   
		/// <summary>
		/// Returns the uniform1f of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform1f of this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
		public Uniform1f getUniform1f (string name)
		{
			return getUniform (name) as Uniform1f;
		}

    
		/// <summary>
		/// Returns the uniform1d of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform1d the uniform of this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
		public Uniform1d getUniform1d (string name)
		{
			return getUniform (name) as Uniform1d;
		}

     
		/// <summary>
		/// Gets the uniform1i.
		/// </summary>
		/// <returns>
		/// The uniform1i of this block whose name is given or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
		public Uniform1i getUniform1i (string name)
		{
			return getUniform (name) as Uniform1i;
		}

     
		/// <summary>
		/// Gets the uniform1ui.
		/// </summary>
		/// <returns>
		/// The uniform1ui of this block whose name is given or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Namea GLSL uniform name.
		/// </param>
		public Uniform1ui getUniform1ui (string name)
		{
			return getUniform (name) as Uniform1ui;
		}

     
		/// <summary>
		/// Returns the uniform1b of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform1b  of this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
		public Uniform1b getUniform1b (string name)
		{
			return getUniform (name) as Uniform1b;
		}

     
		/// <summary>
		/// Returns the uniform2f of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform2f of this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Namea GLSL uniform name.
		/// </param>
		public Uniform2f getUniform2f (string name)
		{
			return getUniform (name) as Uniform2f;
		}

     
		/// <summary>
		/// Gets the uniform2d of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform2d of this block whose name is given,
		/// or NULL if there is no such uniform.
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
		public Uniform2d getUniform2d (string name)
		{
			return getUniform (name) as Uniform2d;
		}

     
		/// <summary>
		/// Returns the uniform2i of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform2i of this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Namea GLSL uniform name.
		/// </param>
		public Uniform2i getUniform2i (string name)
		{
			return getUniform (name) as Uniform2i;
		}

    
		/// <summary>
		/// Returns the uniform2ui of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform2uiof this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
		public Uniform2ui getUniform2ui (string name)
		{
			return getUniform (name) as Uniform2ui;
		}

     
		/// <summary>
		/// Returns the uniform2b of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform2b of this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
		public Uniform2b getUniform2b (string name)
		{
			return getUniform (name) as Uniform2b;
		}

     
		/// <summary>
		///Returns the uniform3f of this block whose name is given.
		/// or NULL if there is no such uniform.
		/// </summary>
		/// <returns>
		/// The uniform3f.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
		public Uniform3f getUniform3f (string name)
		{
			return getUniform (name) as Uniform3f;
		}

     
		/// <summary>
		/// Returns the uniform3d of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform3dof this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
		public Uniform3d getUniform3d (string name)
		{
			return getUniform (name) as Uniform3d;
		}

    
		/// <summary>
		/// Returns the uniform3i of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform3i of this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Namea GLSL uniform name.
		/// </param>
		public Uniform3i getUniform3i (string name)
		{
			return getUniform (name) as Uniform3i;
		}

   
		/// <summary>
		/// Returns the uniform3ui of this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </summary>
		/// <returns>
		/// The uniform3ui.
		/// </returns>
		/// <param name='name'>
		/// Namea GLSL uniform name.
		/// </param>
		public Uniform3ui getUniform3ui (string name)
		{
			return getUniform (name) as Uniform3ui;
		}

      
		/// <summary>
		///  Returns the uniform3b of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform3bof this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
		public Uniform3b getUniform3b (string name)
		{
			return getUniform (name) as Uniform3b;
		}

     
		/// <summary>
		///  Returns the uniform4f of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform4fof this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
		public Uniform4f getUniform4f (string name)
		{
			return getUniform (name) as Uniform4f;
		}

       
		/// <summary>
		/// Returns the uniform4d of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform4dthe uniform of this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
		public Uniform4d getUniform4d (string name)
		{
			return getUniform (name) as Uniform4d;
		}

       
		/// <summary>
		/// Returns the uniform4i of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform4iof this block whose name is given,
		///  or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
		public Uniform4i getUniform4i (string name)
		{
			return getUniform (name) as Uniform4i;
		}

   
		/// <summary>
		/// Returns the uniform4ui of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform4uif this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Namea GLSL uniform name.
		/// </param>
		public Uniform4ui getUniform4ui (string name)
		{
			return getUniform (name) as Uniform4ui;
		}

   
		/// <summary>
		/// Returns the uniform4b of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform4bthe uniform of this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Namea GLSL uniform name.
		/// </param>
		public Uniform4b getUniform4b (string name)
		{
			return getUniform (name) as Uniform4b;
		}

     
		/// <summary>
		/// Returns the uniformMatrix2f of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix2f of this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Namea GLSL uniform name.
		/// </param>
		public UniformMatrix2f getUniformMatrix2f (string name)
		{
			return getUniform (name) as UniformMatrix2f;
		}

     
		/// <summary>
		/// Returns the uniformMatrix2d of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix2dof this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Namea GLSL uniform name.
		/// </param>
		public UniformMatrix2d getUniformMatrix2d (string name)
		{
			return getUniform (name) as UniformMatrix2d;
		}

     
		/// <summary>
		/// Returns the uniformMatrix3f of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix3f of this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
		public UniformMatrix3f getUniformMatrix3f (string name)
		{
			return getUniform (name) as UniformMatrix3f;
		}

      
		/// <summary>
		/// Returns the uniformMatrix3d of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix3dthis block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Namea GLSL uniform name.
		/// </param>
		public UniformMatrix3d getUniformMatrix3d (string name)
		{
			return getUniform (name) as UniformMatrix3d;
		}

    
		/// <summary>
		/// Returns the uniformMatrix4f of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix4f of this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
		public UniformMatrix4f getUniformMatrix4f (string name)
		{
			return getUniform (name) as UniformMatrix4f;
		}

       
		/// <summary>
		/// Returns the uniformMatrix4d of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix4dof this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Namea GLSL uniform name.
		/// </param>
		public UniformMatrix4d getUniformMatrix4d (string name)
		{
			return getUniform (name) as UniformMatrix4d;
		}

 
		/// <summary>
		/// RReturns the uniformMatrix2x3f of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix4dof this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Namea GLSL uniform name.
		/// </param>
		public UniformMatrix2x3f getUniformMatrix2x3f (string name)
		{
			return getUniform (name) as UniformMatrix2x3f;
		}

    
		/// <summary>
		/// Returns the uniformMatrix2x3d of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix2x3dof this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Namea GLSL uniform name.
		/// </param>
		public UniformMatrix2x3d getUniformMatrix2x3d (string name)
		{
			return getUniform (name) as UniformMatrix2x3d;
		}

    
		/// <summary>
		/// Returns the uniformMatrix2x4f of this block whose name is given.
		/// Gets the uniform matrix2x4f.
		/// </summary>
		/// <returns>
		/// The uniform matrix2x4fof this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Namea GLSL uniform name.
		/// </param>
		public UniformMatrix2x4f getUniformMatrix2x4f (string name)
		{
			return getUniform (name) as UniformMatrix2x4f;
		}

     
		/// <summary>
		/// Returns the uniformMatrix2x4d of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix2x4dof this block whose name is given,
		///  or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Namea GLSL uniform name.
		/// </param>
		public UniformMatrix2x4d getUniformMatrix2x4d (string name)
		{
			return getUniform (name) as UniformMatrix2x4d;
		}

		/// <summary>
		///  Returns the uniformMatrix3x2f of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix3x2fof this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Namea GLSL uniform name.
		/// </param>
		public UniformMatrix3x2f getUniformMatrix3x2f (string name)
		{
			return getUniform (name) as UniformMatrix3x2f;
		}

   
		/// <summary>
		/// Returns the uniformMatrix3x2d of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix3x2d of this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Namea GLSL uniform name.
		/// </param>
		public UniformMatrix3x2d getUniformMatrix3x2d (string name)
		{
			return getUniform (name) as UniformMatrix3x2d;
		}

   
		/// <summary>
		/// Returns the uniformMatrix3x4f of this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </summary>
		/// <returns>
		/// The uniform matrix3x4f.
		/// </returns>
		/// <param name='name'>
		/// Namea GLSL uniform name.
		/// </param>
		public UniformMatrix3x4f getUniformMatrix3x4f (string name)
		{
			return getUniform (name) as UniformMatrix3x4f;
		}

     
		/// <summary>
		///Returns the uniformMatrix3x4d of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix3x4dof this block whose name is given,
		///  or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
		public UniformMatrix3x4d getUniformMatrix3x4d (string name)
		{
			return getUniform (name) as UniformMatrix3x4d;
		}

    
		/// <summary>
		/// Returns the uniformMatrix4x2f of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix4x2f off this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
		public UniformMatrix4x2f getUniformMatrix4x2f (string name)
		{
			return getUniform (name) as UniformMatrix4x2f;
		}

     
		/// <summary>
		/// Returns the uniformMatrix4x2d of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix4x2dof this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Namea GLSL uniform name.
		/// </param>
		public UniformMatrix4x2d getUniformMatrix4x2d (string name)
		{
			return getUniform (name) as UniformMatrix4x2d;
		}

     
		/// <summary>
		/// Returns the uniformMatrix4x3f of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform matrix4x3fof this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Name a GLSL uniform name.
		/// </param>
		public UniformMatrix4x3f getUniformMatrix4x3f (string name)
		{
			return getUniform (name) as UniformMatrix4x3f;
		}


		/// <summary>
		/// Returns the uniformMatrix4x2d of this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </summary>
		/// <returns>
		/// The uniform matrix4x3d.
		/// </returns>
		/// <param name='name'>
		/// Namea GLSL uniform name.
		/// </param>
		public UniformMatrix4x3d getUniformMatrix4x3d (string name)
		{
			return getUniform (name) as UniformMatrix4x3d;
		}

  
		/// <summary>
		/// Returns the uniform sampler of this block whose name is given.
		/// </summary>
		/// <returns>
		/// The uniform samplerof this block whose name is given,
		/// or NULL if there is no such uniform.
		/// </returns>
		/// <param name='name'>
		/// Namea GLSL uniform name.
		/// </param>
		public UniformSampler getUniformSampler (string name)
		{
			return getUniform (name) as UniformSampler;
		}

		/// <summary>
		///  Sets the GPUBuffer to store the values of the uniforms of this block.
		/// </summary>
		/// <returns>
		/// The buffer a GPUBuffer. This buffer can be shared between different
		/// UniformBlock instances corresponding to the same uniform block
		/// declaration.
		/// </returns>
		/// <param name='buffer'>
		/// Buffer.
		/// </param>
		public void setBuffer (GPUBuffer buffer)
		{
			if (this.buffer != null) {// && isMapped())
				if (isMapped ())
					unmapBuffer ();
				UniformBlockBuffer b = this.buffer as UniformBlockBuffer;
				if (b != null) {
					buffers.Put (b.name);
				}
			}
            if (buffer == null && this.buffer != null)
                this.buffer.Dispose();
			this.buffer = buffer;
		}

		public void setValue<T> (int offset, int size, T data) where T : struct
		{
			this.buffer.setSubData<T> (offset, size, data);
		}

      
		/// <summary>
		/// The Program to which this uniform block belongs.
		/// </summary>
		internal protected Program program;

      
		/// <summary>
		///  The name of this uniform block.
		/// </summary>
		internal protected string name;

      
		/// <summary>
		/// The index of this uniform block in its program.
		/// </summary>
		internal protected int index;

       
		/// <summary>
		/// The total size of this uniform block's uniforms.
		/// </summary>
		internal protected int size;

       
		/// <summary>
		///The GPUBuffer that stores the values of the uniforms of this block.
		/// </summary>
		internal protected GPUBuffer buffer;

    
		/// <summary>
		/// The uniforms of this block.
		/// </summary>
		internal IDictionary<string, Uniform> uniforms = new Dictionary<string, Uniform> ();

        
		/// <summary>
		/// The buffers associated to each uniform blocks.
		/// When creating a new uniform block, the user should check if a buffer was
		/// already created for that UB name. Otherwise, he may create a new one.
		/// </summary>
		internal protected static Factory<string, GPUBuffer> buffers = new Factory<string, GPUBuffer> (UniformBlock.newBuffer);

     
		/// <summary>
		/// Callback method to create a new buffer. For use with #buffers.
		/// </summary>
		/// <returns>
		/// The buffer.
		/// </returns>
		/// <param name='name'>
		/// Name.
		/// </param>
		protected static GPUBuffer newBuffer (string name)
		{
			return new UniformBlockBuffer (name);
		}


		/// <summary>
		/// Creates a new uniform block.
		/// Initializes a new instance of the <see cref="Sxta.Render.UniformBlock"/> class.
		/// </summary>
		/// <param name='program'>
		/// Program to which this uniform block belongs.
		/// </param>
		/// <param name='name'>
		/// Namehe name of this uniform block in the GLSL code.
		/// </param>
		/// <param name='index'>
		/// Index the index of this uniform block in its program.
		/// </param>
		/// <param name='size'>
		/// Sizethe minimum buffer size to store the uniforms of this block.
		/// </param>
		internal UniformBlock (Program program, string name, int index, int size)
		{
			this.program = program;
			this.name = name;
			this.index = index;
			this.size = size;
			this.buffer = null;
		}

    
		/// <summary>
		/// Returns true if the GPUBuffer associated with this block is currently
		/// mapped in client memory.
		/// </summary>
		/// <returns>
		/// The mapped.
		/// </returns>
		internal protected bool isMapped ()
		{
			Debug.Assert (buffer != null);
			return buffer.getMappedData () != IntPtr.Zero;
		}

      
		/// <summary>
		/// Maps the GPUBuffer associated with this block in client memory. This
		/// method also returns the address in client memory of the valueC at the
		/// given offset in the buffer.

		/// </summary>
		/// <returns>
		/// a pointer to the valueC at 'offset' in the mapped buffer.
		/// </returns>
		/// <param name='offset'>
		/// Offset an offset in bytes from the start of the buffer.
		/// </param>
		protected internal IntPtr mapBuffer (int offset)
		{
			Debug.Assert (buffer != null);
			IntPtr result = buffer.getMappedData ();
			if (result == null) {
				result = buffer.map (BufferAccess.READ_WRITE);
			}
			return result + offset;
			//return (void*) (((unsigned char*) result) + offset);
		}

   
		/// <summary>
		/// Unmaps the GPUBuffer associated with this block in client memory.
		/// </summary>
		/// <returns>
		/// The buffer.
		/// </returns>
		internal protected void unmapBuffer ()
		{
			Debug.Assert (buffer != null && buffer.getMappedData () != null);
			buffer.unmap ();
		}

        #region Dispose

		// Track whether Dispose has been called. 
		private bool disposed = false;

		public void Dispose ()
		{
			Dispose (true);
			// This object will be cleaned up by the Dispose method. 
			// Therefore, you should call GC.SupressFinalize to 
			// take this object off the finalization queue 
			// and prevent finalization code for this object 
			// from executing a second time.
			GC.SuppressFinalize (this);
		}

		// Dispose(bool disposing) executes in two distinct scenarios. 
		// If disposing equals true, the method has been called directly 
		// or indirectly by a user's code. Managed and unmanaged resources 
		// can be disposed. 
		// If disposing equals false, the method has been called by the 
		// runtime from inside the finalizer and you should not reference 
		// other objects. Only unmanaged resources can be disposed. 
		protected virtual void Dispose (bool disposing)
		{
			// Check to see if Dispose has already been called. 
			if (!this.disposed) {
				// If disposing equals true, dispose all managed 
				// and unmanaged resources. 
				if (disposing) {
					// Dispose managed resources.
				}

				// Call the appropriate methods to clean up 
				// unmanaged resources here. 
				// If disposing is false, 
				// only the following code is executed.
				setBuffer (null);


				//Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
				// Note disposing has been done.
				disposed = true;

			}
		}
        #endregion


	}

	internal class UniformBlockBuffer : GPUBuffer
	{
		public string name;

		public UniformBlockBuffer (string name)
            : base()
		{
			this.name = name;
		}
	}
}
