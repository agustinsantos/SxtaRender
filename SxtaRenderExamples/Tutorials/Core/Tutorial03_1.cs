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
    [Example("Example 3.1: Drawing using Shaders", ExampleCategory.CoreUsage, "3. Shaders", 1, Source = "Tutorial03_1", Documentation = "Tutorial-TODO")]
    public class Tutorial03_1 : GameWindow
    {
        public Tutorial03_1()
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
            quad = new Mesh<Vector4f, uint>(Vector4f.SizeInBytes, MeshMode.TRIANGLES, MeshUsage.GPU_STATIC, 3);
            quad.addAttributeType(0, 4, AttributeType.A32F, false);
            quad.addVertex(new Vector4f(-1.0f, -1.0f, 0.0f, 1));
            quad.addVertex(new Vector4f(1.0f, -1.0f, 0.0f, 1));
            quad.addVertex(new Vector4f(0.0f, 1.0f, 0.0f, 1));
 
            fb.setClearColor(Color.MidnightBlue);
        }

        #endregion

        #region OnUnload

        protected override void OnUnload(EventArgs e)
        {
            if (p != null)
                p.Dispose();
            if (quad != null)
                quad.Dispose();
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
            fb.clear(true, false, false);

            fb.draw(p, quad);
            this.SwapBuffers();
        }

        #endregion

        #region Fields
        FrameBuffer fb;
        Program p;
        Mesh<Vector4f, uint> quad;
  
        const string EXAMPLE_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 Position;
        void main()
        {
            gl_Position = vec4(0.5 * Position.x, 0.5 * Position.y, Position.z, 1.0);
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
            using (Tutorial03_1 example = new Tutorial03_1())
            {
                example.Run(30.0, 10.0);
            }
        }

        #endregion
    }
}
