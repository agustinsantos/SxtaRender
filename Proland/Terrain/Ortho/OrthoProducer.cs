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
using System.Xml;
using System;
using System.Globalization;

namespace Sxta.Proland.Terrain
{
    /// <summary>
    /// A TileProducer to create texture tiles on GPU from CPU residual tiles.
    /// </summary>
    public class OrthoProducer : TileProducer, ISwappable<OrthoProducer>
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /*
             * Creates a new OrthoProducer.
             *
             * @param cache the cache to store the produced tiles. The underlying
             *      storage must be a GPUTileStorage.
             * @param residualTiles the %producer producing the residual tiles. This
             *      %producer should produce its tiles in a CPUTileStorage of
             *      unsigned ybte type. The size of the residual tiles (without
             *      borders) must be equal to the size of the produced tiles (without
             *      borders). The border sizes must be the same.
             * @param orthoTexture a texture used to produce the tiles. Its size must
             *      be equal to the produced tile size (including borders).
             * @param residualTexture a texture used to produce the tiles. Its size
             *      must be equal to the produced tile size (including borders).
             * @param upsample the Program to perform the upsampling and add
             *      procedure on GPU. See \ref sec-ortho.
             * @param scale scaling factor used for residual values.
             * @param maxLevel maximum quadtree level, or -1 to allow any level.
             */
        public OrthoProducer(TileCache cache, TileProducer residualTiles,
            Texture2D orthoTexture, Texture2D residualTexture,
            Program upsample, Vector4f rootNoiseColor, Vector4f noiseColor,
             List<float> noiseAmp, bool noiseHsv,
            float scale, int maxLevel) : base("OrthoProducer", "CreateOrthoTile")
        {
            init(cache, residualTiles, orthoTexture, residualTexture, upsample, rootNoiseColor, noiseColor, noiseAmp, noiseHsv, scale, maxLevel);
        }

        /**
         * Deletes this OrthoProducer.
         */
        //public virtual ~OrthoProducer();

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


        public override bool hasTile(int level, int tx, int ty)
        {
            return maxLevel == -1 || level <= maxLevel;
        }


        public override bool prefetchTile(int level, int tx, int ty)
        {
            bool b = base.prefetchTile(level, tx, ty);
            if (!b)
            {
                if (residualTiles != null && residualTiles.hasTile(level, tx, ty))
                {
                    residualTiles.prefetchTile(level, tx, ty);
                }
            }
            return b;
        }



        protected FrameBuffer frameBuffer;

        /*
         * The %producer producing the residual tiles. This %producer should produce its
         * tiles in a CPUTileStorage of unsigned byte type. The size of the residual
         * tiles (without borders) must be equal to the size of the produced tiles
         * (without borders).
         */
        protected TileProducer residualTiles;

        /*
         * A texture used to produce the tiles. Its size must be equal to the produced
         * tile size (including borders).
         */
        protected Texture2D orthoTexture;

        /*
         * A texture used to produce the tiles. Its size must be equal to the produced
         * tile size (including borders).
         */
        protected Texture2D residualTexture;

        /*
         * Cube face ID for producers targeting spherical terrains.
         */
        protected int face;

        /*
         * The Program to perform the upsampling and add procedure on GPU.
         * See \ref sec-ortho.
         */
        public Program upsample;

        /*
         * Creates an uninitialized OrthoProducer.
         */
        public OrthoProducer() : base("OrthoProducer", "CreateOrthoTile")
        {
        }

