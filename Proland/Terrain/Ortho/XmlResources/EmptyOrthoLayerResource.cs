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
using Sxta.Render.Resources;
using System.Globalization;
using System.Xml;

namespace Sxta.Proland.Terrain.Ortho.XmlResources
{
    public class EmptyOrthoLayerResource : ResourceTemplate<EmptyOrthoLayer>
    {
        public static EmptyOrthoLayerResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new EmptyOrthoLayerResource(manager, name, desc, e);
        }
        public EmptyOrthoLayerResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) :
        base(40, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            Vector4f color = new Vector4f(1.0f, 1.0f, 1.0f, 1.0f);

            checkParameters(desc, e, "name,color,");

            if (!string.IsNullOrWhiteSpace(e.GetAttribute("color") ))
            {
                string c = getParameter(desc, e, "color") + ",";
                int start = 0;
                int index;
                float[] val = new float[4];
                for (int i = 0; i < 3; i++)
                {
                    index = c.IndexOf(',', start);
                    val[i] = float.Parse(c.Substring(start, index - start), CultureInfo.InvariantCulture) / 255;
                    start = index + 1;
                }
                color = new Vector4f(val[0], val[1], val[2], val[3]);
            }

            this.valueC = new EmptyOrthoLayer(color);
        }

        public override bool prepareUpdate()
        {
            oldValue = null;
            newDesc = null;

            return true;
        }
    }
}