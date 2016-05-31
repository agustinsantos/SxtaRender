using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.Math
{
    public class Box2d
    {

        /// <summary>
        /// Creates a new, empty bounding box.
        /// Initializes a new instance of the <see cref="Sxta.Math.Box2d"/> class.
        /// </summary>
        public Box2d()
        {
            xmin = double.MaxValue;
            xmax = double.MinValue;
            ymin = double.MaxValue;
            ymax = double.MinValue;

        }

        /// <summary>
        /// Creates a new bounding box with the given coordinates.
        /// Initializes a new instance of the <see cref="Sxta.Math.Box2d"/> class.
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
        public Box2d(double xmin, double xmax, double ymin, double ymax)
        {
            this.xmin = xmin;
            this.xmax = xmax;
            this.ymin = ymin;
            this.ymax = ymax;
        }


        /// <summary>
        /// Creates a new bounding box enclosing the two given points.
        /// Initializes a new instance of the <see cref="Sxta.Math.Box2d"/> class.
        /// </summary>
        /// <param name='p'>
        /// P.
        /// </param>
        /// <param name='q'>
        /// Q.
        /// </param>
        public Box2d(Vector2d p, Vector2d q)
        {
            this.xmin = System.Math.Min(p.X, q.X);
            this.xmax = System.Math.Max(p.X, q.X);
            this.ymin = System.Math.Min(p.Y, q.Y);
            this.ymax = System.Math.Max(p.Y, q.Y);
        }

        /// <summary>
        /// Minimum x coordinate.
        /// </summary>
        public double xmin { get; private set; }

        /// <summary>
        /// Maximum x coordinate.
        /// Gets the xmax.
        /// </summary>
        /// <value>
        /// The xmax.
        /// </value>
        public double xmax { get; private set; }

        /// <summary>
        /// Minimum y coordinate.
        /// </summary>
        /// <value>
        /// The ymin.
        /// </value>
        public double ymin { get; private set; }

        /// <summary>
        /// Maximum y coordinate.
        /// Gets the ymax.
        /// </summary>
        /// <value>
        /// The ymax.
        /// </value>
        public double ymax { get; private set; }


        /// <summary>
        /// Returns the center of this bounding box.
        /// Center this instance.
        /// </summary>
        Vector2 Center()
        {
            return new Vector2((float)(xmin + xmax) / 2, (float)(ymin + ymax) / 2);
        }

        /// <summary>
        /// Returns the bounding box containing this box and the given point.
        /// Enlarge the specified p.
        /// </summary>
        /// <param name='p'>
        /// P. p an arbitrary point.
        /// </param>
        Box2d Enlarge(Vector2 p)
        {
            return new Box2d(System.Math.Min(xmin, p.X),
                                System.Math.Max(xmax, p.X),
                                System.Math.Min(ymin, p.Y),
                                System.Math.Max(ymax, p.Y));
        }


        /// <summary>
        /// Returns the bounding box containing this box and the given box.
        /// Enlarge the specified r.
        /// </summary>
        /// <param name='r'>
        /// R.  an arbitrary bounding box.
        /// </param>
        public Box2d Enlarge(Box2d r)
        {
            return new Box2d(System.Math.Min(xmin, r.xmin),
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
        public bool Contains(Vector2d p)
        {
            return p.X >= xmin && p.X <= xmax && p.Y >= ymin && p.Y <= ymax;
        }

        public bool Contains(Vector2i p)
        {
            return p.X >= xmin && p.X <= xmax && p.Y >= ymin && p.Y <= ymax;
        }
    }
}
