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
    /// Represents a 2D vector using two bools.
    /// </summary>
    /// <remarks>
    /// The Vector2b structure is suitable for interoperation with unmanaged code requiring two consecutive bools.
    /// </remarks>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2b : IEquatable<Vector2b>
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

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="valueC">The valueC that will initialize this instance.</param>
        public Vector2b(bool value)
        {
            X = value;
            Y = value;
        }

        /// <summary>
        /// Constructs a new Vector2b.
        /// </summary>
        /// <param name="x">The x component of the Vector2b.</param>
        /// <param name="y">The y component of the Vector2b.</param>
        public Vector2b(bool x, bool y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Constructs a new Vector2b from the given Vector2b.
        /// </summary>
        /// <param name="v">The Vector3b to copy components from.</param>
        public Vector2b(Vector2b v)
        {
            X = v.X;
            Y = v.Y;
        }

        #endregion

        #region Public Members

        #region Static

        #region Fields

        /// <summary>
        /// Defines a unit-length Vector3b that points towards the X-axis.
        /// </summary>
        public static readonly Vector2b UnitX = new Vector2b(true, false);

        /// <summary>
        /// Defines a unit-length Vector3b that points towards the Y-axis.
        /// </summary>
        public static readonly Vector2b UnitY = new Vector2b(false, true);

        /// <summary>
        /// Defines a zero-length Vector2b.
        /// </summary>
        public static readonly Vector2b Zero = new Vector2b(false, false);

        /// <summary>
        /// Defines an instance with all components set to 1.
        /// </summary>
        public static readonly Vector2b One = new Vector2b(true, true);

        /// <summary>
        /// Defines the size of the Vector3b struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(new Vector2b());

        #endregion

        #endregion

        #region Operators


        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(Vector2b left, Vector2b right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equa lright; false otherwise.</returns>
        public static bool operator !=(Vector2b left, Vector2b right)
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
            return String.Format("({0}, {1})", X, Y);
        }

        #endregion

        #region public override int GetHashCode()

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
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
            if (!(obj is Vector2b))
                return false;

            return this.Equals((Vector2b)obj);
        }

        #endregion

        #endregion

        #endregion

        #region IEquatable<Vector2b> Members

        /// <summary>Indicates whether the current vector is equal to another vector.</summary>
        /// <param name="other">A vector to compare with this vector.</param>
        /// <returns>true if the current vector is equal to the vector parameter; otherwise, false.</returns>
        public bool Equals(Vector2b other)
        {
            return
                X == other.X &&
                Y == other.Y;
        }

        #endregion
    }
}