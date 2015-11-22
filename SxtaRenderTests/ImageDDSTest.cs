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
    /// <summary>
    /// Test DDS Image loading and Texture generation.
    /// 
    /// Possible scenarios:
    /// 
    /// Compression: BC1 / DTX1,
    /// Compression: BC2 / DTX3,
    /// Compression: BC3 / DTX5,
    /// Compression: BC3n / DTX5n,
    /// Compression: BC4 / ATI1,
    /// Compression: BC5 / ATI2,
    /// Compression: None, Internal Format: RGB8
    /// Compression: None, Internal Format: RGBA8
    /// Compression: None, Internal Format: BGR8
    /// Compression: None, Internal Format: ABGR8
    /// Compression: None, Internal Format: R5G6B5
    /// Compression: None, Internal Format: RGBA4
    /// Compression: None, Internal Format: RGB5A1
    /// Compression: None, Internal Format: RGB10A2
    /// Compression: None, Internal Format: R3G3B2
    /// </summary>
    [TestClass]
    public class ImageDDSTest
    {
        private static GameWindow control = null;
        protected const int Width = 500;
        protected const int Height = 500;
        protected ImageComparator imageComparer = new ImageComparator(DiffOptions.IGNORE_ANTIALIASING);
        protected const float epsilonError = 0.001f;

        private static readonly string TESTSNAME = "ImageDDSTest";
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

        public void DoTest(ImageDDS imgDDS, string testName, string refImage, float threshold = 0, Texture.Parameters params_ = null)
        {
            Bitmap bmp;
            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(330, TestTextureQuad_SHADER));
                Texture tex = imgDDS.BuildTexture(params_);
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
            Assert.AreEqual(threshold, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestImageDDS_bc1_unorm()
        {
            ImageDDS ddsImg = ImageDDS.LoadFromFile("Resources/ImageFormat/DDS/Lenna_bc1_unorm.dds");
            Assert.AreEqual(512, ddsImg.Width);
            Assert.AreEqual(512, ddsImg.Height);
            Assert.AreEqual(1, ddsImg.Depth);

            Assert.AreEqual(TextureTarget.Texture2D, ddsImg.TextureTarget);

            Assert.AreEqual(1, ddsImg.NumberOfMipmapLevels);
            Assert.IsTrue(ddsImg.IsCompressed);
            Assert.AreEqual(PixelInternalFormat.CompressedRgbS3tcDxt1Ext, ddsImg.InternalFormat);

            DoTest(ddsImg, "TestImageDDS_bc1_unorm", "Resources/ControlImages/Lena01_Screenshot.bmp");
        }

        [TestMethod]
        public void TestImageDDS_bc1_unorm_srgbb()
        {
            ImageDDS ddsImg = ImageDDS.LoadFromFile("Resources/ImageFormat/DDS/Lenna_bc1_unorm_srgb.dds");
            Assert.AreEqual(512, ddsImg.Width);
            Assert.AreEqual(512, ddsImg.Height);
            Assert.AreEqual(1, ddsImg.Depth);

            Assert.AreEqual(TextureTarget.Texture2D, ddsImg.TextureTarget);

            Assert.AreEqual(1, ddsImg.NumberOfMipmapLevels);
            Assert.IsTrue(ddsImg.IsCompressed);
            Assert.AreEqual(PixelInternalFormat.CompressedSrgbS3tcDxt1Ext, ddsImg.InternalFormat);

            DoTest(ddsImg, "TestImageDDS_bc1_unorm_srgb", "Resources/ControlImages/Lena01_Screenshot.bmp");
        }

        [TestMethod]
        public void TestImageDDS_bc2_unorm()
        {
            ImageDDS ddsImg = ImageDDS.LoadFromFile("Resources/ImageFormat/DDS/Lenna_bc2_unorm.dds");
            Assert.AreEqual(512, ddsImg.Width);
            Assert.AreEqual(512, ddsImg.Height);
            Assert.AreEqual(1, ddsImg.Depth);

            Assert.AreEqual(TextureTarget.Texture2D, ddsImg.TextureTarget);

            Assert.AreEqual(1, ddsImg.NumberOfMipmapLevels);
            Assert.IsTrue(ddsImg.IsCompressed);
            Assert.AreEqual(PixelInternalFormat.CompressedRgbaS3tcDxt3Ext, ddsImg.InternalFormat);

            DoTest(ddsImg, "TestImageDDS_bc2_unorm", "Resources/ControlImages/Lena01_Screenshot.bmp");
        }

        [TestMethod]
        public void TestImageDDS_bc2_unorm_srgb()
        {
            ImageDDS ddsImg = ImageDDS.LoadFromFile("Resources/ImageFormat/DDS/Lenna_bc2_unorm_srgb.dds");
            Assert.AreEqual(512, ddsImg.Width);
            Assert.AreEqual(512, ddsImg.Height);
            Assert.AreEqual(1, ddsImg.Depth);

            Assert.AreEqual(TextureTarget.Texture2D, ddsImg.TextureTarget);

            Assert.AreEqual(1, ddsImg.NumberOfMipmapLevels);
            Assert.IsTrue(ddsImg.IsCompressed);
            Assert.AreEqual(PixelInternalFormat.CompressedSrgbAlphaS3tcDxt3Ext, ddsImg.InternalFormat);

            DoTest(ddsImg, "TestImageDDS_bc2_unorm_srgb", "Resources/ControlImages/Lena01_Screenshot.bmp");
        }

        [TestMethod]
        public void TestImageDDS_bc3_unorm()
        {
            ImageDDS ddsImg = ImageDDS.LoadFromFile("Resources/ImageFormat/DDS/Lenna_bc3_unorm.dds");
            Assert.AreEqual(512, ddsImg.Width);
            Assert.AreEqual(512, ddsImg.Height);
            Assert.AreEqual(1, ddsImg.Depth);

            Assert.AreEqual(TextureTarget.Texture2D, ddsImg.TextureTarget);

            Assert.AreEqual(1, ddsImg.NumberOfMipmapLevels);
            Assert.IsTrue(ddsImg.IsCompressed);
            Assert.AreEqual(PixelInternalFormat.CompressedRgbaS3tcDxt5Ext, ddsImg.InternalFormat);

            DoTest(ddsImg, "TestImageDDS_bc3_unorm", "Resources/ControlImages/Lena01_Screenshot.bmp");
        }

        [TestMethod]
        public void TestImageDDS_bc3_unorm_srgb()
        {
            ImageDDS ddsImg = ImageDDS.LoadFromFile("Resources/ImageFormat/DDS/Lenna_bc3_unorm_srgb.dds");
            Assert.AreEqual(512, ddsImg.Width);
            Assert.AreEqual(512, ddsImg.Height);
            Assert.AreEqual(1, ddsImg.Depth);

            Assert.AreEqual(TextureTarget.Texture2D, ddsImg.TextureTarget);

            Assert.AreEqual(1, ddsImg.NumberOfMipmapLevels);
            Assert.IsTrue(ddsImg.IsCompressed);
            Assert.AreEqual(PixelInternalFormat.CompressedSrgbAlphaS3tcDxt5Ext, ddsImg.InternalFormat);

            DoTest(ddsImg, "TestImageDDS_bc3_unorm_srgb", "Resources/ControlImages/Lena01_Screenshot.bmp");
        }
    }
}
