using System;
using System.Collections.Generic;

namespace Sxta.OSG
{
    public static class GroupExtensions
    {
        public static void Apply<N>(this IEnumerable<N> items, Action action) where N : Node
        {
            foreach (var item in items)
            {
                 item.Apply(action);
            }
        }
    }
}