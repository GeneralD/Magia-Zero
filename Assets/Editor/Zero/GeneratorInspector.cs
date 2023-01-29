using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Zero.Generator;

namespace Editor.Zero {
	[CustomEditor(typeof(Generator))]
	public class GeneratorInspector : UnityEditor.Editor {
		[SerializeField]
		private VisualTreeAsset editorUxml;

		private Generator Generator => target as Generator;

		public override VisualElement CreateInspectorGUI() {
			var ui = new VisualElement();
			editorUxml.CloneTree(ui);
			ui.Bind(new SerializedObject(target));
			ui.Q<Button>("execute_button").clicked += Generator.Generate;
			ConvertHomeDirectoryExpressionOnTheFly(ui.Q<TextField>("output_directory_textfield"));
			return ui;
		}

		private static void ConvertHomeDirectoryExpressionOnTheFly(TextField textField) {
			var weak = new WeakReference<TextField>(textField);
			var regex = new Regex(@"^~");
			var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

			textField.RegisterValueChangedCallback(ev => {
				if (!weak.TryGetTarget(out var tf)) return;
				tf.value = regex.Replace(ev.newValue, _ => homeDir);
			});
		}
	}
}