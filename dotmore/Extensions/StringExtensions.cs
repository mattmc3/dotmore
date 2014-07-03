using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using mattmc3.dotmore.Text.RegularExpressions;
using System.Globalization;

namespace mattmc3.dotmore.Extensions {
	public static class StringExtensions {
		/// <summary>
		/// Works like JavaScript's slice() method, where you specify a starting index and an ending index
		/// and the substring will be returned.  Note that you can use negative positioning to indicate indexing
		/// backwards from the end of the string.
		/// </summary>
		public static string Slice(this string s, int startIndex, int? endIndex = null) {
			if (s == null) return null;
			int startPosition = startIndex;
			int endPosition = (endIndex.HasValue ? endIndex.Value : s.Length);

			// Convert reverse indexes references from the right of the string (negative) to
			// reference from the left
			if (startPosition < 0) startPosition = s.Length + startIndex;
			if (endPosition < 0) endPosition = s.Length + endPosition;

			// Smooth the positions
			if (startPosition < 0) startPosition = 0;
			if (startPosition > s.Length) startPosition = s.Length;
			if (endPosition < 0) endPosition = 0;
			if (endPosition > s.Length) endPosition = s.Length;

			int length = endPosition - startPosition;
			if (length < 0) length = 0;
			if ((startPosition + length) > s.Length) {
				return s.Substring(startPosition);
			}
			else {
				return s.Substring(startPosition, length);
			}
		}

		public static string SubstringBefore(this string s, string innerString) {
			if (s == null) return null;
			var index = s.IndexOf(innerString);
			return (index < 0 ? null : s.Substring(0, index));
		}

		public static string SubstringAfter(this string s, string innerString) {
			if (s == null) return null;
			var index = s.IndexOf(innerString);
			return (index < 0 ? null : s.Substring(index + 1));
		}

		/// <summary>
		/// Repeats the string a set number of times.
		/// </summary>
		/// <param name="value">Any string</param>
		/// <param name="repetitions">The number of times to repeat the string</param>
		/// <returns>A repeated version of the specified string</returns>
		public static string Repeat(this string s, int repetitions) {
			if (s == null) return null;
			var buf = new StringBuilder();
			for (int i = 0; i < repetitions; i++) {
				buf.Append(s);
			}
			return buf.ToString();
		}

		/// <summary>
		/// Provides a more natural way to call String.Format() on a string.
		/// </summary>
		/// <param name="args">An object array that contains zero or more objects to format</param>
		public static string FormatWith(this string s, params object[] args) {
			if (s == null) return null;
			return String.Format(s, args);
		}

		/// <summary>
		/// Provides a more natural way to call String.Format() on a string.
		/// </summary>
		/// <param name="provider">An object that supplies the culture specific formatting</param>
		/// <param name="args">An object array that contains zero or more objects to format</param>
		public static string FormatWith(this string s, IFormatProvider provider, params object[] args) {
			if (s == null) return null;
			return String.Format(provider, s, args);
		}

		/// <summary>
		/// Takes a string and escapes any special characters so that the result
		/// is in the format that you'd store in a C# string.
		/// </summary>
		/// <param name="value">Any string</param>
		/// <returns>The string with any special characters replaced.</returns>
		public static string ToCSharpEscapedString(this string s) {
			string result = s;
			result = result.Replace("\\", @"\\");
			result = result.Replace("\"", "\\\"");
			result = result.Replace("\0", @"\0");
			result = result.Replace("\a", @"\a");
			result = result.Replace("\b", @"\b");
			result = result.Replace("\f", @"\f");
			result = result.Replace("\n", @"\n");
			result = result.Replace("\r", @"\r");
			result = result.Replace("\t", @"\t");
			result = result.Replace("\v", @"\v");
			return result;
		}

		/// <summary>
		/// Reverse a string
		/// </summary>
		/// <param name="input">The string to reverse</param>
		/// <returns>The reversed string</returns>
		public static string Reverse(this string input) {
			char[] array = input.ToCharArray();
			Array.Reverse(array);
			return new string(array);
		}

		public static bool IsNullOrWhitespace(this string input) {
			return String.IsNullOrWhiteSpace(input);
		}

		public static string[] SplitLines(this string that) {
			return that.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
		}

