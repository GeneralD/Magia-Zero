using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Zero.Generator.TextureOptimization {
	public class TextureMerger {
		private readonly IEnumerable<TargetTexture> _targets;

		public TextureMerger(params TargetTexture[] targets) {
			_targets = targets;
		}

		public void Apply(GameObject root) {
			var renderers = root.GetComponentsInChildren<Renderer>();

			var textureGroups = renderers
				.SelectMany(r => _targets.Select(target => (target, texture: r.sharedMaterial.GetTexture(target.name))))
				.GroupBy(t => t.texture)
				.Select(g => g.First())
				.GroupBy(t => t.target);

			foreach (var group in textureGroups) {
				var target = group.Key;
				var textures = group.Select(t => t.texture).ToArray();
				var atlas = textures.MakeAtlas(target.atlasSize);
				foreach (var renderer in renderers) {
					// protect shared (resource) material
					var material = renderer.ReplaceMaterialsWithCopied();

					var index = Array.IndexOf(textures, renderer.sharedMaterial.GetTexture(target.name));
					material.SetAtlas(atlas, target.name, index, textures.Length);
				}
			}
		}
	}

	internal static class Extensions {
		public static Material ReplaceMaterialsWithCopied(this Renderer renderer) {
			var sharedMaterial = renderer.sharedMaterial;
			var material = new Material(sharedMaterial);
			material.CopyPropertiesFromMaterial(sharedMaterial);
			return renderer.sharedMaterial = material;
		}

		public static void SetAtlas(this Material material, Texture2D atlas, string name, int index, int length) {
			// set atlas
			material.SetTexture(name, atlas);
			// adjust tilling and offset
			var row = Mathf.CeilToInt(Mathf.Pow(length, .5f));
			var xOffset = index % row;
			var yOffset = index / row;
			material.SetTextureScale(name, Vector2.one / row);
			material.SetTextureOffset(name, new Vector2(xOffset, yOffset) / row);
		}

		public static Texture2D MakeAtlas(this IReadOnlyList<Texture> textures, int atlasSize) {
			var row = Mathf.CeilToInt(Mathf.Pow(textures.Count(), .5f));
			atlasSize = SuitableAtlasSize(textures, row, atlasSize);

			var result = new Texture2D(atlasSize, atlasSize);
			var sectionSize = atlasSize / row;
			using var wrapper = new RenderTextureWrapper(sectionSize, sectionSize);

			for (var index = 0; index < textures.Count; index++) {
				var texture = textures[index];
				var dstX = index % row * atlasSize / row;
				var dstY = index / row * atlasSize / row;
				wrapper.Blit(texture);
				result.ReadPixels(new Rect(0, 0, sectionSize, sectionSize), dstX, dstY);
			}

			result.Apply();
			return result;
		}

		/// <summary>
		/// If the given value is too big for the size of the atlas to be created from the textures, an alternative value is returned.
		/// </summary>
		/// <param name="textures">source textures</param>
		/// <param name="row">number of rows of the atlas</param>
		/// <param name="preferredSize">preferred atlas size</param>
		/// <returns>an alternative value or same as preferredSize</returns>
		private static int SuitableAtlasSize(IReadOnlyList<Texture> textures, int row, int preferredSize) {
			var maxWidth = textures.Select(texture => texture.width).OrderByDescending(w => w).Take(row).Sum();
			var maxHeight = textures.Select(texture => texture.height).OrderByDescending(h => h).Take(row).Sum();
			var maxSize = Math.Max(maxWidth, maxHeight);
			var maxSuitableSize = (int)Mathf.Pow(2, Mathf.CeilToInt(Mathf.Log(maxSize, 2)));
			return Math.Min(preferredSize, maxSuitableSize);
		}
	}
}