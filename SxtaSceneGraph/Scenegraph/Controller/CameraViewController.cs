using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sxta.Math;

namespace Sxta.Render.Scenegraph.Controller
{
    public class CameraViewController : ViewController
    {
        /// <summary>
        /// The field of view angle.
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

        public float  GroundHeight { get; set; }

        public double getHeight()
        {
            throw new NotImplementedException();
        }

        public double interpolate(double x01, double y01, double theta1, double phi1, double d1, double x02, double y02, double theta2, double phi2, double d2, double animation)
        {
            throw new NotImplementedException();
        }

        public void interpolatePos(double x01, double y01, double x02, double y02, double lerp, out double x03, out double y03)
        {
            throw new NotImplementedException();
        }

        public void move(Vector3d oldp, Vector3d p)
        {
            throw new NotImplementedException();
        }

        public void moveForward(double v)
        {
            throw new NotImplementedException();
        }

        public void setGroundHeight(float gh)
        {
            throw new NotImplementedException();
        }

 

        public void turn(double v)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public void setProjection(Vector4f viewport, float znear = 0, float zfar = 0)
        {
            throw new NotImplementedException();
        }
    }
}
