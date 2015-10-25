#region --- License ---
/*
Copyright (c) 2008 - 2016 The Sxta Render library.

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
    ///  A quaternion class. It can be used to represent an orientation in 3D space.
    /// Good introductions to Quaternions at:
    /// http://www.gamasutra.com/features/programming/19980703/quaternions_01.htm
    /// http://mathworld.wolfram.com/Quaternion.html
    /// 
    /// Note:
    /// Quaternion is a struct and the default value is (0, 0, 0, 0) instead of (0, 0, 0, 1);
    /// Structs cannot contain explicit parameterless constructors. Struct members are automatically initialized to their default values.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Quaternion : IEquatable<Quaternion>
    {
        #region Fields

        private Vector3f xyz;
        private float w;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new Quaternion from another quaternion. Copy method 
        /// </summary>
        /// <param name="q">The quaternion to be copied</param>
        public Quaternion(Quaternion q)
          : this(new Vector3f(q.X, q.Y, q.Z), q.W)
        {
        }

        /// <summary>
        /// Construct a new Quaternion from vector and w components
        /// This constructor just only copy the elements.
        /// </summary>
        /// <param name="v">The vector part</param>
        /// <param name="w">The w part</param>
        public Quaternion(Vector3f v, float w)
        {
            this.xyz = v;
            this.w = w;
        }

        /// <summary>
        /// Construct a new Quaternion from vector and w components
        /// </summary>
        /// <param name="v">The vector part</param>
        /// <param name="w">The w part</param>
        public Quaternion(Vector4f v)
           : this(v.X, v.Y, v.Z, v.W)
        {
        }

        /// <summary>
        /// Construct a new Quaternion
        /// </summary>
        /// <param name="x">The x component</param>
        /// <param name="y">The y component</param>
        /// <param name="z">The z component</param>
        /// <param name="w">The w component</param>
        public Quaternion(float x, float y, float z, float w)
            : this(new Vector3f(x, y, z), w)
        { }

        /// <summary>
        /// Build a quaternion from the given axis and angle
        /// </summary>
        /// <param name="axis">The axis to rotate about</param>
        /// <param name="angle">The rotation angle in radians</param>
        /// <returns></returns>
        public Quaternion(double angle, Vector3f axis)
        {
            this = Identity;
            MakeRotate(axis, angle);
        }

        /// <summary>
        /// Build a quaternion from the given axis and angle
        /// </summary>
        /// <param name="axis">The axis to rotate about</param>
        /// <param name="angle">The rotation angle in radians</param>
        /// <returns></returns>
        public Quaternion(double angle, Vector3d axis)
            : this(angle, (Vector3f)axis)
        {
        }

        #endregion

        #region Public Members

        #region Properties

        /// <summary>
        /// Gets or sets a Vector3f with the X, Y and Z components of this instance.
        /// </summary>
        public Vector3f Xyz { get { return xyz; } set { xyz = value; } }

        /// <summary>
        /// Gets or sets the X component of this instance.
        /// </summary>
        [XmlIgnore]
        public float X { get { return xyz.X; } set { xyz.X = value; } }

        /// <summary>
        /// Gets or sets the Y component of this instance.
        /// </summary>
        [XmlIgnore]
        public float Y { get { return xyz.Y; } set { xyz.Y = value; } }

        /// <summary>
        /// Gets or sets the Z component of this instance.
        /// </summary>
        [XmlIgnore]
        public float Z { get { return xyz.Z; } set { xyz.Z = value; } }

        /// <summary>
        /// Gets or sets the W component of this instance.
        /// </summary>
        public float W { get { return w; } set { w = value; } }

        #endregion

        #region Instance

        #region Set
        public void Set(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public void Set(Vector4f v)
        {
            this.X = v.X;
            this.Y = v.Y;
            this.Z = v.Z;
            this.W = v.W;
        }

        public void Set(Vector4d v)
        {
            this.X = (float)v.X;
            this.Y = (float)v.Y;
            this.Z = (float)v.Z;
            this.W = (float)v.W;
        }
        #endregion

        #region ToAxisAngle

        /// <summary>
        /// Convert the current quaternion to axis angle representation
        /// </summary>
        /// <param name="axis">The resultant axis</param>
        /// <param name="angle">The resultant angle</param>
        public void ToAxisAngle(out Vector3f axis, out float angle)
        {
            Vector4f result = ToAxisAngle();
            axis = result.Xyz;
            angle = result.W;
        }

        /// <summary>
        /// Convert this instance to an axis-angle representation.
        /// Return the angle and vector components represented by the quaternion.
        /// </summary>
        /// <returns>A Vector4 that is the axis-angle representation of this quaternion.</returns>
        public Vector4f ToAxisAngle()
        {
            Quaternion q = this;
            if (System.Math.Abs(q.W) > 1.0f)
                q.Normalize();

            Vector4f result = new Vector4f();

            result.W = 2.0f * (float)System.Math.Acos(q.W); // angle
            float den = (float)System.Math.Sqrt(1.0 - q.W * q.W);
            if (den > 0.0001f)
            {
                result.Xyz = q.Xyz / den;
            }
            else
            {
                // This occurs when the angle is zero. 
                // Not a problem: just set an arbitrary normalized axis.
                result.Xyz = Vector3f.UnitX;
            }

            return result;
        }

        #endregion

        #region public float Length

        /// <summary>
        /// Gets the length (magnitude) of the quaternion.
        /// </summary>
        /// <seealso cref="LengthSquared"/>
        public double Length
        {
            get
            {
                return System.Math.Sqrt(W * W + Xyz.LengthSquared);
            }
        }

        #endregion

        #region public float LengthSquared

        /// <summary>
        /// Gets the square of the quaternion length (magnitude).
        /// </summary>
        public double LengthSquared
        {
            get
            {
                return (W * W + Xyz.LengthSquared);
            }
        }

        #endregion

        #region public void Normalize()

        /// <summary>
        /// Scales the Quaternion to unit length.
        /// </summary>
        public void Normalize()
        {
            float scale = (float)(1.0 / this.Length);
            Xyz *= scale;
            W *= scale;
        }

        #endregion

        #region public void Conjugate()

        /// <summary>
        /// Convert this quaternion to its conjugate
        /// </summary>
        public void Conjugate()
        {
            Xyz = -Xyz;
        }

        #endregion

        #region ZeroRotation
        /// <summary>
        /// return true if the Quaternion represents a zero rotation,
        /// and therefore can be ignored in computations.
        /// </summary>
        public bool IsZeroRotation
        {
            get { return this.X == 0.0 && this.Y == 0.0 && this.Z == 0.0 && this.W == 1.0; }
        }

        #endregion

        #endregion

        #region Static

        #region Fields

        /// <summary>
        /// Defines the identity quaternion.
        /// </summary>
        public static Quaternion Identity = new Quaternion(0, 0, 0, 1);

        #endregion

        #region Add
        /// <summary>
        /// Add two quaternions
        /// </summary>
        /// <param name="right">The second operand</param>
        /// <returns>The result of the addition</returns>
        public Quaternion Add(Quaternion right)
        {
            this.Xyz += right.Xyz;
            this.W += right.W;

            return this;
        }

        /// <summary>
        /// Add two quaternions
        /// </summary>
        /// <param name="left">The first operand</param>
        /// <param name="right">The second operand</param>
        /// <returns>The result of the addition</returns>
        public static Quaternion Add(Quaternion left, Quaternion right)
        {
            return new Quaternion(left.Xyz + right.Xyz, left.W + right.W);
        }

        /// <summary>
        /// Add two quaternions
        /// </summary>
        /// <param name="left">The first operand</param>
        /// <param name="right">The second operand</param>
        /// <param name="result">The result of the addition</param>
        public static void Add(ref Quaternion left, ref Quaternion right, out Quaternion result)
        {
            result = new Quaternion(left.Xyz + right.Xyz, left.W + right.W);
        }

        #endregion

        #region Sub

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <returns>The result of the operation.</returns>
        public Quaternion Sub(Quaternion right)
        {
            this.Xyz -= right.Xyz;
            this.W -= right.W;

            return this;
        }


        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <returns>The result of the operation.</returns>
        public static Quaternion Sub(Quaternion left, Quaternion right)
        {
            return new Quaternion(left.Xyz - right.Xyz, left.W - right.W);
        }

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <param name="result">The result of the operation.</param>
        public static void Sub(ref Quaternion left, ref Quaternion right, out Quaternion result)
        {
            result = new Quaternion(left.Xyz - right.Xyz, left.W - right.W);
        }

        #endregion

        #region Mult
        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public Quaternion Multiply(Quaternion right)
        {
            this.Xyz = right.W * this.Xyz + this.W * right.Xyz + Vector3f.Cross(this.Xyz, right.Xyz);
            this.W = this.W * right.W - Vector3f.Dot(this.Xyz, right.Xyz);

            return this;
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaternion Multiply(Quaternion left, Quaternion right)
        {
            Quaternion result;
            Multiply(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <param name="result">A new instance containing the result of the calculation.</param>
        public static void Multiply(ref Quaternion left, ref Quaternion right, out Quaternion result)
        {
            result = new Quaternion(
                right.W * left.Xyz + left.W * right.Xyz + Vector3f.Cross(left.Xyz, right.Xyz),
                left.W * right.W - Vector3f.Dot(left.Xyz, right.Xyz));
        }


        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <param name="result">A new instance containing the result of the calculation.</param>
        public static void Multiply(ref Quaternion quaternion, float scale, out Quaternion result)
        {
            result = new Quaternion(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaternion Multiply(Quaternion quaternion, float scale)
        {
            return new Quaternion(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public Quaternion Multiply(float scale)
        {
            this.Xyz *= scale;
            this.W *= scale;
            return this;
        }
        #endregion

        #region Conjugate

        /// <summary>
        /// Get the conjugate of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion</param>
        /// <returns>The conjugate of the given quaternion</returns>
        public static Quaternion Conjugate(Quaternion q)
        {
            return new Quaternion(-q.Xyz, q.W);
        }

        /// <summary>
        /// Get the conjugate of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion</param>
        /// <param name="result">The conjugate of the given quaternion</param>
        public static void Conjugate(ref Quaternion q, out Quaternion result)
        {
            result = new Quaternion(-q.Xyz, q.W);
        }

        #endregion

        #region Invert
        /// <summary>
        /// Get the inverse of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion to invert</param>
        /// <returns>The inverse of the given quaternion</returns>
        public Quaternion Invert()
        {
            float lengthSq = (float)this.LengthSquared;
            if (lengthSq != 0.0)
            {
                float i = 1.0f / lengthSq;
                this.Xyz = this.Xyz * -i;
                this.W = this.W * i;
            }
            return this;
        }

        /// <summary>
        /// Get the inverse of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion to invert</param>
        /// <returns>The inverse of the given quaternion</returns>
        public static Quaternion Invert(Quaternion q)
        {
            Quaternion result;
            Invert(ref q, out result);
            return result;
        }

        /// <summary>
        /// Get the inverse of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion to invert</param>
        /// <param name="result">The inverse of the given quaternion</param>
        public static void Invert(ref Quaternion q, out Quaternion result)
        {
            float lengthSq = (float)q.LengthSquared;
            if (lengthSq != 0.0)
            {
                float i = 1.0f / lengthSq;
                result = new Quaternion(q.Xyz * -i, q.W * i);
            }
            else
            {
                result = q;
            }
        }

        #endregion

        #region Normalize

        /// <summary>
        /// Scale the given quaternion to unit length
        /// </summary>
        /// <param name="q">The quaternion to normalize</param>
        /// <returns>The normalized quaternion</returns>
        public static Quaternion Normalize(Quaternion q)
        {
            Quaternion result;
            Normalize(ref q, out result);
            return result;
        }

        /// <summary>
        /// Scale the given quaternion to unit length
        /// </summary>
        /// <param name="q">The quaternion to normalize</param>
        /// <param name="result">The normalized quaternion</param>
        public static void Normalize(ref Quaternion q, out Quaternion result)
        {
            float scale = (float)(1.0 / q.Length);
            result = new Quaternion(q.Xyz * scale, q.W * scale);
        }

        #endregion

        #region FromAxisAngle
        private const float epsilon = 0.0000001f;

        /// <summary>
        /// Build a quaternion from the given axis and angle
        /// </summary>
        /// <param name="axis">The axis to rotate about</param>
        /// <param name="angle">The rotation angle in radians</param>
        /// <returns></returns>
        public static Quaternion FromAxisAngle(Vector3f axis, double angle)
        {
            Quaternion result = Identity;
            result.MakeRotate(axis, angle);
            return result;
        }

        /// <summary>
        /// Build a quaternion from the given axis and angle
        /// qx = ax * sin(angle/2)
        /// qy = ay* sin(angle/2)
        /// qz = az* sin(angle/2)
        /// qw = cos(angle/2)
        /// where
        /// the axis is normalised so: ax*ax + ay*ay + az*az = 1
        /// the quaternion is also normalised
        /// </summary>
        /// <param name="axis">The axis to rotate about</param>
        /// <param name="angle">The rotation angle in radians</param>
        /// <returns></returns>
        public void MakeRotate(Vector3f axis, double angle)
        {
            double length = axis.Length;
            if (length < epsilon)
            {
                this = Identity;
                return;
            }

            double inversenorm = 1.0f / length;
            double coshalfangle = System.Math.Cos(0.5 * angle);
            double sinhalfangle = System.Math.Sin(0.5 * angle);

            this.X = (float)(axis.X * sinhalfangle * inversenorm);
            this.Y = (float)(axis.Y * sinhalfangle * inversenorm);
            this.Z = (float)(axis.Z * sinhalfangle * inversenorm);
            this.W = (float)(coshalfangle);
        }
        #endregion

        #region Slerp

        /// <summary>
        /// Do Spherical linear interpolation between two quaternions 
        /// As t goes from 0 to 1, the Quat object goes from "from" to "to"
        /// Reference: Shoemake at SIGGRAPH 89
        /// See also
        /// http://www.gamasutra.com/features/programming/19980703/quaternions_01.htm
        /// </summary>
        /// <param name="q1">The first quaternion</param>
        /// <param name="q2">The second quaternion</param>
        /// <param name="blend">The blend factor</param>
        /// <returns>A smooth blend between the given quaternions</returns>
        public static Quaternion Slerp(Quaternion q1, Quaternion q2, float blend)
        {
            // if either input is zero, return the other.
            if (q1.LengthSquared == 0.0f)
            {
                if (q2.LengthSquared == 0.0f)
                {
                    return Identity;
                }
                return q2;
            }
            else if (q2.LengthSquared == 0.0f)
            {
                return q1;
            }


            float cosHalfAngle = q1.W * q2.W + Vector3f.Dot(q1.Xyz, q2.Xyz);

            if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
            {
                // angle = 0.0f, so just return one input.
                return q1;
            }
            else if (cosHalfAngle < 0.0f)
            {
                q2.Xyz = -q2.Xyz;
                q2.W = -q2.W;
                cosHalfAngle = -cosHalfAngle;
            }

            float blendA;
            float blendB;
            if (cosHalfAngle < 0.99f)
            {
                // do proper slerp for big angles
                float halfAngle = (float)System.Math.Acos(cosHalfAngle);
                float sinHalfAngle = (float)System.Math.Sin(halfAngle);
                float oneOverSinHalfAngle = 1.0f / sinHalfAngle;
                blendA = (float)System.Math.Sin(halfAngle * (1.0f - blend)) * oneOverSinHalfAngle;
                blendB = (float)System.Math.Sin(halfAngle * blend) * oneOverSinHalfAngle;
            }
            else
            {
                // do lerp if angle is really small.
                blendA = 1.0f - blend;
                blendB = blend;
            }

            Quaternion result = new Quaternion(blendA * q1.Xyz + blendB * q2.Xyz, blendA * q1.W + blendB * q2.W);
            if (result.LengthSquared > 0.0f)
                return Normalize(result);
            else
                return Identity;
        }

        #endregion

        #endregion

        #region Operators

        /// <summary>
        /// Adds two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Quaternion operator +(Quaternion left, Quaternion right)
        {
            return new Quaternion(left.Xyz + right.Xyz, left.W + right.W);
        }


        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Quaternion operator -(Quaternion left, Quaternion right)
        {
            return new Quaternion(left.Xyz - right.Xyz, left.W - right.W);
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Quaternion operator *(Quaternion left, Quaternion right)
        {
            Quaternion rst = new Quaternion();
            Multiply(ref left, ref right, out rst);
            return rst;
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaternion operator *(Quaternion quaternion, float scale)
        {
            return new Quaternion(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaternion operator *(float scale, Quaternion quaternion)
        {
            return new Quaternion(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
        }


        /// <summary>
        /// Rotate a vector by this quaternion.
        /// </summary>
        /// <param name="quaternion"></param>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3f operator *(Quaternion quaternion, Vector3f vec)
        {
            Vector3f uv, uuv;
            Vector3f qvec = quaternion.xyz;
            Vector3f.Cross(ref qvec, ref vec, out uv);
            Vector3f.Cross(ref qvec, ref uv, out uuv);
            uv *= (float)(2.0f * quaternion.W);
            uuv *= 2.0f;
            return vec + uv + uuv;
        }

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(Quaternion left, Quaternion right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(Quaternion left, Quaternion right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(Quaternion l, Quaternion v)
        {
            if (l.X < v.X) return true;
            else if (l.X > v.X) return false;
            else if (l.Y < v.Y) return true;
            else if (l.Y > v.Y) return false;
            else if (l.Z < v.Z) return true;
            else if (l.Z > v.Z) return false;
            else return (l.W < v.W);
        }

        public static bool operator >(Quaternion l, Quaternion v)
        {
            if (l.X > v.X) return true;
            else if (l.X < v.X) return false;
            else if (l.Y > v.Y) return true;
            else if (l.Y < v.Y) return false;
            else if (l.Z > v.Z) return true;
            else if (l.Z < v.Z) return false;
            else return (l.W > v.W);
        }
        public static bool operator <=(Quaternion l, Quaternion v)
        {
            if (l.X < v.X) return true;
            else if (l.X > v.X) return false;
            else if (l.Y < v.Y) return true;
            else if (l.Y > v.Y) return false;
            else if (l.Z < v.Z) return true;
            else if (l.Z > v.Z) return false;
            else if (l.W <= v.W) return true;
            else return false;
        }

        public static bool operator >=(Quaternion l, Quaternion v)
        {
            if (l.X > v.X) return true;
            else if (l.X < v.X) return false;
            else if (l.Y > v.Y) return true;
            else if (l.Y < v.Y) return false;
            else if (l.Z > v.Z) return true;
            else if (l.W < v.W) return false;
            else return true;
        }
        #endregion

        #region FromMatrix
        /// <summary>
        /// Get the matrix rotation as a Quat. Note that this function
        /// assumes a non-scaled matrix and will return incorrect results
        /// for scaled matrixces.Consider decompose() instead.
        /// Algorithm in: 
        /// http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Quaternion FromRotateMatrix(Matrix3f m)
        {
            Quaternion q = new Quaternion();
            float tr = m.R0C0 + m.R1C1 + m.R2C2;

            if (tr > 0)
            {
                float S = (float)System.Math.Sqrt(tr + 1.0) * 2; // S=4*qw 
                q.W = 0.25f * S;
                q.X = (m.R2C1 - m.R1C2) / S;
                q.Y = (m.R0C2 - m.R2C0) / S;
                q.Z = (m.R1C0 - m.R0C1) / S;
            }
            else if ((m.R0C0 > m.R1C1) & (m.R0C0 > m.R2C2))
            {
                float S = (float)System.Math.Sqrt(1.0 + m.R0C0 - m.R1C1 - m.R2C2) * 2; // S=4*qx 
                q.W = (m.R2C1 - m.R1C2) / S;
                q.X = 0.25f * S;
                q.Y = (m.R0C1 + m.R1C0) / S;
                q.Z = (m.R0C2 + m.R2C0) / S;
            }
            else if (m.R1C1 > m.R2C2)
            {
                float S = (float)System.Math.Sqrt(1.0 + m.R1C1 - m.R0C0 - m.R2C2) * 2; // S=4*qy
                q.W = (m.R0C2 - m.R2C0) / S;
                q.X = (m.R0C1 + m.R1C0) / S;
                q.Y = 0.25f * S;
                q.Z = (m.R1C2 + m.R2C1) / S;
            }
            else
            {
                float S = (float)System.Math.Sqrt(1.0 + m.R2C2 - m.R0C0 - m.R1C1) * 2; // S=4*qz
                q.W = (m.R1C0 - m.R0C1) / S;
                q.X = (m.R0C2 + m.R2C0) / S;
                q.Y = (m.R1C2 + m.R2C1) / S;
                q.Z = 0.25f * S;
            }
            return q;
        }

        /// <summary>Constructs left matrix from the given quaternion.</summary>
        /// <param name="quaternion">The quaternion to use to construct the martix.</param>
        //public Matrix3f ToMatrix3f()
        //{
        //    return new Matrix3f(this);
        //}
        public static Matrix3f ToMatrix3f(Quaternion quaternion)
        {
            quaternion.Normalize();

            double xx = quaternion.X * quaternion.X;
            double yy = quaternion.Y * quaternion.Y;
            double zz = quaternion.Z * quaternion.Z;
            double xy = quaternion.X * quaternion.Y;
            double xz = quaternion.X * quaternion.Z;
            double yz = quaternion.Y * quaternion.Z;
            double wx = quaternion.W * quaternion.X;
            double wy = quaternion.W * quaternion.Y;
            double wz = quaternion.W * quaternion.Z;

            Matrix3f rst = new Matrix3f();
            rst.R0C0 = (float)(1 - 2 * (yy + zz));
            rst.R0C1 = (float)(2 * (xy - wz));
            rst.R0C2 = (float)(2 * (xz + wy));

            rst.R1C0 = (float)(2 * (xy + wz));
            rst.R1C1 = (float)(1 - 2 * (xx + zz));
            rst.R1C2 = (float)(2 * (yz - wx));

            rst.R2C0 = (float)(2 * (xz - wy));
            rst.R2C1 = (float)(2 * (yz + wx));
            rst.R2C2 = (float)(1 - 2 * (xx + yy));

            return rst;
        }
        public static Matrix4f ToMatrix4f(Quaternion quaternion)
        {
            quaternion.Normalize();

            double xx = quaternion.X * quaternion.X;
            double yy = quaternion.Y * quaternion.Y;
            double zz = quaternion.Z * quaternion.Z;
            double xy = quaternion.X * quaternion.Y;
            double xz = quaternion.X * quaternion.Z;
            double yz = quaternion.Y * quaternion.Z;
            double wx = quaternion.W * quaternion.X;
            double wy = quaternion.W * quaternion.Y;
            double wz = quaternion.W * quaternion.Z;

            Matrix4f rst = new Matrix4f();
            rst.R0C0 = (float)(1 - 2 * (yy + zz));
            rst.R0C1 = (float)(2 * (xy - wz));
            rst.R0C2 = (float)(2 * (xz + wy));

            rst.R1C0 = (float)(2 * (xy + wz));
            rst.R1C1 = (float)(1 - 2 * (xx + zz));
            rst.R1C2 = (float)(2 * (yz - wx));

            rst.R2C0 = (float)(2 * (xz - wy));
            rst.R2C1 = (float)(2 * (yz + wx));
            rst.R2C2 = (float)(1 - 2 * (xx + yy));

            rst.R0C3 = rst.R1C3 = rst.R2C3 = rst.R3C0 = rst.R3C1 = rst.R3C2 = 0;
            rst.R3C3 = 1;

            return rst;
        }

        #endregion

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current Quaternion.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("V: {0}, W: {1}", Xyz, W);
        }

        #endregion

        #region public override bool Equals (object o)

        /// <summary>
        /// Compares this object instance to another object for equality. 
        /// </summary>
        /// <param name="other">The other object to be used in the comparison.</param>
        /// <returns>True if both objects are Quaternions of equal value. Otherwise it returns false.</returns>
        public override bool Equals(object other)
        {
            if (other is Quaternion == false) return false;
            return this == (Quaternion)other;
        }

        #endregion

        #region public override int GetHashCode ()

        /// <summary>
        /// Provides the hash code for this object. 
        /// </summary>
        /// <returns>A hash code formed from the bitwise XOR of this objects members.</returns>
        public override int GetHashCode()
        {
            return Xyz.GetHashCode() ^ W.GetHashCode();
        }

        #endregion

        #endregion

        #endregion

        #region IEquatable<Quaternion> Members

        /// <summary>
        /// Compares this Quaternion instance to another Quaternion for equality. 
        /// </summary>
        /// <param name="other">The other Quaternion to be used in the comparison.</param>
        /// <returns>True if both instances are equal; false otherwise.</returns>
        public bool Equals(Quaternion other)
        {
            return Xyz == other.Xyz && W == other.W;
        }

        #endregion
    }
}
