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
using Sxta.Render.OpenGLExt;
using System.Drawing;

namespace Examples.Tutorials
{
    /// <summary>
    /// Drawing a plane using Proland Terrain
    /// </summary>
    [Example("Example 2.01: Noise Generation", ExampleCategory.Proland, "02. Proland Terrain", 1, Source = "Tutorial02_01", Documentation = "Tutorial02_01")]
    public class TutorialProland02_1 : GameWindow
    {
        public TutorialProland02_1(string wd) : base(600, 600)
        {
            if (!string.IsNullOrWhiteSpace(wd))
                dir = wd;
            Keyboard.KeyDown += Keyboard_KeyDown;
        }


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

            if (e.Key == Key.F12)
            {
                ScreenShot.SaveScreenShot(this.ClientSize, this.ClientRectangle, "Screenshot" + this.GetType().Name + ".bmp");
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            RegisterResourceReader.RegisterResources();
            resLoader = new XMLResourceLoader();
            resLoader.addPath(dir);
            resLoader.addArchive(dir + "/HelloWorld.xml");
            resManager = new ResourceManager(resLoader);
            manager = new SceneManager();
            manager.setResourceManager(resManager);
            manager.setScheduler(resManager.loadResource("defaultScheduler").get() as Scheduler);

            manager.setRoot(resManager.loadResource("scene").get() as SceneNode);
            manager.setCameraNode("camera");
            manager.setCameraMethod("draw");

            camera = new SGCamera(this);
            camera.Position = new Sxta.Math.Vector3d(0, 0, 2500);
            camera.MoveSpeed = 50f;

            fb = FrameBuffer.getDefault();
            fb.setClearColor(Color.Black);
            fb.setDepthTest(true, Function.LESS);
        }

        protected override void OnUnload(EventArgs e)
        {
            if (manager != null)
                manager.Dispose();
            if (fb != null)
                fb.Dispose();
            base.OnUnload(e);
        }

        /// <summary>
        /// Respond to resize events here.
        /// </summary>
        /// <param name="e">Contains information on the new GameWindow size.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnResize(EventArgs e)
        {
            fb.setViewport(new Vector4i(0, 0, Width, Height));

            camera.Resize(Width, Height);
            manager.setCameraToScreen(camera.ProjectionMatrix);
        }


        /// <summary>
        /// Add your game logic here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            camera.Update((float)e.Time);
            manager.getCameraNode().setLocalToParent(camera.ViewMatrix);

            fb.clear(true, false, true);
            manager.update(e.Time / 100000); // from Seconds to microseconds);
            manager.draw();
            this.SwapBuffers();
        }

        string dir = ".";
        XMLResourceLoader resLoader;
        ResourceManager resManager;
        SceneManager manager;
        FrameBuffer fb;
        private SGCamera camera;

        [STAThread]
        public static void Main()
        {
            using (TutorialProland02_1 example = new TutorialProland02_1("Resources/Proland/Terrain/Example01"))
            {
                example.Run(60.0);
            }
        }
    }
}


