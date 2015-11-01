using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using Sxta.TestTools.ImageTesting;
using System.Drawing.Imaging;

namespace SxtaRenderTests
{
    [TestClass]
    public class ImageTest
    {
        private const float epsilonError = 0.0001f;

        [TestMethod]
        public void TestImageDiff01()
        {
            Image img1 = Image.FromFile("Resources/People1.jpg");
            Image img2 = Image.FromFile("Resources/People2.jpg");

            ImageComparator imageComparer = new ImageComparator(DiffOptions.IGNORE_COLORS);

            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(img1, img2, out dissimilarity);
            //diffResult.Save("TestImageDiff01.jpg", ImageFormat.Jpeg);
            Assert.AreEqual(0.962, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestImageDiff02()
        {
            Image img1 = Image.FromFile("Resources/People1.jpg");
 
            ImageComparator imageComparer = new ImageComparator(DiffOptions.IGNORE_COLORS);

            Image rst = ImageComparator.ReplaceColor(img1, Color.White, Color.AliceBlue);
            rst.Save("AliceBlue.jpg", ImageFormat.Jpeg);
            Image img2 = Image.FromFile("AliceBlue.jpg");

            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(img1, img2, out dissimilarity);
            //diffResult.Save("TestImageDiff02.jpg", ImageFormat.Jpeg);
            Assert.AreEqual(0.05956, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestImageDiff03()
        {
            Image img1 = Image.FromFile("Resources/People1.jpg");

            ImageComparator imageComparer = new ImageComparator(DiffOptions.IGNORE_COLORS);

            Image rst = ImageComparator.ReplaceColor(img1, Color.White, Color.BurlyWood);
            rst.Save("BurlyWood.jpg", ImageFormat.Jpeg);
            Image img2 = Image.FromFile("BurlyWood.jpg");

            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(img1, img2, out dissimilarity);
            //diffResult.Save("TestImageDiff03.jpg", ImageFormat.Jpeg);
            Assert.AreEqual(44.51, dissimilarity, epsilonError);
        }

        [TestMethod]
        public void TestImageDiff04()
        {
            Image img1 = Image.FromFile("Resources/People1.jpg");

            ImageComparator imageComparer = new ImageComparator(DiffOptions.IGNORE_COLORS);

            Image rst2 = ImageComparator.MakeGreyscale(img1);
            rst2.Save("grayscale.jpg", ImageFormat.Jpeg);
            Image img2 = Image.FromFile("grayscale.jpg");

            float dissimilarity;
            Image diffResult = imageComparer.ComputeSimilarity(img1, img2, out dissimilarity);
            //diffResult.Save("TestImageDiff04.jpg", ImageFormat.Jpeg);
            Assert.AreEqual(0.0376, dissimilarity, epsilonError);
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
            //diffResult.Save("TestImageDiff05.jpg", ImageFormat.Jpeg);
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
            //diffResult.Save("TestImageDiff06.jpg", ImageFormat.Jpeg);
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
            //diffResult.Save("TestImageDiff07.jpg", ImageFormat.Jpeg);
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
            //diffResult.Save("TestImageDiff08.jpg", ImageFormat.Jpeg);
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
            //diffResult.Save("TestImageDiff09.jpg", ImageFormat.Jpeg);
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
            //diffResult.Save("TestImageDiff10.jpg", ImageFormat.Jpeg);
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
            //diffResult.Save("TestImageDiff11.jpg", ImageFormat.Jpeg);
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
            //diffResult.Save("TestImageDiff12.jpg", ImageFormat.Jpeg);
            Assert.AreEqual(0.962, dissimilarity, epsilonError);
        }
    }
}
