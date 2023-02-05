using System;

namespace Zero.Generator.TextureOptimization {
	[Serializable]
	public struct TargetTexture {
		public string name;
		public int atlasSize;

		public TargetTexture(string name, int atlasSize) {
			this.name = name;
			this.atlasSize = atlasSize;
		}

		public static TargetTexture MainTexture(int atlasSize) => new("_MainTex", atlasSize);
	}
}