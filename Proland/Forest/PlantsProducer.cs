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
using Sxta.Math;
using Sxta.Render;
using Sxta.Render.Scenegraph;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sxta.Proland.Forest
{

    public class PlantsProducer : TileProducer
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /**
         * TODO.
         */
        public SceneNode node;

        /**
         * TODO.
         */
        public TerrainNode terrain;

        /**
         * TODO.
         */
        public Vector3d localCameraPos;

        /**
         * TODO.
         */
        public Vector3d tangentCameraPos;

        /**
         * TODO.
         */
        public Matrix4d localToTangentFrame;

        /**
         * TODO.
         */
        public Matrix4d localToScreen;

        /**
         * TODO.
         */
        public Matrix4d screenToLocal;

        /**
         * TODO.
         */
        public Vector3d[] frustumV = new Vector3d[8];

        /**
         * TODO.
         */
        public Vector4d[] frustumP = new Vector4d[6];

        /**
         * TODO.
         */
        public Vector4d frustumZ;

        /**
         * TODO.
         */
        public double zNear;

        /**
         * TODO.
         */
        public double zRange;

        /**
         * TODO.
         */
        public Matrix4d tangentFrameToScreen;

        /**
         * TODO.
         */
        public Matrix4d tangentFrameToWorld;

        /**
         * TODO.
         */
        public Matrix3d tangentSpaceToWorld;

        /**
         * TODO.
         */
        public Matrix4d cameraToTangentFrame;

        /**
         * TODO.
         */
        public Vector3d cameraRefPos;

        /**
         * TODO.
         */
        public Vector3d tangentSunDir;

        /**
         * TODO.
         */
        public int[] offsets;

        /**
         * TODO.
         */
        public int[] sizes;

        /**
         * TODO.
         */
        public int count;

        /**
         * TODO.
         */
        public int total;

        /**
         * TODO.
         */
        public List<Vector4d> plantBounds = new List<Vector4d>(); // frustum z min max, altitude min max

        /**
         * TODO.
         */
        public Box3d plantBox; // bounding box in local space

        /**
         * TODO.
         */
        public List<PlantsProducer> slaves = new List<PlantsProducer>();

        /**
         * TODO.
         */
        public PlantsProducer master;
        private const int VERTEX_SIZE = 24;

        private static TileCache createPlantsCache(Plants plants)
        {
            GPUBufferTileStorage storage = new GPUBufferTileStorage(VERTEX_SIZE * plants.getMaxDensity(), plants.getTileCacheSize());
            return new TileCache(storage, "PlantsCache");
        }

        private static Sxta.Core.Factory<Plants, TileCache> plantsCacheFactory = new Sxta.Core.Factory<Plants, TileCache>(createPlantsCache);

        private static Dictionary<Tuple<SceneNode, Plants>, PlantsProducer> producers;

        /**
         * TODO.
         */
        public PlantsProducer(SceneNode node, TerrainNode terrain, Plants plants,
            TileSampler lcc, TileSampler z, TileSampler n, TileSampler occ, TileCache cache) :
        base("PlantsProducer", "CreatePlants", cache, false)
        {
            this.node = node; this.terrain = terrain; this.master = null; this.plants = plants; this.lcc = lcc; this.z = z; this.n = n; this.occ = occ;
            offsets = new int[cache.getStorage().getCapacity()];
            sizes = new int[cache.getStorage().getCapacity()];
            tileOffsetU = plants.selectProg.getUniform3f("tileOffset");
            tileDeformU = plants.selectProg.getUniformMatrix2f("tileDeform");
            usedTiles = null;
            for (int i = plants.getMinLevel(); i < plants.getMaxLevel(); ++i)
            {
                slaves.Add(new PlantsProducer(this));
            }
            count = 0;
            total = 0;
            cameraRefPos = Vector3d.Zero;
        }

        /**
         * TODO.
         */
        public PlantsProducer(PlantsProducer master) : base("PlantsProducer", "CreatePlants", master.getCache(), false)
        {
            this.master = null;
        }


        public override bool hasTile(int level, int tx, int ty)
        {
            if (master != null)
            {
                return master.hasTile(level, tx, ty);
            }
            return level >= plants.getMinLevel() && level <= plants.getMaxLevel();
        }

        /**
         * TODO.
         */
        public void produceTiles()
        {
            Debug.Assert(master == null);
            localCameraPos = terrain.getLocalCamera();
            Vector3d worldCamera = node.getOwner().getCameraNode().getWorldPos();
            Vector3d deformedCamera = terrain.deform.localToDeformed(localCameraPos);
            Matrix4d A = terrain.deform.localToDeformedDifferential(localCameraPos);
            Matrix4d B = terrain.deform.deformedToTangentFrame(worldCamera);
            Matrix4d ltow = node.getLocalToWorld();
            localToTangentFrame = B * ltow * A;
            tangentFrameToWorld = Matrix4d.Invert(B);
            Matrix3d transpose = new Matrix3d();
            terrain.deform.deformedToTangentFrame(deformedCamera).Mat3x3.Transpose(out transpose);
            tangentSpaceToWorld = ltow.Mat3x3 * transpose;
            tangentFrameToScreen = node.getOwner().getWorldToScreen() * tangentFrameToWorld;
            cameraToTangentFrame = B * node.getOwner().getCameraNode().getLocalToWorld();
            localToScreen = node.getOwner().getWorldToScreen() * ltow * A;
            screenToLocal = Matrix4d.Invert(localToScreen);
            tangentCameraPos = cameraToTangentFrame * Vector3d.Zero;

            frustumV[0] = (screenToLocal * new Vector4d(-1.0, -1.0, -1.0, 1.0)).Xyz;
            frustumV[1] = (screenToLocal * new Vector4d(+1.0, -1.0, -1.0, 1.0)).Xyz;
            frustumV[2] = (screenToLocal * new Vector4d(-1.0, +1.0, -1.0, 1.0)).Xyz;
            frustumV[3] = (screenToLocal * new Vector4d(+1.0, +1.0, -1.0, 1.0)).Xyz;
            frustumV[4] = (screenToLocal * new Vector4d(-1.0, -1.0, +1.0, 1.0)).Xyz - frustumV[0];
            frustumV[5] = (screenToLocal * new Vector4d(+1.0, -1.0, +1.0, 1.0)).Xyz - frustumV[1];
            frustumV[6] = (screenToLocal * new Vector4d(-1.0, +1.0, +1.0, 1.0)).Xyz - frustumV[2];
            frustumV[7] = (screenToLocal * new Vector4d(+1.0, +1.0, +1.0, 1.0)).Xyz - frustumV[3];
            SceneManager.getFrustumPlanes(localToScreen, frustumP);

            frustumZ = new Vector4d(localToScreen.R3C0, localToScreen.R3C1, localToScreen.R3C2, localToScreen.R3C3);

            zNear = Vector4d.Dot(frustumZ, frustumV[0]);
            zRange = Vector4d.Dot(frustumZ, frustumV[4]);

            if (count > 0 && (cameraRefPos.Xy - localCameraPos.Xy).Length > 100000.0)
            {
                cameraRefPos = localCameraPos;
                invalidateTiles();
                for (int i = 0; i < slaves.Count; ++i)
                {
                    slaves[i].invalidateTiles();
                }
            }

            count = 0;
            total = 0;
            plantBounds.Clear();
            plantBox = new Box3d();
            putTiles( usedTiles, terrain.root);
            getTiles( ref usedTiles, terrain.root);
        }

        /**
         * TODO.
         */
        public MeshBuffers getPlantsMesh()
        {
            return ((GPUBufferTileStorage)getCache().getStorage()).mesh;
        }

        /**
         * TODO.
         */
        public static PlantsProducer getPlantsProducer(SceneNode tn, Plants plants)
        {
            PlantsProducer rst;
            if (producers.TryGetValue(Tuple.Create(tn, plants), out rst))
            {
                return rst;
            }
            TerrainNode t = tn.getField("terrain") as TerrainNode;
            TileSampler lcc = tn.getField("lcc") as TileSampler;
            TileSampler z = tn.getField("elevation") as TileSampler;
            TileSampler n = tn.getField("fnormal") as TileSampler;
            TileSampler occ = tn.getField("aperture") as TileSampler;
            if (lcc != null)
            {
                lcc.setStoreFilter(new PlantsTileFilter(plants));
            }
            if (z != null)
            {
                z.setStoreFilter(new PlantsTileFilter(plants));
            }
            if (n != null)
            {
                n.setStoreFilter(new PlantsTileFilter(plants));
            }
            if (occ != null)
            {
                occ.setStoreFilter(new PlantsTileFilter(plants));
            }
            TileCache cache = plantsCacheFactory.Get(plants);
            PlantsProducer p = new PlantsProducer(tn, t, plants, lcc, z, n, occ, cache);
            producers.Add(Tuple.Create(tn, plants), p);
            return p;
        }


        protected internal override bool doCreateTile(int level, int tx, int ty, TileStorage.Slot data)
        {
            if (master != null)
            {
                return master.doCreateTile(level, tx, ty, data);
            }
            GPUBufferTileStorage.GPUBufferSlot slot = (GPUBufferTileStorage.GPUBufferSlot)(data);
            Debug.Assert(level == plants.getMaxLevel());
            Debug.Assert(slot != null);

            if (lcc != null)
            {
                lcc.setTile(level, tx, ty);
            }
            if (z != null)
            {
                z.setTile(level, tx, ty);
            }
            if (n != null)
            {
                n.setTile(level, tx, ty);
            }
            if (occ != null)
            {
                occ.setTile(level, tx, ty);
            }

            if (cameraRefPos == Vector3d.Zero)
            {
                cameraRefPos = localCameraPos;
            }

            double rootQuadSize = terrain.root.l;
            double ox = rootQuadSize * ((float)(tx) / (1 << level) - 0.5f);
            double oy = rootQuadSize * ((float)(ty) / (1 << level) - 0.5f);
            double l = rootQuadSize / (1 << level);
            tileOffsetU.set(new Vector3f((float)(ox - cameraRefPos.X), (float)(oy - cameraRefPos.Y), (float)l));

            if (tileDeformU != null)
            {
                Matrix4d l2d = terrain.deform.localToDeformedDifferential(new Vector3d(ox + l / 2.0f, oy + l / 2.0f, 0.0f));
                Matrix4d d2t = terrain.deform.deformedToTangentFrame(l2d * Vector3d.Zero);
                Matrix4d t2l = Matrix4d.Invert(l2d) * Matrix4d.Invert(d2t);
                tileDeformU.set(new Matrix2f((float)t2l.R0C0, (float)t2l.R0C1, (float)t2l.R1C0, (float)t2l.R1C1));
            }

            int patternId = (int)(881 * System.Math.Abs(System.Math.Cos(ox * oy))) % plants.getPatternCount(); // TODO improve this
            MeshBuffers pattern = plants.getPattern(patternId);
            int nSeeds = (int)(pattern.nvertices);

            TransformFeedback tfb = TransformFeedback.getDefault();
            tfb.setVertexBuffer(0, slot.buffer, (uint)slot.offset, (uint)slot.getOwner().getTileSize());
            slot.size = -1;
            slot.query = new Query(QueryType.PRIMITIVES_GENERATED);
            slot.query.begin();
            TransformFeedback.begin(SceneManager.getCurrentFrameBuffer(), plants.selectProg, MeshMode.POINTS, tfb, false);
            TransformFeedback.transform(pattern, 0, nSeeds);
            TransformFeedback.end();
            slot.query.end();

            return true;
        }


        private class Tree
        {
            public int tileCount;

            public bool needTile;

            public bool[] needTiles;

            public TileCache.Tile[] tiles;

            public Tree[] children;

            public Tree(int tileCount)
            {
                children = new Tree[4];
                this.tileCount = tileCount; this.needTile = false;
                needTiles = tileCount == 0 ? null : new bool[tileCount];
                tiles = tileCount == 0 ? null : new TileCache.Tile[tileCount];
                for (int i = 0; i < tileCount; ++i)
                {
                    needTiles[i] = false;
                    tiles[i] = null;
                }
                children[0] = null;
                children[1] = null;
                children[2] = null;
                children[3] = null;
            }



            public void recursiveDelete(TileProducer owner)
            {
                if (children[0] != null)
                {
                    children[0].recursiveDelete(owner);
                    children[1].recursiveDelete(owner);
                    children[2].recursiveDelete(owner);
                    children[3].recursiveDelete(owner);
                }
                for (int i = 0; i < tileCount; ++i)
                {
                    if (tiles[i] != null)
                    {
                        owner.putTile(tiles[i]);
                    }
                }
                // delete this;
            }
        }

        private Plants plants;

        private TileSampler lcc;

        private TileSampler z;

        private TileSampler n;

        private TileSampler occ;

        private Uniform3f tileOffsetU;

        private UniformMatrix2f tileDeformU;

        private Tree usedTiles;


        private bool mustAmplifyTile(double ox, double oy, double l)
        {
            float d = plants.getMaxDistance() * plants.getMaxDistance();
            Vector2f c = (Vector2f)tangentCameraPos.Xy;
            Vector2f p1 = (Vector2f)(localToTangentFrame * new Vector3d(ox - localCameraPos.X, oy - localCameraPos.Y, 0.0)).Xy;
            Vector2f p2 = (Vector2f)(localToTangentFrame * new Vector3d(ox + l - localCameraPos.X, oy - localCameraPos.Y, 0.0)).Xy;
            Vector2f p3 = (Vector2f)(localToTangentFrame * new Vector3d(ox - localCameraPos.X, oy + l - localCameraPos.Y, 0.0)).Xy;
            Vector2f p4 = (Vector2f)(localToTangentFrame * new Vector3d(ox + l - localCameraPos.X, oy + l - localCameraPos.Y, 0.0)).Xy;
            return new Seg2f(p1, p2).SegmentDistSq(c) < d ||
                    new Seg2f(p2, p3).SegmentDistSq(c) < d ||
                    new Seg2f(p3, p4).SegmentDistSq(c) < d ||
                    new Seg2f(p4, p1).SegmentDistSq(c) < d;
        }


        private void putTiles(Tree t, TerrainQuad q)
        {
            Debug.Assert(q.level <= plants.getMaxLevel());
            if (t == null)
            {
                return;
            }

            bool needTile = q.level == plants.getMaxLevel() || (q.level >= plants.getMinLevel() && q.isLeaf());
            needTile &= q.visible != SceneManager.visibility.INVISIBLE;
            needTile &= mustAmplifyTile(q.ox, q.oy, q.l);
            t.needTile = needTile;

            if (needTile)
            {
                int n = 1 << (plants.getMaxLevel() - q.level);
                for (int y = 0; y < n; ++y)
                {
                    for (int x = 0; x < n; ++x)
                    {
                        int i = x + y * n;
                        double ox = q.ox + x * q.l / n;
                        double oy = q.oy + y * q.l / n;
                        double l = q.l / n;
                        t.needTiles[i] = n == 1 || mustAmplifyTile(ox, oy, l);
                        if (!t.needTiles[i] && t.tiles[i] != null)
                        {
                            putTile(t.tiles[i]);
                            t.tiles[i] = null;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < t.tileCount; ++i)
                {
                    if (t.tiles[i] != null)
                    {
                        putTile(t.tiles[i]);
                        t.tiles[i] = null;
                    }
                }
            }

            if (q.children[0] == null)
            {
                if (t.children[0] != null)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        t.children[i].recursiveDelete(this);
                        t.children[i] = null;
                    }
                }
            }
            else if (q.level < plants.getMaxLevel())
            {
                for (int i = 0; i < 4; ++i)
                {
                    putTiles(t.children[i], q.children[i]);
                }
            }
        }

        private void getTiles(ref Tree t, TerrainQuad q)
        {
            Debug.Assert(q.level <= plants.getMaxLevel());
            if ( t == null)
            {
                t = new Tree(q.level < plants.getMinLevel() ? 0 : 1 << 2 * (plants.getMaxLevel() - q.level));

                bool needTile = q.level == plants.getMaxLevel() || (q.level >= plants.getMinLevel() && q.isLeaf());
                needTile &= q.visible != SceneManager.visibility.INVISIBLE;
                needTile &= mustAmplifyTile(q.ox, q.oy, q.l);
                t.needTile = needTile;

                if (needTile)
                {
                    int n = 1 << (plants.getMaxLevel() - q.level);
                    for (int y = 0; y < n; ++y)
                    {
                        for (int x = 0; x < n; ++x)
                        {
                            int i = x + y * n;
                            double ox = q.ox + x * q.l / n;
                            double oy = q.oy + y * q.l / n;
                            double l = q.l / n;
                            t.needTiles[i] = n == 1 || mustAmplifyTile(ox, oy, l);
                        }
                    }
                }
            }

            if (t.needTile)
            {
                int n = 1 << (plants.getMaxLevel() - q.level);
                for (int y = 0; y < n; ++y)
                {
                    int ty = q.ty * n + y;
                    for (int x = 0; x < n; ++x)
                    {
                        int i = x + y * n;
                        int tx = q.tx * n + x;
                        if (t.needTiles[i])
                        {
                            if (t.tiles[i] == null)
                            {
                                if (q.level == plants.getMaxLevel())
                                {
                                    t.tiles[i] = getTile(plants.getMaxLevel(), tx, ty, 0);
                                }
                                else
                                {
                                    t.tiles[i] = slaves[plants.getMaxLevel() - q.level - 1].getTile(plants.getMaxLevel(), tx, ty, 0);
                                }
                                if (t.tiles[i] == null && log.IsErrorEnabled  )
                                {
                                    log.Error("Insufficient tile cache size for plants");
                                }
                            }
                            Debug.Assert(t.tiles[i] != null);
                            Task task = t.tiles[i].task;

                            uint completionDate = 0;
                            if (lcc != null)
                            {
                                TileCache.Tile u = null;
                                lcc.get().getGpuTileCoords(plants.getMaxLevel(), tx, ty,ref u);
                                if (u != null && u.task != null)
                                {
                                    completionDate = u.task.getCompletionDate();
                                }
                            }
                            if (task.isDone() && ((GPUBufferTileStorage.GPUBufferSlot )(t.tiles[i].getData(false))).date < completionDate)
                            {
                                task.setIsDone(false, 0, Task.reason.DATA_CHANGED);
                            }

                            if (!task.isDone())
                            {
                                task.run();
                                task.setIsDone(true, 0);
                                ((GPUBufferTileStorage.GPUBufferSlot)(t.tiles[i].getData(false))).date = completionDate;
                            }
                            GPUBufferTileStorage.GPUBufferSlot s =  (GPUBufferTileStorage.GPUBufferSlot )(t.tiles[i].getData(false));
                            if (s.size < 0 /*&& s.query.available()*/)
                            { // uncomment for fully asynchronous mode
                                s.size = (int)s.query.getResult();
                                s.query = null;
                            }
                            if (s.size > 0)
                            {
                                offsets[count] = s.offset / VERTEX_SIZE;
                                sizes[count] = s.size;
                                count += 1;
                                total += s.size;
                                updateTerrainHeights(q);
                            }
                        }
                    }
                }
            }

            if (q.children[0] != null && q.level < plants.getMaxLevel())
            {
                for (int i = 0; i < 4; ++i)
                {
                    getTiles(ref t.children[i], q.children[i]);
                }
            }
        }

        private readonly int[] segments = new int[24] { 0, 1, 1, 3, 3, 2, 2, 0, 4, 5, 5, 7, 7, 6, 6, 4, 0, 4, 1, 5, 3, 7, 2, 6 };

        private void updateTerrainHeights(TerrainQuad q)
        {
            double xmin = q.ox - localCameraPos.X;
            double xmax = q.ox - localCameraPos.X + q.l;
            double ymin = q.oy - localCameraPos.Y;
            double ymax = q.oy - localCameraPos.Y + q.l;
            double zmin = q.zmin;
            double zmax = q.zmax + 15.0; // maxTreeHeight (TODO remove this hardcoded constant)

            plantBox = plantBox.enlarge(new Box3d(xmin, xmax, ymin, ymax, zmin, zmax));

            double zMin = double.MaxValue;
            double zMax = 0.0;

            Vector3d[] V = new Vector3d[8];
            double[] Z = new double[8];
            for (int i = 0; i < 8; ++i)
            {
                double x = i % 2 == 0 ? xmin : xmax;
                double y = (i / 2) % 2 == 0 ? ymin : ymax;
                double z = (i / 4) % 2 == 0 ? zmin : zmax;
                V[i] = new Vector3d(x, y, z);
                Z[i] = Vector4d.Dot(frustumZ, V[i]);
            }

            double[,] prods = new double[8,5];
            int[] I = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

            for (int j = 0; j < 5; ++j)
            {
                int J = 0;
                for (int i = 0; i < 8; ++i)
                {
                    double p = Vector4d.Dot(frustumP[j], V[i]);
                    int vin = p >= 0.0 ? 1 : 0;
                    prods[i,j] = p;
                    I[i] += vin;
                    J += vin;
                }
                if (J == 0)
                {
                    // bbox fully outside frustum
                    return;
                }
            }

            int N = 0;
            for (int i = 0; i < 8; ++i)
            {
                if (I[i] == 5)
                {
                    // bbox vertice inside frustum
                    double z = Z[i];
                    zMin = System.Math.Min(zMin, z);
                    zMax = System.Math.Max(zMax, z);
                    N += 1;
                }
            }

            if (N == 8)
            {
                // bbox fully inside frustum
                plantBounds.Add(new Vector4d(zMin, zMax, zmin, zmax));
                return;
            }

            for (int i = 0; i < 24; i += 2)
            {
                int a = segments[i];
                int b = segments[i + 1];
                if (I[a] < 5 || I[b] < 5)
                {
                    double tIn = 0.0;
                    double tOut = 1.0;
                    for (int j = 0; j < 5; ++j)
                    {
                        double p = prods[a,j] - prods[b,j];
                        if (p < 0.0)
                        {
                            tIn = System.Math.Max(tIn, prods[a,j] / p);
                        }
                        else if (p > 0.0)
                        {
                            tOut = System.Math.Min(tOut, prods[a,j] / p);
                        }
                    }
                    if (tIn <= tOut && tIn < 1.0 && tOut > 0.0)
                    {
                        double zIn = Z[a] * (1.0 - tIn) + Z[b] * tIn;
                        double zOut = Z[a] * (1.0 - tOut) + Z[b] * tOut;
                        zMin = System.Math.Min(zMin, System.Math.Min(zIn, zOut));
                        zMax = System.Math.Max(zMax, System.Math.Max(zIn, zOut));
                    }
                }
            }

            for (int i = 0; i < 4; ++i)
            {
                int j = i + 4;
                double tIn = 0.0;
                double tOut = 1.0;
                tIn = System.Math.Max(tIn, ((frustumV[j].X < 0.0 ? xmax : xmin) - frustumV[i].X) / frustumV[j].X);
                tIn = System.Math.Max(tIn, ((frustumV[j].Y < 0.0 ? ymax : ymin) - frustumV[i].Y) / frustumV[j].Y);
                tIn = System.Math.Max(tIn, ((frustumV[j].Z < 0.0 ? zmax : zmin) - frustumV[i].Z) / frustumV[j].Z);
                tOut = System.Math.Min(tOut, ((frustumV[j].X < 0.0 ? xmin : xmax) - frustumV[i].X) / frustumV[j].X);
                tOut = System.Math.Min(tOut, ((frustumV[j].Y < 0.0 ? ymin : ymax) - frustumV[i].Y) / frustumV[j].Y);
                tOut = System.Math.Min(tOut, ((frustumV[j].Z < 0.0 ? zmin : zmax) - frustumV[i].Z) / frustumV[j].Z);
                if (tIn <= tOut && tIn < 1.0 && tOut > 0.0)
                {
                    double zIn = zNear + zRange * tIn;
                    double zOut = zNear + zRange * tOut;
                    zMin = System.Math.Min(zMin, System.Math.Min(zIn, zOut));
                    zMax = System.Math.Max(zMax, System.Math.Max(zIn, zOut));
                }
            }

            if (zMin < zMax)
            {
                plantBounds.Add(new Vector4d(zMin, zMax, zmin, zmax));
            }
        }


#if DELETEME
        /********************************************************/
        public Dictionary<Tile.Id, Tile> GetDrawableTiles()
        {
            return m_drawableTiles;
        }

        protected override void Start()
        {
            base.Start();

            m_drawableTiles = new Dictionary<Tile.Id, Tile>(new Tile.EqualityComparerID());

            m_plants = m_manager.GetPlantsNode();

            m_elevationProducer = m_elevationProducerGO.GetComponent<TileProducer>();
            m_lccProducer = m_lccProducerGO.GetComponent<TileProducer>();
            m_normalProducer = m_normalProducerGO.GetComponent<TileProducer>();

            CBTileStorage storage0 = GetCache().GetStorage(0) as CBTileStorage;
            CBTileStorage storage1 = GetCache().GetStorage(1) as CBTileStorage;

            if (storage0 == null)
            {
                throw new InvalidStorageException("Storage0 must be a CBTileStorage");
            }

            if (storage1 == null)
            {
                throw new InvalidStorageException("Storage1 must be a CBTileStorage");
            }

            if (storage0.GetBufferType() != ComputeBufferType.Default)
            {
                throw new InvalidStorageException("Tile storage0 buffer type must be default");
            }

            if (storage1.GetBufferType() != ComputeBufferType.Default)
            {
                throw new InvalidStorageException("Tile storage1 buffer type must be default");
            }

        }

        public override void PutTile(Tile tile)
        {
            if (tile != null)
            {
                Tile.Id id = tile.GetId();
                if (m_drawableTiles.ContainsKey(id))
                    m_drawableTiles.Remove(id);
            }

            base.PutTile(tile);
        }

        public override Tile GetTile(int level, int tx, int ty)
        {
            Tile tile = base.GetTile(level, tx, ty);

            if (tile != null)
            {
                Tile.Id id = tile.GetId();
                if (!m_drawableTiles.ContainsKey(id))
                    m_drawableTiles.Add(id, tile);
            }

            return tile;
        }

        public override void DoCreateTile(int level, int tx, int ty, List<TileStorage.Slot> slot)
        {

            CBTileStorage.CBSlot bufferSlot0 = slot[0] as CBTileStorage.CBSlot;
            CBTileStorage.CBSlot bufferSlot1 = slot[1] as CBTileStorage.CBSlot;

            Tile elevationTile = m_elevationProducer.FindTile(level, tx, ty, false, true);
            GPUTileStorage.GPUSlot elevationGpuSlot = null;

            if (elevationTile != null)
            {
                elevationGpuSlot = elevationTile.GetSlot(0) as GPUTileStorage.GPUSlot;
            }
            else
            {
                throw new MissingTileException("Find elevation tile failed");
            }

            Tile lccTile = m_lccProducer.FindTile(level, tx, ty, false, true);
            GPUTileStorage.GPUSlot lccGpuSlot = null;

            if (lccTile != null)
            {
                lccGpuSlot = lccTile.GetSlot(0) as GPUTileStorage.GPUSlot;
            }
            else
            {
                throw new MissingTileException("Find lcc tile failed");
            }

            Tile normalTile = m_normalProducer.FindTile(level, tx, ty, false, true);
            GPUTileStorage.GPUSlot normalGpuSlot = null;

            if (normalTile != null)
            {
                normalGpuSlot = normalTile.GetSlot(0) as GPUTileStorage.GPUSlot;
            }
            else
            {
                throw new MissingTileException("Find normal tile failed");
            }

            double rootQuadSize = GetTerrainNode().GetRoot().GetLength();
            double ox = rootQuadSize * ((double)tx / (1 << level) - 0.5);
            double oy = rootQuadSize * ((double)ty / (1 << level) - 0.5);
            double l = rootQuadSize / (1 << level);

            Matrix4x4d l2d = GetTerrainNode().GetDeform().LocalToDeformedDifferential(new Vector3d(ox + l / 2.0, oy + l / 2.0, 0.0));
            Matrix4x4d d2t = GetTerrainNode().GetDeform().DeformedToTangentFrame(l2d * Vector3d.Zero());
            Matrix4x4d t2l = l2d.Inverse() * d2t.Inverse();
            Vector4d tileDeform = new Vector4d(t2l.m[0, 0], t2l.m[0, 1], t2l.m[1, 0], t2l.m[1, 1]);

            Vector4d tileOffset = new Vector4d(ox, oy, l, 0);

            int patternId = (int)(881.0f * Mathf.Abs(Mathf.Cos((float)(ox * oy)))) % m_plants.GetPatternCount(); //TODO improve this

            ComputeBuffer pattern = m_plants.GetPattern(patternId);
            ComputeBuffer buffer0 = bufferSlot0.GetBuffer();
            ComputeBuffer buffer1 = bufferSlot1.GetBuffer();

            m_selectTrees.SetTexture(0, "_Elevation", elevationGpuSlot.GetTexture());
            m_selectTrees.SetInt("_Elevation_Size", m_elevationProducer.GetTileSizeMinBorder(0));
            m_selectTrees.SetInt("_Elevation_Border", m_elevationProducer.GetBorder());

            m_selectTrees.SetTexture(0, "_Lcc", lccGpuSlot.GetTexture());
            m_selectTrees.SetInt("_Lcc_Size", m_lccProducer.GetTileSizeMinBorder(0));
            m_selectTrees.SetInt("_Lcc_Border", m_lccProducer.GetBorder());

            m_selectTrees.SetTexture(0, "_Normal", normalGpuSlot.GetTexture());
            m_selectTrees.SetInt("_Normal_Size", m_normalProducer.GetTileSizeMinBorder(0));
            m_selectTrees.SetInt("_Normal_Border", m_normalProducer.GetBorder());

            m_selectTrees.SetVector("_TileDeform", tileDeform.ToVector4());
            m_selectTrees.SetVector("_TileOffset", tileOffset.ToVector4());
            m_selectTrees.SetInt("_PatternCount", pattern.count);
            m_selectTrees.SetBuffer(0, "_Pattern", pattern);
            m_selectTrees.SetBuffer(0, "_Buffer0", buffer0);
            m_selectTrees.SetBuffer(0, "_Buffer1", buffer1);

            m_selectTrees.Dispatch(0, buffer0.count / 8, 1, 1);

            base.DoCreateTile(level, tx, ty, slot);
        }
#endif
    }

}























