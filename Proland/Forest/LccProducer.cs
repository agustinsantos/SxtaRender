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

using proland;
using Sxta.Core;
using Sxta.Math;
using Sxta.Render;
using Sxta.Render.Resources;
using Sxta.Render.Scenegraph;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Sxta.Proland.Forest
{
    public class LccProducer : TileProducer,  ISwappable<LccProducer>
    {

        public LccProducer(TileProducer delegateCB, Plants plants, Texture2D lccTexture,
                           Program copy, Program dots, int maxLevel, bool deform) :
        base("LccProducer", "CreateLcc", delegateCB.getCache(), true)
        {
            init(delegateCB, plants, lccTexture, copy, dots, maxLevel, deform);
        }

        public override void setRootQuadSize(float size)
        {
            base.setRootQuadSize(size);
            delegateCB.setRootQuadSize(size);
        }


        public override int getBorder()
        {
            return delegateCB.getBorder();
        }


        public override bool hasTile(int level, int tx, int ty)
        {
            if (maxLevel > 0)
            {
                return delegateCB.hasTile(level, tx, ty) || level <= System.Math.Min(plants.getMaxLevel(), maxLevel);
            }
            else
            {
                return delegateCB.hasTile(level, tx, ty) || level <= plants.getMaxLevel();
            }
        }


        public override TileCache.Tile findTile(int level, int tx, int ty, bool includeCache = false, bool done = false)
        {
            if (delegateCB.hasTile(level, tx, ty))
            {
                return delegateCB.findTile(level, tx, ty, includeCache, done);
            }
            return base.findTile(level, tx, ty, includeCache, done);
        }


        public override TileCache.Tile getTile(int level, int tx, int ty, uint deadline)
        {
            if (delegateCB.hasTile(level, tx, ty))
            {
                return delegateCB.getTile(level, tx, ty, deadline);
            }
            return base.getTile(level, tx, ty, deadline);
        }


        //public Vector4f getGpuTileCoords(int level, int tx, int ty, TileCache.Tile** tile);

        public override bool prefetchTile(int level, int tx, int ty)
        {
            if (delegateCB.hasTile(level, tx, ty))
            {
                return delegateCB.prefetchTile(level, tx, ty);
            }
            return base.prefetchTile(level, tx, ty);
        }


        public override void putTile(TileCache.Tile t)
        {
            if (delegateCB.hasTile(t.level, t.tx, t.ty))
            {
                delegateCB.putTile(t);
            }
            base.putTile(t);
        }


        public override void invalidateTiles()
        {
            delegateCB.invalidateTiles();
            base.invalidateTiles();
        }


        public override void invalidateTile(int level, int tx, int ty)
        {
            if (delegateCB.hasTile(level, tx, ty))
            {
                delegateCB.invalidateTile(level, tx, ty);
                return;
            }
            base.invalidateTile(level, tx, ty);
        }


        public override void update(SceneManager scene)
        {
            if (densityU != null)
            {
                float td = densityU.get();
                if (lastTreeDensity != 0.0 && lastTreeDensity != td)
                {
                    invalidateTiles();
                }
                lastTreeDensity = td;
            }

            Vector4d[] frustum = new Vector4d[6];
            SceneManager.getFrustumPlanes(scene.getCameraToScreen(), frustum);
            Vector3d left = Vector3d.Normalize(frustum[0].Xyz);
            Vector3d right = Vector3d.Normalize(frustum[1].Xyz );
            float fov =  MathHelper.Safe_acos(Vector3d.Dot(-left, right));
            if (lastFov != 0.0 && lastFov != fov)
            {
                plants.setMaxDistance((float)(plants.getMaxDistance() * System.Math.Tan(lastFov / 2.0) / System.Math.Tan(fov / 2.0)));
                invalidateTiles();
            }
            lastFov = fov;
        }

        public override void getReferencedProducers(List<TileProducer> producers)
        {
            producers.Add(delegateCB);
        }



        protected FrameBuffer frameBuffer;

        protected Texture2D lccTexture;

        protected Program copy;

        protected Program dots;

        protected LccProducer() : base("LccProducer", "CreateLcc")
        {
        }

        public void init(TileProducer delegateCB, Plants plants, Texture2D lccTexture,
                Program copy, Program dots, int maxLevel, bool deform)
        {
            base.init(delegateCB.getCache(), true);
            this.delegateCB = delegateCB;
            this.plants = plants;
            this.lccTexture = lccTexture;
            this.copy = copy;
            this.dots = dots;
            this.maxLevel = maxLevel;
            this.deform = deform;
            this.lastTreeDensity = 0.0f;
            this.lastFov = 0.0f;
            this.densityU = plants.renderProg.getUniform1f("treeDensity");
            this.sourceSamplerU = copy.getUniformSampler("sourceSampler");
            this.sourceOSLU = copy.getUniform4f("sourceOSL");
            this.tileOffsetU = dots.getUniform4f("tileOffset");
            this.tileDeformU = dots.getUniformMatrix2f("tileDeform");
            this.tileClipU = dots.getUniform4f("tileClip");
            this.densitySamplerU = dots.getUniformSampler("densitySampler");
            this.densityOSLU = dots.getUniform4f("densityOSL");
            this.frameBuffer = lccFramebufferFactory.Get(lccTexture);
        }

        protected internal override ulong getContext()
        {
            return (ulong)lccTexture.GetHashCode();
        }


        protected internal override Task startCreateTile(int level, int tx, int ty, uint deadline, Task task, TaskGraph owner)
        {
            TaskGraph result = owner == null ? createTaskGraph(task) : owner;

            int l = level;
            int x = tx;
            int y = ty;
            while (!delegateCB.hasTile(l, x, y))
            {
                x /= 2;
                y /= 2;
                l -= 1;
            }
            TileCache.Tile t = delegateCB.getTile(l, x, y, deadline);
            result.addTask(t.task);
            result.addDependency(task, t.task);

            base.startCreateTile(level, tx, ty, deadline, task, result);

            return result;
        }

        protected internal override void beginCreateTile()
        {
            old = SceneManager.getCurrentFrameBuffer();
            SceneManager.setCurrentFrameBuffer(frameBuffer);
            base.beginCreateTile();
        }


        protected internal override bool doCreateTile(int level, int tx, int ty, TileStorage.Slot data)
        {
            double rootQuadSize = getRootQuadSize();
            int tileBorder = getBorder();
            int tileWidth = data.getOwner().getTileSize();
            int tileSize = tileWidth - 2 * tileBorder;
            if (deformation == null)
            {
                if (deform)
                {
                    deformation = new SphericalDeformation((float)(rootQuadSize / 2.0));
                }
                else
                {
                    deformation = new Deformation();
                }
            }

            int m = 1 << plants.getMaxLevel();
            int n = 1 << (plants.getMaxLevel() - level);
            float r = plants.getPoissonRadius();

            TileCache.Tile t = null;
            Vector4f coords = delegateCB.getGpuTileCoords(level, tx, ty, ref t);
            Debug.Assert(t != null);
            GPUTileStorage.GPUSlot parentGpuData = t.getData() as GPUTileStorage.GPUSlot;
            Debug.Assert(parentGpuData != null);
            float b = (float)(tileBorder) / (1 << (level - t.level));
            float s = tileWidth;
            float S = s / (s - 2 * tileBorder);

            densitySamplerU.set(parentGpuData.t);

            frameBuffer.clear(true, false, false);

            sourceSamplerU.set(parentGpuData.t);
            sourceOSLU.set(new Vector4f(coords.X - b / parentGpuData.getWidth(), coords.Y - b / parentGpuData.getHeight(), coords.W * S, coords.Z));
            frameBuffer.drawQuad(copy);

            for (int y = 0; y < n; ++y)
            {
                int iy = ty * n + y;
                for (int x = 0; x < n; ++x)
                {
                    int ix = tx * n + x;
                    double ox = rootQuadSize * ((float)(ix) / m - 0.5f);
                    double oy = rootQuadSize * ((float)(iy) / m - 0.5f);
                    double l = rootQuadSize / m;

                    float x0 = (float)(x) / n;
                    float y0 = (float)(y) / n;
                    float ql = 1.0f / n;
                    float bx0 = (float)(2.0 * (tileSize * x0 + tileBorder) / tileWidth - 1.0);
                    float by0 = (float)(2.0 * (tileSize * y0 + tileBorder) / tileWidth - 1.0);
                    float bql = (float)((2.0 * tileSize) / tileWidth * ql);
                    tileOffsetU.set(new Vector4f(bx0, by0, bql, r));

                    tileClipU.set(new Vector4f(x == 0 ? -1 : 0, x == n - 1 ? 2 : 1, y == 0 ? -1 : 0, y == n - 1 ? 2 : 1));

                    if (tileDeformU != null)
                    {
                        Matrix4d l2d = deformation.localToDeformedDifferential(new Vector3d(ox + l / 2.0f, oy + l / 2.0f, 0.0f));
                        Matrix4d d2t = deformation.deformedToTangentFrame(l2d * Vector3d.Zero);
                        Matrix4d t2l = Matrix4d.Invert(l2d) * Matrix4d.Invert(d2t);
                        tileDeformU.set(new Matrix2f((float)t2l.R0C0, (float)t2l.R0C1, (float)t2l.R1C0, (float)t2l.R1C1));
                    }

                    densityOSLU.set(new Vector4f(coords.X + x0 * coords.W, coords.Y + y0 * coords.W, ql * coords.W, coords.Z));

                    int patternId = (int)(881 * System.Math.Abs(System.Math.Cos(ox * oy))) % plants.getPatternCount(); // TODO improve this
                    MeshBuffers pattern = plants.getPattern(patternId);
                    int nSeeds = (int)(pattern.nvertices);

                    frameBuffer.draw(dots, pattern, MeshMode.POINTS, 0, nSeeds);
                }
            }

            GPUTileStorage.GPUSlot gpuData = data as GPUTileStorage.GPUSlot;
            Debug.Assert(gpuData != null);
            ((GPUTileStorage)getCache().getStorage()).notifyChange(gpuData);
            gpuData.copyPixels(frameBuffer, 0, 0, tileWidth, tileWidth);

            return true;
        }

        protected internal override void endCreateTile()
        {
            base.endCreateTile();
            SceneManager.setCurrentFrameBuffer(old);
            old = null;
        }


        protected internal override void stopCreateTile(int level, int tx, int ty)
        {
            int l = level;
            int x = tx;
            int y = ty;
            while (!delegateCB.hasTile(l, x, y))
            {
                x /= 2;
                y /= 2;
                l -= 1;
            }
            TileCache.Tile t = delegateCB.findTile(l, x, y);
            Debug.Assert(t != null);
            delegateCB.putTile(t);

            base.stopCreateTile(level, tx, ty);
        }



        private TileProducer delegateCB;

        private Plants plants;

        private int maxLevel;

        private bool deform;

        private float lastTreeDensity;

        private float lastFov;

        private Uniform1f densityU;

        private UniformSampler sourceSamplerU;

        private Uniform4f sourceOSLU;

        private Uniform4f tileOffsetU;

        private UniformMatrix2f tileDeformU;

        private Uniform4f tileClipU;

        private UniformSampler densitySamplerU;

        private Uniform4f densityOSLU;

        private Deformation deformation;

        private static FrameBuffer old;

        private static FrameBuffer createLccFramebuffer(Texture2D lccTexture)
        {
            int tileWidth = lccTexture.getWidth();
            FrameBuffer frameBuffer = new FrameBuffer();
            frameBuffer.setReadBuffer(BufferId.COLOR0);
            frameBuffer.setDrawBuffer(BufferId.COLOR0);
            frameBuffer.setViewport(new Vector4i(0, 0, tileWidth, tileWidth));
            frameBuffer.setTextureBuffer(BufferId.COLOR0, lccTexture, 0);
            frameBuffer.setPolygonMode(PolygonMode.FILL, PolygonMode.FILL);
            frameBuffer.setBlend(true, BlendEquation.ADD, BlendArgument.ONE, BlendArgument.ONE, BlendEquation.ADD, BlendArgument.ONE, BlendArgument.ZERO);
            return frameBuffer;
        }

        private static Factory<Texture2D, FrameBuffer> lccFramebufferFactory = new Factory<Texture2D, FrameBuffer>(createLccFramebuffer);

        public void swap(LccProducer obj)
        {
            throw new NotImplementedException();
        }
    }
}
