// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Sxta.Math;
using Sxta.Render;
using System;
using System.Drawing;
using MathHelper = Sxta.Math.MathHelper;

namespace Examples.Tutorials
{
    /// <summary>
    /// Demonstrates how to draw a simple figure (cube) in 3D
    /// </summary>
    [Example("Example 4.03: Drawing in 3D (Vertex)", ExampleCategory.Learning, "4. Drawing", 1, Source = "Tutorial04_3", Documentation = "Tutorial04_3")]
    public class TutorialLearning04_3 : GameWindow
    {
        public TutorialLearning04_3()
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

            p = new Program(new Module(330, FRAGMENT_SHADER));
            uMVMatrix = p.getUniformMatrix4f("uMVMatrix");

            UniformMatrix4f uPMatrix = p.getUniformMatrix4f("uPMatrix");
            // fovy, aspect, zNear, zFar
            Matrix4f projection = Matrix4f.CreatePerspectiveFieldOfView((float)MathHelper.ToRadians(60), (float)this.Width / (float)this.Height, 0.01f, 100.0f);
            uPMatrix.set(projection);

            mesh = new Mesh<Vertex_V3C3f, uint>(Vertex_V3C3f.SizeInBytes, MeshMode.TRIANGLES, MeshUsage.GPU_STATIC);
            mesh.addAttributeType(0, 3, AttributeType.A32F, false);
            mesh.addAttributeType(1, 3, AttributeType.A32F, false);

            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, +1), Color = new Vector3f(1, 0, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(+1, -1, +1), Color = new Vector3f(1, 0, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(+1, +1, +1), Color = new Vector3f(1, 0, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(+1, +1, +1), Color = new Vector3f(1, 0, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, +1, +1), Color = new Vector3f(1, 0, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, +1), Color = new Vector3f(1, 0, 0) });

            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(+1, -1, +1), Color = new Vector3f(0, 1, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(+1, -1, -1), Color = new Vector3f(0, 1, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(+1, +1, -1), Color = new Vector3f(0, 1, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(+1, +1, -1), Color = new Vector3f(0, 1, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(+1, +1, +1), Color = new Vector3f(0, 1, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(+1, -1, +1), Color = new Vector3f(0, 1, 0) });

            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, +1, +1), Color = new Vector3f(0, 0, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(+1, +1, +1), Color = new Vector3f(0, 0, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(+1, +1, -1), Color = new Vector3f(0, 0, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(+1, +1, -1), Color = new Vector3f(0, 0, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, +1, -1), Color = new Vector3f(0, 0, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, +1, +1), Color = new Vector3f(0, 0, 1) });

            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(+1, -1, -1), Color = new Vector3f(0, 1, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, -1), Color = new Vector3f(0, 1, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, +1, -1), Color = new Vector3f(0, 1, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, +1, -1), Color = new Vector3f(0, 1, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(+1, +1, -1), Color = new Vector3f(0, 1, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(+1, -1, -1), Color = new Vector3f(0, 1, 1) });

            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, -1), Color = new Vector3f(1, 0, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, +1), Color = new Vector3f(1, 0, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, +1, +1), Color = new Vector3f(1, 0, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, +1, +1), Color = new Vector3f(1, 0, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, +1, -1), Color = new Vector3f(1, 0, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, -1), Color = new Vector3f(1, 0, 1) });

            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, -1), Color = new Vector3f(1, 1, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(+1, -1, -1), Color = new Vector3f(1, 1, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(+1, -1, +1), Color = new Vector3f(1, 1, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(+1, -1, +1), Color = new Vector3f(1, 1, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, +1), Color = new Vector3f(1, 1, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, -1), Color = new Vector3f(1, 1, 0) });

            fb.setClearColor(Color.WhiteSmoke);
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
            angle += 0.01f;
            mat = Matrix4f.CreateTranslation(0.0f, 0.0f, -6.0f) * Matrix4f.CreateRotation(angle, 0.0f, 1.0f, 0.5f);
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

            fb.draw(p, mesh);
            this.SwapBuffers();
        }

        #endregion

        #region Fields
        FrameBuffer fb;
        Program p;
        Mesh<Vertex_V3C3f, uint> mesh;
        Matrix4f mat;
        UniformMatrix4f uMVMatrix;
        float angle = 0;

        const string FRAGMENT_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in vec3 aColor;

        uniform mat4 uMVMatrix;
        uniform mat4 uPMatrix;

        out vec3 vColor;

        void main()
        {
            gl_Position = uPMatrix * uMVMatrix * vec4(aPosition, 1.0);
            vColor = aColor;
        }
#endif
#ifdef _FRAGMENT_
        in vec3 vColor;
        out vec3 FragColor;
 
        void main()
        {
            FragColor =  vColor; 
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
            using (TutorialLearning04_3 example = new TutorialLearning04_3())
            {
                example.Run(30.0, 0.0);
            }
        }

        #endregion

        private struct Vertex_V3C3f
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
