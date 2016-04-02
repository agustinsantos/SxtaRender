using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sxta.Render.Scenegraph;
using log4net;
using System.Reflection;
using Sxta.Core;

namespace proland
{
    public class UpdateTileSamplersTask : AbstractTask
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Creates a new UpdateTileSamplersTask.
        /// </summary>
        /// <param name="terrain">The %terrain whose uniforms must be updated. The first part
        /// of this "node.name" qualified name specifies the scene node containing
        /// the TerrainNode field. The second part specifies the name of this
        /// TerrainNode field.</param>
        public UpdateTileSamplersTask(QualifiedName terrain) : base("UpdateTileSamplersTask")
        {
            init(terrain);
        }

        //TODO
        public override Sxta.Render.Scenegraph.Task getTask(Object context)
        {
            Method N = (Method)context;
            SceneNode n = N.getOwner();
            SceneNode target = terrain.getTarget(n);
            TerrainNode t = new TerrainNode();
            if (target == null)
            {
                t = (TerrainNode)n.getOwner().getResourceManager().loadResource(terrain.name).get();
            }
            else
            {
                t = (TerrainNode)target.getField(terrain.name);
            }
            if (t == null)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("UpdateTileSamplers : cannot find terrain '" + terrain.target + "." + terrain.name + "'");
                }
                throw new Exception("UpdateTileSamplers : cannot find terrain '" + terrain.target + "." + terrain.name + "'");
            }
            TaskGraph result = new TaskGraph();
            //SceneNode.FieldIterator i = n.getFields();
            //while (i.hasNext())
            foreach (KeyValuePair<string, object> i in n.getFields())
            {
                //TOSEE
                TileSampler u = (TileSampler)i.Value;
                if (u != null)
                {
                    Sxta.Render.Scenegraph.Task ut = u.update(n.getOwner(), t.root);
                    if ((TaskGraph)ut == null || !((TaskGraph)(ut)).isEmpty())
                    {
                        result.addTask(ut);
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Creates an uninitialized UpdateTileSamplersTask.
        /// </summary>
        protected UpdateTileSamplersTask() : base("UpdateTileSamplersTask")
        {

        }

        /// <summary>
        /// Initializes this UpdateTileSamplersTask.
        /// </summary>
        /// <param name="terrain">The %terrain whose uniforms must be updated. The first part
        /// of this "node.name" qualified name specifies the scene node containing
        /// the TerrainNode field. The second part specifies the name of this
        /// TerrainNode field.</param>
        protected void init(QualifiedName terrain)
        {
            this.terrain = terrain;
        }

        protected void swap(UpdateTileSamplersTask t)
        {
            UpdateTileSamplersTask _this = this;
            Std.Swap(ref _this, ref t);
        }


        /**
         * The %terrain whose uniforms must be updated. The first part of this "node.name"
         * qualified name specifies the scene node containing the TerrainNode
         * field. The second part specifies the name of this TerrainNode field.
         */
        private QualifiedName terrain;
    }
}
