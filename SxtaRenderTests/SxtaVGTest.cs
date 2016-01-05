#define SAVE_RESULTS
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NanoVG;
using OpenTK;
using OpenTK.Graphics;
using Sxta.Math;
using Sxta.Render;
using Sxta.TestTools.ImageTesting;
using SxtaRenderTests.TestTools;
using System;
using System.Drawing;

namespace SxtaRenderTests
{
    [TestClass]
    public class SxtaVGTest
    {
        private static GameWindow control = null;
        protected const int Width = 500;
        protected const int Height = 500;
        protected ImageComparator imageComparer = new ImageComparator(DiffOptions.IGNORE_NOTHING);
        protected const float epsilonError = 0.0001f;

        private static readonly string TESTSNAME = "SxtaVGTest";

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            control = new GameWindow();
            //RenderTestUtils.DeleteResultDir(TESTSNAME);
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            if (control != null)
                control.Dispose();
        }

        //private 

        private static readonly string Test01_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec2 aPosition;
        layout (location = 1) in vec2 aTexCoord;
        out vec2 ftcoord;
        out vec2 fpos;
        uniform vec2 uViewSize;

        void main(void)
        {
            ftcoord = aTexCoord;
            fpos = aPosition;
            gl_Position = vec4(2.0 * aPosition.x / uViewSize.x - 1.0, 1.0 - 2.0 * aPosition.y / uViewSize.y, 0, 1);
        }
#endif
#ifdef _FRAGMENT_
            precision highp float;
            in vec2 ftcoord; 
            in vec2 fpos; 
            out vec4 outColor; 
            uniform sampler2D tex; 
            layout(std140) uniform frag {
				mat3 scissorMat;
				mat3 paintMat;
				vec4 innerCol;
				vec4 outerCol;
				vec2 scissorExt;
				vec2 scissorScale;
				vec2 extent;
				float radius;
				float feather;
				float strokeMult;
				float strokeThr;
				int texType;
				int type;
			};
		float sdroundrect(vec2 pt, vec2 ext, float rad) {
			vec2 ext2 = ext - vec2(rad,rad);
			vec2 d = abs(pt) - ext2;
			return min(max(d.x,d.y),0.0) + length(max(d,0.0)) - rad;
		}
		
		// Scissoring
		float scissorMask(vec2 p) {
			vec2 sc = (abs((scissorMat * vec3(p,1.0)).xy) - scissorExt);
			sc = vec2(0.5,0.5) - sc * scissorScale;
			return clamp(sc.x,0.0,1.0) * clamp(sc.y,0.0,1.0);
		}
		#ifdef EDGE_AA
		// Stroke - from [0..1] to clipped pyramid, where the slope is 1px.
		float strokeMask() {
			return min(1.0, (1.0-abs(ftcoord.x*2.0-1.0))*strokeMult) * min(1.0, ftcoord.y);
		}
		#endif
		
        void main()
        {
		    vec4 result;
			float scissor = scissorMask(fpos);
		#ifdef EDGE_AA
			float strokeAlpha = strokeMask();
		#else
			float strokeAlpha = 1.0;
		#endif
			if (type == 0) {			// Gradient
				// Calculate gradient color using box gradient
				vec2 pt = (paintMat * vec3(fpos,1.0)).xy;
				float d = clamp((sdroundrect(pt, extent, radius) + feather*0.5) / feather, 0.0, 1.0);
				vec4 color = mix(innerCol,outerCol,d);
				// Combine alpha
				color *= strokeAlpha * scissor;
				result = color;
			} else if (type == 1) {		// Image
				// Calculate color fron texture
				vec2 pt = (paintMat * vec3(fpos,1.0)).xy / extent;
				vec4 color = texture(tex, pt);
				if (texType == 1) color = vec4(color.xyz*color.w,color.w);
				if (texType == 2) color = vec4(color.x);
				// Apply color tint and alpha.
				color *= innerCol;
				// Combine alpha
				color *= strokeAlpha * scissor;
				result = color;
			} else if (type == 2) {		// Stencil fill
				result = vec4(1,1,1,1);
			} else if (type == 3) {		// Textured tris
				vec4 color = texture(tex, ftcoord);
				if (texType == 1) color = vec4(color.xyz*color.w,color.w);
				if (texType == 2) color = vec4(color.x);
				color *= scissor;
				result = color * innerCol;
			}
		#ifdef EDGE_AA
			if (strokeAlpha < strokeThr) discard;
		#endif
			outColor = result;        
        }
#endif";

