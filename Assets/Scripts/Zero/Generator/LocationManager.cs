using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Zero.Utility;

namespace Zero.Generator {
	public class LocationManager {
		private const string ImageOutputDirectoryName = "images";
		private const string ModelOutputDirectoryName = "models";
		private readonly Regex _integerFormatRegex = new(@"%(0(?<digits>\d*))?d");

		private readonly bool _hashFilename;
		private readonly string _filenameFormat;
		private readonly string _outputDirectory;
		private readonly string _baseUri;

		public LocationManager(bool hashFilename, string filenameFormat, string outputDirectory, string baseUri) {
			_hashFilename = hashFilename;
			_filenameFormat = filenameFormat;
			_outputDirectory = outputDirectory;
			_baseUri = baseUri;
		}

		public bool IsValid =>
			!string.IsNullOrEmpty(_outputDirectory) &&
			!string.IsNullOrEmpty(_baseUri) &&
			_integerFormatRegex.IsMatch(_filenameFormat);

		public void InitializeDirectories() {
			Directory.CreateDirectory(_outputDirectory);
			Directory.CreateDirectory(Path.Combine(_outputDirectory, ImageOutputDirectoryName));
			Directory.CreateDirectory(Path.Combine(_outputDirectory, ModelOutputDirectoryName));
		}

		private string Filename(int index) {
			var filename = _integerFormatRegex
				.Replace(_filenameFormat, match => {
					var digits = match.Groups["digits"].Value;
					return index.ToString($"D{digits}");
				});
			return _hashFilename
				? Keccak256
					.ComputeHash(filename)
					.Select(b => b.ToString("x2"))
					.Aggregate("", string.Concat)
				: filename;
		}

		public string MetadataOutputPath(int index) =>
			Path.Combine(_outputDirectory, Filename(index) + ".json");

		public string ModelOutputPath(int index) =>
			Path.Combine(_outputDirectory, ModelOutputDirectoryName, Filename(index) + ".glb");

		public string ModelURL(int index) =>
			Path.Combine(_baseUri, ModelOutputDirectoryName, Filename(index) + ".glb");

		public string ImageURL(int index) =>
			Path.Combine(_baseUri, ImageOutputDirectoryName, Filename(index) + ".png");
	}
}