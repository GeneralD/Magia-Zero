using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UniVRM10;
using Zero.Generator.Entity;

namespace Editor.Zero {
	[CustomPropertyDrawer(typeof(VRM10ObjectMeta))]
	public class VRM10ObjectMetaPropertyDrawer : PropertyDrawer {
		public override VisualElement CreatePropertyGUI(SerializedProperty property) {
			var ui = new VisualElement();
			var xml = Resources.Load<VisualTreeAsset>(GetType().Name);
			xml.CloneTree(ui);
			ui.Bind(property.serializedObject);
			return ui;
		}
	}
}