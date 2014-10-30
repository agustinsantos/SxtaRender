using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.Render.Core
{
    public static class ListExtensions
    {
        public static void Resize<T>(this List<T> list, int count, T elem)
        {
            int actualSize = list.Count;
            for (int i = actualSize; i < count; i++)
                list.Add(elem);
        }
        public static void Resize<T>(this List<T> list, int count)
        {
            int actualSize = list.Count;
            for (int i = actualSize; i < count; i++)
                list.Add(default(T));
        }
    }
}
