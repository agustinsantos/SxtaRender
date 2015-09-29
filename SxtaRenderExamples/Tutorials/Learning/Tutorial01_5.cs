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
    /// Demonstrates how to build a framebuffer.
    /// </summary>
    [Example("Learning 1.05: First triangle", ExampleCategory.Learning, "1. Getting Started", 1, Source = "Tutorial01_5", Documentation = "Tutorial01_5")]
    public class TutorialLearning01_5 : GameWindow
    {
        public TutorialLearning01_5()
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
            fb.setClearColor(Color.Aquamarine);

            p = new Program(new Module(330, TUTORIAL_SHADER));

            triangle = new Mesh<Vector2f, uint>(Vector2f.SizeInBytes, MeshMode.TRIANGLES, MeshUsage.GPU_STATIC, 3);
            triangle.addAttributeType(0, 2, AttributeType.A32F, false);

            // Load up a triangle
            triangle.addVertex(new Vector2f(-0.5f, 0.0f));
            triangle.addVertex(new Vector2f(0.5f, 0.0f));
            triangle.addVertex(new Vector2f(0.0f, 0.5f));
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
        protected Mesh<Vector2f, uint> triangle;

        private const string TUTORIAL_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition;
        
        void main()
        {
            gl_Position = vec4(aPosition, 1.0);
        }
#endif

#ifdef _FRAGMENT_
        out vec4 FragColor;

        void main()
        {
            FragColor = vec4 (1.0f, 0.0f, 0.0f, 1.0f);
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
            using (TutorialLearning01_5 example = new TutorialLearning01_5())
            {
                // Enters the game loop of the GameWindow using the maximum update rate.
                example.Run();
            }
        }

        #endregion
    }
}
