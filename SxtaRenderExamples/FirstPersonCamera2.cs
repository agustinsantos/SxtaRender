using Sxta.Math;
using System;

namespace Examples.Tutorials
{
    public enum CameraMode
    {
        Orthographic,
        Perspective
    }

    /// <summary>
    /// Class for encapsulating a view frustum camera, capable of either orthographic or perspective
    /// projections. Both view and projection matrices can be retrieved from a Camera object at
    /// any time, which reflect the current state of the Camera.
    /// 
    /// Default values for a Camera object in world coordinates include:
    /// position, located at world coordinate origin(0, 0, 0).
    /// left direction, aligned with negative x-axis(-1, 0, 0).
    /// up direction, aligned with y-axis(0, 1, 0).
    /// forward direction, aligned with negative z-axis(0, 0, -1).
    /// </summary>
    public class FirstPersonCamera2
    {
        public FirstPersonCamera2(OpenTK.GameWindow gw)
        {
            parentWindow = gw;
            CameraMode = CameraMode.Perspective;
            recalcViewMatrix = true;
            rotationHitCount = 0;
            InitLocalCoordinateSystem();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        public void LookAt(Vector3f center)
        {
            this.LookAt(center.X, center.Y, center.Z);
        }
        public void LookAt(float centerX, float centerY, float centerZ)
        {
            f.X = centerX - eyePosition.X;
            f.Y = centerY - eyePosition.Y;
            f.X = centerZ - eyePosition.Z;
            f.Normalize();

            // XXX (Dustin) It's possible that f = u here.

            // The following projects u onto the plane defined by the point eyePosition,
            // and the normal f. The goal is to rotate u so that it is orthogonal to f,
            // while attempting to keep u's orientation close to its previous direction.
            {
                // Borrow l vector for calculation, so we don't have to allocate a
                // new vector.
                l = eyePosition + u;

                float t = -1.0f * Vector3f.Dot(f, u);

                // Move point l in the normal direction, f, by t units so that it is
                // on the plane.
                l.X += t * f.X;
                l.Y += t * f.Y;
                l.Z += t * f.Z;

                u = l - eyePosition;
                u.Normalize();
            }

            // Update l vector given new f and u vectors.
            l = Vector3f.Cross(u, f);

            // If f and u are no longer orthogonal, make them so.
            if (Vector3f.Dot(f, u) > 1e-7f)
            {
                u = Vector3f.Cross(f, l);
            }

            Matrix3f m = new Matrix3f();
            Vector3f c0 = -1.0f * l; // Camera's local x axis
            Vector3f c1 = u;         // Camera's local y axis
            Vector3f c2 = -1.0f * f; // Camera's local z axis
            m.R0C0 = c0.X; m.R1C0 = c0.Y; m.R2C0 = c0.Z;
            m.R0C1 = c1.X; m.R1C1 = c1.Y; m.R2C1 = c1.Z;
            m.R0C2 = c2.X; m.R1C2 = c2.Y; m.R2C2 = c2.Z;
            orientation = Quaternion.FromRotateMatrix(m);

            RegisterRotation();
            recalcViewMatrix = true;
        }

        public void LookAt(Vector3f eye,
                    Vector3f center,
                    Vector3f up)
        {
            eyePosition = eye;

            // Orient Camera basis vectors.
            Vector3f.Subtract(ref center, ref eye, out f); f.Normalize();
            l = Vector3f.Cross(up, f); l.Normalize();
            u = Vector3f.Cross(f, l);

            // Compute orientation from 3x3 change of basis matrix whose columns are the
            // world basis vectors given in Camera space coordinates.
            Matrix3f m = new Matrix3f();
            Vector3f c0 = -1.0f * l; // first column, representing new x-axis orientation
            Vector3f c1 = u;         // second column, representing new y-axis orientation
            Vector3f c2 = -1.0f * f; // third column, representing new z-axis orientation
            m.R0C0 = c0.X; m.R1C0 = c0.Y; m.R2C0 = c0.Z;
            m.R0C1 = c1.X; m.R1C1 = c1.Y; m.R2C1 = c1.Z;
            m.R0C2 = c2.X; m.R1C2 = c2.Y; m.R2C2 = c2.Z;
            orientation = Quaternion.FromRotateMatrix(m);

            RegisterRotation();
            recalcViewMatrix = true;
        }

        /// <summary>
        /// Rotates Camera about its local negative z-axis (forward direction)
        /// by angle radians.
        /// Rotation is counter-clockwise if  angle > 0, and clockwise if
        ///  angle is < 0.
        /// </summary>
        /// <param name="angle">rotation angle in radians.</param>
        public void Roll(float angle)
        {
            Quaternion q = new Quaternion(f, angle);

            u = Vector3f.Transform(u, q);
            l = Vector3f.Transform(l, q);

            orientation = q * orientation;

            RegisterRotation();
            recalcViewMatrix = true;
        }

        /// <summary>
        /// Rotates Camera about its local x (right direction) axis by angle
        /// radians.
        /// 
        /// Rotation is counter-clockwise if angle > 0, and clockwise if
        /// angle is < 0.
        /// </summary>
        /// <param name="angle">rotation angle in radians.</param>
        public void Pitch(float angle)
        {
            Quaternion q = new Quaternion(-l, angle);

            u = Vector3f.Transform(u, q);
            f = Vector3f.Transform(f, q);

            orientation = q * orientation;

            RegisterRotation();
            recalcViewMatrix = true;
        }

        /// <summary>
        /// Rotates Camera about its local y (up direction) axis by angle
        /// radians.
        /// 
        /// Rotation is counter-clockwise if angle > 0, and clockwise if
        /// angle is < 0.
        /// </summary>
        /// <param name="angle">rotation angle in radians.</param>
        public void Yaw(float angle)
        {
            Quaternion q = new Quaternion(u, angle);

            l = Vector3f.Transform(l, q);
            f = Vector3f.Transform(f, q);

            orientation = q * orientation;

            RegisterRotation();
            recalcViewMatrix = true;
        }

        /// <summary>
        /// Rotates Camera by angle radians about axis whose components are expressed using
        /// the Camera's local coordinate system.
        /// 
        /// Counter-clockwise rotation for angle > 0, and clockwise rotation otherwise.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="axis"></param>
        public void Rotate(float angle, Vector3f axis)
        {
            Vector3f n = axis;
            n.Normalize();
            Quaternion q = new Quaternion(n, angle);

            l = Vector3f.Transform(l, q);
            u = Vector3f.Transform(u, q);
            f = Vector3f.Transform(f, q);

            orientation = q * orientation;

            RegisterRotation();
            recalcViewMatrix = true;
        }

        /// <summary>
        /// Translates the Camera with respect to the world coordinate system.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Translate(float x, float y, float z)
        {
            eyePosition.X += x;
            eyePosition.Y += y;
            eyePosition.Z += z;

            recalcViewMatrix = true;
        }

        /// <summary>
        /// Translates the Camera with respect to the world coordinate system.
        /// </summary>
        /// <param name="v"></param>
        public void Translate(Vector3f v)
        {
            eyePosition += v;

            recalcViewMatrix = true;
        }

        /// <summary>
        /// Translates the Camera relative to its locate coordinate system.
        /// </summary>
        /// <param name="left">translation along the Camera's left direction.</param>
        /// <param name="up">translation along the Camera's up direction.</param>
        /// <param name="forward">translation along the Camera's forward direction.</param>
        public void TranslateLocal(float left, float up, float forward)
        {
            eyePosition += left * l;
            eyePosition += up * u;
            eyePosition += forward * f;

            recalcViewMatrix = true;
        }

        public void TranslateLocal(Vector3f v)
        { TranslateLocal(v.X, v.Y, v.Z); }

        public void Update(float time)
        {
            var move = moveSpeed * time;

            // get current mouse position
            currentMouseState = OpenTK.Input.Mouse.GetState();
            // warp mouse to the center (not really). so that the cursor 
            // would not go out of bounds.
            OpenTK.Input.Mouse.SetPosition(parentWindow.Width / 2, parentWindow.Height / 2);

            // check if the mouse moved. if it is, move the camera accordingly
            if (currentMouseState.X != previousMouseState.X)
            {
                mouseMove.Y = (currentMouseState.X - previousMouseState.X) *
                    mouseSensitivity * time;
            }
            else
            {
                mouseMove.Y = 0;
            }

            if (currentMouseState.Y != previousMouseState.Y)
            {
                mouseMove.X = (currentMouseState.Y - previousMouseState.Y) *
                    mouseSensitivity * time;
            }
            else
            {
                mouseMove.X = 0;
            }
            // save current mouse position 
            previousMouseState = currentMouseState;

            // move the camera 
            this.Pitch(mouseMove.X);
            this.Yaw(-mouseMove.Y);

            // camera movement by keyboard
            if (parentWindow.Keyboard[OpenTK.Input.Key.A])
            {
                this.MoveX(move);
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.D])
            {
                this.MoveX(-move);
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.Space])
            {
                this.MoveY(move);
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.ControlLeft])
            {
                this.MoveY(-move);
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.W])
            {
                this.MoveZ(move);
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.S])
            {
                this.MoveZ(-move);
            }
        }


        public void Resize(int width, int height, float fov = MathHelper.PiOver4)
        {
            if (CameraMode == CameraMode.Perspective)
                ProjectionMatrix = Matrix4f.CreatePerspectiveFieldOfView(fov, (float)width / (float)height, 0.1f, 1000f);
            else
                ProjectionMatrix = Matrix4f.CreateOrthographic((float)width, (float)height, 0.1f, 1000f);

        }

        public void MoveX(float a)
        {
            eyePosition.X += a;

            recalcViewMatrix = true;
        }

        public void MoveY(float a)
        {
            eyePosition.Y += a;

            recalcViewMatrix = true;
        }
        public void MoveZ(float a)
        {
            eyePosition.Z += a;

            recalcViewMatrix = true;
        }

        public void MoveYLocal(float a)
        {
            eyePosition += a * u;

            recalcViewMatrix = true;
        }

        public void MoveZLocal(float a)
        {
            eyePosition += a * f;

            recalcViewMatrix = true;
        }

        /// <summary>
        /// Location of camera in world coordinates.
        /// </summary>
        public Vector3f EyePosition
        {
            get { return eyePosition; }
            set { eyePosition = value; recalcViewMatrix = true; }
        }

        /// <summary>
        /// return the Camera's left direction vector given in world space coordinates.
        /// </summary>
        public Vector3f LeftPosition { get { return l; } }

        /// <summary>
        /// return the Camera's up direction vector given in world space coordinates.
        /// </summary>
        public Vector3f UpPosition { get { return u; } }

        /// <summary>
        /// return the Camera's forward  direction vector given in world space coordinates.
        /// </summary>
        public Vector3f ForwardPosition { get { return f; } }

        /// <summary>
        /// Orientation of camera basis vectors specified in world coordinates.
        /// Orientation of the Camera is given by a float quaternion of the form
        /// \c(cos(theta/2), sin(theta/2) * u), where the axis of rotation u is given in
        /// world space coordinates.
        /// </summary>
        public Quaternion Orientation { get { return orientation; } }

        /// <summary>
        /// return a 4x4 view matrix representing the 'Camera' object's view transformation.
        /// </summary>
        public Matrix4f ViewMatrix
        {
            get
            {
                if (recalcViewMatrix)
                {
                    // Compute inverse rotation q
                    Quaternion q = orientation;
                    q.X *= -1.0f;
                    q.Y *= -1.0f;
                    q.Z *= -1.0f;
                    viewMatrix = CastToMatrix(q);

                    // Translate by inverse eyePosition.
                    Vector3f v = -eyePosition;
                    Matrix4f m = viewMatrix;
                    Vector4f aux = (m.Column0 * v.X) + (m.Column1 * v.Y) + (m.Column2 * v.Z) + m.Column3;
                    viewMatrix.M31 = aux.X;
                    viewMatrix.M32 = aux.Y;
                    viewMatrix.M33 = aux.Z;
                    viewMatrix.M34 = aux.W;

                    recalcViewMatrix = false;
                }

                return viewMatrix;
            }
        }
        public Matrix4f ProjectionMatrix { get; set; }

        public CameraMode CameraMode { get; set; }
        #region Private 

        private readonly OpenTK.GameWindow parentWindow;
        private Vector2f mouseMove;
        private float mouseSensitivity = 0.1f;
        private float moveSpeed = 0.1f;

        private OpenTK.Input.MouseState currentMouseState;
        private OpenTK.Input.MouseState previousMouseState;

        /// <summary>
        /// Location of camera in world coordinates.
        /// </summary>
        private Vector3f eyePosition;

        /// <summary>
        /// Orientation of camera basis vectors specified in world coordinates.
        /// </summary>
        private Quaternion orientation;

        private Vector3f l; // Camera's left direction vector, given in world coordinates.
        private Vector3f u; // Camera's up direction vector, given in world coordinates.
        private Vector3f f; // Camera's forward direction vector, given in world coordinates.

        private bool recalcViewMatrix;
        private Matrix4f viewMatrix;

        private ushort rotationHitCount;

        // Normalize Camera vectors after rotating this many times.
        private const ushort rotationHitCountMax = 1000;

        private void InitLocalCoordinateSystem()
        {
            EyePosition = new Vector3f(0, 0, 0);
            l = new Vector3f(-1, 0, 0);
            u = new Vector3f(0, 1, 0);
            f = new Vector3f(0, 0, -1);
        }

        private void RegisterRotation()
        {
            rotationHitCount++;

            if (rotationHitCount > rotationHitCountMax)
            {
                rotationHitCount = 0;
                NormalizeCamera();
            }
        }

        private void NormalizeCamera()
        {
            l.Normalize();
            u.Normalize();
            f.Normalize();
            orientation.Normalize(); ;

            // Assuming forward 'f' is correct
            l = Vector3f.Cross(u, f);
            u = Vector3f.Cross(f, l);
        }

        public Matrix4f CastToMatrix(Quaternion quaternion)
        {
            quaternion.Normalize();

            double xx = quaternion.X * quaternion.X;
            double yy = quaternion.Y * quaternion.Y;
            double zz = quaternion.Z * quaternion.Z;
            double xy = quaternion.X * quaternion.Y;
            double xz = quaternion.X * quaternion.Z;
            double yz = quaternion.Y * quaternion.Z;
            double wx = quaternion.W * quaternion.X;
            double wy = quaternion.W * quaternion.Y;
            double wz = quaternion.W * quaternion.Z;


            float R0C0 = (float)(1 - 2 * (yy + zz));
            float R0C1 = (float)(2 * (xy - wz));
            float R0C2 = (float)(2 * (xz + wy));

            float R1C0 = (float)(2 * (xy + wz));
            float R1C1 = (float)(1 - 2 * (xx + zz));
            float R1C2 = (float)(2 * (yz - wx));

            float R2C0 = (float)(2 * (xz - wy));
            float R2C1 = (float)(2 * (yz + wx));
            float R2C2 = (float)(1 - 2 * (xx + yy));

            Matrix4f rst = new Matrix4f(R0C0, R0C1, R0C2, 0,
                                        R1C0, R1C1, R1C2, 0,
                                        R2C0, R2C1, R2C2, 0,
                                           0, 0, 0, 1);
            return rst;
        }
        #endregion
    }

}
