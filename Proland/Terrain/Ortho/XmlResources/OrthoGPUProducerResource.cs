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
using Sxta.Math;
using Sxta.Render;
using Sxta.Render.Resources;
using System.Diagnostics;
using System.Globalization;
using System.Xml;

namespace Sxta.Proland.Terrain.Ortho.XmlResources
{
    public class OrthoGPUProducerResource : ResourceTemplate<OrthoGPUProducer>
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static OrthoGPUProducerResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new OrthoGPUProducerResource(manager, name, desc, e);
        }
        public OrthoGPUProducerResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) :
        base(40, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            TileCache cache;
            TileCache backgroundCache = null;
            TileProducer ortho = null;
            int maxLevel = -1;
            Texture2D compressedTexture = null;
            Texture2D uncompressedTexture= null;
            checkParameters(desc, e, "name,cache,backgroundCache,ortho,maxLevel,");
            this.valueC = new OrthoGPUProducer();
            cache = manager.loadResource(getParameter(desc, e, "cache")).get() as TileCache;
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("backgroundCache") ))
            {
                backgroundCache = manager.loadResource(getParameter(desc, e, "backgroundCache")).get() as TileCache;
            }
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("ortho") ))
            {
                ortho = manager.loadResource(getParameter(desc, e, "ortho")).get() as TileProducer;
            }
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("maxLevel") ))
            {
                getIntParameter(desc, e, "maxLevel", out maxLevel);
            }

            bool hasLayers = false;
             XmlNode n = e.FirstChild;
            while (n != null)
            {
                XmlElement f = n as XmlElement;
                if (f == null)
                {
                    n = n.NextSibling;
                    continue;
                }

                TileLayer l = manager.loadResource(desc, f).get() as TileLayer ;

                if (l != null)
                {
                    this.valueC.addLayer(l);
                    hasLayers = true;
                }
                else
                {
                    if (log.IsWarnEnabled)
                    {
                        log(log, desc, f, "Unknown scene node element '" + f.Value + "'");
                    }
                }
                n = n.NextSibling;
            }

            if (ortho != null && ((OrthoCPUProducer)ortho ).isCompressed())
            {
                int tileSize = ortho.getCache().getStorage().getTileSize();
                int channels = ((CPUTileStorage < byte > )ortho.getCache().getStorage() ).getChannels();
                Debug.Assert(tileSize == cache.getStorage().getTileSize());
                Debug.Assert(channels >= 3);
                 string compTex = "renderbuffer-" + tileSize + "-" + (channels == 3 ? "COMPRESSED_RGB_S3TC_DXT1_EXT" : "COMPRESSED_RGBA_S3TC_DXT5_EXT");
                compressedTexture = manager.loadResource(compTex ).get() as Texture2D ;
            }

            if ((ortho != null && ((OrthoCPUProducer)ortho ).isCompressed()) || hasLayers)
            {
                int tileSize = cache.getStorage().getTileSize();
                int channels = ((GPUTileStorage)cache.getStorage() ).getTexture(0).getComponents();
                 string uncompTex = "renderbuffer-" + tileSize + "-" + (channels == 3 ? "RGB8" : "RGBA8");
                uncompressedTexture = manager.loadResource(uncompTex).get() as Texture2D;
            }

            Debug.Assert(ortho != null || hasLayers);

            this.valueC.init(cache, backgroundCache, ortho, maxLevel, compressedTexture, uncompressedTexture);
        }
    }
}