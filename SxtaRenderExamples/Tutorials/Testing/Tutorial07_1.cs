// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using OpenTK;
using OpenTK.Input;
using Sxta.Math;
using Sxta.Render;
using Sxta.Render.OpenGLExt;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Examples.Tutorials
{
    /// <summary>
    /// Demonstrates the GameWindow class.
    /// </summary>
    [Example("Example 7.0: Using Fonts", ExampleCategory.Testing, "7. Drawing Fonts", 1, Source = "Tutorial07_1", Documentation = "Tutorial-TODO")]
    public class Tutorial07_1 : GameWindow
    {
        public Tutorial07_1()
            : base(600, 600)
        {
            Keyboard.KeyDown += Keyboard_KeyDown;
            //MeasureStringSizeFFormat();
        }

        #region Keyboard_KeyDown

        /// <summary>
        /// Occurs when a key is pressed.
        /// </summary>
        /// <param name="sender">The KeyboardDevice which generated this event.</param>
        /// <param name="e">The key that was pressed.</param>
        void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Exit();

            if (e.Key == Key.F11)
                if (this.WindowState == WindowState.Fullscreen)
                    this.WindowState = WindowState.Normal;
                else
                    this.WindowState = WindowState.Fullscreen;

            if (e.Key == Key.F12)
            {
                ScreenShot.SaveScreenShot(this.ClientSize, this.ClientRectangle);
            }
        }

        #endregion

        #region OnLoad

        /// <summary>
        /// Setup OpenGL and load resources here.
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            fb = new FrameBuffer(true);
            fb.setClearColor(Color.MidnightBlue);
            FrameBuffer.LogOpenGLInfo();
        }

        #endregion

        #region OnUnload

        protected override void OnUnload(EventArgs e)
        {
            if (fb != null)
                fb.Dispose();
            base.OnUnload(e);
        }

        #endregion

        #region OnResize

        /// <summary>
        /// Respond to resize events here.
        /// </summary>
        /// <param name="e">Contains information on the new GameWindow size.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnResize(EventArgs e)
        {
            fb.setViewport(new Vector4i(0, 0, Width, Height));
        }

        #endregion

        #region OnUpdateFrame

        /// <summary>
        /// Add your game logic here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // Nothing to do!
        }

        #endregion

        #region OnRenderFrame

        /// <summary>
        /// Add your game rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            fb.clear(true, true, true);
            // Nothing to do!
            this.SwapBuffers();
        }

        #endregion

        #region Fields

        FrameBuffer fb;

        #endregion

        #region public static void Main()

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (Tutorial07_1 example = new Tutorial07_1())
            {
                example.Run(30.0, 0.0);
            }
        }

        #endregion
        private void MeasureStringSizeFFormat()
        {

            // Set up string. 
            string measureString = "AaFfIiOo";
            FontFamily fontFamily = new FontFamily("Tahoma");
            System.Drawing.Font stringFont = new System.Drawing.Font(
               fontFamily,
               50,
               FontStyle.Regular,
               GraphicsUnit.Pixel);
            // Set character ranges to "First" and "Second".
            CharacterRange[] characterRanges = { new CharacterRange(0, 1), 
                                                 new CharacterRange(1, 1),
                                                 new CharacterRange(2, 1),
                                                 new CharacterRange(3, 1),
                                                 new CharacterRange(4, 1)};
            SolidBrush solidBrush = new SolidBrush(Color.FromArgb(255, 0, 0, 255));


            // Set maximum layout size.
            SizeF layoutSize = new SizeF(700.0F, 100.0F);
            RectangleF layoutRect = new RectangleF(0, 0, 700.0F, 100.0F);

            // Set string format.
            StringFormat stringFormat = StringFormat.GenericDefault;
            stringFormat.SetMeasurableCharacterRanges(characterRanges);
            stringFormat.Alignment = StringAlignment.Near;

            // Measure string.
            SizeF[] stringSize = new SizeF[measureString.Length];
            Bitmap text_bmp = new Bitmap(ClientSize.Width, ClientSize.Height);
            FontChar[] fc = new FontChar[measureString.Length];
            using (Graphics gfx = Graphics.FromImage(text_bmp))
            {
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;


                // Measure two ranges in string.
                Region[] stringRegions = gfx.MeasureCharacterRanges(measureString, stringFont, layoutRect, stringFormat);
                // Draw rectangle representing size of string.
                gfx.DrawString(measureString, stringFont, solidBrush, layoutRect, stringFormat);
                for (int i = 0; i < characterRanges.Length; i++)
                {
                    Rectangle measureRect = Rectangle.Truncate(stringRegions[i].GetBounds(gfx));
                    // we need to store the font width for later drawing
                    fc[i] = new FontChar();
                    fc[i].posX = measureRect.X;
                    fc[i].posY = measureRect.Y;
                    fc[i].width = measureRect.Width;
                    fc[i].height = measureRect.Height;

                    //gfx.DrawRectangle(new Pen(Color.Red, 0.1f), measureRect);
                }
            }
            text_bmp.Save("fontexample.bmp");
            for (int i = 0; i < characterRanges.Length; i++)
            {
                // Most OpenGL cards need textures to be in powers of 2 (128x512 1024X1024 etc etc) so
                // to be safe we will conform to this and calculate the nearest power of 2 for the glyph width and height
                int widthPow2 = UpperPowerOfTwo(fc[i].width);
                int heightPow2 = UpperPowerOfTwo(fc[i].height);
                // now we set the texture co-ords for our quad it is a simple
                // triangle billboard with tex-cords as shown
                //  s0/t0  ---- s1,t0
                //         |\ |
                //         | \|
                //  s0,t1  ---- s1,t1
                // each quad will have the same s0 and the range s0-s1 == 0.0 -> 1.0
                float s0 = 0.0f;
                // we now need to scale the tex cord to it ranges from 0-1 based on the coverage
                // of the glyph and not the power of 2 texture size. This will ensure that kerns
                // / ligatures match
                float s1 = fc[i].width * 1.0f / widthPow2;
                // t0 will always be the same
                float t0 = 0.0f;
                // this will scale the height so we only get coverage of the glyph as above
                float t1 = fc[i].height * -1.0f / heightPow2;

                using (Bitmap img = new Bitmap(widthPow2, heightPow2))
                {
                    using (Graphics gfx2 = Graphics.FromImage(img))
                    {
                        gfx2.DrawImage(text_bmp, 0, 0, new Rectangle(fc[i].posX, fc[i].posY, fc[i].width, fc[i].height), GraphicsUnit.Pixel);
                    }
                    img.Save("fontexample" + i + ".bmp");
                }
            }
        }
        public int UpperPowerOfTwo(int num)
        {
            int v = num > 0 ? num - 1 : 0;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v++;
            return v;
        }

        private List<Char> GetPrintableChars()
        {
            List<Char> printableChars = new List<char>();
            for (int i = char.MinValue; i <= char.MaxValue; i++)
            {
                char c = Convert.ToChar(i);
                if (!char.IsControl(c))
                {
                    printableChars.Add(c);
                }
            }
            return printableChars;
        }

        public struct FontChar
        {
            public int width; /// the width of the font
            public int height; /// the height of the font
            public int posX, posY;
            public int textureID; ///  the texture id of the font billboard
        }
    }
}
