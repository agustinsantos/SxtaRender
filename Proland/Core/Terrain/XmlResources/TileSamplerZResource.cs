using proland;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sxta.Proland.Core.Terrain.XmlResources
{
    class TileSamplerZResource : ResourceTemplate<TileSamplerZ>
    {
        public static TileSamplerZResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new TileSamplerZResource(manager, name, desc, e);
        }
        public TileSamplerZResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(10, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "id,name,sampler,producer,terrains,storeLeaf,storeParent,storeInvisible,async,");
            string uname;
            TileProducer producer;
            uname = getParameter(desc, e, "sampler");
            producer = (TileProducer)(manager.loadResource(getParameter(desc, e, "producer")).get());
            valueC.init(uname, producer);
            string terrainsAtt = e.GetAttribute("terrains");
            if (terrainsAtt != null)
            {
                string nodes = terrainsAtt;
                string[] stringSeparator = new string[] { "," };
                string[] result = nodes.Split(stringSeparator, StringSplitOptions.None); //Maybe StringSplitOptions.RemoveEmptyEntries?
                foreach (var node in result)
                {
                    valueC.addTerrain((TerrainNode)(manager.loadResource(node).get()));
                }
            }
            if (e.GetAttribute("storeLeaf") != null && e.GetAttribute("storeLeaf") == "false")
            {
                valueC.setStoreLeaf(false);
            }
            if (e.GetAttribute("storeParent") != null && e.GetAttribute("storeParent") == "false")
            {
                valueC.setStoreParent(false);
            }
            if (e.GetAttribute("storeInvisible") != null && e.GetAttribute("storeInvisible") == "false")
            {
                valueC.setStoreInvisible(false);
            }
            if (e.GetAttribute("async") != null && e.GetAttribute("async") == "true")
            {
                valueC.setAsynchronous(true);
            }
        }
    }
}
