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
//#define SINGLE_FILE

using proland;
using System.Collections.Generic;
using System.IO;
using System;

namespace Sxta.Proland.Terrain
{
    /// <summary>
    /// A TileProducer to load elevation residuals from disk to CPU memory.
    /// </summary>
    public class ResidualProducer : TileProducer
    {
        /// <summary>
        /// The name of the file containing the residual tiles to load.
        /// </summary>
        private string name;

        /// <summary>
        /// The size of the tiles whose level (on disk) is at least minLevel.
        /// This size does not include the borders.A tile contains
        /// (tileSize+5)*(tileSize+5) samples.
        /// </summary>
        private int tileSize;

        /// <summary>
        /// The level of the root of the tile pyramid managed by this producer
        ///  in the global set of tile pyramids describing a terrain.
        /// </summary>
        private int rootLevel;

        /// <summary>
        /// The stored tiles level that must be considered as the root level in
        ///  this producer.Must be less than or equal to minLevel.
        /// </summary>
        private int deltaLevel;

        /// <summary>
        /// The logical x coordinate of the root of the tile pyramid managed
        /// by this producer in the global set of tile pyramids describing a
        /// terrain.
        /// </summary>
        private int rootTx;

        /// <summary>
        /// The logical y coordinate of the root of the tile pyramid managed
		/// by this producer in the global set of tile pyramids describing a
		/// terrain.
        /// </summary>
        private int rootTy;

        /// <summary>
        /// The stored tile level of the first tile of size tileSize.
        /// </summary>
        private int minLevel;

        /// <summary>
        /// The maximum level of the stored tiles on disk (inclusive, and
        /// relatively to rootLevel).
        /// </summary>
        private int maxLevel;

        /// <summary>
        ///  A scaling factor to be applied to all residuals read from disk.
        /// </summary>
        private float scale;

        /// <summary>
        ///  The offsets of each tile on disk, relatively to offset, for each
        /// tile id(see GetTileId).
        /// </summary>
        private long[] offsets;

        private float[] maxR;

        /// <summary>
        /// A mutex used to serializes accesses to the file storing the tiles.
        /// </summary>
        private object mutex;

        /// <summary>
        /// The file storing the residual tiles on disk.
        /// </summary>
        private string tileFile;

        /// <summary>
        /// The "subproducers" providing more details in some regions.
        /// Each subproducer can have its own subproducers, recursively.
        /// </summary>
        private List<ResidualProducer> producers = new List<ResidualProducer>();

        /// <summary>
        ///  A key to store thread specific buffers used to produce the tiles.
        /// </summary>
        private static object key;

        /// <summary>
        /// Creates a new ResidualProducer.
        /// </summary>
        /// <param name="cache">the cache to store the produced tiles. The underlying
        ///      storage must be a CPUTileStorage of float type.The size
        ///      of tiles in this storage size must be equal to the size of the
        ///      tiles stored on disk, borders included.</param>
        /// <param name="name">the name of the file containing the tiles to load.</param>
        /// <param name="deltaLevel">the stored tiles level that must be considered as
        ///      the root level in this %producer.Must be less than or equal to
        ///      getMinLevel().</param>
        /// <param name="zscale"> a vertical scaling factor to be applied to all elevations.
        /// </param>
        public ResidualProducer(TileCache cache, string name, int deltaLevel, float zscale) :
    base("ResidualProducer", "CreateResidualTile")
        {
            init(cache, name, deltaLevel, zscale);
        }
        void init(TileCache cache, string name, int deltaLevel, float zscale)
        {
            base.init(cache, false);
            this.name = name;

#if TODO
            if (string.IsNullOrWhiteSpace(name) ) {
        this.tileFile = null;
        this.minLevel = 0;
        this.maxLevel = 32;
        this.rootLevel = 0;
        this.deltaLevel = 0;
        this.rootTx = 0;
        this.rootTy = 0;
        this.scale = 1.0f;
    } else {
        fopen(&tileFile, name, "rb");
        if (tileFile == null) {
            if (Logger::ERROR_LOGGER != NULL) {
                Logger::ERROR_LOGGER.log("DEM", "Cannot open file '" + name + "'");
            }
maxLevel = -1;
            scale = 1.0;
        } else {
            fread(&minLevel, sizeof(int), 1, tileFile);
            fread(&maxLevel, sizeof(int), 1, tileFile);
            fread(&tileSize, sizeof(int), 1, tileFile);
            fread(&rootLevel, sizeof(int), 1, tileFile);
            fread(&rootTx, sizeof(int), 1, tileFile);
            fread(&rootTy, sizeof(int), 1, tileFile);
            fread(&scale, sizeof(float), 1, tileFile);
        }

        this.deltaLevel = rootLevel == 0 ? deltaLevel : 0;
        scale = scale * zscale;

        int ntiles = minLevel + ((1 << (max(maxLevel - minLevel, 0) * 2 + 2)) - 1) / 3;
        header = sizeof(float) + sizeof(int) * (6 + ntiles * 2);
        offsets = new unsigned int[ntiles * 2];
        if (tileFile != NULL) {
            fread(offsets, sizeof(unsigned int) * ntiles * 2, 1, tileFile);
#if !SINGLE_FILE
            fclose(tileFile);
            tileFile = NULL;
#endif
        }

                if (key == NULL) {
            key = new pthread_key_t;
            pthread_key_create((pthread_key_t*)key, residualDelete);
        }

        assert(tileSize + 5 < MAX_TILE_SIZE);
        assert(deltaLevel <= minLevel);

#if !SINGLE_FILE
        mutex = new pthread_mutex_t;
        pthread_mutex_init((pthread_mutex_t*) mutex, NULL);
#endif
        }
#endif
            throw new NotImplementedException();
        }

