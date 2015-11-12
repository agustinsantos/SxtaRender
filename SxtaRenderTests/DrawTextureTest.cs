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
    public class DrawTextureTest
    {
        private static GameWindow control = null;
        protected const int Width = 500;
        protected const int Height = 500;
        protected ImageComparator imageComparer = new ImageComparator(DiffOptions.IGNORE_ANTIALIASING);
        protected const float epsilonError = 0.0001f;

        private static readonly string TESTSNAME = "DrawTextureTest";

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

        private Texture CreateTexture(Bitmap img, Texture.Parameters params_)
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
            if (params_ == null) params_ = new Texture.Parameters();
            Sxta.Render.Buffer.Parameters s = new Sxta.Render.Buffer.Parameters();
            Texture texture = new Texture2D(img.Width, img.Height, pif, pf, pt, params_, s, buff);
            buff.Dispose();
            return texture;
        }

        public void DoTest(string refImage, string testName, float repetition = 1, Texture.Parameters params_ = null)
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(330, TestTextureQuad_SHADER));
                Bitmap texture = new Bitmap("Resources/Lenna.png");
                Texture tex = CreateTexture(texture, params_);
                p.getUniformSampler("gSampler").set(tex);
                Mesh<Vertex_V3T2f, uint> quad = new Mesh<Vertex_V3T2f, uint>(Vertex_V3T2f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLE_STRIP, MeshUsage.GPU_STATIC, 4);
                quad.addAttributeType(0, 3, AttributeType.A32F, false);
                quad.addAttributeType(1, 2, AttributeType.A32F, false);
                quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(-0.9f, -0.9f, 0), TexCoord = new Vector2f(0, 0) });
                quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(0.9f, -0.9f, 0), TexCoord = new Vector2f(repetition, 0) });
                quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(-0.9f, 0.9f, 0), TexCoord = new Vector2f(0, repetition) });
                quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(0.9f, 0.9f, 0), TexCoord = new Vector2f(repetition, repetition) });

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
        public void TestDrawTextureDefaultParams01()
        {
            DoTest("Resources/ControlImages/Lena01_Screenshot.bmp", "TestDrawTextureDefaultParams01");
        }

        [TestMethod]
        public void TestDrawTextureParams01()
        {
            Texture.Parameters params_ = new Texture.Parameters();
            params_.wrapR(TextureWrap.MIRRORED_REPEAT);
            params_.wrapS(TextureWrap.MIRRORED_REPEAT);
            params_.wrapT(TextureWrap.MIRRORED_REPEAT);
            DoTest("Resources/ControlImages/Lena02_Screenshot.bmp", "TestDrawTextureParams01", 2, params_);
        }

        [TestMethod]
        public void TestDrawTextureParams02()
        {
            Texture.Parameters params_ = new Texture.Parameters();
            params_.wrapR(TextureWrap.REPEAT);
            params_.wrapS(TextureWrap.REPEAT);
            params_.wrapT(TextureWrap.REPEAT);
            DoTest("Resources/ControlImages/Lena03_Screenshot.bmp", "TestDrawTextureParams02", 2, params_);
        }

        [TestMethod]
        public void TestDrawTextureParams03()
        {
            Texture.Parameters params_ = new Texture.Parameters();
            params_.min(TextureFilter.LINEAR);
            params_.mag(TextureFilter.LINEAR);
            DoTest("Resources/ControlImages/Lena04_Screenshot.bmp", "TestDrawTextureParams03", 0.1f, params_);
        }

        [TestMethod]
        public void TestDrawTextureParams04()
        {
            Texture.Parameters params_ = new Texture.Parameters();
            params_.min(TextureFilter.NEAREST);
            params_.mag(TextureFilter.NEAREST);
            DoTest("Resources/ControlImages/Lena05_Screenshot.bmp", "TestDrawTextureParams04", 0.1f, params_);
        }
    }
}


