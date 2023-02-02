using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UniVRM10;
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
		private VRM10ObjectMeta vrmMeta;

		[SerializeField]
		private GeneratorRule rule;

		public Generator() { }

		public void Generate() {
			if (rootObject == null) return;

			var locationManager = new LocationManager(
				filenameFormat, hashFilename, outputDirectoryUri, rule.metadataRule.baseUri);
			if (!locationManager.IsValid) return;

			var metadataFactory = new MetadataFactory(rule.metadataRule);
			var modelDatalizer = new ModelDatalizer(vrmMeta);

			// Automatically destroy all generated things as soon as operation is done
			using var temporary = new DisposableContainerWrapper(new GameObject("Container"));
			using var photoBooth = new PhotoBooth(temporary.Container.transform, PhotoBooth.Position.Front);

			GeneratedInstances(rootObject, temporary.Container.transform)
				.Zip(Indices(locationManager), (generated, index) => (generated, index))
				.ForEach(v => {
					v.generated.name += $" ({v.index})";

					var modelData = modelDatalizer.Datalize(v.generated);
					FileUtility.CreateDataFile(locationManager.ModelFilePath(v.index), modelData);

					var imageData = photoBooth.Shoot(PhotoBooth.Format.JPG);
					FileUtility.CreateDataFile(locationManager.ImageFilePath(v.index), imageData);

					var metadataJson = metadataFactory.Json(v.generated, v.index,
						locationManager.ImageURL(v.index), locationManager.ModelURL(v.index));
					FileUtility.CreateTextFile(locationManager.MetadataOutputPath(v.index), metadataJson);
				});
		}

		private IEnumerable<int> Indices(LocationManager locationManager) {
			var range = Enumerable.Range((int)startIndex, (int)quantity);
			return overwriteExist ? range : range.Where(i => !File.Exists(locationManager.MetadataOutputPath(i)));
		}

		private IEnumerable<GameObject> GeneratedInstances(GameObject sample, Transform parent) {
			var randomizationApplier = new RandomizationApplier(rule.randomizationRules);
			var combinationChecker = new CombinationChecker(rule.combinationRules);

			while (true) {
				var instance = Instantiate(sample, parent);
				instance.transform.position = Vector3.zero;
				randomizationApplier.ApplyRandomization(instance);
				if (combinationChecker.IsValid(instance)) yield return instance;
				else DestroyImmediate(instance);
			}
			// ReSharper disable once IteratorNeverReturns
		}
	}
}