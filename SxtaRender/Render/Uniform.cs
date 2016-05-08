
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Diagnostics;
using Sxta.Math;
using OpenTK.Graphics.OpenGL;

namespace Sxta.Render
{
		/// <summary>
		/// A uniform holding a single number.
		/// </summary>
		public class  Uniform1f : Uniform
		{
			/*
			 * Deletes this uniform.
			 */
			//~Uniform1f() {}

			public override UniformType getType()
			{
				return UniformType.VEC1F;
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public float get()  
			{
				if (block == null || program  == null) {
					return value;
				} else {
					//block.getValue<float(location);
					throw new NotImplementedException();
				}
			}

			/*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void set(float value)
			{
				this.value = value;
				if (block == null || program  == null) {
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location, sizeof(float), value);
				}
			}

			public override void setValue( Value  v)
			{
			    Value1f val = v as Value1f;
				if (val == null)
					throw new ArgumentException("Setting wrong value at Uniform1f");
				set(val.get());
			}


			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal Uniform1f(Program program, UniformBlock block, string name, int location) :
				base("Uniform1f", program, block, name, location)
			{
			}
			
			internal protected override void setValue()
			{
#if OPENTK
#if ORK_NO_GLPROGRAMUNIFORM
				GL.Uniform1(location, value);
				Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
#else
				GL.ProgramUniform1(program.getId(), location, value);
#endif
#else
#if ORK_NO_GLPROGRAMUNIFORM
				glUniform1f(location, value);
#else
				glProgramUniform1fEXT(program.getId(), location, value);
#endif
#endif
			}


			/*
			 * The current value of this uniform.
			 */
			private float value;
		}

		/// <summary>
		/// A uniform holding a single number.
		/// </summary>
		public class  Uniform1d : Uniform
		{
			/*
			 * Deletes this uniform.
			 */
			//~Uniform1d() {}

			public override UniformType getType()
			{
				return UniformType.VEC1D;
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public double get()  
			{
				if (block == null || program  == null) {
					return value;
				} else {
					//block.getValue<double(location);
					throw new NotImplementedException();
				}
			}

			/*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void set(double value)
			{
				this.value = value;
				if (block == null || program  == null) {
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location, sizeof(double), value);
				}
			}

			public override void setValue( Value  v)
			{
			    Value1d val = v as Value1d;
				if (val == null)
					throw new ArgumentException("Setting wrong value at Uniform1d");
				set(val.get());
			}


			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal Uniform1d(Program program, UniformBlock block, string name, int location) :
				base("Uniform1d", program, block, name, location)
			{
			}
			
			internal protected override void setValue()
			{
#if OPENTK
#if ORK_NO_GLPROGRAMUNIFORM
				GL.Uniform1(location, value);
				Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
#else
				GL.ProgramUniform1(program.getId(), location, value);
#endif
#else
#if ORK_NO_GLPROGRAMUNIFORM
				glUniform1f(location, value);
#else
				glProgramUniform1fEXT(program.getId(), location, value);
#endif
#endif
			}


			/*
			 * The current value of this uniform.
			 */
			private double value;
		}

		/// <summary>
		/// A uniform holding a single number.
		/// </summary>
		public class  Uniform1i : Uniform
		{
			/*
			 * Deletes this uniform.
			 */
			//~Uniform1i() {}

			public override UniformType getType()
			{
				return UniformType.VEC1I;
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public int get()  
			{
				if (block == null || program  == null) {
					return value;
				} else {
					//block.getValue<int(location);
					throw new NotImplementedException();
				}
			}

			/*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void set(int value)
			{
				this.value = value;
				if (block == null || program  == null) {
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location, sizeof(int), value);
				}
			}

			public override void setValue( Value  v)
			{
			    Value1i val = v as Value1i;
				if (val == null)
					throw new ArgumentException("Setting wrong value at Uniform1i");
				set(val.get());
			}


			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal Uniform1i(Program program, UniformBlock block, string name, int location) :
				base("Uniform1i", program, block, name, location)
			{
			}
			
			internal protected override void setValue()
			{
#if OPENTK
#if ORK_NO_GLPROGRAMUNIFORM
				GL.Uniform1(location, value);
				Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
#else
				GL.ProgramUniform1(program.getId(), location, value);
#endif
#else
#if ORK_NO_GLPROGRAMUNIFORM
				glUniform1f(location, value);
#else
				glProgramUniform1fEXT(program.getId(), location, value);
#endif
#endif
			}


			/*
			 * The current value of this uniform.
			 */
			private int value;
		}

		/// <summary>
		/// A uniform holding a single number.
		/// </summary>
		public class  Uniform1ui : Uniform
		{
			/*
			 * Deletes this uniform.
			 */
			//~Uniform1ui() {}

			public override UniformType getType()
			{
				return UniformType.VEC1UI;
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public uint get()  
			{
				if (block == null || program  == null) {
					return value;
				} else {
					//block.getValue<uint(location);
					throw new NotImplementedException();
				}
			}

			/*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void set(uint value)
			{
				this.value = value;
				if (block == null || program  == null) {
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location, sizeof(uint), value);
				}
			}

			public override void setValue( Value  v)
			{
			    Value1ui val = v as Value1ui;
				if (val == null)
					throw new ArgumentException("Setting wrong value at Uniform1ui");
				set(val.get());
			}


			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal Uniform1ui(Program program, UniformBlock block, string name, int location) :
				base("Uniform1ui", program, block, name, location)
			{
			}
			
			internal protected override void setValue()
			{
#if OPENTK
#if ORK_NO_GLPROGRAMUNIFORM
				GL.Uniform1(location, value);
				Debug.Assert(FrameBuffer.getError() == ErrorCode.NoError);
#else
				GL.ProgramUniform1(program.getId(), location, value);
#endif
#else
#if ORK_NO_GLPROGRAMUNIFORM
				glUniform1f(location, value);
#else
				glProgramUniform1fEXT(program.getId(), location, value);
#endif
#endif
			}


			/*
			 * The current value of this uniform.
			 */
			private uint value;
		}

		/// <summary>
		/// A uniform holding a single number.
		/// </summary>
		public class  Uniform1b : Uniform
		{
			/*
			 * Deletes this uniform.
			 */
			//~Uniform1b() {}

			public override UniformType getType()
			{
				return UniformType.VEC1B;
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public bool get()  
			{
				if (block == null || program  == null) {
					return value;
				} else {
					//block.getValue<bool(location);
					throw new NotImplementedException();
				}
			}

			/*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void set(bool value)
			{
				this.value = value;
				if (block == null || program  == null) {
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location, sizeof(bool), value);
				}
			}

			public override void setValue( Value  v)
			{
			    Value1b val = v as Value1b;
				if (val == null)
					throw new ArgumentException("Setting wrong value at Uniform1b");
				set(val.get());
			}


			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal Uniform1b(Program program, UniformBlock block, string name, int location) :
				base("Uniform1b", program, block, name, location)
			{
			}
			
			internal protected override void setValue()
			{
#if OPENTK
#if ORK_NO_GLPROGRAMUNIFORM
				GL.Uniform1(location, (value? 1 : 0));
#else
				GL.ProgramUniform1(program.getId(), location, (value? 1 : 0));
#endif
#else
#if ORK_NO_GLPROGRAMUNIFORM
				glUniform1f(location, value);
#else
				glProgramUniform1fEXT(program.getId(), location, value);
#endif
#endif
			}


			/*
			 * The current value of this uniform.
			 */
			private bool value;
		}

