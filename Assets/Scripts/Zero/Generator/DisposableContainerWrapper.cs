using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Zero.Generator {
	internal readonly struct DisposableContainerWrapper : IDisposable {
		public readonly GameObject Container;

		public DisposableContainerWrapper(GameObject container) => Container = container;

		public void Dispose() => Object.DestroyImmediate(Container);
	}
}