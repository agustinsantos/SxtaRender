using Sxta.Render;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
namespace SxtaRender.Fonts
{
    /// <summary>
    /// The texture-font class is in charge of creating bitmap glyphs and to upload them to the texture atlas.
    /// </summary>
    public class TextureFont
    {
        private string fontName;

        /// <summary>
        /// Vector of glyphs contained in this font.
        /// </summary>
        private TextureGlyph[] glyphs;

        /// <summary>
        /// Bitmap atlas structure to store glyphs data.
        /// </summary>
        private Bitmap normalBmp;

        /// <summary>
        /// Bitmap atlas structure to store glyphs data in SDF.
        /// </summary>
        private Bitmap sdfBmp;

        /// <summary>
        /// Bitmap atlas structure to store glyphs shadows.
        /// </summary>
        private Bitmap shadowBmp;

        private Dictionary<String, int> kerningPairs;

        /// <summary>
        /// Font size
        /// </summary>
        private float size;

        /// <summary>
        /// The font family ascent in design units
        /// The ascender is the vertical distance from the horizontal baseline to
        /// the highest 'character' coordinate in a font face. Unfortunately, font
        /// formats define the ascender differently. For some, it represents the
        /// ascent of all capital latin characters (without accents), for others it
        /// is the ascent of the highest accented character, and finally, other
        /// formats define it as being equal to bbox.yMax.
        /// </summary>
        private float ascender;

        /// <summary>
        /// font family descent in design units
        /// The descender is the vertical distance from the horizontal baseline to
        /// the lowest 'character' coordinate in a font face. Unfortunately, font
        /// formats define the descender differently. For some, it represents the
        /// descent of all capital latin characters (without accents), for others it
        /// is the ascent of the lowest accented character, and finally, other
        /// formats define it as being equal to bbox.yMin. This field is negative
        /// for values below the baseline.
        /// </summary>
        private float descender;

        /// <summary>
        /// font family line spacing in design units
        /// This field is simply used to compute a default line spacing (i.e., the
        /// baseline-to-baseline distance) when writing text with this font. Note
        /// that it usually is larger than the sum of the ascender and descender
        /// taken as absolute values. There is also no guarantee that no glyphs
        /// extend above or below subsequent baselines when using this distance.
        /// </summary>
        private float lineSpacing;

        /// <summary>
        /// Gets the height, in font design units, of the em square for the specified style.
        /// </summary>
        private int emHeight;

        private int margin;

        /// <summary>
        /// Whether the original font was detected to be monospaced
        /// </summary>
        private bool naturallyMonospaced = false;

        private float meanGlyphWidth;

        private Dictionary<char, TextureGlyph> charSetMapping = null;
        public Dictionary<char, TextureGlyph> CharSetMapping
        {
            get
            {
                if (charSetMapping == null)
                {
                    charSetMapping = new Dictionary<char, TextureGlyph>();
                    foreach (var glygh in glyphs)
                        charSetMapping.Add(glygh.charcode, glygh);
                }
                return charSetMapping;
            }
        }

        public Dictionary<String, int> KerningPairs
        {
            get { return kerningPairs; }
        }

        public Bitmap NormalBitmap
        {
            get
            {
                return this.normalBmp;
            }
        }
        public Bitmap SdfBitmap
        {
            get
            {
                return this.sdfBmp;
            }
        }
        public Bitmap ShadowBitmap
        {
            get
            {
                return this.shadowBmp;
            }
        }

        public bool HasSDFTexture
        {
            get
            {
                return this.sdfBmp != null;
            }
        }
        public bool HasShadowTexture
        {
            get
            {
                return this.shadowBmp != null;
            }
        }

        /// <summary>
        /// font family ascent in design units
        /// </summary>
        public float Ascender { get { return ascender; } }

        /// <summary>
        /// font family ascent converted to pixels
        /// </summary>
        public float AscenderPixel { get { return size * ascender / emHeight; } }

