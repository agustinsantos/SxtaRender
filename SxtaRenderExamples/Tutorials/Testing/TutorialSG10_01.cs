// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using Microsoft.CSharp;
using OpenTK;
using OpenTK.Input;
using proland;
using Sxta.Math;
using Sxta.Render;
using Sxta.Render.Resources;
using Sxta.Render.Scenegraph;
using Sxta.Render.Scenegraph.Controller;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MathHelper = Sxta.Math.MathHelper;
using Matrix4d = Sxta.Math.Matrix4d;
using Vector3d = Sxta.Math.Vector3d;

namespace Examples.Tutorials
{
    /// <summary>
    /// Demonstrates a scenegraph application.
    /// </summary>
    [Example("Example 10.1: KeyMouse Controller", ExampleCategory.Testing, "10. Testing ViewController", 1, Source = "TutorialSG10_01", Documentation = "Tutorial-TODO")]
    public class TutorialSG10_01 : GameWindow
    {
        public TutorialSG10_01(string wd)
            : base(600, 600)
        {
            if (!string.IsNullOrWhiteSpace(wd))
                dir = wd;

            Keyboard.KeyDown += OnKeyDown;
            Keyboard.KeyUp += OnKeyUp;
            MouseWheel += OnMouseWheel;
            MouseDown += OnMouseDownEvent;
        }

        private void OnMouseDownEvent(object sender, MouseButtonEventArgs e)
        {
            if (view.OnMouseDown(e))
                return;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (view.OnMouseWheel(e))
                return;
        }

        #region Keyboard_KeyDown

        /// <summary>
        /// Occurs when a key is pressed.
        /// </summary>
        /// <param name="sender">The KeyboardDevice which generated this event.</param>
        /// <param name="e">The key that was pressed.</param>
        void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (view.OnKeyDown(e))
                return;

            if (e.Key == Key.Escape)
                this.Exit();

            if (e.Key == Key.F11)
                if (this.WindowState == WindowState.Fullscreen)
                    this.WindowState = WindowState.Normal;
                else
                    this.WindowState = WindowState.Fullscreen;
        }

        void OnKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            if (view.OnKeyRelease(e))
                return;
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
            resLoader.addArchive(dir + "/Archives/TutorialSG10_1.xml");
            resManager = new ResourceManager(resLoader);
            manager = new SceneManager();
            manager.setResourceManager(resManager);
            manager.setScheduler(new MultithreadScheduler());

            manager.setRoot(resManager.loadResource("sceneTutorial").get() as SceneNode);
            manager.setCameraNode("camera");
            manager.setCameraMethod("draw");

            fb = FrameBuffer.getDefault();

            // position the camera 
            //camera = new SGCamera(this);
            //camera.Position = new Vector3d(0, 0, 10);

            view = resManager.loadResource("viewHandler").get() as BasicViewHandler;
            view.GameWindow = this;
            ViewManager viewManager = new ViewManager() { SceneManager = manager, ViewController = new TerrainViewController(manager.getCameraNode(), 30.0) };
            view.ViewManager = viewManager;
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

            //camera.Resize(Width, Height);
            //manager.setCameraToScreen(camera.ProjectionMatrix);
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
            //camera.Update((float)e.Time);
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
            //manager.getCameraNode().setLocalToParent(camera.ViewMatrix);

            //manager.update(e.Time / 100000); // from Seconds to microseconds);
            //manager.draw();
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

        //private SGCamera camera;
        private BasicViewHandler view;

        #endregion

        #region public static void Main()

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (TutorialSG10_01 example = new TutorialSG10_01("Resources"))
            {
                example.Run(30.0, 0.0);
            }
        }

        #endregion

#if DELETEME
        public static void Do()
        {
            Debugger.Break();
        }

        private const string classTemplate = @"
            using System;
            using System.Linq.Expressions;
            using OpenTK;
            using OpenTK.Input;

            public static class RulesConfiguration
            {{

                private static Expression<Func<Key, bool>>[] rules = new Expression<Func<Key, bool>>[]
                {{
                    {0}
                }};

                public static Expression<Func<Key, bool>>[] Rules {{ get {{ return rules; }} }}
            }}
        ";
        Expression<Func<Key, bool>> lambda1 = key => key == Key.M;
        Expression<Func<Key, bool>> lambda2 = key => { if (key == Key.M) return true; else return false; };

        static Func<Key, bool>[] compiledRules = null;
        static Expression<Func<Key, bool>>[] rules = null;

        static void CheckEvents(Key key)
        {
            if (compiledRules == null)
            {
                var filePath = @"Resources/keyrules.txt";
                var fileContents = File.ReadAllLines(filePath);

                // add commas to the expressions so they can compile as part of the array
                var joined = String.Join("," + Environment.NewLine, fileContents);

                Console.WriteLine("Rules found in file: \n{0}", joined);

                var classSource = String.Format(classTemplate, joined);

                var assembly = CompileAssembly(classSource);

                rules = GetExpressionsFromAssembly(assembly);
                compiledRules = new Func<Key, bool>[rules.Length];
                for (int i = 0; i < rules.Length; i++)
                {
                    var compiledToFunc = rules[i].Compile();
                    compiledRules[i] = compiledToFunc;
                }
            }
            for (int i = 0; i < rules.Length; i++)
            {
                var compiledToFunc = compiledRules[i];
                var rule = rules[i];
                Console.WriteLine("Checking rule {0} against input {1}: {2}", rule, key, compiledToFunc(key));
            }
        }

        static Expression<Func<Key, bool>>[] GetExpressionsFromAssembly(Assembly assembly)
        {
            var type = assembly.GetTypes().Single();
            var property = type.GetProperties().Single();
            var propertyValue = property.GetValue(null, null);
            return propertyValue as Expression<Func<Key, bool>>[];
        }

        static Assembly CompileAssembly(string source)
        {
            var compilerParameters = new CompilerParameters()
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                ReferencedAssemblies =
            {
                "OpenTK.dll",
                "System.Core.dll" // needed for linq + expressions to compile
            },
            };
            var compileProvider = new CSharpCodeProvider();
            var results = compileProvider.CompileAssemblyFromSource(compilerParameters, source);
            if (results.Errors.HasErrors)
            {
                Console.Error.WriteLine("{0} errors during compilation of rules", results.Errors.Count);
                foreach (CompilerError error in results.Errors)
                {
                    Console.Error.WriteLine(error.ErrorText);
                }
                throw new InvalidOperationException("Broken rules configuration, please fix");
            }
            var assembly = results.CompiledAssembly;
            return assembly;
        }
#endif
    }
}
