using System;
using UnityEngine;

namespace Zero.Generator.Entity {
	[Serializable]
	public class ProbabilityRule {
		public TargetSubject target;
		[Min(0)] public double weight = 1;
		public bool divideByMatches = false;

		public double Weight(int numberOfCandidates) =>
			divideByMatches
				? weight / numberOfCandidates
				: weight;
	}
}