using System;
using System.Collections.Generic;

namespace Sxta.OSG
{
    public abstract class NodeVisitor
    {
        private bool completed;

        #region Construction

        public NodeVisitor(TraversalMode tm = TraversalMode.TRAVERSE_NONE)
        {
            traversalMode = tm;
        }

        #endregion

        /// <inheritdoc />
        public virtual bool HasCompleted
        {
            get { return completed; }
            protected set { completed = value; }
        }

        /// <summary>
        /// Method for handling traversal of a nodes.
        /// If you intend to use the visitor for actively traversing
        /// the scene graph then make sure the accept() methods call
        /// this method unless they handle traversal directly.
        /// </summary>
        /// <param name="node"></param>
        public virtual void Traverse(Node node)
        {
            if (traversalMode == TraversalMode.TRAVERSE_PARENTS) node.Ascend(this);
            else if (traversalMode != TraversalMode.TRAVERSE_NONE) node.Traverse(this);
        }


        /// <summary>
        /// Method called by Node.Accept() method before
        /// a call to the NodeVisitor.Apply(..).  The back of the list will,
        /// therefore, be the current node being visited inside the Apply(..),
        /// and the rest of the list will be the parental sequence of nodes
        /// from the top most node applied down the graph to the current node.
        /// Note, the user does not typically call PushNodeOnPath() as it
        /// will be called automatically by the Node.Accept() method.
        /// </summary>
        /// <param name="node"></param>
        public void PushOntoNodePath(Node node)
        {
            if (traversalMode != TraversalMode.TRAVERSE_PARENTS) _nodePath.Add(node);
            else _nodePath.Insert(0, node);
        }

        /// <summary>
        /// Method called by Node.Accept() method after
        /// a call to NodeVisitor.Apply(..).
        /// Note, the user does not typically call PopFromNodePath() as it
        /// will be called automatically by the Node.Accept() method.
        /// </summary>
        public void PopFromNodePath()
        {
            if (traversalMode != TraversalMode.TRAVERSE_PARENTS) _nodePath.RemoveAt(_nodePath.Count - 1);
            else _nodePath.RemoveAt(0);
        }

        /// <summary>
        /// Get the Node Path from the top most node applied down
        ///  to the current Node being visited.
        /// </summary>
        /// <returns></returns>
        public List<Node> GetNodePath() { return _nodePath; }


        /// <summary>
        /// Method to called by Node and its subclass' Node.Accept() method, if the result is true
        /// it is used to cull operations of nodes and their subgraphs.
        /// Return true if the result of a bit wise and of the NodeVisitor::_traversalMask
        /// with the bit or between NodeVisitor.nodeMaskOverride and the Node.nodeMask.
        /// default values for traversalMask is 0xffffffff, nodeMaskOverride is 0x0,
        /// and Node.nodeMask is 0xffffffff.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual bool ValidNodeMask(Node node)
        {
            return (this.TraversalMask & (this.NodeMaskOverride | node.NodeMask)) != 0;
        }

        public virtual void Apply(Node node)
        {
            Traverse(node);
        }

        public virtual void Apply(Drawable drawable)
        {
            Apply((Node)drawable);
        }

        public virtual void Apply(Group group)
        {
            Apply((Node)group);
        }



        /// <summary>
        /// Get/Set the Traversal Mask.
        /// </summary>
        public uint TraversalMask
        {
            get { return traversalMask; }
            set { traversalMask = value; }
        }
        /// <summary>
        /// Get/Set the Override Mask.
        /// </summary>
        public uint NodeMaskOverride
        {
            get { return nodeMaskOverride; }
            set { nodeMaskOverride = value; }
        }

        protected TraversalMode traversalMode;
        protected uint traversalMask = 0xffffffff;
        protected uint nodeMaskOverride = 0x0;
        protected List<Node> _nodePath = new List<Node>();
    }
}