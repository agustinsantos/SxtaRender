using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sxta.OSG;

namespace SxtaRenderTests
{
    [TestClass]
    public class OSGTests
    {
        [TestMethod]
        public void TestVisitor()
        {
            // Setup Group collection
            Group root = new Group() { Name = "Group 1" };
            root.AddChild(new Node() { Name = "Node 1-1" });
            root.AddChild(new Drawable() { Name = "Drawable 1-2" });

            Group g = new Group() { Name = "Group 2" };
            root.AddChild(g);
            g.AddChild(new Node() { Name = "Node 2-1" });
            Drawable child22 = new Drawable() { Name = "Drawable 2-2" };
            g.AddChild(child22);

            root.AddChild(new Node() { Name = "Node 1-3" });


            Console.WriteLine("GeneralVisitor TRAVERSE_NONE");
            root.Accept(new GeneralVisitor(IsNode, TraversalMode.TRAVERSE_NONE));
            Console.WriteLine("GeneralVisitor TRAVERSE_ALL_CHILDREN");
            root.Accept(new GeneralVisitor(IsNode, TraversalMode.TRAVERSE_ALL_CHILDREN));
            Console.WriteLine("GeneralVisitor TRAVERSE_ACTIVE_CHILDREN");
            root.Accept(new GeneralVisitor(IsNode, TraversalMode.TRAVERSE_ACTIVE_CHILDREN));
            Console.WriteLine("GeneralVisitor TRAVERSE_PARENTS");
            child22.Accept(new GeneralVisitor(IsNode, TraversalMode.TRAVERSE_PARENTS));

            Console.WriteLine("ActionVisitor TRAVERSE_NONE");
            root.Accept(new ActionVisitor(ShowType, TraversalMode.TRAVERSE_NONE));
            Console.WriteLine("ActionVisitor TRAVERSE_ALL_CHILDREN");
            root.Accept(new ActionVisitor(ShowType, TraversalMode.TRAVERSE_ALL_CHILDREN));
            Console.WriteLine("ActionVisitor TRAVERSE_ACTIVE_CHILDREN");
            root.Accept(new ActionVisitor(ShowType, TraversalMode.TRAVERSE_ACTIVE_CHILDREN));
            Console.WriteLine("ActionVisitor TRAVERSE_PARENTS");
            child22.Accept(new ActionVisitor(ShowType, TraversalMode.TRAVERSE_PARENTS));

            // Wait for user
            Console.ReadKey();
        }

        private static bool IsNode(object obj)
        {
            return obj is Node;
        }
        private static void ShowType(object obj)
        {
            Console.WriteLine(obj.GetType());
        }
    }
}
