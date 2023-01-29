using System;
using UniGLTF;
using UnityEngine;
using VRM;
using VRMShaders;

namespace Zero.Generator {
	public class ModelDatalizer {
		public enum Mode {
			Simple,
			Normalize,
			NormalizeWithForceTPose,
		}

		private readonly Mode _mode;

		public ModelDatalizer(Mode mode) {
			_mode = mode;
		}

		public byte[] Datalize(GameObject instance) {
			instance = _mode switch {
				Mode.Simple => instance,
				Mode.Normalize => VRMBoneNormalizer.Execute(instance, false),
				Mode.NormalizeWithForceTPose => VRMBoneNormalizer.Execute(instance, true),
				_ => throw new ArgumentOutOfRangeException()
			};

			var vrm = VRMExporter.Export(new GltfExportSettings(), instance, new RuntimeTextureSerializer());
			return vrm.ToGlbBytes();
		}
	}
}