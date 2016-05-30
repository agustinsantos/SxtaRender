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

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;

namespace Sxta.Proland.Terrain
{

	public class HeightMipmap : AbstractTileCache
	{

		public HeightMipmap left;
		public HeightMipmap right;
		public HeightMipmap bottom;
		public HeightMipmap top;
		public int leftr;
		public int rightr;
		public int bottomr;
		public int topr;

        private HeightFunction height;

        private int topLevelSize;

        private int baseLevelSize;

        private int tileSize;

        private float scale;

        private string tmpFolder;

        private int minLevel;

        private int maxLevel;

        private int currentMipLevel;

        private float[] tile;

        private int currentLevel;

        private int constantTile;

        private float[] m_maxR;

		public void SetCurrentLevel (int level) {
			currentLevel = level;
		}

		public HeightMipmap (HeightFunction height, int topLevelSize, int baseLevelSize, int tileSize, float scale, string tmpFolder) :
		                        base(baseLevelSize, baseLevelSize, tileSize, 1, 200)
		{

			this.height = height;
			this.topLevelSize = topLevelSize; 
			this.baseLevelSize = baseLevelSize; 
			this.tileSize = tileSize;
			scale = 1.0f;
			this.tmpFolder = tmpFolder;
			minLevel = 0;
			maxLevel = 0;

			int size = tileSize;
			while (size > topLevelSize) {
				minLevel += 1;
				size /= 2;
			}
			size = baseLevelSize;
			while (size > topLevelSize) {
				maxLevel += 1;
				size /= 2;
			}

			m_maxR = new float[maxLevel+1];
			tile = new float[(tileSize + 5) * (tileSize + 5) * 1];
			constantTile = -1;

			left = null;
			right = null;
			bottom = null;
			top = null;

            Debug.WriteLine("Poland.HeightMipmap.HeightMipmap - top level size = " + this.topLevelSize);
            Debug.WriteLine("Poland.HeightMipmap.HeightMipmap - base level size = " + this.baseLevelSize);
            Debug.WriteLine("Poland.HeightMipmap.HeightMipmap - tile size = " + this.tileSize);
			Debug.WriteLine("Poland.HeightMipmap.HeightMipmap - scale = " + scale);
			Debug.WriteLine("Poland.HeightMipmap.HeightMipmap - min level = " + minLevel);
			Debug.WriteLine("Poland.HeightMipmap.HeightMipmap - max level = " + maxLevel);
		}
        public static void setCube(HeightMipmap hm1, HeightMipmap  hm2, HeightMipmap  hm3, HeightMipmap  hm4, HeightMipmap  hm5, HeightMipmap  hm6)
        { throw new NotImplementedException(); }

        public void Compute1() { throw new NotImplementedException(); }

        public bool Compute2() { throw new NotImplementedException(); }

        public void Compute()
		{
			BuildBaseLevelTiles ();
			currentMipLevel = maxLevel - 1;

			while(currentMipLevel >= 0) {
				BuildMipmapLevel (currentMipLevel--);
			} 
		}

