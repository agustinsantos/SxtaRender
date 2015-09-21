using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SxtaUI2.Core
{
    public class XMLAttributes : Dictionary<string, string>
    {
    }

    public abstract class BaseXMLParser
    {
        /// <summary>
        /// 
        /// </summary>
        public BaseXMLParser()
        {
            open_tag_depth = 0;
        }



        /// <summary>
        /// Registers a tag as containing general character data. This will mean the contents of the tag will be parsed
        /// similarly to a CDATA tag (ie, no other markup will be recognised until the section's closing tag is found).
        /// </summary>
        /// <param name="tag">tag The tag to register as containing generic character data.</param>
        public void RegisterCDATATag(string tag)
        {
            if (!string.IsNullOrWhiteSpace(tag))
                cdata_tags.Add(tag.ToLower());
        }

        /// <summary>
        /// Parses the given stream as an XML file, and calls the handlers when
        /// interesting phenomena are encountered.
        /// </summary>
        /// <param name="stream"></param>
        private void Parse(StreamReader stream)
        {
            throw new NotImplementedException();
        }

        public void Parse(string source)
        {
            this.sourceUri = source;
            using (XmlReader reader = XmlReader.Create(source))
            {
                Parse(reader);
            }
        }

        /// <summary>
        /// Parses the given stream as an XML file, and calls the handlers when
        /// interesting phenomena are encountered.
        /// </summary>
        /// <param name="stream"></param>
        private void Parse(XmlReader reader)
        {
            IXmlLineInfo xmlInfo = (IXmlLineInfo)reader;

            // Parse the file and display each of the nodes.
            while (reader.Read())
            {
                lineNumber = xmlInfo.LineNumber;
                open_tag_depth = 0;

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        open_tag_depth++;
                        string tagName = reader.Name;
                        XMLAttributes attributes = new XMLAttributes();
                        for (int i = 0; i < reader.AttributeCount; i++)
                        {
                            reader.MoveToAttribute(i);
                            attributes.Add(reader.Name.ToLowerInvariant(), reader.Value);
                        }
                        HandleElementStart(tagName, attributes);
                        break;
                    case XmlNodeType.Text:
                        HandleData(reader.Value);
                        break;
                    case XmlNodeType.XmlDeclaration:
                    case XmlNodeType.ProcessingInstruction:
                        Console.WriteLine(reader.Name, reader.Value);
                        break;
                    case XmlNodeType.Comment:
                        Console.WriteLine(reader.Value);
                        break;
                    case XmlNodeType.EndElement:
                        HandleElementEnd(reader.Name);
                        open_tag_depth--;
                        break;
                }

            }
            if (open_tag_depth > 0)
            {
                throw new Exception("XML parse error");
            }
        }

        /// <summary>
        /// Get the line number in the stream.
        /// </summary>
        /// <returns>The line currently being processed in the XML stream.</returns>
        public int GetLineNumber()
        {
            return lineNumber;
        }

        /// <summary>
        /// Returns the source URL of this parse.
        /// </summary>
        /// <returns>The URL of the parsing stream.</returns>
        public string GetSourceURL()
        {
            return sourceUri;
        }

        /// <summary>
        /// Called when the parser finds the beginning of an element tag.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="attributes"></param>
        protected abstract void HandleElementStart(string name, XMLAttributes attributes);

        /// <summary>
        /// Called when the parser finds the end of an element tag.
        /// </summary>
        /// <param name="name"></param>
        protected abstract void HandleElementEnd(string name);

        /// <summary>
        /// Called when the parser encounters data.
        /// </summary>
        /// <param name="data"></param>
        protected abstract void HandleData(string data);

        private int lineNumber;
        private int open_tag_depth;
        private string sourceUri;

        private HashSet<String> cdata_tags = new HashSet<string>();
    }
}
