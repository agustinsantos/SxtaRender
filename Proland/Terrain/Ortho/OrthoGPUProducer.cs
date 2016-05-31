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
using System.IO;

namespace Sxta.Proland.Terrain.Ortho
{
    /**
     * A TileProducer to create texture tiles on GPU from CPU textures tiles.
     * This %producer simply copies the CPU texture data into GPU textures.
     * This %producer accepts layers, which can modify the raw CPU texture
     * data, using shaders on GPU. If layers are used, this %producer can
     * produce tiles whose level is greater than the maximum level of the CPU
     * tile %producer, using an intermediate tile cache to store the CPU tiles
     * to be enlarged to produce zoomed versions. In fact, if layers are used,
     * the CPU tile %producer is not mandatory: in this case tiles are produced
     * enterily on GPU, using the layers alone.
     * @ingroup ortho
     * @authors Eric Bruneton, Antoine Begault, Guillaume Piolat
     */
    public class OrthoGPUProducer : TileProducer, ISwappable<OrthoGPUProducer>
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /**
         * Creates a new OrthoGPUProducer.
         *
         * @param cache the cache to store the produced tiles. The underlying
         *      storage must be a GPUTileStorage. The size of tiles in this
         *      storage size must be equal to the size of the tiles produced
         *      by orthoTiles.
         * @param backgroundCache an intermediate cache to store CPU tiles to
         *      be enlarged to produce zoomed in versions. Only necessary if
         *      layers are used, and if you want to produce tiles of level
         *      greater than the maximum level of orthoTiles.
         * @param orthoTiles the %producer producing the CPU tiles. This
         *      %producer should produce its tiles in a CPUTileStorage of
         *      unsigned byte type. Maybe null if layers are used (in this case
         *      tiles are produced enterily on GPU using the layers).
         * @param compressedTexture a texture used to copy the orthoTiles tiles
         *      to GPU, if they are stored in DXT format on CPU. Its size must
         *      be equal to the produced tile size (including borders), and its
         *      format equal to the DXT format used by orthoTiles. Maybe null if
         *      orthoTiles are not compressed.
         * @param uncompressedTexture a texture used to uncompress tiles on GPU,
         *      or if orthoTiles are not compressed, to copy them directly to GPU.
         *      Its size must be equal to the produced tile size (including borders).
         */
        public OrthoGPUProducer(TileCache cache, TileCache backgroundCache, TileProducer orthoTiles,
        int maxLevel = -1, Texture2D compressedTexture = null, Texture2D uncompressedTexture = null) :
            base("OrthoGPUProducer", "CreateOrthoGPUTile")
        {
            init(cache, backgroundCache, orthoTiles, maxLevel, compressedTexture, uncompressedTexture);
        }


        /**
         * Deletes this OrthoGPUProducer.
         */
        // public  virtual ~OrthoGPUProducer();

        public override void getReferencedProducers(List<TileProducer> producers)
        {
            if (coarseGpuTiles != null)
            {
                producers.Add(coarseGpuTiles);
            }
            if (orthoTiles != null)
            {
                producers.Add(orthoTiles);
            }
        }


        public override void setRootQuadSize(float size)
        {
            base.setRootQuadSize(size);
            if (orthoTiles != null)
            {
                orthoTiles.setRootQuadSize(size);
            }
        }


        public override int getBorder()
        {
            return orthoTiles == null ? 2 : orthoTiles.getBorder();
        }


        public override bool hasTile(int level, int tx, int ty)
        {
            if (hasLayers())
            {
                return maxLevel == -1 || level <= maxLevel;
            }
            else
            {
                return orthoTiles == null ? (maxLevel == -1 || level <= maxLevel) : orthoTiles.hasTile(level, tx, ty);
            }
        }


        public override bool prefetchTile(int level, int tx, int ty)
        {
            bool b = base.prefetchTile(level, tx, ty);
            if (!b)
            {
                if (orthoTiles != null)
                {
                    if (hasLayers() && !orthoTiles.hasTile(level, tx, ty))
                    {
                        int l = level;
                        int x = tx;
                        int y = ty;
                        while (!coarseGpuTiles.hasTile(l, x, y))
                        {
                            l -= 1;
                            x /= 2;
                            y /= 2;
                        }
                        coarseGpuTiles.prefetchTile(l, x, y);
                    }
                    else
                    {
                        orthoTiles.prefetchTile(level, tx, ty);
                    }
                }
            }
            return b;
        }


