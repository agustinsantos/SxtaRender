using Sxta.Math;
using Sxta.Render.Scenegraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxta.Proland.Core.Producer
{
    /// <summary>
    /// An abstract layer for a TileProducer. Some tile producers can be
    /// customized with layers modifying the default tile production algorithm
    /// (for instance to add roads or rivers to an orthographic tile producer).
    /// For these kind of producers, each method of this class is called during
    /// the corresponding method in the TileProducer. The default implementation
    /// of these methods in this class is empty.
    /// @authors Eric Bruneton, Antoine Begault
    /// </summary>
    public abstract class TileLayer
    {

        /// <summary>
        /// Creates a new TileLayer.
        /// </summary>
        /// <param name="type">the layer's type.</param>
        /// <param name="deform">whether we apply a spherical deformation on the layer or not.</param>
        public TileLayer(string type, bool deform = false) { throw new NotImplementedException(); }

        /*
         * Deletes this TileLayer.
         */
        //public virtual ~TileLayer();


        /// <summary>
        /// Returns the TileCache that stores the tiles produced by the producer using this TileLayer.
        /// </summary>
        /// <returns></returns>
        public TileCache getCache() { throw new NotImplementedException(); }


        /// <summary>
        /// Returns the id of the producer using this TileLayer. This id is local to the TileCache used by
        /// this producer, and is used to distinguish all the producers that use this
        /// cache.
        /// </summary>
        /// <returns></returns>
        public int getProducerId() { throw new NotImplementedException(); }


        /// <summary>
        ///   Returns the tile size, i.e. the size in pixels of each tile of the producer to which this layer
        /// belongs. This size includes borders.
        /// </summary>
        /// <returns></returns>
        public int getTileSize() { throw new NotImplementedException(); }

        /*
         * Returns the size in pixels of the border of each tile. Tiles made of
         * raster data may have a border that contains the valueC of the neighboring
         * pixels of the tile. For instance if the tile size (returned by
         * TileStorage#getTileSize) is 196, and if the tile border is 2, this means
         * that the actual tile data is 192x192 pixels, with a 2 pixel border that
         * contains the valueC of the neighboring pixels. Using a border introduces
         * data redundancy but is usefull to get the valueC of the neighboring pixels
         * of a tile without needing to load the neighboring tiles.
         */
        public int getTileBorder() { throw new NotImplementedException(); }

        /*
         * Returns the size in meters of the root quad produced by the producer using this Layer.
         */
        public float getRootQuadSize() { throw new NotImplementedException(); }

        /*
         * Returns the ox,oy,l coordinates of the given tile.
         */
        public Vector3d getTileCoords(int level, int tx, int ty) { throw new NotImplementedException(); }

        /*
         * Returns true if a spherical deformation is applied on the layer or not.
         */
        public bool isDeformed() { throw new NotImplementedException(); }

        public void getDeformParameters(Vector3d tileCoords, out Vector2d nx, out Vector2d ny, out Vector2d lx, out Vector2d ly) { throw new NotImplementedException(); }

        /*
         * Returns true if this TileLayer is enabled.
         */
        public bool isEnabled() { throw new NotImplementedException(); }

        /*
         * Enables or disables this TileLayer.
         *
         * @param enabled true to enable this TileLayer, false otherwise.
         */
        public void setIsEnabled(bool enabled) { throw new NotImplementedException(); }


        /*
         * Sets the TileCache that stores the tiles produced by this Layer.
         */
        public virtual void setCache(TileCache cache, int producerId) { throw new NotImplementedException(); }

        /*
         * Returns the tile producers used by this TileLayer.
         *
         * @param[out] producers the tile producers used by this TileLayer.
         */
        public virtual void getReferencedProducers(List<TileProducer> producers) { throw new NotImplementedException(); }

        /*
         * Sets the tile size valueC.
         */
        public virtual void setTileSize(int tileSize, int tileBorder, float rootQuadSize) { throw new NotImplementedException(); }


        /*
         * Notifies this Layer that the given tile of its TileProducer is in use.
         * This happens when this tile was not previously used, and has been
         * requested with TileProducer#getTile (see also TileCache#getTile).
         *
         * @param level the tile's quadtree level.
         * @param tx the tile's quadtree x coordinate.
         * @param ty the tile's quadtree y coordinate.
         * @param deadline the deadline at which the tile data must be ready. 0 means
         *      the current frame.
         */
        public virtual void useTile(int level, int tx, int ty, uint deadline) { throw new NotImplementedException(); }

        /*
         * Notifies this Layer that the given tile of its TileProducer is unused.
         * This happens when the number of users of this tile becomes null, after a
         * call to TileProducer#putTile (see also TileCache#putTile).
         *
         * @param level the tile's quadtree level.
         * @param tx the tile's quadtree x coordinate.
         * @param ty the tile's quadtree y coordinate.
         */
        public virtual void unuseTile(int level, int tx, int ty) { throw new NotImplementedException(); }

        /*
         * Schedules a prefetch task to create the given tile. If the requested tile
         * is currently in use or in cache but unused, this method does nothing.
         * Otherwise it gets an unused tile storage (evicting an unused tile if
         * necessary), and then creates and schedules a task to produce the data of
         * the requested tile.
         *
         * @param level the tile's quadtree level.
         * @param tx the tile's quadtree x coordinate.
         * @param ty the tile's quadtree y coordinate.
         * @return true if this method has been able to schedule a prefetch task
         *      for the given tile.
         */
        public virtual void prefetchTile(int level, int tx, int ty) { throw new NotImplementedException(); }

        /*
         * Starts the creation of a tile.
         * See TileProducer#startCreateTile.
         */
        public virtual void startCreateTile(int level, int tx, int ty, uint deadline, Task task, TaskGraph owner) { throw new NotImplementedException(); }

        /*
         * Sets the execution context for the Task that produces the tile data.
         * See TileProducer#beginCreateTile.
         */
        public virtual void beginCreateTile() { throw new NotImplementedException(); }

        /*
         * Creates the given tile.
         * See TileProducer#doCreateTile.
         */
        public abstract bool doCreateTile(int level, int tx, int ty, TileStorage.Slot data);

        /*
         * Restores the execution context for the Task that produces the tile data.
         * See TileProducer#endCreateTile.
         */
        public virtual void endCreateTile() { throw new NotImplementedException(); }

        /*
         * Stops the creation of a tile.
         * See TileProducer#stopCreateTile.
         */
        public virtual void stopCreateTile(int level, int tx, int ty) { throw new NotImplementedException(); }

        /*
         * Invalidates the tiles modified by this layer.
         * This means that the tasks to produce the actual data of these tiles will
         * be automatically reexecuted before the data can be used.
         */
        public virtual void invalidateTiles() { throw new NotImplementedException(); }


        /*
         * Initializes TileLayer fields.
         *
         * @param deform whether we apply a spherical deformation on the layer or not.
         */
        protected void init(bool deform = false) { throw new NotImplementedException(); }

        protected virtual void swap(TileLayer p) { throw new NotImplementedException(); }


        /*
         * The TileCache of the TileProducer to which this TileLayer belongs.
         */
        private TileCache cache;

        /*
         * The id of the TileProducer to which this TileLayer belongs in #cache.
         */
        private int producerId;

        /*
         * Size in pixels of each tile of the producer to which this layer
         * belongs. This size includes borders.
         */
        private int tileSize;

        /*
         * Size in pixels of the border of each tile of the producer to which
         * this layer belongs. See TileProducer#getTileBorder.
         */
        private int tileBorder;

        /*
         * Size in meters of the root quad produced by the producer to which
         * this layer belongs.
         */
        private float rootQuadSize;

        /*
         * Whether we apply a spherical deformation on the layer or not.
         */
        private bool deform;

        private bool enabled;
    }
}
