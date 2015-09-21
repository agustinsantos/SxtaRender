using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    /// <summary>
    /// A handler gets ElementStart, ElementEnd and ElementData called by the XMLParser.
    /// </summary>
    public abstract class XMLNodeHandler
    {

        /// <summary>
        /// Called when a new element tag is opened.
        /// </summary>
        /// <param name="parser">The parser executing the parse.</param>
        /// <param name="name">The XML tag name.</param>
        /// <param name="attributes">The tag attributes.</param>
        /// <returns>The new element, may be NULL if no element was created.</returns>
        public abstract Element ElementStart(XMLParser parser, string name, XMLAttributes attributes);

        /// <summary>
        /// Called when an element is closed.
        /// </summary>
        /// <param name="parser">The parser executing the parse.</param>
        /// <param name="name">The XML tag name.</param>
        /// <returns></returns>
        public abstract bool ElementEnd(XMLParser parser, string name);

        /// <summary>
        /// Called for element data.
        /// </summary>
        /// <param name="parser">The parser executing the parse.</param>
        /// <param name="data">The element data.</param>
        /// <returns></returns>
        public abstract bool ElementData(XMLParser parser, string data);
    }
}
