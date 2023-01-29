using System;
using UniGLTF;
using UnityEngine;
using UniVRM10;
using VRM;
using VRMShaders;

namespace Zero.Generator {
	public class ModelDatalizer {
		public enum Mode {
			Simple,
			Normalize,
			NormalizeWithForceTPose,
		}

		public enum Version {
			VRM0x,
			VRM10,
		}

		private readonly Mode _mode;
		private readonly Version _version;

		public ModelDatalizer(Mode mode, Version version) {
			_mode = mode;
			_version = version;
		}

		public byte[] Datalize(GameObject instance) {
			instance = _mode switch {
				Mode.Simple => instance,
				Mode.Normalize => VRMBoneNormalizer.Execute(instance, false),
				Mode.NormalizeWithForceTPose => VRMBoneNormalizer.Execute(instance, true),
				_ => throw new ArgumentOutOfRangeException(nameof(_mode), _mode, null)
			};

			return _version switch {
				Version.VRM0x => VRMExporter.Export(new GltfExportSettings(), instance, new RuntimeTextureSerializer())
					.ToGlbBytes(),
				Version.VRM10 => Vrm10Exporter.Export(instance),
				_ => throw new ArgumentOutOfRangeException(nameof(_version), _version, null)
			};
		}
	}
}