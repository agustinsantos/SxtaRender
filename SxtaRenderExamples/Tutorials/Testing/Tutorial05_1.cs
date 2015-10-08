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
    [Example("Example 5.1: Drawing 3D (I)", ExampleCategory.Testing, "5. Drawing 3D", 1, Source = "Tutorial05_1", Documentation = "Tutorial-TODO")]
    public class Tutorial05_1 : GameWindow
    {
        public Tutorial05_1()
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
            fb.setDepthRange(0.0001f, 100.0f);
            fb.setFrontFaceCW(true);
            fb.setPolygonMode(mode, Sxta.Render.PolygonMode.CULL);

            p = new Program(new Module(330, EXAMPLE_SHADER));
            timer = p.getUniform1f("timer");

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
            time += 0.01f;
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
            timer.set(time);
            fb.draw(p, cube);
 
            this.SwapBuffers();
        }

        #endregion

        #region Fields
        FrameBuffer fb;
        Program p;
        Uniform1f timer;
        float time = 0;
        Mesh<Vertex_V3fC4i, uint> cube;
        Sxta.Render.PolygonMode mode = Sxta.Render.PolygonMode.FILL;

        const string EXAMPLE_SHADER = @"
#ifdef _VERTEX_

        uniform float timer;
        uniform mat4 Transform;

        mat4 view_frustum(
            float angle_of_view,
            float aspect_ratio,
            float z_near,
            float z_far
        ) {
            return mat4(
                vec4(1.0/tan(angle_of_view * 0.5),           0.0, 0.0, 0.0),
                vec4(0.0, aspect_ratio/tan(angle_of_view * 0.5),  0.0, 0.0),
                vec4(0.0, 0.0,    (z_far+z_near)/(z_far-z_near), 1.0),
                vec4(0.0, 0.0, -2.0*z_far*z_near/(z_far-z_near), 0.0)
            );
        }

        mat4 translate(float x, float y, float z)
        {
            return mat4(
                vec4(1.0, 0.0, 0.0, 0.0),
                vec4(0.0, 1.0, 0.0, 0.0),
                vec4(0.0, 0.0, 1.0, 0.0),
                vec4(x,   y,   z,   1.0)
            );
        }

        mat4 rotate(float theta)
        {
            return mat4(
                vec4(cos(theta),  0.0,  sin(theta), 0.0),
                vec4(0.0,  1.0,   0.0,  0.0),
                vec4(-sin(theta), 0.0,  cos(theta), 0.0),
                vec4(0.0,         0.0,  0.0, 1.0)
            ) * mat4(
                vec4(1.0,  0.0,           0.0,      0.0),
                vec4(0.0,  cos(theta),  sin(theta), 0.0),
                vec4(0.0, -sin(theta),  cos(theta), 0.0),
                vec4(0.0,  0.0,           0.0,      1.0)
            );
        }

         layout (location = 0)  in vec3 Position;
         layout (location = 1)  in vec4 Color;

        out vec4 VertexColor;

        void main()
        {
            gl_Position = view_frustum(radians(60.0), 4.0/4.0, 1.0, 500.0)*
                          translate(0.0, 0.0, 5.0)*
                          rotate(timer)*vec4(Position, 1.0);
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
            using (Tutorial05_1 example = new Tutorial05_1())
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
