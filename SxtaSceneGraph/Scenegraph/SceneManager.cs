using SD.Tools.Algorithmia.GeneralDataStructures;
using Sxta.Math;
using Sxta.Render.Resources;
using Sxta.Render.Scenegraph.XmlResources;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Matrix4d = Sxta.Math.Matrix4d;
using Vector3d = Sxta.Math.Vector3d;
using Vector4d = Sxta.Math.Vector4d;

namespace Sxta.Render.Scenegraph
{
    /// <summary>
    /// A manager to manage a scene graph.
    /// </summary>
    public class SceneManager : IDisposable
    {

        /*
         * The visibility of a bounding box in a frustum.
         */
        public enum visibility
        {
            FULLY_VISIBLE, //!< The bounding box is fully visible
            PARTIALLY_VISIBLE, //!< The bounding box is partially visible
            INVISIBLE //!< The bounding box is invisible
        }
        static SceneManager()
        {
            RegisterResourceReader.RegisterResources();
        }

        /*
         * An iterator over a map of SceneNode.
         */
        // typedef MultiMapIterator<std.string, ptr<SceneNode> > NodeIterator;

        /*
         * Creates an empty SceneManager.
         */
        public SceneManager()
        {
            worldToScreen = Matrix4d.Zero; // should call update before using
            frameNumber = 0;
        }

        /*
         * Deletes this SceneManager.
         */
        ~SceneManager()
        {
            // Do not re-create Dispose clean-up code here. 
            // Calling Dispose(false) is optimal in terms of 
            // readability and maintainability.
            Dispose(false);
        }

        /*
         * Returns the root node of the scene graph managed by this manager.
         */
        public SceneNode getRoot()
        {
            return root;
        }

        /*
         * Sets the root node of the scene graph managed by this manager.
         */
        public void setRoot(SceneNode root)
        {
            if (this.root != null)
            {
                this.root.setOwner(null);
            }
            this.root = root;
            this.root.setOwner(this);
            this.camera = null;
        }

        /*
         * Returns the camera node of the scene graph managed by this manager.
         */
        public SceneNode getCameraNode()
        {
            if (camera == null || camera.owner != this)
            {
                camera = null;
                HashSet<SceneNode> nodes = getNodes(cameraNode);
                if (nodes != null && nodes.Count > 0)
                {
                    camera = getNodes(cameraNode).First();
                }
            }
            return camera;
        }

        /*
         * Sets the camera node of the scene graph managed by this manager. This
         * camera node must have a UniformMatrix4f defining the projection from
         * camera space to screen space.
         *
         * @param node a SceneNode flag that identifies the camera node.
         */
        public void setCameraNode(string node)
        {
            camera = null;
            cameraNode = node;
            HashSet<SceneNode> nodes = getNodes(node);
            if (nodes != null && nodes.Count > 0)
            {
                camera = getNodes(node).First();
            }
        }

        /*
         * Returns the name of the camera node method to be called to draw the scene.
         */
        public string getCameraMethod()
        {
            return cameraMethod;
        }

        /*
         * Sets the name of the camera node method to be called to draw the scene.
         *
         * @param method a method name.
         */
        public void setCameraMethod(string method)
        {
            cameraMethod = method;
        }

        /*
         * Returns the nodes of the scene graph that have the given flag.
         *
         * @param flag a SceneNode flag.
         */
        public HashSet<SceneNode> getNodes(string flag)
        {
            if (nodeMap.Count == 0 && root != null)
            {
                buildNodeMap(root);
            }
            return nodeMap.GetValues(flag, false);
        }

        /*
         * Returns the SceneNode currently bound to the given loop variable.
         *
         * @param name a loop variable.
         */
        public SceneNode getNodeVar(string name)
        {
            SceneNode node;
            if (nodeVariables.TryGetValue(name, out node))
            {
                return node;
            }
            return null;
        }

        /*
         * Sets the node currently bound to the given loop variable.
         *
         * @param name a loop variable.
         * @param node the new node bound to this loop variable.
         */
        public void setNodeVar(string name, SceneNode node)
        {
            nodeVariables[name] = node;
        }

        /*
         * Returns the ResourceManager used to manage the resources of the scene
         * graph.
         */
        public ResourceManager getResourceManager()
        {
            return resourceManager;
        }

        /*
         * Sets the ResourceManager used to manage the resources of the scene graph.
         *
         * @param resourceManager a resource manager.
         */
        public void setResourceManager(ResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;
        }

        /*
         * Returns the Scheduler used to schedule the Task to draw the scene.
         */
        public Scheduler getScheduler()
        {
            return scheduler;
        }