		/// <summary>
		/// The translate function returns the first argument string with occurrences of characters
		/// in the second argument string replaced by the character at the corresponding position in
		/// the third argument string. For example, translate("bar","abc","ABC") returns the string
		/// "BAr". If there is a character in the second argument string with no character at a
		/// corresponding position in the third argument string (because the second argument string
		/// is longer than the third argument string), then occurrences of that character in the first
		/// argument string are removed. For example, Translate("--aaa--","abc-","ABC") returns "AAA".
		/// If a character occurs more than once in the second argument string, then the first
		/// occurrence determines the replacement character. If the third argument string is longer
		/// than the second argument string, then excess characters are ignored.
		/// </summary>
		/// <param name="translationFromString">The case-sensitive translation string defining the characters to convert from.</param>
		/// <param name="translationToString">The case-sensitive translation string defining the characters to convert to.</param>
		/// <returns>The result of the string translation.</returns>
		/// <remarks>This is modeled after the XSLT translate() function.</remarks>
		public static string Translate(this string str, string translationFromString, string translationToString) {
			if (string.IsNullOrEmpty(str)) return str;
			if (translationFromString == null) throw new ArgumentNullException("translationFromString");
			if (translationToString == null) throw new ArgumentNullException("translationToString");

			// Perform the translation
			StringBuilder rtn = new StringBuilder();
			foreach (char ch in str) {
				int idx = translationFromString.IndexOf(ch);
				if (idx < 0) {
					rtn.Append(ch); // Unchanged
				}
				else if (idx < translationToString.Length) {
					rtn.Append(translationToString[idx]); // Translate
				}
				// If there was no translation, the char is skipped
			}
			return rtn.ToString();
		}

		/// <summary>
		/// Gets an array containing all the indexes of the specified searchString inside the
		/// baseString.  This method builds off of String.IndexOf().  An empty array is returned
		/// if nothing is found.
		/// </summary>
		/// <param name="baseString">Any string</param>
		/// <param name="searchString">The substring to search for.</param>
		/// <returns>
		/// An array containing the zero based indexes of the search string within the base string.
		/// An empty array is returned if nothing was found.
		/// </returns>
		public static int[] AllIndexesOf(this string baseString, string searchString) {
			var indexes = new List<int>();
			int startIndex = 0;
			do {
				int index = baseString.IndexOf(searchString, startIndex);
				if (index < 0) break;
				indexes.Add(index);
				startIndex = index + 1;
			} while (true);
			return indexes.ToArray();
		}

		/// <summary>
		/// Gets the index of the starting position of any of the inner strings specified.
		/// </summary>
		public static int IndexOfAny(this string baseString, string[] anyOf) {
			var idx = -1;
			foreach (string item in anyOf) {
				idx = baseString.IndexOf(item);
				if (idx >= 0) return idx;
			}
			return idx;
		}

		/// <summary>
		/// Addition to System.String.Replace() that accounts for case-insensitivity and other string
		/// comparison options.
		/// </summary>
		/// <param name="s">The original string</param>
		/// <param name="oldValue">The value to replace</param>
		/// <param name="newValue">The desired replacement</param>
		/// <param name="comparisonType">The comparison algorithm to use</param>
		/// <returns>The original string with the replacements made</returns>
		public static string Replace(this string s, string oldValue, string newValue, StringComparison comparisonType) {
			if (s == null) return null;
			if (string.IsNullOrEmpty(oldValue) || newValue == null) return s;
			int idxNext = s.IndexOf(oldValue, comparisonType);
			if (idxNext < 0) return s;

			var result = new StringBuilder();
			int lenOldValue = oldValue.Length;
			int curPosition = 0;

			while (idxNext >= 0) {
				result.Append(s, curPosition, idxNext - curPosition);
				result.Append(newValue);
				curPosition = idxNext + lenOldValue;
				idxNext = s.IndexOf(oldValue, curPosition, comparisonType);
			}

			result.Append(s, curPosition, s.Length - curPosition);
			return result.ToString();
		}

		public static string Left(this string s, int length) {
			if (s == null) return null;
			if (length > s.Length) {
				length = s.Length;
			}
			else if (length < 0) {
				length = 0;
			}
			return s.Substring(0, length);
		}

		public static string Right(this string s, int length) {
			if (s == null) return null;
			if (length > s.Length) {
				length = s.Length;
			}
			else if (length < 0) {
				length = 0;
			}
			return s.Substring(s.Length - length, length);
		}

