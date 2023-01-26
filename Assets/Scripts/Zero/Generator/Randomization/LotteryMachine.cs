using System;
using System.Collections.Generic;
using System.Linq;
using Zero.Generator.Entity;
using Zero.Extensions;

namespace Zero.Generator.Randomization {
	internal readonly struct LotteryMachine<T> {
		private readonly RandomizationRule _rule;

		internal LotteryMachine(RandomizationRule rule) => _rule = rule;

		internal (T, double) Elect(IEnumerable<T> candidates, Func<T, string> candidateName) {
			var xs = candidates.ToList();
			var initDict = xs.Aggregate(new Dictionary<T, double>(),
				(accum, candidate) => {
					accum[candidate] = 1;
					return accum;
				});

			var dictionary = _rule.probabilityRules.Aggregate(initDict,
				(accum, probability) => {
					var matches = xs.Where(candidate => probability.target.IsMatch(candidateName(candidate))).ToList();
					var weight = probability.Weight(matches.Count());
					matches.ForEach(match => {
						var newValue = weight * accum.GetValueOrDefault(match, 1);
						accum[match] = newValue;
					});

					return accum;
				});

			return dictionary.WeightedRandom();
		}
	}
}