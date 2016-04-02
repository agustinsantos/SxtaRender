using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sxta.Math;

namespace proland
{
    public class Geometry
    {
        /**
         * Returns the z coordinate of the cross product of this vector and of the
         * given vector.
         * @ingroup proland_math
         */
        public static int cross(Vector2i u, Vector2i v)
        {
            return u.X * v.Y - u.Y * v.X;
        }

        public static double cross(Vector2d u, Vector2d v)
        {
            return u.X * v.Y - u.Y * v.X;
        }

        /**
         * Returns the angle between this vector and the given vector. The
         * returned angle is in the 0 2.PI interval.
         * @ingroup proland_math
         */
        public static int angle(Vector2i u, Vector2i v)
        {
            int t = (int)Math.Atan2(u.X * v.Y - u.Y * v.X, u.X * v.X + u.Y * v.X);
            return t > 0 ? t : t + 2 * (int)(Math.PI);
        }

        public static float angle(Vector2f u, Vector2f v)
        {
            float t = (float)Math.Atan2(u.X * v.Y - u.Y * v.X, u.X * v.X + u.Y * v.Y);
            return t > 0 ? t : t + 2 * (float)(Math.PI);
        }
        /**
         * Returns the intersection point between two half lines with widths.
         * The two half lines are defined by ab and ac, with widths width1 and
         * width2 respectively. The returned point is the intersection between
         * the lines parallel to ab and ac and at a distance width1/2 and
         * width2/2 from these lines. There are four such intersections: the
         * returned one is the one inside the sector defined by the ab and ac
         * half lines.
         * @ingroup proland_math
         *
         * @param a extremity of ab and ac half lines.
         * @param b defines the ab half line.
         * @param c defines the ac half line.
         * @param width1 width of the ab half line.
         * @param width2 width of the ac half line.
         */
        public static Vector2i corner(Vector2i a, Vector2i b, Vector2i c, int width1, int width2)
        {
            int dx0 = b.X - a.X;
            int dy0 = b.Y - a.Y;
            int dx1 = c.X - a.X;
            int dy1 = c.Y - a.Y;
            int k0 = width1 / (int)(2 * Math.Sqrt(dx0 * dx0 + dy0 * dy0));
            int k1 = width2 / (int)(2 * Math.Sqrt(dx1 * dx1 + dy1 * dy1));
            int x0, y0, x1, y1;
            if (dx0 * dy1 - dy0 * dx1 > 0)
            {
                x0 = a.X - k0 * dy0;
                y0 = a.Y + k0 * dx0;
                x1 = a.X + k1 * dy1;
                y1 = a.Y - k1 * dx1;
            }
            else
            {
                x0 = a.X + k0 * dy0;
                y0 = a.Y - k0 * dx0;
                x1 = a.X - k1 * dy1;
                y1 = a.Y + k1 * dx1;
            }
            int det = dy0 * dx1 - dx0 * dy1;
            int t;
            if (det == 0)
            {
                t = 0;
            }
            else
            {
                t = (dy1 * (x0 - x1) - dx1 * (y0 - y1)) / det;
            }
            return new Vector2i(x0 + t * dx0, y0 + t * dy0);
        }

        public static Vector2d corner(Vector2d a, Vector2d b, Vector2d c, double width1, double width2)
        {
            double dx0 = b.X - a.X;
            double dy0 = b.Y - a.Y;
            double dx1 = c.X - a.X;
            double dy1 = c.Y - a.Y;
            double k0 = width1 / (2 * Math.Sqrt(dx0 * dx0 + dy0 * dy0));
            double k1 = width2 / (2 * Math.Sqrt(dx1 * dx1 + dy1 * dy1));
            double x0, y0, x1, y1;
            if (dx0 * dy1 - dy0 * dx1 > 0)
            {
                x0 = a.X - k0 * dy0;
                y0 = a.Y + k0 * dx0;
                x1 = a.X + k1 * dy1;
                y1 = a.Y - k1 * dx1;
            }
            else
            {
                x0 = a.X + k0 * dy0;
                y0 = a.Y - k0 * dx0;
                x1 = a.X - k1 * dy1;
                y1 = a.Y + k1 * dx1;
            }
            double det = dy0 * dx1 - dx0 * dy1;
            double t;
            if (det == 0)
            {
                t = 0;
            }
            else
            {
                t = (dy1 * (x0 - x1) - dx1 * (y0 - y1)) / det;
            }
            return new Vector2d(x0 + t * dx0, y0 + t * dy0);
        }
        /**
         * Returns true if the given point is inside this bounding box.
         * @ingroup proland_math
         *
         * @param a an arbitrary point.
         */
         //TODO
        public bool clipPoint(Box2i box, Vector2i a)
        {
            return box.contains(a);
        }

        public bool clipPoint(Box2d box, Vector2i a)
        {
            return box.contains(a);
        }

