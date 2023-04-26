using UnityEngine;

namespace Zero.Generator.TextureOptimization.Atlas {
	internal class Mapping {
		public readonly Texture2D Texture;
		public readonly Vector2 Offset;
		public readonly float Scale;

		internal Mapping(Texture2D texture, Vector2 offset, float scale) {
			Texture = texture;
			Offset = offset;
			Scale = scale;
		}
	}
}