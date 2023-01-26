using System;

namespace Zero.Generator.Entity {
    [Serializable]
    public struct GeneratorRule {
        public RandomizationRule[] randomizationRules;
        public CombinationRule[] combinationRules;
    }
}