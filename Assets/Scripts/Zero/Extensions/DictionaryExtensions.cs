using System;
using System.Collections.Generic;
using System.Linq;
using Zero.Utility;

namespace Zero.Extensions {
	public static class DictionaryExtensions {
		public static (TKey, double) WeightedRandom<TKey>(this IDictionary<TKey, double> dictionary) {
			var totalWeight = dictionary.Values.Aggregate(0d, (l, r) => l + r);
			if (totalWeight <= 0) return (dictionary.Keys.FirstOrDefault(), 0);
			var dict = dictionary.Aggregate(new Dictionary<TKey, DoubleRange>(), (accum, pair) => {
				var start = accum.Values
					.DefaultIfEmpty(new DoubleRange(0, 0))
					.Max(range => range.Upper);
				if (double.IsNaN(start)) start = 0;
				accum[pair.Key] = new DoubleRange(start, start + pair.Value);
				return accum;
			});
			var max = dict.Values.Max(range => range.Upper);
			var rand = new Random().NextDouble() * max;
			var selected = dict.FirstOrDefault(pair => pair.Value.Contains(rand));
			var weight = dictionary[selected.Key];
			return (selected.Key, weight / totalWeight);
		}
	}
}