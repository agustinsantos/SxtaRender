using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxta.OSG
{
    public class Camera
    {
        /** Draw callback for custom operations.*/
        public class DrawCallback : BaseObject
        {
            #region Construction

            /// <summary>
            /// Construct a DrawCallback.
            /// Initialize the parent list to empty, node name to "" and
            /// bounding sphere dirty flag to true.
            /// </summary>
            public DrawCallback() { }

            /// <summary>
            /// Copy constructor using CopyOp to manage deep vs shallow copy.
            /// </summary>
            /// <param name="node"></param>
            /// <param name="copyop"></param>
            public DrawCallback(DrawCallback node, CopyOp copyop = CopyOp.SHALLOW_COPY) { }

            /// <summary>
            /// clone an object of the same type as the DrawCallback.
            /// </summary>
            /// <returns></returns>
            public override BaseObject CloneType() { return new Node(); }

            /// <summary>
            /// return a clone of a DrawCallback.
            /// </summary>
            /// <param name="copyop"></param>
            /// <returns></returns>
            public override BaseObject Clone(CopyOp copyop) { return new DrawCallback(this, copyop); }

            #endregion

            /** Functor method called by rendering thread. Users will typically override this method to carry tasks such as screen capture.*/
            public virtual void Do(RenderInfo renderInfo) { throw new NotImplementedException(); }

            /** Functor method, provided for backwards compatibility, called by operator() (osg::RenderInfo& renderInfo) method.*/
            public virtual void Do(Camera camera) { }
        }
    }
}