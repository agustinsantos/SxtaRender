using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Sxta.Render
{

	/// <summary>
    /// A vertex attribute buffer. Such a buffer contains the values of one
    /// vertex attribute for a list of vertices. A vertex attribute is a vector
    /// of one or more components of the same type. Examples of vertex attributes
    /// are its position, normal, uv coordinates, color, etc. So, for example, a
    /// position attribute buffer contains the positions of a list of vertices, a
    /// color attribute buffer contains the colors of a list of vertices, etc.
    /// An AttributeBuffer describes how these values are organized, based on an
    /// offset and a stride parameters. The values themselves are stored in a
    /// Buffer object. Note that several AttributeBuffer can share the same Buffer
    /// object. So several vertex attributes can be stored in a single buffer. For
    /// instance all positions, followed by all normals, followed by all colors,
    /// etc. Or the position, normal and color of the first vertex, followed by the
    /// position, normal and color of the second vertex, and so on.
	/// </summary>
    public class AttributeBuffer
    {

        
		/// <summary>
		/// Initializes a new instance of the <see cref="Sxta.Render.AttributeBuffer"/> class.
		/// Creates a new attribute buffer for floating point attributes.
        /// The attribute is supposed to be declared as floating point in
        /// the vertex shader, but its valueC can be defined from integer
        /// values (which can then be normalized to 0..1 or not when converted
        /// to floating point).
		/// </summary>
		/// <param name='index'>
		/// Index.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		/// <param name='t'>
		/// T.
		/// </param>
		/// <param name='norm'>
		/// Norm.
		/// </param>
		/// <param name='b'>
		/// B.
		/// </param>
		/// <param name='stride'>
		/// Stride.
		/// </param>
		/// <param name='offset'>
		/// Offset.
		/// </param>
		/// <param name='divisor'>
		/// Divisor.
		/// </param>
		public AttributeBuffer(int index, int size, AttributeType t, bool norm, Buffer b, int stride = 0, int offset = 0, int divisor = 0)
        {
            this.index = index;
            this.size = size;
            this.type = t;
            this.I = false;
            this.L = false;
            this.norm = norm;
            this.b = b;
            this.stride = stride;
            this.offset = offset;
            this.divisor = 0;
        }

  
		/// <summary>
		/// Initializes a new instance of the <see cref="Sxta.Render.AttributeBuffer"/> class.
		/// Creates a new attribute buffer for signed or unsigned integer attributes.
        /// The attribute is supposed to be declared as signed or unsigned integer
        /// type in the vertex shader, and its valueC must be defined from integer
        /// values.
		/// </summary>
		/// <param name='index'>
		/// Index a vertex attribute index.
		/// </param>
		/// <param name='size'>
		/// Size the number of components in attributes of this kind.
		/// </param>
		/// <param name='t'>
		/// T. the type of each component in attributes of this kind. Must be
        /// a signed or unsigned integer type.
		/// </param>
		/// <param name='b'>
		/// B the buffer containing the actual attribute values.
		/// </param>
		/// <param name='stride'>
		/// Stride the offset between two consecutive attribute values in b.
		/// </param>
		/// <param name='offset'>
		/// Offset the offset of the first attribute valueC in b.
		/// </param>
		/// <param name='divisor'>
		/// Divisor ow many times each attribute much be instanced, or 0
        /// to disable attribute instancing.
		/// </param>
        public AttributeBuffer(int index, int size, AttributeType t, Buffer b, int stride = 0, int offset = 0, int divisor = 0)
        {
            this.index = index;
            this.size = size;
            this.type = t;
            this.I = true;
            this.L = false;
            this.norm = false;
            this.b = b;
            this.stride = stride;
            this.offset = offset;
            this.divisor = 0;
        }

		/// <summary>
		/// Creates a new attribute buffer for double precision attributes.
        /// The attribute is supposed to be declared as double precision floating point
        /// type in the vertex shader, and its valueC must be defined from double
        /// values.
		/// Initializes a new instance of the <see cref="Sxta.Render.AttributeBuffer"/> class.
		/// </summary>
		/// <param name='index'>
		/// Index  a vertex attribute index.
		/// </param>
		/// <param name='size'>
		/// Size the number of components in attributes of this kind.
		/// </param>
		/// <param name='b'>
		/// B the buffer containing the actual attribute values.
		/// </param>
		/// <param name='stride'>
		/// Stride the offset between two consecutive attribute values in b.
		/// </param>
		/// <param name='offset'>
		/// Offset of the first attribute valueC in b.
		/// </param>
		/// <param name='divisor'>
		/// Divisorhow many times each attribute much be instanced, or 0
        /// to disable attribute instancing.
		/// </param>
        public AttributeBuffer(int index, int size, Buffer b, int stride = 0, int offset = 0, int divisor = 0)
        {
            this.index = index;
            this.size = size;
            this.type = AttributeType.A64F;
            this.I = true;
            this.L = true;
            this.norm = false;
            this.b = b;
            this.stride = stride;
            this.offset = offset;
            this.divisor = 0;
        }

        /*
         * 
         */
        //public virtual ~AttributeBuffer() {}

  
		/// <summary>
		/// Gets the size.
		/// Returns the number of components in attributes of this kind.
		/// </summary>
		/// <returns>
		/// The size.
		/// </returns>
        public int getSize()
        {
            return size;
        }


		/// <summary>
		/// Returns the type of each component in attributes of this kind.
		/// </summary>
		/// <returns>
		/// The type.
		/// </returns>
        public AttributeType getType()
        {
            return type;
        }

     

		/// <summary>
		/// Returns the size of one attribute valueC. This size is the number of
        /// components per attribute, times the size of each component (which
        /// depends on its type: byte, int, float, etc). Except for the packed
        /// formats A32I_2_10_10_10_REV and A32UI_2_10_10_10_REV.
		/// </summary>
		/// <returns>
		/// The attribute size.
		/// </returns>
        public int getAttributeSize()
        {
            int size = 0;
            switch (type)
            {
                case AttributeType.A8I:
                case AttributeType.A8UI:
                    size = 1;
                    break;
                case AttributeType.A16I:
                case AttributeType.A16UI:
                case AttributeType.A16F:
                    size = 2;
                    break;
                case AttributeType.A32I:
                case AttributeType.A32UI:
                case AttributeType.A32F:
                    size = 4;
                    break;
                case AttributeType.A64F:
                    size = 8;
                    break;
                case AttributeType.A32I_2_10_10_10_REV:
                case AttributeType.A32UI_2_10_10_10_REV:
                    return 4;
            }
            return this.size * size;
        }


		/// <summary>
		/// Gets the buffer.
		/// Returns the buffer that contains the actual data of this attribute
        /// buffer.
		/// </summary>
		/// <returns>
		/// The buffer.
		/// </returns>
        public Buffer getBuffer()
        {
            return b;
        }


		/// <summary>
		/// Sets the buffer.
		/// Sets the buffer that contains the actual data of this attribute
        /// buffer.
		/// </summary>
		/// <returns>
		/// The buffer.
		/// </returns>
		/// <param name='b'>
		/// B.
		/// </param>
        public void setBuffer(Buffer b)
        {
            this.b = b;
        }

        
		/// <summary>
		/// Returns the offset between two consecutive attribute values in this
        /// attribute buffer.
		/// Gets the stride.
		/// </summary>
		/// <returns>
		/// The stride.
		/// </returns>
        public int getStride()
        {
            return stride;
        }

		/// <summary>
		/// Gets the offset.
		/// Returns the offset of the first attribute valueC in this attribute
        /// buffer.
		/// </summary>
		/// <returns>
		/// The offset.
		/// </returns>
        public int getOffset()
        {
            return offset;
        }

		/// <summary>
		/// Returns the number of times each attribute much be instanced, or 0
        /// if attribute instancing is disabled for this attribute.
		/// </summary>
		/// <returns>
		/// The divisor.
		/// </returns>
        public int getDivisor() { throw new NotSupportedException("This methods is not implemented in the C++ version"); }


		/// <summary>
		/// A vertex attribute index.
		/// </summary>
        internal int index;

		/// <summary>
		/// The number of components in attributes of this kind.
		/// </summary>
        internal int size;

		/// <summary>
		/// The type of each component in attributes of this kind.
		/// </summary>
        internal AttributeType type;

		/// <summary>
		/// True if the attribute is declared with an integer type in shaders.
		/// </summary>
        internal bool I;

       
		/// <summary>
		/// True if the attribute is declared with a double floating point precision type in shaders.
		/// </summary>
        internal bool L;

		/// <summary>
		/// True if the attribute components must be normalized to 0..1.
		/// </summary>
        internal bool norm;

		/// <summary>
		/// The buffer that contains the actual vertex attribute values.
		/// </summary>
        internal Buffer b;

		/// <summary>
		/// The offset between two consecutive attribute values in #b.
		/// </summary>
        internal int stride;


		/// <summary>
		/// The offset of the first attribute valueC in #b.
		/// </summary>
        internal int offset;

		/// <summary>
		/// How many times each attribute much be instanced, or 0 to disable
        /// attribute instancing.
		/// </summary>
        internal int divisor;
    }
}
