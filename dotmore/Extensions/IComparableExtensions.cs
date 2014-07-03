using System;

namespace mattmc3.dotmore.Extensions {
	public static class IComparableExtensions {
		public static bool IsBetween<T>(this T cmp, T lower, T upper, bool inclusiveOfUpperValue = true) where T : IComparable<T> {
			if (inclusiveOfUpperValue) {
				return cmp.CompareTo(lower) >= 0 && cmp.CompareTo(upper) <= 0;
			}
			else {
				return cmp.CompareTo(lower) >= 0 && cmp.CompareTo(upper) < 0;
			}
		}

		/// <summary>
		/// If the value is between the minimum and the maximum values provided, then the original value is returned.
		/// Otherwise, if it is less than the minimum, the minimum is returned.  Likewise, if it is greater than the
		/// maximum, the maximum is returned.
		/// </summary>
		public static T ConstrainBetween<T>(this T cmp, T minValue, T maxValue) where T : IComparable<T> {
			if (cmp.CompareTo(minValue) < 0) {
				return minValue;
			}
			else if (cmp.CompareTo(maxValue) > 0) {
				return maxValue;
			}
			else {
				return cmp;
			}
		}
	}
}
