using log4net;
using proland;
using Sxta.Render.Resources;
using Sxta.Render.Scenegraph;
using System;
using System.Reflection;
using System.Xml;

namespace Sxta.Proland.Core.Producer.XmlResources
{

    class TileCacheResource : ResourceTemplate<TileCache>
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static TileCacheResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new TileCacheResource(manager, name, desc, e);
        }
        public TileCacheResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) :
        base(50, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            TileStorage storage = new TileStorage();
            Scheduler scheduler;
            checkParameters(desc, e, "name,storage,scheduler,");
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("storage")))
            {
                string _id = getParameter(desc, e, "storage");
                storage = (TileStorage)manager.loadResource(_id).get();
            }
            else
            {
                XmlNode n = e.FirstChild;
                if (n == null)
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error("Missing storage attribute or subelement");
                    }
                    throw new Exception("Missing storage attribute or subelement");
                }
                XmlElement f = n as XmlElement;
                //TODO
                //storage = ResourceFactory::getInstance()->create(manager, f.Value(), desc, f).cast<TileStorage>();
            }
            string id = getParameter(desc, e, "scheduler");
            scheduler = (Scheduler)manager.loadResource(id).get();
            this.valueC = new TileCache(storage, name, scheduler);
        }
    }
}