		/// <summary>
		/// A uniform holding a two vector value.
		/// </summary>
		public class  Uniform2f : Uniform
		{
			/*
			 * Deletes this uniform.
			 */
			//~Uniform2f() {}

			public override UniformType getType()
			{
				return UniformType.VEC2F;
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public Vector2f get()  
			{
				if (block == null || program  == null) {
					return value;
				} else {
#if TODO
				    float[] buf = (float[]) mapBuffer(location);
					return new Vector2f(buf[0], buf[1]);
#endif
				    throw new NotImplementedException();
				}
			}

			/*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void set(Vector2f value)
			{
				if (block == null || program  == null) {
					this.value = value;
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location, Vector2f.SizeInBytes, value);
				}
			}

			public override void setValue( Value  v)
			{
			    Value2f val = v as Value2f;
				if (val == null)
					throw new ArgumentException("Setting wrong value at Uniform2f");
				set(val.get());
			}


			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal Uniform2f(Program program, UniformBlock block, string name, int location) :
				base("Uniform2f", program, block, name, location)
			{
			}

			internal protected override void setValue()
			{
#if OPENTK
#if  ORK_NO_GLPROGRAMUNIFORM
				GL.Uniform2(location, value.X, value.Y);
#else
				GL.ProgramUniform2(program.getId(),location, value.X, value.Y);
#endif
#else
#if  ORK_NO_GLPROGRAMUNIFORM
				glUniform2f(location, value.X, value.Y);
#else
				glProgramUniform2fEXT(program->getId(), location, value.X, value.Y);
#endif
#endif
			}


			/*
			 * The current value of this uniform.
			 */
			private Vector2f value;
		}
		/// <summary>
		/// A uniform holding a two vector value.
		/// </summary>
		public class  Uniform2d : Uniform
		{
			/*
			 * Deletes this uniform.
			 */
			//~Uniform2d() {}

			public override UniformType getType()
			{
				return UniformType.VEC2D;
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public Vector2d get()  
			{
				if (block == null || program  == null) {
					return value;
				} else {
#if TODO
				    double[] buf = (double[]) mapBuffer(location);
					return new Vector2d(buf[0], buf[1]);
#endif
				    throw new NotImplementedException();
				}
			}

			/*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void set(Vector2d value)
			{
				if (block == null || program  == null) {
					this.value = value;
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location, Vector2d.SizeInBytes, value);
				}
			}

			public override void setValue( Value  v)
			{
			    Value2d val = v as Value2d;
				if (val == null)
					throw new ArgumentException("Setting wrong value at Uniform2d");
				set(val.get());
			}


			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal Uniform2d(Program program, UniformBlock block, string name, int location) :
				base("Uniform2d", program, block, name, location)
			{
			}

			internal protected override void setValue()
			{
#if OPENTK
#if  ORK_NO_GLPROGRAMUNIFORM
				GL.Uniform2(location, value.X, value.Y);
#else
				GL.ProgramUniform2(program.getId(),location, value.X, value.Y);
#endif
#else
#if  ORK_NO_GLPROGRAMUNIFORM
				glUniform2f(location, value.X, value.Y);
#else
				glProgramUniform2fEXT(program->getId(), location, value.X, value.Y);
#endif
#endif
			}


			/*
			 * The current value of this uniform.
			 */
			private Vector2d value;
		}
		/// <summary>
		/// A uniform holding a two vector value.
		/// </summary>
		public class  Uniform2i : Uniform
		{
			/*
			 * Deletes this uniform.
			 */
			//~Uniform2i() {}

			public override UniformType getType()
			{
				return UniformType.VEC2I;
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public Vector2i get()  
			{
				if (block == null || program  == null) {
					return value;
				} else {
#if TODO
				    int[] buf = (int[]) mapBuffer(location);
					return new Vector2i(buf[0], buf[1]);
#endif
				    throw new NotImplementedException();
				}
			}

			/*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void set(Vector2i value)
			{
				if (block == null || program  == null) {
					this.value = value;
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location, Vector2i.SizeInBytes, value);
				}
			}

			public override void setValue( Value  v)
			{
			    Value2i val = v as Value2i;
				if (val == null)
					throw new ArgumentException("Setting wrong value at Uniform2i");
				set(val.get());
			}


			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal Uniform2i(Program program, UniformBlock block, string name, int location) :
				base("Uniform2i", program, block, name, location)
			{
			}

			internal protected override void setValue()
			{
#if OPENTK
#if  ORK_NO_GLPROGRAMUNIFORM
				GL.Uniform2(location, value.X, value.Y);
#else
				GL.ProgramUniform2(program.getId(),location, value.X, value.Y);
#endif
#else
#if  ORK_NO_GLPROGRAMUNIFORM
				glUniform2f(location, value.X, value.Y);
#else
				glProgramUniform2fEXT(program->getId(), location, value.X, value.Y);
#endif
#endif
			}


			/*
			 * The current value of this uniform.
			 */
			private Vector2i value;
		}
		/// <summary>
		/// A uniform holding a two vector value.
		/// </summary>
		public class  Uniform2ui : Uniform
		{
			/*
			 * Deletes this uniform.
			 */
			//~Uniform2ui() {}

			public override UniformType getType()
			{
				return UniformType.VEC2UI;
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public Vector2ui get()  
			{
				if (block == null || program  == null) {
					return value;
				} else {
#if TODO
				    uint[] buf = (uint[]) mapBuffer(location);
					return new Vector2ui(buf[0], buf[1]);
#endif
				    throw new NotImplementedException();
				}
			}

			/*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void set(Vector2ui value)
			{
				if (block == null || program  == null) {
					this.value = value;
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location, Vector2ui.SizeInBytes, value);
				}
			}

			public override void setValue( Value  v)
			{
			    Value2ui val = v as Value2ui;
				if (val == null)
					throw new ArgumentException("Setting wrong value at Uniform2ui");
				set(val.get());
			}


			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal Uniform2ui(Program program, UniformBlock block, string name, int location) :
				base("Uniform2ui", program, block, name, location)
			{
			}

			internal protected override void setValue()
			{
#if OPENTK
#if  ORK_NO_GLPROGRAMUNIFORM
				GL.Uniform2(location, value.X, value.Y);
#else
				GL.ProgramUniform2(program.getId(),location, value.X, value.Y);
#endif
#else
#if  ORK_NO_GLPROGRAMUNIFORM
				glUniform2f(location, value.X, value.Y);
#else
				glProgramUniform2fEXT(program->getId(), location, value.X, value.Y);
#endif
#endif
			}


			/*
			 * The current value of this uniform.
			 */
			private Vector2ui value;
		}
		/// <summary>
		/// A uniform holding a two vector value.
		/// </summary>
		public class  Uniform2b : Uniform
		{
			/*
			 * Deletes this uniform.
			 */
			//~Uniform2b() {}