		public void Generate (int rootLevel, int rootTx, int rootTy, float scale, string file)
		{
			for (int level = 1; level <= maxLevel; ++level) {
				BuildResiduals (level);
			}

			File.Delete(file);

			int nTiles = minLevel + ((1 << (System.Math.Max(maxLevel - minLevel, 0) * 2 + 2)) - 1) / 3;
			Debug.WriteLine("Proland.HeightMipmap.Generate - num tiles = " + nTiles);

			long[] offsets = new long[nTiles * 2];
			byte[] byteArray = new byte[(7 * 4) + (+ m_maxR.Length * 4) + (offsets.Length * 8)];
			long offset = byteArray.Length;

			using(Stream stream = new FileStream(file, FileMode.Create))
			{
				stream.Seek(0, SeekOrigin.Begin);
				stream.Write(byteArray, 0, byteArray.Length);
			}

			for (int l = 0; l < minLevel; ++l) {
				ProduceTile(l, 0, 0, ref offset, offsets, file);
			}

			for (int l = minLevel; l <= maxLevel; ++l) {
				ProduceTilesLebeguesOrder(l - minLevel, 0, 0, 0, ref offset, offsets, file);
			}

			System.Buffer.BlockCopy(new int[]{minLevel}, 0, byteArray, 0, 4);
			System.Buffer.BlockCopy(new int[]{maxLevel}, 0, byteArray, 4, 4);
			System.Buffer.BlockCopy(new int[]{tileSize}, 0, byteArray, 8, 4);
			System.Buffer.BlockCopy(new int[]{rootLevel}, 0, byteArray, 12, 4);
			System.Buffer.BlockCopy(new int[]{rootTx}, 0, byteArray, 16, 4);
			System.Buffer.BlockCopy(new int[]{rootTy}, 0, byteArray, 20, 4);
			System.Buffer.BlockCopy(new float[]{scale}, 0, byteArray, 24, 4);
			System.Buffer.BlockCopy(m_maxR, 0, byteArray, 28, 4 * m_maxR.Length);
			System.Buffer.BlockCopy(offsets, 0, byteArray, 28 + (4 * m_maxR.Length), 8 * offsets.Length);

			using(Stream stream = new FileStream(file, FileMode.Open))
			{
				stream.Seek(0, SeekOrigin.Begin);
				stream.Write(byteArray, 0, byteArray.Length);
			}

			for(int i = 0; i < m_maxR.Length; i++)
				Debug.WriteLine("Proland.HeightMipmap.Generate - level " + i + " max residual = " + m_maxR[i].ToString("F6"));

			Debug.WriteLine("Proland.HeightMipmap.Generate - Saved file = " + file);

		}

		void Rotation(int r, int n, int x, int y, out int xp, out int yp)
		{
			switch (r) 
			{
			case 0:
				xp = x;
				yp = y;
				break;
			case 1:
				xp = y;
				yp = n - 1 - x;
				break;
			case 2:
				xp = n - 1 - x;
				yp = n - 1 - y;
				break;
			case 3:
				xp = n - 1 - y;
				yp = x;
				break;
			default:
				xp = 0;
				yp = 0;
				break;
			}
		}

		public override float GetTileHeight (int x, int y)
		{
			int levelSize = 1 + (baseLevelSize >> (maxLevel - currentLevel));

			if (x <= 2 && y <= 2 && left != null && bottom != null) {
				x = 0;
				y = 0;
			} else if (x > levelSize - 4 && y <= 2 && right != null && bottom != null) {
				x = levelSize - 1;
				y = 0;
			} else if (x <= 2 && y > levelSize - 4 && left != null && top != null) {
				x = 0;
				y = levelSize - 1;
			} else if (x > levelSize - 4 && y > levelSize - 4 && right != null && top != null) {
				x = levelSize - 1;
				y = levelSize - 1;
			}

			if (x < 0 && left != null) {
				int xp;
				int yp;
				Rotation (leftr, levelSize, levelSize - 1 + x, y, out xp, out yp);
				//assert(left->currentLevel == currentLevel);
				return left.GetTileHeight (xp, yp);
			}
			if (x >= levelSize && right != null) {
				int xp;
				int yp;
				Rotation (rightr, levelSize, x - levelSize + 1, y, out xp, out yp);
				//assert(right->currentLevel == currentLevel);
				return right.GetTileHeight (xp, yp);
			}
			if (y < 0 && bottom != null) {
				int xp;
				int yp;
				Rotation (bottomr, levelSize, x, levelSize - 1 + y, out xp, out yp);
				//assert(bottom->currentLevel == currentLevel);
				return bottom.GetTileHeight (xp, yp);
			}
			if (y >= levelSize && top != null) {
				int xp;
				int yp;
				Rotation (topr, levelSize, x, y - levelSize + 1, out xp, out yp);
				//assert(top->currentLevel == currentLevel);
				return top.GetTileHeight (xp, yp);
			}

			return base.GetTileHeight (x, y);

		}

