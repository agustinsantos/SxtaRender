using Sxta.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sxta.Proland.Terrain
{
    internal abstract class FloatTileCache
    {
        public struct FloatTile
        {
            public int level, tx, ty;

            public float[] data;

            public FloatTile(int level, int tx, int ty, float[] data = null)
            {
                this.level = level;
                this.tx = tx;
                this.ty = ty;
                this.data = data;
            }

            public int key()
            {
                return tx + ty * (1 << level) + ((1 << (2 * level)) - 1) / 3;
            }
        }

        public FloatTileCache(int capacity = 20)
        {
            this.capacity = capacity;
        }

        public float[] getTile(int level, int tx, int ty)
        {
            int key = new FloatTile(level, tx, ty).key();
            Queue<FloatTile> li;
            if (tileCache.TryGetValue(key, out li))
            {
                float[] data = readTile(level, tx, ty);

                if (tileCache.Count == capacity)
                {
                    // evict least recently used tile if cache is full
                    FloatTile t = tileCacheOrder.Dequeue();
                    tileCache.Remove(t.key());
                }

                // create tile, put it at the end of tileCacheOrder, and update the map
                FloatTile ftile = new FloatTile(level, tx, ty, data);
                tileCacheOrder.Enqueue(ftile);
                tileCache[key] = li;
                Debug.Assert(tileCache.ContainsKey(key));
                return data;
            }
            else
            {
                // put t at the end of tileCacheOrder, and update the map accordingly
                FloatTile t = li.Dequeue();
                Debug.Assert(t.tx == tx && t.ty == ty);
                tileCacheOrder.Enqueue(t);
                return t.data;
            }
        }

        protected abstract float[] readTile(int level, int tx, int ty);


        private int capacity;

        //private typedef map<int, list<FloatTile*>::iterator> Cache;

        private Dictionary<int, Queue<FloatTile>> tileCache;

        private Queue<FloatTile> tileCacheOrder = new Queue<FloatTile>();
    }

    internal class DemTileCache : FloatTileCache
    {

        //public FILE* tileFile;

        public int tileSize;

        public int rootLevel;

        public int deltaLevel;

        public int rootTx;

        public int rootTy;

        public int minLevel;

        public int maxLevel;

        public int analyzeLevel;

        public float scale;

        public int header;

        public int[] offsets;

        public byte[] compressedData;

        public byte[] uncompressedData;

        public DemTileCache(string name, int capacity = 20)
        {
            DemReader reader = new DemReader();
            reader.Read(name);
#if TODO
            fopen(&tileFile, name.c_str(), "rb");
            fread(&minLevel, sizeof(int), 1, tileFile);
            fread(&maxLevel, sizeof(int), 1, tileFile);
            fread(&tileSize, sizeof(int), 1, tileFile);
            fread(&rootLevel, sizeof(int), 1, tileFile);
            fread(&rootTx, sizeof(int), 1, tileFile);
            fread(&rootTy, sizeof(int), 1, tileFile);
            fread(&scale, sizeof(float), 1, tileFile);

            int ntiles = minLevel + ((1 << (max(maxLevel - minLevel, 0) * 2 + 2)) - 1) / 3;
            header = sizeof(float) + sizeof(int) * (6 + ntiles * 2);
            offsets = new unsigned int[ntiles * 2];
            fread(offsets, sizeof(unsigned int) * ntiles * 2, 1, tileFile);

            compressedData = new unsigned char[512 * 512 * 4];
            uncompressedData = new unsigned char[512 * 512 * 4];
#endif
        }



        protected override float[] readTile(int level, int tx, int ty)
        {
#if TODO
            int tileid = getTileId(level, tx, ty);
            int tilesize = getTileSize(level) + 5;

            if (level > maxLevel)
            {
                float* result = new float[(tileSize + 5) * (tileSize + 5)];
                for (int j = 0; j < tilesize; ++j)
                {
                    for (int i = 0; i < tilesize; ++i)
                    {
                        result[i + j * (tileSize + 5)] = 0.0f;
                    }
                }
                return result;
            }

            int fsize = offsets[2 * tileid + 1] - offsets[2 * tileid];
            assert(fsize < (tileSize + 5) * (tileSize + 5) * 2);

            fseek64(tileFile, header + offsets[2 * tileid], SEEK_SET);
            fread(compressedData, fsize, 1, tileFile);

            mfs_file fd;
            mfs_open(compressedData, fsize, (char*)"r", &fd);
            TIFF* tf = TIFFClientOpen("name", "r", &fd,
                (TIFFReadWriteProc)mfs_read, (TIFFReadWriteProc)mfs_write, (TIFFSeekProc)mfs_lseek,
                (TIFFCloseProc)mfs_close, (TIFFSizeProc)mfs_size, (TIFFMapFileProc)mfs_map,
                (TIFFUnmapFileProc)mfs_unmap);
            TIFFReadEncodedStrip(tf, 0, uncompressedData, (tsize_t) - 1);
            TIFFClose(tf);

            float* result = new float[(tileSize + 5) * (tileSize + 5)];
            for (int j = 0; j < tilesize; ++j)
            {
                for (int i = 0; i < tilesize; ++i)
                {
                    int off = 2 * (i + j * tilesize);
                    short z = short(uncompressedData[off + 1]) << 8 | short(uncompressedData[off]);
                    result[i + j * (tileSize + 5)] = z * scale;
                }
            }

            return result;
#endif 
            throw new NotImplementedException();
        }

        public int getTileId(int level, int tx, int ty)
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


        public int getTileSize(int level)
        {
            return level < minLevel ? tileSize >> (minLevel - level) : tileSize;
        }

    }

    internal class ElevationTileCache : FloatTileCache
    {

        public DemTileCache r;

        public ElevationTileCache(DemTileCache residuals, int capacity = 20) : base(capacity)
        {
            this.r = residuals;
        }


        protected override float[] readTile(int level, int tx, int ty)
        {
            float[] result = new float[(r.tileSize + 5) * (r.tileSize + 5)];

            float[] residuals = r.getTile(level, tx, ty);

            if (level == 0)
            {
                for (int i = 0; i < (r.tileSize + 5) * (r.tileSize + 5); ++i)
                {
                    result[i] = residuals[i];
                }
            }
            else
            {
                float[] parentTile = getTile(level - 1, tx / 2, ty / 2);
                int tileSize = r.getTileSize(level);
                int px = 1 + (tx % 2) * tileSize / 2;
                int py = 1 + (ty % 2) * tileSize / 2;
                int n = r.tileSize + 5;
                for (int j = 0; j <= tileSize + 4; ++j)
                {
                    for (int i = 0; i <= tileSize + 4; ++i)
                    {
                        float z;
                        if (j % 2 == 0)
                        {
                            if (i % 2 == 0)
                            {
                                z = parentTile[i / 2 + px + (j / 2 + py) * n];
                            }
                            else
                            {
                                float z0 = parentTile[i / 2 + px - 1 + (j / 2 + py) * n];
                                float z1 = parentTile[i / 2 + px + (j / 2 + py) * n];
                                float z2 = parentTile[i / 2 + px + 1 + (j / 2 + py) * n];
                                float z3 = parentTile[i / 2 + px + 2 + (j / 2 + py) * n];
                                z = ((z1 + z2) * 9 - (z0 + z3)) / 16;
                            }
                        }
                        else
                        {
                            if (i % 2 == 0)
                            {
                                float z0 = parentTile[i / 2 + px + (j / 2 - 1 + py) * n];
                                float z1 = parentTile[i / 2 + px + (j / 2 + py) * n];
                                float z2 = parentTile[i / 2 + px + (j / 2 + 1 + py) * n];
                                float z3 = parentTile[i / 2 + px + (j / 2 + 2 + py) * n];
                                z = ((z1 + z2) * 9 - (z0 + z3)) / 16;
                            }
                            else
                            {
                                int di, dj;
                                z = 0.0f;
                                for (dj = -1; dj <= 2; ++dj)
                                {
                                    float f = dj == -1 || dj == 2 ? -1 / 16.0f : 9 / 16.0f;
                                    for (di = -1; di <= 2; ++di)
                                    {
                                        float g = di == -1 || di == 2 ? -1 / 16.0f : 9 / 16.0f;
                                        z += f * g * parentTile[i / 2 + di + px + (j / 2 + dj + py) * n];
                                    }
                                }
                            }
                        }
                        int off = i + j * n;
                        result[off] = z + residuals[off];
                    }
                }
            }

            return result;
        }

        public float getHeight(int level, int x, int y)
        {
            int m = level < r.minLevel ? r.tileSize >> (r.minLevel - level) : r.tileSize << (level - r.minLevel);
            x = System.Math.Max(0, System.Math.Min(m, x));
            y = System.Math.Max(0, System.Math.Min(m, y));
            int tx = x == m ? System.Math.Max(0, x / r.tileSize - 1) : x / r.tileSize;
            int ty = y == m ? System.Math.Max(0, y / r.tileSize - 1) : y / r.tileSize;
            x = x == m ? System.Math.Min(m, r.tileSize) : x % r.tileSize;
            y = y == m ? System.Math.Min(m, r.tileSize) : y % r.tileSize;
            return getTile(level, tx, ty)[(x + 2) + (y + 2) * (r.tileSize + 5)];
        }


        public float getHeight(int level, int x, int y, float dx, float dy)
        {
            int m = level < r.minLevel ? r.tileSize >> (r.minLevel - level) : r.tileSize << (level - r.minLevel);
            x = System.Math.Max(0, System.Math.Min(m, x));
            y = System.Math.Max(0, System.Math.Min(m, y));
            int tx = x == m ? System.Math.Max(0, x / r.tileSize - 1) : x / r.tileSize;
            int ty = y == m ? System.Math.Max(0, y / r.tileSize - 1) : y / r.tileSize;
            x = x == m ? System.Math.Min(m, r.tileSize) : x % r.tileSize;
            y = y == m ? System.Math.Min(m, r.tileSize) : y % r.tileSize;
            float[] tile = getTile(level, tx, ty);
            float z00 = tile[(x + 2) + (y + 2) * (r.tileSize + 5)];
            float z10 = tile[(x + 3) + (y + 2) * (r.tileSize + 5)];
            float z01 = tile[(x + 2) + (y + 3) * (r.tileSize + 5)];
            float z11 = tile[(x + 3) + (y + 3) * (r.tileSize + 5)];
            return ((1.0f - dx) * z00 + dx * z10) * (1.0f - dy) + ((1.0f - dx) * z01 + dx * z11) * dy;
        }

    }

    internal class PlanetElevationTileCache
    {

        public ElevationTileCache[] faces;

        public int level;

        public int m;

        public PlanetElevationTileCache(ElevationTileCache[] faces, int level)
        {
            this.faces = faces;
            this.level = level;
            DemTileCache r = faces[0].r;
            m = level < r.minLevel ? r.tileSize >> (r.minLevel - level) : r.tileSize << (level - r.minLevel);
        }


        public float getHeight(double sx, double sy, double sz)
        {
            int face = 0;
            double x = 0, y = 0;
            if ((sx <= -System.Math.Abs(sy)) && (sx <= -System.Math.Abs(sz)))
            {
                face = 4;
                x = sy / sx;
                y = -sz / sx;
            }
            else if ((sx >= System.Math.Abs(sy)) && (sx >= System.Math.Abs(sz)))
            {
                face = 2;
                x = sy / sx;
                y = sz / sx;
            }
            else if ((sy <= -System.Math.Abs(sx)) && (sy <= -System.Math.Abs(sz)))
            {
                face = 1;
                x = -sx / sy;
                y = -sz / sy;
            }
            else if ((sy >= System.Math.Abs(sx)) && (sy >= System.Math.Abs(sz)))
            {
                face = 3;
                x = -sx / sy;
                y = sz / sy;
            }
            else if ((sz <= -System.Math.Abs(sy)) && (sz <= -System.Math.Abs(sx)))
            {
                face = 5;
                x = -sx / sz;
                y = sy / sz;
            }
            else if ((sz >= System.Math.Abs(sy)) && (sz >= System.Math.Abs(sx)))
            {
                face = 0;
                x = sx / sz;
                y = sy / sz;
            }

            x = (x * 0.5 + 0.5) * m;
            y = (y * 0.5 + 0.5) * m;
            int ix = (int)System.Math.Floor(x);
            int iy = (int)System.Math.Floor(y);
            return faces[face].getHeight(level, ix, iy, (float)(x - ix), (float)(y - iy));
        }
    }

    internal class ApertureColorMipmap : ColorMipmap
    {
        public double R;
        private static float Id(float x)
        { return x; }

        public ApertureColorMipmap(int baseLevelSize, int tileSize, string cache, double R) :
        base(null, baseLevelSize, tileSize, 2, 3, Id, Id, cache)
        {
            this.R = R;
        }

        public virtual byte[] readTile(int tx, int ty)
        {
#if TODO
            if (currentLevel == maxLevel)
            {
                string buf;
                byte[] data = new byte[(tileSize + 2 * border) * (tileSize + 2 * border) * channels];
                sprintf(buf, "%s/%.4d-%.4d.tiff", cache.c_str(), tx, ty);
                TIFF* f = TIFFOpen(buf, "rb");
                TIFFReadEncodedStrip(f, 0, data, (tsize_t) - 1);
                TIFFClose(f);
                return data;
            }
            else
            {
                return ColorMipmap.readTile(tx, ty);
            }
#endif 
            throw new NotImplementedException();
        }

        public virtual void produceTile(int level, int tx, int ty)
        {
#if TODO
           if (level == maxLevel)
            {
                char buf[256];
                sprintf(buf, "%s/%.4d-%.4d.tiff", cache.c_str(), tx, ty);
                TIFF* tf = TIFFOpen(buf, "rb");
                TIFFReadEncodedStrip(tf, 0, tile, (tsize_t) - 1);
                TIFFClose(tf);
            }
            else
            {
                ColorMipmap::produceTile(level, tx, ty);
            }
#endif
            throw new NotImplementedException();
        }

        public virtual void buildMipmapLevel(int level)
        {
#if TODO
            string buf;
            int nTiles = 1 << level;
            int nTilesPerFile = min(nTiles, 16);

            printf("Build mipmap level %d...\n", level);

            currentLevel = level + 1;
            reset(tileSize << currentLevel, tileSize << currentLevel, tileSize);

            for (int dy = 0; dy < nTiles / nTilesPerFile; ++dy)
            {
                for (int dx = 0; dx < nTiles / nTilesPerFile; ++dx)
                {
                    sprintf(buf, "%s/%.2d-%.4d-%.4d.tiff", cache.c_str(), level, dx, dy);
                    if (flog(buf))
                    {
                        TIFF f = TIFFOpen(buf, "wb");
                        for (int ny = 0; ny < nTilesPerFile; ++ny)
                        {
                            for (int nx = 0; nx < nTilesPerFile; ++nx)
                            {
                                int tx = nx + dx * nTilesPerFile;
                                int ty = ny + dy * nTilesPerFile;

                                Vector3d pc = new Vector3d((tx + 0.5) / (1 << level) * (2.0 * R) - R, (ty + 0.5) / (1 << level) * (2.0 * R) - R, R);
                                Vector3d uz = pc.normalize();
                                Vector3d ux = Vector3d.UNIT_Y.crossProduct(uz).normalize();
                                Vector3d uy = uz.crossProduct(ux);
                                Matrix3d worldToTangentFrame = new Matrix3d(
                                    ux.x, ux.y, ux.z,
                                    uy.x, uy.y, uy.z,
                                    uz.x, uz.y, uz.z);

                                Matrix3d[][] childToParentFrame = new Matrix3d[4][4];
                                for (int j = -1; j < 3; ++j)
                                {
                                    for (int i = -1; i < 3; ++i)
                                    {
                                        pc = Vector3d((2 * tx + i + 0.5) / (2 << level) * (2.0 * R) - R, (2 * ty + j + 0.5) / (2 << level) * (2.0 * R) - R, R);
                                        uz = pc.normalize();
                                        ux = Vector3d.UNIT_Y.crossProduct(uz).normalize();
                                        uy = uz.crossProduct(ux);
                                        childToParentFrame[i + 1][j + 1] = worldToTangentFrame * new Matrix3d(
                                                ux.x, uy.x, uz.x,
                                                ux.y, uy.y, uz.y,
                                                ux.z, uy.z, uz.z);
                                    }
                                }

                                int off = 0;
                                for (int j = -border; j < tileSize + border; ++j)
                                {
                                    for (int i = -border; i < tileSize + border; ++i)
                                    {
                                        int ix = 2 * (tx * tileSize + i);
                                        int iy = 2 * (ty * tileSize + j);

                                        vec4f c1 = getTileColor(ix, iy);
                                        vec4f c2 = getTileColor(ix + 1, iy);
                                        vec4f c3 = getTileColor(ix, iy + 1);
                                        vec4f c4 = getTileColor(ix + 1, iy + 1);

                                        float a1 = pow(c1.x / 255, 0.25f);
                                        float a2 = pow(c2.x / 255, 0.25f);
                                        float a3 = pow(c3.x / 255, 0.25f);
                                        float a4 = pow(c4.x / 255, 0.25f);
                                        float a = (a1 + a2 + a3 + a4) / 4.0;
                                        //float a = max(a1, max(a2, max(a3, a4)));
                                        //float a = ((a1 + a2 + a3 + a4) / 4.0 + max(a1, max(a2, max(a3, a4)))) / 2.0;
                                        tile[off++] = (int)(roundf(pow(a, 4.0f) * 255));

                                        //TODO #define X(x) tan(x * (2.9 / 255.0) - 1.45) / 8.0
                                        Vector3d n1 = new Vector3d(X(c1.y), X(c1.z), 0.0);
                                        Vector3d n2 = new Vector3d(X(c2.y), X(c2.z), 0.0);
                                        Vector3d n3 = new Vector3d(X(c3.y), X(c3.z), 0.0);
                                        Vector3d n4 = new Vector3d(X(c4.y), X(c4.z), 0.0);
                                        n1.z = sqrt(1.0 - n1.squaredLength());
                                        n2.z = sqrt(1.0 - n2.squaredLength());
                                        n3.z = sqrt(1.0 - n3.squaredLength());
                                        n4.z = sqrt(1.0 - n4.squaredLength());

                                        int ui = (i + tileSize / 2) / (tileSize / 2);
                                        int uj = (j + tileSize / 2) / (tileSize / 2);
                                        vec3d n = childToParentFrame[ui][uj] * n1;
                                        n += childToParentFrame[ui][uj] * n2;
                                        n += childToParentFrame[ui][uj] * n3;
                                        n += childToParentFrame[ui][uj] * n4;
                                        n = n.normalize();

                                        tile[off++] = int((atan(8.0 * n.x) / 2.9 + 0.5) * 255);
                                        tile[off++] = int((atan(8.0 * n.y) / 2.9 + 0.5) * 255);
                                    }
                                }

                                TIFFSetField(f, TIFFTAG_IMAGEWIDTH, tileSize + 2 * border);
                                TIFFSetField(f, TIFFTAG_IMAGELENGTH, tileSize + 2 * border);
                                TIFFSetField(f, TIFFTAG_SAMPLESPERPIXEL, channels);
                                TIFFSetField(f, TIFFTAG_BITSPERSAMPLE, 8);
                                TIFFSetField(f, TIFFTAG_COMPRESSION, COMPRESSION_DEFLATE);
                                TIFFSetField(f, TIFFTAG_ORIENTATION, ORIENTATION_BOTLEFT);
                                TIFFSetField(f, TIFFTAG_PLANARCONFIG, PLANARCONFIG_CONTIG);
                                if (channels == 1)
                                {
                                    TIFFSetField(f, TIFFTAG_PHOTOMETRIC, PHOTOMETRIC_MINISBLACK);
                                }
                                else
                                {
                                    TIFFSetField(f, TIFFTAG_PHOTOMETRIC, PHOTOMETRIC_RGB);
                                }
                                TIFFWriteEncodedStrip(f, 0, tile, (tileSize + 2 * border) * (tileSize + 2 * border) * channels);
                                TIFFWriteDirectory(f);
                            }
                        }
                        TIFFClose(f);
                    }
                }
            }
#endif
            throw new NotImplementedException();
        }
    }

    internal class ApertureMipmap
    {
        public delegate void ProjFun(double x, double y, double w, out double sx, out double sy, out double sz);

        public ApertureMipmap(PlanetElevationTileCache z, ProjFun proj, double R, int buildLevel, int minLevel, int samples)
        {
            this.z = z;
            this.proj = proj;
            this.R = R;
            this.buildLevel = buildLevel;
            this.minLevel = minLevel;
            this.samples = samples;
        }

        public void build(string temp)
        {
#if TODO
     int tileSize = z.faces[0].r.tileSize;
    int blevel = buildLevel - z.faces[0].r.minLevel;
    assert(buildLevel == z.level);

    string buf ;
   byte[] tile = new byte[(tileSize + 4) * (tileSize + 4) * 3];

    int n = 1 << blevel;

    for (int ty = 0; ty<n; ++ty) {
        for (int tx = 0; tx<n; ++tx) {
            sprintf(buf, "%s/%.4d-%.4d.tiff", temp.c_str(), tx, ty);
	   	    if (flog(buf)) {
                vec3d pc = vec3d((tx + 0.5) / (1 << blevel) * (2.0 * R) - R, (ty + 0.5) / (1 << blevel) * (2.0 * R) - R, R);
    vec3d uz = pc.normalize();
    vec3d ux = vec3d::UNIT_Y.crossProduct(uz).normalize();
    vec3d uy = uz.crossProduct(ux);
    mat3d worldToTangentFrame = mat3d(
        ux.x, ux.y, ux.z,
        uy.x, uy.y, uy.z,
        uz.x, uz.y, uz.z);

    double x0 = (tx + 0.5 - 1.0 / tileSize) / (1 << blevel) * (2.0 * R) - R;
    double x1 = (tx + 0.5 + 1.0 / tileSize) / (1 << blevel) * (2.0 * R) - R;
    double y0 = (ty + 0.5 - 1.0 / tileSize) / (1 << blevel) * (2.0 * R) - R;
    double y1 = (ty + 0.5 + 1.0 / tileSize) / (1 << blevel) * (2.0 * R) - R;

    vec3d[] dir[8];
    float[] phi[8], cphi[8], sphi[8];
    float[] len[8];
    dir[0] = vec3d(x1, pc.y, R).normalize(R) - uz* R;
    dir[1] = vec3d(x1, y1, R).normalize(R) - uz* R;
    dir[2] = vec3d(pc.x, y1, R).normalize(R) - uz* R;
    dir[3] = vec3d(x0, y1, R).normalize(R) - uz* R;
    dir[4] = vec3d(x0, pc.y, R).normalize(R) - uz* R;
    dir[5] = vec3d(x0, y0, R).normalize(R) - uz* R;
    dir[6] = vec3d(pc.x, y0, R).normalize(R) - uz* R;
    dir[7] = vec3d(x1, y0, R).normalize(R) - uz* R;
                        for (int i = 0; i < 8; ++i) {
                            dir[i] = worldToTangentFrame * dir[i];
                            phi[i] = System.Math.Atan2(dir[i].y, dir[i].x);
                            cphi[i] = System.Math.Cos(phi[i]);
                            sphi[i] = System.Math.Sin(phi[i]);
                            len[i] = dir[i].length();
                        }

TIFF* f = TIFFOpen(buf, "wb");
                for (int y = -2; y<tileSize + 2; ++y) {
                    for (int x = -2; x<tileSize + 2; ++x) {

                        float[] slopes[8] = { 0, 0, 0, 0, 0, 0, 0, 0 };
double sx, sy, sz;
                        proj(tx* tileSize + x + 0.5, ty* tileSize + y + 0.5, z.m, sx, sy, sz);
float z0 = z.getHeight(sx, sy, sz);
                        compute(buildLevel, tx* tileSize + x, ty* tileSize + y, 0.5, 0.5, z0, len, 1, slopes);

vec3f n(0, 0, 0);
//int v = (int) System.Math.Floor(max(0.0, (ambient(phi, cphi, sphi, slopes, n) * 2.0 - 1.0)) * 255);
int v = (int)System.Math.Floor(pow(ambient(phi, cphi, sphi, slopes, n), 4.0f) * 255);

n = n.normalize();
                        //n.x = (n.x + 0.5) * 255;
                        //n.y = (n.y + 0.5) * 255;
                        n.x = (System.Math.Atan(8.0*n.x)/2.9 + 0.5) * 255;
                        n.y = (System.Math.Atan(8.0*n.y)/2.9 + 0.5) * 255;

                        tile[(x + 2 + (y + 2) * (tileSize + 4)) * 3] = v;
                        tile[(x + 2 + (y + 2) * (tileSize + 4)) * 3 + 1] = max(0, min(255, int(n.x)));
                        tile[(x + 2 + (y + 2) * (tileSize + 4)) * 3 + 2] = max(0, min(255, int(n.y)));
                    }
                }
                TIFFSetField(f, TIFFTAG_IMAGEWIDTH, tileSize + 4);
                TIFFSetField(f, TIFFTAG_IMAGELENGTH, tileSize + 4);
                TIFFSetField(f, TIFFTAG_SAMPLESPERPIXEL, 3);
                TIFFSetField(f, TIFFTAG_BITSPERSAMPLE, 8);
                TIFFSetField(f, TIFFTAG_COMPRESSION, COMPRESSION_DEFLATE);
                TIFFSetField(f, TIFFTAG_ORIENTATION, ORIENTATION_BOTLEFT);
                TIFFSetField(f, TIFFTAG_PLANARCONFIG, PLANARCONFIG_CONTIG);
                TIFFSetField(f, TIFFTAG_PHOTOMETRIC, PHOTOMETRIC_RGB);
                TIFFWriteEncodedStrip(f, 0, tile, (tileSize + 4) * (tileSize + 4) * 3);
                TIFFClose(f);
	   	    }
        }
    }

    tile = null;
#endif
            throw new NotImplementedException();
        }

        public void Generate(string cache, string file)
        {
            int tileSize = z.faces[0].r.tileSize;
            int blevel = buildLevel - z.faces[0].r.minLevel;

            ApertureColorMipmap a = new ApertureColorMipmap(tileSize << blevel, tileSize, cache, R);
            a.ComputeMipmap();
            a.Generate(0, 0, 0, false, false, 90, file);
        }


        private PlanetElevationTileCache z;

        private ProjFun proj;

        private double R;

        private int buildLevel;

        private int minLevel;

        private int samples;

        private void Compute(int level, int x, int y, float dx, float dy, float z0, float[] len, int flen, float[] slopes)
        {
            int[] ddx = new int[8] { 1, 1, 0, -1, -1, -1, 0, 1 };
            int[] ddy = new int[8] { 0, 1, 1, 1, 0, -1, -1, -1 };
            int w = z.m >> (z.level - level);
            for (int i = 0; i < 8; ++i)
            {
                for (int s = 1; s <= samples; ++s)
                {
                    double sx, sy, sz;
                    proj(x + s * ddx[i] + dx, y + s * ddy[i] + dy, w, out sx, out sy, out sz);
                    float z1 = z.getHeight(sx, sy, sz);
                    float slope = (z1 - z0) / (s * flen * len[i]);
                    slopes[i] = System.Math.Max(slope, slopes[i]);
                }
            }
            if (level > minLevel)
            {
                int px = (int)System.Math.Floor(x / 2.0);
                int py = (int)System.Math.Floor(y / 2.0);
                dx = (dx + x - 2 * px) * 0.5f;
                dy = (dy + y - 2 * py) * 0.5f;
                if (dx >= 1.0)
                {
                    px += 1;
                    dx -= 1.0f;
                }
                if (dy >= 1.0)
                {
                    py += 1;
                    dy -= 1.0f;
                }
                Compute(level - 1, px, py, dx, dy, z0, len, 2 * flen, slopes);
            }
        }

        private float ambient(float[] phi, float[] cphi, float[] sphi, float[] slopes, Vector3f norm)
        {
            float result = 0.0f;
            for (int i = 0; i < 8; ++i)
            {
                int j = (i + 1) % 8;
                float theta1 = (float)(System.Math.PI / 2.0 - System.Math.Atan(slopes[i]));
                float theta2 = (float)(System.Math.PI / 2.0 - System.Math.Atan(slopes[j]));
                float theta = (theta2 + theta1) * 0.5f;
                float ctheta = (float)(System.Math.Cos(theta));
                float stheta = (float)(System.Math.Sin(theta));
                float l = theta - ctheta * stheta;
                float dphi = System.Math.Abs(phi[j] - phi[i]);
                dphi = (float)(dphi < System.Math.PI ? dphi : 2.0 * System.Math.PI - dphi);
                result += (float)(dphi * (1.0 - ctheta));
                norm.X += l * (sphi[j] - sphi[i]);
                norm.Y += l * (cphi[i] - cphi[j]);
                norm.Z += dphi * stheta * stheta;
            }
            return (float)(result / (2.0 * System.Math.PI));
        }
    }
}
