using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sxta.Render;
using System.Xml;
using Sxta.Math;

namespace proland
{
    public static class Color

    {
        public static Vector3f rgb2hsl(Vector3f rgb)
        {
            float vmin = Math.Min(rgb.X, Math.Min(rgb.Y, rgb.Z));
            float vmax = Math.Max(rgb.X, Math.Max(rgb.Y, rgb.Z));
            float dmax = vmax - vmin;
            float h, s, l;
            l = (vmax + vmin) / 2;
            if (dmax == 0)
            {
                h = 0;
                s = 0;
            }
            else
            {
                if (l < 0.5)
                {
                    s = dmax / (vmax + vmin);
                }
                else
                {
                    s = dmax / (2 - vmax - vmin);
                }
                float[] d = new float[3];
                d[0] = (((vmax - rgb.X) / 6) + (dmax / 2)) / dmax;
                d[1] = (((vmax - rgb.Y) / 6) + (dmax / 2)) / dmax;
                d[2] = (((vmax - rgb.Z) / 6) + (dmax / 2)) / dmax;
                if (rgb.X == vmax)
                {
                    h = d[2] - d[1];
                }
                else if (rgb.Y == vmax)
                {
                    h = (1.0f / 3) + d[0] - d[2];
                }
                else /*if (rgb.z == vmax)*/
                {
                    h = (2.0f / 3) + d[1] - d[0];
                }
                if (h < 0)
                {
                    h += 1;
                }
                if (h > 1)
                {
                    h -= 1;
                }
            }
            Vector3f v = new Vector3f(h, s, 1);
            return v;
        }
        public static float h2rgb(float v1, float v2, float vH)
        {
            float r;
            if (vH < 0) vH += 1;
            if (vH > 1) vH -= 1;
            if (vH < 1.0 / 6)
            {
                r = v1 + (v2 - v1) * 6 * vH;
            }
            else if (vH < 1.0 / 2)
            {
                r = v2;
            }
            else if (vH < 2.0 / 3)
            {
                r = v1 + (v2 - v1) * ((2.0f / 3) - vH) * 6;
            }
            else
            {
                r = v1;
            }
            return r;
        }

        public static Vector3f hsl2rgb(Vector3f hsl)
        {
            float s = hsl.Y;
            float l = hsl.Z;
            if (s == 0)
            {
                Vector3f v = new Vector3f(l, l, l);
                return v;
            }
            else
            {
                float v1, v2;
                if (l < 0.5)
                {
                    v2 = l * (1 + s);
                }
                else
                {
                    v2 = l + s - s * l;
                }
                v1 = 2 * l - v2;
                float r = h2rgb(v1, v2, hsl.X + 1.0f / 3);
                float g = h2rgb(v1, v2, hsl.X);
                float b = h2rgb(v1, v2, hsl.X - 1.0f / 3);
                Vector3f v = new Vector3f(r, g, b);
                return v;
            }
        }

        public static Matrix3f dcolor(Vector3f rgb, Vector3f amp)
        {
            Vector3f hsl = rgb2hsl(rgb);
            Vector3f RGB;
            Matrix3f m;
            RGB = hsl2rgb(hsl + new Vector3f(0.01f, 0, 0));
            //m.setColumn(0, (RGB - rgb) / 0.01f * amp.X);
            Vector3f rst = (RGB - rgb) / 0.01f * amp.X;
            m.R0C0 = rst.X;
            m.R1C0 = rst.Y;
            m.R2C0 = rst.Z;

            RGB = hsl2rgb(hsl + new Vector3f(0, 0.01f, 0));
            //m.setColumn(1, (RGB - rgb) / 0.01f * amp.Y);
            rst = (RGB - rgb) / 0.01f * amp.X;
            m.R0C1 = rst.X;
            m.R1C1 = rst.Y;
            m.R2C1 = rst.Z;
            RGB = hsl2rgb(hsl + new Vector3f(0, 0, 0.01f));
            //m.setColumn(2, (RGB - rgb) / 0.01f * amp.Z);
            rst = (RGB - rgb) / 0.01f * amp.X;
            m.R0C2 = rst.X;
            m.R1C2 = rst.Y;
            m.R2C2 = rst.Z;
            return m;
        }

    }
}
