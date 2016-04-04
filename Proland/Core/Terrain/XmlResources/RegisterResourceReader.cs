using Sxta.Render.Resources;

namespace Sxta.Proland.Core.Terrain.XmlResources
{
    public class RegisterResourceReader
    {
        public static void RegisterResources()
        {
            ResourceFactory.getInstance().addType("terrainNode", TerrainNodeResource.Create);
            ResourceFactory.getInstance().addType("updateTerrain", UpdateTerrainTaskResource.Create);
            ResourceFactory.getInstance().addType("drawTerrain", DrawTerrainTaskResource.Create);
        }
    }
}
