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

namespace Sxta.Proland.Forest
{
    public class DrawPlantsShadowTask : AbstractTask, ISwappable<DrawPlantsShadowTask>
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public const int MAX_SHADOW_MAPS = 4;

        /**
         * Creates a new DrawPlantsTask.
         *
         * @param terrain used to determine which subNodes are pointing to the Terrain SceneNodes.
         * @param plants the Plants that contains the patterns & models used for our vegetation.
         */
        public DrawPlantsShadowTask(string terrain, Plants plants) :
                base("DrawPlantsShadowTask")
        {
            init(terrain, plants);
        }

        /**
         * Deletes a DrawPlantsTask.
         */
        //public virtual ~DrawPlantsShadowTask();

        /**
         * Initializes #terrainInfos and creates the actual task that will draw plants.
         */
        public override Task getTask(Object context)
        {
            SceneNode n = ((Method)context).getOwner();
            if (producers.Count == 0)
            {
                foreach (var entry in n.getFields())
                {
                    string name = entry.Key;
                    Object value = entry.Value;
                    if (name == terrain)
                    {
                        SceneNode tn = value as SceneNode;
                        PlantsProducer p = PlantsProducer.getPlantsProducer(tn, plants);
                        producers.Add(p);
                    }
                }
            }

            return new Impl(this, n);
        }



        /**
         * Plant models and amplification parameters.
         */
        public Plants plants; // plant models and amplification parameters

        /**
         * Creates a new DrawPlantsTask.
         */
        protected DrawPlantsShadowTask() : base("DrawPlantsTask")
        {
        }

        /**
         * Initializes the field of a DrawPlantsTask.
         *
         * See #DrawPlantsShadowTask().
         */
        public void init(string terrain, Plants plants)
        {
            this.terrain = terrain;
            this.plants = plants;
            this.initialized = false;
        }


        public void swap(DrawPlantsShadowTask t)
        {
            Std.Swap(ref terrain, ref t.terrain);
            Std.Swap(ref plants, ref t.plants);
            Std.Swap(ref producers, ref t.producers);
            initialized = false;
            t.initialized = false;
        }



        /**
         * Name of the terrain to be amplified.
         */
        string terrain; // name of the terrain to be amplified

        private List<TileProducer> producers;

        private bool initialized;

        private FrameBuffer frameBuffer;

        // Uniforms (renderPlantsShadowProg)
        private Uniform3f cameraPosU;
        private UniformMatrix4f localToTangentFrameU;
        private UniformMatrix4f tangentFrameToScreenU;
        private Uniform4f shadowLimitU;
        private Uniform4f shadowCutsU;
        private UniformMatrix4f[] tangentFrameToShadowU = new UniformMatrix4f[MAX_SHADOW_MAPS];
        private Uniform3f tangentSunDirU;
        private Uniform3f focalPosU;
        private Uniform1f plantRadiusU;

