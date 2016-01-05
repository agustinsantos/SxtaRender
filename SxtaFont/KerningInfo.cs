using System;

namespace SxtaRender.Fonts
{
    /// <summary>
    /// A structure that hold a kerning value relatively to a charcode. 
    /// 
    /// This structure cannot be used alone since the(necessary) right charcode is 
    /// implicitely held by the owner of this structure.
    /// </summary>
    public struct  KerningInfo
    {
        /// <summary>
        /// Left character code in the kern pair.
        /// </summary>
        public char Charcode;

        /// <summary>
        /// Kerning value (in fractional pixels).
        /// </summary>
        public float Kerning;

    }
}
