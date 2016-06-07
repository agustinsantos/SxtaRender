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

using System;

namespace proland
{
    public class Noise
    {

        // ----------------------------------------------------------------------------
        // CLASSIC PERLIN NOISE
        // ----------------------------------------------------------------------------
        const int FLOAT_SIZE = 4;

        static bool initialized = false;

        static int[] p = new int[2 * 256 + 2];
        static float[] g1 = new float[2 * 256 + 2];
        static float[,] g2 = new float[2 * 256 + 2,2];
        static float[,] g3 = new float[2 * 256 + 2,3];

        /**
         * @defgroup proland_math math
         * Provides utility math functions.
         * @ingroup proland
         */

        /**
         * Returns a pseudo random integer in the range 0-2147483647.
         * @ingroup proland_math
         *
         * @param the seed used by this pseudo random generator. It is modified each
         *      time this function is called.
         * @return a peudo random integer in the range 0-2147483647.
         */
        public static long lrandom(ref long seed)
        {
            seed = (seed * 1103515245 + 12345) & 0x7FFFFFFF;
            return seed;
        }

        /**
         * Returns a pseudo random float number in the range 0-1.
         * @ingroup proland_math
         *
         * @param the seed used by this pseudo random generator. It is modified each
         *      time this function is called.
         * @return a pseudo random float number in the range 0-1.
         */
        public static float frandom(ref long seed)
        {
            long r = lrandom(ref seed) >> (31 - 24);
            return r / (float)(1 << 24);
        }

        /**
         * Returns a pseudo random float number with a Gaussian distribution.
         * @ingroup proland_math
         *
         * @param mean the mean of the Gaussian distribution.
         * @param stdDeviation the standard deviation of the Gaussian distribution
         *      (square root of its variance).
         * @param the seed used by this pseudo random generator. It is modified each
         *      time this function is called.
         * @return a pseudo random float number with the given Gaussian distribution.
         */
        static float y2;
        public static float grandom(float mean, float stdDeviation, ref long seed)
        {
            float x1, x2, w, y1;

            int use_last = 0;

            if (use_last != 0)
            {
                y1 = y2;
                use_last = 0;
            }
            else
            {
                do
                {
                    x1 = 2.0f * frandom(ref seed) - 1.0f;
                    x2 = 2.0f * frandom(ref seed) - 1.0f;
                    w = x1 * x1 + x2 * x2;
                } while (w >= 1.0f);
                w = (float)Math.Sqrt((-2.0f * Math.Log10(w)) / w);
                y1 = x1 * w;
                y2 = x2 * w;
                use_last = 1;
            }
            return mean + Convert.ToSingle(y1) * stdDeviation;
        }


        static void init()
        {
            int i, j, k;
            long seed = 12345;
            for (i = 0; i < 256; ++i)
            {
                p[i] = i;
                g1[i] = (float)((lrandom(ref seed) % (2 * 256)) - 256) / 256;
                for (j = 0; j < 2; ++j)
                {
                    g2[i,j] = (float)((lrandom(ref seed) % (2 * 256)) - 256) / 256;
                }
                float l = Convert.ToSingle(Math.Sqrt(g2[i,0] * g2[i,0] + g2[i,1] * g2[i,1]));
                g2[i,0] /= l;
                g2[i,1] /= l;
                for (j = 0; j < 3; ++j)
                {
                    g3[i,j] = (float)((lrandom(ref seed) % (2 * 256)) - 256) / 256;
                }
                l = Convert.ToSingle(Math.Sqrt(g3[i,0] * g3[i,0] + g3[i,1] * g3[i,1] + g3[i,2] * g3[i,2]));
                g3[i,0] /= l;
                g3[i,1] /= l;
                g3[i,2] /= l;
            }
            while (i >= 1)
            {
                k = p[i];
                p[i] = p[j = Convert.ToInt16(lrandom(ref seed) % 256)];
                p[j] = k;
            }
            for (i = 0; i < 256 + 2; ++i)
            {
                p[256 + i] = p[i];
                g1[256 + i] = g1[i];
                for (j = 0; j < 2; ++j)
                {
                    g2[256 + i,j] = g2[i,j];
                }
                for (j = 0; j < 3; ++j)
                {
                    g3[256 + i,j] = g3[i,j];
                }
            }
        }

        private static float S_CURVE(float t)
        {
            return (t * t * (3.0f - 2.0f * t));
        }

