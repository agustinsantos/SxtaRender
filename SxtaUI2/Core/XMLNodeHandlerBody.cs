using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    class XMLNodeHandlerBody : XMLNodeHandler
    {
        public override Element ElementStart(XMLParser parser, string name, XMLAttributes attributes)
        {
            Element element = parser.GetParseFrame().element;

            // Check for and apply any template
            string template_name;
            if (attributes.TryGetValue("template", out template_name) && !string.IsNullOrWhiteSpace(template_name))
            {
                element = XMLParser.ParseTemplate(element, template_name);
            }

            // Apply any attributes to the document
            if (parser.GetParseFrame().element != null)
            {
                ElementDocument document = parser.GetParseFrame().element.GetOwnerDocument();
                if (document != null)
                    document.SetAttributes(attributes);
            }
            // Tell the parser to use the element handler for all children
            parser.PushDefaultHandler();

            return element;
        }

        public override bool ElementEnd(XMLParser parser, string name)
        {
            return true;
        }

        public override bool ElementData(XMLParser parser, string data)
        {
            return Factory.InstanceElementText(parser.GetParseFrame().element, data);
        }
    }
}
