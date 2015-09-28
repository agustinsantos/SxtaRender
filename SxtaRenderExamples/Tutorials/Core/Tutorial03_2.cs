// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Sxta.Math;
using Sxta.Render;
using System;

namespace Examples.Tutorials
{
    /// <summary>
    /// Demonstrates the GameWindow class.
    /// </summary>
    [Example("Example 3.2: Using Uniform with Shaders", ExampleCategory.CoreUsage, "3. Shaders", 1, Source = "Tutorial03_2", Documentation = "Tutorial-TODO")]
    public class Tutorial03_2 : GameWindow
    {
        public Tutorial03_2()
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
            p = new Program(new Module(330, EXAMPLE_SHADER));
            gScale = p.getUniform1f("gScale");
            gColor = p.getUniform4f("gColor");
        }

        #endregion

        #region OnUnload

        protected override void OnUnload(EventArgs e)
        {
            if (p != null)
                p.Dispose();
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

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
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
            Scale += 0.005f;
            if (Scale >= 1)
                Scale = 0;
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
            fb.clear(true, false, false);
            gScale.set(Scale);
            gColor.set(new Vector4f(Scale, 0.3f, Scale/2, 1.0f));
            fb.drawQuad(p);
            this.SwapBuffers();
        }

        #endregion

        #region Fields
        float Scale = 0.0f;

        FrameBuffer fb;
        Program p;
        Uniform1f gScale;
        Uniform4f gColor;

        const string EXAMPLE_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 Position; 
        uniform float gScale;   
        void main()
        {
            gl_Position = vec4(gScale * Position.x, gScale * Position.y, Position.z, 1.0);
        }
#endif
#ifdef _FRAGMENT_
        out vec4 FragColor;
        uniform vec4 gColor;
        void main()
        {
            FragColor = gColor; 
        }
#endif";

        #endregion

        #region public static void Main()

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (Tutorial03_2 example = new Tutorial03_2())
            {
                example.Run(30.0, 10.0);
            }
        }

        #endregion
    }
}
