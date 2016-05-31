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
using System.Collections.Generic;
using System.Diagnostics;

namespace Sxta.Proland.Terrain
{
    /// <summary>
    /// An OrthoGPUProducer layer to blend the tiles of two OrthoGPUProducer.
    /// This layer modifies the tiles of its OrthoGPUProducer %producer by
    /// blending into them the tiles produced by another OrthoGPUProducer,
    /// after they have been transformed via a GPU Program.
    /// </summary>
    public class TextureLayer : TileLayer, ISwappable<TextureLayer>
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Structure used to pass blend arguments to the framebuffer.
        /// </summary>
        public class BlendParams
        {
            public BufferId buffer;

            public BlendEquation rgb = BlendEquation.ADD;

            public BlendArgument srgb = BlendArgument.ONE;

            public BlendArgument drgb = BlendArgument.ZERO;

            public BlendEquation alpha = BlendEquation.ADD;

            public BlendArgument salpha = BlendArgument.ONE;

            public BlendArgument dalpha = BlendArgument.ZERO;
        }

        /**
         * Creates a new TextureLayer.
         *
         * @param tiles the %producer producing the tiles that must be blended
         *      into the tiles produced by the %producer to which this layer
         *      belongs. The 'tiles' %producer must be a GPU producer, but the
         *      size and format of its tiles are arbitrary. Its maximum level can
         *      also be less than the maximum level of the %producer to which this
         *      layer belongs (in this cas an automatic zoom is used to produce
         *      finer tiles).
         * @param program the Program to be used to transform the tiles produced
         *      by 'tiles' before bending them into the tiles produced by the
         *      %producer to which this layer belongs.
         * @param tilesSamplerName name of the samplerTile uniform in 'program'
         *      that is used to access the tiles produced by 'tiles'.
         * @param blend the blend equations and functions to be used to blend
                the tiles together.
         * @param minDisplayLevel the quadtree level at which the display of
         *      this layer must start.
         */
        public TextureLayer(TileProducer tiles, Program program, string tilesSamplerName,
            BlendParams blend, int minDisplaylevel, bool storeTiles = false) : base("TextureLayer")
        {
            init(tiles, program, tilesSamplerName, blend, minDisplayLevel, storeTiles);
        }


        /**
         * Deletes this TextureLayer.
         */
        // virtual ~TextureLayer();

        public override void getReferencedProducers(List<TileProducer> producers)
        {
            producers.Add(tiles);
        }

        public override void useTile(int level, int tx, int ty, uint deadline)
        {
            if (storeTiles && level >= minDisplayLevel)
            {
                int l = level;
                int x = tx;
                int y = ty;
                while (!tiles.hasTile(l, x, y))
                {
                    l -= 1;
                    x /= 2;
                    y /= 2;
                    Debug.Assert(l >= 0);
                }
                TileCache.Tile t = tiles.getTile(l, x, y, deadline);
                Debug.Assert(t != null);
            }
        }

        public override void unuseTile(int level, int tx, int ty)
        {
            if (storeTiles && level >= minDisplayLevel)
            {
                int l = level;
                int x = tx;
                int y = ty;
                while (!tiles.hasTile(l, x, y))
                {
                    l -= 1;
                    x /= 2;
                    y /= 2;
                    Debug.Assert(l >= 0);
                }
                TileCache.Tile t = tiles.findTile(l, x, y);
                Debug.Assert(t != null);
                tiles.putTile(t);
            }
        }

