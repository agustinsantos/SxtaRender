using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxta.OSG
{
    /// <summary>
    /// A general visitor that executes the specified <see cref="Action{T}"/> delegate.
    /// </summary>
    /// <typeparam name="T">The type of item to visit.</typeparam>
    public class ActionVisitor : NodeVisitor
    {
        #region Globals

        private readonly Action<object> action;

        #endregion

        #region Construction

        /// <param name="action">The <see cref="Action{T}"/> delegate.  The return value is used to indicate whether the visitor has completed.</param>
        public ActionVisitor(Action<object> action, TraversalMode tm = TraversalMode.TRAVERSE_NONE) : base(tm)
        {
            this.action = action;
        }

        #endregion

        #region IVisitor<T> Members

        /// <inheritdoc />
        public override void Apply(Node obj)
        {
            Console.WriteLine("Node ActionVisitor {0} visited by {1}", obj.Name, this.GetType().Name);
            action(obj);
            Traverse(obj);
         }
        public override void Apply(Drawable obj)
        {
            Console.WriteLine("Drawable ActionVisitor {0} visited by {1}", obj.Name, this.GetType().Name);
            action(obj);
            Traverse(obj);
        }

        public override void Apply(Group obj)
        {
            Console.WriteLine("Group ActionVisitor {0} visited by {1}", obj.Name, this.GetType().Name);
            action(obj);
            Traverse(obj);
        }
        #endregion
    }
}