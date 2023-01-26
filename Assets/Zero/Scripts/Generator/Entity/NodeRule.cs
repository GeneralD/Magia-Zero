using System;

namespace Zero.Generator.Entity {
    [Serializable]
    public class NodeRule {
        public TargetSubject target;
        public RandomizationRule randomization;
    }
}