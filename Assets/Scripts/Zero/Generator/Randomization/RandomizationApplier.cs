using System.Linq;
using UnityEngine;
using Zero.Extensions;
using Zero.Generator.Entity;

namespace Zero.Generator.Randomization {
    public readonly struct RandomizationApplier {
        private readonly RandomizationRule[] _rules;

        public RandomizationApplier(RandomizationRule[] rules) => _rules = rules;

        public void ApplyRandomization(GameObject instance) {
            var rule = _rules.FirstOrDefault(node => node.target.IsMatch(instance.name));
            if (rule == null) {
                instance.transform
                    .Cast<Transform>()
                    .Select(t => t.gameObject)
                    .ForEach(ApplyRandomization);
                return;
            }

            var children = instance.transform.Cast<Transform>().ToList();
            var randomization = new LotteryMachine<Transform>(rule);
            var (chosen, probability) = randomization.Elect(children, child => child.name);

            children.Remove(chosen);
            children.Select(child => child.gameObject).ForEach(Object.DestroyImmediate);
            ApplyRandomization(chosen.gameObject);
        }
    }
}