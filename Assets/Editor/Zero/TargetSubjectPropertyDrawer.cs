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

        private static Action TestRegex(SerializedProperty property, TextField testTextField) {
            return () => {
                var name = property.FindPropertyRelative("name").stringValue;

                try {
                    var regex = new Regex(name);
                    var testText = testTextField.value;
                    var isMatched = regex.IsMatch(testText);
                    EditorUtility.DisplayDialog("Test Result",
                        isMatched
                            ? $"`{testText}` is matched to the regex expression."
                            : $"`{testText}` is NOT matched to the regex expression.", "OK");
                }
                catch (ArgumentException argumentException) {
                    EditorUtility.DisplayDialog("Regex Error", $"`{name}` is invalid regex expression.", "OK");
                }
            };
        }
    }
}