        /// <summary>
        /// font family descent in design units
        /// </summary>
        public float Descender { get { return descender; } }

        /// <summary>
        /// font family descent converted to pixels
        /// </summary>
        public float DescenderPixel { get { return size * descender / emHeight; } }

        /// <summary>
        /// font family line spacing in design units
        /// </summary>
        public float LineSpacing { get { return lineSpacing; } }

        /// <summary>
        /// font family line spacing converted to pixels
        /// </summary>
        public float LineSpacingPixel { get { return size * lineSpacing / emHeight; } }

        /// <summary>
        /// Gets the height, in font design units, of the em square for the specified style.
        /// Em square is a typography term that refers to the rectangle occupied by the font's widest letter, traditionally the letter M.
        /// </summary>
        public int EmHeight { get { return emHeight; } }

        public int Margin { get { return margin; } }

        public bool IsNaturallyMonospaced
        {
            get
            {
                return naturallyMonospaced;
            }
        }

        public float MeanGlyphWidth
        {
            get
            {
                return meanGlyphWidth;
            }
        }

        public static TextureFont FromFile(string filename, string dirName = "Resources/Fonts", string imageExtension = "png")
        {
            TextureFont rstFont = new TextureFont();
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("Filename is null or empty.");
            string nameWithoutExt = Path.GetFileNameWithoutExtension(filename);
            using (StreamReader fileStream = File.OpenText(Path.Combine(dirName, nameWithoutExt + ".fnt")))
            {
                rstFont.Deserialize(fileStream);
            }
            string bitmapFilename = Path.Combine(dirName, nameWithoutExt + "-Normal." + imageExtension);
            rstFont.normalBmp = new Bitmap(bitmapFilename);

            bitmapFilename = Path.Combine(dirName, nameWithoutExt + "-Sdf." + imageExtension);
            if (File.Exists(bitmapFilename))
                rstFont.sdfBmp = new Bitmap(bitmapFilename);

            bitmapFilename = Path.Combine(dirName, nameWithoutExt + "-Shadow." + imageExtension);
            if (File.Exists(bitmapFilename))
                rstFont.shadowBmp = new Bitmap(bitmapFilename);

            return rstFont;
        }

        public static TextureFont CreateNew(Font font, FontGenerationConfig config)
        {
            TextureFont rstFont = new TextureFont();
            rstFont.ascender = font.FontFamily.GetCellAscent(font.Style);
            rstFont.descender = font.FontFamily.GetCellDescent(font.Style);
            rstFont.lineSpacing = font.FontFamily.GetLineSpacing(font.Style);
            rstFont.emHeight = font.FontFamily.GetEmHeight(font.Style);
            rstFont.fontName = font.Name;
            rstFont.size = font.Size;
            rstFont.margin = config.GlyphMargin;
            SizeStatistics stats = new SizeStatistics();
            rstFont.normalBmp = TextureFont.CreateBitmap(font, config, out rstFont.glyphs, out stats, out rstFont.sdfBmp);
            rstFont.naturallyMonospaced = IsMonospaced(stats.MaxSize, stats.MinSize);
            rstFont.meanGlyphWidth = stats.SumSize.Width / (float)rstFont.glyphs.Length + 2 * config.GlyphMargin;
            rstFont.CalculateKerning(config);
            rstFont.CreateShadowBitmap(config.BlurRadius, config.BlurPasses);
            return rstFont;
        }


