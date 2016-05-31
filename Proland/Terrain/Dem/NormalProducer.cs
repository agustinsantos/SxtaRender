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
using System.Reflection;
using System.Xml;

namespace Sxta.Proland.Terrain
{
    class NormalProducer : TileProducer, ISwappable<NormalProducer>
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static FrameBuffer createNormalFramebuffer(Texture2D normalTexture)
        {
            int tileWidth = normalTexture.getWidth();
            FrameBuffer frameBuffer = new FrameBuffer();
            frameBuffer.setReadBuffer(BufferId.COLOR0);
            frameBuffer.setDrawBuffer(BufferId.COLOR0);
            frameBuffer.setViewport(new Vector4i(0, 0, tileWidth, tileWidth));
            frameBuffer.setTextureBuffer(BufferId.COLOR0, normalTexture, 0);
            frameBuffer.setPolygonMode(PolygonMode.FILL, PolygonMode.FILL);
            frameBuffer.setDepthTest(false);
            frameBuffer.setBlend(false);
            frameBuffer.setColorMask(true, true, true, true);
            frameBuffer.setDepthMask(true);
            frameBuffer.setStencilMask(1, 1);
            return frameBuffer;
        }

        //FACTORIES
        Factory<Texture2D, FrameBuffer> normalFramebufferFactory = new Factory<Texture2D, FrameBuffer>(createNormalFramebuffer);

        /// <summary>
        /// Creates a new NormalProducer.
        /// </summary>
        /// <param name="cache"> the cache to store the produced tiles. The underlying
        ///      storage must be a GPUTileStorage with textures of two or four
        /// components per pixel.If two components are used only one normal
        ///      is stored per pixel (n.x and n.y are stored, n.z is implicit). If
        /// four components are used a coarse normal is also stored, to provide
        /// smooth interpolation of normals between quadtree levels.If floating
        /// point textures are used normals components vary between -1 and 1. If
        /// non floating point textures are used, this range is scaled to 0-1 to
        /// fit in the texture format.</param>
        /// <param name="elevationTiles">the %producer producing the elevation tiles on GPU.
        /// The underlying storage must be a GPUTileStorage with floating point
        /// textures with at least 3 components per pixel(4 if 'deform' is true).
        /// The elevation tile size, without borders, must be equal to the normal
        /// tile size, minus 1.</param>
        /// <param name="normalTexture">a texture used to produce the tiles. Its size must be
        /// equal to the normal tile size.Its format must be the same as the
        /// format used for the tile storage of this %producer.</param>
        /// <param name="normals">the Program to compute normals from elevations on GPU.</param>
        /// <param name="gridMeshSize">the size of the grid that will be used to render each tile.
        /// Must be the tile size(minus 1) divided by a power of two.</param>
        /// <param name="deform">true if the produced normals will be mapped on a spherical terrain</param>
        public NormalProducer(TileCache cache, TileProducer elevationTiles,
        Texture2D normalTexture, Program normals, int gridSize, bool deform = false) : base("NormalProducer", "CreateNormalTile")
        {
            init(cache, elevationTiles, normalTexture, normals, gridSize, deform);
        }

        /// <summary>
        /// Creates an uninitialized NormalProducer.
        /// </summary>
        protected NormalProducer() : base("NormalProducer", "CreateNormalTile")
        {

        }

        public override void getReferencedProducers(List<TileProducer> producers)
        {
            producers.Add(elevationTiles);
        }

        public override void setRootQuadSize(float size)
        {
            base.setRootQuadSize(size);
            elevationTiles.setRootQuadSize(size);
        }

        public override int getBorder()
        {
            return 0;
        }

        public override bool hasTile(int level, int tx, int ty)
        {
            return elevationTiles.hasTile(level, tx, ty);
        }


        /// <summary>
        /// The Program to compute normals from elevations on GPU.
        /// </summary>
        internal Program normals;

