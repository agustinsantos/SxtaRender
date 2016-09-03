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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    /// </summary>
    public class TileStorage : IDisposable
    {
        /// <summary>
        ///  A slot managed by a TileStorage. Concrete sub classes of this class must
        ///  provide a reference to the actual tile data.
        /// </summary>
        public class Slot : IDisposable
        {
            /// <summary>
            /// The id of the tile currently stored in this slot.
            /// </summary>
            public TileCache.Tile.TId id;

            /// <summary>
            /// The task that is responsible for producing the data for the tile
            /// stored in this slot.
            /// </summary>
            public object producerTask;

            /// <summary>
            /// Creates a new TileStorage.Slot.
            /// </summary>
            /// <param name="owner">the TileStorage that will manage this slot.</param>
            public Slot(TileStorage owner)
            {
                this.owner = owner;
            }

            /// <summary>
            /// Deletes this TileStorage.Slot. This destroys the data of the tile
            /// stored in this slot, if any.
            /// </summary>
            ~Slot()
            {
                Dispose(false);
            }

            #region IDisposable implementation

            /// <summary>
            /// Deletes this TileStorage.Slot. This destroys the data of the tile
            /// stored in this slot, if any.
            /// </summary>
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
                        //#if DEBUG
                        //                    traceDisposable.CheckDispose();
                        //#endif
                        if (isLocked)
                        {
                            Debugger.Break();
                            System.Threading.Monitor.Exit(mutex);
                        }
                    }

                    m_Disposed = true;
                }
            }

            #endregion

            /// <summary>
            /// Returns the TileStorage that manages this slot.
            /// </summary>
            /// <returns></returns>
            public TileStorage getOwner()
            {
                return owner;
            }

            /// <summary>
            /// Locks or unlocks this slots. Slots can be accessed by several threads
            /// simultaneously.This lock can be used to serialize these accesses.
            /// In particular it is used to change the #producerTask, when a slot is
            /// reused to store new data.
            /// </summary>
            /// <param name="lock_">true to lock the slot, false to unlock it.</param>
            public void lock_(bool lock_)
            {
                if (lock_)
                {
                    //lock (mutex)
                    //Debugger.Break();
                    System.Threading.Monitor.Enter(mutex);
                    isLocked = true;
                }
                else
                {
                    //unlock(mutex)
                    //Debugger.Break();
                    System.Threading.Monitor.Exit(mutex);
                    isLocked = false;
                }
            }

            /// <summary>
            /// The TileStorage that manages this slot.
            /// </summary>
            private TileStorage owner;

            /// <summary>
            ///  A mutex used to serialize parallel accesses to this slot.
            /// </summary>
            private object mutex = new object();
            private bool isLocked = false;
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

        /// <summary>
        /// Deletes this TileStorage. This deletes the data associated with all the
        /// slots managed by this tile storage.
        /// </summary>
        ~TileStorage()
        {
            Dispose(false);
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
                    //#if DEBUG
                    //                    traceDisposable.CheckDispose();
                    //#endif
                    if (freeSlots != null)
                    {
                        foreach (Slot slot in freeSlots)
                        {
                            slot.Dispose();
                        }
                        freeSlots.Clear();
                    }
                    m_Disposed = true;
                }
            }
        }

        #endregion



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

        /// <summary>
        /// Returns the size of each tile. For tiles made of raster data, this size
        /// is the tile width in pixels(the tile height is supposed equal to the
        /// tile width).
        /// </summary>
        /// <returns></returns>
        public int getTileSize()
        {
            return tileSize;
        }

        /// <summary>
        /// Returns the total number of slots managed by this TileStorage. This
        /// includes both unused and used tiles.
        /// </summary>
        /// <returns></returns>
        public int getCapacity()
        {
            return capacity;
        }

        /// <summary>
        /// Returns the number of slots in this TileStorage that are currently unused.
        /// </summary>
        /// <returns></returns>
        public int getFreeSlots()
        {
            return freeSlots.Count();
        }

        /// <summary>
        /// The size of each tile. For tiles made of raster data, this size is the
        /// tile width in pixels(the tile height is supposed equal to the tile
        /// width).
        /// </summary>
        protected int tileSize;

        /// <summary>
        /// The total number of slots managed by this TileStorage. This includes both
        /// unused and used tiles.
        /// </summary>
        protected int capacity;

        /// <summary>
        /// The currently free slots.
        /// </summary>
        protected List<Slot> freeSlots = new List<Slot>();

        /// <summary>
        /// Creates a new uninitialized TileStorage.
        /// </summary>
        internal TileStorage()
        {

        }

        /// <summary>
        /// Initializes this TileStorage.
        /// </summary>
        /// <param name="tileSize">the size of each tile. For tiles made of raster data,
        ///  this size is the tile width in pixels(the tile height is supposed
        ///  equal to the tile width).</param>
        /// <param name="capacity">the number of slots allocated and managed by this tile
        ///  storage.This capacity is fixed and cannot change with time.</param>
        protected void init(int tileSize, int capacity)
        {
            this.tileSize = tileSize;
            this.capacity = capacity;
        }
    }
}
