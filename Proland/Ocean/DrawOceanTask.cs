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
using System.Text;
using System.Threading.Tasks;

namespace Sxta.Proland.Ocean
{
    /// <summary>
    /// An AbstractTask to draw a flat or spherical ocean.
    /// </summary>
    public class DrawOceanTask : AbstractTask, ISwappable<DrawOceanTask>
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /*
         * Creates a new DrawOceanTask.
         *
         * @param radius the radius of the planet for a spherical ocean, or
         *      0 for a flat ocean.
         * @param zmin the maximum altitude at which the ocean must be
         *      displayed.
         * @param brdfShader the Shader used to render the ocean surface.
         */
        public DrawOceanTask(float radius, float zmin, Sxta.Render.Module brdfShader) :
            base("DrawOceanTask")
        {
            init(radius, zmin, brdfShader);
        }

        /*
         * Creates an uninitialized DrawOceanTask.
         */
        protected DrawOceanTask() : base("DrawOceanTask")
        {
        }

        public override Sxta.Render.Scenegraph.Task getTask(Object context)
        {
            SceneNode n = ((Method)(context)).getOwner();
            return new Impl(n, this);
        }

        /*
         * Initializes this DrawOceanTask.
         *
         * @param radius the radius of the planet for a spherical ocean, or
         *      0 for a flat ocean.
         * @param zmin the maximum altitude at which the ocean must be
         *      displayed.
         * @param brdfShader the Shader used to display the ocean surface.
         */
        protected void init(float radius, float zmin, Module brdfShader)
        {
            this.radius = radius;
            this.zmin = zmin;
            this.nbWaves = 60;
            this.lambdaMin = 0.02f;
            this.lambdaMax = 30.0f;
            this.heightMax = 0.4f;//0.5;
            this.seaColor = new Vector3f(10f / 255f, 40f / 255f, 120f / 255f) * 0.1f;
            this.resolution = 8;
            this.oldLtoo = Matrix4d.Identity;
            this.offset = Vector3d.Zero;
            this.brdfShader = brdfShader;
            this.nbWavesU = null;
        }

