using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Sxta.Math;

namespace Sxta.Render
{
   /// <summary>
    /// An abstract value.
    /// </summary>
    public abstract class Value
    {
        /*
         * Deletes this value.
         */
        // ~Value();

        /*
         * Returns the type of this value.
         */
        public UniformType getType()
        {
            return type;
        }
        /*
         * Returns the name of this value.
         */
        public string getName()
        {
            return name;
        }


        /*
         * The name of this value.
         */
        protected string name;

        /*
         * Creates an uninitialized value.
         */
        protected Value(string type, string name)
        {
            this.name = name;
        }

        /*
         * The type of this Value.
         */
        protected UniformType type;
    }

    /*
     * A Value holding a Texture value.
     * @ingroup render
     */
    public class ValueSampler : Value
    {
        /*
         * Creates an uninitialized ValueSampler.
         */
        public ValueSampler(UniformType type, string name)
            : base("UniformSampler", name)
        {
        }


        /*
         * Creates a ValueSampler.
         */
        public ValueSampler(UniformType type, string name, Texture value)
            : this(type, name)
        {
            this.value = value;
        }

        /*
         * Deletes this ValueSampler.
         */
        // ~ValueSampler();


        /*
         * Returns the current value of this ValueSampler.
         */
        public Texture get()
        {
            return value;
        }

        /*
         * Sets the value of this uniform.
         *
         * @param value the new value for this ValueSampler.
         */
        public void set(Texture value)
        {
            this.value = value;
        }


        /*
         * The current value of this ValueSampler.
         */
        private Texture value;
    }

   #region Value1f

   /*
     * A Value holding a single float value.
     * @ingroup render
     */
   public class Value1f : Value
   {
        /*
         * Creates an uninitialized Value1f.
         */
        public Value1f(string name)
            : base("GLfloat", name)
        {
			this.type = UniformType.VEC1F;
        }

        /*
         * Creates a Value1f.
         */
        public Value1f(string name, float value)
            : base("GLfloat", name)
        {
            this.value = value;
        }

        /*
         * Returns the current value of this Value1f.
         */
        public float get()
        {
            return value;
        }

        /*
         * Sets the value of this Value1f.
         *
         * @param value the new value for this Value1f.
         */
        public void set(float value)
        {
            this.value = value;
        }

        /*
         * The current value of this Value1f.
         */
        private float value;
    }

	#endregion

    #region Value2f

    /*
     * A Value holding a two vector value.
     * @ingroup render
     */
    public class Value2f : Value
    {
        /*
         * Creates an uninitialized Value2f.
         */
        public Value2f(string name)
            : base("GLfloat", name)
        {
			this.type = UniformType.VEC1F;
		}

        /*
         * Creates a Value2f.
         */
        public Value2f(string name, Vector2f value)
            : base("GLfloat", name)
        {
            this.value = value;
        }

        /*
         * Returns the current value of this Value2f.
         */
        public Vector2f get()
        {
            return value;
        }

        /*
         * Sets the value of this Value2f.
         *
         * @param value the new value for this Value2f.
         */
        public void set(Vector2f value)
        {
            this.value = value;
        }

        /*
         * The current value of this Value2f.
         */
        private Vector2f value;
    }

	#endregion

	#region  Value3f

    /*
     * A Value holding a three vector value.
     * @ingroup render
     */
    public class Value3f : Value
    {
        /*
         * Creates an uninitialized Value3f.
         */
        public Value3f(string name)
            : base("GLfloat", name)
        {
			value = new Vector3f();
			this.type = UniformType.VEC1F;
		}

        /*
         * Creates a Value3f.
         */
        public Value3f(string name, Vector3f value)
            : base("GLfloat", name)
        {
            this.value = value;
        }
 
        /*
         * Returns the current value of this Value3f.
         */
        public Vector3f get()
        {
            return value;
        }

        /*
         * Sets the value of this Value3f.
         *
         * @param value the new value for this Value3f.
         */
        public void set(Vector3f value)
        {
            this.value = value;
        }

        /*
         * The current value of this Value3f.
         */
        private Vector3f value;
    }

	#endregion

	#region  Value4f

    /*
     * A Value holding a for vector value.
     * @ingroup render
     */
    public class Value4f : Value
    {
        /*
         * Creates an uninitialized Value4f.
         */
        public Value4f(string name)
            : base("GLfloat", name)
        {
			value = new Vector4f();
			this.type = UniformType.VEC1F;
		}

