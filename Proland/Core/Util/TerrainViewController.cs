using Sxta.Math;
using Sxta.Render.Scenegraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proland
{
    public class TerrainViewController
    {

        /**
         * The field of view angle.
         */
        public double fov;

        /**
         * The x coordinate of the point the camera is looking at on the ground.
         */
        public double x0;

        /**
         * The y coordinate of the point the camera is looking at on the ground.
         */
        public double y0;

        /**
         * The zenith angle of the vector between the "look at" point and the camera.
         */
        public double theta;

        /**
         * The azimuth angle of the vector between the "look at" point and the camera.
         */
        public double phi;

        /**
         * The distance between the "look at" point and the camera.
         */
        public double d;

        /**
         * Zoom factor (realized by increasing d and decreasing fov).
         */
        public double zoom;

        /**
         * The camera position in world space resulting from the x0,y0,theta,phi,
         * and d parameters.
         */
        public Vector3d position;

        /**
         * Creates a new TerrainViewController to control the given SceneNode.
         *
         * @param node a SceneNode representing a camera position and orientation
         *      in the scene.
         * @param d0 the initial valued of the #d distance.
         */
        public TerrainViewController(SceneNode node, double d0)
        {
            fov= 80.0;
            x0 = 0.0;
            y0 = 0.0;
            theta = 0.0;
            phi = 0.0;
            d = d0;
            zoom = 1.0;
            this.node = node;
            groundHeight = 0;
        }

        /**
         * Returns the SceneNode associated with this TerrainViewController.
         * This SceneNode represents a camera position and orientation in the
         * scene.
         */
        public SceneNode getNode()
        {
            return node;
        }

        /**
         * Sets the SceneNode associated with this TerrainViewController.
         *
         * @param node a SceneNode representing a camera position and orientation
         *      in the scene.
         */
        public void setNode(SceneNode node)
        {
            this.node = node;
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

        /**
         * Returns the height of the camera above the z=0 surface.
         */
        public virtual double getHeight()
        {
            return position.Z;
        }

        /**
         * Moves the "look at" point so that "oldp" appears at the position of "p"
         * on screen.
         *
         * @param oldp a %terrain point.
         * @param p another %terrain point.
         */
        public virtual void move(Vector3d oldp, Vector3d p)
        {
            x0 -= p.X - oldp.X;
            y0 -= p.Y - oldp.Y;
        }

        public virtual void moveForward(double distance)
        {
            x0 -= Math.Sin(phi) * distance;
            y0 += Math.Cos(phi) * distance;
        }

        public virtual void turn(double angle)
        {
            double l = d * Math.Sin(theta);
            x0 -= (Math.Sin(phi) * (Math.Cos(angle) - 1.0) + Math.Cos(phi) * Math.Sin(angle)) * l;
            y0 += (Math.Cos(phi) * (Math.Cos(angle) - 1.0) - Math.Sin(phi) * Math.Sin(angle)) * l;
            phi += angle;
        }

        /**
         * Sets the position as the interpolation of the two given positions with
         * the interpolation parameter t (between 0 and 1). The source position is
         * sx0,sy0,stheta,sphi,sd, the destination is dx0,dy0,dtheta,dphi,dd.
         *
         * @return the new value of the interpolation parameter t.
         */
        public virtual double interpolate(double sx0, double sy0, double stheta, double sphi, double sd,
                double dx0, double dy0, double dtheta, double dphi, double dd, double t)
        {
            x0 = dx0;
            y0 = dy0;
            theta = dtheta;
            phi = dphi;
            d = dd;
            return 1.0;
        }

        public virtual void interpolatePos(double sx0, double sy0, double dx0, double dy0, double t, double x0, double y0)
        {
            x0 = sx0 * (1.0 - t) + dx0 * t;
            y0 = sy0 * (1.0 - t) + dy0 * t;
        }

        /**
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
            lat = Math.Asin(v.Z);
            lon = Math.Atan2(v.Y, v.X);
        }

        /**
         * Sets the localToParent transform of the SceneNode associated with this
         * TerrainViewController. The transform is computed from the view parameters
         * x0,y0,theta,phi and d.
         */
        public virtual void update()
        {
            Vector3d po = new Vector3d(x0, y0, groundHeight);
            Vector3d px = new Vector3d(1.0, 0.0, 0.0);
            Vector3d py = new Vector3d(0.0, 1.0, 0.0);
            Vector3d pz = new Vector3d(0.0, 0.0, 1.0);

            double ct = Math.Cos(theta);
            double st = Math.Sin(theta);
            double cp = Math.Cos(phi);
            double sp = Math.Sin(phi);
            Vector3d cx = px * cp + py * sp;
            Vector3d cy = -px * sp * ct + py * cp * ct + pz * st;
            Vector3d cz = px * sp * st - py * cp * st + pz * ct;
            position = po + cz * d * zoom;

            if (position.Z < groundHeight + 1.0)
            {
                position.Z = groundHeight + 1.0;
            }

            Matrix4d view = new Matrix4d(cx.X, cx.Y, cx.Z, 0.0,
                    cy.X, cy.Y, cy.Z, 0.0,
                    cz.X, cz.Y, cz.Z, 0.0,
                    0.0, 0.0, 0.0, 1.0);
            //TOSEE view = view * Matrix4d.Translate(-position);
            Matrix4d.Translate(view, -position.X, -position.Y, -position.Z);
            view.Invert();
            node.setLocalToParent(view);
        }

        /**
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
            viewport = new Vector4f(-1.0f, 1.0f, -1.0f, 1.0f);
            Vector4i vp = SceneManager.getCurrentFrameBuffer().getViewport();
            float width = (float)vp.Z;
            float height = (float)vp.W;
            float vfov = (float)((2 * Math.Atan(height / width * Math.Tan((fov / 2) * (Math.PI / 180.0)))) * (180.0 / Math.PI));

            float h = (float)(getHeight() - TerrainNode.groundHeightAtCamera);
            if (znear == 0.0f)
            {
                znear = 0.1f * h;
            }
            if (zfar == 0.0f)
            {
                zfar = 1e6f * h;
            }

            if (zoom > 1.0)
            {
                vfov = (float)((2 * Math.Atan(height / width * Math.Tan((fov / 2) * (Math.PI / 180.0)) / zoom)) * (180.0 / Math.PI));
                znear = (float)(d * zoom * Math.Max(1.0 - 10.0 * Math.Tan((fov / 2) * (Math.PI / 180.0)) / zoom, 0.1));
                zfar = (float)(d * zoom * Math.Min(1.0 + 10.0 * Math.Tan((fov / 2)* (Math.PI / 180.0)) / zoom, 10.0));
            }

            //TOSEEMatrix4d clip = Matrix4d.orthoProjection(viewport.Y, viewport.X, viewport.W, viewport.Z, 1.0f, -1.0f);
            //TOSEEMatrix4d cameraToScreen = Matrix4d.perspectiveProjection(vfov, width / height, znear, zfar);
            //TOSEEnode.getOwner().setCameraToScreen(clip * cameraToScreen);
        }


        /**
         * The SceneNode associated with this TerrainViewController.
         */
        protected SceneNode node;

        /**
         * The %terrain elevation below the camera.
         */
        protected float groundHeight;
    }
}