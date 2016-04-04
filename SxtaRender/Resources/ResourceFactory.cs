using log4net;
using Sxta.Render.Resources.XmlResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sxta.Render.Resources
{

    /**
     * A Resource factory, creates resources from ResourceDescriptor. There is only
     * one instance of this class, which registers a creation function for each
     * %resource type name.
     *
     * @ingroup resource
     */
    public class ResourceFactory
    {

        /**
         * A function that creates a Resource from a ResourceDescriptor.
         *
         * @param manager the manager that will manage the created %resource.
         * @param name the %resource name.
         * @param desc the %resource descriptor.
         * @param e an optional XML element providing contextual information (such
         *      as the XML element in which the %resource descriptor was found).
         * @return the created %resource.
         */
        public delegate Resource createFunc(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e, object context = null);

        static ResourceFactory()
        {
            getInstance().addType("texture1D", Texture1DResource.Create);
            getInstance().addType("texture1DArray", Texture1DArrayResource.Create);
            getInstance().addType("texture2D", Texture2DResource.Create);
            getInstance().addType("texture2DArray", Texture2DArrayResource.Create);
            getInstance().addType("texture3D", Texture3DResource.Create);
            getInstance().addType("textureCube", TextureCubeResource.Create);
            getInstance().addType("textureCube", TextureCubeResource.Create);
            getInstance().addType("textureCubeArray", TextureCubeArrayResource.Create);
            getInstance().addType("textureRectangle", TextureRectangleResource.Create);

            getInstance().addType("mesh", MeshResource.Create);

            getInstance().addType("program", ProgramResource.Create);
            getInstance().addType("module", ModuleResource.Create);

            getInstance().addType("font", FontResource.Create);

        }

        /**
         * Returns the unique instance of this class.
         */
        public static ResourceFactory getInstance()
        {
            if (INSTANCE == null)
            {
                INSTANCE = new ResourceFactory();
            }
            return INSTANCE;
        }


        /**
         * Registers a new %resource type with this factory.
         *
         * @param type a %resource type, as it appears in the tag of a
         *      ResourceDescriptor (e.g. texture1D, texture2D, shader, program, etc).
         * @param f a function that can create %resources of this type.
         */
        public void addType(string type, createFunc f)
        {
            types[type] = f;
        }


        /**
         * Creates a Resource from the given ResourceDescriptor.
         *
         * @param manager the manager that will manage the created %resource.
         * @param name the %resoure name.
         * @param desc the %resource descriptor.
         * @param e an optional XML element providing contextual information (such
         *      as the XML element in which the %resource descriptor was found).
         * @return the created %resource.
         */
        public virtual Resource create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            createFunc ctor;
            e = (e == null ? desc.descriptor : e);
            if (types.TryGetValue(e.Name, out ctor))
            {
                return ctor(manager, name, desc, e, context);
            }
            else
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Unknown resource type '" + e.Name + "'");
                }
                throw new ArgumentException("Unknown resource type '" + e.Name + "'");
            }
        }


        /**
         * A utility template to automate the registration of new %resource types.
         * @tparam t a %resource type (e.g. texture1D, shader, mesh, etc).
         * @tparam T the corresponding concrete Resource class. This class must be
         *      instantiated for all resources of type t.
         */
#if TODO
        public class ResType<T> where T : class
        {
            public static readonly string ResourceType;
            /**
             * Creation function for resources of class T. This function
             * just calls new T(manager, name, desc, e).
             * See ResourceFactory::createFunc
             */
            public static Object ctor(ResourceManager manager, string name, ResourceDescriptor desc, object args)
            {
                return new T(manager, name, desc, e);
            }

            /**
             * Creates a new Type instance. This constructor registers the creation
             * function #ctor (encapsulating a call to new T) with the %resource
             * type t in the ResourceFactory instance. Hence declaring a static
             * variable of type Type<t, T> automatically registers a new %resource
             * type.
             */
            public ResType()
            {
                ResourceFactory.getInstance().addType(ResourceType, ctor);
            }
        }

#endif

        /**
         * The registered creation functions. Maps %resource types (such as shader,
         * program, mesh, etc) to %resource creation functions.
         */
        private Dictionary<string, createFunc> types = new Dictionary<string, createFunc>();

        /**
         * The unique instance of this class.
         */
        private static ResourceFactory INSTANCE;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
