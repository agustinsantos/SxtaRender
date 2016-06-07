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
    public class NodeDataEventArgs<T> : EventArgs
    {
        private T _Data = default(T);

        public T Data { get { return _Data; } }

        public NodeDataEventArgs(T data)
        {
            _Data = data;
        }
    }

    public class NodeEventArgs : EventArgs
    {
        private Node node = null;

        public Node Node { get { return node; } }

        public NodeEventArgs(Node node)
        {
            this.node = node;
        }
    }

    public class NodeInsertedEventArgs : EventArgs
    {
        private int index;
        private Group sender = null;

        public int Index { get { return index; } }

        public Group Sender { get { return sender; } }

        public NodeInsertedEventArgs(Group sender, int pos)
        {
            this.index = pos;
            this.sender = sender;
        }
    }


    /// <summary>
    /// General group node which maintains a list of children.
    /// </summary>
    public class Group : Node
    {
        #region Construction
        
        /// <summary>
        /// Construct a Group.
        /// </summary>
        public Group() { }

        /// <summary>
        /// Copy constructor using CopyOp to manage deep vs shallow copy.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="copyop"></param>
        public Group(Group group, CopyOp copyop = CopyOp.SHALLOW_COPY) :
            base(group, copyop)
        {
            foreach (Node  child in group.children )
            {
                Node  newChild = copyop.Copy(child);
                if (newChild != null) AddChild(newChild);
            }
        }


        /// <summary>
        /// clone an object of the same type as the Group.
        /// </summary>
        /// <returns></returns>
        public override BaseObject CloneType() { return new Group(); }

        /// <summary>
        /// return a clone of a Group.
        /// </summary>
        /// <param name="copyop"></param>
        /// <returns></returns>
        public override BaseObject Clone(CopyOp copyop) { return new Group(this, copyop); }
        
        #endregion

        protected internal override void Traverse(NodeVisitor nv)
        {
            foreach (Node item in this.children)
                item.Accept(nv);
        }

        /// <summary>
        /// Visitor Pattern : calls the apply method of a NodeVisitor with this node's type.
        /// </summary>
        /// <param name="nv"></param>
        public override void Accept(NodeVisitor nv)
        {
            if (nv.ValidNodeMask(this))
            {
                nv.PushOntoNodePath(this);
                nv.Apply(this);
                nv.PopFromNodePath();
            }
        }

        /// <summary>
        /// Set child node at position i.
        /// Return true if set correctly, false on failure (if node==null || i is out of range).
        /// If origNode is not found then return false and do not
        /// add newNode. If newNode is null then return false and do
        /// not remove origNode.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual bool SetChild(int i, Node node)
        {
            if (node == null || i < 0 || i >= children.Count)
                return false;
            else
            {
                children[i] = node;
                return true;
            }
        }

        /// <summary>
        /// Return child node at position i.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public virtual Node GetChild(int i)
        {
            return children[i];
        }

        /// <summary>
        /// Return child node at position i.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Node this[int pos] { get { return children[pos]; } }

        /// <summary>
        /// Add Node to Group.
        /// If node is not null and is not contained in Group then add it to the child list and
        ///  return true for success.
        /// Otherwise return false. 
        /// </summary>
        public virtual bool AddChild(Node node)
        {
            return InsertChild(children.Count, node);
        }


        /// <summary>
        /// Insert Node to Group at specific location.
        /// The new child node is inserted into the child list
        /// before the node at the specified index. No nodes
        /// are removed from the group with this operation.
        /// </summary>
        public virtual bool InsertChild(int index, Node child)
        {
            if (child == null)
                return false;
            else
            {
                if (index >= children.Count)
                {
                    index = children.Count;
                    children.Add(child);
                }
                else
                {
                    children.Insert(index, child);
                }

                child.AddParent(this);

                // tell any subclasses that a child has been inserted so that they can update themselves.
                ChildInserted(index);

                return true;
            }

            // register as parent of child.
        }

        /// <summary>
        /// Remove Node from Group.
        /// If Node is contained in Group then remove it from the child
        /// list, and return true for success. If Node is not found then return false
        /// and do not change the reference count of the Node.
        /// </summary>
        /// <remarks>Note, do not override, only override RemoveChildren(,) is required.</remarks>
        public bool RemoveChild(Node child)
        {
            if (child == null)
                return false;
            else
            {
                int pos = children.IndexOf(child);
                return RemoveChildren(pos);
            }
        }

        public bool RemoveChild(int pos, int numChildrenToRemove = 1)
        {
            return RemoveChildren(pos, numChildrenToRemove);
        }

        /// <summary>
        /// Remove children from Group.
        /// Note, must be override by subclasses of Group which add per child attributes.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="numChildrenToRemove"></param>
        /// <returns></returns>
        public virtual bool RemoveChildren(int pos, int numChildrenToRemove = 1)
        {
            for (int i = pos; i < pos + numChildrenToRemove; i++)
            {
                if (i < 0 || i >= children.Count) return false;
                children.RemoveAt(i);
            }
            ChildRemoved(pos, numChildrenToRemove);
            return true;
        }

        /// <summary>
        /// Replace specified child Node with another Node.
        /// Equivalent to SetChild(GetChildIndex(origChild),node)
        /// See docs for SetChild for further details on implementation.
        /// </summary>
        /// <param name="origChild"></param>
        /// <param name="newChild"></param>
        /// <returns></returns>
        public virtual bool ReplaceChild(Node origChild, Node newChild)
        {
            if (newChild == null) return false;
            int index = children.IndexOf(origChild);
            if (index != -1)
            {
                children[index] = newChild;
                return true;
            }
            else
                return false;
        }

        public IEnumerable<Node> Children
        {
            get { return children; }
        }

        public int NumChildren
        {
            get { return children.Count; }
        }

        public bool HasChildren
        {
            get { return children.Count > 0; }
        }

        public bool ContainsNode(Node child)
        {
            return children.Contains(child);
        }

        public int GetChildIndex(Node node)
        {
            return children.IndexOf(node);
        }

        public event EventHandler<NodeInsertedEventArgs> Inserted;
        public event EventHandler Cleared;
        public event EventHandler<NodeDataEventArgs<Group>> SetDone;
        public event EventHandler CutDone;
        public event EventHandler<NodeEventArgs> Copied;
        public event EventHandler<NodeEventArgs> DeepCopied;

        protected virtual void ChildRemoved(int pos, int numChildrenToRemove) { }
        protected virtual void ChildInserted(int pos) { }

        protected List<Node> children = new List<Node>();
    }
}