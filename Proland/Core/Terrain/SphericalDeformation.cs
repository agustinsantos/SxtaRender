using Sxta.Math;
using Sxta.Render;
using Sxta.Render.Scenegraph;
using System;
using System.Diagnostics;

namespace proland
{
    /// <summary>
    /// A Deformation of space transforming planes to spheres. This deformation
    /// transforms the plane z=0 into a sphere of radius R centered at(0,0,-R).
    /// The plane z = h is transformed into the sphere of radius R+h.The
    /// deformation of p = (x, y, z) in local space is q=(R+z) P /\norm P\norm,
    /// where P=(x,y,R).
    /// </summary>
    public class SphericalDeformation : Deformation
    {
        /// <summary>
        /// The radius of the sphere into which the plane z=0 must be deformed.
        /// </summary>
        public float R;

        /// <summary>
        /// Creates a new SphericalDeformation.
        /// </summary>
        /// <param name="R">the radius of the sphere into which the plane z=0 must be deformed.</param>
        public SphericalDeformation(float R) : base()
        {
            this.R = R;
            radiusU = null;
            localToWorldU = null;
            screenQuadCornerNormsU = null;
            tangentFrameToWorldU = null;
        }


        public override Vector3d localToDeformed(Vector3d localPt)
        {
            Vector3d rst = new Vector3d(localPt.X, localPt.Y, R);
            rst.Normalize(localPt.Z + R);
            return rst;
        }

        public override Matrix4d localToDeformedDifferential(Vector3d localPt, bool clamp = false)
        {
            if (Double.IsInfinity(localPt.X) || Double.IsInfinity(localPt.Y) || Double.IsInfinity(localPt.Z))
            {
                return Matrix4d.Identity;
            }

            Vector3d pt = localPt;
            if (clamp)
            {
                pt.X = pt.X - Math.Floor((pt.X + R) / (2.0 * R)) * 2.0 * R;
                pt.Y = pt.Y - Math.Floor((pt.Y + R) / (2.0 * R)) * 2.0 * R;
            }

            double l = pt.X * pt.X + pt.Y * pt.Y + R * R;
            double c0 = 1.0 / Math.Sqrt(l);
            double c1 = c0 * R / l;
            return new Matrix4d((pt.Y * pt.Y + R * R) * c1, -pt.X * pt.Y * c1, pt.X * c0, R * pt.X * c0,
                -pt.X * pt.Y * c1, (pt.X * pt.X + R * R) * c1, pt.Y * c0, R * pt.Y * c0,
                -pt.X * R * c1, -pt.Y * R * c1, R * c0, (R * R) * c0,
                0.0, 0.0, 0.0, 1.0);
        }

        public override Vector3d deformedToLocal(Vector3d deformedPt)
        {
            double l = deformedPt.Length;
            if (deformedPt.Z >= Math.Abs(deformedPt.X) && deformedPt.Z >= Math.Abs(deformedPt.Y))
            {
                return new Vector3d(deformedPt.X / deformedPt.Z * R, deformedPt.Y / deformedPt.Z * R, l - R);
            }
            if (deformedPt.Z <= -Math.Abs(deformedPt.X) && deformedPt.Z <= -Math.Abs(deformedPt.Y))
            {
                return new Vector3d(Double.PositiveInfinity, Double.PositiveInfinity, Double.PositiveInfinity);
            }
            if (deformedPt.Y >= Math.Abs(deformedPt.X) && deformedPt.Y >= Math.Abs(deformedPt.Z))
            {
                return new Vector3d(deformedPt.X / deformedPt.Y * R, (2.0 - deformedPt.Z / deformedPt.Y) * R, l - R);
            }
            if (deformedPt.Y <= -Math.Abs(deformedPt.X) && deformedPt.Y <= -Math.Abs(deformedPt.Z))
            {
                return new Vector3d(-deformedPt.X / deformedPt.Y * R, (-2.0 - deformedPt.Z / deformedPt.Y) * R, l - R);
            }
            if (deformedPt.X >= Math.Abs(deformedPt.Y) && deformedPt.X >= Math.Abs(deformedPt.Z))
            {
                return new Vector3d((2.0 - deformedPt.Z / deformedPt.X) * R, deformedPt.Y / deformedPt.X * R, l - R);
            }
            if (deformedPt.X <= -Math.Abs(deformedPt.Y) && deformedPt.X <= -Math.Abs(deformedPt.Z))
            {
                return new Vector3d((-2.0 - deformedPt.Z / deformedPt.X) * R, -deformedPt.Y / deformedPt.X * R, l - R);
            }
            Debug.Assert(false, "SphericalDeformation.deformedToLocal");
            return Vector3d.Zero;
        }

