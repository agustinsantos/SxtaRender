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
using Sxta.Proland.Core.Terrain.XmlResources;
using Sxta.Render;
using Sxta.Render.Resources;
using Sxta.Render.Scenegraph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace proland
{
    public class DrawTerrainTask : AbstractTask, ISwappable<DrawTerrainTask>
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>
        /// Creates a new DrawTerrainTask.
        /// </summary>
        /// <param name="terrain">the %terrain to be drawn. The first part of this "node.name"
        /// qualified name specifies the scene node containing the TerrainNode
        /// field.The second part specifies the name of this TerrainNode field.</param>
        /// <param name="mesh">the mesh to be used to draw each quad. The first part of this
        /// "node.name" qualified name specifies the scene node containing the mesh
        /// field.The second part specifies the name of this mesh field.</param>
        /// <param name="culling">culling true to draw only visible leaf quads, false to draw all
        /// leaf quads.</param>
        public DrawTerrainTask(QualifiedName terrain, QualifiedName mesh, bool culling) : base("DrawTerrainTask")
        {
            init(terrain, mesh, culling);
        }



        public override Sxta.Render.Scenegraph.Task getTask(Object context)
        {
            Method N = (Method)context;
            SceneNode n = N.getOwner();

            TerrainNode t = new TerrainNode();
            SceneNode target = terrain.getTarget(n);
            if (target == null)
            {
                t = (TerrainNode)n.getOwner().getResourceManager().loadResource(terrain.name).get();
            }
            else
            {
                TerrainNodeResource tr = (TerrainNodeResource)target.getField(terrain.name);
                t = tr.get() as TerrainNode;
            }
            if (t == null)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("DrawTerrain : cannot find terrain '" + terrain.target + "." + terrain.name + "'");
                }
                throw new Exception("DrawTerrain : cannot find terrain '" + terrain.target + "." + terrain.name + "'");
            }

            MeshBuffers m = null;
            target = mesh.getTarget(n);
            if (target == null)
            {
                m = n.getOwner().getResourceManager().loadResource(mesh.name + ".mesh").get() as MeshBuffers;
            }
            else
            {
                m = target.getMesh(mesh.name);
            }
            if (m == null)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("DrawMesh : cannot find mesh '" + mesh.target + "." + mesh.name + "'");
                }
                throw new Exception("DrawMesh : cannot find mesh '" + mesh.target + "." + mesh.name + "'");
            }
            return new Impl(n, t, m, culling);
        }


        /// <summary>
        /// Creates an uninitialized DrawTerrainTask.
        /// </summary>
        public DrawTerrainTask() : base("DrawTerrainTask")
        {
        }

        /// <summary>
        /// Initializes this DrawTerrainTask.
        /// </summary>
        /// <param name="terrain">the %terrain to be drawn. The first part of this "node.name"
        /// qualified name specifies the scene node containing the TerrainNode
        /// field.The second part specifies the name of this TerrainNode field.</param>
        /// <param name="mesh">the mesh to be used to draw each quad. The first part of this
        /// "node.name" qualified name specifies the scene node containing the mesh
        /// field.The second part specifies the name of this mesh field.</param>
        /// <param name="culling">true to draw only visible leaf quads, false to draw all
        /// leaf quads.</param>
        public void init(QualifiedName terrain, QualifiedName mesh, bool culling)
        {
            this.terrain = terrain;
            this.mesh = mesh;
            this.culling = culling;
        }

        public void swap(DrawTerrainTask t)
        {
            DrawTerrainTask _this = this;
            Std.Swap(ref _this, ref t);
        }


        /// <summary>
        /// The %terrain to be drawn. The first part of this "node.name"
        /// qualified name specifies the scene node containing the TerrainNode
        /// field.The second part specifies the name of this TerrainNode field.
        /// </summary>
        private QualifiedName terrain;

        /// <summary>
        /// The mesh to be drawn for each %terrain quad. The first part of this
        /// "node.name" qualified name specifies the scene node containing the mesh
        /// field.The second part specifies the name of this mesh field.
        /// </summary>
        private QualifiedName mesh;

        /// <summary>
        /// True to draw only visible leaf quads, false to draw all leaf quads.
        /// </summary>
        private bool culling;

        /// <summary>
        /// A Task to draw a %terrain.
        /// </summary>
        private class Impl : Sxta.Render.Scenegraph.Task
        {

            /// <summary>
            /// The SceneNode describing the %terrain position and its associated
            /// data(via TileSampler fields).
            /// </summary>
            public SceneNode n;

            /// <summary>
            /// The TerrainNode describing the %terrain and its quadtree.
            /// </summary>
            public TerrainNode t;

            /// <summary>
            /// The mesh to be drawn for each leaf quad.
            /// </summary>
            public MeshBuffers m;

            /// <summary>
            /// True to draw only visible leaf quads, false to draw all leaf quads.
            /// </summary>
            public bool culling;

            /// <summary>
            /// True if one the TileSampler associated with this terrain
            /// uses the asynchronous mode.
            /// </summary>
            public bool async;

            /// <summary>
            /// Number of primitives (triangles, lines, etc) per *quarter* of
            /// the grid mesh.Used to draw only parts of the mesh to fill holes
            /// when using asynchronous mode.
            /// </summary>
            public int gridSize;

            /// <summary>
            /// Creates a new Impl.
            /// </summary>
            /// <param name="n">the SceneNode describing the %terrain position.</param>
            /// <param name="t">the TerrainNode describing the %terrain and its quadtree.</param>
            /// <param name="m">the mesh to be drawn for each leaf quad.</param>
            /// <param name="culling">true to draw only visible leaf quads.</param>
            public Impl(SceneNode n, TerrainNode t, MeshBuffers m, bool culling) : base("DrawTerrain", true, 0)
            {
                this.n = n;
                this.t = t;
                this.m = m;
                this.culling = culling;
            }


            public override bool run()
            {
                try
                {
                    if (t != null)
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("DrawTerrain");
                        }
                        FrameBuffer fb = SceneManager.getCurrentFrameBuffer();
                        async = false;
                        List<TileSampler> uniforms = new List<TileSampler>();
                        //SceneNode.FieldIterator i = n.getFields();
                        foreach (KeyValuePair<string, object> i in n.getFields())
                        {
                            TileSampler u = ((Resource)i.Value).get() as TileSampler; //TODO Dani to review this casting ?
                            if (u != null)
                            {
                                if (u.getTerrain(0) != null)
                                {
                                    u.setTileMap();
                                }
                                if (u.getStoreLeaf() && u.getTerrain(0) == null)
                                {
                                    uniforms.Add(u);
                                    if (u.getAsync() && !u.getMipMap())
                                    {
                                        async = true;
                                    }
                                }
                            }
                        }

                        Program p = SceneManager.getCurrentProgram();
                        t.deform.setUniforms(n, t, p);
                        if (async)
                        {
                            int k = 0;
                            switch (m.mode)
                            {
                                case MeshMode.TRIANGLES:
                                    k = 6;
                                    break;
                                case MeshMode.TRIANGLES_ADJACENCY:
                                    k = 12;
                                    break;
                                case MeshMode.LINES_ADJACENCY:
                                case MeshMode.PATCHES:
                                    k = 4;
                                    break;
                                //TOSEE
                                default:
                                    // unsupported formats
                                    Debug.Assert(false);
                                    break;
                            }
                            int n = (int)(Math.Sqrt((double)m.nvertices)) - 1;
                            gridSize = (n / 2) * (n / 2) * k;
                            Debug.Assert(m.nindices >= gridSize * 32);

                            findDrawableQuads(t.root, uniforms);
                        }
                        drawQuad(t.root, uniforms);
                    }
                    return true;
                }
                catch (Exception e)
                {
                    log.ErrorFormat("Exception at DrawTerrain {0}.", e);
                    return false;
                }
            }

            /// <summary>
            /// Finds the quads whose associated tiles are ready (this may not be
            /// the case of all quads if async is true).
            /// </summary>
            /// <param name="q">the %terrain quadtree to be drawn.</param>
            /// <param name="uniforms">the TileSampler associated with the %terrain.</param>
            public void findDrawableQuads(TerrainQuad q, List<TileSampler> uniforms)
            {
                q.drawable = false;

                if (culling && q.visible == SceneManager.visibility.INVISIBLE)
                {
                    q.drawable = true;
                    return;
                }

                if (q.isLeaf())
                {
                    for (int i = 0; i < uniforms.Count; ++i)
                    {
                        if (!uniforms[i].getAsync() || uniforms[i].getMipMap())
                        {
                            continue;
                        }
                        TileProducer p = uniforms[i].get();
                        if (p.hasTile(q.level, q.tx, q.ty) && p.findTile(q.level, q.tx, q.ty) == null)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    int nDrawable = 0;
                    for (int i = 0; i < 4; ++i)
                    {
                        findDrawableQuads(q.children[i], uniforms);
                        if (q.children[i].drawable)
                        {
                            ++nDrawable;
                        }
                    }
                    if (nDrawable < 4)
                    {
                        for (int i = 0; i < uniforms.Count; ++i)
                        {
                            if (!uniforms[i].getAsync() || uniforms[i].getMipMap())
                            {
                                continue;
                            }
                            TileProducer p = uniforms[i].get();
                            if (p.hasTile(q.level, q.tx, q.ty) && p.findTile(q.level, q.tx, q.ty) == null)
                            {
                                return;
                            }
                        }
                    }
                }

                q.drawable = true;
            }

            /// <summary>
            /// Draw the mesh #m for the leaf quads of the given quadtree. Before drawing each
            /// quad, this method calls Deformation#setUniforms for this quad, and calls
            /// TileSampler#setTile on each TileSampler the given uniforms vector.
            /// </summary>
            /// <param name="q">the %terrain quadtree to be drawn.</param>
            /// <param name="uniforms">the TileSampler associated with the %terrain.</param>
            public void drawQuad(TerrainQuad q, List<TileSampler> uniforms)
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("DrawTerrain.DrawQuad. Quad {0},{1},{2}, visibility:{3}", q.level, q.tx, q.ty, q.visible);
                }

                if (culling && q.visible == SceneManager.visibility.INVISIBLE)
                {
                    return;
                }
                if (async && !q.drawable)
                {
                    return;
                }

                Program p = SceneManager.getCurrentProgram();
                if (q.isLeaf())
                {
                    for (int i = 0; i < uniforms.Count; ++i)
                    {
                        uniforms[i].setTile(q.level, q.tx, q.ty);
                    }
                    t.deform.setUniforms(n, q, p);
                    if (async)
                    {
                        SceneManager.getCurrentFrameBuffer().draw(p, m, m.mode, 0, gridSize * 4);
                    }
                    else
                    {
                        if (m.nindices == 0)
                        {
                            if (log.IsDebugEnabled)
                            {
                                log.DebugFormat("DrawTerrain.DrawQuad. Draw mode:{0}, nvertices:{1}", m.mode, m.nvertices);
                            }
                            SceneManager.getCurrentFrameBuffer().draw(p, m, m.mode, 0, m.nvertices);
                        }
                        else
                        {
                            if (log.IsDebugEnabled)
                            {
                                log.DebugFormat("DrawTerrain.DrawQuad. Draw mode:{0}, nindices:{1}", m.mode, m.nindices);
                            }
                            SceneManager.getCurrentFrameBuffer().draw(p, m, m.mode, 0, m.nindices);
                        }
                    }
                }
                else
                {
                    int[] order = new int[4];
                    double ox = t.getLocalCamera().X;
                    double oy = t.getLocalCamera().Y;

                    double cx = q.ox + q.l / 2.0;
                    double cy = q.oy + q.l / 2.0;
                    if (oy < cy)
                    {
                        if (ox < cx)
                        {
                            order[0] = 0;
                            order[1] = 1;
                            order[2] = 2;
                            order[3] = 3;
                        }
                        else
                        {
                            order[0] = 1;
                            order[1] = 0;
                            order[2] = 3;
                            order[3] = 2;
                        }
                    }
                    else
                    {
                        if (ox < cx)
                        {
                            order[0] = 2;
                            order[1] = 0;
                            order[2] = 3;
                            order[3] = 1;
                        }
                        else
                        {
                            order[0] = 3;
                            order[1] = 1;
                            order[2] = 2;
                            order[3] = 0;
                        }
                    }

                    int done = 0;
                    for (int i = 0; i < 4; ++i)
                    {
                        if (culling && q.children[order[i]].visible == SceneManager.visibility.INVISIBLE)
                        {
                            done |= (1 << order[i]);
                        }
                        else if (!async || q.children[order[i]].drawable)
                        {
                            drawQuad(q.children[order[i]], uniforms);
                            done |= (1 << order[i]);
                        }
                    }
                    if (done < 15)
                    {
                        int[] sizes = new int[16] { 0, 4, 7, 10, 12, 15, 17, 19, 20, 23, 25, 27, 28, 30, 31, 32 };
                        for (int i = 0; i < uniforms.Count; ++i)
                        {
                            uniforms[i].setTile(q.level, q.tx, q.ty);
                        }
                        t.deform.setUniforms(n, q, p);
                        if (log.IsDebugEnabled)
                        {
                            log.DebugFormat("DrawTerrain.DrawQuad. Draw mode:{0}, primitives:{1}", m.mode, gridSize * sizes[done], gridSize * (sizes[done + 1] - sizes[done]));
                        }
                        SceneManager.getCurrentFrameBuffer().draw(p, m, m.mode, gridSize * sizes[done], gridSize * (sizes[done + 1] - sizes[done]));
                    }

                }
            }
        }
    }
}

