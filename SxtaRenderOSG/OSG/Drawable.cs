using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxta.OSG
{
    public class Drawable : Node
    {
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
    }
}