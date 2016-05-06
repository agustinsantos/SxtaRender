﻿using Sxta.Render.Resources;
using proland;

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
        }
    }
}
