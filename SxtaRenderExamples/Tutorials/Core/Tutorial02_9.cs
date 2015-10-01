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
    [Example("Example 2.9: Antialiasing", ExampleCategory.Core, "2. Drawing", 1, Source = "Tutorial02_9", Documentation = "Tutorial-TODO")]
    public class Tutorial02_9 : GameWindow
    {
        public Tutorial02_9()
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
            fb.setClearColor(Color.Black);
            fb.setLineWidth(6.0f);

            p = new Program(new Module(330, WHITE_SHADER_330));
            quad = new Mesh<Vector4f, uint>(Vector4f.SizeInBytes, sizeof(uint), MeshMode.LINES, MeshUsage.GPU_STATIC, 6);
            quad.addAttributeType(0, 4, AttributeType.A32F, false);
            quad.addVertex(new Vector4f(-0.3f, -0.3f + 0.5f, 0, 1));
            quad.addVertex(new Vector4f(0.3f, -0.3f + 0.5f, 0, 1));
            quad.addVertex(new Vector4f(-0.3f, 0.3f + 0.5f, 0, 1));
            quad.addVertex(new Vector4f(0.3f, 0.3f + 0.5f, 0, 1));
            quad.addVertex(new Vector4f(-0.3f, -0.3f - 0.5f, 0, 1));
            quad.addVertex(new Vector4f(0.3f, -0.3f - 0.5f, 0, 1));
            quad.addVertex(new Vector4f(-0.3f, 0.3f - 0.5f, 0, 1));
            quad.addVertex(new Vector4f(0.3f, 0.3f - 0.5f, 0, 1));
            quad.addIndice(0);
            quad.addIndice(1);
            quad.addIndice(2);
            quad.addIndice(2);
            quad.addIndice(1);
            quad.addIndice(3);
            quad.addIndice(4 + 0);
            quad.addIndice(4 + 1);
            quad.addIndice(4 + 2);
            quad.addIndice(4 + 2);
            quad.addIndice(4 + 1);
            quad.addIndice(4 + 3);
            m = quad.getBuffers();
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

            // Draw lines without antialing
            fb.draw(p, m, MeshMode.LINE_STRIP, 0, 6);

            // Draw lines antialiased
            fb.setLineSmooth(true);
            fb.setBlend(true, BlendEquation.ADD, BlendArgument.SRC_ALPHA, BlendArgument.ONE_MINUS_SRC_ALPHA);
            fb.draw(p, m, MeshMode.LINE_STRIP, 6, 6);

            // Put everything back the way we found it
            fb.setBlend(false);
            fb.setLineSmooth(false);

            this.SwapBuffers();
        }

        #endregion

        #region Fields
        FrameBuffer fb;
        Program p;
        Mesh<Vector4f, uint> quad;
        MeshBuffers m;


        const string WHITE_SHADER_330 = @"
#ifdef _FRAGMENT_
        layout(location=0) out vec4 color;

        void main() 
        { 
            color = vec4(1.0, 1.0, 1.0, 1); 
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
            using (Tutorial02_9 example = new Tutorial02_9())
            {
                example.Run(30.0, 0.0);
            }
        }

        #endregion
    }
}
