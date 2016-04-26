// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using OpenTK;
using OpenTK.Input;
using Sxta.Math;
using Sxta.Render;
using Sxta.Render.Resources;
using Sxta.Render.Scenegraph;
using System;
using MathHelper = Sxta.Math.MathHelper;
using Matrix4d = Sxta.Math.Matrix4d;

namespace Examples.Tutorials
{
    /// <summary>
    /// Demonstrates a scenegraph application.
    /// </summary>
    [Example("Example 1.4: SceneGraph using Code", ExampleCategory.SceneGraph, "1. Getting Started", 1, Source = "TutorialSG01_4", Documentation = "Tutorial-TODO")]
    public class TutorialSG01_4 : GameWindow
    {
        public TutorialSG01_4(string wd)
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
            SceneNode root = new SceneNode();

            SceneNode camera = new SceneNode();
            camera.addFlag("camera");
            camera.addModule("material", (Module)resManager.loadResource("camera").get());
            camera.addMethod("draw", new Method((TaskFactory)resManager.loadResource("Example02Camera").get()));
            root.addChild(camera);

            SceneNode plane = new SceneNode();
            plane.setLocalToParent(Matrix4d.CreateTranslation(0, -10, -20) * Matrix4d.CreateRotationX(MathHelper.ToRadians(90)));
            plane.addFlag("object");
            plane.addMesh("geometry", (MeshBuffers)resManager.loadResource("plane.mesh").get());
            plane.addModule("material", (Module)resManager.loadResource("perspective").get());
            plane.addMethod("draw", new Method((TaskFactory)resManager.loadResource("mesh02Method").get()));
            root.addChild(plane);

            SceneNode cube1 = new SceneNode();
            cube1.setLocalToParent(Matrix4d.CreateTranslation(0, 0, -10) * Matrix4d.CreateRotationX(MathHelper.ToRadians(45)) * Matrix4d.CreateRotationY(MathHelper.ToRadians(45)));
            cube1.addFlag("object");
            cube1.addMesh("geometry", (MeshBuffers)resManager.loadResource("cube.mesh").get());
            cube1.addModule("material", (Module)resManager.loadResource("perspective").get());
            cube1.addMethod("draw", new Method((TaskFactory)resManager.loadResource("mesh02Method").get()));
            root.addChild(cube1);

            SceneNode cube2 = new SceneNode();
            cube2.setLocalToParent(Matrix4d.CreateTranslation(-2, 2, -5) * Matrix4d.CreateRotationX(MathHelper.ToRadians(45)) * Matrix4d.CreateRotationY(MathHelper.ToRadians(45)) * Matrix4d.Scale(0.5, 0.5, 0.5));
            cube2.addFlag("object");
            cube2.addMesh("geometry", (MeshBuffers)resManager.loadResource("cube.mesh").get());
            cube2.addModule("material", (Module)resManager.loadResource("perspective").get());
            cube2.addMethod("draw", new Method((TaskFactory)resManager.loadResource("mesh02Method").get()));
            root.addChild(cube2);

            SceneNode cube3 = new SceneNode();
            cube3.setLocalToParent(Matrix4d.CreateTranslation(2, -3, -8) * Matrix4d.CreateRotationX(MathHelper.ToRadians(45)) * Matrix4d.CreateRotationY(MathHelper.ToRadians(45)) * Matrix4d.Scale(0.3));
            cube3.addFlag("object");
            cube3.addMesh("geometry", (MeshBuffers)resManager.loadResource("cube.mesh").get());
            cube3.addModule("material", (Module)resManager.loadResource("perspective").get());
            cube3.addMethod("draw", new Method((TaskFactory)resManager.loadResource("mesh02Method").get()));
            root.addChild(cube3);

            manager.setRoot(root);
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
            FrameBuffer fb = FrameBuffer.getDefault();
            fb.setViewport(new Vector4i(0, 0, Width, Height));

            double vfov = 2 * Math.Atan((float)Height / (float)Width * Math.Tan(MathHelper.ToRadians(fov / 2)));
            Matrix4d projection = Matrix4d.CreatePerspectiveFieldOfView(vfov, (float)Width / (float)Height, 0.01f, 100.0f);
            manager.setCameraToScreen(projection);
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
        float fov = 60;

        #endregion

        #region public static void Main()

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (TutorialSG01_4 example = new TutorialSG01_4("Resources"))
            {
                example.Run(30.0, 0.0);
            }
        }

        #endregion
    }
}
