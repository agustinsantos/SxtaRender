﻿using log4net;
using proland;
using Sxta.Core;
using Sxta.Math;
using Sxta.Render;
using Sxta.Render.Resources;
using Sxta.Render.Scenegraph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sxta.Proland.Terrain
{
    class ElevationProducer : TileProducer
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private void SetNoise(float[] n, int tileIndex, int tileWidth, int x, int y, float v)
        {
            n[tileIndex + (x) + (y) * tileWidth] = v;
        }
        Texture2DArray createDemNoise(int tileWidth)
        {

            int[] layers = new int[6] { 0, 1, 3, 5, 7, 15 };
            float[] noiseArray = new float[6 * tileWidth * tileWidth];
            long rand = 1234567;
            for (int nl = 0; nl < 6; ++nl)
            {
                int n = nl * tileWidth * tileWidth;

                float val = noiseArray[n];
                int l = layers[nl];
                // corners
                for (int j = 0; j < tileWidth; ++j)
                {
                    for (int i = 0; i < tileWidth; ++i)
                    {
                        SetNoise(noiseArray, n, tileWidth, i, j, 0.0f);
                    }
                }
                long brand;
                // bottom border
                brand = (l & 1) == 0 ? 7654321 : 5647381;
                for (int h = 5; h <= tileWidth / 2; ++h)
                {
                    float N = Noise.frandom(brand) * 2.0f - 1.0f;
                    SetNoise(noiseArray, n, tileWidth, h, 2, N);
                    SetNoise(noiseArray, n, tileWidth, tileWidth - 1 - h, 2, N);
                }
                for (int v = 3; v < 5; ++v)
                {
                    for (int h = 5; h < tileWidth - 5; ++h)
                    {
                        float N = Noise.frandom(brand) * 2.0f - 1.0f;
                        SetNoise(noiseArray, n, tileWidth, h, v, N);
                        SetNoise(noiseArray, n, tileWidth, tileWidth - 1 - h, 4 - v, N);
                    }
                }
                // right border
                brand = (l & 2) == 0 ? 7654321 : 5647381;
                for (int v = 5; v <= tileWidth / 2; ++v)
                {
                    float N = Noise.frandom(brand) * 2.0f - 1.0f;
                    SetNoise(noiseArray, n, tileWidth, tileWidth - 3, v, N);
                    SetNoise(noiseArray, n, tileWidth, tileWidth - 3, tileWidth - 1 - v, N);
                }
                for (int h = tileWidth - 4; h >= tileWidth - 5; --h)
                {
                    for (int v = 5; v < tileWidth - 5; ++v)
                    {
                        float N = Noise.frandom(brand) * 2.0f - 1.0f;
                        SetNoise(noiseArray, n, tileWidth, h, v, N);
                        SetNoise(noiseArray, n, tileWidth, 2 * tileWidth - 6 - h, tileWidth - 1 - v, N);
                    }
                }
                // top border
                brand = (l & 4) == 0 ? 7654321 : 5647381;
                for (int h = 5; h <= tileWidth / 2; ++h)
                {
                    float N = Noise.frandom(brand) * 2.0f - 1.0f;
                    SetNoise(noiseArray, n, tileWidth, h, tileWidth - 3, N);
                    SetNoise(noiseArray, n, tileWidth, tileWidth - 1 - h, tileWidth - 3, N);
                }
                for (int v = tileWidth - 2; v < tileWidth; ++v)
                {
                    for (int h = 5; h < tileWidth - 5; ++h)
                    {
                        float N = Noise.frandom(brand) * 2.0f - 1.0f;
                        SetNoise(noiseArray, n, tileWidth, h, v, N);
                        SetNoise(noiseArray, n, tileWidth, tileWidth - 1 - h, 2 * tileWidth - 6 - v, N);
                    }
                }
                // left border
                brand = (l & 8) == 0 ? 7654321 : 5647381;
                for (int v = 5; v <= tileWidth / 2; ++v)
                {
                    float N = Noise.frandom(brand) * 2.0f - 1.0f;
                    SetNoise(noiseArray, n, tileWidth, 2, v, N);
                    SetNoise(noiseArray, n, tileWidth, 2, tileWidth - 1 - v, N);
                }
                for (int h = 1; h >= 0; --h)
                {
                    for (int v = 5; v < tileWidth - 5; ++v)
                    {
                        float N = Noise.frandom(brand) * 2.0f - 1.0f;
                        SetNoise(noiseArray, n, tileWidth, h, v, N);
                        SetNoise(noiseArray, n, tileWidth, 4 - h, tileWidth - 1 - v, N);
                    }
                }
                // center
                for (int v = 5; v < tileWidth - 5; ++v)
                {
                    for (int h = 5; h < tileWidth - 5; ++h)
                    {
                        SetNoise(noiseArray, n, tileWidth, h, v, Noise.frandom(rand) * 2.0f - 1.0f);
                    }
                }
            }
            Texture2DArray noiseTexture = new Texture2DArray(tileWidth, tileWidth, 6, TextureInternalFormat.R16F,
                TextureFormat.RED, PixelType.FLOAT, new Texture.Parameters().wrapS(TextureWrap.REPEAT).wrapT(TextureWrap.REPEAT).min(TextureFilter.NEAREST).mag(TextureFilter.NEAREST), new Sxta.Render.Buffer.Parameters(), new CPUBuffer<float>(noiseArray));
            return noiseTexture;
        }
#if TODO
        static_ptr<Factory<int, ptr<Texture2DArray>>> demNoiseFactory(new Factory<int, ptr<Texture2DArray>>(createDemNoise));
        static_ptr<Factory<pair<ptr<Texture2D>, ptr<Texture2D>>, ptr<FrameBuffer>>> demFramebufferFactory(
        new Factory<pair<ptr<Texture2D>, ptr<Texture2D>>, ptr<FrameBuffer>>(createDemFramebuffer));
#endif
        FrameBuffer createDemFramebuffer(Tuple<Texture2D, Texture2D> textures)
        {
            int tileWidth = textures.Item1.getWidth();
            FrameBuffer frameBuffer = new FrameBuffer();
            frameBuffer.setReadBuffer(BufferId.COLOR0);
            frameBuffer.setDrawBuffer(BufferId.COLOR0);
            frameBuffer.setViewport(new Vector4i(0, 0, tileWidth, tileWidth));
            frameBuffer.setTextureBuffer(BufferId.COLOR0, textures.Item1, 0);
            if (textures.Item2 != null)
            {
                RenderBuffer depthBuffer = new RenderBuffer(RenderBuffer.RenderBufferFormat.DEPTH_COMPONENT32, tileWidth, tileWidth);
                frameBuffer.setTextureBuffer(BufferId.COLOR1, textures.Item2, 0);
                frameBuffer.setRenderBuffer(BufferId.DEPTH, depthBuffer);
                frameBuffer.setDepthTest(true, Function.ALWAYS);
            }
            else
            {
                frameBuffer.setDepthTest(false);
            }
            frameBuffer.setPolygonMode(PolygonMode.FILL, PolygonMode.FILL);
            return frameBuffer;
        }

        // Factory<Tuple<Texture2D, Texture2D>, FrameBuffer>> demFramebufferFactory(
        //new Factory<Tuple<Texture2D, Texture2D>, FrameBuffer>(createDemFramebuffer));

        /// <summary>
        /// Creates a new ElevationProducer.
        /// </summary>
        /// <param name="cache">the cache to store the produced tiles. The underlying
        /// storage must be a GPUTileStorage with floating point textures of
        /// at least two components per pixel.</param>
        /// <param name="residualTiles">the %producer producing the residual tiles. This
        /// %producer should produce its tiles in a CPUTileStorage of float type.
        /// Maybe null to create a fully random fractal %terrain.The size of the
        /// residual tiles (without borders) must be a multiple of the size of the
        /// elevation tiles(without borders).</param>
        /// <param name="demTexture">a texture used to produce the tiles. Its size must be equal
        /// to the elevation tile size(including borders). Its format must be RGBA32F.</param>
        /// <param name="layerTexture">a texture used to combine the layers of this %producer
        /// with the raw %terrain(maybe null if there are no layers; otherwise its
        /// size must be equal to the elevation tile size, including borders.Its
        /// format must be RGBA32F).</param>
        /// <param name="residualTexture">a texture used to produce the tiles. Its size must be
        /// equal to the elevation tile size(including borders). Its format must be
        /// I32F.</param>
        /// <param name="upsample"> the Program to perform the upsampling and add procedure on GPU.
        /// See \ref sec-elevation.</param>
        /// <param name="blend">the Program to blend the layers of this %producer with the raw
        /// %terrain elevations.</param>
        /// <param name="gridMeshSize">the size of the grid that will be used to render each tile.
        /// Must be the tile size(without borders) divided by a power of two.</param>
        /// <param name="noiseAmp">the amplitude of the noise to be added for each level
        /// (one amplitude per level).</param>
        /// <param name="flipDiagonals">true if the grid used to render each tile will use diagonal
        /// flipping to reduce geometric aliasing.</param>
        public ElevationProducer(TileCache cache, TileProducer residualTiles,
        Texture2D demTexture, Texture2D layerTexture, Texture2D residualTexture,
        Program upsample, Program blend, int gridMeshSize,
        List<float> noiseAmp, bool flipDiagonals = false) : base("ElevationProducer", "CreateElevationTile")
        {
            init(cache, residualTiles, demTexture, layerTexture, residualTexture, upsample, blend, gridMeshSize, noiseAmp, flipDiagonals);
        }

        /// <summary>
        ///  Creates an uninitialized ElevationProducer.
        /// </summary>
        protected ElevationProducer() : base("ElevationProducer", "CreateElevationTile")
        {

        }

        public override void getReferencedProducers(List<TileProducer> producers)
        {
            if (residualTiles != null)
            {
                producers.Add(residualTiles);
            }
        }

        public override void setRootQuadSize(float size)
        {
            base.setRootQuadSize(size);
            if (residualTiles != null)
            {
                residualTiles.setRootQuadSize(size);
            }
        }

        public override int getBorder()
        {
            Debug.Assert(residualTiles == null || residualTiles.getBorder() == 2);
            return 2;
        }


        protected FrameBuffer frameBuffer;

        /// <summary>
        /// The Program to perform the upsampling and add procedure on GPU.
        /// See \ref sec-elevation.
        /// </summary>
        protected Program upsample;

        /// <summary>
        /// The Program to blend the layers of this %producer with the raw %terrain
        /// elevations.
        /// </summary>
        protected Program blend;

        /// <summary>
        /// The %producer producing the residual tiles. This %producer should produce its
        /// tiles in a CPUTileStorage of float type. Maybe null to create a fully random
        /// fractal %terrain. The size of the residual tiles (without borders) must be a
        /// multiple of the size of the elevation tiles (without borders).
        /// </summary>
        protected TileProducer residualTiles;

        /// <summary>
        /// A texture used to produce the tiles. Its size must be equal to the elevation
        /// tile size (including borders). Its format must be RGBA32F.
        /// </summary>
        protected Texture2D demTexture;

        /// <summary>
        /// A texture used to produce the tiles. Its size must be equal to the elevation
        /// tile size (including borders). Its format must be I32F.
        /// </summary>
        protected Texture2D residualTexture;

        /// <summary>
        /// A texture used to combine the layers of this %producer with the raw %terrain
        /// (maybe null if there are no layers; otherwise its size must be equal to the
        /// elevation tile size, including borders. Its format must be RGBA32F).
        /// </summary>
        protected Texture2D layerTexture;

        /// <summary>
        /// Cube face ID for producers targeting spherical terrains.
        /// </summary>
        protected int face;

        /// <summary>
        /// Initializes this ElevationProducer. See #ElevationProducer.
        /// </summary>

        protected void init(TileCache cache, TileProducer residualTiles,
            Texture2D demTexture, Texture2D layerTexture, Texture2D residualTexture,
            Program upsample, Program blend, int gridMeshSize,
            List<float> noiseAmp, bool flipDiagonals = false)
        {
            int tileWidth = cache.getStorage().getTileSize();
            base.init(cache, true);
            this.frameBuffer = demFramebufferFactory.get(make_pair(demTexture, layerTexture));
            this.residualTiles = residualTiles;
            this.demTexture = demTexture;
            this.layerTexture = layerTexture;
            this.residualTexture = residualTexture;
            this.upsample = upsample;
            this.blend = blend;
            this.noiseTexture = demNoiseFactory.get(tileWidth);
            this.noiseAmp = noiseAmp;
            this.gridMeshSize = gridMeshSize;
            this.flipDiagonals = flipDiagonals;

            this.tileWSDFU = upsample.getUniform4f("tileWSDF");
            this.coarseLevelSamplerU = upsample.getUniformSampler("coarseLevelSampler");
            this.coarseLevelOSLU = upsample.getUniform4f("coarseLevelOSL");
            this.residualSamplerU = upsample.getUniformSampler("residualSampler");
            this.residualOSHU = upsample.getUniform4f("residualOSH");
            this.noiseSamplerU = upsample.getUniformSampler("noiseSampler");
            this.noiseUVLHU = upsample.getUniform4f("noiseUVLH");

            if (this.blend != null)
            {
                this.blendCoarseLevelSamplerU = blend.getUniformSampler("coarseLevelSampler");
                this.blendScaleU = blend.getUniform1f("scale");
            }

            if (residualTiles != null)
            {
                this.residualTile = new float[tileWidth * tileWidth];
            }
            else
            {
                this.residualTile = null;
            }
        }

        /// <summary>
        /// Initializes this ElevationProducer from a Resource.
        /// </summary>
        /// <param name="manager">the manager that will manage the created %resource.</param>
        /// <param name="r">the %resource.</param>
        /// <param name="">the %resource name.</param>
        /// <param name="">the %resource descriptor.</param>
        /// <param name="desc"></param>
        /// <param name="e">an optional XML element providing contextual information (such
        /// as the XML element in which the %resource descriptor was found).</param>

        protected void init(ResourceManager manager, Resource r, string name, ResourceDescriptor desc, XmlElement e = null)
        {
            TileCache cache;
            TileProducer residuals;
            Program upsampleProg;
            Program blendProg;
            Texture2D demTexture;
            Texture2D layerTexture;
            Texture2D residualTexture;
            int gridSize = 24;
            List<float> noiseAmp = null;
            bool flip = false;
            cache = (TileCache)(manager.loadResource(Resource.getParameter(desc, e, "cache"))).get();
            if (e.GetAttribute("residuals") != null)
            {
                residuals = (TileProducer)(manager.loadResource(Resource.getParameter(desc, e, "residuals")).get());
            }
            string upsample = "upsampleShader;";
            if (e.GetAttribute("upsampleProg") != null)
            {
                upsample = Resource.getParameter(desc, e, "upsampleProg");
            }
            upsampleProg = (Program)(manager.loadResource(upsample)).get();
            if (e.GetAttribute("gridSize") != null)
            {
                Resource.getIntParameter(desc, e, "gridSize", out gridSize);
            }
            if (e.GetAttribute("noise") != null)
            {
                string noiseAmps = e.GetAttribute("noise") + ",";
                string[] stringSeparator = new string[] { "," };
                string[] result = noiseAmps.Split(stringSeparator, StringSplitOptions.None); //Maybe StringSplitOptions.RemoveEmptyEntries?
                foreach (var node in result)
                    //while ((index = noiseAmps.find(',', start)) != string::npos)
                {
                    //float value;
                    //sscanf(node.c_str(), "%f", value);
                    noiseAmp.Add((float)Convert.ToDouble(node));
                }
            }
            if (e.GetAttribute("flip") != null && e.GetAttribute("flip") == "false")
            {
                flip = true;
            }
            if (e.GetAttribute("face") != null)
            {
                Resource.getIntParameter(desc, e, "face", out face);
            }
            else if (name.ElementAt(name.Length - 1) >= '1' && name.ElementAt(name.Length - 1) <= '6')
            {
                face = name.ElementAt(name.Length - 1) - '0';
            }
            else
            {
                face = 0;
            }

            int tileWidth = cache.getStorage().getTileSize();

            ostringstream demTex;

            demTex << "renderbuffer-" << tileWidth << "-RGBA32F";
            demTexture = (Texture2D)(manager.loadResource(demTex.str())).get();

            ostringstream residualTex;
            residualTex << "renderbuffer-" << tileWidth << "-R32F";
            residualTexture = (Texture2D)(manager.loadResource(residualTex.str())).get();

            XmlNode n = e.FirstChild;
            while (n != null)
            {
                XmlElement f = n as XmlElement;
                if (f == null)
                {
                    n = n.NextSibling;
                    continue;
                }

                TileLayer l = (TileLayer)manager.loadResource(desc, f);
                if (l != null)
                {
                    addLayer(l);
                }
                else
                {
                    if (log.IsWarnEnabled)
                    {
                        log.Warn("Unknown scene node element '" + f.Value.ToString() + "'");
                    }
                }
                n = n.NextSibling;
            }

            if (hasLayers())
            {
                demTex << "-1";
                layerTexture = manager.loadResource(demTex.str()).cast<Texture2D>();

                string blend = "blendShader;";
                if (e.GetAttribute("blendProg") != null)
                {
                    blend = Resource.getParameter(desc, e, "blendProg");
                }
                blendProg = (Program)manager.loadResource(blend).get();
            }

            init(cache, residuals, demTexture, layerTexture, residualTexture, upsampleProg, blendProg, gridSize, noiseAmp, flip);
        }

        protected internal override ulong getContext()
        {
            return layerTexture == null ? demTexture.get() : layerTexture.get();
        }

        protected internal override Render.Scenegraph.Task startCreateTile(int level, int tx, int ty, uint deadline, Render.Scenegraph.Task task, TaskGraph owner)
        {
            TaskGraph result = owner == null ? createTaskGraph(task) : owner;

            if (level > 0)
            {
                TileCache.Tile t = getTile(level - 1, tx / 2, ty / 2, deadline);
                Debug.Assert(t != null);
                result.addTask(t.task);
                result.addDependency(task, t.task);
            }

            if (residualTiles != null)
            {
                int tileSize = getCache().getStorage().getTileSize() - 5;
                int residualTileSize = residualTiles.getCache().getStorage().getTileSize() - 5;
                int mod = residualTileSize / tileSize;
                if (residualTiles.hasTile(level, tx / mod, ty / mod))
                {
                    TileCache.Tile t = residualTiles.getTile(level, tx / mod, ty / mod, deadline);
                    Debug.Assert(t != null);
                    result.addTask(t.task);
                    result.addDependency(task, t.task);
                }
            }
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
                ostringstream oss;
                oss << "Elevation tile " << getId() << " " << level << " " << tx << " " << ty;
                Logger.DEBUG_LOGGER.log("DEM", oss.str());
            }

            GPUTileStorage.GPUSlot gpuData = (GPUTileStorage.GPUSlot)(data);
            Debug.Assert(gpuData != null);

            ((GPUTileStorage)(getCache().getStorage())).notifyChange(gpuData);

            int tileWidth = data.getOwner().getTileSize();
            int tileSize = tileWidth - 5;

            GPUTileStorage.GPUSlot parentGpuData = null;
            if (level > 0)
            {

                TileCache.Tile t = findTile(level - 1, tx / 2, ty / 2);
                Debug.Assert(t != null);
                parentGpuData = (GPUTileStorage.GPUSlot)(t.getData());
                Debug.Assert(parentGpuData != null);
            }

            tileWSDFU.set(new Vector4f((float)tileWidth, getRootQuadSize() / (1 << level) / tileSize, ((tileWidth - 5) / gridMeshSize), flipDiagonals ? 1.0f : 0.0f));

            if (level > 0)
            {
                Texture2DArray t = parentGpuData.t;
                float dx = (float)((tx % 2) * (tileSize / 2));
                float dy = (float)((ty % 2) * (tileSize / 2));
                coarseLevelSamplerU.set(t);
                coarseLevelOSLU.set(new Vector4f(dx / parentGpuData.getWidth(), dy / parentGpuData.getHeight(), 1.0f / parentGpuData.getWidth(), GPUTileStorage.GPUSlot.l));
            }
            else
            {
                coarseLevelOSLU.set(new Vector4f(-1.0f, -1.0f, -1.0f, -1.0f));
            }

            int residualTileWidth = residualTiles == null ? 0 : residualTiles.getCache().getStorage().getTileSize();
            int mod = (residualTileWidth - 5) / tileSize;

            if (residualTiles != null && residualTiles.hasTile(level, tx / mod, ty / mod))
            {
                residualSamplerU.set(residualTexture);
                residualOSHU.set(new Vector4f(0.25f / tileWidth, 0.25f / tileWidth, 2.0f / tileWidth, 1.0f));

                int rx = (tx % mod) * tileSize;
                int ry = (ty % mod) * tileSize;
                TileCache.Tile t = residualTiles.findTile(level, tx / mod, ty / mod);
                Debug.Assert(t != null);
                CPUTileStorage<float>.CPUSlot cpuTile = (CPUTileStorage<float>.CPUSlot)(t.getData());
                Debug.Assert(cpuTile != null);
                for (int y = 0; y < tileWidth; ++y)
                {
                    for (int x = 0; x < tileWidth; ++x)
                    {

                        float r = cpuTile.data[(x + rx) + (y + ry) * residualTileWidth];
                        Debug.Assert(float.IsInfinity(r) == false);
                        residualTile[x + y * tileWidth] = r;
                    }
                }

                residualTexture.setSubImage(0, 0, 0, tileWidth, tileWidth, TextureFormat.RED, PixelType.FLOAT, new Sxta.Render.Buffer.Parameters(), new CPUBuffer<float>(residualTile));
            }
            else
            {
                residualSamplerU.set(residualTexture);
                residualOSHU.set(new Vector4f(0.0f, 0.0f, 1.0f, 0.0f));
            }

            float rs = level < noiseAmp.Count ? noiseAmp[level] : 0.0f;

            int noiseL = 0;
            if (face == 1)
            {
                int offset = 1 << level;
                int bottomB = Noise.cnoise(tx + 0.5f, ty + offset) > 0.0 ? 1 : 0;
                int rightB = (tx == offset - 1 ? Noise.cnoise(ty + offset + 0.5f, offset) : Noise.cnoise(tx + 1, ty + offset + 0.5f)) > 0.0 ? 2 : 0;
                int topB = (ty == offset - 1 ? Noise.cnoise((3 * offset - 1 - tx) + 0.5f, offset) : Noise.cnoise(tx + 0.5f, ty + offset + 1)) > 0.0 ? 4 : 0;
                int leftB = (tx == 0 ? Noise.cnoise((4 * offset - 1 - ty) + 0.5f, offset) : Noise.cnoise(tx, ty + offset + 0.5f)) > 0.0 ? 8 : 0;
                noiseL = bottomB + rightB + topB + leftB;
            }
            else if (face == 6)
            {
                int offset = 1 << level;
                int bottomB = (ty == 0 ? Noise.cnoise((3 * offset - 1 - tx) + 0.5f, 0) : Noise.cnoise(tx + 0.5f, ty - offset)) > 0.0 ? 1 : 0;
                int rightB = (tx == offset - 1 ? Noise.cnoise((2 * offset - 1 - ty) + 0.5f, 0) : Noise.cnoise(tx + 1, ty - offset + 0.5f)) > 0.0 ? 2 : 0;
                int topB = Noise.cnoise(tx + 0.5f, ty - offset + 1) > 0.0 ? 4 : 0;
                int leftB = (tx == 0 ? Noise.cnoise(3 * offset + ty + 0.5f, 0) : Noise.cnoise(tx, ty - offset + 0.5f)) > 0.0 ? 8 : 0;
                noiseL = bottomB + rightB + topB + leftB;
            }
            else
            {
                int offset = (1 << level) * (face - 2);
                int bottomB = Noise.cnoise(tx + offset + 0.5f, ty) > 0.0 ? 1 : 0;
                int rightB = Noise.cnoise((tx + offset + 1) % (4 << level), ty + 0.5f) > 0.0 ? 2 : 0;
                int topB = Noise.cnoise(tx + offset + 0.5f, ty + 1) > 0.0 ? 4 : 0;
                int leftB = Noise.cnoise(tx + offset, ty + 0.5f) > 0.0 ? 8 : 0;
                noiseL = bottomB + rightB + topB + leftB;
            }
            int[] noiseRs = new int[16]{ 0, 0, 1, 0, 2, 0, 1, 0, 3, 3, 1, 3, 2, 2, 1, 0 };
            int[] noiseLs = new int[16]{ 0, 1, 1, 2, 1, 3, 2, 4, 1, 2, 3, 4, 2, 4, 4, 5 };
            int noiseR = noiseRs[noiseL];
            noiseL = noiseLs[noiseL];

            noiseSamplerU.set(noiseTexture);
            noiseUVLHU.set(new Vector4f(noiseR, (noiseR + 1) % 4, noiseL, rs));

            frameBuffer.setClearColor(new Vector4f(0.0f, 0.0f, 0.0f, 255.0f));
            frameBuffer.clear(true, true, true);

            frameBuffer.drawQuad(upsample);

            if (hasLayers())
            {
                frameBuffer.setDepthTest(true, Function.LESS);
                base.doCreateTile(level, tx, ty, data);
                frameBuffer.setDepthTest(false);
                frameBuffer.setDrawBuffer(BufferId.COLOR1);
                frameBuffer.setReadBuffer(BufferId.COLOR1);

                blendCoarseLevelSamplerU.set(demTexture);
                if (blendScaleU != null)
                {
                    blendScaleU.set(1.0f / tileWidth);
                }
                frameBuffer.drawQuad(blend);

                gpuData.copyPixels(frameBuffer, 0, 0, tileWidth, tileWidth);

                frameBuffer.setDrawBuffer(BufferId.COLOR0);
                frameBuffer.setReadBuffer(BufferId.COLOR0);
            }
            else
            {
                gpuData.copyPixels(frameBuffer, 0, 0, tileWidth, tileWidth);
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
            if (level > 0)
            {
                TileCache.Tile t = findTile(level - 1, tx / 2, ty / 2);
                Debug.Assert(t != null);
                putTile(t);
            }

            if (residualTiles != null)
            {
                int tileSize = getCache().getStorage().getTileSize() - 5;
                int residualTileSize = residualTiles.getCache().getStorage().getTileSize() - 5;
                int mod = residualTileSize / tileSize;
                if (residualTiles.hasTile(level, tx / mod, ty / mod))
                {
                    TileCache.Tile t = residualTiles.findTile(level, tx / mod, ty / mod);
                    Debug.Assert(t != null);
                    residualTiles.putTile(t);
                }
            }

            base.stopCreateTile(level, tx, ty);
        }

        protected virtual void swap(ElevationProducer p)
        {
            base.swap(p);
            Std.Swap(ref frameBuffer, ref p.frameBuffer);
            Std.Swap(ref upsample, ref p.upsample);
            Std.Swap(ref blend, ref p.blend);
            Std.Swap(ref residualTiles, ref p.residualTiles);
            Std.Swap(ref demTexture, ref p.demTexture);
            Std.Swap(ref residualTexture, ref p.residualTexture);
            Std.Swap(ref layerTexture, ref p.layerTexture);
            Std.Swap(ref noiseAmp, ref p.noiseAmp);
            Std.Swap(ref residualTile, ref p.residualTile);
            Std.Swap(ref gridMeshSize, ref p.gridMeshSize);
            Std.Swap(ref flipDiagonals, ref p.flipDiagonals);
            Std.Swap(ref old, ref p.old);
            Std.Swap(ref noiseTexture, ref p.noiseTexture);
            Std.Swap(ref tileWSDFU, ref p.tileWSDFU);
            Std.Swap(ref coarseLevelSamplerU, ref p.coarseLevelSamplerU);
            Std.Swap(ref coarseLevelOSLU, ref p.coarseLevelOSLU);
            Std.Swap(ref residualSamplerU, ref p.residualSamplerU);
            Std.Swap(ref residualOSHU, ref p.residualOSHU);
            Std.Swap(ref noiseSamplerU, ref p.noiseSamplerU);
            Std.Swap(ref noiseUVLHU, ref p.noiseUVLHU);
            Std.Swap(ref elevationSamplerU, ref p.elevationSamplerU);
            Std.Swap(ref blendCoarseLevelSamplerU, ref p.blendCoarseLevelSamplerU);
            Std.Swap(ref blendScaleU, ref p.blendScaleU);
        }


        /// <summary>
        /// The amplitude of the noise to be added for each level (one amplitude per level).
        /// </summary>
        private List<float> noiseAmp;

        /// <summary>
        /// A buffer to convert a residual tile produced by #residualTiles to the
        /// appropriate size.
        /// </summary>
        private float[] residualTile;

        /// <summary>
        /// The size of the grid that will be used to render each tile.
        /// Must be the tile size(without borders) divided by a power of two.
        /// </summary>
        private int gridMeshSize;

        /// <summary>
        ///  true if the grid used to render each tile will use diagonal flipping to
        /// reduce geometric aliasing.
        /// </summary>
        private bool flipDiagonals;

        private Texture2DArray noiseTexture;

        private Uniform4f tileWSDFU;

        private UniformSampler coarseLevelSamplerU;

        private Uniform4f coarseLevelOSLU;

        private UniformSampler residualSamplerU;

        private Uniform4f residualOSHU;

        private UniformSampler noiseSamplerU;

        private Uniform4f noiseUVLHU;

        private UniformSampler elevationSamplerU;

        private UniformSampler blendCoarseLevelSamplerU;

        private Uniform1f blendScaleU;

        private static FrameBuffer old;
    }
}