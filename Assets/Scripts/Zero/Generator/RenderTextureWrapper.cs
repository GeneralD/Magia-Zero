using System;
using UnityEngine;

namespace Zero.Generator {
	internal class RenderTextureWrapper : IDisposable {
		private readonly RenderTexture _renderTexture;
		private readonly RenderTexture _active;

		public RenderTextureWrapper(int width, int height, int depth = 0) {
			_renderTexture = new RenderTexture(width, height, depth);
			_renderTexture.Create();
			_active = RenderTexture.active;
			RenderTexture.active = _renderTexture;
		}

		public void Dispose() {
			RenderTexture.active = _active;
			_renderTexture.Release();
		}

		public void Blit(Texture texture) {
			Graphics.Blit(texture, _renderTexture);
		}
	}
}