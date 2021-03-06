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
using Sxta.Core;
using Sxta.Math;
using Sxta.Render;
using Sxta.Render.Scenegraph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace proland
{
    /// <summary>
    /// An abstract %producer of tiles.
    /// Note that several TileProducer can share the same TileCache, and hence the
    /// same TileStorage.
    /// </summary>
    public class TileProducer : IDisposable
    {
        /// <summary>
        /// Creates a new TileProducer.
        /// </summary>
        /// <param name="type">the type of this %producer.</param>
        /// <param name="taskType">the type of the Task that produce the actual tile data.</param>
        /// <param name="cache">the tile cache that stores the tiles produced by this %producer.</param>
        /// <param name="gpuProducer">true if this %producer produces textures on GPU.</param>
        public TileProducer(string type, string taskType, TileCache cache, bool gpuProducer)
        {
            this.taskType = taskType;
            init(cache, gpuProducer);
        }

        #region IDisposable implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected bool m_Disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    //Debugger.Break();
                    Debug.Assert(cache != null);
                    Debug.Assert(cache.producers.ContainsKey(id));
                    cache.producers.Remove(id);
                    for (int i = 0; i < tasks.Count; i++)
                    {
                        CreateTile t = tasks[i] as CreateTile;
                        if (t != null)
                        {
                            t.Dispose();
                            //t.owner = null;
                        }
                        else
                        {
                            ((CreateTileTaskGraph)tasks[i]).Dispose();
                            //((CreateTileTaskGraph)tasks[i]).owner = null;
                        }
                        
                    }
                    layers.Clear();
                    if (tileMap != null)
                    {
                        tileMap = null;
                        //delete[] tileMap;
                    }
                    //pthread_mutex_destroy((pthread_mutex_t*)mutex);
                    //delete (pthread_mutex_t*) mutex;                }

                    cache.Dispose();
                    // Unmanaged resources are released here.

                    m_Disposed = true;
                }
            }
        }
        #endregion

        /// Deletes this TileProducer.
        ~TileProducer()
        {
            Dispose(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>the size in meters of the root quad produced by this %producer.</returns>
        public float getRootQuadSize()
        {
            return rootQuadSize;
        }

        /// <summary>
        /// Sets the size in meters of the root quad produced by this %producer.
        /// </summary>
        /// <param name="size">the size of the root quad of this %producer.</param>
        public virtual void setRootQuadSize(float size)
        {
            rootQuadSize = size;
            for (int i = 0; i < (int)layers.Count; i++)
            {
                layers[i].setCache(cache, id);
                layers[i].setTileSize(cache.getStorage().getTileSize(), getBorder(), getRootQuadSize());
            }
        }

        /// <summary>
        /// Returns the id of this %producer. This id is local to the TileCache used by
        /// this %producer, and is used to distinguish all the producers that use this
        /// cache.
        /// </summary>
        /// <returns></returns>
        public int getId()
        {
            return id;
        }

        ///<summary>
        /// Returns the TileCache that stores the tiles produced by this %producer.
        ///</summary>
        public virtual TileCache getCache()
        {
            return cache;
        }

        ///<summary>
        /// Returns true if this %producer produces textures on GPU.
        ///</summary>
        public bool isGpuProducer()
        {
            return gpuProducer;
        }

        ///<summary>
        /// Returns the size in pixels of the border of each tile. Tiles made of
        /// raster data may have a border that contains the valueC of the neighboring
        /// pixels of the tile. For instance if the tile size (returned by
        /// TileStorage#getTileSize) is 196, and if the tile border is 2, this means
        /// that the actual tile data is 192x192 pixels, with a 2 pixel border that
        /// contains the valueC of the neighboring pixels. Using a border introduces
        /// data redundancy but is usefull to get the valueC of the neighboring pixels
        /// of a tile without needing to load the neighboring tiles.
        ///</summary>
        public virtual int getBorder()
        {
            return 0;
        }

        ///<summary>
        /// Returns true if this %producer can produce the given tile.
        ///
        /// @param level the tile's quadtree level.
        /// @param tx the tile's quadtree x coordinate.
        /// @param ty the tile's quadtree y coordinate.
        ///</summary>
        public virtual bool hasTile(int level, int tx, int ty)
        {
            return true;
        }

        ///<summary>
        /// Returns true if this %producer can produce the children of the given tile.
        ///
        /// @param level the tile's quadtree level.
        /// @param tx the tile's quadtree x coordinate.
        /// @param ty the tile's quadtree y coordinate.
        ///</summary>
        public bool hasChildren(int level, int tx, int ty)
        {
            return hasTile(level + 1, 2 * tx, 2 * ty);
        }

        ///<summary>
        /// Looks for a tile in the TileCache of this TileProducer.
        ///
        /// @param level the tile's quadtree level.
        /// @param tx the tile's quadtree x coordinate.
        /// @param ty the tile's quadtree y coordinate.
        /// @param includeCache true to include both used and unused tiles in the
        ///      search, false to include only the used tiles.
        /// @param done true to check that the tile's creation task is done.
        /// @return the requested tile, or null if it is not in the TileCache or
        ///      if 'done' is true, if it is not ready. This method does not change the
        ///      number of users of the returned tile.
        ///</summary>
        public virtual TileCache.Tile findTile(int level, int tx, int ty, bool includeCache = false, bool done = false)
        {
            TileCache.Tile t = cache.findTile(id, level, tx, ty, includeCache);
            if (done && t != null && !t.task.isDone())
            {
                t = null;
            }
            return t;
        }

        ///<summary>
        /// Returns the requested tile, creating it if necessary. If the tile is
        /// currently in use it is returned directly. If it is in cache but unused,
        /// it marked as used and returned. Otherwise a new tile is created, marked
        /// as used and returned. In all cases the number of users of this tile is
        /// incremented by one.
        ///
        /// @param level the tile's quadtree level.
        /// @param tx the tile's quadtree x coordinate.
        /// @param ty the tile's quadtree y coordinate.
        /// @param deadline the deadline at which the tile data must be ready. 0 means
        ///      the current frame.
        /// @return the requested tile, or null if there is no room left in the
        ///      TileStorage to store the requested tile.
        ///</summary>
        public virtual TileCache.Tile getTile(int level, int tx, int ty, uint deadline)
        {
            int users = 0;
            TileCache.Tile t = cache.getTile(id, level, tx, ty, deadline, users);
            if (users == 0)
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    layers[i].useTile(level, tx, ty, deadline);
                }
            }
            return t;
        }

        ///<summary>
        /// Returns the coordinates in the GPU storage of the given tile. If the
        /// given tile is not in the GPU storage, this method uses the first ancestor
        /// of this tile that is in the storage. It then returns the coordinates of
        /// the area of this ancestor tile that correspond to the requested tile.
        ///
        /// @param level the tile's quadtree level.
        /// @param tx the tile's quadtree x coordinate.
        /// @param ty the tile's quadtree y coordinate.
        /// @param[in,out] tile the tile (level,tx,ty) or its first ancestor that is
        ///     in the storage, if known. If no tile is specified (passing null) this
        ///     method finds it and returns it in this parameter.
        /// @return the coordinates in the GPU storage texture of the requested tile.
        ///     The x,y components correspond to the lower left corner of the tile in
        ///     the GPU storage texture. The z,w components correspond to the width
        ///     and height of the tile. All components are in texture coordinates,
        ///     between 0 and 1. If the tile has borders, the returned coordinates
        ///     correspond to the inside of the tile, excluding its border.
        ///</summary>
        public Vector4f getGpuTileCoords(int level, int tx, int ty, ref TileCache.Tile tile)
        {
            Debug.Assert(isGpuProducer());
            TileStorage storage = getCache().getStorage();
            int s = storage.getTileSize();
            int b = getBorder();
            float dx = 0;
            float dy = 0;
            float dd = 1;
            float ds0 = (s / 2) * 2.0f - 2.0f * b;
            float ds = ds0;
            while (!hasTile(level, tx, ty))
            {
                dx += (tx % 2) * dd;
                dy += (ty % 2) * dd;
                dd *= 2;
                ds /= 2;
                level -= 1;
                tx /= 2;
                ty /= 2;
                Debug.Assert(level >= 0);
            }
            TileCache.Tile t = tile == null ? findTile(level, tx, ty, true, true) : null;
            while (tile == null ? t == null : level != (tile).level)
            {
                dx += (tx % 2) * dd;
                dy += (ty % 2) * dd;
                dd *= 2;
                ds /= 2;
                level -= 1;
                tx /= 2;
                ty /= 2;
                Debug.Assert(level >= 0);
                t = tile == null ? findTile(level, tx, ty, true, true) : null;
            }
            dx = dx * ((s / 2) * 2 - 2 * b) / dd;
            dy = dy * ((s / 2) * 2 - 2 * b) / dd;
            if (tile == null)
            {
                tile = t;
            }
            else
            {
                t = tile;
            }

            GPUTileStorage.GPUSlot gput = (GPUTileStorage.GPUSlot)(t.getData());
            Debug.Assert(gput != null);

            float w = gput.getWidth();
            float h = gput.getHeight();
            Debug.Assert(w == h);

            if (s % 2 == 0)
            {
                return new Vector4f((dx + b) / w, (dy + b) / h, (float)(gput.l), ds / w);
            }
            else
            {
                return new Vector4f((dx + b + 0.5f) / w, (dy + b + 0.5f) / h, (float)(gput.l), ds / w);
            }
        }

        ///<summary>
        /// Schedules a prefetch task to create the given tile. If the requested tile
        /// is currently in use or in cache but unused, this method does nothing.
        /// Otherwise it gets an unused tile storage (evicting an unused tile if
        /// necessary), and then creates and schedules a task to produce the data of
        /// the requested tile.
        ///
        /// @param level the tile's quadtree level.
        /// @param tx the tile's quadtree x coordinate.
        /// @param ty the tile's quadtree y coordinate.
        /// @return true if this method has been able to schedule a prefetch task
        ///      for the given tile.
        ///</summary>
        public virtual bool prefetchTile(int level, int tx, int ty)
        {
            if (cache.getScheduler() != null && cache.getScheduler().supportsPrefetch(isGpuProducer()))
            {
                Task task = cache.prefetchTile(id, level, tx, ty);
                if (task != null)
                {
                    cache.getScheduler().schedule(task);
                    return true;
                }
            }
            for (int i = 0; i < layers.Count; i++)
            {
                layers[i].prefetchTile(level, tx, ty);
            }
            return false;
        }

        ///<summary>
        /// Decrements the number of users of this tile by one. If this number
        /// becomes 0 the tile is marked as unused, and so can be evicted from the
        /// cache at any moment.
        ///
        /// @param t a tile currently in use.
        ///</summary>
        public virtual void putTile(TileCache.Tile t)
        {
            if (cache.putTile(t) == 0)
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    layers[i].unuseTile(t.level, t.tx, t.ty);
                }
            }
        }

        ///<summary>
        /// Invalidates the tiles produced by this producer.
        /// This means that the tasks to produce the actual data of these tiles will
        /// be automatically reexecuted before the data can be used.
        ///</summary>
        public virtual void invalidateTiles()
        {
            cache.invalidateTiles(id);
        }

        ///<summary>
        /// Invalidates the selected tile produced by this producer.
        /// This means that the tasks to produce the actual data of this tile will
        /// be automatically reexecuted before the data can be used.
        ///</summary>
        public virtual void invalidateTile(int level, int tx, int ty)
        {
            getCache().invalidateTile(getId(), level, tx, ty);
        }

        ///<summary>
        /// Updates the tiles produced by this producer, if necessary.
        /// The default implementation of this method does nothing.
        ///</summary>
        public virtual void update(SceneManager scene) { }

        ///<summary>
        /// Updates the GPU tile map for this %producer. A GPU tile map allows a GPU
        /// shader to retrieve the data of any tile from its level,tx,ty coordinates,
        /// thanks to a mapping between these coordinates and texture tile storage
        /// u,v coordinates, stored in the tile map. This is only possible if the
        /// quadtree of tiles is subdivided only based on the distance to the camera.
        /// The camera position and the subdivision parameters are needed to create
        /// the tile map and to decode it on GPU.
        ///
        /// @param splitDistance the distance at which a quad is subdivided. In fact
        ///      a quad is supposed to be subdivided if the camera distance is less
        ///      than splitDistance times the quad size (in meters).
        /// @param camera the camera position. This position, together with
        ///      splitDistance and rootQuadSize completely define the current
        ///      quadtree (i.e. which quads are subdivided, and which are not).
        /// @param maxLevel the maximum subdivision level of the quadtree (included).
        /// @return true if the tile storage for this %producer has a tile map (see
        ///      GPUTileStorage#getTileMap). Otherwise this method does nothing.
        ///</summary>
        public bool updateTileMap(float splitDistance, Vector2f camera, int maxLevel)
        {
            Debug.Assert(isGpuProducer());
            GPUTileStorage gpuStorage = getCache().getStorage() as GPUTileStorage;
            Texture2D tileMapT = gpuStorage.getTileMap();
            if (tileMapT == null)
            {
                return false;
            }
            if (tileMap == null)
            {
                tileMap = new byte[2 * tileMapT.getWidth()];
            }
            Debug.Assert(rootQuadSize != 0.0);
            camera.X += rootQuadSize / 2;
            camera.Y += rootQuadSize / 2;
            int k = (int)System.Math.Ceiling(splitDistance);

            if (k > 2)
            {
                bool collisions = false;
                Debug.Assert(tileMapT.getWidth() >= 4093);
                for (int i = 0; i < tileMapT.getWidth(); ++i)
                {
                    tileMap[2 * i] = 0;
                    tileMap[2 * i + 1] = 0;
                }
                for (int l = 0; l <= maxLevel; ++l)
                {
                    float tileSize = rootQuadSize / (1 << l);
                    int tx0 = (int)System.Math.Floor(camera.X / (2 * tileSize));
                    int ty0 = (int)System.Math.Floor(camera.Y / (2 * tileSize));
                    for (int ty = 2 * (ty0 - k); ty <= 2 * (ty0 + k) + 1; ++ty)
                    {
                        for (int tx = 2 * (tx0 - k); tx <= 2 * (tx0 + k) + 1; ++tx)
                        {
                            GPUTileStorage.GPUSlot gpuData = null;
                            TileCache.Tile t;
                            if (tx >= 0 && tx < (1 << l) && ty >= 0 && ty < (1 << l))
                            {
                                t = findTile(l, tx, ty);
                            }
                            else
                            {
                                t = null;
                            }
                            if (t != null)
                            {
                                TileStorage.Slot data = t.getData(false);
                                if (data != null)
                                {
                                    gpuData = data as GPUTileStorage.GPUSlot;
                                }
                                // hash code key for (level,tx,ty) tile
                                // TODO hash code key collisions not handled!
                                int key = (tx + ty * (1 << l) + ((1 << (2 * l)) - 1) / 3) % 4093;

                                byte x, y;
                                if (gpuData == null)
                                {
                                    x = 0;
                                    y = 0;
                                }
                                else
                                {
                                    x = (byte)(gpuData.l % 256);
                                    y = (byte)(gpuData.l / 256 + 1);
                                }
                                if (y != 0 && tileMap[2 * key + 1] != 0)
                                {
                                    collisions = true;
                                }
                                tileMap[2 * key] = x;
                                tileMap[2 * key + 1] = y;
                            }
                        }
                    }
                }
                if (collisions && log.IsWarnEnabled)
                {
                    log.Warn("TILEMAP COLLISIONS DETECTED (NOT SUPPORTED YET)");
                }
                tileMapT.setSubImage(0, 0, id, tileMapT.getWidth(), 1, TextureFormat.RG, PixelType.UNSIGNED_BYTE, new Sxta.Render.Buffer.Parameters(), new CPUBuffer<byte>(tileMap));
                return true;
            }

            int n = 0;
            for (int l = 0; l <= maxLevel; ++l)
            {
                float tileSize = rootQuadSize / (1 << l);
                int tx0 = (int)System.Math.Floor(camera.X / (2 * tileSize));
                int ty0 = (int)System.Math.Floor(camera.Y / (2 * tileSize));
                for (int ty = 2 * (ty0 - k); ty <= 2 * (ty0 + k) + 1; ++ty)
                {
                    for (int tx = 2 * (tx0 - k); tx <= 2 * (tx0 + k) + 1; ++tx)
                    {
                        GPUTileStorage.GPUSlot gpuData = null;
                        TileCache.Tile t;
                        if (tx >= 0 && tx < (1 << l) && ty >= 0 && ty < (1 << l))
                        {
                            t = findTile(l, tx, ty);
                        }
                        else
                        {
                            t = null;
                        }
                        if (t != null)
                        {
                            TileStorage.Slot data = t.getData(false);
                            if (data != null)
                            {
                                gpuData = (GPUTileStorage.GPUSlot)(data);
                            }
                        }
                        if (n < tileMapT.getWidth())
                        {
                            byte x, y;
                            if (gpuData == null)
                            {
                                x = 0;
                                y = 0;
                            }
                            else
                            {
                                x = (byte)(gpuData.l % 256);
                                y = (byte)(gpuData.l / 256 + 1);
                            }
                            tileMap[2 * n] = x;
                            tileMap[2 * n + 1] = y;
                            ++n;
                        }
                    }
                }
            }
            tileMapT.setSubImage(0, 0, id, n, 1, TextureFormat.RG, PixelType.UNSIGNED_BYTE, new Sxta.Render.Buffer.Parameters(), new CPUBuffer<byte>(tileMap));
            return true;
        }

        ///<summary>
        /// Returns the tile producers used by this TileProducer.
        ///
        /// @param[out] producers the tile producers used by this TileProducer.
        ///</summary>
        public virtual void getReferencedProducers(List<TileProducer> producers)
        {
        }

        ///<summary>
        /// Returns the number of layers of this producer.
        ///</summary>
        public int getLayerCount()
        {
            return layers.Count;
        }

        ///<summary>
        /// Returns the layer of this producer whose index is given.
        ///
        /// @param index a layer index between 0 and #getLayerCount (exclusive).
        ///</summary>
        public TileLayer getLayer(int index)
        {
            return layers[index];
        }

        ///<summary>
        /// Returns true if the list of layers is not empty. False otherwise.
        ///</summary>
        public bool hasLayers()
        {
            return layers.Count > 0;
        }

        ///<summary>
        /// Adds a Layer to this %producer.
        ///
        /// @param l the layer to be added.
        ///</summary>
        public void addLayer(TileLayer l)
        {
            layers.Add(l);
        }


        ///<summary>
        /// Creates an uninitialized TileProducer.
        ///
        /// @param type the type of this %producer.
        /// @param taskType the type of the Task that produce the actual tile data.
        ///</summary>
        protected TileProducer(string type, string taskType)
        {
            this.taskType = taskType;
        }

        ///<summary>
        /// Initializes this TileProducer.
        ///
        /// @param cache the tile cache that stores the tiles produced by this %producer.
        /// @param gpuProducer true if this %producer produces textures on GPU.
        ///</summary>
        protected void init(TileCache cache, bool gpuProducer)
        {
            Debug.Assert(cache != null);
            this.cache = cache;
            this.gpuProducer = gpuProducer;
            this.rootQuadSize = 0.0f;
            this.id = cache.nextProducerId++;
            cache.producers.Add(id, this);
            tileMap = null;
            mutex = new object();
            //TODO pthread_mutex_init((pthread_mutex_t*)mutex, null);
        }


        protected internal virtual void swap(TileProducer p)
        {
            cache.invalidateTiles(id);
            p.cache.invalidateTiles(p.id);
            Std.Swap(ref taskType, ref p.taskType);
            Std.Swap(ref cache, ref p.cache);
            Std.Swap(ref gpuProducer, ref p.gpuProducer);
            //std.swap(id, p.id);
            //std.swap(rootQuadSize, p.rootQuadSize);
            Std.Swap(ref tileMap, ref p.tileMap);
        }

        ///<summary>
        /// Returns the context for the Task that produce the tile data.
        /// This is only needed for GPU tasks (see Task#getContext).
        /// The default implementation of this method does nothing and returns null.
        ///</summary>
        protected internal virtual ulong getContext()
        {
            return 0;
        }

        ///<summary>
        /// Starts the creation of a tile of this %producer. This method is used for
        /// producers that need tiles produced by other producers to create a tile.
        /// In these cases this method must acquire these other tiles with #getTile
        /// so that these tiles are available with #findTile during the actual tile
        /// creation in #doCreateTile.
        ///
        /// @param level the tile's quadtree level.
        /// @param tx the tile's quadtree x coordinate.
        /// @param ty the tile's quadtree y coordinate.
        /// @param deadline the deadline at which the tile data must be ready. 0
        ///      means the current frame.
        /// @param task the task to produce the tile itself.
        /// @param owner the task %graph that contains 'task', or null. This task
        ///      %graph can be used if 'task' depends on other tasks. These other
        ///      tasks must then be added to 'owner'.
        /// @return the task or task %graph to produce the tile itself, and all the
        ///      tiles needed to produce it. The default implementation of this
        ///      method calls Layer#startCreateTile on each layer, and returns
        ///      'owner' if it is not null (otherwise it returns 'task'). NOTE: if
        ///      a task graph is returned, it must be created with #createTaskGraph.
        ///</summary>
        protected internal virtual Task startCreateTile(int level, int tx, int ty, uint deadline, Task task, TaskGraph owner)
        {
            for (int i = 0; i < (int)layers.Count; i++)
            {
                layers[i].startCreateTile(level, tx, ty, deadline, task, owner);
            }
            return owner == null ? task : (Task)owner;
        }

        ///<summary>
        /// Sets the execution context for the Task that produces the tile data.
        /// This is only needed for GPU tasks (see Task#begin).
        /// The default implementation of this method calls Layer#beginCreateTile on
        /// each Layer of this %producer.
        ///</summary>
        protected internal virtual void beginCreateTile()
        {
            for (int i = 0; i < (int)layers.Count; i++)
            {
                layers[i].beginCreateTile();
            }
        }

        ///<summary>
        /// Creates the given tile. If this task requires tiles produced by other
        /// producers (or other tiles produced by this %producer), these tiles must
        /// be acquired and released in #startCreateTile and #stopCreateTile with
        /// #getTile and #putTile, and retrieved in this method with #findTile.
        /// The default implementation of this method calls Layer#doCreateTile on
        /// each Layer of this %producer.
        ///
        /// @param level the tile's quadtree level.
        /// @param tx the tile's quadtree x coordinate.
        /// @param ty the tile's quadtree y coordinate.
        /// @param data where the created tile data must be stored.
        /// @return true the result of this creation is different from the result
        ///      of the last creation of this tile. See Task#run.
        ///</summary>
        protected internal virtual bool doCreateTile(int level, int tx, int ty, TileStorage.Slot data)
        {
            bool changes = false;
            for (int i = 0; i < (int)layers.Count; i++)
            {
                if (layers[i].isEnabled())
                {
                    changes |= layers[i].doCreateTile(level, tx, ty, data);
                }
            }
            return changes;
        }

        ///<summary>
        /// Restores the execution context for the Task that produces the tile data.
        /// This is only needed for GPU tasks (see Task#end).
        /// The default implementation of this method calls Layer#endCreateTile on
        /// each Layer of this %producer.
        ///</summary>
        protected internal virtual void endCreateTile()
        {
            for (int i = 0; i < (int)layers.Count; i++)
            {
                layers[i].endCreateTile();
            }
        }

        ///<summary>
        /// Stops the creation of a tile of this %producer. This method is used for
        /// producers that need tiles produced by other producers to create a tile.
        /// In these cases this method must release these other tiles with #putTile
        /// so that these tiles can be evicted from the cache after use. The default
        /// implementation of this method calls Layer#stopCreateTile on each Layer of
        /// this %producer.
        ///
        /// @param level the tile's quadtree level.
        /// @param tx the tile's quadtree x coordinate.
        /// @param ty the tile's quadtree y coordinate.
        ///</summary>
        protected internal virtual void stopCreateTile(int level, int tx, int ty)
        {
            for (int i = 0; i < (int)layers.Count; i++)
            {
                layers[i].stopCreateTile(level, tx, ty);
            }
        }

        ///<summary>
        /// Removes a task from the vector tasks. This is used to avoid delete-calls
        /// for objects that were already deleted.
        /// This is called when a task created by this producer gets deleted.
        ///</summary>
        internal void removeCreateTile(Task t)
        {
            lock (mutex)
            {
                Debug.Assert(tasks.Contains(t));
                tasks.Remove(t);
            }
        }

        ///<summary>
        /// Creates a task %graph for use in #startCreateTile.
        ///
        /// @param task a task that will be added to the returned task %graph.
        /// @return a task %graph containing 'task', for use in #startCreateTile.
        ///</summary>
        protected TaskGraph createTaskGraph(Task task)
        {
            CreateTile t = (CreateTile)task;
            Debug.Assert(t != null);
            CreateTileTaskGraph r = new CreateTileTaskGraph(this);
            r.addTask(t);
            r.root = t;
            t.parent = r;
            return r;
        }

        ///<summary>
        /// The list of all the Layers used by this %producer.
        ///</summary>
        private List<TileLayer> layers = new List<TileLayer>();

        ///<summary>
        /// The list of all the Tasks created by this %producer.
        ///</summary>
        private List<Task> tasks = new List<Task>();

        ///<summary>
        /// The type of the Task that produce the actual tile data.
        ///</summary>
        internal string taskType;

        ///<summary>
        /// The tile cache that stores the tiles produced by this %producer.
        ///</summary>
        internal TileCache cache;

        ///<summary>
        /// True if this %producer produces textures on GPU.
        ///</summary>
        internal bool gpuProducer;

        ///<summary>
        /// The id of this %producer. This id is local to the TileCache used by this
        /// %producer, and is used to distinguish all the producers that use this
        /// cache.
        ///</summary>
        private int id;

        ///<summary>
        /// The size in meters of the root tile produced by this %producer.
        ///</summary>
        private float rootQuadSize;
        /// <summary>
        /// The data of the tileMap texture line on GPU for this %producer. If a
        /// quadtree is subdivided based only on the distance to the camera, it is
        /// possible to compute on GPU which level,tx,ty tile corresponds to an x, y
        /// position in meters.But in order to compute where the data of this tile
        /// is stored in a texture tile storage, the mapping between level, tx, ty
        /// coordinates and texture tile storage u, v coordinates is needed.A tileMap
        /// provides this information (it must be updated each time the camera moves
        /// or the tile storage layout changes). Each line of this tileMap
        /// corresponds to a single %producer. Only GPU producers can have a tileMap.
        /// </summary>
        private byte[] tileMap;

        ///<summary>
        /// A mutex to serialize parallel accesses to #tasks.
        ///</summary>
        private object mutex;
        /// <summary>
        /// Creates a Task to produce the data of the given tile.
        /// </summary>
        /// <param name="level">the tile's quadtree level.</param>
        /// <param name="tx">the tile's quadtree x coordinate.</param>
        /// <param name="ty">the tile's quadtree y coordinate.</param>
        /// <param name="data">where the produced tile data must be stored.</param>
        /// <param name="deadline">the deadline at which the tile data must be ready. 0 means the current frame.</param>
        /// <param name="old">the existing task to create this tile if it still exists, or null.</param>
        /// <returns></returns>
        internal Task createTile(int level, int tx, int ty, TileStorage.Slot data, uint deadline, Task old)
        {
            Debug.Assert(data != null);
            if (old != null)
            {
                CreateTileTaskGraph r = (CreateTileTaskGraph)old;
                if (r != null)
                {
                    r.restore();
                }
                else
                {
                    Debug.Assert((CreateTile)old != null);
                }
                return old;
            }
            CreateTile t = new CreateTile(this, level, tx, ty, data, deadline);
            Task r2 = startCreateTile(level, tx, ty, deadline, t, null);
            lock (mutex)
            {
                tasks.Add(t);
                if (r2 != t)    //TOSEE Equals???
                {
                    Debug.Assert((CreateTileTaskGraph)r2 != null);
                    tasks.Add(r2);
                }
            }
            return r2;
        }
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }

    ///<summary>
    /// The Task that produces the tiles for a TileProducer.
    ///</summary>
    public class CreateTile : Task, IDisposable
    {

        ///<summary>
        /// The task graph that contains this task to store its dependencies. May be
        /// null for CreateTile tasks without dependencies.
        ///</summary>
        public TaskGraph parent;

        ///<summary>
        /// The TileProducer that created this task.
        ///</summary>
        public TileProducer owner;

        ///<summary>
        /// The level of the tile to create.
        ///</summary>
        public int level;

        ///<summary>
        /// The quadtree x coordinate of the tile to create.
        ///</summary>
        public int tx;

        ///<summary>
        /// The quadtree y coordinate of the tile to create.
        ///</summary>
        public int ty;

        ///<summary>
        /// Where the created tile data must be stored.
        ///</summary>
        public TileStorage.Slot data;

        ///<summary>
        /// Cache last result from getContext.
        ///</summary>
        public object cachedContext;

        ///<summary>
        /// True is the tiles needed to create this tile have been acquired with
        /// TileProducer#getTile. False if they are not or have been released with
        /// TileProducer#putTile.
        ///</summary>
        public bool initialized;

        ///<summary>
        /// Creates a new CreateTile Task.
        ///</summary>
        public CreateTile(TileProducer owner, int level, int tx, int ty, TileStorage.Slot data, uint deadline) :
            base(owner.taskType, owner.isGpuProducer(), deadline)
        {
            this.parent = null;
            this.owner = owner;
            this.level = level;
            this.tx = tx;
            this.ty = ty;
            this.data = data;
            this.initialized = true;
            // the task to produce 'data' is 'this'
            data.lock_(true);
            data.producerTask = this;
            data.lock_(false);
        }

        #region IDisposable implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool m_Disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    // releases the tiles used to create this tile, if necessary
                    stop();
                    if (owner != null)
                    {
                        owner.removeCreateTile(this);
                        if (owner.cache != null)
                        {
                            // cache is null if the owner producer is being deleted
                            // in this case it is not necessary to update the cache
                            owner.cache.createTileTaskDeleted(owner.getId(), level, tx, ty);
                        }
                        owner = null;
                    }
                    if (parent != null)
                    {
                        // this object can not be deleted if 'parent' is still referenced (because
                        // then it has itself references to this object). Hence we now that 'parent'
                        // is no longer referenced at this point, and can be deleted.
                        parent = null;
                    }                  
                    // Unmanaged resources are released here.

                    m_Disposed = true;
                }
            }
        }
        #endregion


        ~CreateTile()
        {
            Dispose(false);
        }

        ///<summary>
        /// Overrides Task#getContext.
        ///</summary>
        //---------------------TODO Review context definition---------------------------------------------------------------
        public override object getContext()
        {
            // combines the context returned by the producer with the producer type
            // to ensure that two tasks from two producers with the same context
            // but with different types return different contexts.

            if (owner != null)
            { // the owner exists
                cachedContext = owner.GetType().FullName + owner.getContext().ToString();
                return cachedContext;
            }
            else
            {
                // the owner may have been destroyed, this is a workaround
                Debug.Assert(cachedContext != null);
                return cachedContext; // return last successful result
            }
        }

        ///<summary>
        /// Overrides Task#init.
        ///</summary>
        public virtual void init(HashSet<Task> initialized)
        {
            if (!isDone() && owner != null)
            {
                // acquires the tiles used to create this tile, if necessary
                start();
            }
        }

        ///<summary>
        /// Acquires the tiles needed to create this tile with TileProducer#getTile,
        /// if this is not already done.
        ///</summary>
        public void start()
        {
            if (!initialized)
            {
                if (parent != null)
                {
                    // as we will reconstruct the content of #parent in
                    // startCreateTile, we clear it first; in fact we remove all
                    // dependencies first, and we remove unused tasks after the
                    // reconstruction (so as to avoid adding a task just after
                    // it is has been removed)
                    parent.clearDependencies();
                }

                // acquires the tiles needed to create this tile, and completes the
                // tasks graph with the corresponding tasks and task dependencies
                owner.startCreateTile(level, tx, ty, getDeadline(), this, parent);

                if (parent != null)
                {
                    // removes no longer used tasks; these are all the tasks without
                    // successors, except this task
                    foreach (Task t in parent.getLastTasks())
                    {
                        if (t != this)
                        {
                            parent.removeTask(t);
                        }
                    }
                }
                initialized = true;
            }
        }

        ///<summary>
        /// Overrides Task#begin.
        ///</summary>
        public override void begin()
        {
            Debug.Assert(!isDone());
            owner.beginCreateTile();
        }

        ///<summary>
        /// Overrides Task#run.
        ///</summary>
        public override bool run()
        {
            bool changes = true;
            Debug.Assert(!isDone());
            data.lock_(true);
            if (data.producerTask == this)
            {
                // since the creation of this CreateTile task,
                // where data.producerTask was set to this, it is possible that
                // 'data' was reaffected to another tile (if this task is a
                // prefetch task, its tile is not yet used and can then be evicted
                // from the cache between the creation and the execution of the
                // task). In this case we do not execute the task, otherwise it
                // could override data already produced for the reaffected tile.
                changes = owner.doCreateTile(level, tx, ty, data);

                //data.id = TileCache.Tile.getTId(owner.getId(), level, tx, ty);
                data.id = TileCache.Tile.getTId(owner.getId(), level, tx, ty);
            }
            data.lock_(false);
            return changes;
        }

        ///<summary>
        /// Overrides Task#end.
        ///</summary>
        public override void end()
        {
            owner.endCreateTile();
        }

        ///<summary>
        /// Releases the tiles used to create this tile with TileProducer#putTile,
        /// if this is not already done.
        ///</summary>
        public void stop()
        {
            if (initialized)
            {
                if (owner != null)
                {
                    owner.stopCreateTile(level, tx, ty);
                }
                initialized = false;
            }
        }

        public override void setIsDone(bool done, uint t, reason r)
        {
            base.setIsDone(done, t, r);
            if (done)
            {
                // releases the tiles used to create this tile, if necessary
                stop();
            }
            else if (r == reason.DATA_NEEDED)
            {
                // the task will need to be reexecuted soon (this is not the case
                // if the reason is DATA_CHANGED - when invalidating tiles, see
                // TileCache#invalidateTiles - for instance for unused tiles)

                // setIsDone(false, DATA_NEEDED) is called from TileCache#getTile
                // and TileCache#prefetchTile, after the tile has been put in the
                // used or unused tiles list. Hence we are sure to find the tile
                // for this task with a findTile
                TileCache.Tile t2 = owner.findTile(level, tx, ty, true);
                Debug.Assert(t2 != null);
                // we get the data storage from the tile. This data storage can be
                // different from the current one if this task is reused from the
                // TileCache#deletedTiles list (this happens if an unused tile T is
                // deleted, its storage reused for another tile, and if T is needed
                // again. If this task was not deleted in between, T is recreated
                // using a new storage but reusing this task which still refers to
                // the old storage).
                data = t2.data;
                // we need to update the producer task for this new storage
                data.lock_(true);
                data.producerTask = this;
                data.lock_(false);
                // the task is about to be executed, so we must acquire the tiles
                // needed for this task.
                start();
            }
        }

        new public virtual Type getTypeInfo()
        {
            return owner.GetType();
        }
    }

    /**
     * A TaskGraph for use with CreateTile. This subclass makes sure that
     * this task graph is not deleted before its internal root sub task
     * of type CreateTile.
     */
    public class CreateTileTaskGraph : TaskGraph, IDisposable
    {

        ///<summary>
        /// The TileProducer that created this task.
        ///</summary>
        public TileProducer owner;

        ///<summary>
        /// The CreateTile task that is the 'root' of this task graph.
        ///</summary>
        public CreateTile root;

        ///<summary>
        /// Saved dependencies of #root to restore them in #restore,
        /// when #root has been removed from this graph in #doRelease.
        ///</summary>
        public HashSet<Task> rootDependencies = new HashSet<Task>();

        public CreateTileTaskGraph(TileProducer owner)
            : base()
        {
            this.owner = owner;
        }

        #region IDisposable implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool m_Disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    if (owner != null)
                    {
                        owner.removeCreateTile(this);
                        owner.Dispose();
                        owner = null;
                    }

                    m_Disposed = true;
                }
            }
        }
        #endregion

        ~CreateTileTaskGraph()
        {
            Dispose(false);
        }

        public virtual void doRelease()
        {
            // we do not delete this task graph right now, because we need
            // it as long as its root primitive task may be used (it has
            // a weeak pointer to this graph via its 'parent' field; also
            // if the graph was destroyed but not its root primitive task,
            // rebuilding a new graph in TileProducer#createTile would not
            // be possible - it woudld require to call startCreateTile, which
            // would redo calls to TileCache#getTile, thus locking these
            // tiles forever in the cache).
            Task root = this.root;
            // removes all strong pointers to root so that it can be deleted
            // when it is no longer referenced (except by this graph)
            cleanup();
            // removes all dependencies on other tasks but save them
            // before in rootDependencies
            removeAndGetDependencies(root, rootDependencies);
            // finally removes 'root' itself
            removeTask(root);
        }

        public void restore()
        {
            // if this method is called then this object has not been deleted
            // which means that #root has not been deleted (because the
            // destructor of #root destructs this object). Hence we now that
            // the weak pointer #root still points to a valid object.
            addTask(root);
            foreach (Task dst in rootDependencies)
            {
                addDependency(root, dst);
            }
        }
    }
}
