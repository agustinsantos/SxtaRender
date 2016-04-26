// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using OpenTK;
using OpenTK.Input;
using Sxta.Math;
using Sxta.Render;
using Sxta.Render.OpenGLExt;
using Sxta.Render.Resources;
using Sxta.Render.Scenegraph;
using System;
using MathHelper = Sxta.Math.MathHelper;
using Matrix4d = Sxta.Math.Matrix4d;
using Vector3d = Sxta.Math.Vector3d;

namespace Examples.Tutorials
{
    /// <summary>
    /// Demonstrates a scenegraph application.
    /// </summary>
    [Example("Example 1.X?: Scene with Light", ExampleCategory.SceneGraph, "1. Getting Started", 1, Source = "TutorialSG01_x", Documentation = "Tutorial-TODO")]
    public class TutorialSG01_X : GameWindow
    {
        public TutorialSG01_X(string wd)
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

            if (e.Key == Key.F12)
            {
                ScreenShot.SaveScreenShot(this.ClientSize, this.ClientRectangle);
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
            SceneNode root = new SceneNode();

            SceneNode cameranode = new SceneNode();
            cameranode.addFlag("camera");
            cameranode.addModule("material", (Module)resManager.loadResource("camera").get());
            cameranode.addMethod("draw", new Method((TaskFactory)resManager.loadResource("cameraMethod").get()));
            root.addChild(cameranode);

            SceneNode light = new SceneNode();
            light.setLocalToParent(Matrix4d.CreateTranslation(0, 0, 0)
                                   //* Matrix4d.CreateRotationX(MathHelper.ToRadians(180))
                                   //* Matrix4d.CreateRotationY(MathHelper.ToRadians(180))
                                   //* Matrix4d.CreateRotationZ(MathHelper.ToRadians(90))
                                   );
            light.addFlag("light");
            light.addModule("material", (Module)resManager.loadResource("spotlight").get());
            light.addMethod("draw", new Method((TaskFactory)resManager.loadResource("lightMethod").get()));
            root.addChild(light);

            SceneNode plane = new SceneNode();
            plane.setLocalToParent(Matrix4d.CreateTranslation(0, -5, 0) * Matrix4d.CreateRotationX(MathHelper.ToRadians(-90)));
            plane.addFlag("object");
            plane.addMesh("geometry", (MeshBuffers)resManager.loadResource("plane.mesh").get());
            plane.addModule("material", (Module)resManager.loadResource("texturedPlastic").get());
            plane.addMethod("draw", new Method((TaskFactory)resManager.loadResource("objectMethod").get()));
            root.addChild(plane);

            SceneNode cube = new SceneNode();
            cube.setLocalToParent(Matrix4d.CreateTranslation(0, -3, 0) * Matrix4d.CreateRotationX(MathHelper.ToRadians(45)) * Matrix4d.CreateRotationZ(MathHelper.ToRadians(45)));
            cube.addFlag("object");
            cube.addMesh("geometry", (MeshBuffers)resManager.loadResource("cube.mesh").get());
            cube.addModule("material", (Module)resManager.loadResource("texturedPlastic").get());
            cube.addMethod("draw", new Method((TaskFactory)resManager.loadResource("objectMethod").get()));
            root.addChild(cube);

            //SceneNode log = new SceneNode();
            //log.addFlag("overlay");
            //log.addMethod("draw", new Method(resManager.loadResource("logMethod").get() as TaskFactory));
            //root.addChild(log);

            //SceneNode info = new SceneNode();
            //info.addFlag("overlay");
            //info.addMethod("draw", new Method(resManager.loadResource("infoMethod").get() as TaskFactory));
            //root.addChild(info);

            manager.setRoot(root);
            manager.setCameraNode("camera");
            manager.setCameraMethod("draw");

            fb = FrameBuffer.getDefault();

            // position the camera 
            camera = new SGCamera(this);
            camera.Position = new Vector3d(0, 0, 15);

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

            camera.Resize(Width, Height);
            manager.setCameraToScreen(camera.ProjectionMatrix);
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
            camera.Update((float)e.Time);
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
            manager.getCameraNode().setLocalToParent(camera.ViewMatrix);
            manager.update(e.Time / 100000); // from Seconds to microseconds);
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

        private SGCamera camera;

        #endregion

        #region public static void Main()

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (TutorialSG01_X example = new TutorialSG01_X("Resources"))
            {
                example.Run(30.0, 0.0);
            }
        }

        #endregion
    }
}
