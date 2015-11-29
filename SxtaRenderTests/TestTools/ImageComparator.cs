using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;

namespace Sxta.TestTools.ImageTesting
{
    public struct Tolerance
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;
        public byte MinBrightness;
        public byte MaxBrightness;
    }

    public enum OverlayType
    {
        Flat,
        Movement,
        FlatDifferenceIntensity,
        MovementDifference
    }

    public struct DiffOptions
    {
        public Color ErrorColor;
        public Tolerance Tolerance;
        public float OverlayTransparency;
        public OverlayType OverlayType;
        public bool IgnoreColor;
        public bool IgnoreAntialiasing;
        public byte AntialiasingDistance;

        public static DiffOptions STRICT = new DiffOptions()
        {
            ErrorColor = Color.FromArgb(255, 0, 255, 255),
            OverlayTransparency = 1.0f,
            OverlayType = OverlayType.Flat,
            Tolerance = new Tolerance() { R = 2, G = 2, B = 2, A = 2, MinBrightness = 5, MaxBrightness = 255 },
            IgnoreColor = false,
            IgnoreAntialiasing = false,
            AntialiasingDistance = 1
        };

        public static DiffOptions SIMILIAR = new DiffOptions()
        {
            ErrorColor = Color.FromArgb(255, 0, 255, 255),
            OverlayTransparency = 1.0f,
            OverlayType = OverlayType.Flat,
            Tolerance = new Tolerance() { R = 5, G = 5, B = 5, A = 5, MinBrightness = 10, MaxBrightness = 125 },
            IgnoreColor = false,
            IgnoreAntialiasing = false,
            AntialiasingDistance = 1
        };

        public static DiffOptions IGNORE_NOTHING = new DiffOptions()
        {
            ErrorColor = Color.Orange,//.FromArgb(255, 0, 255, 255),
            OverlayTransparency = 0.7f,
            OverlayType = OverlayType.Flat,
            Tolerance = new Tolerance() { R = 0, G = 0, B = 0, A = 0, MinBrightness = 0, MaxBrightness = 255 },
            IgnoreColor = false,
            IgnoreAntialiasing = false,
            AntialiasingDistance = 1
        };

        public static DiffOptions IGNORE_ANTIALIASING = new DiffOptions()
        {
            ErrorColor = Color.FromArgb(255, 0, 255, 255),
            OverlayTransparency = 0.7f,
            OverlayType = OverlayType.Flat,
            Tolerance = new Tolerance() { R = 32, G = 32, B = 32, A = 32, MinBrightness = 64, MaxBrightness = 96 },
            IgnoreColor = false,
            IgnoreAntialiasing = true,
            AntialiasingDistance = 1
        };

        public static DiffOptions IGNORE_COLORS = new DiffOptions()
        {
            ErrorColor = Color.FromArgb(255, 255, 0, 255),
            OverlayTransparency = 0.7f,
            OverlayType = OverlayType.Flat,
            Tolerance = new Tolerance() { R = 16, G = 16, B = 16, A = 16, MinBrightness = 16, MaxBrightness = 240 },
            IgnoreColor = true,
            IgnoreAntialiasing = false,
            AntialiasingDistance = 1
        };
    }

    public class ImageComparator
    {
        private delegate Color ErrorPixelTransform(Color d1, Color d2);

        private readonly DiffOptions options;
        private readonly ErrorPixelTransform errorPixelTransformer;

        public ImageComparator(DiffOptions diffOptions)
        {
            options = diffOptions;
            switch (options.OverlayType)
            {
                case OverlayType.Flat:
                    errorPixelTransformer = ErrorPixelFlat;
                    break;
                case OverlayType.Movement:
                    errorPixelTransformer = ErrorPixelMovement;
                    break;
                case OverlayType.FlatDifferenceIntensity:
                    errorPixelTransformer = ErrorPixelFlatDifferenceIntensity;
                    break;
                case OverlayType.MovementDifference:
                    errorPixelTransformer = ErrorPixelMovementDifference;
                    break;
            }
        }

        /// <summary>
        /// The luminosity method is a more sophisticated version of the average method. 
        /// It also averages the values, but it forms a weighted average to account for human perception.
        /// We’re more sensitive to green than other colors, so green is weighted most heavily. 
        /// The formula for luminosity is 0.3 R + 0.59 G + 0.11 B.
        /// </summary>
        /// <param name="img1">Img1.</param>
        public static Bitmap MakeGreyscale(System.Drawing.Image img1)
        {
            try
            {
                Bitmap newImage = new Bitmap(img1);
                newImage.SetResolution(img1.HorizontalResolution, img1.VerticalResolution);
                int width = img1.Width;
                int height = img1.Height;

                using (FastBitmap fastBitmap = new FastBitmap(newImage))
                {
                    Parallel.For(
                        0,
                        height,
                        y =>
                    {
                        for (int x = 0; x < width; x++)
                        {
                            // Get the pixel color.
                            // ReSharper disable once AccessToDisposedClosure
                            Color currentColor = fastBitmap.GetPixel(x, y);

                            float br = GetBrightnessInfo(currentColor);

                            fastBitmap.SetPixel(x, y, Color.FromArgb(currentColor.A, (byte)br, (byte)br, (byte)br));
                        }
                    });
                }
                return newImage;
            }
            catch (Exception ex)
            {
                throw new Exception("Error processing image.", ex);
            }
        }


        private static float GetBrightness(byte r, byte g, byte b)
        {
            return 0.3f * r + 0.59f * g + 0.11f * b;
        }

        private static float GetBrightnessInfo(Color col)
        {
            return GetBrightness(col.R, col.G, col.B);
        }
        private static bool IsRGBSame(Color d1, Color d2)
        {

            return (d1.R == d2.R && d1.G == d2.G && d1.B == d2.B);
        }

        private static bool IsColorSimilar(byte a, byte b, byte tolerance)
        {
            int absDiff = System.Math.Abs(a - b);

            if (a == b)
            {
                return true;
            }
            else if (absDiff < tolerance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsRGBSimilar(Color d1, Color d2)
        {
            bool red = IsColorSimilar(d1.R, d2.R, options.Tolerance.R);
            bool green = IsColorSimilar(d1.G, d2.G, options.Tolerance.G);
            bool blue = IsColorSimilar(d1.B, d2.B, options.Tolerance.B);
            bool alpha = IsColorSimilar(d1.A, d2.A, options.Tolerance.A);

            return red && green && blue && alpha;
        }

        private bool IsContrasting(float brightness1, float brightness2)
        {
            return System.Math.Abs(brightness1 - brightness2) > options.Tolerance.MaxBrightness;
        }
        private static int ColorsDistance(Color c1, Color c2)
        {
            return (int)((System.Math.Abs(c1.R - c2.R) + System.Math.Abs(c1.G - c2.G) + System.Math.Abs(c1.B - c2.B)) / 3);
        }

        private Color ErrorPixelFlat(Color d1, Color d2)
        {
            return options.ErrorColor;
        }
        private Color ErrorPixelMovement(Color d1, Color d2)
        {
            Color rst = Color.FromArgb(
                                        d2.A,
                                        (int)(((d2.R * (options.ErrorColor.R / 255.0)) + options.ErrorColor.R) / 2),
                                        (int)(((d2.G * (options.ErrorColor.G / 255.0)) + options.ErrorColor.G) / 2),
                                        (int)(((d2.B * (options.ErrorColor.B / 255.0)) + options.ErrorColor.B) / 2));

            return rst;
        }

        private Color ErrorPixelFlatDifferenceIntensity(Color d1, Color d2)
        {
            Color rst = Color.FromArgb(
                                        ColorsDistance(d1, d2),
                                        options.ErrorColor.R,
                                        options.ErrorColor.G,
                                        options.ErrorColor.B);

            return rst;
        }
        private Color ErrorPixelMovementDifference(Color d1, Color d2)
        {
            var ratio = ColorsDistance(d1, d2) / 255.0 * 0.8;
            Color rst = Color.FromArgb(
                                d2.A,
                                (int)(((1 - ratio) * (d2.R * (options.ErrorColor.R / 255.0)) + ratio * options.ErrorColor.R) / 2),
                                (int)(((1 - ratio) * (d2.G * (options.ErrorColor.G / 255.0)) + ratio * options.ErrorColor.G) / 2),
                                (int)(((1 - ratio) * (d2.B * (options.ErrorColor.B / 255.0)) + ratio * options.ErrorColor.B) / 2));

            return rst;
        }
        private static float GetHue(Color color)
        {
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            float rn = r / 255f;
            float gn = g / 255f;
            float bn = b / 255f;
            float max = System.Math.Max(System.Math.Max(rn, gn), bn);
            float min = System.Math.Min(System.Math.Min(rn, gn), bn);
            float h = 0;
            float d;

            if (max == min)
            {
                h = 0; // achromatic
            }
            else
            {
                d = max - min;

                if (max == r) h = (g - b) / d + (g < b ? 6 : 0);
                else if (max == g) h = (b - r) / d + 2;
                else if (max == b) h = (r - g) / d + 4;

                h /= 6;
            }

            return h;
        }

        private bool IsAntialiased(int sx, int sy, FastBitmap data)
        {
            Color targetPix;
            int distance = options.AntialiasingDistance;
            var hasHighContrastSibling = 0;
            var hasSiblingWithDifferentHue = 0;
            var hasEquivalentSibling = 0;

            Color sourcePix = data.GetPixel(sx, sy);
            float sourcePix_h = GetHue(sourcePix);

            for (int i = distance * -1; i <= distance; i++)
            {
                for (int j = distance * -1; j <= distance; j++)
                {

                    if (i == 0 && j == 0)
                    {
                        continue; // ignore source pixel
                    }
                    if ((sx + i > 0 && sx + i < data.Width) && (sy + j > 0 && sy + j < data.Height))
                    {
                        targetPix = data.GetPixel(sx + i, sy + j);

                        float targetBr = GetBrightnessInfo(targetPix);
                        float sourceBr = GetBrightnessInfo(targetPix);
                        float targetPix_h = GetHue(targetPix);

                        if (IsContrasting(sourceBr, targetBr))
                        {
                            hasHighContrastSibling++;
                        }

                        if (IsRGBSame(sourcePix, targetPix))
                        {
                            hasEquivalentSibling++;
                        }

                        if (System.Math.Abs(targetPix_h - sourcePix_h) > 0.3)
                        {
                            hasSiblingWithDifferentHue++;
                        }

                        if (hasSiblingWithDifferentHue > 1 || hasHighContrastSibling > 1)
                        {
                            return true;
                        }
                    }
                }
            }

            if (hasEquivalentSibling < 2)
            {
                return true;
            }

            return false;
        }

        public Image ComputeSimilarity(System.Drawing.Image img1, System.Drawing.Image img2, out float dissimilarity)
        {

            Bitmap left = new Bitmap(img1);
            left.SetResolution(img1.HorizontalResolution, img1.VerticalResolution);
            Bitmap right = new Bitmap(img2);
            right.SetResolution(img2.HorizontalResolution, img2.VerticalResolution);

            int width = img1.Width;
            int height = img1.Height;
            int mismatchCount = 0;

            Bitmap diff = null;
            try
            {
                diff = new Bitmap(img1);
                diff.SetResolution(img1.HorizontalResolution, img1.VerticalResolution);

                using (FastBitmap fastLeft = new FastBitmap(left))
                {
                    using (FastBitmap fastRight = new FastBitmap(right))
                    {
                        using (FastBitmap diffBitmap = new FastBitmap(diff))
                        {
                            Parallel.For(0, height, y =>
                             {
                                 for (int x = 0; x < width; x++)
                                 {
                                     Color pixel1 = fastLeft.GetPixel(x, y);
                                     Color pixel2 = fastRight.GetPixel(x, y);
                                     Color targetPix;
                                     if (options.IgnoreColor)
                                     {
                                         float br1 = GetBrightnessInfo(pixel1);
                                         float br2 = GetBrightnessInfo(pixel2);
                                         var alpha = IsColorSimilar(pixel1.A, pixel2.A, options.Tolerance.A);
                                         var brightness = IsColorSimilar((byte)br1, (byte)br2, options.Tolerance.MinBrightness);
                                         if (brightness && alpha) //is PixelBrightnessSimilar
                                                                    {
                                             targetPix = Color.FromArgb(
                                                  (int)(pixel2.A * options.OverlayTransparency),//a
                                                  (int)br2, //r
                                                  (int)br2, //g
                                                  (int)br2 //b
                                                 );
                                         }
                                         else
                                         {
                                             targetPix = errorPixelTransformer(pixel1, pixel2);
                                             Interlocked.Increment(ref mismatchCount);
                                         }
                                         diffBitmap.SetPixel(x, y, targetPix);
                                         continue;
                                     }
                                     if (IsRGBSimilar(pixel1, pixel2))
                                     {
                                         targetPix = Color.FromArgb(
                                                      (int)(pixel2.A * options.OverlayTransparency),//a
                                                       pixel2.R, //r
                                                       pixel2.G, //g
                                                       pixel2.B //b
                                                     );
                                     }
                                     else if (options.IgnoreAntialiasing && (IsAntialiased(x, y, fastLeft) || IsAntialiased(x, y, fastRight)))
                                     {
                                         float br1 = GetBrightnessInfo(pixel1);
                                         float br2 = GetBrightnessInfo(pixel2);
                                         var alpha = IsColorSimilar(pixel1.A, pixel2.A, options.Tolerance.A);
                                         var brightness = IsColorSimilar((byte)br1, (byte)br2, options.Tolerance.MinBrightness);
                                         if (brightness && alpha) //is PixelBrightnessSimilar
                                                                    {
                                             targetPix = Color.FromArgb(
                                                  (int)(pixel2.A * options.OverlayTransparency),//a
                                                  (int)br2, //r
                                                  (int)br2, //g
                                                  (int)br2 //b
                                                 );
                                         }
                                         else
                                         {
                                             targetPix = errorPixelTransformer(pixel1, pixel2);
                                             Interlocked.Increment(ref mismatchCount);
                                         }
                                     }
                                     else
                                     {
                                         targetPix = errorPixelTransformer(pixel1, pixel2);
                                         Interlocked.Increment(ref mismatchCount);
                                     }
                                     diffBitmap.SetPixel(x, y, targetPix);
                                 }
                             });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (diff != null)
                {
                    diff.Dispose();
                }

                throw new Exception("Error processing image.", ex);
            }

            dissimilarity = ((float)mismatchCount / (height * width) * 100f);
            return diff;
        }

        public static Image ReplaceColor(System.Drawing.Image img1, Color original, Color replacement)
        {
            Bitmap newImage = null;

            try
            {
                byte originalR = original.R;
                byte originalG = original.G;
                byte originalB = original.B;
                byte originalA = original.A;

                byte replacementR = replacement.R;
                byte replacementG = replacement.G;
                byte replacementB = replacement.B;
                byte replacementA = replacement.A;

                int fuzziness = 1;

                byte minR = (originalR - fuzziness).ToByte();
                byte minG = (originalG - fuzziness).ToByte();
                byte minB = (originalB - fuzziness).ToByte();

                byte maxR = (originalR + fuzziness).ToByte();
                byte maxG = (originalG + fuzziness).ToByte();
                byte maxB = (originalB + fuzziness).ToByte();

                newImage = new Bitmap(img1);
                newImage.SetResolution(img1.HorizontalResolution, img1.VerticalResolution);
                int width = img1.Width;
                int height = img1.Height;

                using (FastBitmap fastBitmap = new FastBitmap(newImage))
                {
                    Parallel.For(
                        0,
                        height,
                        y =>
                    {
                        for (int x = 0; x < width; x++)
                        {
                            // Get the pixel color.
                            // ReSharper disable once AccessToDisposedClosure
                            Color currentColor = fastBitmap.GetPixel(x, y);

                            byte currentR = currentColor.R;
                            byte currentG = currentColor.G;
                            byte currentB = currentColor.B;
                            byte currentA = currentColor.A;

                            // Test whether it is in the expected range.
                            if (InRange(currentR, minR, maxR))
                            {
                                if (InRange(currentG, minG, maxG))
                                {
                                    if (InRange(currentB, minB, maxB))
                                    {
                                        // Ensure the values are within an acceptable byte range
                                        // and set the new value.
                                        byte r = (originalR - currentR + replacementR).ToByte();
                                        byte g = (originalG - currentG + replacementG).ToByte();
                                        byte b = (originalB - currentB + replacementB).ToByte();

                                        // Allow replacement with transparent color.
                                        byte a = currentA;
                                        if (originalA != replacementA)
                                        {
                                            a = replacementA;
                                        }

                                        // ReSharper disable once AccessToDisposedClosure
                                        fastBitmap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                                    }
                                }
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                if (newImage != null)
                {
                    newImage.Dispose();
                }

                throw new Exception("Error processing image.", ex);
            }

            //DiffResult rst = new DiffResult () { img = image, similarity =  0.0f };
            return newImage;
        }

        /// <summary>
        /// Returns value indicating whether the given number is with in the minimum and maximum
        /// given range.
        /// </summary>
        /// <param name="value">The The value to clamp.</param>
        /// <param name="min">If <paramref name="include"/>The minimum range value.</param>
        /// <param name="max">The maximum range value.</param>
        /// <param name="include">Whether to include the minimum and maximum values. Defaults to true.</param>
        /// <typeparam name="T">The <see cref="System.Type"/> to test.</typeparam>
        /// <returns>
        /// True if the value falls within the maximum and minimum; otherwise, false.
        /// </returns>
        public static bool InRange<T>(T value, T min, T max, bool include = true) where T : IComparable<T>
        {
            if (include)
            {
                return (value.CompareTo(min) >= 0) && (value.CompareTo(max) <= 0);
            }

            return (value.CompareTo(min) > 0) && (value.CompareTo(max) < 0);
        }
    }

    /// <summary>
    /// Encapsulates a series of time saving extension methods to the <see cref="T:System.Float"/> class.
    /// </summary>
    public static class HelperExtensions
    {
        /// <summary>
        /// Converts an <see cref="T:System.Float"/> value into a valid <see cref="T:System.Byte"/>.
        /// <remarks>
        /// If the value given is less than 0 or greater than 255, the value will be constrained into
        /// those restricted ranges.
        /// </remarks>
        /// </summary>
        /// <param name="value">
        /// The <see cref="T:System.Float"/> to convert.
        /// </param>
        /// <returns>
        /// The <see cref="T:System.Byte"/>.
        /// </returns>
        public static byte ToByte(this float value)
        {
            return Convert.ToByte(Clamp(value, 0, 255));
        }

        /// <summary>
        /// Converts an <see cref="T:System.Int32"/> value into a valid <see cref="T:System.Byte"/>.
        /// <remarks>
        /// If the value given is less than 0 or greater than 255, the value will be constrained into
        /// those restricted ranges.
        /// </remarks>
        /// </summary>
        /// <param name="value">
        /// The <see cref="T:System.Int32"/> to convert.
        /// </param>
        /// <returns>
        /// The <see cref="T:System.Byte"/>.
        /// </returns>
        public static byte ToByte(this int value)
        {
            return Convert.ToByte(Clamp(value, 0, 255));
        }

        /// <summary>
        /// Converts the string representation of a number in a specified culture-specific format to its 
        /// 32-bit signed integer equivalent using invariant culture.
        /// </summary>
        /// <param name="value">The integer.</param>
        /// <param name="toParse">A string containing a number to convert.</param>
        /// <returns>A 32-bit signed integer equivalent to the number specified in toParse.</returns>
        public static int ParseInvariant(this int value, string toParse)
        {
            return int.Parse(toParse, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">
        /// The The value to clamp.
        /// </param>
        /// <param name="min">
        /// The minimum value. If value is less than min, min will be returned.
        /// </param>
        /// <param name="max">
        /// The maximum value. If value is greater than max, max will be returned.
        /// </param>
        /// <typeparam name="T">
        /// The <see cref="System.Type"/> to clamp.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IComparable{T}"/> representing the clamped value.
        /// </returns>
        private static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
            {
                return min;
            }

            if (value.CompareTo(max) > 0)
            {
                return max;
            }

            return value;
        }
    }
}

