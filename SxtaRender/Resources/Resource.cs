using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Sxta.Render.Resources
{
    /// <summary>
    /// An abstract %resource (texture, shader, mesh, etc).
    /// </summary>
    public abstract class Resource : IDisposable
    {
        /*
         * Creates a new Resource.
         *
         * @param manager the manager that will keep track of this %resource.
         * @param name the name of this %resource.
         * @param desc the descriptor of this %resource.
         */
        public Resource(ResourceManager manager, string name, ResourceDescriptor desc)
        {
            this.manager = manager;
            this.name = name;
            this.desc = desc;
        }

        /*
         * Deletes this %resource.
         */
        ~Resource()
        {
            // Do not re-create Dispose clean-up code here. 
            // Calling Dispose(false) is optimal in terms of 
            // readability and maintainability.
            Dispose(false);
        }

        public abstract object get();

        /*
         * Returns the name of this %resource.
         */
        public virtual string getName()
        {
            return name;
        }

        /*
         * Returns the update order of this %resource. In order to be updated
         * correctly a %resource must be updated after the %resource it depends on are
         * updated. For instance a program must be updated after the shaders it is
         * made of, which must themselves be updated after the textures they may
         * depend on, and so on. This order is computed by sorting the resources
         * according to their "update order" (0 means "update first").
         */
        public abstract int getUpdateOrder();

        /*
         * Prepares the update of this %resource. In order to update all resources
         * atomically (see ResourceManager#updateResources) a two phase commit is
         * used. In the first phase all resources test if they can be updated
         * successfully or not. If all resources can do so then they are all updated,
         * otherwise none is updated. This method tests is a %resource can be updated
         * successfully. Returning true means that it is guaranteed that the actual
         * update will not fail. NOTE: the default implementation of this method
         * does not follow this contract. Indeed it returns true if the descriptor
         * of this %resource has changed. This method MUST therefore be overriden in
         * subclasses of this class.
         *
         * @return true if it is guaranteed that a call #doUpdate with a true
         *      parameter will not fail.
         */
        public virtual bool prepareUpdate()
        {
            if (manager == null)
            {
                return false;
            }
            ResourceLoader loader = manager.getLoader();
            newDesc = loader.reloadResource(name, desc);
            return newDesc != null;
        }

        /*
         * Do an actual update of this %resource, or reverts the work of
         * #prepareUpdate.
         *
         * @param commit true to do the actual update, or false to revert the work
         *      #prepareUpdate.
         */
        public abstract void doUpdate(bool commit);

        /*
         * Returns true if the descriptor of this %resource has changed.
         */
        public virtual bool changed()
        {
            return newDesc != null;
        }

        /*
         * Utility method to check the attributes of an XML element.
         *
         * @param desc a %resource descriptor.
         * @param e an element of the XML part of the %resource descriptor desc.
         * @param params the authorized XML attributes for e, separated by commas.
         * @throw exception if e has an attribute which is not in the list specified
         *      by params.
         */
        public static void checkParameters(ResourceDescriptor desc, XmlElement e, string params_)
        {
            foreach (XmlAttribute a in e.Attributes)
            {
                if (!params_.Contains(a.Name))
                {
                    logger.Error("Unsupported '" + a.Name + "' attribute");
                    throw new Exception("Unsupported '" + a.Name + "' attribute");
                }
            }
        }

        /*
         * Utility method to Get the int valueC of an XML element attribute.
         *
         * @param desc a %resource descriptor.
         * @param e an element of the XML part of the %resource descriptor desc.
         * @param name the attribute whose valueC must be returned.
         * @param[out] i the valueC of the requested attribute.
         * @throw exception if the attribute is missing or has a wrong format.
         */
        public static void getIntParameter(ResourceDescriptor desc, XmlElement e, string name, out int i)
        {
            string attr = e.GetAttribute(name);
            if (string.IsNullOrWhiteSpace(attr))
            {
                logger.Error("Bad '" + name + "' attribute");
                throw new ArgumentException("Bad '" + name + "' attribute");
            }
            else
                i = int.Parse(attr, System.Globalization.CultureInfo.InvariantCulture);
        }
        public static void get2IntParameter(string val, out int i0, out int i1)
        {
            if (string.IsNullOrWhiteSpace(val))
            {
                logger.Error("Bad attribute value");
                throw new ArgumentException("Bad attribute value");
            }
            else
            {
                string[] vals = val.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                i0 = int.Parse(vals[0], System.Globalization.CultureInfo.InvariantCulture);
                i1 = int.Parse(vals[1], System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        /*
         * Utility method to Get the float valueC of an XML element attribute.
         *
         * @param desc a %resource descriptor.
         * @param e an element of the XML part of the %resource descriptor desc.
         * @param name the attribute whose valueC must be returned.
         * @param[out] valueC the valueC of the requested attribute.
         * @throw exception if the attribute has a wrong format.
         * @return false is the attribute is missing.
         */
        public static bool getFloatParameter(ResourceDescriptor desc, XmlElement e, string name, out float value)
        {
            value = float.NaN;
            string attr = e.GetAttribute(name);
            if (string.IsNullOrWhiteSpace(attr))
            {
                logger.Error("Bad '" + name + "' attribute");
                return false;
            }
            else
                value = float.Parse(attr, System.Globalization.CultureInfo.InvariantCulture);
            return true;
        }

        /*
         * Utility method to Get the valueC of an XML element attribute.
         *
         * @param desc a %resource descriptor.
         * @param e an element of the XML part of the %resource descriptor desc.
         * @param name the attribute whose valueC must be returned.
         * @throw exception if the attribute is missing.
         * @return the attribute's valueC.
         */
        public static string getParameter(ResourceDescriptor desc, XmlElement e, string name)
        {
            string attr = e.GetAttribute(name);
            if (string.IsNullOrWhiteSpace(attr))
            {
                logger.Error("Bad '" + name + "' attribute");
                throw new ArgumentException("Bad '" + name + "' attribute");
            }
            return attr;
        }

        /*
         * Logs a message related to a %resource.
         *
         * @param logger the logger to be used to log the message.
         * @param desc the descriptor of the %resource.
         * @param e an optional element of the XML part of the %resource descriptor.
         * @param msg the message to be logged.
         */
        public static void log(ILog logger, ResourceDescriptor desc, XmlElement e, string msg)
        {
            log(logger, desc.descriptor, e, msg);
        }

        /*
         * Logs a message related to a %resource.
         *
         * @param logger the logger to be used to log the message.
         * @param desc the XML part of the descriptor of the %resource.
         * @param e an optional element of the XML part of the %resource descriptor.
         * @param msg the message to be logged.
         */
        public static void log(ILog logger, XmlElement desc, XmlElement e, string msg)
        {
#if TODO
            TiXmlPrinter p;
            desc.Accept(&p);
            if (e == null)
            {
                logger.log("RESOURCE", msg + " in \033" + p.CStr() + "\033");
            }
            else
            {
                int line = 1;
                if (!TiXmlGetLocation(desc, e, line))
                {
                    line = 0;
                }
                msg += " at line " + line + " in \033" + p + "\033";
                logger.Info("RESOURCE", os.str());
            }
#endif
            throw new NotImplementedException();
        }


        /*
         * The manager that keeps track of this %resource. May be null for an unused
         * %resource (see ResourceManager#releaseResource).
         */
        internal ResourceManager manager;

        /*
         * The name of this %resource.
         */
        protected readonly string name;

        /*
         * The descriptor of this %resource.
         */
        protected ResourceDescriptor desc;

        /*
         * The new valueC of the descriptor of this %resource. This field is set in
         * the #prepareUpdate method. If it is not null it means that the descriptor
         * has changed, and the valueC is stored in this field. This field is set
         * back to null in #doUpdate.
         */
        protected ResourceDescriptor newDesc;

        #region IDisposable
        // Track whether Dispose has been called. 
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly 
        // or indirectly by a user's code. Managed and unmanaged resources 
        // can be disposed. 
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.

                if (manager != null)
                {
                    manager.removeResource(this);
                }

                // Note disposing has been done.
                disposed = true;

            }
        }
        #endregion
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