        public void swap(DrawOceanTask t)
        {
            DrawOceanTask tmp = this;
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
         * Number of wave trains used to synthesize the ocean surface.
         */
        private int nbWaves;

        /*
         * Minimum wavelength of the waves.
         */
        private float lambdaMin;

        /**
         * Maximum wavelength of the waves.
         */
        private float lambdaMax;

        /*
         * Parameter to color the height of waves.
         */
        private float heightMax;

        /*
         * Color of the seabed.
         */
        private Vector3f seaColor;

        // -------

        /*
         * Variance of the x slope over the sea surface.
         */
        private float sigmaXsq;

        /*
         * Variance of the y slope over the sea surface.
         */
        private float sigmaYsq;

        /*
         * Average height of the sea surface.
         */
        private float meanHeight;

        /*
         * Variance of the sea surface height.
         */
        private float heightVariance;

        /*
         * Maximum amplitude between crests and throughs.
         */
        private float amplitudeMax;

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

        /*
         * The Shader used to render the ocean surface.
         */
        private Module brdfShader;

        private Uniform1f nbWavesU;

        private UniformSampler wavesU;

        private UniformMatrix4f cameraToOceanU;

        private UniformMatrix4f screenToCameraU;

        private UniformMatrix4f cameraToScreenU;

        private UniformMatrix4f oceanToWorldU;

        private UniformMatrix3f oceanToCameraU;

        private Uniform3f oceanCameraPosU;

        private Uniform3f oceanSunDirU;

        private Uniform3f horizon1U;

        private Uniform3f horizon2U;

        private Uniform1f timeU;

        private Uniform1f radiusU;

        private Uniform1f heightOffsetU;

        private Uniform4f lodsU;

        public static int nbAngles = 5; // impair

        public static float angle(int i)
        {
            return (float)(1.5 * (((i) % nbAngles) / (nbAngles / 2) - 1));
        }
        public static float dangle()
        {
            return (float)(1.5 / (nbAngles / 2));
        }
        public static float srnd(long seed)
        {
            return (2 * Noise.frandom(ref seed) - 1);
        }
        private void generateWaves()
        {
            long seed = 1234567;
            float min = (float)(System.Math.Log(lambdaMin) / System.Math.Log(2.0f));
            float max = (float)(System.Math.Log(lambdaMax) / System.Math.Log(2.0f));

            Vector4f[] waves = new Vector4f[nbWaves];

            sigmaXsq = 0.0f;
            sigmaYsq = 0.0f;
            meanHeight = 0.0f;
            heightVariance = 0.0f;
            amplitudeMax = 0.0f;

            float[] Wa = new float[nbAngles]; // normalised gaussian samples
            int[] index = new int[nbAngles]; // to hash angle order
            float s = 0;
            for (int i = 0; i < nbAngles; i++)
            {
                index[i] = i;
                float a = angle(i); // (i/(float)(nbAngle/2)-1)*1.5;
                s += Wa[i] = (float)System.Math.Exp(-.5 * a * a);
            }
            for (int i = 0; i < nbAngles; i++)
            {
                Wa[i] /= s;
            }

            const float waveDispersion = 0.9f;//6;
            const float U0 = 10.0f;
            const int spectrumType = 2;

            for (int i = 0; i < nbWaves; ++i)
            {
                float x = i / (nbWaves - 1.0f);

                float lambda = (float)(System.Math.Pow(2.0f, (1.0f - x) * min + x * max));
                float ktheta = Noise.grandom(0.0f, 1.0f, ref seed) * waveDispersion;
                float knorm = 2.0f * (float)System.Math.PI / lambda;
                float omega = (float)System.Math.Sqrt(9.81f * knorm);
                float amplitude;

                if (spectrumType == 1)
                {
                    amplitude = (float)(heightMax * Noise.grandom(0.5f, 0.15f, ref seed) / (knorm * lambdaMax / (2.0f * System.Math.PI)));
                }
                else if (spectrumType == 2)
                {
                    float step = (max - min) / (nbWaves - 1); // dlambda/di
                    float omega0 = 9.81f / U0; // 100.0;
                    if ((i % (nbAngles)) == 0)
                    { // scramble angle ordre
                        for (int k = 0; k < nbAngles; k++)
                        {   // do N swap in indices
                            int n1 = (int)(Noise.lrandom(ref seed) % nbAngles), n2 = (int)Noise.lrandom(ref seed) % nbAngles, n;
                            n = index[n1]; index[n1] = index[n2]; index[n2] = n;
                        }
                    }
                    ktheta = (float)(waveDispersion * (angle(index[(i) % nbAngles]) + .4 * srnd(seed) * dangle()));
                    ktheta *= (float)(1 / (1 + 40 * System.Math.Pow(omega0 / omega, 4)));
                    amplitude = (float)((8.1e-3 * 9.81 * 9.81) / System.Math.Pow(omega, 5) * System.Math.Exp(-0.74 * System.Math.Pow(omega0 / omega, 4)));
                    amplitude = (float)(.5 * System.Math.Sqrt(2 * 3.14 * 9.81 / lambda) * nbAngles * step); // (2/step-step/2);
                    amplitude = (float)(3 * heightMax * System.Math.Sqrt(amplitude));
                }

                // cull breaking trochoids ( d(x+Acos(kx))=1-Akcos(); must be >0 )
                if (amplitude > 1.0f / knorm)
                {
                    amplitude = 1.0f / knorm;
                }
                else if (amplitude < -1.0f / knorm)
                {
                    amplitude = -1.0f / knorm;
                }

                waves[i].X = amplitude;
                waves[i].Y = omega;
                waves[i].Z = (float)(knorm * System.Math.Cos(ktheta));
                waves[i].W = (float)(knorm * System.Math.Sin(ktheta));
                sigmaXsq += (float)(System.Math.Pow(System.Math.Cos(ktheta), 2.0f) * (1.0f - System.Math.Sqrt(1.0f - knorm * knorm * amplitude * amplitude)));
                sigmaYsq += (float)(System.Math.Pow(System.Math.Sin(ktheta), 2.0f) * (1.0f - System.Math.Sqrt(1.0f - knorm * knorm * amplitude * amplitude)));
                meanHeight -= knorm * amplitude * amplitude * 0.5f;
                heightVariance += amplitude * amplitude * (2.0f - knorm * knorm * amplitude * amplitude) * 0.25f;
                amplitudeMax += System.Math.Abs(amplitude);
            }

            float var = 4.0f;
            float h0 = (float)(meanHeight - var * System.Math.Sqrt(heightVariance));
            float h1 = (float)(meanHeight + var * System.Math.Sqrt(heightVariance));
            amplitudeMax = h1 - h0;

            byte[] data = new byte[4 * sizeof(float) * waves.Length];
            int pos = 0;
            foreach (var v in waves)
            {
                Array.Copy(BitConverter.GetBytes(v.X), 0, data, pos, sizeof(float)); pos += sizeof(float);
                Array.Copy(BitConverter.GetBytes(v.Y), 0, data, pos, sizeof(float)); pos += sizeof(float);
                Array.Copy(BitConverter.GetBytes(v.Z), 0, data, pos, sizeof(float)); pos += sizeof(float);
                Array.Copy(BitConverter.GetBytes(v.W), 0, data, pos, sizeof(float)); pos += sizeof(float);
            }
            Texture1D wavesTexture = new Texture1D(nbWaves, TextureInternalFormat.RGBA32F, TextureFormat.RGBA,
                    PixelType.FLOAT, new Texture.Parameters().wrapS(TextureWrap.CLAMP_TO_BORDER).min(TextureFilter.NEAREST).mag(TextureFilter.NEAREST),
                    new Sxta.Render.Buffer.Parameters(), new CPUBuffer<byte>(data));

            //delete[] waves;

            nbWavesU.set(nbWaves);
            wavesU.set(wavesTexture);

            if (brdfShader != null)
            {
                Debug.Assert(brdfShader.getUsers().Count != 0);
                Program prog = (brdfShader.getUsers().First());
                prog.getUniform1f("seaRoughness").set(sigmaXsq);
                prog.getUniform3f("seaColor").set(seaColor);
            }
        }

        private class Impl : Sxta.Render.Scenegraph.Task
        {

            public SceneNode n;

            public DrawOceanTask o;

            public Impl(SceneNode n, DrawOceanTask owner) : base("DrawOcean", true, 0)
            {
                this.n = n;
                o = owner;
            }

            public override bool run()
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("Run DrawOcean Task");
                }
                FrameBuffer fb = SceneManager.getCurrentFrameBuffer();
                Program prog = SceneManager.getCurrentProgram();

                if (o.nbWavesU == null)
                {
                    o.nbWavesU = prog.getUniform1f("nbWaves");
                    o.wavesU = prog.getUniformSampler("wavesSampler");
                    o.cameraToOceanU = prog.getUniformMatrix4f("cameraToOcean");
                    o.screenToCameraU = prog.getUniformMatrix4f("screenToCamera");
                    o.cameraToScreenU = prog.getUniformMatrix4f("cameraToScreen");
                    o.oceanToCameraU = prog.getUniformMatrix3f("oceanToCamera");
                    o.oceanToWorldU = prog.getUniformMatrix4f("oceanToWorld");
                    o.oceanCameraPosU = prog.getUniform3f("oceanCameraPos");
                    o.oceanSunDirU = prog.getUniform3f("oceanSunDir");
                    o.horizon1U = prog.getUniform3f("horizon1");
                    o.horizon2U = prog.getUniform3f("horizon2");
                    o.timeU = prog.getUniform1f("time");
                    o.radiusU = prog.getUniform1f("radius");
                    o.heightOffsetU = prog.getUniform1f("heightOffset");
                    o.lodsU = prog.getUniform4f("lods");

                    Debug.Assert(o.nbWavesU != null);
                    o.generateWaves();
                }

                //List<TileSampler> uniforms;
                //TOSEE SceneNode.FieldIterator ui = n.getFields();
                foreach (KeyValuePair<string, object> ui in n.getFields())
                {
                    TileSampler u = (TileSampler)ui.Value;
                    if (u != null && u.getTerrain(0) != null)
                    {
                        u.setTileMap();
                    }
                }

                // compute ltoo = localToOcean transform, where ocean frame = tangent space at
                // camera projection on sphere o.radius in local space
                Matrix4d ctol = n.getLocalToCamera();
                ctol.Invert();
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
                    uz = cl;
                    uz.Normalize(); // unit z vector of ocean frame, in local space
                    if (o.oldLtoo != Matrix4d.Identity)
                    {
                        Vector3d tmp1 = new Vector3d(o.oldLtoo.R1C0, o.oldLtoo.R1C1, o.oldLtoo.R1C2);
                        Vector3d cross = Vector3d.Cross(tmp1, uz);
                        cross.Normalize();
                        ux = cross;
                    }
                    else
                    {
                        Vector3d tmp2 = Vector3d.UnitZ;
                        Vector3d cross = Vector3d.Cross(tmp2, uz);
                        cross.Normalize();
                        ux = cross;
                    }
                    uy = Vector3d.Cross(uz, ux); // unit y vector
                    oo = uz * o.radius; // origin of ocean frame, in local space
                }
                else
                {
                    // cylindrical ocean
                    uz = new Vector3d(0.0, -cl.Y, -cl.Z);
                    uz.Normalize();
                    ux = Vector3d.UnitX;
                    uy = Vector3d.Cross(uz, ux);
                    oo = new Vector3d(cl.X, 0.0, 0.0) + uz * o.radius;
                }

