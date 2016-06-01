// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Sxta.Math;
using Sxta.Render;
using System;
using System.Drawing;

namespace Examples.Tutorials
{
    /// <summary>
    /// Demonstrates the GameWindow class.
    /// </summary>
    [Example("Example 2.1: Drawing primitives", ExampleCategory.Testing, "02. Drawing", 1, Source = "Tutorial02_1", Documentation = "Tutorial-TODO")]
    public class Tutorial02_1 : GameWindow
    {
        public Tutorial02_1()
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
            fb = new FrameBuffer(true);
            p = new Program(new Module(120, FRAGMENT_SHADER));
            primitivePoints = new Mesh<Vector4f, uint>(Vector4f.SizeInBytes, MeshMode.POINTS, MeshUsage.GPU_STATIC);
            primitivePoints.addAttributeType(0, 4, AttributeType.A32F, false);
            primitivePoints.addVertex(new Vector4f(-0.9f, 0.8f, 0.0f, 1));
            primitivePoints.addVertex(new Vector4f(-0.7f, 0.7f, 0.0f, 1));
            primitivePoints.addVertex(new Vector4f(-0.6f, 0.9f, 0.0f, 1));
            primitivePoints.addVertex(new Vector4f(-0.7f, 0.5f, 0.0f, 1));
            primitivePoints.addVertex(new Vector4f(-0.9f, 0.7f, 0.0f, 1));
            primitivePoints.addVertex(new Vector4f(-0.7f, 0.8f, 0.0f, 1));
            primitivePoints.addVertex(new Vector4f(-0.8f, 0.9f, 0.0f, 1));
            primitivePoints.addVertex(new Vector4f(-0.7f, 0.8f, 0.0f, 1));


            primitiveLines = new Mesh<Vector4f, uint>(Vector4f.SizeInBytes, MeshMode.LINES, MeshUsage.GPU_STATIC);
            primitiveLines.addAttributeType(0, 4, AttributeType.A32F, false);
            primitiveLines.addVertex(new Vector4f(-0.1f, 0.9f, 0.0f, 1));
            primitiveLines.addVertex(new Vector4f(0.2f, 0.8f, 0.0f, 1));
            primitiveLines.addVertex(new Vector4f(-0.1f, 0.7f, 0.0f, 1));
            primitiveLines.addVertex(new Vector4f(0.3f, 0.8f, 0.0f, 1));
            primitiveLines.addVertex(new Vector4f(-0.2f, 0.9f, 0.0f, 1));
            primitiveLines.addVertex(new Vector4f(0.3f, 0.6f, 0.0f, 1));

            primitiveLinesStrip = new Mesh<Vector4f, uint>(Vector4f.SizeInBytes, MeshMode.LINE_STRIP, MeshUsage.GPU_STATIC);
            primitiveLinesStrip.addAttributeType(0, 4, AttributeType.A32F, false);
            primitiveLinesStrip.addVertex(new Vector4f(0.9f, 0.5f, 0.0f, 1));
            primitiveLinesStrip.addVertex(new Vector4f(0.7f, 0.7f, 0.0f, 1));
            primitiveLinesStrip.addVertex(new Vector4f(0.6f, 0.9f, 0.0f, 1));
            primitiveLinesStrip.addVertex(new Vector4f(0.7f, 0.5f, 0.0f, 1));
            primitiveLinesStrip.addVertex(new Vector4f(0.8f, 0.9f, 0.0f, 1));
            primitiveLinesStrip.addVertex(new Vector4f(0.9f, 0.8f, 0.0f, 1));

            primitiveLinesLoop = new Mesh<Vector4f, uint>(Vector4f.SizeInBytes, MeshMode.LINE_LOOP, MeshUsage.GPU_STATIC);
            primitiveLinesLoop.addAttributeType(0, 4, AttributeType.A32F, false);
            primitiveLinesLoop.addVertex(new Vector4f(-0.9f, 0.3f, 0.0f, 1));
            primitiveLinesLoop.addVertex(new Vector4f(-0.7f, -0.2f, 0.0f, 1));
            primitiveLinesLoop.addVertex(new Vector4f(-0.6f, -0.2f, 0.0f, 1));
            primitiveLinesLoop.addVertex(new Vector4f(-0.8f, 0.0f, 0.0f, 1));
            primitiveLinesLoop.addVertex(new Vector4f(-0.8f, -0.4f, 0.0f, 1));

