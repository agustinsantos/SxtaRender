using Sxta.Core;
using Sxta.Math;
using Sxta.Render;
using Sxta.Render.Scenegraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using proland;

namespace Examples.Tutorials
{
    public interface ViewManager
    {

        /// <summary>
        /// Returns the SceneManager. Used to find the light SceneNode
        /// controlled by a BasicViewHandler.
        /// </summary>
        /// <returns></returns>
        SceneManager getScene();

        /// <summary>
        /// Returns the TerrainViewController. Used by a BasicViewHandler to
        /// control the view.
        /// </summary>
        TerrainViewController getViewController();

        /// <summary>
        /// Converts screen coordinates to world space coordinates. Used
        /// by a BasicViewHandler to find the location of mouse clics.
        ///
        /// @param x a screen x coordinate.
        /// @param y a screen y coordinate.
        /// @return the world space point corresponding to (x,y) on screen.
        /// </summary>
        Vector3d getWorldCoordinates(int x, int y);
    }

    /// <summary>
    /// An EventHandler to control a TerrainViewController and a light source with
    /// the mouse and/or the keyboard. This EventHandler relies on a ViewManager
    /// to find the TerrainViewController, to find the light SceneNode (using a
    /// SceneManager), and to convert between screen and world coordinates.
    /// This implementation allows the user to move the view and the light with
    /// the mouse and the PAGE_UP and PAGE_DOWN keys.
    /// It also provides methods to instantly change the view and light positions,
    /// and to start an animation to go smoothly from one position to another.
    /// @ingroup proland_ui
    /// </summary>
    public class BasicViewHandler
    {

        //3d vector to store the camera's position in
        public Vector3f _Position;
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


        double mix2(double x, double y, double t)
        {
            return Math.Abs(x - y) < Math.Max(x, y) * 1e-5 ? y : ((double)1 - t) * x + t * y;
        }
        /// <summary>
        /// A TerrainViewController position and a light source position.
        /// </summary>
        public class Position
        {
            public double x0, y0, theta, phi, d, sx, sy, sz;

            public Position()
            {
                x0 = 0.0;
                y0 = 0.0;
                theta = 0.0;
                phi = 0.0;
                d = 0.0;
                sx = 0.0;
                sy = 0.0;
                sz = 0.0;
            }
        };

        /// <summary>
        /// Creates a new BasicViewHandler.
        ///
        /// @param smooth true to use exponential damping to go to target
        ///      positions, false to go to target positions directly.
        /// @param view the object used to access the view controller.
        /// @param next the EventHandler to which the events not handled by this
        ///      EventHandler must be forwarded.
        /// </summary>
        public BasicViewHandler(bool smooth, ViewManager view)
        {
            init(smooth, view);
        }


        public virtual void redisplay(double t, double dt)
        {
            if (!initialized)
            {
                getPosition(target);
                initialized = true;
            }

            TerrainViewController controller = getViewManager().getViewController();

            if (animation >= 0.0)
            {
                animation = controller.interpolate(start.x0, start.y0, start.theta, start.phi, start.d,
                    end.x0, end.y0, end.theta, end.phi, end.d, animation);

                Vector3d startl = new Vector3d(start.sx, start.sy, start.sz);
                Vector3d endl = new Vector3d(end.sx, end.sy, end.sz);
                Vector3d l = (startl * (1.0 - animation) + endl * animation);
                l.Normalize();
                HashSet<SceneNode> i = getViewManager().getScene().getNodes("light");
                if (i != null && i.Count > 0)
                {
                    SceneNode n = i.First();
                    n.setLocalToParent(Matrix4d.CreateTranslation(new Vector3d(l.X, l.Y, l.Z)));
                }

                if (animation == 1.0)
                {
                    getPosition(target);
                    animation = -1.0;
                }
            }
            else
            {
                updateView(t, dt);
            }
            controller.update();
            controller.setProjection(Vector4f.Zero);

            FrameBuffer fb = FrameBuffer.getDefault();
            fb.clear(true, false, true);

            getViewManager().getScene().update(dt);
            getViewManager().getScene().draw();

            double lerp = 1.0 - Math.Exp(-dt * 2.301e-6);
            double gh = mix2(controller.getGroundHeight(), TerrainNode.groundHeightAtCamera, lerp);
            controller.setGroundHeight((float)gh);

        }

        public virtual void reshape(int x, int y)
        {

        }

