using UnityEngine;
using Zero.Extensions;
using Zero.Generator.Entity;

namespace Zero.Generator.Metadata {
	public readonly struct MetadataFactory {
		private readonly MetadataRule _rule;

		public MetadataFactory(MetadataRule rule) => _rule = rule;

		public void Generate(GameObject instance) {
			// instance.transform.SelfAndDescendants()
		}
	}
}