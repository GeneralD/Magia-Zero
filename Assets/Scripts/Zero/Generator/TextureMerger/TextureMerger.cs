using System;
using System.Collections.Generic;
using System.Linq;
using UniGLTF.MeshUtility;
using UnityEngine;

namespace Zero.Generator.TextureMerger {
	public class TextureMerger {
		private readonly int _atlasSize;

		public TextureMerger(int atlasSize = 1024 * 4) {
			_atlasSize = atlasSize;
		}

		public void Apply(GameObject root) {
			var renderers = root.GetComponentsInChildren<Renderer>();
			var textures = renderers
				.Select(renderer => renderer.sharedMaterial.mainTexture)
				.Distinct()
				.ToArray();

			var atlas = MakeAtlas(textures);

			foreach (var renderer in renderers) {
				// protect original mesh
				var mesh = renderer.ReplaceMeshWithCopied();
				var textureIndex = Array.IndexOf(textures, renderer.sharedMaterial.mainTexture);
				mesh.RemapUVsForAllChannels(textureIndex, textures.Length);

				// protect shared (resource) material
				var material = renderer.ReplaceMaterialsWithCopied();
				material.mainTexture = atlas;
			}
		}

		private Texture2D MakeAtlas(IReadOnlyList<Texture> textures) {
			var row = Mathf.CeilToInt(Mathf.Pow(textures.Count(), .5f));
			var result = new Texture2D(_atlasSize, _atlasSize);
			var sectionSize = _atlasSize / row;
			using var wrapper = new RenderTextureWrapper(sectionSize, sectionSize);

			for (var index = 0; index < textures.Count; index++) {
				var texture = textures[index];
				var dstX = index % row * _atlasSize / row;
				var dstY = index / row * _atlasSize / row;
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
				var remapped = RemappedUV(index, count, uvs);
				mesh.SetUVs(channel, remapped.ToList());
			}
		}

		private static IEnumerable<Vector2> RemappedUV(int index, int count, IEnumerable<Vector2> uv) {
			if (count <= 1) return uv;
			var row = Mathf.CeilToInt(Mathf.Pow(count, .5f));
			var rowOffset = index / row;
			var colOffset = index % row;
			return uv.Select(v => v / row + new Vector2((float)colOffset / row, (float)rowOffset / row));
		}
	}
}