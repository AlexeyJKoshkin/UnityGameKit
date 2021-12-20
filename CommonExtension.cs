using System;
using System.Collections.Generic;

namespace GameKit {
    public static class CommonExtension
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if(collection == null || action == null) return;
            foreach (var item in collection)
            {
                action(item);
            }
        }
    }
}