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
    [Example("Example 2.2: Drawing with color", ExampleCategory.CoreUsage, "2. Drawing", 1, Source = "Tutorial02_2", Documentation = "Tutorial-TODO")]
    public class Tutorial02_2 : GameWindow
    {
        public Tutorial02_2()
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
            p = new Program(new Module(330, FRAGMENT_SHADER));
            primitivePoints = new Mesh<Vertex_V3C3f, uint>(Vertex_V3C3f.SizeInBytes, MeshMode.POINTS, MeshUsage.GPU_STATIC);
            primitivePoints.addAttributeType(0, 3, AttributeType.A32F, false);
            primitivePoints.addAttributeType(1, 3, AttributeType.A32F, false);
            primitivePoints.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-0.9f, 0.8f, 0.0f), Color = new Vector3f(0.8f, 0.4f, 0.8f) });
            primitivePoints.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-0.7f, 0.7f, 0.0f), Color = new Vector3f(0.6f, 0.2f, 0.1f) });
            primitivePoints.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-0.6f, 0.9f, 0.0f), Color = new Vector3f(0.2f, 0.9f, 0.3f) });
            primitivePoints.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-0.7f, 0.5f, 0.0f), Color = new Vector3f(0.5f, 0.7f, 0.1f) });
            primitivePoints.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-0.9f, 0.7f, 0.0f), Color = new Vector3f(0.9f, 0.1f, 0.2f) });
            primitivePoints.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-0.7f, 0.8f, 0.0f), Color = new Vector3f(0.2f, 0.8f, 0.8f) });
            primitivePoints.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-0.8f, 0.9f, 0.0f), Color = new Vector3f(0.5f, 0.1f, 0.1f) });
            primitivePoints.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-0.7f, 0.8f, 0.0f), Color = new Vector3f(0.5f, 0.8f, 0.8f) });


            primitiveLines = new Mesh<Vertex_V3C3f, uint>(Vertex_V3C3f.SizeInBytes, MeshMode.LINES, MeshUsage.GPU_STATIC);
            primitiveLines.addAttributeType(0, 3, AttributeType.A32F, false);
            primitiveLines.addAttributeType(1, 3, AttributeType.A32F, false);
            primitiveLines.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-0.1f, 0.9f, 0.0f), Color = new Vector3f(0.1f, 0.6f, 0.8f) });
            primitiveLines.addVertex(new Vertex_V3C3f() { Position = new Vector3f(0.2f, 0.8f, 0.0f), Color = new Vector3f(0.6f, 0.4f, 0.6f) });
            primitiveLines.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-0.1f, 0.7f, 0.0f), Color = new Vector3f(0.0f, 0.4f, 0.0f) });
            primitiveLines.addVertex(new Vertex_V3C3f() { Position = new Vector3f(0.3f, 0.8f, 0.0f), Color = new Vector3f(0.2f, 0.4f, 0.8f) });
            primitiveLines.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-0.2f, 0.9f, 0.0f), Color = new Vector3f(0.0f, 0.4f, 0.2f) });
            primitiveLines.addVertex(new Vertex_V3C3f() { Position = new Vector3f(0.3f, 0.6f, 0.0f), Color = new Vector3f(0.9f, 0.2f, 0.0f) });

            primitiveLinesStrip = new Mesh<Vertex_V3C3f, uint>(Vertex_V3C3f.SizeInBytes, MeshMode.LINE_STRIP, MeshUsage.GPU_STATIC);
            primitiveLinesStrip.addAttributeType(0, 3, AttributeType.A32F, false);
            primitiveLinesStrip.addAttributeType(1, 3, AttributeType.A32F, false);
            primitiveLinesStrip.addVertex(new Vertex_V3C3f() { Position = new Vector3f(0.9f, 0.5f, 0.0f), Color = new Vector3f(0.2f, 0.9f, 0.8f) });
            primitiveLinesStrip.addVertex(new Vertex_V3C3f() { Position = new Vector3f(0.7f, 0.7f, 0.0f), Color = new Vector3f(0.9f, 0.4f, 0.8f) });
            primitiveLinesStrip.addVertex(new Vertex_V3C3f() { Position = new Vector3f(0.6f, 0.9f, 0.0f), Color = new Vector3f(0.0f, 0.4f, 0.8f) });
            primitiveLinesStrip.addVertex(new Vertex_V3C3f() { Position = new Vector3f(0.7f, 0.5f, 0.0f), Color = new Vector3f(0.9f, 0.4f, 0.5f) });
            primitiveLinesStrip.addVertex(new Vertex_V3C3f() { Position = new Vector3f(0.8f, 0.9f, 0.0f), Color = new Vector3f(0.0f, 0.4f, 0.2f) });
            primitiveLinesStrip.addVertex(new Vertex_V3C3f() { Position = new Vector3f(0.9f, 0.8f, 0.0f), Color = new Vector3f(0.2f, 0.4f, 0.8f) });

            primitiveLinesLoop = new Mesh<Vertex_V3C3f, uint>(Vertex_V3C3f.SizeInBytes, MeshMode.LINE_LOOP, MeshUsage.GPU_STATIC);
            primitiveLinesLoop.addAttributeType(0, 3, AttributeType.A32F, false);
            primitiveLinesLoop.addAttributeType(1, 3, AttributeType.A32F, false);
            primitiveLinesLoop.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-0.9f, 0.3f, 0.0f), Color = new Vector3f(0.0f, 0.9f, 0.9f) });
            primitiveLinesLoop.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-0.7f, -0.2f, 0.0f), Color = new Vector3f(0.2f, 0.2f, 0.8f) });
            primitiveLinesLoop.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-0.6f, -0.2f, 0.0f), Color = new Vector3f(0.9f, 0.3f, 0.2f) });
            primitiveLinesLoop.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-0.8f, 0.0f, 0.0f), Color = new Vector3f(0.4f, 0.8f, 0.1f) });
            primitiveLinesLoop.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-0.8f, -0.4f, 0.0f), Color = new Vector3f(0.9f, 0.0f, 0.0f) });

            primitiveTriangles = new Mesh<Vertex_V3C3f, uint>(Vertex_V3C3f.SizeInBytes, MeshMode.TRIANGLES, MeshUsage.GPU_STATIC);
            primitiveTriangles.addAttributeType(0, 3, AttributeType.A32F, false);
            primitiveTriangles.addAttributeType(1, 3, AttributeType.A32F, false);
            primitiveTriangles.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-0.1f, 0.3f, 0.0f), Color = new Vector3f(0.8f, 0.9f, 0.0f) });
            primitiveTriangles.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-0.2f, -0.2f, 0.0f), Color = new Vector3f(0.1f, 0.2f, 0.8f) });
            primitiveTriangles.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-0.1f, -0.2f, 0.0f), Color = new Vector3f(0.6f, 0.9f, 0.0f) });
            primitiveTriangles.addVertex(new Vertex_V3C3f() { Position = new Vector3f(0.3f, 0.0f, 0.0f), Color = new Vector3f(0.9f, 0.5f, 0.0f) });
            primitiveTriangles.addVertex(new Vertex_V3C3f() { Position = new Vector3f(0.3f, -0.4f, 0.0f), Color = new Vector3f(0.1f, 0.2f, 0.4f) });
            primitiveTriangles.addVertex(new Vertex_V3C3f() { Position = new Vector3f(0.1f, -0.4f, 0.0f), Color = new Vector3f(0.3f, 0.8f, 0.1f) });


            primitiveTriangleStrip = new Mesh<Vertex_V3C3f, uint>(Vertex_V3C3f.SizeInBytes, MeshMode.TRIANGLE_STRIP, MeshUsage.GPU_STATIC);
            primitiveTriangleStrip.addAttributeType(0, 3, AttributeType.A32F, false);
            primitiveTriangleStrip.addAttributeType(1, 3, AttributeType.A32F, false);
            primitiveTriangleStrip.addVertex(new Vertex_V3C3f() { Position = new Vector3f(0.6f, 0.0f, 0.0f), Color = new Vector3f(0.1f, 0.3f, 0.4f) });
            primitiveTriangleStrip.addVertex(new Vertex_V3C3f() { Position = new Vector3f(0.65f, 0.2f, 0.0f), Color = new Vector3f(0.0f, 0.7f, 0.4f) });
            primitiveTriangleStrip.addVertex(new Vertex_V3C3f() { Position = new Vector3f(0.7f, -0.2f, 0.0f), Color = new Vector3f(0.0f, 0.9f, 0.0f) });
            primitiveTriangleStrip.addVertex(new Vertex_V3C3f() { Position = new Vector3f(0.75f, 0.0f, 0.0f), Color = new Vector3f(0.9f, 0.0f, 0.0f) });
            primitiveTriangleStrip.addVertex(new Vertex_V3C3f() { Position = new Vector3f(0.8f, -0.4f, 0.0f), Color = new Vector3f(0.9f, 0.6f, 0.0f) });
            primitiveTriangleStrip.addVertex(new Vertex_V3C3f() { Position = new Vector3f(0.9f, -0.4f, 0.0f), Color = new Vector3f(0.6f, 0.3f, 0.8f) });


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
        Mesh<Vertex_V3C3f, uint> primitivePoints;
        Mesh<Vertex_V3C3f, uint> primitiveLines;
        Mesh<Vertex_V3C3f, uint> primitiveLinesStrip;
        Mesh<Vertex_V3C3f, uint> primitiveLinesLoop;
        Mesh<Vertex_V3C3f, uint> primitiveTriangles;
        Mesh<Vertex_V3C3f, uint> primitiveTriangleStrip;

        const string FRAGMENT_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 Position;
        layout (location = 1) in vec3 Color;

        out vec3 VertexColor;

        void main()
        {
            gl_Position = vec4(Position, 1.0);
            VertexColor = Color;
        }
#endif
#ifdef _FRAGMENT_
        in vec3 VertexColor;
        out vec3 FragColor;
 
        void main()
        {
            FragColor =  VertexColor; 
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
            using (Tutorial02_2 example = new Tutorial02_2())
            {
                example.Run(30.0, 10.0);
            }
        }

        #endregion
        
        private struct Vertex_V3C3f
        {
            public Vector3f Position;
            public Vector3f Color;
            public static int SizeInBytes
            {
                get { return Vector3f.SizeInBytes + Vector3f.SizeInBytes; }
            }
        }
    }


}
