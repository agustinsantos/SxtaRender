using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sxta.Render.Resources;
using Sxta.Render;
using System.Xml;

namespace proland
{
    class ObjectTileStorage : TileStorage, ISwappable<ObjectTileStorage>
    {
        /**
         * A tile managed by an ObjectTileStorage.
         */
        public class ObjectSlot : Slot
        {
            /**
             * The tile data.
             */
            public Object data;

            /**
             * Creates a new ObjectSlot. This constructor does NOT allocate any
             * object.
             *
             * @param owner the TileStorage that manages this tile.
             */
            public ObjectSlot(TileStorage owner) : base(owner)
            {
                data = null;
            }

            /**
             * Deletes this ObjectSlot. This deletes the reference to #data.
             */
            //virtual ~ObjectSlot();
        }

        /**
         * Creates a new ObjectTileStorage.
         *
         * @param capacity the number of tiles managed by this tile storage.
         */
        public ObjectTileStorage(int capacity) : base()
        {
            init(capacity);
        }

        /**
         * Deletes this ObjectTileStorage.
         */
        //virtual ~ObjectTileStorage();

        /**
         * Creates an uninitialized ObjectTileStorage.
         */
        protected ObjectTileStorage() : base()
        {
        }

        /**
         * Initializes this ObjectTileStorage.
         *
         * @param capacity the number of tiles managed by this tile storage.
         */
        internal void init(int capacity)
        {
            init(0, capacity);
            for (int i = 0; i < capacity; i++)
            {
                freeSlots.Add(new ObjectSlot(this));
            }
        }

        public void swap(ObjectTileStorage t)
        {

        }
    }
    class ObjectTileStorageResource : ResourceTemplate<ObjectTileStorage>
    {
        public static ObjectTileStorageResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new ObjectTileStorageResource(manager, name, desc, e);
        }
        public ObjectTileStorageResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) : base(20, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            int capacity;
            Resource.checkParameters(desc, e, "name,capacity,");
            Resource.getIntParameter(desc, e, "capacity", out capacity);
            valueC.init(capacity);
        }
        public const string objectTileStorage = "objectTileStorage";

        #if TODO
        static ResourceFactory::Type<objectTileStorage, ObjectTileStorageResource> ObjectTileStorageType;
        #endif
    }
}