        /*
         * Initializes this OrthoProducer. See #OrthoProducer.
         */
        protected void init(TileCache cache, TileProducer residualTiles,
            Texture2D orthoTexture, Texture2D residualTexture,
            Program upsample, Vector4f rootNoiseColor, Vector4f noiseColor,
            List<float> noiseAmp, bool noiseHsv,
            float scale, int maxLevel)
        {
            int tileWidth = cache.getStorage().getTileSize();
            base.init(cache, true);
            this.frameBuffer = orthoFramebufferFactory.Get(orthoTexture);
            this.residualTiles = residualTiles;
            this.orthoTexture = orthoTexture;
            this.residualTexture = residualTexture;
            this.upsample = upsample;
            this.noiseTexture = orthoNoiseFactory.Get(tileWidth);
            this.rootNoiseColor = rootNoiseColor;
            this.noiseColor = noiseColor;
            this.noiseAmp = noiseAmp;
            this.noiseHsv = noiseHsv;
            this.scale = scale;
            this.maxLevel = maxLevel;

            tileWidthU = upsample.getUniform1f("tileWidth");
            coarseLevelSamplerU = upsample.getUniformSampler("coarseLevelSampler");
            coarseLevelOSLU = upsample.getUniform4f("coarseLevelOSL");
            residualSamplerU = upsample.getUniformSampler("residualSampler");
            residualOSHU = upsample.getUniform4f("residualOSH");
            noiseSamplerU = upsample.getUniformSampler("noiseSampler");
            noiseUVLHU = upsample.getUniform4i("noiseUVLH");
            noiseColorU = upsample.getUniform4f("noiseColor");
            rootNoiseColorU = upsample.getUniform4f("rootNoiseColor");

            if (residualTiles != null)
            {
                TileStorage s = residualTiles.getCache().getStorage();
                channels = ((CPUTileStorage<byte>)s).getChannels();
                Debug.Assert(tileWidth == s.getTileSize());
                Debug.Assert(((GPUTileStorage)cache.getStorage()).getTexture(0).getComponents() >= channels);
            }
            else
            {
                channels = ((GPUTileStorage)cache.getStorage()).getTexture(0).getComponents();
            }
        }