        public void Save(string filename, string dirName = "Resources/Fonts", ImageFormat format = null)
        {
            if (format == null)
                format = ImageFormat.Png;
            string fileExtension = format.ToString().ToLower();

            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("Filename is null or empty.");
            string nameWithoutExt = Path.GetFileNameWithoutExtension(filename);
            if (!Directory.Exists(dirName)) Directory.CreateDirectory(dirName);

            using (StreamWriter fileStream = File.CreateText(Path.Combine(dirName, nameWithoutExt + ".fnt")))
            {
                this.Serialize(fileStream);
            }
            this.normalBmp.Save(Path.Combine(dirName, nameWithoutExt + "-Normal." + fileExtension), format);

            if (this.sdfBmp != null)
                this.sdfBmp.Save(Path.Combine(dirName, nameWithoutExt + "-Sdf." + fileExtension), format);

            if (this.shadowBmp != null)
                this.shadowBmp.Save(Path.Combine(dirName, nameWithoutExt + "-Shadow." + fileExtension), format);
        }

        private static SizeF GetMaxGlyphSize(Font font, char[] charset)
        {
            Bitmap bmp = new Bitmap(512, 512, PixelFormat.Format24bppRgb);
            Graphics graph = Graphics.FromImage(bmp);
            SizeF maxSize = new SizeF(0f, 0f);
            for (int i = 0; i < charset.Length; i++)
            {
                var charSize = graph.MeasureString("" + charset[i], font);
                if (charSize.Width > maxSize.Width)
                    maxSize.Width = charSize.Width;

                if (charSize.Height > maxSize.Height)
                    maxSize.Height = charSize.Height;
            }
            graph.Dispose();
            bmp.Dispose();

            return maxSize;
        }
        private struct SizeStatistics
        {
            public SizeF MaxSize;
            public SizeF MinSize;
            public SizeF SumSize;
        }

        //The initial bitmap is simply a long thin strip of all glyphs in a row
        private static Bitmap CreateInitialBitmap(Font font, int initialMargin, TextRenderingHint renderHint, char[] charSet, out TextureGlyph[] glyphs,
                                                  out SizeStatistics stats)
        {
            // Initial bitmap generation
            glyphs = new TextureGlyph[charSet.Length];
            var maxGlyphSize = GetMaxGlyphSize(font, charSet);
            int spacing = (int)Math.Ceiling(maxGlyphSize.Width) + 2 * initialMargin;
            Bitmap bmp = new Bitmap(spacing * charSet.Length, (int)Math.Ceiling(maxGlyphSize.Height) + 2 * initialMargin, PixelFormat.Format24bppRgb);
            Graphics graph = Graphics.FromImage(bmp);
            graph.TextRenderingHint = renderHint;

            int xOffset = initialMargin;
            for (int i = 0; i < charSet.Length; i++)
            {
                graph.DrawString("" + charSet[i], font, Brushes.White, xOffset, initialMargin);
                var charSize = graph.MeasureString("" + charSet[i], font);
                glyphs[i] = new TextureGlyph()
                {
                    charcode = charSet[i],
                    id = 0,
                    X = xOffset - initialMargin,
                    Y = 0,
                    Width = (int)charSize.Width + initialMargin * 2,
                    Height = (int)charSize.Height + initialMargin * 2,
                    offset_x = 0,
                    offset_y = 0,
                };
                xOffset += (int)charSize.Width + initialMargin * 2;
            }

            graph.Flush();
            graph.Dispose();
#if DEBUG
            bmp.Save("InitialBmp.png", ImageFormat.Png);
#endif
            // Compute stadistics 
            var initialBitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            int minYOffset = int.MaxValue;
            stats.MaxSize = new SizeF(0f, 0f);
            stats.MinSize = new SizeF(float.MaxValue, float.MaxValue);
            stats.SumSize = new SizeF(0f, 0f);
            for (int i = 0; i < charSet.Length; i++)
            {
                glyphs[i].Rect = ComputeGlyphRectangle(initialBitmapData, glyphs[i], true, 0);
                minYOffset = Math.Min(minYOffset, glyphs[i].offset_y);
                // Compute max sizes
                if (glyphs[i].Rect.Width > stats.MaxSize.Width)
                    stats.MaxSize.Width = glyphs[i].Rect.Width;

                if (glyphs[i].Rect.Height > stats.MaxSize.Height)
                    stats.MaxSize.Height = glyphs[i].Rect.Height;

                // Compute min sizes
                if (glyphs[i].Rect.Width < stats.MinSize.Width)
                    stats.MinSize.Width = glyphs[i].Rect.Width;

                if (glyphs[i].Rect.Height < stats.MinSize.Height)
                    stats.MinSize.Height = glyphs[i].Rect.Height;

                // compute sum 
                stats.SumSize.Width += glyphs[i].Rect.Width;
                stats.SumSize.Height += glyphs[i].Rect.Height;
            }
            minYOffset--; //give one pixel of breathing room?

            foreach (var glyph in glyphs)
                glyph.offset_y -= minYOffset;

            bmp.UnlockBits(initialBitmapData);
            return bmp;
        }

