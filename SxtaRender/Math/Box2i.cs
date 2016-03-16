using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.Math
{
    public class Box2i
    {
        /// <summary>
        /// Creates a new, empty bounding box.
        /// Initializes a new instance of the <see cref="Sxta.Math.Box2i"/> class.
        /// </summary>
        public Box2i()
        {
            xmin = int.PositiveInfinity;
            xmax = int.NegativeInfinity;
            ymin = int.PositiveInfinity;
            ymax = int.NegativeInfinity;

        }

        /// <summary>
        /// Creates a new bounding box with the given coordinates.
        /// Initializes a new instance of the <see cref="Sxta.Math.Box2i"/> class.
        /// </summary>
        /// <param name='xmin'>
        /// Xmin.
        /// </param>
        /// <param name='xmax'>
        /// Xmax.
        /// </param>
        /// <param name='ymin'>
        /// Ymin.
        /// </param>
        /// <param name='ymax'>
        /// Ymax.
        /// </param>
        public Box2i(int xmin, int xmax, int ymin, int ymax)
        {
            this.xmin = xmin;
            this.xmax = xmax;
            this.ymin = ymin;
            this.ymax = ymax;
        }


        /// <summary>
        /// Creates a new bounding box enclosing the two given points.
        /// Initializes a new instance of the <see cref="Sxta.Math.Box2i"/> class.
        /// </summary>
        /// <param name='p'>
        /// P.
        /// </param>
        /// <param name='q'>
        /// Q.
        /// </param>
        Box2i(Vector2 p, Vector2 q)
        {
            this.xmin = (int)System.Math.Min(p.X, q.X);
            this.xmax = (int)System.Math.Max(p.X, q.X);
            this.ymin = (int)System.Math.Min(p.Y, q.Y);
            this.ymax = (int)System.Math.Max(p.Y, q.Y);
        }
        /// <summary>
        /// Minimum x coordinate.
        /// </summary>
        public int xmin { get; private set; }

        /// <summary>
        /// Maximum x coordinate.
        /// Gets the xmax.
        /// </summary>
        /// <value>
        /// The xmax.
        /// </value>
        public int xmax { get; private set; }

        /// <summary>
        /// Minimum y coordinate.
        /// </summary>
        /// <value>
        /// The ymin.
        /// </value>
        public int ymin { get; private set; }

        /// <summary>
        /// Maximum y coordinate.
        /// Gets the ymax.
        /// </summary>
        /// <value>
        /// The ymax.
        /// </value>
        public int ymax { get; private set; }


        /// <summary>
        /// Returns the center of this bounding box.
        /// Center this instance.
        /// </summary>
        Vector2 center()
        {
            return new Vector2((xmin + xmax) / 2, (ymin + ymax) / 2);
        }

        /// <summary>
        /// Returns the bounding box containing this box and the given point.
        /// Enlarge the specified p.
        /// </summary>
        /// <param name='p'>
        /// P. p an arbitrary point.
        /// </param>
        Box2i enlarge(Vector2 p)
        {
            return new Box2i((int)System.Math.Min(xmin, p.X),
                                (int)System.Math.Max(xmax, p.X),
                                (int)System.Math.Min(ymin, p.Y),
                                (int)System.Math.Max(ymax, p.Y));
        }


        /// <summary>
        /// Returns the bounding box containing this box and the given box.
        /// Enlarge the specified r.
        /// </summary>
        /// <param name='r'>
        /// R.  an arbitrary bounding box.
        /// </param>
        public Box2i enlarge(Box2i r)
        {
            return new Box2i(System.Math.Min(xmin, r.xmin),
                            System.Math.Max(xmax, r.xmax),
                            System.Math.Min(ymin, r.ymin),
                            System.Math.Max(ymax, r.ymax));
        }


        /// <summary>
        /// Returns true if this bounding box contains the given point.
        /// Contains the specified p.
        /// </summary>
        /// <param name='p'>
        /// P. p an arbitrary point.
        /// </param>
        public bool contains(Vector2i p)
        {
            return p.X >= xmin && p.X <= xmax && p.Y >= ymin && p.Y <= ymax;
        }
        public bool contains(Vector2d p)
        {
            return p.X >= xmin && p.X <= xmax && p.Y >= ymin && p.Y <= ymax;
        }
    }
}
