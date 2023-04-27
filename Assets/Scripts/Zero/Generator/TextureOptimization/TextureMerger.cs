using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Zero.Extensions;
using Zero.Generator.TextureOptimization.Atlas;

namespace Zero.Generator.TextureOptimization {
	public class TextureMerger {
		private readonly int _maxAtlasSize;

		public TextureMerger(int maxAtlasSize = 4096) {
			Assert.IsTrue(maxAtlasSize.IsPowOf2());
			_maxAtlasSize = maxAtlasSize;
		}

		public void Apply(GameObject root) {
			var renderers = root
				.GetComponentsInChildren<Renderer>()
				.Where(renderer => renderer.materials.Any(material => material.HasTexture(TextureGroup.MainTexID)))
				.ToArray();

			var textureGroups = renderers
				.Select(renderer => new TextureGroup(renderer))
				.GroupBy(group => group.MainTexture)
				.Select(group => group.Aggregate(TextureGroup.Merged))
				.ToArray();

			// Make texture atlas
			var mainAtlasSection =
				AtlasSectionFactory.Create(textureGroups.Select(group => group.MainTexture as Texture2D));
			var bumpAtlasSection =
				AtlasSectionFactory.Create(textureGroups.Select(group => group.BumpTexture as Texture2D));
			var emissionAtlasSection =
				AtlasSectionFactory.Create(textureGroups.Select(group => group.EmissionTexture as Texture2D));

			// Validate them
			Assert.IsTrue(mainAtlasSection.Size().x <= _maxAtlasSize);
			Assert.IsTrue(bumpAtlasSection.Size().x <= _maxAtlasSize);
			Assert.IsTrue(emissionAtlasSection.Size().x <= _maxAtlasSize);

			// All atlas mappings have to be same, so we can use main atlas as a sample
			var mappings = mainAtlasSection.Mappings().ToArray();
			var atlasSize = mainAtlasSection.Size();

			// Remap UVs (first channel)
			renderers
				.ForEach(renderer => {
					var mainTexture = renderer.materials
						.First(material => material.HasTexture(TextureGroup.MainTexID))
						.GetTexture(TextureGroup.MainTexID);
					var mapping = mappings.First(map => map.Texture == mainTexture);
					var mesh = renderer.ReplaceMeshWithCopied();
					mesh.RemapUVs(0, atlasSize, mapping.Offset, mapping.Scale);
				});

			var materials = renderers
				.SelectMany(renderer => renderer.materials)
				.ToArray();

			// Replace textures with atlas
			materials
				.Where(material => material.HasTexture(TextureGroup.MainTexID))
				.ForEach(material => material.SetTexture(TextureGroup.MainTexID, mainAtlasSection.Image()));

			materials
				.Where(material => material.HasTexture(TextureGroup.BumpTexID))
				.ForEach(material => material.SetTexture(TextureGroup.BumpTexID, bumpAtlasSection.Image()));

			materials
				.Where(material => material.HasTexture(TextureGroup.EmissionTexID))
				.ForEach(material => material.SetTexture(TextureGroup.EmissionTexID, emissionAtlasSection.Image()));
		}
	}
}