        private static Bitmap CreateBitmap(Font font, FontGenerationConfig config, out TextureGlyph[] glyphs, out SizeStatistics stats, out Bitmap bmpSdf)
        {
            char[] charSet = config.CharSet.ToArray();
            TextureGlyph[] initialGlyphs;
            int initialMargin = 2;
            Bitmap initialBmp = CreateInitialBitmap(font, initialMargin, config.RenderHint, charSet, out initialGlyphs, out stats);
            glyphs = new TextureGlyph[charSet.Length];

            int margin = config.GlyphMargin;

            // Repack the bitmap
            int atlasWidth = config.PageWidth;
            int atlasHeight = config.PageHeight;
            if (atlasWidth <= 0 || atlasHeight <= 0)
            {
                int maxCharPerRow = (int)Math.Ceiling(Math.Sqrt(charSet.Length));
                int spacingH = (int)Math.Ceiling((stats.SumSize.Width + margin));
                int spacingV = (int)Math.Ceiling((stats.SumSize.Height + margin));
                atlasWidth = spacingH / maxCharPerRow;
                atlasHeight = spacingV / maxCharPerRow;
            }
            if (config.ForcePowerOfTwo)
            {
                atlasWidth = UpperPowerOfTwo(atlasWidth);
                atlasHeight = UpperPowerOfTwo(atlasHeight);
            }
            TextureAtlas atlas = new TextureAtlas(atlasWidth, atlasHeight);
            Bitmap bmp = new Bitmap(atlasWidth, atlasHeight, PixelFormat.Format24bppRgb);
            FastBitmap fastbmp = new FastBitmap(bmp);
            fastbmp.LockBitmap();
            Graphics graph = Graphics.FromImage(bmp);

            var initialBitmapData = initialBmp.LockBits(new Rectangle(0, 0, initialBmp.Width, initialBmp.Height), ImageLockMode.ReadOnly, initialBmp.PixelFormat);
            int xOffset = initialMargin;
            for (int i = 0; i < charSet.Length; i++)
            {
                var charSize = initialGlyphs[i].Rect;
                var dstrec = atlas.GetNewRegion(charSize.Width, charSize.Height, margin);
                fastbmp.SetDataRegion(initialBitmapData, charSize, dstrec, margin);
                glyphs[i] = new TextureGlyph()
                {
                    charcode = charSet[i],
                    id = 0,
                    Rect = dstrec, // new Rectangle(dstrec.X + margin, dstrec.Y + margin, charSize.Width, charSize.Height),
                    offset_x = initialGlyphs[i].offset_x,
                    offset_y = initialGlyphs[i].offset_y,
                };
            }
            initialBmp.UnlockBits(initialBitmapData);
            graph.Flush();
            graph.Dispose();
            bmpSdf = fastbmp.MakeDistanceMap();
            fastbmp.UnlockBitmap();
#if DEBUG
            bmp.Save("PackedAtlas.png", ImageFormat.Png);
            bmpSdf.Save("SdfAtlas.png", ImageFormat.Png);
#endif
            return bmp;
        }

