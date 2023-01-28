using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Zero.Generator.Metadata {
	internal class TextFormatter {
		private readonly int _index;
		private readonly IEnumerable<Metadata.Attribute> _attributes;

		internal TextFormatter(int index, IEnumerable<Metadata.Attribute> attributes) {
			_index = index;
			_attributes = attributes;
		}

		internal string Format(string text) {
			var integerFormatRegex = new Regex(@"%(0(?<digits>\d*))?d");
			text = integerFormatRegex
				.Replace(text, match => {
					var digits = match.Groups["digits"].Value;
					return _index.ToString($"D{digits}");
				});

			var attributeRegex = new Regex(@"\$\{(?<attr>.*)\}");
			text = attributeRegex
				.Replace(text,
					match => {
						var label = match.Groups["attr"].Value;
						var attr = _attributes.FirstOrDefault(attr => attr.Label == label);
						return attr?.Value ?? "";
					});
			return text;
		}
	}
}