using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Sxta.Math;
using Sxta.Render;
using System;
using System.Drawing;
using MathHelper = Sxta.Math.MathHelper;

namespace SxtaRenderTests
{
    [TestClass]
    public class TerrainSquareTest : GameWindow
    {
        private struct Vertex_V3C3f
        {
            public Vector3f Position;
            public Vector3f Color;
            public static int SizeInBytes
            {
                get { return Vector3f.SizeInBytes + Vector3f.SizeInBytes; }
            }
        }

        FrameBuffer fb;
        Program p;
        Mesh<Vertex_V3C3f, uint> mesh;
        Matrix4f mat;
        UniformMatrix4f uMVMatrix;
        float angle = 0;

        const string FRAGMENT_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in vec3 aColor;

        uniform mat4 uMVMatrix;
        uniform mat4 uPMatrix;

        out vec3 vColor;

        void main()
        {
            gl_Position = uPMatrix * uMVMatrix * vec4(aPosition, 1.0);
            vColor = aColor;
        }
#endif
#ifdef _FRAGMENT_
        in vec3 vColor;
        out vec3 FragColor;
 
        void main()
        {
            FragColor =  vColor; 
        }
#endif";

        public TerrainSquareTest()
            : base(600, 600)
        {
            Keyboard.KeyDown += Keyboard_KeyDown;
        }
        [TestMethod]
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

        [TestMethod]
        protected override void OnLoad(EventArgs e)
        {
            fb = new FrameBuffer(true);
            fb.setDepthTest(true);

            p = new Program(new Module(330, FRAGMENT_SHADER));
            uMVMatrix = p.getUniformMatrix4f("uMVMatrix");

            UniformMatrix4f uPMatrix = p.getUniformMatrix4f("uPMatrix");
            // fovy, aspect, zNear, zFar
            Matrix4f projection = Matrix4f.CreatePerspectiveFieldOfView((float)MathHelper.ToRadians(60), (float)this.Width / (float)this.Height, 0.01f, 100.0f);
            uPMatrix.set(projection);

            mesh = new Mesh<Vertex_V3C3f, uint>(Vertex_V3C3f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLES, MeshUsage.GPU_STATIC);
            mesh.addAttributeType(0, 3, AttributeType.A32F, false);
            mesh.addAttributeType(1, 3, AttributeType.A32F, false);

            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, -1), Color = new Vector3f(1, 1, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, -1, -1), Color = new Vector3f(1, 1, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, 1, -1), Color = new Vector3f(1, 1, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, 1, -1), Color = new Vector3f(1, 1, 0) });

            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, 1), Color = new Vector3f(0, 0, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, -1, 1), Color = new Vector3f(0, 0, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, 1, 1), Color = new Vector3f(0, 0, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, 1, 1), Color = new Vector3f(0, 0, 1) });

            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, -1), Color = new Vector3f(0, 1, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, 1, -1), Color = new Vector3f(0, 1, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, 1, 1), Color = new Vector3f(0, 1, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, 1), Color = new Vector3f(0, 1, 1) });

            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, -1, -1), Color = new Vector3f(1, 0, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, 1, -1), Color = new Vector3f(1, 0, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, 1, 1), Color = new Vector3f(1, 0, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, -1, 1), Color = new Vector3f(1, 0, 0) });

            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, -1), Color = new Vector3f(1, 0, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, 1), Color = new Vector3f(1, 0, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, -1, 1), Color = new Vector3f(1, 0, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, -1, -1), Color = new Vector3f(1, 0, 1) });

            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, 1, -1), Color = new Vector3f(0, 1, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, 1, 1), Color = new Vector3f(0, 1, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, 1, 1), Color = new Vector3f(0, 1, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, 1, -1), Color = new Vector3f(0, 1, 0) });
            mesh.addIndices(new uint[] {
                               0,1,2,
                               0,2,3,

                               4,5,6,
                               4,6,7,

                               8,9,10,
                               8,10,11,

                               12,13,14,
                               12,14,15,

                               16,17,18,
                               16,18,19,

                               20,21,22,
                               20,22,23 });

            fb.setClearColor(Color.White);
        }
        [TestMethod]
        public void Main()
        {
            using (TerrainSquareTest example = new TerrainSquareTest())
            {
                example.Run(30.0, 10.0);
            }
        }
    }
}
