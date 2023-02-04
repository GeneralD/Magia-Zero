using System;
using System.Collections.Generic;
using System.Linq;
using UniGLTF.MeshUtility;
using UnityEngine;
using Zero.Extensions;

namespace Zero.Generator {
	public class TextureMerger {
		public void Apply(GameObject root) {
			var renderers = root.GetComponentsInChildren<Renderer>();
			var textures = renderers
				.SelectMany(renderer => renderer.sharedMaterials)
				.Select(material => material.mainTexture)
				.Distinct()
				.ToArray();

			var atlas = MakeAtlas(textures);

			renderers.ForEach((renderer, index) => {
				// protect original mesh
				var mesh = ReplaceMeshWithCopied(renderer);
				RemapUVsForAllChannels(mesh, index, renderers.Length);

				// protect shared (resource) material
				var materials = ReplaceMaterialsWithCopied(renderer);
				materials.ForEach(material => material.mainTexture = atlas);
			});
		}

		private static Texture2D MakeAtlas(Texture[] textures, int size = 1024 * 4) {
			var row = Mathf.CeilToInt(Mathf.Pow(textures.Count(), .5f));
			var result = new Texture2D(size, size);
			var sectionSize = size / row;
			using var wrapper = new RenderTextureWrapper(sectionSize, sectionSize);

			for (var index = 0; index < textures.Length; index++) {
				var texture = textures[index];
				var dstX = index % row * size / row;
				var dstY = index / row * size / row;
				wrapper.Blit(texture);
				result.ReadPixels(new Rect(0, 0, sectionSize, sectionSize), dstX, dstY);
			}

			result.Apply();
			return result;
		}

		private static Material[] ReplaceMaterialsWithCopied(Renderer renderer) =>
			renderer.sharedMaterials = renderer.sharedMaterials.Select(m => {
				var material = new Material(m);
				material.CopyPropertiesFromMaterial(m);
				return material;
			}).ToArray();

		private static Mesh ReplaceMeshWithCopied(Renderer renderer, bool copyBlendShape = true) =>
			renderer switch {
				SkinnedMeshRenderer r =>
					r.sharedMesh = r.sharedMesh.Copy(copyBlendShape),
				MeshRenderer mr when mr.GetComponent<MeshFilter>() is { } f =>
					f.sharedMesh = f.sharedMesh.Copy(copyBlendShape),
				_ => throw new ArgumentOutOfRangeException(nameof(renderer), renderer, null)
			};

		private static void RemapUVsForAllChannels(Mesh mesh, int rendererIndex, int numberOfRenderer) {
			for (var channel = 0; channel < 4; channel++) {
				var uvs = new List<Vector2>();
				mesh.GetUVs(channel, uvs);
				if (!uvs.Any()) return;
				var remapped = RemappedUV(rendererIndex, numberOfRenderer, uvs);
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