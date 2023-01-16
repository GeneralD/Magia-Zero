using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Zero {
    public class SummonWindow : EditorWindow {
        [SerializeField] private GameObject rootObject;
        [SerializeField] private string outputDirectoryUri = "~Downloads/Zero";
        [SerializeField] private string vrmOutputDirectoryName = "VRMs";
        [SerializeField] [Min(0)] private int startIndex = 1;
        [SerializeField] [Min(0)] private int quantity = 10;
        [SerializeField] private bool overwriteExist = false;
        [SerializeField] private string nameFormat = "%d";
        [SerializeField] private bool hashName = false;

        public static void Open() {
            var window = GetWindow<SummonWindow>(title: "Summon");
            window.ShowTab();
            window.minSize = new Vector2(450, 600);
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