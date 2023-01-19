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
        public RandomizationRule randomization;
    }

    [Serializable]
    public class TargetSubject {
        public string name;
        public bool useRegex = false;
        public RandomizationRule randomization;
    }

    [Serializable]
    public class RandomizationRule {
        public ProbabilityRule[] probabilities;
    }

    [Serializable]
    public class ProbabilityRule {
        public TargetSubject target;
        [Min(0)] public double weight = 1;
        public bool divideByMatches = false;
    }
}