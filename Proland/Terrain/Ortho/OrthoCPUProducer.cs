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
using Sxta.Render.Resources;
using System;
using System.IO;

namespace Sxta.Proland.Terrain
{
    /// <summary>
    /// A TileProducer to load any kind of texture tile from disk to CPU memory.
    /// </summary>
	public class OrthoCPUProducer : TileProducer, ISwappable<OrthoCPUProducer>
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /**
         * Creates a new OrthoCPUProducer.
         *
         * @param cache the cache to store the produced tiles. The underlying
         *      storage must be a CPUTileStorage of unsigned char type. The size
         *      of tiles in this storage size must be equal to the size of the
         *      tiles stored on disk, borders included.
         * @param name the name of the file containing the tiles to load.
         */
        public OrthoCPUProducer(TileCache cache, string name) : base("OrthoCPUProducer", "CreateOrthoCPUTile")
        {
            init(cache, name);
        }

        /**
         * Deletes this OrthoCPUProducer.
         */
        //public virtual ~OrthoCPUProducer();

        public override int getBorder()
        {
            return border;
        }


        public override bool hasTile(int level, int tx, int ty)
        {
            return level <= maxLevel;
        }


        /**
         * Returns true if the produced tiles are compressed in DXT format.
         */
        public bool isCompressed()
        {
            return dxt;
        }



        /**
         * Creates an uninitialized OrthoCPUProducer.
         */
        protected OrthoCPUProducer() : base("OrthoCPUProducer", "CreateOrthoCPUTile")
        {
        }

        /**
         * Initializes this OrthoCPUProducer.
         *
         * @param cache the cache to store the produced tiles. The underlying
         *      storage must be a CPUTileStorage of unsigned char type. The size
         *      of tiles in this storage size must be equal to the size of the
         *      tiles stored on disk, borders included.
         * @param name the name of the file containing the tiles to load.
         */
        protected virtual void init(TileCache cache, string name)
        {
#if TODO
            base.init(cache, false);
            this.name = name;
            if (string.IsNullOrWhiteSpace(name))
            {
                maxLevel = 1;
                tileSize = 0;
                dxt = false;
                border = 2;
            }
            else
            {
                fopen(&tileFile, name, "rb");
                if (tileFile == null)
                {
                    if (Logger.ERROR_LOGGER != null)
                    {
                        Logger.ERROR_LOGGER.log("ORTHO", "Cannot open file '" + string(name) + "'");
                    }
                    maxLevel = -1;
                }
                else
                {
                    int root;
                    int tx;
                    int ty;
                    int flags;
                    fread(&maxLevel, sizeof(int), 1, tileFile);
                    fread(&tileSize, sizeof(int), 1, tileFile);
                    fread(&channels, sizeof(int), 1, tileFile);
                    fread(&root, sizeof(int), 1, tileFile);
                    fread(&tx, sizeof(int), 1, tileFile);
                    fread(&ty, sizeof(int), 1, tileFile);
                    fread(&flags, sizeof(int), 1, tileFile);
                    dxt = (flags & 1) != 0;
                    border = (flags & 2) != 0 ? 0 : 2;
                }

                int ntiles = ((1 << (maxLevel * 2 + 2)) - 1) / 3;
                header = 7 * sizeof(int) + 2 * ntiles * sizeof(long);
                offsets = new long long[2 * ntiles];
                if (tileFile != null)
                {
                    fread(offsets, sizeof(long) * ntiles * 2, 1, tileFile);
#if !SINGLE_FILE
                    fclose(tileFile);
                    tileFile = null;
#endif
                }

                if (key == null)
                {
                    key = new pthread_key_t;
                    pthread_key_create((pthread_key_t*)key, orthoCPUDelete);
                }

                Debug.Assert(tileSize + 2 * border < MAX_TILE_SIZE);

#if !SINGLE_FILE
                mutex = new pthread_mutex_t;
                pthread_mutex_init((pthread_mutex_t*)mutex, null);
#endif
            }
#endif
            throw new NotImplementedException();
        }


        protected internal override bool doCreateTile(int level, int tx, int ty, TileStorage.Slot data)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("CPU tile " + getId() + " " + level + " " + tx + " " + ty);
            }
