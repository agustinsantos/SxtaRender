using System;

namespace SxtaUI2.Core.Dom
{
    /// <summary>
    /// Additional information for node events.
    /// </summary>
    public class NodeEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        ///
        /// <param name="node">
        /// The node.
        /// </param>
        public NodeEventArgs(IDomObject node)
        {
            Node = node;
        }

        /// <summary>
        /// The node that was added or removed.
        /// </summary>
        public IDomObject Node { get; protected set; }
    }
}