        /// <summary>
        /// Initializes this NormalProducer. See #NormalProducer.
        /// </summary>
        internal void init(TileCache cache, TileProducer elevationTiles,
            Texture2D normalTexture, Program normals, int gridSize, bool deform = false)
        {
            base.init(cache, true);
            this.elevationTiles = elevationTiles;
            this.normalTexture = normalTexture;
            this.normals = normals;
            this.frameBuffer = normalFramebufferFactory.Get(normalTexture);
            this.deform = deform;
            this.gridMeshSize = gridSize;
            this.tileSDFU = normals.getUniform3f("tileSDF");
            this.elevationSamplerU = normals.getUniformSampler("elevationSampler");
            this.elevationOSLU = normals.getUniform4f("elevationOSL");
            this.normalSamplerU = normals.getUniformSampler("normalSampler");
            this.normalOSLU = normals.getUniform4f("normalOSL");
            this.patchCornersU = normals.getUniformMatrix4f("patchCorners");
            this.patchVerticalsU = normals.getUniformMatrix4f("patchVerticals");
            this.patchCornerNormsU = normals.getUniform4f("patchCornerNorms");
            this.worldToTangentFrameU = normals.getUniformMatrix3f("worldToTangentFrame");
            this.parentToTangentFrameU = normals.getUniformMatrix3f("parentToTangentFrame");
            this.deformU = normals.getUniform4f("deform");

            Debug.Assert(cache.getStorage().getTileSize() == elevationTiles.getCache().getStorage().getTileSize() - 2 * elevationTiles.getBorder());
            Debug.Assert(normalTexture.getWidth() == cache.getStorage().getTileSize());
            Debug.Assert(normalTexture.getHeight() == cache.getStorage().getTileSize());
            Debug.Assert((cache.getStorage().getTileSize() - 1) % gridSize == 0);
        }

        protected internal override ulong getContext()
        {
            return 0;//normalTexture.get();
        }

        protected internal override Render.Scenegraph.Task startCreateTile(int level, int tx, int ty, uint deadline, Render.Scenegraph.Task task, TaskGraph owner)
        {
            TaskGraph result = owner == null ? createTaskGraph(task) : owner;

            if (level > 0)
            {
                TileCache.Tile _t = getTile(level - 1, tx / 2, ty / 2, deadline);
                Debug.Assert(_t != null);
                result.addTask(_t.task);
                result.addDependency(task, _t.task);
            }

            TileCache.Tile t = elevationTiles.getTile(level, tx, ty, deadline);
            Debug.Assert(t != null);
            result.addTask(t.task);
            result.addDependency(task, t.task);

            return result;
        }

        protected internal override void beginCreateTile()
        {
            old = SceneManager.getCurrentFrameBuffer();
            SceneManager.setCurrentFrameBuffer(frameBuffer);
        }

