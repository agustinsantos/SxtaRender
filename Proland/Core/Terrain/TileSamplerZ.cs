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

using Sxta.Core;
using Sxta.Math;
using Sxta.Render;
using Sxta.Render.Scenegraph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace proland
{
    class TileSamplerZ : TileSampler
    {
        int MAX_MIPMAP_PER_FRAME = 16;
        const string minmaxShader = @"
uniform vec4 viewport; // size in pixels and one over size in pixels
#ifdef _VERTEX_
layout(location=0) in vec4 vertex;
void main() {
    gl_Position = vec4((vertex.xy + vec2(1.0))///viewport.xy - vec2(1.0), 0.0, 1.0);
}
#endif
#ifdef _FRAGMENT_
uniform vec3 sizes; // size of parent and current tiles in pixels, pass
uniform ivec4 tiles[32];
uniform sampler2DArray inputs[8];
uniform sampler2D input_;
layout(location=0) out vec4 data;
void main() {
    vec2 r[16];
    vec2 ij = floor(gl_FragCoord.xy);
    if (sizes.z == 0.0) {
        ivec4 tile = tiles[int(floor(ij.x / sizes.y))];
        vec4 uv = (tile.z == 0 && tile.w == 0) ? vec4(vec2(2.5) + 4.0///mod(ij, sizes.yy), vec2(sizes.x - 2.5)) : tile.zwzw + vec4(0.5);
        vec4 u = min(vec4(uv.x, uv.x + 1.0, uv.x + 2.0, uv.x + 3.0), uv.zzzz) / sizes.x;
        vec4 v = min(vec4(uv.y, uv.y + 1.0, uv.y + 2.0, uv.y + 3.0), uv.wwww) / sizes.x;
        switch (tile.x) {
        case 0:
            for (int i = 0; i < 16; ++i) {
                r[i] = textureLod(inputs[0], vec3(u[i/4], v[i%4], tile.y), 0.0).zz;
            }
            break;
        case 1:
            for (int i = 0; i < 16; ++i) {
                r[i] = textureLod(inputs[1], vec3(u[i/4], v[i%4], tile.y), 0.0).zz;
            }
            break;
        case 2:
            for (int i = 0; i < 16; ++i) {
                r[i] = textureLod(inputs[2], vec3(u[i/4], v[i%4], tile.y), 0.0).zz;
            }
            break;
        case 3:
            for (int i = 0; i < 16; ++i) {
                r[i] = textureLod(inputs[3], vec3(u[i/4], v[i%4], tile.y), 0.0).zz;
            }
            break;
        case 4:
            for (int i = 0; i < 16; ++i) {
                r[i] = textureLod(inputs[4], vec3(u[i/4], v[i%4], tile.y), 0.0).zz;
            }
            break;
        case 5:
            for (int i = 0; i < 16; ++i) {
                r[i] = textureLod(inputs[5], vec3(u[i/4], v[i%4], tile.y), 0.0).zz;
            }
            break;
        case 6:
            for (int i = 0; i < 16; ++i) {
                r[i] = textureLod(inputs[6], vec3(u[i/4], v[i%4], tile.y), 0.0).zz;
            }
            break;
        case 7:
            for (int i = 0; i < 16; ++i) {
                r[i] = textureLod(inputs[7], vec3(u[i/4], v[i%4], tile.y), 0.0).zz;
            }
            break;
        }
    } else {
        vec2 tile = floor(ij / sizes.y);
        vec2 uvmax = vec2(tile///sizes.x + vec2(sizes.x - 0.5));
        vec2 uv = vec2(0.5) + tile///sizes.x + 4.0///(ij - tile///sizes.y);
        vec4 u = min(vec4(uv.x, uv.x + 1.0, uv.x + 2.0, uv.x + 3.0), uvmax.xxxx)///viewport.z;
        vec4 v = min(vec4(uv.y, uv.y + 1.0, uv.y + 2.0, uv.y + 3.0), uvmax.yyyy)///viewport.w;
        for (int i = 0; i < 16; ++i) {
            r[i] = textureLod(input_, vec2(u[i/4], v[i%4]), 0.0).xy;
        }
    }
    vec2 s0 = vec2(min(r[0].x, r[1].x), max(r[0].y, r[1].y));
    vec2 s1 = vec2(min(r[2].x, r[3].x), max(r[2].y, r[3].y));
    vec2 s2 = vec2(min(r[4].x, r[5].x), max(r[4].y, r[5].y));
    vec2 s3 = vec2(min(r[6].x, r[7].x), max(r[6].y, r[7].y));
    vec2 s4 = vec2(min(r[8].x, r[9].x), max(r[8].y, r[9].y));
    vec2 s5 = vec2(min(r[10].x, r[11].x), max(r[10].y, r[11].y));
    vec2 s6 = vec2(min(r[12].x, r[13].x), max(r[12].y, r[13].y));
    vec2 s7 = vec2(min(r[14].x, r[15].x), max(r[14].y, r[15].y));
    vec2 t0 = vec2(min(s0.x, s1.x), max(s0.y, s1.y));
    vec2 t1 = vec2(min(s2.x, s3.x), max(s2.y, s3.y));
    vec2 t2 = vec2(min(s4.x, s5.x), max(s4.y, s5.y));
    vec2 t3 = vec2(min(s6.x, s7.x), max(s6.y, s7.y));
    vec2 u0 = vec2(min(t0.x, t1.x), max(t0.y, t1.y));
    vec2 u1 = vec2(min(t2.x, t3.x), max(t2.y, t3.y));
    data = vec4(min(u0.x, u1.x), max(u0.y, u1.y), 0.0, 0.0);
}
#endif";

        /// <summary>
        ///Creates a new TileSamplerZ.
        ///
        ///@param name the GLSL name of this uniform.
        ///@param producer the producer to be used to create new tiles in #update.
        ///     Maybe null if this producer is used to update a tileMap (see
        ///     #setTileMap). If not null, must have a
        ///     proland.GPUTileStorage.
        /// </summary>
        public TileSamplerZ(string name, TileProducer producer = null) : base(name, producer)
        {

        }
        // ~TileSamplerZ() { Debugger.Break(); }

        public override Sxta.Render.Scenegraph.Task update(SceneManager scene, TerrainQuad root)
        {
            Sxta.Render.Scenegraph.Task result = base.update(scene, root);
            uint frameNumber = scene.getFrameNumber();

            if ((root.getOwner().getLocalCamera() - oldLocalCamera).Length > 0.1 && cameraQuad != null && cameraQuad.t != null)
            {
                GPUTileStorage.GPUSlot gpuTile = (GPUTileStorage.GPUSlot)(cameraQuad.t.getData(false));
                if (gpuTile != null && state.cameraSlot == null)
                {
                    int border = get().getBorder();
                    int tileSize = get().getCache().getStorage().getTileSize() - 2 * border;
                    int dx = Math.Min((int)(Math.Floor(cameraQuadCoords.X * tileSize)), tileSize - 1);
                    int dy = Math.Min((int)(Math.Floor(cameraQuadCoords.Y * tileSize)), tileSize - 1);
                    Debug.Assert(border == 2);
                    state.cameraSlot = gpuTile;
                    state.cameraOffset = new Vector2i(dx + border, dy + border);
                    oldLocalCamera = root.getOwner().getLocalCamera();
                }
            }
            cameraQuad = null;

            if (frameNumber == state.lastFrame)
            {
                return result;
            }
            state.tileReadback.newFrame();
            state.lastFrame = frameNumber;

            List<GPUTileStorage.GPUSlot> gpuTiles = new List<GPUTileStorage.GPUSlot>();
            List<TerrainQuad> targets = new List<TerrainQuad>();
            Vector2i camera = new Vector2i(0, 0);

            if (state.cameraSlot != null)
            {
                gpuTiles.Add(state.cameraSlot);
                targets.Add(null);
                camera = state.cameraOffset;
                state.cameraSlot = null;
            }

            // TODO DEPURAR DANI 
            while (state.needReadback.Count > 0 && gpuTiles.Count() < MAX_MIPMAP_PER_FRAME)
            {
                TreeZ t = state.needReadback.First();
                state.needReadback.Remove(t);
                TileCache.Tile tile = t.t;

                if (tile != null)
                {
                    GPUTileStorage.GPUSlot gpuTile = (GPUTileStorage.GPUSlot)(tile.getData(false));
                    if (gpuTile != null)
                    {
                        gpuTiles.Add(gpuTile);
                        targets.Add(t.q);
                    }
                    else
                    {
                        t.readback = false;
                    }
                }
            }

            if (!gpuTiles.Any())
            {
                return result;
            }

            int pass = 0;
            int count = (int)(gpuTiles.Count());
            int parentSize = state.storage.getTileSize();
            int currentSize = (parentSize - 4) / 4 + (parentSize % 4 == 0 ? 0 : 1);
            Vector2f viewportSize = new Vector2f(1.0f / (MAX_MIPMAP_PER_FRAME * currentSize), 1.0f / currentSize);

            if (camera != Vector2i.Zero && count == 1)
            {
                state.viewportU.set(new Vector4f(viewportSize.X, viewportSize.Y, viewportSize.X, viewportSize.Y));
                state.sizesU.set(new Vector3f(parentSize, currentSize, pass));
                state.tileU[0].set(new Vector4i(gpuTiles[0].getIndex(), GPUTileStorage.GPUSlot.l, camera.X, camera.Y));
                state.fbo.setDrawBuffer(state.readBuffer);
                state.fbo.drawQuad(state.minmaxProg);
            }
            else
            {
                state.viewportU.set(new Vector4f(count * currentSize * viewportSize.X, currentSize * viewportSize.Y, viewportSize.X, viewportSize.Y));
                state.sizesU.set(new Vector3f(parentSize, currentSize, pass));
                for (int j = 0; j < count; ++j)
                {
                    if (j == 0)
                    {
                        state.tileU[j].set(new Vector4i(gpuTiles[j].getIndex(), GPUTileStorage.GPUSlot.l, camera.X, camera.Y));
                    }
                    else
                    {
                        state.tileU[j].set(new Vector4i(gpuTiles[j].getIndex(), GPUTileStorage.GPUSlot.l, 0, 0));
                    }
                }
                state.fbo.setDrawBuffer(BufferId.COLOR0);
                state.fbo.drawQuad(state.minmaxProg);
                while (currentSize != 1)
                {
                    parentSize = currentSize;
                    currentSize = currentSize / 4 + (currentSize % 4 == 0 ? 0 : 1);
                    pass += 1;
                    state.viewportU.set(new Vector4f(count * currentSize * viewportSize.X, currentSize * viewportSize.Y, viewportSize.X, viewportSize.Y));
                    state.sizesU.set(new Vector3f(parentSize, currentSize, pass));
                    state.inputU.set(state.fbo.getTextureBuffer(pass % 2 == 0 ? BufferId.COLOR1 : BufferId.COLOR0));
                    state.fbo.setDrawBuffer(pass % 2 == 0 ? BufferId.COLOR0 : BufferId.COLOR1);
                    state.fbo.drawQuad(state.minmaxProg);
                }
            }

            Debug.Assert(state.tileReadback.canReadback());
            state.tileReadback.readback(state.fbo, 0, 0, MAX_MIPMAP_PER_FRAME, 1, TextureFormat.RG, PixelType.FLOAT, new TileCallback(targets, camera != Vector2i.Zero));

            return result;
        }


        /// <summary>
        ///Creates an uninitialized TileSamplerZ.
        /// </summary>
        protected TileSamplerZ() : base()
        {

        }

        /// <summary>
        /// Initializes this TileSamplerZ.
        /// </summary>
        /// <param name="name">the GLSL name of this uniform.</param>
        /// <param name="producer">the %producer to be used to create new tiles in #update.
        /// Maybe null if this %producer is used to update a tileMap(see
        /// #setTileMap). If not null, must have a
        /// proland.GPUTileStorage.</param>
        internal override void init(string name, TileProducer producer = null)
        {
            base.init(name, producer);
            factory = stateFactory;
            state = factory.Get((GPUTileStorage)get().getCache().getStorage());
            cameraQuad = null;
            oldLocalCamera = Vector3d.Zero;
        }

        protected override bool needTile(TerrainQuad q)
        {
            Vector3d c = q.getOwner().getLocalCamera();
            if (c.X >= q.ox && c.X < q.ox + q.l && c.Y >= q.oy && c.Y < q.oy + q.l)
            {
                return true;
            }
            return base.needTile(q);
        }

        internal override void getTiles(Tree parent, Tree t, TerrainQuad q, TaskGraph result)
        {
            if (t == null)
            {
                t = new TreeZ(parent, q);
                (t).needTile = needTile(q);
                if (q.level == 0 && get().getRootQuadSize() == 0.0f)
                {
                    get().setRootQuadSize((float)q.l);
                }
            }
            if ((t).t != null && (t).t.task.isDone() && (!((TreeZ)(t)).readback || ((TreeZ)(t)).readbackDate < (t).t.task.getCompletionDate()))
            {
                state.needReadback.Add((TreeZ)t);
                ((TreeZ)(t)).readback = true;
                ((TreeZ)(t)).readbackDate = (t).t.task.getCompletionDate();
            }

           base.getTiles(parent, t, q, result);

            if (cameraQuad == null && (t).t != null && (t).t.task.isDone())
            {
                Vector3d c = q.getOwner().getLocalCamera();
                if (c.X >= q.ox && c.X < q.ox + q.l && c.Y >= q.oy && c.Y < q.oy + q.l)
                {
                    cameraQuadCoords = new Vector2f((float)((c.X - q.ox) / q.l), (float)((c.Y - q.oy) / q.l));
                    cameraQuad = (TreeZ)(t);
                }
            }
        }


        /// <summary>
        ///An internal quadtree to store the texture tile associated with each
        ///%terrain quad, and to keep track of tiles that need to be read back.
        /// </summary>
        internal class TreeZ : Tree
        {

            /// <summary>
            ///The TerrainQuad whose zmin and zmax values must be updated.
            /// </summary>
            public TerrainQuad q;

            /// <summary>
            ///True if the elevation values of this tile have been read back.
            /// </summary>
            public bool readback;

            /// <summary>
            ///Completion date of the elevation tile data at the time of the
            ///last read back. This is used to trigger a new read back if the
            ///elevation data changes.
            /// </summary>
            public uint readbackDate;

            /// <summary>
            ///Creates a new TreeZ.
            ///
            ///@param q a %terrain quad.
            /// </summary>
            internal TreeZ(Tree parent, TerrainQuad q) : base(parent)
            {
                this.parent = parent;
                this.q = q;
                readback = false;
                readbackDate = 0;
            }
            //TODO
            public override void recursiveDelete(TileSampler owner)
            {
                //(TileSamplerZ)(owner).state.needReadback.erase(this);
                //Tree.recursiveDelete(owner);
            }
            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }
                TreeZ TreeZ2 = obj as TreeZ;
                if (this.q.level == TreeZ2.q.level)
                {
                    return true;
                }

                return false;
            }
            // override object.GetHashCode
            public override int GetHashCode()
            {
                return this.q.level;
            }
        }

        /// <summary>
        ///A sort operator to sort TreeZ elements. It is used to read back
        ///coarsest tiles first (i.e. those whose level is minimal).
        /// </summary>

        private class TreeZSort : IComparer<TreeZ>
        {
            public int Compare(TreeZ x, TreeZ y)
            {
                int xLevel = x.q.level;
                int yLevel = y.q.level;

                return xLevel - yLevel;
            }
        }
        /// <summary>
        ///A ork.ReadbackManager.Callback to readback an
        ///elevation tile and to update the zmin and zmax fields
        ///of a TerrainQuad.
        /// </summary>
        private class TileCallback : ReadbackManager.Callback
        {

            /// <summary>
            ///The TerrainQuad(s) whose zmin and zmax values must be read back.
            /// </summary>
            public List<TerrainQuad> targets;

            /// <summary>
            ///True if the first element in target is the quad under the camera.
            ///If so its zmin and zmax are used to update TerrainNode.groundHeightAtCamera
            /// </summary>
            private bool camera;

            /// <summary>
            ///Creates a new ZTileCallback.
            ///
            ///@param targets the TerrainQuad(s) whose zmin and zmax values
            ///     must be read back.
            ///@param camera true if the first element in target is the quad
            ///     under the camera.
            /// </summary>
            internal TileCallback(List<TerrainQuad> targets, bool camera) : base()
            {
                this.targets = targets;
                this.camera = camera;
            }

            public virtual void dataRead(/**volatile**/ Object data)
            {
                float[] values = (float[])data;
                int i = 0;
                if (camera)
                {
                    TerrainNode.groundHeightAtCamera = TerrainNode.nextGroundHeightAtCamera;
                    TerrainNode.nextGroundHeightAtCamera = values[0];
                    i = 1;
                }
                for (; i < targets.Count(); ++i)
                {
                    targets[i].zmin = values[2 * i];
                    targets[i].zmax = values[2 * i + 1];
                }
            }
        }

        /// <summary>
        /// A state object to share the readback managers between all
        /// TileSamplerZ instances with the same tile storage.
        /// </summary>
        public class State
        {
            /// <summary>
            ///The tile storage for which this State is built.
            /// </summary>
            public GPUTileStorage storage;

            /// <summary>
            ///Framebuffer object used to compute zmin and zmax of tiles.
            /// </summary>
            public FrameBuffer fbo;

            /// <summary>
            ///Buffer of FBO used to read back the computed zmin and zmax values.
            /// </summary>
            public BufferId readBuffer;

            /// <summary>
            ///The custom "mipmapping" program used to compute min and max
            ///elevation values over a tile.
            /// </summary>
            public Program minmaxProg;


            public Uniform4f viewportU;

            public Uniform3f sizesU;

            public List<Uniform4i> tileU;

            public UniformSampler inputU;

            /// <summary>
            ///The readback manager used to perform asynchronous readbacks.
            /// </summary>
            public ReadbackManager tileReadback;

            /// <summary>
            ///The set of texture tile that need to be read back.
            /// </summary>

            public SortedSet<TreeZ> needReadback = new SortedSet<TreeZ>(new TreeZSort());

            /// <summary>
            ///The slot of #storage corresponding to the quad below the camera.
            /// </summary>
            public GPUTileStorage.GPUSlot cameraSlot;

            /// <summary>
            ///Relative offset in #cameraSlot of the pixel under the camera.
            ///The value of this pixel is readback into TerrainNode.groundHeightAtCamera.
            /// </summary>
            public Vector2i cameraOffset;

            /// <summary>
            ///The last frame for which a readback was performed. This is used
            ///to avoid doing more readbacks per frame than specified in the
            ///readback managers, when several TileSamplerZ sharing the
            ///same State are used.
            /// </summary>
            public uint lastFrame;

            /// <summary>
            ///Creates a new State for the given tile storage.
            /// </summary>
            public State(GPUTileStorage storage)
            {
                this.storage = storage;
                cameraSlot = null;
                lastFrame = 0;
            }
        };

        /// <summary>
        ///The ork.Factory to create shared State objects for a given
        ///tile storage.
        /// </summary>
        public Factory<GPUTileStorage, State> factory;

        /// <summary>
        ///The State object for this TileSamplerZ (may be shared
        ///with other TileSamplerZ with the same tile storage).
        /// </summary>
        public State state;

        /// <summary>
        ///The %terrain quad directly below the current viewer position.
        /// </summary>
        protected TileSamplerZ.TreeZ cameraQuad;

        /// <summary>
        ///The relative viewer position in the #cameraQuad quad.
        /// </summary>
        public Vector2f cameraQuadCoords;

        /// <summary>
        ///Last camera position used to perform a readback of the camera elevation
        ///above the ground. This is used to avoid reading back this value at each
        ///frame when the camera does not move.
        /// </summary>
        public Vector3d oldLocalCamera;

        /// <summary>
        ///Creates a new State for elevation tiles of the given size.
        /// </summary>
        /// <param name="storage">an elevation tile size.</param>
        /// <returns></returns>
        private static State newState(GPUTileStorage storage)
        {
            return new State(storage);
        }

        /// <summary>
        ///The ork.Factory to create shared State objects for a given
        ///elevation tile size.
        /// </summary>
        private static Factory<GPUTileStorage, State> stateFactory = new Factory<GPUTileStorage, TileSamplerZ.State>(TileSamplerZ.newState);
    }
}

