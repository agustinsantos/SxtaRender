using Sxta.Math;
using System;

namespace Examples.Tutorials
{
    public class FirstPersonCamera
    {
        public Vector3f Position;
        public Vector3f Rotation;
        public Quaternion Orientation;

        public Matrix4f ViewMatrix { get; internal set; }
        public Matrix4f ProjectionMatrix { get; set; }

        private readonly OpenTK.GameWindow parentWindow;
        private Vector2f mouseMove;
        private float mouseSensitivity = 0.1f;
        private float moveSpeed = 0.1f;

        private OpenTK.Input.MouseState currentMouseState;
        private OpenTK.Input.MouseState previousMouseState;

        public FirstPersonCamera(OpenTK.GameWindow gw)
        {
            parentWindow = gw;

            ViewMatrix = Matrix4f.Identity;
            ProjectionMatrix = Matrix4f.Identity;
            Orientation = Quaternion.Identity;

            OpenTK.Input.Mouse.SetPosition(parentWindow.Width / 2, parentWindow.Height / 2);
            previousMouseState = OpenTK.Input.Mouse.GetState();
        }

        public void Update(float time)
        {
            var move = moveSpeed * time;

            // get current mouse position
            currentMouseState = OpenTK.Input.Mouse.GetState();
            // warp mouse to the center (not really). so that the cursor 
            // would not go out of bounds.
            //OpenTK.Input.Mouse.SetPosition(parentWindow.Width / 2, parentWindow.Height / 2);

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
            this.TurnX(mouseMove.X);
            this.TurnY(-mouseMove.Y);

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
            if (parentWindow.Keyboard[OpenTK.Input.Key.Right])
            {
                this.TurnX(1f);
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.Left])
            {
                this.TurnX(-1f);
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.Up])
            {
                this.TurnY(1f);
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.Down])
            {
                this.TurnY(-1f);
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.Space])
            {
                Console.WriteLine(Rotation);
            }

            this.Update();

        }

        public void Update()
        {
            Orientation = Quaternion.FromAxisAngle(Vector3f.UnitY, Rotation.Y) *
                          Quaternion.FromAxisAngle(Vector3f.UnitX, Rotation.X);

            var forward = Vector3f.Transform(Vector3f.UnitZ, Orientation);
            ViewMatrix = Matrix4f.LookAt(Position, Position + forward, Vector3f.UnitY);
        }

        public void Resize(int width, int height, float fov = MathHelper.PiOver4)
        {
            ProjectionMatrix = Matrix4f.CreatePerspectiveFieldOfView(fov, (float)width / (float)height, 0.1f, 1000f);
        }

        /// <summary>
        /// It is a convienence method for auto-setting the
        /// quaternion based on a direction and an up vector.It computes
        /// the rotation to transform the x-axis to point into 'direction'
        /// and the y-axis to 'up'
        /// </summary>
        /// <param name="direction">where to look at in terms of local coordinates</param>
        /// <param name="up">a vector indicating the local up direction.</param>
        public void LookAt(Vector3f direction, Vector3f up)
        {
            throw new NotImplementedException("Working in progress");
            /*
            Vector3f forward = new Vector3f();
            Vector3f.Subtract(ref direction, ref Position, out forward);
            forward.Normalize();
            ViewMatrix = Matrix4f.LookAt(Position, direction, up);

            float angle = (float)Vector3f.CalculateAngle(forward, up);
            Orientation = Quaternion.FromAxisAngle(forward, angle);
            Rotation.X = (float)Vector3f.CalculateAngle(forward, Vector3f.UnitX);
            Rotation.Y = (float)Math.PI;  //(float)Vector3f.CalculateAngle(forward, Vector3f.UnitY);
            Vector3f v;
            float an;
            Orientation.ToAxisAngle(out v, out an);
            Matrix4f tmp = Quaternion.ToMatrix4f(Orientation) * Matrix4f.CreateTranslation(-Position);

            float ax = (float)Vector3f.CalculateAngle(forward, Vector3f.UnitX);
            float ay = (float)Vector3f.CalculateAngle(forward, Vector3f.UnitY);
            float az = (float)Vector3f.CalculateAngle(forward, Vector3f.UnitZ);
        */
        }

        /*
        private void test()
        {
            Vector3f pos = new Vector3f(0, 0, 0);
            Vector3f at = new Vector3f(0, 0, 1);
            Vector3f up = new Vector3f(0, 1, 0);

            Matrix4f tmp1 = Matrix4f.LookAt(pos, at, up);

            Vector3f forward = new Vector3f();
            Vector3f.Subtract(ref at, ref pos, out forward);
            forward.Normalize();

            float dot = Vector3f.Dot(Vector3f.UnitY, up);
            float rotAngle = (float)Math.Acos(dot);
            Quaternion q = Quaternion.FromAxisAngle(forward, rotAngle);
            Matrix4f tmp2 = Quaternion.ToMatrix4f(q);

            Matrix3f tmp3 = new Matrix3f(tmp1.M11, tmp1.M12, tmp1.M13,
                                        tmp1.M21, tmp1.M22, tmp1.M23,
                                        tmp1.M31, tmp1.M32, tmp1.M33);
            Quaternion q2 = Quaternion.FromRotateMatrix(tmp3);
        }
        */

        public void LookAt(Vector3f position, Vector3f direction, Vector3f up)
        {
            Position = position;
            LookAt(direction, up);
        }

        public void TurnX(float a)
        {
            Rotation.X += a;
            Rotation.X = MathHelper.Clamp(Rotation.X, -1.57f, 1.57f);
        }

        public void TurnY(float a)
        {
            Rotation.Y += a;
            Rotation.Y = ClampCircular(Rotation.Y, 0, MathHelper.TwoPi);
        }

        public void MoveX(float a)
        {
            Position += Vector3f.Transform(Vector3f.UnitX * a, Quaternion.FromAxisAngle(Vector3f.UnitY, Rotation.Y));
        }

        public void MoveY(float a)
        {
            Position += Vector3f.Transform(Vector3f.UnitY * a, Quaternion.FromAxisAngle(Vector3f.UnitY, Rotation.Y));
        }

        public void MoveZ(float a)
        {
            Position += Vector3f.Transform(Vector3f.UnitZ * a, Quaternion.FromAxisAngle(Vector3f.UnitY, Rotation.Y));
        }

        public void MoveYLocal(float a)
        {
            Position += Vector3f.Transform(Vector3f.UnitY * a, Orientation);
        }

        public void MoveZLocal(float a)
        {
            Position += Vector3f.Transform(Vector3f.UnitZ * a, Orientation);
        }

        public static float ClampCircular(float n, float min, float max)
        {
            if (n >= max) n -= max;
            if (n < min) n += max;
            return n;
        }
    }

}