        protected internal override bool doCreateTile(int level, int tx, int ty, TileStorage.Slot data)
        {
            if (log.IsDebugEnabled)
            {
                string oss = "Normal tile " + getId() + " " + level + " " + tx + " " + ty;
                log.DebugFormat("DEM", oss);
            }

            GPUTileStorage.GPUSlot gpuData = (GPUTileStorage.GPUSlot)(data);
            Debug.Assert(gpuData != null);

            int tileWidth = data.getOwner().getTileSize();

            Texture storage = ((GPUTileStorage)(getCache().getStorage())).getTexture(0);
            TextureInternalFormat f = storage.getInternalFormat();
            int components = storage.getComponents();
            bool signedComponents = f != TextureInternalFormat.RG8 && f != TextureInternalFormat.RGBA8;

            GPUTileStorage.GPUSlot parentGpuData = null;
            if (level > 0)
            {
                TileCache.Tile tile = findTile(level - 1, tx / 2, ty / 2);
                Debug.Assert(tile != null);
                parentGpuData = (GPUTileStorage.GPUSlot)(tile.getData());
                Debug.Assert(parentGpuData != null);
            }

            TileCache.Tile t = elevationTiles.findTile(level, tx, ty);
            Debug.Assert(t != null);
            GPUTileStorage.GPUSlot elevationGpuData = (GPUTileStorage.GPUSlot)(t.getData());
            Debug.Assert(elevationGpuData != null);

            float format = components == 4 ? (signedComponents ? 0.0f : 1.0f) : (signedComponents ? 2.0f : 3.0f);
            tileSDFU.set(new Vector3f((float)tileWidth, ((getCache().getStorage().getTileSize() - 1) / gridMeshSize), format));

            if (normalSamplerU != null)
            {
                if (level > 0 && components == 4)
                {
                    Texture tile = parentGpuData.t;
                    float _dx = (tx % 2) * (tileWidth / 2.0f);
                    float _dy = (ty % 2) * (tileWidth / 2.0f);
                    normalSamplerU.set(parentGpuData.t);
                    normalOSLU.set(new Vector4f((_dx + 0.25f) / parentGpuData.getWidth(), (_dy + 0.25f) / parentGpuData.getHeight(), 1.0f / parentGpuData.getWidth(), GPUTileStorage.GPUSlot.l));
                }
                else
                {
                    normalOSLU.set(new Vector4f(-1.0f, -1.0f, -1.0f, -1.0f));
                }
            }

            float dx = (elevationTiles.getBorder());
            float dy = (elevationTiles.getBorder());
            elevationSamplerU.set(elevationGpuData.t);
            elevationOSLU.set(new Vector4f((dx + 0.25f) / elevationGpuData.getWidth(), (dy + 0.25f) / elevationGpuData.getHeight(), 1.0f / elevationGpuData.getWidth(), GPUTileStorage.GPUSlot.l));

            if (deform)
            {
                double D = getRootQuadSize();
                double R = D / 2.0;
                Debug.Assert(D > 0.0);
                double x0 = (double)(tx) / (1 << level) * D - R;
                double x1 = (double)(tx + 1) / (1 << level) * D - R;
                double y0 = (double)(ty) / (1 << level) * D - R;
                double y1 = (double)(ty + 1) / (1 << level) * D - R;
                double l0, l1, l2, l3;
                Vector3d p0 = new Vector3d(x0, y0, R);
                Vector3d p1 = new Vector3d(x1, y0, R);
                Vector3d p2 = new Vector3d(x0, y1, R);
                Vector3d p3 = new Vector3d(x1, y1, R);
                Vector3d pc = new Vector3d((x0 + x1) * 0.5, (y0 + y1) * 0.5, R);
                Vector3d v0 = Vector3d.Normalize(p0, out l0);
                Vector3d v1 = Vector3d.Normalize(p1, out l1);
                Vector3d v2 = Vector3d.Normalize(p2, out l2);
                Vector3d v3 = Vector3d.Normalize(p3, out l3);
                Vector3d vc = (v0 + v1 + v2 + v3) * 0.25;

                Matrix4d deformedCorners = new Matrix4d(
                    v0.X * R - vc.X * R, v1.X * R - vc.X * R, v2.X * R - vc.X * R, v3.X * R - vc.X * R,
                    v0.Y * R - vc.Y * R, v1.Y * R - vc.Y * R, v2.Y * R - vc.Y * R, v3.Y * R - vc.Y * R,
                    v0.Z * R - vc.Z * R, v1.Z * R - vc.Z * R, v2.Z * R - vc.Z * R, v3.Z * R - vc.Z * R,
                    1.0, 1.0, 1.0, 1.0);

                Matrix4d deformedVerticals = new Matrix4d(
                    v0.X, v1.X, v2.X, v3.X,
                    v0.Y, v1.Y, v2.Y, v3.Y,
                    v0.Z, v1.Z, v2.Z, v3.Z,
                    0.0, 0.0, 0.0, 0.0);

                Vector3d uz = pc;
                uz.Normalize();
                Vector3d ux = Vector3d.Cross(Vector3d.UnitY, uz);
                ux.Normalize();
                Vector3d uy = Vector3d.Cross(uz, ux);
                Matrix3d worldToTangentFrame = new Matrix3d(
                    ux.X, ux.Y, ux.Z,
                    uy.X, uy.Y, uy.Z,
                    uz.X, uz.Y, uz.Z);

                if (level > 0 && parentToTangentFrameU != null)
                {
                    double px0 = (tx / 2 + 0.5) / (1 << (level - 1)) * D - R;
                    double py0 = (ty / 2 + 0.5) / (1 << (level - 1)) * D - R;
                    pc = new Vector3d(px0, py0, R);
                    uz = pc;
                    uz.Normalize();
                    ux = Vector3d.Cross(Vector3d.UnitY, uz);
                    ux.Normalize();
                    uy = Vector3d.Cross(uz, ux);
                    Matrix3d parentToTangentFrame = new Matrix3d();
                    Matrix3d tmp = new Matrix3d(
                        ux.X, uy.X, uz.X,
                        ux.Y, uy.Y, uz.Y,
                        ux.Z, uy.Z, uz.Z);
                    worldToTangentFrame.Multiply(ref tmp, out parentToTangentFrame);

                    parentToTangentFrameU.setMatrix((Matrix3f)parentToTangentFrame);
                }

                patchCornersU.setMatrix((Matrix4f)deformedCorners);
                patchVerticalsU.setMatrix((Matrix4f)deformedVerticals);
                patchCornerNormsU.set(new Vector4f((float)l0, (float)l1, (float)l2, (float)l3));
                worldToTangentFrameU.setMatrix((Matrix3f)worldToTangentFrame);
                deformU.set(new Vector4f((float)x0, (float)y0, (float)D / (1 << level), (float)R));
            }
            else
            {
                double D = getRootQuadSize();
                double R = D / 2.0;
                double x0 = (double)(tx) / (1 << level) * D - R;
                double y0 = (double)(ty) / (1 << level) * D - R;
                if (worldToTangentFrameU != null)
                {
                    worldToTangentFrameU.setMatrix(Matrix3f.Identity);
                }
                deformU.set(new Vector4f((float)x0, (float)y0, (float)D / (1 << level), 0.0f));
            }

            frameBuffer.drawQuad(normals);
            gpuData.copyPixels(frameBuffer, 0, 0, tileWidth, tileWidth);

            return true;
        }

