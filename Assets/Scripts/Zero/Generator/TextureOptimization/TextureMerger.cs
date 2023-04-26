using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UniGLTF.MeshUtility;
using UnityEngine;
using UnityEngine.Assertions;
using Zero.Extensions;
using Zero.Generator.TextureOptimization.Atlas;

namespace Zero.Generator.TextureOptimization {
	public class TextureMerger {
		private readonly int _maxAtlasSize;

		private static readonly int MainTexID = Shader.PropertyToID("_MainTex");
		private static readonly int BumpTexID = Shader.PropertyToID("_BumpMap");
		private static readonly int EmissionTexID = Shader.PropertyToID("_EmissionMap");

		public TextureMerger(int maxAtlasSize = 4096) {
			Assert.IsTrue(maxAtlasSize.IsPowOf2());
			_maxAtlasSize = maxAtlasSize;
		}

		private class TextureRelation {
			public Texture MainTexture;

			[CanBeNull]
			public Texture BumpTexture;

			[CanBeNull]
			public Texture EmissionTexture;
		}

		public void Apply(GameObject root) {
			var renderers = root
				.GetComponentsInChildren<Renderer>()
				.Where(renderer => renderer.materials.Any(material => material.HasTexture(MainTexID)))
				.ToArray();

			var sets = renderers
				.Select(renderer => {
					var mainTexture = renderer.materials
						.First(material => material.HasTexture(MainTexID))
						.GetTexture(MainTexID);
					return new TextureRelation {
						MainTexture = mainTexture,
						BumpTexture = renderer.materials
							              .FirstOrDefault(m => m.HasTexture(BumpTexID))?
							              .GetTexture(BumpTexID)
						              // allocate blank section
						              ?? new Texture2D(mainTexture.width, mainTexture.height),
						EmissionTexture = renderer.sharedMaterials
							                  .FirstOrDefault(m => m.HasTexture(EmissionTexID))?
							                  .GetTexture(EmissionTexID)
						                  // allocate blank section
						                  ?? new Texture2D(mainTexture.width, mainTexture.height),
					};
				})
				// distinct by MainTexture
				.GroupBy(element => element.MainTexture)
				.Select(group => group.Aggregate((lhs, rhs) => new TextureRelation {
					MainTexture = lhs.MainTexture,
					BumpTexture = lhs.BumpTexture ? lhs.BumpTexture : rhs.BumpTexture,
					EmissionTexture = lhs.EmissionTexture ? lhs.EmissionTexture : rhs.EmissionTexture,
				}))
				.ToArray();

			// Make texture atlas
			var mainAtlasSection =
				AtlasSectionFactory.Create(sets.Select(element => element.MainTexture as Texture2D));
			var bumpAtlasSection =
				AtlasSectionFactory.Create(sets.Select(element => element.BumpTexture as Texture2D));
			var emissionAtlasSection =
				AtlasSectionFactory.Create(sets.Select(element => element.EmissionTexture as Texture2D));

			Assert.IsTrue(mainAtlasSection.Size().x <= _maxAtlasSize);
			Assert.IsTrue(bumpAtlasSection.Size().x <= _maxAtlasSize);
			Assert.IsTrue(emissionAtlasSection.Size().x <= _maxAtlasSize);

			var materials =
				renderers
					.SelectMany(renderer => renderer.materials)
					.ToArray();

			// Replace textures with atlas
			materials
				.Where(material => material.HasTexture(MainTexID))
				.ForEach(material => material.SetTexture(MainTexID, mainAtlasSection.Image()));

			materials
				.Where(material => material.HasTexture(BumpTexID))
				.ForEach(material => material.SetTexture(BumpTexID, bumpAtlasSection.Image()));

			materials
				.Where(material => material.HasTexture(EmissionTexID))
				.ForEach(material => material.SetTexture(EmissionTexID, emissionAtlasSection.Image()));

			// All atlas mappings have to be same, so we can use main atlas as a sample
			var mappings = mainAtlasSection.Mappings().ToArray();
			var atlasSize = mainAtlasSection.Size();

			// Remap UVs (first channel)
			renderers
				.ForEach(renderer => {
					var mainTexture = renderer.materials
						.First(material => material.HasTexture(MainTexID))
						.GetTexture(MainTexID);
					var mapping = mappings.First(map => map.Texture == mainTexture);
					var mesh = renderer.ReplaceMeshWithCopied();
					mesh.RemapUVs(0, atlasSize, mapping.Offset, mapping.Scale);
				});
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
			var normalizedOffset = Vector2.up - offset / atlasSize;
			var remapped = uvs
				.Select(uv => uv * scale + normalizedOffset)
				.ToList();
			mesh.SetUVs(channel, remapped);
		}
	}
}