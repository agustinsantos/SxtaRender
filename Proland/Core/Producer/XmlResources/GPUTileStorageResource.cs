using proland;
using Sxta.Render;
using Sxta.Render.Resources;
using Sxta.Render.Resources.XmlResources;
using System.Xml;

namespace Sxta.Proland.Core.Producer.XmlResources
{
    public class GPUTileStorageResource : ResourceTemplate<GPUTileStorage>
    {
        public static GPUTileStorageResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new GPUTileStorageResource(manager, name, desc, e);
        }
        public GPUTileStorageResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) :
            base(20, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            int tileSize;
            int nTiles;
            TextureInternalFormat tf;
            TextureFormat f;
            PixelType t;
            Texture.Parameters _params = null;
            bool useTileMap = false;
            checkParameters(desc, e, "name,tileSize,nTiles,tileMap,internalformat,format,type,min,mag,minLod,maxLod,minLevel,maxLevel,swizzle,anisotropy,");
            TextureResource.getParameters(desc, e, out tf, out f, out t);
            TextureResource.getParameters(desc, e, ref _params);
            getIntParameter(desc, e, "tileSize", out tileSize);
            getIntParameter(desc, e, "nTiles", out nTiles);
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("tileMap")))
            {
                useTileMap = e.GetAttribute("tileMap") == "true"; // == 0???
            }
            valueC.init(tileSize, nTiles, tf, f, t, _params, useTileMap);

        }
    }
}
