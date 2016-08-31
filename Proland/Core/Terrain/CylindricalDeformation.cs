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

using Sxta.Math;
using Sxta.Render;
using Sxta.Render.Scenegraph;
using System;
using System.Diagnostics;

namespace proland
{
    /// <summary>
    /// A Deformation of space transforming planes to cylinders. This deformation
    /// transforms the plane z=0 into a cylinder of radius R.The
    /// deformation of p= (x, y, z) in local space is q= (x, r.sin(a), r.cos(a)),
    /// where r=R-z and a=y/R.
    /// </summary>
    public class CylindricalDeformation : Deformation
    {
        /// <summary>
        /// The radius of the cylinder into which the plane z=0 must be deformed.
        /// </summary>
        public float R;

        /// <summary>
        /// Creates a new CylindricalDeformation.
        /// </summary>
        /// <param name="R">the radius of the cylinder into which the plane z=0 must be
        ///     deformed.</param>
        public CylindricalDeformation(float R) : base()
        {
            this.R = R;
            this.localToWorldU = null;
            radiusU = null;
        }


        public override Vector3d localToDeformed(Vector3d localPt)
        {
            float alpha = (float)localPt.Y / R;
            float r = R - (float)localPt.Z;
            return new Vector3d(localPt.X, r * Math.Sin(alpha), -r * Math.Cos(alpha));
        }

        public override Matrix4d localToDeformedDifferential(Vector3d localPt, bool clamp = false)
        {
            float alpha = (float)localPt.Y / R;
            return new Matrix4d(1.0, 0.0, 0.0, localPt.X,
                0.0, Math.Cos(alpha), -Math.Sin(alpha), R * Math.Sin(alpha),
                0.0, Math.Sin(alpha), Math.Cos(alpha), -R * Math.Cos(alpha),
                0.0, 0.0, 0.0, 1.0);
        }

        public override Vector3d deformedToLocal(Vector3d deformedPt)
        {
            float Y = R * (float)Math.Atan2((float)deformedPt.Y, (float)-deformedPt.Z);
            float Z = R - (float)Math.Sqrt((float)(deformedPt.Y * deformedPt.Y + deformedPt.Z * deformedPt.Z));
            return new Vector3d(deformedPt.X, Y, Z);
        }

        public override Box2f deformedToLocalBounds(Vector3d deformedCenter, double deformedRadius)
        {
            Debug.Assert(false); // TODO
            return new Box2f();
        }

        public override Matrix4d deformedToTangentFrame(Vector3d deformedPt)
        {
            Vector3d Uz = new Vector3d(0.0, -deformedPt.Y, -deformedPt.Z);
            Uz.Normalize();
            Vector3d Ux = Vector3d.UnitX;
            Vector3d Uy = Vector3d.Cross(Ux, Uz);
            Vector3d O = new Vector3d(deformedPt.X, -Uz.Y * R, -Uz.Z * R);
            return new Matrix4d(Ux.X, Ux.Y, Ux.Z, -Vector3d.Dot(Ux, O),
                Uy.X, Uy.Y, Uy.Z, -Vector3d.Dot(Uy, O),
                Uz.X, Uz.Y, Uz.Z, -Vector3d.Dot(Uz, O),
                0.0, 0.0, 0.0, 1.0);
        }

        public override void setUniforms(SceneNode context, TerrainNode n, Program prog)
        {
            if (lastNodeProg != prog)
            {
                localToWorldU = prog.getUniformMatrix4f("deformation.localToWorld");
                radiusU = prog.getUniform1f("deformation.radius");
            }

            if (localToWorldU != null)
            {
                Matrix4d ltow = context.getLocalToWorld();
                localToWorldU.setMatrix((Matrix4f)ltow);
            }

            base.setUniforms(context, n, prog);

            if (radiusU != null)
            {
                radiusU.set(R);
            }
        }