        public virtual void idle(bool damaged)
        {

        }
#if TODO
        public virtual bool mouseClick(int x, int y)
        {
            oldx = x;
            oldy = y;
            if (OpenTK.Input.Key.ControlLeft != 0)
            {
                mode = userMode.rotate;
                return true;
            }
            else if (m == 0)
            { // no modifier
                if (parentWindow.Keyboard[OpenTK.Input.Key.Left])
                {
                    mode = userMode.move;
                }
                else
                {
                    mode = userMode.light;
                }
                return true;
            }
            return false;
        }
#endif
        public virtual bool mouseMotion(int x, int y)
        {
            if (!initialized)
            {
                getPosition(target);
                initialized = true;
            }
            if (mode == userMode.rotate)
            {
                target.phi += (oldx - x) / 500.0;
                target.theta += (oldy - y) / 500.0;
                target.theta = Math.Max((float)-Math.PI, Math.Min((float)Math.PI, (float)target.theta));
                oldx = x;
                oldy = y;
                return true;
            }
            else if (mode == userMode.move)
            {
                Vector3d oldp = getViewManager().getWorldCoordinates(oldx, oldy);
                Vector3d p = getViewManager().getWorldCoordinates(x, y);
                if (!(double.IsNaN(oldp.X) || double.IsNaN(oldp.Y) || double.IsNaN(oldp.Z) || double.IsNaN(p.X) || double.IsNaN(p.Y) || double.IsNaN(p.Z)))
                {
                    Position current = new Position();
                    getPosition(current, false);
                    setPosition(target, false);
                    TerrainViewController controller = getViewManager().getViewController();
                    controller.move(oldp, p);
                    getPosition(target, false);
                    setPosition(current, false);
                }
                oldx = x;
                oldy = y;
                return true;
            }
            else if (mode == userMode.light)
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
                float vangle = (float)Math.Asin(target.sz);
                float hangle = (float)Math.Atan2(target.sy, target.sx);
                vangle += ((float)Math.PI / 180) * ((float)(oldy - y)) * 0.25f;
                hangle += ((float)Math.PI / 180) * ((float)(oldx - x)) * 0.25f;
                target.sx = Math.Cos(vangle) * Math.Cos(hangle);
                target.sy = Math.Cos(vangle) * Math.Sin(hangle);
                target.sz = Math.Sin(vangle);

                oldx = x;
                oldy = y;
                return true;
            }
            return false;
        }

        public virtual bool mousePassiveMotion()
        {
            return false;
        }
#if TODO
        public virtual bool mouseWheel(int x, int y)
        {
            if (!initialized)
            {
                getPosition(target);
                initialized = true;
            }
            TerrainViewController controller = getViewManager().getViewController();
            const float dzFactor = 1.2f;
            if (/**b == WHEEL_DOWN currentMouseState.Wheel*/)
            {
                target.d = target.d * dzFactor;
                return true;
            }
            if (/**b == WHEEL_UP*/)
            {
                target.d = target.d / dzFactor;
                return true;
            }
            return false;
        }
#endif
        public virtual bool keyTyped()
        {
            return false;
        }

        public virtual bool keyReleased()
        {
            return false;
        }

