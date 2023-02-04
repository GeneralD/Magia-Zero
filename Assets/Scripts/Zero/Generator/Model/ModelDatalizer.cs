using UnityEngine;
using UniVRM10;

namespace Zero.Generator.Model {
	public class ModelDatalizer {
		private readonly VRM10ObjectMeta _meta;

		public ModelDatalizer(VRM10ObjectMeta meta) {
			_meta = meta;
		}

		public byte[] Datalize(GameObject instance, Texture2D thumbnail = null) {
			var copy = new VRM10ObjectMeta();
			_meta.CopyTo(copy);
			// override thumbnail
			if (thumbnail != null) copy.Thumbnail = thumbnail;
			return Vrm10Exporter.Export(instance, null, copy);
		}
	}
}