        public override SceneManager.visibility getVisibility(TerrainNode t, Box3d localBox)
        {
            Vector3d[] deformedBox = new Vector3d[4];
            deformedBox[0] = localToDeformed(new Vector3d(localBox.xmin, localBox.ymin, localBox.zmax));
            deformedBox[1] = localToDeformed(new Vector3d(localBox.xmax, localBox.ymin, localBox.zmax));
            deformedBox[2] = localToDeformed(new Vector3d(localBox.xmax, localBox.ymax, localBox.zmax));
            deformedBox[3] = localToDeformed(new Vector3d(localBox.xmin, localBox.ymax, localBox.zmax));
            double dy = localBox.ymax - localBox.ymin;
            float f = (float)((R - localBox.zmin) / ((R - localBox.zmax) * Math.Cos(dy / (2.0 * R))));

            Vector4d[] deformedFrustumPlanes = t.getDeformedFrustumPlanes();
            SceneManager.visibility v0 = getVisibility(deformedFrustumPlanes[0], deformedBox, f);
            if (v0 == SceneManager.visibility.INVISIBLE)
            {
                return SceneManager.visibility.INVISIBLE;
            }
            SceneManager.visibility v1 = getVisibility(deformedFrustumPlanes[1], deformedBox, f);
            if (v1 == SceneManager.visibility.INVISIBLE)
            {
                return SceneManager.visibility.INVISIBLE;
            }
            SceneManager.visibility v2 = getVisibility(deformedFrustumPlanes[2], deformedBox, f);
            if (v2 == SceneManager.visibility.INVISIBLE)
            {
                return SceneManager.visibility.INVISIBLE;
            }
            SceneManager.visibility v3 = getVisibility(deformedFrustumPlanes[3], deformedBox, f);
            if (v3 == SceneManager.visibility.INVISIBLE)
            {
                return SceneManager.visibility.INVISIBLE;
            }
            SceneManager.visibility v4 = getVisibility(deformedFrustumPlanes[4], deformedBox, f);
            if (v4 == SceneManager.visibility.INVISIBLE)
            {
                return SceneManager.visibility.INVISIBLE;
            }
            if (v0 == SceneManager.visibility.FULLY_VISIBLE && v1 == SceneManager.visibility.FULLY_VISIBLE &&
                v2 == SceneManager.visibility.FULLY_VISIBLE && v3 == SceneManager.visibility.FULLY_VISIBLE &&
                v4 == SceneManager.visibility.FULLY_VISIBLE)
            {
                return SceneManager.visibility.FULLY_VISIBLE;
            }
            return SceneManager.visibility.PARTIALLY_VISIBLE;
        }

        private UniformMatrix4f localToWorldU;

        private Uniform1f radiusU;

        private static SceneManager.visibility getVisibility(Vector4d clip, Vector3d[] b, float f)
        {
            double c1 = b[0].X * clip.X + clip.W;
            double c2 = b[1].X * clip.X + clip.W;
            double c3 = b[2].X * clip.X + clip.W;
            double c4 = b[3].X * clip.X + clip.W;
            double o1 = b[0].Y * clip.X + b[0].Z * clip.Z;
            double o2 = b[1].Y * clip.Y + b[1].Z * clip.Z;
            double o3 = b[2].Y * clip.Y + b[2].Z * clip.Z;
            double o4 = b[3].Y * clip.Y + b[3].Z * clip.Z;
            double p1 = o1 + c1;
            double p2 = o2 + c2;
            double p3 = o3 + c3;
            double p4 = o4 + c4;
            double p5 = o1 * f + c1;
            double p6 = o2 * f + c2;
            double p7 = o3 * f + c3;
            double p8 = o4 * f + c4;
            if (p1 <= 0 && p2 <= 0 && p3 <= 0 && p4 <= 0 && p5 <= 0 && p6 <= 0 && p7 <= 0 && p8 <= 0)
            {
                return SceneManager.visibility.INVISIBLE;
            }
            if (p1 > 0 && p2 > 0 && p3 > 0 && p4 > 0 && p5 > 0 && p6 > 0 && p7 > 0 && p8 > 0)
            {
                return SceneManager.visibility.FULLY_VISIBLE;
            }
            return SceneManager.visibility.PARTIALLY_VISIBLE;
        }
    }
}
