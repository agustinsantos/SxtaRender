using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    class XMLNodeHandlerDefault : XMLNodeHandler
    {
        public override Element ElementStart(XMLParser parser, string name, XMLAttributes attributes)
        {
            // Determine the parent
            Element parent = parser.GetParseFrame().element;

            // Attempt to instance the element with the instancer
            Element element = Factory.InstanceElement(parent, name, name, attributes);
            if (element == null)
            {
                //Log::Message(Log::LT_ERROR, "Failed to create element for tag %s, instancer returned NULL.", name.CString());
                return null;
            }

            // Add the element to its parent and remove the reference
            parent.AppendChild(element);

            return element;
        }

        public override bool ElementEnd(XMLParser parser, string name)
        {
            return true;
        }

        public override bool ElementData(XMLParser parser, string data)
        {
            // Determine the parent
            Element parent = parser.GetParseFrame().element;

            // Parse the text into the element
            return Factory.InstanceElementText(parent, data);
        }
    }
}
