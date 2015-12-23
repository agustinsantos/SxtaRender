using Sxta.Math;
using System.Runtime.InteropServices;

namespace NanoVG
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NVGvertex
    {
        public Vector2f Position;
        public Vector2f TexCoord;
        public static int SizeInBytes
        {
            get { return Vector2f.SizeInBytes + Vector2f.SizeInBytes; }
        }

        public void Set(float x, float y, float u, float v)
        {
            this.Position.X = x;
            this.Position.Y = y;
            this.TexCoord.X = u;
            this.TexCoord.Y = v;
        }

        public NVGvertex(float x, float y, float u, float v)
        {
            this.Position.X = x;
            this.Position.Y = y;
            this.TexCoord.X = u;
            this.TexCoord.Y = v;
        }
    }
}