                Matrix4d ltoo = new Matrix4d(
                    ux.X, ux.Y, ux.Z, -Vector3d.Dot(ux, oo),
                    uy.X, uy.Y, uy.Z, -Vector3d.Dot(uy, oo),
                    uz.X, uz.Y, uz.Z, -Vector3d.Dot(uz, oo),
                    0.0, 0.0, 0.0, 1.0);
                // compute ctoo = cameraToOcean transform
                Matrix4d ctoo = ltoo * ctol;

                if (o.oldLtoo != Matrix4d.Identity)
                {
                    o.oldLtoo.Invert();
                    Vector3d delta = ltoo * (o.oldLtoo * Vector3d.Zero);
                    o.offset += delta;
                }
                o.oldLtoo = ltoo;

                Matrix4d ctos = n.getOwner().getCameraToScreen();
                ctos.Invert();
                Matrix4d stoc = ctos;
                Vector3d oc = ctoo * Vector3d.Zero;

                if (o.oceanSunDirU != null)
                {
                    // TODO how to get sun dir in a better way?
                    HashSet<SceneNode>.Enumerator i = n.getOwner().getNodes("light").GetEnumerator();
                    if (i.MoveNext() == true)
                    {
                        SceneNode l = i.Current;//.next();
                        Vector3d worldSunDir = l.getLocalToParent() * Vector3d.Zero;

                        Matrix3f tmpltoo = new Matrix3f((float)ltoo.R0C0, (float)ltoo.R0C1, (float)ltoo.R0C2,
                            (float)ltoo.R1C0, (float)ltoo.R1C1, (float)ltoo.R1C2,
                            (float)ltoo.R2C0, (float)ltoo.R2C1, (float)ltoo.R2C2);

                        Matrix4d getWorldToLocal = n.getWorldToLocal();
                        Matrix3f tmpgetWorldToLocal = new Matrix3f((float)getWorldToLocal.R0C0, (float)getWorldToLocal.R0C1, (float)getWorldToLocal.R0C2,
                            (float)getWorldToLocal.R1C0, (float)getWorldToLocal.R1C1, (float)getWorldToLocal.R1C2,
                            (float)getWorldToLocal.R2C0, (float)getWorldToLocal.R2C1, (float)getWorldToLocal.R2C2);
                        //TOSEE con Agus
                        Vector3d oceanSunDir = (Vector3d)(tmpltoo * (tmpgetWorldToLocal * (Vector3f)(worldSunDir)));

                        o.oceanSunDirU.set((Vector3f)oceanSunDir);
                    }
                }

