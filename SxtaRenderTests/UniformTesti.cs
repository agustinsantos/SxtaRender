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
    public class UniformTesti
    {
        private static GameWindow control = null;
        protected const int Width = 500;
        protected const int Height = 500;
        protected ImageComparator imageComparer = new ImageComparator(DiffOptions.IGNORE_NOTHING);
        protected const float epsilonError = 0.0001f;

        private static readonly string TESTSNAME = "UniformTesti";

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

        private static readonly string TestUniform1i_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition; 
        void main()
        {
            gl_Position = vec4(aPosition, 1.0);
        }
#endif
#ifdef _FRAGMENT_
        uniform int uC;
        out ivec4 FragColor;
        void main()
        {
            FragColor = ivec4(0.0, 0.0, uC, 1); 
        }
#endif";

        private static readonly string TestUniform2i_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition;    
        void main()
        {
            gl_Position = ivec4(aPosition, 1);
        }
#endif
#ifdef _FRAGMENT_
        uniform ivec2 uC;
        out ivec4 FragColor;
        void main()
        {
            FragColor = ivec4(uC, 0.0, 1); 
        }
#endif";

        private static readonly string TestUniform3i_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition; 
        void main()
        {
            gl_Position = vec4(aPosition, 1.0);
        }
#endif
#ifdef _FRAGMENT_
        uniform ivec3 uC;
        out ivec4 FragColor;
        void main()
        {
            FragColor = ivec4(uC, 1); 
        }
#endif";

        private static readonly string TestUniform4i_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition; 
        void main()
        {
            gl_Position = vec4(aPosition, 1.0);
        }
#endif
#ifdef _FRAGMENT_
        uniform ivec4 uC;
        out ivec4 FragColor;
        void main()
        {
            FragColor = ivec4(uC); 
        }
#endif";

        [TestMethod]
        public void TestUniform1i01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB32I, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(330, TestUniform1i_SHADER));
                p.getUniform1i("uC").set(255);
                fb.drawQuad(p);

                bmp = RenderTestUtils.GetScreenshot(Width, Height, OpenTK.Graphics.OpenGL.PixelFormat.BgrInteger);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform1i01_Screenshot", bmp);
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
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform1i01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestUniform1i02()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB32I, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(330, TestUniform1i_SHADER));
                p.getUniform1i("uC").set(0);
                fb.drawQuad(p);

                bmp = RenderTestUtils.GetScreenshot(Width, Height, OpenTK.Graphics.OpenGL.PixelFormat.BgrInteger);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform1i02_Screenshot", bmp);
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
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform1i02_ImageDiff", diffResult);
#endif
            Assert.AreEqual(100, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestUniform2i01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB32I, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(330, TestUniform2i_SHADER));
                p.getUniform2i("uC").set(new Sxta.Math.Vector2i((int)(0.2*255), (int)(0.8 * 255)));
                fb.drawQuad(p);

                bmp = RenderTestUtils.GetScreenshot(Width, Height, OpenTK.Graphics.OpenGL.PixelFormat.BgrInteger);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform2i01_Screenshot", bmp);
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
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform2i01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestUniform2i02()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB32I, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(330, TestUniform2i_SHADER));
                p.getUniform2i("uC").set(new Sxta.Math.Vector2i((int)(0.8 * 255), (int)(0.2 * 255)));
                fb.drawQuad(p);

                bmp = RenderTestUtils.GetScreenshot(Width, Height, OpenTK.Graphics.OpenGL.PixelFormat.BgrInteger);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform2i02_Screenshot", bmp);
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
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform2i02_ImageDiff", diffResult);
#endif
            Assert.AreEqual(100, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestUniform3i01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB32I, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(330, TestUniform3i_SHADER));
                p.getUniform3i("uC").set(new Sxta.Math.Vector3i((int)(0.2 * 255), (int)(0.4 * 255), (int)(0.6 * 255)));
                fb.drawQuad(p);

                bmp = RenderTestUtils.GetScreenshot(Width, Height, OpenTK.Graphics.OpenGL.PixelFormat.BgrInteger);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform3i01_Screenshot", bmp);
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
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform3i01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestUniform3i02()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB32I, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(330, TestUniform3i_SHADER));
                p.getUniform3i("uC").set(new Sxta.Math.Vector3i((int)(0.6 * 255), (int)(0.4 * 255), (int)(0.2 * 255)));
                fb.drawQuad(p);

                bmp = RenderTestUtils.GetScreenshot(Width, Height, OpenTK.Graphics.OpenGL.PixelFormat.BgrInteger);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform3i02_Screenshot", bmp);
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
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform3i02_ImageDiff", diffResult);
#endif
            Assert.AreEqual(100, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestUniform4i01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB32I, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(330, TestUniform4i_SHADER));
                p.getUniform4i("uC").set(new Sxta.Math.Vector4i((int)(0.2 * 255), (int)(0.4 * 255), (int)(0.6 * 255), 1));
                fb.drawQuad(p);

                bmp = RenderTestUtils.GetScreenshot(Width, Height, OpenTK.Graphics.OpenGL.PixelFormat.BgrInteger);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform4i01_Screenshot", bmp);
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
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform4i01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestUniform4i02()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB32I, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(330, TestUniform4i_SHADER));
                p.getUniform4i("uC").set(new Sxta.Math.Vector4i((int)(0.6 * 255), (int)(0.4 * 255), (int)(0.2 * 255), 0));
                fb.drawQuad(p);

                bmp = RenderTestUtils.GetScreenshot(Width, Height, OpenTK.Graphics.OpenGL.PixelFormat.BgrInteger);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform4i02_Screenshot", bmp);
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
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestUniform4i02_ImageDiff", diffResult);
#endif
            Assert.AreEqual(100, dissimilarity, epsilonError);
        }
    }
}
