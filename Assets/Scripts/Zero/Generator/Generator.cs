using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zero.Extensions;
using Zero.Generator.Combination;
using Zero.Generator.Entity;
using Zero.Generator.Metadata;
using Zero.Generator.Randomization;

namespace Zero.Generator {
	public class Generator : MonoBehaviour {
		[SerializeField]
		internal GameObject rootObject;

		[SerializeField]
		internal string outputDirectoryUri = "~Downloads/Zero";

		[SerializeField]
		internal string vrmOutputDirectoryName = "VRMs";

		[SerializeField]
		internal uint startIndex = 1;

		[SerializeField]
		internal uint quantity = 10;

		[SerializeField]
		internal bool overwriteExist = false;

		[SerializeField]
		internal string filenameFormat = "%d";

		[SerializeField]
		internal bool hashFilename = false;

		[SerializeField]
		private GeneratorRule rule;

		public Generator() { }

		public void Generate() {
			if (!IsValid) return;

			var metadataFactory = new MetadataFactory(rule.metadataRule);

			GeneratedInstances(rootObject)
				.Zip(Indices, (generated, index) => (generated, index))
				.ForEach(v => {
					// TODO: Export as a VRM
					v.generated.name += $" ({v.index})";
					var metadataJson = metadataFactory.Json(v.generated, v.index);
					// TODO: Delete next line
					Debug.Log(metadataJson);
				});
		}

		private bool IsValid =>
			rootObject != null &&
			!string.IsNullOrEmpty(outputDirectoryUri) &&
			!string.IsNullOrEmpty(vrmOutputDirectoryName) &&
			!string.IsNullOrEmpty(filenameFormat);

		private IEnumerable<int> Indices =>
			Enumerable.Range((int)startIndex, (int)quantity);

		private IEnumerable<GameObject> GeneratedInstances(GameObject sample) {
			// Automatically destroy all generated things as soon as operation is done
			// TODO: add `using` before next line
			var temporary = new DisposableContainerWrapper(new GameObject("Container"));

			var randomizationApplier = new RandomizationApplier(rule.randomizationRules);
			var combinationChecker = new CombinationChecker(rule.combinationRules);

			while (true) {
				var instance = Instantiate(sample, temporary.Container.transform);
				randomizationApplier.ApplyRandomization(instance);
				if (combinationChecker.IsValid(instance)) yield return instance;
				else DestroyImmediate(instance);
			}
			// ReSharper disable once IteratorNeverReturns
		}
	}
}