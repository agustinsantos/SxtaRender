using Sxta.Math;
using Sxta.Render.Scenegraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proland
{
    /// <summary>
    /// A TerrainViewController for cylindrical terrains. This subclass
    /// interprets the #y0 field as a "longitude" inside the cylinder,
    /// and considers #theta and #phi as relative to the tangent plane at
    /// the(#x0,#y0) point.
    /// </summary>
    public class CylinderViewController : TerrainViewController
    {
        /// <summary>
        /// The radius of the cylindrical terrain at sea level.
        /// </summary>
        public readonly double R;

        /// <summary>
        /// Creates a new CylinderViewController.
        /// </summary>
        /// <param name="node">a SceneNode representing a camera position and orientation
        ///      in the scene.
        ///</param>
        /// <param name="R">the cylindrical terrain radius at sea level.</param>
        public CylinderViewController(SceneNode node, double R) : base(node, R * 0.9)
        {
            this.R = R;
        }

        /*
         * Deletes this PlanetViewController.
         */
        //virtual ~CylinderViewController();

        public override double getHeight()
        {
            return R - Math.Sqrt(Position.Y * Position.Y + Position.Z * Position.Z);
        }

        public override void move(Vector3d oldp, Vector3d p)
        {
            double oldlon = Math.Atan2(oldp.Z, oldp.Y);
            double lon = Math.Atan2(p.Z, p.Y);
            X0 -= p.X - oldp.X;
            Y0 -= lon - oldlon;
        }

        public override void Update()
        {
            double ca = Math.Cos(Y0);
            double sa = Math.Sin(Y0);
            Vector3d po = new Vector3d(X0, sa * (R - groundHeight), -ca * (R - groundHeight));
            Vector3d px = new Vector3d(1.0, 0.0, 0.0);
            Vector3d py = new Vector3d(0.0, ca, sa);
            Vector3d pz = new Vector3d(0.0, -sa, ca);

            double ct = Math.Cos(Theta);
            double st = Math.Sin(Theta);
            double cp = Math.Cos(Phi);
            double sp = Math.Sin(Phi);
            Vector3d cx = px * cp + py * sp;
            Vector3d cy = -px * sp * ct + py * cp * ct + pz * st;
            Vector3d cz = px * sp * st - py * cp * st + pz * ct;
            Position = po + cz * Distance * Zoom;

            double l = Math.Sqrt(Position.Y * Position.Y + Position.Z * Position.Z);
            if (l > R - 1.0 - groundHeight)
            {
                position.Y = Position.Y * (R - 1.0 - groundHeight) / l;
                position.Z = Position.Z * (R - 1.0 - groundHeight) / l;
            }

            Matrix4d view = new Matrix4d(cx.X, cx.Y, cx.Z, 0.0,
                                        cy.X, cy.Y, cy.Z, 0.0,
                                        cz.X, cz.Y, cz.Z, 0.0,
                                        0.0, 0.0, 0.0, 1.0);

            view = Matrix4d.Translate(view, -Position.X, -Position.Y, -Position.Z);
            node.setLocalToParent(Matrix4d.Invert(view));
        }
    }
}