        /*
         * Sets the Scheduler to schedule the Task to draw the scene.
         */
        public void setScheduler(Scheduler scheduler)
        {
            this.scheduler = scheduler;
        }

        /*
         * Returns the transformation from camera space to screen space.
         */
        public Matrix4d getCameraToScreen()
        {
            return cameraToScreen;
        }

        /*
         * Sets the transformation from camera space to screen space.
         */
        public void setCameraToScreen(Matrix4d cameraToScreen)
        {
            this.cameraToScreen = cameraToScreen;
        }

        /*
         * Returns the transformation from world space to screen space.
         */
        public Matrix4d getWorldToScreen()
        {
            return worldToScreen;
        }

        /*
         * Returns true if the given point is visible from the camera node.
         *
         * @param worldPoint a point in world space.
         */
        public bool isVisible(Vector3d worldPoint)
        {
            for (int i = 0; i < 5; ++i)
            {
                if (Vector4d.Dot(worldFrustumPlanes[i], worldPoint) <= 0)
                {
                    return false;
                }
            }
            return true;
        }


        /*
         * Returns the visibility of the given bounding box from the camera node.
         *
         * @param worldBounds a bounding box in world space.
         */
        public visibility getVisibility(Box3d worldBounds)
        {
            return getVisibility(worldFrustumPlanes, worldBounds);
        }

        /*
         * Returns the visibility of the given bounding box in the given frustum.
         *
         * @param frustumPlanes the frustum plane equations.
         * @param b a bounding box, in the same reference frame as the frustum
         *     planes.
         */
        public static visibility getVisibility(Vector4d[] frustumPlanes, Box3d b)
        {
            visibility v0 = getVisibility(frustumPlanes[0], b);
            if (v0 == visibility.INVISIBLE)
            {
                return visibility.INVISIBLE;
            }
            visibility v1 = getVisibility(frustumPlanes[1], b);
            if (v1 == visibility.INVISIBLE)
            {
                return visibility.INVISIBLE;
            }
            visibility v2 = getVisibility(frustumPlanes[2], b);
            if (v2 == visibility.INVISIBLE)
            {
                return visibility.INVISIBLE;
            }
            visibility v3 = getVisibility(frustumPlanes[3], b);
            if (v3 == visibility.INVISIBLE)
            {
                return visibility.INVISIBLE;
            }
            visibility v4 = getVisibility(frustumPlanes[4], b);
            if (v4 == visibility.INVISIBLE)
            {
                return visibility.INVISIBLE;
            }
            if (v0 == visibility.FULLY_VISIBLE && v1 == visibility.FULLY_VISIBLE &&
                v2 == visibility.FULLY_VISIBLE && v3 == visibility.FULLY_VISIBLE &&
                v4 == visibility.FULLY_VISIBLE)
            {
                return visibility.FULLY_VISIBLE;
            }
            return visibility.PARTIALLY_VISIBLE;
        }

        /*
         * Returns the frustum plane equations from a projection matrix.
         *
         * @param toScreen a camera to screen projection matrix.
         * @param[out] frustumPlanes the frustum plane equations in camera space.
         */
        public static void getFrustumPlanes(Matrix4d toScreen, Vector4d[] frustumPlanes)
        {
            Contract.Assert(frustumPlanes.Length == 6);
            // OpenTK.Math matrices are row-major, OpenGL itself expects a column-major matrix
            // Extract the LEFT plane
            frustumPlanes[0].X = toScreen.M14 + toScreen.M11;
            frustumPlanes[0].Y = toScreen.M24 + toScreen.M21;
            frustumPlanes[0].Z = toScreen.M34 + toScreen.M31;
            frustumPlanes[0].W = toScreen.M44 + toScreen.M41;
            // Extract the RIGHT plane
            frustumPlanes[1].X = toScreen.M14 - toScreen.M11;
            frustumPlanes[1].Y = toScreen.M24 - toScreen.M21;
            frustumPlanes[1].Z = toScreen.M34 - toScreen.M31;
            frustumPlanes[1].W = toScreen.M44 - toScreen.M41;
            // Extract the BOTTOM plane
            frustumPlanes[2].X = toScreen.M14 + toScreen.M12;
            frustumPlanes[2].Y = toScreen.M24 + toScreen.M22;
            frustumPlanes[2].Z = toScreen.M34 + toScreen.M32;
            frustumPlanes[2].W = toScreen.M44 + toScreen.M42;
            // Extract the TOP plane
            frustumPlanes[3].X = toScreen.M14 - toScreen.M12;
            frustumPlanes[3].Y = toScreen.M24 - toScreen.M22;
            frustumPlanes[3].Z = toScreen.M34 - toScreen.M32;
            frustumPlanes[3].W = toScreen.M44 - toScreen.M42;
            // Extract the NEAR plane
            frustumPlanes[4].X = toScreen.M14 + toScreen.M13;
            frustumPlanes[4].Y = toScreen.M24 + toScreen.M23;
            frustumPlanes[4].Z = toScreen.M34 + toScreen.M33;
            frustumPlanes[4].W = toScreen.M44 + toScreen.M43;
            // Extract the FAR plane
            frustumPlanes[5].X = toScreen.M14 - toScreen.M13;
            frustumPlanes[5].Y = toScreen.M24 - toScreen.M23;
            frustumPlanes[5].Z = toScreen.M34 - toScreen.M33;
            frustumPlanes[5].W = toScreen.M44 - toScreen.M43;
        }

