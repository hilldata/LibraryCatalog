using System;
using System.Collections.Generic;
using System.Text;

namespace XRD.LibCat.Models {
	internal static class TextConcatenator {
		internal const char COMBINING_CHAR = (char)0x033c;
		internal static string[] Split(string concatText) {
			if (string.IsNullOrWhiteSpace(concatText))
				return null;
			return concatText.Split(new char[] { COMBINING_CHAR }, StringSplitOptions.RemoveEmptyEntries);
		}

		internal static string Concatenate(IEnumerable<string> vs) {
			if (vs.IsNullOrEmpty())
				return null;
			StringBuilder sb = new StringBuilder(COMBINING_CHAR);
			foreach(var s in vs) {
				if(!string.IsNullOrWhiteSpace(s)) {
					sb.Append(s.Trim());
					sb.Append(COMBINING_CHAR);
				}
			}
			return sb.ToString();
		}

		internal static string GetConcatenatedMatchParameter(string textToFind) {
			if (textToFind == null)
				return null;
			else if (string.IsNullOrEmpty(textToFind))
				return string.Empty;
			else
				return $"{COMBINING_CHAR}{textToFind.Trim()}{COMBINING_CHAR}";
		}
	}
}
