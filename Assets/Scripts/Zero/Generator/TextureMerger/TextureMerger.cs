using System;
using System.Collections.Generic;
using System.Linq;
using UniGLTF.MeshUtility;
using UnityEngine;

namespace Zero.Generator.TextureMerger {
	public class TextureMerger {
		private readonly IEnumerable<TargetTexture> _targets;

		public TextureMerger(params TargetTexture[] targets) {
			_targets = targets;
		}

		public TextureMerger() {
			_targets = new[] { new TargetTexture("_MainTex", 1024 * 2) };
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
				var atlas = MakeAtlas(textures, target.atlasSize);
				foreach (var renderer in renderers) {
					// protect shared (resource) material
					var material = renderer.ReplaceMaterialsWithCopied();

					var index = Array.IndexOf(textures, renderer.sharedMaterial.GetTexture(target.name));
					material.SetAtlas(atlas, target.name, index, textures.Length);
				}
			}
		}

		private Texture2D MakeAtlas(IReadOnlyList<Texture> textures, int atlasSize) {
			var row = Mathf.CeilToInt(Mathf.Pow(textures.Count(), .5f));
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
	}
}