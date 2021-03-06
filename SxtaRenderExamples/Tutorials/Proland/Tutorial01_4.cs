﻿using OpenTK;
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
using proland;
using Sxta.Render.Scenegraph.Controller;

namespace Examples.Tutorials
{
    /// <summary>
    /// Drawing a plane using Scenegraphs
    /// </summary>
    [Example("Example 1.04: Planet Subdivision using spherical TerrainNode", ExampleCategory.Proland, "01. Proland Core", 1, Source = "Tutorial01_3", Documentation = "Tutorial01_3")]
    public class TutorialProland01_4 : GameWindow
    {
        public TerrainViewController controller;

        public TutorialProland01_4(string wd) : base(600, 600)
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
            resLoader.addPath(dir + "/Proland/Core/Example3");
            resLoader.addArchive(dir + "/Proland/Core/Example3/HelloWordPlanet2.xml");
            resManager = new ResourceManager(resLoader);
            manager = new SceneManager();
            manager.setResourceManager(resManager);
            manager.setScheduler(new MultithreadScheduler());

            manager.setRoot(resManager.loadResource("scene").get() as SceneNode);
            manager.setCameraNode("camera");
            manager.setCameraMethod("draw");

            view = resManager.loadResource("viewHandler").get() as BasicViewHandler;
            view.GameWindow = this;
            ViewManager viewManager = new ViewManager() { SceneManager = manager, ViewController = new TerrainViewController(manager.getCameraNode(), 11000000) };
            view.ViewManager = viewManager;
            //camera = new SGCamera(this);
            //camera.Position = new Sxta.Math.Vector3d(0, 0, 19000000);
            //camera.MoveSpeed = 50000f;

            fb = FrameBuffer.getDefault();
            fb.setClearColor(System.Drawing.Color.Black);
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

            //camera.Resize(Width, Height, 100, 20000000);
            //manager.setCameraToScreen(camera.ProjectionMatrix);
        }


        /// <summary>
        /// Add your game logic here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            view.OnUpdateFrame(e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            view.OnRenderFrame(e.Time, e.Time);
            this.SwapBuffers();

            /**camera.Update((float)e.Time);
            manager.getCameraNode().setLocalToParent(camera.ViewMatrix);

            fb.clear(true, false, true);
            manager.update(e.Time / 100000); // from Seconds to microseconds);
            manager.draw();
            this.SwapBuffers();*/
        }

        string dir = ".";
        XMLResourceLoader resLoader;
        ResourceManager resManager;
        SceneManager manager;
        FrameBuffer fb;
        BasicViewHandler view;
        //private SGCamera camera;

        [STAThread]
        public static void Main()
        {
            using (TutorialProland01_4 example = new TutorialProland01_4("Resources"))
            {
                example.Run(60.0);
            }
        }
    }
}


