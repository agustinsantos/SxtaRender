using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;

namespace Sxta.Render.Resources
{
    /// <summary>
    /// A  resource descriptor, contains all the data to create an actual Resource.
    /// This data is described with an XML element and with an optional ASCII or
    /// binary data section. For example, for a texture, the XML part describes the
    /// texture options (internal format, min and mag filter, min and max LOD, etc),
    /// while the binary data part contains the texture data itself. For a shader
    /// the XML part describes default values for the shader uniforms, while the
    /// binary data part contains the shader source code. And so on for other
    /// resources.
    /// </summary>
    public class ResourceDescriptor
    {
        /*
         * The XML part of this %resource descriptor. This part can describe
         * optional elements that cannot be stored in the %resource itself, such as
         * the internal format for a texture, default uniform values for a shader,
         * etc. The tag of the descriptor is the type of the %resource (e.g.
         * texture1D, texture2D, shader, program, mesh, etc).
         */
        public XmlElement descriptor;

        /*
         * Creates a new %resource descriptor.
         *
         * @param descriptor the XML part of this %resource descriptor.
         * @param data the ASCII of binary data part of the descriptor.
         * @param size the size of the ASCII or binary part in bytes.
         */
        public ResourceDescriptor(XmlElement descriptor, object data, int size)
        {
            this.data = data;
            this.size = size;
            this.descriptor = descriptor;
            Debug.Assert(this.descriptor != null);
        }

        /*
         * Deletes this %resource descriptor. This deletes both the XML and the
         * binary data part.
         */
         ~ResourceDescriptor()
        {
            descriptor = null;
            clearData();
        }

        /*
         * Returns the ASCII or binary data part of this %resource descriptor.
         */
         public object getData()
        {
            return data;
        }

        /*
         * Returns the size in bytes of the ASCII or binary data part of this
         * %resource descriptor.
         */
        public int getSize()
        {
            return size;
        }

        /*
         * Deletes the ASCII or binary data part of this %resource descriptor.
         */
        public virtual void clearData()
        {
            if (data != null)
            {
                if (data is IDisposable)
                    ((IDisposable)data).Dispose();
                data = null;
            }
        }


        /*
         * The ASCII or binary data part of this %resource descriptor.
         */
        private object data;

        /*
         * Size in bytes of the ASCII or binary data part of this %resource descriptor.
         */
        private int size;
    }
}