        private static float LERP(float t, float a, float b)
        {
            return (a + t * (b - a));
        }
        private static void SETUP(float x, out int b0, out int b1, out float r0, out float r1)
        {
            int t = (int)x + 0x1000;
            b0 = (t) & 0xFF;
            b1 = (b0 + 1) & 0xFF;
            r0 = t - (int)Math.Floor((double)t);
            r1 = r0 - 1.0f;
        }
        private static float AT2(float rx, float ry, float[,] q, int index)
        {
            return rx * q[index,0] + ry * q[index,1];
        }

        private static float AT3(float rx, float ry, float rz, float[,] q, int index)
        {
            return rx * q[index,0] + ry * q[index,1] + rz * q[index,2];
        }
        /**
         * Computes the classic 2D Perlin noise function.
         * @ingroup proland_math
         *
         * @param x the x coordinate of the point where the function must be evaluated.
         * @param y the y coordinate of the point where the function must be evaluated.
         * @param P an optional period to get a periodic noise function. The default
         *      value 0 means a non periodic function.
         * @return the classic 2D Perlin noise function evaluated at (x,y). This
         *      function has a main frequency of 1, and its value are between -1 and 1.
         */
        public static float cnoise(float x, float y, int period = 0)
        {
            int bx0, bx1, by0, by1, b00, b10, b01, b11;
            float rx0, rx1, ry0, ry1, sx, sy, a, b, t, u, v;
            int i, j;

            if (!initialized)
            {
                initialized = true;
                init();
            }
            SETUP(x, out bx0, out bx1, out rx0, out rx1);
            SETUP(y, out by0, out by1, out ry0, out ry1);

            if (period != 0)
            {
                i = p[bx0 % period];
                j = p[bx1 % period];
                b00 = p[i + by0 % period];
                b10 = p[j + by0 % period];
                b01 = p[i + by1 % period];
                b11 = p[j + by1 % period];
            }
            else
            {
                i = p[bx0];
                j = p[bx1];
                b00 = p[i + by0];
                b10 = p[j + by0];
                b01 = p[i + by1];
                b11 = p[j + by1];
            }

            sx = S_CURVE(rx0);
            sy = S_CURVE(ry0);
            
            //q = g2[b00];
            u = AT2(rx0, ry0, g2, b00);
            //q = g2[b10];
            v = AT2(rx1, ry0, g2, b10);
            a = LERP(sx, u, v);
            
            //q = g2[b01];
            u = AT2(rx0, ry1, g2, b01);

            //q = g2[b11];
            v = AT2(rx1, ry1, g2, b11);
            b = LERP(sx, u, v);

            return LERP(sy, a, b);
        }

        /**
         * Computes the classic 3D Perlin noise function.
         * @ingroup proland_math
         *
         * @param x the x coordinate of the point where the function must be evaluated.
         * @param y the y coordinate of the point where the function must be evaluated.
         * @param z the z coordinate of the point where the function must be evaluated.
         * @param P an optional period to get a periodic noise function. The default
         *      value 0 means a non periodic function.
         * @return the classic 3D Perlin noise function evaluated at (x,y,z). This
         *      function has a main frequency of 1, and its value are between -1 and 1.
         */
        public static float cnoise(float x, float y, float z, int period = 0)
        {
            int bx0, bx1, by0, by1, bz0, bz1, b00, b10, b01, b11;
            float rx0, rx1, ry0, ry1, rz0, rz1, sy, sz, a, b, c, d, t, u, v;
            int i, j;

            if (!initialized)
            {
                initialized = true;
                init();
            }

            SETUP(x, out bx0, out bx1, out rx0, out rx1);
            SETUP(y, out by0, out by1, out ry0, out ry1);
            SETUP(z, out bz0, out bz1, out rz0, out rz1);

            if (period != 0)
            {
                i = p[bx0 % period];
                j = p[bx1 % period];
                b00 = p[i + by0 % period];
                b10 = p[j + by0 % period];
                b01 = p[i + by1 % period];
                b11 = p[j + by1 % period];
                bz0 = bz0 % period;
                bz1 = bz1 % period;
            }
            else
            {
                i = p[bx0];
                j = p[bx1];
                b00 = p[i + by0];
                b10 = p[j + by0];
                b01 = p[i + by1];
                b11 = p[j + by1];
            }

            t = S_CURVE(rx0);
            sy = S_CURVE(ry0);
            sz = S_CURVE(rz0);

            //q = g3[b00 + bz0];
            u = AT3(rx0, ry0, rz0, g3, b00 + bz0);
            //q = g3[b10 + bz0];
            v = AT3(rx1, ry0, rz0, g3, b10 + bz0);
            a = LERP(t, u, v);
            
            //q = g3[b01 + bz0];
            u = AT3(rx0, ry1, rz0, g3, b01 + bz0);
            //q = g3[b11 + bz0];
            v = AT3(rx1, ry1, rz0, g3, b11 + bz0);
            b = LERP(t, u, v);

            c = LERP(sy, a, b);
            
            //q = g3[b00 + bz1];
            u = AT3(rx0, ry0, rz1, g3, b00 + bz1);
            //q = g3[b10 + bz1];
            v = AT3(rx1, ry0, rz1, g3, b10 + bz1);
            a = LERP(t, u, v);
            
            //q = g3[b01 + bz1];
            u = AT3(rx0, ry1, rz1, g3, b01 + bz1);
            //q = g3[b11 + bz1];
            v = AT3(rx1, ry1, rz1, g3, b11 + bz1);
            b = LERP(t, u, v);

            d = LERP(sy, a, b);

            return LERP(sz, c, d);
        }