		public static string Substring(this string s, int startIndex, int length, bool neverFail) {
			if (neverFail == false) return s.Substring(startIndex, length);
			startIndex = startIndex.ConstrainBetween(0, s.Length);

			if (length < 0) length = 0;
			if ((startIndex + length) > s.Length) {
				length = s.Length - startIndex;
			}

			return s.Substring(startIndex, length);
		}

		public static string Truncate(this string s, int maxLength, string truncationSuffix = "...") {
			if (s == null) return null;
			if (maxLength < 0) return "";
			if (s.Length <= maxLength) return s;
			if (maxLength < truncationSuffix.Length) {
				return s.Left(maxLength);
			}
			else {
				return s.Left(maxLength - truncationSuffix.Length) + truncationSuffix;
			}
		}

		public static bool Contains(this string s, string value, StringComparison comparisonType) {
			return (s.IndexOf(value, comparisonType) >= 0);
		}

		public static bool Contains(this string s, char[] values, StringComparison comparisonType) {
			return Contains(s, values.Select(c => new string(c, 1)).ToArray(), comparisonType);
		}

		public static bool Contains(this string s, string[] values) {
			return Contains(s, values, StringComparison.Ordinal);
		}

		public static bool Contains(this string s, string[] values, StringComparison comparisonType) {
			if (s == null) return false;
			var found = false;
			foreach (string value in values) {
				if (value != null) {
					found = (s.IndexOf(value, comparisonType) >= 0);
					if (found) return true;
				}
			}
			return false;
		}

		public static string NormalizeWhiteSpace(this string s) {
			if (s == null) return null;
			if (s_regexNormalizeSpace == null) {
				s_regexNormalizeSpace = new Regex(@"\s+", mattmc3.dotmore.Text.RegularExpressions.RegexHelper.XmsOpts | RegexOptions.Compiled);
			}
			return s_regexNormalizeSpace.Replace(s.Trim(), " ");
		}
		private static Regex s_regexNormalizeSpace;

		public static string NormalizeNewlines(this string s) {
			if (s == null) return null;
			return s.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", System.Environment.NewLine);
		}

		public static string TrimEnd(this string s) {
			if (s == null) return null;
			var buf = new StringBuilder(s);
			while (buf.Length > 0 && Char.IsWhiteSpace(buf[buf.Length - 1])) {
				buf.Remove(buf.Length - 1, 1);
			}
			return buf.ToString();
		}

		public static string TrimStart(this string s) {
			if (s == null) return null;
			var buf = new StringBuilder(s);
			while (buf.Length > 0 && Char.IsWhiteSpace(buf[0])) {
				buf.Remove(0, 1);
			}
			return buf.ToString();
		}

		public static bool IsWildcardMatch(this string s, string wildcardPattern) {
			// make this work like VB.NET's 'Like' keyword
			if (String.IsNullOrEmpty(wildcardPattern)) {
				return String.IsNullOrEmpty(s);
			}
			var rePattern = RegexHelper.ConvertWildcardPatternToRegex(wildcardPattern);
			return Regex.IsMatch(s, rePattern, RegexHelper.XmsiOpts);
		}

		public static string[] Chunk(this string s, int chunkSize) {
			var result = new List<string>();
			var offset = 0;
			while (s != null && offset < s.Length) {
				var size = (chunkSize < s.Length - offset ? chunkSize : s.Length - offset);
				result.Add(s.Substring(offset, size));
				offset += size;
			}
			return result.ToArray();
		}

		/// <summary>
		/// Takes a string and converts it to "Title Case"
		/// </summary>
		/// <param name="value">Any string</param>
		/// <returns>The converted "Title Case" version of the string parameter provided.</returns>
		public static string MakeTitleCase(this string value) {
			if (value == null) throw new ArgumentNullException();
			// Creates a TextInfo based on the "en-US" culture.
			TextInfo ti = new CultureInfo("en-US", false).TextInfo;
			string result = ti.ToTitleCase(value.ToLower());
			MatchEvaluator matchEval = delegate(Match m) {
				return m.Groups[1].Value + m.Groups[2].Value.ToLower();
			};
			result = Regex.Replace(result, "([0-9])([A-Z])", matchEval);
			return result;
		}
	}
}