			public override UniformType getType()
			{
				return UniformType.VEC2B;
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public Vector2b get()  
			{
				if (block == null || program  == null) {
					return value;
				} else {
#if TODO
				    bool[] buf = (bool[]) mapBuffer(location);
					return new Vector2b(buf[0], buf[1]);
#endif
				    throw new NotImplementedException();
				}
			}

			/*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void set(Vector2b value)
			{
				if (block == null || program  == null) {
					this.value = value;
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location, Vector2b.SizeInBytes, value);
				}
			}

			public override void setValue( Value  v)
			{
			    Value2b val = v as Value2b;
				if (val == null)
					throw new ArgumentException("Setting wrong value at Uniform2b");
				set(val.get());
			}


			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal Uniform2b(Program program, UniformBlock block, string name, int location) :
				base("Uniform2b", program, block, name, location)
			{
			}

			internal protected override void setValue()
			{
#if OPENTK
#if  ORK_NO_GLPROGRAMUNIFORM
				GL.Uniform2(location, (value.X? 1 : 0), (value.Y? 1 : 0));
#else
				GL.ProgramUniform2(program.getId(), location, (value.X? 1 : 0), (value.Y? 1 : 0));
#endif
#else
#if  ORK_NO_GLPROGRAMUNIFORM
				glUniform2f(location, value.X, value.Y);
#else
				glProgramUniform2fEXT(program->getId(), location, value.X, value.Y);
#endif
#endif
			}


			/*
			 * The current value of this uniform.
			 */
			private Vector2b value;
		}
		/// <summary>
		/// A uniform holding a three vector value.
		/// </summary>
		public class  Uniform3f : Uniform
		{
			/*
			 * Deletes this uniform.
			 */
			//~Uniform3f() {}

			public override UniformType getType()
			{
				return UniformType.VEC3F;
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public Vector3f get()  
			{
				if (block == null || program  == null) {
					return value;
				} else {
#if TODO
				    float[] buf = (float[]) mapBuffer(location);
					return new Vector3f(buf[0], buf[1], buf[2]);
#endif
				    throw new NotImplementedException();
				}
			}

			/*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void set(Vector3f value)
			{
				if (block == null || program  == null) {
					this.value = value;
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location,Vector3f.SizeInBytes, value);
				}
			}

			public override void setValue( Value  v)
			{
#if TODO
				set(v);					
#endif
				throw new NotImplementedException();
			}


			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal Uniform3f(Program program, UniformBlock block, string name, int location) :
				base("Uniform3f", program, block, name, location)
			{
			}

			internal protected override void setValue()
			{
#if OPENTK
#if  ORK_NO_GLPROGRAMUNIFORM
				GL.Uniform3(location, value.X, value.Y, value.Z);
#else
				GL.ProgramUniform3(program.getId(), location, value.X, value.Y, value.Z);
#endif
#else
#if  ORK_NO_GLPROGRAMUNIFORM
				glUniform3f(location, value.X, value.Y, value.Z);
#else
				glProgramUniform3fEXT(program->getId(), location, value.X, value.Y, value.Z);
#endif
#endif
			}


			/*
			 * The current value of this uniform.
			 */
			private Vector3f value;
		}
		/// <summary>
		/// A uniform holding a three vector value.
		/// </summary>
		public class  Uniform3d : Uniform
		{
			/*
			 * Deletes this uniform.
			 */
			//~Uniform3d() {}

			public override UniformType getType()
			{
				return UniformType.VEC3D;
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public Vector3d get()  
			{
				if (block == null || program  == null) {
					return value;
				} else {
#if TODO
				    double[] buf = (double[]) mapBuffer(location);
					return new Vector3d(buf[0], buf[1], buf[2]);
#endif
				    throw new NotImplementedException();
				}
			}

			/*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void set(Vector3d value)
			{
				if (block == null || program  == null) {
					this.value = value;
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location,Vector3d.SizeInBytes, value);
				}
			}

			public override void setValue( Value  v)
			{
#if TODO
				set(v);					
#endif
				throw new NotImplementedException();
			}


			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal Uniform3d(Program program, UniformBlock block, string name, int location) :
				base("Uniform3d", program, block, name, location)
			{
			}

			internal protected override void setValue()
			{
#if OPENTK
#if  ORK_NO_GLPROGRAMUNIFORM
				GL.Uniform3(location, value.X, value.Y, value.Z);
#else
				GL.ProgramUniform3(program.getId(), location, value.X, value.Y, value.Z);
#endif
#else
#if  ORK_NO_GLPROGRAMUNIFORM
				glUniform3f(location, value.X, value.Y, value.Z);
#else
				glProgramUniform3fEXT(program->getId(), location, value.X, value.Y, value.Z);
#endif
#endif
			}


			/*
			 * The current value of this uniform.
			 */
			private Vector3d value;
		}
		/// <summary>
		/// A uniform holding a three vector value.
		/// </summary>
		public class  Uniform3i : Uniform
		{
			/*
			 * Deletes this uniform.
			 */
			//~Uniform3i() {}

			public override UniformType getType()
			{
				return UniformType.VEC3I;
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public Vector3i get()  
			{
				if (block == null || program  == null) {
					return value;
				} else {
#if TODO
				    int[] buf = (int[]) mapBuffer(location);
					return new Vector3i(buf[0], buf[1], buf[2]);
#endif
				    throw new NotImplementedException();
				}
			}

			/*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void set(Vector3i value)
			{
				if (block == null || program  == null) {
					this.value = value;
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location,Vector3i.SizeInBytes, value);
				}
			}

			public override void setValue( Value  v)
			{
#if TODO
				set(v);					
#endif
				throw new NotImplementedException();
			}


			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal Uniform3i(Program program, UniformBlock block, string name, int location) :
				base("Uniform3i", program, block, name, location)
			{
			}

			internal protected override void setValue()
			{
#if OPENTK
#if  ORK_NO_GLPROGRAMUNIFORM
				GL.Uniform3(location, value.X, value.Y, value.Z);
#else
				GL.ProgramUniform3(program.getId(), location, value.X, value.Y, value.Z);
#endif
#else
#if  ORK_NO_GLPROGRAMUNIFORM
				glUniform3f(location, value.X, value.Y, value.Z);
#else
				glProgramUniform3fEXT(program->getId(), location, value.X, value.Y, value.Z);
#endif
#endif
			}


			/*
			 * The current value of this uniform.
			 */
			private Vector3i value;
		}
		/// <summary>
		/// A uniform holding a three vector value.
		/// </summary>
		public class  Uniform3ui : Uniform
		{
			/*
			 * Deletes this uniform.
			 */
			//~Uniform3ui() {}

