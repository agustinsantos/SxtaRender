using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace proland
{
    /// <summary>
    ///  A shared storage to store tiles of the same kind. This abstract class defines
    /// the behavior of tile storages but does not provide any storage itself. The
    /// slots managed by a tile storage can be used to store any tile identified by
    /// its (level,tx,ty) coordinates. This means that a TileStorage::Slot can store
    /// the data of some tile at some moment, and then be reused to store the data of
    /// tile some time later. The mapping between tiles and TileStorage::Slot is not
    /// managed by the TileStorage itself, but by a TileCache. A TileStorage just
    /// keeps track of which slots in the pool are currently associated with a
    /// tile (i.e., store the data of a tile), and which are not. The first ones are
    /// called allocated slots, the others free slots.
    /// @ingroup producer
    /// @authors Eric Bruneton, Antoine Begault
    /// </summary>
    public class TileStorage
    {
        /// <summary>
        ///  A slot managed by a TileStorage. Concrete sub classes of this class must
        ///  provide a reference to the actual tile data.
        /// </summary>
        public class Slot
        {
             /// <summary>
             /// The id of the tile currently stored in this slot.
             /// </summary>
            public Tuple<int, Tuple<int, Tuple<int, int>>> id;

            /*
             * The task that is responsible for producing the data for the tile
             * stored in this slot.
             */
            public object producerTask;

            /*
             * Creates a new TileStorage::Slot.
             *
             * @param owner the TileStorage that will manage this slot.
             */
            public Slot(TileStorage owner)
            {
                Mutex mutex = new Mutex(true, "Mutex");
            }

            /*
             * Deletes this TileStorage::Slot. This destroys the data of the tile
             * stored in this slot, if any.
             */
            // public virtual ~Slot();

            /*
             * Returns the TileStorage that manages this slot.
             */
            public TileStorage getOwner()
            {
                return owner;
            }

            /*
             * Locks or unlocks this slots. Slots can be accessed by several threads
             * simultaneously. This lock can be used to serialize these accesses.
             * In particular it is used to change the #producerTask, when a slot is
             * reused to store new data.
             *
             * @param lock true to lock the slot, false to unlock it.
             */
            public void lock_(bool lock_)
            {
                if (lock_)
                {
                    //lock (mutex)
                }
                else
                {
                    //unlock(mutex)
                }
            }


            /*
             * The TileStorage that manages this slot.
             */
            private TileStorage owner;

            /*
             * A mutex used to serialize parallel accesses to this slot.
             */
            private object mutex;
        }

        /*
         * Creates a new TileStorage.
         *
         * @param tileSize the size of each tile. For tiles made of raster data,
         *      this size is the tile width in pixels (the tile height is supposed
         *      equal to the tile width).
         * @param capacity the number of slots allocated and managed by this tile
         *      storage. This capacity is fixed and cannot change with time.
         */
        public TileStorage(int tileSize, int capacity)
        {
            init(tileSize, capacity);
        }

        /*
         * Deletes this TileStorage. This deletes the data associated with all the
         * slots managed by this tile storage.
         */
        //public virtual ~TileStorage();

        /*
         * Returns a free slot in the pool of slots managed by this TileStorage.
         *
         * @return a free slot, or NULL if all tiles are currently allocated. The
         *      returned slot is then considered to be allocated, until it is
         *      released with deleteSlot.
         */
        public Slot newSlot()
        {
            Slot i = freeSlots.First();
            if (freeSlots.Contains(i))
            {
                Slot s = i;
                freeSlots.Remove(i);
                return s;
            }
            else
            {
                return null;
            }
        }

        /*
         * Notifies this storage that the given slot is free. The given slot can
         * then be allocated to store a new tile, i.e., it can be returned by a
         * subsequent call to newSlot.
         *
         * @param t a slot that is no longer in use.
         */
        public void deleteSlot(Slot t)
        {
            freeSlots.Remove(t);
        }

        /*
         * Returns the size of each tile. For tiles made of raster data, this size
         * is the tile width in pixels (the tile height is supposed equal to the
         * tile width).
         */
        public int getTileSize()
        {
            return tileSize;
        }

        /*
         * Returns the total number of slots managed by this TileStorage. This
         * includes both unused and used tiles.
         */
        public int getCapacity()
        {
            return capacity;
        }

        /*
         * Returns the number of slots in this TileStorage that are currently unused.
         */
        public int getFreeSlots()
        {
            return freeSlots.Count();
        }


        /*
         * The size of each tile. For tiles made of raster data, this size is the
         * tile width in pixels (the tile height is supposed equal to the tile
         * width).
         */
        protected int tileSize;

        /*
         * The total number of slots managed by this TileStorage. This includes both
         * unused and used tiles.
         */
        protected int capacity;

        /*
         * The currently free slots.
         */
        protected List<Slot> freeSlots;

        /*
         * Creates a new uninitialized TileStorage.
         */
        internal TileStorage()
        {
            
        }

        /*
         * Initializes this TileStorage.
         *
         * @param tileSize the size of each tile. For tiles made of raster data,
         *      this size is the tile width in pixels (the tile height is supposed
         *      equal to the tile width).
         * @param capacity the number of slots allocated and managed by this tile
         *      storage. This capacity is fixed and cannot change with time.
         */
        protected void init(int tileSize, int capacity)
        {
            this.tileSize = tileSize;
            this.capacity = capacity;
        }
    }
}
