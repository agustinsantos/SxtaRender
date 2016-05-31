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
using System;
using System.Collections.Generic;

namespace Sxta.Proland.Terrain.Ortho
{
    /**
     * An OrthoGPUProducer layer that simply fills tiles with a constant color.
     * This layer is useful when the OrthoGPUProducer is used without an
     * OrthoCPUProducer. It can provide a background color for other layers.
     * @ingroup ortho
     * @authors Eric Bruneton, Antoine Begault
     */
    public class EmptyOrthoLayer : TileLayer, ISwappable<EmptyOrthoLayer>
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /**
         * Creates a new EmptyOrthoLayer.
         *
         * @param color the color to be used to fill the produced tiles.
         */
        public EmptyOrthoLayer(Vector4f color) :
                base("EmptyOrthoLayer")
        {
            base.init(false);
            init(color);
        }

        /**
         * Deletes this EmptyOrthoLayer.
         */
        //public virtual ~EmptyOrthoLayer();

        public virtual bool doCreateTile(int level, int tx, int ty, TileStorage.Slot data)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Empty tile " + getProducerId() + " " + level + " " + tx + " " + ty);
            }

            FrameBuffer fb = SceneManager.getCurrentFrameBuffer();
            fb.setClearColor(color);
            fb.clear(true, false, false);
            fb.setClearColor(Vector4f.Zero);

            return true;
        }



        /**
         * Creates an uninitialized EmptyOrthoLayer.
         */
        protected EmptyOrthoLayer() :
               base("EmptyOrthoLayer")
        {
        }

        /**
         * Initializes this EmptyOrthoLayer.
         *
         * @param color the color to be used to fill the produced tiles.
         */
        protected void init(Vector4f color)
        {
            this.color = color;
        }


        public void swap(EmptyOrthoLayer p)
        {
            base.swap(p);
            Std.Swap(ref color, ref p.color);
        }



        /**
         * The color to be used to fill the produced tiles.
         */
        private Vector4f color;
    }
}
