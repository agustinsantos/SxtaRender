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
using Sxta.Math;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sxta.Proland.Terrain
{
    public abstract class AbstractTileCache
    {
        class Tile
        {
            public int tx, ty;
            public float[] data;

            public Tile(int tx, int ty, float[] data = null)
            {
                this.tx = tx;
                this.ty = ty;
                this.data = data;
            }

            public int key(int width)
            {
                return tx + ty * width;
            }

            static public int key(int tx, int ty, int width)
            {
                return tx + ty * width;
            }
        }


        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return height;
        }
        public int GetTileSize()
        {
            return tileSize;
        }

        public int GetChannels()
        {
            return channels;
        }
        public AbstractTileCache(int width, int height, int tileSize, int channels, int capacity = 20)
        {
            this.width = width;
            this.height = height;
            this.tileSize = tileSize;
            this.channels = channels;
            this.capacity = capacity;

            tileCache = new Dictionary<int, Tile>();
            tileCacheOrder = new List<Tile>();

        }

        protected abstract float[] ReadTile(int tx, int ty);

        public float[] GetTile(int tx, int ty)
        {
            int key = Tile.key(tx, ty, width / tileSize + 1);

            if (!tileCache.ContainsKey(key))
            {
                float[] data = ReadTile(tx, ty);

                if (tileCache.Count == capacity)
                {
                    // evict least recently used tile if cache is full
                    Tile t = tileCacheOrder.First();
                    tileCache.Remove(t.key(width / tileSize + 1));
                    tileCacheOrder.Remove(t);
                }

                // create tile, put it at the end of tileCacheOrder, and update the map
                Tile tile = new Tile(tx, ty, data);
                tileCache.Add(key, tile);
                tileCacheOrder.Add(tile);
                Debug.Assert(tileCache.ContainsKey(key));
                return data;
            }
            else
            {
                Tile t = tileCache[key];
                Debug.Assert(t.tx == tx && t.ty == ty);
                // put t at the end of tileCacheOrder, and update the map accordingly
                tileCache.Remove(key);
                tileCache.Add(key, t);
                tileCacheOrder.Add(t);

                return t.data;
            }
        }

        public virtual float GetTileHeight(int x, int y)
        {
            x = System.Math.Max(System.Math.Min(x, width), 0);
            y = System.Math.Max(System.Math.Min(y, height), 0);
            int tx = System.Math.Min(x, width - 1) / tileSize;
            int ty = System.Math.Min(y, height - 1) / tileSize;
            x = (x == width ? tileSize : x % tileSize) + 2;
            y = (y == height ? tileSize : y % tileSize) + 2;

            float[] data = GetTile(tx, ty);
            int off = (x + y * (tileSize + 5));
            return data[off];
        }

        public virtual Vector4f GetTileColor(int x, int y)
        {
            x = System.Math.Max(System.Math.Min(x, width - 1), 0);
            y = System.Math.Max(System.Math.Min(y, height - 1), 0);
            int tx = x / tileSize;
            int ty = y / tileSize;
            x = x % tileSize + 2;
            y = y % tileSize + 2;

            float[] data = GetTile(tx, ty);
            int off = (x + y * (tileSize + 4)) * channels;

            Vector4f c = new Vector4f();
            c.X = data[off];

            if (channels > 1)
            {
                c.Y = data[off + 1];
            }
            if (channels > 2)
            {
                c.Z = data[off + 2];
            }
            if (channels > 3)
            {
                c.W = data[off + 3];
            }

            return c;
        }

        public virtual void Reset(int width, int height, int tileSize)
        {
            tileCache.Clear();
            tileCacheOrder.Clear();
            this.width = width;
            this.height = height;
            this.tileSize = tileSize;
        }

        private int width;
        private int height;
        private int tileSize;
        private int channels;
        private int capacity;
        Dictionary<int, Tile> tileCache;
        List<Tile> tileCacheOrder;
    }
}