        private void drawPlantsShadow(SceneNode context)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("DrawPlantsShadow Task");
            }

            Vector4d worldSunDir = new Vector4d();
            SceneManager scene = ((SceneNode)context).getOwner();
            var rst = scene.getNodes("light");
            if (rst != null && rst.Count > 0)
            {
                worldSunDir = new Vector4d(rst.First().getWorldPos(), 0.0);
            }

            Box3f b;
            Program old = SceneManager.getCurrentProgram();
            SceneManager.setCurrentProgram(plants.selectProg);
            for (int i = 0; i < producers.Count; ++i)
            {
                PlantsProducer p = producers[i] as PlantsProducer;
                p.produceTiles();
                p.tangentSunDir = (Matrix4d.Invert(p.tangentFrameToWorld) * worldSunDir).Xyz;
            }
            SceneManager.setCurrentProgram(old);

            if (plants.shadowProg == null)
            {
                return;
            }

            if (!initialized)
            {
                initialized = true;
                Texture2DArray shadowTexture = null;
                UniformSampler shadowSampler = plants.renderProg.getUniformSampler("treeShadowMap");
                if (shadowSampler != null)
                {
                    shadowTexture = shadowSampler.get() as Texture2DArray;
                }
                if (shadowTexture == null)
                {
                    var fields = context.getFields().GetEnumerator();
                    while (fields.MoveNext() && shadowTexture == null)
                    {
                        string name = fields.Current.Key;
                        Object value = fields.Current.Value;
                        if (name == terrain)
                        {
                            SceneNode tn = value as SceneNode;
                            if (shadowTexture == null)
                            {
                                Module m = tn.getModule("material");
                                if (m != null)
                                {
                                    ISet<Program> progs = m.getUsers();
                                    var i = progs.GetEnumerator();
                                    while (i.MoveNext() && shadowTexture == null)
                                    {
                                        shadowSampler = i.Current.getUniformSampler("treeShadowMap");
                                        if (shadowSampler != null)
                                        {
                                            shadowTexture = shadowSampler.get() as Texture2DArray;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (shadowTexture != null)
                {
                    frameBuffer = shadowFramebufferFactory.Get(shadowTexture);
                    cameraPosU = plants.shadowProg.getUniform3f("cameraRefPos");
                    localToTangentFrameU = plants.shadowProg.getUniformMatrix4f("localToTangentFrame");
                    tangentFrameToScreenU = plants.shadowProg.getUniformMatrix4f("tangentFrameToScreen");
                    shadowLimitU = plants.shadowProg.getUniform4f("shadowLimit");
                    shadowCutsU = plants.shadowProg.getUniform4f("shadowCuts");
                    for (int i = 0; i < MAX_SHADOW_MAPS; ++i)
                    {
                        string name = "tangentFrameToShadow[" + i + "]";
                        tangentFrameToShadowU[i] = plants.shadowProg.getUniformMatrix4f(name);
                    }
                    tangentSunDirU = plants.shadowProg.getUniform3f("tangentSunDir");
                    focalPosU = plants.shadowProg.getUniform3f("focalPos");
                    plantRadiusU = plants.shadowProg.getUniform1f("plantRadius");
                }
            }

            int pid = 0;
            int maxSize = 0;
            for (int i = 0; i < producers.Count; ++i)
            {
                int size = (producers[i] as PlantsProducer).plantBounds.Count;
                if (size > maxSize)
                {
                    maxSize = size;
                    pid = i;
                }
            }

            if (maxSize > 0 && frameBuffer != null)
            {
                PlantsProducer p = producers[pid] as PlantsProducer;
                Matrix4d m = p.tangentFrameToWorld;

                double near = p.plantBounds[0].X;
                double far = p.plantBounds[0].Y;
                for (int i = 1; i < p.plantBounds.Count; ++i)
                {
                    near = System.Math.Min(near, p.plantBounds[i].X);
                    far = System.Math.Max(far, p.plantBounds[i].Y);
                }
                far = System.Math.Min(far, 0.8 * plants.getMaxDistance());
                if (far <= near)
                {
                    return;
                }

                int sliceCount = 0;
                if (frameBuffer.getTextureBuffer(BufferId.DEPTH) != null)
                {
                    sliceCount = (frameBuffer.getTextureBuffer(BufferId.DEPTH) as Texture2DArray).getLayers();
                }
                else
                {
                    sliceCount = (frameBuffer.getTextureBuffer(BufferId.COLOR0) as Texture2DArray).getLayers();
                }
                Debug.Assert(sliceCount <= MAX_SHADOW_MAPS);

                if (far - near < 100.0)
                {
                    sliceCount = 1;
                }

                double[] zLimits = new double[MAX_SHADOW_MAPS + 1];
                double[] zCuts = new double[MAX_SHADOW_MAPS + 1];
                bool[] zSlice = new bool[MAX_SHADOW_MAPS + 1];
                for (int slice = 0; slice <= MAX_SHADOW_MAPS; ++slice)
                {
                    zLimits[slice] = -1.0;
                    zCuts[slice] = -1.0;
                    zSlice[slice] = false;
                }
                for (int slice = 0; slice <= sliceCount; ++slice)
                {
                    double z = near * System.Math.Pow(far / near, (float)(slice) / sliceCount);
                    Vector4d p2 = scene.getCameraToScreen() * new Vector4d(0.0, 0.0, -z, 1.0);
                    zLimits[slice] = p2.Z;
                    zCuts[slice] = p2.W;
                }

                // temporary reference frame for shadow map
                Vector3d zDir = p.tangentSunDir;
                Vector3d xDir = Vector3d.Normalize(new Vector3d(-zDir.Y, zDir.X, 0.0));
                Vector3d yDir = Vector3d.Cross(zDir, xDir);
                Vector3d uDir = xDir;
                Vector3d vDir = yDir;
                Matrix3d s = new Matrix3d(uDir.X, uDir.Y, uDir.Z, vDir.X, vDir.Y, vDir.Z, zDir.X, zDir.Y, zDir.Z);

                Box3d b3 = p.plantBox;
                double smax = -double.MaxValue;
                for (int i = 0; i < 8; ++i)
                {
                    Vector3d c = new Vector3d(i % 2 == 0 ? b3.xmin : b3.xmax, (i / 2) % 2 == 0 ? b3.ymin : b3.ymax, (i / 4) % 2 == 0 ? b3.zmin : b3.zmax);
                    double sz = Vector3d.Dot(zDir, c);
                    smax = System.Math.Max(smax, sz);
                }

                Matrix4d t = Matrix4d.Invert(p.tangentFrameToScreen);
                int[] segments = new int[24] { 0, 1, 1, 3, 3, 2, 2, 0, 4, 5, 5, 7, 7, 6, 6, 4, 0, 4, 1, 5, 3, 7, 2, 6 };

                for (int slice = 0; slice < sliceCount; ++slice)
                {
                    double zi = zCuts[sliceCount - slice - 1];
                    double zj = zCuts[sliceCount - slice];
                    double fi = zLimits[sliceCount - slice - 1];
                    double fj = zLimits[sliceCount - slice];

                    double zmin = double.MaxValue;
                    double zmax = double.MinValue;
                    for (int i = 0; i < p.plantBounds.Count; ++i)
                    {
                        Vector4d h = p.plantBounds[i];
                        if (h.X <= zj && h.Y >= zi)
                        {
                            zmin = System.Math.Min(zmin, h.Z);
                            zmax = System.Math.Max(zmax, h.W);
                        }
                    }

                    List<Vector3d> pts = new List<Vector3d>();
                    Vector3d[] cf = new Vector3d[8];
                    bool[] inside = new bool[8];
                    for (int i = 0; i < 8; ++i)
                    {
                        cf[i] = (t * new Vector4d(i % 2 == 0 ? -1.0 : 1.0, (i / 2) % 2 == 0 ? -1.0 : 1.0, (i / 4) % 2 == 0 ? fi : fj, 1.0)).Xyz;
                        inside[i] = cf[i].Z >= zmin && cf[i].Z <= zmax;
                        if (inside[i])
                        {
                            pts.Add(cf[i]);
                        }
                    }
                    for (int i = 0; i < 24; i += 2)
                    {
                        int pointA = segments[i];
                        int pointB = segments[i + 1];
                        if (!inside[pointA] || !inside[pointB])
                        {
                            Vector3d pA = cf[pointA];
                            Vector3d pB = cf[pointB];
                            Vector3d AB = pB - pA;
                            double tOut = 1.0;
                            double tIn = 0.0;
                            tIn = System.Math.Max(tIn, ((AB.Z > 0.0 ? zmin : zmax) - pA.Z) / AB.Z);
                            tOut = System.Math.Min(tOut, ((AB.Z > 0.0 ? zmax : zmin) - pA.Z) / AB.Z);
                            if (tIn < tOut && tIn < 1.0 && tOut > 0.0)
                            {
                                if (tIn > 0.0)
                                {
                                    pts.Add(pA + AB * tIn);
                                }
                                if (tOut < 1.0)
                                {
                                    pts.Add(pA + AB * tOut);
                                }
                            }
                        }
                    }

                    if (zmin < zmax && pts.Count > 2)
                    {
                        Box3d tb = new Box3d();
                        for (int i = 0; i < pts.Count; ++i)
                        {
                            tb = tb.enlarge(s * pts[i]);
                        }
                        tb.zmax = System.Math.Max(tb.zmax, smax);
                        // tangent frame to shadow
                        Matrix4d ortho;
                        Matrix4d.CreateOrthographicOffCenter(tb.xmax, tb.xmin, tb.ymax, tb.ymin, -tb.zmax, -tb.zmin, out ortho);
                        Matrix4d ttos = ortho * new Matrix4d(s.R0C0, s.R0C1, s.R0C2, 0,
                                                            s.R1C0, s.R1C1, s.R1C2, 0,
                                                            s.R2C0, s.R2C1, s.R2C2, 0,
                                                            0, 0, 0, 0);
                        tangentFrameToShadowU[slice].setMatrix(new Matrix4f((float)ttos.R0C0, (float)ttos.R0C1, (float)ttos.R0C2, (float)ttos.R0C3,
                                                                      (float)ttos.R1C0, (float)ttos.R1C1, (float)ttos.R1C2, (float)ttos.R1C3,
                                                                      (float)ttos.R2C0, (float)ttos.R2C1, (float)ttos.R2C2, (float)ttos.R2C3,
                                                                      0.0f, 0.0f, 0.0f, (float)(1.0f / (tb.zmax - tb.zmin))));

                        zSlice[slice] = true;
                    }
                }

                shadowLimitU.set(new Vector4f((float)zLimits[1], (float)zLimits[2], (float)zLimits[3], (float)(zLimits[4])) * 0.5f + new Vector4f(0.5f, 0.5f, 0.5f, 0.5f));
                shadowCutsU.set(new Vector4f((float)zCuts[1], (float)zCuts[2], (float)zCuts[3], (float)zCuts[4]));

                frameBuffer.clear(true, false, true);
                for (int i = 0; i < producers.Count; ++i)
                {
                    PlantsProducer pp = producers[i] as PlantsProducer;
                    if (pp.count == 0)
                    {
                        continue;
                    }
                    if (cameraPosU != null)
                    {
                        cameraPosU.set(new Vector3f((float)(pp.localCameraPos.X - pp.cameraRefPos.X), (float)(pp.localCameraPos.Y - pp.cameraRefPos.Y), 0.0f));
                    }
                    localToTangentFrameU.setMatrix((Matrix4f)pp.localToTangentFrame);
                    tangentFrameToScreenU.setMatrix((Matrix4f)pp.tangentFrameToScreen);
                    tangentSunDirU.set((Vector3f)(-pp.tangentSunDir));
                    if (focalPosU != null)
                    {
                        Vector2d cgDir = (pp.cameraToTangentFrame * new Vector4d(0.0, 0.0, 1.0, 0.0)).Xy.Normalize(1000.0f);
                        focalPosU.set(new Vector3f((float)(cgDir.X), (float)(cgDir.Y), (float)(pp.tangentCameraPos.Z)));
                    }
                    if (plantRadiusU != null)
                    {
                        plantRadiusU.set((float)(plants.getPoissonRadius() * pp.terrain.root.l / (1 << plants.getMaxLevel())));
                    }
                    frameBuffer.multiDraw(plants.shadowProg, pp.getPlantsMesh(), MeshMode.POINTS, pp.offsets, pp.sizes, pp.count);
                }
            }
        }


        private class Impl : Task
        {

            public DrawPlantsShadowTask owner;

            public SceneNode context;

            public Impl(DrawPlantsShadowTask owner, SceneNode context) :
                base("DrawPlantsShadow", true, 0)
            {
                this.owner = owner;
                this.context = context;
            }



            public override bool run()
            {
                owner.drawPlantsShadow(context);
                return true;
            }

        }


        private static FrameBuffer createShadowFramebuffer(Texture2DArray texture)
        {
            int width = texture.getWidth();
            int height = texture.getHeight();
            FrameBuffer frameBuffer = new FrameBuffer();
            frameBuffer.setViewport(new Vector4i(0, 0, width, height));
            frameBuffer.setReadBuffer((BufferId)(0));
            if (texture.getFormat() == TextureFormat.DEPTH_COMPONENT)
            {
                frameBuffer.setDrawBuffer((BufferId)(0));
                frameBuffer.setTextureBuffer(BufferId.DEPTH, texture, 0, -1);
                frameBuffer.setDepthTest(true, Function.LEQUAL);
            }
            else
            {
                frameBuffer.setDrawBuffer(BufferId.COLOR0);
                frameBuffer.setTextureBuffer(BufferId.COLOR0, texture, 0, -1);
                frameBuffer.setBlend(true, BlendEquation.MIN, BlendArgument.ONE, BlendArgument.ONE);
                frameBuffer.setClearColor(new Vector4f(1.0f, 1.0f, 1.0f, 1.0f));
            }
            frameBuffer.setPolygonMode(PolygonMode.FILL, PolygonMode.FILL);
            return frameBuffer;
        }

        private static Factory<Texture2DArray, FrameBuffer> shadowFramebufferFactory = new Factory<Texture2DArray, FrameBuffer>(createShadowFramebuffer);

    }
}

