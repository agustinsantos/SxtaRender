using Sxta.Render.OSG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.Render.OSGViewer
{
    public class Viewer : ViewerBase
    {
        public override void Advance(double simulationTime = USE_REFERENCE_TIME)
        {
        }
        public override void EventTraversal()
        {
        }
        public override void UpdateTraversal()
        {
        }

        /*  Set the sene graph data that viewer with view.*/
        public virtual Node SceneData
        {
            set
            {
                //setReferenceTime(0.0);
                _sceneData = value;
            }
        }
        public void UpdateSceneGraph(NodeVisitor updateVisitor)
        {
            if (_sceneData == null) return;
            else
                _sceneData.Accept(updateVisitor);
        }

        Node _sceneData;
        NodeVisitor _nodeVisitor;

    }
}
