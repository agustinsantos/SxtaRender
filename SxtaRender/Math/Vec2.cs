using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Sxta.Math
{
    public class Vec2<T>
    {
        /*
         * x coordinate.
         */
        public T x;

        /*
         * x coordinate.
         */
        public T y;

        /*
         * Creates a new, uninitialized vector.
         */
        public Vec2() { }

        /*
         * Creates a new vector with the given coordinates.
         */
        public Vec2(T xi, T yi)
        {
            this.x = xi;
            this.y = yi;
        }

        /*
         * Creates a new vector with the given coordinates.
         */
        public Vec2(T[] v)
        {
            Contract.Assert(v != null && v.Length == 2);
            this.x = v[0];
            this.y = v[1];
        }

        /*
         * Creates a new vector as a copy of the given vector.
         */
        public Vec2(Vec2<T> v)
        {
            this.x = v.x;
            this.y = v.y;
        }
    }
}