        /*
         * Updates all the transformation matrices in the scene graph.
         *
         * @param t the current time in micro-seconds.
         * @param dt the elapsed time in micro-seconds since the last call to #update.
         */
        public void update(double dt)
        {
            this.t += dt;
            this.dt = dt;

            if (root != null)
            {
                root.updateLocalToWorld(null);
                Matrix4d cameraToScreen = getCameraToScreen();
                worldToScreen = getCameraNode().getWorldToLocal() * cameraToScreen; //AQUI MULT MATRIX
                root.updateLocalToCamera(getCameraNode().getWorldToLocal(), cameraToScreen);
                getFrustumPlanes(worldToScreen, worldFrustumPlanes);
                computeVisibility(root, visibility.PARTIALLY_VISIBLE);
            }
        }

        /*
         * Executes the #getCameraMethod of the #getCameraNode node.
         */
        public void draw()
        {
            if (camera != null)
            {
                Method m = camera.getMethod(cameraMethod);
                if (m != null)
                {
                    Task newTask = null;
                    try
                    {
                        newTask = m.getTask();
                    }
                    catch (Exception)
                    {
                    }
                    if (newTask != null)
                    {
                        scheduler.run(newTask);
                        currentTask = newTask;
                    }
                    else if (currentTask != null)
                    {
                        scheduler.run(currentTask);
                    }
                }
            }
            ++frameNumber;
        }

        /*
         * Returns the current frame number. This number is incremented after each
         * call to #draw.
         */
        public uint getFrameNumber()
        {
            return frameNumber;
        }

        /*
         * Returns the time of the current frame in micro-seconds. This time is the
         * one passed as argument to the last call to #update.
         */
        public double getTime()
        {
            return t;
        }

        /*
         * Returns the elapsed time of between the two previous frames. This time
         * is the one passed as argument to the last call to #update.
         */
        public double getElapsedTime()
        {
            return dt;
        }


        /*
         * Returns the current FrameBuffer.
         */
        public static FrameBuffer getCurrentFrameBuffer()
        {
            if (CURRENTFB == null)
            {
                CURRENTFB = FrameBuffer.getDefault();
            }
            return CURRENTFB;
        }

        /*
         * Sets the current FrameBuffer. This can then be used in any module to retrieve
         * a target on which the user wants to render to.
         */
        public static void setCurrentFrameBuffer(FrameBuffer fb)
        {
            CURRENTFB = fb;
        }

        /*
         * Returns the current Program.
         */
        public static Program getCurrentProgram()
        {
            return CURRENTPROG;
        }

        /*
         * Sets the current GLSL Program. This can then be used in any module to retrieve a given
         * Program for further drawings.
         */
        public static void setCurrentProgram(Program prog)
        {
            CURRENTPROG = prog;
        }


        /*
         * The current framebuffer.
         */
        private static FrameBuffer CURRENTFB;

        /*
         * The current GLSL program.
         */
        private static Program CURRENTPROG;

        /*
         * The root node of the scene graph managed by this manager.
         */
        private SceneNode root;

        /*
         * The camera node of the scene graph.
         */
        private SceneNode camera;

        /*
         * The camera to screen transformation.
         */
        private Matrix4d cameraToScreen;

        /*
         * The world to screen transformation.
         */
        private Matrix4d worldToScreen;

        /*
         * The camera frustum planes in world space.
         */
        private Vector4d[] worldFrustumPlanes = new Vector4d[6];

        /*
         * The flag that identifies the camera node in the scene graph.
         */
        private string cameraNode;

        /*
         * The name of the camera node method to be called to draw the scene.
         */
        private string cameraMethod;

        /*
         * The last task or task graph that was used to draw the scene.
         */
        private Task currentTask;

        /*
         * A multimap that associates to each flag all the nodes having this flag.
         */
        private MultiValueDictionary<string, SceneNode> nodeMap = new MultiValueDictionary<string, SceneNode>();

        /*
         * A map that associates to each loop variable its current valueC.
         */
        private IDictionary<string, SceneNode> nodeVariables = new Dictionary<string, SceneNode>();

