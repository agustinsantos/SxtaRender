using System;

namespace SxtaRender.Fonts
{
    [Flags]
    public enum FontAlignment
    {
        // Horizontal align
        ALIGN_LEFT = 1 << 0,   // Default
        ALIGN_CENTER = 1 << 1,
        ALIGN_RIGHT = 1 << 2,
        // Vertical align
        ALIGN_TOP = 1 << 3,
        ALIGN_MIDDLE = 1 << 4,
        ALIGN_BOTTOM = 1 << 5,
        ALIGN_BASELINE = 1 << 6, // Default
    }
}
