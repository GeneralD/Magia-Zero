using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Zero.Generator.Image {
	public class PhotoBooth : IDisposable {
		private readonly Camera _camera;

		public PhotoBooth(Transform subject, CameraPosition position) {
			var n = Mathf.Pow(.5f, .5f);
			_camera = new GameObject {
				name = $"{position} Camera",
				transform = {
					position = subject.position + position switch {
						CameraPosition.Front => new Vector3(0f, 0.9f, 1f),
						CameraPosition.Back => new Vector3(0f, 0.9f, -1f),
						CameraPosition.Top => new Vector3(0f, 2f, 0f),
						CameraPosition.Bottom => new Vector3(0f, -0.1f, 0f),
						CameraPosition.Left => new Vector3(-1f, 0.9f, 0f),
						CameraPosition.Right => new Vector3(1f, 0.9f, 0f),
						CameraPosition.Face => new Vector3(0f, 1.3f, 1f),
						_ => throw new ArgumentOutOfRangeException(nameof(position), position, null)
					},
					rotation = position switch {
						CameraPosition.Front => new Quaternion(0f, 1f, 0f, 0f),
						CameraPosition.Back => new Quaternion(0f, 0f, 0f, 1f),
						CameraPosition.Top => new Quaternion(n, 0f, 0f, n),
						CameraPosition.Bottom => new Quaternion(-n, 0f, 0f, n),
						CameraPosition.Left => new Quaternion(0f, n, 0f, n),
						CameraPosition.Right => new Quaternion(0f, -n, 0f, n),
						CameraPosition.Face => new Quaternion(0f, 1f, 0f, 0f),
						_ => throw new ArgumentOutOfRangeException(nameof(position), position, null)
					}
				}
			}.AddComponent<Camera>();
			_camera.nearClipPlane = .001f;
			_camera.farClipPlane = 5f;
			_camera.orthographic = true;
			_camera.orthographicSize = position switch {
				CameraPosition.Face => .2f,
				_ => 1f
			};
		}

		public void Dispose() => Object.DestroyImmediate(_camera.gameObject);

		public Texture2D Shoot(int size = 1280, int depth = 24) {
			var texture = new RenderTexture(size, size, depth);
			var originalTexture = _camera.targetTexture;
			_camera.targetTexture = texture;
			_camera.Render();
			_camera.targetTexture = originalTexture;
			RenderTexture.active = texture;

			var capture = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
			capture.ReadPixels(new Rect(Vector2.zero, new Vector2(capture.width, capture.height)), 0, 0);
			capture.Apply();
			return capture;
		}
	}
}