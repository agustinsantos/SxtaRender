﻿/*
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
using Sxta.Render.Resources;
using Sxta.Render.Scenegraph;
using System;
using System.Reflection;
using System.Xml;

namespace Sxta.Proland.Core.Producer.XmlResources
{

    class TileCacheResource : ResourceTemplate<TileCache>
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static TileCacheResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new TileCacheResource(manager, name, desc, e);
        }
        public TileCacheResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) :
        base(1, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            TileStorage storage = null;
            Scheduler scheduler;
            checkParameters(desc, e, "name,storage,scheduler,");
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("storage")))
            {
                string _id = getParameter(desc, e, "storage");
                storage = manager.loadResource(_id).get() as TileStorage;
            }
            else
            {
                XmlNode n = e.FirstChild;
                if (n == null)
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error("Missing storage attribute or subelement");
                    }
                    throw new Exception("Missing storage attribute or subelement");
                }
                XmlElement f = n as XmlElement;
                Resource tileStorageResource = ResourceFactory.getInstance().create(manager, f.Name, desc, f);
                storage = tileStorageResource.get() as TileStorage;
                tileStorageResource.clearValue(false);
            }
            string id = getParameter(desc, e, "scheduler");
            scheduler = manager.loadResource(id).get() as Scheduler;
            this.valueC = new TileCache(storage, name, scheduler);
        }
    }
}
