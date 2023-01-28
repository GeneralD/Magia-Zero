using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Zero.Generator.Metadata {
	[JsonObject]
	public class Metadata {
		[JsonProperty("image")]
		public readonly string ImageURL;

		[JsonProperty("animation_url")]
		public readonly string AnimationURL;

		[JsonProperty("external_url")]
		[CanBeNull]
		public readonly string ExternalURL;

		[JsonProperty("description")]
		public readonly string Description;

		[JsonProperty("name")]
		public readonly string Name;

		[JsonProperty("attributes")]
		public readonly Attribute[] Attributes;

		[JsonProperty("background_color")]
		public readonly string BackgroundColor;

		public Metadata(
			string imageURL,
			string animationURL,
			[CanBeNull] string externalURL,
			string description,
			string name,
			Attribute[] attributes,
			string backgroundColor) {
			ImageURL = imageURL;
			AnimationURL = animationURL;
			ExternalURL = externalURL;
			Description = description;
			Name = name;
			Attributes = attributes;
			BackgroundColor = backgroundColor;
		}

		[JsonObject]
		public class Attribute {
			[JsonProperty("trait_type")]
			public readonly string Label;

			[JsonProperty("value")]
			public readonly string Value;

			public Attribute(
				string label,
				string value) {
				Label = label;
				Value = value;
			}
		}
	}
}