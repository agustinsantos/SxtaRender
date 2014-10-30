using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxta.Math
{
    public static class MathHelper
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static double ToRadians(double degrees)
        {
            return System.Math.PI * degrees / 180;
        }
        public static double ToDegrees(double radians)
        {
            return radians * 180 / System.Math.PI;
        }
    }
}