        public override int getBorder()
        {
            return 2;
        }

        public int GetMinLevel()
        {
            return minLevel;
        }

        public int GetDeltaLevel()
        {
            return deltaLevel;
        }

        public void addProducer(ResidualProducer p)
        {
            producers.Add(p);
        }

        public override bool hasTile(int level, int tx, int ty)
        {
            int l = level + deltaLevel - rootLevel;
            if (l >= 0 && (tx >> l) == rootTx && (ty >> l) == rootTy)
            {
                if (l <= maxLevel)
                    return true;
            }
            return false;
        }

        //public override bool doCreateTile(int level, int tx, int ty, List<TileStorage.Slot> slot)
        protected internal override bool doCreateTile(int level, int tx, int ty, TileStorage.Slot slot)
        {
#if TODO
            int l = level + deltaLevel - rootLevel;
            if ((l >= 0 && (tx >> l) == rootTx && (ty >> l) == rootTy))
            {

            }
            else
            {
                return;
            }

            level = l;
            tx = tx - (rootTx << level);
            ty = ty - (rootTy << level);

            CPUTileStorage.CPUSlot<float> cpuSlot = slot[0] as CPUTileStorage.CPUSlot<float>;

            cpuSlot.ClearData();

            ReadTile(level, tx, ty, null, cpuSlot.GetData());

            base.DoCreateTile(level, tx, ty, slot);
#endif
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the size of tiles of the given level (without borders).
        /// This size can vary with the level if #getMinLevel is not 0 (see
        /// \ref sec-residual).
        /// </summary>
        /// <param name="level">a quadtree level.</param>
        /// <returns></returns>
        private int getTileSize(int level)
        {
            throw new NotImplementedException();
        }

        /* 
         * Returns the id of the given tile. This id is used to find the offset
         * the tile data on disk, using #offsets.
         *
         * @param level the level of the tile.
         * @param tx the logical x coordinate of the tile.
         * @param ty the logical y coordinate of the tile.
         * @return the id of the given tile.
         */
        private int getTileId(int level, int tx, int ty)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads compressed tile data on disk, uncompress it and scale it with
        /// scale
        /// </summary>
        /// <param name="level">the level of the tile.</param>
        /// <param name="tx">the logical x coordinate of the tile.</param>
        /// <param name="ty">the logical y coordinate of the tile.</param>
        /// <param name="compressedData">where the compressed tile data must be stored.</param>
        /// <param name="uncompressedData">where the uncompressed data must be stored.</param>
        /// <param name="tile">an optional tile to be added to the result. Maybe NULL.</param>
        /// <param name="result">where the uncompressed data, scaled by #scale and
        ///      optionally offseted by 'tile', must be stored.</param>
        private void readTile(int level, int tx, int ty,
                            byte[] compressedData, byte[] uncompressedData,
                            float[] tile, float[] result)
        {
            throw new NotImplementedException();
        }


#if DELETEME
        protected void Start()
        {
            base.Start();

            CPUTileStorage storage = GetCache().GetStorage(0) as CPUTileStorage;

            if (storage == null)
            {
                throw new InvalidStorageException("Storage must be a CPUTileStorage");
            }

            if (storage.GetChannels() != 1)
            {
                throw new InvalidStorageException("Storage channels must be 1");
            }

            if (storage.GetDataType() != CPUTileStorage.DATA_TYPE.FLOAT)
            {
                throw new InvalidStorageException("Storage data type must be float");
            }

            byte[] data = new byte[7 * 4];

            using (Stream stream = new FileStream(Application.dataPath + name, FileMode.Open))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, data.Length);
            }

