using OpenTK;
using OpenTK.Input;
using Sxta.Math;
using Sxta.Proland.Core.XmlResources;
using Sxta.Render;
using Sxta.Render.Resources;
using Sxta.Render.Scenegraph;
using System;
using System.Diagnostics;
using Vector3d = Sxta.Math.Vector3d;

namespace Examples.Tutorials
{
    [Example("Example 8.05: TerrainSampler for elevation implementation", ExampleCategory.Testing, "08. Proland", 1, Source = "Tutorial08_5", Documentation = "Tutorial-TODO")]
    class Tutorial08_5 : GameWindow
    {
        public Tutorial08_5(string wd) : base(600, 600)
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
        }

        protected override void OnLoad(EventArgs e)
        {
            RegisterResourceReader.RegisterResources();
            resLoader = new XMLResourceLoader();
            resLoader.addPath(dir + "/Textures");
            resLoader.addPath(dir + "/Shaders");
            resLoader.addPath(dir + "/Meshes");
            resLoader.addPath(dir + "/Methods");
            resLoader.addPath(dir + "/Scenes");
            resLoader.addArchive(dir + "/Terrain/HelloWord04.xml");
            resManager = new ResourceManager(resLoader);
            manager = new SceneManager();
            manager.setResourceManager(resManager);
            manager.setScheduler(new MultithreadScheduler());

            manager.setRoot(resManager.loadResource("scene").get() as SceneNode);
            manager.setCameraNode("camera");
            manager.setCameraMethod("draw");

            fb = FrameBuffer.getDefault();

            camera = new SGCamera(this);
            camera.Position = new Vector3d(0, 0, 10);
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
            FrameBuffer fb = FrameBuffer.getDefault();
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
            camera.Update((float)e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            manager.getCameraNode().setLocalToParent(camera.ViewMatrix);

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
            using (Tutorial08_5 example = new Tutorial08_5("Resources"))
            {
                example.Run(30.0, 0.0);
            }
        }
    }
}
