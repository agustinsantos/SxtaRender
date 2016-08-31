/*
 * Proland: a procedural landscape rendering library.
 * Website : http://proland.inrialpes.fr/
 * Copyright (c) 2008-2015 INRIA - LJK (CNRS - Grenoble University)
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright notice, 
 * this list of conditions and the following disclaimer.
 * 
 * 2. Redistributions in binary form must reproduce the above copyright notice, 
 * this list of conditions and the following disclaimer in the documentation 
 * and/or other materials provided with the distribution.
 * 
 * 3. Neither the name of the copyright holder nor the names of its contributors 
 * may be used to endorse or promote products derived from this software without 
 * specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
 * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF 
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE 
 * OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 */
/*
 * Proland is distributed under the Berkeley Software Distribution 3 Licence. 
 * For any assistance, feedback and enquiries about training programs, you can check out the 
 * contact page on our website : 
 * http://proland.inrialpes.fr/
 */
/*
 * Main authors: Eric Bruneton, Antoine Begault, Guillaume Piolat.
* Modified and ported to C# and Sxta Engine by Agustin Santos and Daniel Olmedo 2015-2016
*/

using log4net;
using Sxta.Math;
using Sxta.Render;
using Sxta.Render.Scenegraph;
using System;
using System.Reflection;

namespace proland
{
    public class Deformation// : Object
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Creates a new Deformation.
        /// </summary>

        public Deformation() : base()//deformation?
        {
            offsetU = null;
            cameraU = null;
            blendingU = null;
            localToScreenU = null;
            screenQuadCornersU = null;
            screenQuadVerticalsU = null;
        }

        /// <summary>
        /// Returns the deformed point corresponding to the given source point.
        /// </summary>
        /// <param name="localPt">localPt a point in the local (i.e., source) space.</param>
        /// <returns>the corresponding point in the deformed (i.e., destination) space.</returns>
        public virtual Vector3d localToDeformed(Vector3d localPt)
        {
            return localPt;
        }

        /// <summary>
        /// Returns the differential of the deformation function at the given local
        ///point.This differential gives a linear approximation of the deformation
        ///around a given point, represented with a matrix.More precisely, if p
        ///is near localPt, then the deformed point corresponding to p can be
        ///approximated with localToDeformedDifferential(localPt) * (p - localPt).
        /// </summary>
        /// <param name="localPt">localPt a point in the local (i.e., source) space. <i>The z coordinate of this point is ignored, and considered to be 0</i>.</param>
        /// <param name="clamp"></param>
        /// <returns>the differential of the deformation function at the given local point.</returns>
        public virtual Matrix4d localToDeformedDifferential(Vector3d localPt, bool clamp = false)
        {
            return Matrix4d.CreateTranslation(localPt.X, localPt.Y, 0.0);
        }

        /// <summary>
        /// Returns the local point corresponding to the given source point.
        /// </summary>
        /// <param name="deformedPt">deformedPt a point in the deformed (i.e., destination) space.</param>
        /// <returns>the corresponding point in the local (i.e., source) space.</returns>
        public virtual Vector3d deformedToLocal(Vector3d deformedPt)
        {
            return deformedPt;
        }

        /// <summary>
        /// Returns the local bounding box corresponding to the given source disk.
        /// </summary>
        /// <param name="deformedCenter">deformedPt the source disk center in deformed space.</param>
        /// <param name="deformedRadius">deformedRadius the source disk radius in deformed space.</param>
        /// <returns></returns>
        //TODO Why does it use Box2f for doubles and not Boxd2
        public virtual Box2f deformedToLocalBounds(Vector3d deformedCenter, double deformedRadius)
        {
            return new Box2f((float)(deformedCenter.X - deformedRadius), (float)(deformedCenter.X + deformedRadius),
            (float)(deformedCenter.Y - deformedRadius), (float)(deformedCenter.Y + deformedRadius));
        }

        /// <summary>
        /// Returns an orthonormal reference frame of the tangent space at the given
        ///deformed point.This reference frame is such that its xy plane is the
        ///tangent plane, at deformedPt, to the deformed surface corresponding to
        ///the local plane z = 0.Note that this orthonormal reference frame does
        ///<i>not</i> give the differential of the inverse deformation funtion,
        ///which in general is not an orthonormal transformation. If p is a deformed
        ///point, then deformedToLocalFrame(deformedPt) * p gives the coordinates of
        ///p in the orthonormal reference frame defined above.
        /// </summary>
        /// <param name="deformedPt">a point in the deformed (i.e., destination) space.</param>
        /// <returns>the orthonormal reference frame at deformedPt defined above.</returns>
        public virtual Matrix4d deformedToTangentFrame(Vector3d deformedPt)
        {
            return Matrix4d.CreateTranslation(-deformedPt.X, -deformedPt.Y, 0.0);
        }

