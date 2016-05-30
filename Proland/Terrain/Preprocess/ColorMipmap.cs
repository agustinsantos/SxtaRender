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
using System.IO;
using System;
using log4net;
using System.Reflection;

namespace Sxta.Proland.Terrain
{
    public class ColorMipmap : AbstractTileCache
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        const int RESIDUAL_STEPS = 2;
        public const int RGB_JPEG_QUALITY = 90;

        public ColorMipmap left;
        public ColorMipmap right;
        public ColorMipmap bottom;
        public ColorMipmap top;
        public int leftr;
        public int rightr;
        public int bottomr;
        public int topr;

        protected ColorFunction colorf;

        protected int baseLevelSize;

        protected int tileSize;

        protected int tileWidth;

        protected int border;

        protected int channels;

        protected ColorTransformDelegate r2l;

        protected ColorTransformDelegate l2r;

        protected int maxLevel;

        protected string cache;

        protected byte[] tile;

        protected int currentLevel;

        protected Dictionary<int, int> constantTileIds;
        public ColorMipmap(ColorFunction colorf, int baseLevelSize, int tileSize, int border, int channels, ColorTransformDelegate rgbToLinear, ColorTransformDelegate linearToRgb, string tmpFolder) :
        base(baseLevelSize, baseLevelSize, tileSize, channels, 200)
        {

            this.colorf = colorf;
            this.baseLevelSize = baseLevelSize;
            this.tileSize = tileSize;
            this.border = System.Math.Max(0, border);
            this.channels = MathHelper.Clamp(channels, 1, 4);
            this.r2l = rgbToLinear;
            this.l2r = linearToRgb;
            this.cache = tmpFolder;
            constantTileIds = new Dictionary<int, int>();

            maxLevel = 0;
            int size = this.baseLevelSize;
            while (size > this.tileSize)
            {
                maxLevel += 1;
                size /= 2;
            }

            tile = new byte[(this.tileSize + 2 * this.border) * (this.tileSize + 2 * this.border) * this.channels];

            left = null;
            right = null;
            bottom = null;
            top = null;

            log.Debug("Poland.ColorMipmap.ColorMipmap - base level size = " + this.baseLevelSize);
            log.Debug("Poland.ColorMipmap.ColorMipmap - tile size = " + this.tileSize);
            log.Debug("Poland.ColorMipmap.ColorMipmap - border = " + this.border);
            log.Debug("Poland.ColorMipmap.ColorMipmap - max level = " + maxLevel);
            log.Debug("Poland.ColorMipmap.ColorMipmap - channels = " + this.channels);
        }

        public void Compute()
        {
            BuildBaseLevelTiles();
            ComputeMipmap();
        }

        public void ComputeMipmap()
        {
            for (int level = maxLevel - 1; level >= 0; --level)
            {
                BuildMipmapLevel(level);
            }
        }

        public void Generate(int rootLevel, int rootTx, int rootTy, bool dxt, bool jpg, int jpg_quality, string file)
        {

            File.Delete(file);

            int nTiles = ((1 << (maxLevel * 2 + 2)) - 1) / 3;
            log.Debug("Proland.ColorMipmap.Generate - num tiles = " + nTiles);

            long[] offsets = new long[nTiles * 2];
            byte[] byteArray = new byte[(7 * 4) + (offsets.Length * 8)];
            long offset = byteArray.Length;

            using (Stream stream = new FileStream(file, FileMode.Create))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.Write(byteArray, 0, byteArray.Length);
            }

            for (int l = 0; l <= maxLevel; ++l)
            {
                ProduceTilesLebeguesOrder(l, 0, 0, 0, ref offset, offsets, file);
            }

            System.Buffer.BlockCopy(new int[] { maxLevel }, 0, byteArray, 0, 4);
            System.Buffer.BlockCopy(new int[] { tileSize }, 0, byteArray, 4, 4);
            System.Buffer.BlockCopy(new int[] { channels }, 0, byteArray, 8, 4);
            System.Buffer.BlockCopy(new int[] { border }, 0, byteArray, 12, 4);
            System.Buffer.BlockCopy(new int[] { rootLevel }, 0, byteArray, 16, 4);
            System.Buffer.BlockCopy(new int[] { rootTx }, 0, byteArray, 20, 4);
            System.Buffer.BlockCopy(new int[] { rootTy }, 0, byteArray, 24, 4);
            System.Buffer.BlockCopy(offsets, 0, byteArray, 28, 8 * offsets.Length);

