using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace Zero.Generator.TextureOptimization.Atlas {
	internal class Sectioned : IAtlasSection {
		private readonly IAtlasSection _section1;
		private readonly IAtlasSection _section2;
		private readonly IAtlasSection _section3;
		private readonly IAtlasSection _section4;

		internal Sectioned(IReadOnlyCollection<IAtlasSection> sections) : this(
			sections.First(),
			sections.ElementAtOrDefault(1),
			sections.ElementAtOrDefault(2),
			sections.ElementAtOrDefault(3)
		) { }

		private Sectioned(
			IAtlasSection section1,
			[CanBeNull] IAtlasSection section2,
			[CanBeNull] IAtlasSection section3,
			[CanBeNull] IAtlasSection section4) {
			Assert.IsNotNull(section1);

			_section1 = section1;
			_section2 = section2 ?? new EmptySector(section1.Size());
			_section3 = section3 ?? new EmptySector(section1.Size());
			_section4 = section4 ?? new EmptySector(section1.Size());

			Assert.AreEqual(_section1.Size(), _section2.Size());
			Assert.AreEqual(_section1.Size(), _section3.Size());
			Assert.AreEqual(_section1.Size(), _section4.Size());
		}

		public Vector2 Size() => _section1.Size() * 2;

		public Texture2D Image() {
			var result = new Texture2D((int)Size().x, (int)Size().y);
			var sw = (int)_section1.Size().x;
			var sh = (int)_section1.Size().y;
			using var wrapper = new RenderTextureWrapper(sw, sh);
			var rect = new Rect(Vector2.zero, _section1.Size());

			wrapper.Blit(_section1.Image());
			result.ReadPixels(rect, 0, 0);

			wrapper.Blit(_section2.Image());
			result.ReadPixels(rect, sw, 0);

			wrapper.Blit(_section3.Image());
			result.ReadPixels(rect, 0, sh);

			wrapper.Blit(_section4.Image());
			result.ReadPixels(rect, sw, sh);

			result.Apply();
			return result;
		}

		public IEnumerable<Mapping> Mappings() {
			var offsetX = new Vector2(_section1.Size().x, 0);
			var offsetY = new Vector2(0, _section1.Size().y);
			return new[] {
					_section1.Mappings().Select(mapping => new Mapping(mapping.Texture,
						mapping.Offset, mapping.Scale * .5f)),
					_section2.Mappings().Select(mapping => new Mapping(mapping.Texture,
						mapping.Offset + offsetX, mapping.Scale * .5f)),
					_section3.Mappings().Select(mapping => new Mapping(mapping.Texture,
						mapping.Offset + offsetY, mapping.Scale * .5f)),
					_section4.Mappings().Select(mapping => new Mapping(mapping.Texture,
						mapping.Offset + offsetX + offsetY, mapping.Scale * .5f)),
				}
				.SelectMany(mappings => mappings);
		}
	}
}