using System;
using UnityEngine;

namespace Zero {
    [Serializable]
    public struct GeneratorRule {
        public NodeRule[] nodes;
    }

    [Serializable]
    public class NodeRule {
        public TargetSubject target;
    }

    [Serializable]
    public class TargetSubject {
        public string name;
        public bool useRegex = false;
    }
}