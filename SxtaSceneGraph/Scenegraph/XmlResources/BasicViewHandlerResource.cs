using Sxta.Render.Resources;
using Sxta.Render.Scenegraph.Controller;
using System;
using System.Xml;

namespace Sxta.Render.Scenegraph.XmlResources
{
    public class BasicViewHandlerResource : ResourceTemplate<BasicViewHandler>
    {
        public static BasicViewHandlerResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new BasicViewHandlerResource(manager, name, desc, e);
        }

        string view;

        public BasicViewHandlerResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(100, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "name,viewManager,smooth,next,");

            view = getParameter(desc, e, "viewManager");
            ViewManager viewManager = null;
#if TODO
            if (!string.IsNullOrWhiteSpace(view))
                viewManager = manager.loadResource(view).get() as ViewManager;
#endif

            bool smooth = true;
            EventHandler next = null;
            string s = e.GetAttribute("smooth");
            if (!string.IsNullOrWhiteSpace(s))
            {
                smooth = s.Equals("true", StringComparison.InvariantCultureIgnoreCase);
            }
            s = e.GetAttribute("next");
            if (!string.IsNullOrWhiteSpace(s))
            {
                next = manager.loadResource(getParameter(desc, e, "next")).get() as EventHandler;
            }
            this.valueC = new BasicViewHandler(smooth, viewManager, next);
        }
    }
}
