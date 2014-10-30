// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Sxta.Math;
using Sxta.Render;
using Sxta.Render.Resources;
using Sxta.Render.Resources.XmlResources;
using System;
using System.Drawing;
using System.IO;

namespace Examples.Tutorials
{
    /// <summary>
    /// Demonstrates the GameWindow class.
    /// </summary>
    [Example("Example 2.2: Mesh Resource (sphere)", ExampleCategory.Resource, "2. Mesh Resource", 1, Source = "TutorialRes02_2", Documentation = "Tutorial-TODO")]
    public class TutorialRes02_2 : GameWindow
    {
        public TutorialRes02_2()
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
            resLoader = new XMLResourceLoader();
            resLoader.addPath(dir + "/Resources/Meshes");
            resManager = new ResourceManager(resLoader);
            res = (MeshResource)resManager.loadResource("sphere.mesh");
            mesh = (MeshBuffers)res.get();

            fb = new FrameBuffer(true);
            fb.setDepthTest(true);
            fb.setDepthRange(0.0001f, 100.0f);
            //fb.setFrontFaceCW(false);
            fb.setPolygonMode(Sxta.Render.PolygonMode.LINE, Sxta.Render.PolygonMode.FILL);
            p = new Program(new Module(330, EXAMPLE_SHADER));
            timer = p.getUniform1f("timer");
            fb.setClearColor(Color.Black);
        }

        #endregion

        #region OnUnload

        protected override void OnUnload(EventArgs e)
        {
            resManager.releaseResource(res);
            if (resManager != null)
                resManager.Dispose();

            if (p != null)
                p.Dispose();
            if (mesh != null)
                mesh.Dispose();
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
            fb.draw(p, mesh, mesh.mode, 0, mesh.nvertices, 1, mesh.nindices);
            this.SwapBuffers();
        }

        #endregion

        #region Fields
        string dir = ".";
        XMLResourceLoader resLoader;
        ResourceManager resManager;
        FrameBuffer fb;
        Program p;
        Uniform1f timer;
        float time = 0;

        MeshResource res;
        MeshBuffers mesh;

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

        layout (location = 0) in vec3 Position;
        layout(location = 1) in vec3 normal;
        layout (location = 2) in vec2 TexCoord;
        layout(location = 3) in vec4 Color;

        out vec4 VertexColor;

        void main()
        {
            gl_Position = view_frustum(radians(60.0), 4.0/4.0, 1.0, 500.0)*
                          translate(0.0, 0.0, 5.0)*
                          rotate(timer)*vec4(Position, 1.0);
            VertexColor = Color;
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
            using (TutorialRes02_2 example = new TutorialRes02_2())
            {
                example.Run(30.0, 10.0);
            }
        }

        #endregion
    }
}
