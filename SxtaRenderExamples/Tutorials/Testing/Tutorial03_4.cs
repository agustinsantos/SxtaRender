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
    [Example("Example 3.4: Color using shaders", ExampleCategory.Testing, "03. Shaders", 1, Source = "Tutorial03_4", Documentation = "Tutorial-TODO")]
    public class Tutorial03_4 : GameWindow
    {
        public Tutorial03_4()
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

            quad = new Mesh<Vertex14_V3C3f, uint>(Vertex14_V3C3f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLES, MeshUsage.GPU_STATIC, 4);
            quad.addAttributeType(0, 3, AttributeType.A32F, false);
            quad.addAttributeType(1, 3, AttributeType.A32F, false);
            quad.addVertex(new Vertex14_V3C3f() { Position = new Vector3f(-1, -1, 0), Color = new Vector3f(0.0f, 0.2f, 0.9f) });
            quad.addVertex(new Vertex14_V3C3f() { Position = new Vector3f(1, -1, 0), Color = new Vector3f(0.1f, 0.9f, 0.1f) });
            quad.addVertex(new Vertex14_V3C3f() { Position = new Vector3f(-1, 1, 0), Color = new Vector3f(0.9f, 0.2f, 0.0f) });
            quad.addVertex(new Vertex14_V3C3f() { Position = new Vector3f(1, 1, 0), Color = new Vector3f(0.5f, 0.6f, 0.5f) });
            quad.addIndice(0);
            quad.addIndice(1);
            quad.addIndice(2);
            quad.addIndice(2);
            quad.addIndice(1);
            quad.addIndice(3);
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
            fb.clear(true, true, true);
            fb.draw(p, m, MeshMode.TRIANGLES, 0, 6);
            this.SwapBuffers();
        }

        #endregion

        #region Fields
        FrameBuffer fb;
        Program p;
        Mesh<Vertex14_V3C3f, uint> quad;
        MeshBuffers m;

        const string EXAMPLE_SHADER =
@"#ifdef _VERTEX_
        layout (location = 0) in vec3 Position;
        layout (location = 1) in vec3 Color;

        out vec3 VertexColor;

        void main()
        {
            gl_Position = vec4(Position, 1.0);
            VertexColor = Color;
        }
#endif
#ifdef _FRAGMENT_
        in vec3 VertexColor;
        out vec3 FragColor;
 
        void main()
        {
            FragColor =  VertexColor; 
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
            using (Tutorial03_4 example = new Tutorial03_4())
            {
                example.Run(30.0, 0.0);
            }
        }

        #endregion



        private struct Vertex14_V3C3f
        {
            public Vector3f Position;
            public Vector3f Color;
            public static int SizeInBytes
            {
                get { return Vector3f.SizeInBytes + Vector3f.SizeInBytes; }
            }
        }
    }
}
