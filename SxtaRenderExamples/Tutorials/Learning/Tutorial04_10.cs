// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using OpenTK;
using OpenTK.Graphics.OpenGL;
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
    /// Demonstrates how to draw different geometries
    /// </summary>
    [Example("Example 4.10: Drawing Platonic solids", ExampleCategory.Learning, "4. Drawing", 1, Source = "Tutorial04_10", Documentation = "Tutorial04_10")]
    public class TutorialLearning04_10 : GameWindow
    {
        public TutorialLearning04_10()
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

            // Model 1. Cube
            mesh1 = MeshUtils.GenerateSolidCube(1.0f);
            // Model 2. Tetrahedron 
            mesh2 = MeshUtils.GenerateSolidTetrahedron();
            // Model 3. Octahedron 
            mesh3 = MeshUtils.GenerateSolidOctahedron();
            // Model 4. Dodecahedron 
            mesh4 = MeshUtils.GenerateSolidDodecahedron();
            // Model 5. Dodecahedron 
            mesh5 = MeshUtils.GenerateSolidIcosahedron();
            // Model 6. Dodecahedron 
            mesh6 = MeshUtils.GenerateSolidRhombicDodecahedron();

            fb.setClearColor(Color.White);
        }

        #endregion

        #region OnUnload

        protected override void OnUnload(EventArgs e)
        {
            if (p != null)
                p.Dispose();
            if (mesh1 != null)
                mesh1.Dispose();
            if (mesh2 != null)
                mesh2.Dispose();
            if (mesh3 != null)
                mesh3.Dispose();
            if (mesh4 != null)
                mesh4.Dispose();
            if (mesh5 != null)
                mesh5.Dispose();
            if (mesh6 != null)
                mesh6.Dispose();
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

            Matrix4f camera = Matrix4f.CreateTranslation(-3.0f, -1.0f, -7.0f);

            mat = camera * Matrix4f.CreateTranslation(0.0f, 3.0f, 0.0f) * Matrix4f.CreateRotation(angle * 3, 0.0f, 1.0f, 0.5f);
            uMVMatrix.set(mat);
            fb.draw(p, mesh1);

            mat = camera * Matrix4f.CreateTranslation(2.0f, 3.0f, 0.0f) * Matrix4f.CreateRotation(angle * 3, 0.0f, 1.0f, 0.5f);
            uMVMatrix.set(mat);
            fb.draw(p, mesh2);

            mat = camera * Matrix4f.CreateTranslation(4.0f, 3.0f, 0.0f) * Matrix4f.CreateRotation(angle * 3, 0.0f, 1.0f, 0.5f);
            uMVMatrix.set(mat);
            fb.draw(p, mesh3);

            mat = camera * Matrix4f.CreateTranslation(0.0f, 0.0f, 0.0f) * Matrix4f.Scale(0.5f) * Matrix4f.CreateRotation(angle * 3, 0.0f, 1.0f, 0.5f);
            uMVMatrix.set(mat);
            fb.draw(p, mesh4);

            mat = camera * Matrix4f.CreateTranslation(2.0f, 0.0f, 0.0f) * Matrix4f.CreateRotation(angle * 3, 0.0f, 1.0f, 0.5f);
            uMVMatrix.set(mat);
            fb.draw(p, mesh5);

            mat = camera * Matrix4f.CreateTranslation(4.0f, 0.0f, 0.0f) * Matrix4f.CreateRotation(angle * 3, 0.0f, 1.0f, 0.5f);
            uMVMatrix.set(mat);
            fb.draw(p, mesh6);

            RenderAxes(camera);

            this.SwapBuffers();
        }

        public virtual void RenderAxes(Matrix4f camera, int num = 10)
        {
            for (int i = 0; i < num; i++)
            {
                mat = camera * Matrix4f.CreateTranslation(i, 0.0f, 0.0f) * Matrix4f.Scale(0.25f, 0.1f, 0.1f);
                uMVMatrix.set(mat);
                fb.draw(p, mesh1);
            }
            for (int i = 0; i < num; i++)
            {
                mat = camera * Matrix4f.CreateTranslation(0.0f, i, 0.0f) * Matrix4f.Scale(0.1f, 0.25f, 0.1f);
                uMVMatrix.set(mat);
                fb.draw(p, mesh1);
            }
            for (int i = 0; i < num; i++)
            {
                mat = camera * Matrix4f.CreateTranslation(0.0f, 0.0f, i) * Matrix4f.Scale(0.1f, 0.1f, 0.25f);
                uMVMatrix.set(mat);
                fb.draw(p, mesh1);
            }
        }
        #endregion

        #region Fields
        FrameBuffer fb;
        Program p;
        //Mesh<Vertex_V3C3f, uint> mesh1;
        Mesh<Vertex_V3N3T2f, ushort> mesh1, mesh2, mesh3, mesh4, mesh5, mesh6;
        Matrix4f mat;
        UniformMatrix4f uMVMatrix;
        float angle = 0;

        const string FRAGMENT_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in vec3 aNormal;
        layout (location = 2) in vec3 aTextCoord;

        uniform mat4 uMVMatrix;
        uniform mat4 uPMatrix;

        out vec3 vColor;

        void main()
        {
            gl_Position = uPMatrix * uMVMatrix * vec4(aPosition, 1.0);

            // We use normal vectors as (fake) color.
            // in later examples we will use normals for proper ilumination
            vColor = (aNormal + vec3(1.0, 1.0, 1.0))/2;
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
            using (TutorialLearning04_10 example = new TutorialLearning04_10())
            {
                example.Run(30.0, 10.0);
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
