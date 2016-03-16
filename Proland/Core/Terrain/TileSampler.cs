using log4net;
using Sxta.Core;
using Sxta.Math;
using Sxta.Render;
using Sxta.Render.Scenegraph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace proland
{
    class UpdateTileMapTask : Sxta.Render.Scenegraph.Task
    {
        public TileProducer producer;

        public float splitDistance;

        public Vector2f camera;

        public int depth;

        public UpdateTileMapTask(TileProducer producer, float splitDistance, Vector2f camera, int depth) : base("UpdateTileMapTask", true, 0)
        {
            this.producer = producer;
            this.splitDistance = splitDistance;
            this.camera = camera;
            this.depth = depth;
        }

        public virtual bool run()
        {
            producer.updateTileMap(splitDistance, camera, depth);
            return true;
        }
    };
    class TileSampler
    {

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// A filter to decide whether a texture tile must be produced or not
        /// for a given quad.
        /// </summary>
        public interface TileFilter //TODO INheritance from Object ("")
        {

            /// <summary>
            /// Returns true if a texture tile must be produced for the given quad.
            /// </summary>
            /// <param name="q">a %terrain quad.</param>
            /// <returns></returns>
            bool storeTile(TerrainQuad q);
        };

        /// <summary>
        /// Creates a new TileSampler.
        /// </summary>
        /// <param name="name">the GLSL name of this uniform.</param>
        /// <param name="producer"> the producer to be used to create new tiles in #update.
        /// Maybe null if this producer is used to update a tileMap(see
        /// #setTileMap). If not null, must have a
        /// proland.GPUTileStorage.</param>
        public TileSampler(string name, TileProducer producer = null)// : base("TileSampler")
        {
            init(name, producer);
        }


        /// <summary>
        /// Returns the producer to create new tiles in #update.
        /// </summary>
        /// <returns></returns>
        public TileProducer get()
        {
            return producer;
        }

        /**
         * Returns a %terrain associated with this uniform. Only used with #setTileMap.
         *
         * @param a %terrain index.
         */
        public TerrainNode getTerrain(int i)
        {
            return i < (int)(terrains.Count()) ? terrains[i] : null;
        }

        /// <summary>
        /// Returns true if texture tiles must be created for leaf quads.
        /// </summary>
        /// <returns></returns>
        public bool getStoreLeaf()
        {
            return storeLeaf;
        }

        /// <summary>
        /// Returns true if new tiles must be produced asychronously.
        /// </summary>
        /// <returns></returns>
        public bool getAsync()
        {
            return async;
        }

        /// <summary>
        /// Returns true if a (part of) parent tile can be used instead of the
        /// tile itself for rendering(this may happen if tiles are produced
        /// asynchronously, and if a tile is not ready but a parent tile is).
        /// </summary>
        /// <returns></returns>
        public bool getMipMap()
        {
            return mipmap;
        }

        /// <summary>
        /// Sets the producer to create new tiles in #update. This producer must have
        /// a proland.GPUTileStorage.
        /// </summary>
        /// <param name="producer">the producer to create new tiles in #update.</param>
        public void set(TileProducer producer)
        {
            this.producer = producer;
        }

        /// <summary>
        /// Adds a %terrain associated with this uniform. Only used with #setTileMap.
        /// </summary>
        /// <param name="terrain">a %terrain to be associated with this uniform.</param>
        public void addTerrain(TerrainNode terrain)
        {
            terrains.Add(terrain);
        }

        /// <summary>
        /// Sets the option to create new tiles for leaf quads or not.
        /// </summary>
        /// <param name="storeLeaf">true to create new tiles for leaf quads.</param>
        public void setStoreLeaf(bool storeLeaf)
        {
            this.storeLeaf = storeLeaf;
        }

        /// <summary>
        /// Sets the option to create new tiles for non leaf quads or not.
        /// </summary>
        /// <param name="storeParent">true to create new tiles for non leaf quads.</param>
        public void setStoreParent(bool storeParent)
        {
            this.storeParent = storeParent;
        }

        /// <summary>
        /// Sets the option to create new tiles for invisible quads or not.
        /// </summary>
        /// <param name="storeInvisible">true to create new tiles for invisible quads.</param>
        public void setStoreInvisible(bool storeInvisible)
        {
            this.storeInvisible = storeInvisible;
        }

        /// <summary>
        /// Sets the options to create new tiles for arbitrary quads or not.
        /// </summary>
        /// <param name="filter">a filter to decide whether a new texture tile must
        /// be created for a given quad.This filter is added to previously
        /// added filters with this method.A texture tile is created for
        /// a quad if at least one filter returns true for this quad.</param>
        public void setStoreFilter(TileFilter filter)
        {
            storeFilters.Add(filter);
        }

        /// <summary>
        /// Sets the option to create new tiles for new quads in synchronous or
        /// asychronous way.In synchronous mode, a frame is not displayed until
        /// all the tasks to create the tile data have been executed, which can
        /// lead to perceptible pauses. In asynchronous mode, the frame is
        /// displayed even if all tile data are not yet ready.In this case the
        /// first tile ancestor whose data is ready is used instead (the
        /// asynchronous mode is only possible is #setStoreParent is true). This
        /// mode can lead to visible popping when more precise data suddenly
        /// replaces coarse data. NOTE: the asynchronous mode requires a scheduler
        /// that supports prefetching of any kind of task (both cpu and gpu).
        /// NOTE: you can mix TileSampler in synchronous mode with others
        /// using asynchronous mode.Hence some tile data can be produced
        /// synchronously while other data is produced asynchronously.
        /// </summary>
        /// <param name="async">true to create tiles asynchrously.</param>
        public void setAsynchronous(bool async)
        {
            this.async = async;
            Debug.Assert(!async || storeParent);
        }

        /// <summary>
        /// Sets the option to allow using a (part of) parent tile instead of the
        /// tile itself for rendering.This option is only useful when the
        /// asynchronous mode is set.
        /// </summary>
        /// <param name="mipmap"></param>
        public void setMipMap(bool mipmap)
        {
            this.mipmap = mipmap;
        }

        /// <summary>
        /// Sets the GLSL uniforms necessary to access the texture tile for
        /// the given quad.This methods does nothing if terrains are associated
        /// with this uniform(uniform intended to be used with #setTileMap).
        /// </summary>
        /// <param name="level">a quad level.</param>
        /// <param name="tx">a quad logical x coordinate.</param>
        /// <param name="ty">a quad logical y coordinate.</param>
        public void setTile(int level, int tx, int ty)
        {
            checkUniforms();
            if (samplerU == null)
            {
                return;
            }
            TileCache.Tile t = null;
            int b = producer.getBorder();
            int s = producer.getCache().getStorage().getTileSize();

            float dx = 0;
            float dy = 0;
            float dd = 1;
            float ds0 = (s / 2) * 2.0f - 2.0f * b;
            float ds = ds0;
            while (!producer.hasTile(level, tx, ty))
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

            Tree tt = root;
            Tree tc;
            int tl = 0;
            while (tl != level && (tc = tt.children[((tx >> (level - tl - 1)) & 1) | ((ty >> (level - tl - 1)) & 1) << 1]) != null)
            {
                tl += 1;
                tt = tc;
            }
            while (level > tl)
            {
                dx += (tx % 2) * dd;
                dy += (ty % 2) * dd;
                dd *= 2;
                ds /= 2;
                level -= 1;
                tx /= 2;
                ty /= 2;
            }
            t = tt.t;

            while (t == null)
            {
                dx += (tx % 2) * dd;
                dy += (ty % 2) * dd;
                dd *= 2;
                ds /= 2;
                level -= 1;
                tx /= 2;
                ty /= 2;
                tt = tt.parent;
                Debug.Assert(tt != null);
                t = tt.t;
            }

            dx = dx * ((s / 2) * 2 - 2 * b) / dd;
            dy = dy * ((s / 2) * 2 - 2 * b) / dd;

            GPUTileStorage mip = (GPUTileStorage)(producer.getCache().getStorage());
            mip.generateMipMap();
            GPUTileStorage.GPUSlot gput = (GPUTileStorage.GPUSlot)(t.getData());
            Debug.Assert(gput != null);

            float w = gput.getWidth();
            float h = gput.getHeight();
            Debug.Assert(w == h);

            Vector4f coords;
            if (s % 2 == 0)
            {
                coords = new Vector4f((dx + b) / w, (dy + b) / h, (float)(GPUTileStorage.GPUSlot.l), ds / w);
            }
            else
            {
                coords = new Vector4f((dx + b + 0.5f) / w, (dy + b + 0.5f) / h, (float)(GPUTileStorage.GPUSlot.l), ds / w);
            }

            samplerU.set(gput.t);
            coordsU.set(new Vector3f(coords.X, coords.Y, coords.Z));
            sizeU.set(new Vector3f(coords.W, coords.W, (s / 2) * 2.0f - 2.0f * b));
        }

        /**
         * Sets the GLSL uniforms necessary to access the texture tiles for
         * arbitrary quads on GPU. This method does nothing if terrains have
         * not been associated with this uniform. These terrains must be all
         * the terrains for which the tile map may be used on GPU.
         */
        public void setTileMap()
        {
            if (terrains.Count() == 0)
            {
                return;
            }
            checkUniforms();
            if (samplerU == null)
            {
                return;
            }
            GPUTileStorage storage = (GPUTileStorage)producer.getCache().getStorage();
            if (storage.getTileMap() != null)
            {
                storage.generateMipMap();
                Texture tilePool = storage.getTexture(0);
                TerrainNode n = terrains[0];
                int maxLevel = n.maxLevel;
                while (!producer.hasTile(maxLevel, 0, 0))
                {
                    --maxLevel;
                }

                float k = (float)Math.Ceiling(n.getSplitDistance());
                samplerU.set(storage.getTexture(0));
                tileMapU.set(storage.getTileMap());
                quadInfoU.set(new Vector4f((float)n.root.l, n.getSplitDistance(), 2.0f * k, 4.0f * k + 2.0f));
                float w;
                float h;
                if ((Texture2D)tilePool != null)
                {
                    w = ((Texture2D)tilePool).getWidth();
                    h = ((Texture2D)tilePool).getHeight();
                }
                else
                {
                    w = ((Texture2DArray)tilePool).getWidth();
                    h = ((Texture2DArray)tilePool).getHeight();
                }
                poolInfoU.set(new Vector4f((float)(storage.getTileSize()), (float)(producer.getBorder()), 1.0f / w, 1.0f / h));
                for (int i = 0; i < terrains.Count(); ++i)
                {
                    Vector3d camera = terrains[i].getLocalCamera();
                    cameraU[i].set((new Vector4f((float)(camera.X - n.root.ox), (float)(camera.Y - n.root.oy), (float)(camera.Z - TerrainNode.groundHeightAtCamera) / n.getDistFactor(), maxLevel)));
                }
            }
        }

        /**
         * Returns the task %graph necessary to create new texture tiles for
         * newly created quads in the given %terrain quadtree (and to release
         * tiles for destroyed quads). This method returns an empty task %graph
         * if terrains have been associated with this uniform (uniform intended
         * to be used with #setTileMap).
         *
         * @param scene the scene manager.
         * @param root the root of a %terrain quadtree.
         */
        public virtual Sxta.Render.Scenegraph.Task update(SceneManager scene, TerrainQuad root)
        {
            TaskGraph result = new TaskGraph();
            if (terrains.Count() == 0)
            {
                producer.update(scene);
                if (storeInvisible)
                {
                    root.getOwner().splitInvisibleQuads = true;
                }
                if (!async && storeLeaf && this.root != null)
                {
                    int prefetchCount = producer.getCache().getUnusedTiles() + producer.getCache().getStorage().getFreeSlots();
                    prefetch(this.root, root, prefetchCount);
                }
                putTiles(this.root, root);
                getTiles(null, this.root, root, result);

                GPUTileStorage storage = (GPUTileStorage)producer.getCache().getStorage();
                if (storage.getTileMap() != null)
                {
                    TerrainNode n = root.getOwner();
                    Vector3d camera = n.getLocalCamera();
                    Sxta.Render.Scenegraph.Task t = new UpdateTileMapTask(producer, n.getSplitDistance(), new Vector2f((float)camera.X, (float)camera.Y), root.getDepth());
                    if (result.isEmpty())
                    {
                        t.run();
                    }
                    else
                    {
                        TaskGraph graph = new TaskGraph();
                        graph.addTask(result);
                        graph.addTask(t);
                        graph.addDependency(t, result);
                        result = graph;
                    }
                }
            }
            return result;
        }


        /**
         * An internal quadtree to store the texture tile associated with each
         * %terrain quad.
         */
        internal class Tree
        {

            public bool newTree;

            public bool needTile;

            /**
             * The parent quad of this quad.
             */
            public Tree parent;

            /**
             * The texture tile associated with this quad.
             */
            public TileCache.Tile t;

            /**
             * The subquads of this quad.
             */
            public Tree[] children = new Tree[4];

            /**
             * Creates a new Tree.
             */
            public Tree(Tree parent)
            {
                children[0] = null;
                children[1] = null;
                children[2] = null;
                children[3] = null;
                newTree = true;
                needTile = false;
                this.parent = parent;
                t = null;
            }

            /**
             * Deletes this Tree and all its subelements. Releases
             * all the corresponding texture tiles #t.
             */
            public virtual void recursiveDelete(TileSampler owner)
            {
                if (t != null)
                {
                    owner.producer.putTile(t);
                    t = null;
                }
                if (children[0] != null)
                {
                    children[0].recursiveDelete(owner);
                    children[1].recursiveDelete(owner);
                    children[2].recursiveDelete(owner);
                    children[3].recursiveDelete(owner);
                }
                //TOSEE delete this;
            }
        };

        /**
         * An internal quadtree to store the texture tiles associated with each quad.
         */
        protected Tree root;

        /**
         * Creates an uninitialized TileSampler.
         */
        protected TileSampler()// : base("TileSampler")
        {

        }

        /**
         * Initializes this TileSampler.
         *
         * @param name the GLSL name of this uniform.
         * @param producer the %producer to be used to create new tiles in #update.
         *      Maybe null if this %producer is used to update a tileMap (see
         *      #setTileMap). If not null, must have a
         *      proland.GPUTileStorage.
         */
        protected virtual void init(string name, TileProducer producer = null)
        {
            this.name = name;
            this.producer = producer;
            this.root = null;
            this.storeLeaf = true;
            this.storeParent = true;
            this.storeInvisible = true;
            this.async = false;
            this.mipmap = false;
            GPUTileStorage storage = (GPUTileStorage)producer.getCache().getStorage();
            Debug.Assert(storage != null);
            lastProgram = null;
        }

        /**
         * Returns true if a tile is needed for the given terrain quad.
         *
         * @param q a quadtree node.
         */
        protected virtual bool needTile(TerrainQuad q)
        {
            bool needTile = storeLeaf;
            if (!storeParent && (q.children[0] != null) && (producer.hasChildren(q.level, q.tx, q.ty)))
            {
                needTile = false;
            }
            if (!needTile)
            {
                for (int i = 0; i < storeFilters.Count(); ++i)
                {
                    if (storeFilters[i].storeTile(q))
                    {
                        needTile = true;
                        break;
                    }
                }
            }
            if (!storeInvisible && q.visible == SceneManager.visibility.INVISIBLE)
            {
                needTile = false;
            }
            return needTile;
        }

        /**
         * Updates the internal quadtree to make it identical to the given %terrain
         * quadtree. This method releases the texture tiles corresponding to
         * deleted quads.
         *
         * @param t the internal quadtree node corresponding to q.
         * @param q a quadtree node.
         */
        protected virtual void putTiles(Tree t, TerrainQuad q)
        {
            if (t == null)
            {
                return;
            }

            Debug.Assert(producer.hasTile(q.level, q.tx, q.ty));

            t.needTile = needTile(q);

            if (!t.needTile)
            {
                if (t.t != null)
                {
                    producer.putTile(t.t);
                    t.t = null;
                }
            }

            if (q.children[0] == null)
            {
                if (t.children[0] != null)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        t.children[i].recursiveDelete(this);
                        t.children[i] = null;
                    }
                }
            }
            else if (producer.hasChildren(q.level, q.tx, q.ty))
            {
                for (int i = 0; i < 4; ++i)
                {
                    putTiles(t.children[i], q.children[i]);
                }
            }
        }

        /**
         * Updates the internal quadtree to make it identical to the given %terrain
         * quadtree. Collects the tasks necessary to create the missing texture
         * tiles, corresponding to newly created quads, in the given task %graph.
         *
         * @param t the internal quadtree node corresponding to q.
         * @param q a quadtree node.
         * @param result the task %graph to collect the tile %producer tasks.
         */
        protected virtual void getTiles(Tree parent, Tree t, TerrainQuad q, TaskGraph result)
        {
            if (t == null)
            {
                t = new Tree(parent);
                t.needTile = needTile(q);
                if (q.level == 0 && producer.getRootQuadSize() == 0.0f)
                {
                    producer.setRootQuadSize((float)q.l);
                }
            }

            Debug.Assert(producer.hasTile(q.level, q.tx, q.ty));

            if (t.needTile)
            {
                if (t.t == null)
                {
                    if (async && q.level > 0)
                    {
                        t.t = producer.findTile(q.level, q.tx, q.ty, true);
                        if (t.t == null)
                        {
                            if (q.isLeaf())
                            {
                                producer.prefetchTile(q.level, q.tx, q.ty);
                            }
                        }
                        else
                        {
                            t.t = producer.getTile(q.level, q.tx, q.ty, 0);
                            Debug.Assert(t.t != null);
                        }
                    }
                    else
                    {
                        t.t = producer.getTile(q.level, q.tx, q.ty, 0);
                        if (t.t == null && log.IsErrorEnabled)
                        {
                            log.Error("Insufficient tile cache size for '" + name + "' uniform");
                        }
                        Debug.Assert(t.t != null);
                    }
                }
                if (t.t != null)
                {
                    Sxta.Render.Scenegraph.Task tt = t.t.task;
                    if (!t.t.task.isDone())
                    {
                        result.addTask(t.t.task);
                    }
                }
            }

            if (q.children[0] != null && producer.hasChildren(q.level, q.tx, q.ty))
            {
                for (int i = 0; i < 4; ++i)
                {
                    getTiles(t, t.children[i], q.children[i], result);
                }
            }
        }

        /**
         * Creates prefetch tasks for the sub quads of quads marked as new in
         * Tree#newTree, in the limit of the prefetch count.
         *
         * @param t the internal quadtree node corresponding to q.
         * @param q a quadtree node.
         * @param[in,out] prefetchCount the maximum number of prefetch tasks
         *      that can be created by this method.
         */
        protected void prefetch(Tree t, TerrainQuad q, int prefetchCount)
        {
            if (t.children[0] == null)
            {
                if (t.newTree && q != null)
                {
                    if ((storeInvisible || q.visible != SceneManager.visibility.INVISIBLE) && producer.hasChildren(q.level, q.tx, q.ty))
                    {
                        if (prefetchCount > 0)
                        {
                            if (producer.prefetchTile(q.level + 1, 2 * q.tx, 2 * q.ty))
                            {
                                --prefetchCount;
                            }
                        }
                        if (prefetchCount > 0)
                        {
                            if (producer.prefetchTile(q.level + 1, 2 * q.tx + 1, 2 * q.ty))
                            {
                                --prefetchCount;
                            }
                        }
                        if (prefetchCount > 0)
                        {
                            if (producer.prefetchTile(q.level + 1, 2 * q.tx, 2 * q.ty + 1))
                            {
                                --prefetchCount;
                            }
                        }
                        if (prefetchCount > 0)
                        {
                            if (producer.prefetchTile(q.level + 1, 2 * q.tx + 1, 2 * q.ty + 1))
                            {
                                --prefetchCount;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; ++i)
                {
                    prefetch(t.children[i], q == null ? null : q.children[i], prefetchCount);
                }
            }
            t.newTree = false;
        }

        /**
         * Checks if the last checked Program is the same as the current one,
         * and updates the Uniforms if necessary.
         */
        protected void checkUniforms()
        {

        }

        protected virtual void swap(TileSampler p)
        {
            Std.Swap(ref name, ref p.name);
            Std.Swap(ref producer, ref p.producer);
            Std.Swap(ref terrains, ref p.terrains);
            //    Std.Swap(block, p.block);
            Std.Swap(ref samplerU, ref p.samplerU);
            Std.Swap(ref coordsU, ref p.coordsU);
            Std.Swap(ref sizeU, ref p.sizeU);
            Std.Swap(ref tileMapU, ref p.tileMapU);
            Std.Swap(ref quadInfoU, ref p.quadInfoU);
            Std.Swap(ref poolInfoU, ref p.poolInfoU);
            Std.Swap(ref cameraU, ref p.cameraU);
            Std.Swap(ref storeLeaf, ref p.storeLeaf);
            Std.Swap(ref storeParent, ref p.storeParent);
            Std.Swap(ref storeInvisible, ref p.storeInvisible);
            Std.Swap(ref storeFilters, ref p.storeFilters);
            Std.Swap(ref async, ref p.async);
            Std.Swap(ref mipmap, ref p.mipmap);
        }


        private string name;

        /**
         * The %producer to be used to create texture tiles for newly created quads.
         */
        private TileProducer producer;

        /**
         * The terrains associated with this uniform. Only used with #setTileMap.
         */
        private List<TerrainNode> terrains;

        /**
         * Last used GLSL program. Updated each time #setTile or #setTileMap is called with a
         * different Program. Helps to retrieve the correct Uniforms and store them.
         */
        private Program lastProgram;

        /**
         * The texture sampler to access the proland.GPUTileStorage.
         */
        private UniformSampler samplerU;

        /**
         * The coordinates of a tile in the proland.GPUTileStorage.
         */
        private Uniform3f coordsU;

        /**
         * The relative size of a tile in the proland.GPUTileStorage.
         */
        private Uniform3f sizeU;

        /**
         * The texture sampler to access the proland.GPUTileStorage
         * tile map (an indirection structure to get the storage coordinate of
         * a tile from its logical coordinates).
         */
        private UniformSampler tileMapU;

        /**
         * rootTileSize, splitDistance, k=ceil(splitDistance), 4*k+2.
         */
        private Uniform4f quadInfoU;

        /**
         * Tile size in pixels including borders, border in pixels, tilePool 1/w, 1/h.
         */
        private Uniform4f poolInfoU;

        /**
         * The current camera position in local space for each %terrain associated
         * with this uniform (only used with #setTileMap).
         */
        private List<Uniform4f> cameraU;

        /**
         * True to store texture tiles for leaf quads.
         */
        private bool storeLeaf;

        /**
         * True to store texture tiles for non leaf quads.
         */
        private bool storeParent;

        /**
         * True to store texture tiles for invisible quads.
         */
        private bool storeInvisible;

        /**
         * A set of filters to decide whether a texture tile must be stored
         * for a given quad. A texture is stored if at least one filter returns
         * true.
         */
        private List<TileFilter> storeFilters;

        /**
         * True if tiles must be loaded in an asynchronous way, using prefetching.
         */
        private bool async;

        /**
         * True if a parent tile can be used instead of the tile itself for rendering.
         */
        private bool mipmap;
    }
#if TODO
    class TileSamplerResource : public ResourceTemplate<10, TileSampler>
{
public:
    TileSamplerResource(ptr<ResourceManager> manager, const string &name, ptr<ResourceDescriptor> desc, const TiXmlElement *e = null) :
        ResourceTemplate<10, TileSampler>(manager, name, desc)
    {
        e = e == null ? desc.descriptor : e;
        checkParameters(desc, e, "id,name,sampler,producer,terrains,storeLeaf,storeParent,storeInvisible,async,mipmap,");
        string uname;
        ptr<TileProducer> producer;
        uname = getParameter(desc, e, "sampler");
        producer = manager.loadResource(getParameter(desc, e, "producer")).cast<TileProducer>();
        init(uname, producer);
        if (e.Attribute("terrains") != null) {
            string nodes = string(e.Attribute("terrains"));
            string.size_type start = 0;
            string.size_type index;
            while ((index = nodes.find(',', start)) != string.npos) {
                string node = nodes.substr(start, index - start);
                addTerrain(manager.loadResource(node).cast<TerrainNode>());
                start = index + 1;
            }
        }
        if (e.Attribute("storeLeaf") != null && strcmp(e.Attribute("storeLeaf"), "false") == 0) {
            setStoreLeaf(false);
        }
        if (e.Attribute("storeParent") != null && strcmp(e.Attribute("storeParent"), "false") == 0) {
            setStoreParent(false);
        }
        if (e.Attribute("storeInvisible") != null && strcmp(e.Attribute("storeInvisible"), "false") == 0) {
            setStoreInvisible(false);
        }
        if (e.Attribute("async") != null && strcmp(e.Attribute("async"), "true") == 0) {
            setAsynchronous(true);
        }
        if (e.Attribute("mipmap") != null && strcmp(e.Attribute("mipmap"), "true") == 0) {
            setMipMap(true);
        }
    }
};

extern const char tileSampler[] = "tileSampler";

static ResourceFactory.Type<tileSampler, TileSamplerResource> TileSamplerType;
#endif
}