        /**
         * Initializes this OrthoProducer from a Resource.
         *
         * @param manager the manager that will manage the created %resource.
         * @param r the %resource.
         * @param name the %resource name.
         * @param desc the %resource descriptor.
         * @param e an optional XML element providing contextual information (such
         *      as the XML element in which the %resource descriptor was found).
         */
        public void init(ResourceManager manager, Resource r, string name, ResourceDescriptor desc, XmlElement e = null)
        {
            TileCache cache;
            TileProducer residuals = null;
            Program upsampleProg;
            Texture2D orthoTexture;
            Texture2D residualTexture;
            Vector4f rootNoiseColor = new Vector4f(0.5f, 0.5f, 0.5f, 0.5f);
            Vector4f noiseColor = new Vector4f(1.0f, 1.0f, 1.0f, 1.0f);
            List<float> noiseAmp = new List<float>();
            bool noiseHsv = false;
            float scale = 2.0f;
            int maxLevel = -1;
            cache = manager.loadResource(Resource.getParameter(desc, e, "cache")).get() as TileCache;
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("residuals")))
            {
                residuals = manager.loadResource(Resource.getParameter(desc, e, "residuals")).get() as TileProducer;
            }
            string upsample = "upsampleOrthoShader;";
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("upsampleProg")))
            {
                upsample = Resource.getParameter(desc, e, "upsampleProg");
            }
            upsampleProg = manager.loadResource(upsample).get() as Program;
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("rnoise")))
            {
                string c = e.GetAttribute("rnoise") + ",";
                int start = 0;
                int index;
                float[] val = new float[4];
                for (int i = 0; i < 4; i++)
                {
                    index = c.LastIndexOf(',', start);
                    val[i] = float.Parse(c.Substring(start, index - start), CultureInfo.InvariantCulture) / 255;
                    start = index + 1;
                }
                rootNoiseColor = new Vector4f(val[0], val[1], val[2], val[2]);
            }
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("cnoise")))
            {
                string c = e.GetAttribute("cnoise") + ",";
                int start = 0;
                int index;
                float[] val = new float[4];
                for (int i = 0; i < 4; i++)
                {
                    index = c.LastIndexOf(',', start);
                    val[i] = float.Parse(c.Substring(start, index - start), CultureInfo.InvariantCulture) / 255;
                    start = index + 1;
                }
                noiseColor = new Vector4f(val[0], val[1], val[2], val[2]);
            }
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("noise")))
            {
                string noiseAmps = e.GetAttribute("noise") + ",";
                int start = 0;
                int index;
                while ((index = noiseAmps.LastIndexOf(',', start)) != -1)
                {
                    float value;
                    string amp = noiseAmps.Substring(start, index - start);
                    value = float.Parse(amp, CultureInfo.InvariantCulture);
                    noiseAmp.Add(value);
                    start = index + 1;
                }
            }
            if (e.GetAttribute("hsv") != null && e.GetAttribute("hsv") == "true")
            {
                noiseHsv = true;
            }
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("scale")))
            {
                Resource.getFloatParameter(desc, e, "scale", out scale);
            }
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("maxLevel")))
            {
                Resource.getIntParameter(desc, e, "maxLevel", out maxLevel);
            }
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("face")))
            {
                Resource.getIntParameter(desc, e, "face", out face);
            }
            else if (name[name.Length - 1] >= '1' && name[name.Length - 1] <= '6')
            {
                face = name[name.Length - 1] - '0';
            }
            else
            {
                face = 1;
            }

            XmlNode n = e.FirstChild;
            while (n != null)
            {
                XmlElement f = n as XmlElement;
                if (f == null)
                {
                    n = n.NextSibling;
                    continue;
                }

                TileLayer l = manager.loadResource(desc, f).get() as TileLayer;
                if (l != null)
                {
                    addLayer(l);
                }
                else
                {
                    if (log.IsWarnEnabled)
                    {
                        Resource.log(log, desc, f, "Unknown scene node element '" + f + "'");
                    }
                }
                n = n.NextSibling;
            }

            int tileWidth = cache.getStorage().getTileSize();
            int channels = hasLayers() ? 4 : ((GPUTileStorage)cache.getStorage()).getTexture(0).getComponents();

            string orthoTex = "renderbuffer-" + tileWidth;
            switch (channels)
            {
                case 1:
                    orthoTex += "-R8";
                    break;
                case 2:
                    orthoTex += "-RG8";
                    break;
                case 3:
                    orthoTex += "-RGB8";
                    break;
                default:
                    orthoTex += "-RGBA8";
                    break;
            }
            orthoTexture = manager.loadResource(orthoTex).get() as Texture2D;

            string residualTex = "renderbuffer-" + tileWidth;
            switch (((GPUTileStorage)cache.getStorage()).getTexture(0).getComponents())
            {
                case 1:
                    residualTex += "-R8";
                    break;
                case 2:
                    residualTex += "-RG8";
                    break;
                case 3:
                    residualTex += "-RGB8";
                    break;
                default:
                    residualTex += "-RGBA8";
                    break;
            }
            residualTex += "-1";
            residualTexture = manager.loadResource(residualTex).get() as Texture2D;

            init(cache, residuals, orthoTexture, residualTexture, upsampleProg, rootNoiseColor, noiseColor, noiseAmp, noiseHsv, scale, maxLevel);
        }

        protected internal override ulong getContext()
        {
            return (uint)orthoTexture.GetHashCode();
        }


        protected internal override Task startCreateTile(int level, int tx, int ty, uint deadline, Task task, TaskGraph owner)
        {
            TaskGraph result = owner == null ? createTaskGraph(task) : owner;

            if (level > 0)
            {
                TileCache.Tile t = getTile(level - 1, tx / 2, ty / 2, deadline);
                Debug.Assert(t != null);
                result.addTask(t.task);
                result.addDependency(task, t.task);
            }

            if (residualTiles != null && residualTiles.hasTile(level, tx, ty))
            {
                TileCache.Tile t = residualTiles.getTile(level, tx, ty, deadline);
                Debug.Assert(t != null);
                result.addTask(t.task);
                result.addDependency(task, t.task);
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
                log.Debug("Ortho tile " + getId() + " " + level + " " + tx + " " + ty);
            }

            GPUTileStorage.GPUSlot gpuData = (GPUTileStorage.GPUSlot)data;
            Debug.Assert(gpuData != null);
            ((GPUTileStorage)getCache().getStorage()).notifyChange(gpuData);

            int tileWidth = data.getOwner().getTileSize();
            int tileSize = tileWidth - 4;

            GPUTileStorage.GPUSlot parentGpuData = null;
            if (level > 0)
            {
                TileCache.Tile t = findTile(level - 1, tx / 2, ty / 2);
                Debug.Assert(t != null);
                parentGpuData = (GPUTileStorage.GPUSlot)(t.getData());
                Debug.Assert(parentGpuData != null);
            }

            tileWidthU.set((float)tileWidth);

            if (level > 0)
            {
                Texture t = parentGpuData.t;
                float dx = (float)((tx % 2) * (tileSize / 2));
                float dy = (float)((ty % 2) * (tileSize / 2));
                coarseLevelSamplerU.set(t);
                coarseLevelOSLU.set(new Vector4f((dx + 0.5f) / parentGpuData.getWidth(), (dy + 0.5f) / parentGpuData.getHeight(), 1.0f / parentGpuData.getWidth(), GPUTileStorage.GPUSlot.l));
            }
            else
            {
                coarseLevelOSLU.set(new Vector4f(-1.0f, -1.0f, -1.0f, -1.0f));
            }

            if (residualTiles != null && residualTiles.hasTile(level, tx, ty))
            {
                residualSamplerU.set(residualTexture);
                residualOSHU.set(new Vector4f(0.5f / tileWidth, 0.5f / tileWidth, 1.0f / tileWidth, scale));

                TileCache.Tile t = residualTiles.findTile(level, tx, ty);
                Debug.Assert(t != null);
                CPUTileStorage<byte>.CPUSlot cpuTile = (CPUTileStorage<byte>.CPUSlot)(t.getData());
                Debug.Assert(cpuTile != null);

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
                residualTexture.setSubImage(0, 0, 0, tileWidth, tileWidth, f, PixelType.UNSIGNED_BYTE, new Render.Buffer.Parameters(), new CPUBuffer<byte>(cpuTile.data));
            }
            else
            {
                residualOSHU.set(new Vector4f(-1.0f, -1.0f, -1.0f, -1.0f));
            }

            float rs = level < (int)(noiseAmp.Count) ? noiseAmp[level] : 0.0f;

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
            int[] noiseRs = new int[16] { 0, 0, 1, 0, 2, 0, 1, 0, 3, 3, 1, 3, 2, 2, 1, 0 };
            int[] noiseLs = new int[16] { 0, 1, 1, 2, 1, 3, 2, 4, 1, 2, 3, 4, 2, 4, 4, 5 };
            int noiseR = noiseRs[noiseL];
            noiseL = noiseLs[noiseL];

            noiseSamplerU.set(noiseTexture);
            noiseUVLHU.set(new Vector4i(noiseR, (noiseR + 1) % 4, noiseL, noiseHsv ? 1 : 0));
            if (noiseHsv)
            {
                noiseColorU.set(Vector4f.Multiply(new Vector4f(noiseColor), new Vector4f(rs, rs, rs, scale * rs)) / 255.0f);
            }
            else
            {
                noiseColorU.set(noiseColor * scale * rs / 255.0f);
            }
            if (rootNoiseColorU != null)
            {
                rootNoiseColorU.set(rootNoiseColor);
            }

            frameBuffer.drawQuad(upsample);
            if (hasLayers())
            {
                base.doCreateTile(level, tx, ty, data);
            }
            gpuData.copyPixels(frameBuffer, 0, 0, tileWidth, tileWidth);

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

            if (residualTiles != null && residualTiles.hasTile(level, tx, ty))
            {
                TileCache.Tile t = residualTiles.findTile(level, tx, ty);
                Debug.Assert(t != null);
                residualTiles.putTile(t);
            }

            base.stopCreateTile(level, tx, ty);
        }

        public virtual void swap(OrthoProducer p)
        {
            base.swap(p);
            Std.Swap(ref frameBuffer, ref p.frameBuffer);
            Std.Swap(ref residualTiles, ref p.residualTiles);
            Std.Swap(ref orthoTexture, ref p.orthoTexture);
            Std.Swap(ref residualTexture, ref p.residualTexture);
            //Std.Swap(ref old, ref p.old);
            Std.Swap(ref upsample, ref p.upsample);
            Std.Swap(ref channels, ref p.channels);
            Std.Swap(ref maxLevel, ref p.maxLevel);
            Std.Swap(ref noiseTexture, ref p.noiseTexture);
            Std.Swap(ref rootNoiseColor, ref p.rootNoiseColor);
            Std.Swap(ref noiseColor, ref p.noiseColor);
            Std.Swap(ref noiseAmp, ref p.noiseAmp);
            Std.Swap(ref noiseHsv, ref p.noiseHsv);
            Std.Swap(ref scale, ref p.scale);
            Std.Swap(ref tileWidthU, ref p.tileWidthU);
            Std.Swap(ref coarseLevelSamplerU, ref p.coarseLevelSamplerU);
            Std.Swap(ref coarseLevelOSLU, ref p.coarseLevelOSLU);
            Std.Swap(ref residualSamplerU, ref p.residualSamplerU);
            Std.Swap(ref residualOSHU, ref p.residualOSHU);
            Std.Swap(ref noiseSamplerU, ref p.noiseSamplerU);
            Std.Swap(ref noiseUVLHU, ref p.noiseUVLHU);
            Std.Swap(ref noiseColorU, ref p.noiseColorU);
            Std.Swap(ref rootNoiseColorU, ref p.rootNoiseColorU);
        }


        /**
         * The number of components per pixel in the CPU residual tiles.
         */
        private int channels;

        /**
         * Maximum quadtree level, or -1 to allow any level.
         */
        private int maxLevel;

        private Texture2DArray noiseTexture;

        private Vector4f rootNoiseColor;

        private Vector4f noiseColor;

        private List<float> noiseAmp = new List<float>();

        private bool noiseHsv;

        /**
         * Scaling factor used for residual values.
         */
        private float scale;

        private Uniform1f tileWidthU;

        private UniformSampler coarseLevelSamplerU;

        private Uniform4f coarseLevelOSLU;

        private UniformSampler residualSamplerU;

        private Uniform4f residualOSHU;

        private UniformSampler noiseSamplerU;

        private Uniform4i noiseUVLHU;

        private Uniform4f noiseColorU;

        private Uniform4f rootNoiseColorU;

        private static FrameBuffer old;

        private static Texture2DArray createOrthoNoise(int tileWidth)
        {
#if TODO
            int[] layers = new int[6] { 0, 1, 3, 5, 7, 15 };
            byte[] noiseArray = new byte[6 * tileWidth * tileWidth * 4];
            long rand = 1234567;
            for (int nl = 0; nl < 6; ++nl)
            {
                byte* n = noiseArray + nl * tileWidth * tileWidth * 4;
                int l = layers[nl];
                // corners
                for (int v = 0; v < tileWidth; ++v)
                {
                    for (int h = 0; h < tileWidth; ++h)
                    {
                        for (int c = 0; c < 4; ++c)
                        {
                            n[4 * (h + v * tileWidth) + c] = 128;
                        }
                    }
                }
                long brand;
                // bottom border
                brand = (l & 1) == 0 ? 7654321 : 5647381;
                for (int v = 2; v < 4; ++v)
                {
                    for (int h = 4; h < tileWidth - 4; ++h)
                    {
                        for (int c = 0; c < 4; ++c)
                        {
                            int N = (int)(Noise.frandom(ref brand) * 255.0f);
                            n[4 * (h + v * tileWidth) + c] = N;
                            n[4 * (tileWidth - 1 - h + (3 - v) * tileWidth) + c] = N;
                        }
                    }
                }
                // right border
                brand = (l & 2) == 0 ? 7654321 : 5647381;
                for (int h = tileWidth - 3; h >= tileWidth - 4; --h)
                {
                    for (int v = 4; v < tileWidth - 4; ++v)
                    {
                        for (int c = 0; c < 4; ++c)
                        {
                            int N = (int)(Noise.frandom(ref brand) * 255.0f);
                            n[4 * (h + v * tileWidth) + c] = N;
                            n[4 * (2 * tileWidth - 5 - h + (tileWidth - 1 - v) * tileWidth) + c] = N;
                        }
                    }
                }
                // top border
                brand = (l & 4) == 0 ? 7654321 : 5647381;
                for (int v = tileWidth - 2; v < tileWidth; ++v)
                {
                    for (int h = 4; h < tileWidth - 4; ++h)
                    {
                        for (int c = 0; c < 4; ++c)
                        {
                            int N = (int)(Noise.frandom(ref brand) * 255.0f);
                            n[4 * (h + v * tileWidth) + c] = N;
                            n[4 * (tileWidth - 1 - h + (2 * tileWidth - 5 - v) * tileWidth) + c] = N;
                        }
                    }
                }
                // left border
                brand = (l & 8) == 0 ? 7654321 : 5647381;
                for (int h = 1; h >= 0; --h)
                {
                    for (int v = 4; v < tileWidth - 4; ++v)
                    {
                        for (int c = 0; c < 4; ++c)
                        {
                            int N = (int)(Noise.frandom(ref brand) * 255.0f);
                            n[4 * (h + v * tileWidth) + c] = N;
                            n[4 * (3 - h + (tileWidth - 1 - v) * tileWidth) + c] = N;
                        }
                    }
                }
                // center
                for (int v = 4; v < tileWidth - 4; ++v)
                {
                    for (int h = 4; h < tileWidth - 4; ++h)
                    {
                        for (int c = 0; c < 4; ++c)
                        {
                            n[4 * (h + v * tileWidth) + c] = (int)(Noise.frandom(ref rand) * 255.0f);
                        }
                    }
                }
            }
            Texture2DArray noiseTexture = new Texture2DArray(tileWidth, tileWidth, 6, TextureInternalFormat.RGBA8,
                                                            TextureFormat.RGBA, PixelType.UNSIGNED_BYTE, 
                                                            new Texture.Parameters().wrapS(TextureWrap.REPEAT).wrapT(TextureWrap.REPEAT).min(TextureFilter.NEAREST).mag(TextureFilter.NEAREST), 
                                                            new Buffer.Parameters(), new CPUBuffer<byte>(noiseArray));
            //delete[] noiseArray;
            return noiseTexture;
#endif 
            throw new NotImplementedException();
        }

        private static Factory<int, Texture2DArray> orthoNoiseFactory = new Factory<int, Texture2DArray>(createOrthoNoise);

        private static FrameBuffer createOrthoFramebuffer(Texture2D orthoTexture)
        {
            int tileWidth = orthoTexture.getWidth();
            FrameBuffer frameBuffer = new FrameBuffer();
            frameBuffer.setReadBuffer(BufferId.COLOR0);
            frameBuffer.setDrawBuffer(BufferId.COLOR0);
            frameBuffer.setViewport(new Vector4i(0, 0, tileWidth, tileWidth));
            frameBuffer.setTextureBuffer(BufferId.COLOR0, orthoTexture, 0);
            frameBuffer.setPolygonMode(PolygonMode.FILL, PolygonMode.FILL);
            return frameBuffer;
        }

        private static Factory<Texture2D, FrameBuffer> orthoFramebufferFactory = new Factory<Texture2D, FrameBuffer>(createOrthoFramebuffer);
    }
}



















