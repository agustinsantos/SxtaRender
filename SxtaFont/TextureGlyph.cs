using System;
using System.Collections.Generic;
using System.Drawing;

namespace SxtaRender.Fonts
{
    /*
     * Glyph metrics:
     * --------------
     *
     *                       xmin                     xmax
     *                        |                         |
     *                        |<-------- width -------->|
     *                        |                         |
     *              |         +-------------------------+----------------- ymax
     *              |         |    ggggggggg   ggggg    |     ^        ^
     *              |         |   g:::::::::ggg::::g    |     |        |
     *              |         |  g:::::::::::::::::g    |     |        |
     *              |         | g::::::ggggg::::::gg    |     |        |
     *              |         | g:::::g     g:::::g     |     |        |
     *    offset_x -|-------->| g:::::g     g:::::g     |  offset_y    |
     *              |         | g:::::g     g:::::g     |     |        |
     *              |         | g::::::g    g:::::g     |     |        |
     *              |         | g:::::::ggggg:::::g     |     |        |
     *              |         |  g::::::::::::::::g     |     |      height
     *              |         |   gg::::::::::::::g     |     |        |
     *  baseline ---*---------|---- gggggggg::::::g-----*--------      |
     *            / |         |             g:::::g     |              |
     *     origin   |         | gggggg      g:::::g     |              |
     *              |         | g:::::gg   gg:::::g     |              |
     *              |         |  g::::::ggg:::::::g     |              |
     *              |         |   gg:::::::::::::g      |              |
     *              |         |     ggg::::::ggg        |              |
     *              |         |         gggggg          |              v
     *              |         +-------------------------+----------------- ymin
     *              |                                   |
     *              |------------- advance_x ---------->|
     */

    /// <summary>
    ///  A structure that describe a glyph.
    /// </summary>
    [Serializable]
    public class TextureGlyph
    {
        /// <summary>
        /// Unicode character this glyph represents
        /// </summary>
        public char charcode;

        /// <summary>
        /// Glyph id (used for display lists)
        /// </summary>
        public uint id;

        /// <summary>
        /// The rectangle defining the glyphs position on the page
        /// </summary>
        public Rectangle Rect = new Rectangle();

         public int X
        {
            get { return Rect.X; }
            set { Rect.X = value; }
        }

        /// <summary>
        ///  Glyph's height in pixels.
        /// </summary>
         public int Y
        {
            get { return Rect.Y; }
            set { Rect.Y = value; }
        }


        /// <summary>
        /// Glyph's width in pixels.
        /// </summary>
        public int Width {
            get { return Rect.Width; }
            set { Rect.Width = value;  }
        }

        /// <summary>
        ///  Glyph's height in pixels.
        /// </summary>
        public int Height
        {
            get { return Rect.Height; }
            set { Rect.Height = value; }
        }

         /// <summary>
        /// Glyph's left bearing expressed in integer pixels.
        /// </summary>
        public int offset_x;

        /// <summary>
        /// Glyphs's top bearing expressed in integer pixels.
        /// 
        /// Remember that this is the distance from the baseline to the top-most
        /// glyph scanline, upwards y coordinates being positive.
        /// </summary>
        public int offset_y;

        /// <summary>
        /// For horizontal text layouts, this is the horizontal distance (in
        /// fractional pixels) used to increment the pen position when the glyph is
        /// drawn as part of a string of text.
        /// </summary>
        public float advance_x;

        /// <summary>
        /// For vertical text layouts, this is the vertical distance (in fractional
        /// pixels) used to increment the pen position when the glyph is drawn as
        /// part of a string of text.
        /// </summary>
        public float advance_y;

        /// <summary>
        /// First normalized texture coordinate (x) of top-left corner
        /// </summary>
        public float s0;

        /// <summary>
        /// Second normalized texture coordinate (y) of top-left corner
        /// </summary>
        public float t0;

        /// <summary>
        /// First normalized texture coordinate (x) of bottom-right corner
        /// </summary>
        public float s1;

        /// <summary>
        /// Second normalized texture coordinate (y) of bottom-right corner
        /// </summary>
        public float t1;

        /// <summary>
        /// A vector of kerning pairs relative to this glyph.
        /// </summary>
        public Dictionary<char, KerningInfo> kerning = new Dictionary<char, KerningInfo>();

        /// <summary>
        /// Glyph outline type (0 = None, 1 = line, 2 = inner, 3 = outer)
        /// </summary>
        public int outline_type;

        /// <summary>
        /// Glyph outline thickness
        /// </summary>
        public float outline_thickness;

        /// <summary>
        /// Get the kerning between two horizontal glyphs.
        /// </summary>
        /// <param name="charcode">codepoint of the peceding glyph</param>
        /// <returns>kerning value</returns>
        public float GetKerning(char charcode)
        {
            KerningInfo info;
            if (kerning.TryGetValue(charcode, out info))
                return info.Kerning;
            else
                return 0;
        }

        public TextureGlyph Clone()
        {
            return (TextureGlyph)this.MemberwiseClone();
        }
    }
}
