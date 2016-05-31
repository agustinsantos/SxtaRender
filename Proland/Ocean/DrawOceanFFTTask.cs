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

using log4net;
using proland;
using Sxta.Core;
using Sxta.Math;
using Sxta.Render;
using Sxta.Render.Resources;
using Sxta.Render.Scenegraph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sxta.Proland.Ocean
{
    /// <summary>
    /// An AbstractTask to draw a flat or spherical ocean.
    /// </summary>
    public class DrawOceanFFTTask : AbstractTask, ISwappable<DrawOceanFFTTask>
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /*
         * Creates a new DrawOceanFFTTask.
         *
         * @param radius the radius of the planet for a spherical ocean, or
         *      0 for a flat ocean.
         * @param zmin the maximum altitude at which the ocean must be
         *      displayed.
         * @param brdfShader the Shader used to render the ocean surface.
         */
        public DrawOceanFFTTask(float radius, float zmin, Sxta.Render.Module brdfShader) :
            base("DrawOceanTask")
        {
            init(radius, zmin, fftInit, fftx, ffty, variances, brdfShader);
        }
        public DrawOceanFFTTask(float radius, float zmin, Program fftInit, Program fftx, Program ffty, Program variances, Module brdfShader) :
            base("DrawOceanTask")
        {
            init(radius, zmin, fftInit, fftx, ffty, variances, brdfShader);
        }
        /*
         * Creates an uninitialized DrawOceanTask.
         */
        protected DrawOceanFFTTask() : base("DrawOceanFFTTask")
        {
        }

        public override Sxta.Render.Scenegraph.Task getTask(Object context)
        {
            SceneNode n = ((Method)(context)).getOwner();
            return new Impl(n, this);
        }

        /*
     * Initializes this DrawOceanFFTTask.
     *
     * @param radius the radius of the planet for a spherical ocean, or
     *      0 for a flat ocean.
     * @param zmin the maximum altitude at which the ocean must be
     *      displayed.
     * @param brdfShader the Shader used to display the ocean surface.
     */
        public void init(float radius, float zmin, Program fftInit, Program fftx, Program ffty, Program variances, Module brdfShader)
        {
            this.radius = radius;
            this.zmin = zmin;
            this.seaColor = new Vector3f(10.0f / 255.0f, 40.0f / 255.0f, 120.0f / 255.0f) * 0.1f;

            this.fftInit = fftInit;
            this.fftx = fftx;
            this.ffty = ffty;
            this.variances = variances;

            spectrum12 = new Texture2D(FFT_SIZE, FFT_SIZE, TextureInternalFormat.RGBA16F, TextureFormat.RGBA, PixelType.FLOAT,
                new Texture.Parameters().min(TextureFilter.NEAREST).mag(TextureFilter.NEAREST).wrapS(TextureWrap.REPEAT).wrapT(TextureWrap.REPEAT),
                new Render.Buffer.Parameters(), new CPUBuffer<byte>(null));
            spectrum34 = new Texture2D(FFT_SIZE, FFT_SIZE, TextureInternalFormat.RGBA16F, TextureFormat.RGBA, PixelType.FLOAT,
                new Texture.Parameters().min(TextureFilter.NEAREST).mag(TextureFilter.NEAREST).wrapS(TextureWrap.REPEAT).wrapT(TextureWrap.REPEAT),
                new Render.Buffer.Parameters(), new CPUBuffer<byte>(null));
            slopeVariances = new Texture3D(N_SLOPE_VARIANCE, N_SLOPE_VARIANCE, N_SLOPE_VARIANCE, TextureInternalFormat.R16F, TextureFormat.RED, PixelType.FLOAT,
                new Texture.Parameters().min(TextureFilter.NEAREST).mag(TextureFilter.NEAREST).wrapS(TextureWrap.CLAMP_TO_EDGE).wrapT(TextureWrap.CLAMP_TO_EDGE).wrapR(TextureWrap.CLAMP_TO_EDGE),
                new Render.Buffer.Parameters(), new CPUBuffer<byte>(null));
            ffta = new Texture2DArray(FFT_SIZE, FFT_SIZE, 5, TextureInternalFormat.RGBA16F, TextureFormat.RGBA, PixelType.FLOAT,
                (Texture.Parameters)(new Texture.Parameters().min(TextureFilter.LINEAR_MIPMAP_LINEAR).mag(TextureFilter.LINEAR).wrapS(TextureWrap.REPEAT).wrapT(TextureWrap.REPEAT).maxAnisotropyEXT(16.0f)),
                new Render.Buffer.Parameters(), new CPUBuffer<byte>(null));
            fftb = new Texture2DArray(FFT_SIZE, FFT_SIZE, 5, TextureInternalFormat.RGBA16F, TextureFormat.RGBA, PixelType.FLOAT,
                 (Texture.Parameters)(new Texture.Parameters().min(TextureFilter.LINEAR_MIPMAP_LINEAR).mag(TextureFilter.LINEAR).wrapS(TextureWrap.REPEAT).wrapT(TextureWrap.REPEAT).maxAnisotropyEXT(16.0f)),
                new Render.Buffer.Parameters(), new CPUBuffer<byte>(null));
            float[] data = computeButterflyLookupTexture();
            Texture2D butterfly = new Texture2D(FFT_SIZE, PASSES, TextureInternalFormat.RGBA16F, TextureFormat.RGBA, PixelType.FLOAT,
               new Texture.Parameters().min(TextureFilter.NEAREST).mag(TextureFilter.NEAREST).wrapS(TextureWrap.CLAMP_TO_EDGE).wrapT(TextureWrap.CLAMP_TO_EDGE),
               new Render.Buffer.Parameters(), new CPUBuffer<float>(data));
            //delete[] data;
            fftFbo1 = new FrameBuffer();
            fftFbo1.setTextureBuffer(BufferId.COLOR0, ffta, 0, 0);
            fftFbo1.setTextureBuffer(BufferId.COLOR1, ffta, 0, 1);
            fftFbo1.setTextureBuffer(BufferId.COLOR2, ffta, 0, 2);
            fftFbo1.setTextureBuffer(BufferId.COLOR3, ffta, 0, 3);
            fftFbo1.setTextureBuffer(BufferId.COLOR4, ffta, 0, 4);
            fftFbo1.setDrawBuffers(BufferId.COLOR0 | BufferId.COLOR1 | BufferId.COLOR2 | BufferId.COLOR3 | BufferId.COLOR4);
            fftFbo1.setViewport(new Vector4i(0, 0, FFT_SIZE, FFT_SIZE));
            fftFbo2 = new FrameBuffer();
            fftFbo2.setTextureBuffer(BufferId.COLOR0, ffta, 0, -1);
            fftFbo2.setTextureBuffer(BufferId.COLOR1, fftb, 0, -1);
            fftFbo2.setViewport(new Vector4i(0, 0, FFT_SIZE, FFT_SIZE));
            variancesFbo = new FrameBuffer();
            variancesFbo.setViewport(new Vector4i(0, 0, N_SLOPE_VARIANCE, N_SLOPE_VARIANCE));

            fftInit.getUniformSampler("spectrum_1_2_Sampler").set(spectrum12);
            fftInit.getUniformSampler("spectrum_3_4_Sampler").set(spectrum34);
            fftInit.getUniform1f("FFT_SIZE").set(FFT_SIZE);
            fftInit.getUniform4f("INVERSE_GRID_SIZES").set(new Vector4f(
                2.0f * M_PI * FFT_SIZE / GRID1_SIZE,
                2.0f * M_PI * FFT_SIZE / GRID2_SIZE,
                2.0f * M_PI * FFT_SIZE / GRID3_SIZE,
                2.0f * M_PI * FFT_SIZE / GRID4_SIZE));

            fftx.getUniform1i("nLayers").set(5);
            fftx.getUniformSampler("butterflySampler").set(butterfly);
            ffty.getUniform1i("nLayers").set(5);
            ffty.getUniformSampler("butterflySampler").set(butterfly);

            generateWavesSpectrum(spectrum12, spectrum34);

            if (variances != null)
            {
                variances.getUniformSampler("spectrum_1_2_Sampler").set(spectrum12);
                variances.getUniformSampler("spectrum_3_4_Sampler").set(spectrum34);
                variances.getUniform1i("FFT_SIZE").set(FFT_SIZE);
                variances.getUniform1f("N_SLOPE_VARIANCE").set(N_SLOPE_VARIANCE);
                computeSlopeVariances(variancesFbo, variances, slopeVariances);
            }

            this.resolution = 8;
            this.oldLtoo = Matrix4d.Identity;
            this.offset = Vector3d.Zero;
            this.brdfShader = brdfShader;
        }

        public void swap(DrawOceanFFTTask t)
        {
            DrawOceanFFTTask tmp = this;
            Std.Swap(ref tmp, ref t);
        }

        /*
      * The radius of the planet for a spherical ocean, or 0 for a flat ocean.
      */
        private float radius;

        /*
         * The maximum altitude at which the ocean must be displayed.
         */
        private float zmin;

        /*
         * Color of the seabed.
         */
        private Vector3f seaColor;

        // -------

        private Program fftInit;

        private Program fftx;

        private Program ffty;

        private Program variances;

        private Texture2D spectrum12;

        private Texture2D spectrum34;

        private Texture3D slopeVariances;

        private Texture2DArray ffta;

        private Texture2DArray fftb;

        private FrameBuffer fftFbo1;

        private FrameBuffer fftFbo2;

        private FrameBuffer variancesFbo;

        // -------

        /*
         * Number of pixels per cell to use for the screen space grid
         * used to display the ocean surface.
         */
        private int resolution;

        /*
         * Current width of the viewport, in pixels.
         */
        private int screenWidth;

        /*
         * Current height of the viewport, in pixels.
         */
        private int screenHeight;

        /*
         * The mesh used to display the ocean surface.
         */
        private Mesh<Vector2f, uint> screenGrid;

        // -------

        private Matrix4d oldLtoo;

        private Vector3d offset;

        // -------

        /**
         * The Shader used to render the ocean surface.
         */
        private Module brdfShader;

        private UniformMatrix4f cameraToOceanU;

        private UniformMatrix4f screenToCameraU;

        private UniformMatrix4f cameraToScreenU;

        private UniformMatrix4f oceanToWorldU;

        private UniformMatrix3f oceanToCameraU;

        private Uniform3f oceanCameraPosU;

        private Uniform3f oceanSunDirU;

        private Uniform3f horizon1U;

        private Uniform3f horizon2U;

        private Uniform1f radiusU;

        private Uniform1f heightOffsetU;

        private Uniform2f gridSizeU;

        private const int PASSES = 8; // number of passes needed for the FFT 6 . 64, 7 . 128, 8 . 256, etc
        private const int FFT_SIZE = 1 << PASSES; // size of the textures storing the waves in frequency and spatial domains
        private const int N_SLOPE_VARIANCE = 10;

        // ----------------------------------------------------------------------------
        // WAVES SPECTRUM GENERATION
        // ----------------------------------------------------------------------------

        private static float GRID1_SIZE = 5488.0f; // size in meters (i.e. in spatial domain) of the first grid

        private static float GRID2_SIZE = 392.0f; // size in meters (i.e. in spatial domain) of the second grid

        private static float GRID3_SIZE = 28.0f; // size in meters (i.e. in spatial domain) of the third grid

        private static float GRID4_SIZE = 2.0f; // size in meters (i.e. in spatial domain) of the fourth grid

        private static float WIND = 5.0f; // wind speed in meters per second (at 10m above surface)

        private static float OMEGA = 0.84f; // sea state (inverse wave age)

        private static bool propagate = true; // wave propagation?

        private static float A = 1.0f; // wave amplitude factor (should be one)

        private const float cm = 0.23f; // Eq 59

        private const float km = 370.0f; // Eq 59

        private static float maxSlopeVariance = 0.0f;

        private static float sqr(float x) { return x * x; }
        private static float omega(float k)
        {
            return sqr(9.81f * k * (1.0f + sqr(k / km))); // Eq 24
        }

        // 1/kx and 1/ky in meters
        private static float spectrum(float kx, float ky, bool omnispectrum = false)
        {
            float U10 = WIND;
            float Omega = OMEGA;

            // phase speed
            float k = sqr(kx * kx + ky * ky);
            float c = omega(k) / k;

            // spectral peak
            float kp = 9.81f * sqr(Omega / U10); // after Eq 3
            float cp = omega(kp) / kp;

            // friction velocity
            float z0 = 3.7e-5f * sqr(U10) / 9.81f * (float)System.Math.Pow(U10 / cp, 0.9f); // Eq 66
            float u_star = 0.41f * U10 / (float)System.Math.Log(10.0 / z0); // Eq 60

            float Lpm = (float)System.Math.Exp(-5.0 / 4.0 * sqr(kp / k)); // after Eq 3
            float gamma = Omega < 1.0f ? 1.7f : 1.7f + 6.0f * (float)System.Math.Log(Omega); // after Eq 3 // log10 or log??
            float sigma = 0.08f * (1.0f + 4.0f / (float)System.Math.Pow(Omega, 3.0f)); // after Eq 3
            float Gamma = (float)System.Math.Exp(-1.0f / (2.0f * sqr(sigma)) * sqr((float)System.Math.Sqrt(k / kp) - 1.0f));
            float Jp = (float)System.Math.Pow(gamma, Gamma); // Eq 3
            float Fp = Lpm * Jp * (float)System.Math.Exp(-Omega / (float)System.Math.Sqrt(10.0) * ((float)System.Math.Sqrt(k / kp) - 1.0)); // Eq 32
            float alphap = 0.006f * (float)System.Math.Sqrt(Omega); // Eq 34
            float Bl = 0.5f * alphap * cp / c * Fp; // Eq 31

            float alpham = 0.01f * (u_star < cm ? 1.0f + (float)System.Math.Log(u_star / cm) : 1.0f + 3.0f * (float)System.Math.Log(u_star / cm)); // Eq 44
            float Fm = (float)System.Math.Exp(-0.25 * sqr(k / km - 1.0f)); // Eq 41
            float Bh = 0.5f * alpham * cm / c * Fm; // Eq 40

            Bh *= Lpm; // bug fix???

            if (omnispectrum)
            {
                return A * (Bl + Bh) / (k * sqr(k)); // Eq 30
            }

            float a0 = (float)System.Math.Log(2.0) / 4.0f; float ap = 4.0f; float am = 0.13f * u_star / cm; // Eq 59
            float Delta = (float)System.Math.Tanh(a0 + ap * (float)System.Math.Pow(c / cp, 2.5f) + am * (float)System.Math.Pow(cm / c, 2.5f)); // Eq 57

            float phi = (float)System.Math.Atan2(ky, kx);

            if (propagate)
            {
                if (kx < 0.0)
                {
                    return 0.0f;
                }
                else
                {
                    Bl *= 2.0f;
                    Bh *= 2.0f;
                }
            }

            return A * (Bl + Bh) * (1.0f + Delta * (float)System.Math.Cos(2.0f * phi)) / (2.0f * M_PI * sqr(sqr(k))); // Eq 67
        }

        static int seed = 1234;
        static Random rnd = new Random(seed);

        private static Vector2f getSpectrumSample(int i, int j, float lengthScale, float kMin)
        {
            Vector2f result = new Vector2f(0.0f, 0.0f);

            float dk = 2.0f * M_PI / lengthScale;
            float kx = i * dk;
            float ky = j * dk;
            if (System.Math.Abs(kx) < kMin && System.Math.Abs(ky) < kMin)
            {
                result.X = 0.0f;
                result.Y = 0.0f;
            }
            else
            {

                float S = spectrum(kx, ky);
                float h = (float)System.Math.Sqrt(S / 2.0f) * dk;
                float phi = (float)rnd.NextDouble() * 2.0f * M_PI;
                result.X = h * (float)System.Math.Cos(phi);
                result.Y = h * (float)System.Math.Sin(phi);
            }
            return result;
        }

        // generates the waves spectrum
        private static float[] spectrum01 = new float[FFT_SIZE * FFT_SIZE * 4];
        private static float[] spectrum23 = new float[FFT_SIZE * FFT_SIZE * 4];
        public static void generateWavesSpectrum(Texture2D spectrum12Tex, Texture2D spectrum34Tex)
        {
            Vector2f sample12XY;
            Vector2f sample12ZW;
            Vector2f sample34XY;
            Vector2f sample34ZW;

            for (int y = 0; y < FFT_SIZE; ++y)
            {
                for (int x = 0; x < FFT_SIZE; ++x)
                {
                    int offset = 4 * (x + y * FFT_SIZE);
                    int i = x >= FFT_SIZE / 2 ? x - FFT_SIZE : x;
                    int j = y >= FFT_SIZE / 2 ? y - FFT_SIZE : y;
                    sample12XY = getSpectrumSample(i, j, GRID1_SIZE, M_PI / GRID1_SIZE);
                    sample12ZW = getSpectrumSample(i, j, GRID2_SIZE, M_PI * FFT_SIZE / GRID1_SIZE);
                    sample34XY = getSpectrumSample(i, j, GRID3_SIZE, M_PI * FFT_SIZE / GRID2_SIZE);
                    sample34ZW = getSpectrumSample(i, j, GRID4_SIZE, M_PI * FFT_SIZE / GRID3_SIZE);

                    spectrum01[offset + 0] = sample12XY.X;
                    spectrum01[offset + 1] = sample12XY.Y;
                    spectrum01[offset + 2] = sample12ZW.X;
                    spectrum01[offset + 3] = sample12ZW.Y;

                    spectrum23[offset + 0] = sample34XY.X;
                    spectrum23[offset + 1] = sample34XY.Y;
                    spectrum23[offset + 2] = sample34ZW.X;
                    spectrum23[offset + 3] = sample34ZW.Y;
                }
            }

            spectrum12Tex.setSubImage(0, 0, 0, FFT_SIZE, FFT_SIZE, TextureFormat.RGBA, PixelType.FLOAT, new Render.Buffer.Parameters(), new CPUBuffer<float>(spectrum01));
            spectrum34Tex.setSubImage(0, 0, 0, FFT_SIZE, FFT_SIZE, TextureFormat.RGBA, PixelType.FLOAT, new Render.Buffer.Parameters(), new CPUBuffer<float>(spectrum23));
        }

        float getSlopeVariance(float kx, float ky, Vector2f spectrumSample)
        {
            float kSquare = kx * kx + ky * ky;
            float real = spectrumSample.X;
            float img = spectrumSample.Y;
            float hSquare = real * real + img * img;
            return kSquare * hSquare * 2.0f;
        }

        // precomputes filtered slope variances in a 3d texture, based on the wave spectrum
        void computeSlopeVariances(FrameBuffer fbo, Program variances, Texture3D variancesTex)
        {

            // slope variance due to all waves, by integrating over the full spectrum
            float theoreticSlopeVariance = 0.0f;
            float k = 5e-3f;
            while (k < 1e3)
            {
                float nextK = k * 1.001f;
                theoreticSlopeVariance += k * k * spectrum(k, 0, true) * (nextK - k);
                k = nextK;
            }

            // slope variance due to waves, by integrating over the spectrum part
            // that is covered by the four nested grids. This can give a smaller result
            // than the theoretic total slope variance, because the higher frequencies
            // may not be covered by the four nested grid. Hence the difference between
            // the two is added as a "delta" slope variance in the "variances" shader,
            // to be sure not to lose the variance due to missing wave frequencies in
            // the four nested grids
            float totalSlopeVariance = 0.0f;
            for (int y = 0; y < FFT_SIZE; ++y)
            {
                for (int x = 0; x < FFT_SIZE; ++x)
                {
                    int offset = 4 * (x + y * FFT_SIZE);
                    Vector2f sample12XY = new Vector2f(spectrum01[offset + 0], spectrum01[offset + 1]);
                    Vector2f sample12ZW = new Vector2f(spectrum01[offset + 2], spectrum01[offset + 3]);
                    Vector2f sample34XY = new Vector2f(spectrum23[offset + 0], spectrum23[offset + 1]);
                    Vector2f sample34ZW = new Vector2f(spectrum23[offset + 0], spectrum23[offset + 1]);

                    float i = 2.0f * M_PI * (x >= FFT_SIZE / 2 ? x - FFT_SIZE : x);
                    float j = 2.0f * M_PI * (y >= FFT_SIZE / 2 ? y - FFT_SIZE : y);
                    totalSlopeVariance += getSlopeVariance(i / GRID1_SIZE, j / GRID1_SIZE, sample12XY);
                    totalSlopeVariance += getSlopeVariance(i / GRID2_SIZE, j / GRID2_SIZE, sample12ZW);
                    totalSlopeVariance += getSlopeVariance(i / GRID3_SIZE, j / GRID3_SIZE, sample34XY);
                    totalSlopeVariance += getSlopeVariance(i / GRID4_SIZE, j / GRID4_SIZE, sample34ZW);
                }
            }

            variances.getUniform4f("GRID_SIZES").set(new Vector4f(GRID1_SIZE, GRID2_SIZE, GRID3_SIZE, GRID4_SIZE));
            variances.getUniform1f("slopeVarianceDelta").set(theoreticSlopeVariance - totalSlopeVariance);

            for (int layer = 0; layer < N_SLOPE_VARIANCE; ++layer)
            {
                fbo.setTextureBuffer(BufferId.COLOR0, variancesTex, 0, layer);
                variances.getUniform1f("c").set(layer);
                fbo.drawQuad(variances);
            }

            maxSlopeVariance = 0.0f;
            byte[] data = new byte[N_SLOPE_VARIANCE * N_SLOPE_VARIANCE * N_SLOPE_VARIANCE * sizeof(float)];
            variancesTex.getImage(0, TextureFormat.RED, PixelType.FLOAT, data);
            for (int i = 0; i < N_SLOPE_VARIANCE * N_SLOPE_VARIANCE * N_SLOPE_VARIANCE; ++i)
            {
                float d = BitConverter.ToSingle(data, i * 4);
                maxSlopeVariance = System.Math.Max(maxSlopeVariance, d);
            }
            //delete[] data;
        }

        // ----------------------------------------------------------------------------
        // WAVES GENERATION AND ANIMATION (using FFT on GPU)
        // ----------------------------------------------------------------------------

        int bitReverse(int i, int N)
        {
            int j = i;
            int M = N;
            int Sum = 0;
            int W = 1;
            M = M / 2;
            while (M != 0)
            {
                j = ((i & M) > M - 1) ? 1 : 0;
                //j = ((i & M) > M ? 1 : 0) - 1; TODO check operator order
                Sum += j * W;
                W *= 2;
                M = M / 2;
            }
            return Sum;
        }

        private void computeWeight(int N, int k, out float Wr, out float Wi)
        {
            Wr = (float)System.Math.Cos(2.0f * M_PI * k / (float)N);
            Wi = (float)System.Math.Sin(2.0f * M_PI * k / (float)N);
        }

        private float[] computeButterflyLookupTexture()
        {
            float[] data = new float[FFT_SIZE * PASSES * 4];

            for (int i = 0; i < PASSES; i++)
            {
                int nBlocks = (int)System.Math.Pow(2.0, (float)(PASSES - 1 - i));
                int nHInputs = (int)System.Math.Pow(2.0, (float)(i));
                for (int j = 0; j < nBlocks; j++)
                {
                    for (int k = 0; k < nHInputs; k++)
                    {
                        int i1, i2, j1, j2;
                        if (i == 0)
                        {
                            i1 = j * nHInputs * 2 + k;
                            i2 = j * nHInputs * 2 + nHInputs + k;
                            j1 = bitReverse(i1, FFT_SIZE);
                            j2 = bitReverse(i2, FFT_SIZE);
                        }
                        else
                        {
                            i1 = j * nHInputs * 2 + k;
                            i2 = j * nHInputs * 2 + nHInputs + k;
                            j1 = i1;
                            j2 = i2;
                        }

                        float wr, wi;
                        computeWeight(FFT_SIZE, k * nBlocks, out wr, out wi);

                        int offset1 = 4 * (i1 + i * FFT_SIZE);
                        data[offset1 + 0] = (j1 + 0.5f) / FFT_SIZE;
                        data[offset1 + 1] = (j2 + 0.5f) / FFT_SIZE;
                        data[offset1 + 2] = wr;
                        data[offset1 + 3] = wi;

                        int offset2 = 4 * (i2 + i * FFT_SIZE);
                        data[offset2 + 0] = (j1 + 0.5f) / FFT_SIZE;
                        data[offset2 + 1] = (j2 + 0.5f) / FFT_SIZE;
                        data[offset2 + 2] = -wr;
                        data[offset2 + 3] = -wi;
                    }
                }
            }

            return data;
        }

        private void simulateFFTWaves(float t)
        {
            // init
            fftInit.getUniform1f("t").set(t);
            fftFbo1.drawQuad(fftInit);

            // fft passes
            for (int i = 0; i < PASSES; ++i)
            {
                fftx.getUniform1f("pass").set((float)(i + 0.5f) / PASSES);
                if (i % 2 == 0)
                {
                    fftx.getUniformSampler("imgSampler").set(ffta);
                    fftFbo2.setDrawBuffer(BufferId.COLOR1);
                }
                else
                {
                    fftx.getUniformSampler("imgSampler").set(fftb);
                    fftFbo2.setDrawBuffer(BufferId.COLOR0);
                }
                fftFbo2.drawQuad(fftx);
            }
            for (int i = PASSES; i < 2 * PASSES; ++i)
            {
                ffty.getUniform1f("pass").set((float)(i - PASSES + 0.5f) / PASSES);
                if (i % 2 == 0)
                {
                    ffty.getUniformSampler("imgSampler").set(ffta);
                    fftFbo2.setDrawBuffer(BufferId.COLOR1);
                }
                else
                {
                    ffty.getUniformSampler("imgSampler").set(fftb);
                    fftFbo2.setDrawBuffer(BufferId.COLOR0);
                }
                fftFbo2.drawQuad(ffty);
            }

            ffta.generateMipMap();
        }

        class Impl : Task
        {

            public SceneNode n;

            public DrawOceanFFTTask o;

            public Impl(SceneNode n, DrawOceanFFTTask owner) :
                base("DrawOcean", true, 0)
            {
                this.n = n;
                this.o = owner;
            }


            public override bool run()
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("DrawOcean run task");
                }
                FrameBuffer fb = SceneManager.getCurrentFrameBuffer();
                Program prog = SceneManager.getCurrentProgram();

                if (o.cameraToOceanU == null)
                {
                    o.cameraToOceanU = prog.getUniformMatrix4f("cameraToOcean");
                    o.screenToCameraU = prog.getUniformMatrix4f("screenToCamera");
                    o.cameraToScreenU = prog.getUniformMatrix4f("cameraToScreen");
                    o.oceanToCameraU = prog.getUniformMatrix3f("oceanToCamera");
                    o.oceanToWorldU = prog.getUniformMatrix4f("oceanToWorld");
                    o.oceanCameraPosU = prog.getUniform3f("oceanCameraPos");
                    o.oceanSunDirU = prog.getUniform3f("oceanSunDir");
                    o.horizon1U = prog.getUniform3f("horizon1");
                    o.horizon2U = prog.getUniform3f("horizon2");
                    o.radiusU = prog.getUniform1f("radius");
                    o.heightOffsetU = prog.getUniform1f("heightOffset");
                    o.gridSizeU = prog.getUniform2f("gridSize");
                    prog.getUniformSampler("fftWavesSampler").set(o.ffta);
                    if (prog.getUniformSampler("slopeVarianceSampler") != null)
                    {
                        prog.getUniformSampler("slopeVarianceSampler").set(o.slopeVariances);
                    }
                    prog.getUniform4f("GRID_SIZES").set(new Vector4f(GRID1_SIZE, GRID2_SIZE, GRID3_SIZE, GRID4_SIZE));

                    if (o.brdfShader != null)
                    {
                        Debug.Assert(o.brdfShader.getUsers().Count != 0);
                        Program p = o.brdfShader.getUsers().First();
                        Uniform1f u = p.getUniform1f("seaRoughness");
                        if (u != null)
                        {
                            u.set(maxSlopeVariance);
                        }
                        p.getUniform3f("seaColor").set(o.seaColor);
                    }
                }

                //List<TileSampler> uniforms;
                foreach (object obj in n.getFields())
                {
                    TileSampler u = obj as TileSampler;
                    if (u != null && u.getTerrain(0) != null)
                    {
                        u.setTileMap();
                    }
                }

                // compute ltoo = localToOcean transform, where ocean frame = tangent space at
                // camera projection on sphere o.radius in local space
                Matrix4d ctol = Matrix4d.Invert(n.getLocalToCamera());
                Vector3d cl = ctol * Vector3d.Zero; // camera in local space

                if ((o.radius == 0.0 && cl.Z > o.zmin) ||
                    (o.radius > 0.0 && cl.Length > o.radius + o.zmin) ||
                    (o.radius < 0.0 && new Vector2d(cl.Y, cl.Z).Length < -o.radius - o.zmin))
                {
                    o.oldLtoo = Matrix4d.Identity;
                    o.offset = Vector3d.Zero;
                    return true;
                }

                Vector3d ux, uy, uz, oo;

                if (o.radius == 0.0)
                {
                    // flat ocean
                    ux = Vector3d.UnitX;
                    uy = Vector3d.UnitY;
                    uz = Vector3d.UnitZ;
                    oo = new Vector3d(cl.X, cl.Y, 0.0);
                }
                else if (o.radius > 0.0)
                {
                    // spherical ocean
                    uz = Vector3d.Normalize(cl); // unit z vector of ocean frame, in local space
                    if (o.oldLtoo != Matrix4d.Identity)
                    {
                        ux = Vector3d.Normalize(Vector3d.Cross(new Vector3d(o.oldLtoo.R1C0/*[1][0]*/, o.oldLtoo.R1C1/*[1][1]*/, o.oldLtoo.R1C2/*[1][2]*/), uz));
                    }
                    else
                    {
                        ux = Vector3d.Normalize(Vector3d.Cross(Vector3d.UnitZ, uz));
                    }
                    uy = Vector3d.Cross(uz, ux); // unit y vector
                    oo = uz * o.radius; // origin of ocean frame, in local space
                }
                else
                {
                    // cylindrical ocean
                    uz = Vector3d.Normalize(new Vector3d(0.0, -cl.Y, -cl.Z));
                    ux = Vector3d.UnitX;
                    uy = Vector3d.Cross(uz, ux);
                    oo = new Vector3d(cl.X, 0.0, 0.0) + uz * o.radius;
                }

                Matrix4d ltoo = new Matrix4d(
                    ux.X, ux.Y, ux.Z, Vector3d.Dot(-ux, oo),
                    uy.X, uy.Y, uy.Z, Vector3d.Dot(-uy, oo),
                    uz.X, uz.Y, uz.Z, Vector3d.Dot(-uz, oo),
                    0.0, 0.0, 0.0, 1.0);
                // compute ctoo = cameraToOcean transform
                Matrix4d ctoo = ltoo * ctol;

                if (o.oldLtoo != Matrix4d.Identity)
                {
                    Vector3d delta = ltoo * (Matrix4d.Invert(o.oldLtoo) * Vector3d.Zero);
                    o.offset += delta;
                }
                o.oldLtoo = ltoo;

                Matrix4d ctos = n.getOwner().getCameraToScreen();
                Matrix4d stoc = Matrix4d.Invert(ctos);
                Vector3d oc = ctoo * Vector3d.Zero;

                if (o.oceanSunDirU != null)
                {
                    // TODO how to get sun dir in a better way?
                    SceneNode l = n.getOwner().getNodes("light").FirstOrDefault();
                    if (l != null)
                    {
                        Vector3d worldSunDir = l.getLocalToParent() * Vector3d.Zero;
                        Vector3d oceanSunDir = ltoo.Mat3x3 * (n.getWorldToLocal().Mat3x3 * worldSunDir);
                        o.oceanSunDirU.set((Vector3f)oceanSunDir);
                    }
                }

                Vector4i screen = fb.getViewport();

                o.cameraToOceanU.setMatrix((Matrix4f)ctoo);
                o.screenToCameraU.setMatrix((Matrix4f)stoc);
                o.cameraToScreenU.setMatrix((Matrix4f)ctos);
                o.oceanToCameraU.setMatrix(mat3x3f(Matrix4d.Invert(ctoo)));
                o.oceanCameraPosU.set(new Vector3f((float)(-o.offset.X), (float)(-o.offset.Y), (float)(oc.Z)));
                if (o.oceanToWorldU != null)
                {
                    o.oceanToWorldU.setMatrix((Matrix4f)(n.getLocalToWorld() * Matrix4d.Invert(ltoo)));
                }

                if (o.horizon1U != null)
                {
                    float h = (float)oc.Z;
                    Vector3d A0 = (ctoo * new Vector4d((stoc * new Vector4d(0.0, 0.0, 0.0, 1.0)).Xyz, 0.0)).Xyz;
                    Vector3d dA = (ctoo * new Vector4d((stoc * new Vector4d(1.0, 0.0, 0.0, 0.0)).Xyz, 0.0)).Xyz;
                    Vector3d B = (ctoo * new Vector4d((stoc * new Vector4d(0.0, 1.0, 0.0, 0.0)).Xyz, 0.0)).Xyz;
                    if (o.radius == 0.0)
                    {
                        o.horizon1U.set(new Vector3f((float)(-(h * 1e-6f + A0.Z) / B.Z), (float)(-dA.Z / B.Z), 0.0f));
                        o.horizon2U.set(Vector3f.Zero);
                    }
                    else
                    {
                        double h1 = h * (h + 2.0 * o.radius);
                        double h2 = (h + o.radius) * (h + o.radius);
                        double alpha = Vector3d.Dot(B, B) * h1 - B.Z * B.Z * h2;
                        double beta0 = (Vector3d.Dot(A0, B) * h1 - B.Z * A0.Z * h2) / alpha;
                        double beta1 = (Vector3d.Dot(dA, B) * h1 - B.Z * dA.Z * h2) / alpha;
                        double gamma0 = (Vector3d.Dot(A0, A0) * h1 - A0.Z * A0.Z * h2) / alpha;
                        double gamma1 = (Vector3d.Dot(A0, dA) * h1 - A0.Z * dA.Z * h2) / alpha;
                        double gamma2 = (Vector3d.Dot(dA, dA) * h1 - dA.Z * dA.Z * h2) / alpha;
                        o.horizon1U.set(new Vector3f((float)-beta0, (float)-beta1, 0.0f));
                        o.horizon2U.set(new Vector3f((float)(beta0 * beta0 - gamma0), (float)(2.0 * (beta0 * beta1 - gamma1)), (float)(beta1 * beta1 - gamma2)));
                    }
                }

                if (o.radiusU != null)
                {
                    o.radiusU.set(o.radius < 0.0 ? -o.radius : o.radius);
                }
                o.heightOffsetU.set(0.0f);
                o.gridSizeU.set(new Vector2f(o.resolution / (float)(o.screenWidth), o.resolution / (float)(o.screenHeight)));

                if (o.screenGrid == null || o.screenWidth != screen.Z || o.screenHeight != screen.W)
                {
                    o.screenWidth = screen.Z;
                    o.screenHeight = screen.W;
                    o.screenGrid = new Mesh<Vector2f, uint>(Vector2f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLES, MeshUsage.GPU_STATIC);
                    o.screenGrid.addAttributeType(0, 2, AttributeType.A32F, false);

                    float f = 1.25f;
                    int NX = (int)(f * screen.Z / o.resolution);
                    int NY = (int)(f * screen.W / o.resolution);
                    for (int i = 0; i < NY; ++i)
                    {
                        for (int j = 0; j < NX; ++j)
                        {
                            o.screenGrid.addVertex(new Vector2f(2.0f * f * j / (NX - 1.0f) - f, 2.0f * f * i / (NY - 1.0f) - f));
                        }
                    }
                    for (int i = 0; i < NY - 1; ++i)
                    {
                        for (int j = 0; j < NX - 1; ++j)
                        {
                            o.screenGrid.addIndice((uint)(i * NX + j));
                            o.screenGrid.addIndice((uint)(i * NX + j + 1));
                            o.screenGrid.addIndice((uint)((i + 1) * NX + j));
                            o.screenGrid.addIndice((uint)((i + 1) * NX + j));
                            o.screenGrid.addIndice((uint)(i * NX + j + 1));
                            o.screenGrid.addIndice((uint)((i + 1) * NX + j + 1));
                        }
                    }
                }

                o.simulateFFTWaves((float)n.getOwner().getTime() * 1e-6f);

                fb.draw(prog, o.screenGrid);

                return true;
            }
        }

        private static Matrix3f mat3x3f(Matrix4d m)
        {
            return new Matrix3f((float)m.R0C0, (float)m.R0C1, (float)m.R0C2,
                               (float)m.R1C0, (float)m.R1C1, (float)m.R1C2,
                               (float)m.R2C0, (float)m.R2C1, (float)m.R2C2);
        }
        private const float M_PI = (float)System.Math.PI;
    }
}