        private void CreateShadowBitmap(int radius, int passes)
        {
            this.shadowBmp = this.normalBmp.Clone(new Rectangle(0, 0, this.normalBmp.Width, this.normalBmp.Height), this.normalBmp.PixelFormat);
            FastBitmap fastbmp = new FastBitmap(this.shadowBmp);
            fastbmp.BlurImage(radius, passes);
#if DEBUG
            this.shadowBmp.Save("ShadowAtlas.png", ImageFormat.Png);
#endif

        }

       

        private delegate bool EmptyDel(BitmapData data, int x, int y);
        private static Rectangle ComputeGlyphRectangle(BitmapData bitmapData, TextureGlyph glyph, bool setYOffset, byte alphaTolerance)
        {
            int startX, endX;
            int startY, endY;

            var rect = glyph.Rect;

            EmptyDel emptyPix;

            if (bitmapData.PixelFormat == PixelFormat.Format32bppArgb)
                emptyPix = delegate (BitmapData data, int x, int y) { return FastBitmap.EmptyAlphaPixel(data, x, y, alphaTolerance); };
            else
                emptyPix = delegate (BitmapData data, int x, int y) { return FastBitmap.EmptyPixel(data, x, y); };

            unsafe
            {
                for (startX = rect.X; startX < bitmapData.Width; startX++)
                    for (int j = rect.Y; j < rect.Y + rect.Height; j++)
                        if (!emptyPix(bitmapData, startX, j))
                            goto Done1;
                Done1:

                for (endX = rect.X + rect.Width; endX >= 0; endX--)
                    for (int j = rect.Y; j < rect.Y + rect.Height; j++)
                        if (!emptyPix(bitmapData, endX, j))
                            goto Done2;
                Done2:

                for (startY = rect.Y; startY < bitmapData.Height; startY++)
                    for (int i = startX; i < endX; i++)
                        if (!emptyPix(bitmapData, i, startY))
                            goto Done3;

                Done3:

                for (endY = rect.Y + rect.Height; endY >= 0; endY--)
                    for (int i = startX; i < endX; i++)
                        if (!emptyPix(bitmapData, i, endY))
                            goto Done4;
                Done4:;
            }

            if (endY < startY)
                startY = endY = rect.Y;

            if (endX < startX)
                startX = endX = rect.X;

            Rectangle rectRst = new Rectangle(startX, startY, endX - startX + 1, endY - startY + 1);

            if (setYOffset)
                glyph.offset_y = rectRst.Y;

            return rectRst;
        }