        public override Box2f deformedToLocalBounds(Vector3d deformedCenter, double deformedRadius)
        {
            Vector3d p = deformedToLocal(deformedCenter);
            double r = deformedRadius;

            if (Double.IsInfinity(p.X) || Double.IsInfinity(p.Y))
            {
                return new Box2f();
            }

            double k = (1.0 - r * r / (2.0 * R * R)) * new Vector3d(p.X, p.Y, R).Length;
            double A = k * k - p.X * p.X;
            double B = k * k - p.Y * p.Y;
            double C = -2.0 * p.X * p.Y;
            double D = -2.0 * R * R * p.X;
            double E = -2.0 * R * R * p.Y;
            double F = R * R * (k * k - R * R);

            double a = C * C - 4.0 * A * B;
            double b = 2.0 * C * E - 4.0 * B * D;
            double c = E * E - 4.0 * B * F;
            double d = Math.Sqrt(b * b - 4.0 * a * c);
            double x1 = (-b - d) / (2.0 * a);
            double x2 = (-b + d) / (2.0 * a);

            b = 2.0 * C * D - 4.0 * A * E;
            c = D * D - 4.0 * A * F;
            d = Math.Sqrt(b * b - 4.0 * a * c);
            double y1 = (-b - d) / (2.0 * a);
            double y2 = (-b + d) / (2.0 * a);

            return new Box2f((float)x1, (float)y1, (float)x2, (float)y2);
        }

        public override Matrix4d deformedToTangentFrame(Vector3d deformedPt)
        {
            Vector3d Uz = deformedPt;
            Uz.Normalize();
            Vector3d Ux = (Vector3d.Cross(Vector3d.UnitY, Uz));
            Ux.Normalize();
            Vector3d Uy = Vector3d.Cross(Uz, Ux);//Uz.crossProduct(Ux);
            return new Matrix4d(Ux.X, Ux.Y, Ux.Z, 0.0,
                                Uy.X, Uy.Y, Uy.Z, 0.0,
                                Uz.X, Uz.Y, Uz.Z, -R,
                                0.0, 0.0, 0.0, 1.0);
        }

        public override void setUniforms(SceneNode context, TerrainNode n, Program prog)
        {
            if (lastNodeProg != prog)
            {
                radiusU = prog.getUniform1f("deformation.radius");
                localToWorldU = prog.getUniformMatrix3f("deformation.localToWorld");
            }

            base.setUniforms(context, n, prog);

            if (localToWorldU != null)
            {
                Matrix4d ltow = context.getLocalToWorld();
                localToWorldU.setMatrix(new Matrix3f(
                    (float)ltow.R0C0, (float)ltow.R0C1, (float)ltow.R0C2,
                    (float)ltow.R1C0, (float)ltow.R1C1, (float)ltow.R1C2,
                    (float)ltow.R2C0, (float)ltow.R2C1, (float)ltow.R2C2));
            }

            if (radiusU != null)
            {
                radiusU.set(R);
            }
        }

        public override void setUniforms(SceneNode context, TerrainQuad q, Program prog)
        {
            if (lastQuadProg != prog)
            {
                screenQuadCornerNormsU = prog.getUniform4f("deformation.screenQuadCornerNorms");
                tangentFrameToWorldU = prog.getUniformMatrix3f("deformation.tangentFrameToWorld");
            }
            base.setUniforms(context, q, prog);
        }

        public override SceneManager.visibility getVisibility(TerrainNode t, Box3d localBox)
        {
            Vector3d[] deformedBox = new Vector3d[4];
            deformedBox[0] = localToDeformed(new Vector3d(localBox.xmin, localBox.ymin, localBox.zmin));
            deformedBox[1] = localToDeformed(new Vector3d(localBox.xmax, localBox.ymin, localBox.zmin));
            deformedBox[2] = localToDeformed(new Vector3d(localBox.xmax, localBox.ymax, localBox.zmin));
            deformedBox[3] = localToDeformed(new Vector3d(localBox.xmin, localBox.ymax, localBox.zmin));
            double a = (localBox.zmax + R) / (localBox.zmin + R);
            double dx = (localBox.xmax - localBox.xmin) / 2 * a;
            double dy = (localBox.ymax - localBox.ymin) / 2 * a;
            double dz = localBox.zmax + R;
            double f = Math.Sqrt(dx * dx + dy * dy + dz * dz) / (localBox.zmin + R);

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

            Vector3d c = t.getDeformedCamera();
            double lSq = c.LengthSquared;
            double rm = R + Math.Min(0.0, localBox.zmin);
            double rM = R + localBox.zmax;
            double rmSq = rm * rm;
            double rMSq = rM * rM;
            Vector4d farPlane = new Vector4d(c.X, c.Y, c.Z, Math.Sqrt((lSq - rmSq) * (rMSq - rmSq)) - rmSq);

            SceneManager.visibility v5 = getVisibility(farPlane, deformedBox, f);
            if (v5 == SceneManager.visibility.INVISIBLE)
            {
                return SceneManager.visibility.INVISIBLE;
            }

            if (v0 == SceneManager.visibility.FULLY_VISIBLE && v1 == SceneManager.visibility.FULLY_VISIBLE &&
                v2 == SceneManager.visibility.FULLY_VISIBLE && v3 == SceneManager.visibility.FULLY_VISIBLE &&
                v4 == SceneManager.visibility.FULLY_VISIBLE && v5 == SceneManager.visibility.FULLY_VISIBLE)
            {
                return SceneManager.visibility.FULLY_VISIBLE;
            }
            return SceneManager.visibility.PARTIALLY_VISIBLE;
        }


