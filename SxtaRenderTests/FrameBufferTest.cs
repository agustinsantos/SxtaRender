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
    public class FrameBufferTest
    {
        private static GameWindow control = null;
        protected const int Width = 500;
        protected const int Height = 500;
        protected ImageComparator imageComparer = new ImageComparator(DiffOptions.IGNORE_NOTHING);
        protected const float epsilonError = 0.0001f;

        private static readonly string TESTSNAME = "FrameBufferTest";

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

        [TestMethod]
        public void TestClearColor01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));
                fb.setClearColor(Color.Blue);
                fb.clear(true, false, false);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestClearColor01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot01.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestClearColor01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }


        [TestMethod]
        public void TestClearColor02()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));
                fb.setClearColor(Color.Brown);
                fb.clear(true, false, false);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestClearColor02_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/Screenshot01.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestClearColor02_ImageDiff", diffResult);
#endif
            Assert.AreEqual(100, dissimilarity, epsilonError);
        }
    }
}
