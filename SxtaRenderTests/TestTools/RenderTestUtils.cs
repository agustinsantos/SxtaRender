using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SxtaRenderTests.TestTools
{
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

        public static Bitmap GetScreenshot(int Width, int Height)
        {
            Bitmap bmp = new Bitmap(Width, Height);
            System.Drawing.Imaging.BitmapData data =
                bmp.LockBits(new Rectangle(0, 0, Width, Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, Width, Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, OpenTK.Graphics.OpenGL.PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            return bmp;
        }

        public static void SaveTestResult(string TestClass, string TestName, Image img)
        {
            string path = Path.Combine(SAVE_RESULTS_DIR, TestClass);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string filename = Path.Combine(path, TestName + ".bmp");
            img.Save(filename, ImageFormat.Bmp);
        }

        public static void DeleteResultDir(string TestClass)
        {
            string path = Path.Combine(SAVE_RESULTS_DIR, TestClass);
            if (Directory.Exists(path))
                Directory.Delete(path,true);
        }
    }
}
