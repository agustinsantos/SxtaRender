using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxta.OSG
{
    /// <summary>
    /// A Transform is a group node for which all children are transformed by
    /// a 4x4 matrix.It is often used for positioning objects within a scene,
    /// producing trackball functionality or for animation.
    /// 
    /// Transform itself does not provide set/get functions, only the interface
    /// for defining what the 4x4 transformation is.  Subclasses, such as
    /// MatrixTransform and PositionAttitudeTransform support the use of an
    /// osg::Matrix or a osg::Vec3/osg::Quat respectively.
    /// 
    /// Note: If the transformation matrix scales the subgraph then the normals
    /// of the underlying geometry will need to be renormalized to be unit
    /// vectors once more.This can be done transparently through OpenGL's
    /// use of either GL_NORMALIZE and GL_RESCALE_NORMAL modes.For further
    /// background reading see the glNormalize documentation in the OpenGL
    /// Reference Guide (the blue book). To enable it in the OSG, you simply
    /// need to attach a local osg::StateSet to the osg::Transform, and set
    /// the appropriate mode to ON via
    /// stateset->setMode(GL_NORMALIZE, osg::StateAttribute::ON); 
    /// </summary>
    public class Transform : Group
    {

        #region Construction

        /// <summary>
        /// Construct a Transform.
        /// </summary>
        public Transform() { throw new NotImplementedException();  }

        /// <summary>
        /// Copy constructor using CopyOp to manage deep vs shallow copy.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="copyop"></param>
        public Transform(Group group, CopyOp copyop = CopyOp.SHALLOW_COPY) { throw new NotImplementedException(); }


        /// <summary>
        /// clone an object of the same type as the Transform.
        /// </summary>
        /// <returns></returns>
        public override BaseObject CloneType() { return new Group(); }

        /// <summary>
        /// return a clone of a Transform.
        /// </summary>
        /// <param name="copyop"></param>
        /// <returns></returns>
        public override BaseObject Clone(CopyOp copyop) { return new Group(this, copyop); }

        #endregion
 

        public enum ReferenceFrame
        {
            RELATIVE_RF,
            ABSOLUTE_RF,
            ABSOLUTE_RF_INHERIT_VIEWPOINT
        }



        protected ReferenceFrame _referenceFrame;
    }
}