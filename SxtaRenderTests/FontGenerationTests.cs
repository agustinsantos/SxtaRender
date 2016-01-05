#define SAVE_RESULTS
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK;
using OpenTK.Graphics;
using Sxta.Math;
using Sxta.Render;
using Sxta.TestTools.ImageTesting;
using SxtaRender.Fonts;
using SxtaRenderTests.TestTools;
using System.Drawing;
using System.Drawing.Text;

namespace SxtaRenderTests
{
    [TestClass]
    public class FontGenerationTests
    {
        private static GameWindow control = null;
        protected const int Width = 500;
        protected const int Height = 500;
        protected ImageComparator imageComparer = new ImageComparator(DiffOptions.STRICT);
        protected const float epsilonError = 0.0001f;

        private static readonly string TESTSNAME = "FontGenerationTests";

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
        public void TestFontGenerator01()
        {
            Font font = new Font(FontFamily.GenericSansSerif, 64, FontStyle.Regular);
            FontGenerationConfig config = new FontGenerationConfig();
            config.RenderHint = TextRenderingHint.AntiAliasGridFit;
            config.PageWidth = 0;
            config.PageHeight = 0;
            config.GlyphMargin = 4;
            config.ForcePowerOfTwo = true;
            TextureFont textFont = TextureFont.CreateNew(font, config);
            textFont.Save("GenericSansSerif-Regular-32");

            TextureFont textFont2 = TextureFont.FromFile("GenericSansSerif-Regular-32");
        }

        [TestMethod]
        public void TestFontGenerator02()
        {
            Font font = new Font(FontFamily.GenericSansSerif, 64, FontStyle.Regular);
            FontGenerationConfig config = new FontGenerationConfig();
            config.RenderHint = TextRenderingHint.SingleBitPerPixel;
            config.PageWidth = 1024;
            config.PageHeight = 1024;
            config.GlyphMargin = 4;

            TextureFont textFont = TextureFont.CreateNew(font, config);
            textFont.Save("GenericSansSerif-Regular-64");

            TextureFont textFont2 = TextureFont.FromFile("GenericSansSerif-Regular-64");
        }

        [TestMethod]
        public void TestDrawText01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));
                fb.setClearColor(Color.White);
                fb.setBlend(true, BlendEquation.ADD, BlendArgument.SRC_ALPHA, BlendArgument.ONE_MINUS_SRC_ALPHA);
                fb.clear(true, false, false);

                TextureFont textFont = TextureFont.FromFile("GenericSansSerif-Regular-32");
                TextRenderer renderer = new TextRenderer(fb, textFont, new FontRenderOptions());
                renderer.Init();
                renderer.SetViewport(fb.getViewport());
                //string sourcestring = "hi!\r\nI am  \r    a\twonderful56 text... \r\nyeah...";
                var text = renderer.ProcessText("Hello World!\rNew line..\rAnother line.\rBye, bye....", new SizeF(500, 500), FontAlignment.ALIGN_LEFT);
                renderer.PreRenderText(20, 450, text);
                renderer.Draw();

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestDrawText01_Screenshot", bmp);
#endif
                if (renderer != null)
                    renderer.Dispose();
                if (rb != null)
                    rb.Dispose();
            }
        }
    }
}


