using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    /// <summary>
    /// XML parsing engine. The factory creates an instance of this class for each RML parse.
    /// </summary>
    public class XMLParser : BaseXMLParser
    {
        public XMLParser(Element root)
        {
            RegisterCDATATag("script");

            // Add the first frame.
            ParseFrame frame = new ParseFrame();
            frame.node_handler = null;
            frame.child_handler = null;
            frame.element = root;
            frame.tag = "";
            stack.Push(frame);

            active_handler = null;

            header = new DocumentHeader();
        }

        /// <summary>
        /// Registers a custom node handler to be used to a given tag.
        /// </summary>
        /// <param name="tag">The tag the custom parser will handle.</param>
        /// <param name="handler">The custom handler.</param>
        /// <returns>The registered XML node handler.</returns>
        public static XMLNodeHandler RegisterNodeHandler(string tag, XMLNodeHandler handler)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                default_node_handler = handler;
                return default_node_handler;
            }

            node_handlers[tag.ToLowerInvariant()] = handler;

            return handler;

        }

        /// Releases all registered node handlers. This is called internally.
        public static void ReleaseHandlers()
        {
            node_handlers.Clear();
        }

        /// Returns the XML document's header.
        /// @return The document header.
        public DocumentHeader GetDocumentHeader()
        {
            return header;
        }





        // The parse stack.
        public struct ParseFrame
        {
            // Tag being parsed.
            public string tag;

            // Element representing this frame.
            public Element element;

            // Handler used for this frame.
            public XMLNodeHandler node_handler;

            // The default handler used for this frame's children.
            public XMLNodeHandler child_handler;
        };

        /// <summary>
        /// Pushes an element handler onto the parse stack for parsing child elements.
        /// </summary>
        /// <param name="tag">The tag the handler was registered with.</param>
        /// <returns>if an appropriate handler was found and pushed onto the stack, false if not.</returns>
        public bool PushHandler(string tag)
        {
            string tagaux = tag.ToLowerInvariant();
            if (!node_handlers.ContainsKey(tagaux))
                return false;

            active_handler = node_handlers[tagaux];
            return true;
        }

        /// <summary>
        /// Pushes the default element handler onto the parse stack.
        /// </summary>
        public void PushDefaultHandler()
        {
            active_handler = default_node_handler;
        }

        /// <summary>
        /// Access the current parse frame.
        /// </summary>
        /// <returns>The parser's current parse frame.</returns>
        public ParseFrame GetParseFrame()
        {
            return stack.Peek();

        }


        /// Called when the parser finds the beginning of an element tag.
        protected override void HandleElementStart(string _name, XMLAttributes attributes)
        {
            String name = _name.ToLower();

            Console.WriteLine("Element Begin: " + name);
            if (attributes.Count > 0)
            {
                foreach (var attr in attributes)
                    Console.WriteLine("\t {0} = {1}", attr.Key, attr.Value);

            }

            // Check for a specific handler that will override the child handler.
            if (node_handlers.ContainsKey(name))
                active_handler = node_handlers[name];

            // Store the current active handler, so we can use it through this function (as active handler may change)
            XMLNodeHandler node_handler = active_handler;

            Element element = null;

            // Get the handler to handle the open tag
            if (node_handler != null)
            {
                element = node_handler.ElementStart(this, name, attributes);
            }

            // Push onto the stack
            ParseFrame frame;
            frame.node_handler = node_handler;
            frame.child_handler = active_handler;
            frame.element = element != null ? element : stack.Peek().element;
            frame.tag = name;
            stack.Push(frame);
        }

        /// Called when the parser finds the end of an element tag.
        protected override void HandleElementEnd(string _name)
        {

            String name = _name.ToLowerInvariant();

            Console.WriteLine("Element End: " + name);

            // Copy the top of the stack and pop the frame
            ParseFrame frame = stack.Pop();
            // Restore active handler to the previous frame's child handler
            active_handler = stack.Peek().child_handler;

            // Check frame names
            if (name != frame.tag)
            {
                //Log::Message(Log::LT_ERROR, "Closing tag '%s' mismatched on %s:%d was expecting '%s'.", name.CString(), GetSourceURL().GetURL().CString(), GetLineNumber(), frame.tag.CString());
            }

            // Call element end handler
            if (frame.node_handler != null)
            {
                frame.node_handler.ElementEnd(this, name);
            }
        }

        /// Called when the parser encounters data.
        protected override void HandleData(string data)
        {
            Console.WriteLine("Data: " + data);

            if (stack.Peek().node_handler != null)
                stack.Peek().node_handler.ElementData(this, data);
        }

        static internal Element ParseTemplate(Element element, string template_name)
        {
            // Load the template, and parse it
            Template parse_template = TemplateCache.GetTemplate(template_name);
            if (parse_template == null)
            {
                //Log::ParseError(element->GetOwnerDocument()->GetSourceURL(), -1, "Failed to find template '%s'.", template_name.CString());
                return element;
            }

            return parse_template.ParseTemplate(element);
            return null;
        }

        // The header of the document being parsed.
        private DocumentHeader header;

        // The active node handler.
        private XMLNodeHandler active_handler;

        // The parser stack.
        //typedef std::stack< ParseFrame > ParserStack;
        private Stack<ParseFrame> stack = new Stack<ParseFrame>();

        //typedef std::map< String, XMLNodeHandler* > NodeHandlers;
        private static Dictionary<string, XMLNodeHandler> node_handlers = new Dictionary<string, XMLNodeHandler>();
        private static XMLNodeHandler default_node_handler = null;
    }
}
