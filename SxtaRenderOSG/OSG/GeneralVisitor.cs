using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxta.OSG
{
    public class GeneralVisitor : NodeVisitor
    {
        #region Globals

        private readonly Predicate<object> predicate;

        #endregion

        #region Construction

        /// <param name="hasCompletedPredicate">The <see cref="Predicate{T}"/> delegate.  The return value is used to indicate whether the visitor has completed.</param>
        public GeneralVisitor(Predicate<object> hasCompletedPredicate, TraversalMode tm = TraversalMode.TRAVERSE_NONE) : base(tm)
        {
            predicate = hasCompletedPredicate;
        }

        #endregion

        public override void Apply(Node obj)
        {
            Console.WriteLine("Node NodeVisitor {0} visited by {1}", obj.GetType().Name, this.GetType().Name);
            HasCompleted = predicate(obj);
            Traverse(obj); 
        }
        public override void Apply(Drawable obj)
        {
            Console.WriteLine("Drawable NodeVisitor {0} visited by {1}", obj.GetType().Name, this.GetType().Name);
            HasCompleted = predicate(obj);
            Traverse(obj);
        }
        public override void Apply(Group obj)
        {
            Console.WriteLine("Group NodeVisitor {0} visited by {1}", obj.GetType().Name, this.GetType().Name);
            HasCompleted = predicate(obj);
            Traverse(obj);
        }
    }
}