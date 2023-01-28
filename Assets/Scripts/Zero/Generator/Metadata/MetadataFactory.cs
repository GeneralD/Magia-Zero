using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;
using Zero.Extensions;
using Zero.Generator.Entity;

namespace Zero.Generator.Metadata {
	public readonly struct MetadataFactory {
		private readonly MetadataRule _rule;

		public MetadataFactory(MetadataRule rule) => _rule = rule;

		public string Json(GameObject instance, int index) {
			var attributes = Attributes(instance).ToArray();
			var externalURL = Format(_rule.externalUrlFormat, index, attributes);
			var description = Format(_rule.descriptionFormat, index, attributes);
			var name = Format(_rule.nameFormat, index, attributes);
			var backgroundColor = ColorUtility.ToHtmlStringRGB(_rule.backgroundColor);

			// TODO: fill two URLs
			var metadata = new Metadata("", "", externalURL, description, name, attributes, backgroundColor);
			var json = JsonConvert.SerializeObject(metadata, Formatting.Indented);
			return json;
		}

		private IOrderedEnumerable<Metadata.Attribute> Attributes(GameObject instance) {
			var names = instance.transform
				.SelfAndDescendants()
				.Select(transform => transform.name)
				.ToList();
			var labelOrder = _rule.traitOrder;
			return _rule.traitData
				.Where(rule => rule.requirement switch {
					TraitDataRule.Requirement.Any => names.Any(name =>
						rule.conditions.Any(condition => condition.IsMatch(name))),
					TraitDataRule.Requirement.All => names.Any(name =>
						rule.conditions.All(condition => condition.IsMatch(name))),
					_ => throw new ArgumentOutOfRangeException()
				})
				.GroupBy(rule => rule.label)
				.Select(group => new Metadata.Attribute(group.Key, group.Last().value))
				.OrderBy(attr => Array.IndexOf(labelOrder, attr.Label));
		}

		private string Format(string text, int index, IEnumerable<Metadata.Attribute> attributes) {
			var integerFormatRegex = new Regex(@"%(0(?<digits>\d*))?d");
			text = integerFormatRegex
				.Replace(text, match => {
					var digits = match.Groups["digits"].Value;
					return index.ToString($"D{digits}");
				});

			var attributeRegex = new Regex(@"\$\{(?<attr>.*)\}");
			text = attributeRegex
				.Replace(text,
					match => {
						var label = match.Groups["attr"].Value;
						var attr = attributes.FirstOrDefault(attr => attr.Label == label);
						return attr?.Value ?? "";
					});
			return text;
		}
	}
}