		public override void Reset (int width, int height, int tileSize)
		{

			if (GetWidth () != width || GetHeight () != height) 
			{
				Debug.WriteLine("Proland.HeightMipmap.Reset - Resetting to width = " + width + " height = " + height + " tile size = " + tileSize);
				base.Reset (width, height, tileSize);

				if (left != null) {
					left.SetCurrentLevel(currentLevel);
					left.Reset (width, height, tileSize);
				}
				if (right != null) {
					right.SetCurrentLevel(currentLevel);
					right.Reset (width, height, tileSize);
				}
				if (bottom != null) {
					bottom.SetCurrentLevel(currentLevel);
					bottom.Reset (width, height, tileSize);
				}
				if (top != null) {
					top.SetCurrentLevel(currentLevel);
					top.Reset (width, height, tileSize);
				}
			} 
			else {
				if (left != null) {
					left.SetCurrentLevel(currentLevel);
				}
				if (right != null) {
					right.SetCurrentLevel(currentLevel);
				}
				if (bottom != null) {
					bottom.SetCurrentLevel(currentLevel);
				}
				if (top != null) {
					top.SetCurrentLevel(currentLevel);
				}
			}
		}

		void SaveTile(string name, int level, int tx, int ty, float[] tile)
		{
			string fileName = tmpFolder + "/" + name + "-" + level + "-" + tx + "-" + ty + ".raw";
			//Debug.WriteLine("Proland.HeightMipmap.SaveTile - Saving tile = " + fileName);
			
			byte[] byteArray = new byte[tile.Length * 4];
			System.Buffer.BlockCopy(tile, 0, byteArray, 0, byteArray.Length);
			System.IO.File.WriteAllBytes(fileName, byteArray);
		}

		void LoadTile(string name, int level, int tx, int ty, float[] tile)
		{
			string fileName = tmpFolder + "/" + name + "-" + level + "-" + tx + "-" + ty + ".raw";
			//Debug.WriteLine("Proland.HeightMipmap.LoadTile - Loading tile = " + fileName);
			
			FileInfo fi = new FileInfo(fileName);
			
			if(fi == null) throw new FileNotFoundException("Could not read tile " + fileName);
			
			FileStream fs = fi.OpenRead();
			byte[] data = new byte[fi.Length];
			fs.Read(data, 0, (int)fi.Length);
			fs.Close();

			for(int x = 0, i = 0; x < fi.Length/4; x++, i+=4) {
				tile[x] = System.BitConverter.ToSingle(data, i);
			};
		}

		protected override float[] ReadTile (int tx, int ty)
		{
			float[] tile = new float[(tileSize + 5) * (tileSize + 5)];

			LoadTile("Base", currentLevel, tx, ty, tile);

			return tile;
		}

		protected void GetTile (int level, int tx, int ty, float[] tile)
		{
			int tileSize = System.Math.Min (topLevelSize << level, this.tileSize);
			for (int j = 0; j <= tileSize + 4; ++j) {
				for (int i = 0; i <= tileSize + 4; ++i) {
					tile [i + j * (this.tileSize + 5)] = GetTileHeight(i + tileSize * tx - 2, j + tileSize * ty - 2) / scale;
				}
			}
		}

		void BuildBaseLevelTiles()
		{
			int nTiles = baseLevelSize / tileSize;
			
			Debug.WriteLine("Proland.HeightMipmap.BuildBaseLevelTiles - Build mipmap level " + maxLevel);

			float maxR = float.NegativeInfinity;
			
			for (int ty = 0; ty < nTiles; ++ty) 
			{
				for (int tx = 0; tx < nTiles; ++tx) 
				{
					int off = 0;
					for (int j = -2; j <= tileSize + 2; ++j) 
					{
						for (int i = -2; i <= tileSize + 2; ++i) 
						{
							float h = height.GetHeight(tx * tileSize + i, ty * tileSize + j);
							tile[off++] = h;

							if(h > maxR) maxR = h;
						}
					}

					SaveTile("Base", maxLevel, tx, ty, tile);
				}
			}

			Debug.WriteLine("Proland.HeightMipmap.BuildBaseLevelTiles -  max residual = " + maxR.ToString("F6"));
			m_maxR[0] = maxR / scale;
		}

		void BuildMipmapLevel(int level)
		{

			int nTiles = System.Math.Max(1, (baseLevelSize / tileSize) >> (maxLevel - level));
			
			Debug.WriteLine("Proland.HeightMipmap.BuildMipmapLevel - Build mipmap level = " + level);
			
			currentLevel = level + 1;
			Reset(baseLevelSize >> (maxLevel - currentLevel), baseLevelSize >> (maxLevel - currentLevel), System.Math.Min(topLevelSize << currentLevel, tileSize));

			for (int ty = 0; ty < nTiles; ++ty) 
			{
				for (int tx = 0; tx < nTiles; ++tx) 
				{
				
					int off = 0;
					int currentTileSize = System.Math.Min(topLevelSize << level, tileSize);
					for (int j = -2; j <= currentTileSize + 2; ++j) 
					{
						for (int i = -2; i <= currentTileSize + 2; ++i) 
						{
							int ix = 2 * (tx * currentTileSize + i);
							int iy = 2 * (ty * currentTileSize + j);

							tile[off++] = GetTileHeight(ix, iy);
						}
					}

					SaveTile("Base", level, tx, ty, tile);
				}
			}
		}

