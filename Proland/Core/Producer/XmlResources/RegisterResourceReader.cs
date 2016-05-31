using proland;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.Proland.Core.Producer.XmlResources
{
    public class RegisterResourceReader
    {
        public static void RegisterResources()
        {
            //PRODUCER RESOURCES
            ResourceFactory.getInstance().addType("tileCache", TileCacheResource.Create);
            ResourceFactory.getInstance().addType("gpuTileStorage", GPUTileStorageResource.Create);
            ResourceFactory.getInstance().addType("objectTileStorage", ObjectTileStorageResource.Create);
            ResourceFactory.getInstance().addType("cpuByteTileStorage", CPUByteTileStorageResource.Create);
            ResourceFactory.getInstance().addType("cpuFloatTileStorage", CPUFloatTileStorageResource.Create);
        }
    }
}
