using proland;
using Sxta.Render;
using Sxta.Render.Resources;
using Sxta.Render.Resources.XmlResources;
using System.Xml;


namespace Sxta.Proland.Core.Producer.XmlResources
{
    public class CPUByteTileStorageResource : ResourceTemplate<CPUTileStorage<byte>>
    {
        public static CPUByteTileStorageResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new CPUByteTileStorageResource(manager, name, desc, e);
        }

        public CPUByteTileStorageResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) :
                      base(20, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            int tileSize;
            int channels;
            int capacity;
            checkParameters(desc, e, "name,tileSize,channels,capacity,");
            getIntParameter(desc, e, "tileSize", out tileSize);
            getIntParameter(desc, e, "channels", out channels);
            getIntParameter(desc, e, "capacity", out capacity);
            valueC.init(tileSize, channels, capacity);
        }
    }
}
