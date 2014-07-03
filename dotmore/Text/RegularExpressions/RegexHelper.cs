using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace mattmc3.dotmore.Text.RegularExpressions {

	public static class RegexHelper {

		#region fields/properties

		private static readonly HashSet<char> s_needsEscaped;
		private static readonly Dictionary<char, string> s_translate;

		/// <summary>
		/// Shortcut for IgnorePatternWhitespace | Multiline | Singleline.  Equivalent to Perl's "xms" regex options.
		/// These are the preferred options for all regex patterns.
		/// </summary>
		public static RegexOptions XmsOpts {
			get { return RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.Singleline; }
		}

		/// <summary>
		/// Shortcut for IgnorePatternWhitespace | Multiline | Singleline | IgnoreCase.  Equivalent to Perl's "xmsi" regex options.
		/// </summary>
		public static RegexOptions XmsiOpts {
			get { return XmsOpts | RegexOptions.IgnoreCase; }
		}

		#endregion

		#region constructors

		/// <summary>
		/// Static initializer
		/// </summary>
		static RegexHelper() {
			s_needsEscaped = new HashSet<char>();
			s_needsEscaped.Add(' ');
			s_needsEscaped.Add('{');
			s_needsEscaped.Add('}');
			s_needsEscaped.Add('[');
			s_needsEscaped.Add(']');
			s_needsEscaped.Add('(');
			s_needsEscaped.Add(')');
			s_needsEscaped.Add('<');
			s_needsEscaped.Add('>');
			s_needsEscaped.Add('.');
			s_needsEscaped.Add('/');
			s_needsEscaped.Add('\\');
			s_needsEscaped.Add('^');
			s_needsEscaped.Add('$');
			s_needsEscaped.Add('?');
			s_needsEscaped.Add('+');
			s_needsEscaped.Add('*');
			s_needsEscaped.Add('#');
			s_needsEscaped.Add('|');

			s_translate = new Dictionary<char, string>();
			s_translate.Add('\t', "\\t");
			s_translate.Add('\r', "\\r");
			s_translate.Add('\n', "\\n");
			s_translate.Add('\v', "\\v");
		}

		#endregion

		#region methods

		/// <summary>
		/// Checks to see if a regex pattern string is valid.  Uses XMS regex options.
		/// </summary>
		/// <param name="pattern">The regex pattern to test for validity.</param>
		/// <returns>True if the pattern is a valid regex.  False otherwise.</returns>
		public static bool IsValidRegexPattern(string pattern) {
			return IsValidRegexPattern(pattern, XmsOpts);
		}

		/// <summary>
		/// Checks to see if a regex pattern string is valid.
		/// </summary>
		/// <param name="pattern">The regex pattern to test for validity.</param>
		/// <param name="opts">The regex options to use when testing the pattern.</param>
		/// <returns>True if the pattern is a valid regex.  False otherwise.</returns>
		public static bool IsValidRegexPattern(string pattern, RegexOptions opts) {
			// Yuck... Microsoft should really have provided this method.  Now, I have to trap for
			// an error which is a performance nightmare
			bool result = true;
			try {
				new Regex(pattern, opts);
			}
			catch {
				result = false;
			}
			return result;
		}

		/// <summary>
		/// Takes a wildcard pattern (like *.bat) and converts it to the equivalent RegEx pattern
		/// </summary>
		/// <param name="wildcardPattern">The wildcard pattern to convert.  Syntax similar to VB's Like operator with the addition of pipe ("|") delimited patterns.</param>
		/// <returns>A regex pattern that is equivalent to the wildcard pattern supplied</returns>
		public static string ConvertWildcardPatternToRegex(string wildcardPattern) {
			if (string.IsNullOrEmpty(wildcardPattern)) return "";

			// Split on pipe
			string[] patternParts = wildcardPattern.Split('|');

			// Turn into regex pattern that will match the whole string with ^$
			StringBuilder patternBuilder = new StringBuilder();
			bool firstPass = true;
			patternBuilder.Append("^");
			foreach (string part in patternParts) {
				string rePattern = Regex.Escape(part);

				// add support for ?, #, *, [...], and [!...]
				rePattern = rePattern.Replace("\\[!", "[^");
				rePattern = rePattern.Replace("\\[", "[");
				rePattern = rePattern.Replace("\\]", "]");
				rePattern = rePattern.Replace("\\?", ".");
				rePattern = rePattern.Replace("\\*", ".*");
				rePattern = rePattern.Replace("\\#", "\\d");

				if (firstPass) {
					firstPass = false;
				}
				else {
					patternBuilder.Append("|");
				}
				patternBuilder.Append("(");
				patternBuilder.Append(rePattern);
				patternBuilder.Append(")");
			}
			patternBuilder.Append("$");

			string result = patternBuilder.ToString();
			if (!IsValidRegexPattern(result)) {
				throw new ArgumentException(string.Format("Invalid pattern: {0}", wildcardPattern));
			}
			return result;
		}

		/// <summary>
		/// Takes a SQL 'LIKE' pattern (ie: 'abc%') and converts it to the equivalent RegEx pattern
		/// </summary>
		/// <param name="sqlLikePattern">The SQL LIKE pattern to convert.</param>
		/// <returns>A regex pattern that is equivalent to the SQL 'LIKE' pattern supplied</returns>
		public static string ConvertSqlLikePatternToRegex(string sqlLikePattern) {
			// turn into regex pattern that will match the whole string with ^$
			string rePattern = "^" + Regex.Escape(sqlLikePattern) + "$";

			// add support for %, _, [_], [[], [...], and [^...]
			rePattern = rePattern.Replace("\\[_\\]", "[_]");
			rePattern = rePattern.Replace("\\[\\[\\]", "\\[");
			rePattern = rePattern.Replace("\\[\\^", "[^");
			rePattern = rePattern.Replace("\\[", "[");
			rePattern = rePattern.Replace("\\]", "]");
			rePattern = rePattern.Replace("%", ".*?");
			rePattern = rePattern.Replace("_", ".");

			if (!IsValidRegexPattern(rePattern)) {
				throw new ArgumentException(string.Format("Invalid pattern: {0}", sqlLikePattern));
			}
			return rePattern;
		}

		#endregion

	}

}