		void BuildResiduals(int level)
		{

			int nTiles = System.Math.Max(1, (baseLevelSize / tileSize) >> (maxLevel - level));
			//int tileSize = System.Math.Min(m_topLevelSize << level, m_tileSize);

			Debug.WriteLine("Proland.HeightMipmap.BuildResiduals - Build residuals level = " + level);
			
			currentLevel = level;
			Reset(baseLevelSize >> (maxLevel - currentLevel), baseLevelSize >> (maxLevel - currentLevel), System.Math.Min(topLevelSize << currentLevel, tileSize));
			
			float[] parentTile = new float[(tileSize + 5) * (tileSize + 5)];
			float[] currentTile = new float[(tileSize + 5) * (tileSize + 5)];
			float[] residualTile = new float[(tileSize + 5) * (tileSize + 5)];

			float levelMaxR = float.NegativeInfinity;

			for (int ty = 0; ty < nTiles; ++ty) 
			{
				for (int tx = 0; tx < nTiles; ++tx) 
				{
					float maxR, meanR, maxErr;

					GetApproxTile(level - 1, tx / 2, ty / 2, parentTile);
					GetTile(level, tx, ty, currentTile);
					ComputeResidual(parentTile, currentTile, level, tx, ty, residualTile, out maxR, out meanR);
					ComputeApproxTile(parentTile, residualTile, level, tx, ty, currentTile, out maxErr);

					if (level < maxLevel) {
						SaveTile("Approx", level, tx, ty, currentTile);
					}

					SaveTile("Residual", level, tx, ty, residualTile);

					if(maxR > levelMaxR) levelMaxR = maxR;

					Debug.WriteLine("Proland.HeightMipmap.BuildResiduals - " + level + "-" + tx + "-" + ty + ", max residual = " + maxR.ToString("F6") + ", max error = " + maxErr.ToString("F6"));
				}
			}

			m_maxR[level] = levelMaxR;

		}

		void GetApproxTile(int level, int tx, int ty, float[] tile)
		{
			if (level == 0) 
			{
				int oldLevel = currentLevel;
				currentLevel = 0;
				Reset(topLevelSize, topLevelSize, topLevelSize);
				GetTile(level, tx, ty, tile);
				currentLevel = oldLevel;
				Reset(baseLevelSize >> (maxLevel - currentLevel), baseLevelSize >> (maxLevel - currentLevel), System.Math.Min(topLevelSize << currentLevel, tileSize));
				return;
			}

			LoadTile("Approx", level, tx, ty, tile);
		}

		void ComputeResidual(float[] parentTile, float[] tile, int level, int tx, int ty, float[] residual, out float maxR, out float meanR)
		{
			maxR = 0.0f;
			meanR = 0.0f;
			int tileSize = System.Math.Min(topLevelSize << level, this.tileSize);
			int px = 1 + (tx % 2) * tileSize / 2;
			int py = 1 + (ty % 2) * tileSize / 2;
			int n = this.tileSize + 5;

			for (int j = 0; j <= tileSize + 4; ++j) 
			{
				for (int i = 0; i <= tileSize + 4; ++i) 
				{
					float z;
					if (j%2 == 0) 
					{
						if (i%2 == 0) 
						{
							z = parentTile[i/2+px + (j/2+py)*n];
						} 
						else 
						{
							float z0 = parentTile[i/2+px-1 + (j/2+py)*n];
							float z1 = parentTile[i/2+px + (j/2+py)*n];
							float z2 = parentTile[i/2+px+1 + (j/2+py)*n];
							float z3 = parentTile[i/2+px+2 + (j/2+py)*n];
							z = ((z1+z2)*9.0f-(z0+z3))/16.0f;
						}
					} 
					else 
					{
						if (i%2 == 0) 
						{
							float z0 = parentTile[i/2+px + (j/2-1+py)*n];
							float z1 = parentTile[i/2+px + (j/2+py)*n];
							float z2 = parentTile[i/2+px + (j/2+1+py)*n];
							float z3 = parentTile[i/2+px + (j/2+2+py)*n];
							z = ((z1+z2)*9.0f-(z0+z3))/16.0f;
						} 
						else 
						{
							int di, dj;
							z = 0;
							for (dj = -1; dj <= 2; ++dj) 
							{
								float f = dj == -1 || dj == 2 ? -1.0f/16.0f : 9.0f/16.0f;
								for (di = -1; di <= 2; ++di) 
								{
									float g = di == -1 || di == 2 ? -1.0f/16.0f : 9.0f/16.0f;
									z += f*g*parentTile[i/2+di+px + (j/2+dj+py)*n];
								}
							}
						}
					}

					int off = i + j * n;
					float diff = tile[off] - z;
					residual[off] = diff;
					maxR = System.Math.Max(diff < 0.0f ? -diff : diff, maxR);
					meanR += diff < 0.0f ? -diff : diff;

				}
			}
			meanR = meanR / (n * n);
		}

