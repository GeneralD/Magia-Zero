using System;
using System.Collections.Generic;

namespace Zero {
    public readonly struct DoubleRange {
        public readonly double Lower;
        public readonly double Upper;

        public DoubleRange(double lower, double upper) {
            Lower = lower;
            Upper = upper;
        }

        public double Length => Upper - Lower;

        private static bool _intersects_exclusive(DoubleRange range1, DoubleRange range2) =>
            range1.Lower < range2.Lower && range1.Upper > range2.Lower;

        private static bool _intersects_inclusive(DoubleRange range1, DoubleRange range2) =>
            range1.Lower <= range2.Lower && range1.Upper >= range2.Upper;

        /// <summary>
        /// Tests if this range interests with another
        /// </summary>
        /// <param name="range">the other range</param>
        /// <returns>true if they intersect</returns>
        public bool IntersectsExclusive(DoubleRange range) =>
            _intersects_exclusive(this, range) || _intersects_exclusive(range, this);

        public bool IntersectsInclusive(DoubleRange range) =>
            _intersects_inclusive(this, range) || _intersects_inclusive(range, this);

        private static bool _touches(DoubleRange range1, DoubleRange range2) =>
            Math.Abs(range1.Upper - range2.Lower) == 0d;

        /// <summary>
        /// Tests if this range touches another. For example (1-2) touches (3,5) but (1,2) does not touch (4,5)
        /// </summary>
        /// <param name="range">the other range</param>
        /// <returns>true if they touch</returns>
        public bool Touches(DoubleRange range) => _touches(this, range) || _touches(range, this);

        /// <summary>
        /// Tests if this range contains a specific double
        /// </summary>
        /// <param name="n">the double</param>
        /// <returns>true if the number is contained</returns>
        public bool Contains(double n) => Lower <= n && n <= Upper;

        /// <summary>
        /// Join this range with another and return a single range that contains them both. The ranges must either touch or interest.
        /// for example (0,2) amd (3,7) will yield (0,7)
        /// </summary>
        /// <param name="range">the other range</param>
        /// <returns>the merged range</returns>
        public DoubleRange JoinWith(DoubleRange range) {
            if (!IntersectsExclusive(range) && !Touches(range))
                throw new ArgumentException("Ranges cannot be joined because they do not touch or overlap");

            var newUpper = Math.Max(Upper, range.Upper);
            var newLower = Math.Min(Lower, range.Lower);
            return new DoubleRange(newLower, newUpper);
        }

        public IEnumerable<double> Values => GetValues(1d);

        public IEnumerable<double> GetValues(double step) {
            if (step <= 0.0)
                throw new ArgumentOutOfRangeException(nameof(step), "must be positive");

            var i = Lower;
            while (i <= Upper) {
                yield return i;
                i += step;
            }
        }
    }
}