using proland;
using Sxta.Proland.Terrain;
using Sxta.Render;
using Sxta.Render.Resources;
using Sxta.Render.Resources.XmlResources;
using System;
using System.Xml;

namespace Sxta.Proland.Core.Producer.XmlResources
{
    public class NormalProducerResource : ResourceTemplate<NormalProducer>
    {
        public static NormalProducerResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new NormalProducerResource(manager, name, desc, e);
        }
        public NormalProducerResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) :
        base(50, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            TileCache cache;
            TileProducer elevations;
            Texture2D normalTexture;
            Program normalsProg;
            int gridSize = 24;
            bool deform = false;
            checkParameters(desc, e, "name,cache,elevations,normalProg,gridSize,deform,");
            cache = (TileCache)manager.loadResource(getParameter(desc, e, "cache")).get();
            elevations = (TileProducer)manager.loadResource(getParameter(desc, e, "elevations")).get();
            string normals = "normalShader;";
            if (e.GetAttribute("normalProg") != null)
            {
                normals = getParameter(desc, e, "normalProg");
            }
            normalsProg = (Program)(manager.loadResource(normals).get());
            if (e.GetAttribute("gridSize") != null)
            {
                getIntParameter(desc, e, "gridSize", out gridSize);
            }
            if (e.GetAttribute("deform") != null && e.GetAttribute("deform") == "sphere")
            {
                deform = true;
            }

            int tileSize = cache.getStorage().getTileSize();
            string format = ((GPUTileStorage)(cache.getStorage())).getTexture(0).getInternalFormatName();
            if (format.Substring(0, 3) == "RG8")
            {
                format = "RGBA8";
            }

            string normalTex = "rendebuffer-" + tileSize + "-" + format;
            normalTexture = (Texture2D)manager.loadResource(normalTex).get();

            valueC.init(cache, elevations, normalTexture, normalsProg, gridSize, deform);
        }

        public override bool prepareUpdate()
        {
#if TODO
        if (dynamic_cast<Resource*>(normals.get())->changed()) {
            invalidateTiles();
        }
        return ResourceTemplate<50, NormalProducer>::prepareUpdate();
#endif
            throw new NotImplementedException();
        }
    }
}
