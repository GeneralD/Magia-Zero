using System.Collections.Generic;
using System.Linq;

namespace Zero.Extensions {
	public static class EnumerableExtensions {
		public static void ForEach<T>(this IEnumerable<T> xs, System.Action<T> action) {
			foreach (var x in xs) action(x);
		}

		public static void ForEach<T>(this IEnumerable<T> xs, System.Action<T, int> action) {
			var i = 0;
			foreach (var x in xs) action(x, i++);
		}

		public static IEnumerable<(TX, TY)> Combination<TX, TY>(this IEnumerable<TX> xs, IEnumerable<TY> ys) =>
			xs.SelectMany(x => ys.Select(y => (x, y)));
	}
}