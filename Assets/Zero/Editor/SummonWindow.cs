using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Zero {
    public class SummonWindow : EditorWindow {
        [SerializeField] private GameObject rootObject;

        public static void Open() {
            var window = GetWindow<SummonWindow>(title: "Summon");
            window.ShowTab();
        }

        private void OnEnable() {
            var visualTree = Resources.Load<VisualTreeAsset>(GetType().Name);
            var root = rootVisualElement;
            visualTree.CloneTree(root);
            root.Bind(new SerializedObject(this));

            root.Q<Button>("executeButton").clicked += OnExecuteButtonClicked;
        }

        private void OnDisable() {
            var root = rootVisualElement;
            root.Q<Button>("executeButton").clicked -= OnExecuteButtonClicked;
        }

        private void OnExecuteButtonClicked() {
            Debug.Log(rootObject.name);
        }
    }
}