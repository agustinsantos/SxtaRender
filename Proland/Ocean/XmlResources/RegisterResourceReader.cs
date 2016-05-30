using Sxta.Render.Resources;

namespace Sxta.Proland.Ocean.XmlResources
{
    public class RegisterResourceReader
    {
        public static void RegisterResources()
        {
            ResourceFactory.getInstance().addType("drawOceanFFT", DrawOceanFFTTaskResource.Create);
            ResourceFactory.getInstance().addType("drawOcean", DrawOceanTaskResource.Create);
        }
    }
}
