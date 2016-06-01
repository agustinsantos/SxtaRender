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
    /// Demonstrates Uniform Blocks.
    /// The idea of this tutorial comes from
    /// http://www.packtpub.com/article/opengl-glsl-4-using-uniform-blocks-buffer-objects
    /// </summary>
    [Example("Example 3.5: Uniform Blocks", ExampleCategory.Testing, "03. Shaders", 1, Source = "Tutorial03_5", Documentation = "Tutorial-TODO")]
    public class Tutorial03_5 : GameWindow
    {
        public Tutorial03_5()
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
            p = new Program(new Module(400, EXAMPLE_SHADER));
            p.getUniform1f("RadiusInner").set(innerRadius);
            p.getUniform1f("RadiusOuter").set(outerRadius);
            p.getUniform4f("InnerColor").set(innerColor);
            p.getUniform4f("OuterColor").set(outerColor);

            quad = new Mesh<Vertex_V3T2f, uint>(Vertex_V3T2f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLES, MeshUsage.GPU_STATIC, 4, 6);
            quad.addAttributeType(0, 3, AttributeType.A32F, false);
            quad.addAttributeType(1, 2, AttributeType.A32F, false);
            quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(-1, -1, 0), TexCoord = new Vector2f(0, 0) });
            quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(1, -1, 0), TexCoord = new Vector2f(1, 0) });
            quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(-1, 1, 0), TexCoord = new Vector2f(0, 1) });
            quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(1, 1, 0), TexCoord = new Vector2f(1, 1) });
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
            outerRadius += amount;
            if (outerRadius > 0.9f || outerRadius  < 0.35f)
                amount = -amount;
            p.getUniform1f("RadiusOuter").set(outerRadius);
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
            fb.draw(p, m, MeshMode.TRIANGLES, 0, 6);
            this.SwapBuffers();
        }

        #endregion

        #region Fields
        Vector4f innerColor = new Vector4f(1.0f, 1.0f, 0.75f, 1.0f);
        Vector4f outerColor = new Vector4f(0.0f, 0.0f, 0.0f, 0.0f);
        float innerRadius = 0.25f, outerRadius = 0.35f;
        float amount = 0.005f;
        FrameBuffer fb;
        Program p;
        Mesh<Vertex_V3T2f, uint> quad;
        MeshBuffers m;

        const string EXAMPLE_SHADER =
@"#ifdef _VERTEX_
        layout (location = 0) in vec3 VertexPosition;
        layout (location = 1) in vec3 VertexTexCoord;

        out vec3 TexCoord;

        void main()
        {
            TexCoord = VertexTexCoord;
            gl_Position = vec4(VertexPosition,1.0);
        }
#endif
#ifdef _FRAGMENT_
        in vec3 TexCoord;
        layout (location = 0) out vec4 FragColor;

        uniform BlobSettings {
          vec4 InnerColor;
          vec4 OuterColor;
          float RadiusInner;
          float RadiusOuter;
        };

        void main() {
            float dx = TexCoord.x - 0.5;
            float dy = TexCoord.y - 0.5;
            float dist = sqrt(dx * dx + dy * dy);
            FragColor =
               mix( InnerColor, OuterColor,
                     smoothstep( RadiusInner, RadiusOuter, dist )
               );
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
            using (Tutorial03_5 example = new Tutorial03_5())
            {
                example.Run(30.0, 0.0);
            }
        }

        #endregion

        private struct Vertex_V3T2f
        {
            public Vector3f Position;
            public Vector2f TexCoord;
            public static int SizeInBytes
            {
                get { return Vector2f.SizeInBytes + Vector3f.SizeInBytes; }
            }
        }
    }

}
