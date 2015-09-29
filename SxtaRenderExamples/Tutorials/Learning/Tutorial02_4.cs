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
    /// Demonstrates how add information to attributes.
    /// </summary>
    [Example("Learning 2.04: Add data to attributes", ExampleCategory.Learning, "2. Shaders", 1, Source = "Tutorial02_4", Documentation = "Tutorial02_4")]
    public class TutorialLearning02_4 : GameWindow
    {
        public TutorialLearning02_4()
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
            fb.setClearColor(Color.White);

            p = new Program(new Module(330, TUTORIAL_SHADER));

            triangle = new Mesh<Vertex_V2C3f, uint>(Vertex_V2C3f.SizeInBytes, MeshMode.TRIANGLES, MeshUsage.GPU_STATIC, 3);
            triangle.addAttributeType(0, 2, AttributeType.A32F, false);
            triangle.addAttributeType(1, 3, AttributeType.A32F, false);

            // Load up a triangle
            triangle.addVertex(new Vertex_V2C3f() { Position = new Vector2f(0.0f, 0.5f), Color = new Vector3f(1.0f, 0.0f, 0.0f) });
            triangle.addVertex(new Vertex_V2C3f() { Position = new Vector2f(0.5f, -0.5f), Color = new Vector3f(0.0f, 1.0f, 0.0f) });
            triangle.addVertex(new Vertex_V2C3f() { Position = new Vector2f(-0.5f, -0.5f), Color = new Vector3f(0.0f, 0.0f, 1.0f) });
        }

        #endregion

        #region OnUnload

        /// <summary>
        /// Dispose any OpenGL resources.
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnUnload(EventArgs e)
        {
            if (fb != null)
                fb.Dispose();
            if (p != null)
                p.Dispose();
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

            // Render our triangle
            fb.draw(p, triangle);

            this.SwapBuffers();
        }

        #endregion

        #region Fields

        protected FrameBuffer fb;

        protected Program p;
        protected Mesh<Vertex_V2C3f, uint> triangle;

        private const string TUTORIAL_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec2 aPosition;
        layout (location = 1) in vec3 aColor;
        
        out vec3 vs_color;
        
        void main()
        {
            gl_Position = vec4(aPosition, 0.0, 1.0);

            // Output a value for vs_color
            vs_color = aColor;
        }
#endif

#ifdef _FRAGMENT_
        // Input from the vertex shader
        in vec3 vs_color;
        
        out vec4 FragColor;

        void main()
        {
            FragColor =  vec4(vs_color, 1.0);
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
            using (TutorialLearning02_4 example = new TutorialLearning02_4())
            {
                // Enters the game loop of the GameWindow using the maximum update rate.
                example.Run();
            }
        }

        #endregion

        protected struct Vertex_V2C3f
        {
            public Vector2f Position;
            public Vector3f Color;
            public static int SizeInBytes
            {
                get { return Vector2f.SizeInBytes + Vector3f.SizeInBytes; }
            }
        }
    }
}