                Vector4i screen = fb.getViewport();

                Vector4d[] frustum = new Vector4d[6];
                SceneManager.getFrustumPlanes(ctos, frustum);
                Vector3d left = frustum[0].Xyz;
                left.Normalize();
                Vector3d right = frustum[1].Xyz;
                right.Normalize();
                float fov = (float)System.Math.Acos(-Vector3d.Dot(left, right));
                float pixelSize = (float)(System.Math.Atan(System.Math.Tan(fov / 2.0f) / (screen.W / 2.0f))); // angle under which a screen pixel is viewed from the camera

                o.cameraToOceanU.setMatrix((Matrix4f)ctoo);
                o.screenToCameraU.setMatrix((Matrix4f)stoc);
                o.cameraToScreenU.setMatrix((Matrix4f)ctos);
                ctoo.Invert();
                Matrix3f tmpctoo = new Matrix3f((float)ctoo.R0C0, (float)ctoo.R0C1, (float)ctoo.R0C2,
                    (float)ctoo.R1C0, (float)ctoo.R1C1, (float)ctoo.R1C2,
                    (float)ctoo.R2C0, (float)ctoo.R2C1, (float)ctoo.R2C2);
                o.oceanToCameraU.setMatrix(tmpctoo/**ctoo.Invert().mat3x3().cast<float>()**/);
                o.oceanCameraPosU.set(new Vector3f((float)(-o.offset.X), (float)(-o.offset.Y), (float)(oc.Z)));
                if (o.oceanToWorldU != null)
                {
                    ltoo.Invert();
                    o.oceanToWorldU.setMatrix((Matrix4f)(n.getLocalToWorld() * ltoo));
                }

