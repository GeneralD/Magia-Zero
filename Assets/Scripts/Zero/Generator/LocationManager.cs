using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Zero.Utility;

namespace Zero.Generator {
	public class LocationManager {
		private const string ImageOutputDirectoryName = "images";
		private const string ModelOutputDirectoryName = "models";
		private readonly Regex _integerFormatRegex = new(@"%(0(?<digits>\d*))?d");
		private readonly Regex _webUrlFormatRegex = new(@"https?://[\w!\?/\+\-_~=;\.,\*&@#\$%\(\)'\[\]]+");

		private readonly string _filenameFormat;
		private readonly bool _hashFilename;
		private readonly string _outputDirectory;
		private readonly string _baseUri;
		private readonly ImageFormat _imageFormat;
		private readonly IDictionary<int, string> _filenameCache = new Dictionary<int, string>();

		private const string MetadataExtension = "json";
		private const string ModelExtension = "vrm";

		private string ImageExtension =>
			_imageFormat switch {
				ImageFormat.PNG => "png",
				ImageFormat.JPG => "jpg",
				_ => throw new ArgumentOutOfRangeException()
			};

		public LocationManager(
			string filenameFormat,
			bool hashFilename,
			string outputDirectory,
			string baseUri,
			ImageFormat imageFormat) {
			_filenameFormat = filenameFormat;
			_hashFilename = hashFilename;
			_baseUri = baseUri;
			_imageFormat = imageFormat;

			var regex = new Regex(@"^~");
			var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			_outputDirectory = regex.Replace(outputDirectory, _ => homeDir);
		}

		public bool IsValid =>
			!string.IsNullOrEmpty(_outputDirectory) &&
			_integerFormatRegex.IsMatch(_filenameFormat) &&
			_webUrlFormatRegex.IsMatch(_baseUri);

		public string MetadataOutputPath(int index) =>
			Path.Combine(_outputDirectory, Filename(index, MetadataExtension));

		public string ModelFilePath(int index) =>
			Path.Combine(_outputDirectory, ModelOutputDirectoryName, Filename(index, ModelExtension));

		public string ImageFilePath(int index) =>
			Path.Combine(_outputDirectory, ImageOutputDirectoryName, Filename(index, ImageExtension));

		public string ModelURL(int index) =>
			Path.Combine(_baseUri, ModelOutputDirectoryName, Filename(index, ModelExtension));

		public string ImageURL(int index) =>
			Path.Combine(_baseUri, ImageOutputDirectoryName, Filename(index, ImageExtension));

		private string Filename(int index, string extension) => $"{Filename(index)}.{extension}";

		private string Filename(int index) {
			if (_filenameCache.TryGetValue(index, out var value)) return value;
			var filename = _integerFormatRegex
				.Replace(_filenameFormat, match => {
					var digits = match.Groups["digits"].Value;
					return index.ToString($"D{digits}");
				});
			return _filenameCache[index] = _hashFilename
				? Keccak256
					.ComputeHash(filename)
					.Select(b => b.ToString("x2"))
					.Aggregate("", string.Concat)
				: filename;
		}
	}
}