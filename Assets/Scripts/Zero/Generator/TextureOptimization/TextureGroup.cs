using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Zero.Generator.TextureOptimization {
	internal class TextureGroup {
		public static readonly int MainTexID = Shader.PropertyToID("_MainTex");
		public static readonly int BumpTexID = Shader.PropertyToID("_BumpMap");
		public static readonly int EmissionTexID = Shader.PropertyToID("_EmissionMap");

		public Texture MainTexture;

		[CanBeNull]
		public Texture BumpTexture;

		[CanBeNull]
		public Texture EmissionTexture;

		private TextureGroup() { }

		public TextureGroup(Renderer renderer) {
			var mainTexture = renderer.materials
				.First(material => material.HasTexture(MainTexID))
				.GetTexture(MainTexID);

			MainTexture = mainTexture;
			BumpTexture = renderer.materials
				              .FirstOrDefault(m => m.HasTexture(BumpTexID))?
				              .GetTexture(BumpTexID)
			              // allocate blank section
			              ?? new Texture2D(mainTexture.width, mainTexture.height);
			EmissionTexture = renderer.sharedMaterials
				                  .FirstOrDefault(m => m.HasTexture(EmissionTexID))?
				                  .GetTexture(EmissionTexID)
			                  // allocate blank section
			                  ?? new Texture2D(mainTexture.width, mainTexture.height);
		}

		public static TextureGroup Merged(TextureGroup lhs, TextureGroup rhs) => new() {
			MainTexture = lhs.MainTexture,
			BumpTexture = lhs.BumpTexture ? lhs.BumpTexture : rhs.BumpTexture,
			EmissionTexture = lhs.EmissionTexture ? lhs.EmissionTexture : rhs.EmissionTexture,
		};
	}
}