        protected internal override void endCreateTile()
        {
            SceneManager.setCurrentFrameBuffer(old);
            old = null;
        }

        protected internal override void stopCreateTile(int level, int tx, int ty)
        {
            if (level > 0)
            {
                TileCache.Tile tile = findTile(level - 1, tx / 2, ty / 2);
                Debug.Assert(tile != null);
                putTile(tile);
            }

            TileCache.Tile _t = elevationTiles.findTile(level, tx, ty);
            Debug.Assert(_t != null);
            elevationTiles.putTile(_t);
        }

        public virtual void swap(NormalProducer p)
        {
            base.swap(p);
            Std.Swap(ref frameBuffer, ref p.frameBuffer);
            Std.Swap(ref old, ref p.old);
            Std.Swap(ref normals, ref p.normals);
            Std.Swap(ref elevationTiles, ref p.elevationTiles);
            Std.Swap(ref normalTexture, ref p.normalTexture);
            Std.Swap(ref deform, ref p.deform);
            Std.Swap(ref gridMeshSize, ref p.gridMeshSize);
            Std.Swap(ref tileSDFU, ref p.tileSDFU);
            Std.Swap(ref elevationSamplerU, ref p.elevationSamplerU);
            Std.Swap(ref elevationOSLU, ref p.elevationOSLU);
            Std.Swap(ref normalSamplerU, ref p.normalSamplerU);
            Std.Swap(ref normalOSLU, ref p.normalOSLU);
            Std.Swap(ref patchCornersU, ref p.patchCornersU);
            Std.Swap(ref patchVerticalsU, ref p.patchVerticalsU);
            Std.Swap(ref patchCornerNormsU, ref p.patchCornerNormsU);
            Std.Swap(ref worldToTangentFrameU, ref p.worldToTangentFrameU);
            Std.Swap(ref parentToTangentFrameU, ref p.parentToTangentFrameU);
            Std.Swap(ref deformU, ref p.deformU);
        }


