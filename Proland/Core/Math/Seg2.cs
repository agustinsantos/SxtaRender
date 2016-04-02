using Sxta.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proland
{
    public static class Seg2
    {
#if TODO
        struct seg2
        {
            /**
             * One of the segment extremity.
             */
            public static Vector2i a;
C:\TFG\SxtaRender\SxtaRenderExamples\Tutorials\Testing\Tutorial08_2.cs
            /**
             * The vector joining #a to the other segment extremity.
             */
            public static Vector2i ab;

            /**
             * Creates a new segment with the given extremities.
             *
             * @param a a segment extremity.
             * @param b the other segment extremity.
             */
            Seg2(Vector2i a, Vector2i b)
            {
                a = a;
                ab = b - a;
            }

            /**
             * Returns the square distance between the given point and the line
             * defined by this segment.
             *
             * @param p a point.
             */
            public static int lineDistSq(Vector2i p) {
                Vector2i ap = p - a;
                int dotprod = ab.dot(ap);
                int projLenSq = dotprod * dotprod / ab.squaredLength();
                return ap.squaredLength() - projLenSq;
            }

            /**
             * Returns the square distance between the given point and this segment.
             *
             * @param p a point.
             */
            public static int segmentDistSq(Vector2i p)
            {
                Vector2i ap = p - a;
                int dotprod = ab.dot(ap);
                int projlenSq;

                if (dotprod <= 0.0)
                {
                    projlenSq = 0.0;
                }
                else
                {
                    ap = ab - ap;
                    dotprod = ab.dot(ap);

                    if (dotprod <= 0.0)
                    {
                        projlenSq = 0.0;
                    }
                    else
                    {
                        projlenSq = dotprod * dotprod / ab.squaredLength();
                    }
                }

                return ap.squaredLength() - projlenSq;
            }

            /**
             * Returns true if this segment intersects the given segment.
             *
             * @param s a segment.
             */
            public static bool intersects(seg2 s)
            {
                Vector2i aa = s.a - a;
                int det = cross(ab, s.ab);
                int t0 = cross(aa, s.ab) / det;

                if (t0 > 0 && t0 < 1)
                {
                    int t1 = cross(aa, ab) / det;
                    return t1 > 0 && t1 < 1;
                }

                return false;
            }

            public static bool intersects(seg2 s, int t0)
            {
                Vector2i aa = s.a - a;
                int det = cross(ab, s.ab);
                int t0 = cross(aa, s.ab) / det;

                if (t0 > 0 && t0 < 1)
                {
                    int t1 = Vector2i.Cross(aa, ab) / det;
                    return t1 > 0 && t1 < 1;
                }

                return false;
            }

            /**
             * Returns true if this segment intersects the given segment. If there
             * is an intersection it is returned in the vector.
             *
             * @param s a segment.
             * @param i where to store the intersection point, if any.
             */
            public static bool intersects(seg2 s, Vector2i i)
            {
                Vector2i aa = s.a - a;
                int det = cross(ab, s.ab);
                t0 = cross(aa, s.ab) / det;

                if (t0 > 0 && t0 < 1)
                {
                    int t1 = cross(aa, ab) / det;
                    return t1 > 0 && t1 < 1;
                }

                return false;
            }

            /**
             * Returns true if this segment is inside or intersects the given
             * bounding box.
             *
             * @param r a bounding box.
             */
            public static bool intersects(box2<T> r)
            {
                Vector2i b = a + ab;
                if (r.contains(a) || r.contains(b))
                {
                    return true;
                }

                box2<T> int = box2<T>(a, b);
                if (t.xmin > r.xmax || t.xmax < r.xmin || t.ymin > r.ymax || t.ymax < r.ymin)
                {
                    return false;
                }

                int p0 = cross(ab, Vector2i(r.xmin, r.ymin) - a);
                int p1 = cross(ab, Vector2i(r.xmax, r.ymin) - a);
                if (p1 * p0 <= 0)
                {
                    return true;
                }
                int p2 = cross(ab, Vector2i(r.xmin, r.ymax) - a);
                if (p2 * p0 <= 0)
                {
                    return true;
                }
                int p3 = cross(ab, Vector2i(r.xmax, r.ymax) - a);
                if (p3 * p0 <= 0)
                {
                    return true;
                }

                return false;
            }

            /**
             * Returns true if this segment, with the given width, contains the given
             * point. More precisely this method returns true if the stroked path
             * defined from this segment, with a cap "butt" style, contains the given
             * point.
             *
             * @param p a point.
             * @param w width of this segment.
             */
            public static bool contains(Vector2i p, int w)
            {
                Vector2i ap = p - a;
                int dotprod = ab.dot(ap);
                int projlenSq;

                if (dotprod <= 0.0)
                {
                    return false;
                }
                else
                {
                    ap = ab - ap;
                    dotprod = ab.dot(ap);
                    if (dotprod <= 0.0)
                    {
                        return false;
                    }
                    else
                    {
                        projlenSq = dotprod * dotprod / ab.squaredLength();
                        return ap.squaredLength() - projlenSq < w * w;
                    }
                }
            }
        };

        typedef seg2<float> seg2f;

        typedef seg2<double> seg2d;

        template<typename T>
        inline seg2<T>::seg2(const Vector2i &a, const Vector2i b) : a(a), ab(b - a)
        {
        }
#endif
    }
}
