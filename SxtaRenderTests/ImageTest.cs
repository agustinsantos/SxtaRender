#define SAVE_RESULTS
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sxta.TestTools.ImageTesting;
using SxtaRenderTests.TestTools;
using System.Drawing;

namespace SxtaRenderTests
{
    [TestClass]
    public class ImageTest
    {
        private const float epsilonError = 0.0001f;
        private static readonly string TESTSNAME = "ImageTest";

        [TestMethod]
        public void TestImageDiff01()
        {
            Image img1 = Image.FromFile("Resources/People1.jpg");
            Image img2 = Image.FromFile("Resources/People2.jpg");

            ImageComparator imageComparer = new ImageComparator(DiffOptions.IGNORE_COLORS);

            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(img1, img2, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestImageDiff01_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0.962, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestImageDiff02()
        {
            Image img1 = Image.FromFile("Resources/People1.jpg");

            ImageComparator imageComparer = new ImageComparator(DiffOptions.IGNORE_COLORS);

            Image rst = ImageComparator.ReplaceColor(img1, Color.White, Color.AliceBlue);
            string filename = RenderTestUtils.SaveTestResult(TESTSNAME, "TestImageDiff02_Result", rst);
            Image img2 = Image.FromFile(filename);

            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(img1, img2, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestImageDiff02_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0.0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestImageDiff03()
        {
            Image img1 = Image.FromFile("Resources/People1.jpg");

            ImageComparator imageComparer = new ImageComparator(DiffOptions.IGNORE_NOTHING);

            Image rst = ImageComparator.ReplaceColor(img1, Color.White, Color.BurlyWood);
            string filename = RenderTestUtils.SaveTestResult(TESTSNAME, "TestImageDiff03_Result", rst);
            Image img2 = Image.FromFile(filename);

            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(img1, img2, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestImageDiff03_ImageDiff", diffResult);
#endif
            Assert.AreEqual(44.346, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestImageDiff04()
        {
            Image img1 = Image.FromFile("Resources/People1.jpg");

            ImageComparator imageComparer = new ImageComparator(DiffOptions.IGNORE_COLORS);

            Image rst = ImageComparator.MakeGreyscale(img1);
            string filename = RenderTestUtils.SaveTestResult(TESTSNAME, "TestImageDiff04_Result", rst);
            Image img2 = Image.FromFile(filename);

            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(img1, img2, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestImageDiff04_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0.0, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestImageDiff05()
        {
            Image img1 = Image.FromFile("Resources/People1.jpg");
            Image img2 = Image.FromFile("Resources/People2.jpg");

            DiffOptions diffOptions = DiffOptions.IGNORE_COLORS;
            diffOptions.ErrorColor = Color.FromArgb(255, 0, 0, 255);
            diffOptions.OverlayTransparency = 0.1f;
            diffOptions.OverlayType = OverlayType.Flat;

            ImageComparator imageComparer = new ImageComparator(diffOptions);

            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(img1, img2, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestImageDiff05_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0.962, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestImageDiff06()
        {
            Image img1 = Image.FromFile("Resources/People1.jpg");
            Image img2 = Image.FromFile("Resources/People2.jpg");

            DiffOptions diffOptions = new DiffOptions()
            {
                ErrorColor = Color.FromArgb(255, 0, 255, 0),
                Tolerance = new Tolerance() { R = 16, G = 16, B = 16, A = 16, MinBrightness = 16, MaxBrightness = 240 },
                OverlayTransparency = 0.7f,
                OverlayType = OverlayType.Flat,
                IgnoreColor = true,
                IgnoreAntialiasing = true,
            };

            ImageComparator imageComparer = new ImageComparator(diffOptions);

            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(img1, img2, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestImageDiff06_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0.962, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestImageDiff07()
        {
            Image img1 = Image.FromFile("Resources/People1.jpg");
            Image img2 = Image.FromFile("Resources/People2.jpg");

            ImageComparator imageComparer = new ImageComparator(DiffOptions.IGNORE_ANTIALIASING);

            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(img1, img2, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestImageDiff07_ImageDiff", diffResult);
#endif
            Assert.AreEqual(5.8564, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestImageDiff08()
        {
            Image img1 = Image.FromFile("Resources/People1.jpg");
            Image img2 = Image.FromFile("Resources/People2.jpg");

            ImageComparator imageComparer = new ImageComparator(DiffOptions.IGNORE_NOTHING);

            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(img1, img2, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestImageDiff08_ImageDiff", diffResult);
#endif
            Assert.AreEqual(97.2724, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestImageDiff09()
        {
            Image img1 = Image.FromFile("Resources/People1.jpg");
            Image img2 = Image.FromFile("Resources/People2.jpg");

            ImageComparator imageComparer = new ImageComparator(DiffOptions.STRICT);

            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(img1, img2, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestImageDiff09_ImageDiff", diffResult);
#endif
            Assert.AreEqual(18.4008, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestImageDiff10()
        {
            Image img1 = Image.FromFile("Resources/People1.jpg");
            Image img2 = Image.FromFile("Resources/People2.jpg");

            DiffOptions diffOptions = DiffOptions.IGNORE_COLORS;
            diffOptions.ErrorColor = Color.FromArgb(255, 0, 0, 255);
            diffOptions.OverlayTransparency = 0.7f;
            diffOptions.OverlayType = OverlayType.Movement;

            ImageComparator imageComparer = new ImageComparator(diffOptions);

            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(img1, img2, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestImageDiff10_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0.962, dissimilarity, epsilonError);
        }
        [TestMethod]
        public void TestImageDiff11()
        {
            Image img1 = Image.FromFile("Resources/People1.jpg");
            Image img2 = Image.FromFile("Resources/People2.jpg");

            DiffOptions diffOptions = DiffOptions.IGNORE_COLORS;
            diffOptions.ErrorColor = Color.FromArgb(255, 0, 0, 255);
            diffOptions.OverlayTransparency = 0.7f;
            diffOptions.OverlayType = OverlayType.MovementDifference;

            ImageComparator imageComparer = new ImageComparator(diffOptions);

            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(img1, img2, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestImageDiff11_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0.962, dissimilarity, epsilonError);
        }
        [TestMethod]
        public void TestImageDiff12()
        {
            Image img1 = Image.FromFile("Resources/People1.jpg");
            Image img2 = Image.FromFile("Resources/People2.jpg");

            DiffOptions diffOptions = DiffOptions.IGNORE_COLORS;
            diffOptions.ErrorColor = Color.FromArgb(255, 0, 0, 255);
            diffOptions.OverlayTransparency = 0.7f;
            diffOptions.OverlayType = OverlayType.FlatDifferenceIntensity;

            ImageComparator imageComparer = new ImageComparator(diffOptions);

            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(img1, img2, out dissimilarity);
#if SAVE_RESULTS
            RenderTestUtils.SaveTestResult(TESTSNAME, "TestImageDiff12_ImageDiff", diffResult);
#endif
            Assert.AreEqual(0.962, dissimilarity, epsilonError);
        }
    }
}
