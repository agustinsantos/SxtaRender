using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxta.Render.Resources
{
    /// <summary>
    /// An abstract %resource loader, loads ResourceDescriptor from disk or other
    /// locations.
    /// </summary>
    public abstract class ResourceLoader
    {

        /*
         * Creates a new %resource loader.
         */
        public ResourceLoader() { }

        /*
         * Deletes this %resource loader.
         */
        // public virtual ~ResourceLoader();

        /*
         * Returns the path of the resource of the given name.
         *
         * @param name the name of a resource.
         * @return the path of this resource.
         * @throw exception if the resource is not found.
         */
        public abstract string findResource(string name);

        /*
         * Loads the ResourceDescriptor of the given name.
         *
         * @param name the name of the ResourceDescriptor to be loaded.
         * @return the ResourceDescriptor of the given name, or NULL if the %resource
         *      is not found.
         */
        public abstract ResourceDescriptor loadResource(string name);

        /*
         * Reloads the ResourceDescriptor of the given name.
         *
         * @param name the name of the ResourceDescriptor to be loaded.
         * @param currentValue the current valueC of this ResourceDescriptor.
         * @return the new valueC of this ResourceDescriptor, or NULL if this valueC
         *      has not changed.
         */
        public abstract ResourceDescriptor reloadResource(string name, ResourceDescriptor currentValue);
    }
}
