using Sxta.Math;

namespace Sxta.Render.Scenegraph.Controller
{
    public interface ViewController
    {
        /// <summary>
        /// The field of view angle.
        /// </summary>
        double Fov { get; set; }

        /// <summary>
        /// The x coordinate of the point the camera is looking at on the ground.
        /// </summary>
        double X0 { get; set; }

        /// <summary>
        /// The y coordinate of the point the camera is looking at on the ground.
        /// </summary>
        double Y0 { get; set; }

        /// <summary>
        /// The zenith angle of the vector between the "look at" point and the camera.
        /// </summary>
        double Theta { get; set; }

        /// <summary>
        /// The azimuth angle of the vector between the "look at" point and the camera.
        /// </summary>
        double Phi { get; set; }

        /// <summary>
        /// The distance between the "look at" point and the camera.
        /// </summary>
        double Distance { get; set; }

        /// <summary>
        /// Zoom factor (realized by increasing d and decreasing fov).
        /// </summary>
        double Zoom { get; set; }

        /// <summary>
        /// The camera position in world space resulting from the x0,y0,theta,phi,
        /// and d parameters.
        /// </summary>
        Vector3d Position { get; set; }

        void Update();
        void setProjection(Vector4f viewport, float znear = 0.0f, float zfar = 0.0f);

        /// <summary>
        /// Gets/Sets the terrain elevation below the camera. This elevation is used
        /// to adjust the camera position so that it is not below the ground.
        /// </summary>
        float GroundHeight { get; set; }

        void move(Vector3d oldp, Vector3d p);
        double interpolate(double x01, double y01, double theta1, double phi1, double d1, double x02, double y02, double theta2, double phi2, double d2, double animation);
        double getHeight();
        void moveForward(double v);
        void turn(double v);
        void interpolatePos(double x01, double y01, double x02, double y02, double lerp, out double x03, out double y03);
    }
}