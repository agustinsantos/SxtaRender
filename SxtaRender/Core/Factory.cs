using System;
using System.Collections.Generic;

namespace Sxta.Core
{
    /// <summary>
    /// A factory to create and destroy shared objects. Each object is constructed
    /// from a key, and is destructed when it is no longer used. Clients of this
    /// factory must call Get to get the object corresponding to a given key, and
    /// must call Put when this object is no longer used. The object will be
    /// destroyed automatically when all clients have called Put.
    /// </summary>
    public class Factory<K, C>
    {
        /// <summary>
        /// Function pointer to the factory objects constructor.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public delegate C Constructor(K key);

        /// <summary>
        /// Creates a factory that creates objects from keys with the given constructor function.
        /// </summary>
        /// <param name="ctor">a constructor function.</param>
        public Factory(Constructor ctor)
        {
            this.ctor = ctor;
        }

        /// <summary>
        ///  Returns the object corresponding to the given key. If this object does
        ///  not exist yet it is created with the constructor function of this
        ///  factory. Otherwise a counter is incremented to know when this
        ///  object is no longer used.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public C Get(K key)
        {
            C result;
            int uses;
            Tuple<C, int> val;
            if (!values.TryGetValue(key, out val))
            {
                // if the objet does not exist yet, we create it
                result = ctor(key);
                uses = 1;
            }
            else
            {
                // otherwise we increment its counter
                result = val.Item1;
                uses = val.Item2 + 1;
            }
            values[key] = new Tuple<C, int>(result, uses);
            return result;
        }

        /// <summary>
        /// Releases the object corresponding to the given key. If the
        /// count of this object becomes 0, the object is automatically destroyed (and disposed).
        /// </summary>
        /// <param name="key"></param>
        public void Put(K key)
        {
            Tuple<C, int> val;
            if (values.TryGetValue(key, out val))
            {
                C value = val.Item1;
                int uses = val.Item2 - 1;
                if (uses == 0)
                {
                    // if the counter becomes 0 we remove the object from the
                    // map. It will be destroyed automatically
                    values.Remove(key);
                }
                else
                {
                    values[key] = new Tuple<C, int>(value, uses);
                }
            }
        }


        /// <summary>
        /// The constructor used to create the objects from keys.
        /// </summary>
        private Constructor ctor;

        /// <summary>
        /// The objects created by this factory, associated with their corresponding
        /// keys. Each object has a counter used to automatically destroy
        ///  objects when they are no longer used.
        /// </summary>
        private IDictionary<K, Tuple<C, int>> values = new Dictionary<K, Tuple<C, int>>();
    }
}
