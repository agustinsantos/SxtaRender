using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    class XMLNodeHandlerHead : XMLNodeHandler
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public override Element ElementStart(XMLParser parser, string name, XMLAttributes attributes)
        {
            if (name == "head")
            {
                // Process the head attribute
                parser.GetDocumentHeader().Source = parser.GetSourceURL();
            }

            // Is it a link tag?
            else if (name == "link")
            {
                // Lookup the type and href
                string type = attributes["type"].ToLowerInvariant();
                string href = attributes["href"];

                if (!string.IsNullOrWhiteSpace(type) && !string.IsNullOrWhiteSpace(href))
                {
                    // If its RCSS (... or CSS!), add to the RCSS fields.
                    if (type == "text/rcss" ||
                         type == "text/css")
                    {
                        parser.GetDocumentHeader().RcssExternal.Add(href);
                    }

                    // If its an template, add to the template fields
                    else if (type == "text/template")
                    {
                        parser.GetDocumentHeader().TemplateResources.Add(href);
                    }

                    else
                    {
                        log.ErrorFormat("Invalid link type '{0}' at source {1}, line {2}", type,parser.GetSourceURL(),  parser.GetLineNumber());
                    }
                }
                else
                {
                    log.ErrorFormat("Link tag requires type and href attributes at source {0}, line {1}", parser.GetSourceURL(), parser.GetLineNumber());
                }
            }

            // Process script tags
            else if (name == "script")
            {
                // Check if its an external string
                String src = attributes["src"];
                if (!string.IsNullOrWhiteSpace(src))
                {
                    parser.GetDocumentHeader().ScriptsExternal.Add(src);
                }
            }

            // No elements constructed
            return null;
        }

        public override bool ElementEnd(XMLParser parser, string name)
        {
            // When the head tag closes, inject the header into the active document
            if (name == "head")
            {
                 Element element = parser.GetParseFrame().element;
                if (element == null)
                    return true;

                ElementDocument document = element.GetOwnerDocument();
                if (document != null)
                    document.ProcessHeader(parser.GetDocumentHeader());
            }
            return true;
        }

        public override bool ElementData(XMLParser parser, string data)
        {
#if TODO
            String tag = parser.GetParseFrame().tag;

            // Store the title
            if (tag == "title")
            {
                SystemInterface  system_interface = GetSystemInterface();
                if (system_interface != null)
                    system_interface.TranslateString(parser.GetDocumentHeader().title, data);
            }

            // Store an inline script
            if (tag == "script" && data.Length() > 0)
                parser.GetDocumentHeader().scripts_inline.push_back(data);

            // Store an inline style
            if (tag == "style" && data.Length() > 0)
                parser.GetDocumentHeader().rcss_inline.push_back(data);
#endif
            return true;
        }
    }
}
