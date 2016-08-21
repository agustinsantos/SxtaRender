using log4net;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace Sxta.Render.OpenGLExt
{
    public class ScreenShot
    {
        // Returns a System.Drawing.Bitmap with the contents of the current framebuffer
        public static Bitmap GrabScreenshot(Size clientSize, Rectangle clientRectangle)
        {
            if (GraphicsContext.CurrentContext == null)
                throw new GraphicsContextMissingException();

            Bitmap bmp = new Bitmap(clientSize.Width, clientSize.Height);
            System.Drawing.Imaging.BitmapData data =
                bmp.LockBits(clientRectangle, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, clientSize.Width, clientSize.Height, PixelFormat.Bgr, OpenTK.Graphics.OpenGL.PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bmp;
        }

        public static Bitmap GetScreenshot(int Width, int Height, PixelFormat pf = PixelFormat.Bgr)
        {
            Bitmap bmp = new Bitmap(Width, Height);
            System.Drawing.Imaging.BitmapData data =
                bmp.LockBits(new Rectangle(0, 0, Width, Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, Width, Height, pf, OpenTK.Graphics.OpenGL.PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            return bmp;
        }
        public static string SaveFrameBuffer(int width, int height, string filename = null)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                filename = "Screenshot.bmp";
            }
            CheckFileVersion(".", ref filename);
            Bitmap bmp = GetScreenshot(width, height, PixelFormat.Bgr);
            bmp.Save(filename);
            return filename;
        }

        public static string SaveScreenShot(Size clientSize, Rectangle clientRectangle, string filename = null)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                filename = "Screenshot.bmp";
                CheckFileVersion(".", ref filename);
            }
            Bitmap bmp = GrabScreenshot(clientSize, clientRectangle);
            bmp.Save(filename);
            return filename;
        }

        private static void CheckFileVersion(string dir, ref string file)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            string ext = Path.GetExtension(file);
            int version = 0;

            foreach (var f in Directory.EnumerateFiles(dir))
            {
                if (Path.GetFileNameWithoutExtension(f).Contains(name) &&
                    Path.GetExtension(f) == ext)
                {
                    string versionStr = Path.GetFileNameWithoutExtension(f).Substring(name.Length);
                    int v;
                    if (!int.TryParse(versionStr, out v))
                        log.WarnFormat("The version string {0} was wrong when processing file {1}", versionStr, file);
                    else
                        version = System.Math.Max(version, v);
                }
            }
            version++;
            file = name + String.Format("{0:000}", version) + ext;
        }
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
