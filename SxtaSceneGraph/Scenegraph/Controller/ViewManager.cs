using Sxta.Math;
using Sxta.Render.Scenegraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.Render.Scenegraph.Controller
{
    public class ViewManager
    {
        /// <summary>
        /// Returns the SceneManager. Used to find the light SceneNode
        /// controlled by a BasicViewHandler.
        /// </summary>
        /// <returns></returns>
        public SceneManager SceneManager { get; set; }

        /// <summary>
        /// Returns the ViewController. Used by a BasicViewHandler to
        /// control the view.
        /// </summary>
        public ViewController ViewController { get; set; }

        /// <summary>
        /// Converts screen coordinates to world space coordinates. Used
        /// by a BasicViewHandler to find the location of mouse clics.
        /// </summary>
        /// <param name="x">a screen x coordinate.</param>
        /// <param name="y">a screen y coordinate.</param>
        /// <returns>the world space point corresponding to (x,y) on screen.</returns>
        public Vector3d GetWorldCoordinates(int x, int y)
        {
#if TODO
            Vector3d p = SceneManager.getWorldCoordinates(x, y);
            if (System.Math.Abs(p.X) > 100000.0 || System.Math.Abs(p.Y) > 100000.0 || System.Math.Abs(p.Z) > 100000.0)
            {
                p = new Vector3d(double.NaN, double.NaN, double.NaN);
            }
            return p;
#endif 
            return new Vector3d(double.NaN, double.NaN, double.NaN);
            //throw new NotImplementedException();
        }
    }
}