        #region Kerning Methods
        private void CalculateKerning(FontGenerationConfig config)
        {
            kerningPairs = new Dictionary<String, int>();
            var page = new FastBitmap(normalBmp);
            page.LockBitmap();
            //we start by computing the index of the first and last non-empty pixel in each row of each glyph
            XLimits[][] limits = new XLimits[glyphs.Length][];
            int maxHeight = 0;
            for (int n = 0; n < glyphs.Length; n++)
            {
                var rect = glyphs[n].Rect;

                limits[n] = new XLimits[rect.Height];

                maxHeight = Math.Max(rect.Height, maxHeight);

                int yStart = rect.Y;
                int yEnd = rect.Y + rect.Height;
                int xStart = rect.X;
                int xEnd = rect.X + rect.Width;

                for (int j = yStart; j < yEnd; j++)
                {
                    int last = xStart;

                    bool yetToFindFirst = true;
                    for (int i = xStart; i < xEnd; i++)
                    {
                        if (!FastBitmap.EmptyAlphaPixel(page.bitmapData, i, j, config.AlphaEmptyPixelTolerance))
                        {

                            if (yetToFindFirst)
                            {
                                limits[n][j - yStart].Min = i - xStart;
                                yetToFindFirst = false;
                            }
                            last = i;
                        }
                    }

                    limits[n][j - yStart].Max = last - xStart;

                    if (yetToFindFirst)
                        limits[n][j - yStart].Min = xEnd - 1;
                }
            }


            //we now bring up each row to the max (or min) of it's two adjacent rows, this is to stop glyphs sliding together too closely
            var tmp = new XLimits[maxHeight];

            for (int n = 0; n < glyphs.Length; n++)
            {
                //clear tmp 
                for (int j = 0; j < limits[n].Length; j++)
                    tmp[j] = limits[n][j];

                for (int j = 0; j < limits[n].Length; j++)
                {
                    if (j != 0)
                    {
                        tmp[j].Min = Math.Min(limits[n][j - 1].Min, tmp[j].Min);
                        tmp[j].Max = Math.Max(limits[n][j - 1].Max, tmp[j].Max);
                    }

                    if (j != limits[n].Length - 1)
                    {
                        tmp[j].Min = Math.Min(limits[n][j + 1].Min, tmp[j].Min);
                        tmp[j].Max = Math.Max(limits[n][j + 1].Max, tmp[j].Max);
                    }

                }

                for (int j = 0; j < limits[n].Length; j++)
                    limits[n][j] = tmp[j];

            }

            for (int i = 0; i < glyphs.Length; i++)
                for (int j = 0; j < glyphs.Length; j++)
                    kerningPairs.Add("" + glyphs[i].charcode + glyphs[j].charcode, 1 - Kerning(glyphs[i], glyphs[j], limits[i], limits[j], config));
            page.UnlockBitmap();
        }
        private struct XLimits
        {
            public int Min;
            public int Max;
        }
        private static int Kerning(TextureGlyph g1, TextureGlyph g2, XLimits[] lim1, XLimits[] lim2, FontGenerationConfig config)
        {
            int yOffset1 = g1.offset_y;
            int yOffset2 = g2.offset_y;

            int startY = Math.Max(yOffset1, yOffset2);
            int endY = Math.Min(g1.Rect.Height + yOffset1, g2.Rect.Height + yOffset2);

            int w1 = g1.Rect.Width;

            int worstCase = w1;

            //TODO - offset startY, endY by yOffset1 so that lim1[j-yOffset1] can be written as lim1[j], will need another var for yOffset2

            for (int j = startY; j < endY; j++)
                worstCase = Math.Min(worstCase, w1 - lim1[j - yOffset1].Max + lim2[j - yOffset2].Min);


            worstCase = Math.Min(worstCase, g1.Rect.Width);
            worstCase = Math.Min(worstCase, g2.Rect.Width);


            //modify by character kerning rules
            CharacterKerningRule kerningRule = config.GetOverridingCharacterKerningRuleForPair(g1.charcode, g2.charcode);
            if (kerningRule == CharacterKerningRule.Zero)
            {
                return 0;
            }
            else if (kerningRule == CharacterKerningRule.NotMoreThanHalf)
            {
                return (int)Math.Min(Math.Min(g1.Rect.Width, g2.Rect.Width) * 0.5f, worstCase);
            }


            return worstCase;
        }

        public int GetKerningPairCorrection(int index, string text)
        {
            if (this.KerningPairs == null)
                return 0;

            var chars = new char[2];

            if (index + 1 == text.Length)
            {
                return 0;
            }
            else
            {
                chars[1] = text[index + 1];
            }

            chars[0] = text[index];

            String str = new String(chars);

            if (this.KerningPairs.ContainsKey(str))
                return this.KerningPairs[str];

            return 0;
        }
        #endregion

        private static int UpperPowerOfTwo(int num)
        {
            int v = num > 0 ? num - 1 : 0;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v++;
            return v;
        }
        #region Monospacing Methods

        public bool IsMonospacingActive(FontRenderOptions options)
        {
            return (options.Monospacing == FontMonospacing.Natural && this.IsNaturallyMonospaced) || options.Monospacing == FontMonospacing.Yes;
        }

