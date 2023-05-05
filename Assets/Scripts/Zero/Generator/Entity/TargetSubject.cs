using System;
using System.Text.RegularExpressions;

namespace Zero.Generator.Entity {
	[Serializable]
	public class TargetSubject {
		public string name;
		public bool useRegex = true;
		public bool invertMatch;

		public bool IsMatch(string text)
			=> invertMatch != (useRegex ? new Regex(name).IsMatch(text) : name == text);
	}
}