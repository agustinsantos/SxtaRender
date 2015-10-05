// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Sxta.Math;
using Sxta.Render;
using System;
using System.Drawing;

namespace Examples.Tutorials
{
    /// <summary>
    /// Demonstrates how to combine several operations by matrix multiplications.
    /// </summary>
    [Example("Example 3.4: Concatenating", ExampleCategory.Learning, "3. Math: Matrix operations", 1, Source = "Tutorial03_4", Documentation = "Tutorial03_4")]
    public class TutorialLearning03_4 : GameWindow
    {
        public TutorialLearning03_4()
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
            uMatrix = p.getUniformMatrix4f("uMatrix");
            uColor = p.getUniform3f("uColor");


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

            // Translate then scale (red)
            uColor.set(new Vector3f(1.0f, 0.0f, 0.0f));
            mat = Matrix4f.CreateTranslation(-0.6f, 0.8f, 0.0f);
            uMatrix.set(mat);
            
            // Render our triangle at the initial position
            fb.draw(p, triangle);
            // Render our triangle after a translation and a scale operation
            mat = Matrix4f.Translate(mat, 1.0f, 0.0f, 0.0f);
            mat = Matrix4f.Scale(mat, 0.5f);
            //mat = Matrix4f.Scale(0.5f) * Matrix4f.CreateTranslation(1.0f, 0.0f, 0.0f) * mat;
            uMatrix.set(mat);
            fb.draw(p, triangle);

            // Scale then translate (green)
            uColor.set(new Vector3f(0.0f, 1.0f, 0.0f));
            mat = Matrix4f.CreateTranslation(-0.6f, 0.4f, 0.0f);
            uMatrix.set(mat);
            // Render our triangle at the initial position
            fb.draw(p, triangle);
            mat =  Matrix4f.CreateTranslation(1.0f, 0.0f, 0.0f) * Matrix4f.Scale(0.5f) * mat;
            uMatrix.set(mat);
            // Render our triangle after a scale and translation operation
            fb.draw(p, triangle);

            // Scale then translate then rotate (blue)
            uColor.set(new Vector3f(0.0f, 0.0f, 1.0f));
            mat = Matrix4f.CreateTranslation(-0.6f, 0.0f, 0.0f);
            uMatrix.set(mat);
            // Render our triangle at the initial position
            fb.draw(p, triangle);
            mat = Matrix4f.CreateRotationZ(-75) * Matrix4f.CreateTranslation(1.0f, 0.0f, 0.0f) * Matrix4f.Scale(0.5f) * mat;
            uMatrix.set(mat);
            // Render our triangle after a scale and translation and rotate operation
            fb.draw(p, triangle);

            // Scale then rotate then translate (light blue)
            uColor.set(new Vector3f(0.0f, 1.0f, 1.0f));
            mat = Matrix4f.CreateTranslation(-0.6f, -0.4f, 0.0f);
            uMatrix.set(mat);
            // Render our triangle at the initial position
            fb.draw(p, triangle);
            mat =  Matrix4f.CreateTranslation(1.0f, 0.0f, 0.0f) * Matrix4f.CreateRotationZ(-75) * Matrix4f.Scale(0.5f) * mat;
            uMatrix.set(mat);
            // Render our triangle after a scale and and rotate translation operation
            fb.draw(p, triangle);

            // Rotate then translate then scale (yellow)
            uColor.set(new Vector3f(1.0f, 1.0f, 0.0f));
            mat = Matrix4f.CreateTranslation(-0.6f, -0.8f, 0.0f);
            uMatrix.set(mat);
            // Render our triangle at the initial position
            fb.draw(p, triangle);
            mat = Matrix4f.Scale(0.5f) * Matrix4f.CreateTranslation(1.0f, 0.0f, 0.0f) * Matrix4f.CreateRotationZ(-75) * mat;
            uMatrix.set(mat);
            // Render our triangle after a rotate, translation, scale operation
            fb.draw(p, triangle);

            this.SwapBuffers();
        }

        #endregion

        #region Fields
        Matrix4f mat = new Matrix4f(1.0f, 0.0f, 0.0f, 0.0f,
                                    0.0f, 1.0f, 0.0f, 0.0f,
                                    0.0f, 0.0f, 1.0f, 0.0f,
                                    0.0f, 0.0f, 0.0f, 1.0f);
        UniformMatrix4f uMatrix;
        Uniform3f uColor;

        FrameBuffer fb;
        Program p;

        Mesh<Vector2f, uint> triangle;

        const string EXAMPLE_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition; 
        uniform mat4 uMatrix;   
   
        void main()
        {
            gl_Position = uMatrix * vec4(aPosition, 1.0);
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
            using (TutorialLearning03_4 example = new TutorialLearning03_4())
            {
                example.Run(60.0, 0.0);
            }
        }

        #endregion
    }
}
