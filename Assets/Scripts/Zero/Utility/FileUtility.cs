using System.IO;

namespace Zero.Utility {
	public static class FileUtility {
		public static void CreateTextFile(string path, string text, bool createDirectory = true) {
			var file = new FileInfo(path);
			if (createDirectory) file.Directory?.Create();
			using var writer = file.CreateText();
			writer.Write(text);
		}
	}
}