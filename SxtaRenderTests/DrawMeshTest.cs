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
    public class DrawMeshTest
    {
        private static GameWindow control = null;
        protected const int Width = 500;
        protected const int Height = 500;
        protected ImageComparator imageComparer = new ImageComparator(DiffOptions.STRICT);
        protected const float epsilonError = 0.0001f;

        private static readonly string TESTSNAME = "DrawMeshTest";

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
        private static readonly string TestPrimitives_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec2 aPosition; 
        void main()
        {
            gl_Position =  vec4(aPosition*vec2(0.5, 0.5), 0.5, 1.0);
        }
#endif
#ifdef _FRAGMENT_
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(0.0, 0.2, 0.3, 1.0); 
        }
#endif";


        private static readonly string TestDraw2f_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec2 aPosition; 
        void main()
        {
            gl_Position =  vec4(aPosition*vec2(0.5, 0.5), 0.5, 1.0);
        }
#endif
#ifdef _FRAGMENT_
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(0.0, 1.0, 0.5, 1.0); 
        }
#endif";

        private static readonly string TestDraw3f_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition; 
        void main()
        {
            gl_Position =  vec4(aPosition*vec3(0.5, 0.5, 0.5), 1.0);
        }
#endif
#ifdef _FRAGMENT_
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(0.0, 1.0, 0.5, 1.0); 
        }
#endif";

        private static readonly string TestDraw4f_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec4 aPosition; 
        void main()
        {
            gl_Position =  aPosition*vec4(0.5, 0.5, 0.5, 1.0);
        }
#endif
#ifdef _FRAGMENT_
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(0.0, 1.0, 0.5, 1.0); 
        }
#endif";


        [TestMethod]
        public void TestDrawVector4f01()
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
                quad.addVertex(new Vector4f(-1f, -1, 0, 1));
                quad.addVertex(new Vector4f(1f, -1f, 0, 1));
                quad.addVertex(new Vector4f(-1f, 1f, 0, 1));
                quad.addVertex(new Vector4f(1f, 1f, 0, 1));

                Program p = new Program(new Module(330, TestDraw4f_SHADER));
                fb.setClearColor(Color.AntiqueWhite);
                fb.clear(true, false, false);
                fb.draw(p, quad);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawVector4f01_Screenshot", bmp);
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
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawVector4f01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestDrawVector3f01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));
                Mesh<Vector3f, uint> quad = new Mesh<Vector3f, uint>(Vector3f.SizeInBytes, MeshMode.TRIANGLE_STRIP, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 4, AttributeType.A32F, false);
                quad.addVertex(new Vector3f(-1f, -1, 0));
                quad.addVertex(new Vector3f(1f, -1f, 0));
                quad.addVertex(new Vector3f(-1f, 1f, 0));
                quad.addVertex(new Vector3f(1f, 1f, 0));

                Program p = new Program(new Module(330, TestDraw3f_SHADER));
                fb.setClearColor(Color.AntiqueWhite);
                fb.clear(true, false, false);
                fb.draw(p, quad);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawVector3f01_Screenshot", bmp);
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
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawVector3f01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestDrawVector2f01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));
                Mesh<Vector2f, uint> quad = new Mesh<Vector2f, uint>(Vector2f.SizeInBytes, MeshMode.TRIANGLE_STRIP, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 4, AttributeType.A32F, false);
                quad.addVertex(new Vector2f(-1f, -1));
                quad.addVertex(new Vector2f(1f, -1f));
                quad.addVertex(new Vector2f(-1f, 1f));
                quad.addVertex(new Vector2f(1f, 1f));

                Program p = new Program(new Module(330, TestDraw2f_SHADER));
                fb.setClearColor(Color.AntiqueWhite);
                fb.clear(true, false, false);
                fb.draw(p, quad);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawVector2f01_Screenshot", bmp);
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
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawVector2f01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestDrawLineStrip01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));
                Mesh<Vector2f, uint> quad = new Mesh<Vector2f, uint>(Vector2f.SizeInBytes, MeshMode.LINE_STRIP, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 4, AttributeType.A32F, false);
                quad.addVertex(new Vector2f(-1f, -1));
                quad.addVertex(new Vector2f(1f, -1f));
                quad.addVertex(new Vector2f(-1f, 1f));
                quad.addVertex(new Vector2f(1f, 1f));

                Program p = new Program(new Module(330, TestPrimitives_SHADER));
                fb.setClearColor(Color.White);
                fb.clear(true, false, false);
                fb.draw(p, quad);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawLineStrip01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
                if (quad != null)
                    quad.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot10.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawLineStrip01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestDrawLines01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));
                Mesh<Vector2f, uint> quad = new Mesh<Vector2f, uint>(Vector2f.SizeInBytes, MeshMode.LINES, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 4, AttributeType.A32F, false);
                quad.addVertex(new Vector2f(-1f, -1));
                quad.addVertex(new Vector2f(1f, -1f));
                quad.addVertex(new Vector2f(-1f, 1f));
                quad.addVertex(new Vector2f(1f, 1f));

                Program p = new Program(new Module(330, TestPrimitives_SHADER));
                fb.setClearColor(Color.White);
                fb.clear(true, false, false);
                fb.draw(p, quad);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawLines01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
                if (quad != null)
                    quad.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot09.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawLines01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestDrawLineLoop01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));
                Mesh<Vector2f, uint> quad = new Mesh<Vector2f, uint>(Vector2f.SizeInBytes, MeshMode.LINE_LOOP, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 4, AttributeType.A32F, false);
                quad.addVertex(new Vector2f(-1f, -1));
                quad.addVertex(new Vector2f(1f, -1f));
                quad.addVertex(new Vector2f(-1f, 1f));
                quad.addVertex(new Vector2f(1f, 1f));

                Program p = new Program(new Module(330, TestPrimitives_SHADER));
                fb.setClearColor(Color.White);
                fb.clear(true, false, false);
                fb.draw(p, quad);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawLineLoop01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
                if (quad != null)
                    quad.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot08.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawLineLoop01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestDrawTrinangleFan01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));
                Mesh<Vector2f, uint> quad = new Mesh<Vector2f, uint>(Vector2f.SizeInBytes, MeshMode.TRIANGLE_FAN, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 4, AttributeType.A32F, false);
                quad.addVertex(new Vector2f(-1f, -1));
                quad.addVertex(new Vector2f(1f, -1f));
                quad.addVertex(new Vector2f(1f, 1f));
                quad.addVertex(new Vector2f(-1f, 1f));

                Program p = new Program(new Module(330, TestDraw2f_SHADER));
                fb.setClearColor(Color.AntiqueWhite);
                fb.clear(true, false, false);
                fb.draw(p, quad);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawTrinangleFan01_Screenshot", bmp);
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
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawTrinangleFan01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestDrawTrinangleStrip01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));
                Mesh<Vector2f, uint> quad = new Mesh<Vector2f, uint>(Vector2f.SizeInBytes, MeshMode.TRIANGLE_STRIP, MeshUsage.GPU_STATIC);
                quad.addAttributeType(0, 4, AttributeType.A32F, false);
                quad.addVertex(new Vector2f(-1f, -1));
                quad.addVertex(new Vector2f(1f, -1f));
                quad.addVertex(new Vector2f(-1f, 1f));
                quad.addVertex(new Vector2f(1f, 1f));

                Program p = new Program(new Module(330, TestDraw2f_SHADER));
                fb.setClearColor(Color.AntiqueWhite);
                fb.clear(true, false, false);
                fb.draw(p, quad);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawTrinangleStrip01_Screenshot", bmp);
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
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawTrinangleStrip01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }
    }
}

