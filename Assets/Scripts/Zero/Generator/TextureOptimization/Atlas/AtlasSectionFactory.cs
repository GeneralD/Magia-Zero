using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Zero.Generator.TextureOptimization.Atlas {
	internal static class AtlasSectionFactory {
		public static IAtlasSection Create(IEnumerable<Texture2D> textures) =>
			Create(textures
				.Select(texture => new ImageSector(texture))
				.ToList());

		private static IAtlasSection Create(IReadOnlyCollection<IAtlasSection> sections) {
			if (!sections.Any()) return new EmptySector(new Vector2(16, 16));
			if (sections.Count() == 1) return sections.First();

			var minSections = sections
				.GroupBy(section => section.Size())
				.OrderBy(group => group.Key.x)
				.First()
				.Take(4)
				.ToList();


			var reduced = sections.Where(section => !minSections.Contains(section)).ToList();
			reduced.Add(new Sectioned(minSections));

			return Create(reduced);
		}
	}
}