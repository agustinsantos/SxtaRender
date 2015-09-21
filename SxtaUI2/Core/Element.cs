using log4net;
using SxtaUI2.Core.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    /// <summary>
    /// A generic element in the DOM tree.
    /// </summary>
    public class Element : IDomElement,  ScriptInterface
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Element(string _tag)
        {
            tag = _tag.ToLowerInvariant();
            parent = null;
            focus = null;
            owner_document = null;
        }

        public void Update()
        { throw new NotImplementedException(); }

        public void Render()
        { throw new NotImplementedException(); }

        /// Clones this element, returning a new, unparented element.
        public Element Clone()
        { throw new NotImplementedException(); }


        #region Classes Name
        /// Sets or removes a class on the element.
        /// @param[in] class_name The name of the class to add or remove from the class list.
        /// @param[in] activate True if the class is to be added, false to be removed.
        public void SetClass(string class_name, bool activate)
        { throw new NotImplementedException(); }

        /// Checks if a class is set on the element.
        /// @param[in] class_name The name of the class to check for.
        /// @return True if the class is set on the element, false otherwise.
        public bool IsClassSet(string class_name)
        { throw new NotImplementedException(); }

        /// Specifies the entire list of classes for this element. This will replace any others specified.
        /// @param[in] class_names The list of class names to set on the style, separated by spaces.
        public void SetClassNames(string class_names)
        { throw new NotImplementedException(); }

        /// Return the active class list.
        /// @return The space-separated list of classes active on the element.
        public string GetClassNames()
        { throw new NotImplementedException(); }

        #endregion

        #region Properties
        /// Sets a local property override on the element.
        /// @param[in] name The name of the new property.
        /// @param[in] value The new property to set.
        /// @return True if the property parsed successfully, false otherwise.
        public bool SetProperty(string name, string value)
        { throw new NotImplementedException(); }

        /// Sets a local property override on the element to a pre-parsed value.
        /// @param[in] name The name of the new property.
        /// @param[in] property The parsed property to set.
        /// @return True if the property was set successfully, false otherwise.
        public bool SetProperty(string name, Property property)
        { throw new NotImplementedException(); }

        /// Removes a local property override on the element; its value will revert to that defined in
        /// the style sheet.
        /// @param[in] name The name of the local property definition to remove.
        public void RemoveProperty(string name)
        { throw new NotImplementedException(); }

        /// Returns one of this element's properties. If this element is not defined this property, or a parent cannot
        /// be found that we can inherit the property from, the default value will be returned.
        /// @param[in] name The name of the property to fetch the value for.
        /// @return The value of this property for this element, or NULL if no property exists with the given name.
        public Property GetProperty(string name)
        { throw new NotImplementedException(); }

        /// Returns the values of one of this element's properties.		
        /// @param[in] name The name of the property to get.
        /// @return The value of this property.
        public T GetProperty<T>(string name)
        { throw new NotImplementedException(); }

        /// Returns one of this element's properties. If this element is not defined this property, NULL will be
        /// returned.
        /// @param[in] name The name of the property to fetch the value for.
        /// @return The value of this property for this element, or NULL if this property has not been explicitly defined for this element.
        public Property GetLocalProperty(string name)
        { throw new NotImplementedException(); }

        /// Resolves one of this element's properties. If the value is a number or px, this is returned. If it's a 
        /// percentage then it is resolved based on the second argument (the base value).
        /// @param[in] name The name of the property to resolve the value for.
        /// @param[in] base_value The value that is scaled by the percentage value, if it is a percentage.
        /// @return The value of this property for this element.
        public float ResolveProperty(string name, float base_value)
        { throw new NotImplementedException(); }

        /// Resolves one of this element's non-inherited properties. If the value is a number or px, this is returned. If it's a 
        /// percentage then it is resolved based on the second argument (the base value).
        /// @param[in] name The property to resolve the value for.
        /// @param[in] base_value The value that is scaled by the percentage value, if it is a percentage.
        /// @return The value of this property for this element.
        public float ResolveProperty(Property property, float base_value)
        { throw new NotImplementedException(); }

        /// Gets the name of the element.
        /// @return The name of the element.
        public string TagName { get; private set; }

        /// Gets/Sets the id of the element.
        /// @return The element's id.
        public string Id { get; set; }

        #endregion


        public virtual ElementDocument GetOwnerDocument()
        {
            if (parent == null)
                return null;

            if (owner_document == null)
            {
                owner_document = parent.GetOwnerDocument();
            }

            return owner_document;
        }

        // Gets this element's parent node.
        public Element GetParentNode()
        {
            return parent;
        }

        /// <summary>
        /// Append a child to this element.
        /// </summary>
        /// <param name="child">The element to append as a child.</param>
        /// <param name="dom_element">
        /// True if the element is to be part of the DOM, false otherwise. 
        /// Only set this to false if you know what you're doing!
        /// </param>
        public void AppendChild(Element child, bool dom_element = true)
        { }

        // Set a group of attributes
        public void SetAttributes(XMLAttributes attributes)
        { }

        public object GetScriptObject()
        {
            throw new NotImplementedException();
        }



        public bool IsBlock
        {
            get { throw new NotImplementedException(); }
        }

        public string ElementHtml()
        {
            throw new NotImplementedException();
        }

        public int ElementIndex
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IDomObject> CloneChildren()
        {
            throw new NotImplementedException();
        }

        public IDomDocument Document
        {
            get { throw new NotImplementedException(); }
        }

        public IDomContainer ParentNode
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IDomContainer> GetAncestors()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDomObject> GetDescendents()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDomElement> GetDescendentElements()
        {
            throw new NotImplementedException();
        }

        public IDomObject this[int index]
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string attribute]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IAttributeCollection Attributes
        {
            get { throw new NotImplementedException(); }
        }

        public string ClassName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<string> Classes
        {
            get { throw new NotImplementedException(); }
        }

        public string Value
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string DefaultValue
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string InnerHTML
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string OuterHTML
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string InnerText
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string TextContent
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void AppendChild(IDomObject element)
        {
            throw new NotImplementedException();
        }

        public void RemoveChild(IDomObject element)
        {
            throw new NotImplementedException();
        }

        public void InsertBefore(IDomObject newNode, IDomObject referenceNode)
        {
            throw new NotImplementedException();
        }

        public void InsertAfter(IDomObject newNode, IDomObject referenceNode)
        {
            throw new NotImplementedException();
        }

        public IDomObject FirstChild
        {
            get { throw new NotImplementedException(); }
        }

        public IDomElement FirstElementChild
        {
            get { throw new NotImplementedException(); }
        }

        public IDomObject LastChild
        {
            get { throw new NotImplementedException(); }
        }

        public IDomElement LastElementChild
        {
            get { throw new NotImplementedException(); }
        }

        public IDomObject NextSibling
        {
            get { throw new NotImplementedException(); }
        }

        public IDomObject PreviousSibling
        {
            get { throw new NotImplementedException(); }
        }

        public IDomElement NextElementSibling
        {
            get { throw new NotImplementedException(); }
        }

        public IDomElement PreviousElementSibling
        {
            get { throw new NotImplementedException(); }
        }

        public void SetAttribute(string name)
        {
            throw new NotImplementedException();
        }

        public void SetAttribute(string name, string value)
        {
            throw new NotImplementedException();
        }

        public string GetAttribute(string name)
        {
            throw new NotImplementedException();
        }

        public string GetAttribute(string name, string defaultValue)
        {
            throw new NotImplementedException();
        }

        public bool TryGetAttribute(string name, out string value)
        {
            throw new NotImplementedException();
        }

        public bool HasAttribute(string name)
        {
            throw new NotImplementedException();
        }

        public bool RemoveAttribute(string name)
        {
            throw new NotImplementedException();
        }

        public bool HasClass(string className)
        {
            throw new NotImplementedException();
        }

        public bool AddClass(string className)
        {
            throw new NotImplementedException();
        }

        public bool RemoveClass(string className)
        {
            throw new NotImplementedException();
        }

        public bool HasStyle(string styleName)
        {
            throw new NotImplementedException();
        }

        public void AddStyle(string styleString)
        {
            throw new NotImplementedException();
        }

        public void AddStyle(string style, bool strict)
        {
            throw new NotImplementedException();
        }

        public bool RemoveStyle(string name)
        {
            throw new NotImplementedException();
        }

        public bool HasAttributes
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasClasses
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasStyles
        {
            get { throw new NotImplementedException(); }
        }

        public bool Selected
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool Checked
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool Disabled
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool ReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Type
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool InnerHtmlAllowed
        {
            get { throw new NotImplementedException(); }
        }

        public bool ChildrenAllowed
        {
            get { throw new NotImplementedException(); }
        }

        public int DescendantCount()
        {
            throw new NotImplementedException();
        }

        public int Depth
        {
            get { throw new NotImplementedException(); }
        }

        public ushort NodePathID
        {
            get { throw new NotImplementedException(); }
        }

        public ushort[] NodePath
        {
            get { throw new NotImplementedException(); }
        }

        IDomObject IDomObject.Clone()
        {
            throw new NotImplementedException();
        }

        public ushort NodeNameID
        {
            get { throw new NotImplementedException(); }
        }

        public NodeType NodeType
        {
            get { throw new NotImplementedException(); }
        }

        public string NodeName
        {
            get { throw new NotImplementedException(); }
        }

        public string NodeValue
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool HasChildren
        {
            get { throw new NotImplementedException(); }
        }

        public int Index
        {
            get { throw new NotImplementedException(); }
        }

        public INodeList ChildNodes
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IDomElement> ChildElements
        {
            get { throw new NotImplementedException(); }
        }

        public void Remove()
        {
            throw new NotImplementedException();
        }

        public bool IsIndexed
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsDisconnected
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsFragment
        {
            get { throw new NotImplementedException(); }
        }

        IDomNode IDomNode.Clone()
        {
            throw new NotImplementedException();
        }

        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IDomObject other)
        {
            throw new NotImplementedException();
        }


        // Original tag this element came from.
        private string tag;

        // The optional, unique ID of this object.
        private string id;

        // Parent element.
        private Element parent;

        // Currently focused child object
        private Element focus;

        // The owning document
        private ElementDocument owner_document;
        // Gets the document this element belongs to.
    }
}
