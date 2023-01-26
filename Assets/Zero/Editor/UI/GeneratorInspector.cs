using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Zero.Generator;

namespace GeneratorInspector {
    [CustomEditor(typeof(Generator))]
    public class GeneratorInspector : Editor {
        [SerializeField] private VisualTreeAsset editorUxml;

        private Generator Generator => target as Generator;

        public override VisualElement CreateInspectorGUI() {
            var ui = new VisualElement();
            editorUxml.CloneTree(ui);
            ui.Bind(new SerializedObject(target));
            ui.Q<Button>("execute_button").clicked += Generator.Generate;
            return ui;
        }
    }
}