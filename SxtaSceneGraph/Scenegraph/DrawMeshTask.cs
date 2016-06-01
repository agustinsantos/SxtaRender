using log4net;
using Sxta.Core;
using Sxta.Render.Resources;
using System;
using System.Reflection;

namespace Sxta.Render.Scenegraph
{
    /**
     * An AbstractTask to draw a mesh. The mesh is drawn using the current
     * framebuffer and the current program.
     *
     * @ingroup scenegraph
     */
    public class DrawMeshTask : AbstractTask, ISwappable<DrawMeshTask>
    {

        /**
         * Creates a new DrawMeshTask.
         *
         * @param mesh a "node.mesh" qualified name. The first part specifies the
         *      scene node that contains the mesh. The second part specifies the
         *      name of the mesh in this node.
         * @param count the number of time this mesh must be drawn.
         */
        public DrawMeshTask(QualifiedName mesh, int count = 1)
            : base("DrawMeshTask")
        {
            init(mesh, count);
        }


        /**
         * Deletes this DrawMeshTask.
         */
        // ~DrawMeshTask() {}

        public override Task getTask(Object context)
        {
            SceneNode n = ((Method)context).getOwner();
            SceneNode target = mesh.getTarget(n);
            MeshBuffers m = null;
            if (target == null)
            {
                m = n.getOwner().getResourceManager().loadResource(mesh.name + ".mesh").get() as MeshBuffers;
            }
            else
            {
                m = target.getMesh(mesh.name);
            }
            if (m == null)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("DrawMesh : cannot find mesh '" + mesh.target + "." + mesh.name + "'");
                }
                throw new Exception();
            }
            return new Impl(m, count);
        }


        /**
         * Creates an empty DrawMeshTask.
         */
        public DrawMeshTask()
            : base("DrawMeshTask")
        {
        }

        /**
         * Initializes this DrawMeshTask.
         *
         * @param mesh a "node.mesh" qualified name. The first part specifies the
         *      scene node that contains the mesh. The second part specifies the
         *      name of the mesh in this node.
         * @param count the number of time this mesh must be drawn.
         */
        public void init(QualifiedName mesh, int count)
        {
            this.mesh = mesh;
            this.count = count;
        }


        /**
         * Swaps this DrawMeshTask with anoter one.
         *
         * @param t a DrawMeshTask.
         */
        public void swap(DrawMeshTask t)
        {
            Std.Swap(ref mesh, ref t.mesh);
            Std.Swap(ref count, ref t.count);
        }


        /**
         * A "node.mesh" qualified name. The first part specifies the scene node
         * that contains the mesh. The second part specifies the name of the mesh in
         * this node.
         */
        private QualifiedName mesh;

        /**
         * The number of time the mesh must be drawn.
         */
        private int count;

        /**
         * A ork::Task to draw a mesh.
         */
        public class Impl : Task
        {

            /**
             * The mesh that must be drawn.
             */
            public MeshBuffers m;

            /**
             * The number of time #m must be drawn.
             */
            public int count;

            /**
             * Creates a new DrawMeshTask::Impl task.
             *
             * @param m the mesh to be drawn.
             * @param count the number of time the mesh must be drawn.
             */
            public Impl(MeshBuffers m, int count)
                : base("DrawMesh", true, 0)
            {
                this.m = m;
                this.count = count;
            }


            /**
             * Deletes this DrawMeshTask::Impl task.
             */
            //~Impl();

            public override bool run()
            {
                if (m != null)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("DrawMesh" + m);
                    }
                    Program prog = SceneManager.getCurrentProgram();
                    if (m.nindices == 0)
                    {
                        SceneManager.getCurrentFrameBuffer().draw(prog, m, m.mode, 0, m.nvertices);
                    }
                    else
                    {
                        SceneManager.getCurrentFrameBuffer().draw(prog, m, m.mode, 0, m.nindices);
                    }
                }
                return true;
            }

            private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