			public override UniformType getType()
			{
				return UniformType.VEC3UI;
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public Vector3ui get()  
			{
				if (block == null || program  == null) {
					return value;
				} else {
#if TODO
				    uint[] buf = (uint[]) mapBuffer(location);
					return new Vector3ui(buf[0], buf[1], buf[2]);
#endif
				    throw new NotImplementedException();
				}
			}

			/*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void set(Vector3ui value)
			{
				if (block == null || program  == null) {
					this.value = value;
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location,Vector3ui.SizeInBytes, value);
				}
			}

			public override void setValue( Value  v)
			{
#if TODO
				set(v);					
#endif
				throw new NotImplementedException();
			}


			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal Uniform3ui(Program program, UniformBlock block, string name, int location) :
				base("Uniform3ui", program, block, name, location)
			{
			}

			internal protected override void setValue()
			{
#if OPENTK
#if  ORK_NO_GLPROGRAMUNIFORM
				GL.Uniform3(location, value.X, value.Y, value.Z);
#else
				GL.ProgramUniform3(program.getId(), location, value.X, value.Y, value.Z);
#endif
#else
#if  ORK_NO_GLPROGRAMUNIFORM
				glUniform3f(location, value.X, value.Y, value.Z);
#else
				glProgramUniform3fEXT(program->getId(), location, value.X, value.Y, value.Z);
#endif
#endif
			}


			/*
			 * The current value of this uniform.
			 */
			private Vector3ui value;
		}
		/// <summary>
		/// A uniform holding a three vector value.
		/// </summary>
		public class  Uniform3b : Uniform
		{
			/*
			 * Deletes this uniform.
			 */
			//~Uniform3b() {}

			public override UniformType getType()
			{
				return UniformType.VEC3B;
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public Vector3b get()  
			{
				if (block == null || program  == null) {
					return value;
				} else {
#if TODO
				    bool[] buf = (bool[]) mapBuffer(location);
					return new Vector3b(buf[0], buf[1], buf[2]);
#endif
				    throw new NotImplementedException();
				}
			}

			/*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void set(Vector3b value)
			{
				if (block == null || program  == null) {
					this.value = value;
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location,Vector3b.SizeInBytes, value);
				}
			}

			public override void setValue( Value  v)
			{
#if TODO
				set(v);					
#endif
				throw new NotImplementedException();
			}


			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal Uniform3b(Program program, UniformBlock block, string name, int location) :
				base("Uniform3b", program, block, name, location)
			{
			}

			internal protected override void setValue()
			{
#if OPENTK
#if  ORK_NO_GLPROGRAMUNIFORM
				GL.Uniform3(location, (value.X? 1 : 0), (value.Y? 1 : 0), (value.Z? 1 : 0));
#else
				GL.Uniform3(location, (value.X? 1 : 0), (value.Y? 1 : 0), (value.Z? 1 : 0));
#endif
#else
#if  ORK_NO_GLPROGRAMUNIFORM
				glUniform3f(location, value.X, value.Y, value.Z);
#else
				glProgramUniform3fEXT(program->getId(), location, value.X, value.Y, value.Z);
#endif
#endif
			}


			/*
			 * The current value of this uniform.
			 */
			private Vector3b value;
		}

		/// <summary>
		/// A uniform holding a four vector value.
		/// </summary>
		public class  Uniform4f : Uniform
		{
			/*
			 * Deletes this uniform.
			 */
			//~Uniform4f() {}

			public override UniformType getType()
			{
				return UniformType.VEC4F;
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public Vector4f get()  
			{
				if (block == null || program  == null) {
					return value;
				} else {
#if TODO
				    float[] buf = (float[]) mapBuffer(location);
					return new Vector4f(buf[0], buf[1], buf[2], buf[4]);
#endif
				    throw new NotImplementedException();
				}
			}

			/*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void set(Vector4f value)
			{
				if (block == null || program  == null) {
					this.value = value;
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location, Vector4f.SizeInBytes, value);
				}
			}

			public override void setValue( Value  v)
			{
#if TODO
				set(v);					
#endif
				throw new NotImplementedException();
			}


			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal Uniform4f(Program program, UniformBlock block, string name, int location) :
				base("Uniform4f", program, block, name, location)
			{
			}

			internal protected override void setValue()
			{
#if OPENTK
#if  ORK_NO_GLPROGRAMUNIFORM
				GL.Uniform4(location, value.X, value.Y, value.Z, value.W);
#else
				GL.ProgramUniform4(program.getId(), location, value.X, value.Y, value.Z, value.W);
#endif
#else
#if  ORK_NO_GLPROGRAMUNIFORM
				glUniform4f(location, value.X, value.Y, value.Z, value.W);
#else
				glProgramUniform4fEXT(program->getId(), location, value.X, value.Y, value.Z, value.W);
#endif
#endif
			}


			/*
			 * The current value of this uniform.
			 */
			private Vector4f value;
		}

		/// <summary>
		/// A uniform holding a four vector value.
		/// </summary>
		public class  Uniform4d : Uniform
		{
			/*
			 * Deletes this uniform.
			 */
			//~Uniform4d() {}

			public override UniformType getType()
			{
				return UniformType.VEC4D;
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public Vector4d get()  
			{
				if (block == null || program  == null) {
					return value;
				} else {
#if TODO
				    double[] buf = (double[]) mapBuffer(location);
					return new Vector4d(buf[0], buf[1], buf[2], buf[4]);
#endif
				    throw new NotImplementedException();
				}
			}

			/*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void set(Vector4d value)
			{
				if (block == null || program  == null) {
					this.value = value;
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location, Vector4d.SizeInBytes, value);
				}
			}

			public override void setValue( Value  v)
			{
#if TODO
				set(v);					
#endif
				throw new NotImplementedException();
			}


			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal Uniform4d(Program program, UniformBlock block, string name, int location) :
				base("Uniform4d", program, block, name, location)
			{
			}

			internal protected override void setValue()
			{
#if OPENTK
#if  ORK_NO_GLPROGRAMUNIFORM
				GL.Uniform4(location, value.X, value.Y, value.Z, value.W);
#else
				GL.ProgramUniform4(program.getId(), location, value.X, value.Y, value.Z, value.W);
#endif
#else
#if  ORK_NO_GLPROGRAMUNIFORM
				glUniform4f(location, value.X, value.Y, value.Z, value.W);
#else
				glProgramUniform4fEXT(program->getId(), location, value.X, value.Y, value.Z, value.W);
#endif
#endif
			}


			/*
			 * The current value of this uniform.
			 */
			private Vector4d value;
		}

		/// <summary>
		/// A uniform holding a four vector value.
		/// </summary>
		public class  Uniform4i : Uniform
		{
			/*
			 * Deletes this uniform.
			 */
			//~Uniform4i() {}

