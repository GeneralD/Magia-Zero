using System.Collections.Generic;
using UnityEngine;

namespace Zero.Generator.TextureOptimization.Atlas {
	internal interface IAtlasSection {
		Vector2 Size();
		Texture2D Image();

		IEnumerable<Mapping> Mappings();
	}
}