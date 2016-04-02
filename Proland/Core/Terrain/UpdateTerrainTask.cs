using Sxta.Render.Scenegraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Reflection;
using Sxta.Core;

namespace proland
{
    public class UpdateTerrainTask : AbstractTask
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Creates an uninitialized UpdateTerrainTask.
        /// </summary>
        /// <param name="terrain"></param>
        protected UpdateTerrainTask() : base("UpdateTerrainTask")
        {

        }
        /// <summary>
        /// Creates a new UpdateTerrainTask.
        /// </summary>
        /// <param name="terrain">The %terrain whose quadtree must be updated. The first part
        /// of this "node.name" qualified name specifies the scene node containing
        /// the TerrainNode field. The second part specifies the name of this
        /// TerrainNode field.</param>
        public UpdateTerrainTask(QualifiedName terrain) : base("UpdateTerrainTask")
        {
            init(terrain);
        }

        /// <summary>
        /// Initializes this UpdateTerrainTask.
        /// </summary>
        /// <param name="terrain">The %terrain whose quadtree must be updated. The first part
        /// of this "node.name" qualified name specifies the scene node containing
        /// the TerrainNode field. The second part specifies the name of this
        /// TerrainNode field.</param>
        protected void init(QualifiedName terrain)
        {
            this.terrain = terrain;
        }
        //TOSEE
        protected void swap(UpdateTerrainTask t)
        {
            UpdateTerrainTask _this = this;
            Std.Swap(ref _this, ref t);
        }
        //TOSEE
        public override Sxta.Render.Scenegraph.Task getTask(object context)
        {
            Method N = (Method)context;
            SceneNode n = N.getOwner();
            SceneNode target = terrain.getTarget(n);
            TerrainNode t = new TerrainNode();
            if (target == null)
            {
                t = (TerrainNode)(n.getOwner().getResourceManager().loadResource(terrain.name).get());
            }
            else
            {
                t = (TerrainNode)target.getField(terrain.name);
            }
            if (t == null)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("UpdateTerrain : cannot find terrain '" + terrain.target + "." + terrain.name + "'");
                }
                throw new Exception("UpdateTerrain : cannot find terrain '" + terrain.target + "." + terrain.name + "'");
            }
            if (log.IsErrorEnabled)
            {
                log.Error("UpdateTerrain");
            }
            t.update(n);
            return new TaskGraph();
        }

        /// <summary>
        /// The %terrain whose quadtree must be updated. The first part of this "node.name"
        /// qualified name specifies the scene node containing the TerrainNode
        /// field.The second part specifies the name of this TerrainNode field.
        /// </summary>
        private QualifiedName terrain;
    };
}

