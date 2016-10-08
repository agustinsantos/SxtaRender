using Sxta.Math;
using Sxta.Render.Scenegraph;
using Sxta.Render.Scenegraph.Controller;
using System;

namespace proland
{
    public class TerrainViewController : ViewController
    {
        /// <summary>
        /// The field of view angle (degrees).
        /// </summary>
        public double Fov { get; set; }

        /// <summary>
        /// The x coordinate of the point the camera is looking at on the ground.
        /// </summary>
        public double X0 { get; set; }

        /// <summary>
        /// The y coordinate of the point the camera is looking at on the ground.
        /// </summary>
        public double Y0 { get; set; }

        /// <summary>
        /// The zenith angle of the vector between the "look at" point and the camera.
        /// </summary>
        public double Theta { get; set; }

        /// <summary>
        /// The azimuth angle of the vector between the "look at" point and the camera.
        /// </summary>
        public double Phi { get; set; }

        /// <summary>
        /// The distance between the "look at" point and the camera.
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// Zoom factor (realized by increasing d and decreasing fov).
        /// </summary>
        public double Zoom { get; set; }

        /// <summary>
        /// The camera position in world space resulting from the x0,y0,theta,phi,
        /// and d parameters.
        /// </summary>
        public Vector3d Position { get { return position; } set { position = value; } }
        protected Vector3d position;


        /// <summary>
        /// Creates a new TerrainViewController to control the given SceneNode.
        /// </summary>
        /// <param name="node">a SceneNode representing a camera position and orientation in the scene.</param>
        /// <param name="d0">the initial valued of the distance.</param>
        public TerrainViewController(SceneNode node, double d0)
        {
            Fov = 80.0;
            X0 = 0.0;
            Y0 = 0.0;
            Theta = 0.0;
            Phi = 0.0;
            Distance = d0;
            Zoom = 1.0;
            this.node = node;
            groundHeight = 0;
        }

        /// <summary>
        /// Returns the SceneNode associated with this TerrainViewController.
        /// This SceneNode represents a camera position and orientation in the
        /// scene.
        /// </summary>
        /// <returns></returns>
        public SceneNode getNode()
        {
            return node;
        }

        /*
         * Sets the SceneNode associated with this TerrainViewController.
         *
         * @param node a SceneNode representing a camera position and orientation
         *      in the scene.
         */
        public void setNode(SceneNode node)
        {
            this.node = node;
        }


        /// <summary>
        /// Gets/Sets the terrain elevation below the camera. This elevation is used
        /// to adjust the camera position so that it is not below the ground.
        /// </summary>
        public float GroundHeight
        {
            get
            {
                return groundHeight;
            }
            set { this.groundHeight = value; }
        }
        
        /**
         * Returns the %terrain elevation below the camera.
         */
        public float getGroundHeight()
        {
            return groundHeight;
        }
        /**
         * Sets the %terrain elevation below the camera. This elevation is used
         * to adjust the camera position so that it is not below the ground.
         *
         * @param groundHeight the %terrain elevation below the camera.
         */
        public void setGroundHeight(float groundHeight)
        {
            this.groundHeight = groundHeight;
        }

        /*
         * Returns the height of the camera above the z=0 surface.
         */
        public virtual double getHeight()
        {
            return Position.Z;
        }

        /*
         * Moves the "look at" point so that "oldp" appears at the position of "p"
         * on screen.
         *
         * @param oldp a %terrain point.
         * @param p another %terrain point.
         */
        public virtual void move(Vector3d oldp, Vector3d p)
        {
            X0 -= p.X - oldp.X;
            Y0 -= p.Y - oldp.Y;
        }

        public virtual void moveForward(double distance)
        {
            X0 -= Math.Sin(Phi) * distance;
            Y0 += Math.Cos(Phi) * distance;
        }

        public virtual void turn(double angle)
        {
            double l = Distance * Math.Sin(Theta);
            X0 -= (Math.Sin(Phi) * (Math.Cos(angle) - 1.0) + Math.Cos(Phi) * Math.Sin(angle)) * l;
            Y0 += (Math.Cos(Phi) * (Math.Cos(angle) - 1.0) - Math.Sin(Phi) * Math.Sin(angle)) * l;
            Phi += angle;
        }

        /*
         * Sets the position as the interpolation of the two given positions with
         * the interpolation parameter t (between 0 and 1). The source position is
         * sx0,sy0,stheta,sphi,sd, the destination is dx0,dy0,dtheta,dphi,dd.
         *
         * @return the new value of the interpolation parameter t.
         */
        public virtual double interpolate(double sx0, double sy0, double stheta, double sphi, double sd,
                double dx0, double dy0, double dtheta, double dphi, double dd, double t)
        {
            X0 = dx0;
            Y0 = dy0;
            Theta = dtheta;
            Phi = dphi;
            Distance = dd;
            return 1.0;
        }

        public virtual void interpolatePos(double sx0, double sy0, double dx0, double dy0, double t, out double x0, out double y0)
        {
            x0 = sx0 * (1.0 - t) + dx0 * t;
            y0 = sy0 * (1.0 - t) + dy0 * t;
        }

