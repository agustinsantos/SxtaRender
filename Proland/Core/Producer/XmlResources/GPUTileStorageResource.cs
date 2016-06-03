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

using proland;
using Sxta.Render;
using Sxta.Render.Resources;
using Sxta.Render.Resources.XmlResources;
using System.Xml;

namespace Sxta.Proland.Core.Producer.XmlResources
{
    public class GPUTileStorageResource : ResourceTemplate<GPUTileStorage>
    {
        public static GPUTileStorageResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new GPUTileStorageResource(manager, name, desc, e);
        }
        public GPUTileStorageResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) :
            base(0, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            int tileSize;
            int nTiles;
            TextureInternalFormat tf;
            TextureFormat f;
            PixelType t;
            Texture.Parameters _params = new Texture.Parameters();
            bool useTileMap = false;
            checkParameters(desc, e, "name,tileSize,nTiles,tileMap,internalformat,format,type,min,mag,minLod,maxLod,minLevel,maxLevel,swizzle,anisotropy,");
            TextureResource.getParameters(desc, e, out tf, out f, out t);
            TextureResource.getParameters(desc, e, ref _params);
            getIntParameter(desc, e, "tileSize", out tileSize);
            getIntParameter(desc, e, "nTiles", out nTiles);
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("tileMap")))
            {
                useTileMap = e.GetAttribute("tileMap") == "true"; // == 0???
            }
           this. valueC = new GPUTileStorage(tileSize, nTiles, tf, f, t, _params, useTileMap);

        }
    }
}
