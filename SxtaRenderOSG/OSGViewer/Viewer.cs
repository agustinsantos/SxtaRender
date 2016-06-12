using Sxta.OSG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.OSGViewer
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

        public override Stats ViewerStats
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void UpdateSceneGraph(NodeVisitor updateVisitor)
        {
            if (_sceneData == null) return;
            else
                _sceneData.Accept(updateVisitor);
        }

        public override bool ReadConfiguration(string filename)
        {
            throw new NotImplementedException();
        }

        public override bool IsRealized()
        {
            throw new NotImplementedException();
        }

        public override void Realize()
        {
            throw new NotImplementedException();
        }

        public override bool CheckNeedToDoFrame()
        {
            throw new NotImplementedException();
        }

        public override bool CheckEvents()
        {
            throw new NotImplementedException();
        }

        protected override void viewerInit()
        {
            throw new NotImplementedException();
        }

        Node _sceneData;
        NodeVisitor _nodeVisitor;

    }
}
