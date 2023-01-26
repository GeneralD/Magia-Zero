using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Zero.Generator.Entity;
using Action = System.Action;

namespace Editor.Zero {
	[CustomPropertyDrawer(typeof(TargetSubject))]
	public class TargetSubjectPropertyDrawer : PropertyDrawer {
		public override VisualElement CreatePropertyGUI(SerializedProperty property) {
			var ui = new VisualElement();
			var xml = Resources.Load<VisualTreeAsset>(GetType().Name);
			xml.CloneTree(ui);
			ui.Bind(property.serializedObject);
			ui.Q<Button>("test_regex_button").clicked += TestRegex(property, ui.Q<TextField>("test_text_field"));
			return ui;
		}

		private static Action TestRegex(SerializedProperty property, TextField testTextField) => () => {
			var targetSubject = new TargetSubject {
				name = property.FindPropertyRelative("name").stringValue,
				useRegex = property.FindPropertyRelative("useRegex").boolValue,
				invertMatch = property.FindPropertyRelative("invertMatch").boolValue,
			};

			try {
				var testText = testTextField.value;
				EditorUtility.DisplayDialog("Test Result",
					targetSubject.IsMatch(testText)
						? "Matched🎉🎉"
						: "Not Matched💦💦", "OK");
			} catch (ArgumentException) {
				EditorUtility.DisplayDialog(
					"Regex Error",
					$"`{targetSubject.name}` is invalid regex expression.",
					"OK");
			}
		};
	}
}