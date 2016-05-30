/*
 * Proland: a procedural landscape rendering library.
 * Website : http://proland.inrialpes.fr/
 * Copyright (c) 2008-2015 INRIA - LJK (CNRS - Grenoble University)
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright notice, 
 * this list of conditions and the following disclaimer.
 * 
 * 2. Redistributions in binary form must reproduce the above copyright notice, 
 * this list of conditions and the following disclaimer in the documentation 
 * and/or other materials provided with the distribution.
 * 
 * 3. Neither the name of the copyright holder nor the names of its contributors 
 * may be used to endorse or promote products derived from this software without 
 * specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
 * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF 
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE 
 * OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 */
/*
 * Proland is distributed under the Berkeley Software Distribution 3 Licence. 
 * For any assistance, feedback and enquiries about training programs, you can check out the 
 * contact page on our website : 
 * http://proland.inrialpes.fr/
 */
/*
 * Main authors: Eric Bruneton, Antoine Begault, Guillaume Piolat.
* Modified and ported to C# and Sxta Engine by Agustin Santos and Daniel Olmedo 2015-2016
*/

using Sxta.Math;

namespace Sxta.Proland.Terrain
{
    public abstract class ColorFunction
    {
         public abstract Vector4f GetColor(int x, int y);
    }

    public class PlaneColorFunction : ColorFunction
    {
        InputMap src;
        int dstSize;

        public PlaneColorFunction(InputMap src, int dstSize)
        {
            this.src = src;
            this.dstSize = dstSize;
        }

        public override Vector4f GetColor(int x, int y)
        {
            return GetColor((double)x / (double)(dstSize * src.Width), (double)y / (double)(dstSize * src.Height));
        }

        public Vector4f GetColor(double x, double y)
        {
            int ix = (int)System.Math.Floor(x);
            int iy = (int)System.Math.Floor(y);
            x -= ix;
            y -= iy;
            double cx = 1.0f - x;
            double cy = 1.0f - y;
            Vector4f c1 = src.Get(ix, iy);
            Vector4f c2 = src.Get(ix + 1, iy);
            Vector4f c3 = src.Get(ix, iy + 1);
            Vector4f c4 = src.Get(ix + 1, iy + 1);
            Vector4f c = new Vector4f();
            c.X = (float)((c1.X * cx + c2.X * x) * cy + (c3.X * cx + c4.X * x) * y);
            c.Y = (float)((c1.Y * cx + c2.Y * x) * cy + (c3.Y * cx + c4.Y * x) * y);
            c.Z = (float)((c1.Z * cx + c2.Z * x) * cy + (c3.Z * cx + c4.Z * x) * y);
            c.W = (float)((c1.W * cx + c2.W * x) * cy + (c3.W * cx + c4.W * x) * y);
            return c;
        }
    }

    public class SphericalColorFunction : ColorFunction
    {
        InputMap src;

        ProjectionFunction projection;

        int dstSize;

        public SphericalColorFunction(InputMap src, ProjectionFunction projection, int dstSize)
        {
            this.src = src;
            this.projection = projection;
            this.dstSize = dstSize;
        }

        public override Vector4f GetColor(int x, int y)
        {
            double sx, sy, sz;
            projection(x, y, dstSize, out sx, out sy, out sz);
            double lon = System.Math.Atan2(sy, sx) + System.Math.PI;
            double lat = System.Math.Acos(sz);
            return GetColor(lon, lat);
        }

        public Vector4f GetColor(double lon, double lat)
        {
            lon = lon / System.Math.PI * (src.Width / 2);
            lat = lat / System.Math.PI * src.Height;
            int ilon = (int)System.Math.Floor(lon);
            int ilat = (int)System.Math.Floor(lat);
            lon -= ilon;
            lat -= ilat;
            double clon = 1.0 - lon;
            double clat = 1.0 - lat;
            Vector4f c1 = src.Get((ilon + src.Width) % src.Width, ilat);
            Vector4f c2 = src.Get((ilon + src.Width + 1) % src.Width, ilat);
            Vector4f c3 = src.Get((ilon + src.Width) % src.Width, ilat + 1);
            Vector4f c4 = src.Get((ilon + src.Width + 1) % src.Width, ilat + 1);
            Vector4f c;
            c.X = (float)((c1.X * clon + c2.X * lon) * clat + (c3.X * clon + c4.X * lon) * lat);
            c.Y = (float)((c1.Y * clon + c2.Y * lon) * clat + (c3.Y * clon + c4.Y * lon) * lat);
            c.Z = (float)((c1.Z * clon + c2.Z * lon) * clat + (c3.Z * clon + c4.Z * lon) * lat);
            c.W = (float)((c1.W * clon + c2.W * lon) * clat + (c3.W * clon + c4.W * lon) * lat);
            return c;
        }
    }
}














