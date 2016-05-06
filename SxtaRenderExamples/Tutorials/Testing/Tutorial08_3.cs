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

namespace Examples.Tutorials
{
    /// <summary>
    /// Drawing a plane using Scenegraphs
    /// </summary>
    [Example("Example 8.03: Quadtree Subdivision using TerrainNode", ExampleCategory.Testing, "8. Proland", 1, Source = "Tutorial08_3", Documentation = "Tutorial-TODO")]
    public class Tutorial08_3 : GameWindow
    {
        public Tutorial08_3(string wd) : base(600, 600)
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
            resLoader.addArchive(dir + "/Terrain/HelloWord02.xml");
            resManager = new ResourceManager(resLoader);
            manager = new SceneManager();
            manager.setResourceManager(resManager);
            manager.setScheduler(new MultithreadScheduler());

            manager.setRoot(resManager.loadResource("scene").get() as SceneNode);
            manager.setCameraNode("camera");
            manager.setCameraMethod("draw");

            fb = FrameBuffer.getDefault();
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
            fb.setDepthTest(true, Function.LESS);

            float fov = 60;

            double vfov = 2 * Math.Atan((float)Height / (float)Width * Math.Tan(MathHelper.ToRadians(fov / 2)));
            Matrix4d projection = Matrix4d.CreatePerspectiveFieldOfView(vfov, (float)Width / (float)Height, 0.01f, 1000.0f);
            manager.setCameraToScreen(projection);
        }


        /// <summary>
        /// Add your game logic here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            manager.update(e.Time / 100000); // from Seconds to microseconds;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            fb.clear(true, false, true);
            manager.draw();
            this.SwapBuffers();
        }

        string dir = ".";
        XMLResourceLoader resLoader;
        ResourceManager resManager;
        SceneManager manager;
        FrameBuffer fb;

        [STAThread]
        public static void Main()
        {
            using (Tutorial08_3 example = new Tutorial08_3("Resources"))
            {
                example.Run(30.0, 10.0);
            }
        }
    }
}


