// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Sxta.Math;
using Sxta.Render;
using System;
using System.Drawing;

namespace Examples.Tutorials
{
    /// <summary>
    /// Demonstrates the GameWindow class.
    /// </summary>
    [Example("Example 2.8: Blending Colors", ExampleCategory.Core, "2. Drawing", 1, Source = "Tutorial02_8", Documentation = "Tutorial-TODO")]
    public class Tutorial02_8 : GameWindow
    {
        public Tutorial02_8()
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
            fb.setClearColor(Color.White);
            fb.setBlend(true);
            fb.setBlend(true, BlendEquation.ADD, BlendArgument.SRC_ALPHA, BlendArgument.ONE_MINUS_SRC_ALPHA);

            p = new Program(new Module(330, SHADER_330));
            gCenter = p.getUniform3f("gCenter");
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
            angle += 0.02f;
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

            RenderQuads();

            gCenter.set(new Vector3f((float)(0.3*Math.Cos(angle)), (float)(0.3*Math.Sin(angle)), 0.0f));
            gColor.set(new Vector4f(1.0f, 0.0f, 0.0f, 0.5f));
            fb.drawQuad(p);

            this.SwapBuffers();
        }

        private void RenderQuads()
        {
            gCenter.set(new Vector3f(-0.5f, 0.5f, 0.0f));
            gColor.set(new Vector4f(1.0f, 0.0f, 0.0f, 1.0f));
            fb.drawQuad(p);

            gCenter.set(new Vector3f(-0.5f, -0.5f, 0.0f));
            gColor.set(new Vector4f(0.0f, 0.0f, 1.0f, 1.0f));
            fb.drawQuad(p);

            gCenter.set(new Vector3f(0.5f, 0.5f, 0.0f));
            gColor.set(new Vector4f(0.0f, 1.0f, 0.0f, 1.0f));
            fb.drawQuad(p);

            gCenter.set(new Vector3f(0.5f, -0.5f, 0.0f));
            gColor.set(new Vector4f(0.0f, 0.0f, 0.0f, 1.0f));
            fb.drawQuad(p);
        }

        #endregion

        #region Fields
        FrameBuffer fb;
        float angle;
        Uniform3f gCenter;
        Uniform4f gColor;
        Program p;

        const string SHADER_330 = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 Position; 
        uniform vec3 gCenter;   
        void main()
        {
            gl_Position = vec4((Position*0.3+gCenter), 1.0);
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
            using (Tutorial02_8 example = new Tutorial02_8())
            {
                example.Run(30.0, 0.0);
            }
        }

        #endregion
    }
}