        /*
         * Creates a Value4f.
         */
        public Value4f(string name, Vector4f value)
            : base("GLfloat", name)
        {
            this.value = value;
        }

         /*
         * Returns the current value of this Value4f.
         */
        public Vector4f get()
        {
            return value;
        }

        /*
         * Sets the value of this Value4f.
         *
         * @param value the new value for this Value4f.
         */
        public void set(Vector4f value)
        {
            this.value = value;
        }

        /*
         * The current value of this Value4f.
         */
        private Vector4f value;
    }

	#endregion
	

		#region TODO NotImplemented

	public class ValueMatrix2f : Value
	{
		public ValueMatrix2f(string name, Matrix2f value)
            : base("GLfloat", name)
        {
            throw new NotImplementedException();
        }

		public ValueMatrix2f(string name, float[] value)
            : base("GLfloat", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix3f : Value
	{
		public ValueMatrix3f(string name, Matrix3f value)
            : base("GLfloat", name)
        {
            throw new NotImplementedException();
        }

		public ValueMatrix3f(string name, float[] value)
            : base("GLfloat", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix4f : Value
	{
		public ValueMatrix4f(string name, Matrix4f value)
            : base("GLfloat", name)
        {
            throw new NotImplementedException();
        }

		public ValueMatrix4f(string name, float[] value)
            : base("GLfloat", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix2x3f : Value
	{
		public ValueMatrix2x3f(string name, float[] value)
            : base("GLfloat", name)
        {
            throw new NotImplementedException();
        }
	}
	
	public class ValueMatrix2x4f : Value
	{
		public ValueMatrix2x4f(string name, float[] value)
            : base("GLfloat", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix3x2f : Value
	{
		public ValueMatrix3x2f(string name, float[] value)
            : base("GLfloat", name)
        {
            throw new NotImplementedException();
        }
	}
	
	public class ValueMatrix3x4f : Value
	{
		public ValueMatrix3x4f(string name, float[] value)
            : base("GLfloat", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix4x2f : Value
	{
		public ValueMatrix4x2f(string name, float[] value)
            : base("GLfloat", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix4x3f : Value
	{
		public ValueMatrix4x3f(string name, float[] value)
            : base("GLfloat", name)
        {
            throw new NotImplementedException();
        }
	}
	#endregion
	    #region Value1d

   /*
     * A Value holding a single double value.
     * @ingroup render
     */
   public class Value1d : Value
   {
        /*
         * Creates an uninitialized Value1d.
         */
        public Value1d(string name)
            : base("GLdouble", name)
        {
			this.type = UniformType.VEC1D;
        }

        /*
         * Creates a Value1d.
         */
        public Value1d(string name, double value)
            : base("GLdouble", name)
        {
            this.value = value;
        }

        /*
         * Returns the current value of this Value1d.
         */
        public double get()
        {
            return value;
        }

        /*
         * Sets the value of this Value1d.
         *
         * @param value the new value for this Value1d.
         */
        public void set(double value)
        {
            this.value = value;
        }

        /*
         * The current value of this Value1d.
         */
        private double value;
    }

	#endregion

    #region Value2d

    /*
     * A Value holding a two vector value.
     * @ingroup render
     */
    public class Value2d : Value
    {
        /*
         * Creates an uninitialized Value2d.
         */
        public Value2d(string name)
            : base("GLdouble", name)
        {
			this.type = UniformType.VEC1D;
		}

        /*
         * Creates a Value2d.
         */
        public Value2d(string name, Vector2d value)
            : base("GLdouble", name)
        {
            this.value = value;
        }

        /*
         * Returns the current value of this Value2d.
         */
        public Vector2d get()
        {
            return value;
        }

        /*
         * Sets the value of this Value2d.
         *
         * @param value the new value for this Value2d.
         */
        public void set(Vector2d value)
        {
            this.value = value;
        }

        /*
         * The current value of this Value2d.
         */
        private Vector2d value;
    }

	#endregion

	#region  Value3d

    /*
     * A Value holding a three vector value.
     * @ingroup render
     */
    public class Value3d : Value
    {
        /*
         * Creates an uninitialized Value3d.
         */
        public Value3d(string name)
            : base("GLdouble", name)
        {
			value = new Vector3d();
			this.type = UniformType.VEC1D;
		}

        /*
         * Creates a Value3d.
         */
        public Value3d(string name, Vector3d value)
            : base("GLdouble", name)
        {
            this.value = value;
        }
 
        /*
         * Returns the current value of this Value3d.
         */
        public Vector3d get()
        {
            return value;
        }

        /*
         * Sets the value of this Value3d.
         *
         * @param value the new value for this Value3d.
         */
        public void set(Vector3d value)
        {
            this.value = value;
        }

        /*
         * The current value of this Value3d.
         */
        private Vector3d value;
    }

	#endregion

	#region  Value4d

    /*
     * A Value holding a for vector value.
     * @ingroup render
     */
    public class Value4d : Value
    {
        /*
         * Creates an uninitialized Value4d.
         */
        public Value4d(string name)
            : base("GLdouble", name)
        {
			value = new Vector4d();
			this.type = UniformType.VEC1D;
		}

        /*
         * Creates a Value4d.
         */
        public Value4d(string name, Vector4d value)
            : base("GLdouble", name)
        {
            this.value = value;
        }

         /*
         * Returns the current value of this Value4d.
         */
        public Vector4d get()
        {
            return value;
        }

        /*
         * Sets the value of this Value4d.
         *
         * @param value the new value for this Value4d.
         */
        public void set(Vector4d value)
        {
            this.value = value;
        }

        /*
         * The current value of this Value4d.
         */
        private Vector4d value;
    }

	#endregion
	

		#region TODO NotImplemented

	public class ValueMatrix2d : Value
	{
		public ValueMatrix2d(string name, Matrix2d value)
            : base("GLdouble", name)
        {
            throw new NotImplementedException();
        }

		public ValueMatrix2d(string name, double[] value)
            : base("GLdouble", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix3d : Value
	{
		public ValueMatrix3d(string name, Matrix3d value)
            : base("GLdouble", name)
        {
            throw new NotImplementedException();
        }

		public ValueMatrix3d(string name, double[] value)
            : base("GLdouble", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix4d : Value
	{
		public ValueMatrix4d(string name, Matrix4d value)
            : base("GLdouble", name)
        {
            throw new NotImplementedException();
        }

		public ValueMatrix4d(string name, double[] value)
            : base("GLdouble", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix2x3d : Value
	{
		public ValueMatrix2x3d(string name, double[] value)
            : base("GLdouble", name)
        {
            throw new NotImplementedException();
        }
	}
	
	public class ValueMatrix2x4d : Value
	{
		public ValueMatrix2x4d(string name, double[] value)
            : base("GLdouble", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix3x2d : Value
	{
		public ValueMatrix3x2d(string name, double[] value)
            : base("GLdouble", name)
        {
            throw new NotImplementedException();
        }
	}
	
	public class ValueMatrix3x4d : Value
	{
		public ValueMatrix3x4d(string name, double[] value)
            : base("GLdouble", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix4x2d : Value
	{
		public ValueMatrix4x2d(string name, double[] value)
            : base("GLdouble", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix4x3d : Value
	{
		public ValueMatrix4x3d(string name, double[] value)
            : base("GLdouble", name)
        {
            throw new NotImplementedException();
        }
	}
	#endregion
	    #region Value1i

   /*
     * A Value holding a single int value.
     * @ingroup render
     */
   public class Value1i : Value
   {
        /*
         * Creates an uninitialized Value1i.
         */
        public Value1i(string name)
            : base("GLint", name)
        {
			this.type = UniformType.VEC1I;
        }

        /*
         * Creates a Value1i.
         */
        public Value1i(string name, int value)
            : base("GLint", name)
        {
            this.value = value;
        }

        /*
         * Returns the current value of this Value1i.
         */
        public int get()
        {
            return value;
        }

        /*
         * Sets the value of this Value1i.
         *
         * @param value the new value for this Value1i.
         */
        public void set(int value)
        {
            this.value = value;
        }

        /*
         * The current value of this Value1i.
         */
        private int value;
    }

	#endregion

    #region Value2i

    /*
     * A Value holding a two vector value.
     * @ingroup render
     */
    public class Value2i : Value
    {
        /*
         * Creates an uninitialized Value2i.
         */
        public Value2i(string name)
            : base("GLint", name)
        {
			this.type = UniformType.VEC1I;
		}

        /*
         * Creates a Value2i.
         */
        public Value2i(string name, Vector2i value)
            : base("GLint", name)
        {
            this.value = value;
        }

        /*
         * Returns the current value of this Value2i.
         */
        public Vector2i get()
        {
            return value;
        }

        /*
         * Sets the value of this Value2i.
         *
         * @param value the new value for this Value2i.
         */
        public void set(Vector2i value)
        {
            this.value = value;
        }

        /*
         * The current value of this Value2i.
         */
        private Vector2i value;
    }

	#endregion

	#region  Value3i

    /*
     * A Value holding a three vector value.
     * @ingroup render
     */
    public class Value3i : Value
    {
        /*
         * Creates an uninitialized Value3i.
         */
        public Value3i(string name)
            : base("GLint", name)
        {
			value = new Vector3i();
			this.type = UniformType.VEC1I;
		}

        /*
         * Creates a Value3i.
         */
        public Value3i(string name, Vector3i value)
            : base("GLint", name)
        {
            this.value = value;
        }
 
        /*
         * Returns the current value of this Value3i.
         */
        public Vector3i get()
        {
            return value;
        }

        /*
         * Sets the value of this Value3i.
         *
         * @param value the new value for this Value3i.
         */
        public void set(Vector3i value)
        {
            this.value = value;
        }

        /*
         * The current value of this Value3i.
         */
        private Vector3i value;
    }

	#endregion

	#region  Value4i

    /*
     * A Value holding a for vector value.
     * @ingroup render
     */
    public class Value4i : Value
    {
        /*
         * Creates an uninitialized Value4i.
         */
        public Value4i(string name)
            : base("GLint", name)
        {
			value = new Vector4i();
			this.type = UniformType.VEC1I;
		}

        /*
         * Creates a Value4i.
         */
        public Value4i(string name, Vector4i value)
            : base("GLint", name)
        {
            this.value = value;
        }

         /*
         * Returns the current value of this Value4i.
         */
        public Vector4i get()
        {
            return value;
        }

        /*
         * Sets the value of this Value4i.
         *
         * @param value the new value for this Value4i.
         */
        public void set(Vector4i value)
        {
            this.value = value;
        }

        /*
         * The current value of this Value4i.
         */
        private Vector4i value;
    }

	#endregion
	

		#region TODO NotImplemented

	public class ValueMatrix2i : Value
	{
		public ValueMatrix2i(string name, Matrix2i value)
            : base("GLint", name)
        {
            throw new NotImplementedException();
        }

		public ValueMatrix2i(string name, int[] value)
            : base("GLint", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix3i : Value
	{
		public ValueMatrix3i(string name, Matrix3i value)
            : base("GLint", name)
        {
            throw new NotImplementedException();
        }

		public ValueMatrix3i(string name, int[] value)
            : base("GLint", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix4i : Value
	{
		public ValueMatrix4i(string name, Matrix4i value)
            : base("GLint", name)
        {
            throw new NotImplementedException();
        }

		public ValueMatrix4i(string name, int[] value)
            : base("GLint", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix2x3i : Value
	{
		public ValueMatrix2x3i(string name, int[] value)
            : base("GLint", name)
        {
            throw new NotImplementedException();
        }
	}
	
	public class ValueMatrix2x4i : Value
	{
		public ValueMatrix2x4i(string name, int[] value)
            : base("GLint", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix3x2i : Value
	{
		public ValueMatrix3x2i(string name, int[] value)
            : base("GLint", name)
        {
            throw new NotImplementedException();
        }
	}
	
	public class ValueMatrix3x4i : Value
	{
		public ValueMatrix3x4i(string name, int[] value)
            : base("GLint", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix4x2i : Value
	{
		public ValueMatrix4x2i(string name, int[] value)
            : base("GLint", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix4x3i : Value
	{
		public ValueMatrix4x3i(string name, int[] value)
            : base("GLint", name)
        {
            throw new NotImplementedException();
        }
	}
	#endregion
	    #region Value1ui

   /*
     * A Value holding a single uint value.
     * @ingroup render
     */
   public class Value1ui : Value
   {
        /*
         * Creates an uninitialized Value1ui.
         */
        public Value1ui(string name)
            : base("GLuint", name)
        {
			this.type = UniformType.VEC1UI;
        }

        /*
         * Creates a Value1ui.
         */
        public Value1ui(string name, uint value)
            : base("GLuint", name)
        {
            this.value = value;
        }

        /*
         * Returns the current value of this Value1ui.
         */
        public uint get()
        {
            return value;
        }

        /*
         * Sets the value of this Value1ui.
         *
         * @param value the new value for this Value1ui.
         */
        public void set(uint value)
        {
            this.value = value;
        }

        /*
         * The current value of this Value1ui.
         */
        private uint value;
    }

	#endregion

    #region Value2ui

    /*
     * A Value holding a two vector value.
     * @ingroup render
     */
    public class Value2ui : Value
    {
        /*
         * Creates an uninitialized Value2ui.
         */
        public Value2ui(string name)
            : base("GLuint", name)
        {
			this.type = UniformType.VEC1UI;
		}

        /*
         * Creates a Value2ui.
         */
        public Value2ui(string name, Vector2ui value)
            : base("GLuint", name)
        {
            this.value = value;
        }

        /*
         * Returns the current value of this Value2ui.
         */
        public Vector2ui get()
        {
            return value;
        }

        /*
         * Sets the value of this Value2ui.
         *
         * @param value the new value for this Value2ui.
         */
        public void set(Vector2ui value)
        {
            this.value = value;
        }

        /*
         * The current value of this Value2ui.
         */
        private Vector2ui value;
    }

	#endregion

	#region  Value3ui

    /*
     * A Value holding a three vector value.
     * @ingroup render
     */
    public class Value3ui : Value
    {
        /*
         * Creates an uninitialized Value3ui.
         */
        public Value3ui(string name)
            : base("GLuint", name)
        {
			value = new Vector3ui();
			this.type = UniformType.VEC1UI;
		}

        /*
         * Creates a Value3ui.
         */
        public Value3ui(string name, Vector3ui value)
            : base("GLuint", name)
        {
            this.value = value;
        }
 
        /*
         * Returns the current value of this Value3ui.
         */
        public Vector3ui get()
        {
            return value;
        }

        /*
         * Sets the value of this Value3ui.
         *
         * @param value the new value for this Value3ui.
         */
        public void set(Vector3ui value)
        {
            this.value = value;
        }

        /*
         * The current value of this Value3ui.
         */
        private Vector3ui value;
    }

	#endregion

	#region  Value4ui

    /*
     * A Value holding a for vector value.
     * @ingroup render
     */
    public class Value4ui : Value
    {
        /*
         * Creates an uninitialized Value4ui.
         */
        public Value4ui(string name)
            : base("GLuint", name)
        {
			value = new Vector4ui();
			this.type = UniformType.VEC1UI;
		}

        /*
         * Creates a Value4ui.
         */
        public Value4ui(string name, Vector4ui value)
            : base("GLuint", name)
        {
            this.value = value;
        }

         /*
         * Returns the current value of this Value4ui.
         */
        public Vector4ui get()
        {
            return value;
        }

        /*
         * Sets the value of this Value4ui.
         *
         * @param value the new value for this Value4ui.
         */
        public void set(Vector4ui value)
        {
            this.value = value;
        }

        /*
         * The current value of this Value4ui.
         */
        private Vector4ui value;
    }

	#endregion
	

		#region TODO NotImplemented

	public class ValueMatrix2ui : Value
	{
		public ValueMatrix2ui(string name, Matrix2ui value)
            : base("GLuint", name)
        {
            throw new NotImplementedException();
        }

		public ValueMatrix2ui(string name, uint[] value)
            : base("GLuint", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix3ui : Value
	{
		public ValueMatrix3ui(string name, Matrix3ui value)
            : base("GLuint", name)
        {
            throw new NotImplementedException();
        }

		public ValueMatrix3ui(string name, uint[] value)
            : base("GLuint", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix4ui : Value
	{
		public ValueMatrix4ui(string name, Matrix4ui value)
            : base("GLuint", name)
        {
            throw new NotImplementedException();
        }

		public ValueMatrix4ui(string name, uint[] value)
            : base("GLuint", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix2x3ui : Value
	{
		public ValueMatrix2x3ui(string name, uint[] value)
            : base("GLuint", name)
        {
            throw new NotImplementedException();
        }
	}
	
	public class ValueMatrix2x4ui : Value
	{
		public ValueMatrix2x4ui(string name, uint[] value)
            : base("GLuint", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix3x2ui : Value
	{
		public ValueMatrix3x2ui(string name, uint[] value)
            : base("GLuint", name)
        {
            throw new NotImplementedException();
        }
	}
	
	public class ValueMatrix3x4ui : Value
	{
		public ValueMatrix3x4ui(string name, uint[] value)
            : base("GLuint", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix4x2ui : Value
	{
		public ValueMatrix4x2ui(string name, uint[] value)
            : base("GLuint", name)
        {
            throw new NotImplementedException();
        }
	}

	public class ValueMatrix4x3ui : Value
	{
		public ValueMatrix4x3ui(string name, uint[] value)
            : base("GLuint", name)
        {
            throw new NotImplementedException();
        }
	}
	#endregion
	    #region Value1b

   /*
     * A Value holding a single bool value.
     * @ingroup render
     */
   public class Value1b : Value
   {
        /*
         * Creates an uninitialized Value1b.
         */
        public Value1b(string name)
            : base("GLbool", name)
        {
			this.type = UniformType.VEC1B;
        }

        /*
         * Creates a Value1b.
         */
        public Value1b(string name, bool value)
            : base("GLbool", name)
        {
            this.value = value;
        }

        /*
         * Returns the current value of this Value1b.
         */
        public bool get()
        {
            return value;
        }

        /*
         * Sets the value of this Value1b.
         *
         * @param value the new value for this Value1b.
         */
        public void set(bool value)
        {
            this.value = value;
        }

        /*
         * The current value of this Value1b.
         */
        private bool value;
    }

	#endregion

    #region Value2b

    /*
     * A Value holding a two vector value.
     * @ingroup render
     */
    public class Value2b : Value
    {
        /*
         * Creates an uninitialized Value2b.
         */
        public Value2b(string name)
            : base("GLbool", name)
        {
			this.type = UniformType.VEC1B;
		}

        /*
         * Creates a Value2b.
         */
        public Value2b(string name, Vector2b value)
            : base("GLbool", name)
        {
            this.value = value;
        }

        /*
         * Returns the current value of this Value2b.
         */
        public Vector2b get()
        {
            return value;
        }

        /*
         * Sets the value of this Value2b.
         *
         * @param value the new value for this Value2b.
         */
        public void set(Vector2b value)
        {
            this.value = value;
        }

        /*
         * The current value of this Value2b.
         */
        private Vector2b value;
    }

	#endregion

	#region  Value3b

    /*
     * A Value holding a three vector value.
     * @ingroup render
     */
    public class Value3b : Value
    {
        /*
         * Creates an uninitialized Value3b.
         */
        public Value3b(string name)
            : base("GLbool", name)
        {
			value = new Vector3b();
			this.type = UniformType.VEC1B;
		}

        /*
         * Creates a Value3b.
         */
        public Value3b(string name, Vector3b value)
            : base("GLbool", name)
        {
            this.value = value;
        }
 
        /*
         * Returns the current value of this Value3b.
         */
        public Vector3b get()
        {
            return value;
        }

        /*
         * Sets the value of this Value3b.
         *
         * @param value the new value for this Value3b.
         */
        public void set(Vector3b value)
        {
            this.value = value;
        }

        /*
         * The current value of this Value3b.
         */
        private Vector3b value;
    }

	#endregion

	#region  Value4b

    /*
     * A Value holding a for vector value.
     * @ingroup render
     */
    public class Value4b : Value
    {
        /*
         * Creates an uninitialized Value4b.
         */
        public Value4b(string name)
            : base("GLbool", name)
        {
			value = new Vector4b();
			this.type = UniformType.VEC1B;
		}

        /*
         * Creates a Value4b.
         */
        public Value4b(string name, Vector4b value)
            : base("GLbool", name)
        {
            this.value = value;
        }

         /*
         * Returns the current value of this Value4b.
         */
        public Vector4b get()
        {
            return value;
        }

        /*
         * Sets the value of this Value4b.
         *
         * @param value the new value for this Value4b.
         */
        public void set(Vector4b value)
        {
            this.value = value;
        }

        /*
         * The current value of this Value4b.
         */
        private Vector4b value;
    }

	#endregion
	

	 }
