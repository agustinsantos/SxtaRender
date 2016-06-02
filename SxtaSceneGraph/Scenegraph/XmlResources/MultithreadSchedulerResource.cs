using Sxta.Render.Resources;
using Sxta.Render.Scenegraph.Controller;
using System;
using System.Xml;

namespace Sxta.Render.Scenegraph.XmlResources
{
     public class MultithreadSchedulerResource : ResourceTemplate<MultithreadScheduler>
    {
        public static MultithreadSchedulerResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new MultithreadSchedulerResource(manager, name, desc, e);
        }

 
        public MultithreadSchedulerResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(0, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            int prefetchRate = 0;
            int prefetchQueue = 0;
            float frameRate = 0.0f;
            int nthreads = 0;
            checkParameters(desc, e, "name,prefetchRate,prefetchQueue,fps,nthreads,");
            string s = e.GetAttribute("prefetchRate");
            if (!string.IsNullOrWhiteSpace(s))
             {
                getIntParameter(desc, e, "prefetchRate", out prefetchRate);
            }
              s = e.GetAttribute("prefetchQueue");
            if (!string.IsNullOrWhiteSpace(s))
             {
                getIntParameter(desc, e, "prefetchQueue", out prefetchQueue);
            }
            s = e.GetAttribute("fps");
            if (!string.IsNullOrWhiteSpace(s))
             {
                getFloatParameter(desc, e, "fps", out frameRate);
            }
            s = e.GetAttribute("nthreads");
            if (!string.IsNullOrWhiteSpace(s))
             {
                getIntParameter(desc, e, "nthreads", out nthreads);
            }
            this.valueC = new MultithreadScheduler(prefetchRate, prefetchQueue, frameRate, nthreads);
        }
    }
}
