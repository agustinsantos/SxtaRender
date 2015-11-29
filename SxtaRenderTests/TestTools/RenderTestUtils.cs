using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Sxta.Math;
using Sxta.Render;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SxtaRenderTests.TestTools
{
    public class FrameBufferForTest : IDisposable
    {
        public FrameBuffer FrameBuffer { get; private set; }
        private RenderBuffer rb = null;
        private RenderBuffer stencil = null;

        public static FrameBufferForTest NewFrameBufferForTest(int Width, int Height, RenderBuffer.RenderBufferFormat colorFormat = RenderBuffer.RenderBufferFormat.RGB8, RenderBuffer.RenderBufferFormat stencilFormat = RenderBuffer.RenderBufferFormat.DEPTH24_STENCIL8)
        {
            FrameBufferForTest fb = new FrameBufferForTest();
            fb.FrameBuffer = new FrameBuffer();
            fb.rb = new RenderBuffer(colorFormat, Width, Height);
            fb.stencil = new RenderBuffer(stencilFormat, Width, Height);
            fb.FrameBuffer.setRenderBuffer(BufferId.COLOR0, fb.rb);
            fb.FrameBuffer.setRenderBuffer(BufferId.STENCIL, fb.stencil);
            fb.FrameBuffer.setViewport(new Vector4i(0, 0, Width, Height));
            fb.FrameBuffer.clear(true, true, true);

            return fb;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (FrameBuffer != null)
                        FrameBuffer.Dispose();
                    if (rb != null)
                        rb.Dispose();
                    if (stencil != null)
                        stencil.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FrameBufferForTest() {
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
    public class RenderTestUtils
    {
        private static readonly string SAVE_RESULTS_DIR = "Results";

        public static IGraphicsContext PrepareContext()
        {
            INativeWindow window = new NativeWindow();
            IGraphicsContext context = new GraphicsContext(GraphicsMode.Default, window.WindowInfo);
            context.MakeCurrent(window.WindowInfo);

            return context;
        }

        public static Bitmap GetScreenshot(int Width, int Height, OpenTK.Graphics.OpenGL.PixelFormat pf = OpenTK.Graphics.OpenGL.PixelFormat.Bgr)
        {
            Bitmap bmp = new Bitmap(Width, Height);
            System.Drawing.Imaging.BitmapData data =
                bmp.LockBits(new Rectangle(0, 0, Width, Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, Width, Height, pf, OpenTK.Graphics.OpenGL.PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            return bmp;
        }

        public static string SaveTestResult(string TestClass, string TestName, Image img)
        {
            string path = Path.Combine(SAVE_RESULTS_DIR, TestClass);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string filename = Path.Combine(path, TestName + ".bmp");
            img.Save(filename, ImageFormat.Bmp);
            return filename;
        }

        public static void DeleteResultDir(string TestClass)
        {
            string path = Path.Combine(SAVE_RESULTS_DIR, TestClass);
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
    }
}
