using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SxtaUI2.Core
{
    /// <summary>
    /// Contains a RML template. The Header is stored in parsed form, body in an unparsed stream.
    /// </summary>
    class Template
    {
        /// Load a template from the given stream
        public bool Load(string url)
        {
            using (XmlReader reader = XmlReader.Create(url))
            {
                // Find the RML tag, skip over it and read the attributes,
                // storing the ones we're interested in.
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            string tagName = reader.Name.ToLowerInvariant();
                            if (tagName == "template")
                            {
                                name = reader.GetAttribute("name");
                                content = reader.GetAttribute("content");
                            }
                            break;
                    }

                }
            }
            XMLParser parser = new XMLParser(null);
            parser.Parse(url);
            header = parser.GetDocumentHeader();
            return true;
        }

        /// Get the ID of the template
        public string GetName()
        {
            return name;
        }

        /// Parse the template into the given element
        /// @param element Element to parse into
        /// @returns The element to continue the parse from
        public Element ParseTemplate(Element element)
        {

            XMLParser parser = new XMLParser(element);
#if TODO
            parser.Parse(body);

            // If theres an inject attribute on the template, 
            // attempt to find the required element
            if (!string.IsNullOrWhiteSpace(content))
            {
		        Element  content_element = ElementUtilities.GetElementById(element, content);
		        if (content_element != null)
			        element = content_element;
            }
#endif
            return element;
        }


        /// Get the template header
        public DocumentHeader GetHeader()
        {
            return header;
        }


        private String name;
        private String content;
        private DocumentHeader header;

        private String ReadAttribute(string str)
        { throw new NotImplementedException(); }
    }
}
