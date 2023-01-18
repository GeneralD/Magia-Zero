using System;
using UnityEngine;

namespace Zero {
    [Serializable]
    public struct GeneratorRule {
        public NodeRule[] nodes;

        [Serializable]
        public class NodeRule {
            public string name;
        }
    }
}