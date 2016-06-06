using Sxta.Core;
using Sxta.Math;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Sxta.Render.OpenGLExt
{

    /// <summary>
    /// Helper class to draw text in a given font.
    /// A Font allows to easily write a line of text directly in a FrameBuffer
    /// at a given position. It has a texture containing ascii chars, and knows
    /// which can be displayed. Any character outside its range will be displayed
    /// as a blank character defined in the texture (for example a square, or a
    /// question mark).
    /// </summary>
    public class Font : ISwappable<Font>
    {
        /// <summary>
        /// Vertex format for a text mesh.
        /// </summary>
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct Vertex
        {

            public Vector4f pos_uv;

            public byte r, g, b, a;

            public Vertex(Vector4f pos_uv, uint color)
            {
                this.pos_uv = pos_uv;
                r = (byte)(color >> 24);
                g = (byte)(color >> 16);
                b = (byte)(color >> 8);
                a = (byte)(color);
            }
            public static int SizeInBytes
            {
                get { return Vector4f.SizeInBytes + 4 * sizeof(byte); }
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Sxta.Render.OpenGLExt.Font"/> class.
        /// </summary>
        /// <param name='fontTex'>
        /// fontTex the Texture2D which contains the images of the font.
        /// </param>
        /// <param name='nCols'>
        /// nCols the number of characters columns in the texture.
        /// </param>
        /// <param name='nRows'>
        /// nRows the number of characters rows in the texture.
        /// </param>
        /// <param name='minChar'>
        /// minChar the first ascii char code to take into account.
        /// </param>
        /// <param name='maxChar'>
        /// maxChar the last ascii char code to take into account.
        /// </param>
        /// <param name='invalidChar'>
        /// invalidChar the character to be used to display invalid characters.
        /// </param>
        /// <param name='fixedWidth'>
        /// fixedWidth whether the font is fixed-width (faster draw call, disallow overlapping characters)
        /// </param>
        /// <param name='charWidths'>
        /// charWidths an array of (maxChar - minChar + 1) character widths in texel
        /// NOTE: charWidth does NOT give texture coordinates, it gives spaces between characters.
        /// Lower chars will be replaced with 'invalidChar'.
        /// Higher chars will be replaced with 'invalidChar'.
        /// </param>
        public Font(Texture2D fontTex, int nCols, int nRows, int minChar, int maxChar, int invalidChar, bool fixedWidth, List<int> charWidths)
        {
            init(fontTex, nCols, nRows, minChar, maxChar, invalidChar, fixedWidth, charWidths);
        }


        /// <summary>
        /// Deletes this Font.
        /// </summary>
        /// <returns>
        /// Returns the texture containing the image of this font.
        /// </returns>
        public Texture2D getImage()
        {
            return fontTex;
        }


        /// <summary>
        /// Returns the width of a character tile.
        /// </summary>
        /// <returns>
        /// The tile width.
        /// </returns>
        public float getTileWidth()
        {
            return (float)fontTex.getWidth() / nCols;
        }


        /// <summary>
        /// Gets the height of the tile.
        /// </summary>
        /// <returns>
        /// Returns the height of a character tile.
        /// </returns>
        public float getTileHeight()
        {
            return (float)fontTex.getHeight() / nRows;
        }


        /// <summary>
        /// Returns the aspect ratio of a character tile.
        /// </summary>
        /// <returns>
        /// The tile aspect ratio.
        /// </returns>
        public float getTileAspectRatio()
        {
            return getTileWidth() / getTileHeight();
        }

        /// <summary>
        /// Returns the space between this char and the next one.
        ///  Approximate the width of the drawed char (it's a bit different for legibility purpose).
        /// </summary>
        /// <returns>
        /// The char width.
        /// </returns>
        /// <param name='c'>
        /// C.
        /// </param>
		public float getCharWidth(char c)
        {
            return charWidths[charCount(c)];
        }

        /// <summary>
        /// Returns the size of a given line of text.
        /// </summary>
        /// <returns>
        /// The size.
        /// </returns>
        /// <param name='line'>
        /// The line of text.
        /// </param>
        /// <param name='height'>
        /// the height of output text in pixels
        /// </param>
        public Vector2f getSize(string line, float height)
        {
            float res = 0.0f;
            int lineSize = line.Length;

            for (int i = 0; i < lineSize; ++i)
            {
                int index = charCount(line[i]);
                int width = charWidths[index];
                res += (height * width) / getTileWidth();
            }
            return new Vector2f(res, height);
        }



        /// <summary>
        /// Add a given line of text in a given Mesh and returns the final
        /// position of the line.
        /// </summary>
        /// <returns>
        /// The line.
        /// </returns>
        /// <param name='v'>
        /// viewport the framebuffer viewport, in pixels.
        /// </param>
        /// <param name='xs'>
        /// xs the x coordinate of the first character to display.
        /// </param>
        /// <param name='ys'>
        /// ys the y coordinate of the first character to display.
        /// </param>
        /// <param name='line'>
        /// line the line of text to display.
        /// </param>
        /// <param name='height'>
        /// height height of a char in pixels.
        /// </param>
        /// <param name='color'>
        /// Color color of this line of text (in RGBA8 format).
        /// </param>
        /// <param name='textMesh'>
        /// Text meshthe mesh to write into.
        /// </param>
        public Vector2f addLine(Vector4i v, float xs, float ys, string line, float height,
                                uint color, Mesh<Vertex, uint> textMesh)
        {
            return addLine(new Vector4f(v.X, v.Y, v.Z, v.W), xs, ys, line, height, color, textMesh);
        }
        public Vector2f addLine(Vector4f viewport, float xs, float ys, string line, float height,
                                uint color, Mesh<Vertex, uint> textMesh)
        {
            for (int i = 0; i < line.Length; ++i)
            {

                int index = charCount(line[i]);
                int width = charWidths[index];

                // use isFixedSize to determine
                // If isFixedSize == true, draw smaller non-overlapping quads
                //                == false, draw overlapping quads to allow overlapping characters

                float charRatio = fixedWidth ? (width / (float)getTileWidth()) : 1.0f;

                int x = index % nCols;
                int y = index / nCols;
                y = (nRows - 1) - y;

                float u0 = x / (float)nCols;
                float u1 = (x + charRatio) / (float)nCols;
                float v0 = y / (float)nRows;
                float v1 = (y + 1.0f) / (float)nRows;

                float tileAspectRatio = getTileAspectRatio();

                float xs0 = xs / viewport.Z;
                float xs1 = (xs + charRatio * tileAspectRatio * height) / viewport.Z;
                float ys0 = ys / viewport.W;
                float ys1 = (ys + height) / viewport.W;

                Vector4f pos_uv0 = new Vector4f(xs0 * 2.0f - 1.0f, 1.0f - ys1 * 2.0f, u0, v0);
                Vector4f pos_uv1 = new Vector4f(xs1 * 2.0f - 1.0f, 1.0f - ys1 * 2.0f, u1, v0);
                Vector4f pos_uv2 = new Vector4f(xs1 * 2.0f - 1.0f, 1.0f - ys0 * 2.0f, u1, v1);
                Vector4f pos_uv3 = new Vector4f(xs0 * 2.0f - 1.0f, 1.0f - ys0 * 2.0f, u0, v1);

                textMesh.addVertex(new Vertex(pos_uv0, color));
                textMesh.addVertex(new Vertex(pos_uv1, color));
                textMesh.addVertex(new Vertex(pos_uv2, color));
                textMesh.addVertex(new Vertex(pos_uv2, color));
                textMesh.addVertex(new Vertex(pos_uv3, color));
                textMesh.addVertex(new Vertex(pos_uv0, color));

                xs += (height * width) / getTileWidth();
            }

            return new Vector2f(xs, ys);
        }


        /// <summary>
        /// Add a given line of text in a given Mesh centered at a given
        /// position and returns the size of the line.
        /// </summary>
        /// <returns>
        /// The centered line.
        /// </returns>
        /// <param name='viewport'>
        /// Viewport the framebuffer viewport, in pixels.
        /// </param>
        /// <param name='xs'>
        /// Xs the x coordinate of the center of the line to display.
        /// </param>
        /// <param name='ys'>
        /// Ys the y coordinate of the center of the line to display.
        /// </param>
        /// <param name='line'>
        /// Line the line of text to display.
        /// </param>
        /// <param name='height'>
        /// Height of a char in pixels.
        /// </param>
        /// <param name='color'>
        /// Color of this line of text (in RGBA8 format).
        /// </param>
        /// <param name='textMesh'>
        /// TextMesh the mesh to write into.
        /// </param>
        public Vector2f addCenteredLine(Vector4f viewport, float xs, float ys, string line, float height,
                uint color, Mesh<Vertex, uint> textMesh)
        {
            Vector2f size = getSize(line, height);
            xs -= size.X * 0.5f;
            addLine(viewport, xs, ys, line, height, color, textMesh);
            return size;
        }


        /// <summary>
        /// The Texture2D which contains the images of the font.
        /// This texture is splitted in rows and colums which form "tiles".
        /// </summary>
        protected Texture2D fontTex;

        /// <summary>
        /// The number of character columns in the texture.
        /// </summary>
        protected int nCols;

        /// <summary>
        /// The number of character rows in the texture.
        /// </summary>
        protected int nRows;

        /// <summary>
        /// The first ascii char code to take into account.
        /// Lower chars will be replaced with #invalidChar.
        /// </summary>
        protected int minChar;


        /// <summary>
        /// The last ascii char code to take into account.
        /// Higher chars will be replaced with #invalidChar.
        /// </summary>
        protected int maxChar;


        /// <summary>
        /// The character to be used to display invalid characters.
        /// Must be between #minChar and #maxChar.
        /// </summary>
        protected int invalidChar;


        /// <summary>
        /// If the font has fixed-width characters.
        /// </summary>
        protected bool fixedWidth;

        /// <summary>
        /// The width of a char (in texels).
        /// Allows to have good-looking variable sized fonts.
        /// The ratio width/height of a given character is logicalWidth[c] / (textureHeight /
        /// </summary>
		protected List<int> charWidths;

        /// <summary> 
        /// Creates an uninitialized Font.
        /// Initializes a new instance of the <see cref="Sxta.Render.OpenGLExt.Font"/> class.
        /// </summary>
        public Font() { }


        /// <summary>
        /// Init the specified fontTex, nCols, nRows, minChar, maxChar, invalidChar, fixedWidth and charWidths.
        /// </summary>
        /// <param name='fontTex'>
        /// fontTex the Texture2D which contains the images of the font.
        /// </param>
        /// <param name='nCols'>
        /// N cols the number of characters columns in the texture.
        /// </param>
        /// <param name='nRows'>
        /// N rows the number of characters rows in the texture.
        /// </param>
        /// <param name='minChar'>
        /// Minimum char the first ascii char code to take into account.
        /// Lower chars will be replaced with 'invalidChar'.
        /// </param>
        /// <param name='maxChar'>
        /// Max char the last ascii char code to take into account.
        /// Higher chars will be replaced with 'invalidChar'.
        /// </param>
        /// <param name='invalidChar'>
        /// Invalid char the character to be used to display invalid characters.
        /// </param>
        /// <param name='fixedWidth'>
        /// Fixed width whether the font is fixed-width (faster draw call, disallow overlapping characters)
        /// </param>
        /// <param name='charWidths'>
        /// Char widths an array of (maxChar - minChar + 1) character widths in texel
        /// NOTE: charWidth does NOT give texture coordinates, it gives spaces between characters.
        /// </param>
        public virtual void init(Texture2D fontTex, int nCols, int nRows, int minChar, int maxChar, int invalidChar, bool fixedWidth, List<int> charWidths)
        {
            this.fontTex = fontTex;
            this.nCols = nCols;
            this.nRows = nRows;
            this.minChar = minChar;
            this.maxChar = maxChar;
            this.invalidChar = invalidChar;
            this.charWidths = charWidths;
            this.fixedWidth = fixedWidth;
            Debug.Assert(charWidths.Count == 1 + maxChar - minChar);
        }


        /// <summary>
        /// Get the tile index for this character.
        /// Force the last character if c is not supported by this Font.
        /// </summary>
        /// <returns>
        /// The count.
        /// </returns>
        /// <param name='c'>
        /// C.
        /// </param>
        protected int charCount(char c)
        {
            int i = c;
            if (i < minChar)
            {
                return invalidChar - minChar;
            }
            if (i > maxChar)
            {
                return invalidChar - minChar;
            }
            return i - minChar;
        }

        protected virtual void swap(Font t)
        {
            Std.Swap(ref fontTex, ref t.fontTex);
            Std.Swap(ref nCols, ref t.nCols);
            Std.Swap(ref nRows, ref t.nRows);
            Std.Swap(ref minChar, ref t.minChar);
            Std.Swap(ref maxChar, ref t.maxChar);
            Std.Swap(ref invalidChar, ref t.invalidChar);
            Std.Swap(ref fixedWidth, ref t.fixedWidth);
            Std.Swap(ref charWidths, ref t.charWidths);
        }

        void ISwappable<Font>.swap(Font obj)
        {
            throw new NotImplementedException();
        }
    }
}
