using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Zero.Generator.Entity {
	[Serializable]
	public struct MetadataRule {
		public string baseUri;
		public string nameFormat;
		public string descriptionFormat;
		[CanBeNull]
		public string externalUrlFormat;
		public Color32 backgroundColor;
		public TraitDataRule[] traitData;
		public string[] traitOrder;
	}
}