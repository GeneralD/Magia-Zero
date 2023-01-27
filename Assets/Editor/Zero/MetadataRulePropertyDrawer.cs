using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Zero.Generator.Entity;

namespace Editor.Zero {
	[CustomPropertyDrawer(typeof(MetadataRule))]
	public class MetadataRulePropertyDrawer : PropertyDrawer {
		public override VisualElement CreatePropertyGUI(SerializedProperty property) {
			var ui = new VisualElement();
			var xml = Resources.Load<VisualTreeAsset>(GetType().Name);
			xml.CloneTree(ui);
			ui.Bind(property.serializedObject);
			return ui;
		}
	}
}