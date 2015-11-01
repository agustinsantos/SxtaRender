using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SxtaRenderTests.TestTools;

namespace SxtaRenderTests
{
    [TestClass]
    public class OpenGLContextTest
    {
        private static GameWindow control = null;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            control = new GameWindow();
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            if (control != null)
                control.Dispose();
        }

        [TestMethod]
        public void TestContext01()
        {
            IGraphicsContext context = RenderTestUtils.PrepareContext();
            string vendor = GL.GetString(StringName.Vendor);
            string renderer = GL.GetString(StringName.Renderer);
            string version = GL.GetString(StringName.Version);
            string shadingLanguageVersion = GL.GetString(StringName.ShadingLanguageVersion);
            string extensions = GL.GetString(StringName.Extensions);
            context.MakeCurrent(null);

            Assert.IsFalse(string.IsNullOrWhiteSpace(vendor));
            Assert.IsFalse(string.IsNullOrWhiteSpace(renderer));
            Assert.IsFalse(string.IsNullOrWhiteSpace(version));
            Assert.IsFalse(string.IsNullOrWhiteSpace(shadingLanguageVersion));
            Assert.IsFalse(string.IsNullOrWhiteSpace(extensions));
        }



        [TestMethod]
        public void TestContext04()
        {
            IGraphicsContext context = RenderTestUtils.PrepareContext();
            string vendor = GL.GetString(StringName.Vendor);
            string renderer = GL.GetString(StringName.Renderer);
            string version = GL.GetString(StringName.Version);
            string shadingLanguageVersion = GL.GetString(StringName.ShadingLanguageVersion);
            string extensions = GL.GetString(StringName.Extensions);
            context.MakeCurrent(null);

            Assert.IsFalse(string.IsNullOrWhiteSpace(vendor));
            Assert.IsFalse(string.IsNullOrWhiteSpace(renderer));
            Assert.IsFalse(string.IsNullOrWhiteSpace(version));
            Assert.IsFalse(string.IsNullOrWhiteSpace(shadingLanguageVersion));
            Assert.IsFalse(string.IsNullOrWhiteSpace(extensions));
        }

        [TestMethod]
        public void TestContext05()
        {
            IGraphicsContext context = RenderTestUtils.PrepareContext();
            string vendor = GL.GetString(StringName.Vendor);
            string renderer = GL.GetString(StringName.Renderer);
            string version = GL.GetString(StringName.Version);
            string shadingLanguageVersion = GL.GetString(StringName.ShadingLanguageVersion);
            string extensions = GL.GetString(StringName.Extensions);
            context.MakeCurrent(null);

            Assert.IsFalse(string.IsNullOrWhiteSpace(vendor));
            Assert.IsFalse(string.IsNullOrWhiteSpace(renderer));
            Assert.IsFalse(string.IsNullOrWhiteSpace(version));
            Assert.IsFalse(string.IsNullOrWhiteSpace(shadingLanguageVersion));
            Assert.IsFalse(string.IsNullOrWhiteSpace(extensions));
        }

        [TestMethod]
        public void TestContext06()
        {
            IGraphicsContext context = RenderTestUtils.PrepareContext();
            string vendor = GL.GetString(StringName.Vendor);
            string renderer = GL.GetString(StringName.Renderer);
            string version = GL.GetString(StringName.Version);
            string shadingLanguageVersion = GL.GetString(StringName.ShadingLanguageVersion);
            string extensions = GL.GetString(StringName.Extensions);
            context.MakeCurrent(null);

            Assert.IsFalse(string.IsNullOrWhiteSpace(vendor));
            Assert.IsFalse(string.IsNullOrWhiteSpace(renderer));
            Assert.IsFalse(string.IsNullOrWhiteSpace(version));
            Assert.IsFalse(string.IsNullOrWhiteSpace(shadingLanguageVersion));
            Assert.IsFalse(string.IsNullOrWhiteSpace(extensions));
        }

        [TestMethod]
        public void TestContext07()
        {
            IGraphicsContext context = RenderTestUtils.PrepareContext();
            string vendor = GL.GetString(StringName.Vendor);
            string renderer = GL.GetString(StringName.Renderer);
            string version = GL.GetString(StringName.Version);
            string shadingLanguageVersion = GL.GetString(StringName.ShadingLanguageVersion);
            string extensions = GL.GetString(StringName.Extensions);
            context.MakeCurrent(null);

            Assert.IsFalse(string.IsNullOrWhiteSpace(vendor));
            Assert.IsFalse(string.IsNullOrWhiteSpace(renderer));
            Assert.IsFalse(string.IsNullOrWhiteSpace(version));
            Assert.IsFalse(string.IsNullOrWhiteSpace(shadingLanguageVersion));
            Assert.IsFalse(string.IsNullOrWhiteSpace(extensions));
        }
    }
}
