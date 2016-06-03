using OpenTK;
using Sxta.Core;
using Sxta.Math;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vector3d = Sxta.Math.Vector3d;
using Matrix4d = Sxta.Math.Matrix4d;
using OpenTK.Input;

namespace Sxta.Render.Scenegraph.Controller
{

    /// <summary>
    /// An EventHandler to control a ViewController and a light source with
    /// the mouse and/or the keyboard. This EventHandler relies on a ViewManager
    /// to find the ViewController, to find the light SceneNode (using a
    /// SceneManager), and to convert between screen and world coordinates.
    /// This implementation allows the user to move the view and the light with
    /// the mouse and the PAGE_UP and PAGE_DOWN keys.
    /// It also provides methods to instantly change the view and light positions,
    /// and to start an animation to go smoothly from one position to another.
    /// </summary>
    public class BasicViewHandler : ISwappable<BasicViewHandler>
    {
        /// <summary>
        /// The terrain elevation below the current viewer position. This field must be
        /// updated manually by users(the TileSamplerZ class can do this for you).
        /// </summary>
        public float groundHeightAtCamera = 0.0f;


        private float moveSpeed = 0.5f;
        private float turnSpeed = 0.5f;
        private float lerpFactor = 2.301f; // original lerp factor was 2.301e-6
        private GameWindow gameWindow;

        private float mouseSensitivity = 700f;

        private double Mix2(double x, double y, double t)
        {
            return System.Math.Abs(x - y) < System.Math.Max(x, y) * 1e-5 ? y : (1 - t) * x + t * y;
        }
        public GameWindow GameWindow
        {
            get { return gameWindow; }
            set
            {
                if (gameWindow != null)
                {
                    gameWindow.Keyboard.KeyDown -= OnKeyDownEvent;
                    gameWindow.Keyboard.KeyUp -= OnKeyUpEvent;
                    gameWindow.MouseWheel -= OnMouseWheelEvent;
                    gameWindow.MouseDown -= OnMouseDownEvent;
                }
                gameWindow = value;
                if (gameWindow != null)
                {
                    gameWindow.Keyboard.KeyDown += OnKeyDownEvent;
                    gameWindow.Keyboard.KeyUp += OnKeyUpEvent;
                    gameWindow.MouseWheel += OnMouseWheelEvent;
                    gameWindow.MouseDown += OnMouseDownEvent;
                }

            }
        }

        /// <summary>
        /// A TerrainViewController position and a light source position.
        /// </summary>
        public struct Position
        {
            public double x0, y0, theta, phi, d, sx, sy, sz;
        }

        /// <summary>
        /// Creates a new BasicViewHandler.
        /// </summary>
        /// <param name="smooth">true to use exponential damping to go to target
        ///      positions, false to go to target positions directly.</param>
        /// <param name="view">the object used to access the view controller.</param>
        /// <param name="next">the EventHandler to which the events not handled by this
        ///      EventHandler must be forwarded.</param>
        public BasicViewHandler(bool smooth, ViewManager view, EventHandler next)
        {
            init(smooth, view, next);
        }


        public virtual void OnRenderFrame(double t, double dt)
        {
            if (!initialized)
            {
                GetPosition(ref target);
                initialized = true;
            }

            ViewController controller = viewManager.ViewController;

            if (animation >= 0.0)
            {
                animation = controller.interpolate(start.x0, start.y0, start.theta, start.phi, start.d,
                    end.x0, end.y0, end.theta, end.phi, end.d, animation);

                Vector3d startl = new Vector3d(start.sx, start.sy, start.sz);
                Vector3d endl = new Vector3d(end.sx, end.sy, end.sz);
                Vector3d l = (startl * (1.0 - animation) + endl * animation);
                l.Normalize();
                HashSet<SceneNode> i = viewManager.SceneManager.getNodes("light");
                if (i != null && i.Count > 0)
                {
                    SceneNode n = i.First();
                    n.setLocalToParent(Matrix4d.CreateTranslation(new Vector3d(l.X, l.Y, l.Z)));
                }

                if (animation == 1.0)
                {
                    GetPosition(ref target);
                    animation = -1.0;
                }
            }
            else
            {
                updateView(t, dt);
            }
            controller.Update();
            controller.setProjection(Vector4f.Zero);

            FrameBuffer fb = FrameBuffer.getDefault();
            fb.clear(true, false, true);

            viewManager.SceneManager.update(dt);
            viewManager.SceneManager.draw();

            double lerp = 1.0 - System.Math.Exp(-dt * 2.301e-6);
            double gh = Mix2(controller.GroundHeight, groundHeightAtCamera, lerp);
            controller.GroundHeight = (float)gh;
        }

