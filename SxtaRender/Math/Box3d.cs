using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxta.Math
{
    /// <summary>
    /// A 3D bounding box.
    /// </summary>
    public class Box3d
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Sxta.Math.Box3d"/> class.
        /// Creates a new, empty bounding box.
        /// </summary>
        public Box3d()
        {
            xmin = double.PositiveInfinity;
            xmax = double.NegativeInfinity;
            ymin = double.PositiveInfinity;
            ymax = double.NegativeInfinity;
            zmin = double.PositiveInfinity;
            zmax = double.NegativeInfinity;

        }


        /// <summary>
        /// Creates a new bounding box with the given coordinates.
        /// Initializes a new instance of the <see cref="Sxta.Math.Box3d"/> class.
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
        /// <param name='zmin'>
        /// Zmin.
        /// </param>
        /// <param name='zmax'>
        /// Zmax.
        /// </param>
        public Box3d(double xmin, double xmax, double ymin, double ymax, double zmin, double zmax)
        {
            this.xmin = xmin;
            this.xmax = xmax;
            this.ymin = ymin;
            this.ymax = ymax;
            this.zmin = zmin;
            this.zmax = zmax;
        }

        /// <summary>
        /// Creates a new bounding box enclosing the two given points.
        /// Initializes a new instance of the <see cref="Sxta.Math.Box3d"/> class.
        /// </summary>
        /// <param name='p'>
        /// P.
        /// </param>
        /// <param name='q'>
        /// Q.
        /// </param>
        public Box3d(Vector3d p, Vector3d q)
        {
            this.xmin = System.Math.Min(p.X, q.X);
            this.xmax = System.Math.Max(p.X, q.X);
            this.ymin = System.Math.Min(p.Y, q.Y);
            this.ymax = System.Math.Max(p.Y, q.Y);
            this.zmin = System.Math.Min(p.Z, q.Z);
            this.zmax = System.Math.Max(p.Z, q.Z);
        }
        /// <summary>
        /// Minimum x coordinate.
        /// </summary>
        public double xmin { get; set; }

        /// <summary>
        /// Maximum x coordinate.
        /// Gets the xmax.
        /// </summary>
        /// <value>
        /// The xmax.
        /// </value>
        public double xmax { get; set; }

        /// <summary>
        /// Minimum y coordinate.
        /// Gets the ymin.
        /// </summary>
        /// <value>
        /// The ymin.
        /// </value>
        public double ymin { get; set; }

        /// <summary>
        /// Maximum y coordinate.
        /// Gets the ymax.
        /// </summary>
        /// <value>
        /// The ymax.
        /// </value>
        public double ymax { get; set; }


        /// <summary>
        /// Minimum z coordinate.
        /// Gets the zmin.
        /// </summary>
        /// <value>
        /// The zmin.
        /// </value>
        public double zmin { get; set; }

        /// <summary>
        /// Maximum z coordinate.
        /// Gets the zmax.
        /// </summary>
        /// <value>
        /// The zmax.
        /// </value>
        public double zmax { get; set; }


        /// <summary>
        /// Returns the center of this bounding box.
        /// Center this instance.
        /// </summary>
        public Vector3d center()
        {
            return new Vector3d((xmin + xmax) / 2, (ymin + ymax) / 2, (zmin + zmax) / 2);
        }

        /// <summary>
        /// Returns the bounding box containing this box and the given point.
        /// Enlarge the specified p.
        /// </summary>
        /// <param name='p'>
        /// P. an arbitrary point.
        /// </param>
        public Box3d enlarge(Vector3d p)
        {
            return new Box3d(System.Math.Min(xmin, p.X),
                                System.Math.Max(xmax, p.X),
                                System.Math.Min(ymin, p.Y),
                                System.Math.Max(ymax, p.Y),
                                System.Math.Min(zmin, p.Z),
                                System.Math.Max(zmax, p.Z));
        }

        public Box3d enlarge(Vector3f p)
        {
            return new Box3d(System.Math.Min(xmin, p.X),
                                System.Math.Max(xmax, p.X),
                                System.Math.Min(ymin, p.Y),
                                System.Math.Max(ymax, p.Y),
                                System.Math.Min(zmin, p.Z),
                                System.Math.Max(zmax, p.Z));
        }


        /// <summary>
        /// Returns the bounding box containing this box and the given box.
        /// Enlarge the specified r.
        /// </summary>
        /// <param name='r'>
        /// R. r an arbitrary bounding box.
        /// </param>
        public Box3d enlarge(Box3d r)
        {
            return new Box3d(System.Math.Min(xmin, r.xmin),
                            System.Math.Max(xmax, r.xmax),
                            System.Math.Min(ymin, r.ymin),
                            System.Math.Max(ymax, r.ymax),
                            System.Math.Min(zmin, r.zmin),
                            System.Math.Max(zmax, r.zmax));
        }
        public Box3d enlarge(Box3f r)
        {
            return new Box3d(System.Math.Min(xmin, r.xmin),
                            System.Math.Max(xmax, r.xmax),
                            System.Math.Min(ymin, r.ymin),
                            System.Math.Max(ymax, r.ymax),
                            System.Math.Min(zmin, r.zmin),
                            System.Math.Max(zmax, r.zmax));
        }


        /// <summary>
        /// Returns true if this bounding box contains the given point.
        /// Contains the specified p.
        /// </summary>
        /// <param name='p'>
        /// P. @param p an arbitrary point.
        /// </param>
        public bool contains(Vector3d p)
        {
            return p.X >= xmin && p.X <= xmax && p.Y >= ymin && p.Y <= ymax && p.Z >= zmin && p.Z <= zmax;
        }
    }
}

