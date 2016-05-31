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
using System;
using System.Diagnostics;
using System.Xml;

namespace Sxta.Proland.Terrain.Ortho.XmlResources
{

    public class TextureLayerResource : ResourceTemplate<TextureLayer>
    {
        public static TextureLayerResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new TextureLayerResource(manager, name, desc, e);
        }
        public TextureLayerResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) :
        base(40, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "name,producer,renderProg,tileSamplerName,storeTiles,equation,sourceFunction,destinationFunction,equationAlpha,sourceFunctionAlpha,destinationFunctionAlpha,level,");

            Object programRessource = manager.loadResource(getParameter(desc, e, "renderProg"));
            Debug.Assert(programRessource != null);
            Program program = programRessource as Program;
            Debug.Assert(program != null);

            TileProducer tiles = manager.loadResource(getParameter(desc, e, "producer")).get() as TileProducer;
            Debug.Assert(tiles != null);

            string tileSamplerName = getParameter(desc, e, "tileSamplerName");
            Debug.Assert(tileSamplerName != "");

            TextureLayer.BlendParams blendParams = new TextureLayer.BlendParams();
            blendParams.rgb = BlendEquation.ADD;
            blendParams.srgb = BlendArgument.SRC_ALPHA;
            blendParams.drgb = BlendArgument.ONE_MINUS_SRC_ALPHA;
            blendParams.alpha = BlendEquation.ADD;
            blendParams.salpha = BlendArgument.SRC_ALPHA;
            blendParams.dalpha = BlendArgument.ONE_MINUS_SRC_ALPHA;
            string c = e.GetAttribute("buffer");
            if (c != null)
            {
                if (c == "COLOR0")
                {
                    blendParams.buffer = BufferId.COLOR0;
                }
                else if (c == "COLOR1")
                {
                    blendParams.buffer = BufferId.COLOR1;
                }
                else if (c == "COLOR2")
                {
                    blendParams.buffer = BufferId.COLOR2;
                }
                else if (c == "COLOR3")
                {
                    blendParams.buffer = BufferId.COLOR3;
                }
                else if (c == "DEPTH")
                {
                    blendParams.buffer = BufferId.DEPTH;
                }
                else if (c == "STENCIL")
                {
                    blendParams.buffer = BufferId.STENCIL;
                }
            }
            if (e.GetAttribute("equation") != null)
            {
                blendParams.rgb = getBlendEquation(desc, e, "equation");
                blendParams.srgb = getBlendArgument(desc, e, "sourceFunction");
                blendParams.drgb = getBlendArgument(desc, e, "destinationFunction");
            }
            if (e.GetAttribute("equationAlpha") != null)
            {
                blendParams.alpha = getBlendEquation(desc, e, "equationAlpha");
                blendParams.salpha = getBlendArgument(desc, e, "sourceFunctionAlpha");
                blendParams.dalpha = getBlendArgument(desc, e, "destinationFunctionAlpha");
            }

            int displayLevel = 0;
            if (e.GetAttribute("level") != null)
            {
                getIntParameter(desc, e, "level", out displayLevel);
            }

            bool storeTiles = false;
            if (e.GetAttribute("storeTiles") != null)
            {
                storeTiles = e.GetAttribute("storeTiles") == "true";
            }

            this.valueC = new TextureLayer(tiles, program, tileSamplerName, blendParams, displayLevel, storeTiles);
        }

        public BlendEquation getBlendEquation(ResourceDescriptor desc, XmlElement e, string name)
        {
            try
            {
                if (e.GetAttribute(name) == "ADD")
                {
                    return BlendEquation.ADD;
                }
                if (e.GetAttribute(name) == "SUBTRACT")
                {
                    return BlendEquation.SUBTRACT;
                }
                if (e.GetAttribute(name) == "REVERSE_SUBTRACT")
                {
                    return BlendEquation.REVERSE_SUBTRACT;
                }
                if (e.GetAttribute(name) == "MIN")
                {
                    return BlendEquation.MIN;
                }
                if (e.GetAttribute(name) == "MAX")
                {
                    return BlendEquation.MAX;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid blend equation. " + ex);
            }
            throw new Exception("Invalid blend equation. ");
        }

        public BlendArgument getBlendArgument(ResourceDescriptor desc, XmlElement e, string name)
        {
            try
            {
                if (e.GetAttribute(name) == "ZERO") 
                    {
                    return BlendArgument.ZERO;
                }
                if (e.GetAttribute(name) == "ONE") 
                    {
                    return BlendArgument.ONE;
                }
                if (e.GetAttribute(name) == "SRC_COLOR") 
                    {
                    return BlendArgument.SRC_COLOR;
                }
                if (e.GetAttribute(name) == "ONE_MINUS_SRC_COLOR") 
                    {
                    return BlendArgument.ONE_MINUS_SRC_COLOR;
                }
                if (e.GetAttribute(name) == "DST_COLOR") 
                    {
                    return BlendArgument.DST_COLOR;
                }
                if (e.GetAttribute(name) == "ONE_MINUS_DST_COLOR") 
                    {
                    return BlendArgument.ONE_MINUS_DST_COLOR;
                }
                if (e.GetAttribute(name) == "SRC_ALPHA") 
                    {
                    return BlendArgument.SRC_ALPHA;
                }
                if (e.GetAttribute(name) == "ONE_MINUS_SRC_ALPHA") 
                    {
                    return BlendArgument.ONE_MINUS_SRC_ALPHA;
                }
                if (e.GetAttribute(name) == "DST_ALPHA")
                    {
                    return BlendArgument.DST_ALPHA;
                }
                if (e.GetAttribute(name) == "ONE_MINUS_DST_ALPHA")
                    {
                    return BlendArgument.ONE_MINUS_DST_ALPHA;
                }
                if (e.GetAttribute(name) == "CONSTANT_COLOR") 
                    {
                    return BlendArgument.CONSTANT_COLOR;
                }
                if (e.GetAttribute(name) == "ONE_MINUS_CONSTANT_COLOR") 
                    {
                    return BlendArgument.ONE_MINUS_CONSTANT_COLOR;
                }
                if (e.GetAttribute(name) == "CONSTANT_ALPHA")
                    {
                    return BlendArgument.CONSTANT_ALPHA;
                }
                if (e.GetAttribute(name) == "ONE_MINUS_CONSTANT_ALPHA")
                    {
                    return BlendArgument.ONE_MINUS_CONSTANT_ALPHA;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid blend argument. " + ex);
            }
            throw new Exception("Invalid blend argument. ");

        }
    }
}
