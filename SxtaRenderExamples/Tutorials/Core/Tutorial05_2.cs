// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using OpenTK;
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
    [Example("Example 5.1: Drawing 3D (II)", ExampleCategory.Core, "5. Drawing 3D", 1, Source = "Tutorial05_2", Documentation = "Tutorial-TODO")]
    public class Tutorial05_2 : GameWindow
    {
        public Tutorial05_2()
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

            if (e.Key == Key.P)
            {
                if (mode == Sxta.Render.PolygonMode.FILL)
                    mode = Sxta.Render.PolygonMode.LINE;
                else if (mode == Sxta.Render.PolygonMode.LINE)
                    mode = Sxta.Render.PolygonMode.POINT;
                else
                    mode = Sxta.Render.PolygonMode.FILL; ;
                fb.setPolygonMode(mode, Sxta.Render.PolygonMode.CULL);
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
            fb.setDepthTest(true);
            fb.setDepthRange(0.001f, 1000.0f);
            fb.setFrontFaceCW(false);
            fb.setPolygonMode(mode, Sxta.Render.PolygonMode.CULL);

            p = new Program(new Module(330, EXAMPLE_SHADER));
            uniformMat = p.getUniformMatrix4f("gMatrix");

            cube = new Mesh<Vertex_V3fC4i, uint>(Vertex_V3fC4i.SizeInBytes, MeshMode.TRIANGLES, MeshUsage.GPU_STATIC);
            cube.addAttributeType(0, 3, AttributeType.A32F, false);
            cube.addAttributeType(1, 4, AttributeType.A32I, false);

            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(-1, -1, +1), Color = new Vector4i(255, 0, 0, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(+1, -1, +1), Color = new Vector4i(255, 0, 0, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(+1, +1, +1), Color = new Vector4i(255, 0, 0, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(+1, +1, +1), Color = new Vector4i(255, 0, 0, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(-1, +1, +1), Color = new Vector4i(255, 0, 0, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(-1, -1, +1), Color = new Vector4i(255, 0, 0, 0) });

            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(+1, -1, +1), Color = new Vector4i(0, 255, 0, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(+1, -1, -1), Color = new Vector4i(0, 255, 0, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(+1, +1, -1), Color = new Vector4i(0, 255, 0, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(+1, +1, -1), Color = new Vector4i(0, 255, 0, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(+1, +1, +1), Color = new Vector4i(0, 255, 0, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(+1, -1, +1), Color = new Vector4i(0, 255, 0, 0) });

            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(-1, +1, +1), Color = new Vector4i(0, 0, 255, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(+1, +1, +1), Color = new Vector4i(0, 0, 255, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(+1, +1, -1), Color = new Vector4i(0, 0, 255, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(+1, +1, -1), Color = new Vector4i(0, 0, 255, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(-1, +1, -1), Color = new Vector4i(0, 0, 255, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(-1, +1, +1), Color = new Vector4i(0, 0, 255, 0) });

            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(+1, -1, -1), Color = new Vector4i(0, 255, 255, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(-1, -1, -1), Color = new Vector4i(0, 255, 255, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(-1, +1, -1), Color = new Vector4i(0, 255, 255, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(-1, +1, -1), Color = new Vector4i(0, 255, 255, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(+1, +1, -1), Color = new Vector4i(0, 255, 255, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(+1, -1, -1), Color = new Vector4i(0, 255, 255, 0) });

            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(-1, -1, -1), Color = new Vector4i(255, 0, 255, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(-1, -1, +1), Color = new Vector4i(255, 0, 255, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(-1, +1, +1), Color = new Vector4i(255, 0, 255, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(-1, +1, +1), Color = new Vector4i(255, 0, 255, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(-1, +1, -1), Color = new Vector4i(255, 0, 255, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(-1, -1, -1), Color = new Vector4i(255, 0, 255, 0) });
            
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(-1, -1, -1), Color = new Vector4i(255, 255, 0, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(+1, -1, -1), Color = new Vector4i(255, 255, 0, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(+1, -1, +1), Color = new Vector4i(255, 255, 0, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(+1, -1, +1), Color = new Vector4i(255, 255, 0, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(-1, -1, +1), Color = new Vector4i(255, 255, 0, 0) });
            cube.addVertex(new Vertex_V3fC4i() { Position = new Vector3f(-1, -1, -1), Color = new Vector4i(255, 255, 0, 0) });

            fb.setClearColor(Color.MidnightBlue);
        }

        #endregion

        #region OnUnload

        protected override void OnUnload(EventArgs e)
        {
            if (p != null)
                p.Dispose();
            if (cube != null)
                cube.Dispose();
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
            projection = Matrix4f.CreatePerspectiveFieldOfView((float)(70.0 * 2.0 * Math.PI / 360.0), (float)Width / (float)Height, 0.01f, 100.0f);
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
            mat =  rotation  * projection;
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
            fb.draw(p, cube);
 
            this.SwapBuffers();
        }

        #endregion

        #region Fields
        FrameBuffer fb;
        Matrix4f projection;
        Matrix4f mat = Matrix4f.Identity;
        UniformMatrix4f uniformMat;
        Program p;
        float angle = 0;
        Mesh<Vertex_V3fC4i, uint> cube;
        Sxta.Render.PolygonMode mode = Sxta.Render.PolygonMode.FILL;

        const string EXAMPLE_SHADER = @"
#ifdef _VERTEX_
         layout (location = 0)  in vec3 Position;
         layout (location = 1)  in vec4 Color;

        uniform mat4 gMatrix;   
        out vec4 VertexColor;

        void main()
        {
            gl_Position = gMatrix * vec4(Position, 1.0);
            VertexColor = Color/255.0;
        }
#endif
#ifdef _FRAGMENT_
        in vec4 VertexColor;
        out vec4 FragColor;
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
            using (Tutorial05_2 example = new Tutorial05_2())
            {
                example.Run(30.0, 10.0);
            }
        }

        #endregion


        private struct Vertex_V3fC4i
        {
            public Vector3f Position;
            public Vector4i Color;
            public static int SizeInBytes
            {
                get { return Vector3f.SizeInBytes + Vector4i.SizeInBytes; }
            }
        }
    }
}
