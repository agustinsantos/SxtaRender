#region --- License ---
/*
Copyright (c) 2008 - 2016 The Sxta Render library.

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion

using Sxta.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proland
{
    /// <summary>Represents a A 2D segment.</summary>
    [Serializable]
    public struct Seg2f : IEquatable<Seg2f>
    {     
        #region Fields

        /// <summary>
        /// One of the segment extremity.
        /// </summary>
        public Vector2f  a;

        /// <summary>
        /// The vector joining #a to the other segment extremity.
        /// </summary>
        public Vector2f ab;

        #endregion

        #region Public Members

        /// <summary>
        /// Creates a new segment with the given extremities.
        /// </summary>
        /// <param name="a">a segment extremity.</param>
        /// <param name="b">the other segment extremity.</param>
        public Seg2f(Vector2f a, Vector2f b)
        {
            this.a = a;
            this.ab = b - a;
        }

        /// <summary>
        /// Returns the square distance between the given point and the line
        /// defined by this segment.
        /// </summary>
        /// <param name="p">a point.</param>
        /// <returns></returns>
        public float LineDistSq(Vector2f p)
        {
            Vector2f ap = p - a;
            float dotprod = Vector2f.Dot(ab, ap);
            float projLenSq = dotprod * dotprod / ab.LengthSquared;
            return ap.LengthSquared - projLenSq;
        }

        /// <summary>
        /// Returns the square distance between the given point and this segment.
        /// </summary>
        /// <param name="p">a point.</param>
        /// <returns></returns>
        public float SegmentDistSq(Vector2f p)
        {
            Vector2f ap = p - a;
            float dotprod = Vector2f.Dot(ab, ap);
            float projlenSq;

            if (dotprod <= 0.0)
            {
                projlenSq = 0;
            }
            else
            {
                ap = ab - ap;
                dotprod = Vector2f.Dot(ab, ap);

                if (dotprod <= 0.0)
                {
                    projlenSq = 0;
                }
                else
                {
                    projlenSq = dotprod * dotprod / ab.LengthSquared;
                }
            }

            return ap.LengthSquared - projlenSq;
        }


        /// <summary>
        /// Returns true if this segment intersects the given segment.
        /// </summary>
        /// <param name="s">a segment.</param>
        /// <returns></returns>
        public bool Intersects(Seg2f s)
		{
			Vector2f aa = s.a - a;
			float det = Vector2f.Cross(ab, s.ab);
			float t0 = Vector2f.Cross(aa, s.ab) / det;

			if (t0 > 0 && t0 < 1) {
				float t1 = Vector2f.Cross(aa, ab) / det;
				return t1 > 0 && t1 < 1;
			}

			return false;
		}


		public bool Intersects(Seg2f  s, float  t0)
		 {
			Vector2f aa = s.a - a;
			float det = Vector2f.Cross(ab, s.ab);
			t0 = Vector2f.Cross(aa, s.ab) / det;

			if (t0 > 0 && t0 < 1) {
				float t1 = Vector2f.Cross(aa, ab) / det;
				return t1 > 0 && t1 < 1;
			}

			return false;
		}

        /// <summary>
        /// Returns true if this segment intersects the given segment. If there
        /// is an intersection it is returned in the vector.
        /// </summary>
        /// <param name="s">a segment.</param>
        /// <param name="i">where to store the intersection point, if any.</param>
        /// <returns></returns>
        public bool Intersects(Seg2f s, out Vector2f i)  
		{
			Vector2f aa = s.a - a;
			float det = Vector2f.Cross(ab, s.ab);
			float t0 = Vector2f.Cross(aa, s.ab) / det;

            if (t0 > 0 && t0 < 1)
            {
                i = a + ab * t0;
                float t1 = Vector2f.Cross(aa, ab) / det;
                return t1 > 0 && t1 < 1;
            }
            else
                i = new Vector2f();

			return false;
		}

        /// <summary>
        /// Returns true if this segment is inside or intersects the given
        /// bounding box.
        /// </summary>
        /// <param name="r">a bounding box.</param>
        /// <returns></returns>
        public bool Intersects(Box2f r)
		{
			Vector2f b = a + ab;
			if (r.Contains(a) || r.Contains(b)) {
				return true;
			}

			Box2f t = new Box2f(a, b);
			if (t.xmin > r.xmax || t.xmax < r.xmin || t.ymin > r.ymax || t.ymax < r.ymin) {
				return false;
			}

			float p0 = Vector2f.Cross(ab, new Vector2f(r.xmin, r.ymin) - a);
			float p1 = Vector2f.Cross(ab, new Vector2f(r.xmax, r.ymin) - a);
			if (p1 * p0 <= 0) {
				return true;
			}
			float p2 = Vector2f.Cross(ab, new Vector2f(r.xmin, r.ymax) - a);
			if (p2 * p0 <= 0) {
				return true;
			}
			float p3 = Vector2f.Cross(ab, new Vector2f(r.xmax, r.ymax) - a);
			if (p3 * p0 <= 0) {
				return true;
			}

			return false;
        }

        /// <summary>
        /// Returns true if this segment, with the given width, contains the given
        /// point.More precisely this method returns true if the stroked path
        /// defined from this segment, with a cap "butt" style, contains the given
        /// point.
        /// </summary>
        /// <param name="p">a point.</param>
        /// <param name="w">width of this segment.</param>
        /// <returns></returns>
        public bool Contains(Vector2f p, float w)
		 {
			Vector2f ap = p - a;
			float dotprod = Vector2f.Dot(ab, ap);
			float projlenSq;

			if (dotprod <= 0.0) {
				return false;
			} else {
				ap = ab - ap;
				dotprod = Vector2f.Dot(ab, ap);
				if (dotprod <= 0.0) {
					return false;
				} else {
					projlenSq = dotprod * dotprod / ab.LengthSquared;
					return ap.LengthSquared - projlenSq < w*w;
				}
			}
		}
		
		#endregion

		#region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("({0}, {1})", a, ab);
        }

        #endregion

        #region public override int GetHashCode()

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            return a.GetHashCode() ^ ab.GetHashCode();
        }

        #endregion

        #region public override bool Equals(object obj)

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>True if the instances are equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Seg2f))
                return false;

            return this.Equals((Seg2f)obj);
        }

        #endregion

        #endregion

        #region IEquatable<Seg2f> Members

        /// <summary>Indicates whether the current segment is equal to another segment.</summary>
        /// <param name="other">A segment to compare with this segment.</param>
        /// <returns>true if the current vector is equal to the segment parameter; otherwise, false.</returns>
        public bool Equals(Seg2f other)
        {
            return
                this.a == other.a &&
                this.ab == other.ab;
        }

        #endregion
    }


    /// <summary>Represents a A 2D segment.</summary>
    [Serializable]
    public struct Seg2d : IEquatable<Seg2d>
    {     
        #region Fields

        /// <summary>
        /// One of the segment extremity.
        /// </summary>
        public Vector2d  a;

        /// <summary>
        /// The vector joining #a to the other segment extremity.
        /// </summary>
        public Vector2d ab;

        #endregion

        #region Public Members

        /// <summary>
        /// Creates a new segment with the given extremities.
        /// </summary>
        /// <param name="a">a segment extremity.</param>
        /// <param name="b">the other segment extremity.</param>
        public Seg2d(Vector2d a, Vector2d b)
        {
            this.a = a;
            this.ab = b - a;
        }

        /// <summary>
        /// Returns the square distance between the given point and the line
        /// defined by this segment.
        /// </summary>
        /// <param name="p">a point.</param>
        /// <returns></returns>
        public double LineDistSq(Vector2d p)
        {
            Vector2d ap = p - a;
            double dotprod = Vector2d.Dot(ab, ap);
            double projLenSq = dotprod * dotprod / ab.LengthSquared;
            return ap.LengthSquared - projLenSq;
        }

        /// <summary>
        /// Returns the square distance between the given point and this segment.
        /// </summary>
        /// <param name="p">a point.</param>
        /// <returns></returns>
        public double SegmentDistSq(Vector2d p)
        {
            Vector2d ap = p - a;
            double dotprod = Vector2d.Dot(ab, ap);
            double projlenSq;

            if (dotprod <= 0.0)
            {
                projlenSq = 0;
            }
            else
            {
                ap = ab - ap;
                dotprod = Vector2d.Dot(ab, ap);

                if (dotprod <= 0.0)
                {
                    projlenSq = 0;
                }
                else
                {
                    projlenSq = dotprod * dotprod / ab.LengthSquared;
                }
            }

            return ap.LengthSquared - projlenSq;
        }


        /// <summary>
        /// Returns true if this segment intersects the given segment.
        /// </summary>
        /// <param name="s">a segment.</param>
        /// <returns></returns>
        public bool Intersects(Seg2d s)
		{
			Vector2d aa = s.a - a;
			double det = Vector2d.Cross(ab, s.ab);
			double t0 = Vector2d.Cross(aa, s.ab) / det;

			if (t0 > 0 && t0 < 1) {
				double t1 = Vector2d.Cross(aa, ab) / det;
				return t1 > 0 && t1 < 1;
			}

			return false;
		}


		public bool Intersects(Seg2d  s, double  t0)
		 {
			Vector2d aa = s.a - a;
			double det = Vector2d.Cross(ab, s.ab);
			t0 = Vector2d.Cross(aa, s.ab) / det;

			if (t0 > 0 && t0 < 1) {
				double t1 = Vector2d.Cross(aa, ab) / det;
				return t1 > 0 && t1 < 1;
			}

			return false;
		}

        /// <summary>
        /// Returns true if this segment intersects the given segment. If there
        /// is an intersection it is returned in the vector.
        /// </summary>
        /// <param name="s">a segment.</param>
        /// <param name="i">where to store the intersection point, if any.</param>
        /// <returns></returns>
        public bool Intersects(Seg2d s, out Vector2d i)  
		{
			Vector2d aa = s.a - a;
			double det = Vector2d.Cross(ab, s.ab);
			double t0 = Vector2d.Cross(aa, s.ab) / det;

            if (t0 > 0 && t0 < 1)
            {
                i = a + ab * t0;
                double t1 = Vector2d.Cross(aa, ab) / det;
                return t1 > 0 && t1 < 1;
            }
            else
                i = new Vector2d();

			return false;
		}

        /// <summary>
        /// Returns true if this segment is inside or intersects the given
        /// bounding box.
        /// </summary>
        /// <param name="r">a bounding box.</param>
        /// <returns></returns>
        public bool Intersects(Box2d r)
		{
			Vector2d b = a + ab;
			if (r.Contains(a) || r.Contains(b)) {
				return true;
			}

			Box2d t = new Box2d(a, b);
			if (t.xmin > r.xmax || t.xmax < r.xmin || t.ymin > r.ymax || t.ymax < r.ymin) {
				return false;
			}

			double p0 = Vector2d.Cross(ab, new Vector2d(r.xmin, r.ymin) - a);
			double p1 = Vector2d.Cross(ab, new Vector2d(r.xmax, r.ymin) - a);
			if (p1 * p0 <= 0) {
				return true;
			}
			double p2 = Vector2d.Cross(ab, new Vector2d(r.xmin, r.ymax) - a);
			if (p2 * p0 <= 0) {
				return true;
			}
			double p3 = Vector2d.Cross(ab, new Vector2d(r.xmax, r.ymax) - a);
			if (p3 * p0 <= 0) {
				return true;
			}

			return false;
        }

        /// <summary>
        /// Returns true if this segment, with the given width, contains the given
        /// point.More precisely this method returns true if the stroked path
        /// defined from this segment, with a cap "butt" style, contains the given
        /// point.
        /// </summary>
        /// <param name="p">a point.</param>
        /// <param name="w">width of this segment.</param>
        /// <returns></returns>
        public bool Contains(Vector2d p, double w)
		 {
			Vector2d ap = p - a;
			double dotprod = Vector2d.Dot(ab, ap);
			double projlenSq;

			if (dotprod <= 0.0) {
				return false;
			} else {
				ap = ab - ap;
				dotprod = Vector2d.Dot(ab, ap);
				if (dotprod <= 0.0) {
					return false;
				} else {
					projlenSq = dotprod * dotprod / ab.LengthSquared;
					return ap.LengthSquared - projlenSq < w*w;
				}
			}
		}
		
		#endregion

		#region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("({0}, {1})", a, ab);
        }

        #endregion

        #region public override int GetHashCode()

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            return a.GetHashCode() ^ ab.GetHashCode();
        }

        #endregion

        #region public override bool Equals(object obj)

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>True if the instances are equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Seg2d))
                return false;

            return this.Equals((Seg2d)obj);
        }

        #endregion

        #endregion

        #region IEquatable<Seg2d> Members

        /// <summary>Indicates whether the current segment is equal to another segment.</summary>
        /// <param name="other">A segment to compare with this segment.</param>
        /// <returns>true if the current vector is equal to the segment parameter; otherwise, false.</returns>
        public bool Equals(Seg2d other)
        {
            return
                this.a == other.a &&
                this.ab == other.ab;
        }

        #endregion
    }



	}