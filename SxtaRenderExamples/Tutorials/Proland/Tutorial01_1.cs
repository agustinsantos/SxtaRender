// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using OpenTK;
using OpenTK.Input;
using Sxta.Math;
using Sxta.Render;
using Sxta.Render.OpenGLExt;
using System;
using System.Drawing;
using MathHelper = Sxta.Math.MathHelper;

namespace Examples.Tutorials
{
    /// <summary> 
    /// Demonstrates how to draw a plane using quads
    /// </summary>
    [Example("Example 1.01: Drawing a plane using quads", ExampleCategory.Proland, "01. Proland Core", 1, Source = "Tutorial01_1", Documentation = "Tutorial01_1")]
    public class TutorialProland01_1 : GameWindow
    {
        public TutorialProland01_1()
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

            if (e.Key == Key.F12)
            {
                ScreenShot.SaveScreenShot(this.ClientSize, this.ClientRectangle, "Screenshot" + this.GetType().Name + ".bmp");
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
            fb.setClearColor(Color.White);

            p = new Program(new Module(330, FRAGMENT_SHADER));
            uMVMatrix = p.getUniformMatrix4f("deformation.uMVMatrix");
            uOffset = p.getUniform4f("deformation.offset");

        mesh = new Mesh<Vertex_V3f, uint>(Vertex_V3f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLES, MeshUsage.GPU_STATIC);
        mesh.addAttributeType(0, 3, AttributeType.A32F, false);

        mesh.addVertex(new Vertex_V3f() { Position = new Vector3f(-1, -1, 0) });
        mesh.addVertex(new Vertex_V3f() { Position = new Vector3f(1, -1, 0) });
        mesh.addVertex(new Vertex_V3f() { Position = new Vector3f(1, 1, 0) });
        mesh.addVertex(new Vertex_V3f() { Position = new Vector3f(-1, 1, 0) });
        mesh.addIndices(new uint[] {
                            0,1,2,
                            0,2,3,
                        });

        }

        #endregion

        #region OnUnload

        protected override void OnUnload(EventArgs e)
        {
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
            fb.setViewport(new Vector4i(0, 0, Width, Height));
            UniformMatrix4f uPMatrix = p.getUniformMatrix4f("deformation.uPMatrix");
            // fovy, aspect, zNear, zFar
            Matrix4f projection = Matrix4f.CreatePerspectiveFieldOfView((float)MathHelper.ToRadians(60), (float)this.Width / (float)this.Height, 0.01f, 100.0f);
            uPMatrix.set(projection);
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
            Matrix4f mat = Matrix4f.CreateTranslation(1.0f, 1.0f, -12.0f) * Matrix4f.CreateRotation((float)(-Math.PI / 4), 7.0f, -7.0f, 0.5f);
            uMVMatrix.set(mat);
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
            fb.clear(true, false, true);
            for (int j = 0; j < 7; j++)
            {
                for (int i = 0; i < 7; i++)
                {
                    uOffset.set(new Vector4f((j - 3) * 2, (i - 3) * 2, 1, 0));
                    fb.draw(p, mesh);
                }
            }
            this.SwapBuffers();
        }

        #endregion

        #region Fields
        private FrameBuffer fb;
        private Program p;
        private Mesh<Vertex_V3f, uint> mesh;
        private UniformMatrix4f uMVMatrix;
        private Uniform4f uOffset;
        private const string FRAGMENT_SHADER = @"

        uniform struct {
            vec4 offset;
            vec4 camera;
            vec2 blending;
            mat4 uMVMatrix;
            mat4 uPMatrix;
        } deformation;

    #ifdef _VERTEX_

        layout(location=0) in vec3 vertex;
        out vec4 p;

        void main() {
            p = vec4(vertex.xy * deformation.offset.z + deformation.offset.xy, vertex.z, 1.0);
            gl_Position = deformation.uPMatrix * deformation.uMVMatrix * p;
        }

    #endif

    #ifdef _FRAGMENT_

        in vec4 p;
        layout(location=0) out vec4 data;

        void main() {
            data = vec4(vec3(0.2 + 0.3 * sin(10.1 * length(p.xy))), 1.0);
            data.r += mod(dot(floor(deformation.offset.xy / deformation.offset.z + 0.5), vec2(1.0)), 4);
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
            using (TutorialProland01_1 example = new TutorialProland01_1())
            {
                example.Run(30.0, 10.0);
            }
        }

        #endregion

        private struct Vertex_V3f
        {
            public Vector3f Position;
            public static int SizeInBytes
            {
                get { return Vector3f.SizeInBytes; }
            }
        }
    }


}

