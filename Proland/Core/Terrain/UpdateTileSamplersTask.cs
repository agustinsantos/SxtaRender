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
using Sxta.Core;
using Sxta.Render.Resources;
using Sxta.Render.Scenegraph;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace proland
{
    public class UpdateTileSamplersTask : AbstractTask, ISwappable<UpdateTileSamplersTask>
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Creates a new UpdateTileSamplersTask.
        /// </summary>
        /// <param name="terrain">The %terrain whose uniforms must be updated. The first part
        /// of this "node.name" qualified name specifies the scene node containing
        /// the TerrainNode field. The second part specifies the name of this
        /// TerrainNode field.</param>
        public UpdateTileSamplersTask(QualifiedName terrain) : base("UpdateTileSamplersTask")
        {
            init(terrain);
        }

        //TODO
        public override Sxta.Render.Scenegraph.Task getTask(Object context)
        {
            Method N = (Method)context;
            SceneNode n = N.getOwner();
            SceneNode target = terrain.getTarget(n);
            TerrainNode t = new TerrainNode();
            if (target == null)
            {
                t = n.getOwner().getResourceManager().loadResource(terrain.name).get() as TerrainNode;
            }
            else
            {
                t = ((Resource)target.getField(terrain.name)).get() as TerrainNode;
            }
            if (t == null)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("UpdateTileSamplers : cannot find terrain '" + terrain.target + "." + terrain.name + "'");
                }
                throw new Exception("UpdateTileSamplers : cannot find terrain '" + terrain.target + "." + terrain.name + "'");
            }
            TaskGraph result = new TaskGraph();
            //SceneNode.FieldIterator i = n.getFields();
            //while (i.hasNext())
            foreach (KeyValuePair<string, object> i in n.getFields())
            {
                //TOSEE
                TileSampler u = ((Resource)i.Value).get() as TileSampler;
                if (u != null)
                {
                    Sxta.Render.Scenegraph.Task ut = u.update(n.getOwner(), t.root);
                    if ((TaskGraph)ut == null || !((TaskGraph)(ut)).isEmpty())
                    {
                        result.addTask(ut);
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Creates an uninitialized UpdateTileSamplersTask.
        /// </summary>
        public UpdateTileSamplersTask() : base("UpdateTileSamplersTask")
        {

        }

        /// <summary>
        /// Initializes this UpdateTileSamplersTask.
        /// </summary>
        /// <param name="terrain">The %terrain whose uniforms must be updated. The first part
        /// of this "node.name" qualified name specifies the scene node containing
        /// the TerrainNode field. The second part specifies the name of this
        /// TerrainNode field.</param>
        public void init(QualifiedName terrain)
        {
            this.terrain = terrain;
        }

        public void swap(UpdateTileSamplersTask t)
        {
            UpdateTileSamplersTask _this = this;
            Std.Swap(ref _this, ref t);
        }


        /**
         * The %terrain whose uniforms must be updated. The first part of this "node.name"
         * qualified name specifies the scene node containing the TerrainNode
         * field. The second part specifies the name of this TerrainNode field.
         */
        private QualifiedName terrain;
    }
}
