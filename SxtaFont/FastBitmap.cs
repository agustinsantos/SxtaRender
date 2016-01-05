using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SxtaRender.Fonts
{
    /// <summary>
    /// Allows fast access to <see cref="System.Drawing.Bitmap"/>'s pixel data.
    /// </summary>
    public unsafe class FastBitmap : IDisposable
    {
        /// <summary>
        /// The integral representation of the 8bppIndexed pixel format.
        /// </summary>
        private const int Format8bppIndexed = (int)PixelFormat.Format8bppIndexed;

        /// <summary>
        /// The integral representation of the 24bppRgb pixel format.
        /// </summary>
        private const int Format24bppRgb = (int)PixelFormat.Format24bppRgb;

        /// <summary>
        /// The integral representation of the 32bppArgb pixel format.
        /// </summary>
        private const int Format32bppArgb = (int)PixelFormat.Format32bppArgb;

        /// <summary>
        /// The integral representation of the 32bppPArgb pixel format.
        /// </summary>
        private const int Format32bppPArgb = (int)PixelFormat.Format32bppPArgb;

        /// <summary>
        /// The bitmap.
        /// </summary>
        internal Bitmap bitmap;

        /// <summary>
        /// The width of the bitmap.
        /// </summary>
        private readonly int width;

        /// <summary>
        /// The height of the bitmap.
        /// </summary>
        private readonly int height;

        /// <summary>
        /// The color channel - blue, green, red, alpha.
        /// </summary>
        private readonly int channel;

        ///// <summary>
        ///// The number of bytes in a row.
        ///// </summary>
        private int bytesInARow;

        /// <summary>
        /// The size of the color32 structure.
        /// </summary>
        private int pixelSize;

        /// <summary>
        /// The bitmap data.
        /// </summary>
        internal BitmapData bitmapData;

        /// <summary>
        /// The position of the first pixel in the bitmap.
        /// </summary>
        private byte* pixelBase;

        /// <summary>
        /// Whether the current bitmap is locked
        /// </summary>
        private bool locked;

        /// <summary>
        /// A value indicating whether this instance of the given entity has been disposed.
        /// </summary>
        /// <value><see langword="true"/> if this instance has been disposed; otherwise, <see langword="false"/>.</value>
        /// <remarks>
        /// If the entity is disposed, it must not be disposed a second
        /// time. The isDisposed field is set the first time the entity
        /// is disposed. If the isDisposed field is true, then the Dispose()
        /// method will not dispose again. This help not to prolong the entity's
        /// life in the Garbage Collector.
        /// </remarks>
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="FastBitmap"/> class.
        /// </summary>
        /// <param name="bitmap">The input bitmap.</param>
        public FastBitmap(Image bitmap)
        {
            int pixelFormat = (int)bitmap.PixelFormat;

            // Check image format
            if (!(pixelFormat == Format8bppIndexed ||
                  pixelFormat == Format24bppRgb ||
                  pixelFormat == Format32bppArgb ||
                  pixelFormat == Format32bppPArgb))
            {
                throw new ArgumentException("Only 8bpp, 24bpp and 32bpp images are supported.");
            }

            this.bitmap = (Bitmap)bitmap;
            this.width = this.bitmap.Width;
            this.height = this.bitmap.Height;

            this.channel = pixelFormat == Format8bppIndexed ? 0 : 2;
            this.locked = false;
        }

        /// <summary>
        /// Gets the width, in pixels of the <see cref="System.Drawing.Bitmap"/>.
        /// </summary>
        public int Width { get { return this.width; } }

        /// <summary>
        /// Gets the height, in pixels of the <see cref="System.Drawing.Bitmap"/>.
        /// </summary>
        public int Height { get { return this.height; } }

        /// <summary>
        /// Gets a boolean value that states whether this FastBitmap is currently locked in memory
        /// </summary>
        public bool IsLocked { get { return locked; } }

        /// <summary>
        /// Locks the bitmap into system memory.
        /// </summary>
        public void LockBitmap()
        {
            if (locked)
            {
                throw new InvalidOperationException("Unlock must be called before a Lock operation");
            }
            Rectangle bounds = new Rectangle(Point.Empty, this.bitmap.Size);

            // Figure out the number of bytes in a row. This is rounded up to be a multiple
            // of 4 bytes, since a scan line in an image must always be a multiple of 4 bytes
            // in length.
            this.pixelSize = Image.GetPixelFormatSize(this.bitmap.PixelFormat) / 8;
            this.bytesInARow = bounds.Width * this.pixelSize;
            if (this.bytesInARow % 4 != 0)
            {
                this.bytesInARow = 4 * ((this.bytesInARow / 4) + 1);
            }

            // Lock the bitmap
            this.bitmapData = this.bitmap.LockBits(bounds, ImageLockMode.ReadWrite, this.bitmap.PixelFormat);

            // Set the value to the first scan line
            this.pixelBase = (byte*)this.bitmapData.Scan0.ToPointer();
            this.locked = true;
        }

        /// <summary>
        /// Unlocks the bitmap from system memory.
        /// </summary>
        public void UnlockBitmap()
        {
            if (!locked) return;

            // Copy the RGB values back to the bitmap and unlock the bitmap.
            this.bitmap.UnlockBits(this.bitmapData);
            this.bitmapData = null;
            this.pixelBase = null;
            this.locked = false;
        }

        /// <summary>
        /// Gets the pixel data for the given position.
        /// </summary>
        /// <param name="x">
        /// The x position of the pixel.
        /// </param>
        /// <param name="y">
        /// The y position of the pixel.
        /// </param>
        /// <returns>
        /// The <see cref="Color32"/>.
        /// </returns>
        private Color32* this[int x, int y]
        {
            get { return (Color32*)(this.pixelBase + (y * this.bytesInARow) + (x * this.pixelSize)); }
        }

        public void Clear32(byte r, byte g, byte b, byte a)
        {
            unsafe
            {
                byte* sourcePtr = this.pixelBase;

                for (int i = 0; i < bitmapData.Height; i++)
                {
                    for (int j = 0; j < bitmapData.Width; j++)
                    {
                        *(sourcePtr) = b;
                        *(sourcePtr + 1) = g;
                        *(sourcePtr + 2) = r;
                        *(sourcePtr + 3) = a;

                        sourcePtr += 4;
                    }
                    sourcePtr += bitmapData.Stride - bitmapData.Width * 4; //move to the end of the line (past unused space)
                }
            }
        }
        /// <summary>
        /// Returns try if the given pixel is empty (i.e. black)
        /// </summary>
        public unsafe static bool EmptyPixel(BitmapData bitmapData, int px, int py)
        {
            byte* addr = (byte*)(bitmapData.Scan0) + bitmapData.Stride * py + px * 3;
            return (*addr == 0 && *(addr + 1) == 0 && *(addr + 2) == 0);
        }

        /// <summary>
        /// Allows the implicit conversion of an instance of <see cref="FastBitmap"/> to a 
        /// <see cref="System.Drawing.Image"/>.
        /// </summary>
        /// <param name="fastBitmap">
        /// The instance of <see cref="FastBitmap"/> to convert.
        /// </param>
        /// <returns>
        /// An instance of <see cref="System.Drawing.Image"/>.
        /// </returns>
        public static implicit operator Image(FastBitmap fastBitmap)
        {
            return fastBitmap.bitmap;
        }

        /// <summary>
        /// Allows the implicit conversion of an instance of <see cref="FastBitmap"/> to a 
        /// <see cref="System.Drawing.Bitmap"/>.
        /// </summary>
        /// <param name="fastBitmap">
        /// The instance of <see cref="FastBitmap"/> to convert.
        /// </param>
        /// <returns>
        /// An instance of <see cref="System.Drawing.Bitmap"/>.
        /// </returns>
        public static implicit operator Bitmap(FastBitmap fastBitmap)
        {
            return fastBitmap.bitmap;
        }

        /// <summary>
        /// Gets the color at the specified pixel of the <see cref="System.Drawing.Bitmap"/>.
        /// </summary>
        /// <param name="x">The x-coordinate of the pixel to retrieve.</param>
        /// <param name="y">The y-coordinate of the pixel to retrieve.</param>
        /// <returns>The <see cref="System.Drawing.Color"/> at the given pixel.</returns>
        public Color GetPixel(int x, int y)
        {
#if DEBUG
            if ((x < 0) || (x >= this.width))
            {
                throw new ArgumentOutOfRangeException("x", "Value cannot be less than zero or greater than the bitmap width.");
            }

            if ((y < 0) || (y >= this.height))
            {
                throw new ArgumentOutOfRangeException("y", "Value cannot be less than zero or greater than the bitmap height.");
            }
#endif
            Color32* data = this[x, y];
            return Color.FromArgb(data->A, data->R, data->G, data->B);
        }

        /// <summary>
        /// Sets the color of the specified pixel of the <see cref="System.Drawing.Bitmap"/>.
        /// </summary>
        /// <param name="x">The x-coordinate of the pixel to set.</param>
        /// <param name="y">The y-coordinate of the pixel to set.</param>
        /// <param name="color">
        /// A <see cref="System.Drawing.Color"/> color structure that represents the 
        /// color to set the specified pixel.
        /// </param>
        public void SetPixel(int x, int y, Color color)
        {
#if DEBUG
            if ((x < 0) || (x >= this.width))
            {
                throw new ArgumentOutOfRangeException("x", "Value cannot be less than zero or greater than the bitmap width.");
            }

            if ((y < 0) || (y >= this.height))
            {
                throw new ArgumentOutOfRangeException("y", "Value cannot be less than zero or greater than the bitmap height.");
            }
#endif
            Color32* data = this[x, y];
            data->R = color.R;
            data->G = color.G;
            data->B = color.B;
            data->A = color.A;
        }



        /// <summary>
        /// Disposes the object and frees resources for the Garbage Collector.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            FastBitmap fastBitmap = obj as FastBitmap;

            if (fastBitmap == null)
            {
                return false;
            }

            return this.bitmap == fastBitmap.bitmap;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.bitmap.GetHashCode();
        }

        /// <summary>
        /// Disposes the object and frees resources for the Garbage Collector.
        /// </summary>
        /// <param name="disposing">If true, the object gets disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing & locked)
            {
                UnlockBitmap();
            }


            // Note disposing is done.
            this.isDisposed = true;
        }




        /// <summary>
        /// Returns true if the given pixel is empty (i.e. alpha is zero)
        /// </summary>
        public static unsafe bool EmptyAlphaPixel(BitmapData bitmapData, int px, int py, byte alphaEmptyPixelTolerance)
        {
            Debug.Assert(px < bitmapData.Width && py < bitmapData.Height);
            int d = Image.GetPixelFormatSize(bitmapData.PixelFormat) / 8;
            byte* addr = (byte*)(bitmapData.Scan0) + bitmapData.Stride * py + px * d;
            return (*(addr + (d - 1)) <= alphaEmptyPixelTolerance);

        }

        /// <summary>
        /// Blits a block of a bitmap data from source to destination, using the luminance of the source to determine the 
        /// alpha of the target. Source must be 24-bit, target must be 32-bit.
        /// </summary>
        public static void BlitMask(BitmapData source, BitmapData target, int srcPx, int srcPy, int srcW, int srcH, int px, int py)
        {
            int sourceBpp = 3;
            int targetBpp = 4;

            int targetStartX, targetEndX;
            int targetStartY, targetEndY;
            int copyW, copyH;

            targetStartX = Math.Max(px, 0);
            targetEndX = Math.Min(px + srcW, target.Width);

            targetStartY = Math.Max(py, 0);
            targetEndY = Math.Min(py + srcH, target.Height);

            copyW = targetEndX - targetStartX;
            copyH = targetEndY - targetStartY;

            if (copyW < 0)
            {
                return;
            }

            if (copyH < 0)
            {
                return;
            }

            int sourceStartX = srcPx + targetStartX - px;
            int sourceStartY = srcPy + targetStartY - py;


            unsafe
            {
                byte* sourcePtr = (byte*)(source.Scan0);
                byte* targetPtr = (byte*)(target.Scan0);


                byte* targetY = targetPtr + targetStartY * target.Stride;
                byte* sourceY = sourcePtr + sourceStartY * source.Stride;
                for (int y = 0; y < copyH; y++, targetY += target.Stride, sourceY += source.Stride)
                {

                    byte* targetOffset = targetY + targetStartX * targetBpp;
                    byte* sourceOffset = sourceY + sourceStartX * sourceBpp;
                    for (int x = 0; x < copyW; x++, targetOffset += targetBpp, sourceOffset += sourceBpp)
                    {
                        int lume = *(sourceOffset) + *(sourceOffset + 1) + *(sourceOffset + 2);

                        lume /= 3;

                        if (lume > 255)
                            lume = 255;

                        *(targetOffset + 3) = (byte)lume;

                    }
                }
            }
        }

        /// <summary>
        /// Blits from source to target. Both source and target must be 32-bit
        /// </summary>
        public static void Blit(BitmapData source, BitmapData target, Rectangle sourceRect, int px, int py)
        {
            Blit(source, target, sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height, px, py);
        }

        /// <summary>
        /// Blits from source to target. Both source and target must be 32-bit
        /// </summary>
        public static void Blit(BitmapData source, BitmapData target, int srcPx, int srcPy, int srcW, int srcH, int destX, int destY)
        {
            int bpp = 4;

            int targetStartX, targetEndX;
            int targetStartY, targetEndY;
            int copyW, copyH;

            targetStartX = Math.Max(destX, 0);
            targetEndX = Math.Min(destX + srcW, target.Width);

            targetStartY = Math.Max(destY, 0);
            targetEndY = Math.Min(destY + srcH, target.Height);

            copyW = targetEndX - targetStartX;
            copyH = targetEndY - targetStartY;

            if (copyW < 0)
            {
                return;
            }

            if (copyH < 0)
            {
                return;
            }

            int sourceStartX = srcPx + targetStartX - destX;
            int sourceStartY = srcPy + targetStartY - destY;


            unsafe
            {
                byte* sourcePtr = (byte*)(source.Scan0);
                byte* targetPtr = (byte*)(target.Scan0);


                byte* targetY = targetPtr + targetStartY * target.Stride;
                byte* sourceY = sourcePtr + sourceStartY * source.Stride;
                for (int y = 0; y < copyH; y++, targetY += target.Stride, sourceY += source.Stride)
                {

                    byte* targetOffset = targetY + targetStartX * bpp;
                    byte* sourceOffset = sourceY + sourceStartX * bpp;
                    for (int x = 0; x < copyW * bpp; x++, targetOffset++, sourceOffset++)
                        *(targetOffset) = *(sourceOffset);

                }
            }
        }

        public unsafe void PutPixel32(int px, int py, byte r, byte g, byte b, byte a)
        {
            byte* addr = (byte*)(bitmapData.Scan0) + bitmapData.Stride * py + px * 4;

            *addr = b;
            *(addr + 1) = g;
            *(addr + 2) = r;
            *(addr + 3) = a;
        }

        public unsafe void GetPixel32(int px, int py, ref byte r, ref byte g, ref byte b, ref byte a)
        {
            byte* addr = (byte*)(bitmapData.Scan0) + bitmapData.Stride * py + px * 4;

            b = *addr;
            g = *(addr + 1);
            r = *(addr + 2);
            a = *(addr + 3);
        }

        public unsafe void PutAlpha32(int px, int py, byte a)
        {
            *((byte*)(bitmapData.Scan0) + bitmapData.Stride * py + px * 4 + 3) = a;
        }

        public unsafe void GetAlpha32(int px, int py, ref byte a)
        {
            a = *((byte*)(bitmapData.Scan0) + bitmapData.Stride * py + px * 4 + 3);
        }

        public void SetDataRegion(BitmapData source, Rectangle srcRect, Rectangle dstRect, int margin)
        {
            int srcStride = source.Stride;
            int dstStride = this.bitmapData.Stride;
            if (this.pixelSize != Image.GetPixelFormatSize(source.PixelFormat) / 8)
                throw new Exception("different depth is not allowed");

            int depth = this.pixelSize;
            for (int j = 0; j < srcRect.Height; ++j)
            {
                //int offset = (0 + i) * stride + srcRect.X * depth;
                int srcOffset = (srcRect.Y + j) * srcStride + srcRect.X * depth;
                int dstOffset = (dstRect.Y + margin + j) * dstStride + (dstRect.X + margin) * depth;
                for (int i = 0; i < srcRect.Width * depth; ++i)
                {
                    *((byte*)(this.pixelBase) + dstOffset + i) = *((byte*)(source.Scan0) + srcOffset + i);
                }
            }
        }

        public void DownScale32(int newWidth, int newHeight)
        {
            if (!locked)
                this.LockBitmap();

            FastBitmap newBitmap = new FastBitmap(new Bitmap(newWidth, newHeight, bitmap.PixelFormat));
            newBitmap.LockBitmap();

            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new Exception("DownsScale32 only works on 32 bit images");

            float xscale = (float)bitmapData.Width / newWidth;
            float yscale = (float)bitmapData.Height / newHeight;

            byte r = 0, g = 0, b = 0, a = 0;
            float summedR = 0f;
            float summedG = 0f;
            float summedB = 0f;
            float summedA = 0f;

            int left, right, top, bottom;  //the area of old pixels covered by the new bitmap


            float targetStartX, targetEndX;
            float targetStartY, targetEndY;

            float leftF, rightF, topF, bottomF; //edges of new pixel in old pixel coords
            float weight;
            float weightScale = xscale * yscale;
            float totalColourWeight = 0f;

            for (int m = 0; m < newHeight; m++)
            {
                for (int n = 0; n < newWidth; n++)
                {

                    leftF = n * xscale;
                    rightF = (n + 1) * xscale;

                    topF = m * yscale;
                    bottomF = (m + 1) * yscale;

                    left = (int)leftF;
                    right = (int)rightF;

                    top = (int)topF;
                    bottom = (int)bottomF;

                    if (left < 0) left = 0;
                    if (top < 0) top = 0;
                    if (right >= bitmapData.Width) right = bitmapData.Width - 1;
                    if (bottom >= bitmapData.Height) bottom = bitmapData.Height - 1;

                    summedR = 0f;
                    summedG = 0f;
                    summedB = 0f;
                    summedA = 0f;
                    totalColourWeight = 0f;

                    for (int j = top; j <= bottom; j++)
                    {
                        for (int i = left; i <= right; i++)
                        {
                            targetStartX = Math.Max(leftF, i);
                            targetEndX = Math.Min(rightF, i + 1);

                            targetStartY = Math.Max(topF, j);
                            targetEndY = Math.Min(bottomF, j + 1);

                            weight = (targetEndX - targetStartX) * (targetEndY - targetStartY);

                            GetPixel32(i, j, ref r, ref g, ref b, ref a);

                            summedA += weight * a;

                            if (a != 0)
                            {
                                summedR += weight * r;
                                summedG += weight * g;
                                summedB += weight * b;
                                totalColourWeight += weight;
                            }

                        }
                    }

                    summedR /= totalColourWeight;
                    summedG /= totalColourWeight;
                    summedB /= totalColourWeight;
                    summedA /= weightScale;

                    if (summedR < 0) summedR = 0f;
                    if (summedG < 0) summedG = 0f;
                    if (summedB < 0) summedB = 0f;
                    if (summedA < 0) summedA = 0f;

                    if (summedR >= 256) summedR = 255;
                    if (summedG >= 256) summedG = 255;
                    if (summedB >= 256) summedB = 255;
                    if (summedA >= 256) summedA = 255;

                    newBitmap.PutPixel32(n, m, (byte)summedR, (byte)summedG, (byte)summedB, (byte)summedA);
                }

            }

            this.UnlockBitmap();
            newBitmap.UnlockBitmap();

            this.bitmap = newBitmap.bitmap;
        }

        /// <summary>
        /// Sets colour without touching alpha values
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public void Colour32(byte r, byte g, byte b)
        {
            unsafe
            {
                byte* addr;
                for (int i = 0; i < bitmapData.Width; i++)
                {
                    for (int j = 0; j < bitmapData.Height; j++)
                    {
                        addr = (byte*)(bitmapData.Scan0) + bitmapData.Stride * j + i * 4;
                        *addr = b;
                        *(addr + 1) = g;
                        *(addr + 2) = r;
                    }
                }
            }
        }

        public void BlurAlpha(int radius, int passes)
        {
            using (FastBitmap tmp = new FastBitmap(new Bitmap(this.bitmap.Width, this.bitmap.Height, bitmap.PixelFormat)))
            {
                tmp.LockBitmap();

                byte a = 0;
                int summedA;
                int weight = 0;
                int xpos, ypos, x, y, kx, ky;
                int width = bitmap.Width;
                int height = bitmap.Height;

                for (int pass = 0; pass < passes; pass++)
                {

                    //horizontal pass
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            summedA = weight = 0;
                            for (kx = -radius; kx <= radius; kx++)
                            {
                                xpos = x + kx;
                                if (xpos >= 0 && xpos < width)
                                {
                                    GetAlpha32(xpos, y, ref a);
                                    summedA += a;
                                    weight++;
                                }
                            }

                            summedA /= weight;
                            tmp.PutAlpha32(x, y, (byte)summedA);
                        }
                    }

                    //vertical pass
                    for (x = 0; x < width; ++x)
                    {
                        for (y = 0; y < height; ++y)
                        {
                            summedA = weight = 0;
                            for (ky = -radius; ky <= radius; ky++)
                            {
                                ypos = y + ky;
                                if (ypos >= 0 && ypos < height)
                                {
                                    tmp.GetAlpha32(x, ypos, ref a);
                                    summedA += a;
                                    weight++;
                                }
                            }

                            summedA /= weight;

                            PutAlpha32(x, y, (byte)summedA);

                        }
                    }
                }
            }
        }

        public Bitmap MakeDistanceMap()
        {
            int[] xdist = new int[width * height];
            int[] ydist = new int[width * height];
            float[] gx = new float[width * height];
            float[] gy = new float[width * height];
            float[] data = new float[width * height];
            float[] outside = new float[width * height];
            float[] inside = new float[width * height];

            if (!locked)
                this.LockBitmap();

            // Convert img into float (data)
            float img_min = float.MaxValue, img_max = float.MinValue;
            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; ++j)
                {
                    byte v = *(this.pixelBase + bitmapData.Stride * j + i * this.pixelSize + this.channel); //img[i];
                    data[i + j * width] = v;
                    if (v > img_max) img_max = v;
                    if (v < img_min) img_min = v;
                }
            // Rescale image levels between 0 and 1
            for (int i = 0; i < width * height; ++i)
            {
                data[i] = (data[i] - img_min) / img_max;
            }
            // Compute outside = edtaa3(bitmap); % Transform background (0's)
            SdfComputation.ComputeGradient(data, width, height, gx, gy);
            SdfComputation.Edtaa3(data, gx, gy, width, height, xdist, ydist, outside);
            for (int i = 0; i < width * height; ++i)
                if (outside[i] < 0)
                    outside[i] = 0.0f;

            // Compute inside = edtaa3(1-bitmap); % Transform foreground (1's)
            gx.Initialize();
            gy.Initialize();
            for (int i = 0; i < width * height; ++i)
                data[i] = 1 - data[i];
            SdfComputation.ComputeGradient(data, width, height, gx, gy);
            SdfComputation.Edtaa3(data, gx, gy, width, height, xdist, ydist, inside);
            for (int i = 0; i < width * height; ++i)
                if (inside[i] < 0)
                    inside[i] = 0.0f;

            // distmap = outside - inside; % Bipolar distance field
            byte[] _out = new byte[width * height];
            for (int i = 0; i < width * height; ++i)
            {
                outside[i] -= inside[i];
                outside[i] = 128 + outside[i] * 16;
                if (outside[i] < 0) outside[i] = 0;
                if (outside[i] > 255) outside[i] = 255;
                _out[i] = (byte)(255 - outside[i]);
                //out[i] = (unsigned char) outside[i];
            }
            var im = ConvertToBitmap(this.width, this.height, _out);
            this.UnlockBitmap();
            return im;
        }

        private static byte[] PadLines(byte[] bytes, int rows, int columns)
        {
            var currentStride = columns; // 3
            var newStride = columns;  // 4
            var newBytes = new byte[newStride * rows];
            for (var i = 0; i < rows; i++)
                Buffer.BlockCopy(bytes, currentStride * i, newBytes, newStride * i, currentStride);
            return newBytes;
        }

        private Bitmap ConvertToBitmap(int width, int height, byte[] imageData)
        {
            var data = new byte[width * height * 4];

            int o = 0;
            for (var i = 0; i < width * height; i++)
            {
                byte value = imageData[i];
                data[o++] = value;
                data[o++] = value;
                data[o++] = value;
                data[o++] = 0;
            }
            var im = new Bitmap(this.width, this.height, this.width * 4,
                                PixelFormat.Format32bppRgb,
                                Marshal.UnsafeAddrOfPinnedArrayElement(data, 0));
            return im;
        }

        public void Save(string filename, ImageFormat format)
        {
            this.UnlockBitmap();
            this.bitmap.Save(filename, format);
        }
    }
}

