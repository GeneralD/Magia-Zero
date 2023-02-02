using UnityEngine;
using UniVRM10;

namespace Zero.Generator {
	public class ModelDatalizer {
		private readonly VRM10ObjectMeta _meta;

		public ModelDatalizer(VRM10ObjectMeta meta) {
			_meta = meta;
		}

		public byte[] Datalize(GameObject instance) => Vrm10Exporter.Export(instance, null, _meta);
	}
}