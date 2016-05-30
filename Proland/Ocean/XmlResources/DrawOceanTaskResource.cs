using Sxta.Render;
using Sxta.Render.Resources;
using System.Xml;

namespace Sxta.Proland.Ocean.XmlResources
{
    public class DrawOceanTaskResource : ResourceTemplate<DrawOceanTask>
    {
        public static DrawOceanTaskResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new DrawOceanTaskResource(manager, name, desc, e);
        }
        public DrawOceanTaskResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) :
        base(40, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "name,radius,zmin,brdfShader,");
            float radius;
            float zmin;
            Module brdfShader = null;
            if (e.GetAttribute("brdfShader") != null)
            {
                brdfShader = manager.loadResource(getParameter(desc, e, "brdfShader")).get() as Module;
            }
            getFloatParameter(desc, e, "radius", out radius);
            getFloatParameter(desc, e, "zmin", out zmin);
            this.valueC = new DrawOceanTask(radius, zmin, brdfShader);
        }
    }

}
