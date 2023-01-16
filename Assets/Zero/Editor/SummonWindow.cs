using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Zero {
    public class SummonWindow : EditorWindow {
        public static void Open() {
            var window = GetWindow<SummonWindow>(title: "Summon");
            window.ShowTab();
        }

        private void OnEnable() {
            // load UXML and USS
            var visualTree = Resources.Load<VisualTreeAsset>(GetType().Name);
            // apply them
            var root = rootVisualElement;
            visualTree.CloneTree(root);
        }

        private void OnGUI() { }
    }
}