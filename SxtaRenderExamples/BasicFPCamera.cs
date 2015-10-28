using Sxta.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples.Tutorials
{
    public class BasicFPCamera
    {
        //3d vector to store the camera's position in
        public Vector3f Position;
        //the rotation around the Y axis of the camera
        private float yaw = 0.0f;
        //the rotation around the X axis of the camera
        private float pitch = 0.0f;

        private OpenTK.Input.MouseState currentMouseState;
        private OpenTK.Input.MouseState previousMouseState;
        private readonly OpenTK.GameWindow parentWindow;
        private Vector2f mouseMove;
        private float mouseSensitivity = 0.05f;
        private float moveSpeed = 0.2f;

        public Matrix4f ViewMatrix { get; internal set; }

        

        public Matrix4f ProjectionMatrix { get; set; }

        public BasicFPCamera(OpenTK.GameWindow gw)
        {
            parentWindow = gw;

            ViewMatrix = Matrix4f.Identity;
            ProjectionMatrix = Matrix4f.Identity;

            OpenTK.Input.Mouse.SetPosition(parentWindow.Width / 2, parentWindow.Height / 2);
            previousMouseState = OpenTK.Input.Mouse.GetState();
        }

        public BasicFPCamera(OpenTK.GameWindow gw, float x, float y, float z)
            : this(gw)
        {
            //instantiate position Vector3f to the x y z params.
            Position = new Vector3f(x, y, z);
        }
        //increment the camera's current yaw rotation

        public void Yaw(float amount)
        {
            //increment the yaw by the amount param
            yaw += amount;
        }

        //increment the camera's current yaw rotation
        public void Pitch(float amount)
        {
            //increment the pitch by the amount param
            pitch += amount;
        }

        //moves the camera forward relative to its current rotation (yaw)
        public void WalkForward(float distance)
        {
            Position.X += distance * (float)Math.Sin(MathHelper.ToRadians(yaw));
            Position.Z -= distance * (float)Math.Cos(MathHelper.ToRadians(yaw));
        }

        //moves the camera backward relative to its current rotation (yaw)
        public void WalkBackwards(float distance)
        {
            Position.X -= distance * (float)Math.Sin(MathHelper.ToRadians(yaw));
            Position.Z += distance * (float)Math.Cos(MathHelper.ToRadians(yaw));
        }

        //strafes the camera left relitive to its current rotation (yaw)
        public void StrafeLeft(float distance)
        {
            Position.X -= distance * (float)Math.Sin(MathHelper.ToRadians(yaw - 90));
            Position.Z += distance * (float)Math.Cos(MathHelper.ToRadians(yaw - 90));
        }

        //strafes the camera right relitive to its current rotation (yaw)
        public void StrafeRight(float distance)
        {
            Position.X -= distance * (float)Math.Sin(MathHelper.ToRadians(yaw + 90));
            Position.Z += distance * (float)Math.Cos(MathHelper.ToRadians(yaw + 90));
        }

        //translates and rotate the matrix so that it looks through the camera
        //this dose basic what gluLookAt() does
        public void Update()
        {
            //rotate the pitch around the X axis
            Matrix4f matrix = Matrix4f.CreateRotation(pitch, 1.0f, 0.0f, 0.0f);

            //rotate the yaw around the Y axis
            matrix = Matrix4f.CreateRotation(yaw, 0.0f, 1.0f, 0.0f) * matrix;

            //translate to the position vector's location
            ViewMatrix = matrix * Matrix4f.CreateTranslation(-Position);
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
            // controll camera yaw from y movement fromt the mouse
            this.Yaw(mouseMove.Y);
            //controll camera pitch from x movement fromt the mouse
            this.Pitch(mouseMove.X);

            // camera movement by keyboard
            if (parentWindow.Keyboard[OpenTK.Input.Key.A])
            {
                this.StrafeLeft(move);
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.D])
            {
                this.StrafeRight(move);
            }
            //if (parentWindow.Keyboard[OpenTK.Input.Key.Space])
            //{
            //    this.MoveY(move);
            //}
            //if (parentWindow.Keyboard[OpenTK.Input.Key.ControlLeft])
            //{
            //    this.MoveY(-move);
            //}
            if (parentWindow.Keyboard[OpenTK.Input.Key.W])
            {
                this.WalkForward(move);
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.S])
            {
                this.WalkBackwards(move);
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.Right])
            {
                this.Yaw(1f);
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.Left])
            {
                this.Yaw(-1f);
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.Up])
            {
                this.Pitch(1f);
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.Down])
            {
                this.Pitch(-1f);
            }

            this.Update();
        }

        public void Resize(int width, int height, float fov = MathHelper.PiOver4)
        {
            ProjectionMatrix = Matrix4f.CreatePerspectiveFieldOfView(fov, (float)width / (float)height, 0.1f, 1000f);
        }

        public float MouseSensitivity
        {
            get { return mouseSensitivity; }
            set { this.mouseSensitivity = value; }
        }

        public float MoveSpeed
        {
            get { return moveSpeed; }
            set { this.moveSpeed = value; }
        }
    }
}
