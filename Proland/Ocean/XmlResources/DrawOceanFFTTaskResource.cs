using Sxta.Render;
using Sxta.Render.Resources;
using System.Xml;

namespace Sxta.Proland.Ocean.XmlResources
{
    public class DrawOceanFFTTaskResource  : ResourceTemplate<DrawOceanFFTTask>
    {
        public static DrawOceanFFTTaskResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new DrawOceanFFTTaskResource(manager, name, desc, e);
        }
        public DrawOceanFFTTaskResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) :
        base(40, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "name,radius,zmin,brdfShader,");
            Program variances = null;
            Module brdfShader= null;
            float radius;
            float zmin;
            Program fftInit = manager.loadResource("fftInitShader;").get() as Program ;
            Program fftx = manager.loadResource("fftxShader;").get() as Program;
            Program ffty = manager.loadResource("fftyShader;").get() as Program;
            if (e.GetAttribute("brdfShader") != null)
            {
                variances = manager.loadResource("variancesShader;").get() as Program;
                brdfShader = manager.loadResource(getParameter(desc, e, "brdfShader")).get() as Module;
            }
            getFloatParameter(desc, e, "radius", out radius);
            getFloatParameter(desc, e, "zmin", out zmin);
            this.valueC = new DrawOceanFFTTask(radius, zmin, fftInit, fftx, ffty, variances, brdfShader);
        }
    }
}
