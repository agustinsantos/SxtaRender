using proland;
using Sxta.Render.Resources;
using Sxta.Render.Scenegraph;
using System;
using System.Xml;


namespace Sxta.Proland.Core.Terrain.XmlResources
{
    public class TileSamplerResource : ResourceTemplate<TileSampler>
    {
        public static TileSamplerResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null)
        {
            return new TileSamplerResource(manager, name, desc, e);
        }
        public TileSamplerResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) :
            base(40, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "id,name,sampler,producer,terrains,storeLeaf,storeParent,storeInvisible,async,mipmap,");
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
            if (e.GetAttribute("mipmap") != null && e.GetAttribute("mipmap") == "true")
            {
                valueC.setMipMap(true);
            }
        }
    }
}
