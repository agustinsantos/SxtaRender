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
    public class UniformTestd
    {
        private static GameWindow control = null;
        protected const int Width = 500;
        protected const int Height = 500;
        protected ImageComparator imageComparer = new ImageComparator(DiffOptions.IGNORE_NOTHING);
        protected const float epsilonError = 0.0001f;

        private static readonly string TESTSNAME = "UniformTestd";

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

        private static readonly string TestUniform1d_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition; 
        void main()
        {
            gl_Position = vec4(aPosition, 1.0);
        }
#endif
#ifdef _FRAGMENT_
        uniform double uC;
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(0.0, 0.0, float(uC), 1.0); 
        }
#endif";

        private static readonly string TestUniform2d_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition;    
        void main()
        {
            gl_Position = vec4(aPosition, 1.0);
        }
#endif
#ifdef _FRAGMENT_
        uniform dvec2 uC;
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(uC, 0.0, 1.0); 
        }
#endif";

        private static readonly string TestUniform3d_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition; 
        void main()
        {
            gl_Position = vec4(aPosition, 1.0);
        }
#endif
#ifdef _FRAGMENT_
        uniform dvec3 uC;
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(uC, 1.0); 
        }
#endif";

        private static readonly string TestUniform4d_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition; 
        void main()
        {
            gl_Position = vec4(aPosition, 1.0);
        }
#endif
#ifdef _FRAGMENT_
        uniform dvec4 uC;
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(uC); 
        }
#endif";

        [TestMethod]
        public void TestUniform1d01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(400, TestUniform1d_SHADER));
                p.getUniform1d("uC").set(1.0);
                fb.drawQuad(p);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform1d01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot01.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform1d01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestUniform1d02()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(400, TestUniform1d_SHADER));
                p.getUniform1d("uC").set(0.5);
                fb.drawQuad(p);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform1d02_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot01.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform1d02_ImageDiff", diffResult);
#endif
            Assert.AreEqual(100, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestUniform2d01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(400, TestUniform2d_SHADER));
                p.getUniform2d("uC").set(new Sxta.Math.Vector2d(0.2, 0.8));
                fb.drawQuad(p);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform2d01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot02.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform2d01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }
        [TestMethod]
        public void TestUniform2d02()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(400, TestUniform2d_SHADER));
                p.getUniform2d("uC").set(new Sxta.Math.Vector2d(0.8, 0.2));
                fb.drawQuad(p);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform2d02_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot02.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform2d02_ImageDiff", diffResult);
#endif
            Assert.AreEqual(100, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestUniform3d01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(400, TestUniform3d_SHADER));
                p.getUniform3d("uC").set(new Sxta.Math.Vector3d(0.2, 0.4, 0.6));
                fb.drawQuad(p);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform3d01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot03.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform3d01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestUniform3d02()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(400, TestUniform3d_SHADER));
                p.getUniform3d("uC").set(new Sxta.Math.Vector3d(0.6, 0.4, 0.2));
                fb.drawQuad(p);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform3d02_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot03.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform3d02_ImageDiff", diffResult);
#endif
            Assert.AreEqual(100, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestUniform4d01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(400, TestUniform4d_SHADER));
                p.getUniform4d("uC").set(new Sxta.Math.Vector4d(0.2, 0.4, 0.6, 1.0));
                fb.drawQuad(p);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform4d01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot03.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform4d01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestUniform4d02()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(400, TestUniform4d_SHADER));
                p.getUniform4d("uC").set(new Sxta.Math.Vector4d(0.6, 0.4, 0.2, 0.01));
                fb.drawQuad(p);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform4d02_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot03.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform4d02_ImageDiff", diffResult);
#endif
            Assert.AreEqual(100, dissimilarity, epsilonError);
        }
    }
}
