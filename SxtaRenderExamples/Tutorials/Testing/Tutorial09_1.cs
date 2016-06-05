// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using OpenTK;
using OpenTK.Input;
using Sxta.Math;
using Sxta.Proland.Core.XmlResources;
using Sxta.Render;
using Sxta.Render.Resources;
using Sxta.Render.Scenegraph;
using System;
using Matrix4d = Sxta.Math.Matrix4d;
using MathHelper = Sxta.Math.MathHelper;
using Vector3d = Sxta.Math.Vector3d;
using Sxta.Proland.Atmo;
using Sxta.Render.Scenegraph.Controller;
using proland;
using log4net;
using System.Reflection;
using Sxta.Render.OpenGLExt;

namespace Examples.Tutorials
{
    /// <summary>
    /// Demonstrates the GameWindow class.
    /// </summary>
    [Example("Example 9.1: Atmo", ExampleCategory.Testing, "09. Atmo", 1, Source = "Tutorial09_1", Documentation = "Tutorial-TODO")]
    public class Tutorial09_1 : GameWindow
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Tutorial09_1(string wd)
            : base(1024, 768)
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

            if (e.Key == Key.F10)
            {
                ShowLogTask showLogTask = resManager.loadResource("logMethod").get() as ShowLogTask;
                if (showLogTask != null)
                    showLogTask.Enabled = !showLogTask.Enabled;
            }
            else
            if (e.Key == Key.F11)
            {
                if (this.WindowState == WindowState.Fullscreen)
                    this.WindowState = WindowState.Normal;
                else
                    this.WindowState = WindowState.Fullscreen;
            }
            else
            if (e.Key == Key.F12)
            {
                string filename = ScreenShot.SaveScreenShot(this.ClientSize, this.ClientRectangle);
                log.Debug("Saved screenshot with name " + filename);
            }
        }

        #endregion

        #region OnLoad

        /// <summary>
        /// Setup OpenGL and load resources here.
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            PreprocessAtmo.PreprocessAtmosphereParameters(new AtmoParameters(), dir + "/Atmo");

            RegisterResourceReader.RegisterResources();
            resLoader = new XMLResourceLoader();
            resLoader.addPath(dir + "/Atmo");
            resLoader.addArchive(dir + "/Atmo/HelloWorld01.xml");
            resManager = new ResourceManager(resLoader);
            manager = new SceneManager();
            manager.setResourceManager(resManager);
            manager.setScheduler(new MultithreadScheduler());

            manager.setRoot(resManager.loadResource("scene").get() as SceneNode);
            manager.setCameraNode("camera");
            manager.setCameraMethod("draw");

            view = resManager.loadResource("viewHandler").get() as BasicViewHandler;
            view.GameWindow = this;
            ViewManager viewManager = new ViewManager() { SceneManager = manager, ViewController = new TerrainViewController(manager.getCameraNode(), 50000.0) };
            view.ViewManager = viewManager;
            view.ViewManager.ViewController.Theta = System.Math.PI / 2;
            view.ViewManager.ViewController.Phi = System.Math.PI / 2;

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
            FrameBuffer fb = FrameBuffer.getDefault();
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
            view.OnUpdateFrame(e.Time);
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
            view.OnRenderFrame(e.Time, e.Time);
            this.SwapBuffers();
        }

        #endregion

        #region Fields
        private string dir = ".";
        private XMLResourceLoader resLoader;
        private ResourceManager resManager;
        private SceneManager manager;
        private FrameBuffer fb;
        private BasicViewHandler view;
        #endregion

        #region public static void Main()

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (Tutorial09_1 example = new Tutorial09_1("Resources"))
            {
                example.Run(60.0, 0.0);
            }
        }

        #endregion
    }
}
