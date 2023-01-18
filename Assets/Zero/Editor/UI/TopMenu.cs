using UnityEditor;
using UnityEngine;

namespace Zero {
    public abstract class TopMenu {
        private const string MenuPrefix = "Magia 零";

        [MenuItem(MenuPrefix + "/New Generator %#&0")]
        private static void NewGenerator() => new GameObject("Generator").AddComponent<Generator>();
    }
}