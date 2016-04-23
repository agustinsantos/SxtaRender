using Sxta.Render.Resources;

namespace Sxta.Proland.Core.Terrain.XmlResources
{
    public class RegisterResourceReader
    {
        public static void RegisterResources()
        {
            //TERRAIN RESOURCES
            ResourceFactory.getInstance().addType("terrainNode", TerrainNodeResource.Create);
            ResourceFactory.getInstance().addType("updateTerrain", UpdateTerrainTaskResource.Create);
            ResourceFactory.getInstance().addType("drawTerrain", DrawTerrainTaskResource.Create);
            ResourceFactory.getInstance().addType("tileSampler", TileSamplerResource.Create);
            //PRODUCER RESOURCES
            /**ResourceFactory.getInstance().addType("tileCache", proland.TileCacheResource.Create);
            ResourceFactory.getInstance().addType("cpuByteTileStorage", proland.CPUTileStorageResource<T>.Create);
            ResourceFactory.getInstance().addType("cpuFloatTileStorage", proland.CPUTileStorageResource<T>.Create);
            ResourceFactory.getInstance().addType("gpuTileStorage", proland.GPUTileStorageResource.Create);
            ResourceFactory.getInstance().addType("objectTileStorage", proland.ObjectTileStorageResource.Create);*/
        }
    }
}