        public void OnUpdateFrame(double time)
        {
            CheckMouseMode();
            // get current mouse position
            OpenTK.Input.MouseState currentMouseState = OpenTK.Input.Mouse.GetState();
            MouseMotion(currentMouseState.X, currentMouseState.Y);
        }

        private bool CheckMouseMode()
        {
            var keyboardState = OpenTK.Input.Keyboard.GetState();
            if (keyboardState.IsKeyDown(Key.ControlLeft))
            {
                mode = userMode.Light;
                return true;
            }
            else
            { // no modifier
                if (keyboardState.IsKeyDown(Key.ShiftLeft))
                {
                    mode = userMode.Move;
                }
                else
                {
                    mode = userMode.Rotate;
                }
                return true;
            }
        }
        private void OnMouseDownEvent(object sender, MouseButtonEventArgs e)
        {
            //if (this.OnMouseDown(e))
            return;
        }

        private void OnMouseWheelEvent(object sender, MouseWheelEventArgs e)
        {
            if (this.OnMouseWheel(e))
                return;
        }
        /// <summary>
        /// Occurs when a key is pressed.
        /// </summary>
        /// <param name="sender">The KeyboardDevice which generated this event.</param>
        /// <param name="e">The key that was pressed.</param>
        private void OnKeyDownEvent(object sender, KeyboardKeyEventArgs e)
        {
            if (this.OnKeyDown(e))
                return;
        }

        private void OnKeyUpEvent(object sender, KeyboardKeyEventArgs e)
        {
            if (this.OnKeyRelease(e))
                return;
        }

        public bool OnMouseDown(MouseButtonEventArgs e)
        {
            oldx = e.X;
            oldy = e.Y;
            return CheckMouseMode();
        }

        public virtual bool MouseMotion(int x, int y)
        {
            if (!initialized)
            {
                GetPosition(ref target);
                initialized = true;
            }
            if (mode == userMode.Rotate)
            {
                target.phi += (oldx - x) / mouseSensitivity;
                target.theta += (oldy - y) / mouseSensitivity;
                target.theta = System.Math.Max((float)-System.Math.PI, System.Math.Min((float)System.Math.PI, (float)target.theta));
                oldx = x;
                oldy = y;
                return true;
            }
            else if (mode == userMode.Move)
            {
                Vector3d oldp = viewManager.GetWorldCoordinates(oldx, oldy);
                Vector3d p = viewManager.GetWorldCoordinates(x, y);
                if (!(double.IsNaN(oldp.X) || double.IsNaN(oldp.Y) || double.IsNaN(oldp.Z) || double.IsNaN(p.X) || double.IsNaN(p.Y) || double.IsNaN(p.Z)))
                {
                    Position current = new Position();
                    GetPosition(ref current, false);
                    SetPosition(target, false);
                    ViewController controller = viewManager.ViewController;
                    controller.move(oldp, p);
                    GetPosition(ref target, false);
                    SetPosition(current, false);
                }
                oldx = x;
                oldy = y;
                return true;
            }
            else if (mode == userMode.Light)
            {
                //safe_asin implementation
                if (target.sz <= -1)
                {
                    target.sz = -1;
                }
                else if (target.sz >= 1)
                {
                    target.sz = 1;
                }
                float vangle = (float)System.Math.Asin(target.sz);
                float hangle = (float)System.Math.Atan2(target.sy, target.sx);
                vangle += ((float)System.Math.PI / 180) * ((float)(oldy - y)) * 0.25f;
                hangle += ((float)System.Math.PI / 180) * ((float)(oldx - x)) * 0.25f;
                target.sx = System.Math.Cos(vangle) * System.Math.Cos(hangle);
                target.sy = System.Math.Cos(vangle) * System.Math.Sin(hangle);
                target.sz = System.Math.Sin(vangle);

                oldx = x;
                oldy = y;
                return true;
            }
            return false;
        }

        public virtual bool OnMouseWheel(MouseWheelEventArgs e)
        {
            if (!initialized)
            {
                GetPosition(ref target);
                initialized = true;
            }
            ViewController controller = viewManager.ViewController;
            const float dzFactor = 1.2f;
            if (e.Delta < 0) // WHEEL_DOWN
            {
                target.d = target.d * e.DeltaPrecise * dzFactor;
                return true;
            }
            if (e.Delta > 0) // WHEEL_UP
            {
                target.d = target.d / (e.DeltaPrecise * dzFactor);
                return true;
            }
            return false;
        }

