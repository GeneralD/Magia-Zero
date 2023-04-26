using System.Collections.Generic;
using UnityEngine;

namespace Zero.Generator.TextureOptimization.Atlas {
	internal class ImageSector : IAtlasSection {
		private readonly Texture2D _image;

		public ImageSector(Texture2D texture) {
			_image = texture;
		}

		public Vector2 Size() => new(_image.width, _image.height);

		public Texture2D Image() => _image;

		public IEnumerable<Mapping> Mappings() => new[] { new Mapping(_image, Vector2.zero, 1) };
	}
}