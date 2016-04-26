// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using OpenTK;
using OpenTK.Input;
using Sxta.Math;
using Sxta.Render;
using Sxta.Render.Resources;
using Sxta.Render.Scenegraph;
using System;

namespace Examples.Tutorials
{
    /// <summary>
    /// Demonstrates a scenegraph application.
    /// </summary>
    [Example("Example 1.1: Basic SceneGraph", ExampleCategory.SceneGraph, "1. Getting Started", 1, Source = "TutorialSG01_1", Documentation = "Tutorial-TODO")]
    public class TutorialSG01_1 : GameWindow
    {
        public TutorialSG01_1(string wd)
            : base(600, 600)
        {
            if (!string.IsNullOrWhiteSpace(wd))
                dir = wd;
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
            resLoader.addPath(dir + "/Textures");
            resLoader.addPath(dir + "/Shaders");
            resLoader.addPath(dir + "/Meshes");
            resLoader.addPath(dir + "/Methods");
            resLoader.addPath(dir + "/Scenes");
            resManager = new ResourceManager(resLoader);
            manager = new SceneManager();
            manager.setResourceManager(resManager);
            manager.setScheduler(new MultithreadScheduler());

            manager.setRoot(resManager.loadResource("Example01Scene").get() as SceneNode);
            manager.setCameraNode("camera");
            manager.setCameraMethod("draw");

            fb = FrameBuffer.getDefault();
        }

        #endregion

        #region OnUnload

        protected override void OnUnload(EventArgs e)
        {
            if (manager != null)
                manager.Dispose();
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
            fb.setDepthTest(true, Function.LESS);
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
            manager.update(e.Time / 100000); // from Seconds to microseconds;
            manager.draw();
            this.SwapBuffers();
        }

        #endregion

        #region Fields

        string dir = ".";
        XMLResourceLoader resLoader;
        ResourceManager resManager;
        SceneManager manager;
        FrameBuffer fb;

        #endregion

        #region public static void Main()

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (TutorialSG01_1 example = new TutorialSG01_1("Resources"))
            {
                example.Run(30.0, 0.0);
            }
        }

        #endregion
    }
}