		void ComputeApproxTile(float[] parentTile, float[] residual, int level, int tx, int ty, float[] tile, out float maxErr)
		{
			maxErr = 0.0f;
			int tileSize = System.Math.Min(topLevelSize << level, this.tileSize);
			int px = 1 + (tx % 2) * tileSize / 2;
			int py = 1 + (ty % 2) * tileSize / 2;
			int n = this.tileSize + 5;

			for (int j = 0; j <= tileSize + 4; ++j) 
			{
				for (int i = 0; i <= tileSize + 4; ++i) 
				{
					float z;
					if (j%2 == 0) 
					{
						if (i%2 == 0) 
						{
							z = parentTile[i/2+px + (j/2+py)*n];
						} 
						else 
						{
							float z0 = parentTile[i/2+px-1 + (j/2+py)*n];
							float z1 = parentTile[i/2+px + (j/2+py)*n];
							float z2 = parentTile[i/2+px+1 + (j/2+py)*n];
							float z3 = parentTile[i/2+px+2 + (j/2+py)*n];
							z = ((z1+z2)*9.0f-(z0+z3))/16.0f;
						}
					} 
					else 
					{
						if (i%2 == 0) 
						{
							float z0 = parentTile[i/2+px + (j/2-1+py)*n];
							float z1 = parentTile[i/2+px + (j/2+py)*n];
							float z2 = parentTile[i/2+px + (j/2+1+py)*n];
							float z3 = parentTile[i/2+px + (j/2+2+py)*n];
							z = ((z1+z2)*9.0f-(z0+z3))/16.0f;
						} 
						else 
						{
							int di, dj;
							z = 0.0f;
							for (dj = -1; dj <= 2; ++dj) 
							{
								float f = dj == -1 || dj == 2 ? -1.0f/16.0f : 9/16.0f;
								for (di = -1; di <= 2; ++di) 
								{
									float g = di == -1 || di == 2 ? -1.0f/16.0f : 9/16.0f;
									z += f*g*parentTile[i/2+di+px + (j/2+dj+py)*n];
								}
							}
						}
					}
					int off = i + j * n;
					float err = tile[off] - (z + residual[off]);
					maxErr = System.Math.Max(err < 0.0f ? -err : err, maxErr);
					tile[off] = z + residual[off];
				}
			}
		}

