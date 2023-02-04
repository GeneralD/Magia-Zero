using System;
using System.Collections.Generic;
using System.Linq;
using UniGLTF.MeshUtility;
using UnityEngine;
using Zero.Extensions;

namespace Zero.Generator {
	public class TextureBaker {
		public void Bake(GameObject root) {
			var renderers = root.GetComponentsInChildren<Renderer>();
			// TODO: merge textures
			// var mergedTexture = 
			renderers.ForEach((renderer, index) => {
				// protect original mesh
				var mesh = ReplaceMeshWithCopied(renderer);
				
				RemapUVsForAllChannels(mesh, index, renderers.Length);

				renderer.materials.ForEach(material => { });
			});
		}

		private static Mesh ReplaceMeshWithCopied(Renderer renderer) =>
			renderer switch {
				SkinnedMeshRenderer r =>
					r.sharedMesh = r.sharedMesh.Copy(true),
				MeshRenderer mr when mr.GetComponent<MeshFilter>() is { } f =>
					f.sharedMesh = f.sharedMesh.Copy(true),
				_ => throw new ArgumentOutOfRangeException(nameof(renderer), renderer, null)
			};

		private static void RemapUVsForAllChannels(Mesh mesh, int rendererIndex, int numberOfRenderer) {
			Enumerable.Range(0, 7)
				.ForEach(channel => {
					var uvs = new List<Vector2>();
					mesh.GetUVs(channel, uvs);
					var remapped = RemappedUV(rendererIndex, numberOfRenderer, uvs);
					mesh.SetUVs(channel, remapped.ToList());
				});
		}

		private static IEnumerable<Vector2> RemappedUV(int index, int count, IEnumerable<Vector2> uv) {
			if (count < 1) return uv;
			var row = 1;
			while (Math.Pow(row, 2) <= count && count < Math.Pow(row + 1, 2)) row++;
			var rowOffset = index / row;
			var colOffset = index % row;
			return uv.Select(v => v / row + new Vector2((float)colOffset / row, (float)rowOffset / row));
		}
	}
}