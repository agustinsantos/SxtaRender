using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.Render.OSG
{
    /// <summary>
    /// Callback to allow users to override the default computation of bounding volume.
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public delegate BoundingSphere ComputeBoundDelegate(Node n);

    public class Node
    {
        public virtual void Ascend(NodeVisitor nv)
        {
            throw new NotImplementedException();
        }

        public virtual void Traverse(NodeVisitor nv)
        {
            throw new NotImplementedException();
        }
        public virtual void Accept(NodeVisitor nv)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get/Set the node Mask.
        /// </summary>
        public uint NodeMask
        {
            get { return _nodeMask; }
            set { _nodeMask = value; }
        }


        /*
        * This is a set of bits (flags) that represent the Node.
        * The default value is 0xffffffff (all bits set).
        *
        * The most common use of these is during traversal of the scene graph.
        * For instance, when traversing the scene graph the osg::NodeVisitor does a bitwise
        * AND of its TraversalMask with the Node's NodeMask to
        * determine if the Node should be processed/traversed.
        *
        * For example, if a Node has a NodeMask value of 0x02 (only 2nd bit set)
        * and the osg::Camera has a CullMask of 0x4 (2nd bit not set) then during cull traversal,
        * which takes it's TraversalMask from the Camera's CullMask, the node and any children
        * would be ignored and thereby treated as "culled" and thus not rendered.
        * Conversely, if the osg::Camera CullMask were 0x3 (2nd bit set) then the node
        * would be processed and child Nodes would be examined.
        */
        //typedef unsigned int NodeMask;
        protected uint _nodeMask = 0xffffffff;
    }
#if TODO
    /// <summary>
    /// Base class for all internal nodes in the scene graph.
    /// Provides interface for most common node operations (Composite Pattern).
    /// </summary>
    public class Node : IVisitorPattern
    {

        /** Construct a node.
    Initialize the parent list to empty, node name to "" and
    bounding sphere dirty flag to true.*/
        public Node()
        {
            _boundingSphereComputed = false;
#if TODO
            _nodeMask = 0xffffffff;

            _numChildrenRequiringUpdateTraversal = 0;

            _numChildrenRequiringEventTraversal = 0;

            _cullingActive = true;
            _numChildrenWithCullingDisabled = 0;

            _numChildrenWithOccluderNodes = 0;
 
#endif
        }

        /** Copy constructor using CopyOp to manage deep vs shallow copy.*/
        public Node(Node node, CopyFlags copyop = CopyFlags.SHALLOW_COPY)
        { throw new NotImplementedException(); }



       

       

        internal void AddParent(Group node)
        {
            _parents.Add(node);
        }

        internal void RemoveParent(Group node)
        {
            _parents.Remove(node);
        }

        /// <summary>
        /// the initial bounding volume to use when computing the overall bounding volume
        /// </summary>
        public BoundingSphere InitialBound
        {
            get { return _initialBound; }
            set { _initialBound = value; DirtyBound(); }
        }

        /// <summary>
        ///  Mark this node's bounding sphere dirty.
        ///  Forcing it to be computed on the next call to getBound()
        /// </summary>
        public void DirtyBound()
        {
            if (_boundingSphereComputed)
            {
                _boundingSphereComputed = false;

                // dirty parent bounding sphere's to ensure that all are valid.
                if (_parents != null)
                    foreach (Group parent in _parents)
                    {
                        parent.DirtyBound();
                    }

            }
        }

        /// <summary>
        /// Get the bounding sphere of node.
        /// Using lazy evaluation computes the bounding sphere if it is 'dirty'
        /// </summary>
        /// <returns></returns>
        public BoundingSphere GetBound()
        {
            if (!_boundingSphereComputed)
            {
                _boundingSphere = _initialBound;
                if (_computeBoundCallback != null)
                    _boundingSphere.ExpandBy(_computeBoundCallback(this));
                else
                    _boundingSphere.ExpandBy(ComputeBound());

                _boundingSphereComputed = true;
            }
            return _boundingSphere;
        }


        /// <summary>
        /// Compute the bounding sphere around Node's geometry or children.
        ///  This method is automatically called by getBound() when the bounding
        /// sphere has been marked dirty via dirtyBound()
        /// </summary>
        /// <returns></returns>
        public virtual BoundingSphere ComputeBound()
        {
            return new BoundingSphere();
        }


        /// <summary>
        /// the compute bound callback to override the default computeBound.
        /// </summary>
        public ComputeBoundDelegate ComputeBoundingSphereCallback
        {
            get { return _computeBoundCallback; }
            set { _computeBoundCallback = value; }
        }



    #region Protected
        protected BoundingSphere _initialBound;
        protected ComputeBoundDelegate _computeBoundCallback;
        protected BoundingSphere _boundingSphere;
        protected bool _boundingSphereComputed;

        protected List<Group> _parents;

        
    #endregion
    }
#endif
}
