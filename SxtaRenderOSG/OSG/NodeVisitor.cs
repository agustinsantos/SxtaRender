using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.Render.OSG
{
    public enum TraversalMode
    {
        TRAVERSE_NONE,
        TRAVERSE_PARENTS,
        TRAVERSE_ALL_CHILDREN,
        TRAVERSE_ACTIVE_CHILDREN
    }

    public enum VisitorType
    {
        NODE_VISITOR = 0,
        UPDATE_VISITOR,
        EVENT_VISITOR,
        COLLECT_OCCLUDER_VISITOR,
        CULL_VISITOR
    }

    /// <summary>
    /// Visitor for type safe operations on osg::Nodes.
    /// Based on GOF's Visitor pattern. The NodeVisitor
    /// is useful for developing type safe operations to nodes
    /// in the scene graph (as per Visitor pattern), and adds to this
    /// support for optional scene graph traversal to allow
    /// operations to be applied to whole scenes at once. The Visitor
    /// pattern uses a technique of double dispatch as a mechanism to
    /// call the appropriate apply(..) method of the NodeVisitor.  To
    /// use this feature one must use the Node::accept(NodeVisitor) which
    /// is extended in each Node subclass, rather than the NodeVisitor
    /// apply directly.  So use root->accept(myVisitor); instead of
    /// myVisitor.apply(*root).  The later method will bypass the double
    /// dispatch and the appropriate NodeVisitor::apply(..) method will
    /// not be called.
    /// </summary>
    public class NodeVisitor
    {
        public NodeVisitor(TraversalMode tm = TraversalMode.TRAVERSE_NONE)
        {
            throw new NotImplementedException();
        }

        public NodeVisitor(VisitorType type, TraversalMode tm = TraversalMode.TRAVERSE_NONE)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method to call to reset visitor. Useful if your visitor accumulates
        /// state during a traversal, and you plan to reuse the visitor.
        /// To flush that state for the next traversal: call reset() prior
        /// to each traversal.
        /// </summary>
        public virtual void Reset() { }

        /// <summary>
        /// Get/Set the VisitorType, used to distinguish different visitors during
        /// traversal of the scene, typically used in the Node::traverse() method
        /// to select which behaviour to use for different types of traversal/visitors.
        /// </summary>
        public VisitorType VisitorType
        {
            get { return _visitorType; }
            set { _visitorType = value; }
        }

        /// <summary>
        /// Get/Set the traversal number. Typically used to denote the frame count.
        /// </summary>
        public uint TraversalNumber
        {
            get { return _traversalNumber; }
            set { _traversalNumber = value; }
        }

        /// <summary>
        /// Get/Set user data. 
        /// </summary>
        public object UserData
        {
            get { return _userData; }
            set { _userData = value; }
        }

        /// <summary>
        /// Method for handling traversal of a nodes.
        ///  If you intend to use the visitor for actively traversing
        /// the scene graph then make sure the accept() methods call
        /// this method unless they handle traversal directly.
        /// </summary>
        /// <param name="node"></param>
        public void Traverse(Node node)
        {
            if (_traversalMode == TraversalMode.TRAVERSE_PARENTS) node.Ascend(this);
            else if (_traversalMode != TraversalMode.TRAVERSE_NONE) node.Traverse(this);
        }
        /** Method called by osg::Node::accept() method before
  * a call to the NodeVisitor::apply(..).  The back of the list will,
  * therefore, be the current node being visited inside the apply(..),
  * and the rest of the list will be the parental sequence of nodes
  * from the top most node applied down the graph to the current node.
  * Note, the user does not typically call pushNodeOnPath() as it
  * will be called automatically by the Node::accept() method.*/
        public void PushOntoNodePath(Node node)
        {
            if (_traversalMode != TraversalMode.TRAVERSE_PARENTS)
                _nodePath.Add(node);
            else
                _nodePath.Insert(0, node);
        }

        /** Method called by osg::Node::accept() method after
          * a call to NodeVisitor::apply(..).
          * Note, the user does not typically call popFromNodePath() as it
          * will be called automatically by the Node::accept() method.*/
        public void PopFromNodePath()
        {
            if (_traversalMode != TraversalMode.TRAVERSE_PARENTS)
                _nodePath.RemoveAt(_nodePath.Count - 1);
            else _nodePath.RemoveAt(0);
        }

        /** Get the non const NodePath from the top most node applied down
          * to the current Node being visited.*/
        public List<Node> NodePath { get { return _nodePath; } }


        public virtual void Apply(Node node)
        {
            Traverse(node);
        }


        /// <summary>
        /// Set the TraversalMask of this NodeVisitor.
        /// The TraversalMask is used by the NodeVisitor::validNodeMask() method
        /// to determine whether to operate on a node and its subgraph.
        /// validNodeMask() is called automatically in the Node::accept() method before
        /// any call to NodeVisitor::apply(), apply() is only ever called if validNodeMask
        /// returns true. Note, if NodeVisitor::_traversalMask is 0 then all operations
        /// will be switched off for all nodes.  Whereas setting both _traversalMask and
        /// _nodeMaskOverride to 0xffffffff will allow a visitor to work on all nodes
        /// regardless of their own Node::_nodeMask state.
        /// </summary>
        public uint TraversalMask
        {
            get { return _traversalMask; }
            set { _traversalMask = value; }
        }

        /// <summary>
        /// Set the NodeMaskOverride mask.
        ///  Used in validNodeMask() to determine whether to operate on a node or its
        ///  subgraph, by OR'ing NodeVisitor::_nodeMaskOverride with the Node's own Node::_nodeMask.
        ///  Typically used to force on nodes which may have
        ///  been switched off by their own Node::_nodeMask.
        /// </summary>
        public uint NodeMaskOverride
        {
            get { return _nodeMaskOverride; }
            set { _nodeMaskOverride = value; }
        }


        /// <summary>
        /// Method to called by Node and its subclass' Node::accept() method, if the result is true
        ///  it is used to cull operations of nodes and their subgraphs.
        ///  Return true if the result of a bit wise and of the NodeVisitor::_traversalMask
        ///  with the bit or between NodeVistor::_nodeMaskOverride and the Node::_nodeMask.
        ///  default values for _traversalMask is 0xffffffff, _nodeMaskOverride is 0x0,
        ///  and osg::Node::_nodeMask is 0xffffffff.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool ValidNodeMask(Node node)
        {
            return (TraversalMask & (NodeMaskOverride | node.NodeMask)) != 0;
        }

        #region Protected

        protected VisitorType _visitorType;
        protected uint _traversalNumber;
        protected TraversalMode _traversalMode;
        protected object _userData;
        protected List<Node> _nodePath;

        uint _traversalMask;
        uint _nodeMaskOverride;

        #endregion
    }
}
