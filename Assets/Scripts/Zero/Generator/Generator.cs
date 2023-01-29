using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Zero.Extensions;
using Zero.Generator.Combination;
using Zero.Generator.Entity;
using Zero.Generator.Metadata;
using Zero.Generator.Randomization;
using Zero.Utility;

namespace Zero.Generator {
	public class Generator : MonoBehaviour {
		[SerializeField]
		internal GameObject rootObject;

		[SerializeField]
		internal string outputDirectoryUri = "~/Downloads/Zero";

		[SerializeField]
		internal uint startIndex = 1;

		[SerializeField]
		internal uint quantity = 10;

		[SerializeField]
		internal bool overwriteExist;

		[SerializeField]
		internal string filenameFormat = "%d";

		[SerializeField]
		internal bool hashFilename;

		[SerializeField]
		private GeneratorRule rule;

		public Generator() { }

		public void Generate() {
			if (rootObject == null) return;

			var locationManager = new LocationManager(
				filenameFormat, hashFilename, outputDirectoryUri, rule.metadataRule.baseUri);
			if (!locationManager.IsValid) return;

			var metadataFactory = new MetadataFactory(rule.metadataRule);
			var modelExporter = new ModelExporter();

			GeneratedInstances(rootObject)
				.Zip(Indices(locationManager), (generated, index) => (generated, index))
				.ForEach(v => {
					v.generated.name += $" ({v.index})";
					modelExporter.Export(v.generated, locationManager.ModelFilePath(v.index));

					var metadataJson = metadataFactory.Json(v.generated, v.index,
						locationManager.ImageURL(v.index), locationManager.ModelURL(v.index));
					FileUtility.CreateTextFile(locationManager.MetadataOutputPath(v.index), metadataJson);
				});
		}

		private IEnumerable<int> Indices(LocationManager locationManager) {
			var range = Enumerable.Range((int)startIndex, (int)quantity);
			return overwriteExist ? range : range.Where(i => !File.Exists(locationManager.MetadataOutputPath(i)));
		}

		private IEnumerable<GameObject> GeneratedInstances(GameObject sample) {
			// Automatically destroy all generated things as soon as operation is done
			using var temporary = new DisposableContainerWrapper(new GameObject("Container"));

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