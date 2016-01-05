using System;
using System.Linq;
using System.Text;

namespace SxtaRender.Fonts
{

    public class UnicodeSubset
    {
        public string Name;
        public int BeginChar;
        public int EndChar;

        public UnicodeSubset(string name, int begin, int end)
        {
            this.Name = name;
            this.BeginChar = begin;
            this.EndChar = end;
        }

        public static string GetCharset(params string[] charSetNames)
        {
            StringBuilder cs = new StringBuilder();
            foreach (var charsetname in charSetNames)
            {
                var charset = UnicodeSubsets.FirstOrDefault(us => us.Name.Equals(charsetname, StringComparison.InvariantCultureIgnoreCase));
                if (charset != null)
                    for (int i = 0; i <= charset.EndChar - charset.BeginChar; i++)
                    {
                        char c = (char)i;
                        if (!Char.IsControl(c))
                            cs.Append((char)i);
                    }
            }
            return cs.ToString();
        }

        public string GetCharset(params UnicodeSubset[] charSets)
        {
            StringBuilder cs = new StringBuilder();
            foreach (var charset in charSets)
            {
                for (int i = 0; i <= charset.EndChar - charset.BeginChar; i++)
                {
                    char c = (char)i;
                    if (!Char.IsControl(c))
                        cs.Append((char)i);
                }
            }
            return cs.ToString();
        }

