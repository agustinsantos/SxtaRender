using proland;
using Sxta.Render;
using Sxta.Render.Resources;
using Sxta.Render.Resources.XmlResources;
using System.Xml;


namespace Sxta.Proland.Core.Producer.XmlResources
{
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
