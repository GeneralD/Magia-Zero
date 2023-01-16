using UnityEditor;

namespace Zero {
	public abstract class TopMenu {
		private const string MenuPrefix = "Magia 零";

		[MenuItem(MenuPrefix + "/Summon %#&0")]
		private static void OpenSummonWindow() => SummonWindow.Open();
	}
}
