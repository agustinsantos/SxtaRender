using System.Collections.Generic;

namespace SxtaUI2.Core.Dom
{
    /// <summary>
    /// An interface to a DOM Document, the high-level representation of an HTML document. This is
    /// analagous to the web browser "document" object.
    /// </summary>
    public interface IDomDocument : IDomContainer
    {

        /// <summary>
        /// Gets the style sheets for this document. (This feature is not implemented completely).
        /// </summary>
        IList<ExCSS.StyleSheet> StyleSheets { get; }

        /// <summary>
        /// Returns a reference to the element by its ID.
        /// </summary>
        ///
        /// <param name="id">
        /// The identifier.
        /// </param>
        ///
        /// <returns>
        /// The element by identifier.
        /// </returns>
        IDomElement GetElementById(string id);

        /// <summary>
        /// Gets an element by identifier, and return a strongly-typed interface
        /// </summary>
        ///
        /// <typeparam name="T">
        /// Generic type parameter.
        /// </typeparam>
        /// <param name="id">
        /// The identifier.
        /// </param>
        ///
        /// <returns>
        /// The element by id&lt; t&gt;
        /// </returns>
        T GetElementById<T>(string id) where T : IDomElement;

        /// <summary>
        /// Creates the specified HTML element.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// Name of the node.
        /// </param>
        ///
        /// <returns>
        /// The new element.
        /// </returns>
        IDomElement CreateElement(string nodeName);
 
        /// <summary>
        /// Returns the first element within the document (using depth-first pre-order traversal of the
        /// document's nodes) that matches the specified group of selectors.
        /// </summary>
        ///
        /// <param name="selector">
        /// The selector.
        /// </param>
        ///
        /// <returns>
        /// An element, the first that matches the selector.
        /// </returns>
        IDomElement QuerySelector(string selector);

        /// <summary>
        /// Returns a list of the elements within the document (using depth-first pre-order traversal of
        /// the document's nodes) that match the specified group of selectors.
        /// </summary>
        ///
        /// <param name="selector">
        /// The selector.
        /// </param>
        ///
        /// <returns>
        /// A sequence of elements matching the selector.
        /// </returns>
        IList<IDomElement> QuerySelectorAll(string selector);

        /// <summary>
        /// Returns a list of elements with the given tag name. The subtree underneath the specified
        /// element is searched, excluding the element itself.
        /// </summary>
        ///
        /// <remarks>
        /// Unlike the browser DOM version, this list is not live; it will represent the selection at the
        /// time the query was run.
        /// </remarks>
        ///
        /// <param name="tagName">
        /// Name of the tag.
        /// </param>
        ///
        /// <returns>
        /// The element by tag name.
        /// </returns>
        INodeList<IDomElement> GetElementsByTagName(string tagName);

        /// <summary>
        /// Return the body element for this Document.
        /// </summary>

        IDomElement Body { get; }

        /// <summary>
        /// Creates an IDomDocument that is derived from this one. The new type can also be a derived
        /// type, such as IDomFragment. The new object will inherit DomRenderingOptions from this one.
        /// </summary>
        ///
        /// <typeparam name="T">
        /// The type of object to create that is IDomDocument
        /// </typeparam>
        ///
        /// <returns>
        /// A new, empty concrete class that is represented by the interface T, configured with the same
        /// options as the current object.
        /// </returns>
        IDomDocument CreateNew<T>() where T : IDomDocument;

        /// <summary>
        /// Creates an IDomDocument that is derived from this one. The new type can also be a derived
        /// type, such as IDomFragment. The new object will inherit DomRenderingOptions from this one.
        /// </summary>
        ///
        /// <returns>
        /// The new Document.
        /// </returns>
        IDomDocument CreateNew();


        /// <summary>
        /// Any user data to be persisted with this DOM.
        /// </summary>
        IDictionary<string, object> Data { get; set; }
    }
}