        public virtual bool OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (e.Key == OpenTK.Input.Key.PageUp)
            {
                far = true;
                return true;
            }
            if (e.Key == OpenTK.Input.Key.PageDown)
            {
                near = true;
                return true;
            }
            if (e.Key == OpenTK.Input.Key.Up || e.Key == OpenTK.Input.Key.W)
            {
                forward = true;
                return true;
            }
            if (e.Key == OpenTK.Input.Key.Down || e.Key == OpenTK.Input.Key.S)
            {
                backward = true;
                return true;
            }
            if (e.Key == OpenTK.Input.Key.Left || e.Key == OpenTK.Input.Key.A)
            {
                left = true;
                return true;
            }
            if (e.Key == OpenTK.Input.Key.Right || e.Key == OpenTK.Input.Key.D)
            {
                right = true;
                return true;
            }
            return false;
        }

        public virtual bool OnKeyRelease(KeyboardKeyEventArgs e)
        {
            if (e.Key == OpenTK.Input.Key.F10)
            {
                smooth = !smooth;
                return true;
            }
            if (e.Key == OpenTK.Input.Key.PageUp)
            {
                far = false;
                return true;
            }
            if (e.Key == OpenTK.Input.Key.PageDown)
            {
                near = false;
                return true;
            }
            if (e.Key == OpenTK.Input.Key.Up || e.Key == OpenTK.Input.Key.W)
            {
                forward = false;
                return true;
            }
            if (e.Key == OpenTK.Input.Key.Down || e.Key == OpenTK.Input.Key.S)
            {
                backward = false;
                return true;
            }
            if (e.Key == OpenTK.Input.Key.Left || e.Key == OpenTK.Input.Key.A)
            {
                left = false;
                return true;
            }
            if (e.Key == OpenTK.Input.Key.Right || e.Key == OpenTK.Input.Key.D)
            {
                right = false;
                return true;
            }
            return false;
        }


        /// <summary>
        /// Returns the current view and light positions.
        /// </summary>
        /// <param name="light">the current view and light position.</param>
        /// <returns></returns>
        /// </summary>
        public void GetPosition(ref Position p, bool light = true)
        {
            ViewController view = viewManager.ViewController;
            p.x0 = view.X0;
            p.y0 = view.Y0;
            p.theta = view.Theta;
            p.phi = view.Phi;
            p.d = view.Distance;
            if (light)
            {
                HashSet<SceneNode> i = viewManager.SceneManager.getNodes("light");
                if (i != null && i.Count > 0)
                {
                    SceneNode n = i.First();
                    Vector3d l = n.getLocalToParent() * Vector3d.Zero;
                    p.sx = l.X;
                    p.sy = l.Y;
                    p.sz = l.Z;
                }
            }
        }


        /// <summary>
        /// Sets the current view and light position.
        /// <param name="p">the new view and light position.</param>
        /// <param name="light"></param>
        /// </summary>
        public void SetPosition(Position p, bool light = true)
        {
            ViewController view = viewManager.ViewController;
            view.X0 = p.x0;
            view.Y0 = p.y0;
            view.Theta = p.theta;
            view.Phi = p.phi;
            view.Distance = p.d;
            if (light)
            {
                HashSet<SceneNode> i = viewManager.SceneManager.getNodes("light");
                if (i != null && i.Count > 0)
                {
                    SceneNode n = i.First();
                    n.setLocalToParent(Matrix4d.CreateTranslation(new Vector3d(p.sx, p.sy, p.sz)));
                }
            }
            animation = -1.0;
        }

        /// <summary>
        /// Starts an animation to go smoothly from the current position
        /// to the given position.
        /// </summary>
        /// <param name="p">the target position.</param>
        public virtual void GoToPosition(Position p)
        {
            GetPosition(ref start);
            end = p;
            animation = 0.0;
        }

        /// <summary>
        /// Goes immediately to the given position.
        ///
        /// @param p the new view and light position.
        /// </summary>
        public void JumpToPosition(Position p)
        {
            SetPosition(p, true);
            target = p;
        }


        /// <summary>
        /// The ViewManager to find the ViewController, to find the light
        /// SceneNode, and to convert between screen and world coordinates.
        /// </summary>
        internal ViewManager viewManager;

        /// <summary>
        /// Creates an uninitialized BasicViewHandler.
        /// </summary>
        protected BasicViewHandler()
        {

        }

        /// <summary>
        /// Initializes this BasicViewHandler.
        /// </summary>
        internal void init(bool smooth, ViewManager view, EventHandler next)
        {
            this.viewManager = view;
            this.next = next;
            this.smooth = smooth;
            this.near = false;
            this.far = false;
            this.forward = false;
            this.backward = false;
            this.left = false;
            this.right = false;
            this.initialized = false;
            this.animation = -1;
        }

        /// <summary>
        /// Returns the ViewManager used by this BasicViewHandler to find the
        /// TerrainViewController, to find the light SceneNode, and to convert
        /// between screen and world coordinates.
        /// </summary>
        public virtual ViewManager ViewManager
        {
            get { return viewManager; }
            set { viewManager = value; }
        }

