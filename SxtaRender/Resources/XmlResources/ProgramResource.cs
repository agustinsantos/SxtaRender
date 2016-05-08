using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sxta.Render.Resources.XmlResources
{
    public class ProgramResource : ResourceTemplate<Program>
    {
        public static ProgramResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new ProgramResource(manager, name, desc, e);
        }

        public ProgramResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(30, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            List<Module> modules = new List<Module>();
            checkParameters(desc, e, "name,");
            this.valueC = new Program();
            foreach (XmlNode n in e.ChildNodes)
            {
                XmlElement f = n as XmlElement;
                if (f != null)
                {
                    if (f.Name != "module")
                    {
                        logger.Error("Invalid subelement '" + f.Name + "'");
                        throw new Exception("Invalid subelement '" + f.Name + "'");
                    }
                    checkParameters(desc, f, "name,");
                    string moduleName = f.GetAttribute("name");
                    if (moduleName == null)
                    {
                        logger.Error("Missing 'name' attribute");
                        throw new Exception("Missing 'name' attribute");
                    }
                    Module module = null;
                    try
                    {
                        module = (Module)manager.loadResource(moduleName).get();
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Error loading '" + moduleName + "' module, exception: " + ex);
                        throw new Exception("Error loading '" + moduleName + "' module, exception: " + ex);
                    }
                    if (module == null)
                    {
                        logger.Error("Cannot find '" + moduleName + "' module");
                        throw new Exception("Cannot find '" + moduleName + "' module");
                    }
                    modules.Add(module);
                }
            }
            this.valueC.init(modules);

        }

        public override bool prepareUpdate()
        {
#if TODO
           bool changed = false;

            if (base.prepareUpdate())
            {
                changed = true;
            }
            else if (manager != null)
            {
                for (uint i = 0; i < modules.Count; ++i)
                {
                    if ((Resource)(modules[i]).changed())
                    {
                        changed = true;
                        break;
                    }
                }
            }

            if (changed)
            {
                oldValue = null;
                try
                {
                    oldValue = new ProgramResource(manager, name, newDesc == null ? desc : newDesc);
                }
                catch (Exception e)
                {
                }
                if (oldValue != null)
                {
                    swap(oldValue);
                    return true;
                }
                return false;
            }
            return true;  
#endif
            throw new NotImplementedException();
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (this.valueC != null && this.valueC is IDisposable)

                ((IDisposable)this.valueC).Dispose();
        }

        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