        /**
         * Returns true if the given box is inside or intersects this bounding box.
         * @ingroup proland_math
         *
         * @param a an arbitrary bounding box.
         */
        public bool clipRectangle(Box2i box, Box2i a)
        {
            return a.xmax >= box.xmin && a.xmin <= box.xmax && a.ymax >= box.ymin && a.ymin <= box.ymax;
        }

        public bool clipRectangle(Box2d box, Box2d a)
        {
            return a.xmax >= box.xmin && a.xmin <= box.xmax && a.ymax >= box.ymin && a.ymin <= box.ymax;
        }

        /**
         * Alias for clipRectangle.
         * @ingroup proland_math
         */

        public bool intersects(Box2i box, Box2i a)
        {
            return a.xmax >= box.xmin && a.xmin <= box.xmax && a.ymax >= box.ymin && a.ymin <= box.ymax;
        }

        public bool intersects(Box2d box, Box2d a)
        {
            return a.xmax >= box.xmin && a.xmin <= box.xmax && a.ymax >= box.ymin && a.ymin <= box.ymax;
        }

        /**
         * Returns true if the given segment is inside or may intersect this
         * bounding box.
         * @ingroup proland_math
         *
         * @param a a segment extremity.
         * @param b the other segment extremity.
         */
        public bool clipSegment(Box2i box, Vector2i a, Vector2i b)
        {
            if (box.contains(a) || box.contains(b))
            {
                return true;
            }

            Box2i ab = new Box2i(a.X, a.Y, b.X, b.Y);

            if (ab.xmax < box.xmin || ab.xmin > box.xmax || ab.ymax < box.ymin || ab.ymin > box.ymax)
            {
                return false;
            }

            // conservative result: potential intersection
            return true;
        }

        public bool clipSegment(Box2d box, Vector2d a, Vector2d b)
        {
            if (box.contains(a) || box.contains(b))
            {
                return true;
            }

            Box2d ab = new Box2d(a.X, a.Y, b.X, b.Y);

            if (ab.xmax < box.xmin || ab.xmin > box.xmax || ab.ymax < box.ymin || ab.ymin > box.ymax)
            {
                return false;
            }

            // conservative result: potential intersection
            return true;
        }
        /**
         * Returns true if the given quadratic Bezier arc is inside or may
         * intersect this bounding box.
         * @ingroup proland_math
         *
         * @param a Bezier arc extremity.
         * @param b Bezier arc control point.
         * @param c the other Bezier arc extremity.
         */
        public bool clipQuad(Box2i box, Vector2i a, Vector2i b, Vector2i c)
        {
            if (box.contains(a) || box.contains(b) || box.contains(c))
            {
                return true;
            }
#if TODO
            Box2i abc = new Box2i(a.X, a.Y, b.X, b.Y).enlarge(c);

            if (abc.xmax < box.xmin || abc.xmin > box.xmax || abc.ymax < box.ymin || abc.ymin > box.ymax)
            {
                return false;
            }
#endif
            // conservative result: potential intersection
            return true;
        }

        public bool clipQuad(Box2d box, Vector2d a, Vector2d b, Vector2d c)
        {
            if (box.contains(a) || box.contains(b) || box.contains(c))
            {
                return true;
            }
#if TODO
            Box2d abc = new Box2d(a.X, a.Y, b.X, b.Y).enlarge(c);

            if (abc.xmax < box.xmin || abc.xmin > box.xmax || abc.ymax < box.ymin || abc.ymin > box.ymax)
            {
                return false;
            }
#endif
            // conservative result: potential intersection
            return true;
        }

        /**
         * Returns true if the given cubic Bezier arc is inside or may
         * intersect this bounding box.
         * @ingroup proland_math
         *
         * @param a Bezier arc extremity.
         * @param b first Bezier arc control point.
         * @param c second Bezier arc control point.
         * @param d the other Bezier arc extremity.
         */
        public bool clipCubic(Box2i box, Vector2i a, Vector2i b, Vector2i c, Vector2i d)
        {
            if (box.contains(a) || box.contains(b) || box.contains(c) || box.contains(d))
            {
                return true;
            }

            Box2i abcd = new Box2i(a.X, a.Y, b.X, b.Y).enlarge(new Box2i(c.X, c.Y, d.X, d.Y));

            if (abcd.xmax < box.xmin || abcd.xmin > box.xmax || abcd.ymax < box.ymin || abcd.ymin > box.ymax)
            {
                return false;
            }

            // conservative result: potential intersection
            return true;
        }
        public bool clipCubic(Box2d box, Vector2d a, Vector2d b, Vector2d c, Vector2d d)
        {
            if (box.contains(a) || box.contains(b) || box.contains(c) || box.contains(d))
            {
                return true;
            }

            Box2d abcd = new Box2d(a.X, a.Y, b.X, b.Y).enlarge(new Box2d(c.X, c.Y, d.X, d.Y));

            if (abcd.xmax < box.xmin || abcd.xmin > box.xmax || abcd.ymax < box.ymin || abcd.ymin > box.ymax)
            {
                return false;
            }

            // conservative result: potential intersection
            return true;
        }
    }
}