        [TestMethod]
        public void TestFillRectangle01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                fb.setRenderBuffer(BufferId.COLOR0, rb);
                fb.setViewport(new Vector4i(0, 0, Width, Height));

                Program p = new Program(new Module(330, Test01_SHADER));
                GLNVGfragUniforms block = new GLNVGfragUniforms()
                {
                    scissorMat = new Matrix3x4f(),
                    paintMat = Matrix3x4f.Identity,
                    innerCol = new Vector4f(1, 0.8f, 0, 1),
                    outerCol = new Vector4f(1, 0.8f, 0, 1),
                    scissorExt = new Vector2f(1, 1),
                    scissorScale = new Vector2f(1, 1),
                    extent = new Vector2f(0, 0),
                    radius = 0,
                    feather = 1,
                    strokeMult = 1,
                    strokeThr = -1,
                    texType = 0,
                    type = GLNVGshaderType.NSVG_SHADER_FILLGRAD
                };
                p.getUniformBlock("frag").set(block);
                p.getUniform2f("uViewSize").set(new Vector2f(Width, Height));
                Mesh<NVGvertex, uint> data = new Mesh<NVGvertex, uint>(NVGvertex.SizeInBytes, sizeof(uint), MeshMode.TRIANGLES, MeshUsage.GPU_STATIC);
                data.addAttributeType(0, 2, AttributeType.A32F, false);
                data.addAttributeType(1, 2, AttributeType.A32F, false);
                data.addVertex(new NVGvertex(100, 160, 0.5f, 1f));
                data.addVertex(new NVGvertex(Width - 100, 160, 0.5f, 1f));
                data.addVertex(new NVGvertex(Width - 100, 100, 0.5f, 1f));
                data.addVertex(new NVGvertex(100, 160, 0.5f, 1f));
                data.addVertex(new NVGvertex(Width - 100, 100, 0.5f, 1f));
                data.addVertex(new NVGvertex(100, 100, 0.5f, 1f));
                fb.draw(p, data);

                bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                RenderTestUtils.SaveTestResult(TESTSNAME, "TestFillRectangle01_Screenshot", bmp);
#endif
                if (rb != null)
                    rb.Dispose();
                if (p != null)
                    p.Dispose();
                if (data != null)
                    data.Dispose();
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/VG/ScreenshotVG01.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestFillRectangle01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestFillRectangle02()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                using (var nvg = Nvg.CreateContext(fb))
                {
                    RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                    fb.setRenderBuffer(BufferId.COLOR0, rb);
                    fb.setViewport(new Vector4i(0, 0, Width, Height));

                    Nvg.BeginFrame(nvg, Width, Height, (float)Width / (float)Height);

                    //Simple rectangle
                    Nvg.BeginPath(nvg);
                    Nvg.Rect(nvg, 50, 100, Width - 100, 50);
                    Nvg.FillColor(nvg, new Vector4f(1, 0.8f, 0, 1));
                    Nvg.Fill(nvg);
                    Nvg.EndFrame(nvg);

                    // Vertical gradient rectangle
                    NVGpaint gradient = Nvg.LinearGradient(nvg, 50, 200, 50, 200 + 40, Nvg.RGBA(128, 255, 128, 255), Nvg.RGBA(0, 64, 0, 255));
                    Nvg.BeginPath(nvg);
                    Nvg.Rect(nvg, 50, 200, Width - 100, 50);
                    Nvg.FillPaint(nvg, gradient);
                    Nvg.Fill(nvg);
                    Nvg.EndFrame(nvg);

                    // Vertical gradient rectangle
                    gradient = Nvg.LinearGradient(nvg, 50, 300 + 40, 50, 300, Nvg.RGBA(128, 255, 128, 255), Nvg.RGBA(0, 64, 0, 255));
                    Nvg.BeginPath(nvg);
                    Nvg.Rect(nvg, 50, 300, Width - 100, 50);
                    Nvg.FillPaint(nvg, gradient);
                    Nvg.Fill(nvg);
                    Nvg.EndFrame(nvg);

                    // Horizontal gradient rectangle
                    gradient = Nvg.LinearGradient(nvg, 50, 400, Width - 90 + 50, 400, Nvg.RGBA(255, 128, 128, 255), Nvg.RGBA(64, 0, 0, 255));
                    Nvg.BeginPath(nvg);
                    Nvg.Rect(nvg, 50, 400, Width - 100, 50);
                    Nvg.FillPaint(nvg, gradient);
                    Nvg.Fill(nvg);
                    Nvg.EndFrame(nvg);

                    bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                    RenderTestUtils.SaveTestResult(TESTSNAME, "TestFillRectangle02_Screenshot", bmp);
#endif
                    if (rb != null)
                        rb.Dispose();
                }
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/VG/ScreenshotVG02.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestFillRectangle02_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestFillRectangle03()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                using (var nvg = Nvg.CreateContext(fb))
                {
                    RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                    fb.setRenderBuffer(BufferId.COLOR0, rb);
                    fb.setViewport(new Vector4i(0, 0, Width, Height));

                    Nvg.BeginFrame(nvg, Width, Height, (float)Width / (float)Height);

                    //Box gradient rectangle
                    NVGpaint gradient = Nvg.BoxGradient(nvg, 50, 100 + 1.5f, Width - 100, 300, 10, 50, Nvg.RGBA(128, 255, 128, 255), Nvg.RGBA(0, 64, 0, 255));
                    Nvg.BeginPath(nvg);
                    Nvg.Rect(nvg, 50, 100, Width - 100, 300);
                    Nvg.FillPaint(nvg, gradient);
                    Nvg.Fill(nvg);
                    Nvg.EndFrame(nvg);

                    bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                    RenderTestUtils.SaveTestResult(TESTSNAME, "TestFillRectangle03_Screenshot", bmp);
#endif
                    if (rb != null)
                        rb.Dispose();
                }
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/VG/ScreenshotVG03.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestFillRectangle03_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestFillRectangle04()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBuffer fb = new FrameBuffer())
            {
                using (var nvg = Nvg.CreateContext(fb))
                {
                    RenderBuffer rb = new RenderBuffer(RenderBuffer.RenderBufferFormat.RGB8, Width, Height);
                    fb.setRenderBuffer(BufferId.COLOR0, rb);
                    fb.setViewport(new Vector4i(0, 0, Width, Height));

                    Nvg.BeginFrame(nvg, Width, Height, (float)Width / (float)Height);

                    //Radial Gradient rectangle
                    NVGpaint gradient = Nvg.RadialGradient(nvg, (400 + 100) / 2, (400 + 100) / 2, 10, 150, Nvg.RGBA(128, 255, 128, 255), Nvg.RGBA(0, 32, 0, 255));
                    Nvg.BeginPath(nvg);
                    Nvg.Rect(nvg, 100, 100, 300, 300);
                    Nvg.FillPaint(nvg, gradient);
                    Nvg.Fill(nvg);
                    Nvg.EndFrame(nvg);

                    bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                    RenderTestUtils.SaveTestResult(TESTSNAME, "TestFillRectangle04_Screenshot", bmp);
#endif
                    if (rb != null)
                        rb.Dispose();
                }
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/VG/ScreenshotVG04.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestFillRectangle04_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestFillCircle01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBufferForTest fb = FrameBufferForTest.NewFrameBufferForTest(Width, Height))
            {
                using (var nvg = Nvg.CreateContext(fb.FrameBuffer))
                {
                    Nvg.BeginFrame(nvg, Width, Height, (float)Width / (float)Height);

                    //Simple Circle
                    Nvg.BeginPath(nvg);
                    Nvg.Circle(nvg, 250, 250, 150);
                    Nvg.FillColor(nvg, new Vector4f(1, 0.8f, 0, 1));
                    Nvg.Fill(nvg);
                    Nvg.EndFrame(nvg);

                    bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                    RenderTestUtils.SaveTestResult(TESTSNAME, "TestFillCircle01_Screenshot", bmp);
#endif
                }
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/VG/ScreenshotVG05.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestFillCircle01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestFillCircle02()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBufferForTest fb = FrameBufferForTest.NewFrameBufferForTest(Width, Height))
            {
                using (var nvg = Nvg.CreateContext(fb.FrameBuffer))
                {
                    Nvg.BeginFrame(nvg, Width, Height, (float)Width / (float)Height);

                    //Radial Gradient Circle
                    NVGpaint gradient = Nvg.RadialGradient(nvg, (400 + 100) / 2, (400 + 100) / 2, 10, 150, Nvg.RGBA(128, 255, 128, 255), Nvg.RGBA(0, 32, 0, 255));
                    Nvg.BeginPath(nvg);
                    Nvg.Circle(nvg, 250, 250, 150);
                    Nvg.FillPaint(nvg, gradient);
                    Nvg.Fill(nvg);
                    Nvg.EndFrame(nvg);

                    bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                    RenderTestUtils.SaveTestResult(TESTSNAME, "TestFillCircle02_Screenshot", bmp);
#endif
                }
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/VG/ScreenshotVG06.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestFillCircle02_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestFillElipse01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBufferForTest fb = FrameBufferForTest.NewFrameBufferForTest(Width, Height))
            {
                using (var nvg = Nvg.CreateContext(fb.FrameBuffer))
                {
                    Nvg.BeginFrame(nvg, Width, Height, (float)Width / (float)Height);

                    //Simple Ellipse
                    Nvg.BeginPath(nvg);
                    Nvg.Ellipse(nvg, 250, 250, 200, 150);
                    Nvg.FillColor(nvg, new Vector4f(1, 0.8f, 0, 1));
                    Nvg.Fill(nvg);
                    Nvg.EndFrame(nvg);

                    bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                    RenderTestUtils.SaveTestResult(TESTSNAME, "TestFillElipse01_Screenshot", bmp);
#endif
                }
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/VG/ScreenshotVG07.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestFillElipse01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestFillElipse02()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBufferForTest fb = FrameBufferForTest.NewFrameBufferForTest(Width, Height))
            {
                using (var nvg = Nvg.CreateContext(fb.FrameBuffer))
                {
                    Nvg.BeginFrame(nvg, Width, Height, (float)Width / (float)Height);

                    //Radial Gradient Ellipse
                    NVGpaint gradient = Nvg.RadialGradient(nvg, (400 + 100) / 2, (400 + 100) / 2, 10, 200, Nvg.RGBA(128, 255, 128, 255), Nvg.RGBA(0, 32, 0, 255));
                    Nvg.BeginPath(nvg);
                    Nvg.Ellipse(nvg, 250, 250, 200, 150);
                    Nvg.FillPaint(nvg, gradient);
                    Nvg.Fill(nvg);
                    Nvg.EndFrame(nvg);

                    bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                    RenderTestUtils.SaveTestResult(TESTSNAME, "TestFillElipse02_Screenshot", bmp);
#endif
                }
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/VG/ScreenshotVG08.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestFillElipse02_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestFillRoundRect01()
        {
            Bitmap bmp;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBufferForTest fb = FrameBufferForTest.NewFrameBufferForTest(Width, Height))
            {
                using (var nvg = Nvg.CreateContext(fb.FrameBuffer))
                {
                    Nvg.BeginFrame(nvg, Width, Height, (float)Width / (float)Height);

                    Nvg.BeginPath(nvg);
                    Nvg.RoundedRect(nvg, 50, 100, Width - 100, 300, 30);
                    Nvg.FillColor(nvg, new Vector4f(1, 0.8f, 0, 1));
                    Nvg.Fill(nvg);
                    Nvg.EndFrame(nvg);

                    bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                    RenderTestUtils.SaveTestResult(TESTSNAME, "TestFillRoundRect01_Screenshot", bmp);
#endif
                }
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/VG/ScreenshotVG09.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestFillRoundRect01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestFillArc01()
        {
            Bitmap bmp;
            float a0 = 0.0f + 6;
            float a1 = (float)(System.Math.PI) + 6;
            int cx = 250;
            int cy = 250;
            float r0 = 150;
            float r1 = r0 * 0.80f;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBufferForTest fb = FrameBufferForTest.NewFrameBufferForTest(Width, Height))
            {
                using (var nvg = Nvg.CreateContext(fb.FrameBuffer))
                {
                    Nvg.BeginFrame(nvg, Width, Height, (float)Width / (float)Height);

                    Nvg.BeginPath(nvg);
                    Nvg.Arc(nvg, cx, cy, r0, a0, a1, NVGwinding.NVG_CW);
                    Nvg.Arc(nvg, cx, cy, r1, a1, a0, NVGwinding.NVG_CCW);
                    float ax = (float)(cx + Math.Cos(a0) * (r0 + r1) * 0.5f);
                    float ay = (float)(cy + Math.Sin(a0) * (r0 + r1) * 0.5f);
                    float bx = (float)(cx + Math.Cos(a1) * (r0 + r1) * 0.5f);
                    float by = (float)(cy + Math.Sin(a1) * (r0 + r1) * 0.5f);
                    NVGpaint gradient = Nvg.LinearGradient(nvg, ax, ay, bx, by, Nvg.RGBAf(0, 0.8f, 0, 1), Nvg.RGBAf(0, 0, 0, 1));
                    Nvg.FillPaint(nvg, gradient);
                    Nvg.Fill(nvg);
                    Nvg.EndFrame(nvg);

                    bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                    RenderTestUtils.SaveTestResult(TESTSNAME, "TestFillArc01_Screenshot", bmp);
#endif
                }
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/VG/ScreenshotVG10.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestFillArc01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestLineTo01()
        {
            Bitmap bmp;
            int sx = 50;
            int sy = 200;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBufferForTest fb = FrameBufferForTest.NewFrameBufferForTest(Width, Height))
            {
                using (var nvg = Nvg.CreateContext(fb.FrameBuffer))
                {
                    Nvg.BeginFrame(nvg, Width, Height, (float)Width / (float)Height);

                    Nvg.BeginPath(nvg);
                    Nvg.MoveTo(nvg, sx, sy);
                    Nvg.LineTo(nvg, sx + 400, sy);
                    Nvg.LineTo(nvg, sx + 200, sy + 200);
                    Nvg.FillColor(nvg, new Vector4f(1, 0.8f, 0, 1));
                    Nvg.Fill(nvg);
                    Nvg.EndFrame(nvg);

                    bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                    RenderTestUtils.SaveTestResult(TESTSNAME, "TestLineTo01_Screenshot", bmp);
#endif
                }
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/VG/ScreenshotVG11.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestLineTo01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestLineTo02()
        {
            Bitmap bmp;
            int sx = 50;
            int sy = 400;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBufferForTest fb = FrameBufferForTest.NewFrameBufferForTest(Width, Height))
            {
                using (var nvg = Nvg.CreateContext(fb.FrameBuffer))
                {
                    Nvg.BeginFrame(nvg, Width, Height, (float)Width / (float)Height);

                    Nvg.BeginPath(nvg);
                    Nvg.MoveTo(nvg, sx, sy);
                    Nvg.BezierTo(nvg, sx + 150, sy + 70, sx + 300, sy - 50, sx + 400, sy);
                    Nvg.LineTo(nvg, sx + 200, sy - 200);
                    Nvg.FillColor(nvg, new Vector4f(1, 0.8f, 0, 1));
                    Nvg.Fill(nvg);
                    Nvg.EndFrame(nvg);

                    bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                    RenderTestUtils.SaveTestResult(TESTSNAME, "TestLineTo02_Screenshot", bmp);
#endif
                }
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/VG/ScreenshotVG12.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestLineTo02_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestStrokeLine01()
        {
            Bitmap bmp;
            int sx = 150;
            int sy = 300;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBufferForTest fb = FrameBufferForTest.NewFrameBufferForTest(Width, Height))
            {
                using (var nvg = Nvg.CreateContext(fb.FrameBuffer))
                {
                    Nvg.BeginFrame(nvg, Width, Height, (float)Width / (float)Height);

                    Nvg.BeginPath(nvg);
                    Nvg.MoveTo(nvg, sx, sy);
                    Nvg.LineTo(nvg, sx, sy - 200);
                    Nvg.LineTo(nvg, sx + 200, sy - 200);
                    Nvg.LineTo(nvg, sx + 200, sy);
                    Nvg.StrokeWidth(nvg, 20);
                    Nvg.StrokeColor(nvg, new Vector4f(1, 0.8f, 0, 1));
                    Nvg.Stroke(nvg);
                    Nvg.EndFrame(nvg);

                    bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                    RenderTestUtils.SaveTestResult(TESTSNAME, "TestStrokeLine01_Screenshot", bmp);
#endif
                }
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/VG/ScreenshotVG12.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestStrokeLine01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestStrokeLineCaps01()
        {
            Bitmap bmp;
            int sx = 150;
            int sy = 100;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBufferForTest fb = FrameBufferForTest.NewFrameBufferForTest(Width, Height))
            {
                using (var nvg = Nvg.CreateContext(fb.FrameBuffer))
                {
                    Nvg.BeginFrame(nvg, Width, Height, (float)Width / (float)Height);

                    Nvg.StrokeWidth(nvg, 20);
                    Nvg.BeginPath(nvg);
                    Nvg.MoveTo(nvg, sx, sy);
                    Nvg.LineTo(nvg, sx + 200, sy);
                    Nvg.LineCap(nvg, NVGlineCap.NVG_BUTT);
                    Nvg.StrokeColor(nvg, new Vector4f(1, 0, 0, 1));
                    Nvg.Stroke(nvg);

                    Nvg.BeginPath(nvg);
                    Nvg.MoveTo(nvg, sx, sy + 100);
                    Nvg.LineTo(nvg, sx + 200, sy + 100);
                    Nvg.LineCap(nvg, NVGlineCap.NVG_ROUND);
                    Nvg.StrokeColor(nvg, new Vector4f(0, 1, 0, 1));
                    Nvg.Stroke(nvg);

                    Nvg.BeginPath(nvg);
                    Nvg.MoveTo(nvg, sx, sy + 200);
                    Nvg.LineTo(nvg, sx + 200, sy + 200);
                    Nvg.LineCap(nvg, NVGlineCap.NVG_SQUARE);
                    Nvg.StrokeColor(nvg, new Vector4f(0, 0, 1, 1));
                    Nvg.Stroke(nvg);

                    Nvg.EndFrame(nvg);

                    bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                    RenderTestUtils.SaveTestResult(TESTSNAME, "TestStrokeLineCaps01_Screenshot", bmp);
#endif
                }
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/VG/ScreenshotVG12.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestStrokeLineCaps01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestStrokeLineJoins01()
        {
            Bitmap bmp;
            int sx = 150;
            int sy = 100;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBufferForTest fb = FrameBufferForTest.NewFrameBufferForTest(Width, Height))
            {
                using (var nvg = Nvg.CreateContext(fb.FrameBuffer))
                {
                    Nvg.BeginFrame(nvg, Width, Height, (float)Width / (float)Height);

                    Nvg.StrokeWidth(nvg, 20);
                    Nvg.BeginPath(nvg);
                    Nvg.MoveTo(nvg, sx, sy);
                    Nvg.LineTo(nvg, sx, sy + 50);
                    Nvg.LineTo(nvg, sx + 200, sy);
                    Nvg.LineJoin(nvg, NVGlineJoin.NVG_MITER);
                    Nvg.StrokeColor(nvg, new Vector4f(1, 0, 0, 1));
                    Nvg.Stroke(nvg);

                    Nvg.BeginPath(nvg);
                    Nvg.MoveTo(nvg, sx, sy + 100);
                    Nvg.LineTo(nvg, sx, sy + 150);
                    Nvg.LineTo(nvg, sx + 200, sy + 100);
                    Nvg.LineJoin(nvg, NVGlineJoin.NVG_ROUND);
                    Nvg.StrokeColor(nvg, new Vector4f(0, 1, 0, 1));
                    Nvg.Stroke(nvg);

                    Nvg.BeginPath(nvg);
                    Nvg.MoveTo(nvg, sx, sy + 200);
                    Nvg.LineTo(nvg, sx, sy + 250);
                    Nvg.LineTo(nvg, sx + 200, sy + 200);
                    Nvg.LineJoin(nvg, NVGlineJoin.NVG_BEVEL);
                    Nvg.StrokeColor(nvg, new Vector4f(0, 0, 1, 1));
                    Nvg.Stroke(nvg);

                    Nvg.EndFrame(nvg);

                    bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                    RenderTestUtils.SaveTestResult(TESTSNAME, "TestStrokeLineJoins01_Screenshot", bmp);
#endif
                }
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/VG/ScreenshotVG12.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestStrokeLineJoins01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestPathWinding01()
        {
            Bitmap bmp;
            int x = 150;
            int y = 100;
            int w = 200;
            int h = 100;
            float cornerRadius = 10;

            IGraphicsContext context = RenderTestUtils.PrepareContext();
            using (FrameBufferForTest fb = FrameBufferForTest.NewFrameBufferForTest(Width, Height))
            {
                using (var nvg = Nvg.CreateContext(fb.FrameBuffer))
                {
                    Nvg.BeginFrame(nvg, Width, Height, (float)Width / (float)Height);

                    NVGpaint shadowPaint = Nvg.BoxGradient(nvg, x, y + 2, w, h, cornerRadius * 2, 10, Nvg.RGBAf(0, 0.8f, 0, 1), Nvg.RGBAf(0, 0.2f, 0, 1));
                    Nvg.BeginPath(nvg);
                    Nvg.Rect(nvg, x - 10, y - 10, w + 20, h + 30);
                    Nvg.RoundedRect(nvg, x, y, w, h, cornerRadius);
                    Nvg.PathWinding(nvg, NVGsolidity.NVG_HOLE);
                    Nvg.FillPaint(nvg, shadowPaint);
                    Nvg.Fill(nvg);


                    Nvg.EndFrame(nvg);

                    bmp = RenderTestUtils.GetScreenshot(Width, Height);
#if SAVE_RESULTS
                    RenderTestUtils.SaveTestResult(TESTSNAME, "TestPathWinding01_Screenshot", bmp);
#endif
                }
            }
            context.MakeCurrent(null);

            Image expectedImg = Image.FromFile("Resources/ControlImages/VG/ScreenshotVG12.bmp");
            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(expectedImg, bmp, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestPathWinding01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0, dissimilarity, epsilonError);
        }

    }
}
