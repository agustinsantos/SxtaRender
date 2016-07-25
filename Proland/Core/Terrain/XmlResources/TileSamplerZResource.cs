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
using Sxta.Render.Resources;
using System;
using System.Xml;

namespace Sxta.Proland.Core.Terrain.XmlResources
{
    class TileSamplerZResource : ResourceTemplate<TileSamplerZ>
    {
        public static TileSamplerZResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new TileSamplerZResource(manager, name, desc, e);
        }
        public TileSamplerZResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(10, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "id,name,sampler,producer,terrains,storeLeaf,storeParent,storeInvisible,async,");
            string uname = getParameter(desc, e, "sampler");
            TileProducer producer = manager.loadResource(getParameter(desc, e, "producer")).get() as TileProducer;
            this.valueC = new TileSamplerZ(uname, producer);
            string terrainsAtt = e.GetAttribute("terrains");
            if (terrainsAtt != null)
            {
                string nodes = terrainsAtt;
                string[] stringSeparator = new string[] { "," };
                string[] result = nodes.Split(stringSeparator, StringSplitOptions.RemoveEmptyEntries);
                foreach (var node in result)
                {
                    valueC.addTerrain(manager.loadResource(node).get() as TerrainNode);
                }
            }
            if (e.GetAttribute("storeLeaf") != null && e.GetAttribute("storeLeaf") == "false")
            {
                valueC.setStoreLeaf(false);
            }
            if (e.GetAttribute("storeParent") != null && e.GetAttribute("storeParent") == "false")
            {
                valueC.setStoreParent(false);
            }
            if (e.GetAttribute("storeInvisible") != null && e.GetAttribute("storeInvisible") == "false")
            {
                valueC.setStoreInvisible(false);
            }
            if (e.GetAttribute("async") != null && e.GetAttribute("async") == "true")
            {
                valueC.setAsynchronous(true);
            }
        }
    }
}
