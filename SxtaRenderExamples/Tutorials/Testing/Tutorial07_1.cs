// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using NanoVG;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Sxta.Math;
using Sxta.Render;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Examples.Tutorials
{
    /// <summary>
    /// Demonstrates the GameWindow class.
    /// </summary>
    [Example("Example 7.1: testing.. vector graphics rendering ", ExampleCategory.Testing, "6. Vector Graphics", 1, Source = "Tutorial07_1", Documentation = "Tutorial-TODO")]
    public class Tutorial07_1 : GameWindow
    {
        public Tutorial07_1()
            : base(600, 600)
        {
            Keyboard.KeyDown += Keyboard_KeyDown;
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
            //context = Nvg.CreateContext(fb, NVGcreateFlags.NVG_ANTIALIAS | NVGcreateFlags.NVG_STENCIL_STROKES | NVGcreateFlags.NVG_DEBUG);
            context = Nvg.CreateContext(fb);
        }

        #endregion

        #region OnUnload

        protected override void OnUnload(EventArgs e)
        {
            if (context != null)
                context.Dispose();
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
            // GL.Viewport(0, 0, Width, Height);
            fb.setViewport(new Vector4i(0, 0, Width, Height));
            context.RenderViewport(Width, Height);
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

            // Drawing a simple shape using NanoVG consists of four steps: 
            // 1) begin a new shape, 
            // 2) define the path to draw, 
            // 3) set fill or stroke,
            // 4) and finally fill or stroke the path.
            Nvg.BeginFrame(context, Width, Height, (float)Width / (float)Height);
            //Simple rectangle
            Nvg.BeginPath(context);
            Nvg.Rect(context, 50, 100, 100, 40);
            Nvg.FillColor(context, new Vector4f(1, 0.8f, 0, 1));
            Nvg.Fill(context);

            // Vertical gradient rectangle
            NVGpaint gradient = Nvg.LinearGradient(context, 200, 100, 200, 100 + 40, Nvg.RGBA(128, 255, 128, 255), Nvg.RGBA(0, 64, 0, 255));
            Nvg.BeginPath(context);
            Nvg.Rect(context, 200, 100, 100, 40);
            Nvg.FillPaint(context, gradient);
            Nvg.Fill(context);
            Nvg.EndFrame(context);

            // Horizontal gradient rectangle
            gradient = Nvg.LinearGradient(context, 350, 100, 350 + 100, 100, Nvg.RGBA(255, 128, 128, 255), Nvg.RGBA(64, 0, 0, 255));
            Nvg.BeginPath(context);
            Nvg.Rect(context, 350, 100, 100, 40);
            Nvg.FillPaint(context, gradient);
            Nvg.Fill(context);
            Nvg.EndFrame(context);


            this.SwapBuffers();
        }

        #endregion

        #region Fields
        FrameBuffer fb;
        NVGcontext context;

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
    }
}
