namespace Zero.Extensions {
	public static class IntExtensions {
		public static bool IsPowOf2(this int n) {
			var u = (uint)n;
			return u > 0 && (u & u - 1) == 0;
		}
	}
}