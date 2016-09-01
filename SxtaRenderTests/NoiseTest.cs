using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using proland;

namespace SxtaRenderTests
{
    [TestClass]
    public class NoiseTest
    {
        [TestMethod]
        public void TestNoise2D01()
        {
            int width = 1024 * 4;
            int height = 1024 * 4;
            float min = float.MaxValue, max = float.MinValue;

            // Create a Bitmap object
            Bitmap bitmap = new Bitmap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float noiseVal = Noise.cnoise(x / 256.0f, y / 256.0f);
                    if (noiseVal > max) max = noiseVal;
                    if (noiseVal < min) min = noiseVal;
                    bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb((int)(255 * (1 + noiseVal) / 2), 0, 0));
                }
            }
            Console.WriteLine("Max={0}, Min={1}", max, min);
            bitmap.Save("TestNoise2D01.bmp");
        }

        [TestMethod]
        public void TestFRandom01()
        {
            int numberOfValues = 10000;
            float min = float.MaxValue, max = float.MinValue;
            int seed = 12345;

            for (int i = 0; i < numberOfValues; i++)
            {
                float noiseVal = Noise.frandom(ref seed);
                if (noiseVal > max) max = noiseVal;
                if (noiseVal < min) min = noiseVal;
            }
            Console.WriteLine("Max={0}, Min={1}", max, min);
        }

        [TestMethod]
        public void TestLRandom02()
        {
            int numberOfValues = 10000;
            float min = float.MaxValue, max = float.MinValue;
            int seed = 12345;

            for (int i = 0; i < numberOfValues; i++)
            {
                float noiseVal = (float)((Noise.lrandom(ref seed) % (2 * 256)) - 256) / 256;
                if (noiseVal > max) max = noiseVal;
                if (noiseVal < min) min = noiseVal;
            }
            Console.WriteLine("Max={0}, Min={1}", max, min);
        }
    }
}