        public override bool doCreateTile(int level, int tx, int ty, TileStorage.Slot data)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("ORTHO Texture tile " + getProducerId() + " " + level + " " + tx + " " + ty);
            }

            if (level < minDisplayLevel)
            {
                return true;
            }

            int l = level;
            int x = tx;
            int y = ty;
            while (!tiles.hasTile(l, x, y))
            {
                l -= 1;
                x /= 2;
                y /= 2;
                Debug.Assert(l >= 0);
            }

            FrameBuffer fb = SceneManager.getCurrentFrameBuffer();

            TileCache.Tile t = tiles.findTile(l, x, y);
            Debug.Assert(t != null);
            GPUTileStorage.GPUSlot gput = (GPUTileStorage.GPUSlot)(t.getData());
            Debug.Assert(gput != null);

            Vector4f coords = tiles.getGpuTileCoords(level, tx, ty, ref t);
            int s = tiles.getCache().getStorage().getTileSize();
            float b = (float)(tiles.getBorder()) / (1 << (level - l));
            float S = s / (s - 2.0f * tiles.getBorder());

            // correct border
            Vector4f coordsCorrected = coords;
            coordsCorrected.X -= b / gput.getWidth();
            coordsCorrected.Y -= b / gput.getHeight();
            coordsCorrected.W *= S;

            samplerU.set(gput.t);
            coordsU.set(new Vector3f(coordsCorrected.X, coordsCorrected.Y, coords.Z));
            sizeU.set(new Vector3f(coordsCorrected.W, coordsCorrected.W, (s / 2) * 2.0f - 2.0f * b));

            if (blend.buffer != (BufferId)(-1))
            {
                fb.setBlend(blend.buffer, true, blend.rgb, blend.srgb, blend.drgb,
                    blend.alpha, blend.salpha, blend.dalpha);
            }
            else
            {
                fb.setBlend(true, blend.rgb, blend.srgb, blend.drgb,
                    blend.alpha, blend.salpha, blend.dalpha);
            }

            fb.drawQuad(program);

            if (blend.buffer != (BufferId)(-1))
            {
                fb.setBlend(blend.buffer, false);
            }
            else
            {
                fb.setBlend(false);
            }

            return true;
        }

        public override void startCreateTile(int level, int tx, int ty, uint deadline, Task task, TaskGraph result)
        {
            if ((result != null) && (level >= minDisplayLevel))
            {
                int l = level;
                int x = tx;
                int y = ty;
                while (!tiles.hasTile(l, x, y))
                {
                    l -= 1;
                    x /= 2;
                    y /= 2;
                    Debug.Assert(l >= 0);
                }

                TileCache.Tile tile = tiles.getTile(l, x, y, deadline);

                // if you fail here, try to log "CACHE" debug messages
                // because a cache might be full
                Debug.Assert(tile != null);

                result.addTask(tile.task);
                result.addDependency(task, tile.task);
            }
        }

        public override void stopCreateTile(int level, int tx, int ty)
        {
            if (level >= minDisplayLevel)
            {
                int l = level;
                int x = tx;
                int y = ty;
                while (!tiles.hasTile(l, x, y))
                {
                    l -= 1;
                    x /= 2;
                    y /= 2;
                    Debug.Assert(l >= 0);
                }
                TileCache.Tile t = tiles.findTile(l, x, y);
                if (t != null)
                {
                    tiles.putTile(t);
                }
            }
        }

        /// <summary>
        /// The Program to be used to transform the tiles produced by #tiles before
        /// bending them into the tiles produced by the %producer to which this
        /// layer belongs.
        /// </summary>
        protected Program program;

        /// <summary>
        /// Creates an uninitialized TextureLayer.
        /// </summary>
        protected TextureLayer() : base("TextureLayer")
        {
        }

        /// <summary>
        /// Initializes this TextureLayer. See #TextureLayer.
        /// </summary>
        /// <param name="tiles"></param>
        /// <param name="program"></param>
        /// <param name="tilesSamplerName"></param>
        /// <param name="blend"></param>
        /// <param name="minDisplaylevel"></param>
        /// <param name="storeTiles"></param>
        protected void init(TileProducer tiles, Program program, string tilesSamplerName,
                            BlendParams blend, int minDisplaylevel, bool storeTiles = false)
        {
            base.init(false);

            this.tiles = tiles;
            this.program = program;
            this.tilesSamplerName = tilesSamplerName;
            this.blend = blend;
            this.minDisplayLevel = minDisplaylevel;
            this.storeTiles = storeTiles;

            Debug.Assert(tiles.isGpuProducer());

            GPUTileStorage storage = tiles.getCache().getStorage() as GPUTileStorage;
            Debug.Assert(storage != null);

            samplerU = program.getUniformSampler(tilesSamplerName + ".tilePool");
            coordsU = program.getUniform3f(tilesSamplerName + ".tileCoords");
            sizeU = program.getUniform3f(tilesSamplerName + ".tileSize");
        }

        public void swap(TextureLayer p)
        {
            base.swap(p);
            Std.Swap(ref program, ref p.program);
            Std.Swap(ref tiles, ref p.tiles);
            Std.Swap(ref tilesSamplerName, ref p.tilesSamplerName);
            Std.Swap(ref blend, ref p.blend);
            Std.Swap(ref storeTiles, ref p.storeTiles);
            Std.Swap(ref minDisplayLevel, ref p.minDisplayLevel);
            Std.Swap(ref samplerU, ref p.samplerU);
            Std.Swap(ref coordsU, ref p.coordsU);
            Std.Swap(ref sizeU, ref p.sizeU);
        }

        /// <summary>
        /// The  producer producing the tiles that must be blended into the tiles
        /// produced by the %producer to which this layer belongs. Must be a GPU
        /// producer, but the size and format of its tiles are arbitrary.
        /// </summary>
        private TileProducer tiles;

        /// <summary>
        /// Name of the samplerTile uniform in #program that is used to access the
        /// tiles produced by #tiles.
        /// </summary>
        private string tilesSamplerName;

        /// <summary>
        /// The blend equations and functions to be used to blend the tiles together.
        /// </summary>
        private BlendParams blend;

        private bool storeTiles;

        /// <summary>
        /// The quadtree level at which the display of this layer must start.
        /// </summary>
        int minDisplayLevel;

        private UniformSampler samplerU;

        private Uniform3f coordsU;

        private Uniform3f sizeU;
    }

}


























