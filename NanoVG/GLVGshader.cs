using Sxta.Math;
using Sxta.Render;
using System;
using System.Runtime.InteropServices;
using NVGcolor = Sxta.Math.Vector4f;

namespace NanoVG
{
    [StructLayout(LayoutKind.Explicit)]
    public struct GLNVGfragUniforms
    {
        [FieldOffset(0)]
        public Matrix3x4f scissorMat;
        [FieldOffset(48)]
        public Matrix3x4f paintMat;
        [FieldOffset(96)]
        public NVGcolor innerCol;
        [FieldOffset(112)]
        public NVGcolor outerCol;
        [FieldOffset(128)]
        public Vector2f scissorExt;
        [FieldOffset(136)]
        public Vector2f scissorScale;
        [FieldOffset(144)]
        public Vector2f extent;
        [FieldOffset(152)]
        public float radius;
        [FieldOffset(156)]
        public float feather;
        [FieldOffset(160)]
        public float strokeMult;
        [FieldOffset(164)]
        public float strokeThr;
        [FieldOffset(168)]
        public int texType;
        [FieldOffset(172)]
        public GLNVGshaderType type;
    }


    internal class GLVGshader : IDisposable
    {
        public Program Prog;
        public Uniform2f ViewSizeUniform;
        public UniformSampler TexUniform;
        public UniformBlock FragUniform;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Prog != null)
                        Prog.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~GLNVGshader() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
