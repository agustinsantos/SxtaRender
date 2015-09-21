using System;
using System.Collections.Generic;

namespace SxtaUI2.Core.Dom
{
    /// <summary>
    /// Interface for a node. This is the most generic construct in the CsQuery DOM.
    /// </summary>

    public interface IDomNode : ICloneable
    {
        /// <summary>
        /// Gets the type of the node.
        /// </summary>

        NodeType NodeType { get; }

        /// <summary>
        /// The node (tag) name, in lower case.
        /// </summary>
        string NodeName { get; }

        /// <summary>
        /// Gets or sets the value of this node.
        /// </summary>
        ///
        /// <remarks>
        /// For the document itself, nodeValue returns null. For text, comment, and CDATA nodes,
        /// nodeValue returns the content of the node.
        /// </remarks>
        string NodeValue { get; set; }

        /// <summary>
        /// Gets a value indicating whether this object has any children. For node types that cannot have
        /// children, it will always return false. To determine if a node is allowed to have children,
        /// use the ChildrenAllowed property.
        bool HasChildren { get; }

        /// <summary>
        /// Gets zero-based index of this object relative to its siblings including all node types.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Return an INodeList of the direct children of this node.
        /// </summary>
        INodeList ChildNodes { get; }

        /// <summary>
        /// Return a sequence containing only the element children of this node (e.g. no text, cdata, comments)
        /// </summary>
        IEnumerable<IDomElement> ChildElements { get; }

        /// <summary>
        /// Renders the complete Dom tree for this element, including its children.
        /// </summary>
        void Render();

        /// <summary>
        /// Removes this object from it's parent, and consequently the Document, if any, to which it belongs.
        /// </summary>
        void Remove();

        /// <summary>
        /// Gets a value indicating whether this node should be is indexed. Generally, this is true for IDomElement
        /// nodes that are within an IDomDocument and false otherwise.
        /// </summary>
        bool IsIndexed { get; }

        /// <summary>
        /// Gets a value indicating whether this object belongs to a Document or not.
        /// </summary>
        ///
        /// <remarks>
        /// Disconnected elements are not bound to a DomDocument object. This could be because
        /// they were instantiated outside a document context, or were removed as a result of
        /// an operation such as ReplaceWith.
        /// </remarks>
        bool IsDisconnected { get; }

        /// <summary>
        /// Gets a value indicating whether this object belongs is a fragmment and is bound to an 
        /// IDomFragment object.
        /// </summary>
        bool IsFragment { get; }

        /// <summary>
        /// Makes a deep copy of this object.
        /// </summary>
        ///
        /// <returns>
        /// A copy of this object.
        /// </returns>
        new IDomNode Clone();
    }
}