			public override UniformType getType()
			{
				return UniformType.VEC4I;
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public Vector4i get()  
			{
				if (block == null || program  == null) {
					return value;
				} else {
#if TODO
				    int[] buf = (int[]) mapBuffer(location);
					return new Vector4i(buf[0], buf[1], buf[2], buf[4]);
#endif
				    throw new NotImplementedException();
				}
			}

			/*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void set(Vector4i value)
			{
				if (block == null || program  == null) {
					this.value = value;
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location, Vector4i.SizeInBytes, value);
				}
			}

			public override void setValue( Value  v)
			{
#if TODO
				set(v);					
#endif
				throw new NotImplementedException();
			}


			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal Uniform4i(Program program, UniformBlock block, string name, int location) :
				base("Uniform4i", program, block, name, location)
			{
			}

			internal protected override void setValue()
			{
#if OPENTK
#if  ORK_NO_GLPROGRAMUNIFORM
				GL.Uniform4(location, value.X, value.Y, value.Z, value.W);
#else
				GL.ProgramUniform4(program.getId(), location, value.X, value.Y, value.Z, value.W);
#endif
#else
#if  ORK_NO_GLPROGRAMUNIFORM
				glUniform4f(location, value.X, value.Y, value.Z, value.W);
#else
				glProgramUniform4fEXT(program->getId(), location, value.X, value.Y, value.Z, value.W);
#endif
#endif
			}


			/*
			 * The current value of this uniform.
			 */
			private Vector4i value;
		}

		/// <summary>
		/// A uniform holding a four vector value.
		/// </summary>
		public class  Uniform4ui : Uniform
		{
			/*
			 * Deletes this uniform.
			 */
			//~Uniform4ui() {}

			public override UniformType getType()
			{
				return UniformType.VEC4UI;
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public Vector4ui get()  
			{
				if (block == null || program  == null) {
					return value;
				} else {
#if TODO
				    uint[] buf = (uint[]) mapBuffer(location);
					return new Vector4ui(buf[0], buf[1], buf[2], buf[4]);
#endif
				    throw new NotImplementedException();
				}
			}

			/*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void set(Vector4ui value)
			{
				if (block == null || program  == null) {
					this.value = value;
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location, Vector4ui.SizeInBytes, value);
				}
			}

			public override void setValue( Value  v)
			{
#if TODO
				set(v);					
#endif
				throw new NotImplementedException();
			}


			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal Uniform4ui(Program program, UniformBlock block, string name, int location) :
				base("Uniform4ui", program, block, name, location)
			{
			}

			internal protected override void setValue()
			{
#if OPENTK
#if  ORK_NO_GLPROGRAMUNIFORM
				GL.Uniform4(location, value.X, value.Y, value.Z, value.W);
#else
				GL.ProgramUniform4(program.getId(), location, value.X, value.Y, value.Z, value.W);
#endif
#else
#if  ORK_NO_GLPROGRAMUNIFORM
				glUniform4f(location, value.X, value.Y, value.Z, value.W);
#else
				glProgramUniform4fEXT(program->getId(), location, value.X, value.Y, value.Z, value.W);
#endif
#endif
			}


			/*
			 * The current value of this uniform.
			 */
			private Vector4ui value;
		}

		/// <summary>
		/// A uniform holding a four vector value.
		/// </summary>
		public class  Uniform4b : Uniform
		{
			/*
			 * Deletes this uniform.
			 */
			//~Uniform4b() {}

			public override UniformType getType()
			{
				return UniformType.VEC4B;
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public Vector4b get()  
			{
				if (block == null || program  == null) {
					return value;
				} else {
#if TODO
				    bool[] buf = (bool[]) mapBuffer(location);
					return new Vector4b(buf[0], buf[1], buf[2], buf[4]);
#endif
				    throw new NotImplementedException();
				}
			}

			/*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void set(Vector4b value)
			{
				if (block == null || program  == null) {
					this.value = value;
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location, Vector4b.SizeInBytes, value);
				}
			}

			public override void setValue( Value  v)
			{
#if TODO
				set(v);					
#endif
				throw new NotImplementedException();
			}


			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal Uniform4b(Program program, UniformBlock block, string name, int location) :
				base("Uniform4b", program, block, name, location)
			{
			}

			internal protected override void setValue()
			{
#if OPENTK
#if  ORK_NO_GLPROGRAMUNIFORM
				GL.Uniform4(location, (value.X? 1 : 0), (value.Y? 1 : 0), (value.Z? 1 : 0), (value.W? 1 : 0));
#else
				GL.ProgramUniform4(program.getId(), location, (value.X? 1 : 0), (value.Y? 1 : 0), (value.Z? 1 : 0), (value.W? 1 : 0));
#endif
#else
#if  ORK_NO_GLPROGRAMUNIFORM
				glUniform4f(location, value.X, value.Y, value.Z, value.W);
#else
				glProgramUniform4fEXT(program->getId(), location, value.X, value.Y, value.Z, value.W);
#endif
#endif
			}


			/*
			 * The current value of this uniform.
			 */
			private Vector4b value;
		}
		public class  UniformMatrix2f : Uniform
		{
			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal UniformMatrix2f(Program program, UniformBlock block, string name, int location, int stride, bool isRowMajor) :
				base("UniformMatrix2f", program, block, name, location)
			{
				this.stride = stride; 
				this.isRowMajor = isRowMajor;
			}
 
			public override UniformType getType()
			{
				return UniformType.MAT2F;
			}
			
			public Matrix2f get()
			{
#if TODO
				if (block != NULL && program != NULL) {
					unsigned char *buf = (unsigned char*) mapBuffer(location);
					if (isRowMajor) {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) value)[r * C + c] = ((T*) (buf + r * stride))[c];
							}
						}
					} else {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) value)[r * C + c] = ((T*) (buf + c * stride))[r];
							}
						}
					}
				}
#endif
				return value;
			}
			
			/*
             * Sets the value of this uniform.
             *
             * @param value the matrix coefficients in row major order (i.e. first row,
             *      second row, etc).
             */
            public void set(Matrix2f value)
            {
			   if (block == null || program  == null) {
					this.value = value;
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location, Matrix2f.SizeInBytes, value);
#if TODO
					unsigned char *buf = (unsigned char*) mapBuffer(location);
					if (isRowMajor) {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) (buf + r * stride))[c] = value[r * C + c];
							}
						}
					} else {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) (buf + c * stride))[r] = value[r * C + c];
							}
						}
					}
#endif
			         throw new NotImplementedException();
				} 
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public Matrix2f getMatrix()
			{
				return this.get();
			}

		   /*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void setMatrix(Matrix2f val)
			{
				this.set(val);
			}

			public override void setValue( Value  v)
			{
				throw new NotImplementedException();
			}

			internal protected override void setValue()
			{
#if OPENTK
#if ORK_NO_GLPROGRAMUNIFORM
			Sxta.OpenGL.GL.UniformMatrix2(location, 1, isRowMajor, ref value);
#else
			GL.ProgramUniformMatrix2(program.getId(), location, 1, isRowMajor, (float[])  value);
#endif
#else
#if ORK_NO_GLPROGRAMUNIFORM
			glUniformMatrix2fv(location, 1, true, value);
#else
			glProgramUniformMatrix2fvEXT(program.getId(), location, 1, true, value);
#endif
#endif
			}

			/*
		     * The current value of this uniform.
			 * The matrix coefficients are stored in column major order.
			 */
			Matrix2f value;

