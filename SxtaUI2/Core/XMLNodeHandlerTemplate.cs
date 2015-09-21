using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    class XMLNodeHandlerTemplate : XMLNodeHandler
    {
        public override Element ElementStart(XMLParser parser, string name, XMLAttributes attributes)
        {
            String template_name;
            if (attributes.TryGetValue("src", out template_name))
            {
                // Tell the parser to use the element handler for all child nodes
                parser.PushDefaultHandler();

                return XMLParser.ParseTemplate(parser.GetParseFrame().element, template_name);
            }
            return null;
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
