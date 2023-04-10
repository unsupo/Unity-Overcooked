using System;
using System.Text.RegularExpressions;
using UnityEngine;
using static UnityEngine.UI.Image;
using System.Linq;
using System.Text;

public class Utility {
	public static bool TryGetChildAndComponent<T>(GameObject parent, string name, out T child) {
		if(TryGetChild(parent, name, out GameObject childl) && childl.TryGetComponent(out T c)) {
			child = c;
			return true;
		}
		child = default;
		return false;
	}

	public static bool TryGetChildAndComponentByPath<T>(GameObject parent, string path, out T child) {
		foreach(string p in path.Split("/")) {
			if(TryGetChildByPath(parent, p, out GameObject child1)) {
				parent = child1;
			} else {
				child = default;
				return false;
			}
		}
		if(parent.TryGetComponent(out T c)) {
			child = c;
			return true;
		}
		child = default;
		return false;
	}

	public static bool TryGetChildByPath(GameObject parent, string path, out GameObject child) {
		foreach(string p in path.Split("/"))
			if(TryGetChild(parent, p, out GameObject child1)) {
				parent = child1;
			} else {
				child = null;
				return false;
			}
		child = parent;
		return true;
	}

	public static bool TryGetChild(GameObject parent, string name, out GameObject child) {
		foreach(Transform c in parent.GetComponentInChildren<Transform>())
			if(c.name.Equals(name)) {
				child = c.gameObject;
				return true;
			}
		child = null;
		return false;
	}


	public static string FormatTime(float timeLeftSeconds) {
		//return TimeSpan.FromSeconds(timeLeftSeconds).ToString("mm:ss");
		int timeInSecondsInt = (int) timeLeftSeconds;  //We don't care about fractions of a second, so easy to drop them by just converting to an int
		int minutes = (int) (timeLeftSeconds / 60);  //Get total minutes
		int seconds = timeInSecondsInt - (minutes * 60);  //Get seconds for display alongside minutes
		return minutes.ToString("D2") + ":" + seconds.ToString("D2");  //Create the string representation, where both seconds and minutes are at minimum 2 digits
	}

	internal static T ThrowGetChildAndComponent<T>(GameObject parent, string name) {
		if(TryGetChildAndComponent<T>(parent, name, out T child))
			return child;
		throw new Exception("No child or component: "+parent+", "+name+", "+child+", "+typeof(T));
	}
	internal static T ThrowGetChildAndComponentByPath<T>(GameObject parent, string path) {
		if(TryGetChildAndComponentByPath<T>(parent, path, out T child))
			return child;
		throw new Exception("No child or component: " + parent + ", " + path + ", " + typeof(T));
	}
	internal static GameObject ThrowGetChildByPath(GameObject parent, string path) {
		if(TryGetChildByPath(parent, path, out GameObject child))
			return child;
		throw new Exception("No child or component: " + parent + ", " + path);
	}
	internal static string ToPascalCase(string original) {
		Regex invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
		Regex whiteSpace = new Regex(@"(?<=\s)");
		Regex startsWithLowerCaseChar = new Regex("^[a-z]");
		Regex firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$");
		Regex lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
		Regex upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

		// replace white spaces with undescore, then replace all invalid chars with empty string
		var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(original, "_"), string.Empty)
			// split by underscores
			.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
			// set first letter to uppercase
			.Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
			// replace second and all following upper case letters to lower if there is no next lower (ABC -> Abc)
			.Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
			// set upper case the first lower case following a number (Ab9cd -> Ab9Cd)
			.Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
			// lower second and next upper case letters except the last if it follows by any lower (ABcDEf -> AbcDef)
			.Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));

		return string.Concat(pascalCase);
	}
	internal static string ToSnakeCase(string text) {
		if(text == null) {
			throw new ArgumentNullException(nameof(text));
		}
		if(text.Length < 2) {
			return text;
		}
		var sb = new StringBuilder();
		sb.Append(char.ToLowerInvariant(text[0]));
		for(int i = 1; i < text.Length; ++i) {
			char c = text[i];
			if(char.IsUpper(c)) {
				sb.Append('_');
				sb.Append(char.ToLowerInvariant(c));
			} else {
				sb.Append(c);
			}
		}
		return sb.ToString();
	}
}

