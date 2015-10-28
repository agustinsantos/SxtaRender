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

            var forward = Vector3f.Transform(-Vector3f.UnitZ, Orientation);
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
            Vector3f forward = direction - Position;

            Rotation.X = 0; // (float)(-Math.PI / 4);// (float)Vector3f.CalculateAngle(forward, Vector3f.UnitX);
            Rotation.Y = 0;//(float)Vector3f.CalculateAngle(forward, Vector3f.UnitY );

            Update();
        }

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