		void ProduceTile(int level, int tx, int ty, ref long offset, long[] offsets, string file)
		{
			//int nTiles = System.Math.Max(1, (m_baseLevelSize / m_tileSize) >> (m_maxLevel - level));
			int tileSize = System.Math.Min(topLevelSize << level, this.tileSize);

			Debug.WriteLine("Proland.HeightMipmap.ProduceTile - Producing tile " + level + " " + tx + " " + ty);

			if (level == 0) 
			{
				currentLevel = 0;
				Reset(tileSize, tileSize, tileSize);

				for (int j = 0; j <= tileSize + 4; ++j) 
				{
					for (int i = 0; i <= tileSize + 4; ++i) 
					{
						int off = i + j * (tileSize + 5);
						tile[off] = GetTileHeight(i - 2, j - 2) / scale;
					}
				}
			} 
			else 
			{
				LoadTile("Residual", level, tx, ty, tile);
			}

			int tileid;
			if (level < minLevel) {
				tileid = level;
			} 
			else {
				int l = System.Math.Max(level - minLevel, 0);
				tileid = minLevel + tx + ty * (1 << l) + ((1 << (2 * l)) - 1) / 3;
			}
			
			bool isConstant = true;
			for (int i = 0; i < (tileSize + 5) * (tileSize + 5) * 1; ++i) {
				if (tile[i] != 0) {
					isConstant = false;
					break;
				}
			}
			
			if (isConstant && constantTile != -1) 
			{
				Debug.WriteLine("Proland.HeightMipmap.ProduceTile - tile is const (all zeros)");
				offsets[2 * tileid] = offsets[2 * constantTile];
				offsets[2 * tileid + 1] = offsets[2 * constantTile + 1];
			} 
			else 
			{

				byte[] byteArray = new byte[tile.Length * 2];

				for (int i = 0; i < tile.Length; i++) 
				{
					short z = (short)System.Math.Round(tile[i] / m_maxR[level] * (float)short.MaxValue);

					byteArray[2 * i] = (byte)(z & 0xFF);
					byteArray[2 * i + 1] = (byte)(z >> 8);
				}

				using(Stream stream = new FileStream(file, FileMode.Open))
				{
					stream.Seek(offset, SeekOrigin.Begin);
					stream.Write(byteArray, 0, byteArray.Length);
				}

				offsets[2 * tileid] = offset;
				offset += byteArray.Length;
				offsets[2 * tileid + 1] = offset;

			}
			
			if (isConstant && constantTile == -1) {
				constantTile = tileid;
			}
		}


		void ProduceTilesLebeguesOrder(int l, int level, int tx, int ty, ref long offset, long[] offsets, string file)
		{
			if (level < l) 
			{
				ProduceTilesLebeguesOrder(l, level+1, 2*tx, 2*ty, ref offset, offsets, file);
				ProduceTilesLebeguesOrder(l, level+1, 2*tx+1, 2*ty, ref offset, offsets, file);
				ProduceTilesLebeguesOrder(l, level+1, 2*tx, 2*ty+1, ref offset, offsets, file);
				ProduceTilesLebeguesOrder(l, level+1, 2*tx+1, 2*ty+1, ref offset, offsets, file);
			} 
			else {
				ProduceTile(minLevel + level, tx, ty, ref offset, offsets, file);
			}
		}

		public static void SetCube (HeightMipmap hm1, HeightMipmap hm2, HeightMipmap hm3, HeightMipmap hm4, HeightMipmap hm5, HeightMipmap hm6)
		{
			hm1.left = hm5;
			hm1.right = hm3;
			hm1.bottom = hm2;
			hm1.top = hm4;
			hm2.left = hm5;
			hm2.right = hm3;
			hm2.bottom = hm6;
			hm2.top = hm1;
			hm3.left = hm2;
			hm3.right = hm4;
			hm3.bottom = hm6;
			hm3.top = hm1;
			hm4.left = hm3;
			hm4.right = hm5;
			hm4.bottom = hm6;
			hm4.top = hm1;
			hm5.left = hm4;
			hm5.right = hm2;
			hm5.bottom = hm6;
			hm5.top = hm1;
			hm6.left = hm5;
			hm6.right = hm3;
			hm6.bottom = hm4;
			hm6.top = hm2;
			hm1.leftr = 3;
			hm1.rightr = 1;
			hm1.bottomr = 0;
			hm1.topr = 2;
			hm2.leftr = 0;
			hm2.rightr = 0;
			hm2.bottomr = 0;
			hm2.topr = 0;
			hm3.leftr = 0;
			hm3.rightr = 0;
			hm3.bottomr = 1;
			hm3.topr = 3;
			hm4.leftr = 0;
			hm4.rightr = 0;
			hm4.bottomr = 2;
			hm4.topr = 2;
			hm5.leftr = 0;
			hm5.rightr = 0;
			hm5.bottomr = 3;
			hm5.topr = 1;
			hm6.leftr = 1;
			hm6.rightr = 3;
			hm6.bottomr = 2;
			hm6.topr = 0;
		}

	}

}


















