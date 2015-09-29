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
    /// Demonstrates how add new vertex attributes.
    /// </summary>
    [Example("Learning 2.02: Vertex Attributes", ExampleCategory.Learning, "2. Shaders", 1, Source = "Tutorial02_2", Documentation = "Tutorial02_2")]
    public class TutorialLearning02_2 : GameWindow
    {
        public TutorialLearning02_2()
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

            triangle = new Mesh<Vector4f, uint>(Vector4f.SizeInBytes, MeshMode.TRIANGLES, MeshUsage.GPU_STATIC, 3);
            triangle.addAttributeType(0, 2, AttributeType.A32F, false);
            triangle.addAttributeType(1, 2, AttributeType.A32F, false);

            // Load up a triangle
            triangle.addVertex(new Vector4f(0.0f,  0.5f, 0.4f, 0.2f));
            triangle.addVertex(new Vector4f(0.5f, -0.5f, 0.4f, 0.2f));
            triangle.addVertex(new Vector4f(-0.5f, -0.5f, 0.4f, 0.2f));
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
        protected Mesh<Vector4f, uint> triangle;

        private const string TUTORIAL_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec2 aPosition;
        layout (location = 1) in vec2 aOffset;
        
        void main()
        {
            gl_Position = vec4(aPosition.x + aOffset.x, aPosition.y + aOffset.y, 0.0, 1.0);
        }
#endif

#ifdef _FRAGMENT_
        out vec4 FragColor;

        void main()
        {
            FragColor = vec4 (183.0/255.0, 207.0/255.0, 228.0/255.0, 1.0f);
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
            using (TutorialLearning02_2 example = new TutorialLearning02_2())
            {
                // Enters the game loop of the GameWindow using the maximum update rate.
                example.Run();
            }
        }

        #endregion
    }
}
