using Sxta.Render.Resources;

namespace Sxta.Proland.Forest.XmlResources
{
    public class RegisterResourceReader
    {
        public static void RegisterResources()
        {
            ResourceFactory.getInstance().addType("plants", PlantsResource.Create);
            ResourceFactory.getInstance().addType("drawPlants", DrawPlantsTaskResource.Create);
            ResourceFactory.getInstance().addType("drawPlantsShadow", DrawPlantsShadowTaskResource.Create);
            ResourceFactory.getInstance().addType("lccProducer", LccProducerResource.Create);
        }
    }
}