#if TODO
            CPUTileStorage<byte>.CPUSlot cpuData = (CPUTileStorage<byte>.CPUSlot)(data);
            Debug.Assert(cpuData != null);

            int tileid = getTileId(level, tx, ty);

            if (string.IsNullOrEmpty( name))
            {
                for (int i = 0; i < cpuData.size; i++)
                {
                    cpuData.data[i] = 0;
                }
            }
            else
            {
                Debug.Assert(((CPUTileStorage < byte >)cpuData.getOwner()).getChannels() == channels);
                Debug.Assert(cpuData.getOwner().getTileSize() == tileSize + 2 * border);
                Debug.Assert(level <= maxLevel);

                byte[]compressedData = (byte[]) pthread_getspecific(*((pthread_key_t*)key));
                if (compressedData == null)
                {
                    compressedData = new byte[MAX_TILE_SIZE * MAX_TILE_SIZE * 4 * 2];
                    pthread_setspecific(*((pthread_key_t*)key), compressedData);
                }

                int fsize = (int)(offsets[2 * tileid + 1] - offsets[2 * tileid]);
                Debug.Assert(fsize < (tileSize + 2 * border) * (tileSize + 2 * border) * channels * 2);

                if (dxt)
                {
#if SINGLE_FILE
                    pthread_mutex_lock((pthread_mutex_t*)mutex);
                    fseek64(tileFile, header + offsets[2 * tileid], SEEK_SET);
                    fread(cpuData.data, fsize, 1, tileFile);
                    pthread_mutex_unlock((pthread_mutex_t*)mutex);
#else
                    FILE* file;
                    fopen(&file, name.c_str(), "rb");
                    fseek64(file, header + offsets[2 * tileid], SEEK_SET);
                    fread(cpuData.data, fsize, 1, file);
                    fclose(file);
                    /*ifstream fs(name.c_str(), ios.binary);
                    fs.seekg(header + offsets[2 * tileid], ios.beg);
                    fs.read((char*) cpuData.data, fsize);
                    fs.close();*/
#endif
                    cpuData.size = fsize;
                }
                else
                {
                    byte[] srcData = compressedData;
#if SINGLE_FILE
                    pthread_mutex_lock((pthread_mutex_t*)mutex);
                    fseek64(tileFile, header + offsets[2 * tileid], SEEK_SET);
                    fread(compressedData, fsize, 1, tileFile);
                    pthread_mutex_unlock((pthread_mutex_t*)mutex);
#else
                    FILE* file;
                    fopen(&file, name.c_str(), "rb");
                    fseek64(file, header + offsets[2 * tileid], SEEK_SET);
                    fread(compressedData, fsize, 1, file);
                    fclose(file);
                    /*ifstream fs(name.c_str(), ios.binary);
                    fs.seekg(header + offsets[2 * tileid], ios.beg);
                    fs.read((char*) src, fsize);
                    fs.close();*/
#endif

                    mfs_file fd;
                    mfs_open(srcData, fsize, (char*)"r", &fd);
                    TIFF* tf = TIFFClientOpen("name", "r", &fd,
                        (TIFFReadWriteProc)mfs_read, (TIFFReadWriteProc)mfs_write, (TIFFSeekProc)mfs_lseek,
                        (TIFFCloseProc)mfs_close, (TIFFSizeProc)mfs_size, (TIFFMapFileProc)mfs_map,
                        (TIFFUnmapFileProc)mfs_unmap);
                    TIFFReadEncodedStrip(tf, 0, cpuData.data, (tsize_t) - 1);
                    TIFFClose(tf);
                }
            }

            return true;
#endif 
            throw new NotImplementedException();
        }

        public void swap(OrthoCPUProducer p)
        {
            base.swap(p);
            Std.Swap(ref name, ref p.name);
            Std.Swap(ref channels, ref p.channels);
            Std.Swap(ref tileSize, ref p.tileSize);
            Std.Swap(ref maxLevel, ref p.maxLevel);
            Std.Swap(ref dxt, ref p.dxt);
            Std.Swap(ref offsets, ref p.offsets);
            Std.Swap(ref mutex, ref p.mutex);
            Std.Swap(ref tileFile, ref p.tileFile);
        }




        /**
         * The name of the file containing the residual tiles to load.
         */
        private string name;

        /**
         * The number of components per pixel in the tiles to load.
         */
        private int channels;

        /**
         * The size of the tiles to load, without borders. A tile contains
         * (tileSize+4)*(tileSize+4)*channels samples.
         */
        private int tileSize;

        /**
         * The size in pixels of the border around each tile. A tile contains
         * (tileSize+4)*(tileSize+4)*channels samples.
         */
        private int border;

        /**
         * The maximum level of the stored tiles on disk (inclusive).
         */
        private int maxLevel;

        /**
         * true if the produced tiles are compressed in DXT format.
         */
        private bool dxt;

        /**
         * Offset of the first stored tile on disk. The offsets indicated in
         * the tile offsets array #offsets are relative to this offset.
         */
        private int header;

        /**
         * The offsets of each tile on disk, relatively to #offset, for each
         * tile id (see #getTileId).
         */
        private long offsets;

        /**
         * A mutex used to serializes accesses to the file storing the tiles.
         */
        private object mutex;

        /**
         * The file storing the tiles on disk.
         */
        private FileStream tileFile;

        /**
         * A key to store thread specific buffers used to produce the tiles.
         */
        private static object key;

        /**
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
            return tx + ty * (1 << level) + ((1 << (2 * level)) - 1) / 3;
        }


        private const int MAX_TILE_SIZE = 512;

    }
}
































