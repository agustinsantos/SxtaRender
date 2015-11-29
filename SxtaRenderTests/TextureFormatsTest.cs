#define SAVE_RESULTS
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK;
using OpenTK.Graphics;
using Sxta.Math;
using Sxta.Render;
using Sxta.TestTools.ImageTesting;
using SxtaRenderTests.TestTools;
using System.Drawing;
using System.Drawing.Imaging;

namespace SxtaRenderTests
{
    [TestClass]
    public class TextureFormatsTest
    {
        private static GameWindow control = null;
        protected const int Width = 500;
        protected const int Height = 500;
        protected ImageComparator imageComparer = new ImageComparator(DiffOptions.IGNORE_ANTIALIASING);
        protected const float epsilonError = 0.0001f;

        private static readonly string TESTSNAME = "TextureFormatsTest";

        private struct Vertex_V3T2f
        {
            public Vector3f Position;
            public Vector2f TexCoord;
            public static int SizeInBytes
            {
                get { return Vector2f.SizeInBytes + Vector3f.SizeInBytes; }
            }
        }

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

        private static readonly string TestTextureQuad_SHADER = @"
#ifdef _VERTEX_
        layout(location = 0) in vec3 aPosition;
        layout(location = 1) in vec2 aTexCoord;

        out vec2 TexCoord0;

        void main()
        {
            gl_Position = vec4(aPosition, 1.0);
            TexCoord0 = aTexCoord;
        }
#endif
#ifdef _FRAGMENT_
        in vec2 TexCoord0;
        out vec4 FragColor;
        uniform sampler2D gSampler;
 
        void main()
        {
            FragColor = texture2D(gSampler, TexCoord0.xy);
        }
#endif";

        private Texture CreateTexture(Bitmap img)
        {
            TextureInternalFormat pif;
            TextureFormat pf;
            Sxta.Render.PixelType pt;
            int size;
            EnumConversion.ConvertPixelFormat(img.PixelFormat, out pif, out pf, out pt, out size);
            img.RotateFlip(RotateFlipType.RotateNoneFlipY);
            BitmapData Data = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
            GPUBuffer buff = new GPUBuffer();
            buff.setData(Data.Width * Data.Height * size, Data.Scan0, BufferUsage.STATIC_DRAW);
            img.UnlockBits(Data);
            Texture.Parameters @params = new Texture.Parameters();
            Sxta.Render.Buffer.Parameters s = new Sxta.Render.Buffer.Parameters();
            Texture texture = new Texture2D(img.Width, img.Height, pif, pf, pt, @params, s, buff);
            buff.Dispose();
            return texture;
        }

        public void DoTest(string imgFile, string testName, string refImage = "Resources/ControlImages/Test01_Screenshot.bmp")
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(330, TestTextureQuad_SHADER));
                Bitmap texture = new Bitmap(imgFile);
                Texture tex = CreateTexture(texture);
                p.getUniformSampler("gSampler").set(tex);
                Mesh<Vertex_V3T2f, uint> quad = new Mesh<Vertex_V3T2f, uint>(Vertex_V3T2f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLE_STRIP, MeshUsage.GPU_STATIC, 4);
                quad.addAttributeType(0, 3, AttributeType.A32F, false);
                quad.addAttributeType(1, 2, AttributeType.A32F, false);
                quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(-0.5f, -0.5f, 0), TexCoord = new Vector2f(0, 0) });
                quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(0.5f, -0.5f, 0), TexCoord = new Vector2f(1, 0) });
                quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(-0.5f, 0.5f, 0), TexCoord = new Vector2f(0, 1) });
                quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(0.5f, 0.5f, 0), TexCoord = new Vector2f(1, 1) });

                fb.setClearColor(Color.White);
                fb.clear(true, false, false);
                fb.draw(p, quad);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, testName + "_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
                if (quad != null)
                    quad.Dispose();
                if (tex != null)
                    tex.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile(refImage);
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, testName + "_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestDrawTexture_BMP_32BppBGR_01()
        {
            DoTest("Resources/ImageFormat/BMP/test_32bppBGR.bmp", "TestDrawTexture_BMP_32BppBGR_01");
        }

        [TestMethod]
        public void TestDrawTexture_BMP_24BppBGR_01()
        {
            DoTest("Resources/ImageFormat/BMP/test_24bppBGR.bmp", "TestDrawTexture_BMP_24BppBGR_01");
        }

        [TestMethod]
        public void TestDrawTexture_BMP_16BppBGR565_01()
        {
            DoTest("Resources/ImageFormat/BMP/test_16bppBGR565.bmp", "TestDrawTexture_BMP_16BppBGR565_01");
        }

        [TestMethod]
        public void TestDrawTexture_BMP_16BppBGR555_01()
        {
            DoTest("Resources/ImageFormat/BMP/test_16bppBGR555.bmp", "TestDrawTexture_BMP_16BppBGR555_01");
        }

        [TestMethod]
        public void TestDrawTexture_BMP_8BppIndexed_01()
        {
            // TODO. Indexed format is not implemented yet.
            DoTest("Resources/ImageFormat/BMP/test_8bppIndexed.bmp", "TestDrawTexture_BMP_8BppIndexed_01");
        }

        [TestMethod]
        public void TestDrawTexture_PNG_32BppBGRA_01()
        {
            DoTest("Resources/ImageFormat/PNG/test_32bppBGRA.png", "TestDrawTexture_PNG_32BppBGRA_01");
        }

        [TestMethod]
        public void TestDrawTexture_PNG_24BppBGR_01()
        {
            DoTest("Resources/ImageFormat/PNG/test_24bppBGR.png", "TestDrawTexture_PNG_24BppBGR_01");
        }

        [TestMethod]
        public void TestDrawTexture_JPG_24BppBGR_01()
        {
            DoTest("Resources/ImageFormat/JPG/test_24bppBGR.jpg", "TestDrawTexture_JPG_24BppBGR_01");
        }

        [TestMethod]
        public void TestDrawTexture_GIF_8BppIndexed_01()
        {
            // TODO. Indexed format is not implemented yet.
            DoTest("Resources/ImageFormat/GIF/test_8bppIndexed.gif", "TestDrawTexture_GIF_8BppIndexed_01");
        }

        [TestMethod]
        public void TestDrawTexture_TIF_32BppBGRA_NOCOMPRESS_01()
        {
            DoTest("Resources/ImageFormat/TIF/test_NC_32bppBGRA.tif", "TestDrawTexture_TIF_32BppBGRA_NOCOMPRESS_01");
        }

        [TestMethod]
        public void TestDrawTexture_TIF_24BppBGR_NOCOMPRESS_01()
        {
            DoTest("Resources/ImageFormat/TIF/test_NC_24bppBGR.tif", "TestDrawTexture_TIF_24BppBGR_NOCOMPRESS_01");
        }

        [TestMethod]
        public void TestDrawTexture_TIF_32BppCMYK_NOCOMPRESS_01()
        {
            // TODO. CMYK format is not implemented yet.
            DoTest("Resources/ImageFormat/TIF/test_NC_32bppCMYK.tif", "TestDrawTexture_TIF_32BppCMYK_NOCOMPRESS_01");
        }
    }
}