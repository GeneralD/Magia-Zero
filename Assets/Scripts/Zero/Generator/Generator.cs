using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UniVRM10;
using Zero.Generator.Combination;
using Zero.Generator.Entity;
using Zero.Generator.FileLocation;
using Zero.Generator.Image;
using Zero.Generator.Metadata;
using Zero.Generator.Model;
using Zero.Generator.Randomization;
using Zero.Generator.TextureOptimization;
using Zero.Utility;

namespace Zero.Generator {
	public class Generator : MonoBehaviour {
		[SerializeField]
		internal GameObject rootObject;

		[SerializeField]
		internal string outputDirectoryUri = "~/Downloads/Zero";

		[SerializeField]
		internal ImageFormat imageFormat = ImageFormat.JPG;

		[SerializeField]
		internal int imageSize = 1280;

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

		public void Generate() {
			if (rootObject == null) return;

			var locationManager = new LocationManager(
				filenameFormat, hashFilename, outputDirectoryUri, rule.metadataRule.baseUri, imageFormat);
			if (!locationManager.IsValid) return;

			var metadataFactory = new MetadataFactory(rule.metadataRule);
			var modelDatalizer = new ModelDatalizer(vrmMeta);

			// Automatically destroy all generated things as soon as operation is done
			using var temporary = new DisposableContainerWrapper(new GameObject("Container"));
			using var photoBooth = new PhotoBooth(temporary.Container.transform, Image.CameraPosition.Front);

			var instances = GeneratedInstances(rootObject, temporary.Container.transform)
				.Zip(Indices(locationManager), (generated, index) => (generated, index));

			foreach (var (instance, index) in instances) {
				instance.name += $" ({index})";

				var imageTexture = photoBooth.Shoot(imageSize);
				var imageData = imageFormat switch {
					ImageFormat.PNG => imageTexture.EncodeToPNG(),
					ImageFormat.JPG => imageTexture.EncodeToJPG(),
					_ => throw new ArgumentOutOfRangeException()
				};
				FileUtility.CreateDataFile(locationManager.ImageFilePath(index), imageData);

				var modelData = modelDatalizer.Datalize(instance, imageTexture);
				FileUtility.CreateDataFile(locationManager.ModelFilePath(index), modelData);

				var metadataJson = metadataFactory.Json(instance, index,
					locationManager.ImageURL(index), locationManager.ModelURL(index));
				FileUtility.CreateTextFile(locationManager.MetadataOutputPath(index), metadataJson);
			}
		}

		private IEnumerable<int> Indices(LocationManager locationManager) {
			var range = Enumerable.Range((int)startIndex, (int)quantity);
			return overwriteExist ? range : range.Where(i => !File.Exists(locationManager.MetadataOutputPath(i)));
		}

		private IEnumerable<GameObject> GeneratedInstances(GameObject sample, Transform parent) {
			var randomizationApplier = new RandomizationApplier(rule.randomizationRules);
			var combinationChecker = new CombinationChecker(rule.combinationRules);
			var textureMerger = new TextureMerger();

			while (true) {
				var instance = Instantiate(sample, parent);
				instance.transform.localPosition = Vector3.zero;
				randomizationApplier.ApplyRandomization(instance);

				if (!combinationChecker.IsValid(instance)) {
					DestroyImmediate(instance);
					continue;
				}

				textureMerger.Apply(instance);
				yield return instance;
			}
			// ReSharper disable once IteratorNeverReturns
		}
	}
}