        /*
         * The ResourceManager that manages the resources of the scene graph.
         */
        private ResourceManager resourceManager;

        /*
         * The Scheduler used to schedule the Task to draw the scene.
         */
        private Scheduler scheduler;

        /*
         * The current frame number.
         */
        private uint frameNumber;

        /*
         * The valueC of the t argument of the last call to #update.
         */
        private double t;

        /*
         * The valueC of the dt argument of the last call to #update.
         */
        private double dt;

        /*
         * Returns the visibility of a bounding box with respect to a frustum plane.
         *
         * @param clip a frustum plane equation.
         * @param b a bounding box in the same reference frame as the frustum plane.
         */
        private static visibility getVisibility(Vector4d clip, Box3d b)
        {
            bool isvisible1 = PointInPlane(clip, new Vector3d(b.xmin, b.ymax, b.zmax));
            bool isvisible2 = PointInPlane(clip, new Vector3d(b.xmin, b.ymax, b.zmin));
            bool isvisible3 = PointInPlane(clip, new Vector3d(b.xmin, b.ymin, b.zmax));
            bool isvisible4 = PointInPlane(clip, new Vector3d(b.xmin, b.ymin, b.zmin));
            bool isvisible5 = PointInPlane(clip, new Vector3d(b.xmax, b.ymax, b.zmax));
            bool isvisible6 = PointInPlane(clip, new Vector3d(b.xmax, b.ymax, b.zmin));
            bool isvisible7 = PointInPlane(clip, new Vector3d(b.xmax, b.ymin, b.zmax));
            bool isvisible8 = PointInPlane(clip, new Vector3d(b.xmax, b.ymin, b.zmin));

            double x0 = b.xmin * clip.X;
            double x1 = b.xmax * clip.X;
            double y0 = b.ymin * clip.Y;
            double y1 = b.ymax * clip.Y;
            double z0 = b.zmin * clip.Z + clip.W;
            double z1 = b.zmax * clip.Z + clip.W;
            double p1 = x0 + y0 + z0;
            double p2 = x1 + y0 + z0;
            double p3 = x1 + y1 + z0;
            double p4 = x0 + y1 + z0;
            double p5 = x0 + y0 + z1;
            double p6 = x1 + y0 + z1;
            double p7 = x1 + y1 + z1;
            double p8 = x0 + y1 + z1;
            if (p1 > 0 && p2 > 0 && p3 > 0 && p4 > 0 && p5 > 0 && p6 > 0 && p7 > 0 && p8 > 0)
            {
                return visibility.INVISIBLE;
            }
            if (p1 <= 0 && p2 <= 0 && p3 <= 0 && p4 <= 0 && p5 <= 0 && p6 <= 0 && p7 <= 0 && p8 <= 0)
            {
                return visibility.FULLY_VISIBLE;
            }
            return visibility.PARTIALLY_VISIBLE;
        }

        private static bool PointInPlane(Vector4d plane, Vector3d p)
        {
            double dis = (plane.X * p.X + plane.Y * p.Y + plane.Z * p.Z + plane.W);
            return dis > 0;
        }

        /*
         * Computes the SceneNode#isVisible flag of the given SceneNode and of its
         * child node (and so on recursively).
         *
         * @param n a SceneNode.
         * @param v the visibility of its parent node.
         */
        private void computeVisibility(SceneNode n, visibility v)
        {
            if (v == visibility.PARTIALLY_VISIBLE)
            {
                v = getVisibility(n.getWorldBounds());
            }
            n.isVisible = v != visibility.INVISIBLE;

            for (int i = 0; i < n.getChildrenCount(); ++i)
            {
                computeVisibility(n.getChild(i), v);
            }
        }

        /*
         * Clears the #nodeMap map.
         */
        internal void clearNodeMap()
        {
            nodeMap.Clear();
        }

        /*
         * Builds the #nodeMap map for the given scene graph.
         *
         * @param node the root node of a scene graph.
         */
        private void buildNodeMap(SceneNode node)
        {
            foreach (var f in node.getFlags())
            {
                nodeMap.Add(f, node);
            }
            int n = node.getChildrenCount();
            for (int i = 0; i < n; ++i)
            {
                buildNodeMap(node.getChild(i));
            }
        }

        #region IDisposable
        // Track whether Dispose has been called. 
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly 
        // or indirectly by a user's code. Managed and unmanaged resources 
        // can be disposed. 
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {

                    if (root != null)
                    {
                        root.setOwner(null);
                    }
                    resourceManager.releaseResources();
                    resourceManager.close();

                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.  

                // Note disposing has been done.
                disposed = true;

            }
        }
        #endregion

    }
}
