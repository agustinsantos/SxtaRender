using System;
using System.Runtime.InteropServices;

namespace Sxta.Math
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3x4f
    {
        #region Fields & Access

        /// <summary>Row 0, Column 0</summary>
        public float R0C0;

        /// <summary>Row 0, Column 1</summary>
        public float R0C1;

        /// <summary>Row 0, Column 2</summary>
        public float R0C2;

        /// <summary>Row 0, Column 3</summary>
        public float R0C3;

        /// <summary>Row 1, Column 0</summary>
        public float R1C0;

        /// <summary>Row 1, Column 1</summary>
        public float R1C1;

        /// <summary>Row 1, Column 2</summary>
        public float R1C2;

        /// <summary>Row 1, Column 3</summary>
        public float R1C3;

        /// <summary>Row 2, Column 0</summary>
        public float R2C0;

        /// <summary>Row 2, Column 1</summary>
        public float R2C1;

        /// <summary>Row 2, Column 2</summary>
        public float R2C2;

        /// <summary>Row 2, Column 3</summary>
        public float R2C3;
        #endregion

        /// <summary>
        /// Defines the size of the Matrix3f struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(new Matrix3x4f());

        /// <summary>The identity matrix.</summary>
        public static readonly Matrix3x4f Identity = new Matrix3x4f
        (
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0
        );

        /// <summary>A matrix of all zeros.</summary>
        public static readonly Matrix3x4f Zero = new Matrix3x4f
        (
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0
        );

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
        public Matrix3x4f
        (
            float r0c0,
            float r0c1,
            float r0c2,
            float r0c3,
            float r1c0,
            float r1c1,
            float r1c2,
            float r1c3, 
            float r2c0,
            float r2c1,
            float r2c2,
            float r2c3
        )
        {
            this.R0C0 = r0c0;
            this.R0C1 = r0c1;
            this.R0C2 = r0c2;
            this.R0C3 = r0c3;
            this.R1C0 = r1c0;
            this.R1C1 = r1c1;
            this.R1C2 = r1c2;
            this.R1C3 = r1c3;
            this.R2C0 = r2c0;
            this.R2C1 = r2c1;
            this.R2C2 = r2c2;
            this.R2C3 = r2c3;
        }

        /// <summary>Constructs left matrix from the given array of float-precision floating-point numbers.</summary>
        /// <param name="floatArray">The array of floats for the components of the matrix in Column-major order.</param>
        public Matrix3x4f(float[] floatArray)
        {
            if (floatArray == null || floatArray.GetLength(0) < 9) throw new MissingFieldException();

            this.R0C0 = floatArray[0];
            this.R0C1 = floatArray[1];
            this.R0C2 = floatArray[2];
            this.R0C3 = floatArray[3];
            this.R1C0 = floatArray[4];
            this.R1C1 = floatArray[5];
            this.R1C2 = floatArray[6];
            this.R1C3 = floatArray[7];
            this.R2C0 = floatArray[8];
            this.R2C1 = floatArray[9];
            this.R2C2 = floatArray[10];
            this.R2C3 = floatArray[11];
        }
        /// <summary>Gets the component at the given row and column in the matrix.</summary>
        /// <param name="row">The row of the matrix.</param>
        /// <param name="column">The column of the matrix.</param>
        /// <returns>The component at the given row and column in the matrix.</returns>
        public float this[int row, int column]
        {
            get
            {
                switch (row)
                {
                    case 0:
                        switch (column)
                        {
                            case 0: return R0C0;
                            case 1: return R0C1;
                            case 2: return R0C2;
                            case 3: return R0C3;
                        }
                        break;

                    case 1:
                        switch (column)
                        {
                            case 0: return R1C0;
                            case 1: return R1C1;
                            case 2: return R1C2;
                            case 3: return R1C3;
                        }
                        break;

                    case 2:
                        switch (column)
                        {
                            case 0: return R2C0;
                            case 1: return R2C1;
                            case 2: return R2C2;
                            case 3: return R2C3;
                        }
                        break;
                }

                throw new IndexOutOfRangeException();
            }
            set
            {
                switch (row)
                {
                    case 0:
                        switch (column)
                        {
                            case 0: R0C0 = value; return;
                            case 1: R0C1 = value; return;
                            case 2: R0C2 = value; return;
                            case 3: R0C3 = value; return;
                        }
                        break;

                    case 1:
                        switch (column)
                        {
                            case 0: R1C0 = value; return;
                            case 1: R1C1 = value; return;
                            case 2: R1C2 = value; return;
                            case 3: R1C3 = value; return;
                        }
                        break;

                    case 2:
                        switch (column)
                        {
                            case 0: R2C0 = value; return;
                            case 1: R2C1 = value; return;
                            case 2: R2C2 = value; return;
                            case 3: R2C3 = value; return;
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
                    case 3: return R0C3;
                    case 4: return R1C0;
                    case 5: return R1C1;
                    case 6: return R1C2;
                    case 7: return R1C3;
                    case 8: return R2C0;
                    case 9: return R2C1;
                    case 10: return R2C2;
                    case 11: return R2C3;
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
                    case 3: R0C3 = value; return;
                    case 4: R1C0 = value; return;
                    case 5: R1C1 = value; return;
                    case 6: R1C2 = value; return;
                    case 7: R1C3 = value; return;
                    case 8: R2C0 = value; return;
                    case 9: R2C1 = value; return;
                    case 10: R2C2 = value; return;
                    case 11: R2C3 = value; return;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

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
}