                if (o.horizon1U != null)
                {
                    float h = (float)oc.Z;
                    Vector3d A0 = (ctoo * new Vector4d((stoc * new Vector4d(0.0, 0.0, 0.0, 1.0)).Xyz, 0.0)).Xyz;
                    Vector3d dA = (ctoo * new Vector4d((stoc * new Vector4d(1.0, 0.0, 0.0, 0.0)).Xyz, 0.0)).Xyz;
                    Vector3d B = (ctoo * new Vector4d((stoc * new Vector4d(0.0, 1.0, 0.0, 0.0)).Xyz, 0.0)).Xyz;
                    if (o.radius == 0.0)
                    {
                        o.horizon1U.set(new Vector3f((float)(-(h * 1e-6 + A0.Z) / B.Z), (float)(-dA.Z / B.Z), 0));
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
                        o.horizon1U.set(new Vector3f((float)(-beta0), (float)(-beta1), 0));
                        o.horizon2U.set(new Vector3f((float)(beta0 * beta0 - gamma0), (float)(2.0 * (beta0 * beta1 - gamma1)), (float)(beta1 * beta1 - gamma2)));
                    }
                }

                o.timeU.set((float)(n.getOwner().getTime() * 1e-6));
                if (o.radiusU != null)
                {
                    o.radiusU.set(o.radius < 0.0 ? -o.radius : o.radius);
                }
                o.heightOffsetU.set(-o.meanHeight);
                o.lodsU.set(new Vector4f(o.resolution,
                        pixelSize * o.resolution,
                        (float)(System.Math.Log(o.lambdaMin) / System.Math.Log(2.0f)),
                        (float)((o.nbWavesU.get() - 1.0f) / (System.Math.Log(o.lambdaMax) / System.Math.Log(2.0f) - System.Math.Log(o.lambdaMin) / System.Math.Log(2.0f)))));

                if (o.screenGrid == null || o.screenWidth != screen.Z || o.screenHeight != screen.W)
                {
                    o.screenWidth = screen.Z;
                    o.screenHeight = screen.W;
                    //Same as Tutorial04_9.cs
                    o.screenGrid = new Mesh<Vector2f, uint>(Vector2f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLES, MeshUsage.GPU_STATIC);
                    o.screenGrid.addAttributeType(0, 2, AttributeType.A32F, false);

                    float f = 1.25f;
                    int NX = (int)(f * screen.Z / o.resolution);
                    int NY = (int)(f * screen.W / o.resolution);
                    for (int i = 0; i < NY; ++i)
                    {
                        for (int j = 0; j < NX; ++j)
                        {
                            o.screenGrid.addVertex(new Vector2f((float)(2.0 * f * j / (NX - 1.0f) - f), (float)(2.0 * f * i / (NY - 1.0f) - f)));
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

                fb.draw(prog, (o.screenGrid));

                return true;
            }
        }
    }
}
