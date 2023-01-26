using System;
using UnityEngine.Serialization;

namespace Zero.Generator.Entity {
    [Serializable]
    public class RandomizationRule {
        public TargetSubject target;
        public ProbabilityRule[] probabilityRules;
    }
}