			/*
			* The stride between two consecutive rows or columns when this uniform is
			* stored in an uniform block.
			*/
			private int stride;

			/*
			* True if this uniform is stored in row major order in an uniform block.
			*/
			private bool isRowMajor = false;

			private const int R = 2;
			private const int C = 2;
		}
		public class  UniformMatrix2d : Uniform
		{
			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal UniformMatrix2d(Program program, UniformBlock block, string name, int location, int stride, bool isRowMajor) :
				base("UniformMatrix2d", program, block, name, location)
			{
				this.stride = stride; 
				this.isRowMajor = isRowMajor;
			}
 
			public override UniformType getType()
			{
				return UniformType.MAT2D;
			}
			
			public Matrix2d get()
			{
#if TODO
				if (block != NULL && program != NULL) {
					unsigned char *buf = (unsigned char*) mapBuffer(location);
					if (isRowMajor) {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) value)[r * C + c] = ((T*) (buf + r * stride))[c];
							}
						}
					} else {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) value)[r * C + c] = ((T*) (buf + c * stride))[r];
							}
						}
					}
				}
#endif
				return value;
			}
			
			/*
             * Sets the value of this uniform.
             *
             * @param value the matrix coefficients in row major order (i.e. first row,
             *      second row, etc).
             */
            public void set(Matrix2d value)
            {
			   if (block == null || program  == null) {
					this.value = value;
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location, Matrix2d.SizeInBytes, value);
#if TODO
					unsigned char *buf = (unsigned char*) mapBuffer(location);
					if (isRowMajor) {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) (buf + r * stride))[c] = value[r * C + c];
							}
						}
					} else {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) (buf + c * stride))[r] = value[r * C + c];
							}
						}
					}
#endif
			         throw new NotImplementedException();
				} 
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public Matrix2d getMatrix()
			{
				return this.get();
			}

		   /*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void setMatrix(Matrix2d val)
			{
				this.set(val);
			}

			public override void setValue( Value  v)
			{
				throw new NotImplementedException();
			}

			internal protected override void setValue()
			{
#if OPENTK
#if ORK_NO_GLPROGRAMUNIFORM
			Sxta.OpenGL.GL.UniformMatrix2(location, 1, isRowMajor, ref value);
#else
			GL.ProgramUniformMatrix2(program.getId(), location, 1, isRowMajor, (double[])  value);
#endif
#else
#if ORK_NO_GLPROGRAMUNIFORM
			glUniformMatrix2fv(location, 1, true, value);
#else
			glProgramUniformMatrix2fvEXT(program.getId(), location, 1, true, value);
#endif
#endif
			}

			/*
		     * The current value of this uniform.
			 * The matrix coefficients are stored in column major order.
			 */
			Matrix2d value;

			/*
			* The stride between two consecutive rows or columns when this uniform is
			* stored in an uniform block.
			*/
			private int stride;

			/*
			* True if this uniform is stored in row major order in an uniform block.
			*/
			private bool isRowMajor = false;

			private const int R = 2;
			private const int C = 2;
		}
		
		public class  UniformMatrix3f : Uniform
		{
			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal UniformMatrix3f(Program program, UniformBlock block, string name, int location, int stride, bool isRowMajor) :
				base("UniformMatrix3f", program, block, name, location)
			{
				this.stride = stride; 
				this.isRowMajor = isRowMajor;
			}

			public override UniformType getType()
			{
				return UniformType.MAT3F;
			}

			public Matrix3f get()
			{
#if TODO
				if (block != NULL && program != NULL) {
					unsigned char *buf = (unsigned char*) mapBuffer(location);
					if (isRowMajor) {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) value)[r * C + c] = ((T*) (buf + r * stride))[c];
							}
						}
					} else {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) value)[r * C + c] = ((T*) (buf + c * stride))[r];
							}
						}
					}
				}
#endif
				return value;
			}
			
			/*
             * Sets the value of this uniform.
             *
             * @param value the matrix coefficients in row major order (i.e. first row,
             *      second row, etc).
             */
            public void set(Matrix3f value)
            {
			   if (block == null || program  == null) {
					this.value = value;
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location, Matrix3f.SizeInBytes, value);
#if TODO
					unsigned char *buf = (unsigned char*) mapBuffer(location);
					if (isRowMajor) {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) (buf + r * stride))[c] = value[r * C + c];
							}
						}
					} else {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) (buf + c * stride))[r] = value[r * C + c];
							}
						}
					}
#endif
				} 
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public Matrix3f getMatrix()
			{
				return this.get();
			}

		   /*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void setMatrix(Matrix3f val)
			{
				this.set(val);
			}

			public override void setValue( Value  v)
			{
#if TODO
				set(v.cast< ValueMatrix<U, T, C, R, W> >()->get());
#endif
				throw new NotImplementedException();
			}

			internal protected override void setValue()
			{
#if OPENTK
#if ORK_NO_GLPROGRAMUNIFORM
			Sxta.OpenGL.GL.UniformMatrix3(location, 1, isRowMajor, ref value);
#else
			GL.ProgramUniformMatrix3(program.getId(), location, 1, isRowMajor, (float[])  value);
#endif
#else
#if ORK_NO_GLPROGRAMUNIFORM
			glUniformMatrix2fv(location, 1, true, value);
#else
			glProgramUniformMatrix2fvEXT(program.getId(), location, 1, true, value);
#endif
#endif
			}

			/*
		     * The current value of this uniform.
			 * The matrix coefficients are stored in row major order.
			 */
			Matrix3f value;

			/*
			* The stride between two consecutive rows or columns when this uniform is
			* stored in an uniform block.
			*/
			private int stride;

			/*
			* True if this uniform is stored in row major order in an uniform block.
			*/
			private bool isRowMajor = false;

			private const int R = 3;
			private const int C = 3;
		}
		
		
		public class  UniformMatrix3d : Uniform
		{
			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal UniformMatrix3d(Program program, UniformBlock block, string name, int location, int stride, bool isRowMajor) :
				base("UniformMatrix3d", program, block, name, location)
			{
				this.stride = stride; 
				this.isRowMajor = isRowMajor;
			}

			public override UniformType getType()
			{
				return UniformType.MAT3D;
			}

			public Matrix3d get()
			{
#if TODO
				if (block != NULL && program != NULL) {
					unsigned char *buf = (unsigned char*) mapBuffer(location);
					if (isRowMajor) {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) value)[r * C + c] = ((T*) (buf + r * stride))[c];
							}
						}
					} else {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) value)[r * C + c] = ((T*) (buf + c * stride))[r];
							}
						}
					}
				}
#endif
				return value;
			}
			
			/*
             * Sets the value of this uniform.
             *
             * @param value the matrix coefficients in row major order (i.e. first row,
             *      second row, etc).
             */
            public void set(Matrix3d value)
            {
			   if (block == null || program  == null) {
					this.value = value;
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location, Matrix3d.SizeInBytes, value);
#if TODO
					unsigned char *buf = (unsigned char*) mapBuffer(location);
					if (isRowMajor) {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) (buf + r * stride))[c] = value[r * C + c];
							}
						}
					} else {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) (buf + c * stride))[r] = value[r * C + c];
							}
						}
					}
