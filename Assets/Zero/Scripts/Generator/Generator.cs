using System;
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
            Debug.Log(rootObject.name);
        }

        private bool IsValid =>
            rootObject != null &&
            !string.IsNullOrEmpty(outputDirectoryUri) &&
            !string.IsNullOrEmpty(vrmOutputDirectoryName) &&
            !string.IsNullOrEmpty(nameFormat);
    }
}