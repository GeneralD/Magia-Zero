using System.Text.RegularExpressions;

namespace Zero {
    internal static class Extensions {
        internal static bool IsMatch(this TargetSubject subject, string text)
            => new Regex(subject.useRegex ? subject.name : $"^{subject.name}$").IsMatch(text);

        internal static double Weight(this ProbabilityRule probability, int numberOfCandidates) =>
            probability.divideByMatches
                ? probability.weight / numberOfCandidates
                : probability.weight;
    }
}