        // ----------------------------------------------------------------------------
        // SIMPLEX NOISE
        // ----------------------------------------------------------------------------

        static int[] perm = {151,160,137,91,90,15,
    131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
    190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
    88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
    77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
    102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
    135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
    5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
    223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
    129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
    251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
    49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
    138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,151,
    160,137,91,90,15,131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
    190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
    88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
    77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
    102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
    135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
    5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
    223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
    129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
    251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
    49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
    138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180};

        static int[][] grad3 = new int[][]{new int[]{1,1,0},new int[]{-1,1,0},new int[]{1,-1,0},new int[]{-1,-1,0},
    new int[]{1,0,1},new int[]{-1,0,1},new int[]{1,0,-1},new int[]{-1,0,-1},
    new int[]{0,1,1},new int[]{0,-1,1},new int[]{0,1,-1},new int[]{0,-1,-1}};

        static int[][] grad4 = new int[][]{new int[]{0,1,1,1},new int[]{0,1,1,-1}, new int[]{0,1,-1,1}, new int[]{0,1,-1,-1},
    new int[]{0,-1,1,1},new int[] {0,-1,1,-1},new int[] {0,-1,-1,1},new int[] {0,-1,-1,-1},
    new int[]{1,0,1,1}, new int[]{1,0,1,-1}, new int[]{1,0,-1,1}, new int[]{1,0,-1,-1},
    new int[]{-1,0,1,1}, new int[]{-1,0,1,-1}, new int[]{-1,0,-1,1}, new int[]{-1,0,-1,-1},
    new int[]{1,1,0,1}, new int[]{1,1,0,-1}, new int[]{1,-1,0,1}, new int[]{1,-1,0,-1},
    new int[]{-1,1,0,1}, new int[]{-1,1,0,-1}, new int[]{-1,-1,0,1},new int[] {-1,-1,0,-1},
    new int[]{1,1,1,0}, new int[]{1,1,-1,0}, new int[]{1,-1,1,0}, new int[]{1,-1,-1,0},
    new int[]{-1,1,1,0}, new int[]{-1,1,-1,0}, new int[]{-1,-1,1,0}, new int[]{-1,-1,-1,0}};

        static int[][] simplex = new int[][]{
    new int[]{0,1,2,3},new int[]{0,1,3,2},new int[]{0,0,0,0},new int[]{0,2,3,1},new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{1,2,3,0},
    new int[]{0,2,1,3},new int[]{0,0,0,0},new int[]{0,3,1,2},new int[]{0,3,2,1},new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{1,3,2,0},
    new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{0,0,0,0},
    new int[]{1,2,0,3},new int[]{0,0,0,0},new int[]{1,3,0,2},new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{2,3,0,1},new int[]{2,3,1,0},
    new int[]{1,0,2,3},new int[]{1,0,3,2},new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{2,0,3,1},new int[]{0,0,0,0},new int[]{2,1,3,0},
    new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{0,0,0,0},
    new int[]{2,0,1,3},new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{3,0,1,2},new int[]{3,0,2,1},new int[]{0,0,0,0},new int[]{3,1,2,0},
    new int[]{2,1,0,3},new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{0,0,0,0},new int[]{3,1,0,2},new int[]{0,0,0,0},new int[]{3,2,0,1},new int[]{3,2,1,0}};

        static int fastfloor(float x)
        {
            return x > 0 ? (int)x : (int)x - 1;
        }

        static float dot(int[] g, float x, float y)
        {
            return g[0] * x + g[1] * y;
        }

        static float dot(int[] g, float x, float y, float z)
        {
            return g[0] * x + g[1] * y + g[2] * z;
        }

        static float dot(int[] g, float x, float y, float z, float w)
        {
            return g[0] * x + g[1] * y + g[2] * z + g[3] * w;
        }



