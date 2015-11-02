#define SAVE_RESULTS
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK;
using OpenTK.Graphics;
using Sxta.Math;
using Sxta.Render;
using Sxta.TestTools.ImageTesting;
using SxtaRenderTests.TestTools;
using System.Drawing;
 
namespace SxtaRenderTests
{
    [TestClass]
    public class DrawTestf
    {
        private static GameWindow control = null;
        protected const int Width = 500;
        protected const int Height = 500;
        protected ImageComparator imageComparer = new ImageComparator(DiffOptions.IGNORE_NOTHING);
        protected const float epsilonError = 0.0001f;

        private static readonly string TESTSNAME = "DrawTestf";

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            control = new GameWindow();
            RenderTestUtils.DeleteResultDir(TESTSNAME);
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            if (control != null)
                control.Dispose();
        }

        private static readonly string TestDrawf_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition; 
        void main()
        {
            gl_Position = vec4(aPosition*vec3(0.5,0.5,0.5), 1.0);
        }
#endif
#ifdef _FRAGMENT_
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(0.0, 1.0, 0.5, 1.0); 
        }
#endif";

        private static readonly string TestDrawInstancing01_SHADER = @"
#ifdef _VERTEX_
        layout(location=0) in vec4 pos;
        out vec4 VertexColor;
        void main() {
            gl_Position = vec4(pos.x + gl_InstanceID * 3.0/ 6.0f - 0.8, pos.yzw);
            VertexColor = vec4(1.0-gl_InstanceID*0.3, gl_InstanceID*0.1, gl_InstanceID*0.2+0.1, 1.0);
        }
#endif
#ifdef _FRAGMENT_
        in vec4 VertexColor;
        out vec4 FragColor;
        void main()
        {
            FragColor = VertexColor;
        }
#endif";

        [TestMethod]
        public void TestDraw01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(330, TestDrawf_SHADER));
                fb.setClearColor(Color.AntiqueWhite);
                fb.clear(true, false, false);
                fb.drawQuad(p);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestDraw01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot04.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestDraw01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestDrawInstancing01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));
                Mesh<Vector4f, uint>  quad = new Mesh<Vector4f, uint>(Vector4f.SizeInBytes, MeshMode.TRIANGLE_STRIP, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 4, AttributeType.A32F, false);
                quad.addVertex(new Vector4f(-0.2f, -0.2f, 0, 1));
                quad.addVertex(new Vector4f(0.2f, -0.2f, 0, 1));
                quad.addVertex(new Vector4f(-0.2f, 0.2f, 0, 1));
                quad.addVertex(new Vector4f(0.2f, 0.2f, 0, 1));

                Program p = new Program(new Module(330, TestDrawInstancing01_SHADER));
                fb.setClearColor(Color.AliceBlue);
                fb.clear(true, false, false);
                fb.draw(p, quad, 4);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawInstancing01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
                if (quad != null)
                    quad.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot05.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawInstancing01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestDrawPartDirect01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));
                Mesh<Vector4f, uint> quad = new Mesh<Vector4f, uint>(Vector4f.SizeInBytes, MeshMode.TRIANGLE_STRIP, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 4, AttributeType.A32F, false);
                quad.addVertex(new Vector4f(-1, -1, 0, 1));
                quad.addVertex(new Vector4f(1, -1, 0, 1));
                quad.addVertex(new Vector4f(-1, 1, 0, 1));
                quad.addVertex(new Vector4f(-1, 1, 0, 1));
                quad.addVertex(new Vector4f(1, -1, 0, 1));
                quad.addVertex(new Vector4f(1, 1, 0, 1));

                Program p = new Program(new Module(330, TestDrawf_SHADER));
                fb.setClearColor(Color.AliceBlue);
                fb.clear(true, false, false);
                MeshBuffers m = quad.getBuffers();
                fb.draw(p, m, MeshMode.TRIANGLES, 0, 3);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawPartDirect01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
                if (quad != null)
                    quad.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot06.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawPartDirect01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestDrawPartInstancingDirect01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));
                Mesh<Vector4f, uint> quad = new Mesh<Vector4f, uint>(Vector4f.SizeInBytes, MeshMode.TRIANGLE_STRIP, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 4, AttributeType.A32F, false);
                quad.addVertex(new Vector4f(-0.2f, -0.2f, 0, 1));
                quad.addVertex(new Vector4f(0.2f, -0.2f, 0, 1));
                quad.addVertex(new Vector4f(-0.2f, 0.2f, 0, 1));
                quad.addVertex(new Vector4f(0.2f, 0.2f, 0, 1));
                quad.addVertex(new Vector4f(-0.2f, 0.2f, 0, 1));
                quad.addVertex(new Vector4f(0.2f, -0.2f, 0, 1));
                quad.addVertex(new Vector4f(-0.2f, -0.2f, 0, 1));

                Program p = new Program(new Module(330, TestDrawInstancing01_SHADER));
                fb.setClearColor(Color.AliceBlue);
                fb.clear(true, false, false);
                MeshBuffers m = quad.getBuffers();
                fb.draw(p, m, MeshMode.TRIANGLES, 0, 6, 4);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawPartInstancingDirect01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
                if (quad != null)
                    quad.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot05.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawPartInstancingDirect01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

    }
}
