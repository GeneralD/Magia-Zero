using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Zero.Extensions {
	public static class TransformExtensions {
		public static IEnumerable<Transform> Children(this Transform transform) => transform.Cast<Transform>();

		public static IEnumerable<Transform> SelfAndChildren(this Transform transform) {
			yield return transform;
			foreach (var t in transform.Children()) yield return t;
		}

		public static IEnumerable<Transform> Descendants(this Transform transform) {
			foreach (var t in transform.Children()) yield return t;
			foreach (var t in transform.Children().SelectMany(Descendants)) yield return t;
		}

		public static IEnumerable<Transform> SelfAndDescendants(this Transform transform) {
			yield return transform;
			foreach (var t in transform.Descendants()) yield return t;
		}
	}
}