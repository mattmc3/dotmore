using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace mattmc3.dotmore.Text.RegularExpressions {
	public static class ReflectionExtensionMethods {
		/// <summary>
		/// Converts a string to regex options.  The string matches Perl's mappings.
		/// x = RegexOptions.IgnorePatternWhitespace,
		/// c = RegexOptions.Compiled,
		/// m = RegexOptions.Multiline,
		/// s = RegexOptions.Singleline,
		/// i = RegexOptions.IgnoreCase
		/// </summary>
		public static RegexOptions ToRegexOptions(this string reOpts) {
			var resultOpts = RegexOptions.None;
			foreach (char c in reOpts.ToLower().ToCharArray()) {
				switch (c) {
					case 'x': resultOpts |= RegexOptions.IgnorePatternWhitespace; break;
					case 'm': resultOpts |= RegexOptions.Multiline; break;
					case 's': resultOpts |= RegexOptions.Singleline; break;
					case 'i': resultOpts |= RegexOptions.IgnoreCase; break;
					case 'c': resultOpts |= RegexOptions.Compiled; break;
					default: break;
				}
			}
			return resultOpts;
		}

		/// <summary>
		/// Uses a regular expression to get a substring match without all the extras involved in
		/// assembling regex objects.
		/// </summary>
		/// <param name="searchString">The string to match with the regex.</param>
		/// <param name="matchGroupNumber">The 1-based index of the regex grouping (parentheses).</param>
		/// <returns>The string corresponding to the match group.  Null if there was no match.</returns>
		public static string GetGroupMatch(this Regex re, string searchString, int matchGroupNumber) {
			Match m = re.Match(searchString);
			if (!m.Success) return null;
			return m.Groups[matchGroupNumber].Value;
		}

		/// <summary>
		/// Uses a regular expression to get a substring match without all the extras involved in
		/// assembling regex objects.
		/// </summary>
		/// <param name="searchString">The string to match with the regex.</param>
		/// <param name="matchGroupName">The name of the regex group.</param>
		/// <returns>The string corresponding to the match group.  Null if there was no match.</returns>
		public static string GetGroupMatch(this Regex re, string searchString, string matchGroupName) {
			Match m = re.Match(searchString);
			if (!m.Success) return null;
			return m.Groups[matchGroupName].Value;
		}

		/// <summary>
		/// Uses a regular expression to replace a substring grouping match
		/// </summary>
		/// <param name="searchString">The string to match with the regex.</param>
		/// <param name="replacementString">The string to replace in the match group number specified.</param>
		/// <param name="matchGroupNumber">The 1-based index of the regex grouping (parentheses).</param>
		/// <returns>The replacement string.</returns>
		public static string ReplaceSubstringMatch(this Regex re, string searchString, string replacementString, int matchGroupNumber) {
			Match m = re.Match(searchString);
			if (!m.Success) return searchString;
			var grp = m.Groups[matchGroupNumber];
			return searchString.Substring(0, grp.Index) + replacementString + searchString.Substring(grp.Index + grp.Length);
		}
	}
}
