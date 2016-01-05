using System;
using System.Collections.Generic;
using System.Drawing.Text;

namespace SxtaRender.Fonts
{
    public class FontGenerationConfig
    {
        /// <summary>
        /// Scale in relation to the actual font glyphs
        /// </summary>
        public float Scale = 1.0f;

        /// <summary>
        /// The blur radius. Caution: high values will greatly impact the 
        /// time it takes to build a font shadow
        /// </summary>
        public int BlurRadius = 3;

        /// <summary>
        /// Number of blur passes. Caution: high values will greatly impact the 
        /// time it takes to build a font shadow
        /// </summary>
        public int BlurPasses = 2;

        /// <summary>
        /// The standard width of texture pages (the page will
        /// automatically be cropped if there is extra space)
        /// </summary>
        public int PageWidth = 512;

        /// <summary>
        /// The standard height of texture pages (the page will
        /// automatically be cropped if there is extra space)
        /// </summary>
        public int PageHeight = 512;

        /// <summary>
        /// Whether to force texture pages to use a power of two.
        /// </summary>
        public bool ForcePowerOfTwo = true;

        /// <summary>
        /// The margin (on all sides) around glyphs when rendered to
        /// their texture page. Note this is in addition to 3xblurRadius margin
        /// which is automatically added.
        /// </summary>
        public int GlyphMargin = 2;

        /// <summary>
        ///  Which render hint to use when rendering the ttf character set to create the texture
        /// </summary>
        public TextRenderingHint RenderHint = TextRenderingHint.SingleBitPerPixel;

        /// <summary>
        /// Set of characters to support
        /// for instance: 
        ///   CharSet = UnicodeSubset.GetCharset("Latin Extended B");
        /// or
        ///   CharSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890.:,;'\"(!?)+-*/=_{}[]@~#\\<>|^%$£&";
        /// </summary>
        public string CharSet = UnicodeSubset.GetCharset("ASCII");

        /// <summary>
        /// When measuring the bounds of glyphs, and performing kerning calculations, 
        /// this is the minimum alpha level that is necessray for a pixel to be considered
        /// non-empty. This should be set to a value on the range [0,255]
        /// </summary>
        public byte AlphaEmptyPixelTolerance = 0;


        /// <summary>
        /// Kerning rules for particular characters
        /// </summary>
        private Dictionary<char, CharacterKerningRule> CharacterKerningRules = new Dictionary<char, CharacterKerningRule>()
        {
            { '_', CharacterKerningRule.Zero },
            { '^', CharacterKerningRule.Zero },
            { '\"', CharacterKerningRule.NotMoreThanHalf },
            { '\'', CharacterKerningRule.NotMoreThanHalf }
        };

        /// <summary>
        /// Sets all characters in the given string to the specified kerning rule.
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="rule"></param>
        public void BatchSetCharacterKerningRule(String chars, CharacterKerningRule rule)
        {
            foreach (var c in chars)
            {
                CharacterKerningRules[c] = rule;
            }
        }

        /// <summary>
        /// Sets the specified character kerning rule.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="rule"></param>
        public void SetCharacterKerningRule(char c, CharacterKerningRule rule)
        {
            CharacterKerningRules[c] = rule;
        }

        /// <summary>
        /// Given a pair of characters, this will return the overriding 
        /// CharacterKerningRule.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public CharacterKerningRule GetOverridingCharacterKerningRuleForPair(char c1, char c2)
        {
            if (GetCharacterKerningRule(c1) == CharacterKerningRule.Zero || GetCharacterKerningRule(c2) == CharacterKerningRule.Zero)
            {
                return CharacterKerningRule.Zero;
            }
            else if (GetCharacterKerningRule(c1) == CharacterKerningRule.NotMoreThanHalf || GetCharacterKerningRule(c2) == CharacterKerningRule.NotMoreThanHalf)
            {
                return CharacterKerningRule.NotMoreThanHalf;
            }

            return CharacterKerningRule.Normal;
        }

        public CharacterKerningRule GetCharacterKerningRule(char c)
        {
            if (CharacterKerningRules.ContainsKey(c))
            {
                return CharacterKerningRules[c];
            }

            return CharacterKerningRule.Normal;
        }
    }

    public enum CharacterKerningRule
    {
        /// <summary>
        /// Ordinary kerning
        /// </summary>
        Normal,

        /// <summary>
        /// All kerning pairs involving this character will kern by 0. This will
        /// override both Normal and NotMoreThanHalf for any pair.
        /// </summary>
        Zero,

        /// <summary>
        /// Any kerning pairs involving this character will not kern
        /// by more than half the minimum width of the two characters 
        /// involved. This will override Normal for any pair.
        /// </summary>
        NotMoreThanHalf
    }
}
