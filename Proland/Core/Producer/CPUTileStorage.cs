using log4net;
using Sxta.Core;
using Sxta.Math;
using Sxta.Render;
using Sxta.Render.Scenegraph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Sxta.Proland.Core.Producer
{
    /**
51 	 * A TileStorage that store tiles on CPU.
52 	 * @ingroup producer
53 	 * @authors Eric Bruneton, Antoine Begault
54 	 *
55 	 * @tparam T the type of each tile pixel component (e.g. char, float, etc).
56 	 */
    public class CPUTileStorage<T> : TileStorage
    {
        /**
68 	    * The data of the tile stored in this slot.
69 	    */
        public T data;
        /**
73 	    * The number of elements in the data array.
74 	    */
        public int size;
        /**
78 	    * Creates a new CPUSlot. This constructor creates a new array to store
79 	    * the tile data.
80 	    *
81 	    * @param owner the TileStorage that manages this slot.
82 	    * @param size the number of elements in the data array.
83 	    */

        public CPUSlot(TileStorage owner, int size) : base(owner) 
        {
            this.data = new T.size; //new T[size]
            this.size = size;
        }

        ~CPUSlot()
        {
            if (data != null)
            {
                delete[] data;  //Delete process?????
            }
        }

        /** ------------------------------------CONSTRUCTOR------------------------------------ **
        * Creates a new CPUTileStorage.
        *
        * @param tileSize the size in pixels of each (square) tile.
        * @param channels the number of components per pixel of each tile. Each
        *      component is of type T.
        * @param capacity the number of slots managed by this tile storage.
        */
        CPUTileStorage(int tileSize, int channels, int capacity) : base(owner)
        {
            init(tileSize, channels, capacity);
        }

        /**
        * Deletes this CPUTileStorage.
        */
        virtual ~CPUTileStorage()
        {
            //      ¿EMPTY?
        }
        /**
        * Returns the number of components per pixel of each tile. Each component
        * is of type T.
        */
        int getChannels()
        {
            return channels;
        }
        /**
        * Creates an uninitialized CPUTileStorage.
        */
        protected CPUTileStorage() : TileStorage()
        {
            //      ¿EMPTY?
        }
        /**
        * Initializes this CPUTileStorage.
        *
        * @param tileSize the size in pixels of each (square) tile.
        * @param channels the number of components per pixel of each tile. Each
        *      component is of type T.
        * @param capacity the number of slots managed by this tile storage.
        */
        protected void init(in tileSize, int channels, int capacity)
        {
            TileStorage.init(tileSize, capacity);
            this.channels = channels;
            int size = tileSize * tileSize * channels;
            for (int i = 0; i < capacity; i++)
            {
                freeSlots.push_back(new CPUSlot(this, size));
            }
        }

        protected void swap(ptr<CPUTileStorage<T>> t)
        {
            //  ¿EMPTY?
        }

        /**
        * The number of components per pixel of each tile.
        */
        private int channels;

        extern const char cpuByteTileStorage[] = "cpuByteTileStorage";

        extern const char cpuFloatTileStorage[] = "cpuFloatTileStorage";

        static ResourceFactory::Type<cpuByteTileStorage, CPUTileStorageResource<unsigned char>> CPUByteTileStorageType;

        static ResourceFactory::Type<cpuFloatTileStorage, CPUTileStorageResource<float>> CPUFloatTileStorageType;

        public CPUTileStorageResource() : ResourceTemplate<0, CPUTileStorage<T>>()
 	    {

 	    }
    }
}
