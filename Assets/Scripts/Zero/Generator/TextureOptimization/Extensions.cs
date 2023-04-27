using System;
using System.Collections.Generic;
using System.Linq;
using UniGLTF.MeshUtility;
using UnityEngine;
using UnityEngine.Assertions;

namespace Zero.Generator.TextureOptimization {
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
				.Select(uv => uv * scale + normalizedOffset)
				.ToList();
			mesh.SetUVs(channel, remapped);
		}
	}
}