        /*
         * Returns a direction interpolated between the two given direction.
         *
         * @param slon start longitude.
         * @param slat start latitude.
         * @param elon end longitude.
         * @param elat end latitude.
         * @param t interpolation parameter between 0 and 1.
         * @param[out] lon interpolated longitude.
         * @param[out] lat interpolated latitude.
         */
        public void interpolateDirection(double slon, double slat, double elon, double elat, double t, double lon, double lat)
        {
            Vector3d s = new Vector3d(Math.Cos(slon) * Math.Cos(slat), Math.Sin(slon) * Math.Cos(slat), Math.Sin(slat));
            Vector3d e = new Vector3d(Math.Cos(elon) * Math.Cos(elat), Math.Sin(elon) * Math.Cos(elat), Math.Sin(elat));
            Vector3d v = (s * (1.0 - t) + e * t);
            v.Normalize();
            //safe_asin implementation
            if (v.Z <= -1)
            {
                v.Z = -1;
            }
            else if (v.Z >= 1)
            {
                v.Z = 1;
            }
            lat = Math.Asin(v.Z);
            lon = Math.Atan2(v.Y, v.X);
        }

        /// <summary>
        /// Sets the localToParent transform of the SceneNode associated with this
        /// TerrainViewController. The transform is computed from the view parameters
        /// x0,y0,theta,phi and d.
        /// </summary>
        public virtual void Update()
        {
            Vector3d po = new Vector3d(X0, Y0, groundHeight);
            Vector3d px = new Vector3d(1.0, 0.0, 0.0);
            Vector3d py = new Vector3d(0.0, 1.0, 0.0);
            Vector3d pz = new Vector3d(0.0, 0.0, 1.0);

            double ct = Math.Cos(Theta);
            double st = Math.Sin(Theta);
            double cp = Math.Cos(Phi);
            double sp = Math.Sin(Phi);
            Vector3d cx = px * cp + py * sp;
            Vector3d cy = -px * sp * ct + py * cp * ct + pz * st;
            Vector3d cz = px * sp * st - py * cp * st + pz * ct;
            position = po + cz * Distance * Zoom;

            if (position.Z < groundHeight + 1.0)
            {
                position.Z = groundHeight + 1.0;
            }

            Matrix4d view = new Matrix4d(cx.X, cx.Y, cx.Z, 0.0,
                                        cy.X, cy.Y, cy.Z, 0.0,
                                        cz.X, cz.Y, cz.Z, 0.0,
                                        0.0, 0.0, 0.0, 1.0);
            view = Matrix4d.Translate(view, -Position.X, -Position.Y, -Position.Z);
            view.Invert();
            node.setLocalToParent(view);
        }

        /*
         * Sets the camera to screen perspective projection.
         *
         * @param znear an optional znear plane (0.0 means that a default value
         *      must be used).
         * @param zfar an optional zfar plane (0.0 means that a default value
         *      must be used).
         * @param viewport an optional viewport to select a part of the image.
         *      The default value [-1:1]x[-1:1] selects the whole image.
         */

        public virtual void setProjection(Vector4f viewport, float znear = 0.0f, float zfar = 0.0f)
        {
            if (viewport == Vector4f.Zero)
            {
                viewport = new Vector4f(-1.0f, 1.0f, -1.0f, 1.0f);
            }
            Vector4i vp = SceneManager.getCurrentFrameBuffer().getViewport();
            float width = vp.Z;
            float height = vp.W;
            float vfov = (float)((2 * Math.Atan(height / width * Math.Tan((Fov / 2) * (Math.PI / 180.0)))));

            float h = (float)(getHeight() - TerrainNode.groundHeightAtCamera);
            if (znear == 0.0f)
            {
                znear = 0.1f * h;
            }
            if (zfar == 0.0f)
            {
                zfar = 1e6f * h;
            }

            if (Zoom > 1.0)
            {
                vfov = (float)((2 * Math.Atan(height / width * Math.Tan((Fov / 2) * (Math.PI / 180.0)) / Zoom)));
                znear = (float)(Distance * Zoom * Math.Max(1.0 - 10.0 * Math.Tan((Fov / 2) * (Math.PI / 180.0)) / Zoom, 0.1));
                zfar = (float)(Distance * Zoom * Math.Min(1.0 + 10.0 * Math.Tan((Fov / 2) * (Math.PI / 180.0)) / Zoom, 10.0));
            }
            // Matrix4d.CreateOrthographic AND Matrix4d.perspectiveProjection C++
            Matrix4d clip = new Matrix4d();
            Matrix4d.CreateOrthographicOffCenter(viewport.X, viewport.Y, viewport.Z, viewport.W, 1.0f, -1.0f, out clip);
            Matrix4d cameraToScreen = Matrix4d.CreatePerspectiveFieldOfView(vfov, ((double)width) / (double)height, znear, zfar);
            node.getOwner().setCameraToScreen(clip * cameraToScreen);

        }

        /*
         * The SceneNode associated with this TerrainViewController.
         */
        protected SceneNode node;

        /*
         * The %terrain elevation below the camera.
         */
        protected float groundHeight;
    }
}