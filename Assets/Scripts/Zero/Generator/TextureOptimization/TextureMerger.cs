using System;
using System.Collections.Generic;
using System.Linq;
using UniGLTF.MeshUtility;
using UnityEngine;
using UnityEngine.Assertions;
using Zero.Extensions;
using Zero.Generator.TextureOptimization.Atlas;

namespace Zero.Generator.TextureOptimization {
	public class TextureMerger {
		private readonly int _maxAtlasSize;
		private static readonly int MainTex = Shader.PropertyToID("_MainTex");
		private static readonly int NormalMapSampler = Shader.PropertyToID("_NormalMapSampler");

		public TextureMerger(int maxAtlasSize = 4096) {
			Assert.IsTrue(maxAtlasSize.IsPowOf2());
			_maxAtlasSize = maxAtlasSize;
		}

		private class RendererSet {
			public Texture MainTexture;
			public Texture NormalTexture;
			public Mesh Mesh;
		}

		public void Apply(GameObject root) {
			var renderers = root.GetComponentsInChildren<Renderer>();

			var sets = renderers
				.Where(renderer => renderer.sharedMaterial.HasTexture("_MainTex"))
				.Select(renderer => {
					var mainTexture = renderer.sharedMaterial.GetTexture(MainTex);
					return new RendererSet {
						MainTexture = mainTexture,
						NormalTexture = renderer.sharedMaterial.HasTexture(NormalMapSampler)
							? renderer.sharedMaterial.GetTexture(NormalMapSampler)
							: new Texture2D(mainTexture.width, mainTexture.height),
						Mesh = renderer.ReplaceMeshWithCopied(),
					};
				})
				// distinct by MainTexture
				.GroupBy(element => element.MainTexture)
				.Select(x => x.First())
				.ToArray();

			// Make texture atlas
			var mainAtlasSection =
				AtlasSectionFactory.Create(sets.Select(element => element.MainTexture as Texture2D));
			var normalAtlasSection =
				AtlasSectionFactory.Create(sets.Select(element => element.NormalTexture as Texture2D));

			Assert.IsTrue(mainAtlasSection.Size().x <= _maxAtlasSize);
			Assert.IsTrue(normalAtlasSection.Size().x <= _maxAtlasSize);

			// Replace all materials with an unified material
			Material unifiedMaterial = null;
			foreach (var renderer in renderers)
				renderer.sharedMaterial = unifiedMaterial ??= new Material(renderer.sharedMaterial);

			// Set texture atlas
			if (unifiedMaterial != null)
				unifiedMaterial.SetTexture(MainTex, mainAtlasSection.Image());
			if (unifiedMaterial != null)
				unifiedMaterial.SetTexture(NormalMapSampler, normalAtlasSection.Image());

			// Remap UVs (first channel)
			var mappings = mainAtlasSection.Mappings().ToArray();
			var atlasSize = mainAtlasSection.Size();
			foreach (var set in sets) {
				var mapping = mappings.First(mapping => mapping.Texture == set.MainTexture);
				set.Mesh.RemapUVs(0, atlasSize, mapping.Offset, mapping.Scale);
			}
		}
	}

	internal static class Extensions {
		public static Mesh ReplaceMeshWithCopied(this Renderer renderer, bool copyBlendShape = true) =>
			renderer switch {
				SkinnedMeshRenderer r =>
					r.sharedMesh = r.sharedMesh.Copy(copyBlendShape),
				MeshRenderer mr when mr.GetComponent<MeshFilter>() is { } f =>
					f.sharedMesh = f.sharedMesh.Copy(copyBlendShape),
				_ => throw new ArgumentOutOfRangeException(nameof(renderer), renderer, null)
			};

		public static void RemapUVs(this Mesh mesh, int channel, Vector2 atlasSize, Vector2 offset, float scale) {
			Assert.IsTrue(scale <= 1);
			var uvs = new List<Vector2>();
			mesh.GetUVs(channel, uvs);
			if (!uvs.Any()) return;
			var normalizedOffset = offset / atlasSize;
			var remapped = uvs
				.Select(v => v * scale + normalizedOffset)
				// TODO: Delete
				.Select(vector2 => {
					Debug.Log(
						$"UV: {vector2}, scale: {scale}, offset: {offset}, atlasSize: {atlasSize}, normalizedOffset: {normalizedOffset}");
					return vector2;
				})
				// TODO: to here
				.ToList();
			mesh.SetUVs(channel, remapped);
		}
	}
}