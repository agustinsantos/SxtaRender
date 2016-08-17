using proland;
using Sxta.Render.Resources;
using Sxta.Render.Scenegraph;
using System.Xml;

namespace Sxta.Proland.Core.Terrain.XmlResources
{
    public class UpdateTerrainTaskResource : ResourceTemplate<UpdateTerrainTask>
    {
        public static UpdateTerrainTaskResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new UpdateTerrainTaskResource(manager, name, desc, e);
        }
        public UpdateTerrainTaskResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(40, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "name,");
            this.valueC = new UpdateTerrainTask();
            string n = getParameter(desc, e, "name");
            this.valueC.init(new AbstractTask.QualifiedName(n));
        }
    }
    public class UpdateTileSamplersTaskResource : ResourceTemplate<UpdateTileSamplersTask>
    {
        public static UpdateTileSamplersTaskResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new UpdateTileSamplersTaskResource(manager, name, desc, e);
        }
        public UpdateTileSamplersTaskResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(40, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "name,");
            string n = getParameter(desc, e, "name");
            this.valueC = new UpdateTileSamplersTask();
            this.valueC.init(new AbstractTask.QualifiedName(n));
        }
    }

    public class DrawTerrainTaskResource : ResourceTemplate<DrawTerrainTask>
    {
        public static DrawTerrainTaskResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new DrawTerrainTaskResource(manager, name, desc, e);
        }
        public DrawTerrainTaskResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(40, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "name,mesh,culling");
            this.valueC = new DrawTerrainTask();
            string n = getParameter(desc, e, "name");
            string meshName = getParameter(desc, e, "mesh");
            bool culling = false;
            string CullingAttr = e.GetAttribute("culling");
            if (!string.IsNullOrEmpty(CullingAttr))
            {
                bool.TryParse(CullingAttr.ToLowerInvariant(), out culling);
            }
            this.valueC.init(new AbstractTask.QualifiedName(n), new AbstractTask.QualifiedName(meshName), culling);
        }
    }
}
