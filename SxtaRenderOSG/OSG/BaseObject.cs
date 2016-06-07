/* OpenSceneGraph - Copyright (C) 1998-2006 Robert Osfield
 *
 * This library is open source and may be redistributed and/or modified under
 * the terms of the OpenSceneGraph Public License (OSGPL) version 0.0 or
 * (at your option) any later version.  The full license is in LICENSE file
 * included with this distribution, and on the openscenegraph.org website.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * OpenSceneGraph Public License for more details.
 *
 * Ported to C#  by Agustin Santos
*/

namespace Sxta.OSG
{
    /// <summary>
    /// Base class/standard interface for objects which require IO support,
    /// cloning and reference counting.
    /// Based on GOF Composite, Prototype and Template Method patterns.
    /// </summary>
    public abstract class BaseObject
    {
        #region Construction

        /// <summary>
        /// Construct an object. Note Object is a pure virtual base class
        /// and therefore cannot be constructed on its own, only derived
        /// classes which override the clone and className methods are
        /// concrete classes and can be constructed.
        /// </summary>
        public BaseObject() { }

        /// <summary>
        /// Copy constructor, optional CopyOp object can be used to control
        /// shallow vs deep copying of dynamic data.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="copyop"></param>
        /// <returns></returns>
        public BaseObject(BaseObject obj, CopyOp copyop = CopyOp.SHALLOW_COPY)
        {
            this.name = obj.name;
            this.dataVariance = obj.dataVariance;
            this.userDataContainer = null;

            if (obj.userDataContainer != null)
            {
                if (copyop.HasFlag(CopyOp.DEEP_COPY_USERDATA))
                {
                    UserDataContainer = CloneUtil<UserDataContainer>.Clone(obj.userDataContainer, copyop);
                }
                else
                {
                    UserDataContainer = obj.userDataContainer;
                }
            }
        }

        /// <summary>
        /// Clone the type of an object, with BaseObject return type.
        ///  Must be defined by derived classes.
        /// </summary>
        /// <returns></returns>
        public abstract BaseObject CloneType();

        /// <summary>
        /// Clone an object, with BaseObject return type.
        /// Must be defined by derived classes.
        /// </summary>
        /// <param name="copyop"></param>
        /// <returns></returns>
        public abstract BaseObject Clone(CopyOp copyop);

        #endregion

        /// <summary>
        /// Return the name of the object's library.
        /// The OpenSceneGraph convention is that the
        /// namespace of a library is the same as the library name.
        /// </summary>
        public string LibraryName { get { return this.GetType().Namespace; } }

        /// <summary>
        /// Return the name of the object's class type.
        /// </summary>
        public string ClassName { get { return this.GetType().Name; } }

        /// <summary>
        /// Return the compound class name that combines the library name and class name (FullName).
        /// </summary>
        public string CompoundClassName { get { return this.GetType().FullName; } }


        /// <summary>
        /// Get/Set the name of object.
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                if (value != null)
                    name = value;
                else
                    name = "";
            }
        }

        /// <summary>
        /// Get/Set the data variance of this object.
        /// </summary>
        public DataVariance DataVariance
        {
            get { return dataVariance; }
            set { dataVariance = value; }
        }

        public virtual void ComputeDataVariance() { }

        /// <summary>
        /// Get/Set the UserDataContainer attached to this object.
        /// </summary>
        public UserDataContainer UserDataContainer
        {
            get { return userDataContainer; }
            set { userDataContainer = value; }
        }

        /// <summary>
        /// Convenience method that returns the UserDataContainer, and if one doesn't already exist creates and assigns
        /// a DefaultUserDataContainer to the Object and then return this new UserDataContainer.
        /// </summary>
        /// <returns></returns>
        public UserDataContainer GetOrCreateUserDataContainer()
        {
            if (userDataContainer == null)
                UserDataContainer = new DefaultUserDataContainer();
            return userDataContainer;
        }

        /// <summary>
        /// Get/Set user data.
        /// </summary>
        public virtual object UserData
        {
            get
            {
                return userDataContainer != null ? userDataContainer.UserData : null;
            }
            set
            {
                GetOrCreateUserDataContainer().UserData = value;
            }
        }

#if TODO
        /** Convenience method that casts the named UserObject to osg::TemplateValueObject<T> and gets the value.
          * To use this template method you need to include the osg/ValueObject header.*/
        template<typename T>
        bool getUserValue(const std::string& name, T& value) const;

        /** Convenience method that creates the osg::TemplateValueObject<T> to store the
          * specified value and adds it as a named UserObject.
          * To use this template method you need to include the osg/ValueObject header. */
        template<typename T>
        void setUserValue(const std::string& name, const T& value);


        /** Resize any per context GLObject buffers to specified size. */
        virtual void resizeGLObjectBuffers(unsigned int /*maxSize*/) {}

        /** If State is non-zero, this function releases any associated OpenGL objects for
           * the specified graphics context. Otherwise, releases OpenGL objects
           * for all graphics contexts. */
        virtual void releaseGLObjects(osg::State* = 0) const {}
#endif

        protected string name;
        protected DataVariance dataVariance = DataVariance.UNSPECIFIED;

        protected UserDataContainer userDataContainer = null;
    }
}