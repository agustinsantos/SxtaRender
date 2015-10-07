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
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using OpenTK;
using Sxta.Core;

namespace Sxta.Math
{

	[Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix2i : IEquatable<Matrix2i>
    {
        #region Fields & Access

        /// <summary>Row 0, Column 0</summary>
        public int R0C0;

        /// <summary>Row 0, Column 1</summary>
        public int R0C1;

        /// <summary>Row 1, Column 0</summary>
        public int R1C0;

        /// <summary>Row 1, Column 1</summary>
        public int R1C1;

        /// <summary>Gets the component at the given row and column in the matrix.</summary>
        /// <param name="row">The row of the matrix.</param>
        /// <param name="column">The column of the matrix.</param>
        /// <returns>The component at the given row and column in the matrix.</returns>
        public int this[int row, int column]
        {
            get
            {
                switch( row )
                {
                    case 0:
                        switch (column)
                        {
                            case 0: return R0C0;
                            case 1: return R0C1;
                        }
                        break;

                    case 1:
                        switch (column)
                        {
                            case 0: return R1C0;
                            case 1: return R1C1;
                        }
                        break;
                }

                throw new IndexOutOfRangeException();
            }
            set
            {
                switch( row )
                {
                    case 0:
                        switch (column)
                        {
                            case 0: R0C0 = value; return;
                            case 1: R0C1 = value; return;
                        }
                        break;

                    case 1:
                        switch (column)
                        {
                            case 0: R1C0 = value; return;
                            case 1: R1C1 = value; return;
                        }
                        break;
                }

                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>Gets the component at the index into the matrix.</summary>
        /// <param name="index">The index into the components of the matrix.</param>
        /// <returns>The component at the given index into the matrix.</returns>
        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return R0C0;
                    case 1: return R0C1;
                    case 2: return R1C0;
                    case 3: return R1C1;
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0: R0C0 = value; return;
                    case 1: R0C1 = value; return;
                    case 2: R1C0 = value; return;
                    case 3: R1C1 = value; return;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>Converts the matrix into an IntPtr.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>An IntPtr for the matrix.</returns>
        public static explicit operator IntPtr(Matrix2i matrix)
        {
            unsafe
            {
                return (IntPtr)(&matrix.R0C0);
            }
        }

        /// <summary>Converts the matrix into left int*.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>A int* for the matrix.</returns>
        [CLSCompliant(false)]
        unsafe public static explicit operator int*(Matrix2i matrix)
        {
            return &matrix.R0C0;
        }

        /// <summary>Converts the matrix into an array of ints.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>An array of ints for the matrix.</returns>
        public static explicit operator int[](Matrix2i matrix)
        {
            return new int[4]
            {
                matrix.R0C0,
                matrix.R0C1,
                matrix.R1C0,
                matrix.R1C1
            };
        }

        #endregion

        #region Constructors

        /// <summary>Constructs left matrix with the same components as the given matrix.</summary>
        /// <param name="vector">The matrix whose components to copy.</param>
        public Matrix2i(ref Matrix2i matrix)
        {
            this.R0C0 = matrix.R0C0;
            this.R0C1 = matrix.R0C1;
            this.R1C0 = matrix.R1C0;
            this.R1C1 = matrix.R1C1;
        }

        /// <summary>Constructs left matrix with the given values.</summary>
        /// <param name="r0c0">The value for row 0 column 0.</param>
        /// <param name="r0c1">The value for row 0 column 1.</param>
        /// <param name="r1c0">The value for row 1 column 0.</param>
        /// <param name="r1c1">The value for row 1 column 1.</param>
        public Matrix2i
        (
            int r0c0,
            int r0c1,
            int r1c0,
            int r1c1
        )
        {
            this.R0C0 = r0c0;
            this.R0C1 = r0c1;
            this.R1C0 = r1c0;
            this.R1C1 = r1c1;
        }

        /// <summary>Constructs left matrix from the given array of int-precision floating-point numbers.</summary>
        /// <param name="intArray">The array of ints for the components of the matrix in Column-major order.</param>
        public Matrix2i(int[] intArray)
        {
            if (intArray == null || intArray.GetLength(0) < 4) throw new MissingFieldException();

            this.R0C0 = intArray[0];
            this.R0C1 = intArray[1];
            this.R1C0 = intArray[2];
            this.R1C1 = intArray[3];
        }

        #endregion

        #region Equality

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix3i structure to compare with.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        [CLSCompliant(false)]
        public bool Equals(Matrix2i matrix)
        {
            return
                R0C0 == matrix.R0C0 &&
                R0C1 == matrix.R0C1 &&
                R1C0 == matrix.R1C0 &&
                R1C1 == matrix.R1C1;
        }

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix3i structure to compare to.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public bool Equals(ref Matrix2i matrix)
        {
            return
                R0C0 == matrix.R0C0 &&
                R0C1 == matrix.R0C1 &&
                R1C0 == matrix.R1C0 &&
                R1C1 == matrix.R1C1;
        }

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="left">The left-hand operand.</param>
        /// <param name="right">The right-hand operand.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public static bool Equals(ref Matrix2i left, ref Matrix2i right)
        {
            return
                left.R0C0 == right.R0C0 &&
                left.R0C1 == right.R0C1 &&
                left.R1C0 == right.R1C0 &&
                left.R1C1 == right.R1C1;
        }

        /// <summary>Indicates whether the current matrix is approximately equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix2i structure to compare with.</param>
        /// <param name="tolerance">The limit below which the matrices are considered equal.</param>
        /// <returns>true if the current matrix is approximately equal to the matrix parameter; otherwise, false.</returns>
        public bool EqualsApprox(ref Matrix2i matrix, int tolerance)
        {
            return
                System.Math.Abs(R0C0 - matrix.R0C0) <= tolerance &&
                System.Math.Abs(R0C1 - matrix.R0C1) <= tolerance &&
                System.Math.Abs(R1C0 - matrix.R1C0) <= tolerance &&
                System.Math.Abs(R1C1 - matrix.R1C1) <= tolerance;
        }

        /// <summary>Indicates whether the current matrix is approximately equal to another matrix.</summary>
        /// <param name="left">The left-hand operand.</param>
        /// <param name="right">The right-hand operand.</param>
        /// <param name="tolerance">The limit below which the matrices are considered equal.</param>
        /// <returns>true if the current matrix is approximately equal to the matrix parameter; otherwise, false.</returns>
        public static bool EqualsApprox(ref Matrix2i left, ref Matrix2i right, int tolerance)
        {
            return
                System.Math.Abs(left.R0C0 - right.R0C0) <= tolerance &&
                System.Math.Abs(left.R0C1 - right.R0C1) <= tolerance &&
                System.Math.Abs(left.R1C0 - right.R1C0) <= tolerance &&
                System.Math.Abs(left.R1C1 - right.R1C1) <= tolerance;
        }

        #endregion

        #region Arithmetic Operators


        /// <summary>Add left matrix to this matrix.</summary>
        /// <param name="matrix">The matrix to add.</param>
        public void Add(ref Matrix2i matrix)
        {
            R0C0 = R0C0 + matrix.R0C0;
            R0C1 = R0C1 + matrix.R0C1;
            R1C0 = R1C0 + matrix.R1C0;
            R1C1 = R1C1 + matrix.R1C1;
        }

        /// <summary>Add left matrix to this matrix.</summary>
        /// <param name="matrix">The matrix to add.</param>
        /// <param name="result">The resulting matrix of the addition.</param>
        public void Add(ref Matrix2i matrix, out Matrix2i result)
        {
            result.R0C0 = R0C0 + matrix.R0C0;
            result.R0C1 = R0C1 + matrix.R0C1;
            result.R1C0 = R1C0 + matrix.R1C0;
            result.R1C1 = R1C1 + matrix.R1C1;
        }

        /// <summary>Add left matrix to left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the addition.</param>
        public static void Add(ref Matrix2i left, ref Matrix2i right, out Matrix2i result)
        {
            result.R0C0 = left.R0C0 + right.R0C0;
            result.R0C1 = left.R0C1 + right.R0C1;
            result.R1C0 = left.R1C0 + right.R1C0;
            result.R1C1 = left.R1C1 + right.R1C1;
        }


        /// <summary>Subtract left matrix from this matrix.</summary>
        /// <param name="matrix">The matrix to subtract.</param>
        public void Subtract(ref Matrix2i matrix)
        {
            R0C0 = R0C0 + matrix.R0C0;
            R0C1 = R0C1 + matrix.R0C1;
            R1C0 = R1C0 + matrix.R1C0;
            R1C1 = R1C1 + matrix.R1C1;
        }

        /// <summary>Subtract left matrix from this matrix.</summary>
        /// <param name="matrix">The matrix to subtract.</param>
        /// <param name="result">The resulting matrix of the subtraction.</param>
        public void Subtract(ref Matrix2i matrix, out Matrix2i result)
        {
            result.R0C0 = R0C0 + matrix.R0C0;
            result.R0C1 = R0C1 + matrix.R0C1;
            result.R1C0 = R1C0 + matrix.R1C0;
            result.R1C1 = R1C1 + matrix.R1C1;
        }

        /// <summary>Subtract left matrix from left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the subtraction.</param>
        public static void Subtract(ref Matrix2i left, ref Matrix2i right, out Matrix2i result)
        {
            result.R0C0 = left.R0C0 + right.R0C0;
            result.R0C1 = left.R0C1 + right.R0C1;
            result.R1C0 = left.R1C0 + right.R1C0;
            result.R1C1 = left.R1C1 + right.R1C1;
        }


        /// <summary>Multiply left martix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        public void Multiply(ref Matrix2i matrix)
        {
            int r0c0 = matrix.R0C0 * R0C0 + matrix.R0C1 * R1C0;
            int r0c1 = matrix.R0C0 * R0C1 + matrix.R0C1 * R1C1;

            int r1c0 = matrix.R1C0 * R0C0 + matrix.R1C1 * R1C0;
            int r1c1 = matrix.R1C0 * R0C1 + matrix.R1C1 * R1C1;

            R0C0 = r0c0;
            R0C1 = r0c1;

            R1C0 = r1c0;
            R1C1 = r1c1;
        }

        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public void Multiply(ref Matrix2i matrix, out Matrix2i result)
        {
            result.R0C0 = matrix.R0C0 * R0C0 + matrix.R0C1 * R1C0;
            result.R0C1 = matrix.R0C0 * R0C1 + matrix.R0C1 * R1C1;
            result.R1C0 = matrix.R1C0 * R0C0 + matrix.R1C1 * R1C0;
            result.R1C1 = matrix.R1C0 * R0C1 + matrix.R1C1 * R1C1;
        }

        /// <summary>Multiply left matrix times left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public static void Multiply(ref Matrix2i left, ref Matrix2i right, out Matrix2i result)
        {
            result.R0C0 = right.R0C0 * left.R0C0 + right.R0C1 * left.R1C0;
            result.R0C1 = right.R0C0 * left.R0C1 + right.R0C1 * left.R1C1;
            result.R1C0 = right.R1C0 * left.R0C0 + right.R1C1 * left.R1C0;
            result.R1C1 = right.R1C0 * left.R0C1 + right.R1C1 * left.R1C1;
        }


        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        public void Multiply(int scalar)
        {
            R0C0 = scalar * R0C0;
            R0C1 = scalar * R0C1;
            R1C0 = scalar * R1C0;
            R1C1 = scalar * R1C1;
        }

        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public void Multiply(int scalar, out Matrix2i result)
        {
            result.R0C0 = scalar * R0C0;
            result.R0C1 = scalar * R0C1;
            result.R1C0 = scalar * R1C0;
            result.R1C1 = scalar * R1C1;
        }

        /// <summary>Multiply left matrix times left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public static void Multiply(ref Matrix3i matrix, int scalar, out Matrix2i result)
        {
            result.R0C0 = scalar * matrix.R0C0;
            result.R0C1 = scalar * matrix.R0C1;
            result.R1C0 = scalar * matrix.R1C0;
            result.R1C1 = scalar * matrix.R1C1;
        }


        #endregion

        #region Functions

        public int Determinant
        {
            get
            {
                return R0C0 * R1C1 - R0C1 * R1C0;
            }
        }

        public void Transpose()
        {
            Std.Swap(ref R0C1, ref R1C0);
        }
        public void Transpose(out Matrix2i result)
        {
            result.R0C0 = R0C0;
            result.R0C1 = R1C0;
            result.R1C0 = R0C1;
            result.R1C1 = R1C1;
        }
        public static void Transpose(ref Matrix2i matrix, out Matrix2i result)
        {
            result.R0C0 = matrix.R0C0;
            result.R0C1 = matrix.R1C0;
            result.R1C0 = matrix.R0C1;
            result.R1C1 = matrix.R1C1;
        }

        #endregion

        #region Transformation Functions

        public void Transform(ref Vector2d vector)
        {
            vector.X = R0C0 * vector.X + R0C1 * vector.Y;
            vector.Y = R1C0 * vector.X + R1C1 * vector.Y;
        }
        public static void Transform(ref Matrix2i matrix, ref Vector2d vector)
        {
            vector.X = matrix.R0C0 * vector.X + matrix.R0C1 * vector.Y;
            vector.Y = matrix.R1C0 * vector.X + matrix.R1C1 * vector.Y;
        }
        public void Transform(ref Vector2d vector, out Vector2d result)
        {
            result.X = R0C0 * vector.X + R0C1 * vector.Y;
            result.Y = R1C0 * vector.X + R1C1 * vector.Y;
        }
        public static void Transform(ref Matrix2i matrix, ref Vector2d vector, out Vector2d result)
        {
            result.X = matrix.R0C0 * vector.X + matrix.R0C1 * vector.Y;
            result.Y = matrix.R1C0 * vector.X + matrix.R1C1 * vector.Y;
        }

        public void Rotate(int angle)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin =  System.Math.Sin(angleRadians);
            double cos =  System.Math.Cos(angleRadians);

            R1C0 = (int)(cos * R1C0 - sin * R0C0);
            R1C1 = (int)(cos * R1C1 - sin * R0C1);

            R0C0 = (int)(cos * R0C0 + sin * R1C0);
            R0C1 = (int)(cos * R0C1 + sin * R1C1);
        }
        public void Rotate(int angle, out Matrix2i result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos =  System.Math.Cos(angleRadians);

            result.R0C0 = (int)(cos * R0C0 + sin * R1C0);
            result.R0C1 = (int)(cos * R0C1 + sin * R1C1);
            result.R1C0 = (int)(cos * R1C0 - sin * R0C0);
            result.R1C1 = (int)(cos * R1C1 - sin * R0C1);
        }
        public static void Rotate(ref Matrix2i matrix, int angle, out Matrix2i result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos = System.Math.Cos(angleRadians);

            result.R0C0 = (int)(cos * matrix.R0C0 + sin * matrix.R1C0);
            result.R0C1 = (int)(cos * matrix.R0C1 + sin * matrix.R1C1);
            result.R1C0 = (int)(cos * matrix.R1C0 - sin * matrix.R0C0);
            result.R1C1 = (int)(cos * matrix.R1C1 - sin * matrix.R0C1);
        }
        public static void RotateMatrix(int angle, out Matrix2i result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos = System.Math.Cos(angleRadians);

            result.R0C0 =  (int)cos;
            result.R0C1 =  (int)sin;
            result.R1C0 =  (int)-sin;
            result.R1C1 =  (int)cos;
        }

        #endregion

        #region Constants

		/// <summary>
        /// Defines the size of the Matrix2i struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(new Matrix2i());

        /// <summary>The identity matrix.</summary>
        public static readonly Matrix2i Identity = new Matrix2i
        (
            1, 0,
            0, 1
        );

        /// <summary>A matrix of all zeros.</summary>
        public static readonly Matrix2i Zero = new Matrix2i
        (
            0, 0,
            0, 0
        );

        #endregion

        #region HashCode

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return
                R0C0.GetHashCode() ^ R0C1.GetHashCode() ^
                R1C0.GetHashCode() ^ R1C1.GetHashCode();
        }

        #endregion

        #region String

        /// <summary>Returns the fully qualified type name of this instance.</summary>
        /// <returns>A System.String containing left fully qualified type name.</returns>
        public override string ToString()
        {
            return String.Format(
                "|{00}, {01}|\n" +
                "|{02}, {03}|\n" +
                R0C0, R0C1, 
                R1C0, R1C1);
        }

        #endregion
    }

	[Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3i : IEquatable<Matrix3i>
    {
        #region Fields & Access

        /// <summary>Row 0, Column 0</summary>
        public int R0C0;

        /// <summary>Row 0, Column 1</summary>
        public int R0C1;

        /// <summary>Row 0, Column 2</summary>
        public int R0C2;

        /// <summary>Row 1, Column 0</summary>
        public int R1C0;

        /// <summary>Row 1, Column 1</summary>
        public int R1C1;

        /// <summary>Row 1, Column 2</summary>
        public int R1C2;

        /// <summary>Row 2, Column 0</summary>
        public int R2C0;

        /// <summary>Row 2, Column 1</summary>
        public int R2C1;

        /// <summary>Row 2, Column 2</summary>
        public int R2C2;

        /// <summary>Gets the component at the given row and column in the matrix.</summary>
        /// <param name="row">The row of the matrix.</param>
        /// <param name="column">The column of the matrix.</param>
        /// <returns>The component at the given row and column in the matrix.</returns>
        public int this[int row, int column]
        {
            get
            {
                switch( row )
                {
                    case 0:
                        switch (column)
                        {
                            case 0: return R0C0;
                            case 1: return R0C1;
                            case 2: return R0C2;
                        }
                        break;

                    case 1:
                        switch (column)
                        {
                            case 0: return R1C0;
                            case 1: return R1C1;
                            case 2: return R1C2;
                        }
                        break;

                    case 2:
                        switch (column)
                        {
                            case 0: return R2C0;
                            case 1: return R2C1;
                            case 2: return R2C2;
                        }
                        break;
                }

                throw new IndexOutOfRangeException();
            }
            set
            {
                switch( row )
                {
                    case 0:
                        switch (column)
                        {
                            case 0: R0C0 = value; return;
                            case 1: R0C1 = value; return;
                            case 2: R0C2 = value; return;
                        }
                        break;

                    case 1:
                        switch (column)
                        {
                            case 0: R1C0 = value; return;
                            case 1: R1C1 = value; return;
                            case 2: R1C2 = value; return;
                        }
                        break;

                    case 2:
                        switch (column)
                        {
                            case 0: R2C0 = value; return;
                            case 1: R2C1 = value; return;
                            case 2: R2C2 = value; return;
                        }
                        break;
                }

                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>Gets the component at the index into the matrix.</summary>
        /// <param name="index">The index into the components of the matrix.</param>
        /// <returns>The component at the given index into the matrix.</returns>
        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return R0C0;
                    case 1: return R0C1;
                    case 2: return R0C2;
                    case 3: return R1C0;
                    case 4: return R1C1;
                    case 5: return R1C2;
                    case 6: return R2C0;
                    case 7: return R2C1;
                    case 8: return R2C2;
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0: R0C0 = value; return;
                    case 1: R0C1 = value; return;
                    case 2: R0C2 = value; return;
                    case 3: R1C0 = value; return;
                    case 4: R1C1 = value; return;
                    case 5: R1C2 = value; return;
                    case 6: R2C0 = value; return;
                    case 7: R2C1 = value; return;
                    case 8: R2C2 = value; return;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>Converts the matrix into an IntPtr.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>An IntPtr for the matrix.</returns>
        public static explicit operator IntPtr(Matrix3i matrix)
        {
            unsafe
            {
                return (IntPtr)(&matrix.R0C0);
            }
        }

        /// <summary>Converts the matrix into left int*.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>A int* for the matrix.</returns>
        [CLSCompliant(false)]
        unsafe public static explicit operator int*(Matrix3i matrix)
        {
            return &matrix.R0C0;
        }

        /// <summary>Converts the matrix into an array of ints.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>An array of ints for the matrix.</returns>
        public static explicit operator int[](Matrix3i matrix)
        {
            return new int[9]
            {
                matrix.R0C0,
                matrix.R0C1,
                matrix.R0C2,
                matrix.R1C0,
                matrix.R1C1,
                matrix.R1C2,
                matrix.R2C0,
                matrix.R2C1,
                matrix.R2C2
            };
        }

        #endregion

        #region Constructors

        /// <summary>Constructs left matrix with the same components as the given matrix.</summary>
        /// <param name="vector">The matrix whose components to copy.</param>
        public Matrix3i(ref Matrix3i matrix)
        {
            this.R0C0 = matrix.R0C0;
            this.R0C1 = matrix.R0C1;
            this.R0C2 = matrix.R0C2;
            this.R1C0 = matrix.R1C0;
            this.R1C1 = matrix.R1C1;
            this.R1C2 = matrix.R1C2;
            this.R2C0 = matrix.R2C0;
            this.R2C1 = matrix.R2C1;
            this.R2C2 = matrix.R2C2;
        }

        /// <summary>Constructs left matrix with the given values.</summary>
        /// <param name="r0c0">The value for row 0 column 0.</param>
        /// <param name="r0c1">The value for row 0 column 1.</param>
        /// <param name="r0c2">The value for row 0 column 2.</param>
        /// <param name="r1c0">The value for row 1 column 0.</param>
        /// <param name="r1c1">The value for row 1 column 1.</param>
        /// <param name="r1c2">The value for row 1 column 2.</param>
        /// <param name="r2c0">The value for row 2 column 0.</param>
        /// <param name="r2c1">The value for row 2 column 1.</param>
        /// <param name="r2c2">The value for row 2 column 2.</param>
        public Matrix3i
        (
            int r0c0,
            int r0c1,
            int r0c2,
            int r1c0,
            int r1c1,
            int r1c2,
            int r2c0,
            int r2c1,
            int r2c2
        )
        {
            this.R0C0 = r0c0;
            this.R0C1 = r0c1;
            this.R0C2 = r0c2;
            this.R1C0 = r1c0;
            this.R1C1 = r1c1;
            this.R1C2 = r1c2;
            this.R2C0 = r2c0;
            this.R2C1 = r2c1;
            this.R2C2 = r2c2;
        }

        /// <summary>Constructs left matrix from the given array of int-precision floating-point numbers.</summary>
        /// <param name="intArray">The array of ints for the components of the matrix in Column-major order.</param>
        public Matrix3i(int[] intArray)
        {
            if (intArray == null || intArray.GetLength(0) < 9) throw new MissingFieldException();

            this.R0C0 = intArray[0];
            this.R0C1 = intArray[1];
            this.R0C2 = intArray[2];
            this.R1C0 = intArray[3];
            this.R1C1 = intArray[4];
            this.R1C2 = intArray[5];
            this.R2C0 = intArray[6];
            this.R2C1 = intArray[7];
            this.R2C2 = intArray[8];
        }

        /// <summary>Constructs left matrix from the given quaternion.</summary>
        /// <param name="quaternion">The quaternion to use to construct the martix.</param>
        public Matrix3i(Quaterniond quaternion)
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

            R0C0 = (int)(1 - 2 * (yy + zz));
            R0C1 = (int)(2 * (xy - wz));
            R0C2 = (int)(2 * (xz + wy));

            R1C0 = (int)(2 * (xy + wz));
            R1C1 = (int)(1 - 2 * (xx + zz));
            R1C2 = (int)(2 * (yz - wx));

            R2C0 = (int)(2 * (xz - wy));
            R2C1 = (int)(2 * (yz + wx));
            R2C2 = (int)(1 - 2 * (xx + yy));
        }

        #endregion

        #region Equality

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix3i structure to compare with.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        [CLSCompliant(false)]
        public bool Equals(Matrix3i matrix)
        {
            return
                R0C0 == matrix.R0C0 &&
                R0C1 == matrix.R0C1 &&
                R0C2 == matrix.R0C2 &&
                R1C0 == matrix.R1C0 &&
                R1C1 == matrix.R1C1 &&
                R1C2 == matrix.R1C2 &&
                R2C0 == matrix.R2C0 &&
                R2C1 == matrix.R2C1 &&
                R2C2 == matrix.R2C2;
        }

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix3i structure to compare to.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public bool Equals(ref Matrix3i matrix)
        {
            return
                R0C0 == matrix.R0C0 &&
                R0C1 == matrix.R0C1 &&
                R0C2 == matrix.R0C2 &&
                R1C0 == matrix.R1C0 &&
                R1C1 == matrix.R1C1 &&
                R1C2 == matrix.R1C2 &&
                R2C0 == matrix.R2C0 &&
                R2C1 == matrix.R2C1 &&
                R2C2 == matrix.R2C2;
        }

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="left">The left-hand operand.</param>
        /// <param name="right">The right-hand operand.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public static bool Equals(ref Matrix3i left, ref Matrix3i right)
        {
            return
                left.R0C0 == right.R0C0 &&
                left.R0C1 == right.R0C1 &&
                left.R0C2 == right.R0C2 &&
                left.R1C0 == right.R1C0 &&
                left.R1C1 == right.R1C1 &&
                left.R1C2 == right.R1C2 &&
                left.R2C0 == right.R2C0 &&
                left.R2C1 == right.R2C1 &&
                left.R2C2 == right.R2C2;
        }

        /// <summary>Indicates whether the current matrix is approximately equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix3i structure to compare with.</param>
        /// <param name="tolerance">The limit below which the matrices are considered equal.</param>
        /// <returns>true if the current matrix is approximately equal to the matrix parameter; otherwise, false.</returns>
        public bool EqualsApprox(ref Matrix3i matrix, int tolerance)
        {
            return
                System.Math.Abs(R0C0 - matrix.R0C0) <= tolerance &&
                System.Math.Abs(R0C1 - matrix.R0C1) <= tolerance &&
                System.Math.Abs(R0C2 - matrix.R0C2) <= tolerance &&
                System.Math.Abs(R1C0 - matrix.R1C0) <= tolerance &&
                System.Math.Abs(R1C1 - matrix.R1C1) <= tolerance &&
                System.Math.Abs(R1C2 - matrix.R1C2) <= tolerance &&
                System.Math.Abs(R2C0 - matrix.R2C0) <= tolerance &&
                System.Math.Abs(R2C1 - matrix.R2C1) <= tolerance &&
                System.Math.Abs(R2C2 - matrix.R2C2) <= tolerance;
        }

        /// <summary>Indicates whether the current matrix is approximately equal to another matrix.</summary>
        /// <param name="left">The left-hand operand.</param>
        /// <param name="right">The right-hand operand.</param>
        /// <param name="tolerance">The limit below which the matrices are considered equal.</param>
        /// <returns>true if the current matrix is approximately equal to the matrix parameter; otherwise, false.</returns>
        public static bool EqualsApprox(ref Matrix3i left, ref Matrix3i right, int tolerance)
        {
            return
                System.Math.Abs(left.R0C0 - right.R0C0) <= tolerance &&
                System.Math.Abs(left.R0C1 - right.R0C1) <= tolerance &&
                System.Math.Abs(left.R0C2 - right.R0C2) <= tolerance &&
                System.Math.Abs(left.R1C0 - right.R1C0) <= tolerance &&
                System.Math.Abs(left.R1C1 - right.R1C1) <= tolerance &&
                System.Math.Abs(left.R1C2 - right.R1C2) <= tolerance &&
                System.Math.Abs(left.R2C0 - right.R2C0) <= tolerance &&
                System.Math.Abs(left.R2C1 - right.R2C1) <= tolerance &&
                System.Math.Abs(left.R2C2 - right.R2C2) <= tolerance;
        }

        #endregion

        #region Arithmetic Operators


        /// <summary>Add left matrix to this matrix.</summary>
        /// <param name="matrix">The matrix to add.</param>
        public void Add(ref Matrix3i matrix)
        {
            R0C0 = R0C0 + matrix.R0C0;
            R0C1 = R0C1 + matrix.R0C1;
            R0C2 = R0C2 + matrix.R0C2;
            R1C0 = R1C0 + matrix.R1C0;
            R1C1 = R1C1 + matrix.R1C1;
            R1C2 = R1C2 + matrix.R1C2;
            R2C0 = R2C0 + matrix.R2C0;
            R2C1 = R2C1 + matrix.R2C1;
            R2C2 = R2C2 + matrix.R2C2;
        }

        /// <summary>Add left matrix to this matrix.</summary>
        /// <param name="matrix">The matrix to add.</param>
        /// <param name="result">The resulting matrix of the addition.</param>
        public void Add(ref Matrix3i matrix, out Matrix3i result)
        {
            result.R0C0 = R0C0 + matrix.R0C0;
            result.R0C1 = R0C1 + matrix.R0C1;
            result.R0C2 = R0C2 + matrix.R0C2;
            result.R1C0 = R1C0 + matrix.R1C0;
            result.R1C1 = R1C1 + matrix.R1C1;
            result.R1C2 = R1C2 + matrix.R1C2;
            result.R2C0 = R2C0 + matrix.R2C0;
            result.R2C1 = R2C1 + matrix.R2C1;
            result.R2C2 = R2C2 + matrix.R2C2;
        }

        /// <summary>Add left matrix to left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the addition.</param>
        public static void Add(ref Matrix3i left, ref Matrix3i right, out Matrix3i result)
        {
            result.R0C0 = left.R0C0 + right.R0C0;
            result.R0C1 = left.R0C1 + right.R0C1;
            result.R0C2 = left.R0C2 + right.R0C2;
            result.R1C0 = left.R1C0 + right.R1C0;
            result.R1C1 = left.R1C1 + right.R1C1;
            result.R1C2 = left.R1C2 + right.R1C2;
            result.R2C0 = left.R2C0 + right.R2C0;
            result.R2C1 = left.R2C1 + right.R2C1;
            result.R2C2 = left.R2C2 + right.R2C2;
        }


        /// <summary>Subtract left matrix from this matrix.</summary>
        /// <param name="matrix">The matrix to subtract.</param>
        public void Subtract(ref Matrix3i matrix)
        {
            R0C0 = R0C0 + matrix.R0C0;
            R0C1 = R0C1 + matrix.R0C1;
            R0C2 = R0C2 + matrix.R0C2;
            R1C0 = R1C0 + matrix.R1C0;
            R1C1 = R1C1 + matrix.R1C1;
            R1C2 = R1C2 + matrix.R1C2;
            R2C0 = R2C0 + matrix.R2C0;
            R2C1 = R2C1 + matrix.R2C1;
            R2C2 = R2C2 + matrix.R2C2;
        }

        /// <summary>Subtract left matrix from this matrix.</summary>
        /// <param name="matrix">The matrix to subtract.</param>
        /// <param name="result">The resulting matrix of the subtraction.</param>
        public void Subtract(ref Matrix3i matrix, out Matrix3i result)
        {
            result.R0C0 = R0C0 + matrix.R0C0;
            result.R0C1 = R0C1 + matrix.R0C1;
            result.R0C2 = R0C2 + matrix.R0C2;
            result.R1C0 = R1C0 + matrix.R1C0;
            result.R1C1 = R1C1 + matrix.R1C1;
            result.R1C2 = R1C2 + matrix.R1C2;
            result.R2C0 = R2C0 + matrix.R2C0;
            result.R2C1 = R2C1 + matrix.R2C1;
            result.R2C2 = R2C2 + matrix.R2C2;
        }

        /// <summary>Subtract left matrix from left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the subtraction.</param>
        public static void Subtract(ref Matrix3i left, ref Matrix3i right, out Matrix3i result)
        {
            result.R0C0 = left.R0C0 + right.R0C0;
            result.R0C1 = left.R0C1 + right.R0C1;
            result.R0C2 = left.R0C2 + right.R0C2;
            result.R1C0 = left.R1C0 + right.R1C0;
            result.R1C1 = left.R1C1 + right.R1C1;
            result.R1C2 = left.R1C2 + right.R1C2;
            result.R2C0 = left.R2C0 + right.R2C0;
            result.R2C1 = left.R2C1 + right.R2C1;
            result.R2C2 = left.R2C2 + right.R2C2;
        }


        /// <summary>Multiply left martix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        public void Multiply(ref Matrix3i matrix)
        {
            int r0c0 = matrix.R0C0 * R0C0 + matrix.R0C1 * R1C0 + matrix.R0C2 * R2C0;
            int r0c1 = matrix.R0C0 * R0C1 + matrix.R0C1 * R1C1 + matrix.R0C2 * R2C1;
            int r0c2 = matrix.R0C0 * R0C2 + matrix.R0C1 * R1C2 + matrix.R0C2 * R2C2;

            int r1c0 = matrix.R1C0 * R0C0 + matrix.R1C1 * R1C0 + matrix.R1C2 * R2C0;
            int r1c1 = matrix.R1C0 * R0C1 + matrix.R1C1 * R1C1 + matrix.R1C2 * R2C1;
            int r1c2 = matrix.R1C0 * R0C2 + matrix.R1C1 * R1C2 + matrix.R1C2 * R2C2;

            R2C0 = matrix.R2C0 * R0C0 + matrix.R2C1 * R1C0 + matrix.R2C2 * R2C0;
            R2C1 = matrix.R2C0 * R0C1 + matrix.R2C1 * R1C1 + matrix.R2C2 * R2C1;
            R2C2 = matrix.R2C0 * R0C2 + matrix.R2C1 * R1C2 + matrix.R2C2 * R2C2;


            R0C0 = r0c0;
            R0C1 = r0c1;
            R0C2 = r0c2;

            R1C0 = r1c0;
            R1C1 = r1c1;
            R1C2 = r1c2;
        }

        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public void Multiply(ref Matrix3i matrix, out Matrix3i result)
        {
            result.R0C0 = matrix.R0C0 * R0C0 + matrix.R0C1 * R1C0 + matrix.R0C2 * R2C0;
            result.R0C1 = matrix.R0C0 * R0C1 + matrix.R0C1 * R1C1 + matrix.R0C2 * R2C1;
            result.R0C2 = matrix.R0C0 * R0C2 + matrix.R0C1 * R1C2 + matrix.R0C2 * R2C2;
            result.R1C0 = matrix.R1C0 * R0C0 + matrix.R1C1 * R1C0 + matrix.R1C2 * R2C0;
            result.R1C1 = matrix.R1C0 * R0C1 + matrix.R1C1 * R1C1 + matrix.R1C2 * R2C1;
            result.R1C2 = matrix.R1C0 * R0C2 + matrix.R1C1 * R1C2 + matrix.R1C2 * R2C2;
            result.R2C0 = matrix.R2C0 * R0C0 + matrix.R2C1 * R1C0 + matrix.R2C2 * R2C0;
            result.R2C1 = matrix.R2C0 * R0C1 + matrix.R2C1 * R1C1 + matrix.R2C2 * R2C1;
            result.R2C2 = matrix.R2C0 * R0C2 + matrix.R2C1 * R1C2 + matrix.R2C2 * R2C2;
        }

        /// <summary>Multiply left matrix times left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public static void Multiply(ref Matrix3i left, ref Matrix3i right, out Matrix3i result)
        {
            result.R0C0 = right.R0C0 * left.R0C0 + right.R0C1 * left.R1C0 + right.R0C2 * left.R2C0;
            result.R0C1 = right.R0C0 * left.R0C1 + right.R0C1 * left.R1C1 + right.R0C2 * left.R2C1;
            result.R0C2 = right.R0C0 * left.R0C2 + right.R0C1 * left.R1C2 + right.R0C2 * left.R2C2;
            result.R1C0 = right.R1C0 * left.R0C0 + right.R1C1 * left.R1C0 + right.R1C2 * left.R2C0;
            result.R1C1 = right.R1C0 * left.R0C1 + right.R1C1 * left.R1C1 + right.R1C2 * left.R2C1;
            result.R1C2 = right.R1C0 * left.R0C2 + right.R1C1 * left.R1C2 + right.R1C2 * left.R2C2;
            result.R2C0 = right.R2C0 * left.R0C0 + right.R2C1 * left.R1C0 + right.R2C2 * left.R2C0;
            result.R2C1 = right.R2C0 * left.R0C1 + right.R2C1 * left.R1C1 + right.R2C2 * left.R2C1;
            result.R2C2 = right.R2C0 * left.R0C2 + right.R2C1 * left.R1C2 + right.R2C2 * left.R2C2;
        }


        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        public void Multiply(int scalar)
        {
            R0C0 = scalar * R0C0;
            R0C1 = scalar * R0C1;
            R0C2 = scalar * R0C2;
            R1C0 = scalar * R1C0;
            R1C1 = scalar * R1C1;
            R1C2 = scalar * R1C2;
            R2C0 = scalar * R2C0;
            R2C1 = scalar * R2C1;
            R2C2 = scalar * R2C2;
        }

        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public void Multiply(int scalar, out Matrix3i result)
        {
            result.R0C0 = scalar * R0C0;
            result.R0C1 = scalar * R0C1;
            result.R0C2 = scalar * R0C2;
            result.R1C0 = scalar * R1C0;
            result.R1C1 = scalar * R1C1;
            result.R1C2 = scalar * R1C2;
            result.R2C0 = scalar * R2C0;
            result.R2C1 = scalar * R2C1;
            result.R2C2 = scalar * R2C2;
        }

        /// <summary>Multiply left matrix times left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public static void Multiply(ref Matrix3i matrix, int scalar, out Matrix3i result)
        {
            result.R0C0 = scalar * matrix.R0C0;
            result.R0C1 = scalar * matrix.R0C1;
            result.R0C2 = scalar * matrix.R0C2;
            result.R1C0 = scalar * matrix.R1C0;
            result.R1C1 = scalar * matrix.R1C1;
            result.R1C2 = scalar * matrix.R1C2;
            result.R2C0 = scalar * matrix.R2C0;
            result.R2C1 = scalar * matrix.R2C1;
            result.R2C2 = scalar * matrix.R2C2;
        }


        #endregion

        #region Functions

        public int Determinant
        {
            get
            {
                return R0C0 * R1C1 * R2C2 - R0C0 * R1C2 * R2C1 - R0C1 * R1C0 * R2C2 + R0C2 * R1C0 * R2C1 + R0C1 * R1C2 * R2C0 - R0C2 * R1C1 * R2C0;
            }
        }

        public void Transpose()
        {
            Std.Swap(ref R0C1, ref R1C0);
            Std.Swap(ref R0C2, ref R2C0);
            Std.Swap(ref R1C2, ref R2C1);
        }
        public void Transpose(out Matrix3i result)
        {
            result.R0C0 = R0C0;
            result.R0C1 = R1C0;
            result.R0C2 = R2C0;
            result.R1C0 = R0C1;
            result.R1C1 = R1C1;
            result.R1C2 = R2C1;
            result.R2C0 = R0C2;
            result.R2C1 = R1C2;
            result.R2C2 = R2C2;
        }
        public static void Transpose(ref Matrix3i matrix, out Matrix3i result)
        {
            result.R0C0 = matrix.R0C0;
            result.R0C1 = matrix.R1C0;
            result.R0C2 = matrix.R2C0;
            result.R1C0 = matrix.R0C1;
            result.R1C1 = matrix.R1C1;
            result.R1C2 = matrix.R2C1;
            result.R2C0 = matrix.R0C2;
            result.R2C1 = matrix.R1C2;
            result.R2C2 = matrix.R2C2;
        }

        #endregion

        #region Transformation Functions

        public void Transform(ref Vector3d vector)
        {
            double x = R0C0 * vector.X + R0C1 * vector.Y + R0C2 * vector.Z;
            double y = R1C0 * vector.X + R1C1 * vector.Y + R1C2 * vector.Z;
            vector.Z = (int)(R2C0 * vector.X + R2C1 * vector.Y + R2C2 * vector.Z);
            vector.X = x;
            vector.Y = y;
        }
        public static void Transform(ref Matrix3i matrix, ref Vector3d vector)
        {
            double x = (int)(matrix.R0C0 * vector.X + matrix.R0C1 * vector.Y + matrix.R0C2 * vector.Z);
            double y = (int)(matrix.R1C0 * vector.X + matrix.R1C1 * vector.Y + matrix.R1C2 * vector.Z);
            vector.Z = (int)(matrix.R2C0 * vector.X + matrix.R2C1 * vector.Y + matrix.R2C2 * vector.Z);
            vector.X = x;
            vector.Y = y;
        }
        public void Transform(ref Vector3d vector, out Vector3d result)
        {
            result.X = (int)(R0C0 * vector.X + R0C1 * vector.Y + R0C2 * vector.Z);
            result.Y = (int)(R1C0 * vector.X + R1C1 * vector.Y + R1C2 * vector.Z);
            result.Z = (int)(R2C0 * vector.X + R2C1 * vector.Y + R2C2 * vector.Z);
        }
        public static void Transform(ref Matrix3i matrix, ref Vector3d vector, out Vector3d result)
        {
            result.X = (int)(matrix.R0C0 * vector.X + matrix.R0C1 * vector.Y + matrix.R0C2 * vector.Z);
            result.Y = (int)(matrix.R1C0 * vector.X + matrix.R1C1 * vector.Y + matrix.R1C2 * vector.Z);
            result.Z = (int)(matrix.R2C0 * vector.X + matrix.R2C1 * vector.Y + matrix.R2C2 * vector.Z);
        }

        public void Rotate(int angle)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin =  System.Math.Sin(angleRadians);
            double cos =  System.Math.Cos(angleRadians);

            double r0c0 = cos * R0C0 + sin * R1C0;
            double r0c1 = cos * R0C1 + sin * R1C1;
            double r0c2 = cos * R0C2 + sin * R1C2;

            R1C0 = (int)(cos * R1C0 - sin * R0C0);
            R1C1 = (int)(cos * R1C1 - sin * R0C1);
            R1C2 = (int)(cos * R1C2 - sin * R0C2);

            R0C0 = (int)(r0c0);
            R0C1 = (int)(r0c1);
            R0C2 = (int)(r0c2);
        }
        public void Rotate(int angle, out Matrix3i result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos =  System.Math.Cos(angleRadians);

            result.R0C0 = (int)(cos * R0C0 + sin * R1C0);
            result.R0C1 = (int)(cos * R0C1 + sin * R1C1);
            result.R0C2 = (int)(cos * R0C2 + sin * R1C2);
            result.R1C0 = (int)(cos * R1C0 - sin * R0C0);
            result.R1C1 = (int)(cos * R1C1 - sin * R0C1);
            result.R1C2 = (int)(cos * R1C2 - sin * R0C2);
            result.R2C0 = (int)(R2C0);
            result.R2C1 = (int)(R2C1);
            result.R2C2 = (int)(R2C2);
        }
        public static void Rotate(ref Matrix3i matrix, int angle, out Matrix3i result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos = System.Math.Cos(angleRadians);

            result.R0C0 = (int)(cos * matrix.R0C0 + sin * matrix.R1C0);
            result.R0C1 = (int)(cos * matrix.R0C1 + sin * matrix.R1C1);
            result.R0C2 = (int)(cos * matrix.R0C2 + sin * matrix.R1C2);
            result.R1C0 = (int)(cos * matrix.R1C0 - sin * matrix.R0C0);
            result.R1C1 = (int)(cos * matrix.R1C1 - sin * matrix.R0C1);
            result.R1C2 = (int)(cos * matrix.R1C2 - sin * matrix.R0C2);
            result.R2C0 =(int)( matrix.R2C0);
            result.R2C1 = (int)(matrix.R2C1);
            result.R2C2 = (int)(matrix.R2C2);
        }
        public static void RotateMatrix(int angle, out Matrix3i result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos = System.Math.Cos(angleRadians);

            result.R0C0 =  (int)cos;
            result.R0C1 =  (int)sin;
            result.R0C2 =  (int)0;
            result.R1C0 =  (int)-sin;
            result.R1C1 =  (int)cos;
            result.R1C2 =  (int)0;
            result.R2C0 =  (int)0;
            result.R2C1 =  (int)0;
            result.R2C2 =  (int)1;
        }

        public Quaterniond ToQuaternion()
        {
            //return new Quaterniond(ref this);
            throw new NotImplementedException();
		}

        #endregion

        #region Constants

		
		/// <summary>
        /// Defines the size of the Matrix3i struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(new Matrix3i());

        /// <summary>The identity matrix.</summary>
        public static readonly Matrix3i Identity = new Matrix3i
        (
            1, 0, 0,
            0, 1, 0,
            0, 0, 1
        );

        /// <summary>A matrix of all zeros.</summary>
        public static readonly Matrix3i Zero = new Matrix3i
        (
            0, 0, 0,
            0, 0, 0,
            0, 0, 0
        );

        #endregion

        #region HashCode

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return
                R0C0.GetHashCode() ^ R0C1.GetHashCode() ^ R0C2.GetHashCode() ^
                R1C0.GetHashCode() ^ R1C1.GetHashCode() ^ R1C2.GetHashCode() ^
                R2C0.GetHashCode() ^ R2C1.GetHashCode() ^ R2C2.GetHashCode();
        }

        #endregion

        #region String

        /// <summary>Returns the fully qualified type name of this instance.</summary>
        /// <returns>A System.String containing left fully qualified type name.</returns>
        public override string ToString()
        {
            return String.Format(
                "|{00}, {01}, {02}|\n" +
                "|{03}, {04}, {05}|\n" +
                "|{06}, {07}, {18}|\n" +
                R0C0, R0C1, R0C2,
                R1C0, R1C1, R1C2,
                R2C0, R2C1, R2C2);
        }

        #endregion
    }

    /// <summary>
    /// Represents a 4x4 Matrix
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix4i : IEquatable<Matrix4i>
    {
        #region Fields

        /// <summary>
        /// Top row of the matrix
        /// </summary>
        public Vector4i Row0;
        /// <summary>
        /// 2nd row of the matrix
        /// </summary>
        public Vector4i Row1;
        /// <summary>
        /// 3rd row of the matrix
        /// </summary>
        public Vector4i Row2;
        /// <summary>
        /// Bottom row of the matrix
        /// </summary>
        public Vector4i Row3;
 
        /// <summary>
        /// The identity matrix
        /// </summary>
        public static Matrix4i Identity = new Matrix4i(Vector4i.UnitX, Vector4i.UnitY, Vector4i.UnitZ, Vector4i.UnitW);

		 /// <summary>
        /// The zero matrix
        /// </summary>
		public static Matrix4i Zero = new Matrix4i(Vector4i.Zero, Vector4i.Zero, Vector4i.Zero, Vector4i.Zero);


		/// <summary>
        /// Defines the size of the Matrix4i struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(new Matrix4i());

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="row0">Top row of the matrix</param>
        /// <param name="row1">Second row of the matrix</param>
        /// <param name="row2">Third row of the matrix</param>
        /// <param name="row3">Bottom row of the matrix</param>
        public Matrix4i(Vector4i row0, Vector4i row1, Vector4i row2, Vector4i row3)
        {
            Row0 = row0;
            Row1 = row1;
            Row2 = row2;
            Row3 = row3;
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="m00">First item of the first row of the matrix.</param>
        /// <param name="m01">Second item of the first row of the matrix.</param>
        /// <param name="m02">Third item of the first row of the matrix.</param>
        /// <param name="m03">Fourth item of the first row of the matrix.</param>
        /// <param name="m10">First item of the second row of the matrix.</param>
        /// <param name="m11">Second item of the second row of the matrix.</param>
        /// <param name="m12">Third item of the second row of the matrix.</param>
        /// <param name="m13">Fourth item of the second row of the matrix.</param>
        /// <param name="m20">First item of the third row of the matrix.</param>
        /// <param name="m21">Second item of the third row of the matrix.</param>
        /// <param name="m22">Third item of the third row of the matrix.</param>
        /// <param name="m23">First item of the third row of the matrix.</param>
        /// <param name="m30">Fourth item of the fourth row of the matrix.</param>
        /// <param name="m31">Second item of the fourth row of the matrix.</param>
        /// <param name="m32">Third item of the fourth row of the matrix.</param>
        /// <param name="m33">Fourth item of the fourth row of the matrix.</param>
        public Matrix4i(
            int m00, int m01, int m02, int m03,
            int m10, int m11, int m12, int m13,
            int m20, int m21, int m22, int m23,
            int m30, int m31, int m32, int m33)
        {
            Row0 = new Vector4i(m00, m01, m02, m03);
            Row1 = new Vector4i(m10, m11, m12, m13);
            Row2 = new Vector4i(m20, m21, m22, m23);
            Row3 = new Vector4i(m30, m31, m32, m33);
        }
		
		/// <summary>Constructs left matrix from the given array of int-precision floating-point numbers.</summary>
        /// <param name="intArray">The array of ints for the components of the matrix.</param>
        public Matrix4i(int[] intArray)
        {
            if (intArray == null || intArray.GetLength(0) < 16) throw new MissingFieldException();
			Row0 = new Vector4i(intArray[0], intArray[1], intArray[2], intArray[3]);
            Row1 = new Vector4i(intArray[4], intArray[5], intArray[6], intArray[7]);
            Row2 = new Vector4i(intArray[8], intArray[9], intArray[10], intArray[11]);
            Row3 = new Vector4i(intArray[12], intArray[13], intArray[14], intArray[15]);
        }
        
		/// <summary>Converts the matrix into an array of ints.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>An array of ints for the matrix in Column-major order.</returns>
        public static explicit operator int[](Matrix4i matrix)
        {
            return new int[16]
            {
                matrix.Row0.X,
                matrix.Row1.X,
                matrix.Row2.X,
                matrix.Row3.X,
                matrix.Row0.Y,
                matrix.Row1.Y,
                matrix.Row2.Y,
                matrix.Row3.Y,
                matrix.Row0.Z,
                matrix.Row1.Z,
                matrix.Row2.Z,
                matrix.Row3.Z,
                matrix.Row0.W,
                matrix.Row1.W,
                matrix.Row2.W,
                matrix.Row3.W
            };
        }
		
		#endregion

        #region Public Members

        #region Properties

        /// <summary>
        /// The determinant of this matrix
        /// </summary>
        public int Determinant
        {
            get
            {
                return
                    Row0.X * Row1.Y * Row2.Z * Row3.W - Row0.X * Row1.Y * Row2.W * Row3.Z + Row0.X * Row1.Z * Row2.W * Row3.Y - Row0.X * Row1.Z * Row2.Y * Row3.W
                  + Row0.X * Row1.W * Row2.Y * Row3.Z - Row0.X * Row1.W * Row2.Z * Row3.Y - Row0.Y * Row1.Z * Row2.W * Row3.X + Row0.Y * Row1.Z * Row2.X * Row3.W
                  - Row0.Y * Row1.W * Row2.X * Row3.Z + Row0.Y * Row1.W * Row2.Z * Row3.X - Row0.Y * Row1.X * Row2.Z * Row3.W + Row0.Y * Row1.X * Row2.W * Row3.Z
                  + Row0.Z * Row1.W * Row2.X * Row3.Y - Row0.Z * Row1.W * Row2.Y * Row3.X + Row0.Z * Row1.X * Row2.Y * Row3.W - Row0.Z * Row1.X * Row2.W * Row3.Y
                  + Row0.Z * Row1.Y * Row2.W * Row3.X - Row0.Z * Row1.Y * Row2.X * Row3.W - Row0.W * Row1.X * Row2.Y * Row3.Z + Row0.W * Row1.X * Row2.Z * Row3.Y
                  - Row0.W * Row1.Y * Row2.Z * Row3.X + Row0.W * Row1.Y * Row2.X * Row3.Z - Row0.W * Row1.Z * Row2.X * Row3.Y + Row0.W * Row1.Z * Row2.Y * Row3.X;
            }
        }

        /// <summary>
        /// The first column of this matrix
        /// </summary>
        public Vector4i Column0
        {
            get { return new Vector4i(Row0.X, Row1.X, Row2.X, Row3.X); }
        }

        /// <summary>
        /// The second column of this matrix
        /// </summary>
        public Vector4i Column1
        {
            get { return new Vector4i(Row0.Y, Row1.Y, Row2.Y, Row3.Y); }
        }

        /// <summary>
        /// The third column of this matrix
        /// </summary>
        public Vector4i Column2
        {
            get { return new Vector4i(Row0.Z, Row1.Z, Row2.Z, Row3.Z); }
        }

        /// <summary>
        /// The fourth column of this matrix
        /// </summary>
        public Vector4i Column3
        {
            get { return new Vector4i(Row0.W, Row1.W, Row2.W, Row3.W); }
        }

        /// <summary>
        /// Gets or sets the value at row 1, column 1 of this instance.
        /// </summary>
        public int M11 { get { return Row0.X; } set { Row0.X = value; } }

        /// <summary>
        /// Gets or sets the value at row 1, column 2 of this instance.
        /// </summary>
        public int M12 { get { return Row0.Y; } set { Row0.Y = value; } }

        /// <summary>
        /// Gets or sets the value at row 1, column 3 of this instance.
        /// </summary>
        public int M13 { get { return Row0.Z; } set { Row0.Z = value; } }

        /// <summary>
        /// Gets or sets the value at row 1, column 4 of this instance.
        /// </summary>
        public int M14 { get { return Row0.W; } set { Row0.W = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 1 of this instance.
        /// </summary>
        public int M21 { get { return Row1.X; } set { Row1.X = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 2 of this instance.
        /// </summary>
        public int M22 { get { return Row1.Y; } set { Row1.Y = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 3 of this instance.
        /// </summary>
        public int M23 { get { return Row1.Z; } set { Row1.Z = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 4 of this instance.
        /// </summary>
        public int M24 { get { return Row1.W; } set { Row1.W = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 1 of this instance.
        /// </summary>
        public int M31 { get { return Row2.X; } set { Row2.X = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 2 of this instance.
        /// </summary>
        public int M32 { get { return Row2.Y; } set { Row2.Y = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 3 of this instance.
        /// </summary>
        public int M33 { get { return Row2.Z; } set { Row2.Z = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 4 of this instance.
        /// </summary>
        public int M34 { get { return Row2.W; } set { Row2.W = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 1 of this instance.
        /// </summary>
        public int M41 { get { return Row3.X; } set { Row3.X = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 2 of this instance.
        /// </summary>
        public int M42 { get { return Row3.Y; } set { Row3.Y = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 3 of this instance.
        /// </summary>
        public int M43 { get { return Row3.Z; } set { Row3.Z = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 4 of this instance.
        /// </summary>
        public int M44 { get { return Row3.W; } set { Row3.W = value; } }

        #endregion

        #region Instance

        #region public void Invert()

        /// <summary>
        /// Converts this instance into its inverse.
        /// </summary>
        public void Invert()
        {
            this = Matrix4i.Invert(this);
        }

        #endregion

        #region public void Transpose()

        /// <summary>
        /// Converts this instance into its transpose.
        /// </summary>
        public void Transpose()
        {
            this = Matrix4i.Transpose(this);
        }

        #endregion

        #endregion

        #region Static
		                #region Multiply Functions

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <returns>A new instance that is the result of the multiplication</returns>
        public static Matrix4i Mult(Matrix4i left, Matrix4i right)
        {
            Matrix4i result;
            Mult(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <param name="result">A new instance that is the result of the multiplication</param>
        public static void Mult(ref Matrix4i left, ref Matrix4i right, out Matrix4i result)
        {
            result = new Matrix4i(
                right.M11 * left.M11 + right.M12 * left.M21 + right.M13 * left.M31 + right.M14 * left.M41,
                right.M11 * left.M12 + right.M12 * left.M22 + right.M13 * left.M32 + right.M14 * left.M42,
                right.M11 * left.M13 + right.M12 * left.M23 + right.M13 * left.M33 + right.M14 * left.M43,
                right.M11 * left.M14 + right.M12 * left.M24 + right.M13 * left.M34 + right.M14 * left.M44,
                right.M21 * left.M11 + right.M22 * left.M21 + right.M23 * left.M31 + right.M24 * left.M41,
                right.M21 * left.M12 + right.M22 * left.M22 + right.M23 * left.M32 + right.M24 * left.M42,
                right.M21 * left.M13 + right.M22 * left.M23 + right.M23 * left.M33 + right.M24 * left.M43,
                right.M21 * left.M14 + right.M22 * left.M24 + right.M23 * left.M34 + right.M24 * left.M44,
                right.M31 * left.M11 + right.M32 * left.M21 + right.M33 * left.M31 + right.M34 * left.M41,
                right.M31 * left.M12 + right.M32 * left.M22 + right.M33 * left.M32 + right.M34 * left.M42,
                right.M31 * left.M13 + right.M32 * left.M23 + right.M33 * left.M33 + right.M34 * left.M43,
                right.M31 * left.M14 + right.M32 * left.M24 + right.M33 * left.M34 + right.M34 * left.M44,
                right.M41 * left.M11 + right.M42 * left.M21 + right.M43 * left.M31 + right.M44 * left.M41,
                right.M41 * left.M12 + right.M42 * left.M22 + right.M43 * left.M32 + right.M44 * left.M42,
                right.M41 * left.M13 + right.M42 * left.M23 + right.M43 * left.M33 + right.M44 * left.M43,
                right.M41 * left.M14 + right.M42 * left.M24 + right.M43 * left.M34 + right.M44 * left.M44);
		}

        #endregion

        #region Invert Functions

        /// <summary>
        /// Calculate the inverse of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to invert</param>
        /// <returns>The inverse of the given matrix if it has one, or the input if it is singular</returns>
        /// <exception cref="InvalidOperationException">Thrown if the Matrix4i is singular.</exception>
        public static Matrix4i Invert(Matrix4i mat)
        {
            int[] colIdx = { 0, 0, 0, 0 };
            int[] rowIdx = { 0, 0, 0, 0 };
            int[] pivotIdx = { -1, -1, -1, -1 };

            // convert the matrix to an array for easy looping
            float[,] inverse = {{(float)mat.Row0.X, (float)mat.Row0.Y, (float)mat.Row0.Z, (float)mat.Row0.W}, 
                                {(float)mat.Row1.X, (float)mat.Row1.Y, (float)mat.Row1.Z, (float)mat.Row1.W}, 
                                {(float)mat.Row2.X, (float)mat.Row2.Y, (float)mat.Row2.Z, (float)mat.Row2.W}, 
                                {(float)mat.Row3.X, (float)mat.Row3.Y, (float)mat.Row3.Z, (float)mat.Row3.W} };
            int icol = 0;
            int irow = 0;
            for (int i = 0; i < 4; i++)
            {
                // Find the largest pivot value
                float maxPivot = 0;
                for (int j = 0; j < 4; j++)
                {
                    if (pivotIdx[j] != 0)
                    {
                        for (int k = 0; k < 4; ++k)
                        {
                            if (pivotIdx[k] == -1)
                            {
                               float absVal = System.Math.Abs(inverse[j, k]);
                                if (absVal > maxPivot)
                                {
                                    maxPivot = absVal;
                                    irow = j;
                                    icol = k;
                                }
                            }
                            else if (pivotIdx[k] > 0)
                            {
                                return mat;
                            }
                        }
                    }
                }

                ++(pivotIdx[icol]);

                // Swap rows over so pivot is on diagonal
                if (irow != icol)
                {
                    for (int k = 0; k < 4; ++k)
                    {
                        float f = inverse[irow, k];
                        inverse[irow, k] = inverse[icol, k];
                        inverse[icol, k] = f;
                    }
                }

                rowIdx[i] = irow;
                colIdx[i] = icol;

                float pivot = inverse[icol, icol];
                // check for singular matrix
                if (pivot == 0)
                {
                    throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
                    //return mat;
                }

                // Scale row so it has a unit diagonal
                float oneOverPivot = 1.0f / pivot;
                inverse[icol, icol] = 1.0f;
                for (int k = 0; k < 4; ++k)
                    inverse[icol, k] *= oneOverPivot;

                // Do elimination of non-diagonal elements
                for (int j = 0; j < 4; ++j)
                {
                    // check this isn't on the diagonal
                    if (icol != j)
                    {
                        float f = inverse[j, icol];
                        inverse[j, icol] = 0;
                        for (int k = 0; k < 4; ++k)
                            inverse[j, k] -= inverse[icol, k] * f;
                    }
                }
            }

            for (int j = 3; j >= 0; --j)
            {
                int ir = rowIdx[j];
                int ic = colIdx[j];
                for (int k = 0; k < 4; ++k)
                {
                    float f = inverse[k, ir];
                    inverse[k, ir] = inverse[k, ic];
                    inverse[k, ic] = f;
                }
            }

            mat.Row0 = new Vector4i((int)inverse[0, 0], (int)inverse[0, 1], (int)inverse[0, 2], (int)inverse[0, 3]);
            mat.Row1 = new Vector4i((int)inverse[1, 0], (int)inverse[1, 1], (int)inverse[1, 2], (int)inverse[1, 3]);
            mat.Row2 = new Vector4i((int)inverse[2, 0], (int)inverse[2, 1], (int)inverse[2, 2], (int)inverse[2, 3]);
            mat.Row3 = new Vector4i((int)inverse[3, 0], (int)inverse[3, 1], (int)inverse[3, 2], (int)inverse[3, 3]);
            return mat;
        }

        #endregion

        #region Transpose

        /// <summary>
        /// Calculate the transpose of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to transpose</param>
        /// <returns>The transpose of the given matrix</returns>
        public static Matrix4i Transpose(Matrix4i mat)
        {
            return new Matrix4i(mat.Column0, mat.Column1, mat.Column2, mat.Column3);
        }


        /// <summary>
        /// Calculate the transpose of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to transpose</param>
        /// <param name="result">The result of the calculation</param>
        public static void Transpose(ref Matrix4i mat, out Matrix4i result)
        {
            result.Row0 = mat.Column0;
            result.Row1 = mat.Column1;
            result.Row2 = mat.Column2;
            result.Row3 = mat.Column3;
        }

        #endregion

        #endregion

        #region Operators

        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new Matrix4i which holds the result of the multiplication</returns>
        public static Matrix4i operator *(Matrix4i left, Matrix4i right)
        {
            return Matrix4i.Mult(left, right);
        }

		/// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new Vector3i which holds the result of the multiplication</returns>
        public static Vector3i operator *(Matrix4i left, Vector3i right)
		{
			Vector3i r;

			double fInvW = 1.0 / (left.Row3.X * right.X + left.Row3.Y * right.Y + left.Row3.Z * right.Z + left.Row3.W);

			r.X =  (int)((left.Row0.X * right.X + left.Row0.Y * right.Y + left.Row0.Z * right.Z + left.Row0.W) * fInvW);
			r.Y =  (int)((left.Row1.X * right.X + left.Row1.Y * right.Y + left.Row1.Z * right.Z + left.Row1.W) * fInvW);
			r.Z =  (int)((left.Row2.X * right.X + left.Row2.Y * right.Y + left.Row2.Z * right.Z + left.Row2.W) * fInvW);

			return r;
		}
		
		/// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new Vector3i which holds the result of the multiplication</returns>
        public static Vector4i operator *(Matrix4i left, Vector4i right)
		{
			Vector4i r;

			double fInvW = 1.0 / (left.Row3.X * right.X + left.Row3.Y * right.Y + left.Row3.Z * right.Z + left.Row3.W);

			r.X =  (int)((left.Row0.X * right.X + left.Row0.Y * right.Y + left.Row0.Z * right.Z + left.Row0.W * right.W) * fInvW);
			r.Y =  (int)((left.Row1.X * right.X + left.Row1.Y * right.Y + left.Row1.Z * right.Z + left.Row1.W * right.W) * fInvW);
			r.Z =  (int)((left.Row2.X * right.X + left.Row2.Y * right.Y + left.Row2.Z * right.Z + left.Row2.W * right.W) * fInvW);
			r.W =  (int)((left.Row3.X * right.X + left.Row3.Y * right.Y + left.Row3.Z * right.Z + left.Row3.W * right.W) * fInvW);
			return r;
		}

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(Matrix4i left, Matrix4i right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(Matrix4i left, Matrix4i right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current Matrix4i4.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}\n{1}\n{2}\n{3}", Row0, Row1, Row2, Row3);
        }

        #endregion

        #region public override int GetHashCode()

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            return Row0.GetHashCode() ^ Row1.GetHashCode() ^ Row2.GetHashCode() ^ Row3.GetHashCode();
        }

        #endregion

        #region public override bool Equals(object obj)

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare tresult.</param>
        /// <returns>True if the instances are equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Matrix4i))
                return false;

            return this.Equals((Matrix4i)obj);
        }

        #endregion

        #endregion

        #endregion

        #region IEquatable<Matrix4i> Members

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="other">An matrix to compare with this matrix.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public bool Equals(Matrix4i other)
        {
            return
                Row0 == other.Row0 &&
                Row1 == other.Row1 &&
                Row2 == other.Row2 &&
                Row3 == other.Row3;
        }

        #endregion
    }

	[Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix2ui : IEquatable<Matrix2ui>
    {
        #region Fields & Access

        /// <summary>Row 0, Column 0</summary>
        public uint R0C0;

        /// <summary>Row 0, Column 1</summary>
        public uint R0C1;

        /// <summary>Row 1, Column 0</summary>
        public uint R1C0;

        /// <summary>Row 1, Column 1</summary>
        public uint R1C1;

        /// <summary>Gets the component at the given row and column in the matrix.</summary>
        /// <param name="row">The row of the matrix.</param>
        /// <param name="column">The column of the matrix.</param>
        /// <returns>The component at the given row and column in the matrix.</returns>
        public uint this[int row, int column]
        {
            get
            {
                switch( row )
                {
                    case 0:
                        switch (column)
                        {
                            case 0: return R0C0;
                            case 1: return R0C1;
                        }
                        break;

                    case 1:
                        switch (column)
                        {
                            case 0: return R1C0;
                            case 1: return R1C1;
                        }
                        break;
                }

                throw new IndexOutOfRangeException();
            }
            set
            {
                switch( row )
                {
                    case 0:
                        switch (column)
                        {
                            case 0: R0C0 = value; return;
                            case 1: R0C1 = value; return;
                        }
                        break;

                    case 1:
                        switch (column)
                        {
                            case 0: R1C0 = value; return;
                            case 1: R1C1 = value; return;
                        }
                        break;
                }

                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>Gets the component at the index into the matrix.</summary>
        /// <param name="index">The index into the components of the matrix.</param>
        /// <returns>The component at the given index into the matrix.</returns>
        public uint this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return R0C0;
                    case 1: return R0C1;
                    case 2: return R1C0;
                    case 3: return R1C1;
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0: R0C0 = value; return;
                    case 1: R0C1 = value; return;
                    case 2: R1C0 = value; return;
                    case 3: R1C1 = value; return;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>Converts the matrix into an IntPtr.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>An IntPtr for the matrix.</returns>
        public static explicit operator IntPtr(Matrix2ui matrix)
        {
            unsafe
            {
                return (IntPtr)(&matrix.R0C0);
            }
        }

        /// <summary>Converts the matrix into left uint*.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>A uint* for the matrix.</returns>
        [CLSCompliant(false)]
        unsafe public static explicit operator uint*(Matrix2ui matrix)
        {
            return &matrix.R0C0;
        }

        /// <summary>Converts the matrix into an array of uints.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>An array of uints for the matrix.</returns>
        public static explicit operator uint[](Matrix2ui matrix)
        {
            return new uint[4]
            {
                matrix.R0C0,
                matrix.R0C1,
                matrix.R1C0,
                matrix.R1C1
            };
        }

        #endregion

        #region Constructors

        /// <summary>Constructs left matrix with the same components as the given matrix.</summary>
        /// <param name="vector">The matrix whose components to copy.</param>
        public Matrix2ui(ref Matrix2ui matrix)
        {
            this.R0C0 = matrix.R0C0;
            this.R0C1 = matrix.R0C1;
            this.R1C0 = matrix.R1C0;
            this.R1C1 = matrix.R1C1;
        }

        /// <summary>Constructs left matrix with the given values.</summary>
        /// <param name="r0c0">The value for row 0 column 0.</param>
        /// <param name="r0c1">The value for row 0 column 1.</param>
        /// <param name="r1c0">The value for row 1 column 0.</param>
        /// <param name="r1c1">The value for row 1 column 1.</param>
        public Matrix2ui
        (
            uint r0c0,
            uint r0c1,
            uint r1c0,
            uint r1c1
        )
        {
            this.R0C0 = r0c0;
            this.R0C1 = r0c1;
            this.R1C0 = r1c0;
            this.R1C1 = r1c1;
        }

        /// <summary>Constructs left matrix from the given array of uint-precision floating-point numbers.</summary>
        /// <param name="uintArray">The array of uints for the components of the matrix in Column-major order.</param>
        public Matrix2ui(uint[] uintArray)
        {
            if (uintArray == null || uintArray.GetLength(0) < 4) throw new MissingFieldException();

            this.R0C0 = uintArray[0];
            this.R0C1 = uintArray[1];
            this.R1C0 = uintArray[2];
            this.R1C1 = uintArray[3];
        }

        #endregion

        #region Equality

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix3ui structure to compare with.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        [CLSCompliant(false)]
        public bool Equals(Matrix2ui matrix)
        {
            return
                R0C0 == matrix.R0C0 &&
                R0C1 == matrix.R0C1 &&
                R1C0 == matrix.R1C0 &&
                R1C1 == matrix.R1C1;
        }

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix3ui structure to compare to.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public bool Equals(ref Matrix2ui matrix)
        {
            return
                R0C0 == matrix.R0C0 &&
                R0C1 == matrix.R0C1 &&
                R1C0 == matrix.R1C0 &&
                R1C1 == matrix.R1C1;
        }

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="left">The left-hand operand.</param>
        /// <param name="right">The right-hand operand.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public static bool Equals(ref Matrix2ui left, ref Matrix2ui right)
        {
            return
                left.R0C0 == right.R0C0 &&
                left.R0C1 == right.R0C1 &&
                left.R1C0 == right.R1C0 &&
                left.R1C1 == right.R1C1;
        }

        /// <summary>Indicates whether the current matrix is approximately equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix2ui structure to compare with.</param>
        /// <param name="tolerance">The limit below which the matrices are considered equal.</param>
        /// <returns>true if the current matrix is approximately equal to the matrix parameter; otherwise, false.</returns>
        public bool EqualsApprox(ref Matrix2ui matrix, uint tolerance)
        {
            return
                System.Math.Abs(R0C0 - matrix.R0C0) <= tolerance &&
                System.Math.Abs(R0C1 - matrix.R0C1) <= tolerance &&
                System.Math.Abs(R1C0 - matrix.R1C0) <= tolerance &&
                System.Math.Abs(R1C1 - matrix.R1C1) <= tolerance;
        }

        /// <summary>Indicates whether the current matrix is approximately equal to another matrix.</summary>
        /// <param name="left">The left-hand operand.</param>
        /// <param name="right">The right-hand operand.</param>
        /// <param name="tolerance">The limit below which the matrices are considered equal.</param>
        /// <returns>true if the current matrix is approximately equal to the matrix parameter; otherwise, false.</returns>
        public static bool EqualsApprox(ref Matrix2ui left, ref Matrix2ui right, uint tolerance)
        {
            return
                System.Math.Abs(left.R0C0 - right.R0C0) <= tolerance &&
                System.Math.Abs(left.R0C1 - right.R0C1) <= tolerance &&
                System.Math.Abs(left.R1C0 - right.R1C0) <= tolerance &&
                System.Math.Abs(left.R1C1 - right.R1C1) <= tolerance;
        }

        #endregion

        #region Arithmetic Operators


        /// <summary>Add left matrix to this matrix.</summary>
        /// <param name="matrix">The matrix to add.</param>
        public void Add(ref Matrix2ui matrix)
        {
            R0C0 = R0C0 + matrix.R0C0;
            R0C1 = R0C1 + matrix.R0C1;
            R1C0 = R1C0 + matrix.R1C0;
            R1C1 = R1C1 + matrix.R1C1;
        }

        /// <summary>Add left matrix to this matrix.</summary>
        /// <param name="matrix">The matrix to add.</param>
        /// <param name="result">The resulting matrix of the addition.</param>
        public void Add(ref Matrix2ui matrix, out Matrix2ui result)
        {
            result.R0C0 = R0C0 + matrix.R0C0;
            result.R0C1 = R0C1 + matrix.R0C1;
            result.R1C0 = R1C0 + matrix.R1C0;
            result.R1C1 = R1C1 + matrix.R1C1;
        }

        /// <summary>Add left matrix to left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the addition.</param>
        public static void Add(ref Matrix2ui left, ref Matrix2ui right, out Matrix2ui result)
        {
            result.R0C0 = left.R0C0 + right.R0C0;
            result.R0C1 = left.R0C1 + right.R0C1;
            result.R1C0 = left.R1C0 + right.R1C0;
            result.R1C1 = left.R1C1 + right.R1C1;
        }


        /// <summary>Subtract left matrix from this matrix.</summary>
        /// <param name="matrix">The matrix to subtract.</param>
        public void Subtract(ref Matrix2ui matrix)
        {
            R0C0 = R0C0 + matrix.R0C0;
            R0C1 = R0C1 + matrix.R0C1;
            R1C0 = R1C0 + matrix.R1C0;
            R1C1 = R1C1 + matrix.R1C1;
        }

        /// <summary>Subtract left matrix from this matrix.</summary>
        /// <param name="matrix">The matrix to subtract.</param>
        /// <param name="result">The resulting matrix of the subtraction.</param>
        public void Subtract(ref Matrix2ui matrix, out Matrix2ui result)
        {
            result.R0C0 = R0C0 + matrix.R0C0;
            result.R0C1 = R0C1 + matrix.R0C1;
            result.R1C0 = R1C0 + matrix.R1C0;
            result.R1C1 = R1C1 + matrix.R1C1;
        }

        /// <summary>Subtract left matrix from left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the subtraction.</param>
        public static void Subtract(ref Matrix2ui left, ref Matrix2ui right, out Matrix2ui result)
        {
            result.R0C0 = left.R0C0 + right.R0C0;
            result.R0C1 = left.R0C1 + right.R0C1;
            result.R1C0 = left.R1C0 + right.R1C0;
            result.R1C1 = left.R1C1 + right.R1C1;
        }


        /// <summary>Multiply left martix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        public void Multiply(ref Matrix2ui matrix)
        {
            uint r0c0 = matrix.R0C0 * R0C0 + matrix.R0C1 * R1C0;
            uint r0c1 = matrix.R0C0 * R0C1 + matrix.R0C1 * R1C1;

            uint r1c0 = matrix.R1C0 * R0C0 + matrix.R1C1 * R1C0;
            uint r1c1 = matrix.R1C0 * R0C1 + matrix.R1C1 * R1C1;

            R0C0 = r0c0;
            R0C1 = r0c1;

            R1C0 = r1c0;
            R1C1 = r1c1;
        }

        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public void Multiply(ref Matrix2ui matrix, out Matrix2ui result)
        {
            result.R0C0 = matrix.R0C0 * R0C0 + matrix.R0C1 * R1C0;
            result.R0C1 = matrix.R0C0 * R0C1 + matrix.R0C1 * R1C1;
            result.R1C0 = matrix.R1C0 * R0C0 + matrix.R1C1 * R1C0;
            result.R1C1 = matrix.R1C0 * R0C1 + matrix.R1C1 * R1C1;
        }

        /// <summary>Multiply left matrix times left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public static void Multiply(ref Matrix2ui left, ref Matrix2ui right, out Matrix2ui result)
        {
            result.R0C0 = right.R0C0 * left.R0C0 + right.R0C1 * left.R1C0;
            result.R0C1 = right.R0C0 * left.R0C1 + right.R0C1 * left.R1C1;
            result.R1C0 = right.R1C0 * left.R0C0 + right.R1C1 * left.R1C0;
            result.R1C1 = right.R1C0 * left.R0C1 + right.R1C1 * left.R1C1;
        }


        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        public void Multiply(uint scalar)
        {
            R0C0 = scalar * R0C0;
            R0C1 = scalar * R0C1;
            R1C0 = scalar * R1C0;
            R1C1 = scalar * R1C1;
        }

        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public void Multiply(uint scalar, out Matrix2ui result)
        {
            result.R0C0 = scalar * R0C0;
            result.R0C1 = scalar * R0C1;
            result.R1C0 = scalar * R1C0;
            result.R1C1 = scalar * R1C1;
        }

        /// <summary>Multiply left matrix times left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public static void Multiply(ref Matrix3ui matrix, uint scalar, out Matrix2ui result)
        {
            result.R0C0 = scalar * matrix.R0C0;
            result.R0C1 = scalar * matrix.R0C1;
            result.R1C0 = scalar * matrix.R1C0;
            result.R1C1 = scalar * matrix.R1C1;
        }


        #endregion

        #region Functions

        public uint Determinant
        {
            get
            {
                return R0C0 * R1C1 - R0C1 * R1C0;
            }
        }

        public void Transpose()
        {
            Std.Swap(ref R0C1, ref R1C0);
        }
        public void Transpose(out Matrix2ui result)
        {
            result.R0C0 = R0C0;
            result.R0C1 = R1C0;
            result.R1C0 = R0C1;
            result.R1C1 = R1C1;
        }
        public static void Transpose(ref Matrix2ui matrix, out Matrix2ui result)
        {
            result.R0C0 = matrix.R0C0;
            result.R0C1 = matrix.R1C0;
            result.R1C0 = matrix.R0C1;
            result.R1C1 = matrix.R1C1;
        }

        #endregion

        #region Transformation Functions

        public void Transform(ref Vector2d vector)
        {
            vector.X = R0C0 * vector.X + R0C1 * vector.Y;
            vector.Y = R1C0 * vector.X + R1C1 * vector.Y;
        }
        public static void Transform(ref Matrix2ui matrix, ref Vector2d vector)
        {
            vector.X = matrix.R0C0 * vector.X + matrix.R0C1 * vector.Y;
            vector.Y = matrix.R1C0 * vector.X + matrix.R1C1 * vector.Y;
        }
        public void Transform(ref Vector2d vector, out Vector2d result)
        {
            result.X = R0C0 * vector.X + R0C1 * vector.Y;
            result.Y = R1C0 * vector.X + R1C1 * vector.Y;
        }
        public static void Transform(ref Matrix2ui matrix, ref Vector2d vector, out Vector2d result)
        {
            result.X = matrix.R0C0 * vector.X + matrix.R0C1 * vector.Y;
            result.Y = matrix.R1C0 * vector.X + matrix.R1C1 * vector.Y;
        }

        public void Rotate(uint angle)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin =  System.Math.Sin(angleRadians);
            double cos =  System.Math.Cos(angleRadians);

            R1C0 = (uint)(cos * R1C0 - sin * R0C0);
            R1C1 = (uint)(cos * R1C1 - sin * R0C1);

            R0C0 = (uint)(cos * R0C0 + sin * R1C0);
            R0C1 = (uint)(cos * R0C1 + sin * R1C1);
        }
        public void Rotate(uint angle, out Matrix2ui result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos =  System.Math.Cos(angleRadians);

            result.R0C0 = (uint)(cos * R0C0 + sin * R1C0);
            result.R0C1 = (uint)(cos * R0C1 + sin * R1C1);
            result.R1C0 = (uint)(cos * R1C0 - sin * R0C0);
            result.R1C1 = (uint)(cos * R1C1 - sin * R0C1);
        }
        public static void Rotate(ref Matrix2ui matrix, uint angle, out Matrix2ui result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos = System.Math.Cos(angleRadians);

            result.R0C0 = (uint)(cos * matrix.R0C0 + sin * matrix.R1C0);
            result.R0C1 = (uint)(cos * matrix.R0C1 + sin * matrix.R1C1);
            result.R1C0 = (uint)(cos * matrix.R1C0 - sin * matrix.R0C0);
            result.R1C1 = (uint)(cos * matrix.R1C1 - sin * matrix.R0C1);
        }
        public static void RotateMatrix(uint angle, out Matrix2ui result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos = System.Math.Cos(angleRadians);

            result.R0C0 =  (uint)cos;
            result.R0C1 =  (uint)sin;
            result.R1C0 =  (uint)-sin;
            result.R1C1 =  (uint)cos;
        }

        #endregion

        #region Constants

		/// <summary>
        /// Defines the size of the Matrix2ui struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(new Matrix2ui());

        /// <summary>The identity matrix.</summary>
        public static readonly Matrix2ui Identity = new Matrix2ui
        (
            1, 0,
            0, 1
        );

        /// <summary>A matrix of all zeros.</summary>
        public static readonly Matrix2ui Zero = new Matrix2ui
        (
            0, 0,
            0, 0
        );

        #endregion

        #region HashCode

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return
                R0C0.GetHashCode() ^ R0C1.GetHashCode() ^
                R1C0.GetHashCode() ^ R1C1.GetHashCode();
        }

        #endregion

        #region String

        /// <summary>Returns the fully qualified type name of this instance.</summary>
        /// <returns>A System.String containing left fully qualified type name.</returns>
        public override string ToString()
        {
            return String.Format(
                "|{00}, {01}|\n" +
                "|{02}, {03}|\n" +
                R0C0, R0C1, 
                R1C0, R1C1);
        }

        #endregion
    }

	[Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3ui : IEquatable<Matrix3ui>
    {
        #region Fields & Access

        /// <summary>Row 0, Column 0</summary>
        public uint R0C0;

        /// <summary>Row 0, Column 1</summary>
        public uint R0C1;

        /// <summary>Row 0, Column 2</summary>
        public uint R0C2;

        /// <summary>Row 1, Column 0</summary>
        public uint R1C0;

        /// <summary>Row 1, Column 1</summary>
        public uint R1C1;

        /// <summary>Row 1, Column 2</summary>
        public uint R1C2;

        /// <summary>Row 2, Column 0</summary>
        public uint R2C0;

        /// <summary>Row 2, Column 1</summary>
        public uint R2C1;

        /// <summary>Row 2, Column 2</summary>
        public uint R2C2;

        /// <summary>Gets the component at the given row and column in the matrix.</summary>
        /// <param name="row">The row of the matrix.</param>
        /// <param name="column">The column of the matrix.</param>
        /// <returns>The component at the given row and column in the matrix.</returns>
        public uint this[int row, int column]
        {
            get
            {
                switch( row )
                {
                    case 0:
                        switch (column)
                        {
                            case 0: return R0C0;
                            case 1: return R0C1;
                            case 2: return R0C2;
                        }
                        break;

                    case 1:
                        switch (column)
                        {
                            case 0: return R1C0;
                            case 1: return R1C1;
                            case 2: return R1C2;
                        }
                        break;

                    case 2:
                        switch (column)
                        {
                            case 0: return R2C0;
                            case 1: return R2C1;
                            case 2: return R2C2;
                        }
                        break;
                }

                throw new IndexOutOfRangeException();
            }
            set
            {
                switch( row )
                {
                    case 0:
                        switch (column)
                        {
                            case 0: R0C0 = value; return;
                            case 1: R0C1 = value; return;
                            case 2: R0C2 = value; return;
                        }
                        break;

                    case 1:
                        switch (column)
                        {
                            case 0: R1C0 = value; return;
                            case 1: R1C1 = value; return;
                            case 2: R1C2 = value; return;
                        }
                        break;

                    case 2:
                        switch (column)
                        {
                            case 0: R2C0 = value; return;
                            case 1: R2C1 = value; return;
                            case 2: R2C2 = value; return;
                        }
                        break;
                }

                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>Gets the component at the index into the matrix.</summary>
        /// <param name="index">The index into the components of the matrix.</param>
        /// <returns>The component at the given index into the matrix.</returns>
        public uint this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return R0C0;
                    case 1: return R0C1;
                    case 2: return R0C2;
                    case 3: return R1C0;
                    case 4: return R1C1;
                    case 5: return R1C2;
                    case 6: return R2C0;
                    case 7: return R2C1;
                    case 8: return R2C2;
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0: R0C0 = value; return;
                    case 1: R0C1 = value; return;
                    case 2: R0C2 = value; return;
                    case 3: R1C0 = value; return;
                    case 4: R1C1 = value; return;
                    case 5: R1C2 = value; return;
                    case 6: R2C0 = value; return;
                    case 7: R2C1 = value; return;
                    case 8: R2C2 = value; return;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>Converts the matrix into an IntPtr.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>An IntPtr for the matrix.</returns>
        public static explicit operator IntPtr(Matrix3ui matrix)
        {
            unsafe
            {
                return (IntPtr)(&matrix.R0C0);
            }
        }

        /// <summary>Converts the matrix into left uint*.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>A uint* for the matrix.</returns>
        [CLSCompliant(false)]
        unsafe public static explicit operator uint*(Matrix3ui matrix)
        {
            return &matrix.R0C0;
        }

        /// <summary>Converts the matrix into an array of uints.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>An array of uints for the matrix.</returns>
        public static explicit operator uint[](Matrix3ui matrix)
        {
            return new uint[9]
            {
                matrix.R0C0,
                matrix.R0C1,
                matrix.R0C2,
                matrix.R1C0,
                matrix.R1C1,
                matrix.R1C2,
                matrix.R2C0,
                matrix.R2C1,
                matrix.R2C2
            };
        }

        #endregion

        #region Constructors

        /// <summary>Constructs left matrix with the same components as the given matrix.</summary>
        /// <param name="vector">The matrix whose components to copy.</param>
        public Matrix3ui(ref Matrix3ui matrix)
        {
            this.R0C0 = matrix.R0C0;
            this.R0C1 = matrix.R0C1;
            this.R0C2 = matrix.R0C2;
            this.R1C0 = matrix.R1C0;
            this.R1C1 = matrix.R1C1;
            this.R1C2 = matrix.R1C2;
            this.R2C0 = matrix.R2C0;
            this.R2C1 = matrix.R2C1;
            this.R2C2 = matrix.R2C2;
        }

        /// <summary>Constructs left matrix with the given values.</summary>
        /// <param name="r0c0">The value for row 0 column 0.</param>
        /// <param name="r0c1">The value for row 0 column 1.</param>
        /// <param name="r0c2">The value for row 0 column 2.</param>
        /// <param name="r1c0">The value for row 1 column 0.</param>
        /// <param name="r1c1">The value for row 1 column 1.</param>
        /// <param name="r1c2">The value for row 1 column 2.</param>
        /// <param name="r2c0">The value for row 2 column 0.</param>
        /// <param name="r2c1">The value for row 2 column 1.</param>
        /// <param name="r2c2">The value for row 2 column 2.</param>
        public Matrix3ui
        (
            uint r0c0,
            uint r0c1,
            uint r0c2,
            uint r1c0,
            uint r1c1,
            uint r1c2,
            uint r2c0,
            uint r2c1,
            uint r2c2
        )
        {
            this.R0C0 = r0c0;
            this.R0C1 = r0c1;
            this.R0C2 = r0c2;
            this.R1C0 = r1c0;
            this.R1C1 = r1c1;
            this.R1C2 = r1c2;
            this.R2C0 = r2c0;
            this.R2C1 = r2c1;
            this.R2C2 = r2c2;
        }

        /// <summary>Constructs left matrix from the given array of uint-precision floating-point numbers.</summary>
        /// <param name="uintArray">The array of uints for the components of the matrix in Column-major order.</param>
        public Matrix3ui(uint[] uintArray)
        {
            if (uintArray == null || uintArray.GetLength(0) < 9) throw new MissingFieldException();

            this.R0C0 = uintArray[0];
            this.R0C1 = uintArray[1];
            this.R0C2 = uintArray[2];
            this.R1C0 = uintArray[3];
            this.R1C1 = uintArray[4];
            this.R1C2 = uintArray[5];
            this.R2C0 = uintArray[6];
            this.R2C1 = uintArray[7];
            this.R2C2 = uintArray[8];
        }

        /// <summary>Constructs left matrix from the given quaternion.</summary>
        /// <param name="quaternion">The quaternion to use to construct the martix.</param>
        public Matrix3ui(Quaterniond quaternion)
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

            R0C0 = (uint)(1 - 2 * (yy + zz));
            R0C1 = (uint)(2 * (xy - wz));
            R0C2 = (uint)(2 * (xz + wy));

            R1C0 = (uint)(2 * (xy + wz));
            R1C1 = (uint)(1 - 2 * (xx + zz));
            R1C2 = (uint)(2 * (yz - wx));

            R2C0 = (uint)(2 * (xz - wy));
            R2C1 = (uint)(2 * (yz + wx));
            R2C2 = (uint)(1 - 2 * (xx + yy));
        }

        #endregion

        #region Equality

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix3ui structure to compare with.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        [CLSCompliant(false)]
        public bool Equals(Matrix3ui matrix)
        {
            return
                R0C0 == matrix.R0C0 &&
                R0C1 == matrix.R0C1 &&
                R0C2 == matrix.R0C2 &&
                R1C0 == matrix.R1C0 &&
                R1C1 == matrix.R1C1 &&
                R1C2 == matrix.R1C2 &&
                R2C0 == matrix.R2C0 &&
                R2C1 == matrix.R2C1 &&
                R2C2 == matrix.R2C2;
        }

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix3ui structure to compare to.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public bool Equals(ref Matrix3ui matrix)
        {
            return
                R0C0 == matrix.R0C0 &&
                R0C1 == matrix.R0C1 &&
                R0C2 == matrix.R0C2 &&
                R1C0 == matrix.R1C0 &&
                R1C1 == matrix.R1C1 &&
                R1C2 == matrix.R1C2 &&
                R2C0 == matrix.R2C0 &&
                R2C1 == matrix.R2C1 &&
                R2C2 == matrix.R2C2;
        }

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="left">The left-hand operand.</param>
        /// <param name="right">The right-hand operand.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public static bool Equals(ref Matrix3ui left, ref Matrix3ui right)
        {
            return
                left.R0C0 == right.R0C0 &&
                left.R0C1 == right.R0C1 &&
                left.R0C2 == right.R0C2 &&
                left.R1C0 == right.R1C0 &&
                left.R1C1 == right.R1C1 &&
                left.R1C2 == right.R1C2 &&
                left.R2C0 == right.R2C0 &&
                left.R2C1 == right.R2C1 &&
                left.R2C2 == right.R2C2;
        }

        /// <summary>Indicates whether the current matrix is approximately equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix3ui structure to compare with.</param>
        /// <param name="tolerance">The limit below which the matrices are considered equal.</param>
        /// <returns>true if the current matrix is approximately equal to the matrix parameter; otherwise, false.</returns>
        public bool EqualsApprox(ref Matrix3ui matrix, uint tolerance)
        {
            return
                System.Math.Abs(R0C0 - matrix.R0C0) <= tolerance &&
                System.Math.Abs(R0C1 - matrix.R0C1) <= tolerance &&
                System.Math.Abs(R0C2 - matrix.R0C2) <= tolerance &&
                System.Math.Abs(R1C0 - matrix.R1C0) <= tolerance &&
                System.Math.Abs(R1C1 - matrix.R1C1) <= tolerance &&
                System.Math.Abs(R1C2 - matrix.R1C2) <= tolerance &&
                System.Math.Abs(R2C0 - matrix.R2C0) <= tolerance &&
                System.Math.Abs(R2C1 - matrix.R2C1) <= tolerance &&
                System.Math.Abs(R2C2 - matrix.R2C2) <= tolerance;
        }

        /// <summary>Indicates whether the current matrix is approximately equal to another matrix.</summary>
        /// <param name="left">The left-hand operand.</param>
        /// <param name="right">The right-hand operand.</param>
        /// <param name="tolerance">The limit below which the matrices are considered equal.</param>
        /// <returns>true if the current matrix is approximately equal to the matrix parameter; otherwise, false.</returns>
        public static bool EqualsApprox(ref Matrix3ui left, ref Matrix3ui right, uint tolerance)
        {
            return
                System.Math.Abs(left.R0C0 - right.R0C0) <= tolerance &&
                System.Math.Abs(left.R0C1 - right.R0C1) <= tolerance &&
                System.Math.Abs(left.R0C2 - right.R0C2) <= tolerance &&
                System.Math.Abs(left.R1C0 - right.R1C0) <= tolerance &&
                System.Math.Abs(left.R1C1 - right.R1C1) <= tolerance &&
                System.Math.Abs(left.R1C2 - right.R1C2) <= tolerance &&
                System.Math.Abs(left.R2C0 - right.R2C0) <= tolerance &&
                System.Math.Abs(left.R2C1 - right.R2C1) <= tolerance &&
                System.Math.Abs(left.R2C2 - right.R2C2) <= tolerance;
        }

        #endregion

        #region Arithmetic Operators


        /// <summary>Add left matrix to this matrix.</summary>
        /// <param name="matrix">The matrix to add.</param>
        public void Add(ref Matrix3ui matrix)
        {
            R0C0 = R0C0 + matrix.R0C0;
            R0C1 = R0C1 + matrix.R0C1;
            R0C2 = R0C2 + matrix.R0C2;
            R1C0 = R1C0 + matrix.R1C0;
            R1C1 = R1C1 + matrix.R1C1;
            R1C2 = R1C2 + matrix.R1C2;
            R2C0 = R2C0 + matrix.R2C0;
            R2C1 = R2C1 + matrix.R2C1;
            R2C2 = R2C2 + matrix.R2C2;
        }

        /// <summary>Add left matrix to this matrix.</summary>
        /// <param name="matrix">The matrix to add.</param>
        /// <param name="result">The resulting matrix of the addition.</param>
        public void Add(ref Matrix3ui matrix, out Matrix3ui result)
        {
            result.R0C0 = R0C0 + matrix.R0C0;
            result.R0C1 = R0C1 + matrix.R0C1;
            result.R0C2 = R0C2 + matrix.R0C2;
            result.R1C0 = R1C0 + matrix.R1C0;
            result.R1C1 = R1C1 + matrix.R1C1;
            result.R1C2 = R1C2 + matrix.R1C2;
            result.R2C0 = R2C0 + matrix.R2C0;
            result.R2C1 = R2C1 + matrix.R2C1;
            result.R2C2 = R2C2 + matrix.R2C2;
        }

        /// <summary>Add left matrix to left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the addition.</param>
        public static void Add(ref Matrix3ui left, ref Matrix3ui right, out Matrix3ui result)
        {
            result.R0C0 = left.R0C0 + right.R0C0;
            result.R0C1 = left.R0C1 + right.R0C1;
            result.R0C2 = left.R0C2 + right.R0C2;
            result.R1C0 = left.R1C0 + right.R1C0;
            result.R1C1 = left.R1C1 + right.R1C1;
            result.R1C2 = left.R1C2 + right.R1C2;
            result.R2C0 = left.R2C0 + right.R2C0;
            result.R2C1 = left.R2C1 + right.R2C1;
            result.R2C2 = left.R2C2 + right.R2C2;
        }


        /// <summary>Subtract left matrix from this matrix.</summary>
        /// <param name="matrix">The matrix to subtract.</param>
        public void Subtract(ref Matrix3ui matrix)
        {
            R0C0 = R0C0 + matrix.R0C0;
            R0C1 = R0C1 + matrix.R0C1;
            R0C2 = R0C2 + matrix.R0C2;
            R1C0 = R1C0 + matrix.R1C0;
            R1C1 = R1C1 + matrix.R1C1;
            R1C2 = R1C2 + matrix.R1C2;
            R2C0 = R2C0 + matrix.R2C0;
            R2C1 = R2C1 + matrix.R2C1;
            R2C2 = R2C2 + matrix.R2C2;
        }

        /// <summary>Subtract left matrix from this matrix.</summary>
        /// <param name="matrix">The matrix to subtract.</param>
        /// <param name="result">The resulting matrix of the subtraction.</param>
        public void Subtract(ref Matrix3ui matrix, out Matrix3ui result)
        {
            result.R0C0 = R0C0 + matrix.R0C0;
            result.R0C1 = R0C1 + matrix.R0C1;
            result.R0C2 = R0C2 + matrix.R0C2;
            result.R1C0 = R1C0 + matrix.R1C0;
            result.R1C1 = R1C1 + matrix.R1C1;
            result.R1C2 = R1C2 + matrix.R1C2;
            result.R2C0 = R2C0 + matrix.R2C0;
            result.R2C1 = R2C1 + matrix.R2C1;
            result.R2C2 = R2C2 + matrix.R2C2;
        }

        /// <summary>Subtract left matrix from left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the subtraction.</param>
        public static void Subtract(ref Matrix3ui left, ref Matrix3ui right, out Matrix3ui result)
        {
            result.R0C0 = left.R0C0 + right.R0C0;
            result.R0C1 = left.R0C1 + right.R0C1;
            result.R0C2 = left.R0C2 + right.R0C2;
            result.R1C0 = left.R1C0 + right.R1C0;
            result.R1C1 = left.R1C1 + right.R1C1;
            result.R1C2 = left.R1C2 + right.R1C2;
            result.R2C0 = left.R2C0 + right.R2C0;
            result.R2C1 = left.R2C1 + right.R2C1;
            result.R2C2 = left.R2C2 + right.R2C2;
        }


        /// <summary>Multiply left martix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        public void Multiply(ref Matrix3ui matrix)
        {
            uint r0c0 = matrix.R0C0 * R0C0 + matrix.R0C1 * R1C0 + matrix.R0C2 * R2C0;
            uint r0c1 = matrix.R0C0 * R0C1 + matrix.R0C1 * R1C1 + matrix.R0C2 * R2C1;
            uint r0c2 = matrix.R0C0 * R0C2 + matrix.R0C1 * R1C2 + matrix.R0C2 * R2C2;

            uint r1c0 = matrix.R1C0 * R0C0 + matrix.R1C1 * R1C0 + matrix.R1C2 * R2C0;
            uint r1c1 = matrix.R1C0 * R0C1 + matrix.R1C1 * R1C1 + matrix.R1C2 * R2C1;
            uint r1c2 = matrix.R1C0 * R0C2 + matrix.R1C1 * R1C2 + matrix.R1C2 * R2C2;

            R2C0 = matrix.R2C0 * R0C0 + matrix.R2C1 * R1C0 + matrix.R2C2 * R2C0;
            R2C1 = matrix.R2C0 * R0C1 + matrix.R2C1 * R1C1 + matrix.R2C2 * R2C1;
            R2C2 = matrix.R2C0 * R0C2 + matrix.R2C1 * R1C2 + matrix.R2C2 * R2C2;


            R0C0 = r0c0;
            R0C1 = r0c1;
            R0C2 = r0c2;

            R1C0 = r1c0;
            R1C1 = r1c1;
            R1C2 = r1c2;
        }

        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public void Multiply(ref Matrix3ui matrix, out Matrix3ui result)
        {
            result.R0C0 = matrix.R0C0 * R0C0 + matrix.R0C1 * R1C0 + matrix.R0C2 * R2C0;
            result.R0C1 = matrix.R0C0 * R0C1 + matrix.R0C1 * R1C1 + matrix.R0C2 * R2C1;
            result.R0C2 = matrix.R0C0 * R0C2 + matrix.R0C1 * R1C2 + matrix.R0C2 * R2C2;
            result.R1C0 = matrix.R1C0 * R0C0 + matrix.R1C1 * R1C0 + matrix.R1C2 * R2C0;
            result.R1C1 = matrix.R1C0 * R0C1 + matrix.R1C1 * R1C1 + matrix.R1C2 * R2C1;
            result.R1C2 = matrix.R1C0 * R0C2 + matrix.R1C1 * R1C2 + matrix.R1C2 * R2C2;
            result.R2C0 = matrix.R2C0 * R0C0 + matrix.R2C1 * R1C0 + matrix.R2C2 * R2C0;
            result.R2C1 = matrix.R2C0 * R0C1 + matrix.R2C1 * R1C1 + matrix.R2C2 * R2C1;
            result.R2C2 = matrix.R2C0 * R0C2 + matrix.R2C1 * R1C2 + matrix.R2C2 * R2C2;
        }

        /// <summary>Multiply left matrix times left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public static void Multiply(ref Matrix3ui left, ref Matrix3ui right, out Matrix3ui result)
        {
            result.R0C0 = right.R0C0 * left.R0C0 + right.R0C1 * left.R1C0 + right.R0C2 * left.R2C0;
            result.R0C1 = right.R0C0 * left.R0C1 + right.R0C1 * left.R1C1 + right.R0C2 * left.R2C1;
            result.R0C2 = right.R0C0 * left.R0C2 + right.R0C1 * left.R1C2 + right.R0C2 * left.R2C2;
            result.R1C0 = right.R1C0 * left.R0C0 + right.R1C1 * left.R1C0 + right.R1C2 * left.R2C0;
            result.R1C1 = right.R1C0 * left.R0C1 + right.R1C1 * left.R1C1 + right.R1C2 * left.R2C1;
            result.R1C2 = right.R1C0 * left.R0C2 + right.R1C1 * left.R1C2 + right.R1C2 * left.R2C2;
            result.R2C0 = right.R2C0 * left.R0C0 + right.R2C1 * left.R1C0 + right.R2C2 * left.R2C0;
            result.R2C1 = right.R2C0 * left.R0C1 + right.R2C1 * left.R1C1 + right.R2C2 * left.R2C1;
            result.R2C2 = right.R2C0 * left.R0C2 + right.R2C1 * left.R1C2 + right.R2C2 * left.R2C2;
        }


        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        public void Multiply(uint scalar)
        {
            R0C0 = scalar * R0C0;
            R0C1 = scalar * R0C1;
            R0C2 = scalar * R0C2;
            R1C0 = scalar * R1C0;
            R1C1 = scalar * R1C1;
            R1C2 = scalar * R1C2;
            R2C0 = scalar * R2C0;
            R2C1 = scalar * R2C1;
            R2C2 = scalar * R2C2;
        }

        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public void Multiply(uint scalar, out Matrix3ui result)
        {
            result.R0C0 = scalar * R0C0;
            result.R0C1 = scalar * R0C1;
            result.R0C2 = scalar * R0C2;
            result.R1C0 = scalar * R1C0;
            result.R1C1 = scalar * R1C1;
            result.R1C2 = scalar * R1C2;
            result.R2C0 = scalar * R2C0;
            result.R2C1 = scalar * R2C1;
            result.R2C2 = scalar * R2C2;
        }

        /// <summary>Multiply left matrix times left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public static void Multiply(ref Matrix3ui matrix, uint scalar, out Matrix3ui result)
        {
            result.R0C0 = scalar * matrix.R0C0;
            result.R0C1 = scalar * matrix.R0C1;
            result.R0C2 = scalar * matrix.R0C2;
            result.R1C0 = scalar * matrix.R1C0;
            result.R1C1 = scalar * matrix.R1C1;
            result.R1C2 = scalar * matrix.R1C2;
            result.R2C0 = scalar * matrix.R2C0;
            result.R2C1 = scalar * matrix.R2C1;
            result.R2C2 = scalar * matrix.R2C2;
        }


        #endregion

        #region Functions

        public uint Determinant
        {
            get
            {
                return R0C0 * R1C1 * R2C2 - R0C0 * R1C2 * R2C1 - R0C1 * R1C0 * R2C2 + R0C2 * R1C0 * R2C1 + R0C1 * R1C2 * R2C0 - R0C2 * R1C1 * R2C0;
            }
        }

        public void Transpose()
        {
            Std.Swap(ref R0C1, ref R1C0);
            Std.Swap(ref R0C2, ref R2C0);
            Std.Swap(ref R1C2, ref R2C1);
        }
        public void Transpose(out Matrix3ui result)
        {
            result.R0C0 = R0C0;
            result.R0C1 = R1C0;
            result.R0C2 = R2C0;
            result.R1C0 = R0C1;
            result.R1C1 = R1C1;
            result.R1C2 = R2C1;
            result.R2C0 = R0C2;
            result.R2C1 = R1C2;
            result.R2C2 = R2C2;
        }
        public static void Transpose(ref Matrix3ui matrix, out Matrix3ui result)
        {
            result.R0C0 = matrix.R0C0;
            result.R0C1 = matrix.R1C0;
            result.R0C2 = matrix.R2C0;
            result.R1C0 = matrix.R0C1;
            result.R1C1 = matrix.R1C1;
            result.R1C2 = matrix.R2C1;
            result.R2C0 = matrix.R0C2;
            result.R2C1 = matrix.R1C2;
            result.R2C2 = matrix.R2C2;
        }

        #endregion

        #region Transformation Functions

        public void Transform(ref Vector3d vector)
        {
            double x = R0C0 * vector.X + R0C1 * vector.Y + R0C2 * vector.Z;
            double y = R1C0 * vector.X + R1C1 * vector.Y + R1C2 * vector.Z;
            vector.Z = (uint)(R2C0 * vector.X + R2C1 * vector.Y + R2C2 * vector.Z);
            vector.X = x;
            vector.Y = y;
        }
        public static void Transform(ref Matrix3ui matrix, ref Vector3d vector)
        {
            double x = (uint)(matrix.R0C0 * vector.X + matrix.R0C1 * vector.Y + matrix.R0C2 * vector.Z);
            double y = (uint)(matrix.R1C0 * vector.X + matrix.R1C1 * vector.Y + matrix.R1C2 * vector.Z);
            vector.Z = (uint)(matrix.R2C0 * vector.X + matrix.R2C1 * vector.Y + matrix.R2C2 * vector.Z);
            vector.X = x;
            vector.Y = y;
        }
        public void Transform(ref Vector3d vector, out Vector3d result)
        {
            result.X = (uint)(R0C0 * vector.X + R0C1 * vector.Y + R0C2 * vector.Z);
            result.Y = (uint)(R1C0 * vector.X + R1C1 * vector.Y + R1C2 * vector.Z);
            result.Z = (uint)(R2C0 * vector.X + R2C1 * vector.Y + R2C2 * vector.Z);
        }
        public static void Transform(ref Matrix3ui matrix, ref Vector3d vector, out Vector3d result)
        {
            result.X = (uint)(matrix.R0C0 * vector.X + matrix.R0C1 * vector.Y + matrix.R0C2 * vector.Z);
            result.Y = (uint)(matrix.R1C0 * vector.X + matrix.R1C1 * vector.Y + matrix.R1C2 * vector.Z);
            result.Z = (uint)(matrix.R2C0 * vector.X + matrix.R2C1 * vector.Y + matrix.R2C2 * vector.Z);
        }

        public void Rotate(uint angle)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin =  System.Math.Sin(angleRadians);
            double cos =  System.Math.Cos(angleRadians);

            double r0c0 = cos * R0C0 + sin * R1C0;
            double r0c1 = cos * R0C1 + sin * R1C1;
            double r0c2 = cos * R0C2 + sin * R1C2;

            R1C0 = (uint)(cos * R1C0 - sin * R0C0);
            R1C1 = (uint)(cos * R1C1 - sin * R0C1);
            R1C2 = (uint)(cos * R1C2 - sin * R0C2);

            R0C0 = (uint)(r0c0);
            R0C1 = (uint)(r0c1);
            R0C2 = (uint)(r0c2);
        }
        public void Rotate(uint angle, out Matrix3ui result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos =  System.Math.Cos(angleRadians);

            result.R0C0 = (uint)(cos * R0C0 + sin * R1C0);
            result.R0C1 = (uint)(cos * R0C1 + sin * R1C1);
            result.R0C2 = (uint)(cos * R0C2 + sin * R1C2);
            result.R1C0 = (uint)(cos * R1C0 - sin * R0C0);
            result.R1C1 = (uint)(cos * R1C1 - sin * R0C1);
            result.R1C2 = (uint)(cos * R1C2 - sin * R0C2);
            result.R2C0 = (uint)(R2C0);
            result.R2C1 = (uint)(R2C1);
            result.R2C2 = (uint)(R2C2);
        }
        public static void Rotate(ref Matrix3ui matrix, uint angle, out Matrix3ui result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos = System.Math.Cos(angleRadians);

            result.R0C0 = (uint)(cos * matrix.R0C0 + sin * matrix.R1C0);
            result.R0C1 = (uint)(cos * matrix.R0C1 + sin * matrix.R1C1);
            result.R0C2 = (uint)(cos * matrix.R0C2 + sin * matrix.R1C2);
            result.R1C0 = (uint)(cos * matrix.R1C0 - sin * matrix.R0C0);
            result.R1C1 = (uint)(cos * matrix.R1C1 - sin * matrix.R0C1);
            result.R1C2 = (uint)(cos * matrix.R1C2 - sin * matrix.R0C2);
            result.R2C0 =(uint)( matrix.R2C0);
            result.R2C1 = (uint)(matrix.R2C1);
            result.R2C2 = (uint)(matrix.R2C2);
        }
        public static void RotateMatrix(uint angle, out Matrix3ui result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos = System.Math.Cos(angleRadians);

            result.R0C0 =  (uint)cos;
            result.R0C1 =  (uint)sin;
            result.R0C2 =  (uint)0;
            result.R1C0 =  (uint)-sin;
            result.R1C1 =  (uint)cos;
            result.R1C2 =  (uint)0;
            result.R2C0 =  (uint)0;
            result.R2C1 =  (uint)0;
            result.R2C2 =  (uint)1;
        }

        public Quaterniond ToQuaternion()
        {
            //return new Quaterniond(ref this);
            throw new NotImplementedException();
		}

        #endregion

        #region Constants

		
		/// <summary>
        /// Defines the size of the Matrix3ui struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(new Matrix3ui());

        /// <summary>The identity matrix.</summary>
        public static readonly Matrix3ui Identity = new Matrix3ui
        (
            1, 0, 0,
            0, 1, 0,
            0, 0, 1
        );

        /// <summary>A matrix of all zeros.</summary>
        public static readonly Matrix3ui Zero = new Matrix3ui
        (
            0, 0, 0,
            0, 0, 0,
            0, 0, 0
        );

        #endregion

        #region HashCode

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return
                R0C0.GetHashCode() ^ R0C1.GetHashCode() ^ R0C2.GetHashCode() ^
                R1C0.GetHashCode() ^ R1C1.GetHashCode() ^ R1C2.GetHashCode() ^
                R2C0.GetHashCode() ^ R2C1.GetHashCode() ^ R2C2.GetHashCode();
        }

        #endregion

        #region String

        /// <summary>Returns the fully qualified type name of this instance.</summary>
        /// <returns>A System.String containing left fully qualified type name.</returns>
        public override string ToString()
        {
            return String.Format(
                "|{00}, {01}, {02}|\n" +
                "|{03}, {04}, {05}|\n" +
                "|{06}, {07}, {18}|\n" +
                R0C0, R0C1, R0C2,
                R1C0, R1C1, R1C2,
                R2C0, R2C1, R2C2);
        }

        #endregion
    }

    /// <summary>
    /// Represents a 4x4 Matrix
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix4ui : IEquatable<Matrix4ui>
    {
        #region Fields

        /// <summary>
        /// Top row of the matrix
        /// </summary>
        public Vector4ui Row0;
        /// <summary>
        /// 2nd row of the matrix
        /// </summary>
        public Vector4ui Row1;
        /// <summary>
        /// 3rd row of the matrix
        /// </summary>
        public Vector4ui Row2;
        /// <summary>
        /// Bottom row of the matrix
        /// </summary>
        public Vector4ui Row3;
 
        /// <summary>
        /// The identity matrix
        /// </summary>
        public static Matrix4ui Identity = new Matrix4ui(Vector4ui.UnitX, Vector4ui.UnitY, Vector4ui.UnitZ, Vector4ui.UnitW);

		 /// <summary>
        /// The zero matrix
        /// </summary>
		public static Matrix4ui Zero = new Matrix4ui(Vector4ui.Zero, Vector4ui.Zero, Vector4ui.Zero, Vector4ui.Zero);


		/// <summary>
        /// Defines the size of the Matrix4ui struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(new Matrix4ui());

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="row0">Top row of the matrix</param>
        /// <param name="row1">Second row of the matrix</param>
        /// <param name="row2">Third row of the matrix</param>
        /// <param name="row3">Bottom row of the matrix</param>
        public Matrix4ui(Vector4ui row0, Vector4ui row1, Vector4ui row2, Vector4ui row3)
        {
            Row0 = row0;
            Row1 = row1;
            Row2 = row2;
            Row3 = row3;
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="m00">First item of the first row of the matrix.</param>
        /// <param name="m01">Second item of the first row of the matrix.</param>
        /// <param name="m02">Third item of the first row of the matrix.</param>
        /// <param name="m03">Fourth item of the first row of the matrix.</param>
        /// <param name="m10">First item of the second row of the matrix.</param>
        /// <param name="m11">Second item of the second row of the matrix.</param>
        /// <param name="m12">Third item of the second row of the matrix.</param>
        /// <param name="m13">Fourth item of the second row of the matrix.</param>
        /// <param name="m20">First item of the third row of the matrix.</param>
        /// <param name="m21">Second item of the third row of the matrix.</param>
        /// <param name="m22">Third item of the third row of the matrix.</param>
        /// <param name="m23">First item of the third row of the matrix.</param>
        /// <param name="m30">Fourth item of the fourth row of the matrix.</param>
        /// <param name="m31">Second item of the fourth row of the matrix.</param>
        /// <param name="m32">Third item of the fourth row of the matrix.</param>
        /// <param name="m33">Fourth item of the fourth row of the matrix.</param>
        public Matrix4ui(
            uint m00, uint m01, uint m02, uint m03,
            uint m10, uint m11, uint m12, uint m13,
            uint m20, uint m21, uint m22, uint m23,
            uint m30, uint m31, uint m32, uint m33)
        {
            Row0 = new Vector4ui(m00, m01, m02, m03);
            Row1 = new Vector4ui(m10, m11, m12, m13);
            Row2 = new Vector4ui(m20, m21, m22, m23);
            Row3 = new Vector4ui(m30, m31, m32, m33);
        }
		
		/// <summary>Constructs left matrix from the given array of uint-precision floating-point numbers.</summary>
        /// <param name="uintArray">The array of uints for the components of the matrix.</param>
        public Matrix4ui(uint[] uintArray)
        {
            if (uintArray == null || uintArray.GetLength(0) < 16) throw new MissingFieldException();
			Row0 = new Vector4ui(uintArray[0], uintArray[1], uintArray[2], uintArray[3]);
            Row1 = new Vector4ui(uintArray[4], uintArray[5], uintArray[6], uintArray[7]);
            Row2 = new Vector4ui(uintArray[8], uintArray[9], uintArray[10], uintArray[11]);
            Row3 = new Vector4ui(uintArray[12], uintArray[13], uintArray[14], uintArray[15]);
        }
        
		/// <summary>Converts the matrix into an array of uints.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>An array of uints for the matrix in Column-major order.</returns>
        public static explicit operator uint[](Matrix4ui matrix)
        {
            return new uint[16]
            {
                matrix.Row0.X,
                matrix.Row1.X,
                matrix.Row2.X,
                matrix.Row3.X,
                matrix.Row0.Y,
                matrix.Row1.Y,
                matrix.Row2.Y,
                matrix.Row3.Y,
                matrix.Row0.Z,
                matrix.Row1.Z,
                matrix.Row2.Z,
                matrix.Row3.Z,
                matrix.Row0.W,
                matrix.Row1.W,
                matrix.Row2.W,
                matrix.Row3.W
            };
        }
		
		#endregion

        #region Public Members

        #region Properties

        /// <summary>
        /// The determinant of this matrix
        /// </summary>
        public uint Determinant
        {
            get
            {
                return
                    Row0.X * Row1.Y * Row2.Z * Row3.W - Row0.X * Row1.Y * Row2.W * Row3.Z + Row0.X * Row1.Z * Row2.W * Row3.Y - Row0.X * Row1.Z * Row2.Y * Row3.W
                  + Row0.X * Row1.W * Row2.Y * Row3.Z - Row0.X * Row1.W * Row2.Z * Row3.Y - Row0.Y * Row1.Z * Row2.W * Row3.X + Row0.Y * Row1.Z * Row2.X * Row3.W
                  - Row0.Y * Row1.W * Row2.X * Row3.Z + Row0.Y * Row1.W * Row2.Z * Row3.X - Row0.Y * Row1.X * Row2.Z * Row3.W + Row0.Y * Row1.X * Row2.W * Row3.Z
                  + Row0.Z * Row1.W * Row2.X * Row3.Y - Row0.Z * Row1.W * Row2.Y * Row3.X + Row0.Z * Row1.X * Row2.Y * Row3.W - Row0.Z * Row1.X * Row2.W * Row3.Y
                  + Row0.Z * Row1.Y * Row2.W * Row3.X - Row0.Z * Row1.Y * Row2.X * Row3.W - Row0.W * Row1.X * Row2.Y * Row3.Z + Row0.W * Row1.X * Row2.Z * Row3.Y
                  - Row0.W * Row1.Y * Row2.Z * Row3.X + Row0.W * Row1.Y * Row2.X * Row3.Z - Row0.W * Row1.Z * Row2.X * Row3.Y + Row0.W * Row1.Z * Row2.Y * Row3.X;
            }
        }

        /// <summary>
        /// The first column of this matrix
        /// </summary>
        public Vector4ui Column0
        {
            get { return new Vector4ui(Row0.X, Row1.X, Row2.X, Row3.X); }
        }

        /// <summary>
        /// The second column of this matrix
        /// </summary>
        public Vector4ui Column1
        {
            get { return new Vector4ui(Row0.Y, Row1.Y, Row2.Y, Row3.Y); }
        }

        /// <summary>
        /// The third column of this matrix
        /// </summary>
        public Vector4ui Column2
        {
            get { return new Vector4ui(Row0.Z, Row1.Z, Row2.Z, Row3.Z); }
        }

        /// <summary>
        /// The fourth column of this matrix
        /// </summary>
        public Vector4ui Column3
        {
            get { return new Vector4ui(Row0.W, Row1.W, Row2.W, Row3.W); }
        }

        /// <summary>
        /// Gets or sets the value at row 1, column 1 of this instance.
        /// </summary>
        public uint M11 { get { return Row0.X; } set { Row0.X = value; } }

        /// <summary>
        /// Gets or sets the value at row 1, column 2 of this instance.
        /// </summary>
        public uint M12 { get { return Row0.Y; } set { Row0.Y = value; } }

        /// <summary>
        /// Gets or sets the value at row 1, column 3 of this instance.
        /// </summary>
        public uint M13 { get { return Row0.Z; } set { Row0.Z = value; } }

        /// <summary>
        /// Gets or sets the value at row 1, column 4 of this instance.
        /// </summary>
        public uint M14 { get { return Row0.W; } set { Row0.W = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 1 of this instance.
        /// </summary>
        public uint M21 { get { return Row1.X; } set { Row1.X = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 2 of this instance.
        /// </summary>
        public uint M22 { get { return Row1.Y; } set { Row1.Y = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 3 of this instance.
        /// </summary>
        public uint M23 { get { return Row1.Z; } set { Row1.Z = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 4 of this instance.
        /// </summary>
        public uint M24 { get { return Row1.W; } set { Row1.W = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 1 of this instance.
        /// </summary>
        public uint M31 { get { return Row2.X; } set { Row2.X = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 2 of this instance.
        /// </summary>
        public uint M32 { get { return Row2.Y; } set { Row2.Y = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 3 of this instance.
        /// </summary>
        public uint M33 { get { return Row2.Z; } set { Row2.Z = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 4 of this instance.
        /// </summary>
        public uint M34 { get { return Row2.W; } set { Row2.W = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 1 of this instance.
        /// </summary>
        public uint M41 { get { return Row3.X; } set { Row3.X = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 2 of this instance.
        /// </summary>
        public uint M42 { get { return Row3.Y; } set { Row3.Y = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 3 of this instance.
        /// </summary>
        public uint M43 { get { return Row3.Z; } set { Row3.Z = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 4 of this instance.
        /// </summary>
        public uint M44 { get { return Row3.W; } set { Row3.W = value; } }

        #endregion

        #region Instance

        #region public void Invert()

        /// <summary>
        /// Converts this instance into its inverse.
        /// </summary>
        public void Invert()
        {
            this = Matrix4ui.Invert(this);
        }

        #endregion

        #region public void Transpose()

        /// <summary>
        /// Converts this instance into its transpose.
        /// </summary>
        public void Transpose()
        {
            this = Matrix4ui.Transpose(this);
        }

        #endregion

        #endregion

        #region Static
		                #region Multiply Functions

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <returns>A new instance that is the result of the multiplication</returns>
        public static Matrix4ui Mult(Matrix4ui left, Matrix4ui right)
        {
            Matrix4ui result;
            Mult(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <param name="result">A new instance that is the result of the multiplication</param>
        public static void Mult(ref Matrix4ui left, ref Matrix4ui right, out Matrix4ui result)
        {
            result = new Matrix4ui(
                right.M11 * left.M11 + right.M12 * left.M21 + right.M13 * left.M31 + right.M14 * left.M41,
                right.M11 * left.M12 + right.M12 * left.M22 + right.M13 * left.M32 + right.M14 * left.M42,
                right.M11 * left.M13 + right.M12 * left.M23 + right.M13 * left.M33 + right.M14 * left.M43,
                right.M11 * left.M14 + right.M12 * left.M24 + right.M13 * left.M34 + right.M14 * left.M44,
                right.M21 * left.M11 + right.M22 * left.M21 + right.M23 * left.M31 + right.M24 * left.M41,
                right.M21 * left.M12 + right.M22 * left.M22 + right.M23 * left.M32 + right.M24 * left.M42,
                right.M21 * left.M13 + right.M22 * left.M23 + right.M23 * left.M33 + right.M24 * left.M43,
                right.M21 * left.M14 + right.M22 * left.M24 + right.M23 * left.M34 + right.M24 * left.M44,
                right.M31 * left.M11 + right.M32 * left.M21 + right.M33 * left.M31 + right.M34 * left.M41,
                right.M31 * left.M12 + right.M32 * left.M22 + right.M33 * left.M32 + right.M34 * left.M42,
                right.M31 * left.M13 + right.M32 * left.M23 + right.M33 * left.M33 + right.M34 * left.M43,
                right.M31 * left.M14 + right.M32 * left.M24 + right.M33 * left.M34 + right.M34 * left.M44,
                right.M41 * left.M11 + right.M42 * left.M21 + right.M43 * left.M31 + right.M44 * left.M41,
                right.M41 * left.M12 + right.M42 * left.M22 + right.M43 * left.M32 + right.M44 * left.M42,
                right.M41 * left.M13 + right.M42 * left.M23 + right.M43 * left.M33 + right.M44 * left.M43,
                right.M41 * left.M14 + right.M42 * left.M24 + right.M43 * left.M34 + right.M44 * left.M44);
		}

        #endregion

        #region Invert Functions

        /// <summary>
        /// Calculate the inverse of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to invert</param>
        /// <returns>The inverse of the given matrix if it has one, or the input if it is singular</returns>
        /// <exception cref="InvalidOperationException">Thrown if the Matrix4ui is singular.</exception>
        public static Matrix4ui Invert(Matrix4ui mat)
        {
            int[] colIdx = { 0, 0, 0, 0 };
            int[] rowIdx = { 0, 0, 0, 0 };
            int[] pivotIdx = { -1, -1, -1, -1 };

            // convert the matrix to an array for easy looping
            float[,] inverse = {{(float)mat.Row0.X, (float)mat.Row0.Y, (float)mat.Row0.Z, (float)mat.Row0.W}, 
                                {(float)mat.Row1.X, (float)mat.Row1.Y, (float)mat.Row1.Z, (float)mat.Row1.W}, 
                                {(float)mat.Row2.X, (float)mat.Row2.Y, (float)mat.Row2.Z, (float)mat.Row2.W}, 
                                {(float)mat.Row3.X, (float)mat.Row3.Y, (float)mat.Row3.Z, (float)mat.Row3.W} };
            int icol = 0;
            int irow = 0;
            for (int i = 0; i < 4; i++)
            {
                // Find the largest pivot value
                float maxPivot = 0;
                for (int j = 0; j < 4; j++)
                {
                    if (pivotIdx[j] != 0)
                    {
                        for (int k = 0; k < 4; ++k)
                        {
                            if (pivotIdx[k] == -1)
                            {
                               float absVal = System.Math.Abs(inverse[j, k]);
                                if (absVal > maxPivot)
                                {
                                    maxPivot = absVal;
                                    irow = j;
                                    icol = k;
                                }
                            }
                            else if (pivotIdx[k] > 0)
                            {
                                return mat;
                            }
                        }
                    }
                }

                ++(pivotIdx[icol]);

                // Swap rows over so pivot is on diagonal
                if (irow != icol)
                {
                    for (int k = 0; k < 4; ++k)
                    {
                        float f = inverse[irow, k];
                        inverse[irow, k] = inverse[icol, k];
                        inverse[icol, k] = f;
                    }
                }

                rowIdx[i] = irow;
                colIdx[i] = icol;

                float pivot = inverse[icol, icol];
                // check for singular matrix
                if (pivot == 0)
                {
                    throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
                    //return mat;
                }

                // Scale row so it has a unit diagonal
                float oneOverPivot = 1.0f / pivot;
                inverse[icol, icol] = 1.0f;
                for (int k = 0; k < 4; ++k)
                    inverse[icol, k] *= oneOverPivot;

                // Do elimination of non-diagonal elements
                for (int j = 0; j < 4; ++j)
                {
                    // check this isn't on the diagonal
                    if (icol != j)
                    {
                        float f = inverse[j, icol];
                        inverse[j, icol] = 0;
                        for (int k = 0; k < 4; ++k)
                            inverse[j, k] -= inverse[icol, k] * f;
                    }
                }
            }

            for (int j = 3; j >= 0; --j)
            {
                int ir = rowIdx[j];
                int ic = colIdx[j];
                for (int k = 0; k < 4; ++k)
                {
                    float f = inverse[k, ir];
                    inverse[k, ir] = inverse[k, ic];
                    inverse[k, ic] = f;
                }
            }

            mat.Row0 = new Vector4ui((uint)inverse[0, 0], (uint)inverse[0, 1], (uint)inverse[0, 2], (uint)inverse[0, 3]);
            mat.Row1 = new Vector4ui((uint)inverse[1, 0], (uint)inverse[1, 1], (uint)inverse[1, 2], (uint)inverse[1, 3]);
            mat.Row2 = new Vector4ui((uint)inverse[2, 0], (uint)inverse[2, 1], (uint)inverse[2, 2], (uint)inverse[2, 3]);
            mat.Row3 = new Vector4ui((uint)inverse[3, 0], (uint)inverse[3, 1], (uint)inverse[3, 2], (uint)inverse[3, 3]);
            return mat;
        }

        #endregion

        #region Transpose

        /// <summary>
        /// Calculate the transpose of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to transpose</param>
        /// <returns>The transpose of the given matrix</returns>
        public static Matrix4ui Transpose(Matrix4ui mat)
        {
            return new Matrix4ui(mat.Column0, mat.Column1, mat.Column2, mat.Column3);
        }


        /// <summary>
        /// Calculate the transpose of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to transpose</param>
        /// <param name="result">The result of the calculation</param>
        public static void Transpose(ref Matrix4ui mat, out Matrix4ui result)
        {
            result.Row0 = mat.Column0;
            result.Row1 = mat.Column1;
            result.Row2 = mat.Column2;
            result.Row3 = mat.Column3;
        }

        #endregion

        #endregion

        #region Operators

        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new Matrix4ui which holds the result of the multiplication</returns>
        public static Matrix4ui operator *(Matrix4ui left, Matrix4ui right)
        {
            return Matrix4ui.Mult(left, right);
        }

		/// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new Vector3ui which holds the result of the multiplication</returns>
        public static Vector3ui operator *(Matrix4ui left, Vector3ui right)
		{
			Vector3ui r;

			double fInvW = 1.0 / (left.Row3.X * right.X + left.Row3.Y * right.Y + left.Row3.Z * right.Z + left.Row3.W);

			r.X =  (uint)((left.Row0.X * right.X + left.Row0.Y * right.Y + left.Row0.Z * right.Z + left.Row0.W) * fInvW);
			r.Y =  (uint)((left.Row1.X * right.X + left.Row1.Y * right.Y + left.Row1.Z * right.Z + left.Row1.W) * fInvW);
			r.Z =  (uint)((left.Row2.X * right.X + left.Row2.Y * right.Y + left.Row2.Z * right.Z + left.Row2.W) * fInvW);

			return r;
		}
		
		/// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new Vector3ui which holds the result of the multiplication</returns>
        public static Vector4ui operator *(Matrix4ui left, Vector4ui right)
		{
			Vector4ui r;

			double fInvW = 1.0 / (left.Row3.X * right.X + left.Row3.Y * right.Y + left.Row3.Z * right.Z + left.Row3.W);

			r.X =  (uint)((left.Row0.X * right.X + left.Row0.Y * right.Y + left.Row0.Z * right.Z + left.Row0.W * right.W) * fInvW);
			r.Y =  (uint)((left.Row1.X * right.X + left.Row1.Y * right.Y + left.Row1.Z * right.Z + left.Row1.W * right.W) * fInvW);
			r.Z =  (uint)((left.Row2.X * right.X + left.Row2.Y * right.Y + left.Row2.Z * right.Z + left.Row2.W * right.W) * fInvW);
			r.W =  (uint)((left.Row3.X * right.X + left.Row3.Y * right.Y + left.Row3.Z * right.Z + left.Row3.W * right.W) * fInvW);
			return r;
		}

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(Matrix4ui left, Matrix4ui right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(Matrix4ui left, Matrix4ui right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current Matrix4ui4.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}\n{1}\n{2}\n{3}", Row0, Row1, Row2, Row3);
        }

        #endregion

        #region public override int GetHashCode()

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            return Row0.GetHashCode() ^ Row1.GetHashCode() ^ Row2.GetHashCode() ^ Row3.GetHashCode();
        }

        #endregion

        #region public override bool Equals(object obj)

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare tresult.</param>
        /// <returns>True if the instances are equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Matrix4ui))
                return false;

            return this.Equals((Matrix4ui)obj);
        }

        #endregion

        #endregion

        #endregion

        #region IEquatable<Matrix4ui> Members

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="other">An matrix to compare with this matrix.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public bool Equals(Matrix4ui other)
        {
            return
                Row0 == other.Row0 &&
                Row1 == other.Row1 &&
                Row2 == other.Row2 &&
                Row3 == other.Row3;
        }

        #endregion
    }

	[Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix2f : IEquatable<Matrix2f>
    {
        #region Fields & Access

        /// <summary>Row 0, Column 0</summary>
        public float R0C0;

        /// <summary>Row 0, Column 1</summary>
        public float R0C1;

        /// <summary>Row 1, Column 0</summary>
        public float R1C0;

        /// <summary>Row 1, Column 1</summary>
        public float R1C1;

        /// <summary>Gets the component at the given row and column in the matrix.</summary>
        /// <param name="row">The row of the matrix.</param>
        /// <param name="column">The column of the matrix.</param>
        /// <returns>The component at the given row and column in the matrix.</returns>
        public float this[int row, int column]
        {
            get
            {
                switch( row )
                {
                    case 0:
                        switch (column)
                        {
                            case 0: return R0C0;
                            case 1: return R0C1;
                        }
                        break;

                    case 1:
                        switch (column)
                        {
                            case 0: return R1C0;
                            case 1: return R1C1;
                        }
                        break;
                }

                throw new IndexOutOfRangeException();
            }
            set
            {
                switch( row )
                {
                    case 0:
                        switch (column)
                        {
                            case 0: R0C0 = value; return;
                            case 1: R0C1 = value; return;
                        }
                        break;

                    case 1:
                        switch (column)
                        {
                            case 0: R1C0 = value; return;
                            case 1: R1C1 = value; return;
                        }
                        break;
                }

                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>Gets the component at the index into the matrix.</summary>
        /// <param name="index">The index into the components of the matrix.</param>
        /// <returns>The component at the given index into the matrix.</returns>
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return R0C0;
                    case 1: return R0C1;
                    case 2: return R1C0;
                    case 3: return R1C1;
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0: R0C0 = value; return;
                    case 1: R0C1 = value; return;
                    case 2: R1C0 = value; return;
                    case 3: R1C1 = value; return;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>Converts the matrix into an IntPtr.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>An IntPtr for the matrix.</returns>
        public static explicit operator IntPtr(Matrix2f matrix)
        {
            unsafe
            {
                return (IntPtr)(&matrix.R0C0);
            }
        }

        /// <summary>Converts the matrix into left float*.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>A float* for the matrix.</returns>
        [CLSCompliant(false)]
        unsafe public static explicit operator float*(Matrix2f matrix)
        {
            return &matrix.R0C0;
        }

        /// <summary>Converts the matrix into an array of floats.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>An array of floats for the matrix.</returns>
        public static explicit operator float[](Matrix2f matrix)
        {
            return new float[4]
            {
                matrix.R0C0,
                matrix.R0C1,
                matrix.R1C0,
                matrix.R1C1
            };
        }

        #endregion

        #region Constructors

        /// <summary>Constructs left matrix with the same components as the given matrix.</summary>
        /// <param name="vector">The matrix whose components to copy.</param>
        public Matrix2f(ref Matrix2f matrix)
        {
            this.R0C0 = matrix.R0C0;
            this.R0C1 = matrix.R0C1;
            this.R1C0 = matrix.R1C0;
            this.R1C1 = matrix.R1C1;
        }

        /// <summary>Constructs left matrix with the given values.</summary>
        /// <param name="r0c0">The value for row 0 column 0.</param>
        /// <param name="r0c1">The value for row 0 column 1.</param>
        /// <param name="r1c0">The value for row 1 column 0.</param>
        /// <param name="r1c1">The value for row 1 column 1.</param>
        public Matrix2f
        (
            float r0c0,
            float r0c1,
            float r1c0,
            float r1c1
        )
        {
            this.R0C0 = r0c0;
            this.R0C1 = r0c1;
            this.R1C0 = r1c0;
            this.R1C1 = r1c1;
        }

        /// <summary>Constructs left matrix from the given array of float-precision floating-point numbers.</summary>
        /// <param name="floatArray">The array of floats for the components of the matrix in Column-major order.</param>
        public Matrix2f(float[] floatArray)
        {
            if (floatArray == null || floatArray.GetLength(0) < 4) throw new MissingFieldException();

            this.R0C0 = floatArray[0];
            this.R0C1 = floatArray[1];
            this.R1C0 = floatArray[2];
            this.R1C1 = floatArray[3];
        }

        #endregion

        #region Equality

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix3f structure to compare with.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        [CLSCompliant(false)]
        public bool Equals(Matrix2f matrix)
        {
            return
                R0C0 == matrix.R0C0 &&
                R0C1 == matrix.R0C1 &&
                R1C0 == matrix.R1C0 &&
                R1C1 == matrix.R1C1;
        }

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix3f structure to compare to.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public bool Equals(ref Matrix2f matrix)
        {
            return
                R0C0 == matrix.R0C0 &&
                R0C1 == matrix.R0C1 &&
                R1C0 == matrix.R1C0 &&
                R1C1 == matrix.R1C1;
        }

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="left">The left-hand operand.</param>
        /// <param name="right">The right-hand operand.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public static bool Equals(ref Matrix2f left, ref Matrix2f right)
        {
            return
                left.R0C0 == right.R0C0 &&
                left.R0C1 == right.R0C1 &&
                left.R1C0 == right.R1C0 &&
                left.R1C1 == right.R1C1;
        }

        /// <summary>Indicates whether the current matrix is approximately equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix2f structure to compare with.</param>
        /// <param name="tolerance">The limit below which the matrices are considered equal.</param>
        /// <returns>true if the current matrix is approximately equal to the matrix parameter; otherwise, false.</returns>
        public bool EqualsApprox(ref Matrix2f matrix, float tolerance)
        {
            return
                System.Math.Abs(R0C0 - matrix.R0C0) <= tolerance &&
                System.Math.Abs(R0C1 - matrix.R0C1) <= tolerance &&
                System.Math.Abs(R1C0 - matrix.R1C0) <= tolerance &&
                System.Math.Abs(R1C1 - matrix.R1C1) <= tolerance;
        }

        /// <summary>Indicates whether the current matrix is approximately equal to another matrix.</summary>
        /// <param name="left">The left-hand operand.</param>
        /// <param name="right">The right-hand operand.</param>
        /// <param name="tolerance">The limit below which the matrices are considered equal.</param>
        /// <returns>true if the current matrix is approximately equal to the matrix parameter; otherwise, false.</returns>
        public static bool EqualsApprox(ref Matrix2f left, ref Matrix2f right, float tolerance)
        {
            return
                System.Math.Abs(left.R0C0 - right.R0C0) <= tolerance &&
                System.Math.Abs(left.R0C1 - right.R0C1) <= tolerance &&
                System.Math.Abs(left.R1C0 - right.R1C0) <= tolerance &&
                System.Math.Abs(left.R1C1 - right.R1C1) <= tolerance;
        }

        #endregion

        #region Arithmetic Operators


        /// <summary>Add left matrix to this matrix.</summary>
        /// <param name="matrix">The matrix to add.</param>
        public void Add(ref Matrix2f matrix)
        {
            R0C0 = R0C0 + matrix.R0C0;
            R0C1 = R0C1 + matrix.R0C1;
            R1C0 = R1C0 + matrix.R1C0;
            R1C1 = R1C1 + matrix.R1C1;
        }

        /// <summary>Add left matrix to this matrix.</summary>
        /// <param name="matrix">The matrix to add.</param>
        /// <param name="result">The resulting matrix of the addition.</param>
        public void Add(ref Matrix2f matrix, out Matrix2f result)
        {
            result.R0C0 = R0C0 + matrix.R0C0;
            result.R0C1 = R0C1 + matrix.R0C1;
            result.R1C0 = R1C0 + matrix.R1C0;
            result.R1C1 = R1C1 + matrix.R1C1;
        }

        /// <summary>Add left matrix to left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the addition.</param>
        public static void Add(ref Matrix2f left, ref Matrix2f right, out Matrix2f result)
        {
            result.R0C0 = left.R0C0 + right.R0C0;
            result.R0C1 = left.R0C1 + right.R0C1;
            result.R1C0 = left.R1C0 + right.R1C0;
            result.R1C1 = left.R1C1 + right.R1C1;
        }


        /// <summary>Subtract left matrix from this matrix.</summary>
        /// <param name="matrix">The matrix to subtract.</param>
        public void Subtract(ref Matrix2f matrix)
        {
            R0C0 = R0C0 + matrix.R0C0;
            R0C1 = R0C1 + matrix.R0C1;
            R1C0 = R1C0 + matrix.R1C0;
            R1C1 = R1C1 + matrix.R1C1;
        }

        /// <summary>Subtract left matrix from this matrix.</summary>
        /// <param name="matrix">The matrix to subtract.</param>
        /// <param name="result">The resulting matrix of the subtraction.</param>
        public void Subtract(ref Matrix2f matrix, out Matrix2f result)
        {
            result.R0C0 = R0C0 + matrix.R0C0;
            result.R0C1 = R0C1 + matrix.R0C1;
            result.R1C0 = R1C0 + matrix.R1C0;
            result.R1C1 = R1C1 + matrix.R1C1;
        }

        /// <summary>Subtract left matrix from left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the subtraction.</param>
        public static void Subtract(ref Matrix2f left, ref Matrix2f right, out Matrix2f result)
        {
            result.R0C0 = left.R0C0 + right.R0C0;
            result.R0C1 = left.R0C1 + right.R0C1;
            result.R1C0 = left.R1C0 + right.R1C0;
            result.R1C1 = left.R1C1 + right.R1C1;
        }


        /// <summary>Multiply left martix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        public void Multiply(ref Matrix2f matrix)
        {
            float r0c0 = matrix.R0C0 * R0C0 + matrix.R0C1 * R1C0;
            float r0c1 = matrix.R0C0 * R0C1 + matrix.R0C1 * R1C1;

            float r1c0 = matrix.R1C0 * R0C0 + matrix.R1C1 * R1C0;
            float r1c1 = matrix.R1C0 * R0C1 + matrix.R1C1 * R1C1;

            R0C0 = r0c0;
            R0C1 = r0c1;

            R1C0 = r1c0;
            R1C1 = r1c1;
        }

        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public void Multiply(ref Matrix2f matrix, out Matrix2f result)
        {
            result.R0C0 = matrix.R0C0 * R0C0 + matrix.R0C1 * R1C0;
            result.R0C1 = matrix.R0C0 * R0C1 + matrix.R0C1 * R1C1;
            result.R1C0 = matrix.R1C0 * R0C0 + matrix.R1C1 * R1C0;
            result.R1C1 = matrix.R1C0 * R0C1 + matrix.R1C1 * R1C1;
        }

        /// <summary>Multiply left matrix times left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public static void Multiply(ref Matrix2f left, ref Matrix2f right, out Matrix2f result)
        {
            result.R0C0 = right.R0C0 * left.R0C0 + right.R0C1 * left.R1C0;
            result.R0C1 = right.R0C0 * left.R0C1 + right.R0C1 * left.R1C1;
            result.R1C0 = right.R1C0 * left.R0C0 + right.R1C1 * left.R1C0;
            result.R1C1 = right.R1C0 * left.R0C1 + right.R1C1 * left.R1C1;
        }


        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        public void Multiply(float scalar)
        {
            R0C0 = scalar * R0C0;
            R0C1 = scalar * R0C1;
            R1C0 = scalar * R1C0;
            R1C1 = scalar * R1C1;
        }

        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public void Multiply(float scalar, out Matrix2f result)
        {
            result.R0C0 = scalar * R0C0;
            result.R0C1 = scalar * R0C1;
            result.R1C0 = scalar * R1C0;
            result.R1C1 = scalar * R1C1;
        }

        /// <summary>Multiply left matrix times left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public static void Multiply(ref Matrix3f matrix, float scalar, out Matrix2f result)
        {
            result.R0C0 = scalar * matrix.R0C0;
            result.R0C1 = scalar * matrix.R0C1;
            result.R1C0 = scalar * matrix.R1C0;
            result.R1C1 = scalar * matrix.R1C1;
        }


        #endregion

        #region Functions

        public float Determinant
        {
            get
            {
                return R0C0 * R1C1 - R0C1 * R1C0;
            }
        }

        public void Transpose()
        {
            Std.Swap(ref R0C1, ref R1C0);
        }
        public void Transpose(out Matrix2f result)
        {
            result.R0C0 = R0C0;
            result.R0C1 = R1C0;
            result.R1C0 = R0C1;
            result.R1C1 = R1C1;
        }
        public static void Transpose(ref Matrix2f matrix, out Matrix2f result)
        {
            result.R0C0 = matrix.R0C0;
            result.R0C1 = matrix.R1C0;
            result.R1C0 = matrix.R0C1;
            result.R1C1 = matrix.R1C1;
        }

        #endregion

        #region Transformation Functions

        public void Transform(ref Vector2d vector)
        {
            vector.X = R0C0 * vector.X + R0C1 * vector.Y;
            vector.Y = R1C0 * vector.X + R1C1 * vector.Y;
        }
        public static void Transform(ref Matrix2f matrix, ref Vector2d vector)
        {
            vector.X = matrix.R0C0 * vector.X + matrix.R0C1 * vector.Y;
            vector.Y = matrix.R1C0 * vector.X + matrix.R1C1 * vector.Y;
        }
        public void Transform(ref Vector2d vector, out Vector2d result)
        {
            result.X = R0C0 * vector.X + R0C1 * vector.Y;
            result.Y = R1C0 * vector.X + R1C1 * vector.Y;
        }
        public static void Transform(ref Matrix2f matrix, ref Vector2d vector, out Vector2d result)
        {
            result.X = matrix.R0C0 * vector.X + matrix.R0C1 * vector.Y;
            result.Y = matrix.R1C0 * vector.X + matrix.R1C1 * vector.Y;
        }

        public void Rotate(float angle)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin =  System.Math.Sin(angleRadians);
            double cos =  System.Math.Cos(angleRadians);

            R1C0 = (float)(cos * R1C0 - sin * R0C0);
            R1C1 = (float)(cos * R1C1 - sin * R0C1);

            R0C0 = (float)(cos * R0C0 + sin * R1C0);
            R0C1 = (float)(cos * R0C1 + sin * R1C1);
        }
        public void Rotate(float angle, out Matrix2f result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos =  System.Math.Cos(angleRadians);

            result.R0C0 = (float)(cos * R0C0 + sin * R1C0);
            result.R0C1 = (float)(cos * R0C1 + sin * R1C1);
            result.R1C0 = (float)(cos * R1C0 - sin * R0C0);
            result.R1C1 = (float)(cos * R1C1 - sin * R0C1);
        }
        public static void Rotate(ref Matrix2f matrix, float angle, out Matrix2f result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos = System.Math.Cos(angleRadians);

            result.R0C0 = (float)(cos * matrix.R0C0 + sin * matrix.R1C0);
            result.R0C1 = (float)(cos * matrix.R0C1 + sin * matrix.R1C1);
            result.R1C0 = (float)(cos * matrix.R1C0 - sin * matrix.R0C0);
            result.R1C1 = (float)(cos * matrix.R1C1 - sin * matrix.R0C1);
        }
        public static void RotateMatrix(float angle, out Matrix2f result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos = System.Math.Cos(angleRadians);

            result.R0C0 =  (float)cos;
            result.R0C1 =  (float)sin;
            result.R1C0 =  (float)-sin;
            result.R1C1 =  (float)cos;
        }

        #endregion

        #region Constants

		/// <summary>
        /// Defines the size of the Matrix2f struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(new Matrix2f());

        /// <summary>The identity matrix.</summary>
        public static readonly Matrix2f Identity = new Matrix2f
        (
            1, 0,
            0, 1
        );

        /// <summary>A matrix of all zeros.</summary>
        public static readonly Matrix2f Zero = new Matrix2f
        (
            0, 0,
            0, 0
        );

        #endregion

        #region HashCode

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return
                R0C0.GetHashCode() ^ R0C1.GetHashCode() ^
                R1C0.GetHashCode() ^ R1C1.GetHashCode();
        }

        #endregion

        #region String

        /// <summary>Returns the fully qualified type name of this instance.</summary>
        /// <returns>A System.String containing left fully qualified type name.</returns>
        public override string ToString()
        {
            return String.Format(
                "|{00}, {01}|\n" +
                "|{02}, {03}|\n" +
                R0C0, R0C1, 
                R1C0, R1C1);
        }

        #endregion
    }

	[Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3f : IEquatable<Matrix3f>
    {
        #region Fields & Access

        /// <summary>Row 0, Column 0</summary>
        public float R0C0;

        /// <summary>Row 0, Column 1</summary>
        public float R0C1;

        /// <summary>Row 0, Column 2</summary>
        public float R0C2;

        /// <summary>Row 1, Column 0</summary>
        public float R1C0;

        /// <summary>Row 1, Column 1</summary>
        public float R1C1;

        /// <summary>Row 1, Column 2</summary>
        public float R1C2;

        /// <summary>Row 2, Column 0</summary>
        public float R2C0;

        /// <summary>Row 2, Column 1</summary>
        public float R2C1;

        /// <summary>Row 2, Column 2</summary>
        public float R2C2;

        /// <summary>Gets the component at the given row and column in the matrix.</summary>
        /// <param name="row">The row of the matrix.</param>
        /// <param name="column">The column of the matrix.</param>
        /// <returns>The component at the given row and column in the matrix.</returns>
        public float this[int row, int column]
        {
            get
            {
                switch( row )
                {
                    case 0:
                        switch (column)
                        {
                            case 0: return R0C0;
                            case 1: return R0C1;
                            case 2: return R0C2;
                        }
                        break;

                    case 1:
                        switch (column)
                        {
                            case 0: return R1C0;
                            case 1: return R1C1;
                            case 2: return R1C2;
                        }
                        break;

                    case 2:
                        switch (column)
                        {
                            case 0: return R2C0;
                            case 1: return R2C1;
                            case 2: return R2C2;
                        }
                        break;
                }

                throw new IndexOutOfRangeException();
            }
            set
            {
                switch( row )
                {
                    case 0:
                        switch (column)
                        {
                            case 0: R0C0 = value; return;
                            case 1: R0C1 = value; return;
                            case 2: R0C2 = value; return;
                        }
                        break;

                    case 1:
                        switch (column)
                        {
                            case 0: R1C0 = value; return;
                            case 1: R1C1 = value; return;
                            case 2: R1C2 = value; return;
                        }
                        break;

                    case 2:
                        switch (column)
                        {
                            case 0: R2C0 = value; return;
                            case 1: R2C1 = value; return;
                            case 2: R2C2 = value; return;
                        }
                        break;
                }

                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>Gets the component at the index into the matrix.</summary>
        /// <param name="index">The index into the components of the matrix.</param>
        /// <returns>The component at the given index into the matrix.</returns>
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return R0C0;
                    case 1: return R0C1;
                    case 2: return R0C2;
                    case 3: return R1C0;
                    case 4: return R1C1;
                    case 5: return R1C2;
                    case 6: return R2C0;
                    case 7: return R2C1;
                    case 8: return R2C2;
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0: R0C0 = value; return;
                    case 1: R0C1 = value; return;
                    case 2: R0C2 = value; return;
                    case 3: R1C0 = value; return;
                    case 4: R1C1 = value; return;
                    case 5: R1C2 = value; return;
                    case 6: R2C0 = value; return;
                    case 7: R2C1 = value; return;
                    case 8: R2C2 = value; return;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>Converts the matrix into an IntPtr.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>An IntPtr for the matrix.</returns>
        public static explicit operator IntPtr(Matrix3f matrix)
        {
            unsafe
            {
                return (IntPtr)(&matrix.R0C0);
            }
        }

        /// <summary>Converts the matrix into left float*.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>A float* for the matrix.</returns>
        [CLSCompliant(false)]
        unsafe public static explicit operator float*(Matrix3f matrix)
        {
            return &matrix.R0C0;
        }

        /// <summary>Converts the matrix into an array of floats.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>An array of floats for the matrix.</returns>
        public static explicit operator float[](Matrix3f matrix)
        {
            return new float[9]
            {
                matrix.R0C0,
                matrix.R0C1,
                matrix.R0C2,
                matrix.R1C0,
                matrix.R1C1,
                matrix.R1C2,
                matrix.R2C0,
                matrix.R2C1,
                matrix.R2C2
            };
        }

        #endregion

        #region Constructors

        /// <summary>Constructs left matrix with the same components as the given matrix.</summary>
        /// <param name="vector">The matrix whose components to copy.</param>
        public Matrix3f(ref Matrix3f matrix)
        {
            this.R0C0 = matrix.R0C0;
            this.R0C1 = matrix.R0C1;
            this.R0C2 = matrix.R0C2;
            this.R1C0 = matrix.R1C0;
            this.R1C1 = matrix.R1C1;
            this.R1C2 = matrix.R1C2;
            this.R2C0 = matrix.R2C0;
            this.R2C1 = matrix.R2C1;
            this.R2C2 = matrix.R2C2;
        }

        /// <summary>Constructs left matrix with the given values.</summary>
        /// <param name="r0c0">The value for row 0 column 0.</param>
        /// <param name="r0c1">The value for row 0 column 1.</param>
        /// <param name="r0c2">The value for row 0 column 2.</param>
        /// <param name="r1c0">The value for row 1 column 0.</param>
        /// <param name="r1c1">The value for row 1 column 1.</param>
        /// <param name="r1c2">The value for row 1 column 2.</param>
        /// <param name="r2c0">The value for row 2 column 0.</param>
        /// <param name="r2c1">The value for row 2 column 1.</param>
        /// <param name="r2c2">The value for row 2 column 2.</param>
        public Matrix3f
        (
            float r0c0,
            float r0c1,
            float r0c2,
            float r1c0,
            float r1c1,
            float r1c2,
            float r2c0,
            float r2c1,
            float r2c2
        )
        {
            this.R0C0 = r0c0;
            this.R0C1 = r0c1;
            this.R0C2 = r0c2;
            this.R1C0 = r1c0;
            this.R1C1 = r1c1;
            this.R1C2 = r1c2;
            this.R2C0 = r2c0;
            this.R2C1 = r2c1;
            this.R2C2 = r2c2;
        }

        /// <summary>Constructs left matrix from the given array of float-precision floating-point numbers.</summary>
        /// <param name="floatArray">The array of floats for the components of the matrix in Column-major order.</param>
        public Matrix3f(float[] floatArray)
        {
            if (floatArray == null || floatArray.GetLength(0) < 9) throw new MissingFieldException();

            this.R0C0 = floatArray[0];
            this.R0C1 = floatArray[1];
            this.R0C2 = floatArray[2];
            this.R1C0 = floatArray[3];
            this.R1C1 = floatArray[4];
            this.R1C2 = floatArray[5];
            this.R2C0 = floatArray[6];
            this.R2C1 = floatArray[7];
            this.R2C2 = floatArray[8];
        }

        /// <summary>Constructs left matrix from the given quaternion.</summary>
        /// <param name="quaternion">The quaternion to use to construct the martix.</param>
        public Matrix3f(Quaterniond quaternion)
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

            R0C0 = (float)(1 - 2 * (yy + zz));
            R0C1 = (float)(2 * (xy - wz));
            R0C2 = (float)(2 * (xz + wy));

            R1C0 = (float)(2 * (xy + wz));
            R1C1 = (float)(1 - 2 * (xx + zz));
            R1C2 = (float)(2 * (yz - wx));

            R2C0 = (float)(2 * (xz - wy));
            R2C1 = (float)(2 * (yz + wx));
            R2C2 = (float)(1 - 2 * (xx + yy));
        }

        #endregion

        #region Equality

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix3f structure to compare with.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        [CLSCompliant(false)]
        public bool Equals(Matrix3f matrix)
        {
            return
                R0C0 == matrix.R0C0 &&
                R0C1 == matrix.R0C1 &&
                R0C2 == matrix.R0C2 &&
                R1C0 == matrix.R1C0 &&
                R1C1 == matrix.R1C1 &&
                R1C2 == matrix.R1C2 &&
                R2C0 == matrix.R2C0 &&
                R2C1 == matrix.R2C1 &&
                R2C2 == matrix.R2C2;
        }

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix3f structure to compare to.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public bool Equals(ref Matrix3f matrix)
        {
            return
                R0C0 == matrix.R0C0 &&
                R0C1 == matrix.R0C1 &&
                R0C2 == matrix.R0C2 &&
                R1C0 == matrix.R1C0 &&
                R1C1 == matrix.R1C1 &&
                R1C2 == matrix.R1C2 &&
                R2C0 == matrix.R2C0 &&
                R2C1 == matrix.R2C1 &&
                R2C2 == matrix.R2C2;
        }

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="left">The left-hand operand.</param>
        /// <param name="right">The right-hand operand.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public static bool Equals(ref Matrix3f left, ref Matrix3f right)
        {
            return
                left.R0C0 == right.R0C0 &&
                left.R0C1 == right.R0C1 &&
                left.R0C2 == right.R0C2 &&
                left.R1C0 == right.R1C0 &&
                left.R1C1 == right.R1C1 &&
                left.R1C2 == right.R1C2 &&
                left.R2C0 == right.R2C0 &&
                left.R2C1 == right.R2C1 &&
                left.R2C2 == right.R2C2;
        }

        /// <summary>Indicates whether the current matrix is approximately equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix3f structure to compare with.</param>
        /// <param name="tolerance">The limit below which the matrices are considered equal.</param>
        /// <returns>true if the current matrix is approximately equal to the matrix parameter; otherwise, false.</returns>
        public bool EqualsApprox(ref Matrix3f matrix, float tolerance)
        {
            return
                System.Math.Abs(R0C0 - matrix.R0C0) <= tolerance &&
                System.Math.Abs(R0C1 - matrix.R0C1) <= tolerance &&
                System.Math.Abs(R0C2 - matrix.R0C2) <= tolerance &&
                System.Math.Abs(R1C0 - matrix.R1C0) <= tolerance &&
                System.Math.Abs(R1C1 - matrix.R1C1) <= tolerance &&
                System.Math.Abs(R1C2 - matrix.R1C2) <= tolerance &&
                System.Math.Abs(R2C0 - matrix.R2C0) <= tolerance &&
                System.Math.Abs(R2C1 - matrix.R2C1) <= tolerance &&
                System.Math.Abs(R2C2 - matrix.R2C2) <= tolerance;
        }

        /// <summary>Indicates whether the current matrix is approximately equal to another matrix.</summary>
        /// <param name="left">The left-hand operand.</param>
        /// <param name="right">The right-hand operand.</param>
        /// <param name="tolerance">The limit below which the matrices are considered equal.</param>
        /// <returns>true if the current matrix is approximately equal to the matrix parameter; otherwise, false.</returns>
        public static bool EqualsApprox(ref Matrix3f left, ref Matrix3f right, float tolerance)
        {
            return
                System.Math.Abs(left.R0C0 - right.R0C0) <= tolerance &&
                System.Math.Abs(left.R0C1 - right.R0C1) <= tolerance &&
                System.Math.Abs(left.R0C2 - right.R0C2) <= tolerance &&
                System.Math.Abs(left.R1C0 - right.R1C0) <= tolerance &&
                System.Math.Abs(left.R1C1 - right.R1C1) <= tolerance &&
                System.Math.Abs(left.R1C2 - right.R1C2) <= tolerance &&
                System.Math.Abs(left.R2C0 - right.R2C0) <= tolerance &&
                System.Math.Abs(left.R2C1 - right.R2C1) <= tolerance &&
                System.Math.Abs(left.R2C2 - right.R2C2) <= tolerance;
        }

        #endregion

        #region Arithmetic Operators


        /// <summary>Add left matrix to this matrix.</summary>
        /// <param name="matrix">The matrix to add.</param>
        public void Add(ref Matrix3f matrix)
        {
            R0C0 = R0C0 + matrix.R0C0;
            R0C1 = R0C1 + matrix.R0C1;
            R0C2 = R0C2 + matrix.R0C2;
            R1C0 = R1C0 + matrix.R1C0;
            R1C1 = R1C1 + matrix.R1C1;
            R1C2 = R1C2 + matrix.R1C2;
            R2C0 = R2C0 + matrix.R2C0;
            R2C1 = R2C1 + matrix.R2C1;
            R2C2 = R2C2 + matrix.R2C2;
        }

        /// <summary>Add left matrix to this matrix.</summary>
        /// <param name="matrix">The matrix to add.</param>
        /// <param name="result">The resulting matrix of the addition.</param>
        public void Add(ref Matrix3f matrix, out Matrix3f result)
        {
            result.R0C0 = R0C0 + matrix.R0C0;
            result.R0C1 = R0C1 + matrix.R0C1;
            result.R0C2 = R0C2 + matrix.R0C2;
            result.R1C0 = R1C0 + matrix.R1C0;
            result.R1C1 = R1C1 + matrix.R1C1;
            result.R1C2 = R1C2 + matrix.R1C2;
            result.R2C0 = R2C0 + matrix.R2C0;
            result.R2C1 = R2C1 + matrix.R2C1;
            result.R2C2 = R2C2 + matrix.R2C2;
        }

        /// <summary>Add left matrix to left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the addition.</param>
        public static void Add(ref Matrix3f left, ref Matrix3f right, out Matrix3f result)
        {
            result.R0C0 = left.R0C0 + right.R0C0;
            result.R0C1 = left.R0C1 + right.R0C1;
            result.R0C2 = left.R0C2 + right.R0C2;
            result.R1C0 = left.R1C0 + right.R1C0;
            result.R1C1 = left.R1C1 + right.R1C1;
            result.R1C2 = left.R1C2 + right.R1C2;
            result.R2C0 = left.R2C0 + right.R2C0;
            result.R2C1 = left.R2C1 + right.R2C1;
            result.R2C2 = left.R2C2 + right.R2C2;
        }


        /// <summary>Subtract left matrix from this matrix.</summary>
        /// <param name="matrix">The matrix to subtract.</param>
        public void Subtract(ref Matrix3f matrix)
        {
            R0C0 = R0C0 + matrix.R0C0;
            R0C1 = R0C1 + matrix.R0C1;
            R0C2 = R0C2 + matrix.R0C2;
            R1C0 = R1C0 + matrix.R1C0;
            R1C1 = R1C1 + matrix.R1C1;
            R1C2 = R1C2 + matrix.R1C2;
            R2C0 = R2C0 + matrix.R2C0;
            R2C1 = R2C1 + matrix.R2C1;
            R2C2 = R2C2 + matrix.R2C2;
        }

        /// <summary>Subtract left matrix from this matrix.</summary>
        /// <param name="matrix">The matrix to subtract.</param>
        /// <param name="result">The resulting matrix of the subtraction.</param>
        public void Subtract(ref Matrix3f matrix, out Matrix3f result)
        {
            result.R0C0 = R0C0 + matrix.R0C0;
            result.R0C1 = R0C1 + matrix.R0C1;
            result.R0C2 = R0C2 + matrix.R0C2;
            result.R1C0 = R1C0 + matrix.R1C0;
            result.R1C1 = R1C1 + matrix.R1C1;
            result.R1C2 = R1C2 + matrix.R1C2;
            result.R2C0 = R2C0 + matrix.R2C0;
            result.R2C1 = R2C1 + matrix.R2C1;
            result.R2C2 = R2C2 + matrix.R2C2;
        }

        /// <summary>Subtract left matrix from left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the subtraction.</param>
        public static void Subtract(ref Matrix3f left, ref Matrix3f right, out Matrix3f result)
        {
            result.R0C0 = left.R0C0 + right.R0C0;
            result.R0C1 = left.R0C1 + right.R0C1;
            result.R0C2 = left.R0C2 + right.R0C2;
            result.R1C0 = left.R1C0 + right.R1C0;
            result.R1C1 = left.R1C1 + right.R1C1;
            result.R1C2 = left.R1C2 + right.R1C2;
            result.R2C0 = left.R2C0 + right.R2C0;
            result.R2C1 = left.R2C1 + right.R2C1;
            result.R2C2 = left.R2C2 + right.R2C2;
        }


        /// <summary>Multiply left martix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        public void Multiply(ref Matrix3f matrix)
        {
            float r0c0 = matrix.R0C0 * R0C0 + matrix.R0C1 * R1C0 + matrix.R0C2 * R2C0;
            float r0c1 = matrix.R0C0 * R0C1 + matrix.R0C1 * R1C1 + matrix.R0C2 * R2C1;
            float r0c2 = matrix.R0C0 * R0C2 + matrix.R0C1 * R1C2 + matrix.R0C2 * R2C2;

            float r1c0 = matrix.R1C0 * R0C0 + matrix.R1C1 * R1C0 + matrix.R1C2 * R2C0;
            float r1c1 = matrix.R1C0 * R0C1 + matrix.R1C1 * R1C1 + matrix.R1C2 * R2C1;
            float r1c2 = matrix.R1C0 * R0C2 + matrix.R1C1 * R1C2 + matrix.R1C2 * R2C2;

            R2C0 = matrix.R2C0 * R0C0 + matrix.R2C1 * R1C0 + matrix.R2C2 * R2C0;
            R2C1 = matrix.R2C0 * R0C1 + matrix.R2C1 * R1C1 + matrix.R2C2 * R2C1;
            R2C2 = matrix.R2C0 * R0C2 + matrix.R2C1 * R1C2 + matrix.R2C2 * R2C2;


            R0C0 = r0c0;
            R0C1 = r0c1;
            R0C2 = r0c2;

            R1C0 = r1c0;
            R1C1 = r1c1;
            R1C2 = r1c2;
        }

        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public void Multiply(ref Matrix3f matrix, out Matrix3f result)
        {
            result.R0C0 = matrix.R0C0 * R0C0 + matrix.R0C1 * R1C0 + matrix.R0C2 * R2C0;
            result.R0C1 = matrix.R0C0 * R0C1 + matrix.R0C1 * R1C1 + matrix.R0C2 * R2C1;
            result.R0C2 = matrix.R0C0 * R0C2 + matrix.R0C1 * R1C2 + matrix.R0C2 * R2C2;
            result.R1C0 = matrix.R1C0 * R0C0 + matrix.R1C1 * R1C0 + matrix.R1C2 * R2C0;
            result.R1C1 = matrix.R1C0 * R0C1 + matrix.R1C1 * R1C1 + matrix.R1C2 * R2C1;
            result.R1C2 = matrix.R1C0 * R0C2 + matrix.R1C1 * R1C2 + matrix.R1C2 * R2C2;
            result.R2C0 = matrix.R2C0 * R0C0 + matrix.R2C1 * R1C0 + matrix.R2C2 * R2C0;
            result.R2C1 = matrix.R2C0 * R0C1 + matrix.R2C1 * R1C1 + matrix.R2C2 * R2C1;
            result.R2C2 = matrix.R2C0 * R0C2 + matrix.R2C1 * R1C2 + matrix.R2C2 * R2C2;
        }

        /// <summary>Multiply left matrix times left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public static void Multiply(ref Matrix3f left, ref Matrix3f right, out Matrix3f result)
        {
            result.R0C0 = right.R0C0 * left.R0C0 + right.R0C1 * left.R1C0 + right.R0C2 * left.R2C0;
            result.R0C1 = right.R0C0 * left.R0C1 + right.R0C1 * left.R1C1 + right.R0C2 * left.R2C1;
            result.R0C2 = right.R0C0 * left.R0C2 + right.R0C1 * left.R1C2 + right.R0C2 * left.R2C2;
            result.R1C0 = right.R1C0 * left.R0C0 + right.R1C1 * left.R1C0 + right.R1C2 * left.R2C0;
            result.R1C1 = right.R1C0 * left.R0C1 + right.R1C1 * left.R1C1 + right.R1C2 * left.R2C1;
            result.R1C2 = right.R1C0 * left.R0C2 + right.R1C1 * left.R1C2 + right.R1C2 * left.R2C2;
            result.R2C0 = right.R2C0 * left.R0C0 + right.R2C1 * left.R1C0 + right.R2C2 * left.R2C0;
            result.R2C1 = right.R2C0 * left.R0C1 + right.R2C1 * left.R1C1 + right.R2C2 * left.R2C1;
            result.R2C2 = right.R2C0 * left.R0C2 + right.R2C1 * left.R1C2 + right.R2C2 * left.R2C2;
        }


        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        public void Multiply(float scalar)
        {
            R0C0 = scalar * R0C0;
            R0C1 = scalar * R0C1;
            R0C2 = scalar * R0C2;
            R1C0 = scalar * R1C0;
            R1C1 = scalar * R1C1;
            R1C2 = scalar * R1C2;
            R2C0 = scalar * R2C0;
            R2C1 = scalar * R2C1;
            R2C2 = scalar * R2C2;
        }

        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public void Multiply(float scalar, out Matrix3f result)
        {
            result.R0C0 = scalar * R0C0;
            result.R0C1 = scalar * R0C1;
            result.R0C2 = scalar * R0C2;
            result.R1C0 = scalar * R1C0;
            result.R1C1 = scalar * R1C1;
            result.R1C2 = scalar * R1C2;
            result.R2C0 = scalar * R2C0;
            result.R2C1 = scalar * R2C1;
            result.R2C2 = scalar * R2C2;
        }

        /// <summary>Multiply left matrix times left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public static void Multiply(ref Matrix3f matrix, float scalar, out Matrix3f result)
        {
            result.R0C0 = scalar * matrix.R0C0;
            result.R0C1 = scalar * matrix.R0C1;
            result.R0C2 = scalar * matrix.R0C2;
            result.R1C0 = scalar * matrix.R1C0;
            result.R1C1 = scalar * matrix.R1C1;
            result.R1C2 = scalar * matrix.R1C2;
            result.R2C0 = scalar * matrix.R2C0;
            result.R2C1 = scalar * matrix.R2C1;
            result.R2C2 = scalar * matrix.R2C2;
        }


        #endregion

        #region Functions

        public float Determinant
        {
            get
            {
                return R0C0 * R1C1 * R2C2 - R0C0 * R1C2 * R2C1 - R0C1 * R1C0 * R2C2 + R0C2 * R1C0 * R2C1 + R0C1 * R1C2 * R2C0 - R0C2 * R1C1 * R2C0;
            }
        }

        public void Transpose()
        {
            Std.Swap(ref R0C1, ref R1C0);
            Std.Swap(ref R0C2, ref R2C0);
            Std.Swap(ref R1C2, ref R2C1);
        }
        public void Transpose(out Matrix3f result)
        {
            result.R0C0 = R0C0;
            result.R0C1 = R1C0;
            result.R0C2 = R2C0;
            result.R1C0 = R0C1;
            result.R1C1 = R1C1;
            result.R1C2 = R2C1;
            result.R2C0 = R0C2;
            result.R2C1 = R1C2;
            result.R2C2 = R2C2;
        }
        public static void Transpose(ref Matrix3f matrix, out Matrix3f result)
        {
            result.R0C0 = matrix.R0C0;
            result.R0C1 = matrix.R1C0;
            result.R0C2 = matrix.R2C0;
            result.R1C0 = matrix.R0C1;
            result.R1C1 = matrix.R1C1;
            result.R1C2 = matrix.R2C1;
            result.R2C0 = matrix.R0C2;
            result.R2C1 = matrix.R1C2;
            result.R2C2 = matrix.R2C2;
        }

        #endregion

        #region Transformation Functions

        public void Transform(ref Vector3d vector)
        {
            double x = R0C0 * vector.X + R0C1 * vector.Y + R0C2 * vector.Z;
            double y = R1C0 * vector.X + R1C1 * vector.Y + R1C2 * vector.Z;
            vector.Z = (float)(R2C0 * vector.X + R2C1 * vector.Y + R2C2 * vector.Z);
            vector.X = x;
            vector.Y = y;
        }
        public static void Transform(ref Matrix3f matrix, ref Vector3d vector)
        {
            double x = (float)(matrix.R0C0 * vector.X + matrix.R0C1 * vector.Y + matrix.R0C2 * vector.Z);
            double y = (float)(matrix.R1C0 * vector.X + matrix.R1C1 * vector.Y + matrix.R1C2 * vector.Z);
            vector.Z = (float)(matrix.R2C0 * vector.X + matrix.R2C1 * vector.Y + matrix.R2C2 * vector.Z);
            vector.X = x;
            vector.Y = y;
        }
        public void Transform(ref Vector3d vector, out Vector3d result)
        {
            result.X = (float)(R0C0 * vector.X + R0C1 * vector.Y + R0C2 * vector.Z);
            result.Y = (float)(R1C0 * vector.X + R1C1 * vector.Y + R1C2 * vector.Z);
            result.Z = (float)(R2C0 * vector.X + R2C1 * vector.Y + R2C2 * vector.Z);
        }
        public static void Transform(ref Matrix3f matrix, ref Vector3d vector, out Vector3d result)
        {
            result.X = (float)(matrix.R0C0 * vector.X + matrix.R0C1 * vector.Y + matrix.R0C2 * vector.Z);
            result.Y = (float)(matrix.R1C0 * vector.X + matrix.R1C1 * vector.Y + matrix.R1C2 * vector.Z);
            result.Z = (float)(matrix.R2C0 * vector.X + matrix.R2C1 * vector.Y + matrix.R2C2 * vector.Z);
        }

        public void Rotate(float angle)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin =  System.Math.Sin(angleRadians);
            double cos =  System.Math.Cos(angleRadians);

            double r0c0 = cos * R0C0 + sin * R1C0;
            double r0c1 = cos * R0C1 + sin * R1C1;
            double r0c2 = cos * R0C2 + sin * R1C2;

            R1C0 = (float)(cos * R1C0 - sin * R0C0);
            R1C1 = (float)(cos * R1C1 - sin * R0C1);
            R1C2 = (float)(cos * R1C2 - sin * R0C2);

            R0C0 = (float)(r0c0);
            R0C1 = (float)(r0c1);
            R0C2 = (float)(r0c2);
        }
        public void Rotate(float angle, out Matrix3f result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos =  System.Math.Cos(angleRadians);

            result.R0C0 = (float)(cos * R0C0 + sin * R1C0);
            result.R0C1 = (float)(cos * R0C1 + sin * R1C1);
            result.R0C2 = (float)(cos * R0C2 + sin * R1C2);
            result.R1C0 = (float)(cos * R1C0 - sin * R0C0);
            result.R1C1 = (float)(cos * R1C1 - sin * R0C1);
            result.R1C2 = (float)(cos * R1C2 - sin * R0C2);
            result.R2C0 = (float)(R2C0);
            result.R2C1 = (float)(R2C1);
            result.R2C2 = (float)(R2C2);
        }
        public static void Rotate(ref Matrix3f matrix, float angle, out Matrix3f result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos = System.Math.Cos(angleRadians);

            result.R0C0 = (float)(cos * matrix.R0C0 + sin * matrix.R1C0);
            result.R0C1 = (float)(cos * matrix.R0C1 + sin * matrix.R1C1);
            result.R0C2 = (float)(cos * matrix.R0C2 + sin * matrix.R1C2);
            result.R1C0 = (float)(cos * matrix.R1C0 - sin * matrix.R0C0);
            result.R1C1 = (float)(cos * matrix.R1C1 - sin * matrix.R0C1);
            result.R1C2 = (float)(cos * matrix.R1C2 - sin * matrix.R0C2);
            result.R2C0 =(float)( matrix.R2C0);
            result.R2C1 = (float)(matrix.R2C1);
            result.R2C2 = (float)(matrix.R2C2);
        }
        public static void RotateMatrix(float angle, out Matrix3f result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos = System.Math.Cos(angleRadians);

            result.R0C0 =  (float)cos;
            result.R0C1 =  (float)sin;
            result.R0C2 =  (float)0;
            result.R1C0 =  (float)-sin;
            result.R1C1 =  (float)cos;
            result.R1C2 =  (float)0;
            result.R2C0 =  (float)0;
            result.R2C1 =  (float)0;
            result.R2C2 =  (float)1;
        }

        public Quaterniond ToQuaternion()
        {
            //return new Quaterniond(ref this);
            throw new NotImplementedException();
		}

        #endregion

        #region Constants

		
		/// <summary>
        /// Defines the size of the Matrix3f struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(new Matrix3f());

        /// <summary>The identity matrix.</summary>
        public static readonly Matrix3f Identity = new Matrix3f
        (
            1, 0, 0,
            0, 1, 0,
            0, 0, 1
        );

        /// <summary>A matrix of all zeros.</summary>
        public static readonly Matrix3f Zero = new Matrix3f
        (
            0, 0, 0,
            0, 0, 0,
            0, 0, 0
        );

        #endregion

        #region HashCode

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return
                R0C0.GetHashCode() ^ R0C1.GetHashCode() ^ R0C2.GetHashCode() ^
                R1C0.GetHashCode() ^ R1C1.GetHashCode() ^ R1C2.GetHashCode() ^
                R2C0.GetHashCode() ^ R2C1.GetHashCode() ^ R2C2.GetHashCode();
        }

        #endregion

        #region String

        /// <summary>Returns the fully qualified type name of this instance.</summary>
        /// <returns>A System.String containing left fully qualified type name.</returns>
        public override string ToString()
        {
            return String.Format(
                "|{00}, {01}, {02}|\n" +
                "|{03}, {04}, {05}|\n" +
                "|{06}, {07}, {18}|\n" +
                R0C0, R0C1, R0C2,
                R1C0, R1C1, R1C2,
                R2C0, R2C1, R2C2);
        }

        #endregion
    }

    /// <summary>
    /// Represents a 4x4 Matrix
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix4f : IEquatable<Matrix4f>
    {
        #region Fields

        /// <summary>
        /// Top row of the matrix
        /// </summary>
        public Vector4 Row0;
        /// <summary>
        /// 2nd row of the matrix
        /// </summary>
        public Vector4 Row1;
        /// <summary>
        /// 3rd row of the matrix
        /// </summary>
        public Vector4 Row2;
        /// <summary>
        /// Bottom row of the matrix
        /// </summary>
        public Vector4 Row3;
 
        /// <summary>
        /// The identity matrix
        /// </summary>
        public static Matrix4f Identity = new Matrix4f(Vector4.UnitX, Vector4.UnitY, Vector4.UnitZ, Vector4.UnitW);

		 /// <summary>
        /// The zero matrix
        /// </summary>
		public static Matrix4f Zero = new Matrix4f(Vector4.Zero, Vector4.Zero, Vector4.Zero, Vector4.Zero);


		/// <summary>
        /// Defines the size of the Matrix4f struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(new Matrix4f());

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="row0">Top row of the matrix</param>
        /// <param name="row1">Second row of the matrix</param>
        /// <param name="row2">Third row of the matrix</param>
        /// <param name="row3">Bottom row of the matrix</param>
        public Matrix4f(Vector4 row0, Vector4 row1, Vector4 row2, Vector4 row3)
        {
            Row0 = row0;
            Row1 = row1;
            Row2 = row2;
            Row3 = row3;
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="m00">First item of the first row of the matrix.</param>
        /// <param name="m01">Second item of the first row of the matrix.</param>
        /// <param name="m02">Third item of the first row of the matrix.</param>
        /// <param name="m03">Fourth item of the first row of the matrix.</param>
        /// <param name="m10">First item of the second row of the matrix.</param>
        /// <param name="m11">Second item of the second row of the matrix.</param>
        /// <param name="m12">Third item of the second row of the matrix.</param>
        /// <param name="m13">Fourth item of the second row of the matrix.</param>
        /// <param name="m20">First item of the third row of the matrix.</param>
        /// <param name="m21">Second item of the third row of the matrix.</param>
        /// <param name="m22">Third item of the third row of the matrix.</param>
        /// <param name="m23">First item of the third row of the matrix.</param>
        /// <param name="m30">Fourth item of the fourth row of the matrix.</param>
        /// <param name="m31">Second item of the fourth row of the matrix.</param>
        /// <param name="m32">Third item of the fourth row of the matrix.</param>
        /// <param name="m33">Fourth item of the fourth row of the matrix.</param>
        public Matrix4f(
            float m00, float m01, float m02, float m03,
            float m10, float m11, float m12, float m13,
            float m20, float m21, float m22, float m23,
            float m30, float m31, float m32, float m33)
        {
            Row0 = new Vector4(m00, m01, m02, m03);
            Row1 = new Vector4(m10, m11, m12, m13);
            Row2 = new Vector4(m20, m21, m22, m23);
            Row3 = new Vector4(m30, m31, m32, m33);
        }
		
		/// <summary>Constructs left matrix from the given array of float-precision floating-point numbers.</summary>
        /// <param name="floatArray">The array of floats for the components of the matrix.</param>
        public Matrix4f(float[] floatArray)
        {
            if (floatArray == null || floatArray.GetLength(0) < 16) throw new MissingFieldException();
			Row0 = new Vector4(floatArray[0], floatArray[1], floatArray[2], floatArray[3]);
            Row1 = new Vector4(floatArray[4], floatArray[5], floatArray[6], floatArray[7]);
            Row2 = new Vector4(floatArray[8], floatArray[9], floatArray[10], floatArray[11]);
            Row3 = new Vector4(floatArray[12], floatArray[13], floatArray[14], floatArray[15]);
        }
        
		/// <summary>Converts the matrix into an array of floats.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>An array of floats for the matrix in Column-major order.</returns>
        public static explicit operator float[](Matrix4f matrix)
        {
            return new float[16]
            {
                matrix.Row0.X,
                matrix.Row1.X,
                matrix.Row2.X,
                matrix.Row3.X,
                matrix.Row0.Y,
                matrix.Row1.Y,
                matrix.Row2.Y,
                matrix.Row3.Y,
                matrix.Row0.Z,
                matrix.Row1.Z,
                matrix.Row2.Z,
                matrix.Row3.Z,
                matrix.Row0.W,
                matrix.Row1.W,
                matrix.Row2.W,
                matrix.Row3.W
            };
        }
		
		#endregion

        #region Public Members

        #region Properties

        /// <summary>
        /// The determinant of this matrix
        /// </summary>
        public float Determinant
        {
            get
            {
                return
                    Row0.X * Row1.Y * Row2.Z * Row3.W - Row0.X * Row1.Y * Row2.W * Row3.Z + Row0.X * Row1.Z * Row2.W * Row3.Y - Row0.X * Row1.Z * Row2.Y * Row3.W
                  + Row0.X * Row1.W * Row2.Y * Row3.Z - Row0.X * Row1.W * Row2.Z * Row3.Y - Row0.Y * Row1.Z * Row2.W * Row3.X + Row0.Y * Row1.Z * Row2.X * Row3.W
                  - Row0.Y * Row1.W * Row2.X * Row3.Z + Row0.Y * Row1.W * Row2.Z * Row3.X - Row0.Y * Row1.X * Row2.Z * Row3.W + Row0.Y * Row1.X * Row2.W * Row3.Z
                  + Row0.Z * Row1.W * Row2.X * Row3.Y - Row0.Z * Row1.W * Row2.Y * Row3.X + Row0.Z * Row1.X * Row2.Y * Row3.W - Row0.Z * Row1.X * Row2.W * Row3.Y
                  + Row0.Z * Row1.Y * Row2.W * Row3.X - Row0.Z * Row1.Y * Row2.X * Row3.W - Row0.W * Row1.X * Row2.Y * Row3.Z + Row0.W * Row1.X * Row2.Z * Row3.Y
                  - Row0.W * Row1.Y * Row2.Z * Row3.X + Row0.W * Row1.Y * Row2.X * Row3.Z - Row0.W * Row1.Z * Row2.X * Row3.Y + Row0.W * Row1.Z * Row2.Y * Row3.X;
            }
        }

        /// <summary>
        /// The first column of this matrix
        /// </summary>
        public Vector4 Column0
        {
            get { return new Vector4(Row0.X, Row1.X, Row2.X, Row3.X); }
        }

        /// <summary>
        /// The second column of this matrix
        /// </summary>
        public Vector4 Column1
        {
            get { return new Vector4(Row0.Y, Row1.Y, Row2.Y, Row3.Y); }
        }

        /// <summary>
        /// The third column of this matrix
        /// </summary>
        public Vector4 Column2
        {
            get { return new Vector4(Row0.Z, Row1.Z, Row2.Z, Row3.Z); }
        }

        /// <summary>
        /// The fourth column of this matrix
        /// </summary>
        public Vector4 Column3
        {
            get { return new Vector4(Row0.W, Row1.W, Row2.W, Row3.W); }
        }

        /// <summary>
        /// Gets or sets the value at row 1, column 1 of this instance.
        /// </summary>
        public float M11 { get { return Row0.X; } set { Row0.X = value; } }

        /// <summary>
        /// Gets or sets the value at row 1, column 2 of this instance.
        /// </summary>
        public float M12 { get { return Row0.Y; } set { Row0.Y = value; } }

        /// <summary>
        /// Gets or sets the value at row 1, column 3 of this instance.
        /// </summary>
        public float M13 { get { return Row0.Z; } set { Row0.Z = value; } }

        /// <summary>
        /// Gets or sets the value at row 1, column 4 of this instance.
        /// </summary>
        public float M14 { get { return Row0.W; } set { Row0.W = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 1 of this instance.
        /// </summary>
        public float M21 { get { return Row1.X; } set { Row1.X = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 2 of this instance.
        /// </summary>
        public float M22 { get { return Row1.Y; } set { Row1.Y = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 3 of this instance.
        /// </summary>
        public float M23 { get { return Row1.Z; } set { Row1.Z = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 4 of this instance.
        /// </summary>
        public float M24 { get { return Row1.W; } set { Row1.W = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 1 of this instance.
        /// </summary>
        public float M31 { get { return Row2.X; } set { Row2.X = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 2 of this instance.
        /// </summary>
        public float M32 { get { return Row2.Y; } set { Row2.Y = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 3 of this instance.
        /// </summary>
        public float M33 { get { return Row2.Z; } set { Row2.Z = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 4 of this instance.
        /// </summary>
        public float M34 { get { return Row2.W; } set { Row2.W = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 1 of this instance.
        /// </summary>
        public float M41 { get { return Row3.X; } set { Row3.X = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 2 of this instance.
        /// </summary>
        public float M42 { get { return Row3.Y; } set { Row3.Y = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 3 of this instance.
        /// </summary>
        public float M43 { get { return Row3.Z; } set { Row3.Z = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 4 of this instance.
        /// </summary>
        public float M44 { get { return Row3.W; } set { Row3.W = value; } }

        #endregion

        #region Instance

        #region public void Invert()

        /// <summary>
        /// Converts this instance into its inverse.
        /// </summary>
        public void Invert()
        {
            this = Matrix4f.Invert(this);
        }

        #endregion

        #region public void Transpose()

        /// <summary>
        /// Converts this instance into its transpose.
        /// </summary>
        public void Transpose()
        {
            this = Matrix4f.Transpose(this);
        }

        #endregion

        #endregion

        #region Static
				/// <summary>Converts Matrix4d to Matrix4f.</summary>
        /// <param name="m">The Matrix4d to convert.</param>
        /// <returns>The resulting Matrix4f.</returns>
        public static explicit operator Matrix4f(Matrix4d m)
        {
            return new Matrix4f((float)m.Row0.X, (float)m.Row0.Y, (float)m.Row0.Z, (float)m.Row0.W,
								(float)m.Row1.X, (float)m.Row1.Y, (float)m.Row1.Z, (float)m.Row1.W,
								(float)m.Row2.X, (float)m.Row2.Y, (float)m.Row2.Z, (float)m.Row2.W,
								(float)m.Row3.X, (float)m.Row3.Y, (float)m.Row3.Z, (float)m.Row3.W);
        }
		                #region CreateFromAxisAngle
        
        /// <summary>
        /// Build a rotation matrix from the specified axis/angle rotation.
        /// </summary>
        /// <param name="axis">The axis to rotate about.</param>
        /// <param name="angle">Angle in radians to rotate counter-clockwise (looking in the direction of the given axis).</param>
        /// <param name="result">A matrix instance.</param>
        public static void CreateFromAxisAngle(OpenTK.Vector3 axis, float angle, out Matrix4f result)
        {
            double cos =  System.Math.Cos(-angle);
            double sin =  System.Math.Sin(-angle);
            double t = 1 - cos;

            axis.Normalize();

            result = new Matrix4f((float)(t * axis.X * axis.X + cos), (float)(t * axis.X * axis.Y - sin * axis.Z), (float)(t * axis.X * axis.Z + sin * axis.Y), 0,
                                 (float)(t * axis.X * axis.Y + sin * axis.Z), (float)(t * axis.Y * axis.Y + cos), (float)(t * axis.Y * axis.Z - sin * axis.X), 0,
                                 (float)(t * axis.X * axis.Z - sin * axis.Y), (float)(t * axis.Y * axis.Z + sin * axis.X), (float)(t * axis.Z * axis.Z + cos), 0,
                                 0, 0, 0, 1);
        }
        
        /// <summary>
        /// Build a rotation matrix from the specified axis/angle rotation.
        /// </summary>
        /// <param name="axis">The axis to rotate about.</param>
        /// <param name="angle">Angle in radians to rotate counter-clockwise (looking in the direction of the given axis).</param>
        /// <returns>A matrix instance.</returns>
        public static Matrix4f CreateFromAxisAngle(OpenTK.Vector3 axis, float angle)
        {
            Matrix4f result;
            CreateFromAxisAngle(axis, angle, out result);
            return result;
        }
        
        #endregion

        #region CreateRotation[XYZ]

        /// <summary>
        /// Builds a rotation matrix for a rotation around the x-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <param name="result">The resulting Matrix4f instance.</param>
        public static void CreateRotationX(double angle, out Matrix4f result)
        {
            double cos =  System.Math.Cos(angle);
            double sin =  System.Math.Sin(angle);

            result.Row0 = Vector4.UnitX;
            result.Row1 = new Vector4(0, (float)(cos), (float)(sin), 0);
            result.Row2 = new Vector4(0, (float)(-sin), (float)(cos), 0);
            result.Row3 = Vector4.UnitW;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the x-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting Matrix4f instance.</returns>
        public static Matrix4f CreateRotationX(double angle)
        {
            Matrix4f result;
            CreateRotationX(angle, out result);
            return result;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y-axis.
        ///
		///   | cos  0.0 -sin 0.0 |
		///   | 0.0  1.0  1.0 0.0 |
		///   | sin  0.0  cos 0.0 |
		///   | 0.0  0.0  0.0 1.0 |
		///
		/// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <param name="result">The resulting Matrix4f instance.</param>
        public static void CreateRotationY(double angle, out Matrix4f result)
        {
            double cos = (float)System.Math.Cos(angle);
            double sin = (float)System.Math.Sin(angle);

            result.Row0 = new Vector4((float)cos, 0, (float)-sin, 0);
            result.Row1 = Vector4.UnitY;
            result.Row2 = new Vector4((float)sin, 0, (float)cos, 0);
            result.Row3 = Vector4.UnitW;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y-axis.
        ///
		///   | cos  0.0 -sin 0.0 |
		///   | 0.0  1.0  1.0 0.0 |
		///   | sin  0.0  cos 0.0 |
		///   | 0.0  0.0  0.0 1.0 |
		///
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting Matrix4f instance.</returns>
        public static Matrix4f CreateRotationY(double angle)
        {
            Matrix4f result;
            CreateRotationY(angle, out result);
            return result;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the z-axis.
		///
		///   | cos -sin 0.0 0.0 |
		///   | sin  cos 0.0 0.0 |
		///   | 0.0  0.0 1.0 0.0 |
		///   | 0.0  0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <param name="result">The resulting Matrix4f instance.</param>
        public static void CreateRotationZ(double angle, out Matrix4f result)
        {
            double cos =  System.Math.Cos(angle);
            double sin =  System.Math.Sin(angle);

            result.Row0 = new Vector4((float)cos, -(float)sin, 0, 0);
            result.Row1 = new Vector4((float)sin, (float)cos, 0, 0);
            result.Row2 = Vector4.UnitZ;
            result.Row3 = Vector4.UnitW;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the z-axis.
		///
		///   | cos -sin 0.0 0.0 |
		///   | sin  cos 0.0 0.0 |
		///   | 0.0  0.0 1.0 0.0 |
		///   | 0.0  0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting Matrix4f instance.</returns>
        public static Matrix4f CreateRotationZ(double angle)
        {
            Matrix4f result;
            CreateRotationZ(angle, out result);
            return result;
        }

		/// <summary>
        /// Creates a matrix from a given angle around a given axis
        /// </summary>
        /// <param name="rad">the angle to rotate the matrix by</param>
        /// <param name="x">the x component of the axis to rotate around</param>
        /// <param name="y">the y component of the axis to rotate around</param>
        /// <param name="z">the z component of the axis to rotate around</param>
        /// <returns></returns>
        public static Matrix4f CreateRotation(float rad, float x, float y, float z)
        {
            Matrix4f result = new Matrix4f();
            double len = System.Math.Sqrt(x * x + y * y + z * z);
            if (System.Math.Abs(len) < 0.000001)
                throw new ArgumentOutOfRangeException("Small length of vector.");

            len = 1 / len;
            x = (float)(x * len);
            y = (float)(y * len);
            z = (float)(z * len);

            double s = System.Math.Sin(rad);
            double c = System.Math.Cos(rad);
            double t = 1 - c;

            // Construct the elements of the rotation matrix
            result.M11 = (float)(x * x * t + c);
            result.M12 = (float)(y * x * t + z * s);
            result.M13 = (float)(z * x * t - y * s);
            result.M21 = (float)(x * y * t - z * s);
            result.M22 = (float)(y * y * t + c);
            result.M23 = (float)(z * y * t + x * s);
            result.M31 = (float)(x * z * t + y * s);
            result.M32 = (float)(y * z * t - x * s);
            result.M33 = (float)(z * z * t + c);
            result.M44 = 1;

            return result;
        }

        #endregion

        #region CreateTranslation
		
		/// <summary>
        /// Multiplay a matrix by a translation matrix.
		///
		///   | 1.0 0.0 0.0 tx  |
		///   | 0.0 1.0 0.0 ty  |
		///   | 0.0 0.0 1.0 tz  |
		///   | 0.0 0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="mat">The original and resulting Matrix4f instance.</param>
        /// <param name="x">X translation.</param>
        /// <param name="y">Y translation.</param>
        /// <param name="z">Z translation.</param>
        public static Matrix4f Translate(Matrix4f mat, float x, float y, float z)
        {
			mat.M14 = mat.M11 * x + mat.M12 * y + mat.M13 * z + mat.M14;
		    mat.M24 = mat.M21 * x + mat.M22 * y + mat.M23 * z + mat.M24;
            mat.M34 = mat.M31 * x + mat.M32 * y + mat.M33 * z + mat.M34;
            mat.M44 = mat.M41 * x + mat.M42 * y + mat.M43 * z + mat.M44;
			return mat;
        }


        /// <summary>
        /// Creates a translation matrix.
		///
		///   | 1.0 0.0 0.0 tx  |
		///   | 0.0 1.0 0.0 ty  |
		///   | 0.0 0.0 1.0 tz  |
		///   | 0.0 0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="x">X translation.</param>
        /// <param name="y">Y translation.</param>
        /// <param name="z">Z translation.</param>
        /// <param name="result">The resulting Matrix4f instance.</param>
        public static void CreateTranslation(float x, float y, float z, out Matrix4f result)
        {
            result = Identity;
            result.Row0.W = x;
            result.Row1.W = y;
            result.Row2.W = z;
        }

        /// <summary>
        /// Creates a translation matrix.
		///
		///   | 1.0 0.0 0.0 tx  |
		///   | 0.0 1.0 0.0 ty  |
		///   | 0.0 0.0 1.0 tz  |
		///   | 0.0 0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="vector">The translation vector.</param>
        /// <param name="result">The resulting Matrix4f instance.</param>
        public static void CreateTranslation(ref Vector3 vector, out Matrix4f result)
        {
            result = Identity;
			result.Row0.W = vector.X;
            result.Row1.W = vector.Y;
            result.Row2.W = vector.Z;
        }

        /// <summary>
        /// Creates a translation matrix.
		///
		///   | 1.0 0.0 0.0 tx  |
		///   | 0.0 1.0 0.0 ty  |
		///   | 0.0 0.0 1.0 tz  |
		///   | 0.0 0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="x">X translation.</param>
        /// <param name="y">Y translation.</param>
        /// <param name="z">Z translation.</param>
        /// <returns>The resulting Matrix4f instance.</returns>
        public static Matrix4f CreateTranslation(float x, float y, float z)
        {
            Matrix4f result;
            CreateTranslation(x, y, z, out result);
            return result;
        }

        /// <summary>
        /// Creates a translation matrix.
		///
		///   | 1.0 0.0 0.0 tx  |
		///   | 0.0 1.0 0.0 ty  |
		///   | 0.0 0.0 1.0 tz  |
		///   | 0.0 0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="vector">The translation vector.</param>
        /// <returns>The resulting Matrix4f instance.</returns>
        public static Matrix4f CreateTranslation(Vector3 vector)
        {
            Matrix4f result;
            CreateTranslation(vector.X, vector.Y, vector.Z, out result);
            return result;
        }

        #endregion

        #region CreateOrthographic

        /// <summary>
        /// Creates an orthographic projection matrix.
        /// </summary>
        /// <param name="width">The width of the projection volume.</param>
        /// <param name="height">The height of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <param name="result">The resulting Matrix4f instance.</param>
        public static void CreateOrthographic(float width, float height, float zNear, float zFar, out Matrix4f result)
        {
            CreateOrthographicOffCenter(-width / 2, width / 2, -height / 2, height / 2, zNear, zFar, out result);
        }

        /// <summary>
        /// Creates an orthographic projection matrix.
        /// </summary>
        /// <param name="width">The width of the projection volume.</param>
        /// <param name="height">The height of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <rereturns>The resulting Matrix4f instance.</rereturns>
        public static Matrix4f CreateOrthographic(float width, float height, float zNear, float zFar)
        {
            Matrix4f result;
            CreateOrthographicOffCenter(-width / 2, width / 2, -height / 2, height / 2, zNear, zFar, out result);
            return result;
        }

        #endregion

        #region CreateOrthographicOffCenter

        /// <summary>
        /// Creates an orthographic projection matrix.
        /// </summary>
        /// <param name="left">The left edge of the projection volume.</param>
        /// <param name="right">The right edge of the projection volume.</param>
        /// <param name="bottom">The bottom edge of the projection volume.</param>
        /// <param name="top">The top edge of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <param name="result">The resulting Matrix4f instance.</param>
        public static void CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNear, float zFar, out Matrix4f result)
        {
            result = new Matrix4f();

            float invLR = 1 / (left - right);
            float invBT = 1 / (bottom - top);
            float invNF = 1 / (zNear - zFar);

            result.M11 = -2 * invLR;
            result.M22 = -2 * invBT;
            result.M33 =  2 * invNF;

            result.M14 = (right + left) * invLR;
            result.M24 = (top + bottom) * invBT;
            result.M34 = (zFar + zNear) * invNF;
            result.M44 = 1;

		}

        /// <summary>
        /// Creates an orthographic projection matrix.
        /// </summary>
        /// <param name="left">The left edge of the projection volume.</param>
        /// <param name="right">The right edge of the projection volume.</param>
        /// <param name="bottom">The bottom edge of the projection volume.</param>
        /// <param name="top">The top edge of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <returns>The resulting Matrix4f instance.</returns>
        public static Matrix4f CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNear, float zFar)
        {
            Matrix4f result;
            CreateOrthographicOffCenter(left, right, bottom, top, zNear, zFar, out result);
            return result;
        }

        #endregion
        
        #region CreatePerspectiveFieldOfView
        
        /// <summary>
        /// Creates a perspective projection matrix.
        /// </summary>
        /// <param name="fovy">Angle of the field of view in the y direction (in radians)</param>
        /// <param name="aspect">Aspect ratio of the view (width / height)</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <param name="result">A projection matrix that transforms camera space to raster space</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown under the following conditions:
        /// <list type="bullet">
        /// <item>fovy is zero, less than zero or larger than Math.PI</item>
        /// <item>aspect is negative or zero</item>
        /// <item>zNear is negative or zero</item>
        /// <item>zFar is negative or zero</item>
        /// <item>zNear is larger than zFar</item>
        /// </list>
        /// </exception>
        public static void CreatePerspectiveFieldOfView(float fovy, float aspect, float zNear, float zFar, out Matrix4f result)
        {
            if (fovy <= 0 || fovy > System.Math.PI)
                throw new ArgumentOutOfRangeException("fovy");
            if (aspect <= 0)
                throw new ArgumentOutOfRangeException("aspect");
            if (zNear <= 0)
                throw new ArgumentOutOfRangeException("zNear");
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException("zFar");
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException("zNear");
            
            float yMax = zNear * (float)System.Math.Tan(0.5f * fovy);
            float yMin = -yMax;
            float xMin = yMin * aspect;
            float xMax = yMax * aspect;

            CreatePerspectiveOffCenter(xMin, xMax, yMin, yMax, zNear, zFar, out result);
        }
        
        /// <summary>
        /// Creates a perspective projection matrix.
        /// </summary>
        /// <param name="fovy">Angle of the field of view in the y direction (in radians)</param>
        /// <param name="aspect">Aspect ratio of the view (width / height)</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <returns>A projection matrix that transforms camera space to raster space</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown under the following conditions:
        /// <list type="bullet">
        /// <item>fovy is zero, less than zero or larger than Math.PI</item>
        /// <item>aspect is negative or zero</item>
        /// <item>zNear is negative or zero</item>
        /// <item>zFar is negative or zero</item>
        /// <item>zNear is larger than zFar</item>
        /// </list>
        /// </exception>
        public static Matrix4f CreatePerspectiveFieldOfView(float fovy, float aspect, float zNear, float zFar)
        {
            Matrix4f result;
            CreatePerspectiveFieldOfView(fovy, aspect, zNear, zFar, out result);
            return result;
        }
        
        #endregion
        
        #region CreatePerspectiveOffCenter
        
        /// <summary>
        /// Creates an perspective projection matrix.
        /// </summary>
        /// <param name="left">Left edge of the view frustum</param>
        /// <param name="right">Right edge of the view frustum</param>
        /// <param name="bottom">Bottom edge of the view frustum</param>
        /// <param name="top">Top edge of the view frustum</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <param name="result">A projection matrix that transforms camera space to raster space</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown under the following conditions:
        /// <list type="bullet">
        /// <item>zNear is negative or zero</item>
        /// <item>zFar is negative or zero</item>
        /// <item>zNear is larger than zFar</item>
        /// </list>
        /// </exception>
        public static void CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float zNear, float zFar, out Matrix4f result)
        {
            if (zNear <= 0)
                throw new ArgumentOutOfRangeException("zNear");
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException("zFar");
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException("zNear");
            
            float x = (float)((2  * zNear) / (right - left));
            float y = (float)((2  * zNear) / (top - bottom));
            float a = (float)((right + left) / (right - left));
            float b = (float)((top + bottom) / (top - bottom));
            float c = (float)((zFar + zNear) / (zNear - zFar ));
            float d = (float)((2  * zFar * zNear) / (zNear - zFar));
            
            result = new Matrix4f(x, 0,  a, 0,
								  0, y,  b, 0,
								  0, b,  c, d,
							 	  0, 0, -1, 0);
        }
        
        /// <summary>
        /// Creates an perspective projection matrix.
        /// </summary>
        /// <param name="left">Left edge of the view frustum</param>
        /// <param name="right">Right edge of the view frustum</param>
        /// <param name="bottom">Bottom edge of the view frustum</param>
        /// <param name="top">Top edge of the view frustum</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <returns>A projection matrix that transforms camera space to raster space</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown under the following conditions:
        /// <list type="bullet">
        /// <item>zNear is negative or zero</item>
        /// <item>zFar is negative or zero</item>
        /// <item>zNear is larger than zFar</item>
        /// </list>
        /// </exception>
        public static Matrix4f CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float zNear, float zFar)
        {
            Matrix4f result;
            CreatePerspectiveOffCenter(left, right, bottom, top, zNear, zFar, out result);
            return result;
        }
        
        #endregion

        #region Scale Functions

        /// <summary>
        /// Build a scaling matrix
		///
		///   | sx  0.0 0.0 0.0 |
		///   | 0.0 sy  0.0 0.0 |
		///   | 0.0 0.0 sz  0.0 |
		///   | 0.0 0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="scale">Single scale factor for x,y and z axes</param>
        /// <returns>A scaling matrix</returns>
        public static Matrix4f Scale(float scale)
        {
            return Scale(scale, scale, scale);
        }

        /// <summary>
        /// Build a scaling matrix
		///
		///   | sx  0.0 0.0 0.0 |
		///   | 0.0 sy  0.0 0.0 |
		///   | 0.0 0.0 sz  0.0 |
		///   | 0.0 0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="scale">Scale factors for x,y and z axes</param>
        /// <returns>A scaling matrix</returns>
        public static Matrix4f Scale(Vector3 scale)
        {
            return Scale(scale.X, scale.Y, scale.Z);
        }

        /// <summary>
        /// Build a scaling matrix
		///
		///   | sx  0.0 0.0 0.0 |
		///   | 0.0 sy  0.0 0.0 |
		///   | 0.0 0.0 sz  0.0 |
		///   | 0.0 0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="x">Scale factor for x-axis</param>
        /// <param name="y">Scale factor for y-axis</param>
        /// <param name="z">Scale factor for z-axis</param>
        /// <returns>A scaling matrix</returns>
        public static Matrix4f Scale(float x, float y, float z)
        {
            Matrix4f result;
            result.Row0 = Vector4.UnitX * x;
            result.Row1 = Vector4.UnitY * y;
            result.Row2 = Vector4.UnitZ * z;
            result.Row3 = Vector4.UnitW;
            return result;
        }
        

		/// <summary>
        /// Build a scaling matrix
		///
		///   | sx  0.0 0.0 0.0 |
		///   | 0.0 sy  0.0 0.0 |
		///   | 0.0 0.0 sz  0.0 |
		///   | 0.0 0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="s">Scale factor for all axies</param>
        /// <returns>A scaling matrix</returns>
        public static Matrix4f Scale(Matrix4f matrix, float s)
        {
			return Scale(matrix, s, s, s);
        }

		/// <summary>
        /// Build a scaling matrix
		///
		///   | sx  0.0 0.0 0.0 |
		///   | 0.0 sy  0.0 0.0 |
		///   | 0.0 0.0 sz  0.0 |
		///   | 0.0 0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="x">Scale factor for x-axis</param>
        /// <param name="y">Scale factor for y-axis</param>
        /// <param name="z">Scale factor for z-axis</param>
        /// <returns>A scaling matrix</returns>
        public static Matrix4f Scale(Matrix4f matrix, float x, float y, float z)
        {
			matrix.M11 = matrix.M11 * x;
			matrix.M21 = matrix.M21 * x;
			matrix.M31 = matrix.M31 * x;
			matrix.M41 = matrix.M41 * x;
			matrix.M12 = matrix.M12 * y;
			matrix.M22 = matrix.M22 * y;
			matrix.M32 = matrix.M32 * y;
			matrix.M42 = matrix.M42 * y;
			matrix.M13 = matrix.M13 * z;
			matrix.M23 = matrix.M23 * z;
			matrix.M33 = matrix.M33 * z;
			matrix.M43 = matrix.M43 * z;
			matrix.M14 = matrix.M14;
			matrix.M24 = matrix.M24;
			matrix.M34 = matrix.M34;
			matrix.M44 = matrix.M44;
			return matrix;
        }
        
		#endregion

        #region Rotation Function

        /// <summary>
        /// Build a rotation matrix from a quaternion
        /// </summary>
        /// <param name="q">the quaternion</param>
        /// <returns>A rotation matrix</returns>
        public static Matrix4f Rotate(Quaternion q)
        {
            OpenTK.Vector3 axis;
            float angle;
            q.ToAxisAngle(out axis, out angle);
            return CreateFromAxisAngle(axis, angle);
        }

        #endregion

        #region Camera Helper Functions

        /// <summary>
        /// Build a world space to camera space matrix
        /// </summary>
        /// <param name="eye">Eye (camera) position in world space</param>
        /// <param name="target">Target position in world space</param>
        /// <param name="up">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
        /// <returns>A Matrix4f that transforms world space to camera space</returns>
        public static Matrix4f LookAt(Vector3 eye, Vector3 target, Vector3 up)
        {
            Vector3 z = Vector3.Normalize(eye - target);
            Vector3 x = Vector3.Normalize(Vector3.Cross(up, z));
            Vector3 y = Vector3.Normalize(Vector3.Cross(z, x));

            Matrix4f rot = new Matrix4f(new Vector4(x.X, y.X, z.X, 0),
                                        new Vector4(x.Y, y.Y, z.Y, 0),
                                        new Vector4(x.Z, y.Z, z.Z, 0),
                                        Vector4.UnitW);

            Matrix4f trans = Matrix4f.CreateTranslation(-eye);

            return trans * rot;
        }

        /// <summary>
        /// Build a world space to camera space matrix
        /// </summary>
        /// <param name="eyeX">Eye (camera) position in world space</param>
        /// <param name="eyeY">Eye (camera) position in world space</param>
        /// <param name="eyeZ">Eye (camera) position in world space</param>
        /// <param name="targetX">Target position in world space</param>
        /// <param name="targetY">Target position in world space</param>
        /// <param name="targetZ">Target position in world space</param>
        /// <param name="upX">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
        /// <param name="upY">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
        /// <param name="upZ">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
        /// <returns>A Matrix4f that transforms world space to camera space</returns>
        public static Matrix4f LookAt(float eyeX, float eyeY, float eyeZ, float targetX, float targetY, float targetZ, float upX, float upY, float upZ)
        {
            return LookAt(new Vector3(eyeX, eyeY, eyeZ), new Vector3(targetX, targetY, targetZ), new Vector3(upX, upY, upZ));
        }

		
        #endregion
        #region Multiply Functions

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <returns>A new instance that is the result of the multiplication</returns>
        public static Matrix4f Mult(Matrix4f left, Matrix4f right)
        {
            Matrix4f result;
            Mult(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <param name="result">A new instance that is the result of the multiplication</param>
        public static void Mult(ref Matrix4f left, ref Matrix4f right, out Matrix4f result)
        {
            result = new Matrix4f(
                right.M11 * left.M11 + right.M12 * left.M21 + right.M13 * left.M31 + right.M14 * left.M41,
                right.M11 * left.M12 + right.M12 * left.M22 + right.M13 * left.M32 + right.M14 * left.M42,
                right.M11 * left.M13 + right.M12 * left.M23 + right.M13 * left.M33 + right.M14 * left.M43,
                right.M11 * left.M14 + right.M12 * left.M24 + right.M13 * left.M34 + right.M14 * left.M44,
                right.M21 * left.M11 + right.M22 * left.M21 + right.M23 * left.M31 + right.M24 * left.M41,
                right.M21 * left.M12 + right.M22 * left.M22 + right.M23 * left.M32 + right.M24 * left.M42,
                right.M21 * left.M13 + right.M22 * left.M23 + right.M23 * left.M33 + right.M24 * left.M43,
                right.M21 * left.M14 + right.M22 * left.M24 + right.M23 * left.M34 + right.M24 * left.M44,
                right.M31 * left.M11 + right.M32 * left.M21 + right.M33 * left.M31 + right.M34 * left.M41,
                right.M31 * left.M12 + right.M32 * left.M22 + right.M33 * left.M32 + right.M34 * left.M42,
                right.M31 * left.M13 + right.M32 * left.M23 + right.M33 * left.M33 + right.M34 * left.M43,
                right.M31 * left.M14 + right.M32 * left.M24 + right.M33 * left.M34 + right.M34 * left.M44,
                right.M41 * left.M11 + right.M42 * left.M21 + right.M43 * left.M31 + right.M44 * left.M41,
                right.M41 * left.M12 + right.M42 * left.M22 + right.M43 * left.M32 + right.M44 * left.M42,
                right.M41 * left.M13 + right.M42 * left.M23 + right.M43 * left.M33 + right.M44 * left.M43,
                right.M41 * left.M14 + right.M42 * left.M24 + right.M43 * left.M34 + right.M44 * left.M44);
		}

        #endregion

        #region Invert Functions

        /// <summary>
        /// Calculate the inverse of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to invert</param>
        /// <returns>The inverse of the given matrix if it has one, or the input if it is singular</returns>
        /// <exception cref="InvalidOperationException">Thrown if the Matrix4f is singular.</exception>
        public static Matrix4f Invert(Matrix4f mat)
        {
            int[] colIdx = { 0, 0, 0, 0 };
            int[] rowIdx = { 0, 0, 0, 0 };
            int[] pivotIdx = { -1, -1, -1, -1 };

            // convert the matrix to an array for easy looping
            float[,] inverse = {{(float)mat.Row0.X, (float)mat.Row0.Y, (float)mat.Row0.Z, (float)mat.Row0.W}, 
                                {(float)mat.Row1.X, (float)mat.Row1.Y, (float)mat.Row1.Z, (float)mat.Row1.W}, 
                                {(float)mat.Row2.X, (float)mat.Row2.Y, (float)mat.Row2.Z, (float)mat.Row2.W}, 
                                {(float)mat.Row3.X, (float)mat.Row3.Y, (float)mat.Row3.Z, (float)mat.Row3.W} };
            int icol = 0;
            int irow = 0;
            for (int i = 0; i < 4; i++)
            {
                // Find the largest pivot value
                float maxPivot = 0;
                for (int j = 0; j < 4; j++)
                {
                    if (pivotIdx[j] != 0)
                    {
                        for (int k = 0; k < 4; ++k)
                        {
                            if (pivotIdx[k] == -1)
                            {
                               float absVal = System.Math.Abs(inverse[j, k]);
                                if (absVal > maxPivot)
                                {
                                    maxPivot = absVal;
                                    irow = j;
                                    icol = k;
                                }
                            }
                            else if (pivotIdx[k] > 0)
                            {
                                return mat;
                            }
                        }
                    }
                }

                ++(pivotIdx[icol]);

                // Swap rows over so pivot is on diagonal
                if (irow != icol)
                {
                    for (int k = 0; k < 4; ++k)
                    {
                        float f = inverse[irow, k];
                        inverse[irow, k] = inverse[icol, k];
                        inverse[icol, k] = f;
                    }
                }

                rowIdx[i] = irow;
                colIdx[i] = icol;

                float pivot = inverse[icol, icol];
                // check for singular matrix
                if (pivot == 0)
                {
                    throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
                    //return mat;
                }

                // Scale row so it has a unit diagonal
                float oneOverPivot = 1.0f / pivot;
                inverse[icol, icol] = 1.0f;
                for (int k = 0; k < 4; ++k)
                    inverse[icol, k] *= oneOverPivot;

                // Do elimination of non-diagonal elements
                for (int j = 0; j < 4; ++j)
                {
                    // check this isn't on the diagonal
                    if (icol != j)
                    {
                        float f = inverse[j, icol];
                        inverse[j, icol] = 0;
                        for (int k = 0; k < 4; ++k)
                            inverse[j, k] -= inverse[icol, k] * f;
                    }
                }
            }

            for (int j = 3; j >= 0; --j)
            {
                int ir = rowIdx[j];
                int ic = colIdx[j];
                for (int k = 0; k < 4; ++k)
                {
                    float f = inverse[k, ir];
                    inverse[k, ir] = inverse[k, ic];
                    inverse[k, ic] = f;
                }
            }

            mat.Row0 = new Vector4((float)inverse[0, 0], (float)inverse[0, 1], (float)inverse[0, 2], (float)inverse[0, 3]);
            mat.Row1 = new Vector4((float)inverse[1, 0], (float)inverse[1, 1], (float)inverse[1, 2], (float)inverse[1, 3]);
            mat.Row2 = new Vector4((float)inverse[2, 0], (float)inverse[2, 1], (float)inverse[2, 2], (float)inverse[2, 3]);
            mat.Row3 = new Vector4((float)inverse[3, 0], (float)inverse[3, 1], (float)inverse[3, 2], (float)inverse[3, 3]);
            return mat;
        }

        #endregion

        #region Transpose

        /// <summary>
        /// Calculate the transpose of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to transpose</param>
        /// <returns>The transpose of the given matrix</returns>
        public static Matrix4f Transpose(Matrix4f mat)
        {
            return new Matrix4f(mat.Column0, mat.Column1, mat.Column2, mat.Column3);
        }


        /// <summary>
        /// Calculate the transpose of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to transpose</param>
        /// <param name="result">The result of the calculation</param>
        public static void Transpose(ref Matrix4f mat, out Matrix4f result)
        {
            result.Row0 = mat.Column0;
            result.Row1 = mat.Column1;
            result.Row2 = mat.Column2;
            result.Row3 = mat.Column3;
        }

        #endregion

        #endregion

        #region Operators

        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new Matrix4f which holds the result of the multiplication</returns>
        public static Matrix4f operator *(Matrix4f left, Matrix4f right)
        {
            return Matrix4f.Mult(left, right);
        }

		/// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new Vector3 which holds the result of the multiplication</returns>
        public static Vector3 operator *(Matrix4f left, Vector3 right)
		{
			Vector3 r;

			double fInvW = 1.0 / (left.Row3.X * right.X + left.Row3.Y * right.Y + left.Row3.Z * right.Z + left.Row3.W);

			r.X =  (float)((left.Row0.X * right.X + left.Row0.Y * right.Y + left.Row0.Z * right.Z + left.Row0.W) * fInvW);
			r.Y =  (float)((left.Row1.X * right.X + left.Row1.Y * right.Y + left.Row1.Z * right.Z + left.Row1.W) * fInvW);
			r.Z =  (float)((left.Row2.X * right.X + left.Row2.Y * right.Y + left.Row2.Z * right.Z + left.Row2.W) * fInvW);

			return r;
		}
		
		/// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new Vector3 which holds the result of the multiplication</returns>
        public static Vector4 operator *(Matrix4f left, Vector4 right)
		{
			Vector4 r;

			double fInvW = 1.0 / (left.Row3.X * right.X + left.Row3.Y * right.Y + left.Row3.Z * right.Z + left.Row3.W);

			r.X =  (float)((left.Row0.X * right.X + left.Row0.Y * right.Y + left.Row0.Z * right.Z + left.Row0.W * right.W) * fInvW);
			r.Y =  (float)((left.Row1.X * right.X + left.Row1.Y * right.Y + left.Row1.Z * right.Z + left.Row1.W * right.W) * fInvW);
			r.Z =  (float)((left.Row2.X * right.X + left.Row2.Y * right.Y + left.Row2.Z * right.Z + left.Row2.W * right.W) * fInvW);
			r.W =  (float)((left.Row3.X * right.X + left.Row3.Y * right.Y + left.Row3.Z * right.Z + left.Row3.W * right.W) * fInvW);
			return r;
		}

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(Matrix4f left, Matrix4f right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(Matrix4f left, Matrix4f right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current Matrix4f4.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}\n{1}\n{2}\n{3}", Row0, Row1, Row2, Row3);
        }

        #endregion

        #region public override int GetHashCode()

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            return Row0.GetHashCode() ^ Row1.GetHashCode() ^ Row2.GetHashCode() ^ Row3.GetHashCode();
        }

        #endregion

        #region public override bool Equals(object obj)

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare tresult.</param>
        /// <returns>True if the instances are equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Matrix4f))
                return false;

            return this.Equals((Matrix4f)obj);
        }

        #endregion

        #endregion

        #endregion

        #region IEquatable<Matrix4f> Members

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="other">An matrix to compare with this matrix.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public bool Equals(Matrix4f other)
        {
            return
                Row0 == other.Row0 &&
                Row1 == other.Row1 &&
                Row2 == other.Row2 &&
                Row3 == other.Row3;
        }

        #endregion
    }

	[Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix2d : IEquatable<Matrix2d>
    {
        #region Fields & Access

        /// <summary>Row 0, Column 0</summary>
        public double R0C0;

        /// <summary>Row 0, Column 1</summary>
        public double R0C1;

        /// <summary>Row 1, Column 0</summary>
        public double R1C0;

        /// <summary>Row 1, Column 1</summary>
        public double R1C1;

        /// <summary>Gets the component at the given row and column in the matrix.</summary>
        /// <param name="row">The row of the matrix.</param>
        /// <param name="column">The column of the matrix.</param>
        /// <returns>The component at the given row and column in the matrix.</returns>
        public double this[int row, int column]
        {
            get
            {
                switch( row )
                {
                    case 0:
                        switch (column)
                        {
                            case 0: return R0C0;
                            case 1: return R0C1;
                        }
                        break;

                    case 1:
                        switch (column)
                        {
                            case 0: return R1C0;
                            case 1: return R1C1;
                        }
                        break;
                }

                throw new IndexOutOfRangeException();
            }
            set
            {
                switch( row )
                {
                    case 0:
                        switch (column)
                        {
                            case 0: R0C0 = value; return;
                            case 1: R0C1 = value; return;
                        }
                        break;

                    case 1:
                        switch (column)
                        {
                            case 0: R1C0 = value; return;
                            case 1: R1C1 = value; return;
                        }
                        break;
                }

                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>Gets the component at the index into the matrix.</summary>
        /// <param name="index">The index into the components of the matrix.</param>
        /// <returns>The component at the given index into the matrix.</returns>
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return R0C0;
                    case 1: return R0C1;
                    case 2: return R1C0;
                    case 3: return R1C1;
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0: R0C0 = value; return;
                    case 1: R0C1 = value; return;
                    case 2: R1C0 = value; return;
                    case 3: R1C1 = value; return;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>Converts the matrix into an IntPtr.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>An IntPtr for the matrix.</returns>
        public static explicit operator IntPtr(Matrix2d matrix)
        {
            unsafe
            {
                return (IntPtr)(&matrix.R0C0);
            }
        }

        /// <summary>Converts the matrix into left double*.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>A double* for the matrix.</returns>
        [CLSCompliant(false)]
        unsafe public static explicit operator double*(Matrix2d matrix)
        {
            return &matrix.R0C0;
        }

        /// <summary>Converts the matrix into an array of doubles.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>An array of doubles for the matrix.</returns>
        public static explicit operator double[](Matrix2d matrix)
        {
            return new double[4]
            {
                matrix.R0C0,
                matrix.R0C1,
                matrix.R1C0,
                matrix.R1C1
            };
        }

        #endregion

        #region Constructors

        /// <summary>Constructs left matrix with the same components as the given matrix.</summary>
        /// <param name="vector">The matrix whose components to copy.</param>
        public Matrix2d(ref Matrix2d matrix)
        {
            this.R0C0 = matrix.R0C0;
            this.R0C1 = matrix.R0C1;
            this.R1C0 = matrix.R1C0;
            this.R1C1 = matrix.R1C1;
        }

        /// <summary>Constructs left matrix with the given values.</summary>
        /// <param name="r0c0">The value for row 0 column 0.</param>
        /// <param name="r0c1">The value for row 0 column 1.</param>
        /// <param name="r1c0">The value for row 1 column 0.</param>
        /// <param name="r1c1">The value for row 1 column 1.</param>
        public Matrix2d
        (
            double r0c0,
            double r0c1,
            double r1c0,
            double r1c1
        )
        {
            this.R0C0 = r0c0;
            this.R0C1 = r0c1;
            this.R1C0 = r1c0;
            this.R1C1 = r1c1;
        }

        /// <summary>Constructs left matrix from the given array of double-precision floating-point numbers.</summary>
        /// <param name="doubleArray">The array of doubles for the components of the matrix in Column-major order.</param>
        public Matrix2d(double[] doubleArray)
        {
            if (doubleArray == null || doubleArray.GetLength(0) < 4) throw new MissingFieldException();

            this.R0C0 = doubleArray[0];
            this.R0C1 = doubleArray[1];
            this.R1C0 = doubleArray[2];
            this.R1C1 = doubleArray[3];
        }

        #endregion

        #region Equality

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix3d structure to compare with.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        [CLSCompliant(false)]
        public bool Equals(Matrix2d matrix)
        {
            return
                R0C0 == matrix.R0C0 &&
                R0C1 == matrix.R0C1 &&
                R1C0 == matrix.R1C0 &&
                R1C1 == matrix.R1C1;
        }

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix3d structure to compare to.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public bool Equals(ref Matrix2d matrix)
        {
            return
                R0C0 == matrix.R0C0 &&
                R0C1 == matrix.R0C1 &&
                R1C0 == matrix.R1C0 &&
                R1C1 == matrix.R1C1;
        }

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="left">The left-hand operand.</param>
        /// <param name="right">The right-hand operand.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public static bool Equals(ref Matrix2d left, ref Matrix2d right)
        {
            return
                left.R0C0 == right.R0C0 &&
                left.R0C1 == right.R0C1 &&
                left.R1C0 == right.R1C0 &&
                left.R1C1 == right.R1C1;
        }

        /// <summary>Indicates whether the current matrix is approximately equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix2d structure to compare with.</param>
        /// <param name="tolerance">The limit below which the matrices are considered equal.</param>
        /// <returns>true if the current matrix is approximately equal to the matrix parameter; otherwise, false.</returns>
        public bool EqualsApprox(ref Matrix2d matrix, double tolerance)
        {
            return
                System.Math.Abs(R0C0 - matrix.R0C0) <= tolerance &&
                System.Math.Abs(R0C1 - matrix.R0C1) <= tolerance &&
                System.Math.Abs(R1C0 - matrix.R1C0) <= tolerance &&
                System.Math.Abs(R1C1 - matrix.R1C1) <= tolerance;
        }

        /// <summary>Indicates whether the current matrix is approximately equal to another matrix.</summary>
        /// <param name="left">The left-hand operand.</param>
        /// <param name="right">The right-hand operand.</param>
        /// <param name="tolerance">The limit below which the matrices are considered equal.</param>
        /// <returns>true if the current matrix is approximately equal to the matrix parameter; otherwise, false.</returns>
        public static bool EqualsApprox(ref Matrix2d left, ref Matrix2d right, double tolerance)
        {
            return
                System.Math.Abs(left.R0C0 - right.R0C0) <= tolerance &&
                System.Math.Abs(left.R0C1 - right.R0C1) <= tolerance &&
                System.Math.Abs(left.R1C0 - right.R1C0) <= tolerance &&
                System.Math.Abs(left.R1C1 - right.R1C1) <= tolerance;
        }

        #endregion

        #region Arithmetic Operators


        /// <summary>Add left matrix to this matrix.</summary>
        /// <param name="matrix">The matrix to add.</param>
        public void Add(ref Matrix2d matrix)
        {
            R0C0 = R0C0 + matrix.R0C0;
            R0C1 = R0C1 + matrix.R0C1;
            R1C0 = R1C0 + matrix.R1C0;
            R1C1 = R1C1 + matrix.R1C1;
        }

        /// <summary>Add left matrix to this matrix.</summary>
        /// <param name="matrix">The matrix to add.</param>
        /// <param name="result">The resulting matrix of the addition.</param>
        public void Add(ref Matrix2d matrix, out Matrix2d result)
        {
            result.R0C0 = R0C0 + matrix.R0C0;
            result.R0C1 = R0C1 + matrix.R0C1;
            result.R1C0 = R1C0 + matrix.R1C0;
            result.R1C1 = R1C1 + matrix.R1C1;
        }

        /// <summary>Add left matrix to left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the addition.</param>
        public static void Add(ref Matrix2d left, ref Matrix2d right, out Matrix2d result)
        {
            result.R0C0 = left.R0C0 + right.R0C0;
            result.R0C1 = left.R0C1 + right.R0C1;
            result.R1C0 = left.R1C0 + right.R1C0;
            result.R1C1 = left.R1C1 + right.R1C1;
        }


        /// <summary>Subtract left matrix from this matrix.</summary>
        /// <param name="matrix">The matrix to subtract.</param>
        public void Subtract(ref Matrix2d matrix)
        {
            R0C0 = R0C0 + matrix.R0C0;
            R0C1 = R0C1 + matrix.R0C1;
            R1C0 = R1C0 + matrix.R1C0;
            R1C1 = R1C1 + matrix.R1C1;
        }

        /// <summary>Subtract left matrix from this matrix.</summary>
        /// <param name="matrix">The matrix to subtract.</param>
        /// <param name="result">The resulting matrix of the subtraction.</param>
        public void Subtract(ref Matrix2d matrix, out Matrix2d result)
        {
            result.R0C0 = R0C0 + matrix.R0C0;
            result.R0C1 = R0C1 + matrix.R0C1;
            result.R1C0 = R1C0 + matrix.R1C0;
            result.R1C1 = R1C1 + matrix.R1C1;
        }

        /// <summary>Subtract left matrix from left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the subtraction.</param>
        public static void Subtract(ref Matrix2d left, ref Matrix2d right, out Matrix2d result)
        {
            result.R0C0 = left.R0C0 + right.R0C0;
            result.R0C1 = left.R0C1 + right.R0C1;
            result.R1C0 = left.R1C0 + right.R1C0;
            result.R1C1 = left.R1C1 + right.R1C1;
        }


        /// <summary>Multiply left martix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        public void Multiply(ref Matrix2d matrix)
        {
            double r0c0 = matrix.R0C0 * R0C0 + matrix.R0C1 * R1C0;
            double r0c1 = matrix.R0C0 * R0C1 + matrix.R0C1 * R1C1;

            double r1c0 = matrix.R1C0 * R0C0 + matrix.R1C1 * R1C0;
            double r1c1 = matrix.R1C0 * R0C1 + matrix.R1C1 * R1C1;

            R0C0 = r0c0;
            R0C1 = r0c1;

            R1C0 = r1c0;
            R1C1 = r1c1;
        }

        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public void Multiply(ref Matrix2d matrix, out Matrix2d result)
        {
            result.R0C0 = matrix.R0C0 * R0C0 + matrix.R0C1 * R1C0;
            result.R0C1 = matrix.R0C0 * R0C1 + matrix.R0C1 * R1C1;
            result.R1C0 = matrix.R1C0 * R0C0 + matrix.R1C1 * R1C0;
            result.R1C1 = matrix.R1C0 * R0C1 + matrix.R1C1 * R1C1;
        }

        /// <summary>Multiply left matrix times left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public static void Multiply(ref Matrix2d left, ref Matrix2d right, out Matrix2d result)
        {
            result.R0C0 = right.R0C0 * left.R0C0 + right.R0C1 * left.R1C0;
            result.R0C1 = right.R0C0 * left.R0C1 + right.R0C1 * left.R1C1;
            result.R1C0 = right.R1C0 * left.R0C0 + right.R1C1 * left.R1C0;
            result.R1C1 = right.R1C0 * left.R0C1 + right.R1C1 * left.R1C1;
        }


        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        public void Multiply(double scalar)
        {
            R0C0 = scalar * R0C0;
            R0C1 = scalar * R0C1;
            R1C0 = scalar * R1C0;
            R1C1 = scalar * R1C1;
        }

        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public void Multiply(double scalar, out Matrix2d result)
        {
            result.R0C0 = scalar * R0C0;
            result.R0C1 = scalar * R0C1;
            result.R1C0 = scalar * R1C0;
            result.R1C1 = scalar * R1C1;
        }

        /// <summary>Multiply left matrix times left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public static void Multiply(ref Matrix3d matrix, double scalar, out Matrix2d result)
        {
            result.R0C0 = scalar * matrix.R0C0;
            result.R0C1 = scalar * matrix.R0C1;
            result.R1C0 = scalar * matrix.R1C0;
            result.R1C1 = scalar * matrix.R1C1;
        }


        #endregion

        #region Functions

        public double Determinant
        {
            get
            {
                return R0C0 * R1C1 - R0C1 * R1C0;
            }
        }

        public void Transpose()
        {
            Std.Swap(ref R0C1, ref R1C0);
        }
        public void Transpose(out Matrix2d result)
        {
            result.R0C0 = R0C0;
            result.R0C1 = R1C0;
            result.R1C0 = R0C1;
            result.R1C1 = R1C1;
        }
        public static void Transpose(ref Matrix2d matrix, out Matrix2d result)
        {
            result.R0C0 = matrix.R0C0;
            result.R0C1 = matrix.R1C0;
            result.R1C0 = matrix.R0C1;
            result.R1C1 = matrix.R1C1;
        }

        #endregion

        #region Transformation Functions

        public void Transform(ref Vector2d vector)
        {
            vector.X = R0C0 * vector.X + R0C1 * vector.Y;
            vector.Y = R1C0 * vector.X + R1C1 * vector.Y;
        }
        public static void Transform(ref Matrix2d matrix, ref Vector2d vector)
        {
            vector.X = matrix.R0C0 * vector.X + matrix.R0C1 * vector.Y;
            vector.Y = matrix.R1C0 * vector.X + matrix.R1C1 * vector.Y;
        }
        public void Transform(ref Vector2d vector, out Vector2d result)
        {
            result.X = R0C0 * vector.X + R0C1 * vector.Y;
            result.Y = R1C0 * vector.X + R1C1 * vector.Y;
        }
        public static void Transform(ref Matrix2d matrix, ref Vector2d vector, out Vector2d result)
        {
            result.X = matrix.R0C0 * vector.X + matrix.R0C1 * vector.Y;
            result.Y = matrix.R1C0 * vector.X + matrix.R1C1 * vector.Y;
        }

        public void Rotate(double angle)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin =  System.Math.Sin(angleRadians);
            double cos =  System.Math.Cos(angleRadians);

            R1C0 = (double)(cos * R1C0 - sin * R0C0);
            R1C1 = (double)(cos * R1C1 - sin * R0C1);

            R0C0 = (double)(cos * R0C0 + sin * R1C0);
            R0C1 = (double)(cos * R0C1 + sin * R1C1);
        }
        public void Rotate(double angle, out Matrix2d result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos =  System.Math.Cos(angleRadians);

            result.R0C0 = (double)(cos * R0C0 + sin * R1C0);
            result.R0C1 = (double)(cos * R0C1 + sin * R1C1);
            result.R1C0 = (double)(cos * R1C0 - sin * R0C0);
            result.R1C1 = (double)(cos * R1C1 - sin * R0C1);
        }
        public static void Rotate(ref Matrix2d matrix, double angle, out Matrix2d result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos = System.Math.Cos(angleRadians);

            result.R0C0 = (double)(cos * matrix.R0C0 + sin * matrix.R1C0);
            result.R0C1 = (double)(cos * matrix.R0C1 + sin * matrix.R1C1);
            result.R1C0 = (double)(cos * matrix.R1C0 - sin * matrix.R0C0);
            result.R1C1 = (double)(cos * matrix.R1C1 - sin * matrix.R0C1);
        }
        public static void RotateMatrix(double angle, out Matrix2d result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos = System.Math.Cos(angleRadians);

            result.R0C0 =  (double)cos;
            result.R0C1 =  (double)sin;
            result.R1C0 =  (double)-sin;
            result.R1C1 =  (double)cos;
        }

        #endregion

        #region Constants

		/// <summary>
        /// Defines the size of the Matrix2d struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(new Matrix2d());

        /// <summary>The identity matrix.</summary>
        public static readonly Matrix2d Identity = new Matrix2d
        (
            1, 0,
            0, 1
        );

        /// <summary>A matrix of all zeros.</summary>
        public static readonly Matrix2d Zero = new Matrix2d
        (
            0, 0,
            0, 0
        );

        #endregion

        #region HashCode

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return
                R0C0.GetHashCode() ^ R0C1.GetHashCode() ^
                R1C0.GetHashCode() ^ R1C1.GetHashCode();
        }

        #endregion

        #region String

        /// <summary>Returns the fully qualified type name of this instance.</summary>
        /// <returns>A System.String containing left fully qualified type name.</returns>
        public override string ToString()
        {
            return String.Format(
                "|{00}, {01}|\n" +
                "|{02}, {03}|\n" +
                R0C0, R0C1, 
                R1C0, R1C1);
        }

        #endregion
    }

	[Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3d : IEquatable<Matrix3d>
    {
        #region Fields & Access

        /// <summary>Row 0, Column 0</summary>
        public double R0C0;

        /// <summary>Row 0, Column 1</summary>
        public double R0C1;

        /// <summary>Row 0, Column 2</summary>
        public double R0C2;

        /// <summary>Row 1, Column 0</summary>
        public double R1C0;

        /// <summary>Row 1, Column 1</summary>
        public double R1C1;

        /// <summary>Row 1, Column 2</summary>
        public double R1C2;

        /// <summary>Row 2, Column 0</summary>
        public double R2C0;

        /// <summary>Row 2, Column 1</summary>
        public double R2C1;

        /// <summary>Row 2, Column 2</summary>
        public double R2C2;

        /// <summary>Gets the component at the given row and column in the matrix.</summary>
        /// <param name="row">The row of the matrix.</param>
        /// <param name="column">The column of the matrix.</param>
        /// <returns>The component at the given row and column in the matrix.</returns>
        public double this[int row, int column]
        {
            get
            {
                switch( row )
                {
                    case 0:
                        switch (column)
                        {
                            case 0: return R0C0;
                            case 1: return R0C1;
                            case 2: return R0C2;
                        }
                        break;

                    case 1:
                        switch (column)
                        {
                            case 0: return R1C0;
                            case 1: return R1C1;
                            case 2: return R1C2;
                        }
                        break;

                    case 2:
                        switch (column)
                        {
                            case 0: return R2C0;
                            case 1: return R2C1;
                            case 2: return R2C2;
                        }
                        break;
                }

                throw new IndexOutOfRangeException();
            }
            set
            {
                switch( row )
                {
                    case 0:
                        switch (column)
                        {
                            case 0: R0C0 = value; return;
                            case 1: R0C1 = value; return;
                            case 2: R0C2 = value; return;
                        }
                        break;

                    case 1:
                        switch (column)
                        {
                            case 0: R1C0 = value; return;
                            case 1: R1C1 = value; return;
                            case 2: R1C2 = value; return;
                        }
                        break;

                    case 2:
                        switch (column)
                        {
                            case 0: R2C0 = value; return;
                            case 1: R2C1 = value; return;
                            case 2: R2C2 = value; return;
                        }
                        break;
                }

                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>Gets the component at the index into the matrix.</summary>
        /// <param name="index">The index into the components of the matrix.</param>
        /// <returns>The component at the given index into the matrix.</returns>
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return R0C0;
                    case 1: return R0C1;
                    case 2: return R0C2;
                    case 3: return R1C0;
                    case 4: return R1C1;
                    case 5: return R1C2;
                    case 6: return R2C0;
                    case 7: return R2C1;
                    case 8: return R2C2;
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0: R0C0 = value; return;
                    case 1: R0C1 = value; return;
                    case 2: R0C2 = value; return;
                    case 3: R1C0 = value; return;
                    case 4: R1C1 = value; return;
                    case 5: R1C2 = value; return;
                    case 6: R2C0 = value; return;
                    case 7: R2C1 = value; return;
                    case 8: R2C2 = value; return;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>Converts the matrix into an IntPtr.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>An IntPtr for the matrix.</returns>
        public static explicit operator IntPtr(Matrix3d matrix)
        {
            unsafe
            {
                return (IntPtr)(&matrix.R0C0);
            }
        }

        /// <summary>Converts the matrix into left double*.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>A double* for the matrix.</returns>
        [CLSCompliant(false)]
        unsafe public static explicit operator double*(Matrix3d matrix)
        {
            return &matrix.R0C0;
        }

        /// <summary>Converts the matrix into an array of doubles.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>An array of doubles for the matrix.</returns>
        public static explicit operator double[](Matrix3d matrix)
        {
            return new double[9]
            {
                matrix.R0C0,
                matrix.R0C1,
                matrix.R0C2,
                matrix.R1C0,
                matrix.R1C1,
                matrix.R1C2,
                matrix.R2C0,
                matrix.R2C1,
                matrix.R2C2
            };
        }

        #endregion

        #region Constructors

        /// <summary>Constructs left matrix with the same components as the given matrix.</summary>
        /// <param name="vector">The matrix whose components to copy.</param>
        public Matrix3d(ref Matrix3d matrix)
        {
            this.R0C0 = matrix.R0C0;
            this.R0C1 = matrix.R0C1;
            this.R0C2 = matrix.R0C2;
            this.R1C0 = matrix.R1C0;
            this.R1C1 = matrix.R1C1;
            this.R1C2 = matrix.R1C2;
            this.R2C0 = matrix.R2C0;
            this.R2C1 = matrix.R2C1;
            this.R2C2 = matrix.R2C2;
        }

        /// <summary>Constructs left matrix with the given values.</summary>
        /// <param name="r0c0">The value for row 0 column 0.</param>
        /// <param name="r0c1">The value for row 0 column 1.</param>
        /// <param name="r0c2">The value for row 0 column 2.</param>
        /// <param name="r1c0">The value for row 1 column 0.</param>
        /// <param name="r1c1">The value for row 1 column 1.</param>
        /// <param name="r1c2">The value for row 1 column 2.</param>
        /// <param name="r2c0">The value for row 2 column 0.</param>
        /// <param name="r2c1">The value for row 2 column 1.</param>
        /// <param name="r2c2">The value for row 2 column 2.</param>
        public Matrix3d
        (
            double r0c0,
            double r0c1,
            double r0c2,
            double r1c0,
            double r1c1,
            double r1c2,
            double r2c0,
            double r2c1,
            double r2c2
        )
        {
            this.R0C0 = r0c0;
            this.R0C1 = r0c1;
            this.R0C2 = r0c2;
            this.R1C0 = r1c0;
            this.R1C1 = r1c1;
            this.R1C2 = r1c2;
            this.R2C0 = r2c0;
            this.R2C1 = r2c1;
            this.R2C2 = r2c2;
        }

        /// <summary>Constructs left matrix from the given array of double-precision floating-point numbers.</summary>
        /// <param name="doubleArray">The array of doubles for the components of the matrix in Column-major order.</param>
        public Matrix3d(double[] doubleArray)
        {
            if (doubleArray == null || doubleArray.GetLength(0) < 9) throw new MissingFieldException();

            this.R0C0 = doubleArray[0];
            this.R0C1 = doubleArray[1];
            this.R0C2 = doubleArray[2];
            this.R1C0 = doubleArray[3];
            this.R1C1 = doubleArray[4];
            this.R1C2 = doubleArray[5];
            this.R2C0 = doubleArray[6];
            this.R2C1 = doubleArray[7];
            this.R2C2 = doubleArray[8];
        }

        /// <summary>Constructs left matrix from the given quaternion.</summary>
        /// <param name="quaternion">The quaternion to use to construct the martix.</param>
        public Matrix3d(Quaterniond quaternion)
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

            R0C0 = (double)(1 - 2 * (yy + zz));
            R0C1 = (double)(2 * (xy - wz));
            R0C2 = (double)(2 * (xz + wy));

            R1C0 = (double)(2 * (xy + wz));
            R1C1 = (double)(1 - 2 * (xx + zz));
            R1C2 = (double)(2 * (yz - wx));

            R2C0 = (double)(2 * (xz - wy));
            R2C1 = (double)(2 * (yz + wx));
            R2C2 = (double)(1 - 2 * (xx + yy));
        }

        #endregion

        #region Equality

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix3d structure to compare with.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        [CLSCompliant(false)]
        public bool Equals(Matrix3d matrix)
        {
            return
                R0C0 == matrix.R0C0 &&
                R0C1 == matrix.R0C1 &&
                R0C2 == matrix.R0C2 &&
                R1C0 == matrix.R1C0 &&
                R1C1 == matrix.R1C1 &&
                R1C2 == matrix.R1C2 &&
                R2C0 == matrix.R2C0 &&
                R2C1 == matrix.R2C1 &&
                R2C2 == matrix.R2C2;
        }

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix3d structure to compare to.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public bool Equals(ref Matrix3d matrix)
        {
            return
                R0C0 == matrix.R0C0 &&
                R0C1 == matrix.R0C1 &&
                R0C2 == matrix.R0C2 &&
                R1C0 == matrix.R1C0 &&
                R1C1 == matrix.R1C1 &&
                R1C2 == matrix.R1C2 &&
                R2C0 == matrix.R2C0 &&
                R2C1 == matrix.R2C1 &&
                R2C2 == matrix.R2C2;
        }

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="left">The left-hand operand.</param>
        /// <param name="right">The right-hand operand.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public static bool Equals(ref Matrix3d left, ref Matrix3d right)
        {
            return
                left.R0C0 == right.R0C0 &&
                left.R0C1 == right.R0C1 &&
                left.R0C2 == right.R0C2 &&
                left.R1C0 == right.R1C0 &&
                left.R1C1 == right.R1C1 &&
                left.R1C2 == right.R1C2 &&
                left.R2C0 == right.R2C0 &&
                left.R2C1 == right.R2C1 &&
                left.R2C2 == right.R2C2;
        }

        /// <summary>Indicates whether the current matrix is approximately equal to another matrix.</summary>
        /// <param name="matrix">The OpenTK.Matrix3d structure to compare with.</param>
        /// <param name="tolerance">The limit below which the matrices are considered equal.</param>
        /// <returns>true if the current matrix is approximately equal to the matrix parameter; otherwise, false.</returns>
        public bool EqualsApprox(ref Matrix3d matrix, double tolerance)
        {
            return
                System.Math.Abs(R0C0 - matrix.R0C0) <= tolerance &&
                System.Math.Abs(R0C1 - matrix.R0C1) <= tolerance &&
                System.Math.Abs(R0C2 - matrix.R0C2) <= tolerance &&
                System.Math.Abs(R1C0 - matrix.R1C0) <= tolerance &&
                System.Math.Abs(R1C1 - matrix.R1C1) <= tolerance &&
                System.Math.Abs(R1C2 - matrix.R1C2) <= tolerance &&
                System.Math.Abs(R2C0 - matrix.R2C0) <= tolerance &&
                System.Math.Abs(R2C1 - matrix.R2C1) <= tolerance &&
                System.Math.Abs(R2C2 - matrix.R2C2) <= tolerance;
        }

        /// <summary>Indicates whether the current matrix is approximately equal to another matrix.</summary>
        /// <param name="left">The left-hand operand.</param>
        /// <param name="right">The right-hand operand.</param>
        /// <param name="tolerance">The limit below which the matrices are considered equal.</param>
        /// <returns>true if the current matrix is approximately equal to the matrix parameter; otherwise, false.</returns>
        public static bool EqualsApprox(ref Matrix3d left, ref Matrix3d right, double tolerance)
        {
            return
                System.Math.Abs(left.R0C0 - right.R0C0) <= tolerance &&
                System.Math.Abs(left.R0C1 - right.R0C1) <= tolerance &&
                System.Math.Abs(left.R0C2 - right.R0C2) <= tolerance &&
                System.Math.Abs(left.R1C0 - right.R1C0) <= tolerance &&
                System.Math.Abs(left.R1C1 - right.R1C1) <= tolerance &&
                System.Math.Abs(left.R1C2 - right.R1C2) <= tolerance &&
                System.Math.Abs(left.R2C0 - right.R2C0) <= tolerance &&
                System.Math.Abs(left.R2C1 - right.R2C1) <= tolerance &&
                System.Math.Abs(left.R2C2 - right.R2C2) <= tolerance;
        }

        #endregion

        #region Arithmetic Operators


        /// <summary>Add left matrix to this matrix.</summary>
        /// <param name="matrix">The matrix to add.</param>
        public void Add(ref Matrix3d matrix)
        {
            R0C0 = R0C0 + matrix.R0C0;
            R0C1 = R0C1 + matrix.R0C1;
            R0C2 = R0C2 + matrix.R0C2;
            R1C0 = R1C0 + matrix.R1C0;
            R1C1 = R1C1 + matrix.R1C1;
            R1C2 = R1C2 + matrix.R1C2;
            R2C0 = R2C0 + matrix.R2C0;
            R2C1 = R2C1 + matrix.R2C1;
            R2C2 = R2C2 + matrix.R2C2;
        }

        /// <summary>Add left matrix to this matrix.</summary>
        /// <param name="matrix">The matrix to add.</param>
        /// <param name="result">The resulting matrix of the addition.</param>
        public void Add(ref Matrix3d matrix, out Matrix3d result)
        {
            result.R0C0 = R0C0 + matrix.R0C0;
            result.R0C1 = R0C1 + matrix.R0C1;
            result.R0C2 = R0C2 + matrix.R0C2;
            result.R1C0 = R1C0 + matrix.R1C0;
            result.R1C1 = R1C1 + matrix.R1C1;
            result.R1C2 = R1C2 + matrix.R1C2;
            result.R2C0 = R2C0 + matrix.R2C0;
            result.R2C1 = R2C1 + matrix.R2C1;
            result.R2C2 = R2C2 + matrix.R2C2;
        }

        /// <summary>Add left matrix to left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the addition.</param>
        public static void Add(ref Matrix3d left, ref Matrix3d right, out Matrix3d result)
        {
            result.R0C0 = left.R0C0 + right.R0C0;
            result.R0C1 = left.R0C1 + right.R0C1;
            result.R0C2 = left.R0C2 + right.R0C2;
            result.R1C0 = left.R1C0 + right.R1C0;
            result.R1C1 = left.R1C1 + right.R1C1;
            result.R1C2 = left.R1C2 + right.R1C2;
            result.R2C0 = left.R2C0 + right.R2C0;
            result.R2C1 = left.R2C1 + right.R2C1;
            result.R2C2 = left.R2C2 + right.R2C2;
        }


        /// <summary>Subtract left matrix from this matrix.</summary>
        /// <param name="matrix">The matrix to subtract.</param>
        public void Subtract(ref Matrix3d matrix)
        {
            R0C0 = R0C0 + matrix.R0C0;
            R0C1 = R0C1 + matrix.R0C1;
            R0C2 = R0C2 + matrix.R0C2;
            R1C0 = R1C0 + matrix.R1C0;
            R1C1 = R1C1 + matrix.R1C1;
            R1C2 = R1C2 + matrix.R1C2;
            R2C0 = R2C0 + matrix.R2C0;
            R2C1 = R2C1 + matrix.R2C1;
            R2C2 = R2C2 + matrix.R2C2;
        }

        /// <summary>Subtract left matrix from this matrix.</summary>
        /// <param name="matrix">The matrix to subtract.</param>
        /// <param name="result">The resulting matrix of the subtraction.</param>
        public void Subtract(ref Matrix3d matrix, out Matrix3d result)
        {
            result.R0C0 = R0C0 + matrix.R0C0;
            result.R0C1 = R0C1 + matrix.R0C1;
            result.R0C2 = R0C2 + matrix.R0C2;
            result.R1C0 = R1C0 + matrix.R1C0;
            result.R1C1 = R1C1 + matrix.R1C1;
            result.R1C2 = R1C2 + matrix.R1C2;
            result.R2C0 = R2C0 + matrix.R2C0;
            result.R2C1 = R2C1 + matrix.R2C1;
            result.R2C2 = R2C2 + matrix.R2C2;
        }

        /// <summary>Subtract left matrix from left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the subtraction.</param>
        public static void Subtract(ref Matrix3d left, ref Matrix3d right, out Matrix3d result)
        {
            result.R0C0 = left.R0C0 + right.R0C0;
            result.R0C1 = left.R0C1 + right.R0C1;
            result.R0C2 = left.R0C2 + right.R0C2;
            result.R1C0 = left.R1C0 + right.R1C0;
            result.R1C1 = left.R1C1 + right.R1C1;
            result.R1C2 = left.R1C2 + right.R1C2;
            result.R2C0 = left.R2C0 + right.R2C0;
            result.R2C1 = left.R2C1 + right.R2C1;
            result.R2C2 = left.R2C2 + right.R2C2;
        }


        /// <summary>Multiply left martix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        public void Multiply(ref Matrix3d matrix)
        {
            double r0c0 = matrix.R0C0 * R0C0 + matrix.R0C1 * R1C0 + matrix.R0C2 * R2C0;
            double r0c1 = matrix.R0C0 * R0C1 + matrix.R0C1 * R1C1 + matrix.R0C2 * R2C1;
            double r0c2 = matrix.R0C0 * R0C2 + matrix.R0C1 * R1C2 + matrix.R0C2 * R2C2;

            double r1c0 = matrix.R1C0 * R0C0 + matrix.R1C1 * R1C0 + matrix.R1C2 * R2C0;
            double r1c1 = matrix.R1C0 * R0C1 + matrix.R1C1 * R1C1 + matrix.R1C2 * R2C1;
            double r1c2 = matrix.R1C0 * R0C2 + matrix.R1C1 * R1C2 + matrix.R1C2 * R2C2;

            R2C0 = matrix.R2C0 * R0C0 + matrix.R2C1 * R1C0 + matrix.R2C2 * R2C0;
            R2C1 = matrix.R2C0 * R0C1 + matrix.R2C1 * R1C1 + matrix.R2C2 * R2C1;
            R2C2 = matrix.R2C0 * R0C2 + matrix.R2C1 * R1C2 + matrix.R2C2 * R2C2;


            R0C0 = r0c0;
            R0C1 = r0c1;
            R0C2 = r0c2;

            R1C0 = r1c0;
            R1C1 = r1c1;
            R1C2 = r1c2;
        }

        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public void Multiply(ref Matrix3d matrix, out Matrix3d result)
        {
            result.R0C0 = matrix.R0C0 * R0C0 + matrix.R0C1 * R1C0 + matrix.R0C2 * R2C0;
            result.R0C1 = matrix.R0C0 * R0C1 + matrix.R0C1 * R1C1 + matrix.R0C2 * R2C1;
            result.R0C2 = matrix.R0C0 * R0C2 + matrix.R0C1 * R1C2 + matrix.R0C2 * R2C2;
            result.R1C0 = matrix.R1C0 * R0C0 + matrix.R1C1 * R1C0 + matrix.R1C2 * R2C0;
            result.R1C1 = matrix.R1C0 * R0C1 + matrix.R1C1 * R1C1 + matrix.R1C2 * R2C1;
            result.R1C2 = matrix.R1C0 * R0C2 + matrix.R1C1 * R1C2 + matrix.R1C2 * R2C2;
            result.R2C0 = matrix.R2C0 * R0C0 + matrix.R2C1 * R1C0 + matrix.R2C2 * R2C0;
            result.R2C1 = matrix.R2C0 * R0C1 + matrix.R2C1 * R1C1 + matrix.R2C2 * R2C1;
            result.R2C2 = matrix.R2C0 * R0C2 + matrix.R2C1 * R1C2 + matrix.R2C2 * R2C2;
        }

        /// <summary>Multiply left matrix times left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public static void Multiply(ref Matrix3d left, ref Matrix3d right, out Matrix3d result)
        {
            result.R0C0 = right.R0C0 * left.R0C0 + right.R0C1 * left.R1C0 + right.R0C2 * left.R2C0;
            result.R0C1 = right.R0C0 * left.R0C1 + right.R0C1 * left.R1C1 + right.R0C2 * left.R2C1;
            result.R0C2 = right.R0C0 * left.R0C2 + right.R0C1 * left.R1C2 + right.R0C2 * left.R2C2;
            result.R1C0 = right.R1C0 * left.R0C0 + right.R1C1 * left.R1C0 + right.R1C2 * left.R2C0;
            result.R1C1 = right.R1C0 * left.R0C1 + right.R1C1 * left.R1C1 + right.R1C2 * left.R2C1;
            result.R1C2 = right.R1C0 * left.R0C2 + right.R1C1 * left.R1C2 + right.R1C2 * left.R2C2;
            result.R2C0 = right.R2C0 * left.R0C0 + right.R2C1 * left.R1C0 + right.R2C2 * left.R2C0;
            result.R2C1 = right.R2C0 * left.R0C1 + right.R2C1 * left.R1C1 + right.R2C2 * left.R2C1;
            result.R2C2 = right.R2C0 * left.R0C2 + right.R2C1 * left.R1C2 + right.R2C2 * left.R2C2;
        }


        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        public void Multiply(double scalar)
        {
            R0C0 = scalar * R0C0;
            R0C1 = scalar * R0C1;
            R0C2 = scalar * R0C2;
            R1C0 = scalar * R1C0;
            R1C1 = scalar * R1C1;
            R1C2 = scalar * R1C2;
            R2C0 = scalar * R2C0;
            R2C1 = scalar * R2C1;
            R2C2 = scalar * R2C2;
        }

        /// <summary>Multiply matrix times this matrix.</summary>
        /// <param name="matrix">The matrix to multiply.</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public void Multiply(double scalar, out Matrix3d result)
        {
            result.R0C0 = scalar * R0C0;
            result.R0C1 = scalar * R0C1;
            result.R0C2 = scalar * R0C2;
            result.R1C0 = scalar * R1C0;
            result.R1C1 = scalar * R1C1;
            result.R1C2 = scalar * R1C2;
            result.R2C0 = scalar * R2C0;
            result.R2C1 = scalar * R2C1;
            result.R2C2 = scalar * R2C2;
        }

        /// <summary>Multiply left matrix times left matrix.</summary>
        /// <param name="matrix">The matrix on the matrix side of the equation.</param>
        /// <param name="right">The matrix on the right side of the equation</param>
        /// <param name="result">The resulting matrix of the multiplication.</param>
        public static void Multiply(ref Matrix3d matrix, double scalar, out Matrix3d result)
        {
            result.R0C0 = scalar * matrix.R0C0;
            result.R0C1 = scalar * matrix.R0C1;
            result.R0C2 = scalar * matrix.R0C2;
            result.R1C0 = scalar * matrix.R1C0;
            result.R1C1 = scalar * matrix.R1C1;
            result.R1C2 = scalar * matrix.R1C2;
            result.R2C0 = scalar * matrix.R2C0;
            result.R2C1 = scalar * matrix.R2C1;
            result.R2C2 = scalar * matrix.R2C2;
        }


        #endregion

        #region Functions

        public double Determinant
        {
            get
            {
                return R0C0 * R1C1 * R2C2 - R0C0 * R1C2 * R2C1 - R0C1 * R1C0 * R2C2 + R0C2 * R1C0 * R2C1 + R0C1 * R1C2 * R2C0 - R0C2 * R1C1 * R2C0;
            }
        }

        public void Transpose()
        {
            Std.Swap(ref R0C1, ref R1C0);
            Std.Swap(ref R0C2, ref R2C0);
            Std.Swap(ref R1C2, ref R2C1);
        }
        public void Transpose(out Matrix3d result)
        {
            result.R0C0 = R0C0;
            result.R0C1 = R1C0;
            result.R0C2 = R2C0;
            result.R1C0 = R0C1;
            result.R1C1 = R1C1;
            result.R1C2 = R2C1;
            result.R2C0 = R0C2;
            result.R2C1 = R1C2;
            result.R2C2 = R2C2;
        }
        public static void Transpose(ref Matrix3d matrix, out Matrix3d result)
        {
            result.R0C0 = matrix.R0C0;
            result.R0C1 = matrix.R1C0;
            result.R0C2 = matrix.R2C0;
            result.R1C0 = matrix.R0C1;
            result.R1C1 = matrix.R1C1;
            result.R1C2 = matrix.R2C1;
            result.R2C0 = matrix.R0C2;
            result.R2C1 = matrix.R1C2;
            result.R2C2 = matrix.R2C2;
        }

        #endregion

        #region Transformation Functions

        public void Transform(ref Vector3d vector)
        {
            double x = R0C0 * vector.X + R0C1 * vector.Y + R0C2 * vector.Z;
            double y = R1C0 * vector.X + R1C1 * vector.Y + R1C2 * vector.Z;
            vector.Z = (double)(R2C0 * vector.X + R2C1 * vector.Y + R2C2 * vector.Z);
            vector.X = x;
            vector.Y = y;
        }
        public static void Transform(ref Matrix3d matrix, ref Vector3d vector)
        {
            double x = (double)(matrix.R0C0 * vector.X + matrix.R0C1 * vector.Y + matrix.R0C2 * vector.Z);
            double y = (double)(matrix.R1C0 * vector.X + matrix.R1C1 * vector.Y + matrix.R1C2 * vector.Z);
            vector.Z = (double)(matrix.R2C0 * vector.X + matrix.R2C1 * vector.Y + matrix.R2C2 * vector.Z);
            vector.X = x;
            vector.Y = y;
        }
        public void Transform(ref Vector3d vector, out Vector3d result)
        {
            result.X = (double)(R0C0 * vector.X + R0C1 * vector.Y + R0C2 * vector.Z);
            result.Y = (double)(R1C0 * vector.X + R1C1 * vector.Y + R1C2 * vector.Z);
            result.Z = (double)(R2C0 * vector.X + R2C1 * vector.Y + R2C2 * vector.Z);
        }
        public static void Transform(ref Matrix3d matrix, ref Vector3d vector, out Vector3d result)
        {
            result.X = (double)(matrix.R0C0 * vector.X + matrix.R0C1 * vector.Y + matrix.R0C2 * vector.Z);
            result.Y = (double)(matrix.R1C0 * vector.X + matrix.R1C1 * vector.Y + matrix.R1C2 * vector.Z);
            result.Z = (double)(matrix.R2C0 * vector.X + matrix.R2C1 * vector.Y + matrix.R2C2 * vector.Z);
        }

        public void Rotate(double angle)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin =  System.Math.Sin(angleRadians);
            double cos =  System.Math.Cos(angleRadians);

            double r0c0 = cos * R0C0 + sin * R1C0;
            double r0c1 = cos * R0C1 + sin * R1C1;
            double r0c2 = cos * R0C2 + sin * R1C2;

            R1C0 = (double)(cos * R1C0 - sin * R0C0);
            R1C1 = (double)(cos * R1C1 - sin * R0C1);
            R1C2 = (double)(cos * R1C2 - sin * R0C2);

            R0C0 = (double)(r0c0);
            R0C1 = (double)(r0c1);
            R0C2 = (double)(r0c2);
        }
        public void Rotate(double angle, out Matrix3d result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos =  System.Math.Cos(angleRadians);

            result.R0C0 = (double)(cos * R0C0 + sin * R1C0);
            result.R0C1 = (double)(cos * R0C1 + sin * R1C1);
            result.R0C2 = (double)(cos * R0C2 + sin * R1C2);
            result.R1C0 = (double)(cos * R1C0 - sin * R0C0);
            result.R1C1 = (double)(cos * R1C1 - sin * R0C1);
            result.R1C2 = (double)(cos * R1C2 - sin * R0C2);
            result.R2C0 = (double)(R2C0);
            result.R2C1 = (double)(R2C1);
            result.R2C2 = (double)(R2C2);
        }
        public static void Rotate(ref Matrix3d matrix, double angle, out Matrix3d result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos = System.Math.Cos(angleRadians);

            result.R0C0 = (double)(cos * matrix.R0C0 + sin * matrix.R1C0);
            result.R0C1 = (double)(cos * matrix.R0C1 + sin * matrix.R1C1);
            result.R0C2 = (double)(cos * matrix.R0C2 + sin * matrix.R1C2);
            result.R1C0 = (double)(cos * matrix.R1C0 - sin * matrix.R0C0);
            result.R1C1 = (double)(cos * matrix.R1C1 - sin * matrix.R0C1);
            result.R1C2 = (double)(cos * matrix.R1C2 - sin * matrix.R0C2);
            result.R2C0 =(double)( matrix.R2C0);
            result.R2C1 = (double)(matrix.R2C1);
            result.R2C2 = (double)(matrix.R2C2);
        }
        public static void RotateMatrix(double angle, out Matrix3d result)
        {
            double angleRadians = Functions.DTOR * angle;
            double sin = System.Math.Sin(angleRadians);
            double cos = System.Math.Cos(angleRadians);

            result.R0C0 =  (double)cos;
            result.R0C1 =  (double)sin;
            result.R0C2 =  (double)0;
            result.R1C0 =  (double)-sin;
            result.R1C1 =  (double)cos;
            result.R1C2 =  (double)0;
            result.R2C0 =  (double)0;
            result.R2C1 =  (double)0;
            result.R2C2 =  (double)1;
        }

        public Quaterniond ToQuaternion()
        {
            //return new Quaterniond(ref this);
            throw new NotImplementedException();
		}

        #endregion

        #region Constants

		
		/// <summary>
        /// Defines the size of the Matrix3d struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(new Matrix3d());

        /// <summary>The identity matrix.</summary>
        public static readonly Matrix3d Identity = new Matrix3d
        (
            1, 0, 0,
            0, 1, 0,
            0, 0, 1
        );

        /// <summary>A matrix of all zeros.</summary>
        public static readonly Matrix3d Zero = new Matrix3d
        (
            0, 0, 0,
            0, 0, 0,
            0, 0, 0
        );

        #endregion

        #region HashCode

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return
                R0C0.GetHashCode() ^ R0C1.GetHashCode() ^ R0C2.GetHashCode() ^
                R1C0.GetHashCode() ^ R1C1.GetHashCode() ^ R1C2.GetHashCode() ^
                R2C0.GetHashCode() ^ R2C1.GetHashCode() ^ R2C2.GetHashCode();
        }

        #endregion

        #region String

        /// <summary>Returns the fully qualified type name of this instance.</summary>
        /// <returns>A System.String containing left fully qualified type name.</returns>
        public override string ToString()
        {
            return String.Format(
                "|{00}, {01}, {02}|\n" +
                "|{03}, {04}, {05}|\n" +
                "|{06}, {07}, {18}|\n" +
                R0C0, R0C1, R0C2,
                R1C0, R1C1, R1C2,
                R2C0, R2C1, R2C2);
        }

        #endregion
    }

    /// <summary>
    /// Represents a 4x4 Matrix
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix4d : IEquatable<Matrix4d>
    {
        #region Fields

        /// <summary>
        /// Top row of the matrix
        /// </summary>
        public Vector4d Row0;
        /// <summary>
        /// 2nd row of the matrix
        /// </summary>
        public Vector4d Row1;
        /// <summary>
        /// 3rd row of the matrix
        /// </summary>
        public Vector4d Row2;
        /// <summary>
        /// Bottom row of the matrix
        /// </summary>
        public Vector4d Row3;
 
        /// <summary>
        /// The identity matrix
        /// </summary>
        public static Matrix4d Identity = new Matrix4d(Vector4d.UnitX, Vector4d.UnitY, Vector4d.UnitZ, Vector4d.UnitW);

		 /// <summary>
        /// The zero matrix
        /// </summary>
		public static Matrix4d Zero = new Matrix4d(Vector4d.Zero, Vector4d.Zero, Vector4d.Zero, Vector4d.Zero);


		/// <summary>
        /// Defines the size of the Matrix4d struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(new Matrix4d());

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="row0">Top row of the matrix</param>
        /// <param name="row1">Second row of the matrix</param>
        /// <param name="row2">Third row of the matrix</param>
        /// <param name="row3">Bottom row of the matrix</param>
        public Matrix4d(Vector4d row0, Vector4d row1, Vector4d row2, Vector4d row3)
        {
            Row0 = row0;
            Row1 = row1;
            Row2 = row2;
            Row3 = row3;
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="m00">First item of the first row of the matrix.</param>
        /// <param name="m01">Second item of the first row of the matrix.</param>
        /// <param name="m02">Third item of the first row of the matrix.</param>
        /// <param name="m03">Fourth item of the first row of the matrix.</param>
        /// <param name="m10">First item of the second row of the matrix.</param>
        /// <param name="m11">Second item of the second row of the matrix.</param>
        /// <param name="m12">Third item of the second row of the matrix.</param>
        /// <param name="m13">Fourth item of the second row of the matrix.</param>
        /// <param name="m20">First item of the third row of the matrix.</param>
        /// <param name="m21">Second item of the third row of the matrix.</param>
        /// <param name="m22">Third item of the third row of the matrix.</param>
        /// <param name="m23">First item of the third row of the matrix.</param>
        /// <param name="m30">Fourth item of the fourth row of the matrix.</param>
        /// <param name="m31">Second item of the fourth row of the matrix.</param>
        /// <param name="m32">Third item of the fourth row of the matrix.</param>
        /// <param name="m33">Fourth item of the fourth row of the matrix.</param>
        public Matrix4d(
            double m00, double m01, double m02, double m03,
            double m10, double m11, double m12, double m13,
            double m20, double m21, double m22, double m23,
            double m30, double m31, double m32, double m33)
        {
            Row0 = new Vector4d(m00, m01, m02, m03);
            Row1 = new Vector4d(m10, m11, m12, m13);
            Row2 = new Vector4d(m20, m21, m22, m23);
            Row3 = new Vector4d(m30, m31, m32, m33);
        }
		
		/// <summary>Constructs left matrix from the given array of double-precision floating-point numbers.</summary>
        /// <param name="doubleArray">The array of doubles for the components of the matrix.</param>
        public Matrix4d(double[] doubleArray)
        {
            if (doubleArray == null || doubleArray.GetLength(0) < 16) throw new MissingFieldException();
			Row0 = new Vector4d(doubleArray[0], doubleArray[1], doubleArray[2], doubleArray[3]);
            Row1 = new Vector4d(doubleArray[4], doubleArray[5], doubleArray[6], doubleArray[7]);
            Row2 = new Vector4d(doubleArray[8], doubleArray[9], doubleArray[10], doubleArray[11]);
            Row3 = new Vector4d(doubleArray[12], doubleArray[13], doubleArray[14], doubleArray[15]);
        }
        
		/// <summary>Converts the matrix into an array of doubles.</summary>
        /// <param name="matrix">The matrix to convert.</param>
        /// <returns>An array of doubles for the matrix in Column-major order.</returns>
        public static explicit operator double[](Matrix4d matrix)
        {
            return new double[16]
            {
                matrix.Row0.X,
                matrix.Row1.X,
                matrix.Row2.X,
                matrix.Row3.X,
                matrix.Row0.Y,
                matrix.Row1.Y,
                matrix.Row2.Y,
                matrix.Row3.Y,
                matrix.Row0.Z,
                matrix.Row1.Z,
                matrix.Row2.Z,
                matrix.Row3.Z,
                matrix.Row0.W,
                matrix.Row1.W,
                matrix.Row2.W,
                matrix.Row3.W
            };
        }
		
		#endregion

        #region Public Members

        #region Properties

        /// <summary>
        /// The determinant of this matrix
        /// </summary>
        public double Determinant
        {
            get
            {
                return
                    Row0.X * Row1.Y * Row2.Z * Row3.W - Row0.X * Row1.Y * Row2.W * Row3.Z + Row0.X * Row1.Z * Row2.W * Row3.Y - Row0.X * Row1.Z * Row2.Y * Row3.W
                  + Row0.X * Row1.W * Row2.Y * Row3.Z - Row0.X * Row1.W * Row2.Z * Row3.Y - Row0.Y * Row1.Z * Row2.W * Row3.X + Row0.Y * Row1.Z * Row2.X * Row3.W
                  - Row0.Y * Row1.W * Row2.X * Row3.Z + Row0.Y * Row1.W * Row2.Z * Row3.X - Row0.Y * Row1.X * Row2.Z * Row3.W + Row0.Y * Row1.X * Row2.W * Row3.Z
                  + Row0.Z * Row1.W * Row2.X * Row3.Y - Row0.Z * Row1.W * Row2.Y * Row3.X + Row0.Z * Row1.X * Row2.Y * Row3.W - Row0.Z * Row1.X * Row2.W * Row3.Y
                  + Row0.Z * Row1.Y * Row2.W * Row3.X - Row0.Z * Row1.Y * Row2.X * Row3.W - Row0.W * Row1.X * Row2.Y * Row3.Z + Row0.W * Row1.X * Row2.Z * Row3.Y
                  - Row0.W * Row1.Y * Row2.Z * Row3.X + Row0.W * Row1.Y * Row2.X * Row3.Z - Row0.W * Row1.Z * Row2.X * Row3.Y + Row0.W * Row1.Z * Row2.Y * Row3.X;
            }
        }

        /// <summary>
        /// The first column of this matrix
        /// </summary>
        public Vector4d Column0
        {
            get { return new Vector4d(Row0.X, Row1.X, Row2.X, Row3.X); }
        }

        /// <summary>
        /// The second column of this matrix
        /// </summary>
        public Vector4d Column1
        {
            get { return new Vector4d(Row0.Y, Row1.Y, Row2.Y, Row3.Y); }
        }

        /// <summary>
        /// The third column of this matrix
        /// </summary>
        public Vector4d Column2
        {
            get { return new Vector4d(Row0.Z, Row1.Z, Row2.Z, Row3.Z); }
        }

        /// <summary>
        /// The fourth column of this matrix
        /// </summary>
        public Vector4d Column3
        {
            get { return new Vector4d(Row0.W, Row1.W, Row2.W, Row3.W); }
        }

        /// <summary>
        /// Gets or sets the value at row 1, column 1 of this instance.
        /// </summary>
        public double M11 { get { return Row0.X; } set { Row0.X = value; } }

        /// <summary>
        /// Gets or sets the value at row 1, column 2 of this instance.
        /// </summary>
        public double M12 { get { return Row0.Y; } set { Row0.Y = value; } }

        /// <summary>
        /// Gets or sets the value at row 1, column 3 of this instance.
        /// </summary>
        public double M13 { get { return Row0.Z; } set { Row0.Z = value; } }

        /// <summary>
        /// Gets or sets the value at row 1, column 4 of this instance.
        /// </summary>
        public double M14 { get { return Row0.W; } set { Row0.W = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 1 of this instance.
        /// </summary>
        public double M21 { get { return Row1.X; } set { Row1.X = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 2 of this instance.
        /// </summary>
        public double M22 { get { return Row1.Y; } set { Row1.Y = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 3 of this instance.
        /// </summary>
        public double M23 { get { return Row1.Z; } set { Row1.Z = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 4 of this instance.
        /// </summary>
        public double M24 { get { return Row1.W; } set { Row1.W = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 1 of this instance.
        /// </summary>
        public double M31 { get { return Row2.X; } set { Row2.X = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 2 of this instance.
        /// </summary>
        public double M32 { get { return Row2.Y; } set { Row2.Y = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 3 of this instance.
        /// </summary>
        public double M33 { get { return Row2.Z; } set { Row2.Z = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 4 of this instance.
        /// </summary>
        public double M34 { get { return Row2.W; } set { Row2.W = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 1 of this instance.
        /// </summary>
        public double M41 { get { return Row3.X; } set { Row3.X = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 2 of this instance.
        /// </summary>
        public double M42 { get { return Row3.Y; } set { Row3.Y = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 3 of this instance.
        /// </summary>
        public double M43 { get { return Row3.Z; } set { Row3.Z = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 4 of this instance.
        /// </summary>
        public double M44 { get { return Row3.W; } set { Row3.W = value; } }

        #endregion

        #region Instance

        #region public void Invert()

        /// <summary>
        /// Converts this instance into its inverse.
        /// </summary>
        public void Invert()
        {
            this = Matrix4d.Invert(this);
        }

        #endregion

        #region public void Transpose()

        /// <summary>
        /// Converts this instance into its transpose.
        /// </summary>
        public void Transpose()
        {
            this = Matrix4d.Transpose(this);
        }

        #endregion

        #endregion

        #region Static
		                #region CreateFromAxisAngle
        
        /// <summary>
        /// Build a rotation matrix from the specified axis/angle rotation.
        /// </summary>
        /// <param name="axis">The axis to rotate about.</param>
        /// <param name="angle">Angle in radians to rotate counter-clockwise (looking in the direction of the given axis).</param>
        /// <param name="result">A matrix instance.</param>
        public static void CreateFromAxisAngle(OpenTK.Vector3 axis, float angle, out Matrix4d result)
        {
            double cos =  System.Math.Cos(-angle);
            double sin =  System.Math.Sin(-angle);
            double t = 1 - cos;

            axis.Normalize();

            result = new Matrix4d((double)(t * axis.X * axis.X + cos), (double)(t * axis.X * axis.Y - sin * axis.Z), (double)(t * axis.X * axis.Z + sin * axis.Y), 0,
                                 (double)(t * axis.X * axis.Y + sin * axis.Z), (double)(t * axis.Y * axis.Y + cos), (double)(t * axis.Y * axis.Z - sin * axis.X), 0,
                                 (double)(t * axis.X * axis.Z - sin * axis.Y), (double)(t * axis.Y * axis.Z + sin * axis.X), (double)(t * axis.Z * axis.Z + cos), 0,
                                 0, 0, 0, 1);
        }
        
        /// <summary>
        /// Build a rotation matrix from the specified axis/angle rotation.
        /// </summary>
        /// <param name="axis">The axis to rotate about.</param>
        /// <param name="angle">Angle in radians to rotate counter-clockwise (looking in the direction of the given axis).</param>
        /// <returns>A matrix instance.</returns>
        public static Matrix4d CreateFromAxisAngle(OpenTK.Vector3 axis, float angle)
        {
            Matrix4d result;
            CreateFromAxisAngle(axis, angle, out result);
            return result;
        }
        
        #endregion

        #region CreateRotation[XYZ]

        /// <summary>
        /// Builds a rotation matrix for a rotation around the x-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <param name="result">The resulting Matrix4d instance.</param>
        public static void CreateRotationX(double angle, out Matrix4d result)
        {
            double cos =  System.Math.Cos(angle);
            double sin =  System.Math.Sin(angle);

            result.Row0 = Vector4d.UnitX;
            result.Row1 = new Vector4d(0, (double)(cos), (double)(sin), 0);
            result.Row2 = new Vector4d(0, (double)(-sin), (double)(cos), 0);
            result.Row3 = Vector4d.UnitW;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the x-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting Matrix4d instance.</returns>
        public static Matrix4d CreateRotationX(double angle)
        {
            Matrix4d result;
            CreateRotationX(angle, out result);
            return result;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y-axis.
        ///
		///   | cos  0.0 -sin 0.0 |
		///   | 0.0  1.0  1.0 0.0 |
		///   | sin  0.0  cos 0.0 |
		///   | 0.0  0.0  0.0 1.0 |
		///
		/// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <param name="result">The resulting Matrix4d instance.</param>
        public static void CreateRotationY(double angle, out Matrix4d result)
        {
            double cos = (double)System.Math.Cos(angle);
            double sin = (double)System.Math.Sin(angle);

            result.Row0 = new Vector4d((double)cos, 0, (double)-sin, 0);
            result.Row1 = Vector4d.UnitY;
            result.Row2 = new Vector4d((double)sin, 0, (double)cos, 0);
            result.Row3 = Vector4d.UnitW;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y-axis.
        ///
		///   | cos  0.0 -sin 0.0 |
		///   | 0.0  1.0  1.0 0.0 |
		///   | sin  0.0  cos 0.0 |
		///   | 0.0  0.0  0.0 1.0 |
		///
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting Matrix4d instance.</returns>
        public static Matrix4d CreateRotationY(double angle)
        {
            Matrix4d result;
            CreateRotationY(angle, out result);
            return result;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the z-axis.
		///
		///   | cos -sin 0.0 0.0 |
		///   | sin  cos 0.0 0.0 |
		///   | 0.0  0.0 1.0 0.0 |
		///   | 0.0  0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <param name="result">The resulting Matrix4d instance.</param>
        public static void CreateRotationZ(double angle, out Matrix4d result)
        {
            double cos =  System.Math.Cos(angle);
            double sin =  System.Math.Sin(angle);

            result.Row0 = new Vector4d((double)cos, -(double)sin, 0, 0);
            result.Row1 = new Vector4d((double)sin, (double)cos, 0, 0);
            result.Row2 = Vector4d.UnitZ;
            result.Row3 = Vector4d.UnitW;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the z-axis.
		///
		///   | cos -sin 0.0 0.0 |
		///   | sin  cos 0.0 0.0 |
		///   | 0.0  0.0 1.0 0.0 |
		///   | 0.0  0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting Matrix4d instance.</returns>
        public static Matrix4d CreateRotationZ(double angle)
        {
            Matrix4d result;
            CreateRotationZ(angle, out result);
            return result;
        }

		/// <summary>
        /// Creates a matrix from a given angle around a given axis
        /// </summary>
        /// <param name="rad">the angle to rotate the matrix by</param>
        /// <param name="x">the x component of the axis to rotate around</param>
        /// <param name="y">the y component of the axis to rotate around</param>
        /// <param name="z">the z component of the axis to rotate around</param>
        /// <returns></returns>
        public static Matrix4d CreateRotation(double rad, double x, double y, double z)
        {
            Matrix4d result = new Matrix4d();
            double len = System.Math.Sqrt(x * x + y * y + z * z);
            if (System.Math.Abs(len) < 0.000001)
                throw new ArgumentOutOfRangeException("Small length of vector.");

            len = 1 / len;
            x = (double)(x * len);
            y = (double)(y * len);
            z = (double)(z * len);

            double s = System.Math.Sin(rad);
            double c = System.Math.Cos(rad);
            double t = 1 - c;

            // Construct the elements of the rotation matrix
            result.M11 = (double)(x * x * t + c);
            result.M12 = (double)(y * x * t + z * s);
            result.M13 = (double)(z * x * t - y * s);
            result.M21 = (double)(x * y * t - z * s);
            result.M22 = (double)(y * y * t + c);
            result.M23 = (double)(z * y * t + x * s);
            result.M31 = (double)(x * z * t + y * s);
            result.M32 = (double)(y * z * t - x * s);
            result.M33 = (double)(z * z * t + c);
            result.M44 = 1;

            return result;
        }

        #endregion

        #region CreateTranslation
		
		/// <summary>
        /// Multiplay a matrix by a translation matrix.
		///
		///   | 1.0 0.0 0.0 tx  |
		///   | 0.0 1.0 0.0 ty  |
		///   | 0.0 0.0 1.0 tz  |
		///   | 0.0 0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="mat">The original and resulting Matrix4d instance.</param>
        /// <param name="x">X translation.</param>
        /// <param name="y">Y translation.</param>
        /// <param name="z">Z translation.</param>
        public static Matrix4d Translate(Matrix4d mat, double x, double y, double z)
        {
			mat.M14 = mat.M11 * x + mat.M12 * y + mat.M13 * z + mat.M14;
		    mat.M24 = mat.M21 * x + mat.M22 * y + mat.M23 * z + mat.M24;
            mat.M34 = mat.M31 * x + mat.M32 * y + mat.M33 * z + mat.M34;
            mat.M44 = mat.M41 * x + mat.M42 * y + mat.M43 * z + mat.M44;
			return mat;
        }


        /// <summary>
        /// Creates a translation matrix.
		///
		///   | 1.0 0.0 0.0 tx  |
		///   | 0.0 1.0 0.0 ty  |
		///   | 0.0 0.0 1.0 tz  |
		///   | 0.0 0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="x">X translation.</param>
        /// <param name="y">Y translation.</param>
        /// <param name="z">Z translation.</param>
        /// <param name="result">The resulting Matrix4d instance.</param>
        public static void CreateTranslation(double x, double y, double z, out Matrix4d result)
        {
            result = Identity;
            result.Row0.W = x;
            result.Row1.W = y;
            result.Row2.W = z;
        }

        /// <summary>
        /// Creates a translation matrix.
		///
		///   | 1.0 0.0 0.0 tx  |
		///   | 0.0 1.0 0.0 ty  |
		///   | 0.0 0.0 1.0 tz  |
		///   | 0.0 0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="vector">The translation vector.</param>
        /// <param name="result">The resulting Matrix4d instance.</param>
        public static void CreateTranslation(ref Vector3d vector, out Matrix4d result)
        {
            result = Identity;
			result.Row0.W = vector.X;
            result.Row1.W = vector.Y;
            result.Row2.W = vector.Z;
        }

        /// <summary>
        /// Creates a translation matrix.
		///
		///   | 1.0 0.0 0.0 tx  |
		///   | 0.0 1.0 0.0 ty  |
		///   | 0.0 0.0 1.0 tz  |
		///   | 0.0 0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="x">X translation.</param>
        /// <param name="y">Y translation.</param>
        /// <param name="z">Z translation.</param>
        /// <returns>The resulting Matrix4d instance.</returns>
        public static Matrix4d CreateTranslation(double x, double y, double z)
        {
            Matrix4d result;
            CreateTranslation(x, y, z, out result);
            return result;
        }

        /// <summary>
        /// Creates a translation matrix.
		///
		///   | 1.0 0.0 0.0 tx  |
		///   | 0.0 1.0 0.0 ty  |
		///   | 0.0 0.0 1.0 tz  |
		///   | 0.0 0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="vector">The translation vector.</param>
        /// <returns>The resulting Matrix4d instance.</returns>
        public static Matrix4d CreateTranslation(Vector3d vector)
        {
            Matrix4d result;
            CreateTranslation(vector.X, vector.Y, vector.Z, out result);
            return result;
        }

        #endregion

        #region CreateOrthographic

        /// <summary>
        /// Creates an orthographic projection matrix.
        /// </summary>
        /// <param name="width">The width of the projection volume.</param>
        /// <param name="height">The height of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <param name="result">The resulting Matrix4d instance.</param>
        public static void CreateOrthographic(double width, double height, double zNear, double zFar, out Matrix4d result)
        {
            CreateOrthographicOffCenter(-width / 2, width / 2, -height / 2, height / 2, zNear, zFar, out result);
        }

        /// <summary>
        /// Creates an orthographic projection matrix.
        /// </summary>
        /// <param name="width">The width of the projection volume.</param>
        /// <param name="height">The height of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <rereturns>The resulting Matrix4d instance.</rereturns>
        public static Matrix4d CreateOrthographic(double width, double height, double zNear, double zFar)
        {
            Matrix4d result;
            CreateOrthographicOffCenter(-width / 2, width / 2, -height / 2, height / 2, zNear, zFar, out result);
            return result;
        }

        #endregion

        #region CreateOrthographicOffCenter

        /// <summary>
        /// Creates an orthographic projection matrix.
        /// </summary>
        /// <param name="left">The left edge of the projection volume.</param>
        /// <param name="right">The right edge of the projection volume.</param>
        /// <param name="bottom">The bottom edge of the projection volume.</param>
        /// <param name="top">The top edge of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <param name="result">The resulting Matrix4d instance.</param>
        public static void CreateOrthographicOffCenter(double left, double right, double bottom, double top, double zNear, double zFar, out Matrix4d result)
        {
            result = new Matrix4d();

            double invLR = 1 / (left - right);
            double invBT = 1 / (bottom - top);
            double invNF = 1 / (zNear - zFar);

            result.M11 = -2 * invLR;
            result.M22 = -2 * invBT;
            result.M33 =  2 * invNF;

            result.M14 = (right + left) * invLR;
            result.M24 = (top + bottom) * invBT;
            result.M34 = (zFar + zNear) * invNF;
            result.M44 = 1;

		}

        /// <summary>
        /// Creates an orthographic projection matrix.
        /// </summary>
        /// <param name="left">The left edge of the projection volume.</param>
        /// <param name="right">The right edge of the projection volume.</param>
        /// <param name="bottom">The bottom edge of the projection volume.</param>
        /// <param name="top">The top edge of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <returns>The resulting Matrix4d instance.</returns>
        public static Matrix4d CreateOrthographicOffCenter(double left, double right, double bottom, double top, double zNear, double zFar)
        {
            Matrix4d result;
            CreateOrthographicOffCenter(left, right, bottom, top, zNear, zFar, out result);
            return result;
        }

        #endregion
        
        #region CreatePerspectiveFieldOfView
        
        /// <summary>
        /// Creates a perspective projection matrix.
        /// </summary>
        /// <param name="fovy">Angle of the field of view in the y direction (in radians)</param>
        /// <param name="aspect">Aspect ratio of the view (width / height)</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <param name="result">A projection matrix that transforms camera space to raster space</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown under the following conditions:
        /// <list type="bullet">
        /// <item>fovy is zero, less than zero or larger than Math.PI</item>
        /// <item>aspect is negative or zero</item>
        /// <item>zNear is negative or zero</item>
        /// <item>zFar is negative or zero</item>
        /// <item>zNear is larger than zFar</item>
        /// </list>
        /// </exception>
        public static void CreatePerspectiveFieldOfView(double fovy, double aspect, double zNear, double zFar, out Matrix4d result)
        {
            if (fovy <= 0 || fovy > System.Math.PI)
                throw new ArgumentOutOfRangeException("fovy");
            if (aspect <= 0)
                throw new ArgumentOutOfRangeException("aspect");
            if (zNear <= 0)
                throw new ArgumentOutOfRangeException("zNear");
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException("zFar");
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException("zNear");
            
            double yMax = zNear * (double)System.Math.Tan(0.5f * fovy);
            double yMin = -yMax;
            double xMin = yMin * aspect;
            double xMax = yMax * aspect;

            CreatePerspectiveOffCenter(xMin, xMax, yMin, yMax, zNear, zFar, out result);
        }
        
        /// <summary>
        /// Creates a perspective projection matrix.
        /// </summary>
        /// <param name="fovy">Angle of the field of view in the y direction (in radians)</param>
        /// <param name="aspect">Aspect ratio of the view (width / height)</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <returns>A projection matrix that transforms camera space to raster space</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown under the following conditions:
        /// <list type="bullet">
        /// <item>fovy is zero, less than zero or larger than Math.PI</item>
        /// <item>aspect is negative or zero</item>
        /// <item>zNear is negative or zero</item>
        /// <item>zFar is negative or zero</item>
        /// <item>zNear is larger than zFar</item>
        /// </list>
        /// </exception>
        public static Matrix4d CreatePerspectiveFieldOfView(double fovy, double aspect, double zNear, double zFar)
        {
            Matrix4d result;
            CreatePerspectiveFieldOfView(fovy, aspect, zNear, zFar, out result);
            return result;
        }
        
        #endregion
        
        #region CreatePerspectiveOffCenter
        
        /// <summary>
        /// Creates an perspective projection matrix.
        /// </summary>
        /// <param name="left">Left edge of the view frustum</param>
        /// <param name="right">Right edge of the view frustum</param>
        /// <param name="bottom">Bottom edge of the view frustum</param>
        /// <param name="top">Top edge of the view frustum</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <param name="result">A projection matrix that transforms camera space to raster space</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown under the following conditions:
        /// <list type="bullet">
        /// <item>zNear is negative or zero</item>
        /// <item>zFar is negative or zero</item>
        /// <item>zNear is larger than zFar</item>
        /// </list>
        /// </exception>
        public static void CreatePerspectiveOffCenter(double left, double right, double bottom, double top, double zNear, double zFar, out Matrix4d result)
        {
            if (zNear <= 0)
                throw new ArgumentOutOfRangeException("zNear");
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException("zFar");
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException("zNear");
            
            double x = (double)((2  * zNear) / (right - left));
            double y = (double)((2  * zNear) / (top - bottom));
            double a = (double)((right + left) / (right - left));
            double b = (double)((top + bottom) / (top - bottom));
            double c = (double)((zFar + zNear) / (zNear - zFar ));
            double d = (double)((2  * zFar * zNear) / (zNear - zFar));
            
            result = new Matrix4d(x, 0,  a, 0,
								  0, y,  b, 0,
								  0, b,  c, d,
							 	  0, 0, -1, 0);
        }
        
        /// <summary>
        /// Creates an perspective projection matrix.
        /// </summary>
        /// <param name="left">Left edge of the view frustum</param>
        /// <param name="right">Right edge of the view frustum</param>
        /// <param name="bottom">Bottom edge of the view frustum</param>
        /// <param name="top">Top edge of the view frustum</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <returns>A projection matrix that transforms camera space to raster space</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown under the following conditions:
        /// <list type="bullet">
        /// <item>zNear is negative or zero</item>
        /// <item>zFar is negative or zero</item>
        /// <item>zNear is larger than zFar</item>
        /// </list>
        /// </exception>
        public static Matrix4d CreatePerspectiveOffCenter(double left, double right, double bottom, double top, double zNear, double zFar)
        {
            Matrix4d result;
            CreatePerspectiveOffCenter(left, right, bottom, top, zNear, zFar, out result);
            return result;
        }
        
        #endregion

        #region Scale Functions

        /// <summary>
        /// Build a scaling matrix
		///
		///   | sx  0.0 0.0 0.0 |
		///   | 0.0 sy  0.0 0.0 |
		///   | 0.0 0.0 sz  0.0 |
		///   | 0.0 0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="scale">Single scale factor for x,y and z axes</param>
        /// <returns>A scaling matrix</returns>
        public static Matrix4d Scale(double scale)
        {
            return Scale(scale, scale, scale);
        }

        /// <summary>
        /// Build a scaling matrix
		///
		///   | sx  0.0 0.0 0.0 |
		///   | 0.0 sy  0.0 0.0 |
		///   | 0.0 0.0 sz  0.0 |
		///   | 0.0 0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="scale">Scale factors for x,y and z axes</param>
        /// <returns>A scaling matrix</returns>
        public static Matrix4d Scale(Vector3d scale)
        {
            return Scale(scale.X, scale.Y, scale.Z);
        }

        /// <summary>
        /// Build a scaling matrix
		///
		///   | sx  0.0 0.0 0.0 |
		///   | 0.0 sy  0.0 0.0 |
		///   | 0.0 0.0 sz  0.0 |
		///   | 0.0 0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="x">Scale factor for x-axis</param>
        /// <param name="y">Scale factor for y-axis</param>
        /// <param name="z">Scale factor for z-axis</param>
        /// <returns>A scaling matrix</returns>
        public static Matrix4d Scale(double x, double y, double z)
        {
            Matrix4d result;
            result.Row0 = Vector4d.UnitX * x;
            result.Row1 = Vector4d.UnitY * y;
            result.Row2 = Vector4d.UnitZ * z;
            result.Row3 = Vector4d.UnitW;
            return result;
        }
        

		/// <summary>
        /// Build a scaling matrix
		///
		///   | sx  0.0 0.0 0.0 |
		///   | 0.0 sy  0.0 0.0 |
		///   | 0.0 0.0 sz  0.0 |
		///   | 0.0 0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="s">Scale factor for all axies</param>
        /// <returns>A scaling matrix</returns>
        public static Matrix4d Scale(Matrix4d matrix, double s)
        {
			return Scale(matrix, s, s, s);
        }

		/// <summary>
        /// Build a scaling matrix
		///
		///   | sx  0.0 0.0 0.0 |
		///   | 0.0 sy  0.0 0.0 |
		///   | 0.0 0.0 sz  0.0 |
		///   | 0.0 0.0 0.0 1.0 |
		///
        /// </summary>
        /// <param name="x">Scale factor for x-axis</param>
        /// <param name="y">Scale factor for y-axis</param>
        /// <param name="z">Scale factor for z-axis</param>
        /// <returns>A scaling matrix</returns>
        public static Matrix4d Scale(Matrix4d matrix, double x, double y, double z)
        {
			matrix.M11 = matrix.M11 * x;
			matrix.M21 = matrix.M21 * x;
			matrix.M31 = matrix.M31 * x;
			matrix.M41 = matrix.M41 * x;
			matrix.M12 = matrix.M12 * y;
			matrix.M22 = matrix.M22 * y;
			matrix.M32 = matrix.M32 * y;
			matrix.M42 = matrix.M42 * y;
			matrix.M13 = matrix.M13 * z;
			matrix.M23 = matrix.M23 * z;
			matrix.M33 = matrix.M33 * z;
			matrix.M43 = matrix.M43 * z;
			matrix.M14 = matrix.M14;
			matrix.M24 = matrix.M24;
			matrix.M34 = matrix.M34;
			matrix.M44 = matrix.M44;
			return matrix;
        }
        
		#endregion

        #region Rotation Function

        /// <summary>
        /// Build a rotation matrix from a quaternion
        /// </summary>
        /// <param name="q">the quaternion</param>
        /// <returns>A rotation matrix</returns>
        public static Matrix4d Rotate(Quaternion q)
        {
            OpenTK.Vector3 axis;
            float angle;
            q.ToAxisAngle(out axis, out angle);
            return CreateFromAxisAngle(axis, angle);
        }

        #endregion

        #region Camera Helper Functions

        /// <summary>
        /// Build a world space to camera space matrix
        /// </summary>
        /// <param name="eye">Eye (camera) position in world space</param>
        /// <param name="target">Target position in world space</param>
        /// <param name="up">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
        /// <returns>A Matrix4d that transforms world space to camera space</returns>
        public static Matrix4d LookAt(Vector3d eye, Vector3d target, Vector3d up)
        {
            Vector3d z = Vector3d.Normalize(eye - target);
            Vector3d x = Vector3d.Normalize(Vector3d.Cross(up, z));
            Vector3d y = Vector3d.Normalize(Vector3d.Cross(z, x));

            Matrix4d rot = new Matrix4d(new Vector4d(x.X, y.X, z.X, 0),
                                        new Vector4d(x.Y, y.Y, z.Y, 0),
                                        new Vector4d(x.Z, y.Z, z.Z, 0),
                                        Vector4d.UnitW);

            Matrix4d trans = Matrix4d.CreateTranslation(-eye);

            return trans * rot;
        }

        /// <summary>
        /// Build a world space to camera space matrix
        /// </summary>
        /// <param name="eyeX">Eye (camera) position in world space</param>
        /// <param name="eyeY">Eye (camera) position in world space</param>
        /// <param name="eyeZ">Eye (camera) position in world space</param>
        /// <param name="targetX">Target position in world space</param>
        /// <param name="targetY">Target position in world space</param>
        /// <param name="targetZ">Target position in world space</param>
        /// <param name="upX">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
        /// <param name="upY">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
        /// <param name="upZ">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
        /// <returns>A Matrix4d that transforms world space to camera space</returns>
        public static Matrix4d LookAt(double eyeX, double eyeY, double eyeZ, double targetX, double targetY, double targetZ, double upX, double upY, double upZ)
        {
            return LookAt(new Vector3d(eyeX, eyeY, eyeZ), new Vector3d(targetX, targetY, targetZ), new Vector3d(upX, upY, upZ));
        }

		
        #endregion
        #region Multiply Functions

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <returns>A new instance that is the result of the multiplication</returns>
        public static Matrix4d Mult(Matrix4d left, Matrix4d right)
        {
            Matrix4d result;
            Mult(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <param name="result">A new instance that is the result of the multiplication</param>
        public static void Mult(ref Matrix4d left, ref Matrix4d right, out Matrix4d result)
        {
            result = new Matrix4d(
                right.M11 * left.M11 + right.M12 * left.M21 + right.M13 * left.M31 + right.M14 * left.M41,
                right.M11 * left.M12 + right.M12 * left.M22 + right.M13 * left.M32 + right.M14 * left.M42,
                right.M11 * left.M13 + right.M12 * left.M23 + right.M13 * left.M33 + right.M14 * left.M43,
                right.M11 * left.M14 + right.M12 * left.M24 + right.M13 * left.M34 + right.M14 * left.M44,
                right.M21 * left.M11 + right.M22 * left.M21 + right.M23 * left.M31 + right.M24 * left.M41,
                right.M21 * left.M12 + right.M22 * left.M22 + right.M23 * left.M32 + right.M24 * left.M42,
                right.M21 * left.M13 + right.M22 * left.M23 + right.M23 * left.M33 + right.M24 * left.M43,
                right.M21 * left.M14 + right.M22 * left.M24 + right.M23 * left.M34 + right.M24 * left.M44,
                right.M31 * left.M11 + right.M32 * left.M21 + right.M33 * left.M31 + right.M34 * left.M41,
                right.M31 * left.M12 + right.M32 * left.M22 + right.M33 * left.M32 + right.M34 * left.M42,
                right.M31 * left.M13 + right.M32 * left.M23 + right.M33 * left.M33 + right.M34 * left.M43,
                right.M31 * left.M14 + right.M32 * left.M24 + right.M33 * left.M34 + right.M34 * left.M44,
                right.M41 * left.M11 + right.M42 * left.M21 + right.M43 * left.M31 + right.M44 * left.M41,
                right.M41 * left.M12 + right.M42 * left.M22 + right.M43 * left.M32 + right.M44 * left.M42,
                right.M41 * left.M13 + right.M42 * left.M23 + right.M43 * left.M33 + right.M44 * left.M43,
                right.M41 * left.M14 + right.M42 * left.M24 + right.M43 * left.M34 + right.M44 * left.M44);
		}

        #endregion

        #region Invert Functions

        /// <summary>
        /// Calculate the inverse of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to invert</param>
        /// <returns>The inverse of the given matrix if it has one, or the input if it is singular</returns>
        /// <exception cref="InvalidOperationException">Thrown if the Matrix4d is singular.</exception>
        public static Matrix4d Invert(Matrix4d mat)
        {
            int[] colIdx = { 0, 0, 0, 0 };
            int[] rowIdx = { 0, 0, 0, 0 };
            int[] pivotIdx = { -1, -1, -1, -1 };

            // convert the matrix to an array for easy looping
            float[,] inverse = {{(float)mat.Row0.X, (float)mat.Row0.Y, (float)mat.Row0.Z, (float)mat.Row0.W}, 
                                {(float)mat.Row1.X, (float)mat.Row1.Y, (float)mat.Row1.Z, (float)mat.Row1.W}, 
                                {(float)mat.Row2.X, (float)mat.Row2.Y, (float)mat.Row2.Z, (float)mat.Row2.W}, 
                                {(float)mat.Row3.X, (float)mat.Row3.Y, (float)mat.Row3.Z, (float)mat.Row3.W} };
            int icol = 0;
            int irow = 0;
            for (int i = 0; i < 4; i++)
            {
                // Find the largest pivot value
                float maxPivot = 0;
                for (int j = 0; j < 4; j++)
                {
                    if (pivotIdx[j] != 0)
                    {
                        for (int k = 0; k < 4; ++k)
                        {
                            if (pivotIdx[k] == -1)
                            {
                               float absVal = System.Math.Abs(inverse[j, k]);
                                if (absVal > maxPivot)
                                {
                                    maxPivot = absVal;
                                    irow = j;
                                    icol = k;
                                }
                            }
                            else if (pivotIdx[k] > 0)
                            {
                                return mat;
                            }
                        }
                    }
                }

                ++(pivotIdx[icol]);

                // Swap rows over so pivot is on diagonal
                if (irow != icol)
                {
                    for (int k = 0; k < 4; ++k)
                    {
                        float f = inverse[irow, k];
                        inverse[irow, k] = inverse[icol, k];
                        inverse[icol, k] = f;
                    }
                }

                rowIdx[i] = irow;
                colIdx[i] = icol;

                float pivot = inverse[icol, icol];
                // check for singular matrix
                if (pivot == 0)
                {
                    throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
                    //return mat;
                }

                // Scale row so it has a unit diagonal
                float oneOverPivot = 1.0f / pivot;
                inverse[icol, icol] = 1.0f;
                for (int k = 0; k < 4; ++k)
                    inverse[icol, k] *= oneOverPivot;

                // Do elimination of non-diagonal elements
                for (int j = 0; j < 4; ++j)
                {
                    // check this isn't on the diagonal
                    if (icol != j)
                    {
                        float f = inverse[j, icol];
                        inverse[j, icol] = 0;
                        for (int k = 0; k < 4; ++k)
                            inverse[j, k] -= inverse[icol, k] * f;
                    }
                }
            }

            for (int j = 3; j >= 0; --j)
            {
                int ir = rowIdx[j];
                int ic = colIdx[j];
                for (int k = 0; k < 4; ++k)
                {
                    float f = inverse[k, ir];
                    inverse[k, ir] = inverse[k, ic];
                    inverse[k, ic] = f;
                }
            }

            mat.Row0 = new Vector4d((double)inverse[0, 0], (double)inverse[0, 1], (double)inverse[0, 2], (double)inverse[0, 3]);
            mat.Row1 = new Vector4d((double)inverse[1, 0], (double)inverse[1, 1], (double)inverse[1, 2], (double)inverse[1, 3]);
            mat.Row2 = new Vector4d((double)inverse[2, 0], (double)inverse[2, 1], (double)inverse[2, 2], (double)inverse[2, 3]);
            mat.Row3 = new Vector4d((double)inverse[3, 0], (double)inverse[3, 1], (double)inverse[3, 2], (double)inverse[3, 3]);
            return mat;
        }

        #endregion

        #region Transpose

        /// <summary>
        /// Calculate the transpose of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to transpose</param>
        /// <returns>The transpose of the given matrix</returns>
        public static Matrix4d Transpose(Matrix4d mat)
        {
            return new Matrix4d(mat.Column0, mat.Column1, mat.Column2, mat.Column3);
        }


        /// <summary>
        /// Calculate the transpose of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to transpose</param>
        /// <param name="result">The result of the calculation</param>
        public static void Transpose(ref Matrix4d mat, out Matrix4d result)
        {
            result.Row0 = mat.Column0;
            result.Row1 = mat.Column1;
            result.Row2 = mat.Column2;
            result.Row3 = mat.Column3;
        }

        #endregion

        #endregion

        #region Operators

        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new Matrix4d which holds the result of the multiplication</returns>
        public static Matrix4d operator *(Matrix4d left, Matrix4d right)
        {
            return Matrix4d.Mult(left, right);
        }

		/// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new Vector3d which holds the result of the multiplication</returns>
        public static Vector3d operator *(Matrix4d left, Vector3d right)
		{
			Vector3d r;

			double fInvW = 1.0 / (left.Row3.X * right.X + left.Row3.Y * right.Y + left.Row3.Z * right.Z + left.Row3.W);

			r.X =  (double)((left.Row0.X * right.X + left.Row0.Y * right.Y + left.Row0.Z * right.Z + left.Row0.W) * fInvW);
			r.Y =  (double)((left.Row1.X * right.X + left.Row1.Y * right.Y + left.Row1.Z * right.Z + left.Row1.W) * fInvW);
			r.Z =  (double)((left.Row2.X * right.X + left.Row2.Y * right.Y + left.Row2.Z * right.Z + left.Row2.W) * fInvW);

			return r;
		}
		
		/// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new Vector3d which holds the result of the multiplication</returns>
        public static Vector4d operator *(Matrix4d left, Vector4d right)
		{
			Vector4d r;

			double fInvW = 1.0 / (left.Row3.X * right.X + left.Row3.Y * right.Y + left.Row3.Z * right.Z + left.Row3.W);

			r.X =  (double)((left.Row0.X * right.X + left.Row0.Y * right.Y + left.Row0.Z * right.Z + left.Row0.W * right.W) * fInvW);
			r.Y =  (double)((left.Row1.X * right.X + left.Row1.Y * right.Y + left.Row1.Z * right.Z + left.Row1.W * right.W) * fInvW);
			r.Z =  (double)((left.Row2.X * right.X + left.Row2.Y * right.Y + left.Row2.Z * right.Z + left.Row2.W * right.W) * fInvW);
			r.W =  (double)((left.Row3.X * right.X + left.Row3.Y * right.Y + left.Row3.Z * right.Z + left.Row3.W * right.W) * fInvW);
			return r;
		}
		/// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new Box3d which holds the result of the multiplication</returns>
        public static Box3d operator *(Matrix4d left, Box3d right)
		{
				Box3d b = new Box3d();
				b = b.enlarge(left*(new Vector3d(right.xmin, right.ymin, right.zmin)));
				b = b.enlarge(left*(new Vector3d(right.xmax, right.ymin, right.zmin)));
				b = b.enlarge(left*(new Vector3d(right.xmin, right.ymax, right.zmin)));
				b = b.enlarge(left*(new Vector3d(right.xmax, right.ymax, right.zmin)));
				b = b.enlarge(left*(new Vector3d(right.xmin, right.ymin, right.zmax)));
				b = b.enlarge(left*(new Vector3d(right.xmax, right.ymin, right.zmax)));
				b = b.enlarge(left*(new Vector3d(right.xmin, right.ymax, right.zmax)));
				b = b.enlarge(left*(new Vector3d(right.xmax, right.ymax, right.zmax)));
				return b;
		}

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(Matrix4d left, Matrix4d right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(Matrix4d left, Matrix4d right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current Matrix4d4.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}\n{1}\n{2}\n{3}", Row0, Row1, Row2, Row3);
        }

        #endregion

        #region public override int GetHashCode()

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            return Row0.GetHashCode() ^ Row1.GetHashCode() ^ Row2.GetHashCode() ^ Row3.GetHashCode();
        }

        #endregion

        #region public override bool Equals(object obj)

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare tresult.</param>
        /// <returns>True if the instances are equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Matrix4d))
                return false;

            return this.Equals((Matrix4d)obj);
        }

        #endregion

        #endregion

        #endregion

        #region IEquatable<Matrix4d> Members

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="other">An matrix to compare with this matrix.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public bool Equals(Matrix4d other)
        {
            return
                Row0 == other.Row0 &&
                Row1 == other.Row1 &&
                Row2 == other.Row2 &&
                Row3 == other.Row3;
        }

        #endregion
    }
}
