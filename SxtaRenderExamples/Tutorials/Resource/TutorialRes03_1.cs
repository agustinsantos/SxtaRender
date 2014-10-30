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
    [Example("Example 3.1: Module Resource", ExampleCategory.Resource, "3. Module Resource", 1, Source = "TutorialRes03_1", Documentation = "Tutorial-TODO")]
    public class TutorialRes03_1 : GameWindow
    {
        public TutorialRes03_1()
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
            resLoader.addPath(dir + "/Resources/Shaders");
            resManager = new ResourceManager(resLoader);

            fb = new FrameBuffer(true);
            res = (ProgramResource)(resManager.loadResource("test;"));
            p = (Program)res.get();
            quad = new Mesh<Vertex_V3T2f, uint>(Vertex_V3T2f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLES, MeshUsage.GPU_STATIC, 4, 6);
            quad.addAttributeType(0, 3, AttributeType.A32F, false);
            quad.addAttributeType(1, 2, AttributeType.A32F, false);
            quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(-1, -1, 0), TexCoord = new Vector2f(0, 0) });
            quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(1, -1, 0), TexCoord = new Vector2f(1, 0) });
            quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(-1, 1, 0), TexCoord = new Vector2f(0, 1) });
            quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(1, 1, 0), TexCoord = new Vector2f(1, 1) });
            quad.addIndice(0);
            quad.addIndice(1);
            quad.addIndice(2);
            quad.addIndice(2);
            quad.addIndice(1);
            quad.addIndice(3);
            m = quad.getBuffers();


            fb.setClearColor(Color.Blue);
        }

        #endregion

        #region OnUnload

        protected override void OnUnload(EventArgs e)
        {
            resManager.releaseResource(res);
            if (resManager != null)
                resManager.Dispose();

            if (quad != null)
                quad.Dispose();
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
            // Nothing to do!
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

            fb.draw(p, quad);
            this.SwapBuffers();
        }

        #endregion

        #region Fields
        string dir = ".";
        XMLResourceLoader resLoader;
        ResourceManager resManager;
        ProgramResource res;

        FrameBuffer fb;
        Program p;
        Mesh<Vertex_V3T2f, uint> quad;
        MeshBuffers m;


        #endregion

        #region public static void Main()

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (TutorialRes03_1 example = new TutorialRes03_1())
            {
                example.Run(30.0, 10.0);
            }
        }

        #endregion

        private struct Vertex_V3T2f
        {
            public Vector3f Position;
            public Vector2f TexCoord;
            public static int SizeInBytes
            {
                get { return Vector2f.SizeInBytes + Vector3f.SizeInBytes; }
            }
        }

    }
}
