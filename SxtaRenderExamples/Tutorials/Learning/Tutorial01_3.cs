// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using log4net;
using OpenTK;
using OpenTK.Input;
using Sxta.Math;
using Sxta.Render;
using System;
using System.Drawing;
using System.Reflection;

namespace Examples.Tutorials
{
    /// <summary>
    /// Demonstrates the GameWindow game loop.
    /// </summary>
    [Example("Learning 1.03: Game Loop", ExampleCategory.Learning, "1. Getting Started", 1, Source = "Tutorial01_3", Documentation = "Tutorial01_3")]
    public class TutorialLearning01_3 : GameWindow
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public TutorialLearning01_3()
            : base(600, 600)
        {
            Keyboard.KeyDown += Keyboard_KeyDown;
            log.Debug("In the constructor!");
        }

        #region Keyboard_KeyDown

        /// <summary>
        /// Occurs when a key is pressed.
        /// </summary>
        /// <param name="sender">The KeyboardDevice which generated this event.</param>
        /// <param name="e">The key that was pressed.</param>
        void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            log.Debug("In Keyboard_KeyDown!");

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
        }

        #endregion

        #region OnUnload

        protected override void OnUnload(EventArgs e)
        {
            log.Debug("In OnUnload event!");
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
            log.Debug("In OnResize event!");
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
            // Usually you don't want to do this in this event. 
            // It will generate a LOT of log messages!
            log.Debug("In OnUpdateFrame event!");
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
            log.Debug("In OnRenderFrame event!");
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
            using (TutorialLearning01_3 example = new TutorialLearning01_3())
            {
                // Enters the game loop of the GameWindow using the maximum update rate.
                example.Run();
            }
        }

        #endregion
    }
}