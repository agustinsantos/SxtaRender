#define SAVE_RESULTS
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Sxta.Math;
using Sxta.Render;
using Sxta.Render.OpenGLExt;
using Sxta.TestTools.ImageTesting;
using SxtaRenderTests.TestTools;
using System.Drawing;

namespace SxtaRenderTests
{
    [TestClass]
    public class ImageKTXTest
    {
        private static GameWindow control = null;
        protected const int Width = 500;
        protected const int Height = 500;
        protected ImageComparator imageComparer = new ImageComparator(DiffOptions.IGNORE_ANTIALIASING);
        protected const float epsilonError = 0.0004f;

        private static readonly string TESTSNAME = "ImageKTXTest";
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

        private TextureKTX CreateTexture(ImageKTX imgKTX, Texture.Parameters params_ = null)
        {
            TextureKTX texture = imgKTX.BuildTexture(params_);
            return texture;
        }
        public void DoTest(ImageKTX imgKTX, string testName, string refImage, Texture.Parameters params_ = null)
        {
            Bitmap bmp;
            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(330, TestTextureQuad_SHADER));
                Texture tex = CreateTexture(imgKTX, params_);
                p.getUniformSampler("gSampler").set(tex);
                Mesh<Vertex_V3T2f, uint> quad = new Mesh<Vertex_V3T2f, uint>(Vertex_V3T2f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLE_STRIP, MeshUsage.GPU_STATIC, 4);
                quad.addAttributeType(0, 3, AttributeType.A32F, false);
                quad.addAttributeType(1, 2, AttributeType.A32F, false);
                quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(-0.9f, -0.9f, 0), TexCoord = new Vector2f(0, 1) });
                quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(0.9f, -0.9f, 0), TexCoord = new Vector2f(1, 1) });
                quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(-0.9f, 0.9f, 0), TexCoord = new Vector2f(0, 0) });
                quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(0.9f, 0.9f, 0), TexCoord = new Vector2f(1, 0) });

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
        public void TestImageKTX_rgb8_srgb()
        {
            ImageKTX ktxImg = ImageKTX.LoadTexture("Resources/ImageFormat/KTX/kueken7_rgb8_srgb.ktx");
 
            Assert.AreEqual(256, ktxImg.Width);
            Assert.AreEqual(256, ktxImg.Height);
            Assert.AreEqual(0, ktxImg.Depth);
            Assert.AreEqual(2u, ktxImg.TextureDimensions);

            Assert.AreEqual(OpenTK.Graphics.OpenGL.PixelType.UnsignedByte, ktxImg.PixelType);
            Assert.AreEqual(TextureTarget.Texture2D, ktxImg.TextureTarget);

            Assert.AreEqual(0, ktxImg.NumberOfArrayElements);
            Assert.AreEqual(1, ktxImg.NumberOfFaces);
            Assert.AreEqual(9, ktxImg.NumberOfMipmapLevels);

            Assert.AreEqual(1, ktxImg.KeyValueData.Count);
            Assert.AreEqual("KTXOrientation", ktxImg.KeyValueData[0].Item1);
            Assert.AreEqual("S=r,T=d,R=i", ktxImg.KeyValueData[0].Item2);

            DoTest(ktxImg, "TestImageKTX_rgb8_srgb", "Resources/ControlImages/KTXImage_rgb8_srgb_Screenshot.bmp");
        }

        [TestMethod]
        public void TestImageKTX_rgb_dxt1_srgb()
        {
            ImageKTX ktxImg = ImageKTX.LoadTexture("Resources/ImageFormat/KTX/kueken7_rgb_dxt1_srgb.ktx");

            Assert.AreEqual(256, ktxImg.Width);
            Assert.AreEqual(256, ktxImg.Height);
            Assert.AreEqual(0, ktxImg.Depth);
            Assert.AreEqual(2u, ktxImg.TextureDimensions);

            Assert.AreEqual(TextureTarget.Texture2D, ktxImg.TextureTarget);

            Assert.AreEqual(0, ktxImg.NumberOfArrayElements);
            Assert.AreEqual(1, ktxImg.NumberOfFaces);
            Assert.AreEqual(9, ktxImg.NumberOfMipmapLevels);

            DoTest(ktxImg, "TestImageKTX_rgb_dxt1_srgb", "Resources/ControlImages/KTXImage_rgb8_srgb_Screenshot.bmp");
        }

        [TestMethod]
        public void TestImageKTX_rgb_etc2_srgb()
        {
            ImageKTX ktxImg = ImageKTX.LoadTexture("Resources/ImageFormat/KTX/kueken7_rgb_etc2_srgb.ktx");

            Assert.AreEqual(256, ktxImg.Width);
            Assert.AreEqual(256, ktxImg.Height);
            Assert.AreEqual(0, ktxImg.Depth);
            Assert.AreEqual(2u, ktxImg.TextureDimensions);

            Assert.AreEqual(TextureTarget.Texture2D, ktxImg.TextureTarget);

            Assert.AreEqual(0, ktxImg.NumberOfArrayElements);
            Assert.AreEqual(1, ktxImg.NumberOfFaces);
            Assert.AreEqual(9, ktxImg.NumberOfMipmapLevels);

            DoTest(ktxImg, "TestImageKTX_rgb_etc2_srgb", "Resources/ControlImages/KTXImage_rgb8_srgb_Screenshot.bmp");
        }

        [TestMethod]
        public void TestImageKTX_rgba16_sfloat()
        {
            ImageKTX ktxImg = ImageKTX.LoadTexture("Resources/ImageFormat/KTX/kueken7_rgba16_sfloat.ktx");

            Assert.AreEqual(256, ktxImg.Width);
            Assert.AreEqual(256, ktxImg.Height);
            Assert.AreEqual(0, ktxImg.Depth);
            Assert.AreEqual(2u, ktxImg.TextureDimensions);

            Assert.AreEqual(TextureTarget.Texture2D, ktxImg.TextureTarget);

            Assert.AreEqual(0, ktxImg.NumberOfArrayElements);
            Assert.AreEqual(1, ktxImg.NumberOfFaces);
            Assert.AreEqual(9, ktxImg.NumberOfMipmapLevels);

            DoTest(ktxImg, "TestImageKTX_rgba16_sfloat", "Resources/ControlImages/KTXImage_rgb8_srgb_Screenshot.bmp");
        }

        [TestMethod]
        public void TestImageKTX_etc2_RGB()
        {
            ImageKTX ktxImg = ImageKTX.LoadTexture("Resources/ImageFormat/KTX/Lenna_etc2_RGB.ktx");

            Assert.AreEqual(512, ktxImg.Width);
            Assert.AreEqual(512, ktxImg.Height);
            Assert.AreEqual(0, ktxImg.Depth);
            Assert.AreEqual(2u, ktxImg.TextureDimensions);

            Assert.AreEqual(TextureTarget.Texture2D, ktxImg.TextureTarget);

            Assert.AreEqual(0, ktxImg.NumberOfArrayElements);
            Assert.AreEqual(1, ktxImg.NumberOfFaces);
            Assert.AreEqual(1, ktxImg.NumberOfMipmapLevels);

            DoTest(ktxImg, "TestImageKTX_etc2_RGB", "Resources/ControlImages/Lena01_Screenshot.bmp");
        }
   }
}
