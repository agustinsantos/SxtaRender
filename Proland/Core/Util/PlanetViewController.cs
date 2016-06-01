using Sxta.Math;
using Sxta.Render.Scenegraph;
using System;

namespace proland
{
    class PlanetViewController : TerrainViewController
    {

        /*
         * The radius of the planet at sea level.
         */
        public double R;

        /*
         * Creates a new PlanetViewController.
         *
         * @param node a SceneNode representing a camera position and orientation
         *      in the scene.
         * @param R the planet radius at sea level.
         */
        public PlanetViewController(SceneNode node, double R) : base(node, 6.0 * R)
        {
            this.R = R;
        }

        public override double getHeight()
        {
            return Position.Length - R;
        }

        public override void move(Vector3d oldp, Vector3d p)
        {
            oldp.Normalize();
            p.Normalize();
            double oldlat = Math.Asin(oldp.Z);
            double oldlon = Math.Atan2(oldp.Y, oldp.X);
            double lat = Math.Asin(p.Z);
            double lon = Math.Atan2(p.Y, p.X);
            X0 -= lon - oldlon;
            Y0 -= lat - oldlat;
            Y0 = Math.Max(-Math.PI / 2.0, Math.Min(Math.PI / 2.0, Y0));
        }

        public override void moveForward(double distance)
        {
            double co = Math.Cos(X0); // x0 = longitude
            double so = Math.Sin(X0);
            double ca = Math.Cos(Y0); // y0 = latitude
            double sa = Math.Sin(Y0);
            Vector3d po = new Vector3d(co * ca, so * ca, sa) * R;
            Vector3d px = new Vector3d(-so, co, 0.0);
            Vector3d py = new Vector3d(-co * sa, -so * sa, ca);
            Vector3d pd = (po - px * Math.Sin(Phi) * distance + py * Math.Cos(Phi) * distance);
            pd.Normalize();
            X0 = Math.Atan2(pd.Y, pd.X);
            Y0 = Math.Asin(pd.Z);
        }

        public override void turn(double angle)
        {
            double co = Math.Cos(X0); // x0 = longitude
            double so = Math.Sin(X0);
            double ca = Math.Cos(Y0); // y0 = latitude
            double sa = Math.Sin(Y0);
            double l = Distance * Math.Sin(Theta);
            Vector3d po = new Vector3d(co * ca, so * ca, sa) * R;
            Vector3d px = new Vector3d(-so, co, 0.0);
            Vector3d py = new Vector3d(-co * sa, -so * sa, ca);
            Vector3d f = -px * Math.Sin(Phi) + py * Math.Cos(Phi);
            Vector3d r = px * Math.Cos(Phi) + py * Math.Sin(Phi);
            Vector3d pd = (po + f * (Math.Cos(angle) - 1.0) * l - r * Math.Sin(angle) * l);
            pd.Normalize();
            X0 = Math.Atan2(pd.Y, pd.X);
            Y0 = Math.Asin(pd.Z);
            Phi += angle;
        }

        public override double interpolate(double sx0, double sy0, double stheta, double sphi, double sd,
                double dx0, double dy0, double dtheta, double dphi, double dd, double t)
        {
            Vector3d s = new Vector3d(Math.Cos(sx0) * Math.Cos(sy0), Math.Sin(sx0) * Math.Cos(sy0), Math.Sin(sy0));
            Vector3d e = new Vector3d(Math.Cos(dx0) * Math.Cos(dy0), Math.Sin(dx0) * Math.Cos(dy0), Math.Sin(dy0));
            double dist = Math.Max(Math.Cos(Vector3d.Dot(s, e)) * R, 1e-3);

            t = Math.Min(t + Math.Min(0.1, 5000.0 / dist), 1.0);
            double T = 0.5 * Math.Atan(4.0 * (t - 0.5)) / Math.Atan(4.0 * 0.5) + 0.5;

            interpolateDirection(sx0, sy0, dx0, dy0, T, X0, Y0);
            interpolateDirection(sphi, stheta, dphi, dtheta, T, Phi, Theta);

            const double W = 10.0;
            Distance = sd * (1.0 - t) + dd * t + dist * (Math.Exp(-W * (t - 0.5) * (t - 0.5)) - Math.Exp(-W * 0.25));

            return t;
        }

        public virtual void interpolatePos(double sx0, double sy0, double dx0, double dy0, double t, double x0, double y0)
        {
            interpolateDirection(sx0, sy0, dx0, dy0, t, x0, y0);
        }

        public override void Update()
        {
            double co = Math.Cos(X0); // x0 = longitude
            double so = Math.Sin(X0);
            double ca = Math.Cos(Y0); // y0 = latitude
            double sa = Math.Sin(Y0);
            Vector3d po = new Vector3d(co * ca, so * ca, sa) * (R + groundHeight);
            Vector3d px = new Vector3d(-so, co, 0.0);
            Vector3d py = new Vector3d(-co * sa, -so * sa, ca);
            Vector3d pz = new Vector3d(co * ca, so * ca, sa);

            double ct = Math.Cos(Theta);
            double st = Math.Sin(Theta);
            double cp = Math.Cos(Phi);
            double sp = Math.Sin(Phi);
            Vector3d cx = px * cp + py * sp;
            Vector3d cy = -px * sp * ct + py * cp * ct + pz * st;
            Vector3d cz = px * sp * st - py * cp * st + pz * ct;
            Position = po + cz * Distance * Zoom;
#if TODO
            if (position.Length < R + 0.5 + groundHeight)
            {
                position = position.Normalize(R + 0.5 + groundHeight);
            }
#endif
            Matrix4d view = new Matrix4d(cx.X, cx.Y, cx.Z, 0.0,
                    cy.X, cy.Y, cy.Z, 0.0,
                    cz.X, cz.Y, cz.Z, 0.0,
                    0.0, 0.0, 0.0, 1.0);
            //TOSEE view = view * Matrix4d.Translate(-position);
            Matrix4d.Translate(view, -Position.X, -Position.Y, -Position.Z);
            view.Invert();
            node.setLocalToParent(view);
        }
    }
}
