// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using OpenTK;
using OpenTK.Input;
using Sxta.Math;
using Sxta.Render;
using System;
using MathHelper = Sxta.Math.MathHelper;

namespace Examples.Tutorials
{
    /// <summary>
    /// Demonstrates how to use Perspective Projection Matrix.
    /// </summary>
    [Example("Example 3.7: Camera", ExampleCategory.Learning, "3. Math: Matrix operations", 1, Source = "Tutorial03_7", Documentation = "Tutorial03_7")]
    public class TutorialLearning03_7 : GameWindow
    {
        public TutorialLearning03_7()
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

            p = new Program(new Module(330, EXAMPLE_SHADER));
            uMVMatrix = p.getUniformMatrix4f("uMVMatrix");
            uColor = p.getUniform3f("uColor");

            uPMatrix = p.getUniformMatrix4f("uPMatrix");
            // position the camera 
            camera = new FirstPersonCamera(this);
            camera.Position = new Vector3f(0, 0, -5);

            triangle = new Mesh<Vector2f, uint>(Vector2f.SizeInBytes, MeshMode.TRIANGLES, MeshUsage.GPU_STATIC, 3);
            triangle.addAttributeType(0, 2, AttributeType.A32F, false);

            // Load up a triangle
            triangle.addVertex(new Vector2f(0.0f, 0.2f));
            triangle.addVertex(new Vector2f(0.2f, -0.2f));
            triangle.addVertex(new Vector2f(-0.2f, -0.2f));

        }

        #endregion

        #region OnUnload

        protected override void OnUnload(EventArgs e)
        {
            if (p != null)
                p.Dispose();
            if (fb != null)
                fb.Dispose();
            if (triangle != null)
                triangle.Dispose();

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
            camera.Resize(Width, Height);
            camera.Update();

            // fovy, aspect, zNear, zFar
            //Matrix4f projection = Matrix4f.CreatePerspectiveFieldOfView((float)MathHelper.ToRadians(60), (float)this.Width / (float)this.Height, 0.01f, 100.0f);
            uPMatrix.set(camera.Projection);

        }

        #endregion
        #region OnRenderFrame
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            camera.Update((float)e.Time);
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

            Matrix4f cam = camera.Matrix;
            // Red
            uColor.set(new Vector3f(1.0f, 0.0f, 0.0f));
            mat = Matrix4f.CreateTranslation(-0.6f, -0.4f, -1.0f) * cam;
            float[] vals = (float[])mat;
            uMVMatrix.set(mat);
            fb.draw(p, triangle);

            // Green
            uColor.set(new Vector3f(0.0f, 1.0f, 0.0f));
            mat = Matrix4f.CreateTranslation(-0.4f, -0.4f, -2.0f) * cam;
            uMVMatrix.set(mat);
            fb.draw(p, triangle);

            // Blue
            uColor.set(new Vector3f(0.0f, 0.0f, 1.0f));
            mat = Matrix4f.CreateTranslation(-0.2f, -0.4f, -3.0f) * cam;
            uMVMatrix.set(mat);
            fb.draw(p, triangle);

            // Light Blue
            uColor.set(new Vector3f(0.0f, 1.0f, 1.0f));
            mat = Matrix4f.CreateTranslation(0.0f, -0.4f, -4.0f) * cam;
            uMVMatrix.set(mat);
            fb.draw(p, triangle);

            // Yellow
            uColor.set(new Vector3f(01.0f, 1.0f, 0.0f));
            mat = Matrix4f.CreateTranslation(0.2f, -0.4f, -5.0f) * cam;
            uMVMatrix.set(mat);
            fb.draw(p, triangle);

            this.SwapBuffers();
        }

        #endregion

        #region Fields
        Matrix4f mat = new Matrix4f(1.0f, 0.0f, 0.0f, 0.0f,
                                    0.0f, 1.0f, 0.0f, 0.0f,
                                    0.0f, 0.0f, 1.0f, 0.0f,
                                    0.0f, 0.0f, 0.0f, 1.0f);
        UniformMatrix4f uMVMatrix;
        UniformMatrix4f uPMatrix;

        Uniform3f uColor;

        FrameBuffer fb;
        Program p;

        Mesh<Vector2f, uint> triangle;

        private FirstPersonCamera camera;

        const string EXAMPLE_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition; 
        uniform mat4 uMVMatrix;
        uniform mat4 uPMatrix;
        
        void main()
        {
            gl_Position = uPMatrix * uMVMatrix * vec4(aPosition, 1.0);
        }
#endif
#ifdef _FRAGMENT_
        uniform vec3 uColor;
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(uColor, 1.0); 
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
            using (TutorialLearning03_7 example = new TutorialLearning03_7())
            {
                example.Run(60.0, 0.0);
            }
        }

        #endregion
    }
}
