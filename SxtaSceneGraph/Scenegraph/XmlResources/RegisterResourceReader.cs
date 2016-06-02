using Sxta.Render.Resources;

namespace Sxta.Render.Scenegraph.XmlResources
{
    public static class RegisterResourceReader
    {
        public static void RegisterResources()
        {
            ResourceFactory.getInstance().addType("callMethod", CallMethodTaskResource.Create);
            ResourceFactory.getInstance().addType("drawMesh", DrawMeshTaskResource.Create);
            ResourceFactory.getInstance().addType("foreach", LoopTaskResource.Create);
            ResourceFactory.getInstance().addType("sequence", SequenceTaskResource.Create);
            ResourceFactory.getInstance().addType("setProgram", SetProgramTaskResource.Create);
            ResourceFactory.getInstance().addType("setState", SetStateTaskResource.Create);
            ResourceFactory.getInstance().addType("setTarget", SetTargetTaskResource.Create);
            ResourceFactory.getInstance().addType("setTransforms", SetTransformsTaskResource.Create);
            ResourceFactory.getInstance().addType("showInfo", ShowInfoTaskResource.Create);
            ResourceFactory.getInstance().addType("showLog", ShowLogTaskResource.Create);

            ResourceFactory.getInstance().addType("multithreadScheduler", MultithreadSchedulerResource.Create);

            ResourceFactory.getInstance().addType("node", SceneNodeResource.Create);

            ResourceFactory.getInstance().addType("basicViewHandler", BasicViewHandlerResource.Create);
        }
    }
}
