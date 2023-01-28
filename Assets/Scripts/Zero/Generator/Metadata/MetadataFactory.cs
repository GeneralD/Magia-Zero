using System;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Zero.Extensions;
using Zero.Generator.Entity;

namespace Zero.Generator.Metadata {
	public partial class MetadataFactory {
		private readonly MetadataRule _rule;

		public MetadataFactory(MetadataRule rule) => _rule = rule;

		public string Json(GameObject instance, int index, string imageURL, string animationURL) {
			var attributes = Attributes(instance).ToArray();
			var formatter = new TextFormatter(index, attributes);
			var externalURL = string.IsNullOrEmpty(_rule.externalUrlFormat)
				? null
				: formatter.Format(_rule.externalUrlFormat);
			var description = formatter.Format(_rule.descriptionFormat);
			var name = formatter.Format(_rule.nameFormat);
			var backgroundColor = ColorUtility.ToHtmlStringRGB(_rule.backgroundColor).ToLower();

			var metadata = new Metadata(imageURL, animationURL, externalURL, description, name, attributes,
				backgroundColor);
			var json = JsonConvert.SerializeObject(metadata, Formatting.Indented,
				new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
			return json;
		}

		private IOrderedEnumerable<Metadata.Attribute> Attributes(GameObject instance) {
			var names = instance.transform
				.SelfAndDescendants()
				.Select(transform => transform.name)
				.ToList();
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
				.OrderBy(attr => Array.IndexOf(_rule.traitOrder, attr.Label));
		}
	}
}