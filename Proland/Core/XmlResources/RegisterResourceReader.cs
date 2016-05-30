namespace Sxta.Proland.Core.XmlResources
{
    public class RegisterResourceReader
    {
        public static void RegisterResources()
        {
            //PRODUCER RESOURCES
            Sxta.Proland.Core.Producer.XmlResources.RegisterResourceReader.RegisterResources();

            //TERRAIN RESOURCES
            Sxta.Proland.Core.Terrain.XmlResources.RegisterResourceReader.RegisterResources();

            //OCEAN RESOURCES
            Sxta.Proland.Ocean.XmlResources.RegisterResourceReader.RegisterResources();

            //FOREST RESOURCES
            Sxta.Proland.Forest.XmlResources.RegisterResourceReader.RegisterResources();
        }
    }
}
