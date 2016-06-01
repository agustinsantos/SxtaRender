// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using OpenTK;
using OpenTK.Input;
using Sxta.Math;
using Sxta.Render;
using System;

namespace Examples.Tutorials
{
    /// <summary>
    /// Demonstrates the GameWindow class.
    /// </summary>
    [Example("Example 4.6: Perspective Matrix", ExampleCategory.Testing, "04. Matrix Transformation", 1, Source = "Tutorial04_6", Documentation = "Tutorial-TODO")]
    public class Tutorial04_6 : GameWindow
    {
        public Tutorial04_6()
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
            fb.setDepthTest(true);
            fb.setDepthRange(0.0001f, 100.0f);

            p = new Program(new Module(330, EXAMPLE_SHADER));
            uniformMat = p.getUniformMatrix4f("gMatrix");
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
            fb.setViewport(new Vector4i(0, 0, Width, Height));
            projection = Matrix4f.CreatePerspectiveFieldOfView((float)(60 * 2 * Math.PI / 360), (float)Width / (float)Height, 0.001f, 1000.0f);
            Matrix4f translation = Matrix4f.CreateTranslation(0.0f, 0.0f, -5.0f);
            projection = translation * projection;
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
            angle += 0.005f;
            Matrix4f rotation = Matrix4f.CreateRotationY(angle);
            mat = rotation * projection;
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
            uniformMat.set(mat);
            fb.drawQuad(p);
            this.SwapBuffers();
        }

        #endregion

        #region Fields
        float angle;
        Matrix4f projection;
        Matrix4f mat = Matrix4f.Identity;
        UniformMatrix4f uniformMat;
        FrameBuffer fb;
        Program p;

        const string EXAMPLE_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 Position; 
        uniform mat4 gMatrix;   
        void main()
        {
            gl_Position = gMatrix * vec4(Position, 1.0);
        }
#endif
#ifdef _FRAGMENT_
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(1.0, 0.0, 0.0, 1.0); 
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
            using (Tutorial04_6 example = new Tutorial04_6())
            {
                example.Run(60.0, 0.0);
            }
        }

        #endregion
    }
}
