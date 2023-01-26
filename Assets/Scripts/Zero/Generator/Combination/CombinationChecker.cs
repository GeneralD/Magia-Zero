using System.Linq;
using UnityEngine;
using Zero.Generator.Entity;

namespace Zero.Generator.Combination {
    public readonly struct CombinationChecker {
        private readonly CombinationRule[] _rules;

        public CombinationChecker(CombinationRule[] rules) => _rules = rules;

        public bool IsValid(GameObject instance) {
            return true;
            // return _rules
            //     .Where(rule => { rule.target.IsMatch() })
            //     .All(rule => { });
        }
    }
}