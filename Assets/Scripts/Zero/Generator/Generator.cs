using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zero.Extensions;
using Zero.Generator.Entity;
using Zero.Generator.Randomization;

namespace Zero.Generator {
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

            GeneratedInstances(rootObject)
                .Take((int)quantity)
                .Select((o, i) => new { generated = o, index = i + startIndex })
                .ForEach(v => {
                    // TODO: Export as a VRM
                    Debug.Log($"Export index: {v.index}.");
                });
        }

        private bool IsValid =>
            rootObject != null &&
            !string.IsNullOrEmpty(outputDirectoryUri) &&
            !string.IsNullOrEmpty(vrmOutputDirectoryName) &&
            !string.IsNullOrEmpty(nameFormat);

        private IEnumerable<GameObject> GeneratedInstances(GameObject sample) {
            while (true) {
                var instance = Instantiate(sample);
                new RandomizationApplier(rule.randomizationRules).ApplyRandomization(instance);
                yield return instance;
            }
        }
    }
}