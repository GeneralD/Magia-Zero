using System;

namespace Zero.Generator.Entity {
	[Serializable]
	public struct TraitDataRule {
		public string label;
		public string value;
		public TargetSubject[] conditions;
	}
}