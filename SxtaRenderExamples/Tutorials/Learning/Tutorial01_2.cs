// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using log4net;
using OpenTK;
using OpenTK.Input;
using Sxta.Render;
using System;
using System.Reflection;

namespace Examples.Tutorials
{
    /// <summary>
    /// Demonstrates the logging system (based on log4net).
    /// </summary>
    [Example("Learning 1.02: Testing and Logging", ExampleCategory.Learning, "1. Getting Started", 1, Source = "Tutorial01_2", Documentation = "Tutorial01_2")]
    public class TutorialLearning01_2 : GameWindow
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public TutorialLearning01_2()
            : base(600, 600)
        {
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
            log.Debug("You have press the key: " + e.Key);
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
            log.Debug("In OnLoad event!");
            FrameBuffer.LogOpenGLInfo();
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
            // Usually you don't want to do this in this event. 
            // It will generate a LOT of log messages!
            this.SwapBuffers();
        }

        #endregion

        #region public static void Main()

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (TutorialLearning01_2 example = new TutorialLearning01_2())
            {
                // Enters the game loop of the GameWindow using the maximum update rate.
                example.Run();
            }
        }

        #endregion
    }
}