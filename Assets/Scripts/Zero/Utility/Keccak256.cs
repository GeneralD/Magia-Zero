using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zero.Utility {
	public static class Keccak256 {
		private static readonly Keccak256Hasher Hasher = new();

		public static byte[] ComputeHash(byte[] input) {
			if (Monitor.TryEnter(Hasher))
				try {
					return ComputeHash(Hasher, input, true);
				} finally {
					Monitor.Exit(Hasher);
				}

			return ComputeHash(new Keccak256Hasher(), input, false);
		}

		public static byte[] ComputeHash(string input) => ComputeHash(Encoding.UTF8.GetBytes(input));

		public static async Task<byte[]> ComputeHashAsync(Stream input,
			CancellationToken cancellationToken = default) {
			var hasher = new Keccak256Hasher();
			var buffer = new byte[8192];
			while (true) {
				var r = await input.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
				if (r == 0) {
					var hash = new byte[32];
					hasher.Finalize(hash, 0);
					return hash;
				}

				hasher.BlockUpdate(buffer, 0, r);
			}
		}

		public static byte[] ComputeHash(Keccak256Hasher hasher, byte[] input, bool reset) {
			if (reset) {
				hasher.Reset();
			}

			hasher.BlockUpdate(input, 0, input.Length);
			var hash = new byte[32];
			hasher.Finalize(hash, 0);
			return hash;
		}

		public static string ComputeEthereumFunctionSelector(string functionSignature, bool prefix0x = true) {
			var hash = ComputeHash(functionSignature);
			var s = prefix0x ? "0x" : "";
			s += hash[0].ToString("x2");
			s += hash[1].ToString("x2");
			s += hash[2].ToString("x2");
			s += hash[3].ToString("x2");
			return s;
		}
	}
}