using Sxta.Math;
using System;

namespace Examples.Tutorials
{
    public class FirstPersonCamera
    {
        public Vector3f Position;
        public Vector3f Rotation;
        public Quaternion Orientation;

        public Matrix4f Matrix { get; internal set; }
        public Matrix4f Projection { get; set; }

        private readonly OpenTK.GameWindow parentWindow;
        private Vector2f mouseMove;
        private float mouseSensitivity = 0.1f;
        private float moveSpeed = 0.1f;

        private OpenTK.Input.MouseState currentMouseState;
        private OpenTK.Input.MouseState previousMouseState;

        public FirstPersonCamera(OpenTK.GameWindow gw)
        {
            parentWindow = gw;

            Matrix = Matrix4f.Identity;
            Projection = Matrix4f.Identity;
            Orientation = Quaternion.Identity;
        }

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

            this.Update();

        }

        public void Update()
        {
            Orientation = Quaternion.FromAxisAngle(Vector3f.UnitY, Rotation.Y) *
                          Quaternion.FromAxisAngle(Vector3f.UnitX, Rotation.X);

            var forward = Vector3f.Transform(Vector3f.UnitZ, Orientation);
            Matrix = Matrix4f.LookAt(Position, Position + forward, Vector3f.UnitY);
        }

        public void Resize(int width, int height, float fov = MathHelper.PiOver4)
        {
            Projection = Matrix4f.CreatePerspectiveFieldOfView(fov, (float)width / (float)height, 0.1f, 1000f);
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
