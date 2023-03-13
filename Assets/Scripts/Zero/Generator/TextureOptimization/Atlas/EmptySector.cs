using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Zero.Generator.TextureOptimization.Atlas {
	internal class EmptySector : IAtlasSection {
		private readonly Vector2 _size;

		public EmptySector(Vector2 size) => _size = size;

		public Vector2 Size() => _size;

		public Texture2D Image() => Texture2D.blackTexture;
		public IEnumerable<Mapping> Mappings() => Enumerable.Empty<Mapping>();
	}
}