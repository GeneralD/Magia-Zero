using System;

namespace Zero.Generator.TextureMerger {
	[Serializable]
	public struct TargetTexture {
		public string name;
		public int atlasSize;

		public TargetTexture(string name, int atlasSize) {
			this.name = name;
			this.atlasSize = atlasSize;
		}
	}
}