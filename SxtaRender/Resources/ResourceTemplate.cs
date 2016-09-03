using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sxta.Render.Resources
{
    public interface ISwappable<in C>
    {
        void swap(C obj);
    }

    /// <summary>
    ///  A template Resource class to ease the implementation of concrete Resource
    /// subclasses. This template class takes care of the two phase commit for the
    /// update of resources, provided that the actual %resource class implements a
    /// swap(ptr<C>) method that can swap two instances of resources of class C.
    ///
    /// @ingroup resource
    ///
    /// @tparam o the update order of resources of class C (see #getUpdateOrder).
    /// @tparam C the concrete %resource class.
    /// </summary>
    public class ResourceTemplate<C> : Resource where C : ISwappable<C>
    {
        protected readonly int updateorder;
        protected C valueC;

        public override object get()
        {
            return valueC;
        }

        /*
         * Creates a new %resource of class C.
         *
         * @param manager the manager that will keep track of this %resource.
         * @param name the name of this %resource.
         * @param desc the descriptor of this %resource.
         */
        public ResourceTemplate(int order, ResourceManager manager, string name, ResourceDescriptor desc)
            : base(manager, name, desc)
        {
            this.updateorder = order;
        }

        /*
         * Returns the template parameter o.
         */
        public override int getUpdateOrder()
        {
            return updateorder;
        }
        public override void clearValue(bool dispose)
        {
            if (dispose && valueC != null && valueC is IDisposable)
                ((IDisposable)valueC).Dispose();
            valueC = default(C);
        }

        /*
         * If the descriptor of this method has not changed, does nothing and
         * returns true. Otherwise creates a new instance of this %resource using the
         * new descriptor #newDesc and then swaps it with this instance, saving the
         * current valueC in #oldValue. If the %resource creation fails, does nothing
         * and returns false.
         */
        public override bool prepareUpdate()
        {
            if (base.prepareUpdate())
            { // if the descriptor has changed
                oldValue = default(C);
                try
                {
                    // creates a new resource using this new descriptor
                    oldValue = (C)ResourceFactory.getInstance().create(base.manager, base.name, newDesc).get();
                }
                catch (Exception e)
                {
                    log.Error("Exception with resource " + e);
                }
                if (oldValue != null)
                { // if the creation is a success
                    this.valueC.swap(oldValue); // swaps the current value with the new one
                    return true;
                }
                return false; // if the creation fails, do nothing
            }
            return true; // nothing to do if the descriptor has not changed
        }

        /*
         * If commit is true swaps the #desc and #newDesc fields, and sets the
         * #oldValue to null. If commit is false reverts the changes made in
         * #prepareUpdate by swapping again this instance with #oldValue.
         */
        public override void doUpdate(bool commit)
        {
            if (commit)
            { // if we must commit changes
                if (newDesc != null)
                { // and if there was some changes
                    desc = newDesc; // we set the descriptor to its new value
                    if (log.IsInfoEnabled)
                    {
                        log.Info("Resource '" + base.getName() + "' updated");
                    }
                    // nothing to do for the actual resource,
                    // this has already been done in prepareUpdate with the swap
                }
            }
            else
            { // if we must abort changes
                if (oldValue != null)
                { // and if there was some changes
                    this.valueC.swap(oldValue); // we revert them with a swap to cancel the first one
                }
            }
            // in all cases we set oldValue and newDesc to null to release memory
            oldValue = default(C);
            newDesc = null;
        }

        /*
         * Returns true if this %resource has changed.
         */
        public override bool changed()
        {
            return oldValue != null;
        }


        /*
         * The old valueC of this %resource.
         */
        protected C oldValue;

        /*
         * Calls ResourceManager#releaseResource to release this %resource.
         */
        protected virtual void doRelease()
        {
            if (base.manager != null)
            {
                base.manager.releaseResource(this);
            }
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            clearValue(true);
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
