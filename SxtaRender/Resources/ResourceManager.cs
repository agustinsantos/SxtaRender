using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Sxta.Render.Resources
{
    /// <summary>
    /// A resource manager, loads, unloads and updates a set of resources. A manager
    /// uses a ResourceLoader to load ResourceDescriptor, then uses a ResourceFactory
    /// to create actual Resource. A manager keeps track of the resources it has
    /// loaded: it can update (i.e. reload) them when their descriptors change, and
    /// it automatically deletes them when they are unused (i.e. unreferenced).
    /// Alternatively a manager can cache unused resources so that they can be loaded
    /// quickly if they are needed again.
    /// </summary>
    public class ResourceManager : IDisposable
    {
        /*
         * Creates a new ResourceManager.
         *
         * @param loader the object used to load the ResourceDescriptor.
         * @param cacheSize the size of the cache of unused resources.
         */
        public ResourceManager(ResourceLoader loader, uint cacheSize = 0)
        {
            this.loader = loader;
            this.cacheSize = cacheSize;
        }

        /*
         * Deletes this %resource manager. This deletes the cached unused resources,
         * if any.
         */
        ~ResourceManager()
        {
            // Do not re-create Dispose clean-up code here. 
            // Calling Dispose(false) is optimal in terms of 
            // readability and maintainability.
            Dispose(false);
        }

        /*
         * Returns the object used to load the ResourceDescriptor.
         */
        public ResourceLoader getLoader()
        {
            return loader;
        }

        /*
         * Loads the given %resource. This method first loads its descriptor with
         * #getLoader, then creates the %resource with ResourceFactory, and finally
         * returns the result. Unless the %resource has already been loaded, in which
         * case it is returned directly.
         *
         * @param name the name of the %resource to be loaded.
         * @return the %resource corresponding to the given name, or null if the %resource is not
         *      found.
         */
        public Resource loadResource(string name, object context = null)
        {
            Tuple<int, Resource> val;
            if (resources.TryGetValue(name, out val))
            { // if the requested resource has already been loaded
                Resource r = val.Item2;
                // and if it is currently unused
                List<Resource> l;
                if (unusedResources.TryGetValue(r, out l))
                {
                    // we remove it from the cache of unused resources
                    unusedResourcesOrder.Remove(r);
                    unusedResources.Remove(r);
                }
                // we restore the link from the resource to the manager, which may have
                // been set to null if the resource was unused (see #releaseResource)
                r.manager = this;
                // and we return the resource
                return r;
            }
            if (log.IsInfoEnabled)
            {
                log.Info("Loading resource '" + name + "'");
            }
            // otherwise the resource is not already loaded; we first load its descriptor
            Object ob = null;
            ResourceDescriptor d = loader.loadResource(name);

            if (d != null)
            {
                // then we create the actual resource from this descriptor
                ob = ResourceFactory.getInstance().create(this, name, d, null, context);
                if (ob != null)
                {
                    // and we register this resource with this manager
                    Resource res = (Resource)ob;
                    resources[name] = new Tuple<int, Resource>(res.getUpdateOrder(), res);
                    resourceOrder[new Tuple<int, string>(res.getUpdateOrder(), res.getName())] = res;
                    return res;
                }
            }
            if (log.IsErrorEnabled)
            {
                log.Error("Missing or invalid resource '" + name + "'");
            }
            throw new Exception("Missing or invalid resource '" + name + "'");
        }

        /*
         * Loads the given %resource. This method first loads its descriptor with
         * #getLoader, then creates the %resource with ResourceFactory, and finally
         * returns the result. Unless the %resource has already been loaded, in which
         * case it is returned directly.
         *
         * @param desc descriptor of the resource to load.
         * @param f the XML part of a ResourceDescriptor.
         * @return the %resource corresponding to the given name, or null if the %resource is not
         *      found.
         */
        public Resource loadResource(ResourceDescriptor desc, XmlElement f)
        {
            string name;
            string nm = f.Attributes["name"].Value;
            if (string.IsNullOrWhiteSpace(nm))
            {
                name = f.Value + resources.Count;
            }
            else
            {
                name = nm;
            }
            Resource r = null;

            if (desc != null)
            {
                // then we create the actual resource from this descriptor
                r = ResourceFactory.getInstance().create(this, name, desc, f);
                if (r != null)
                {
                    // and we register this resource with this manager
                    Resource res = (Resource)(r);
                    resources[name] = new Tuple<int, Resource>(res.getUpdateOrder(), res);
                    resourceOrder[new Tuple<int, string>(res.getUpdateOrder(), res.getName())] = res;
                    return r;
                }
            }
            if (log.IsErrorEnabled)
            {
                log.Error("Missing or invalid resource '" + name + "'");
            }
            throw new Exception("Missing or invalid resource '" + name + "'");
        }

        /*
         * Updates the already loaded resources if their descriptors have changed.
         * This update is atomic, i.e. either all resources are updated, or none are
         * updated.
         *
         * @return true if the resources have been updated successfully.
         */
        public bool updateResources()
        {
            if (log.IsInfoEnabled)
            {
                log.Info("Updating resources");
            }

            // in order to atomically update all resources we use a two phase commit

            // in the first phase we prepare the update of each resource, without doing
            // the actual update. If this preparation succeeds it means that the actual
            // update will succeed. Otherwise, if at least one prepare fails, then no
            // actual update will be performed. Note that resources are handled in a
            // predefined order, so that resources that depend on other resources are
            // updated after their dependent resources (for instance a program resource
            // is updated after its shader resources, itself updated after the texture
            // resources it may depend on, and so on).
            bool commit = true;
            foreach (var i in resourceOrder)
            {
                commit &= i.Value.prepareUpdate();
            }

            // in the second phase we either do all actual updates (and we now that they
            // cannot fail), or we revert all the preparation done in the first step.
            foreach (var i in resourceOrder)
            {
                i.Value.doUpdate(commit);
            }

            if (!commit && log.IsErrorEnabled)
            {
                log.Error("Resources update failed");
            }
            if (log.IsInfoEnabled)
            {
                log.Info(resources.Count + " resources used, " + unusedResources.Count + " unused.");
            }
            return commit;
        }

        /*
         * Closes this manager. This method disables the cache of unused resources.
         */
        public void close()
        {
            cacheSize = 0;
        }

        /*
         * Releases an unused %resource. If there is a cache of unused resources
         * then this %resource is Put in this cache (the oldest %resource in the
         * cache is evicted if the cache is full). Otherwise if there is no cache,
         * the %resource is deleted directly.
         *
         * @param resource an unused %resource, i.e. an unreferenced %resource.
         * 
         */
        public void releaseResource(Resource resource)
        {
            if (cacheSize > 0)
            {
                Tuple<int, Resource> res;
                if (!resources.TryGetValue(resource.getName(), out res))
                {
                    // if this resource is not managed by this manager, we delete it
                    if (resource is IDisposable)
                        ((IDisposable)resource).Dispose();
                    return;
                }
                // otherwise we put it in the cache of unused resources
                if (unusedResourcesOrder.Count == cacheSize)
                {
                    // before that, if the cache is full, we evict and delete the last
                    // recently (un)used resource
                    Resource r = unusedResources.First().Key;
                    unusedResources.Remove(r);
                    unusedResourcesOrder.Remove(r);
                    if (r is IDisposable)
                        ((IDisposable)r).Dispose();
                }
                unusedResourcesOrder.Add(resource);
                unusedResources.Add(resource, unusedResourcesOrder);
                // we remove the link from the resource to its manager so that the
                // manager gets deleted when there are no resources in use, even if
                // there are still some unused resources.
                resource.manager = null;
            }
            else
            {
                // if there is no cache of unused resources, then we delete resources as
                // soon as they become unused
                if (resource is IDisposable)
                    ((IDisposable)resource).Dispose();
            }
        }


        public void releaseResources()
        {
            var values = new List<Tuple<int, Resource>>(resources.Values);
            foreach (var val in values)
            {
                releaseResource(val.Item2);
            }
        }


        /*
         * Removes a %resource from this manager. This method is called from the
         * Resource destructor when a %resource gets deleted (for example when a
         * %resource is deleted in the #releaseResource method).
         *
         * @param resource a %resource which is currently being deleted.
         */
        protected internal void removeResource(Resource resource)
        {
            int order = 0;
            // removes this resource from the #resources map
            Tuple<int, Resource> res;
            if (resources.TryGetValue(resource.getName(), out res) && res.Item2 == resource)
            {
                order = res.Item1;
                resources.Remove(resource.getName());
            }
            // removes this resource from the #resourceOrder map
            Resource ress;
            Tuple<int, string> tuple = new Tuple<int, string>(order, resource.getName());
            if (resourceOrder.TryGetValue(tuple, out ress) && ress == resource)
            {
                resourceOrder.Remove(tuple);
            }
            // it is not necessary to remove the resource from the unused resource cache
            // indeed this should have been done already (see #releaseResource)
        }

        /*
        * The object used to load the ResourceDescriptor.
        */
        private ResourceLoader loader;

        /*
         * The resources currently managed by this manager. This map contains both
         * the resources currently in use and the unused resources. It maps %resource
         * names to %resource instances (together with their update order - see
         * Resource#getUpdateOrder).
         */
        private IDictionary<string, Tuple<int, Resource>> resources = new Dictionary<string, Tuple<int, Resource>>();

        /*
         * The resources currently managed by this manager. This map contains both
         * the resources currently in use and the unused resources. It maps %resource
         * names (together with their update order - see Resource#getUpdateOrder) to
         * %resource instances.
         */
        private IDictionary<Tuple<int, string>, Resource> resourceOrder = new Dictionary<Tuple<int, string>, Resource>();

        /*
         * The cache of unused resources. This map maps %resource instances to
         * positions in the sorted list of unused resources #unusedResourcesOrder.
         */
        private IDictionary<Resource, List<Resource>> unusedResources = new Dictionary<Resource, List<Resource>>();

        /*
         * The unused resources, sorted by date of last use. This list is used to
         * implement a LRU cache.
         */
        private List<Resource> unusedResourcesOrder = new List<Resource>();

        /*
         * The maximum number of unused resources that can be stored in cache.
         */
        private uint cacheSize;

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


                // since a Resource has a ptr to its manager, the manager cannot be
                // deleted until all the resources it manages are deleted or unused (when
                // a Resource gets released its pointer to its manager is set to null)
                // Hence, at this point, all the managed resources should be unused.
                Debug.Assert(unusedResources.Count == resources.Count);
                // we can then safely delete the unused resources
                foreach (var j in unusedResources)
                {
                    if (j.Key is IDisposable)
                        ((IDisposable)j.Key).Dispose();
                }

                // Note disposing has been done.
                disposed = true;

            }
        }
        #endregion

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