        /// <summary>
        /// Sets the shader uniforms that are necessary to project on screen the
        ///TerrainQuad of the given TerrainNode.This method can set the uniforms
        ///that are common to all the quads of the given terrain.
        /// </summary>
        /// <param name="context">context the SceneNode to which the TerrainNode belongs. This node
        /// defines the absolute position and orientation of the terrain in
        /// world space(through SceneNode#getLocalToWorld).</param>
        /// <param name="n">a TerrainNode.</param>
        /// <param name="prog"></param>
        public virtual void setUniforms(SceneNode context, TerrainNode n, Program prog)
        {
            if (lastNodeProg != prog)
            {
                blendingU = prog.getUniform2f("deformation.blending");
                localToScreenU = prog.getUniformMatrix4f("deformation.localToScreen");
                tileToTangentU = prog.getUniformMatrix3f("deformation.tileToTangent");
                lastNodeProg = prog;
            }

            if (blendingU != null)
            {
                float d1 = n.getSplitDistance() + 1.0f;
                float d2 = 2.0f * n.getSplitDistance();
                blendingU.set(new Vector2f(d1, d2 - d1));
            }
            cameraToScreen = (Matrix4f)context.getOwner().getCameraToScreen();
            Matrix4d cameraToScreenTmp = context.getOwner().getCameraToScreen();
            Matrix4d localToCameraTmp = context.getLocalToCamera();
            // original localToScreen = context.getOwner().getCameraToScreen() * context.getLocalToCamera();
            localToScreen = cameraToScreenTmp * localToCameraTmp;
            Vector4d p2 = localToScreen * new Vector4d(101.0, 60.0, 140, 1.0);

            if (localToScreenU != null)
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Deformation.setUniforms. localToScreenU:{0}", localToScreen);
                }
                localToScreenU.setMatrix((Matrix4f)localToScreen);
            }
            if (tileToTangentU != null)
            {
                Vector3d localCameraPos = n.getLocalCamera();
                Vector3d worldCamera = context.getOwner().getCameraNode().getWorldPos();
                Vector3d deformedCamera = localToDeformed(localCameraPos);
                Matrix4d A = localToDeformedDifferential(localCameraPos);
                Matrix4d B = deformedToTangentFrame(worldCamera);
                Matrix4d ltow = context.getLocalToWorld();
                Matrix4d ltot = B * ltow * A;
                localToTangent = new Matrix3f((float)ltot.R0C0, (float)ltot.R0C1, (float)ltot.R0C3,
                                       (float)ltot.R1C0, (float)ltot.R1C1, (float)ltot.R1C3,
                                       (float)ltot.R3C0, (float)ltot.R3C1, (float)ltot.R3C3);
            }
        }

        /// <summary>
        ///Sets the shader uniforms that are necessary to project on screen the
        ///given TerrainQuad.This method can set the uniforms that are specific to
        ///the given quad.
        /// </summary>
        /// <param name="context">context the SceneNode to which the TerrainNode belongs. This node
        ///defines the absolute position and orientation of the terrain in
        ///world space(through SceneNode#getLocalToWorld).</param>
        /// <param name="q">a TerrainQuad.</param>
        /// <param name="prog"></param>
        public virtual void setUniforms(SceneNode context, TerrainQuad q, Program prog)
        {
            if (lastQuadProg != prog)
            {
                offsetU = prog.getUniform4f("deformation.offset");
                cameraU = prog.getUniform4f("deformation.camera");
                tileToTangentU = prog.getUniformMatrix3f("deformation.tileToTangent");
                screenQuadCornersU = prog.getUniformMatrix4f("deformation.screenQuadCorners");
                screenQuadVerticalsU = prog.getUniformMatrix4f("deformation.screenQuadVerticals");
                lastQuadProg = prog;
            }

            if (offsetU != null)
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Deformation.setUniforms. offsetU:{0}, {1}, {2}, {3}", (float)q.ox, (float)q.oy, (float)q.l, q.level);
                }
                offsetU.set(new Vector4f((float)q.ox, (float)q.oy, (float)q.l, q.level));
            }
            if (cameraU != null)
            {
                Vector3d camera = q.getOwner().getLocalCamera();
                cameraU.set(new Vector4f((float)((camera.X - q.ox) / q.l),
                    (float)((camera.Y - q.oy) / q.l),
                    (float)((camera.Z - TerrainNode.groundHeightAtCamera) / (q.l * q.getOwner().getDistFactor())),
                    (float)camera.Z));
            }
            if (tileToTangentU != null)
            {
                Vector3d c = q.getOwner().getLocalCamera();
                Matrix3f m = new Matrix3f();
                Matrix3f tmp = new Matrix3f((float)q.l, (float)0.0, (float)(q.ox - c.X), (float)0.0, (float)q.l, (float)(q.oy - c.Y), (float)0.0, (float)0.0, (float)1.0);
                localToTangent.Multiply(ref tmp, out m);
                tileToTangentU.setMatrix(m);
            }

            setScreenUniforms(context, q, prog);
        }

        /// <summary>
        /// Returns the distance in local (i.e., source) space between a point and a
        /// bounding box.
        /// </summary>
        /// <param name="localPt">a point in local space.</param>
        /// <param name="localBox">a bounding box in local space.</param>
        /// <returns></returns>
        public virtual float getLocalDist(Vector3d localPt, Box3d localBox)
        {
            return (float)Math.Max(Math.Abs(localPt.Z - localBox.zmax),
                Math.Max(Math.Min(Math.Abs(localPt.X - localBox.xmin), Math.Abs(localPt.X - localBox.xmax)),
                Math.Min(Math.Abs(localPt.Y - localBox.ymin), Math.Abs(localPt.Y - localBox.ymax))));
        }

        /// <summary>
        /// Returns the visibility of a bounding box in local space, in a view
        /// frustum defined in deformed space.
        /// </summary>
        /// <param name="t">a TerrainNode. This is node is used to get the camera position
        /// in local and deformed space with TerrainNode#getLocalCamera and
        /// TerrainNode#getDeformedCamera, as well as the view frustum planes
        /// in deformed space with TerrainNode#getDeformedFrustumPlanes.</param>
        /// <param name="localBox">a bounding box in local space.</param>
        /// <returns>the visibility of the bounding box in the view frustum.</returns>
        public virtual SceneManager.visibility getVisibility(TerrainNode t, Box3d localBox)
        {
            return SceneManager.getVisibility(t.getDeformedFrustumPlanes(), localBox);
        }

        /// <summary>
        /// The transformation from camera space to screen space.
        /// </summary>
        protected Matrix4f cameraToScreen;

        /// <summary>
        /// The transformation from local space to screen space.
        /// </summary>
        protected Matrix4d localToScreen;

        /// <summary>
        /// The transformation from local space to tangent space (in z=0 plane).
        /// </summary>
        protected Matrix3f localToTangent;

        /// <summary>
        /// The program that contains the uniforms that were set during the last
        /// call to setUniforms(ptr<SceneNode>, ptr<TerrainNode>, ...).
        /// </summary>
        protected Program lastNodeProg;

        /// <summary>
        /// The program that contains the uniforms that were set during the last
        /// call to setUniforms(ptr<SceneNode>, ptr<TerrainQuad>, ...).
        /// </summary>
        protected Program lastQuadProg;

        /// <summary>
        /// The coordinates of a TerrainQuad (ox,oy,l,l).
        /// </summary>
        protected Uniform4f offsetU;

        /// <summary>
        /// The camera coordinates relatively to a TerrainQuad. This vector is
        /// defined as (camera.x - ox) / l, (camera.y - oy) / l), (camera.z -
        /// groundHeightAtCamera) / l), 1.
        /// </summary>
        protected Uniform4f cameraU;

        /// <summary>
        /// The blending parameters for geomorphing. This vector is defined by
        /// (splitDistance + 1, splitDistance - 1).
        /// </summary>
        protected Uniform2f blendingU;

        /// <summary>
        /// The transformation from local space to screen space.
        /// </summary>
        protected UniformMatrix4f localToScreenU;

        /// <summary>
        /// The transformation from local tile coordinates to tangent space.
        /// </summary>
        protected UniformMatrix3f tileToTangentU;

        /// <summary>
        /// The deformed corners of a quad in screen space.
        /// </summary>
        protected UniformMatrix4f screenQuadCornersU;

        /// <summary>
        /// The deformed vertical vectors at the corners of a quad,
        /// in screen space.
        /// </summary>
        protected UniformMatrix4f screenQuadVerticalsU;

        protected virtual void setScreenUniforms(SceneNode context, TerrainQuad q, Program prog)
        {
            Vector3d p0 = new Vector3d(q.ox, q.oy, 0.0);
            Vector3d p1 = new Vector3d(q.ox + q.l, q.oy, 0.0);
            Vector3d p2 = new Vector3d(q.ox, q.oy + q.l, 0.0);
            Vector3d p3 = new Vector3d(q.ox + q.l, q.oy + q.l, 0.0);

            if (screenQuadCornersU != null)
            {
                Matrix4d corners = new Matrix4d(
                    p0.X, p1.X, p2.X, p3.X,
                    p0.Y, p1.Y, p2.Y, p3.Y,
                    p0.Z, p1.Z, p2.Z, p3.Z,
                    1.0, 1.0, 1.0, 1.0);
                screenQuadCornersU.setMatrix((Matrix4f)(localToScreen * corners));
            }

            if (screenQuadVerticalsU != null)
            {
                Matrix4d verticals = new Matrix4d(
                    0.0, 0.0, 0.0, 0.0,
                    0.0, 0.0, 0.0, 0.0,
                    1.0, 1.0, 1.0, 1.0,
                    0.0, 0.0, 0.0, 0.0);
                screenQuadVerticalsU.setMatrix((Matrix4f)(localToScreen * verticals));
            }
        }
    }
}
