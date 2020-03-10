using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace XRD.LibCat {
	internal static class TextExtensions {
		internal static bool HasValue(this string s) => !string.IsNullOrWhiteSpace(s);

		internal const char ELLIPSIS = (char)8230;
		internal static bool IsValidEmail(this string text) {
			if (string.IsNullOrWhiteSpace(text))
				return false;
			if (!text.EqualsWildcard("*@*.*"))
				return false;
			return true;
		}
		private static readonly char[] DEFAULT_SPACE_CHARS = { '_', '+' };

		/// <summary>
		/// "Splits" a camel-cased word by inserting spaces before upper-cased characters. Also replaces punctuation with spaces and removes lower-case prefixes from the front of the word.
		/// </summary>
		/// <param name="s">The word to split</param>
		/// <returns>String of words with spaces inserted</returns>
		internal static string SplitCamelCase(this string s) => s.SplitCamelCase(true, DEFAULT_SPACE_CHARS);
		/// <summary>
		/// "Splits" a camel-cased word by inserting spaces before upper-cased characters. Also replaces specified character with spaces and optionally removes lower-case prefixes from the front of the word.
		/// </summary>
		/// <param name="s">The word to split</param>
		/// <param name="removePrefix">Should the lower-case prefix be removed from the front of the word (if exists)?</param>
		/// <param name="spaceChars">Array of <see cref="char"/>s to be replaced with spaces</param>
		/// <returns>String of words with spaces inserted</returns>
		internal static string SplitCamelCase(this string s, bool removePrefix, params char[] spaceChars) {
			if (string.IsNullOrWhiteSpace(s))
				return string.Empty;
			string res = s.Trim();

			// Remove any prefix (lower-case)
			if (removePrefix) {
				int pos = -1;
				for (int i = 0; i < res.Length; i++) {
					if (char.IsUpper(res[i])) {
						pos = i;
						break;
					}
				}
				if (pos > 0)
					res = res.Substring(pos);
			}

			// Work backward through the string to insert a space before every capital letter if the following is lower-case (to avoid acronyms)
			bool currUpper;
			bool nextUpper = false;
			for (int i = res.Length - 1; i > 0; i--) {
				currUpper = char.IsUpper(res[i]);
				if (currUpper && !nextUpper)
					res = res.Insert(i, " ");
				nextUpper = currUpper;
			}

			// Replace any "spaceChars" with spaces
			if (spaceChars != null && spaceChars.Length > 0) {
				foreach (char c in spaceChars)
					res = res.Replace(c, ' ');
			}

			// Remove any double-spaces.
			while (res.IndexOf("  ") >= 0)
				res = res.Replace("  ", " ");

			return res.Trim();
		}

		private static readonly List<string> SingularPluralSame = new List<string>() {
			"advice",
			"aircraft",
			"bison",
			"caribou",
			"cattle",
			"chalk",
			"chassis",
			"chinos",
			"clothing",
			"cod",
			"concrete",
			"correspondence",
			"deer",
			"elk",
			"faux pas",
			"fish",
			"flour",
			"food",
			"furnature",
			"haddock",
			"halibut",
			"help",
			"homework",
			"hovercraft",
			"insignia",
			"knickers",
			"knowledge",
			"kudos",
			"luggage",
			"moose",
			"offspring",
			"pyjamas",
			"police",
			"pendezvous",
			"salmon",
			"sheep",
			"shrimp",
			"spacecraft",
			"squid",
			"swine",
			"trout",
			"tuna",
			"you",
			"wheat",
			"wood"
		};
		private static readonly Dictionary<string, string> IrregularNouns = new Dictionary<string, string>() {
			{"agenda", "agendas" },
			{"belief", "beliefs" },
			{"chef", "chefs" },
			{"chief", "chiefs" },
			{"child", "children" },
			{"concerto", "concertos" },
			{"die", "dice" },
			{"fez", "fezzes" },
			{"foot", "feet" },
			{"gas", "gasses" },
			{"genus", "genera" },
			{"goose", "geese" },
			{"halo", "halos" },
			{"louse", "lice" },
			{"man", "men" },
			{"mouse", "mice" },
			{"octopus", "octopuses" },
			{"opus", "opera" },
			{ "person", "people" },
			{"photo", "photos" },
			{"piano", "pianos" },
			{"roof", "roofs" },
			{"tooth", "teeth" },
			{"virus", "viruses" },
			{"woman", "women" }
		};

		private static bool IsVowel(this char c) {
			if (!char.IsLetter(c))
				return false;
			char[] vowels = new char[] { 'a', 'e', 'i', 'o', 'u' };
			return vowels.Contains(c);
		}
		private static bool IsConsonant(this char c) {
			if (!char.IsLetter(c))
				return false;
			return !c.IsVowel();
		}
		private static string DropFromEnd(this string s, int count) {
			if (s.Length < count)
				throw new ArgumentOutOfRangeException(nameof(count));
			return s.Substring(0, s.Length - count);
		}
		private static char FromEnd(this string s, int reverseIndex) {
			if (s.Length < reverseIndex)
				throw new ArgumentOutOfRangeException(nameof(reverseIndex));
			return s[s.Length - 1 - reverseIndex];
		}

		/// <summary>
		/// Pluralize the specified word
		/// </summary>
		/// <param name="s">The word to pluralize</param>
		/// <returns>The word in its plural form</returns>
		internal static string Pluralize(this string s) {
			if (string.IsNullOrWhiteSpace(s))
				return string.Empty;
			s = s.Trim();
			string temp = s.Trim().ToLower();

			// First, check to see if already plural
			if (temp.EndsWith("es") || (temp.FromEnd(0) == 's' && temp.FromEnd(1).IsConsonant()))
				return s;

			// Check identical plural/singular list
			foreach (var ident in SingularPluralSame) {
				if (temp.EndsWith(ident))
					return s;
			}

			// Check irregular list
			foreach (var irr in IrregularNouns) {
				if (temp.EndsWith(irr.Key) || temp.EndsWith(irr.Value))
					return s.DropFromEnd(irr.Key.Length - 1) + irr.Value.Substring(1);
			}

			// Apply rules
			if (temp.EndsWith("a"))
				return s + "e";
			else if (temp.EndsWith("eau"))
				return s + "x";
			else if (temp.EndsWith("ex"))
				return s.DropFromEnd(2) + "ices";
			else if (temp.EndsWith("f"))
				return s.DropFromEnd(1) + "ves";
			else if (temp.EndsWith("fe"))
				return s.DropFromEnd(2) + "ves";
			else if (temp.EndsWith("is"))
				return s.DropFromEnd(2) + "es";
			else if (temp.EndsWith("ix"))
				return s.DropFromEnd(1) + "ces";
			else if (temp.EndsWith("on"))
				return s.DropFromEnd(2) + "a";
			else if (temp.EndsWith("um"))
				return s.DropFromEnd(2) + "a";
			else if (temp.EndsWith("us"))
				return s.DropFromEnd(2) + "i";
			else if (temp.EndsWith("ch") || temp.EndsWith("o") || temp.EndsWith("s") || temp.EndsWith("sh") || temp.EndsWith("ss") || temp.EndsWith("x") || temp.EndsWith("z"))
				return s + "es";
			else if (temp.EndsWith("y"))
				return s.Substring(0, s.Length - 2) + "ies";
			else
				return s + "s";
		}

		/// <summary>
		/// Trims and truncates the input to not exceed the specified length (appends an ellipsis if any characters were truncated).
		/// </summary>
		/// <param name="text">The text to truncate</param>
		/// <param name="maxLen">The maximum length of the result. (default = 250 characters). Less than or equal to ZERO simply trims the string.</param>
		/// <returns>The truncated string. If any characters were actually truncated, an <see cref="ELLIPSIS"/> char is appended</returns>
		internal static string Truncate(this string text, int maxLen = 250) {
			if (string.IsNullOrWhiteSpace(text))
				return string.Empty;

			string temp = text.Trim();
			if (maxLen <= 0 || temp.Length <= maxLen + 1)
				return temp;
			int cutoff = temp.IndexOfPreviousNonBreakingChar(maxLen - 1);
			if (cutoff <= 0)
				return temp.Substring(0, maxLen - 1) + ELLIPSIS;
			else
				return temp.Substring(0, cutoff + 1) + ELLIPSIS;
		}

		/// <summary>
		/// Trims and truncates the input to not exceed the specified length (nothing is appended if any characters were truncated).
		/// </summary>
		/// <param name="text">The text to truncate</param>
		/// <param name="maxLen">The maximum length of the result. (default = 250 characters). Less than or equal to ZERO simply trims the string.</param>
		/// <returns>The truncated string.</returns>
		internal static string TrimTo(this string text, int maxLen = 250) {
			if (string.IsNullOrWhiteSpace(text))
				return string.Empty;

			string temp = text.Trim();
			if (maxLen <= 0 || temp.Length <= maxLen + 1)
				return temp;
			return temp.Substring(0, maxLen);
		}


		// Private method used to find the index of the closest (previous) non-breaking (letter or number) character before the specified maximum length.
		internal static int IndexOfPreviousNonBreakingChar(this string text, int maxLen) {
			if (string.IsNullOrWhiteSpace(text))
				return -1;
			if (text.Length <= maxLen)
				return -1;
			int counter = maxLen - 1;
			bool breakFound = false;
			for (int i = maxLen - 1; i > 0; i--) {
				if (IsBreakingChar(text[i])) {
					if (!breakFound)
						breakFound = true;
				} else if (breakFound)
					return i;
			}
			return -1;
		}

		// Private method used to determine if the specified character is a breaking character (control, whitespace, punctuation, etc.)
		private static bool IsBreakingChar(char charToCheck) {
			return !char.IsLetterOrDigit(charToCheck);
		}

		/// <summary>
		/// Wildcard character for a single matching character.
		/// </summary>
		internal const char SINGLE_CHAR_WILDCARD = '?';
		/// <summary>
		/// Wildcard character for multiple/no matching characters.
		/// </summary>
		internal const char MULTI_CHAR_WILDCARD = '*';

		/// <summary>
		/// Wildcard character for a single matching character (for TSQL LIKE filters)
		/// </summary>
		internal const char TSQL_SINGLE_CHAR_WILDCARD = '_';
		/// <summary>
		/// Wildcard character for multiple/no matching characters (for TSQL LIKE filters)
		/// </summary>
		internal const char TSQL_MULTI_CHAR_WILDCARD = '%';

		/// <summary>
		/// Convert a string with standard filesystem wildcards to a SQL "like" filter
		/// </summary>
		/// <param name="s">The string to convert.</param>
		/// <returns>The filter string.</returns>
		internal static string ToSqlLikeFilter(this string s) {
			if (!s.ContainsWildcard())
				return s.Trim();
			return s.Replace(SINGLE_CHAR_WILDCARD, TSQL_SINGLE_CHAR_WILDCARD).Replace(MULTI_CHAR_WILDCARD, TSQL_MULTI_CHAR_WILDCARD).Trim();
		}

		/// <summary>
		/// Determine if the string contains any standard filesystem wildcards.
		/// </summary>
		/// <param name="s">The string to check</param>
		/// <returns>Boolean indicating whether or not the string contains wildcards</returns>
		internal static bool ContainsWildcard(this string s) {
			if (string.IsNullOrWhiteSpace(s))
				return false;
			return s.Contains(MULTI_CHAR_WILDCARD.ToString()) || s.Contains(SINGLE_CHAR_WILDCARD.ToString());
		}

		/// <summary>
		/// Split the specified string on wildcard characters.
		/// </summary>
		/// <param name="s">The string to split</param>
		/// <returns>An array of strings.</returns>
		internal static string[] SplitOnWildcards(this string s) =>
			s.Split(new char[] { MULTI_CHAR_WILDCARD, SINGLE_CHAR_WILDCARD }, StringSplitOptions.RemoveEmptyEntries);

		/// <summary>
		/// Determines if a string matches a wildcard pattern.
		/// </summary>
		/// <param name="text">The text to compare</param>
		/// <param name="wildcardString">String containing the comparison/matching pattern containing wildcards (* or ?)</param>
		/// <param name="ignoreCase">Boolean indicating whether or not the comparison is case-insensitive (default = ignore character casing)</param>
		/// <returns>A Boolean indicating whether or not the text matches the comparison string.</returns>
		internal static bool EqualsWildcard(this string text, string wildcardString, bool ignoreCase = true) {
			// Return false if either of the strings are null or empty.
			if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(wildcardString))
				return false;

			if (ignoreCase) {
				text = text.ToLower();
				wildcardString = wildcardString.ToLower();
			}
			bool isLike = true;
			byte matchCase = 0;
			char[] filter;
			char[] reversedFilter;
			char[] reversedWord;
			char[] word;
			int currentPatternStartIndex = 0;
			int lastCheckedHeadIndex = 0;
			int lastCheckedTailIndex = 0;
			int reversedWordIndex = 0;
			List<char[]> reversedPatterns = new List<char[]>();

			word = text.ToCharArray();
			filter = wildcardString.ToCharArray();

			// Set which case will be used (0 = no wildcards, 1 = only ?, 2 = only *, 3 = both ? and *
			for (int i = 0; i < filter.Length; i++) {
				if (filter[i] == SINGLE_CHAR_WILDCARD) {
					matchCase += 1;
					break;
				}
			}
			for (int i = 0; i < filter.Length; i++) {
				if (filter[i] == MULTI_CHAR_WILDCARD) {
					matchCase += 2;
					break;
				}
			}

			if ((matchCase == 0 || matchCase == 1) && word.Length != filter.Length)
				return false;

			switch (matchCase) {
				case 0:
					isLike = (text == wildcardString);
					break;
				case 1: {
						for (int i = 0; i < text.Length; i++) {
							if ((word[i] != filter[i]) && filter[i] != SINGLE_CHAR_WILDCARD)
								isLike = false;
						}
						break;
					}
				case 2: {
						//Search for matches until the first *
						for (int i = 0; i < filter.Length; i++) {
							if (filter[i] != MULTI_CHAR_WILDCARD) {
								if (filter[i] != word[i])
									return false;
							} else {
								lastCheckedHeadIndex = i;
								break;
							}
						}
						// Search Tail for matches until first *
						for (int i = 0; i < filter.Length; i++) {
							if (filter[filter.Length - 1 - i] != MULTI_CHAR_WILDCARD) {
								if (filter[filter.Length - 1 - i] != word[word.Length - 1 - i])
									return false;
							} else {
								lastCheckedTailIndex = i;
								break;
							}
						}

						//Create a reverse word and filter for searching in reverse. The reversed word and filter do not include already checks chars
						reversedWord = new char[word.Length - lastCheckedHeadIndex - lastCheckedTailIndex];
						reversedFilter = new char[filter.Length - lastCheckedHeadIndex - lastCheckedTailIndex];
						for (int i = 0; i < reversedWord.Length; i++)
							reversedWord[i] = word[word.Length - (i + 1) - lastCheckedTailIndex];
						for (int i = 0; i < reversedFilter.Length; i++)
							reversedFilter[i] = filter[filter.Length - (i + 1) - lastCheckedTailIndex];

						//Cut up the filter into separate patterns, exclude * as they are no longer needed.
						for (int i = 0; i < reversedFilter.Length; i++) {
							if (reversedFilter[i] == MULTI_CHAR_WILDCARD) {
								if (i - currentPatternStartIndex > 0) {
									char[] pattern = new char[i - currentPatternStartIndex];
									for (int j = 0; j < pattern.Length; j++)
										pattern[j] = reversedFilter[currentPatternStartIndex + j];
									reversedPatterns.Add(pattern);
								}
								currentPatternStartIndex = i + 1;
							}
						}

						//Search for the patterns
						for (int i = 0; i < reversedPatterns.Count; i++) {
							for (int j = 0; j < reversedPatterns[i].Length; j++) {
								if (reversedWordIndex > reversedWord.Length - 1)
									return false;

								if (reversedPatterns[i][j] != reversedWord[reversedWordIndex + j]) {
									reversedWordIndex += 1;
									j = -1;
								} else {
									if (j == reversedPatterns[i].Length - 1)
										reversedWordIndex = reversedWordIndex + reversedPatterns[i].Length;
								}
							}
						}
						break;
					}
				case 3: {
						// Same as Case2, except ? is considered a match
						// Search Head for matches until first *
						for (int i = 0; i < filter.Length; i++) {
							if (filter[i] != MULTI_CHAR_WILDCARD) {
								if (filter[i] != word[i] && filter[i] != SINGLE_CHAR_WILDCARD)
									return false;
								else {
									lastCheckedHeadIndex = i;
									break;
								}
							}
						}
						//Search Tail for matches until first *
						for (int i = 0; i < filter.Length; i++) {
							if (filter[filter.Length - 1 - i] != MULTI_CHAR_WILDCARD) {
								if (filter[filter.Length - 1 - i] != word[word.Length - 1 - i] && filter[filter.Length - 1 - i] != SINGLE_CHAR_WILDCARD)
									return false;
							} else {
								lastCheckedTailIndex = i;
								break;
							}
						}
						// Reverse and trim word and filter.
						reversedWord = new char[word.Length - lastCheckedHeadIndex - lastCheckedTailIndex];
						reversedFilter = new char[filter.Length - lastCheckedHeadIndex - lastCheckedTailIndex];
						for (int i = 0; i < reversedWord.Length; i++)
							reversedWord[i] = word[word.Length - (i + 1) - lastCheckedTailIndex];
						for (int i = 0; i < reversedFilter.Length; i++)
							reversedFilter[i] = filter[filter.Length - (i + 1) - lastCheckedTailIndex];

						for (int i = 0; i < reversedFilter.Length; i++) {
							if (reversedFilter[i] == MULTI_CHAR_WILDCARD) {
								if (i - currentPatternStartIndex > 0) {
									char[] patter = new char[i - currentPatternStartIndex];
									for (int j = 0; j < patter.Length; j++)
										patter[j] = reversedFilter[currentPatternStartIndex + j];
									reversedPatterns.Add(patter);
								}
							}
						}
						// Search for the patterns
						for (int i = 0; i < reversedPatterns.Count; i++) {
							for (int j = 0; j < reversedPatterns[i].Length; j++) {
								if (reversedWordIndex > reversedWord.Length - 1)
									return false;
								if (reversedPatterns[i][j] != SINGLE_CHAR_WILDCARD && reversedPatterns[i][j] != reversedWord[reversedWordIndex + j]) {
									reversedWordIndex += 1;
									j = -1;
								} else {
									if (j == reversedPatterns[i].Length - 1)
										reversedWordIndex = reversedWordIndex + reversedPatterns[i].Length;
								}
							}
						}
						break;
					}
			}
			return isLike;
		}
	}
}
