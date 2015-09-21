using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    /// <summary>
    /// A context for storing, rendering and processing RML documents. Multiple contexts can exist simultaneously.
    /// </summary>
    public class Context : ScriptInterface
    {
        /// <summary>
        /// Load a document into the context.
        /// </summary>
        /// <param name="document_path"> The path to the document to load.</param>
        /// <returns>The loaded document, or NULL if no document was loaded. The document is returned with a reference owned by the caller.</returns>
        public ElementDocument LoadDocument(string document_path)
        {
            // Open the stream based on the file path
            StreamReader stream = new StreamReader(document_path);

            // Load the document from the stream
            ElementDocument document = LoadDocument(stream);
            return document;
        }

        /// <summary>
        /// Load a document into the context.
        /// </summary>
        /// <param name="document_stream">The opened stream, ready to read.</param>
        /// <returns>The loaded document, or NULL if no document was loaded. The document is returned with a reference owned by the caller.</returns>
        public ElementDocument LoadDocument(StreamReader document_stream)
        {
            string streamName = "NoName";
            if (document_stream.BaseStream is FileStream)
            {
                streamName = (document_stream.BaseStream as FileStream).Name;
            }

            PluginRegistry.NotifyDocumentOpen(this, streamName);

            // Load the document from the stream.
            ElementDocument document = Factory.InstanceDocumentStream(this, document_stream);
            if (document == null)
                return null;
#if TODO
            root.AppendChild(document);

            // Bind the events, run the layout and fire the 'onload' event.
            ElementUtilities.BindEventAttributes(document);
            document.UpdateLayout();

            // Dispatch the load notifications.
            PluginRegistry.NotifyDocumentLoad(document);
            document.DispatchEvent(LOAD, Dictionary(), false);
#endif
            return document;
        }

        /// <summary>
        /// Load a document into the context.
        /// </summary>
        /// <param name="str">The string containing the document RML.</param>
        /// <returns>The loaded document, or NULL if no document was loaded. The document is returned with a reference owned by the caller.</returns>
        public ElementDocument LoadDocumentFromMemory(string str)
        {
            throw new NotImplementedException();
        }

        public object GetScriptObject()
        {
            throw new NotImplementedException();
        }


        // Root of the element tree.
        private Element root;

    }
}