        // These are the defined character sets from the Unicode 6.2 standard
        // http://www.unicode.org/charts/PDF/
        public static readonly UnicodeSubset[] UnicodeSubsets = new UnicodeSubset[]{
            new UnicodeSubset("ASCII"                                         , 0x0000  , 0x007F),
            // Plane 0 - Base Multilingual Plane
            new UnicodeSubset("Latin + Latin Supplement"                      , 0x0000  , 0x00FF),
            new UnicodeSubset("Latin Extended A"                              , 0x0100  , 0x017F),
            new UnicodeSubset("Latin Extended B"                              , 0x0180  , 0x024F),
            new UnicodeSubset("IPA Extensions"                                , 0x0250  , 0x02AF),
            new UnicodeSubset("Spacing Modifier Letters"                      , 0x02B0  , 0x02FF),
            new UnicodeSubset("Combining Diacritical Marks"                   , 0x0300  , 0x036F),
            new UnicodeSubset("Greek and Coptic"                              , 0x0370  , 0x03FF),
            new UnicodeSubset("Cyrillic"                                      , 0x0400  , 0x04FF),
            new UnicodeSubset("Cyrillic Supplement"                           , 0x0500  , 0x052F),
            new UnicodeSubset("Armenian"                                      , 0x0530  , 0x058F),
            new UnicodeSubset("Hebrew"                                        , 0x0590  , 0x05FF),
            new UnicodeSubset("Arabic"                                        , 0x0600  , 0x06FF),
            new UnicodeSubset("Syriac"                                        , 0x0700  , 0x074F),
            new UnicodeSubset("Arabic Supplement"                             , 0x0750  , 0x077F),
            new UnicodeSubset("Thaana"                                        , 0x0780  , 0x07BF),
            new UnicodeSubset("N'Ko"                                          , 0x07C0  , 0x07FF),
            new UnicodeSubset("Samaritan"                                     , 0x0800  , 0x083F),
            new UnicodeSubset("Mandaic"                                       , 0x0840  , 0x085F),
            new UnicodeSubset("(0x0860 - 0x08FF)"                             , 0x0860  , 0x08FF),
            new UnicodeSubset("Devanagari"                                    , 0x0900  , 0x097F),
            new UnicodeSubset("Bengali"                                       , 0x0980  , 0x09FF),
            new UnicodeSubset("Gurmukhi"                                      , 0x0A00  , 0x0A7F),
            new UnicodeSubset("Gujarati"                                      , 0x0A80  , 0x0AFF),
            new UnicodeSubset("Oriya"                                         , 0x0B00  , 0x0B7F),
            new UnicodeSubset("Tamil"                                         , 0x0B80  , 0x0BFF),
            new UnicodeSubset("Telugu"                                        , 0x0C00  , 0x0C7F),
            new UnicodeSubset("Kannada"                                       , 0x0C80  , 0x0CFF),
            new UnicodeSubset("Malayalam"                                     , 0x0D00  , 0x0D7F),
            new UnicodeSubset("Sinhala"                                       , 0x0D80  , 0x0DFF),
            new UnicodeSubset("Thai"                                          , 0x0E00  , 0x0E7F),
            new UnicodeSubset("Lao"                                           , 0x0E80  , 0x0EFF),
            new UnicodeSubset("Tibetan"                                       , 0x0F00  , 0x0FFF),
            new UnicodeSubset("Myanmar"                                       , 0x1000  , 0x109F),
            new UnicodeSubset("Georgian"                                      , 0x10A0  , 0x10FF),
            new UnicodeSubset("Hangul Jamo"                                   , 0x1100  , 0x11FF),
            new UnicodeSubset("Ethiopic"                                      , 0x1200  , 0x12BF),
            new UnicodeSubset("(0x12C0 - 0x137F)"                             , 0x12C0  , 0x137F),
            new UnicodeSubset("Ethiopic Supplement"                           , 0x1380  , 0x139F),
            new UnicodeSubset("Cherokee"                                      , 0x13A0  , 0x13FF),
            new UnicodeSubset("Canadian Aboriginal Syllabics"                 , 0x1400  , 0x167F),
            new UnicodeSubset("Ogham"                                         , 0x1680  , 0x169F),
            new UnicodeSubset("Runic"                                         , 0x16A0  , 0x16FF),
            new UnicodeSubset("Tagalog"                                       , 0x1700  , 0x171F),
            new UnicodeSubset("Hanunoo"                                       , 0x1720  , 0x173F),
            new UnicodeSubset("Buhid"                                         , 0x1740  , 0x175F),
            new UnicodeSubset("Tagbanwa"                                      , 0x1760  , 0x177F),
            new UnicodeSubset("Khmer"                                         , 0x1780  , 0x17FF),
            new UnicodeSubset("Mongolian"                                     , 0x1800  , 0x18AF),
            new UnicodeSubset("Unified Canadian Aboriginal Syllabics Extended", 0x18B0  , 0x18FF),
            new UnicodeSubset("Limbu"                                         , 0x1900  , 0x194F),
            new UnicodeSubset("Tai Le"                                        , 0x1950  , 0x197F),
            new UnicodeSubset("New Tai Lue"                                   , 0x1980  , 0x19DF),
            new UnicodeSubset("Khmer Symbols"                                 , 0x19E0  , 0x19FF),
            new UnicodeSubset("Buginese"                                      , 0x1A00  , 0x1A1F),
            new UnicodeSubset("Tai Tham"                                      , 0x1A20  , 0x1AAF),
            new UnicodeSubset("(0x1AB0 - 0x1AFF)"                             , 0x1AB0  , 0x1AFF),
            new UnicodeSubset("Balinese"                                      , 0x1B00  , 0x1B7F),
            new UnicodeSubset("Sundanese"                                     , 0x1B80  , 0x1BBF),
            new UnicodeSubset("Batak"                                         , 0x1BC0  , 0x1BFF),
            new UnicodeSubset("Lepcha"                                        , 0x1C00  , 0x1C4F),
            new UnicodeSubset("Ol Chiki"                                      , 0x1C50  , 0x1C7F),
            new UnicodeSubset("(0x1C80 - 0x1CBF)"                             , 0x1C80  , 0x1CBF),
            new UnicodeSubset("Sundanese Supplement"                          , 0x1CC0  , 0x1CCF),
            new UnicodeSubset("Vedic Extensions"                              , 0x1CD0  , 0x1CFF),
            new UnicodeSubset("Phonetic Extensions"                           , 0x1D00  , 0x1D7F),
            new UnicodeSubset("Phonetic Extensions Supplement"                , 0x1D80  , 0x1DBF),
            new UnicodeSubset("Combining Diacritical Marks Supplement"        , 0x1DC0  , 0x1DFF),
            new UnicodeSubset("Latin Extended Additional"                     , 0x1E00  , 0x1EFF),
            new UnicodeSubset("Greek Extended"                                , 0x1F00  , 0x1FFF),
            new UnicodeSubset("General Punctuation"                           , 0x2000  , 0x206F),
            new UnicodeSubset("Subscripts and Superscripts"                   , 0x2070  , 0x209F),
            new UnicodeSubset("Currency Symbols"                              , 0x20A0  , 0x20CF),
            new UnicodeSubset("Combining Diacritical Marks for Symbols"       , 0x20D0  , 0x20FF),
            new UnicodeSubset("Letterlike Symbols"                            , 0x2100  , 0x214F),
            new UnicodeSubset("Number Forms"                                  , 0x2150  , 0x218F),
            new UnicodeSubset("Arrows"                                        , 0x2190  , 0x21FF),
            new UnicodeSubset("Mathematical Operators"                        , 0x2200  , 0x22FF),
            new UnicodeSubset("Miscellaneous Technical"                       , 0x2300  , 0x23FF),
            new UnicodeSubset("Control Pictures"                              , 0x2400  , 0x243F),
            new UnicodeSubset("Optical Character Recognition"                 , 0x2440  , 0x245F),
            new UnicodeSubset("Enclosed Alphanumerics"                        , 0x2460  , 0x24FF),
            new UnicodeSubset("Box Drawing"                                   , 0x2500  , 0x257F),
            new UnicodeSubset("Block Elements"                                , 0x2580  , 0x259F),
            new UnicodeSubset("Geometric Shapes"                              , 0x25A0  , 0x25FF),
            new UnicodeSubset("Miscellaneous Symbols"                         , 0x2600  , 0x26FF),
            new UnicodeSubset("Dingbats"                                      , 0x2700  , 0x27BF),
            new UnicodeSubset("Miscellaneous Mathematical Symbols A"          , 0x27C0  , 0x27EF),
            new UnicodeSubset("Supplemental Arrows A"                         , 0x27F0  , 0x27FF),
            new UnicodeSubset("Braille"                                       , 0x2800  , 0x28FF),
            new UnicodeSubset("Supplemental Arrows B"                         , 0x2900  , 0x297F),
            new UnicodeSubset("Miscellaneous Mathematical Symbols B"          , 0x2980  , 0x29FF),
            new UnicodeSubset("Supplemental Mathematical Operators"           , 0x2A00  , 0x2AFF),
            new UnicodeSubset("Miscellaneous Symbols and Arrows"              , 0x2B00  , 0x2BFF),
            new UnicodeSubset("Glagolitic"                                    , 0x2C00  , 0x2C5F),
            new UnicodeSubset("Latin Extended C"                              , 0x2C60  , 0x2C7F),
            new UnicodeSubset("Coptic"                                        , 0x2C80  , 0x2CFF),
            new UnicodeSubset("Georgian Supplement"                           , 0x2D00  , 0x2D2F),
            new UnicodeSubset("Tifinagh"                                      , 0x2D30  , 0x2D7F),
            new UnicodeSubset("Ethiopic Extended"                             , 0x2D80  , 0x2DDF),
            new UnicodeSubset("Cyrillic Extended A"                           , 0x2DE0  , 0x2DFF),
            new UnicodeSubset("Supplemental Punctuation"                      , 0x2E00  , 0x2E7F),
            new UnicodeSubset("CJK Radicals Supplement"                       , 0x2E80  , 0x2EFF),
            new UnicodeSubset("KangXi Radicals"                               , 0x2F00  , 0x2FDF),
            new UnicodeSubset("(0x2FE0 - 0x2FEF)"                             , 0x2FE0  , 0x2FEF),
            new UnicodeSubset("Ideographic Description"                       , 0x2FF0  , 0x2FFF),
            new UnicodeSubset("CJK Symbols and Punctuation"                   , 0x3000  , 0x303F),
            new UnicodeSubset("Hiragana"                                      , 0x3040  , 0x309F),
            new UnicodeSubset("Katakana"                                      , 0x30A0  , 0x30FF),
            new UnicodeSubset("Bopomofo"                                      , 0x3100  , 0x312F),
            new UnicodeSubset("Hangul Compatibility Jamo"                     , 0x3130  , 0x318F),
            new UnicodeSubset("Kanbun"                                        , 0x3190  , 0x319F),
            new UnicodeSubset("Extended Bopomofo"                             , 0x31A0  , 0x31BF),
            new UnicodeSubset("CJK Strokes"                                   , 0x31C0  , 0x31EF),
            new UnicodeSubset("Katakana Phonetic Extensions"                  , 0x31F0  , 0x31FF),
            new UnicodeSubset("Enclosed CJK Letters and Months"               , 0x3200  , 0x32FF),
            new UnicodeSubset("CJK Compatibility"                             , 0x3300  , 0x33FF),
            new UnicodeSubset("CJK Unified Ideographs Extension A"            , 0x3400  , 0x4DBF),
            new UnicodeSubset("Yijing Hexagram Symbols"                       , 0x4DC0  , 0x4DFF),
            new UnicodeSubset("CJK Unified Ideographs"                        , 0x4E00  , 0x9FFF),
            new UnicodeSubset("Yi"                                            , 0xA000  , 0xA48F),
            new UnicodeSubset("Yi Radicals"                                   , 0xA490  , 0xA4CF),
            new UnicodeSubset("Lisu"                                          , 0xA4D0  , 0xA4FF),
            new UnicodeSubset("Vai"                                           , 0xA500  , 0xA59F),
            new UnicodeSubset("(0xA600 - 0xA63F)"                             , 0xA600  , 0xA63F),
            new UnicodeSubset("Cyrillic Extended B"                           , 0xA640  , 0xA69F),
            new UnicodeSubset("Bamum"                                         , 0xA6A0  , 0xA6FF),
            new UnicodeSubset("Modifier Tone Letters"                         , 0xA700  , 0xA71F),
            new UnicodeSubset("Latin Extended D"                              , 0xA720  , 0xA7FF),
            new UnicodeSubset("Syloti Nagri"                                  , 0xA800  , 0xA82F),
            new UnicodeSubset("Common Indic Number Forms"                     , 0xA830  , 0xA83F),
            new UnicodeSubset("Phags-Pa"                                      , 0xA840  , 0xA87F),
            new UnicodeSubset("Saurashtra"                                    , 0xA880  , 0xA8DF),
            new UnicodeSubset("Devanagari Extended"                           , 0xA8E0  , 0xA8FF),
            new UnicodeSubset("Kayah Li"                                      , 0xA900  , 0xA92F),
            new UnicodeSubset("Rejang"                                        , 0xA930  , 0xA95F),
            new UnicodeSubset("Hangul Jamo Extended A"                        , 0xA960  , 0xA97F),
            new UnicodeSubset("Javanese"                                      , 0xA980  , 0xA9DF),
            new UnicodeSubset("(0xA9E0 - 0xA9FF)"                             , 0xA9E0  , 0xA9FF),
            new UnicodeSubset("Cham"                                          , 0xAA00  , 0xAA5F),
            new UnicodeSubset("Myanmar Extended A"                            , 0xAA60  , 0xAA7F),
            new UnicodeSubset("Tai Viet"                                      , 0xAA80  , 0xAADF),
            new UnicodeSubset("Meetei Mayek Extensions"                       , 0xAAE0  , 0xAAFF),
            new UnicodeSubset("Ethiopic Extended A"                           , 0xAB00  , 0xAB2F),
            new UnicodeSubset("(0xAB30 - 0xABBF)"                             , 0xAB30  , 0xABBF),
            new UnicodeSubset("Meetei Mayek"                                  , 0xABC0  , 0xABFF),
            new UnicodeSubset("Hangul"                                        , 0xAC00  , 0xD7AF),
            new UnicodeSubset("Hangul Jamo Extended B"                        , 0xD7B0  , 0xD7FF),
            new UnicodeSubset("(High Surrogates)"                             , 0xD800  , 0xDBFF),
            new UnicodeSubset("(Low Surrogates)"                              , 0xDC00  , 0xDFFF),
            new UnicodeSubset("Private Use Area"                              , 0xE000  , 0xF8FF),
            new UnicodeSubset("CJK Compatibility Ideographs"                  , 0xF900  , 0xFAFF),
            new UnicodeSubset("Alphabetical Presentation Forms"               , 0xFB00  , 0xFB4F),
            new UnicodeSubset("Arabic Presentation Forms A"                   , 0xFB50  , 0xFDFF),
            new UnicodeSubset("Variation Selectors"                           , 0xFE00  , 0xFE0F),
            new UnicodeSubset("Vertical Forms"                                , 0xFE10  , 0xFE1F),
            new UnicodeSubset("Combining Half Marks"                          , 0xFE20  , 0xFE2F),
            new UnicodeSubset("CJK Compatibility Forms"                       , 0xFE30  , 0xFE4F),
            new UnicodeSubset("Small Form Variants"                           , 0xFE50  , 0xFE6F),
            new UnicodeSubset("Arabic Presentation Forms B"                   , 0xFE70  , 0xFEFF),
            new UnicodeSubset("Halfwidth and Fullwidth Forms"                 , 0xFF00  , 0xFFEF),
            new UnicodeSubset("Specials"                                      , 0xFFF0  , 0xFFFD),
            new UnicodeSubset("(Reserved)"                                    , 0xFFFE  , 0xFFFF),

            // Plane 1
            new UnicodeSubset("Linear B Syllabary"                       , 0x10000 , 0x1007F),
            new UnicodeSubset("Linear B Ideograms"                       , 0x10080 , 0x100FF),
            new UnicodeSubset("Aegean Numbers"                           , 0x10100 , 0x1013F),
            new UnicodeSubset("Ancient Greek Numbers"                    , 0x10140 , 0x1018F),
            new UnicodeSubset("Ancient Symbols"                          , 0x10190 , 0x101CF),
            new UnicodeSubset("Phaistos Disc"                            , 0x101D0 , 0x101FF),
            new UnicodeSubset("(0x10200 - 0x1027F)"                      , 0x10200 , 0x1027F),
            new UnicodeSubset("Lycian"                                   , 0x10280 , 0x1029F),
            new UnicodeSubset("Carian"                                   , 0x102A0 , 0x102DF),
            new UnicodeSubset("(0x102E0 - 0x102FF)"                      , 0x102E0 , 0x102FF),
            new UnicodeSubset("Old Italic"                               , 0x10300 , 0x1032F),
            new UnicodeSubset("Gothic"                                   , 0x10330 , 0x1034F),
            new UnicodeSubset("(0x10350 - 0x1037F)"                      , 0x10350 , 0x1037F),
            new UnicodeSubset("Ugaritic"                                 , 0x10380 , 0x1039F),
            new UnicodeSubset("Old Persian"                              , 0x103A0 , 0x103DF),
            new UnicodeSubset("(0x103E0 - 0x103FF)"                      , 0x103E0 , 0x103FF),
            new UnicodeSubset("Deseret"                                  , 0x10400 , 0x1044F),
            new UnicodeSubset("Shavian"                                  , 0x10450 , 0x1047F),
            new UnicodeSubset("Osmanya"                                  , 0x10480 , 0x104AF),
            new UnicodeSubset("(0x104B0 - 0x107FF)"                      , 0x104B0 , 0x107FF),
            new UnicodeSubset("Cypriot Syllabary"                        , 0x10800 , 0x1083F),
            new UnicodeSubset("Imperial Aramaic"                         , 0x10840 , 0x1085F),
            new UnicodeSubset("(0x10860 - 0x108FF)"                      , 0x10860 , 0x108FF),
            new UnicodeSubset("Phoenician"                               , 0x10900 , 0x1091F),
            new UnicodeSubset("Lydian"                                   , 0x10920 , 0x1093F),
            new UnicodeSubset("(0x10940 - 0x1099F)"                      , 0x10940 , 0x1099F),
            new UnicodeSubset("Meroitic Cursive"                         , 0x109A0 , 0x109FF),
            new UnicodeSubset("Kharoshthi"                               , 0x10A00 , 0x10A5F),
            new UnicodeSubset("Old South Arabian"                        , 0x10A60 , 0x10A7F),
            new UnicodeSubset("(0x10A80 - 0x10AFF)"                      , 0x10A80 , 0x10AFF),
            new UnicodeSubset("Avestan"                                  , 0x10B00 , 0x10B3F),
            new UnicodeSubset("Inscriptional Parthian"                   , 0x10B40 , 0x10B5F),
            new UnicodeSubset("Inscriptional Pahlavi"                    , 0x10B60 , 0x10B7F),
            new UnicodeSubset("(0x10B80 - 0x10BFF)"                      , 0x10B80 , 0x10BFF),
            new UnicodeSubset("Old Turkic"                               , 0x10C00 , 0x10C4F),
            new UnicodeSubset("(0x10C50 - 0x10E5F)"                      , 0x10C50 , 0x10E5F),
            new UnicodeSubset("Rumi Numeral Symbols"                     , 0x10E60 , 0x10E7F),
            new UnicodeSubset("(0x10E60 - 0x110CF)"                      , 0x10E80 , 0x110CF),
            new UnicodeSubset("Sora Sompeng"                             , 0x110D0 , 0x110FF),
            new UnicodeSubset("Chakma"                                   , 0x11100 , 0x1114F),
            new UnicodeSubset("(0x11150 - 0x1117F)"                      , 0x11150 , 0x1117F),
            new UnicodeSubset("Sharada"                                  , 0x11180 , 0x111DF),
            new UnicodeSubset("(0x111E0 - 0x1167F)"                      , 0x111E0 , 0x1167F),
            new UnicodeSubset("Takri"                                    , 0x11680 , 0x116CF),
            new UnicodeSubset("(0x116D0 - 0x11FFF)"                      , 0x116D0 , 0x11FFF),
            new UnicodeSubset("Cuneiform"                                , 0x12000 , 0x123FF),
            new UnicodeSubset("Cuneiform Numbers and Punctuation"        , 0x12400 , 0x1247F),
            new UnicodeSubset("(0x12480 - 0x1CFFF)"                      , 0x12480 , 0x1CFFF),
            new UnicodeSubset("Byzantine Musical Symbols"                , 0x1D000 , 0x1D0FF),
            new UnicodeSubset("Musical Symbols"                          , 0x1D100 , 0x1D1FF),
            new UnicodeSubset("Ancient Greek Musical Notation"           , 0x1D200 , 0x1D24F),
            new UnicodeSubset("(0x1D250 - 0x1D3FF)"                      , 0x1D250 , 0x1D2FF),
            new UnicodeSubset("Tai Xuan Jing Symbols"                    , 0x1D300 , 0x1D35F),
            new UnicodeSubset("Counting Rod Numerals"                    , 0x1D360 , 0x1D37F),
            new UnicodeSubset("(0x1D380 - 0x1D3FF)"                      , 0x1D380 , 0x1D3FF),
            new UnicodeSubset("Mathematical Alphanumeric Symbols"        , 0x1D400 , 0x1D7FF),
            new UnicodeSubset("(0x1D800 - 0x1EFFF)"                      , 0x1D800 , 0x1EFFF),
            new UnicodeSubset("Mahjong Tiles"                            , 0x1F000 , 0x1F02F),
            new UnicodeSubset("Domino Tiles"                             , 0x1F030 , 0x1F09F),
            new UnicodeSubset("Playing Cards"                            , 0x1F0A0 , 0x1F0FF),
            new UnicodeSubset("Enclosed Alphanumeric Supplement"         , 0x1F100 , 0x1F1FF),
            new UnicodeSubset("Enclosed Ideographic Supplement"          , 0x1F200 , 0x1F2FF),
            new UnicodeSubset("Miscellaneous Symbols and Pictographs"    , 0x1F300 , 0x1F5FF),
            new UnicodeSubset("Emoticons"                                , 0x1F600 , 0x1F64F),
            new UnicodeSubset("(0x1F650 - 0x1F67F)"                      , 0x1F650 , 0x1F67F),
            new UnicodeSubset("Transport and Map Symbols"                , 0x1F680 , 0x1F6FF),
            new UnicodeSubset("Alchemical Symbols"                       , 0x1F700 , 0x1F77F),
            new UnicodeSubset("(0x1F780 - 0x1FF7F)"                      , 0x1F780 , 0x1FF7F),
            new UnicodeSubset("Unassigned"                               , 0x1FF80 , 0x1FFFD),
            new UnicodeSubset("(Reserved)"                               , 0x1FFFE , 0x1FFFF),

            // Plane 2
            new UnicodeSubset("CJK Unified Ideographs Extension B"       , 0x20000 , 0x2A6DF),
            new UnicodeSubset("(0x2A6E0 - 0x2A6FF)"                      , 0x2A6E0 , 0x2A6FF),
            new UnicodeSubset("CJK Unified Ideographs Extension C"       , 0x2A700 , 0x2B73F),
            new UnicodeSubset("CJK Unified Ideographs Extension D"       , 0x2B740 , 0x2B81F),
            new UnicodeSubset("(0x2B820 - 0x2F7FF)"                      , 0x2B820 , 0x2F7FF),
            new UnicodeSubset("CJK Compatibility Ideographs Supplement"  , 0x2F800 , 0x2FA1F),
            new UnicodeSubset("(0x2FA20 - 0x2FF7F)"                      , 0x2FA20 , 0x2FF7F),
            new UnicodeSubset("Unassigned"                               , 0x2FF80 , 0x2FFFD),
            new UnicodeSubset("(Reserved)"                               , 0x2FFFE , 0x2FFFF),

            // Plane 3
            new UnicodeSubset("(0x30000 - 0x3FFFD)"                      , 0x30000 , 0x3FFFD),
            new UnicodeSubset("(Reserved)"                               , 0x3FFFE , 0x3FFFF),

            // Plane 4
            new UnicodeSubset("(0x40000 - 0x4FFFD)"                      , 0x40000 , 0x4FFFD),
            new UnicodeSubset("(Reserved)"                               , 0x4FFFE , 0x4FFFF),

            // Plane 5
            new UnicodeSubset("(0x50000 - 0x5FFFD)"                      , 0x50000 , 0x5FFFD),
            new UnicodeSubset("(Reserved)"                               , 0x5FFFE , 0x5FFFF),

            // Plane 6
            new UnicodeSubset("(0x60000 - 0x6FFFD)"                      , 0x60000 , 0x6FFFD),
            new UnicodeSubset("(Reserved)"                               , 0x6FFFE , 0x6FFFF),

            // Plane 7
            new UnicodeSubset("(0x70000 - 0x7FFFD)"                      , 0x70000 , 0x7FFFD),
            new UnicodeSubset("(Reserved)"                               , 0x7FFFE , 0x7FFFF),

            // Plane 8
            new UnicodeSubset("(0x80000 - 0x8FFFD)"                      , 0x80000 , 0x8FFFD),
            new UnicodeSubset("(Reserved)"                               , 0x8FFFE , 0x8FFFF),

            // Plane 9
            new UnicodeSubset("(0x90000 - 0x9FFFD)"                      , 0x90000 , 0x9FFFD),
            new UnicodeSubset("(Reserved)"                               , 0x9FFFE , 0x9FFFF),

            // Plane 10
            new UnicodeSubset("(0xA0000 - 0xAFFFD)"                      , 0xA0000 , 0xAFFFD),
            new UnicodeSubset("(Reserved)"                               , 0xAFFFE , 0xAFFFF),

            // Plane 11
            new UnicodeSubset("(0xB0000 - 0xBFFFD)"                      , 0xB0000 , 0xBFFFD),
            new UnicodeSubset("(Reserved)"                               , 0xBFFFE , 0xBFFFF),

            // Plane 12
            new UnicodeSubset("(0xC0000 - 0xCFFFD)"                      , 0xC0000 , 0xCFFFD),
            new UnicodeSubset("(Reserved)"                               , 0xCFFFE , 0xCFFFF),

            // Plane 13
            new UnicodeSubset("(0xD0000 - 0xDFFFD)"                      , 0xD0000 , 0xDFFFD),
            new UnicodeSubset("(Reserved)"                               , 0xDFFFE , 0xDFFFF),

            // Plane 14
            new UnicodeSubset("Tags"                                     , 0xE0000 , 0xE007F),
            new UnicodeSubset("(0xE0080 - 0xE00FF)"                      , 0xE0080 , 0xE00FF),
            new UnicodeSubset("Variation Selectors Supplement"           , 0xE0100 , 0xE01EF),
            new UnicodeSubset("(0xE01F0 - 0xEFFFD)"                      , 0xE01F0 , 0xEFFFD),
            new UnicodeSubset("(Reserved)"                               , 0xEFFFE , 0xEFFFF),

            // Plane 15
            new UnicodeSubset("Supplementary Private Use Area A"         , 0xF0000 , 0xFFFFD),
            new UnicodeSubset("(Reserved)"                               , 0xFFFFE , 0xFFFFF),

            // Plane 16
            new UnicodeSubset("Supplementary Private Use Area B"         , 0x100000, 0x10FFFD),
            new UnicodeSubset("(Reserved)"                               , 0x10FFFE, 0x10FFFF),
};
    }
}
