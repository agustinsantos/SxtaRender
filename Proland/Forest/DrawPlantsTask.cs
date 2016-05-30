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

namespace Sxta.Proland.Forest
{
    public class DrawPlantsTask : AbstractTask, ISwappable<DrawPlantsTask>
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /**
         * Creates a new DrawPlantsTask.
         *
         * @param terrain used to determine which subNodes are pointing to the Terrain SceneNodes.
         * @param plants the Plants that contains the patterns & models used for our vegetation.
         */
        public DrawPlantsTask(string terrain, Plants plants) : base("DrawPlantsTask")
        {
            init(terrain, plants);
        }

        /**
         * Deletes a DrawPlantsTask.
         */
        // public virtual ~DrawPlantsTask();

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
        protected Plants plants; // plant models and amplification parameters

        /**
         * Creates a new DrawPlantsTask.
         */
        protected DrawPlantsTask() : base("DrawPlantsTask")
        {
        }

        /**
         * Initializes the field of a DrawPlantsTask.
         *
         * See #DrawPlantsTask().
         */
        protected void init(string terrain, Plants plants)
        {
            this.terrain = terrain;
            this.plants = plants;
            this.cameraPosU = plants.renderProg.getUniform3f("cameraRefPos");
            this.clipPlane0U = plants.renderProg.getUniform4f("clip[0]");
            this.clipPlane1U = plants.renderProg.getUniform4f("clip[1]");
            this.clipPlane2U = plants.renderProg.getUniform4f("clip[2]");
            this.clipPlane3U = plants.renderProg.getUniform4f("clip[3]");
            this.localToTangentFrameU = plants.renderProg.getUniformMatrix4f("localToTangentFrame");
            this.tangentFrameToScreenU = plants.renderProg.getUniformMatrix4f("tangentFrameToScreen");
            this.tangentFrameToWorldU = plants.renderProg.getUniformMatrix4f("tangentFrameToWorld");
            this.tangentSpaceToWorldU = plants.renderProg.getUniformMatrix3f("tangentSpaceToWorld");
            this.tangentSunDirU = plants.renderProg.getUniform3f("tangentSunDir");
            this.focalPosU = plants.renderProg.getUniform3f("focalPos");
            this.plantRadiusU = plants.renderProg.getUniform1f("plantRadius");
        }


        public void swap(DrawPlantsTask t)
        {
            Std.Swap(ref terrain, ref t.terrain);
            Std.Swap(ref plants, ref t.plants);
            Std.Swap(ref producers, ref t.producers);
            Std.Swap(ref cameraPosU, ref t.cameraPosU);
            Std.Swap(ref clipPlane0U, ref t.clipPlane0U);
            Std.Swap(ref clipPlane1U, ref t.clipPlane1U);
            Std.Swap(ref clipPlane2U, ref t.clipPlane2U);
            Std.Swap(ref clipPlane3U, ref t.clipPlane3U);
            Std.Swap(ref localToTangentFrameU, ref t.localToTangentFrameU);
            Std.Swap(ref tangentFrameToScreenU, ref t.tangentFrameToScreenU);
            Std.Swap(ref tangentFrameToWorldU, ref t.tangentFrameToWorldU);
            Std.Swap(ref tangentSpaceToWorldU, ref t.tangentSpaceToWorldU);
            Std.Swap(ref tangentSunDirU, ref t.tangentSunDirU);
            Std.Swap(ref focalPosU, ref t.focalPosU);
            Std.Swap(ref plantRadiusU, ref t.plantRadiusU);
        }

        /**
         * Name of the terrain to be amplified.
         */
        private string terrain; // name of the terrain to be amplified

        private List<TileProducer> producers = new List<TileProducer>();

        // Uniforms (renderPlantProg)
        private Uniform3f cameraPosU;
        private Uniform4f clipPlane0U;
        private Uniform4f clipPlane1U;
        private Uniform4f clipPlane2U;
        private Uniform4f clipPlane3U;
        private UniformMatrix4f localToTangentFrameU;
        private UniformMatrix4f tangentFrameToScreenU;
        private UniformMatrix4f tangentFrameToWorldU;
        private UniformMatrix3f tangentSpaceToWorldU;
        private Uniform3f tangentSunDirU;
        private Uniform3f focalPosU;
        private Uniform1f plantRadiusU;

