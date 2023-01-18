using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Zero;

namespace GeneratorInspector {
    [CustomEditor(typeof(Generator))]
    public class GeneratorInspector : Editor {
        [SerializeField] private VisualTreeAsset _editorUXML;

        private Generator Generator => target as Generator;

        public override VisualElement CreateInspectorGUI() {
            var ui = new VisualElement();
            _editorUXML.CloneTree(ui);
            ui.Bind(new SerializedObject(target));
            ui.Q<Button>("executeButton").clicked += Generator.Generate;
            return ui;
        }
    }
}