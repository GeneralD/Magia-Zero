using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
		internal string outputDirectoryUri = "~Downloads/Zero";

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

		private readonly string imageOutputDirectoryName = "images";
		private readonly string vrmOutputDirectoryName = "models";

		public Generator() { }

		public void Generate() {
			if (!IsValid) return;

			var metadataFactory = new MetadataFactory(rule.metadataRule);

			GeneratedInstances(rootObject)
				.Zip(Indices, (generated, index) => (generated, index))
				.ForEach(v => {
					// TODO: Export as a VRM
					v.generated.name += $" ({v.index})";

					var filename = Filename(v.index);

					var imageURL = Path.Combine(
						rule.metadataRule.baseUri,
						imageOutputDirectoryName,
						filename + ".png");

					var animationURL = Path.Combine(
						rule.metadataRule.baseUri,
						vrmOutputDirectoryName,
						filename + ".glb");
					;
					var metadataJson = metadataFactory.Json(v.generated, v.index, imageURL, animationURL);
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

		private string Filename(int index) {
			var integerFormatRegex = new Regex(@"%(0(?<digits>\d*))?d");
			var filename = integerFormatRegex
				.Replace(filenameFormat, match => {
					var digits = match.Groups["digits"].Value;
					return index.ToString($"D{digits}");
				});
			return !hashFilename
				? filename
				: Keccak256
					.ComputeHash(filename)
					.Select(b => b.ToString("x2"))
					.Aggregate("", string.Concat);
		}

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