        public float GetMonoSpaceWidth(FontRenderOptions options)
        {
            return (float)Math.Ceiling(1 + (1 + options.CharacterSpacing) * this.MeanGlyphWidth);
        }

        /// <summary>
        /// Returns true if all glyph widths are within 5% of each other
        /// </summary>
        /// <param name="sizes"></param>
        /// <returns></returns>
        private static bool IsMonospaced(SizeF max, SizeF min)
        {
            if (max.Width - min.Width < max.Width * 0.05f)
                return true;

            return false;
        }
        #endregion

        #region Serialization Methods
        private void Serialize(TextWriter stream)
        {
            CultureInfo invC = CultureInfo.InvariantCulture;

            stream.WriteLine(this.fontName);
            stream.WriteLine("{0},{1},{2},{3},{4}",
                this.size.ToString(invC),
                this.emHeight.ToString(invC),
                this.ascender.ToString(invC),
                this.descender.ToString(invC),
                this.lineSpacing.ToString(invC));
            stream.WriteLine(this.naturallyMonospaced.ToString(invC));
            stream.WriteLine(this.meanGlyphWidth.ToString(invC));
            stream.WriteLine(this.margin.ToString(invC));
            stream.WriteLine(this.glyphs.Length.ToString(invC));
            foreach (var glyph in this.glyphs)
            {
                stream.WriteLine("{0},{1},{2},{3},{4},{5},{6}",
                            glyph.charcode,
                            glyph.Rect.X.ToString(invC),
                            glyph.Rect.Y.ToString(invC),
                            glyph.Rect.Width.ToString(invC),
                            glyph.Rect.Height.ToString(invC),
                            glyph.offset_x.ToString(invC),
                            glyph.offset_y.ToString(invC));
            }
            stream.WriteLine(this.kerningPairs.Count.ToString(invC));
            foreach (var kerningPair in this.kerningPairs)
            {
                stream.WriteLine("{0},{1}",
                            kerningPair.Key,
                            kerningPair.Value.ToString(invC));
            }
        }
        private void Deserialize(TextReader stream)
        {
            CultureInfo invC = CultureInfo.InvariantCulture;

            this.fontName = stream.ReadLine();
            string[] parts = stream.ReadLine().Split(',');
            this.size = float.Parse(parts[0], invC);
            this.emHeight = int.Parse(parts[1], invC);
            this.ascender = float.Parse(parts[2], invC);
            this.descender = float.Parse(parts[3], invC);
            this.lineSpacing = float.Parse(parts[4], invC);
            this.naturallyMonospaced = bool.Parse(stream.ReadLine());
            this.meanGlyphWidth = float.Parse(stream.ReadLine(), invC);
            this.margin = int.Parse(stream.ReadLine(), invC);
            int len = int.Parse(stream.ReadLine(), invC);
            this.glyphs = new TextureGlyph[len];
            for (int i = 0; i < len; i++)
            {
                TextureGlyph glyph = new TextureGlyph();
                string line = stream.ReadLine();
                glyph.charcode = line[0];
                parts = line.Substring(2, line.Length - 2).Split(',');
                glyph.Rect.X = int.Parse(parts[0], invC);
                glyph.Rect.Y = int.Parse(parts[1], invC);
                glyph.Rect.Width = int.Parse(parts[2], invC);
                glyph.Rect.Height = int.Parse(parts[3], invC);
                glyph.offset_x = int.Parse(parts[4], invC);
                glyph.offset_y = int.Parse(parts[5], invC);
                glyphs[i] = glyph;
            }
            this.kerningPairs = new Dictionary<string, int>();
            len = int.Parse(stream.ReadLine(), invC);
            for (int i = 0; i < len; i++)
            {
                string line = stream.ReadLine();
                int k = int.Parse(line.Substring(3, line.Length - 3), invC);
                this.kerningPairs.Add(line.Substring(0, 2), k);
            }
        }

        #endregion
    }
}
