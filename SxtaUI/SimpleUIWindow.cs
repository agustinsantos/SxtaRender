using Gwen.Control;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Sxta.Render.OpenGLExt;
using Sxta.Render.Resources;
using Sxta.UI.XmlResources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;


namespace Sxta.UI.Sample
{
    /// <summary>
    /// Demonstrates the GameWindow class.
    /// </summary>
    public class SimpleUIWindow : GameWindow
    {
        private Gwen.Input.OpenTK input;

        const int fps_frames = 50;
        private readonly List<long> ftime;
        private readonly Stopwatch stopwatch;
        private long lastTime;
        private bool altDown = false;
        ResourceManager resManager;
        GuiManager guiManager;

        private StatusBar m_StatusBar;
        public double Fps; // set this in your rendering loop
        public String Note; // additional text to display in status bar

        private static ListBox m_TextOutput;

        static SimpleUIWindow()
        {
            RegisterResourceReader.RegisterResources();
        }

        public SimpleUIWindow()
            : base(1366, 768)
        {
            Keyboard.KeyDown += Keyboard_KeyDown;
            Keyboard.KeyUp += Keyboard_KeyUp;

            Mouse.ButtonDown += Mouse_ButtonDown;
            Mouse.ButtonUp += Mouse_ButtonUp;
            Mouse.Move += Mouse_Move;
            Mouse.WheelChanged += Mouse_Wheel;

            ftime = new List<long>(fps_frames);
            stopwatch = new Stopwatch();
        }

        public override void Dispose()
        {
            resManager.releaseResources();
            guiManager.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// Occurs when a key is pressed.
        /// </summary>
        /// <param name="sender">The KeyboardDevice which generated this event.</param>
        /// <param name="e">The key that was pressed.</param>
        void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == global::OpenTK.Input.Key.Escape)
                Exit();
            else if (e.Key == global::OpenTK.Input.Key.AltLeft)
                altDown = true;
            else if (altDown && e.Key == global::OpenTK.Input.Key.Enter)
                if (WindowState == WindowState.Fullscreen)
                    WindowState = WindowState.Normal;
                else
                    WindowState = WindowState.Fullscreen;

            input.ProcessKeyDown(e);
        }

        void Keyboard_KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            altDown = false;
            input.ProcessKeyUp(e);
        }

        void Mouse_ButtonDown(object sender, MouseButtonEventArgs args)
        {
            input.ProcessMouseMessage(args);
        }

        void Mouse_ButtonUp(object sender, MouseButtonEventArgs args)
        {
            input.ProcessMouseMessage(args);
        }

        void Mouse_Move(object sender, MouseMoveEventArgs args)
        {
            input.ProcessMouseMessage(args);
        }

        void Mouse_Wheel(object sender, MouseWheelEventArgs args)
        {
            input.ProcessMouseMessage(args);
        }

        /// <summary>
        /// Setup OpenGL and load resources here.
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
#if TESTING
            using (FileStream stream = new FileStream("Media/rgb-mipmap-reference.ktx", FileMode.Open))
            {
                uint pTexture = 0;
                TextureTarget pTarget;
                Sxta.Render.OpenGLExt.LibKTX.KTX_dimensions pDimensions;
                bool pIsMipmapped;
                ErrorCode pGlerror;
                int pKvdLen = 0;
                byte[] ppKvd = null;
                LibKTX.LoadTexture(stream, ref pTexture, out pTarget,
                                    out pDimensions, out pIsMipmapped,
                                    out pGlerror, ref pKvdLen, ref ppKvd);
            }
#endif

            XMLResourceLoader resLoader = new XMLResourceLoader();
            resLoader.addPath("./Resources/Textures");
            resLoader.addPath("./Resources/GUI");

            resManager = new ResourceManager(resLoader);
            guiManager = (GuiManager)resManager.loadResource("MainWindow").get();

            m_StatusBar = guiManager["statusbar"] as StatusBar;
            m_TextOutput = guiManager["textOutput"] as ListBox;

            GL.ClearColor(Color.MidnightBlue);

            input = new Gwen.Input.OpenTK(this);
            input.Initialize(guiManager.Canvas);

            stopwatch.Restart();
            lastTime = 0;
        }

        /// <summary>
        /// Respond to resize events here.
        /// </summary>
        /// <param name="e">Contains information on the new GameWindow size.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Width, Height, 0, -1, 1);

            guiManager.Canvas.SetSize(Width, Height);
        }

        /// <summary>
        /// Add your game logic here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (ftime.Count == fps_frames)
                ftime.RemoveAt(0);

            ftime.Add(stopwatch.ElapsedMilliseconds - lastTime);
            lastTime = stopwatch.ElapsedMilliseconds;

            if (stopwatch.ElapsedMilliseconds > 1000)
            {
                //Note = String.Format("String Cache size: {0} Draw Calls: {1} Vertex Count: {2}",  renderer.TextCacheSize, renderer.DrawCallCount, renderer.VertexCount);
                Fps = 1000f * ftime.Count / ftime.Sum();
                m_StatusBar.Text = String.Format("Sxta.UI - {0:F0} fps. {1}", Fps, Note);
                stopwatch.Restart();

                //if (renderer.TextCacheSize > 1000) // each cached string is an allocated texture, flush the cache once in a while in your real project
                //    renderer.FlushTextCache();
            }
        }

        /// <summary>
        /// Add your game rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            m_StatusBar.Text = String.Format("Sxta.UI - {0:F0} fps. {1}", Fps, Note);
            guiManager.Canvas.RenderCanvas();

            SwapBuffers();
        }

        #region GUI Methods
        public static void PrintText(String str)
        {
            m_TextOutput.AddRow(str);
            m_TextOutput.ScrollToBottom();
        }
        public static void onButtonIA(Base control)
        {
            PrintText("Button INSTANT ACTION.");
        }
        public static void onButtonDF(Base control)
        {
            PrintText("Button DOGFIGHT.");
        }
        public static void onButtonTE(Base control)
        {
            PrintText("Button TACTICAL ENGAGEMENT.");
        }
        public static void onButtonCP(Base control)
        {
            PrintText("Button CAMPAIGN.");
        }
        public static void onButtonTACREF(Base control)
        {
            PrintText("Button TACTICAL REFERENCE.");
        }
        public static void onButtonLB(Base control)
        {
            PrintText("Button LOGBOOK.");
            // Logbook logbook = new Logbook(this);
        }
        public static void onButtonACMI(Base control)
        {
            PrintText("Button ACMI.");
        }
        public static void onButtonCO(Base control)
        {
            PrintText("Button COMMS.");
        }
        public static void onButtonTHEATER(Base control)
        {
            PrintText("Button THEATER.");
        }
        public static void onButtonSTP(Base control)
        {
            PrintText("Button SETUP.");
        }
        public static void onButtonEXIT(Base control)
        {
            PrintText("Button EXIT.");
            //MessageBox window = new MessageBox(GetCanvas(), "Do you want to quit?");
            //window.Position(Pos.Center);
            //parentContext.Exit();
        }
        #endregion

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (SimpleUIWindow example = new SimpleUIWindow())
            {
                example.Title = "Gwen-DotNet OpenTK canvas";
                // example.VSync = VSyncMode.Off; // to measure performance
                example.Run(0.0, 0.0);
                //example.TargetRenderFrequency = 60;
            }
        }
    }
}
