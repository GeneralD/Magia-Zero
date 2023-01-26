using System.Linq;
using UnityEngine;
using Zero.Extensions;
using Zero.Generator.Entity;

namespace Zero.Generator.Combination {
	public readonly struct CombinationChecker {
		private readonly CombinationRule[] _rules;

		public CombinationChecker(CombinationRule[] rules) => _rules = rules;

		public bool IsValid(GameObject instance) {
			var names = instance.transform
				.SelfAndDescendants()
				.Select(t => t.name);

			return _rules
				.Where(rule => names.Any(rule.target.IsMatch))
				.SelectMany(rule => rule.dependencies)
				.Combination(instance.transform.SelfAndDescendants())
				.Select(v => (dependency: v.Item1, transform: v.Item2))
				.All(tuple => tuple.dependency.IsMatch(tuple.transform.name));
		}
	}
}