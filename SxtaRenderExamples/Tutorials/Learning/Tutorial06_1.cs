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
using System.Drawing.Imaging;
using MathHelper = Sxta.Math.MathHelper;

namespace Examples.Tutorials
{
    /// <summary>
    /// Demonstrates how Debuging Normals
    /// </summary>
    [Example("Example 6.1: Debuging Normals", ExampleCategory.Learning, "6. Lighting", 1, Source = "Tutorial06_1", Documentation = "Tutorial06_1")]
    public class TutorialLearning06_1 : GameWindow
    {
        public TutorialLearning06_1()
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
            // position the camera 
            camera = new FirstPersonCamera(this);
            camera.LookAt(new Vector3f(2, 2, 7), new Vector3f(0, 0, 0), new Vector3f(0, 1, 0));

            // fovy, aspect, zNear, zFar
            Matrix4f projection = Matrix4f.CreatePerspectiveFieldOfView((float)MathHelper.ToRadians(60), (float)this.Width / (float)this.Height, 0.01f, 100.0f);
            uPMatrix.set(projection);

            // Model 1. Sphere 
            mesh1 = MeshUtils.GenerateSolidSphere(1.0f, 40, 40);
            // Model 2. Cone 
            mesh2 = MeshUtils.GenerateSolidCone(1.0, 2.0, 20, 20);
            // Model 3. Cylinder 
            mesh3 = MeshUtils.GenerateSolidCylinder(1.0, 1.0, 20, 20);
            // Model 4. Torus 
            mesh4 = MeshUtils.GenerateSolidTorus(0.5, 1.0, 20, 20);


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
            camera.Update((float)e.Time);
            angle += 0.01f;
            //mat = Matrix4f.CreateRotationZ(-angle) * camera.Matrix;
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

            mat = Matrix4f.CreateRotation(angle * 3, 0.0f, 1.0f, 0.5f) * Matrix4f.CreateTranslation(0.5f, 3.0f, 0.0f) * camera.ViewMatrix;
            uMVMatrix.set(mat);
            fb.draw(p, mesh1);

            mat = Matrix4f.CreateRotation(angle * 3, 0.0f, 1.0f, 0.5f) * Matrix4f.CreateTranslation(5.0f, 3.0f, 0.0f) * camera.ViewMatrix;
            uMVMatrix.set(mat);
            fb.draw(p, mesh2);

            mat = Matrix4f.CreateRotation(angle * 3, 0.0f, 1.0f, 0.5f) * Matrix4f.CreateTranslation(0.5f, 0.0f, 0.0f) * camera.ViewMatrix;
            uMVMatrix.set(mat);
            fb.draw(p, mesh3);

            mat = Matrix4f.CreateRotation(angle * 3, 0.0f, 1.0f, 0.5f) * Matrix4f.CreateTranslation(5.0f, 0.0f, 0.0f) * camera.ViewMatrix;
            uMVMatrix.set(mat);
            fb.draw(p, mesh4);

            this.SwapBuffers();
        }

        #endregion

        #region Fields
        FrameBuffer fb;
        Program p;
        Mesh<Vertex_V3N3T2f, ushort> mesh1, mesh2, mesh3, mesh4;
        Matrix4f mat;
        UniformMatrix4f uMVMatrix;
        private FirstPersonCamera camera;
        float angle = 0;

        const string FRAGMENT_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in vec3 aNormal;

        uniform mat4 uMVMatrix;
        uniform mat4 uPMatrix;

        out vec3 vColor;

        void main()
        {
            gl_Position = uPMatrix * uMVMatrix * vec4(aPosition, 1.0);

            // We use normal vectors as (fake) color.
            // in later examples we will use normals for proper ilumination
            vColor = normalize(aNormal);
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
            using (TutorialLearning06_1 example = new TutorialLearning06_1())
            {
                example.Run(30.0, 10.0);
            }
        }

        #endregion
    }


}
