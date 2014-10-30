using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Sxta.Math
{
    /// <summary>
    /// Represents a 4D vector using four bools.
    /// </summary>
    /// <remarks>
    /// The Vector4b structure is suitable for interoperation with unmanaged code requiring four bools.
    /// </remarks>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4b : IEquatable<Vector4b>
    {
        #region Fields

        /// <summary>
        /// The X component of the Vector4b.
        /// </summary>
        public bool X;

        /// <summary>
        /// The Y component of the Vector4b.
        /// </summary>
        public bool Y;

        /// <summary>
        /// The Z component of the Vector4b.
        /// </summary>
        public bool Z;

        /// <summary>
        /// The W component of the Vector4b.
        /// </summary>
        public bool W;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="valueC">The valueC that will initialize this instance.</param>
        public Vector4b(bool value)
        {
            X = value;
            Y = value;
            Z = value;
            W = value;
        }

        /// <summary>
        /// Constructs a new Vector4b.
        /// </summary>
        /// <param name="x">The x component of the Vector4b.</param>
        /// <param name="y">The y component of the Vector4b.</param>
        /// <param name="z">The z component of the Vector4b.</param>
        /// <param name="w">The w component of the Vector4b.</param>
        public Vector4b(bool x, bool y, bool z, bool w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Constructs a new Vector4b from the given Vector4b.
        /// </summary>
        /// <param name="v">The Vector4b to copy components from.</param>
        public Vector4b(Vector4b v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = v.W;
        }

        #endregion

        #region Public Members

        #region Static

        #region Fields

        /// <summary>
        /// Defines a unit-length Vector4b that points towards the X-axis.
        /// </summary>
        public static readonly Vector4b UnitX = new Vector4b(true, false, false, false);

        /// <summary>
        /// Defines a unit-length Vector4b that points towards the Y-axis.
        /// </summary>
        public static readonly Vector4b UnitY = new Vector4b(false, true, false, false);

        /// <summary>
        /// /// Defines a unit-length Vector4b that points towards the Z-axis.
        /// </summary>
        public static readonly Vector4b UnitZ = new Vector4b(false, false, true, false);

        /// <summary>
        /// Defines a zero-length Vector4b.
        /// </summary>
        public static readonly Vector4b Zero = new Vector4b(false, false, false, false);

        /// <summary>
        /// Defines an instance with all components set to 1.
        /// </summary>
        public static readonly Vector4b One = new Vector4b(true, true, true, true);

        /// <summary>
        /// Defines the size of the Vector4b struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(new Vector4b());

        #endregion

        #endregion

        #region Operators


        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(Vector4b left, Vector4b right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equa lright; false otherwise.</returns>
        public static bool operator !=(Vector4b left, Vector4b right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current Vector4b.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("({0}, {1}, {2}, {3})", X, Y, Z, W);
        }

        #endregion

        #region public override int GetHashCode()

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
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
            if (!(obj is Vector4b))
                return false;

            return this.Equals((Vector4b)obj);
        }

        #endregion

        #endregion

        #endregion

        #region IEquatable<Vector4b> Members

        /// <summary>Indicates whether the current vector is equal to another vector.</summary>
        /// <param name="other">A vector to compare with this vector.</param>
        /// <returns>true if the current vector is equal to the vector parameter; otherwise, false.</returns>
        public bool Equals(Vector4b other)
        {
            return
                X == other.X &&
                Y == other.Y &&
                Z == other.Z &&
                W == other.W;
        }

        #endregion
    }
}
