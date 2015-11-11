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
        protected ImageComparator imageComparer = new ImageComparator(DiffOptions.STRICT);
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
        public void TestDrawIndices01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(330, TestDrawf_SHADER));
                Mesh<Vector3f, uint> quad = new Mesh<Vector3f, uint>(Vector3f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLES, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 3, AttributeType.A32F, false);
                quad.addVertex(new Vector3f(-1f, -1, 0));
                quad.addVertex(new Vector3f(1f, -1f, 0));
                quad.addVertex(new Vector3f(-1f, 1f, 0));
                quad.addVertex(new Vector3f(1f, 1f, 0));
                quad.addIndice(0);
                quad.addIndice(1);
                quad.addIndice(2);
                quad.addIndice(2);
                quad.addIndice(1);
                quad.addIndice(3);

                fb.setClearColor(Color.AntiqueWhite);
                fb.clear(true, false, false);
                fb.draw(p, quad);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawIndices01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
                if (quad != null)
                    quad.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot04.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawIndices01_ImageDiff", diffResult);
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
                Mesh<Vector4f, uint> quad = new Mesh<Vector4f, uint>(Vector4f.SizeInBytes, MeshMode.TRIANGLE_STRIP, MeshUsage.GPU_STATIC);
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
        public void TestDrawPartIndices01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(330, TestDrawf_SHADER));
                Mesh<Vector3f, uint> quad = new Mesh<Vector3f, uint>(Vector3f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLES, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 3, AttributeType.A32F, false);
                quad.addVertex(new Vector3f(-1f, -1, 0));
                quad.addVertex(new Vector3f(1f, -1f, 0));
                quad.addVertex(new Vector3f(-1f, 1f, 0));
                quad.addVertex(new Vector3f(1f, 1f, 0));
                quad.addIndice(2);
                quad.addIndice(1);
                quad.addIndice(3);
                quad.addIndice(0);
                quad.addIndice(1);
                quad.addIndice(2);

                fb.setClearColor(Color.AliceBlue);
                fb.clear(true, false, false);
                MeshBuffers m = quad.getBuffers();
                fb.draw(p, m, MeshMode.TRIANGLES, 3, 3);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawPartIndices01_Screenshot", bmp);
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
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawPartIndices01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestPartIndicesWithBase01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(330, TestDrawf_SHADER));
                Mesh<Vector3f, uint> quad = new Mesh<Vector3f, uint>(Vector3f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLES, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 3, AttributeType.A32F, false);
                quad.addVertex(new Vector3f(0, 0, 0));
                quad.addVertex(new Vector3f(-1f, -1, 0));
                quad.addVertex(new Vector3f(1f, -1f, 0));
                quad.addVertex(new Vector3f(-1f, 1f, 0));
                quad.addVertex(new Vector3f(1f, 1f, 0));
                quad.addIndice(0);
                quad.addIndice(1);
                quad.addIndice(2);
                quad.addIndice(2);
                quad.addIndice(1);
                quad.addIndice(3);

                fb.setClearColor(Color.AliceBlue);
                fb.clear(true, false, false);
                MeshBuffers m = quad.getBuffers();
                fb.draw(p, m, MeshMode.TRIANGLES, 0, 3, 1, 1);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestPartIndicesWithBase01_Screenshot", bmp);
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
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestPartIndicesWithBase01_ImageDiff", diffResult);
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

        [TestMethod]
        public void TestDrawPartInstancingIndices01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));
                Mesh<Vector4f, uint> quad = new Mesh<Vector4f, uint>(Vector4f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLE_STRIP, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 4, AttributeType.A32F, false);
                quad.addVertex(new Vector4f(-0.2f, -0.2f, 0, 1));
                quad.addVertex(new Vector4f(0.2f, -0.2f, 0, 1));
                quad.addVertex(new Vector4f(-0.2f, 0.2f, 0, 1));
                quad.addVertex(new Vector4f(0.2f, 0.2f, 0, 1));
                quad.addVertex(new Vector4f(-0.2f, 0.2f, 0, 1));
                quad.addVertex(new Vector4f(0.2f, -0.2f, 0, 1));
                quad.addVertex(new Vector4f(-0.2f, -0.2f, 0, 1));
                quad.addIndice(2);
                quad.addIndice(1);
                quad.addIndice(3);
                quad.addIndice(0);
                quad.addIndice(1);
                quad.addIndice(2);
                quad.addIndice(2);
                quad.addIndice(1);
                quad.addIndice(3);

                Program p = new Program(new Module(330, TestDrawInstancing01_SHADER));
                fb.setClearColor(Color.AliceBlue);
                fb.clear(true, false, false);
                MeshBuffers m = quad.getBuffers();
                fb.draw(p, m, MeshMode.TRIANGLES, 3, 6, 4);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawPartInstancingIndices01_Screenshot", bmp);
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
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawPartInstancingIndices01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestDrawPartInstancingIndicesWithBase01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));
                Mesh<Vector4f, uint> quad = new Mesh<Vector4f, uint>(Vector4f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLE_STRIP, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 4, AttributeType.A32F, false);
                quad.addVertex(new Vector4f(0, 0, 0, 1));
                quad.addVertex(new Vector4f(0, 0, 0, 1));
                quad.addVertex(new Vector4f(-0.2f, -0.2f, 0, 1));
                quad.addVertex(new Vector4f(0.2f, -0.2f, 0, 1));
                quad.addVertex(new Vector4f(-0.2f, 0.2f, 0, 1));
                quad.addVertex(new Vector4f(0.2f, 0.2f, 0, 1));
                quad.addVertex(new Vector4f(-0.2f, 0.2f, 0, 1));
                quad.addVertex(new Vector4f(0.2f, -0.2f, 0, 1));
                quad.addVertex(new Vector4f(-0.2f, -0.2f, 0, 1));
                quad.addIndice(2);
                quad.addIndice(1);
                quad.addIndice(3);
                quad.addIndice(0);
                quad.addIndice(1);
                quad.addIndice(2);
                quad.addIndice(2);
                quad.addIndice(1);
                quad.addIndice(3);

                Program p = new Program(new Module(330, TestDrawInstancing01_SHADER));
                fb.setClearColor(Color.AliceBlue);
                fb.clear(true, false, false);
                MeshBuffers m = quad.getBuffers();
                fb.draw(p, m, MeshMode.TRIANGLES, 0, 6, 4, 2);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawPartInstancingIndicesWithBase01_Screenshot", bmp);
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
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawPartInstancingIndicesWithBase01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestMultiDrawInstancingDirect01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(330, TestDrawf_SHADER));
                Mesh<Vector3f, uint> quad = new Mesh<Vector3f, uint>(Vector3f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLES, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 3, AttributeType.A32F, false);
                quad.addVertex(new Vector3f(-1, -1, 0));
                quad.addVertex(new Vector3f(0, -1, 0));
                quad.addVertex(new Vector3f(-1, 0, 0));
                quad.addVertex(new Vector3f(-1, 0, 0));
                quad.addVertex(new Vector3f(0, -1, 0));
                quad.addVertex(new Vector3f(0, 0, 0));
                quad.addVertex(new Vector3f(0, -1, 0));
                quad.addVertex(new Vector3f(1, -1, 0));
                quad.addVertex(new Vector3f(0, 0, 0));
                quad.addVertex(new Vector3f(0, 0, 0));
                quad.addVertex(new Vector3f(1, -1, 0));
                quad.addVertex(new Vector3f(1, 0, 0));
                quad.addVertex(new Vector3f(-1, 0, 0));
                quad.addVertex(new Vector3f(0, 0, 0));
                quad.addVertex(new Vector3f(-1, 1, 0));
                quad.addVertex(new Vector3f(-1, 1, 0));
                quad.addVertex(new Vector3f(0, 0, 0));
                quad.addVertex(new Vector3f(0, 1, 0));
                quad.addVertex(new Vector3f(0, 0, 0));
                quad.addVertex(new Vector3f(1, 0, 0));
                quad.addVertex(new Vector3f(0, 1, 0));
                quad.addVertex(new Vector3f(0, 1, 0));
                quad.addVertex(new Vector3f(1, 0, 0));
                quad.addVertex(new Vector3f(1, 1, 0));
                MeshBuffers m = quad.getBuffers();

                fb.setClearColor(Color.AntiqueWhite);
                fb.clear(true, false, false);
                int[] firsts = new int[] { 0, 18 };
                int[] counts = new int[] { 6, 6 };
                fb.multiDraw(p, m, MeshMode.TRIANGLES, firsts, counts, 2);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestMultiDrawInstancingDirect01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
                if (quad != null)
                    quad.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot07.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestMultiDrawInstancingDirect01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }


        [TestMethod]
        public void TestMultiDrawInstancingIndices01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(330, TestDrawf_SHADER));
                Mesh<Vector3f, uint> quad = new Mesh<Vector3f, uint>(Vector3f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLES, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 3, AttributeType.A32F, false);
                quad.addVertex(new Vector3f(-1, -1, 0));
                quad.addVertex(new Vector3f(0, -1, 0));
                quad.addVertex(new Vector3f(1, -1, 0));
                quad.addVertex(new Vector3f(-1, 0, 0));

                quad.addVertex(new Vector3f(0, 0, 0));

                quad.addVertex(new Vector3f(1, 0, 0));
                quad.addVertex(new Vector3f(-1, 1, 0));
                quad.addVertex(new Vector3f(0, 1, 0));
                quad.addVertex(new Vector3f(1, 1, 0));
                quad.addIndice(0); // 0
                quad.addIndice(1);
                quad.addIndice(3);
                quad.addIndice(3);
                quad.addIndice(1);
                quad.addIndice(4);
                quad.addIndice(1);
                quad.addIndice(2);
                quad.addIndice(4);
                quad.addIndice(4);
                quad.addIndice(2);
                quad.addIndice(5);
                quad.addIndice(3);
                quad.addIndice(4);
                quad.addIndice(6);
                quad.addIndice(6);
                quad.addIndice(4);
                quad.addIndice(7);
                quad.addIndice(4); // 18
                quad.addIndice(5);
                quad.addIndice(7);
                quad.addIndice(7);
                quad.addIndice(5);
                quad.addIndice(8);
                MeshBuffers m = quad.getBuffers();

                fb.setClearColor(Color.AntiqueWhite);
                fb.clear(true, false, false);
                int[] firsts = new int[] { 0, 18 };
                int[] counts = new int[] { 6, 6 };
                fb.multiDraw(p, m, MeshMode.TRIANGLES, firsts, counts, firsts.Length);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestMultiDrawInstancingIndices01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
                if (quad != null)
                    quad.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot07.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestMultiDrawInstancingIndices01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }


        [TestMethod]
        public void TestMultiDrawInstancingIndicesWithBase01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));
                Program p = new Program(new Module(330, TestDrawf_SHADER));

                Mesh<Vector3f, uint> quad = new Mesh<Vector3f, uint>(Vector3f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLE_STRIP, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 3, AttributeType.A32F, false);
                quad.addVertex(new Vector3f(-1, -1, 0));
                quad.addVertex(new Vector3f(0, -1, 0));
                quad.addVertex(new Vector3f(1, -1, 0));
                quad.addVertex(new Vector3f(-1, 0, 0));

                quad.addVertex(new Vector3f(0, 0, 0));

                quad.addVertex(new Vector3f(1, 0, 0));
                quad.addVertex(new Vector3f(-1, 1, 0));
                quad.addVertex(new Vector3f(0, 1, 0));
                quad.addVertex(new Vector3f(1, 1, 0));
                quad.addIndice(0); // 0
                quad.addIndice(1);
                quad.addIndice(3);
                quad.addIndice(3);
                quad.addIndice(1);
                quad.addIndice(4);
                quad.addIndice(1); // 6
                quad.addIndice(2);
                quad.addIndice(4);
                quad.addIndice(4);
                quad.addIndice(2);
                quad.addIndice(5);
                quad.addIndice(3); // 12
                quad.addIndice(4);
                quad.addIndice(6);
                quad.addIndice(6);
                quad.addIndice(4);
                quad.addIndice(7);
                quad.addIndice(4); // 18
                quad.addIndice(5);
                quad.addIndice(7);
                quad.addIndice(7);
                quad.addIndice(5);
                quad.addIndice(8);
                MeshBuffers m = quad.getBuffers();

                fb.setClearColor(Color.AntiqueWhite);
                fb.clear(true, false, false);
                int[] firsts = new int[] { 6, 12 };
                int[] counts = new int[] { 6, 6 };
                int[] bases = new int[] { -1, +1 };
                fb.multiDraw(p, m, MeshMode.TRIANGLES, firsts, counts, firsts.Length, bases);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestMultiDrawInstancingIndicesWithBase01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
                if (quad != null)
                    quad.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot07.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestMultiDrawInstancingIndicesWithBase01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestDrawIndirectInstancing01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));
                Program p = new Program(new Module(330, TestDrawInstancing01_SHADER));

                Mesh<Vector4f, uint> quad = new Mesh<Vector4f, uint>(Vector4f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLE_STRIP, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 4, AttributeType.A32F, false);
                quad.addVertex(new Vector4f(-0.2f, -0.2f, 0, 1));
                quad.addVertex(new Vector4f(0.2f, -0.2f, 0, 1));
                quad.addVertex(new Vector4f(-0.2f, 0.2f, 0, 1));
                quad.addVertex(new Vector4f(0.2f, 0.2f, 0, 1));
                quad.addVertex(new Vector4f(-0.2f, 0.2f, 0, 1));
                quad.addVertex(new Vector4f(0.2f, -0.2f, 0, 1));
                quad.addVertex(new Vector4f(-0.2f, -0.2f, 0, 1));

                MeshBuffers m = quad.getBuffers();

                fb.setClearColor(Color.AliceBlue);
                fb.clear(true, false, false);
                GPUBuffer buff = new GPUBuffer();

                // typedef  struct {
                //    uint count; 6 vertices to form a quad
                //    uint instanceCount; 4 instances
                //    uint first;  0 -- start at 0
                //    uint baseInstance;
                // } DrawArraysIndirectCommand;     
                buff.setData<int>(4 * 4, new int[] { 6, 4, 0, 0 }, BufferUsage.STATIC_READ);
                fb.drawIndirect(p, m, MeshMode.TRIANGLES, buff);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawIndirectInstancing01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
                if (quad != null)
                    quad.Dispose();
                if (buff != null)
                    buff.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot05.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawIndirectInstancing01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestDrawIndirectInstancingElements01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));
                Program p = new Program(new Module(330, TestDrawInstancing01_SHADER));

                Mesh<Vector4f, uint> quad = new Mesh<Vector4f, uint>(Vector4f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLE_STRIP, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 4, AttributeType.A32F, false);
                quad.addVertex(new Vector4f(-0.2f, -0.2f, 0, 1));
                quad.addVertex(new Vector4f(0.2f, -0.2f, 0, 1));
                quad.addVertex(new Vector4f(-0.2f, 0.2f, 0, 1));
                quad.addVertex(new Vector4f(0.2f, 0.2f, 0, 1));
                quad.addVertex(new Vector4f(-0.2f, 0.2f, 0, 1));
                quad.addVertex(new Vector4f(0.2f, -0.2f, 0, 1));
                quad.addVertex(new Vector4f(-0.2f, -0.2f, 0, 1));
                quad.addIndice(2);
                quad.addIndice(1);
                quad.addIndice(3);
                quad.addIndice(0);
                quad.addIndice(1);
                quad.addIndice(2);
                quad.addIndice(2);
                quad.addIndice(1);
                quad.addIndice(3);

                MeshBuffers m = quad.getBuffers();

                fb.setClearColor(Color.AliceBlue);
                fb.clear(true, false, false);
                GPUBuffer buff = new GPUBuffer();

                //typedef  struct {
                //    uint count;
                //    uint instanceCount;
                //    uint firstIndex;
                //    uint baseVertex;
                //    uint baseInstance;
                //}  DrawElementsIndirectCommand;    
                buff.setData<int>(5 * 4, new int[] { 6, 4, 0, 0, 0}, BufferUsage.STATIC_READ);
                fb.drawIndirect(p, m, MeshMode.TRIANGLES, buff);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawIndirectInstancingElements01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
                if (quad != null)
                    quad.Dispose();
                if (buff != null)
                    buff.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot05.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawIndirectInstancingElements01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestDrawIndirectInstancingElementsWithBase01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));
                Program p = new Program(new Module(330, TestDrawInstancing01_SHADER));

                Mesh<Vector4f, uint> quad = new Mesh<Vector4f, uint>(Vector4f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLE_STRIP, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 4, AttributeType.A32F, false);
                quad.addVertex(new Vector4f(1f, 1f, 0, 1));
                quad.addVertex(new Vector4f(-0.2f, -0.2f, 0, 1));
                quad.addVertex(new Vector4f(0.2f, -0.2f, 0, 1));
                quad.addVertex(new Vector4f(-0.2f, 0.2f, 0, 1));
                quad.addVertex(new Vector4f(0.2f, 0.2f, 0, 1));
                quad.addVertex(new Vector4f(-0.2f, 0.2f, 0, 1));
                quad.addVertex(new Vector4f(0.2f, -0.2f, 0, 1));
                quad.addVertex(new Vector4f(-0.2f, -0.2f, 0, 1));
                quad.addIndice(2);
                quad.addIndice(1);
                quad.addIndice(3);
                quad.addIndice(0);
                quad.addIndice(1);
                quad.addIndice(2);
                quad.addIndice(2);
                quad.addIndice(1);
                quad.addIndice(3);

                MeshBuffers m = quad.getBuffers();

                fb.setClearColor(Color.AliceBlue);
                fb.clear(true, false, false);
                GPUBuffer buff = new GPUBuffer();

                //typedef  struct {
                //    uint count;
                //    uint instanceCount;
                //    uint firstIndex;
                //    uint baseVertex;
                //    uint baseInstance;
                //}  DrawElementsIndirectCommand;    
                buff.setData<int>(5 * 4, new int[] { 6, 4, 0, 1, 0 }, BufferUsage.STATIC_READ);
                fb.drawIndirect(p, m, MeshMode.TRIANGLES, buff);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawIndirectInstancingElementsWithBase01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
                if (quad != null)
                    quad.Dispose();
                if (buff != null)
                    buff.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot05.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawIndirectInstancingElementsWithBase01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestDrawPrimitiveRestart01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(330, TestDrawf_SHADER));
                Mesh<Vector3f, uint> quad = new Mesh<Vector3f, uint>(Vector3f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLE_STRIP, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 3, AttributeType.A32F, false);
                quad.addVertex(new Vector3f(-1f, -1, 0));
                quad.addVertex(new Vector3f(1f, -1f, 0));
                quad.addVertex(new Vector3f(-1f, 1f, 0));
                quad.addVertex(new Vector3f(1f, 1f, 0));
                quad.addIndice(0);
                quad.addIndice(1);
                quad.addIndice(2);
                quad.addIndice(255);
                quad.addIndice(2);
                quad.addIndice(1);
                quad.addIndice(3);
                quad.setPrimitiveRestart(255);

                fb.setClearColor(Color.AntiqueWhite);
                fb.clear(true, false, false);
                fb.draw(p, quad);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawPrimitiveRestart01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
                if (quad != null)
                    quad.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot04.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawPrimitiveRestart01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestMeshModificationDirect01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(330, TestDrawf_SHADER));
                Mesh<Vector3f, uint> quad = new Mesh<Vector3f, uint>(Vector3f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLES, MeshUsage.GPU_DYNAMIC);
                quad.addAttributeType(0, 3, AttributeType.A32F, false);
                quad.addVertex(new Vector3f(-1f, -1, 0));
                quad.addVertex(new Vector3f(1f, -1f, 0));
                quad.addVertex(new Vector3f(-1f, 1f, 0));

                fb.setClearColor(Color.AntiqueWhite);
                fb.clear(true, false, false);
                fb.draw(p, quad);

                quad.setVertex(0, new Vector3f(-1f, 1f, 0));
                quad.setVertex(1, new Vector3f(1f, -1f, 0));
                quad.setVertex(2, new Vector3f(1f, 1f, 0));
                fb.draw(p, quad);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestMeshModificationDirect01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
                if (quad != null)
                    quad.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot04.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestMeshModificationDirect01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestMeshModificationIndices01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(330, TestDrawf_SHADER));
                Mesh<Vector3f, uint> quad = new Mesh<Vector3f, uint>(Vector3f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLES, MeshUsage.GPU_DYNAMIC);
                quad.addAttributeType(0, 3, AttributeType.A32F, false);
                quad.addVertex(new Vector3f(-1f, -1, 0));
                quad.addVertex(new Vector3f(1f, -1f, 0));
                quad.addVertex(new Vector3f(-1f, 1f, 0));
                quad.addVertex(new Vector3f(1f, 1f, 0));
                quad.addIndice(0);
                quad.addIndice(1);
                quad.addIndice(2);

                fb.setClearColor(Color.AntiqueWhite);
                fb.clear(true, false, false);
                fb.draw(p, quad);

                quad.setIndice(0, 2);
                quad.setIndice(1, 1);
                quad.setIndice(2, 3);
                fb.draw(p, quad);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestMeshModificationIndices01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
                if (quad != null)
                    quad.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot04.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestMeshModificationIndices01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }
    }
}
