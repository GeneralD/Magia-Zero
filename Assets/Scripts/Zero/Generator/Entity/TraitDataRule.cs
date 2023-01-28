using System;

namespace Zero.Generator.Entity {
	[Serializable]
	public struct TraitDataRule {
		public string label;
		public string value;
		public Requirement requirement;
		public TargetSubject[] conditions;

		[Serializable]
		public enum Requirement {
			Any,
			All,
		}
	}
}