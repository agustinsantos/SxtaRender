﻿using log4net;
using Sxta.Render.Scenegraph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sxta.Render.Producer
{
    /// <summary>
    /// A cache of tiles to avoid recomputing recently produced tiles. A tile cache
    /// keeps track of which tiles (identified by their level,tx,ty coordinates) are
    /// currently stored in an associated TileStorage. It also keeps track of which
    /// tiles are in use, and which are not. Unused tiles are kept in the TileStorage
    /// as long as possible, in order to avoid re creating them if they become needed
    /// again. But the storage associated with unused tiles can be reused to store
    /// other tiles at any moment (in this case we say that a tile is evicted from
    /// the cache of unused tiles).
    /// Conversely, the storage associated with tiles currently in use cannot be
    /// reaffected until these tiles become unused. A tile is in use when it is
    /// returned by #getTile, and becomes unused when #putTile is called (more
    /// precisely when the number of users of this tile becomes 0, this number being
    /// incremented and decremented by #getTile and #putTile, respectively). The
    /// tiles that are needed to render the current frame should be declared in use,
    /// so that they are not evicted between their creation and their actual
    //rendering.
    /// @authors Eric Bruneton, Antoine Begault, Guillaume Piolat
    /// </summary>
    public class TileCache
    {
        /*
 * A tile described by its level,tx,ty coordinates. A TileCache.Tile
 * describes where the tile is stored in the TileStorage, how its data can
 * be produced, and how many users currently use it.
 */
        public class Tile
        {
            /*
             * A tile identifier for a given producer. Contains the tile coordinates
             * level, tx, ty.
             */
            //typedef std.pair<int, std.pair<int, int> > Id;
            public class Id : Tuple<int, Tuple<int, int>>
            {
                public Id(int level, int tx, int ty)
                    : base(level, new Tuple<int, int>(tx, ty)) { }
            }

            /*
             * A tile identifier. Contains a %producer id (first pair element) and
             * tile coordinates level,tx,ty (second pair element).
             */
            // typedef std.pair<int, Id> TId;
            public class TId : Tuple<int, Id>
            {
                public TId(int producerId, Id id) : base(producerId, id) { }
            }

            /*
             * The id of the %producer that manages this tile.  This local id is
             * assigned to each new %producer that uses this TileCache.
             */
            public readonly int producerId;

            /*
             * The quadtree level of this tile.
             */
            public readonly int level;

            /*
             * The quadtree x coordinate of this tile at level #level.
             * Varies between 0 and 2^level - 1.
             */
            public readonly int tx;

            /*
             * The quadtree y coordinate of this tile at level #level.
             * Varies between 0 and 2^level - 1.
             */
            public readonly int ty;

            /*
             * The task that produces or produced the actual tile data.
             */
            public readonly Task task;

            /*
             * Creates a new tile.
             *
             * @param producerId the id of the %producer of this tile.
             * @param level the quadtree level of this tile.
             * @param tx the quadtree x coordinate of this tile.
             * @param ty the quadtree y coordinate of this tile.
             * @param task the task that will produce the tile data.
             * @param data where the produced tile data must be stored.
             */
            public Tile(int producerId, int level, int tx, int ty, Task task, TileStorage.Slot data)
            {
                this.producerId = producerId;
                this.level = level;
                this.tx = tx;
                this.ty = ty;
                this.task = task;
                this.data = data;
                users = 0;

                Debug.Assert(data != null);
            }

            /*
             * Deletes this tile. This does not delete the tile data itself, only
             * the mapping between the tile and its location in the TileStorage.
             */
            // ~Tile();

            /*
             * Returns the actual data of this tile.
             *
             * @param check true to check that the task that produced this data is
             *      actually done.
             * @return the actual data of this tile, or null if the task that
             *      produces this data is not done.
             */
            public TileStorage.Slot getData(bool check = true)
            {
                bool isDone = task.isDone();
                Debug.Assert(isDone || !check);
                Debug.Assert(getTId() == data.id || !check);
                return isDone ? data : null;
            }


            /*
             * Returns the identifier of this tile.
             */
            public Id getId()
            {
                return getId(level, tx, ty);
            }

            /*
             * Returns the identifier of this tile.
             */
            public TId getTId()
            {
                return getTId(producerId, level, tx, ty);
            }

            /*
             * Returns the identifier of a tile.
             *
             * @param level the tile's quadtree level.
             * @param tx the tile's quadtree x coordinate.
             * @param ty the tile's quadtree y coordinate.
             */
            public static Id getId(int level, int tx, int ty)
            {
                return new Id(level, tx, ty);
            }

            /*
             * Returns the identifier of a tile.
             *
             * @param producerId the id of the tile's %producer.
             * @param level the tile's quadtree level.
             * @param tx the tile's quadtree x coordinate.
             * @param ty the tile's quadtree y coordinate.
             */
            public static TId getTId(int producerId, int level, int tx, int ty)
            {
                return new TId(producerId, new Id(level, tx, ty));
            }


            /*
             * The actual data of this tile. This data is not ready before #task is
             * done.
             */
            internal TileStorage.Slot data;

            /*
             * The number of users of this tile. This number is incremented by
             * #getTile and decremented by #putTile. When it becomes 0 the tile
             * becomes unused.
             */
            internal int users;
        }

        /*
         * Creates a new TileCache.
         *
         * @param storage the tile storage to store the actual tiles data.
         * @param name name of this cache, for logging.
         * @param scheduler an optional scheduler to schedule the creation of
         *      prefetched tiles. If no scheduler is specified, prefetch is
         *      disabled.
         */
        public TileCache(TileStorage storage, string name, Scheduler scheduler = null)
        {
            init(storage, name, scheduler);
        }


        /*
         * Deletes this TileCache.
         */
        ~TileCache()
        {
            //TODO pthread_mutex_destroy((pthread_mutex_t*)mutex);
            //TODO delete (pthread_mutex_t*) mutex;
            mutex = null;
            // The users of a TileCache must release all their tiles with putTile
            // before they erase their reference to the TileCache. Hence a TileCache
            // cannot be deleted before all tiles are unused. So usedTiles should be
            // empty at this point
            Debug.Assert(usedTiles.Count == 0);
            unusedTiles.Clear();
            // releases the storage used by the unused tiles
            foreach (Tile i in unusedTilesOrder)
            {
                storage.deleteSlot(i.data);
                // TODO delete *i;
            }
            unusedTilesOrder.Clear();
            deletedTiles.Clear();
        }

        /*
         * Returns the storage used to store the actual tiles data.
         */
        public TileStorage getStorage()
        {
            return storage;
        }

        /*
         * Returns the scheduler used to schedule prefetched tiles creation tasks.
         */
        public Scheduler getScheduler()
        {
            return scheduler;
        }

        /*
         * Returns the number of tiles currently in use in this cache.
         */
        public int getUsedTiles()
        {
            return usedTiles.Count;
        }

        /*
         * Returns the number of tiles currently unused in this cache.
         */
        public int getUnusedTiles()
        {
            return unusedTiles.Count;
        }

        /*
         * Looks for a tile in this TileCache.
         *
         * @param producerId the id of the tile's %producer.
         * @param level the tile's quadtree level.
         * @param tx the tile's quadtree x coordinate.
         * @param ty the tile's quadtree y coordinate.
         * @param includeCache true to include both used and unused tiles in the
         *      search, false to include only the used tiles.
         * @return the requested tile, or null if it is not in this TileCache. This
         *      method does not change the number of users of the returned tile.
         */
        public Tile findTile(int producerId, int level, int tx, int ty, bool includeCache = false)
        {
            Debug.Assert(producers.ContainsKey(producerId));
            lock (mutex)
            {
                Tile.TId id = Tile.getTId(producerId, level, tx, ty);
                Tile t = null;
                // looks for the requested tile in the used tiles list
                if (usedTiles.TryGetValue(id, out t))
                {
                    Debug.Assert(t.producerId == producerId && t.level == level && t.tx == tx && t.ty == ty);
                }
                // looks for the requested tile in the unused tiles list (if includeCache is true)
                if (t == null && includeCache)
                {
                    List<Tile> list;
                    if (unusedTiles.TryGetValue(id, out list))
                    {
                        t = list[0];
                        Debug.Assert(t.producerId == producerId && t.level == level && t.tx == tx && t.ty == ty);
                    }
                }

                return t;
            }
        }

        /*
         * Returns the requested tile, creating it if necessary. If the tile is
         * currently in use it is returned directly. If it is in cache but unused,
         * it marked as used and returned. Otherwise a new tile is created, marked
         * as used and returned. In all cases the number of users of this tile is
         * incremented by one.
         *
         * @param producerId the id of the tile's %producer.
         * @param level the tile's quadtree level.
         * @param tx the tile's quadtree x coordinate.
         * @param ty the tile's quadtree y coordinate.
         * @param deadline the deadline at which the tile data must be ready. 0 means
         *      the current frame.
         * @param[out] the number of users of this tile, <i>before</i> it is
         *      incremented.
         * @return the requested tile, or null if there is no room left in the
         *      TileStorage to store the requested tile.
         */
        public Tile getTile(int producerId, int level, int tx, int ty, uint deadline, ref int users)
        {
            Debug.Assert(producers.ContainsKey(producerId));
            lock (mutex)
            {
                Tile.TId id = Tile.getTId(producerId, level, tx, ty);
                Tile t;
                if (!usedTiles.TryGetValue(id, out t))
                {
                    bool deletedTile = false;
                    ++queries;
                    List<Tile> list;
                    if (unusedTiles.TryGetValue(id, out list))
                    {
                        // the requested tile is not in storage, it must be created
                        TileStorage.Slot data = storage.newSlot();
                        if (data == null && unusedTiles.Count > 0)
                        {
                            // evict least recently used tile to reuse its data storage
                            t = unusedTilesOrder[0];
                            data = t.data;
                            unusedTiles.Remove(t.getTId());
                            unusedTilesOrder.RemoveAt(0);
                            deletedTiles.Add(t.getTId(), t.task);
                            //delete t;
                        }
                        if (data == null)
                        { // cache is full
                            t = null;
                        }
                        else
                        {
                            ++misses;
                            Task task;
                            if (deletedTiles.TryGetValue(id, out task))
                            {
                                deletedTile = true;
                                deletedTiles.Remove(id);
                            }
                            task = producers[producerId].createTile(level, tx, ty, data, deadline, task);
                            // creates the requested tile
                            t = new Tile(producerId, level, tx, ty, task, data);
                        }
                    }
                    else
                    {
                        // requested tile found in unused tile list
                        t = list[0];
                        unusedTiles.Remove(id);
                        unusedTilesOrder.Remove(t);
                    }
                    if (t != null)
                    {
                        // marks requested tile as used
                        usedTiles.Add(id, t);
                        if (deletedTile)
                        {
                            // if the tile data was not in storage and if the task to create it
                            // was reused from a deleted tile, we need to reexecute the task
                            // to recreate the tile data
                            if (scheduler == null)
                            {
                                t.task.setIsDone(false, 0, Task.reason.DATA_NEEDED);
                            }
                            else
                            {
                                scheduler.reschedule(t.task, Task.reason.DATA_NEEDED, deadline);
                            }
                        }
                    }
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("{0}: tiles: {1} used, {2} reusable, total {3}", name, usedTiles.Count, unusedTiles.Count, storage.getCapacity());
                    }
                }
                else
                {
                    // requested tile found in used tiles list . nothing to do
                    Debug.Assert(t.producerId == producerId && t.level == level && t.tx == tx && t.ty == ty);
                }
                if (t != null)
                {
                    if (users != null)
                    {
                        users = t.users;
                    }
                    t.users += 1;
                }
                return t;
            }
        }

        /*
         * Returns a prefetch task to create the given tile. If the requested tile
         * is currently in use or in cache but unused, this method does nothing.
         * Otherwise it gets an unused tile storage (evicting an unused tile if
         * necessary), and then creates a task to produce the data of the requested
         * tile, in this storage. This method must not be called if #getScheduler
         * returns null.
         *
         * @param producerId the id of the tile's %producer.
         * @param level the tile's quadtree level.
         * @param tx the tile's quadtree x coordinate.
         * @param ty the tile's quadtree y coordinate.
         */
        public Task prefetchTile(int producerId, int level, int tx, int ty)
        {
            Debug.Assert(producers.ContainsKey(producerId));
            lock (mutex)
            {
                Tile.TId id = Tile.getTId(producerId, level, tx, ty);
                Task task = null;
                if (!usedTiles.ContainsKey(id))
                {
                    if (!unusedTiles.ContainsKey(id))
                    {
                        // the requested tile is not in storage, it must be created
                        TileStorage.Slot data = storage.newSlot();
                        if (data == null && unusedTiles.Count > 0)
                        {
                            // evict least recently used tile to reuse its data storage
                            Tile t = unusedTilesOrder[0];
                            data = t.data;
                            Debug.Assert(data != null);
                            unusedTiles.Remove(t.getTId());
                            unusedTilesOrder.Remove(t);
                            deletedTiles.Add(t.getTId(), t.task);
                            //TODO delete t;
                        }
                        if (data != null)
                        {
                            uint deadline = uint.MaxValue; //1 << 31
                            bool deletedTile = false;
                            if (deletedTiles.TryGetValue(id, out task))
                            {
                                // if the task for creating this tile still exists, we reuse it
                                deletedTile = true;
                                deletedTiles.Remove(id);
                            }
                            task = producers[producerId].createTile(level, tx, ty, data, deadline, task);
                            // creates the requested tile
                            Tile t = new Tile(producerId, level, tx, ty, task, data);
                            //TODO List<Tile> li =
                            unusedTilesOrder.Add(t);
                            unusedTiles[id] = unusedTilesOrder;
                            if (deletedTile)
                            {
                                // if the tile data was not in storage and if the task to create it
                                // was reused from a deleted tile, we need to reexecute the task
                                // to recreate the tile data
                                if (scheduler == null)
                                {
                                    task.setIsDone(false, 0, Task.reason.DATA_NEEDED);
                                }
                                else
                                {
                                    scheduler.reschedule(task, Task.reason.DATA_NEEDED, deadline);
                                }
                            }
                            /*if (Logger.DEBUG_LOGGER != null) {
                                ostringstream oss;
                                oss << "tiles: " << usedTiles.size() << " used, " << unusedTiles.size() << " reusable";
                                Logger.DEBUG_LOGGER.log("CACHE", oss.str());
                            }*/
                        }
                    }
                }
                return task;
            }
        }

        /*
         * Decrements the number of users of this tile by one. If this number
         * becomes 0 the tile is marked as unused, and so can be evicted from the
         * cache at any moment.
         *
         * @param t a tile currently in use.
         * @return the number of users of this tile, <i>after</i> it has been
         *      decremented.
         */
        public int putTile(Tile t)
        {
            lock (mutex)
            {
                t.users -= 1;
                if (t.users == 0)
                {
                    // the tile is now unused
                    Tile.TId id = t.getTId();
                    // removes it from the used tiles list
                    //TODO map<Tile.TId, Tile*>.iterator i = usedTiles.find(id);
                    //TODO Debug.Assert(i != usedTiles.end() && i.second == t);
                    usedTiles.Remove(id);
                    // adds it to the unused tiles list
                    Debug.Assert(!unusedTiles.ContainsKey(id));
                    //list<Tile*>.iterator li =
                    unusedTilesOrder.Add(t);
                    unusedTiles[id] = unusedTilesOrder;
                    /*if (Logger.DEBUG_LOGGER != null) {
                        ostringstream oss;
                        oss << "tiles: " << usedTiles.size() << " used, " << unusedTiles.size() << " reusable";
                        Logger.DEBUG_LOGGER.log("CACHE", oss.str());
                    }*/
                }
                int users = t.users;

                return users;
            }
        }

        /*
         * Invalidates the tiles from this cache produced by the given producer.
         * This means that the tasks to produce the actual data of these tiles will
         * be automatically reexecuted before the data can be used.
         *
         * @param producerId the id of a producer using this cache.
         *      See TileProducer#getId.
         */
        public void invalidateTiles(int producerId)
        {
            // marks the tasks to produce the tiles of the given producer as not done
            // so that they will be reexecuted when their result will be needed
            lock (mutex)
            {
                foreach (Tile tile in usedTiles.Values)
                {
                    if (tile.producerId == producerId)
                    {
                        if (scheduler == null)
                        {
                            tile.task.setIsDone(false, 0, Task.reason.DATA_CHANGED);
                        }
                        else
                        {
                            scheduler.reschedule(tile.task, Task.reason.DATA_CHANGED, uint.MaxValue);
                        }
                    }
                }
                foreach (var j in unusedTilesOrder)
                {
                    if (j.producerId == producerId)
                    {
                        if (scheduler == null)
                        {
                            j.task.setIsDone(false, 0, Task.reason.DATA_CHANGED);
                        }
                        else
                        {
                            scheduler.reschedule(j.task, Task.reason.DATA_CHANGED, uint.MaxValue);
                        }
                    }
                }
                foreach (var k in deletedTiles)
                {
                    if (k.Key.Item1 == producerId)
                    {
                        if (scheduler == null)
                        {
                            k.Value.setIsDone(false, 0, Task.reason.DATA_CHANGED);
                        }
                        else
                        {
                            scheduler.reschedule(k.Value, Task.reason.DATA_CHANGED, uint.MaxValue);
                        }
                    }
                }
            }
        }

        /*
         * Invalidates the selected tile from this cache produced by the given producer.
         * This means that the tasks to produce the actual data of this tile will
         * be automatically reexecuted before the data can be used.
         *
         * @param producerId the id of a producer using this cache.
         *      See TileProducer#getId.
         * @param level the level of a tile in the producer
         * @param tx the x coord of that tile
         * @param ty the y coord of that tile
         */
        public void invalidateTile(int producerId, int level, int tx, int ty)
        {
            Tile.TId id = TileCache.Tile.getTId(producerId, level, tx, ty);

            lock (mutex)
            {
                foreach (var i in usedTiles)
                {
                    if (i.Key == id)
                    {
                        if (scheduler == null)
                        {
                            i.Value.task.setIsDone(false, 0, Task.reason.DATA_CHANGED);
                        }
                        else
                        {
                            scheduler.reschedule(i.Value.task, Task.reason.DATA_CHANGED, uint.MaxValue);
                        }
                    }
                 }

                foreach (Tile j in unusedTilesOrder)
                {
                    if (j.getTId() == id)
                    {
                        if (scheduler == null)
                        {
                            j.task.setIsDone(false, 0, Task.reason.DATA_CHANGED);
                        }
                        else
                        {
                            scheduler.reschedule(j.task, Task.reason.DATA_CHANGED, uint.MaxValue);
                        }
                    }
                }
                foreach (var k in deletedTiles)
                {
                    if (k.Key == id)
                    {
                        if (scheduler == null)
                        {
                            k.Value.setIsDone(false, 0, Task.reason.DATA_CHANGED);
                        }
                        else
                        {
                            scheduler.reschedule(k.Value, Task.reason.DATA_CHANGED, uint.MaxValue);
                        }
                    }
                }
            }
        }


        /*
         * Creates a new uninitalized TileCache.
         */
        protected TileCache() { }

        /*
         * The name of this cache for debugging purpose.
         */
        protected string name;

        /*
         * Initializes this TileCache.
         *
         * @param storage the tile storage to store the actual tiles data.
         * @param name name of this cache, for logging.
         * @param scheduler an optional scheduler to schedule the creation of
         *      prefetched tiles. If no scheduler is specified, prefetch is
         *      disabled.
         */
        protected void init(TileStorage storage, string name, Scheduler scheduler = null)
        {
            this.nextProducerId = 0;
            this.storage = storage;
            this.scheduler = scheduler;
            this.queries = 0;
            this.misses = 0;
            this.name = name;
            mutex = new object();
#if TODO
            pthread_mutexattr_t attrs;
            pthread_mutexattr_init(&attrs);
            pthread_mutexattr_settype(&attrs, PTHREAD_MUTEX_RECURSIVE);
            pthread_mutex_init((pthread_mutex_t*)mutex, &attrs);
            pthread_mutexattr_destroy(&attrs);
 
#endif
            throw new NotImplementedException();
        }

        protected void swap(TileCache c)
        {
        }


        //typedef std.map<Tile.TId, std.list<Tile*>.iterator> Cache;
        public class Cache : Dictionary<Tile.TId, List<Tile>> { }

        /*
         * Next local identifier to be used for a TileProducer using this cache.
         */
        internal int nextProducerId;

        /*
        * The producers that use this TileCache. Maps local %producer identifiers to
        * actual producers.
        */
        internal Dictionary<int, TileProducer> producers = new Dictionary<int, TileProducer>();

        /*
         * The storage to store the tiles data.
         */
        private TileStorage storage;

        /*
         * The scheduler to schedule prefetched tiles creation tasks, and to
         * reschedule invalidated tiles creation tasks.
         */
        private Scheduler scheduler;

        /*
         * The tiles currently in use. These tiles cannot be evicted from the cache
         * and from the TileStorage, until they become unused. Maps tile identifiers
         * to actual tiles.
         */
        private Dictionary<Tile.TId, Tile> usedTiles = new Dictionary<Tile.TId,Tile>();

        /*
         * The unused tiles. These tiles can be evicted from the cache at any
         * moment. Maps tile identifiers to positions in the ordered list of tiles
         * #unusedTilesOrder (used to implement a LRU cache).
         */
        private Cache unusedTiles = new Cache();

        /*
         * The unused tiles, ordered by date of last use (to implement a LRU cache).
         */
        private List<Tile> unusedTilesOrder = new List<Tile>();

        /*
         * The tasks to produce the data of deleted tiles. When an unused tile is
         * evicted from the cache it is destroyed, but its %producer task may not be
         * destroyed (if there remain some reference to it, for example via a task
         * graph). If the tile is needed again, #getTile will create a new Tile,
         * which could produce a new %producer task. Hence we could get two %producer
         * tasks for the same tile, which could lead to inconsistencies (the two
         * tasks may not have the same execution state, may not use the same storage
         * to store their result, etc). To avoid this problem we store the tasks of
         * deleted tiles in this map, in order to reuse them if a deleted tile is
         * needed again. When a %producer task gets deleted, it removes itself from
         * this map by calling #createTileTaskDeleted (because then it is not a
         * problem to recreate a new Task, there will be no duplication). So the size
         * of this map cannot grow unbounded.
         */
        private Dictionary<Tile.TId, Task> deletedTiles = new Dictionary<Tile.TId,Task>();

        /*
         * The number of queries to this tile cache. Only used for statistics.
         */
        private int queries;

        /*
         * The number of missed queries to this tile cache. This is the number of
         * times a tile was requested but not found in the cache, requiring to
         * (re)create it. Only used for statistics.
         */
        private int misses;

        /*
         * A mutex to serialize parallel accesses to this cache.
         */
        private object mutex;

        /*
         * Notifies this TileCache that a tile creation task has been deleted.
         */
        internal void createTileTaskDeleted(int producerId, int level, int tx, int ty)
        {
            Tile.TId id = Tile.getTId(producerId, level, tx, ty);
            Debug.Assert(mutex != null);
            lock (mutex)
            {
                Debug.Assert(deletedTiles.ContainsKey(id));
                deletedTiles.Remove(id);
            }
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }


}
