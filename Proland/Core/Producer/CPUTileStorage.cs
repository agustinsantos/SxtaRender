﻿using Sxta.Render.Resources;
using System.Xml;

namespace proland
{
    /// <summary>
    ///  A TileStorage that store tiles on CPU.
    /// </summary>
    /// <typeparam name="T">the type of each tile pixel component (e.g. char, float, etc).</typeparam>
    public class CPUTileStorage<T> : TileStorage, ISwappable<CPUTileStorage<T>>
    {
        /// <summary>
        /// A slot managed by a CPUTileStorage.
        /// </summary>
        public class CPUSlot : Slot
        {
            /// <summary>
            /// The data of the tile stored in this slot.
            /// </summary>
            public T[] data;

            /// <summary>
            /// The number of elements in the data array.
            /// </summary>
            public int size;

            /// <summary>
            /// Creates a new CPUSlot. This constructor creates a new array to store
            /// the tile data.
            /// </summary>
            /// <param name="owner">the TileStorage that manages this slot.</param>
            /// <param name="size"> the number of elements in the data array.</param>
            public CPUSlot(TileStorage owner, int size) : base(owner)
            {
                this.data = new T[size];
                this.size = size;
            }

        }

        /// <summary>
        /// Creates a new CPUTileStorage.
        /// </summary>
        /// <param name="tileSize">the size in pixels of each (square) tile.</param>
        /// <param name="channels">the number of components per pixel of each tile. Each the number of components per pixel of each tile. Each</param>
        /// <param name="capacity">the number of slots managed by this tile storage.</param>
        public CPUTileStorage(int tileSize, int channels, int capacity) : base()
        {
            init(tileSize, channels, capacity);

        }

        /*
        * Deletes this CPUTileStorage.
        */
        //public  virtual ~CPUTileStorage()
        //{
        //    //      ¿EMPTY?
        //}

        /// <summary>
        ///  Returns the number of components per pixel of each tile. Each component
        /// is of type T.
        /// </summary>
        /// <returns></returns>
        public int getChannels()
        {
            return channels;
        }

        /// <summary>
        /// Creates an uninitialized CPUTileStorage.
        /// </summary>
        protected CPUTileStorage() : base()
        {
            //      ¿EMPTY?
        }

        /// <summary>
        /// Initializes this CPUTileStorage.
        /// </summary>
        /// <param name="tileSize">the size in pixels of each (square) tile.</param>
        /// <param name="channels">the number of components per pixel of each tile. Each component is of type T.</param>
        /// <param name="capacity">the number of slots managed by this tile storage.</param>
        internal void init(int tileSize, int channels, int capacity)
        {
            base.init(tileSize, capacity);
            this.channels = channels;
            int size = tileSize * tileSize * channels;
            for (int i = 0; i < capacity; i++)
            {
                freeSlots.Add(new CPUSlot(this, size));
            }
        }

        public void swap(CPUTileStorage<T> t)
        {
            //  ¿EMPTY?
        }

        /// <summary>
        /// The number of components per pixel of each tile.
        /// </summary>
        private int channels;

    }
    //Definimos el tipo en T en CPUTileStorageResource??????
}