            minLevel = System.BitConverter.ToInt32(data, 0);
            maxLevel = System.BitConverter.ToInt32(data, 4);
            tileSize = System.BitConverter.ToInt32(data, 8);
            rootLevel = System.BitConverter.ToInt32(data, 12);
            rootTx = System.BitConverter.ToInt32(data, 16);
            rootTy = System.BitConverter.ToInt32(data, 20);
            scale = System.BitConverter.ToSingle(data, 24);

            maxR = new float[maxLevel + 1];

            data = new byte[maxR.Length * 4];

            using (Stream stream = new FileStream(Application.dataPath + name, FileMode.Open))
            {
                stream.Seek(7 * 4, SeekOrigin.Begin);
                stream.Read(data, 0, data.Length);
            }

            for (int i = 0; i < maxR.Length; i++)
            {
                maxR[i] = System.BitConverter.ToSingle(data, 4 * i);
                //Debug.Log("Proland::ResidualProducer::Start - maxR at level " + i + " = " + m_maxR[i]);
            }

            //			Debug.Log("Proland::ResidualProducer::Start - min level = " + m_minLevel);
            //			Debug.Log("Proland::ResidualProducer::Start - max level = " + m_maxLevel);
            //			Debug.Log("Proland::ResidualProducer::Start - tile size = " + m_tileSize);
            //			Debug.Log("Proland::ResidualProducer::Start - root level = " + m_rootLevel);
            //			Debug.Log("Proland::ResidualProducer::Start - root tx = " + m_rootTx);
            //			Debug.Log("Proland::ResidualProducer::Start - root ty = " + m_rootTy);
            //			Debug.Log("Proland::ResidualProducer::Start - scale = " + m_scale);

            deltaLevel = rootLevel == 0 ? deltaLevel : 0;
            scale = scale * m_zscale;

            if (deltaLevel > minLevel)
                throw new InvalidParameterException("delta level can not be greater than min level");

            int ntiles = minLevel + ((1 << (System.Math.Max(maxLevel - minLevel, 0) * 2 + 2)) - 1) / 3;
            offsets = new long[ntiles * 2];

            data = new byte[ntiles * 2 * 8];

            using (Stream stream = new FileStream(Application.dataPath + name, FileMode.Open))
            {
                stream.Seek((7 + maxR.Length) * 4, SeekOrigin.Begin);
                stream.Read(data, 0, data.Length);
            }

            for (int i = 0; i < ntiles * 2; i++)
            {
                offsets[i] = System.BitConverter.ToInt64(data, 8 * i);
                //Debug.Log("Proland::ResidualProducer::Start - offset = " + m_offsets[i]);
            }

            //Debug.Log("Proland::ResidualProducer::Start - Delta level = " + m_deltaLevel);
            //Debug.Log("Proland::ResidualProducer::Start - Num offsets = " + m_offsets.Length);

        }

        int TileSize(int level)
        {
            return level < minLevel ? tileSize >> (minLevel - level) : tileSize;
        }

        int GetTileId(int level, int tx, int ty)
        {
            if (level < minLevel)
            {
                return level;
            }
            else
            {
                int l = System.Math.Max(level - minLevel, 0);
                return minLevel + tx + ty * (1 << l) + ((1 << (2 * l)) - 1) / 3;
            }
        }

       
        void ReadTile(int level, int tx, int ty, float[] tile, float[] result)
        {
            int tilesize = TileSize(level) + 5;

            int tileid = GetTileId(level, tx, ty);
            long fsize = offsets[2 * tileid + 1] - offsets[2 * tileid];

            if (fsize > (tileSize + 5) * (tileSize + 5) * 2)
                throw new ArgumentException("file size of tile is larger than actual tile size");

            byte[] data = new byte[fsize];

            using (Stream stream = new FileStream(Application.dataPath + name, FileMode.Open))
            {
                stream.Seek(offsets[2 * tileid], SeekOrigin.Begin);
                stream.Read(data, 0, data.Length);
            }


            for (int j = 0; j < tilesize; ++j)
            {
                for (int i = 0; i < tilesize; ++i)
                {
                    int off = 2 * (i + j * tilesize);
                    short z = (short)((short)data[off + 1] << 8 | (short)data[off]);
                    result[i + j * (tileSize + 5)] = (float)z / (float)short.MaxValue * maxR[level] * scale;
                }
            }

        }
#endif
    }

}




