#endif
				} 
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public Matrix3d getMatrix()
			{
				return this.get();
			}

		   /*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void setMatrix(Matrix3d val)
			{
				this.set(val);
			}

			public override void setValue( Value  v)
			{
#if TODO
				set(v.cast< ValueMatrix<U, T, C, R, W> >()->get());
#endif
				throw new NotImplementedException();
			}

			internal protected override void setValue()
			{
#if OPENTK
#if ORK_NO_GLPROGRAMUNIFORM
			Sxta.OpenGL.GL.UniformMatrix3(location, 1, isRowMajor, ref value);
#else
			GL.ProgramUniformMatrix3(program.getId(), location, 1, isRowMajor, (double[])  value);
#endif
#else
#if ORK_NO_GLPROGRAMUNIFORM
			glUniformMatrix2fv(location, 1, true, value);
#else
			glProgramUniformMatrix2fvEXT(program.getId(), location, 1, true, value);
#endif
#endif
			}

			/*
		     * The current value of this uniform.
			 * The matrix coefficients are stored in row major order.
			 */
			Matrix3d value;

			/*
			* The stride between two consecutive rows or columns when this uniform is
			* stored in an uniform block.
			*/
			private int stride;

			/*
			* True if this uniform is stored in row major order in an uniform block.
			*/
			private bool isRowMajor = false;

			private const int R = 3;
			private const int C = 3;
		}
		
		
		public class  UniformMatrix4f : Uniform
		{
			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal UniformMatrix4f(Program program, UniformBlock block, string name, int location, int stride, bool isRowMajor) :
				base("UniformMatrix4f", program, block, name, location)
			{
				this.stride = stride; 
				this.isRowMajor = isRowMajor;
			}


			public override UniformType getType()
			{
				return UniformType.MAT4F;
			}

			public Matrix4f get()
			{
#if TODO
				if (block != NULL && program != NULL) {
					unsigned char *buf = (unsigned char*) mapBuffer(location);
					if (isRowMajor) {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) value)[r * C + c] = ((T*) (buf + r * stride))[c];
							}
						}
					} else {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) value)[r * C + c] = ((T*) (buf + c * stride))[r];
							}
						}
					}
				}
#endif
				return value;
			}
			
			/*
             * Sets the value of this uniform.
             *
             * @param value the matrix coefficients in row major order (i.e. first row,
             *      second row, etc).
             */
            public void set(Matrix4f value)
            {
			   if (block == null || program  == null) {
					this.value = value;
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location,  Matrix4f.SizeInBytes, value);
#if TODO
					unsigned char *buf = (unsigned char*) mapBuffer(location);
					if (isRowMajor) {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) (buf + r * stride))[c] = value[r * C + c];
							}
						}
					} else {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) (buf + c * stride))[r] = value[r * C + c];
							}
						}
					}
#endif
				} 
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public Matrix4f getMatrix()
			{
				return this.get();
			}

		   /*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void setMatrix(Matrix4f val)
			{
				this.set(val);
			}

			public override void setValue( Value  v)
			{
				throw new NotImplementedException();
			}

			internal protected override void setValue()
			{
#if OPENTK
#if ORK_NO_GLPROGRAMUNIFORM
			Sxta.OpenGL.GL.UniformMatrix4(location, 1, isRowMajor, ref value);
#else
			GL.ProgramUniformMatrix4(program.getId(), location, 1, isRowMajor, (float[])  value);
#endif
#else
#if ORK_NO_GLPROGRAMUNIFORM
			glUniformMatrix4fv(location, 1, true, value);
#else
			glProgramUniformMatrix4fvEXT(program.getId(), location, 1, true, value);
#endif
#endif
			}

			/*
		     * The current value of this uniform.
			 * The matrix coefficients are stored in column major order.
			 */
			Matrix4f value;

			/*
			* The stride between two consecutive rows or columns when this uniform is
			* stored in an uniform block.
			*/
			private int stride;

			/*
			* True if this uniform is stored in row major order in an uniform block.
			*/
			private bool isRowMajor = false;

			private const int R = 4;
			private const int C = 4;
		}
		
		public class  UniformMatrix4d : Uniform
		{
			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal UniformMatrix4d(Program program, UniformBlock block, string name, int location, int stride, bool isRowMajor) :
				base("UniformMatrix4d", program, block, name, location)
			{
				this.stride = stride; 
				this.isRowMajor = isRowMajor;
			}


			public override UniformType getType()
			{
				return UniformType.MAT4D;
			}

			public Matrix4d get()
			{
#if TODO
				if (block != NULL && program != NULL) {
					unsigned char *buf = (unsigned char*) mapBuffer(location);
					if (isRowMajor) {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) value)[r * C + c] = ((T*) (buf + r * stride))[c];
							}
						}
					} else {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) value)[r * C + c] = ((T*) (buf + c * stride))[r];
							}
						}
					}
				}
#endif
				return value;
			}
			
			/*
             * Sets the value of this uniform.
             *
             * @param value the matrix coefficients in row major order (i.e. first row,
             *      second row, etc).
             */
            public void set(Matrix4d value)
            {
			   if (block == null || program  == null) {
					this.value = value;
					if (program != null) {
#if ORK_NO_GLPROGRAMUNIFORM
						    setValueIfCurrent();
#else
						    setValue();
#endif					
					}
				} else {
					block.setValue(location,  Matrix4d.SizeInBytes, value);
#if TODO
					unsigned char *buf = (unsigned char*) mapBuffer(location);
					if (isRowMajor) {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) (buf + r * stride))[c] = value[r * C + c];
							}
						}
					} else {
						for (int r = 0; r < R; ++r) {
							for (int c = 0; c < C; ++c) {
								((T*) (buf + c * stride))[r] = value[r * C + c];
							}
						}
					}