        /// <summary>
        /// Updates the view for the current frame based on user inputs.
        /// </summary>
        protected virtual void updateView(double t, double dt)
        {
            ViewController controller = viewManager.ViewController;
            float dzFactor = (float)(System.Math.Pow(1.02f, System.Math.Min((float)(50.0 * dt), 1.0f)));
            if (near)
            {
                target.d = target.d / dzFactor;
            }
            else if (far)
            {
                target.d = target.d * dzFactor;
            }
            Position p = new Position();
            GetPosition(ref p, true);
            SetPosition(target, false);
            //specialkey();
            if (forward)
            {
                float speed = (float)System.Math.Max(controller.getHeight() - controller.GroundHeight, 0.0);
                controller.moveForward(speed * dt * moveSpeed);
            }
            else if (backward)
            {
                float speed = (float)System.Math.Max(controller.getHeight() - controller.GroundHeight, 0.0);
                controller.moveForward(-speed * dt * moveSpeed);
            }
            if (left)
            {
                controller.turn(dt * turnSpeed);
            }
            else if (right)
            {
                controller.turn(-dt * turnSpeed);
            }
            GetPosition(ref target, false);

            if (smooth)
            {
                double lerp = 1.0 - System.Math.Exp(-dt * lerpFactor);// original lerp factor was 2.301e-6
                double x0;
                double y0;

                controller.interpolatePos(p.x0, p.y0, target.x0, target.y0, lerp, out x0, out y0);
                p.x0 = x0;
                p.y0 = y0;
                p.theta = Mix2(p.theta, target.theta, lerp);
                p.phi = Mix2(p.phi, target.phi, lerp);
                p.d = Mix2(p.d, target.d, lerp);
                p.sx = Mix2(p.sx, target.sx, lerp);
                p.sy = Mix2(p.sy, target.sy, lerp);
                p.sz = Mix2(p.sz, target.sz, lerp);
                double l = 1.0 / System.Math.Sqrt(p.sx * p.sx + p.sy * p.sy + p.sz * p.sz);
                p.sx *= l;
                p.sy *= l;
                p.sz *= l;
                SetPosition(p);
            }
            else
            {
                SetPosition(target);
            }
        }

        public void swap(BasicViewHandler o)
        {
            Std.Swap(ref viewManager, ref o.viewManager);
            Std.Swap(ref mode, ref o.mode);
            Std.Swap(ref oldx, ref o.oldx);
            Std.Swap(ref oldy, ref o.oldy);
            Std.Swap(ref near, ref o.near);
            Std.Swap(ref far, ref o.far);
        }


        /// <summary>
        /// The EventHandler to which the events not handled by this EventHandler
        /// must be forwarded.
        /// </summary>
        private EventHandler next;

        /// <summary>
        /// True to use exponential damping to go to target positions, false to go
        /// to target positions directly.
        /// </summary>
        private bool smooth;

        /// <summary>
        /// True if the PAGE_DOWN key is currently pressed.
        /// </summary>
        private bool near;

        /// <summary>
        /// True if the PAGE_UP key is currently pressed.
        /// </summary>
        private bool far;

        /// <summary>
        /// True if the UP key is currently pressed.
        /// </summary>
        private bool forward;

        /// <summary>
        /// True if the DOWN key is currently pressed.
        /// </summary>
        private bool backward;

        /// <summary>
        /// True if the LEFT key is currently pressed.
        /// </summary>
        private bool left;

        /// <summary>
        /// True if the RIGHT key is currently pressed.
        /// </summary>
        private bool right;

        /// <summary>
        /// A navigation mode.
        /// </summary>
        private enum userMode
        {
            Rotate, ///< rotates around the "look-at" point
            Move, ///< moves the "look-at" point.
            Light ///< moves the light.
        };

        /// <summary>
        /// The current navigation mode.
        /// </summary>
        private userMode mode;

        /// <summary>
        /// The mouse x coordinate at the last call to #mouseClick or #mouseMotion.
        /// </summary>
        private int oldx;

        /// <summary>
        /// The mouse x coordinate at the last call to #mouseClick or #mouseMotion.
        /// </summary>
        private int oldy;

        /// <summary>
        /// The target position manipulated by the user via the mouse and keyboard.
        /// </summary>
        private Position target;

        /// <summary>
        /// True if the target position #target is initialized.
        /// </summary>
        private bool initialized;

        /// <summary>
        /// Start position for an animation between two positions.
        /// </summary>
        private Position start;

        /// <summary>
        /// End position for an animation between two positions.
        /// </summary>
        private Position end;

        /// <summary>
        /// Animation status. Negative values mean no animation.
        /// 0 corresponds to the start position, 1 to the end position,
        /// and values between 0 and 1 to intermediate positions between
        /// the start and end positions.
        /// </summary>
        private double animation;
    }
}
