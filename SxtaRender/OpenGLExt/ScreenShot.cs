using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static void SaveScreenShot(Size clientSize, Rectangle clientRectangle, string filename = null)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                filename = "screenshot.bmp";
            }
            Bitmap bmp = GrabScreenshot(clientSize, clientRectangle);
            bmp.Save(filename);
         }
    }
}
