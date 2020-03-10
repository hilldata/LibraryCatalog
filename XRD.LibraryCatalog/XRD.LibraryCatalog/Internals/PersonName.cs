using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace XRD.LibCat {
	/// <summary>
	/// Structured person's name. Used to parse a text into a structured name.
	/// </summary>
	public class PersonName {
		/// <summary>
		/// Construct an instance of PersonName by parsing the text provided.
		/// </summary>
		/// <param name="text">The text to parse into a name.</param>
		public PersonName(string text) {
			if (string.IsNullOrWhiteSpace(text))
				throw new ArgumentNullException(nameof(text));

			string working = text.Trim();
			Prefix = string.Empty;
			Last = string.Empty;
			First = string.Empty;
			Middle = string.Empty;
			Suffix = string.Empty;
			Nickname = string.Empty;

			// Check for nicknames (in quotes)
			if (working.Contains("\"")) {
				int x1 = working.IndexOf("\"");
				int x2 = working.IndexOf("\"", x1 + 1);
				if (x2 > x1) {
					Nickname = working.Substring(x1 + 1, x2 - x1 - 1).Trim();
					working = text.Replace("\"" + Nickname + "\"", string.Empty);
				}
			} else if (working.Contains("'")) {
				int x1 = working.IndexOf("'");
				int x2 = working.IndexOf("'", x1 + 1);
				if (x2 > x1) {
					Nickname = working.Substring(x1 + 1, x2 - x1 - 1).Trim();
					working = working.Replace("'" + Nickname + "'", string.Empty);
				}
			}

			// Check for known prefixes.
			foreach (var prefix in Honorifics.ForParsingName) {
				if (working.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) {
					Prefix = prefix.Trim();
					working = working.Substring(prefix.Length).Trim();
				}
			}

			StringBuilder m = new StringBuilder();

			// Check to see if text contains comma.
			if (working.Contains(",")) {
				int commaInd = working.IndexOf(",");
				int spaceInd = working.IndexOf(" ");
				// If a space occurs before the comma, it's most likely formatted as [First Last, Suffix]
				if (spaceInd > 0 && spaceInd < commaInd) {
					Suffix = working.Substring(commaInd + 1).Trim();
					var split = splitText(working.Substring(0, commaInd)).ToList();
					if (split.Count > 2) {
						First = split.First().Trim();
						Last = split.Last().Trim();
						foreach (var s in split.Skip(1).Take(split.Count - 2)) {
							m.Append(" ");
							m.Append(s);
						}
						Middle = m.ToString().Trim();
					} else if (split.Count == 2) {
						First = split.First().Trim();
						Last = split.Last().Trim();
					} else {
						Last = split[0].Trim();
					}
				} else { // If there is no space before the comma, it's most likely formatted as [Last, First]
					Last = working.Substring(0, commaInd).Trim();
					working = working.Substring(commaInd + 1).Trim();
					if (!working.Contains(" ")) {
						First = working.Trim();
					} else {
						var split = splitText(working);
						First = split.First().Trim();
						foreach (var s in split.Skip(1)) {
							m.Append(" ");
							m.Append(s.Trim());
						}
						Middle = m.ToString().Trim();
						// Check to see if there's another comma in the remainer, if so, what follows is a suffix
						if (Middle.Contains(",")) {
							Suffix = Middle.Substring(Middle.IndexOf(",") + 1).Trim();
							Middle = Middle.Substring(0, Middle.IndexOf(",")).Trim();
						}
					}
				}
			} else { // No commas. Search for common suffixes and assume it's formatted as [First Last]
				if (!working.Contains(" ")) {// No space. It's just a last name.
					Last = working.Trim();
				} else {
					var split = splitText(working).ToList();
					string suff = string.Empty;
					foreach (var s in split) {
						if (CommonSuffixes.Contains(s, StringComparer.OrdinalIgnoreCase)) {
							suff = s;
						}
					}
					if (!string.IsNullOrEmpty(suff)) {
						split.Remove(suff);
						Suffix = suff.Trim();
					}
					if (split.Count == 1)
						Last = split.First().Trim();
					else if (split.Count == 2) {
						First = split.First().Trim();
						Last = split.Last().Trim();
					} else {
						First = split.First().Trim();
						Last = split.Last().Trim();
						foreach (var s in split.Skip(1).Take(split.Count - 2)) {
							m.Append(" ");
							m.Append(s);
						}
						Middle = m.ToString().Trim();
					}
				}
			}
		}

		/// <summary>
		/// Construct an instance of PersonName using the provided values.
		/// </summary>
		/// <param name="first">The first/given name.</param>
		/// <param name="last">The last/family name.</param>
		/// <param name="prefix">Honorific prefix</param>
		/// <param name="middle">Middle name(s)</param>
		/// <param name="suffix">Suffix (generational or credentials)</param>
		/// <param name="nickname">Preferred nickname/alias.</param>
		public PersonName(string first, string last, string prefix = null, string middle = null, string suffix = null, string nickname = null) {
			Prefix = prefix.HasValue() ? prefix.Trim() : string.Empty;
			First = first.HasValue() ? first.Trim() : string.Empty;
			Middle = middle.HasValue() ? middle.Trim() : string.Empty;
			Last = last.HasValue() ? last.Trim() : string.Empty;
			Suffix = suffix.HasValue() ? suffix.Trim() : string.Empty;
			Nickname = nickname.HasValue() ? nickname.Trim() : string.Empty;
		}

		/// <summary>
		/// Converts the string into a structured personal name.
		/// </summary>
		/// <param name="text">A string that contains the value to parse.</param>
		/// <returns>The structured personal name.</returns>
		public static PersonName Parse(string text) => new PersonName(text);
		/// <summary>
		/// Converts the string into a structure personal name and returns a value that indicates whether the conversion succeeded.
		/// </summary>
		/// <param name="text">A string that contains the value to parse.</param>
		/// <param name="name">The structured personal name.</param>
		/// <returns>A value that indicates whether the conversion succeeded.</returns>
		public static bool TryParse(string text, out PersonName name) {
			if (string.IsNullOrWhiteSpace(text)) {
				name = default;
				return false;
			}
			name = new PersonName(text);
			return true;
		}

		/// <summary>
		/// The person's honorific prefix.
		/// </summary>
		public string Prefix { get; set; }
		/// <summary>
		/// The person's first/given name.
		/// </summary>
		public string First { get; set; }
		/// <summary>
		/// The person's middle name(s)/initial(s).
		/// </summary>
		public string Middle { get; set; }
		/// <summary>
		/// The person's last/family name.
		/// </summary>
		public string Last { get; set; }
		/// <summary>
		/// The person's suffix.
		/// </summary>
		public string Suffix { get; set; }
		/// <summary>
		/// The person's preferred nickname/alias.
		/// </summary>
		public string Nickname { get; set; }

		/// <summary>
		/// List of common personal suffixes.
		/// </summary>
		public static List<string> CommonSuffixes => new List<string>() {
			"JR",
			"JR.",
			"SR",
			"SR.",
			"I",
			"II",
			"III",
			"IV",
			"V",
			"VI",
			"MD",
			"M.D.",
			"PHD",
			"Ph.D.",
			"DO",
			"D.O."
		};

		private string[] splitText(string text, char c = ' ') {
			if (!text.Contains(" "))
				return new string[] { text.Trim() };
			return text.Split(new char[] { c }, StringSplitOptions.RemoveEmptyEntries);
		}

		/// <summary>
		/// Gets the initials from the name
		/// </summary>
		/// <param name="maxLength">The maximum number of characters to include in the initials.</param>
		/// <returns>The initials from the name.</returns>
		public string ToInitials(int maxLength = 3) {
			StringBuilder sb = new StringBuilder();
			if (First.HasValue())
				sb.Append(First[0]);
			if (Middle.HasValue()) {
				if (Middle.Contains(" ")) {
					foreach (var v in splitText(Middle))
						sb.Append(v.Trim()[0]);
				} else
					sb.Append(Middle[0]);
			}
			if (Last.HasValue()) {
				if (Last.Contains(" ")) {
					foreach (var v in splitText(Last))
						sb.Append(v.Trim()[0]);
				}
			} else
				sb.Append(Last[0]);

			if (maxLength < 2)
				maxLength = 2;
			if (sb.Length > maxLength) {
				while (sb.Length > maxLength)
					sb.Remove(1, 1);
			}
			return sb.ToString();
		}

		private const string COMMA = ", ";
		private const string SPACE = " ";
		private const string QUOTE1 = "\"";
		private const string QUOTE2 = "\" ";
		private const string PAREN1 = "(";
		private const string PAREN2 = ") ";

		/// <summary>
		/// Build a person's formatted full name for display using the provided criteria.
		/// </summary>
		/// <param name="orgName">The name of the organization the person is primarily associated with.</param>
		/// <param name="includeMiddle">Should the middle name(s) be included in the result?</param>
		/// <param name="includePrefix">Should the honorific prefix be included in the result?</param>
		/// <param name="includeSuffix">Should the honorific suffix be included in the result?</param>
		/// <param name="includeNickname">Should the nickname be included in the result?</param>
		/// <param name="lastFirst">Should the last/family name be listed first?</param>
		/// <returns>The full name as computed from the provided criteria.</returns>
		public string ToFullName(string orgName = null, bool includeMiddle = false, bool includePrefix = false, bool includeSuffix = true, bool includeNickname = true, bool lastFirst = true) {
			StringBuilder sb = new StringBuilder();
			if (lastFirst) {
				if (Last.HasValue()) {
					sb.Append(Last);
					sb.Append(COMMA);
				}
				if (First.HasValue()) {
					sb.Append(First);
					sb.Append(SPACE);
				}
				if (includeNickname && Nickname.HasValue()) {
					sb.Append(QUOTE1);
					sb.Append(Nickname);
					sb.Append(QUOTE2);
				}
				if (includeMiddle && Middle.HasValue()) {
					sb.Append(Middle);
					sb.Append(SPACE);
				}
				if (includePrefix && Prefix.HasValue()) {
					sb.Append(PAREN1);
					sb.Append(Prefix);
					sb.Append(PAREN2);
				}
				if (includeSuffix && Suffix.HasValue()) {
					sb.Append(COMMA);
					sb.Append(Suffix);
				}
			} else {
				if (includePrefix && Prefix.HasValue()) {
					sb.Append(Prefix);
					sb.Append(SPACE);
				}
				if (First.HasValue()) {
					sb.Append(First);
					sb.Append(SPACE);
				}
				if (includeNickname && Nickname.HasValue()) {
					sb.Append(QUOTE1);
					sb.Append(Nickname);
					sb.Append(QUOTE2);
				}
				if (includeMiddle && Middle.HasValue()) {
					sb.Append(Middle);
					sb.Append(SPACE);
				}
				if (Last.HasValue()) {
					sb.Append(Last);
				}
				if (includeSuffix && Suffix.HasValue()) {
					sb.Append(COMMA);
					sb.Append(Suffix);
				}
			}

			if (orgName.HasValue()) {
				sb.Append(SPACE);
				sb.Append(PAREN1);
				sb.Append(orgName);
				sb.Append(PAREN2);
			}

			string res = sb.ToString().Trim();
			while (res.EndsWith(","))
				res = res.Substring(0, res.Length - 1);
			return res;
		}

		public string ToFullName(PersonalNameProperties includedFields, string orgName = null, bool lastFirst = true) =>
			ToFullName(orgName,
				includeMiddle: includedFields.HasFlag(PersonalNameProperties.Middle),
				includePrefix: includedFields.HasFlag(PersonalNameProperties.Prefix),
				includeSuffix: includedFields.HasFlag(PersonalNameProperties.Suffix),
				includeNickname: includedFields.HasFlag(PersonalNameProperties.Nickname),
				lastFirst
			);

		/// <summary>
		/// Indexer for name properties.
		/// </summary>
		/// <param name="name">The name property to get or set.</param>
		/// <returns>The value of the name property.</returns>
		public string this[PersonalNameProperties name] {
			get {
				switch (name) {
					case PersonalNameProperties.First: return First.HasValue() ? First : string.Empty;
					case PersonalNameProperties.Last: return Last.HasValue() ? Last : string.Empty;
					case PersonalNameProperties.Middle: return Middle.HasValue() ? Middle : string.Empty;
					case PersonalNameProperties.Nickname: return Nickname.HasValue() ? Nickname : string.Empty;
					case PersonalNameProperties.Prefix: return Prefix.HasValue() ? Prefix : string.Empty;
					case PersonalNameProperties.Suffix: return Suffix.HasValue() ? Suffix : string.Empty;
					default: return string.Empty;
				}
			}
			set {
				switch (name) {
					case PersonalNameProperties.First:
						First = value.HasValue() ? value.Trim() : string.Empty;
						break;
					case PersonalNameProperties.Last:
						Last = value.HasValue() ? value.Trim() : string.Empty;
						break;
					case PersonalNameProperties.Middle:
						Middle = value.HasValue() ? value.Trim() : string.Empty;
						break;
					case PersonalNameProperties.Nickname:
						Nickname = value.HasValue() ? value.Trim() : string.Empty;
						break;
					case PersonalNameProperties.Prefix:
						Prefix = value.HasValue() ? value.Trim() : string.Empty;
						break;
					case PersonalNameProperties.Suffix:
						Suffix = value.HasValue() ? value.Trim() : string.Empty;
						break;
					default: break;
				}
			}
		}

		/// <summary>
		/// Get the value of the specified name property
		/// </summary>
		/// <param name="name">The name property to retrieve</param>
		/// <param name="maxLen">The maximum length of the value to return. If the actual value is longer, it is <see cref="TextExtensions.Truncate(string, int)"/>d to the specified length.</param>
		/// <returns>Either <see cref="string.Empty"/> (for null or empty values) or the value of the name property truncated to the specified maxLen (if applicable).</returns>
		public string GetValue(PersonalNameProperties name, int maxLen = 50) {
			switch (name) {
				case PersonalNameProperties.First: return First.HasValue() ? First.Truncate(maxLen) : string.Empty;
				case PersonalNameProperties.Last: return Last.HasValue() ? Last.Truncate(maxLen) : string.Empty;
				case PersonalNameProperties.Middle: return Middle.HasValue() ? Middle.Truncate(maxLen) : string.Empty;
				case PersonalNameProperties.Nickname: return Nickname.HasValue() ? Nickname.Truncate(maxLen) : string.Empty;
				case PersonalNameProperties.Prefix: return Prefix.HasValue() ? Prefix.Truncate(maxLen) : string.Empty;
				case PersonalNameProperties.Suffix: return Suffix.HasValue() ? Suffix.Truncate(50) : string.Empty;
				default: return string.Empty;
			}
		}
	}

	/// <summary>
	/// Enumeration of the PersonalName properties.
	/// </summary>
	[Flags]
	public enum PersonalNameProperties {
		/// <summary>
		/// The first or given name.
		/// </summary>
		First = 0x_0001,
		/// <summary>
		/// The middle name(s) or initial(s)
		/// </summary>
		Middle = 0x_0010,
		/// <summary>
		/// The last or family name.
		/// </summary>
		Last = 0x_0100,
		/// <summary>
		/// Honorific prefix.
		/// </summary>
		Prefix = 0x_1000,
		/// <summary>
		/// Generational or credential suffix.
		/// </summary>
		Suffix = 0x_0001_0000,
		/// <summary>
		/// Preferred nickname or alias.
		/// </summary>
		Nickname = 0x_0010_0000
	}
}