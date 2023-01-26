using System;

namespace Zero.Generator.Entity {
    [Serializable]
    public struct CombinationRule {
        public TargetSubject target;
        public TargetSubject[] dependencies;
    }
}