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
using System;
using System.Diagnostics;

namespace Sxta.Proland.Terrain
{
    /// <summary>
    /// An abstract raster data map. A map is a 2D array of pixels, whose
    /// values can come from anywhere (this depends on how you implement
    /// the GetValue method). A map can be read pixel by pixel, or tile
    /// by tile. The tiles are cached for better efficiency.
    /// </summary>
    public abstract class InputMap 
	{

		public class Id
		{
			public int tx, ty;
			
			public Id (int tx, int ty)
			{
				this.tx = tx;
				this.ty = ty;
			}

			public bool Equals(Id id) {
				return (tx == id.tx && ty == id.ty);
			}
			
			public override int GetHashCode() {
				int code = tx ^ ty;
				return code;
			}
		}

		//A Id is compared based on its tx and ty
		public class EqualityComparerID : IEqualityComparer<Id>
		{
			public bool Equals(Id t1, Id t2) {
				return t1.Equals(t2);
			}
			
			public int GetHashCode(Id t) {
				return t.GetHashCode();
			}
		}
		
		public class Tile
		{
			public int tx, ty;
			public float[] data;

			public Tile (int tx, int ty, float[] data)
			{
				this.tx = tx;
				this.ty = ty;
				this.data = data;
			}
		}

		private Dictionary<Id, Tile> tileCache;
        private List<Tile > tileCacheOrder;

        //capacity of the cache
        private int capacity = 200;

        /// <summary>
        /// The tile size to use when reading this map by tile.
        /// The width and height must be multiples of this size.
        /// </summary>
        public int tileSize;

        /// <summary>
        /// The width of this map.
        /// </summary>
        public abstract int Width { get; }

        /// <summary>
        /// The height of this map.
        /// </summary>
        public abstract int Height { get; } 

        public abstract int GetChannels();

		public int GetCapacity() {
			return capacity;
		}

		public int GetTileSize() {
			return tileSize;
		}

		protected virtual void Start()
		{

			if (tileSize <= 0)
				throw new ArgumentException("Tile size must be greater than 0");
			if (Width % tileSize != 0)
				throw new ArgumentException("Tile size must be divisable by width");
			if (Height % tileSize != 0)
				throw new ArgumentException("Tile size must be divisable by height");

			tileCache = new Dictionary<Id, Tile>(new EqualityComparerID());
		}

         /// <summary>
        /// Returns the value of the given pixel. You can implement this
        /// method any way you want.
        /// </summary>
        /// <param name="x">the x coordinate of the pixel to be read.</param>
        /// <param name="y">the y coordinate of the pixel to be read.</param>
        /// <returns>the value of the (x,y) pixel.</returns>
        public abstract Vector4f GetValue (int x, int y);

        /// <summary>
        /// Returns the values of the pixels of the given tile. The default
        /// implementation of this method calls GetValue to read each
        /// pixel.If GetValue reads a value from disk, it is strongly
        /// advised to override this method for better efficiency.
        /// </summary>
        /// <param name="tx">the tx coordinate of the pixel to be read.</param>
        /// <param name="ty">the ty coordinate of the pixel to be read.</param>
        /// <returns>an array of size tileSize x tileSize, containing the
 		///       values of the pixels in the[tx * tileSize, (tx + 1) *
        ///       tileSize[x[ty * tileSize, (ty + 1) * tileSize[region.
        /// </returns>
        public virtual float[] GetValues (int tx, int ty)
		{
			int c = GetChannels();

			float[] v = new float[tileSize * tileSize * c];

			for (int j = 0; j < tileSize; ++j) 
			{
				for (int i = 0; i < tileSize; ++i) 
				{
					Vector4f value = GetValue (tx + i, ty + j);
					int off = (i + j * tileSize) * c;
					v [off] = value.X;

					if (c > 1) {
						v [off + 1] = value.Y;
					}
					if (c > 2) {
						v [off + 2] = value.Z;
					}
					if (c > 3) {
						v [off + 3] = value.W;
					}
				}
			}

			return v;
		}

		float[] GetTile (int tx, int ty)
		{
			Id key = new Id (tx, ty);
            Tile t;
            if (!tileCache.TryGetValue (key, out t)) 
			{
				float[] data = GetValues (tx * tileSize, ty * tileSize);
				
				if (tileCache.Count == capacity) {
                    // evict least recently used tile if cache is full
                    Tile t2 = tileCacheOrder[0];
                    tileCache.Remove (new Id(t2.tx, t2.ty));
                    tileCacheOrder.RemoveAt(0);
                }
				
				// create tile, put it at the end of tileCache
				Tile tile = new Tile (tx, ty, data);
				
				tileCache.Add (key, tile);
                tileCacheOrder.Add(tile);
                Debug.Assert(tileCache.ContainsKey(key));
                return data;
			} 
			else 
			{
                 Debug.Assert(t.tx == tx && t.ty == ty);
                // put t at the end of tileCacheOrder, and update the map accordingly
                tileCacheOrder.Remove(t);
                tileCache.Add (key, t);
				
				return t.data;
			}
        }

         /// <summary>
        /// Returns the value of the given pixel. This method uses a cache
        ///  for better efficiency: it reads the tile containing the given pixel,
        ///  if it is not already in cache, puts it in cache, and returns the
        ///  requested pixel from this tile.
        /// </summary>
        /// <param name="x">the x coordinate of the pixel to be read.</param>
        /// <param name="y">the y coordinate of the pixel to be read.</param>
        /// <returns>the value of the (x,y) pixel.</returns>
        public Vector4f Get (int x, int y)
		{
			int c = GetChannels();

			x = System.Math.Max (System.Math.Min (x, Width - 1), 0);
			y = System.Math.Max (System.Math.Min (y, Height - 1), 0);
			int tx = x / tileSize;
			int ty = y / tileSize;
			x = x % tileSize;
			y = y % tileSize;

			int off = (x + y * tileSize) * c;
			float[] data = GetTile (tx, ty);
			Vector4f col = new Vector4f ();
			col.X = data [off];

			if (c > 1) {
				col.Y = data [off + 1];
			}
			if (c > 2) {
				col.Z = data [off + 2];
			}
			if (c > 3) {
				col.W = data [off + 3];
			}

			return col;
		}
	}
}