        protected override void setScreenUniforms(SceneNode context, TerrainQuad q, Program prog)
        {
            Vector3d p0 = new Vector3d(q.ox, q.oy, R);
            Vector3d p1 = new Vector3d(q.ox + q.l, q.oy, R);
            Vector3d p2 = new Vector3d(q.ox, q.oy + q.l, R);
            Vector3d p3 = new Vector3d(q.ox + q.l, q.oy + q.l, R);
            Vector3d pc = (p0 + p3) * 0.5;
            double l0 = p0.Length, l1 = p1.Length, l2 = p2.Length, l3 = p3.Length;
            Vector3d v0 = p0; v0.Normalize();
            Vector3d v1 = p1; v1.Normalize();
            Vector3d v2 = p2; v2.Normalize();
            Vector3d v3 = p3; v3.Normalize();

            if (screenQuadCornersU != null)
            {
                Matrix4d deformedCorners = new Matrix4d(
                    v0.X * R, v1.X * R, v2.X * R, v3.X * R,
                    v0.Y * R, v1.Y * R, v2.Y * R, v3.Y * R,
                    v0.Z * R, v1.Z * R, v2.Z * R, v3.Z * R,
                    1.0, 1.0, 1.0, 1.0);
                screenQuadCornersU.setMatrix((Matrix4f)(localToScreen * deformedCorners));
            }

            if (screenQuadVerticalsU != null)
            {
                Matrix4d deformedVerticals = new Matrix4d(
                    v0.X, v1.X, v2.X, v3.X,
                    v0.Y, v1.Y, v2.Y, v3.Y,
                    v0.Z, v1.Z, v2.Z, v3.Z,
                    0.0, 0.0, 0.0, 0.0);
                screenQuadVerticalsU.setMatrix((Matrix4f)(localToScreen * deformedVerticals));
            }

            if (screenQuadCornerNormsU != null)
            {
                screenQuadCornerNormsU.set(new Vector4f((float)l0, (float)l1, (float)l2, (float)l3));
            }
            if (tangentFrameToWorldU != null)
            {
                Vector3d uz = pc;
                uz.Normalize();
                Vector3d ux = Vector3d.Cross(Vector3d.UnitY, uz);
                ux.Normalize();
                Vector3d uy = Vector3d.Cross(uz, ux);

                Matrix4d ltow = context.getLocalToWorld();
                Matrix3d tangentFrameToWorld = new Matrix3d(
                    ltow.R0C0, ltow.R0C1, ltow.R0C2,
                    ltow.R1C0, ltow.R1C1, ltow.R1C2,
                    ltow.R2C0, ltow.R2C1, ltow.R2C2) *
                new Matrix3d(
                    ux.X, uy.X, uz.X,
                    ux.Y, uy.Y, uz.Y,
                    ux.Z, uy.Z, uz.Z);
                tangentFrameToWorldU.setMatrix((Matrix3f)tangentFrameToWorld);
            }
        }

        /// <summary>
        /// The radius of the deformation.
        /// </summary>
        private Uniform1f radiusU;

        /// <summary>
        /// The transformation from local space to world space.
        /// </summary>
        private UniformMatrix3f localToWorldU;

        /// <summary>
        /// The norms of the (x,y,R) vectors corresponding to
        /// the corners of a quad.
        /// </summary>
        private Uniform4f screenQuadCornerNormsU;

        /// <summary>
        /// The transformation from the tangent space at a quad's center to
        /// world space.
        /// </summary>
        private UniformMatrix3f tangentFrameToWorldU;

        private static SceneManager.visibility getVisibility(Vector4d clip, Vector3d[] b, double f)
        {
            double o = b[0].X * clip.X + b[0].Y * clip.Y + b[0].Z * clip.Z;
            bool p = o + clip.W > 0.0;
            if ((o * f + clip.W > 0.0) == p)
            {
                o = b[1].X * clip.X + b[1].Y * clip.Y + b[1].Z * clip.Z;
                if ((o + clip.W > 0.0) == p && (o * f + clip.W > 0.0) == p)
                {
                    o = b[2].X * clip.X + b[2].Y * clip.Y + b[2].Z * clip.Z;
                    if ((o + clip.W > 0.0) == p && (o * f + clip.W > 0.0) == p)
                    {
                        o = b[3].X * clip.X + b[3].Y * clip.Y + b[3].Z * clip.Z;
                        return (o + clip.W > 0.0) == p && (o * f + clip.W > 0.0) == p ? (p ? SceneManager.visibility.FULLY_VISIBLE : SceneManager.visibility.INVISIBLE) : SceneManager.visibility.PARTIALLY_VISIBLE;
                    }
                }
            }
            return SceneManager.visibility.PARTIALLY_VISIBLE;
        }
    }
}