#endif
				} 
			}

			/*
			 * Returns the current value of this uniform.
			 */
			public Matrix4d getMatrix()
			{
				return this.get();
			}

		   /*
			 * Sets the value of this uniform.
			 *
			 * @param value the new value for this uniform.
			 */
			public void setMatrix(Matrix4d val)
			{
				this.set(val);
			}

			public override void setValue( Value  v)
			{
				throw new NotImplementedException();
			}

			internal protected override void setValue()
			{
#if OPENTK
#if ORK_NO_GLPROGRAMUNIFORM
			Sxta.OpenGL.GL.UniformMatrix4(location, 1, isRowMajor, ref value);
#else
			GL.ProgramUniformMatrix4(program.getId(), location, 1, isRowMajor, (double[])  value);
#endif
#else
#if ORK_NO_GLPROGRAMUNIFORM
			glUniformMatrix4fv(location, 1, true, value);
#else
			glProgramUniformMatrix4fvEXT(program.getId(), location, 1, true, value);
#endif
#endif
			}

			/*
		     * The current value of this uniform.
			 * The matrix coefficients are stored in column major order.
			 */
			Matrix4d value;

			/*
			* The stride between two consecutive rows or columns when this uniform is
			* stored in an uniform block.
			*/
			private int stride;

			/*
			* True if this uniform is stored in row major order in an uniform block.
			*/
			private bool isRowMajor = false;

			private const int R = 4;
			private const int C = 4;
		}
		

		public class  UniformMatrix2x3f : Uniform
		{
			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal UniformMatrix2x3f(Program program, UniformBlock block, string name, int location, int stride, int isRowMajor) :
				base("UniformMatrix2x3f", program, block, name, location)
			{
				throw new NotImplementedException();
			}

			public override UniformType getType()
			{
				throw new NotImplementedException();
			}
			
			public override void setValue( Value  v)
			{
				throw new NotImplementedException();
			}

			internal protected override void setValue()
			{
				throw new NotImplementedException();
			}
		}

		public class  UniformMatrix2x4f : Uniform
		{
			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal UniformMatrix2x4f(Program program, UniformBlock block, string name, int location, int stride, int isRowMajor) :
				base("UniformMatrix2x4f", program, block, name, location)
			{
				throw new NotImplementedException();
			}

			public override UniformType getType()
			{
				throw new NotImplementedException();
			}
			
			public override void setValue( Value  v)
			{
				throw new NotImplementedException();
			}

			internal protected override void setValue()
			{
				throw new NotImplementedException();
			}
		}

		public class  UniformMatrix3x2f : Uniform
		{
			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal UniformMatrix3x2f(Program program, UniformBlock block, string name, int location, int stride, int isRowMajor) :
				base("UniformMatrix3x2f", program, block, name, location)
			{
				throw new NotImplementedException();
			}

			public override UniformType getType()
			{
				throw new NotImplementedException();
			}
			
			public override void setValue( Value  v)
			{
				throw new NotImplementedException();
			}

			internal protected override void setValue()
			{
				throw new NotImplementedException();
			}
		}

		public class  UniformMatrix3x4f : Uniform
		{
			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal UniformMatrix3x4f(Program program, UniformBlock block, string name, int location, int stride, int isRowMajor) :
				base("UniformMatrix3x4f", program, block, name, location)
			{
				throw new NotImplementedException();
			}

			public override UniformType getType()
			{
				throw new NotImplementedException();
			}
			
			public override void setValue( Value  v)
			{
				throw new NotImplementedException();
			}

			internal protected override void setValue()
			{
				throw new NotImplementedException();
			}
		}

		public class  UniformMatrix4x2f : Uniform
		{
			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal UniformMatrix4x2f(Program program, UniformBlock block, string name, int location, int stride, int isRowMajor) :
				base("UniformMatrix4x2f", program, block, name, location)
			{
				throw new NotImplementedException();
			}

			public override UniformType getType()
			{
				throw new NotImplementedException();
			}
			
			public override void setValue( Value  v)
			{
				throw new NotImplementedException();
			}

			internal protected override void setValue()
			{
				throw new NotImplementedException();
			}
		}

		public class  UniformMatrix4x3f : Uniform
		{
			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal UniformMatrix4x3f(Program program, UniformBlock block, string name, int location, int stride, int isRowMajor) :
				base("UniformMatrix4x3f", program, block, name, location)
			{
				throw new NotImplementedException();
			}

			public override UniformType getType()
			{
				throw new NotImplementedException();
			}
			
			public override void setValue( Value  v)
			{
				throw new NotImplementedException();
			}

			internal protected override void setValue()
			{
				throw new NotImplementedException();
			}
		}
 		

		public class  UniformMatrix2x3d : Uniform
		{
			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal UniformMatrix2x3d(Program program, UniformBlock block, string name, int location, int stride, int isRowMajor) :
				base("UniformMatrix2x3d", program, block, name, location)
			{
				throw new NotImplementedException();
			}

			public override UniformType getType()
			{
				throw new NotImplementedException();
			}
			
			public override void setValue( Value  v)
			{
				throw new NotImplementedException();
			}

			internal protected override void setValue()
			{
				throw new NotImplementedException();
			}
		}

		public class  UniformMatrix2x4d : Uniform
		{
			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal UniformMatrix2x4d(Program program, UniformBlock block, string name, int location, int stride, int isRowMajor) :
				base("UniformMatrix2x4d", program, block, name, location)
			{
				throw new NotImplementedException();
			}

			public override UniformType getType()
			{
				throw new NotImplementedException();
			}
			
			public override void setValue( Value  v)
			{
				throw new NotImplementedException();
			}

			internal protected override void setValue()
			{
				throw new NotImplementedException();
			}
		}

		public class  UniformMatrix3x2d : Uniform
		{
			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal UniformMatrix3x2d(Program program, UniformBlock block, string name, int location, int stride, int isRowMajor) :
				base("UniformMatrix3x2d", program, block, name, location)
			{
				throw new NotImplementedException();
			}

			public override UniformType getType()
			{
				throw new NotImplementedException();
			}
			
			public override void setValue( Value  v)
			{
				throw new NotImplementedException();
			}

			internal protected override void setValue()
			{
				throw new NotImplementedException();
			}
		}

		public class  UniformMatrix3x4d : Uniform
		{
			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal UniformMatrix3x4d(Program program, UniformBlock block, string name, int location, int stride, int isRowMajor) :
				base("UniformMatrix3x4d", program, block, name, location)
			{
				throw new NotImplementedException();
			}

			public override UniformType getType()
			{
				throw new NotImplementedException();
			}
			
			public override void setValue( Value  v)
			{
				throw new NotImplementedException();
			}

			internal protected override void setValue()
			{
				throw new NotImplementedException();
			}
		}

		public class  UniformMatrix4x2d : Uniform
		{
			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal UniformMatrix4x2d(Program program, UniformBlock block, string name, int location, int stride, int isRowMajor) :
				base("UniformMatrix4x2d", program, block, name, location)
			{
				throw new NotImplementedException();
			}

			public override UniformType getType()
			{
				throw new NotImplementedException();
			}
			
			public override void setValue( Value  v)
			{
				throw new NotImplementedException();
			}

			internal protected override void setValue()
			{
				throw new NotImplementedException();
			}
		}

		public class  UniformMatrix4x3d : Uniform
		{
			/*
			 * Creates a new uniform.
			 *
			 * @param program the Program to which this uniform belongs.
			 * @param block UniformBlock to which this uniform belongs. Maybe null.
			 * @param name the name of the uniform in the GLSL shader code.
			 * @param location the location of this uniform. For an uniform inside a
			 *      block, this location is an offset inside the uniform block buffer.
			 */
			internal UniformMatrix4x3d(Program program, UniformBlock block, string name, int location, int stride, int isRowMajor) :
				base("UniformMatrix4x3d", program, block, name, location)
			{
				throw new NotImplementedException();
			}

			public override UniformType getType()
			{
				throw new NotImplementedException();
			}
			
			public override void setValue( Value  v)
			{
				throw new NotImplementedException();
			}

			internal protected override void setValue()
			{
				throw new NotImplementedException();
			}
		}
 }
