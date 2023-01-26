using System;
using System.Text.RegularExpressions;

namespace Zero.Generator.Entity {
    [Serializable]
    public class TargetSubject {
        public string name;
        public bool useRegex = true;
        public RandomizationRule randomization;

        public bool IsMatch(string text)
            => new Regex(useRegex ? name : $"^{name}$").IsMatch(text);
    }
}