        public virtual bool specialKey()
        {
            if (parentWindow.Keyboard[OpenTK.Input.Key.F10])
            {
                smooth = !smooth;
                return true;
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.PageUp])
            {
                far = true;
                return true;
            }
            else
            {
                far = false;
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.PageDown])
            {
                near = true;
                return true;
            }
            else
            {
                near = false;
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.Up])
            {
                forward = true;
                return true;
            }
            else
            {
                forward = false;
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.Down])
            {
                backward = true;
                return true;
            }
            else
            {
                backward = false;
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.Left])
            {
                left = true;
                return true;
            }
            else
            {
                left = false;
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.Right])
            {
                right = true;
                return true;
            }
            else
            {
                right = false;
            }
            return false;
        }

        public virtual bool specialKeyReleased()
        {
            if (parentWindow.Keyboard[OpenTK.Input.Key.F10])
            {
                smooth = !smooth;
                return true;
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.PageUp])
            {
                far = false;
                return true;
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.PageDown])
            {
                near = false;
                return true;
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.Up])
            {
                forward = false;
                return true;
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.Down])
            {
                backward = false;
                return true;
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.Left])
            {
                left = false;
                return true;
            }
            if (parentWindow.Keyboard[OpenTK.Input.Key.Right])
            {
                right = false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the current view and light positions.
        ///
        /// @param[out] p the current view and light position.
        /// </summary>
        public void getPosition(Position p, bool light = true)
        {
            TerrainViewController view = getViewManager().getViewController();
            p.x0 = view.x0;
            p.y0 = view.y0;
            p.theta = view.theta;
            p.phi = view.phi;
            p.d = view.d;
            if (light)
            {
                HashSet<SceneNode> i = getViewManager().getScene().getNodes("light");
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
        ///
        /// @param p the new view and light position.
        /// </summary>
        public void setPosition(Position p, bool light = true)
        {
            TerrainViewController view = getViewManager().getViewController();
            view.x0 = p.x0;
            view.y0 = p.y0;
            view.theta = p.theta;
            view.phi = p.phi;
            view.d = p.d;
            if (light)
            {
                HashSet<SceneNode> i = getViewManager().getScene().getNodes("light");
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
        ///
        /// @param p the target position.
        /// </summary>
        public virtual void goToPosition(Position p)
        {
            getPosition(start);
            end = p;
            animation = 0.0;
        }

        /// <summary>
        /// Goes immediately to the given position.
        ///
        /// @param p the new view and light position.
        /// </summary>
        public void jumpToPosition(Position p)
        {
            setPosition(p, true);
            target = p;
        }


        /// <summary>
        /// The ViewManager to find the TerrainViewController, to find the light
        /// SceneNode, and to convert between screen and world coordinates.
        /// </summary>
        protected ViewManager viewManager;

        /// <summary>
        /// Creates an uninitialized BasicViewHandler.
        /// </summary>
        protected BasicViewHandler()
        {

        }

        /// <summary>
        /// Initializes this BasicViewHandler.
        /// See #BasicViewHandler.
        /// </summary>
        protected void init(bool smooth, ViewManager view)
        {
            this.viewManager = view;
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
        protected virtual ViewManager getViewManager()
        {
            return viewManager;
        }

        /// <summary>
        /// Updates the view for the current frame based on user inputs.
        /// </summary>
        protected virtual void updateView(double t, double dt)
        {
            TerrainViewController controller = getViewManager().getViewController();
            float dzFactor = (float)(Math.Pow(1.02f, Math.Min((float)(50.0e-6 * dt), 1.0f)));
            if (near)
            {
                target.d = target.d / dzFactor;
            }
            else if (far)
            {
                target.d = target.d * dzFactor;
            }
            Position p = new Position();
            getPosition(p, true);
            setPosition(target, false);
            //specialkey();
            if (forward)
            {
                float speed = (float)Math.Max(controller.getHeight() - controller.getGroundHeight(), 0.0);
                controller.moveForward(speed * dt * 1e-6);
            }
            else if (backward)
            {
                float speed = (float)Math.Max(controller.getHeight() - controller.getGroundHeight(), 0.0);
                controller.moveForward(-speed * dt * 1e-6);
            }
            if (left)
            {
                controller.turn(dt * 5e-7);
            }
            else if (right)
            {
                controller.turn(-dt * 5e-7);
            }
            getPosition(target, false);

            if (smooth)
            {
                double lerp = 1.0 - Math.Exp(-dt * 2.301e-6);
                double x0;
                double y0;
                controller.interpolatePos(p.x0, p.y0, target.x0, target.y0, lerp, out x0, out y0);
                p.x0 = x0;
                p.y0 = y0;
                p.theta = mix2(p.theta, target.theta, lerp);
                p.phi = mix2(p.phi, target.phi, lerp);
                p.d = mix2(p.d, target.d, lerp);
                p.sx = mix2(p.sx, target.sx, lerp);
                p.sy = mix2(p.sy, target.sy, lerp);
                p.sz = mix2(p.sz, target.sz, lerp);
                double l = 1.0 / Math.Sqrt(p.sx * p.sx + p.sy * p.sy + p.sz * p.sz);
                p.sx *= l;
                p.sy *= l;
                p.sz *= l;
                setPosition(p);
            }
            else
            {
                setPosition(target);
            }
        }

        protected void swap(BasicViewHandler o)
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
        //private EventHandler next;

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
            move, ///< moves the "look-at" point.
            rotate, ///< rotates around the "look-at" point
            light ///< moves the light.
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
