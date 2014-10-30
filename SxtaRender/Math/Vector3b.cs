#region --- License ---
/*
Copyright (c) 2006 - 2008 The Open Toolkit library.

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */
#endregion

using System;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
namespace Sxta.Math
{
    /// <summary>
    /// Represents a 3D vector using three bools.
    /// </summary>
    /// <remarks>
    /// The Vector3b structure is suitable for interoperation with unmanaged code requiring three consecutive bools.
    /// </remarks>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3b  : IEquatable<Vector3b>
    {
        #region Fields

        /// <summary>
        /// The X component of the Vector3b.
        /// </summary>
        public bool X;

        /// <summary>
        /// The Y component of the Vector3b.
        /// </summary>
        public bool Y;

        /// <summary>
        /// The Z component of the Vector3b.
        /// </summary>
        public bool Z;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="valueC">The valueC that will initialize this instance.</param>
        public Vector3b(bool value)
        {
            X = value;
            Y = value;
            Z = value;
        }

        /// <summary>
        /// Constructs a new Vector3b.
        /// </summary>
        /// <param name="x">The x component of the Vector3b.</param>
        /// <param name="y">The y component of the Vector3b.</param>
        /// <param name="z">The z component of the Vector3b.</param>
        public Vector3b(bool x, bool y, bool z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Constructs a new Vector3b from the given Vector3b.
        /// </summary>
        /// <param name="v">The Vector3b to copy components from.</param>
        public Vector3b(Vector3b v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        #endregion

        #region Public Members

        #region Static

        #region Fields

        /// <summary>
        /// Defines a unit-length Vector3b that points towards the X-axis.
        /// </summary>
        public static readonly Vector3b UnitX = new Vector3b(true, false, false);

        /// <summary>
        /// Defines a unit-length Vector3b that points towards the Y-axis.
        /// </summary>
        public static readonly Vector3b UnitY = new Vector3b(false, true, false);

        /// <summary>
        /// /// Defines a unit-length Vector3b that points towards the Z-axis.
        /// </summary>
        public static readonly Vector3b UnitZ = new Vector3b(false, false, true);

        /// <summary>
        /// Defines a zero-length Vector3b.
        /// </summary>
        public static readonly Vector3b Zero = new Vector3b(false, false, false);

        /// <summary>
        /// Defines an instance with all components set to 1.
        /// </summary>
        public static readonly Vector3b One = new Vector3b(true, true, true);

        /// <summary>
        /// Defines the size of the Vector3b struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(new Vector3b());

        #endregion

   

      
        #endregion

        #region Operators

        
        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(Vector3b left, Vector3b right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equa lright; false otherwise.</returns>
        public static bool operator !=(Vector3b left, Vector3b right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current Vector3b.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("({0}, {1}, {2})", X, Y, Z);
        }

        #endregion

        #region public override int GetHashCode()

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        #endregion

        #region public override bool Equals(object obj)

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>True if the instances are equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Vector3b))
                return false;

            return this.Equals((Vector3b)obj);
        }

        #endregion

        #endregion

        #endregion

        #region IEquatable<Vector3b> Members

        /// <summary>Indicates whether the current vector is equal to another vector.</summary>
        /// <param name="other">A vector to compare with this vector.</param>
        /// <returns>true if the current vector is equal to the vector parameter; otherwise, false.</returns>
        public bool Equals(Vector3b other)
        {
            return
                X == other.X &&
                Y == other.Y &&
                Z == other.Z;
        }

        #endregion
    }
}
