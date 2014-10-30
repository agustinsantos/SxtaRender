
namespace Sxta.Core
{
    public static class Std
    {
        /// <summary>
        /// Swaps two values.
        /// </summary>
        /// <typeparam name="T"> type of the valueC</typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

    }
}
