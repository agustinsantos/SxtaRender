/* OpenSceneGraph - Copyright (C) 1998-2006 Robert Osfield
 *
 * This library is open source and may be redistributed and/or modified under
 * the terms of the OpenSceneGraph Public License (OSGPL) version 0.0 or
 * (at your option) any later version.  The full license is in LICENSE file
 * included with this distribution, and on the openscenegraph.org website.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * OpenSceneGraph Public License for more details.
 *
 * Ported to C#  by Agustin Santos
*/

using System;
using System.Collections.Generic;

namespace Sxta.OSG
{
    /// <summary>
    /// Base class for all internal nodes in the tree.
    /// </summary>
    public partial class Node : BaseObject
    {
        #region Construction

        /// <summary>
        /// Construct a node.
        /// Initialize the parent list to empty, node name to "" and
        /// bounding sphere dirty flag to true.
        /// </summary>
        public Node() { }

        /// <summary>
        /// Copy constructor using CopyOp to manage deep vs shallow copy.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="copyop"></param>
        public Node(Node node, CopyOp copyop = CopyOp.SHALLOW_COPY) :
            base(node, copyop)
        {
            nodeMask = node.nodeMask;
        }

        /// <summary>
        /// clone an object of the same type as the node.
        /// </summary>
        /// <returns></returns>
        public override BaseObject CloneType() { return new Node(); }

        /// <summary>
        /// return a clone of a node.
        /// </summary>
        /// <param name="copyop"></param>
        /// <returns></returns>
        public override BaseObject Clone(CopyOp copyop) { return new Node(this, copyop); }

        #endregion

        public void Apply(Action action) { }

        /// <summary>
        /// Visitor Pattern : calls the apply method of a NodeVisitor with this node's type.
        /// </summary>
        /// <param name="nv"></param>
        public virtual void Accept(NodeVisitor nv)
        {
            if (nv.ValidNodeMask(this))
            {
                nv.PushOntoNodePath(this);
                nv.Apply(this);
                nv.PopFromNodePath();
            }
        }

        /// <summary>
        ///  Traverse upwards : calls parents' accept method with NodeVisitor.
        /// </summary>
        /// <param name="nv"></param>
        protected internal virtual void Ascend(NodeVisitor nv)
        {
            foreach (var parent in parents)
            {
                parent.Accept(nv);
            }
        }

        /// <summary>
        /// Traverse downwards : calls children's accept method with NodeVisitor.
        /// </summary>
        /// <param name="nv"></param>
        protected internal virtual void Traverse(NodeVisitor nv) { }


        /// <summary>
        /// Get/Set the node Mask.
        /// </summary>
        public uint NodeMask
        {
            get { return nodeMask; }
            set { nodeMask = value; }
        }

        internal void AddParent(Group parent)
        {
            parents.Add(parent);
        }
        internal void RemoveParent(Group parent)
        {
            parents.Remove(parent);
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
        protected uint nodeMask = 0xffffffff;

        /// <summary>
        /// A list of Group references which is used to store the parent(s) of node.
        /// </summary>
        protected List<Group> parents = new List<Group>();
    }
}