            using (Stream stream = new FileStream(file, FileMode.Open))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.Write(byteArray, 0, byteArray.Length);
            }

            constantTileIds.Clear();

            log.Debug("Proland.ColorMipmap.Generate - Saved file = " + file);
        }

        public void GenerateResiduals(bool jpg, int jpg_quality, string fin, string fout)
        {
            throw new NotImplementedException();
        }

        public void ReorderResiduals(string fin, string fout)
        {
            throw new NotImplementedException();
        }


        void SaveTile(string name, int level, int tx, int ty, byte[] tile)
        {
            string fileName = cache + "/" + name + "-" + level + "-" + tx + "-" + ty + ".raw";
            System.IO.File.WriteAllBytes(fileName, tile);
        }

        void LoadTile(string name, int level, int tx, int ty, byte[] tile)
        {
            string fileName = cache + "/" + name + "-" + level + "-" + tx + "-" + ty + ".raw";

            FileInfo fi = new FileInfo(fileName);
            if (fi == null) throw new FileNotFoundException("Could not read tile " + fileName);

            FileStream fs = fi.OpenRead();
            fs.Read(tile, 0, (int)fi.Length);
            fs.Close();
        }

        protected override float[] ReadTile(int tx, int ty)
        {
            float[] tile = new float[(tileSize + 2 * border) * (tileSize + 2 * border) * channels];
            byte[] data = new byte[(tileSize + 2 * border) * (tileSize + 2 * border) * channels];

            LoadTile("Base", currentLevel, tx, ty, data);

            for (int i = 0; i < tile.Length; i++)
                tile[i] = (float)data[i];

            return tile;
        }

        void BuildBaseLevelTiles()
        {

            int nTiles = baseLevelSize / tileSize;

            log.Debug("Proland.ColorMipmap.BuildBaseLevelTiles - Build mipmap level " + maxLevel);

            for (int ty = 0; ty < nTiles; ++ty)
            {
                for (int tx = 0; tx < nTiles; ++tx)
                {
                    int off = 0;
                    for (int j = -border; j < tileSize + border; ++j)
                    {
                        for (int i = -border; i < tileSize + border; ++i)
                        {
                            Vector4f c = colorf.GetColor(tx * tileSize + i, ty * tileSize + j) * 255.0f;
                            tile[off++] = (byte)System.Math.Round(c.X);

                            if (channels > 1)
                            {
                                tile[off++] = (byte)System.Math.Round(c.Y);
                            }
                            if (channels > 2)
                            {
                                tile[off++] = (byte)System.Math.Round(c.Z);
                            }
                            if (channels > 3)
                            {
                                tile[off++] = (byte)System.Math.Round(c.W);
                            }
                        }
                    }

                    SaveTile("Base", maxLevel, tx, ty, tile);
                }
            }
        }

        void BuildMipmapLevel(int level)
        {
            int nTiles = 1 << level;

            log.Debug("Proland.ColorMipmap.BuildMipmapLevel - Build mipmap level = " + level);

            currentLevel = level + 1;
            Reset(tileSize << currentLevel, tileSize << currentLevel, tileSize);

            for (int ty = 0; ty < nTiles; ++ty)
            {
                for (int tx = 0; tx < nTiles; ++tx)
                {

                    int off = 0;
                    for (int j = -border; j < tileSize + border; ++j)
                    {
                        for (int i = -border; i < tileSize + border; ++i)
                        {

                            int ix = 2 * (tx * tileSize + i);
                            int iy = 2 * (ty * tileSize + j);

                            Vector4f c1 = GetTileColor(ix, iy);
                            Vector4f c2 = GetTileColor(ix + 1, iy);
                            Vector4f c3 = GetTileColor(ix, iy + 1);
                            Vector4f c4 = GetTileColor(ix + 1, iy + 1);

                            tile[off++] = (byte)System.Math.Round((c1.X + c2.X + c3.X + c4.X) / 4.0f);

                            if (channels > 1)
                            {
                                tile[off++] = (byte)System.Math.Round((c1.Y + c2.Y + c3.Y + c4.Y) / 4.0f);
                            }

                            if (channels > 2)
                            {
                                tile[off++] = (byte)System.Math.Round((c1.Z + c2.Z + c3.Z + c4.Z) / 4.0f);
                            }

                            if (channels > 3)
                            {
                                float w1 = System.Math.Max(2.0f * c1.W - 255.0f, 0.0f);
                                float n1 = System.Math.Max(255.0f - 2.0f * c1.W, 0.0f);
                                float w2 = System.Math.Max(2.0f * c2.W - 255.0f, 0.0f);
                                float n2 = System.Math.Max(255.0f - 2.0f * c2.W, 0.0f);
                                float w3 = System.Math.Max(2.0f * c3.W - 255.0f, 0.0f);
                                float n3 = System.Math.Max(255.0f - 2.0f * c3.W, 0.0f);
                                float w4 = System.Math.Max(2.0f * c4.W - 255.0f, 0.0f);
                                float n4 = System.Math.Max(255.0f - 2.0f * c4.W, 0.0f);
                                byte w = (byte)System.Math.Round((w1 + w2 + w3 + w4) / 4.0f);
                                byte n = (byte)System.Math.Round((n1 + n2 + n3 + n4) / 4.0f);
                                tile[off++] = (byte)(127 + w / 2 - n / 2);
                            }
                        }
                    }

                    SaveTile("Base", level, tx, ty, tile);
                }
            }

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

        public void ProduceRawTile(int level, int tx, int ty)
        {
            LoadTile("Base", level, tx, ty, tile);
        }

        void ProduceTile(int level, int tx, int ty)
        {
            int nTiles = 1 << level;
            int tileWidth = tileSize + 2 * border;

            ProduceRawTile(level, tx, ty);

            if (tx == 0 && border > 0 && left != null)
            {
                int txp, typ;
                Rotation(leftr, nTiles, nTiles - 1, ty, out txp, out typ);
                left.ProduceRawTile(level, txp, typ);

                for (int y = 0; y < tileWidth; ++y)
                {
                    for (int x = 0; x < border; ++x)
                    {
                        int xp, yp;
                        Rotation(leftr, tileWidth, tileSize - x, y, out xp, out yp);

                        for (int c = 0; c < channels; ++c)
                        {
                            tile[(x + y * tileWidth) * channels + c] = left.tile[(xp + yp * tileWidth) * channels + c];
                        }
                    }
                }
            }

            if (tx == nTiles - 1 && border > 0 && right != null)
            {
                int txp, typ;
                Rotation(rightr, nTiles, 0, ty, out txp, out typ);
                right.ProduceRawTile(level, txp, typ);

                for (int y = 0; y < tileWidth; ++y)
                {
                    for (int x = tileSize + border; x < tileWidth; ++x)
                    {
                        int xp, yp;
                        Rotation(rightr, tileWidth, x - tileSize, y, out xp, out yp);

                        for (int c = 0; c < channels; ++c)
                        {
                            tile[(x + y * tileWidth) * channels + c] = right.tile[(xp + yp * tileWidth) * channels + c];
                        }
                    }
                }
            }

            if (ty == 0 && border > 0 && bottom != null)
            {
                int txp, typ;
                Rotation(bottomr, nTiles, tx, nTiles - 1, out txp, out typ);
                bottom.ProduceRawTile(level, txp, typ);

                for (int y = 0; y < border; ++y)
                {
                    for (int x = 0; x < tileWidth; ++x)
                    {
                        int xp, yp;
                        Rotation(bottomr, tileWidth, x, tileSize - y, out xp, out yp);

                        for (int c = 0; c < channels; ++c)
                        {
                            tile[(x + y * tileWidth) * channels + c] = bottom.tile[(xp + yp * tileWidth) * channels + c];
                        }
                    }
                }
            }

            if (ty == nTiles - 1 && border > 0 && top != null)
            {
                int txp, typ;
                Rotation(topr, nTiles, tx, 0, out txp, out typ);
                top.ProduceRawTile(level, txp, typ);

                for (int y = tileSize + border; y < tileWidth; ++y)
                {
                    for (int x = 0; x < tileWidth; ++x)
                    {
                        int xp, yp;
                        Rotation(topr, tileWidth, x, y - tileSize, out xp, out yp);

                        for (int c = 0; c < channels; ++c)
                        {
                            tile[(x + y * tileWidth) * channels + c] = top.tile[(xp + yp * tileWidth) * channels + c];
                        }
                    }
                }
            }

            if (tx == 0 && ty == 0 && border > 0 && left != null && bottom != null)
            {
                for (int c = 0; c < channels; ++c)
                {
                    int x1 = border;
                    int y1 = border;
                    int x2;
                    int y2;
                    int x3;
                    int y3;

                    Rotation(leftr, tileWidth, tileWidth - 1 - border, border, out x2, out y2);
                    Rotation(bottomr, tileWidth, border, tileWidth - 1 - border, out x3, out y3);

                    int corner1 = tile[(x1 + y1 * tileWidth) * channels + c];
                    int corner2 = left.tile[(x2 + y2 * tileWidth) * channels + c];
                    int corner3 = bottom.tile[(x3 + y3 * tileWidth) * channels + c];
                    int corner = (corner1 + corner2 + corner3) / 3;

                    for (int y = 0; y < 2 * border; ++y)
                    {
                        for (int x = 0; x < 2 * border; ++x)
                        {
                            tile[(x + y * tileWidth) * channels + c] = (byte)corner;
                        }
                    }
                }
            }

            if (tx == nTiles - 1 && ty == 0 && border > 0 && right != null && bottom != null)
            {
                for (int c = 0; c < channels; ++c)
                {
                    int x1 = tileWidth - 1 - border;
                    int y1 = border;
                    int x2;
                    int y2;
                    int x3;
                    int y3;

                    Rotation(rightr, tileWidth, border, border, out x2, out y2);
                    Rotation(bottomr, tileWidth, tileWidth - 1 - border, tileWidth - 1 - border, out x3, out y3);

                    int corner1 = tile[(x1 + y1 * tileWidth) * channels + c];
                    int corner2 = right.tile[(x2 + y2 * tileWidth) * channels + c];
                    int corner3 = bottom.tile[(x3 + y3 * tileWidth) * channels + c];
                    int corner = (corner1 + corner2 + corner3) / 3;

                    for (int y = 0; y < 2 * border; ++y)
                    {
                        for (int x = tileSize; x < tileWidth; ++x)
                        {
                            tile[(x + y * tileWidth) * channels + c] = (byte)corner;
                        }
                    }
                }
            }

            if (tx == 0 && ty == nTiles - 1 && border > 0 && left != null && top != null)
            {
                for (int c = 0; c < channels; ++c)
                {
                    int x1 = border;
                    int y1 = tileWidth - 1 - border;
                    int x2;
                    int y2;
                    int x3;
                    int y3;

                    Rotation(leftr, tileWidth, tileWidth - 1 - border, tileWidth - 1 - border, out x2, out y2);
                    Rotation(topr, tileWidth, border, border, out x3, out y3);

                    int corner1 = tile[(x1 + y1 * tileWidth) * channels + c];
                    int corner2 = left.tile[(x2 + y2 * tileWidth) * channels + c];
                    int corner3 = top.tile[(x3 + y3 * tileWidth) * channels + c];
                    int corner = (corner1 + corner2 + corner3) / 3;

                    for (int y = tileSize; y < tileWidth; ++y)
                    {
                        for (int x = 0; x < 2 * border; ++x)
                        {
                            tile[(x + y * tileWidth) * channels + c] = (byte)corner;
                        }
                    }
                }
            }

            if (tx == nTiles - 1 && ty == nTiles - 1 && border > 0 && right != null && top != null)
            {
                for (int c = 0; c < channels; ++c)
                {
                    int x1 = tileWidth - 1 - border;
                    int y1 = tileWidth - 1 - border;
                    int x2;
                    int y2;
                    int x3;
                    int y3;

                    Rotation(rightr, tileWidth, border, tileWidth - 1 - border, out x2, out y2);
                    Rotation(topr, tileWidth, tileWidth - 1 - border, border, out x3, out y3);

                    int corner1 = tile[(x1 + y1 * tileWidth) * channels + c];
                    int corner2 = right.tile[(x2 + y2 * tileWidth) * channels + c];
                    int corner3 = top.tile[(x3 + y3 * tileWidth) * channels + c];
                    int corner = (corner1 + corner2 + corner3) / 3;

                    for (int y = tileSize; y < tileWidth; ++y)
                    {
                        for (int x = tileSize; x < tileWidth; ++x)
                        {
                            tile[(x + y * tileWidth) * channels + c] = (byte)corner;
                        }
                    }
                }
            }

        }

        void ProduceTile(int level, int tx, int ty, ref long offset, long[] offsets, string file)
        {
            log.Debug("Proland.ColorMipmap.ProduceTile - Producing tile " + level + " " + tx + " " + ty);

            ProduceTile(level, tx, ty);

            int tileid = tx + ty * (1 << level) + ((1 << (2 * level)) - 1) / 3;

            bool isConstant = true;
            int constantValue = tile[0];

            for (int i = 1; i < (tileSize + 2 * border) * (tileSize + 2 * border); ++i)
            {
                if (tile[i] != tile[i - 1])
                {
                    isConstant = false;
                    break;
                }
            }

            if (isConstant && constantTileIds.ContainsKey(constantValue))
            {
                log.Debug("Proland.ColorMipmap.ProduceTile - tile is const (all same value)");
                int constantId = constantTileIds[constantValue];
                offsets[2 * tileid] = offsets[2 * constantId];
                offsets[2 * tileid + 1] = offsets[2 * constantId + 1];
            }
            else
            {

                using (Stream stream = new FileStream(file, FileMode.Open))
                {
                    stream.Seek(offset, SeekOrigin.Begin);
                    stream.Write(tile, 0, tile.Length);
                }

                offsets[2 * tileid] = offset;
                offset += tile.Length;
                offsets[2 * tileid + 1] = offset;
            }

            if (isConstant && !constantTileIds.ContainsKey(constantValue))
            {
                constantTileIds.Add(constantValue, tileid);
            }
        }

        void ProduceTilesLebeguesOrder(int l, int level, int tx, int ty, ref long offset, long[] offsets, string file)
        {
            if (level < l)
            {
                ProduceTilesLebeguesOrder(l, level + 1, 2 * tx, 2 * ty, ref offset, offsets, file);
                ProduceTilesLebeguesOrder(l, level + 1, 2 * tx + 1, 2 * ty, ref offset, offsets, file);
                ProduceTilesLebeguesOrder(l, level + 1, 2 * tx, 2 * ty + 1, ref offset, offsets, file);
                ProduceTilesLebeguesOrder(l, level + 1, 2 * tx + 1, 2 * ty + 1, ref offset, offsets, file);
            }
            else
            {
                ProduceTile(level, tx, ty, ref offset, offsets, file);
            }
        }

        public static void SetCube(ColorMipmap hm1, ColorMipmap hm2, ColorMipmap hm3, ColorMipmap hm4, ColorMipmap hm5, ColorMipmap hm6)
        {
            hm1.left = hm5; hm1.right = hm3; hm1.bottom = hm2; hm1.top = hm4;
            hm2.left = hm5; hm2.right = hm3; hm2.bottom = hm6; hm2.top = hm1;
            hm3.left = hm2; hm3.right = hm4; hm3.bottom = hm6; hm3.top = hm1;
            hm4.left = hm3; hm4.right = hm5; hm4.bottom = hm6; hm4.top = hm1;
            hm5.left = hm4; hm5.right = hm2; hm5.bottom = hm6; hm5.top = hm1;
            hm6.left = hm5; hm6.right = hm3; hm6.bottom = hm4; hm6.top = hm2;
            hm1.leftr = 3; hm1.rightr = 1; hm1.bottomr = 0; hm1.topr = 2;
            hm2.leftr = 0; hm2.rightr = 0; hm2.bottomr = 0; hm2.topr = 0;
            hm3.leftr = 0; hm3.rightr = 0; hm3.bottomr = 1; hm3.topr = 3;
            hm4.leftr = 0; hm4.rightr = 0; hm4.bottomr = 2; hm4.topr = 2;
            hm5.leftr = 0; hm5.rightr = 0; hm5.bottomr = 3; hm5.topr = 1;
            hm6.leftr = 1; hm6.rightr = 3; hm6.bottomr = 2; hm6.topr = 0;
        }

    }
}




































