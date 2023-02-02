using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Zero.Generator {
	public class PhotoBooth : IDisposable {
		private readonly Camera _camera;

		public enum Position {
			Front,
			Back,
			Top,
			Bottom,
			Left,
			Right,
			Face,
		}

		public enum Format {
			PNG,
			JPG,
		}

		public PhotoBooth(Transform subject, Position position) {
			var n = Mathf.Pow(.5f, .5f);
			_camera = new GameObject {
				name = $"{position} Camera",
				transform = {
					position = subject.position + position switch {
						Position.Front => new Vector3(0f, 0.9f, 1f),
						Position.Back => new Vector3(0f, 0.9f, -1f),
						Position.Top => new Vector3(0f, 2f, 0f),
						Position.Bottom => new Vector3(0f, -0.1f, 0f),
						Position.Left => new Vector3(-1f, 0.9f, 0f),
						Position.Right => new Vector3(1f, 0.9f, 0f),
						Position.Face => new Vector3(0f, 1.3f, 1f),
						_ => throw new ArgumentOutOfRangeException(nameof(position), position, null)
					},
					rotation = position switch {
						Position.Front => new Quaternion(0f, 1f, 0f, 0f),
						Position.Back => new Quaternion(0f, 0f, 0f, 1f),
						Position.Top => new Quaternion(n, 0f, 0f, n),
						Position.Bottom => new Quaternion(-n, 0f, 0f, n),
						Position.Left => new Quaternion(0f, n, 0f, n),
						Position.Right => new Quaternion(0f, -n, 0f, n),
						Position.Face => new Quaternion(0f, 1f, 0f, 0f),
						_ => throw new ArgumentOutOfRangeException(nameof(position), position, null)
					}
				}
			}.AddComponent<Camera>();
			_camera.nearClipPlane = .001f;
			_camera.farClipPlane = 5f;
			_camera.orthographic = true;
			_camera.orthographicSize = position switch {
				Position.Face => .2f,
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