        /**
         * Computes the 2D Perlin simplex noise function.
         * @ingroup proland_math
         *
         * @param x the x coordinate of the point where the function must be evaluated.
         * @param y the y coordinate of the point where the function must be evaluated.
         * @param P an optional period to get a periodic noise function. The default
         *      value 0 means a non periodic function.
         * @return the 2D Perlin simplex noise function evaluated at (x,y). This
         *      function has a main frequency of 1, and its value are between -1 and 1.
         */
        public static float snoise(float xin, float yin, int period = 0)
        {
            float n0, n1, n2; // Noise contributions from the three corners
                              // Skew the input space to determine which simplex cell we're in
            float F2 = 0.5f * ((float)Math.Sqrt(3.0f) - 1.0f);
            float s = (xin + yin) * F2; // Hairy factor for 2D
            int i = fastfloor(xin + s);
            int j = fastfloor(yin + s);
            float G2 = (3.0f - (float)Math.Sqrt(3.0f)) / 6.0f;
            float t = (i + j) * G2;
            float X0 = i - t; // Unskew the cell origin back to (x,y) space
            float Y0 = j - t;
            float x0 = xin - X0; // The x,y distances from the cell origin
            float y0 = yin - Y0;
            // For the 2D case, the simplex shape is an equilateral triangle.
            // Determine which simplex we are in.
            int i1, j1; // Offsets for second (middle) corner of simplex in (i,j) coords
            if (x0 > y0) { i1 = 1; j1 = 0; } // lower triangle, XY order: (0,0)->(1,0)->(1,1)
            else { i1 = 0; j1 = 1; } // upper triangle, YX order: (0,0)->(0,1)->(1,1)
                                     // A step of (1,0) in (i,j) means a step of (1-c,-c) in (x,y), and
                                     // a step of (0,1) in (i,j) means a step of (-c,1-c) in (x,y), where
                                     // c = (3-sqrt(3))/6
            float x1 = x0 - i1 + G2; // Offsets for middle corner in (x,y) unskewed coords
            float y1 = y0 - j1 + G2;
            float x2 = x0 - 1.0f + 2.0f * G2; // Offsets for last corner in (x,y) unskewed coords
            float y2 = y0 - 1.0f + 2.0f * G2;
            // Work out the hashed gradient indices of the three simplex corners
            int ii, jj, gi0, gi1, gi2;
            if (period == 0)
            {
                ii = i & 255;
                jj = j & 255;
                gi0 = perm[ii + perm[jj]] % 12;
                gi1 = perm[ii + i1 + perm[jj + j1]] % 12;
                gi2 = perm[ii + 1 + perm[jj + 1]] % 12;
            }
            else
            {
                ii = (i % period) & 255;
                jj = (j % period) & 255;
                gi0 = perm[ii + perm[jj]] % 12;
                gi1 = perm[(ii + i1) % period + perm[(jj + j1) % period]] % 12;
                gi2 = perm[(ii + 1) % period + perm[(jj + 1) % period]] % 12;
            }
            // Calculate the contribution from the three corners
            float t0 = 0.5f - x0 * x0 - y0 * y0;
            if (t0 < 0) n0 = 0.0f;
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * dot(grad3[gi0], x0, y0); // (x,y) of grad3 used for 2D gradient
            }
            float t1 = 0.5f - x1 * x1 - y1 * y1;
            if (t1 < 0) n1 = 0.0f;
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * dot(grad3[gi1], x1, y1);
            }
            float t2 = 0.5f - x2 * x2 - y2 * y2;
            if (t2 < 0) n2 = 0.0f;
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * dot(grad3[gi2], x2, y2);
            }
            // Add contributions from each corner to get the final noise value.
            // The result is scaled to return values in the interval [-1,1].
            return 70.0f * (n0 + n1 + n2);
        }

        /**
         * Computes the 3D Perlin simplex noise function.
         * @ingroup proland_math
         *
         * @param x the x coordinate of the point where the function must be evaluated.
         * @param y the y coordinate of the point where the function must be evaluated.
         * @param z the z coordinate of the point where the function must be evaluated.
         * @param P an optional period to get a periodic noise function. The default
         *      value 0 means a non periodic function.
         * @return the 3D Perlin simplex noise function evaluated at (x,y,z). This
         *      function has a main frequency of 1, and its value are between -1 and 1.
         */
        public static float snoise(float xin, float yin, float zin, int P = 0)
        {
            float n0, n1, n2, n3; // Noise contributions from the four corners
                                  // Skew the input space to determine which simplex cell we're in
            float F3 = 1.0f / 3.0f;
            float s = (xin + yin + zin) * F3; // Very nice and simple skew factor for 3D
            int i = fastfloor(xin + s);
            int j = fastfloor(yin + s);
            int k = fastfloor(zin + s);
            float G3 = 1.0f / 6.0f; // Very nice and simple unskew factor, too
            float t = (i + j + k) * G3;
            float X0 = i - t; // Unskew the cell origin back to (x,y,z) space
            float Y0 = j - t;
            float Z0 = k - t;
            float x0 = xin - X0; // The x,y,z distances from the cell origin
            float y0 = yin - Y0;
            float z0 = zin - Z0;
            // For the 3D case, the simplex shape is a slightly irregular tetrahedron.
            // Determine which simplex we are in.
            int i1, j1, k1; // Offsets for second corner of simplex in (i,j,k) coords
            int i2, j2, k2; // Offsets for third corner of simplex in (i,j,k) coords
            if (x0 >= y0)
            {
                if (y0 >= z0)
                { i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 1; k2 = 0; } // X Y Z order
                else if (x0 >= z0) { i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 0; k2 = 1; } // X Z Y order
                else { i1 = 0; j1 = 0; k1 = 1; i2 = 1; j2 = 0; k2 = 1; } // Z X Y order
            }
            else
            { // x0<y0
                if (y0 < z0) { i1 = 0; j1 = 0; k1 = 1; i2 = 0; j2 = 1; k2 = 1; } // Z Y X order
                else if (x0 < z0) { i1 = 0; j1 = 1; k1 = 0; i2 = 0; j2 = 1; k2 = 1; } // Y Z X order
                else { i1 = 0; j1 = 1; k1 = 0; i2 = 1; j2 = 1; k2 = 0; } // Y X Z order
            }
            // A step of (1,0,0) in (i,j,k) means a step of (1-c,-c,-c) in (x,y,z),
            // a step of (0,1,0) in (i,j,k) means a step of (-c,1-c,-c) in (x,y,z), and
            // a step of (0,0,1) in (i,j,k) means a step of (-c,-c,1-c) in (x,y,z), where
            // c = 1/6.
            float x1 = x0 - i1 + G3; // Offsets for second corner in (x,y,z) coords
            float y1 = y0 - j1 + G3;
            float z1 = z0 - k1 + G3;
            float x2 = x0 - i2 + 2.0f * G3; // Offsets for third corner in (x,y,z) coords
            float y2 = y0 - j2 + 2.0f * G3;
            float z2 = z0 - k2 + 2.0f * G3;
            float x3 = x0 - 1.0f + 3.0f * G3; // Offsets for last corner in (x,y,z) coords
            float y3 = y0 - 1.0f + 3.0f * G3;
            float z3 = z0 - 1.0f + 3.0f * G3;
            // Work out the hashed gradient indices of the four simplex corners
            int ii, jj, kk, gi0, gi1, gi2, gi3;
            if (P == 0)
            {
                ii = i & 255;
                jj = j & 255;
                kk = k & 255;
                gi0 = perm[ii + perm[jj + perm[kk]]] % 12;
                gi1 = perm[ii + i1 + perm[jj + j1 + perm[kk + k1]]] % 12;
                gi2 = perm[ii + i2 + perm[jj + j2 + perm[kk + k2]]] % 12;
                gi3 = perm[ii + 1 + perm[jj + 1 + perm[kk + 1]]] % 12;
            }
            else
            {
                ii = (i % P) & 255;
                jj = (j % P) & 255;
                kk = (k % P) & 255;
                gi0 = perm[ii + perm[jj + perm[kk]]] % 12;
                gi1 = perm[(ii + i1) % P + perm[(jj + j1) % P + perm[(kk + k1) % P]]] % 12;
                gi2 = perm[(ii + i2) % P + perm[(jj + j2) % P + perm[(kk + k2) % P]]] % 12;
                gi3 = perm[(ii + 1) % P + perm[(jj + 1) % P + perm[(kk + 1) % P]]] % 12;
            }
            // Calculate the contribution from the four corners
            float t0 = 0.6f - x0 * x0 - y0 * y0 - z0 * z0;
            if (t0 < 0) n0 = 0.0f;
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * dot(grad3[gi0], x0, y0, z0);
            }
            float t1 = 0.6f - x1 * x1 - y1 * y1 - z1 * z1;
            if (t1 < 0) n1 = 0.0f;
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * dot(grad3[gi1], x1, y1, z1);
            }
            float t2 = 0.6f - x2 * x2 - y2 * y2 - z2 * z2;
            if (t2 < 0) n2 = 0.0f;
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * dot(grad3[gi2], x2, y2, z2);
            }
            float t3 = 0.6f - x3 * x3 - y3 * y3 - z3 * z3;
            if (t3 < 0) n3 = 0.0f;
            else
            {
                t3 *= t3;
                n3 = t3 * t3 * dot(grad3[gi3], x3, y3, z3);
            }
            // Add contributions from each corner to get the final noise value.
            // The result is scaled to stay just inside [-1,1]
            return 32.0f * (n0 + n1 + n2 + n3);
        }

        /**
         * Computes the 4D Perlin simplex noise function.
         * @ingroup proland_math
         *
         * @param x the x coordinate of the point where the function must be evaluated.
         * @param y the y coordinate of the point where the function must be evaluated.
         * @param z the z coordinate of the point where the function must be evaluated.
         * @param w the w coordinate of the point where the function must be evaluated.
         * @param P an optional period to get a periodic noise function. The default
         *      value 0 means a non periodic function.
         * @return the 4D Perlin simplex noise function evaluated at (x,y,z,w). This
         *      function has a main frequency of 1, and its value are between -1 and 1.
         */
        public static float snoise(float x, float y, float z, float w, int P = 0)
        {
            // The skewing and unskewing factors are hairy again for the 4D case
            float F4 = ((float)Math.Sqrt(5.0f) - 1.0f) / 4.0f;
            float G4 = (5.0f - (float)Math.Sqrt(5.0f)) / 20.0f;
            float n0, n1, n2, n3, n4; // Noise contributions from the five corners
                                      // Skew the (x,y,z,w) space to determine which cell of 24 simplices we're in
            float s = (x + y + z + w) * F4; // Factor for 4D skewing
            int i = fastfloor(x + s);
            int j = fastfloor(y + s);
            int k = fastfloor(z + s);
            int l = fastfloor(w + s);
            float t = (i + j + k + l) * G4; // Factor for 4D unskewing
            float X0 = i - t; // Unskew the cell origin back to (x,y,z,w) space
            float Y0 = j - t;
            float Z0 = k - t;
            float W0 = l - t;
            float x0 = x - X0; // The x,y,z,w distances from the cell origin
            float y0 = y - Y0;
            float z0 = z - Z0;
            float w0 = w - W0;
            // For the 4D case, the simplex is a 4D shape I won't even try to describe.
            // To find out which of the 24 possible simplices we're in, we need to
            // determine the magnitude ordering of x0, y0, z0 and w0.
            // The method below is a good way of finding the ordering of x,y,z,w and
            // then find the correct traversal order for the simplex we’re in.
            // First, six pair-wise comparisons are performed between each possible pair
            // of the four coordinates, and the results are used to add up binary bits
            // for an integer index.
            int c1 = (x0 > y0) ? 32 : 0;
            int c2 = (x0 > z0) ? 16 : 0;
            int c3 = (y0 > z0) ? 8 : 0;
            int c4 = (x0 > w0) ? 4 : 0;
            int c5 = (y0 > w0) ? 2 : 0;
            int c6 = (z0 > w0) ? 1 : 0;
            int c = c1 + c2 + c3 + c4 + c5 + c6;
            int i1, j1, k1, l1; // The integer offsets for the second simplex corner
            int i2, j2, k2, l2; // The integer offsets for the third simplex corner
            int i3, j3, k3, l3; // The integer offsets for the fourth simplex corner
                                // simplex[c] is a 4-vector with the numbers 0, 1, 2 and 3 in some order.
                                // Many values of c will never occur, since e.g. x>y>z>w makes x<z, y<w and x<w
                                // impossible. Only the 24 indices which have non-zero entries make any sense.
                                // We use a thresholding to set the coordinates in turn from the largest magnitude.
                                // The number 3 in the "simplex" array is at the position of the largest coordinate.
            i1 = simplex[c][0] >= 3 ? 1 : 0;
            j1 = simplex[c][1] >= 3 ? 1 : 0;
            k1 = simplex[c][2] >= 3 ? 1 : 0;
            l1 = simplex[c][3] >= 3 ? 1 : 0;
            // The number 2 in the "simplex" array is at the second largest coordinate.
            i2 = simplex[c][0] >= 2 ? 1 : 0;
            j2 = simplex[c][1] >= 2 ? 1 : 0;
            k2 = simplex[c][2] >= 2 ? 1 : 0;
            l2 = simplex[c][3] >= 2 ? 1 : 0;
            // The number 1 in the "simplex" array is at the second smallest coordinate.
            i3 = simplex[c][0] >= 1 ? 1 : 0;
            j3 = simplex[c][1] >= 1 ? 1 : 0;
            k3 = simplex[c][2] >= 1 ? 1 : 0;
            l3 = simplex[c][3] >= 1 ? 1 : 0;
            // The fifth corner has all coordinate offsets = 1, so no need to look that up.
            float x1 = x0 - i1 + G4; // Offsets for second corner in (x,y,z,w) coords
            float y1 = y0 - j1 + G4;
            float z1 = z0 - k1 + G4;
            float w1 = w0 - l1 + G4;
            float x2 = x0 - i2 + 2.0f * G4; // Offsets for third corner in (x,y,z,w) coords
            float y2 = y0 - j2 + 2.0f * G4;
            float z2 = z0 - k2 + 2.0f * G4;
            float w2 = w0 - l2 + 2.0f * G4;
            float x3 = x0 - i3 + 3.0f * G4; // Offsets for fourth corner in (x,y,z,w) coords
            float y3 = y0 - j3 + 3.0f * G4;
            float z3 = z0 - k3 + 3.0f * G4;
            float w3 = w0 - l3 + 3.0f * G4;
            float x4 = x0 - 1.0f + 4.0f * G4; // Offsets for last corner in (x,y,z,w) coords
            float y4 = y0 - 1.0f + 4.0f * G4;
            float z4 = z0 - 1.0f + 4.0f * G4;
            float w4 = w0 - 1.0f + 4.0f * G4;
            // Work out the hashed gradient indices of the five simplex corners
            int ii, jj, kk, ll, gi0, gi1, gi2, gi3, gi4;
            if (P == 0)
            {
                ii = i & 255;
                jj = j & 255;
                kk = k & 255;
                ll = l & 255;
                gi0 = perm[ii + perm[jj + perm[kk + perm[ll]]]] % 32;
                gi1 = perm[ii + i1 + perm[jj + j1 + perm[kk + k1 + perm[ll + l1]]]] % 32;
                gi2 = perm[ii + i2 + perm[jj + j2 + perm[kk + k2 + perm[ll + l2]]]] % 32;
                gi3 = perm[ii + i3 + perm[jj + j3 + perm[kk + k3 + perm[ll + l3]]]] % 32;
                gi4 = perm[ii + 1 + perm[jj + 1 + perm[kk + 1 + perm[ll + 1]]]] % 32;
            }
            else
            {
                ii = (i % P) & 255;
                jj = (j % P) & 255;
                kk = (k % P) & 255;
                ll = (l % P) & 255;
                gi0 = perm[ii + perm[jj + perm[kk + perm[ll]]]] % 32;
                gi1 = perm[(ii + i1) % P + perm[(jj + j1) % P + perm[(kk + k1) % P + perm[(ll + l1) % P]]]] % 32;
                gi2 = perm[(ii + i2) % P + perm[(jj + j2) % P + perm[(kk + k2) % P + perm[(ll + l2) % P]]]] % 32;
                gi3 = perm[(ii + i3) % P + perm[(jj + j3) % P + perm[(kk + k3) % P + perm[(ll + l3) % P]]]] % 32;
                gi4 = perm[(ii + 1) % P + perm[(jj + 1) % P + perm[(kk + 1) % P + perm[(ll + 1) % P]]]] % 32;
            }
            // Calculate the contribution from the five corners
            float t0 = 0.6f - x0 * x0 - y0 * y0 - z0 * z0 - w0 * w0;
            if (t0 < 0) n0 = 0.0f;
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * dot(grad4[gi0], x0, y0, z0, w0);
            }
            float t1 = 0.6f - x1 * x1 - y1 * y1 - z1 * z1 - w1 * w1;
            if (t1 < 0) n1 = 0.0f;
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * dot(grad4[gi1], x1, y1, z1, w1);
            }
            float t2 = 0.6f - x2 * x2 - y2 * y2 - z2 * z2 - w2 * w2;
            if (t2 < 0) n2 = 0.0f;
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * dot(grad4[gi2], x2, y2, z2, w2);
            }
            float t3 = 0.6f - x3 * x3 - y3 * y3 - z3 * z3 - w3 * w3;
            if (t3 < 0) n3 = 0.0f;
            else
            {
                t3 *= t3;
                n3 = t3 * t3 * dot(grad4[gi3], x3, y3, z3, w3);
            }
            float t4 = 0.6f - x4 * x4 - y4 * y4 - z4 * z4 - w4 * w4;
            if (t4 < 0) n4 = 0.0f;
            else
            {
                t4 *= t4;
                n4 = t4 * t4 * dot(grad4[gi4], x4, y4, z4, w4);
            }
            // Sum up and scale the result to cover the range [-1,1]
            return 27.0f * (n0 + n1 + n2 + n3 + n4);
        }

        /**
         * Computes a 2D Fbm noise function in a 2D float array. This function is a sum
         * of several Perlin noise function with different frequencies and amplitudes.
         * @ingroup proland_math
         *
         * @param size the width and height of the array of values to be computed.
         * @param freq the pseudo frequency of the lower frequency noise function. The
         *      corresponding pseudo period in pixels is size/freq.
         * @param octaves the number of Perlin noise functions to add.
         * @param lacunarity the frequency factor between each noise function.
         * @param gain the amplitude factor between each noise function.
         * @return the computed size*size array of values. These values are normalized
         *      to stay in the range 0-1.
         */
        public static float[] buildFbm4NoiseTexture2D(int size, int freq, int octaves, int lacunarity, float gain)
        {
            int i, j, k, l;
            float[] _base = new float[4 * size * size];
            float[] data = new float[4 * size * size];
            float c = ((float)freq) / size;

            for (j = 0; j < size; ++j)
            {
                for (i = 0; i < size; ++i)
                {
                    _base[4 * (i + j * size)] = cnoise(i * c + 0.33f, j * c + 0.33f, freq);
                    _base[4 * (i + j * size) + 1] = cnoise(-i * c + 0.33f, j * c + 0.33f, freq);
                    _base[4 * (i + j * size) + 2] = cnoise(i * c + 0.33f, -j * c + 0.33f, freq);
                    _base[4 * (i + j * size) + 3] = cnoise(-i * c + 0.33f, -j * c + 0.33f, freq);
                }
            }

            float max = 0;
            for (j = 0; j < size; ++j)
            {
                for (i = 0; i < size; ++i)
                {
                    for (k = 0; k < 4; ++k)
                    {
                        int ip = i;
                        int jp = j;
                        float amp = 1;
                        float f = 0;
                        for (l = 0; l < octaves; ++l)
                        {
                            f += _base[4 * (ip + jp * size) + k] * amp;
                            ip = (ip * lacunarity) % size;
                            jp = (jp * lacunarity) % size;
                            amp *= gain;
                        }
                        data[4 * (i + j * size) + k] = f;
                        max = Math.Max(Math.Abs(f), max);
                    }
                }
            }

            for (j = 0; j < size; ++j)
            {
                for (i = 0; i < size; ++i)
                {
                    for (k = 0; k < 4; ++k)
                    {
                        data[4 * (i + j * size) + k] = 0.5f + data[4 * (i + j * size) + k] / max * 0.5f;
                    }
                }
            }

            return data;
        }

        /**
         * Computes a 3D Fbm noise function in a 3D float array. This function is a sum
         * of several Perlin noise function with different frequencies and amplitudes.
         * @ingroup proland_math
         *
         * @param size the width, height and depth of the array of values to be computed.
         * @param freq the pseudo frequency of the lower frequency noise function. The
         *      corresponding pseudo period in pixels is size/freq.
         * @param octaves the number of Perlin noise functions to add.
         * @param lacunarity the frequency factor between each noise function.
         * @param gain the amplitude factor between each noise function.
         * @return the computed size*size*size array of values. These values are
         *      normalized to stay in the range 0-1.
         */
        public static float[] buildFbm1NoiseTexture3D(int size, int freq, int octaves, int lacunarity, float gain)
        {
            int i, j, k, l;
            float[] _base = new float[size * size * size];
            float[] data = new float[size * size * size];
            float c = ((float)freq) / size;

            for (k = 0; k < size; ++k)
            {
                for (j = 0; j < size; ++j)
                {
                    for (i = 0; i < size; ++i)
                    {
                        _base[(i + j * size + k * size * size)] = cnoise(i * c + 0.33f, j * c + 0.33f, k * c + 0.33f, freq);
                    }
                }
            }

            float max = 0;
            for (k = 0; k < size; ++k)
            {
                for (j = 0; j < size; ++j)
                {
                    for (i = 0; i < size; ++i)
                    {
                        int ip = i;
                        int jp = j;
                        int kp = k;
                        float amp = 1;
                        float f = 0;
                        for (l = 0; l < octaves; ++l)
                        {
                            f += _base[ip + jp * size + kp * size * size] * amp;
                            ip = (ip * lacunarity) % size;
                            jp = (jp * lacunarity) % size;
                            kp = (kp * lacunarity) % size;
                            amp *= gain;
                        }
                        data[i + j * size + k * size * size] = f;
                        max = Math.Max(Math.Abs(f), max);
                    }
                }
            }

            for (k = 0; k < size; ++k)
            {
                for (j = 0; j < size; ++j)
                {
                    for (i = 0; i < size; ++i)
                    {
                        data[i + j * size + k * size * size] = 0.5f + data[i + j * size + k * size * size] / max * 0.5f;
                    }
                }
            }

            return data;
        }
    }
}
