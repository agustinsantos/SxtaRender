using OpenTK;
using System;
using System.Runtime.InteropServices;

namespace Sxta.Math
{
    /// <summary>
    /// A 3D bounding box.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public class Box3f
    {
        
		/// <summary>
		/// Creates a new, empty bounding box.
		/// Initializes a new instance of the <see cref="Sxta.Math.Box3f"/> class.
		/// </summary>
        public Box3f()
        {
            xmin = float.PositiveInfinity;
            xmax = float.NegativeInfinity;
            ymin = float.PositiveInfinity;
            ymax = float.NegativeInfinity;
            zmin = float.PositiveInfinity;
            zmax = float.NegativeInfinity;

        }
       
		/// <summary>
		/// Creates a new bounding box with the given coordinates.
		/// Initializes a new instance of the <see cref="Sxta.Math.Box3f"/> class.
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
        public Box3f(float xmin, float xmax, float ymin, float ymax, float zmin, float zmax)
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
		/// Initializes a new instance of the <see cref="Sxta.Math.Box3f"/> class.
		/// </summary>
		/// <param name='p'>
		/// P.
		/// </param>
		/// <param name='q'>
		/// Q.
		/// </param>
        Box3f(Vector3 p, Vector3 q)
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
        public float xmin { get; private set; }

		/// <summary>
		/// Maximum x coordinate.
		/// Gets the xmax.
		/// </summary>
		/// <value>
		/// The xmax.
		/// </value>
        public float xmax { get; private set; }

		/// <summary>
		/// Minimum y coordinate.
		/// </summary>
		/// <value>
		/// The ymin.
		/// </value>
        public float ymin { get; private set; }

		/// <summary>
		/// Maximum y coordinate.
		/// Gets the ymax.
		/// </summary>
		/// <value>
		/// The ymax.
		/// </value>
        public float ymax { get; private set; }

		/// <summary>
		/// Minimum z coordinate.
		/// Gets the zmin.
		/// </summary>
		/// <value>
		/// The zmin.
		/// </value>
        public float zmin { get; private set; }

		/// <summary>
		/// Maximum z coordinate.
		/// Gets the zmax.
		/// </summary>
		/// <value>
		/// The zmax.
		/// </value>
        public float zmax { get; private set; }


		/// <summary>
		/// Returns the center of this bounding box.
		/// Center this instance.
		/// </summary>
        Vector3 center()
        {
            return new Vector3((xmin + xmax) / 2, (ymin + ymax) / 2, (zmin + zmax) / 2);
        }

		/// <summary>
		/// Returns the bounding box containing this box and the given point.
		/// Enlarge the specified p.
		/// </summary>
		/// <param name='p'>
		/// P. p an arbitrary point.
		/// </param>
        Box3f enlarge(Vector3 p)
        {
            return new Box3f(System.Math.Min(xmin, p.X),
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
		/// R.  an arbitrary bounding box.
		/// </param>
        public Box3f enlarge(Box3f r)
        {
            return new Box3f(System.Math.Min(xmin, r.xmin),
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
		/// P. p an arbitrary point.
		/// </param>
        public bool contains(Vector3 p)
        {
            return p.X >= xmin && p.X <= xmax && p.Y >= ymin && p.Y <= ymax && p.Z >= zmin && p.Z <= zmax;
        }


    }
}