        /**
         * Creates an uninitialized OrthoGPUProducer.
         */
        public OrthoGPUProducer() :
                    base("OrthoGPUProducer", "CreateOrthoGPUTile")
        {
        }

        /**
         * Initializes this OrthoGPUProducer. See #OrthoGPUProducer.
         */
        public void init(TileCache cache, TileCache backgroundCache, TileProducer orthoTiles,
                    int maxLevel = -1, Texture2D compressedTexture = null, Texture2D uncompressedTexture = null)
        {
            base.init(cache, true);
            this.orthoTiles = orthoTiles;
            this.frameBuffer = orthoGPUFramebufferFactory.Get(uncompressedTexture);
            this.maxLevel = maxLevel;
            this.compressedTexture = compressedTexture;
            this.uncompressedTexture = uncompressedTexture;
            tileSize = cache.getStorage().getTileSize();
            channels = 0;
            ((GPUTileStorage)cache.getStorage()).getTexture(0).getComponents();
            if (orthoTiles != null)
            {
                TileStorage s = orthoTiles.getCache().getStorage();
                Debug.Assert(tileSize == s.getTileSize());
                channels = ((CPUTileStorage<byte>)s).getChannels();
            }

            if (orthoTiles != null && ((OrthoCPUProducer)orthoTiles).isCompressed())
            {
                Debug.Assert(compressedTexture != null);
                Debug.Assert(compressedTexture.getWidth() == tileSize);
                Debug.Assert(compressedTexture.getHeight() == tileSize);
                Debug.Assert(uncompressedTexture != null);
                Debug.Assert(uncompressedTexture.getWidth() == tileSize);
                Debug.Assert(uncompressedTexture.getHeight() == tileSize);
                if (uncompress == null)
                {
                    uncompress = new Program(new Module(330, uncompressShader));
                }
                uncompressSourceU = uncompress.getUniformSampler("source");
            }

            if (backgroundCache != null)
            {
                coarseGpuTiles = new OrthoGPUProducer(backgroundCache, null, orthoTiles, -1, compressedTexture, uncompressedTexture);
            }
        }

        protected internal override ulong getContext()
        {
            return (ulong)uncompressedTexture.GetHashCode();
        }


