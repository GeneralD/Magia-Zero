using System;
using System.Collections.Generic;
using System.Linq;
using UniGLTF.MeshUtility;
using UnityEngine;

namespace Zero.Generator.TextureOptimization {
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
				var atlas = textures.MakeAtlas(target.atlasSize);
				foreach (var renderer in renderers) {
					// protect original mesh
					var mesh = renderer.ReplaceMeshWithCopied();
					var textureIndex = Array.IndexOf(textures, renderer.sharedMaterial.GetTexture(target.name));
					mesh.RemapUVsForAllChannels(textureIndex, textures.Length);

					// protect shared (resource) material
					var material = renderer.ReplaceMaterialsWithCopied();
					material.SetTexture(target.name, atlas);
				}
			}
		}
	}

	internal static class Extensions {
		public static Texture2D MakeAtlas(this IReadOnlyList<Texture> textures, int atlasSize) {
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

		public static Material ReplaceMaterialsWithCopied(this Renderer renderer) {
			var sharedMaterial = renderer.sharedMaterial;
			if (sharedMaterial.name.EndsWith(" [[Copy]]")) return sharedMaterial;
			var material = new Material(sharedMaterial);
			material.name += " [[Copy]]";
			material.CopyPropertiesFromMaterial(sharedMaterial);
			return renderer.sharedMaterial = material;
		}

		public static Mesh ReplaceMeshWithCopied(this Renderer renderer, bool copyBlendShape = true) =>
			renderer switch {
				SkinnedMeshRenderer r =>
					r.sharedMesh = r.sharedMesh.Copy(copyBlendShape),
				MeshRenderer mr when mr.GetComponent<MeshFilter>() is { } f =>
					f.sharedMesh = f.sharedMesh.Copy(copyBlendShape),
				_ => throw new ArgumentOutOfRangeException(nameof(renderer), renderer, null)
			};


		public static void RemapUVsForAllChannels(this Mesh mesh, int index, int count) {
			for (var channel = 0; channel < 4; channel++) {
				var uvs = new List<Vector2>();
				mesh.GetUVs(channel, uvs);
				if (!uvs.Any()) return;
				mesh.SetUVs(channel, uvs.Remapped(index, count));
			}
		}

		private static List<Vector2> Remapped(this List<Vector2> uv, int index, int count) {
			if (count <= 1) return uv;
			var row = Mathf.CeilToInt(Mathf.Pow(count, .5f));
			var rowOffset = index / row;
			var colOffset = index % row;
			return uv
				.Select(v => (v + new Vector2(colOffset, rowOffset)) / row)
				.ToList();
		}
	}
}