            primitiveTriangles = new Mesh<Vector4f, uint>(Vector4f.SizeInBytes, MeshMode.TRIANGLES, MeshUsage.GPU_STATIC);
            primitiveTriangles.addAttributeType(0, 4, AttributeType.A32F, false);
            primitiveTriangles.addVertex(new Vector4f(-0.1f, 0.3f, 0.0f, 1));
            primitiveTriangles.addVertex(new Vector4f(-0.2f, -0.2f, 0.0f, 1));
            primitiveTriangles.addVertex(new Vector4f(-0.1f, -0.2f, 0.0f, 1));
            primitiveTriangles.addVertex(new Vector4f(0.3f, 0.0f, 0.0f, 1));
            primitiveTriangles.addVertex(new Vector4f(0.3f, -0.4f, 0.0f, 1));
            primitiveTriangles.addVertex(new Vector4f(0.1f, -0.4f, 0.0f, 1));


            primitiveTriangleStrip = new Mesh<Vector4f, uint>(Vector4f.SizeInBytes, MeshMode.TRIANGLE_STRIP, MeshUsage.GPU_STATIC);
            primitiveTriangleStrip.addAttributeType(0, 4, AttributeType.A32F, false);
            primitiveTriangleStrip.addVertex(new Vector4f(0.6f, 0.0f, 0.0f, 1));
            primitiveTriangleStrip.addVertex(new Vector4f(0.65f, 0.2f, 0.0f, 1));
            primitiveTriangleStrip.addVertex(new Vector4f(0.7f, -0.2f, 0.0f, 1));
            primitiveTriangleStrip.addVertex(new Vector4f(0.75f, 0.0f, 0.0f, 1));
            primitiveTriangleStrip.addVertex(new Vector4f(0.8f, -0.4f, 0.0f, 1));
            primitiveTriangleStrip.addVertex(new Vector4f(0.9f, -0.4f, 0.0f, 1));


            fb.setClearColor(Color.MidnightBlue);
        }

        #endregion

        #region OnUnload

        protected override void OnUnload(EventArgs e)
        {
            if (p != null)
                p.Dispose();
            if (primitivePoints != null)
                primitivePoints.Dispose();
            if (primitiveLines != null)
                primitiveLines.Dispose();
            if (primitiveLinesLoop != null)
                primitiveLinesLoop.Dispose();
            if (primitiveLinesStrip != null)
                primitiveLinesStrip.Dispose();
            if (primitiveTriangles != null)
                primitiveTriangles.Dispose();
            if (primitiveTriangleStrip != null)
                primitiveTriangleStrip.Dispose();
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
            // GL.Viewport(0, 0, Width, Height);
            fb.setViewport(new Vector4i(0, 0, Width, Height));

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
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
            // Nothing to do!
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
            fb.clear(true, true, true);

            fb.draw(p, primitivePoints);
            fb.draw(p, primitiveLines);
            fb.draw(p, primitiveLinesStrip);
            fb.draw(p, primitiveLinesLoop);
            fb.draw(p, primitiveTriangles);
            fb.draw(p, primitiveTriangleStrip);
            this.SwapBuffers();
        }

        #endregion

        #region Fields
        FrameBuffer fb;
        Program p;
        Mesh<Vector4f, uint> primitivePoints;
        Mesh<Vector4f, uint> primitiveLines;
        Mesh<Vector4f, uint> primitiveLinesStrip;
        Mesh<Vector4f, uint> primitiveLinesLoop;
        Mesh<Vector4f, uint> primitiveTriangles;
        Mesh<Vector4f, uint> primitiveTriangleStrip;

        const string FRAGMENT_SHADER = @"
#ifdef _FRAGMENT_
        /* Copies incoming fragment color without change. */
        void main()
        {
            gl_FragColor = gl_Color;
        }

#endif";

        #endregion

        #region public static void Main()

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (Tutorial02_1 example = new Tutorial02_1())
            {
                example.Run(30.0, 10.0);
            }
        }

        #endregion
    }
}