        protected internal override Task startCreateTile(int level, int tx, int ty, uint deadline, Task task, TaskGraph owner)
        {
            TaskGraph result = owner == null ? createTaskGraph(task) : owner;
            if (orthoTiles != null)
            {
                TileCache.Tile t;
                if (hasLayers() && !orthoTiles.hasTile(level, tx, ty))
                {
                    int l = level;
                    int x = tx;
                    int y = ty;
                    while (!coarseGpuTiles.hasTile(l, x, y))
                    {
                        l -= 1;
                        x /= 2;
                        y /= 2;
                    }
                    t = coarseGpuTiles.getTile(l, x, y, deadline);

                    if (upsample == null)
                    {
                        upsample = new Program(new Module(330, upsampleShader));
                        upsampleSourceU = upsample.getUniformSampler("source");
                        tileU = upsample.getUniform4f("tile");
                    }
                }
                else
                {
                    t = orthoTiles.getTile(level, tx, ty, deadline);
                }
                Debug.Assert(t != null);
                result.addTask(t.task);
                result.addDependency(task, t.task);
            }

            // calls each layer so that it can complete the task graph
            // with the necessary sub tasks
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
            if (log.IsDebugEnabled)
            {
                log.Debug("GPU tile " + getId() + " " + level + " " + tx + " " + ty);
            }

            GPUTileStorage.GPUSlot gpuData = (GPUTileStorage.GPUSlot)(data);
            Debug.Assert(gpuData != null);
            ((GPUTileStorage)getCache().getStorage()).notifyChange(gpuData);

            CPUTileStorage<byte>.CPUSlot cpuData = null;

            TileCache.Tile coarseTile = null;
            GPUTileStorage.GPUSlot coarseGpuData = null;

            if (orthoTiles != null)
            {
                if (hasLayers() && !orthoTiles.hasTile(level, tx, ty))
                {
                    int l = level;
                    int x = tx;
                    int y = ty;
                    while (!coarseGpuTiles.hasTile(l, x, y))
                    {
                        l -= 1;
                        x /= 2;
                        y /= 2;
                    }

                    coarseTile = coarseGpuTiles.findTile(l, x, y);
                    Debug.Assert(coarseTile != null);
                    coarseGpuData = (GPUTileStorage.GPUSlot)(coarseTile.getData());
                    Debug.Assert(coarseGpuData != null);
                }
                else
                {
                    TileCache.Tile t = orthoTiles.findTile(level, tx, ty);
                    Debug.Assert(t != null);
                    cpuData = (CPUTileStorage<byte>.CPUSlot)(t.getData());
                    Debug.Assert(cpuData != null);
                }
            }

            TextureFormat f;
            switch (channels)
            {
                case 1:
                    f = TextureFormat.RED;
                    break;
                case 2:
                    f = TextureFormat.RG;
                    break;
                case 3:
                    f = TextureFormat.RGB;
                    break;
                default:
                    f = TextureFormat.RGBA;
                    break;
            }
            if (compressedTexture == null && !hasLayers())
            {
                Debug.Assert(cpuData != null);
                if (channels != 2 || tileSize % 2 == 0)
                {
                    gpuData.setSubImage(0, 0, tileSize, tileSize, f, PixelType.UNSIGNED_BYTE, new Render.Buffer.Parameters(), new CPUBuffer<byte>(cpuData.data));
                }
                else
                {
                    // TODO better way to fix this "OpenGL bug" (?) with odd texture sizes?
                    byte[] tmp = new byte[tileSize * tileSize * 4];
                    for (int i = 0; i < tileSize * tileSize; ++i)
                    {
                        tmp[4 * i] = cpuData.data[2 * i];
                        tmp[4 * i + 1] = cpuData.data[2 * i + 1];
                        tmp[4 * i + 2] = 0;
                        tmp[4 * i + 3] = 0;
                    }
                    gpuData.setSubImage(0, 0, tileSize, tileSize, TextureFormat.RGBA, PixelType.UNSIGNED_BYTE, new Render.Buffer.Parameters(), new CPUBuffer<byte>(tmp));
                    // delete[] tmp;
                }
            }
            else
            {
                if (cpuData != null)
                {
                    if (compressedTexture != null)
                    {
                        compressedTexture.setCompressedSubImage(0, 0, 0, tileSize, tileSize, cpuData.size, new CPUBuffer<byte>(cpuData.data));
                        uncompressSourceU.set(compressedTexture);
                        frameBuffer.drawQuad(uncompress);
                    }
                    else
                    {
                        uncompressedTexture.setSubImage(0, 0, 0, tileSize, tileSize, f, PixelType.UNSIGNED_BYTE, new Render.Buffer.Parameters(), new CPUBuffer<byte>(cpuData.data));
                    }
                }
                if (coarseGpuData != null)
                {
                    Vector4f coords = coarseGpuTiles.getGpuTileCoords(level, tx, ty, ref coarseTile);
                    float b = (float)(getBorder()) / (1 << (level - coarseTile.level));
                    float s = (float)getCache().getStorage().getTileSize();
                    float S = s / (s - 2 * getBorder());
                    coords.X -= b / coarseGpuData.getWidth();
                    coords.Y -= b / coarseGpuData.getHeight();
                    coords.W *= S;

                    upsampleSourceU.set(coarseGpuData.t);
                    tileU.set(coords);
                    frameBuffer.drawQuad(upsample);
                }
                if (hasLayers())
                {
                    base.doCreateTile(level, tx, ty, data);
                }
                gpuData.copyPixels(frameBuffer, 0, 0, tileSize, tileSize);
            }

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
            if (orthoTiles != null)
            {
                TileCache.Tile t;
                if (hasLayers() && !orthoTiles.hasTile(level, tx, ty))
                {
                    int l = level;
                    int x = tx;
                    int y = ty;
                    while (!coarseGpuTiles.hasTile(l, x, y))
                    {
                        l -= 1;
                        x /= 2;
                        y /= 2;
                    }
                    t = coarseGpuTiles.findTile(l, x, y);
                    Debug.Assert(t != null);
                    coarseGpuTiles.putTile(t);
                }
                else
                {
                    t = orthoTiles.findTile(level, tx, ty);
                    Debug.Assert(t != null);
                    orthoTiles.putTile(t);
                }
            }

            base.stopCreateTile(level, tx, ty);
        }