        private Query q;

        private void drawPlants(SceneNode context)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("DrawPlants Task");
            }

            FrameBuffer fb = SceneManager.getCurrentFrameBuffer();

            //fb.setMultisample(true);
            //fb.setSampleAlpha(true, true);

            int totalTiles = 0;
            int totalTrees = 0;
            int realTrees = (int)(q == null ? 0 : q.getResult() / 2);
            if (q == null)
            {
                q = new Query(QueryType.PRIMITIVES_GENERATED);
            }
            q.begin();
            for (int i = 0; i < producers.Count; ++i)
            {
                PlantsProducer p = producers[i] as PlantsProducer;
                if (p.count == 0)
                {
                    continue;
                }
                if (cameraPosU != null)
                {
                    double d = (1.0 - p.node.getOwner().getCameraToScreen().R2C2) / 2.0;
                    cameraPosU.set(new Vector3f((float)(p.localCameraPos.X - p.cameraRefPos.X), (float)(p.localCameraPos.Y - p.cameraRefPos.Y), (float)d));
                }
                if (clipPlane0U != null)
                {
                    Vector4d[] clipPlanes = new Vector4d[6];
                    SceneManager.getFrustumPlanes(p.tangentFrameToScreen, clipPlanes);
                    clipPlane0U.set((Vector4f)(clipPlanes[0] / clipPlanes[0].Xyz.Length));
                    clipPlane1U.set((Vector4f)(clipPlanes[1] / clipPlanes[1].Xyz.Length));
                    clipPlane2U.set((Vector4f)(clipPlanes[2] / clipPlanes[2].Xyz.Length));
                    clipPlane3U.set((Vector4f)(clipPlanes[3] / clipPlanes[3].Xyz.Length));
                }
                localToTangentFrameU.setMatrix((Matrix4f)p.localToTangentFrame);
                tangentFrameToScreenU.setMatrix((Matrix4f)p.tangentFrameToScreen);
                if (tangentFrameToWorldU != null)
                {
                    tangentFrameToWorldU.setMatrix((Matrix4f)p.tangentFrameToWorld);
                }
                if (tangentSpaceToWorldU != null)
                {
                    tangentSpaceToWorldU.setMatrix((Matrix3f)p.tangentSpaceToWorld);
                }
                if (tangentSunDirU != null)
                {
                    tangentSunDirU.set((Vector3f)p.tangentSunDir);
                }
                if (focalPosU != null)
                {
                    Vector2d cgDir = (p.cameraToTangentFrame * new Vector4d(0.0, 0.0, 1.0, 0.0)).Xy.Normalize(1000.0);
                    focalPosU.set(new Vector3f((float)cgDir.X, (float)cgDir.Y, (float)p.tangentCameraPos.Z));
                }
                if (plantRadiusU != null)
                {
                    plantRadiusU.set((float)(plants.getPoissonRadius() * p.terrain.root.l / (1 << plants.getMaxLevel())));
                }

                totalTiles += p.count;
                totalTrees += p.total;
                fb.multiDraw(plants.renderProg, p.getPlantsMesh(), MeshMode.POINTS, p.offsets, p.sizes, p.count);
            }
            q.end();

            //fb.setMultisample(false);

            //char buf[256];
            //sprintf(buf, "%d trees (%d seeds, %d tiles)", realTrees, totalTrees, totalTiles);
            //ork::ShowInfoTask::setInfo(string(), string(buf));
        }


        private class Impl : Task
        {

            public DrawPlantsTask owner;

            public SceneNode context;

            public Impl(DrawPlantsTask owner, SceneNode context) : base("DrawPlants", true, 0)
            {
                this.owner = owner;
                this.context = context;
            }
            //public virtual ~Impl();

            public override bool run()
            {
                owner.drawPlants(context);
                return true;
            }
        }
    }
}
