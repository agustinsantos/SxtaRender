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
    /// Demonstrates how to combine several operations by matrix multiplications.
    /// </summary>
    [Example("Example 3.4: Concatenating", ExampleCategory.Learning, "3. Math: Matrix operations", 1, Source = "Tutorial03_4", Documentation = "Tutorial03_4")]
    public class TutorialLearning03_4 : GameWindow
    {
        public TutorialLearning03_4()
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
            uMatrix = p.getUniformMatrix4f("uMatrix");
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
            distX += xAmount;
            distY += yAmount;

            if (distX > 0.7f || distX < -0.7f)
            {
                xAmount = -xAmount;
                if (distX > 0.7f) distX = 0.7f;
                else distX = -0.7f;
            }
            if (distY > 0.7f || distY < -0.7f)
            {
                yAmount = -yAmount;
                if (distY > 0.7f) distY = 0.7f;
                else distY = -0.7f;
            }
            angle += 0.005f;

            Matrix4f translation = Matrix4f.CreateTranslation(distX, distY, 0.0f);
            Matrix4f rotation = Matrix4f.CreateRotationZ(2 * angle);
            Matrix4f scale = Matrix4f.Scale((float)Math.Sin(angle));
            mat = scale * rotation * translation;
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
            uMatrix.set(mat);
            fb.drawQuad(p);
            this.SwapBuffers();
        }

        #endregion

        #region Fields
        float angle;
        float xAmount = 0.004f;
        float yAmount = 0.003f;
        float distX = 0.0f;
        float distY = 0.0f;
        Matrix4f mat = new Matrix4f(1.0f, 0.0f, 0.0f, 0.0f,
                                    0.0f, 1.0f, 0.0f, 0.0f,
                                    0.0f, 0.0f, 1.0f, 0.0f,
                                    0.0f, 0.0f, 0.0f, 1.0f);
        UniformMatrix4f uMatrix;
        FrameBuffer fb;
        Program p;

        const string EXAMPLE_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition; 
        uniform mat4 uMatrix;   
        void main()
        {
            gl_Position = uMatrix * vec4(aPosition, 1.0);
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
            using (TutorialLearning03_4 example = new TutorialLearning03_4())
            {
                example.Run(60.0, 0.0);
            }
        }

        #endregion
    }
}