        public void swap(OrthoGPUProducer p)
        {
            base.swap(p);
            Std.Swap(ref frameBuffer, ref p.frameBuffer);
            //Std.Swap(ref old, ref p.old);
            Std.Swap(ref orthoTiles, ref p.orthoTiles);
            Std.Swap(ref coarseGpuTiles, ref p.coarseGpuTiles);
            Std.Swap(ref channels, ref p.channels);
            Std.Swap(ref tileSize, ref p.tileSize);
            Std.Swap(ref compressedTexture, ref p.compressedTexture);
            Std.Swap(ref uncompressedTexture, ref p.uncompressedTexture);
        }



        private FrameBuffer frameBuffer;

        /**
         * The %producer producing the CPU tiles. This %producer should produce
         * its tiles in a CPUTileStorage of unsigned byte type. Maybe null if
         * layers are used (in this case tiles are produced enterily on GPU
         * using the layers).
         */
        private TileProducer orthoTiles;

        /**
         * An intermediate OrthoGPUProducer to produce the CPU tiles to
         * be enlarged to produce zoomed in versions. Only necessary if
         * layers are used, and if you want to produce tiles of level
         * greater than the maximum level of orthoTiles.
         */
        private TileProducer coarseGpuTiles;

        /**
         * The number of components per pixel of the CPU tiles.
         */
        private int channels;

        /**
         * The size of the produced tiles, including borders.
         */
        private int tileSize;

        private int maxLevel;

        /**
         * A texture used to copy the #orthoTiles tiles to GPU, if they are
         * stored in DXT format on CPU.
         */
        private Texture2D compressedTexture;

        /**
         * A texture used to uncompress tiles on GPU, or if #orthoTiles are
         * not compressed, to copy them directly to GPU.
         */
        private Texture2D uncompressedTexture;

        private UniformSampler uncompressSourceU;

        private UniformSampler upsampleSourceU;

        private Uniform4f tileU;

        private static Program uncompress;

        private static Program upsample;

        private static FrameBuffer old;

        private static Factory<Texture2D, FrameBuffer> orthoGPUFramebufferFactory =
                new Factory<Texture2D, FrameBuffer>(createOrthoGPUFramebuffer);

        private static FrameBuffer createOrthoGPUFramebuffer(Texture2D uncompressedTexture)
        {
            if (uncompressedTexture == null)
            {
                return null;
            }
            int tileSize = uncompressedTexture.getWidth();
            FrameBuffer frameBuffer = new FrameBuffer();
            frameBuffer.setReadBuffer(BufferId.COLOR0);
            frameBuffer.setDrawBuffer(BufferId.COLOR0);
            frameBuffer.setViewport(new Vector4i(0, 0, tileSize, tileSize));
            frameBuffer.setTextureBuffer(BufferId.COLOR0, uncompressedTexture, 0);
            frameBuffer.setPolygonMode(PolygonMode.FILL, PolygonMode.FILL);
            frameBuffer.setDepthTest(false);
            frameBuffer.setBlend(false);
            frameBuffer.setColorMask(true, true, true, true);
            frameBuffer.setDepthMask(true);
            return frameBuffer;
        }
        public const string uncompressShader = @"
                #ifdef _VERTEX_
                    layout (location = 0) in vec4 vertex;
                    out vec2 uv;
                    void main() {
                        gl_Position = vertex;
                        uv = vertex.xy * 0.5 + 0.5;
                    }
                #endif
                #ifdef _FRAGMENT_
                    layout (location = 0) out vec4 data;
                    in vec2 uv;
                    uniform sampler2D source;
                    void main() {
                        data = textureLod(source, uv, 0.0);
                    }
                #endif";

        public const string upsampleShader = @"
            #ifdef _VERTEX_
                uniform vec4 tile;
                layout (location = 0) in vec4 vertex;
                out vec3 uvl;
                void main() {
                    gl_Position = vertex;
                    uvl = vec3(tile.xy + (vertex.xy * 0.5 + 0.5) * tile.w, tile.z);
                }
            #endif
            #ifdef _FRAGMENT_
                uniform sampler2DArray source;
                layout (location = 0) out vec4 data;
                in vec3 uvl;
                void main() {
                    data = texture(source, uvl);
                }
            #endif";
    }
}