        private FrameBuffer frameBuffer;

        /// <summary>
        /// The %producer producing the elevation tiles on GPU. The underlying storage
        /// must be a GPUTileStorage with floating point textures with at least 3
        /// components per pixel(4 if 'deform' is true). The elevation tile size,
        /// without borders, must be equal to the normal tile size, minus 1.
        /// </summary>
        private TileProducer elevationTiles;

        /// <summary>
        /// A texture used to produce the tiles. Its size must be  equal to the normal
        /// tile size.Its format must be the same as the format used for the tile
        /// storage of this %producer.
        /// </summary>
        private Texture2D normalTexture;

        /// <summary>
        /// true if the produced elevations will be mapped on a spherical %terrain.
        /// </summary>
        private bool deform;

        /// <summary>
        /// The size of the grid that will be used to render each tile.
        /// Must be the tile size(without borders) divided by a power of two.
        /// </summary>
        private int gridMeshSize;

        private Uniform3f tileSDFU;

        private UniformSampler elevationSamplerU;

        private Uniform4f elevationOSLU;

        private UniformSampler normalSamplerU;

        private Uniform4f normalOSLU;

        private UniformMatrix4f patchCornersU;

        private UniformMatrix4f patchVerticalsU;

        private Uniform4f patchCornerNormsU;

        private UniformMatrix3f worldToTangentFrameU;

        private UniformMatrix3f parentToTangentFrameU;

        private Uniform4f deformU;

        private FrameBuffer old;
    }
    class NormalProducerResource : ResourceTemplate<NormalProducer>
    {
        public NormalProducerResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) :
        base(50, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            TileCache cache;
            TileProducer elevations;
            Texture2D normalTexture;
            Program normalsProg;
            int gridSize = 24;
            bool deform = false;
            checkParameters(desc, e, "name,cache,elevations,normalProg,gridSize,deform,");
            cache = (TileCache)manager.loadResource(getParameter(desc, e, "cache")).get();
            elevations = (TileProducer)manager.loadResource(getParameter(desc, e, "elevations")).get();
            string normals = "normalShader;";
            if (e.GetAttribute("normalProg") != null)
            {
                normals = getParameter(desc, e, "normalProg");
            }
            normalsProg = (Program)(manager.loadResource(normals).get());
            if (e.GetAttribute("gridSize") != null)
            {
                getIntParameter(desc, e, "gridSize", out gridSize);
            }
            if (e.GetAttribute("deform") != null && e.GetAttribute("deform") == "sphere")
            {
                deform = true;
            }

            int tileSize = cache.getStorage().getTileSize();
            string format = ((GPUTileStorage)(cache.getStorage())).getTexture(0).getInternalFormatName();
            if (format.Substring(0, 3) == "RG8")
            {
                format = "RGBA8";
            }

            string normalTex = "rendebuffer-" + tileSize + "-" + format;
            normalTexture = (Texture2D)manager.loadResource(normalTex).get();

            valueC.init(cache, elevations, normalTexture, normalsProg, gridSize, deform);
        }

        public override bool prepareUpdate()
        {
#if TODO
        if (dynamic_cast<Resource*>(normals.get())->changed()) {
            invalidateTiles();
        }
        return ResourceTemplate<50, NormalProducer>::prepareUpdate();
#endif
            throw new NotImplementedException();
        }
    }
}