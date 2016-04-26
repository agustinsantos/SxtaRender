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
        /// <summary>
        /// The visibility of a bounding box in a frustum.
        /// </summary>
        public enum visibility
        {
            FULLY_VISIBLE, // The bounding box is fully visible
            PARTIALLY_VISIBLE, // The bounding box is partially visible
            INVISIBLE // The bounding box is invisible
        }
        static SceneManager()
        {
            RegisterResourceReader.RegisterResources();
        }

        /*
         * An iterator over a map of SceneNode.
         */
        // typedef MultiMapIterator<std.string, ptr<SceneNode> > NodeIterator;

        /// <summary>
        /// Creates an empty SceneManager.
        /// </summary>
        public SceneManager()
        {
            worldToScreen = Matrix4d.Zero; // should call update before using
            frameNumber = 0;
        }

        /// <summary>
        /// Deletes this SceneManager.
        /// </summary>
        ~SceneManager()
        {
            // Do not re-create Dispose clean-up code here. 
            // Calling Dispose(false) is optimal in terms of 
            // readability and maintainability.
            Dispose(false);
        }

        /// <summary>
        /// Returns the root node of the scene graph managed by this manager.
        /// </summary>
        /// <returns></returns>
        public SceneNode getRoot()
        {
            return root;
        }

        /// <summary>
        /// Sets the root node of the scene graph managed by this manager.
        /// </summary>
        /// <param name="root"></param>
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

        /// <summary>
        /// Returns the camera node of the scene graph managed by this manager.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Sets the camera node of the scene graph managed by this manager. This
        /// camera node must have a UniformMatrix4f defining the projection from
        /// camera space to screen space.
        /// </summary>
        /// <param name="node">a SceneNode flag that identifies the camera node.</param>
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

        /// <summary>
        /// Returns the name of the camera node method to be called to draw the scene.
        /// </summary>
        /// <returns></returns>
        public string getCameraMethod()
        {
            return cameraMethod;
        }

        /// <summary>
        /// Sets the name of the camera node method to be called to draw the scene.
        /// </summary>
        /// <param name="method"> a method name.</param>
        public void setCameraMethod(string method)
        {
            cameraMethod = method;
        }

        /// <summary>
        /// Returns the nodes of the scene graph that have the given flag.
        /// </summary>
        /// <param name="flag">a SceneNode flag.</param>
        /// <returns></returns>
        public HashSet<SceneNode> getNodes(string flag)
        {
            if (nodeMap.Count == 0 && root != null)
            {
                buildNodeMap(root);
            }
            return nodeMap.GetValues(flag, false);
        }

        /// <summary>
        /// Returns the SceneNode currently bound to the given loop variable.
        /// </summary>
        /// <param name="name">a loop variable.</param>
        /// <returns></returns>
        public SceneNode getNodeVar(string name)
        {
            SceneNode node;
            if (nodeVariables.TryGetValue(name, out node))
            {
                return node;
            }
            return null;
        }

        /// <summary>
        /// Sets the node currently bound to the given loop variable.
        /// </summary>
        /// <param name="name">a loop variable.</param>
        /// <param name="node">the new node bound to this loop variable.</param>
        public void setNodeVar(string name, SceneNode node)
        {
            nodeVariables[name] = node;
        }

        /// <summary>
        /// Returns the ResourceManager used to manage the resources of the scene
        /// graph.
        /// </summary>
        /// <returns></returns>
        public ResourceManager getResourceManager()
        {
            return resourceManager;
        }

        /// <summary>
        /// Sets the ResourceManager used to manage the resources of the scene graph.
        /// </summary>
        /// <param name="resourceManager">a resource manager.</param>
        public void setResourceManager(ResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;
        }

        /// <summary>
        /// Returns the Scheduler used to schedule the Task to draw the scene.
        /// </summary>
        /// <returns></returns>
        public Scheduler getScheduler()
        {
            return scheduler;
        }

        /// <summary>
        /// Sets the Scheduler to schedule the Task to draw the scene.
        /// </summary>
        /// <param name="scheduler"></param>
        public void setScheduler(Scheduler scheduler)
        {
            this.scheduler = scheduler;
        }

        /// <summary>
        /// Returns the transformation from camera space to screen space.
        /// </summary>
        /// <returns></returns>
        public Matrix4d getCameraToScreen()
        {
            return cameraToScreen;
        }

        /// <summary>
        /// Sets the transformation from camera space to screen space.
        /// </summary>
        /// <param name="cameraToScreen"></param>
        public void setCameraToScreen(Matrix4d cameraToScreen)
        {
            this.cameraToScreen = cameraToScreen;
        }

        /// <summary>
        /// Returns the transformation from world space to screen space.
        /// </summary>
        /// <returns></returns>
        public Matrix4d getWorldToScreen()
        {
            return worldToScreen;
        }

        /// <summary>
        /// Returns true if the given point is visible from the camera node.
        /// </summary>
        /// <param name="worldPoint">a point in world space.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the visibility of the given bounding box from the camera node.
        /// </summary>
        /// <param name="worldBounds"> a bounding box in world space.</param>
        /// <returns></returns>
        public visibility getVisibility(Box3d worldBounds)
        {
            return getVisibility(worldFrustumPlanes, worldBounds);
        }

        /// <summary>
        /// Returns the visibility of the given bounding box in the given frustum.
        /// </summary>
        /// <param name="frustumPlanes">the frustum plane equations.</param>
        /// <param name="b">a bounding box, in the same reference frame as the frustum planes</param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the frustum plane equations from a projection matrix.
        /// </summary>
        /// <param name="toScreen">a camera to screen projection matrix.</param>
        /// <param name="frustumPlanes">(out) frustumPlanes the frustum plane equations in camera space.</param>
        public static void getFrustumPlanes(Matrix4d toScreen, Vector4d[] frustumPlanes)
        {
            Contract.Assert(frustumPlanes.Length == 6);
            // Extract the LEFT plane
            // (near, 0, left, 0)
            frustumPlanes[0].X = toScreen.R0C0 + toScreen.R3C0;
            frustumPlanes[0].Y = toScreen.R0C1 + toScreen.R3C1;
            frustumPlanes[0].Z = toScreen.R0C2 + toScreen.R3C2;
            frustumPlanes[0].W = toScreen.R0C3 + toScreen.R3C3;
            // Extract the RIGHT plane
            // (-near, 0, -right, 0)
            frustumPlanes[1].X = -toScreen.R0C0 + toScreen.R3C0;
            frustumPlanes[1].Y = -toScreen.R0C1 + toScreen.R3C1;
            frustumPlanes[1].Z = -toScreen.R0C2 + toScreen.R3C2;
            frustumPlanes[1].W = -toScreen.R0C3 + toScreen.R3C3;
            // Extract the BOTTOM plane
            // (0, near, bottom, 0)
            frustumPlanes[2].X = toScreen.R1C0 + toScreen.R3C0;
            frustumPlanes[2].Y = toScreen.R1C1 + toScreen.R3C1;
            frustumPlanes[2].Z = toScreen.R1C2 + toScreen.R3C2;
            frustumPlanes[2].W = toScreen.R1C3 + toScreen.R3C3;
            // Extract the TOP plane
            // (0, -near, -top, 0)
            frustumPlanes[3].X = -toScreen.R1C0 + toScreen.R3C0;
            frustumPlanes[3].Y = -toScreen.R1C1 + toScreen.R3C1;
            frustumPlanes[3].Z = -toScreen.R1C2 + toScreen.R3C2;
            frustumPlanes[3].W = -toScreen.R1C3 + toScreen.R3C3;
            // Extract the NEAR plane
            // (0, 0, -1, -n)
            frustumPlanes[4].X = toScreen.R2C0 + toScreen.R3C0;
            frustumPlanes[4].Y = toScreen.R2C1 + toScreen.R3C1;
            frustumPlanes[4].Z = toScreen.R2C2 + toScreen.R3C2;
            frustumPlanes[4].W = toScreen.R2C3 + toScreen.R3C3;
            // Extract the FAR plane
            // (0, 0, 1, f)
            frustumPlanes[5].X = -toScreen.R2C0 + toScreen.R3C0;
            frustumPlanes[5].Y = -toScreen.R2C1 + toScreen.R3C1;
            frustumPlanes[5].Z = -toScreen.R2C2 + toScreen.R3C2;
            frustumPlanes[5].W = -toScreen.R2C3 + toScreen.R3C3;
        }

        /// <summary>
        /// Updates all the transformation matrices in the scene graph.
        /// </summary>
        /// <param name="dt">the elapsed time in micro-seconds since the last call to #update.</param>
        public void update(double dt)
        {
            this.t += dt;
            this.dt = dt;

            if (root != null)
            {
                root.updateLocalToWorld(null);
                Matrix4d cameraToScreen = getCameraToScreen();
                worldToScreen = cameraToScreen * getCameraNode().getWorldToLocal(); //AQUI MULT MATRIX CAMBIADO
                root.updateLocalToCamera(getCameraNode().getWorldToLocal(), cameraToScreen);
                getFrustumPlanes(worldToScreen, worldFrustumPlanes);
                computeVisibility(root, visibility.PARTIALLY_VISIBLE);
            }
        }

        /// <summary>
        /// Executes the #getCameraMethod of the #getCameraNode node.
        /// </summary>
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

        /// <summary>
        /// Returns the current frame number. This number is incremented after each
        /// call to #draw.
        /// </summary>
        /// <returns></returns>
        public uint getFrameNumber()
        {
            return frameNumber;
        }

        /// <summary>
        /// Returns the time of the current frame in micro-seconds. This time is the
        /// one passed as argument to the last call to #update.
        /// </summary>
        /// <returns></returns>
        public double getTime()
        {
            return t;
        }

        /// <summary>
        /// Returns the elapsed time of between the two previous frames. This time
        /// is the one passed as argument to the last call to #update.
        /// </summary>
        /// <returns></returns>
        public double getElapsedTime()
        {
            return dt;
        }

        /// <summary>
        /// Returns the current FrameBuffer.
        /// </summary>
        /// <returns></returns>
        public static FrameBuffer getCurrentFrameBuffer()
        {
            if (CURRENTFB == null)
            {
                CURRENTFB = FrameBuffer.getDefault();
            }
            return CURRENTFB;
        }

        /// <summary>
        ///  Sets the current FrameBuffer. This can then be used in any module to retrieve
        /// a target on which the user wants to render to.
        /// </summary>
        /// <param name="fb"></param>
        public static void setCurrentFrameBuffer(FrameBuffer fb)
        {
            CURRENTFB = fb;
        }

        /// <summary>
        /// Returns the current Program.
        /// </summary>
        /// <returns></returns>
        public static Program getCurrentProgram()
        {
            return CURRENTPROG;
        }

        /// <summary>
        /// Sets the current GLSL Program. This can then be used in any module to retrieve a given
        /// Program for further drawings.
        /// </summary>
        /// <param name="prog"></param>
        public static void setCurrentProgram(Program prog)
        {
            CURRENTPROG = prog;
        }

        /// <summary>
        /// The current framebuffer.
        /// </summary>
        private static FrameBuffer CURRENTFB;

        /// <summary>
        /// The current GLSL program.
        /// </summary>
        private static Program CURRENTPROG;

        /// <summary>
        /// The root node of the scene graph managed by this manager.
        /// </summary>
        private SceneNode root;

        /// <summary>
        /// The camera node of the scene graph.
        /// </summary>
        private SceneNode camera;

        /// <summary>
        /// The camera to screen transformation.
        /// </summary>
        private Matrix4d cameraToScreen;

        /// <summary>
        /// The world to screen transformation.
        /// </summary>
        private Matrix4d worldToScreen;

        /// <summary>
        /// The camera frustum planes in world space.
        /// </summary>
        private Vector4d[] worldFrustumPlanes = new Vector4d[6];

        /// <summary>
        /// The flag that identifies the camera node in the scene graph.
        /// </summary>
        private string cameraNode;

        /// <summary>
        /// The name of the camera node method to be called to draw the scene.
        /// </summary>
        private string cameraMethod;

        /// <summary>
        /// The last task or task graph that was used to draw the scene.
        /// </summary>
        private Task currentTask;

        /// <summary>
        /// A multimap that associates to each flag all the nodes having this flag.
        /// </summary>
        private MultiValueDictionary<string, SceneNode> nodeMap = new MultiValueDictionary<string, SceneNode>();

        /// <summary>
        /// A map that associates to each loop variable its current valueC.
        /// </summary>
        private IDictionary<string, SceneNode> nodeVariables = new Dictionary<string, SceneNode>();

        /// <summary>
        /// The ResourceManager that manages the resources of the scene graph.
        /// </summary>
        private ResourceManager resourceManager;

        /// <summary>
        /// The Scheduler used to schedule the Task to draw the scene.
        /// </summary>
        private Scheduler scheduler;

        /// <summary>
        /// The current frame number.
        /// </summary>
        private uint frameNumber;

        /// <summary>
        /// The valueC of the t argument of the last call to #update.
        /// </summary>
        private double t;

        /// <summary>
        /// The valueC of the dt argument of the last call to #update.
        /// </summary>
        private double dt;

        /// <summary>
        /// Returns the visibility of a bounding box with respect to a frustum plane.
        /// </summary>
        /// <param name="clip">a frustum plane equation.</param>
        /// <param name="b">a bounding box in the same reference frame as the frustum plane.</param>
        /// <returns></returns>
        private static visibility getVisibility(Vector4d clip, Box3d b)
        {
            /*
            bool isvisible1 = PointInPlane(clip, new Vector3d(b.xmin, b.ymin, b.zmin));
            bool isvisible2 = PointInPlane(clip, new Vector3d(b.xmax, b.ymin, b.zmin));
            bool isvisible3 = PointInPlane(clip, new Vector3d(b.xmax, b.ymax, b.zmin));
            bool isvisible4 = PointInPlane(clip, new Vector3d(b.xmin, b.ymax, b.zmin));
            bool isvisible5 = PointInPlane(clip, new Vector3d(b.xmin, b.ymin, b.zmax));
            bool isvisible6 = PointInPlane(clip, new Vector3d(b.xmax, b.ymin, b.zmax));
            bool isvisible7 = PointInPlane(clip, new Vector3d(b.xmax, b.ymax, b.zmax));
            bool isvisible8 = PointInPlane(clip, new Vector3d(b.xmin, b.ymax, b.zmax));
            */

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
                return visibility.FULLY_VISIBLE;
            }
            else
            if (p1 <= 0 && p2 <= 0 && p3 <= 0 && p4 <= 0 && p5 <= 0 && p6 <= 0 && p7 <= 0 && p8 <= 0)
            {
                return visibility.INVISIBLE;
            }
            else
                return visibility.PARTIALLY_VISIBLE;
        }

        private static bool PointInPlane(Vector4d plane, Vector3d p)
        {
            double dis = (plane.X * p.X + plane.Y * p.Y + plane.Z * p.Z + plane.W);
            return dis > 0;
        }

        /// <summary>
        /// Computes the SceneNode#isVisible flag of the given SceneNode and of its
        /// child node(and so on recursively).
        /// </summary>
        /// <param name="n">a SceneNode.</param>
        /// <param name="v">the visibility of its parent node.</param>
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

        /// <summary>
        /// Clears the #nodeMap map.
        /// </summary>
        internal void clearNodeMap()
        {
            nodeMap.Clear();
        }

        /// <summary>
        /// Builds the #nodeMap map for the given scene graph.
        /// </summary>
        /// <param name="node">Builds the #nodeMap map for the given scene graph.</param>
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
