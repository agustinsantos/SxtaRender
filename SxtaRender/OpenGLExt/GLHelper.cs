#region --- Using Directives ---

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;
using System.Reflection.Emit;
using Sxta.Math;
using OpenTK.Graphics.OpenGL;


#endregion

namespace Sxta.OpenGL
{
    public sealed class GL
    {
        #region Normal|RasterPos|Vertex|TexCoord|Rotate|Scale|Translate|*Matrix

        // Matrix 2
        public static void UniformMatrix2(int location, bool transpose, ref Matrix2f matrix)
        {
            unsafe
            {
                fixed (float* matrix_ptr = &matrix.R0C0)
                {
                    OpenTK.Graphics.OpenGL.GL.UniformMatrix2(location, 1, transpose, matrix_ptr);
                }
            }
        }

        public static void UniformMatrix2(int location, int cnt, bool transpose, ref Matrix2f matrix)
        {
            unsafe
            {
                fixed (float* matrix_ptr = &matrix.R0C0)
                {
                    OpenTK.Graphics.OpenGL.GL.UniformMatrix2(location, cnt, transpose, matrix_ptr);
                }
            }
        }

        public static void UniformMatrix2(int location, bool transpose, ref Matrix2d matrix)
        {
            unsafe
            {
                fixed (double* matrix_ptr = &matrix.R0C0)
                {
                    OpenTK.Graphics.OpenGL.GL.UniformMatrix2(location, 1, transpose, matrix_ptr);
                }
            }
        }
        public static void UniformMatrix2(int location, int cnt, bool transpose, ref Matrix2d matrix)
        {
            unsafe
            {
                fixed (double* matrix_ptr = &matrix.R0C0)
                {
                    OpenTK.Graphics.OpenGL.GL.UniformMatrix2(location, cnt, transpose, matrix_ptr);
                }
            }
        }

        // Matrix 3
        public static void UniformMatrix3(int location, bool transpose, ref Matrix3f matrix)
        {
            unsafe
            {
                fixed (float* matrix_ptr = &matrix.R0C0)
                {
                    OpenTK.Graphics.OpenGL.GL.UniformMatrix3(location, 1, transpose, matrix_ptr);
                }
            }
        }

        public static void UniformMatrix3(int location, int cnt, bool transpose, ref Matrix3f matrix)
        {
            unsafe
            {
                fixed (float* matrix_ptr = &matrix.R0C0)
                {
                    OpenTK.Graphics.OpenGL.GL.UniformMatrix3(location, cnt, transpose, matrix_ptr);
                }
            }
        }

        public static void UniformMatrix3(int location, bool transpose, ref Matrix3d matrix)
        {
            unsafe
            {
                fixed (double* matrix_ptr = &matrix.R0C0)
                {
                    OpenTK.Graphics.OpenGL.GL.UniformMatrix3(location, 1, transpose, matrix_ptr);
                }
            }
        }
        public static void UniformMatrix3(int location, int cnt, bool transpose, ref Matrix3d matrix)
        {
            unsafe
            {
                fixed (double* matrix_ptr = &matrix.R0C0)
                {
                    OpenTK.Graphics.OpenGL.GL.UniformMatrix3(location, cnt, transpose, matrix_ptr);
                }
            }
        }

        // Matrix 4
        public static void UniformMatrix4(int location, bool transpose, ref Matrix4f matrix)
        {
            unsafe
            {
                fixed (float* matrix_ptr = &matrix.Row0.X)
                {
                    OpenTK.Graphics.OpenGL.GL.UniformMatrix4(location, 1, transpose, matrix_ptr);
                }
            }
        }

        public static void UniformMatrix4(int location, int cnt, bool transpose, ref Matrix4f matrix)
        {
            unsafe
            {
                fixed (float* matrix_ptr = &matrix.Row0.X)
                {
                    OpenTK.Graphics.OpenGL.GL.UniformMatrix4(location, cnt, transpose, matrix_ptr);
                }
            }
        }

        public static void UniformMatrix4(int location, bool transpose, ref Matrix4d matrix)
        {
            unsafe
            {
                fixed (double* matrix_ptr = &matrix.Row0.X)
                {
                    OpenTK.Graphics.OpenGL.GL.UniformMatrix4(location, 1, transpose, matrix_ptr);
                }
            }
        }
        public static void UniformMatrix4(int location, int cnt, bool transpose, ref Matrix4d matrix)
        {
            unsafe
            {
                fixed (double* matrix_ptr = &matrix.Row0.X)
                {
                    OpenTK.Graphics.OpenGL.GL.UniformMatrix4(location, cnt, transpose, matrix_ptr);
                }
            }
        }
        #endregion
    }
}
