using System.Collections.Generic;

namespace Zero.Extensions {
    public static class EnumerableExtensions {
        public static void ForEach<T>(this IEnumerable<T> xs, System.Action<T> action) {
            foreach (var x in xs) action(x);
        }
    }
}