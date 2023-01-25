using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Zero {
    public class Generator : MonoBehaviour {
        [SerializeField] internal GameObject rootObject;
        [SerializeField] internal string outputDirectoryUri = "~Downloads/Zero";
        [SerializeField] internal string vrmOutputDirectoryName = "VRMs";
        [SerializeField] internal uint startIndex = 1;
        [SerializeField] internal uint quantity = 10;
        [SerializeField] internal bool overwriteExist = false;
        [SerializeField] internal string nameFormat = "%d";
        [SerializeField] internal bool hashName = false;

        [SerializeField] private GeneratorRule rule;

        public Generator() { }

        public void Generate() {
            if (!IsValid) return;

            var instance = Instantiate(rootObject);
            ApplyRule(instance);
        }

        private bool IsValid =>
            rootObject != null &&
            !string.IsNullOrEmpty(outputDirectoryUri) &&
            !string.IsNullOrEmpty(vrmOutputDirectoryName) &&
            !string.IsNullOrEmpty(nameFormat);

        private void ApplyRule(GameObject instance) {
            var nodeRule = rule.nodes.FirstOrDefault(node => node.target.IsMatch(instance.name));
            if (nodeRule == null) {
                instance.transform
                    .Cast<Transform>()
                    .Select(t => t.gameObject)
                    .ToList()
                    .ForEach(ApplyRule);
                return;
            }

            var children = instance.transform.Cast<Transform>().ToList();

            var initDict = new Dictionary<Transform, double>();
            foreach (var child in children) {
                initDict[child] = 1;
            }

            var dictionary = nodeRule.randomization.probabilities.Aggregate(initDict,
                (accum, probability) => {
                    var matches = children.Where(child => probability.target.IsMatch(child.name)).ToList();
                    var weight = probability.Weight(matches.Count());
                    foreach (var match in matches) {
                        var newValue = weight * accum.GetValueOrDefault(match, 1);
                        accum[match] = newValue;
                    }

                    return accum;
                });

            var result = dictionary.WeightedRandom();
            var item = result.Item1;
            children.Remove(item);
            children.Select(child => child.gameObject).ForEach(DestroyImmediate);
            ApplyRule(item.gameObject);
        }
    }
}