using System;
using System.Linq;

namespace mattmc3.dotmore.Extensions {
	public static class DateTimeExtensions {
		/// <summary>
		/// Returns true if this is the last day of the month
		/// </summary>
		public static DateTime ToLocalTime(this DateTime utcTime, TimeZoneInfo timeZone) {
			if (utcTime.Kind == DateTimeKind.Local) {
				throw new ArgumentException("The date time specified must have a DateTimeKind of UTC, not local");
			}
			if (timeZone == null) {
				throw new ArgumentNullException("timeZone");
			}
			var curLocalTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZone);
			return curLocalTime;
		}

		/// <summary>
		/// Returns true if this is the last day of the month
		/// </summary>
		public static bool IsLastDayOfMonth(this DateTime dt) {
			return (dt.AddDays(1).Month != dt.Month);
		}

		/// <summary>
		/// Returns the next occurrence of the day of the week specified.
		/// </summary>
		public static DateTime GetNext(this DateTime dt, DayOfWeek dayOfWeek) {
			int daysToAdd = 0;
			int dow = (int)dayOfWeek;
			if (dt.DayOfWeek < dayOfWeek) {
				daysToAdd = dayOfWeek - dt.DayOfWeek;
			}
			else {
				daysToAdd = (7 - dow) + dow;
			}
			return dt.AddDays(daysToAdd);
		}

		/// <summary>
		/// Returns the prior occurrence of the day of the week specified.
		/// </summary>
		public static DateTime GetLast(this DateTime dt, DayOfWeek dayOfWeek) {
			int daysToSubtract = 0;
			int dow = (int)dayOfWeek;
			if (dt.DayOfWeek > dayOfWeek) {
				daysToSubtract = dt.DayOfWeek - dayOfWeek;
			}
			else {
				daysToSubtract = (7 - dow) + dow;
			}
			return dt.AddDays(daysToSubtract * -1);
		}

		/// <summary>
		/// Gets the Unix representation of time from the 1/1/1970 epoch.
		/// </summary>
		public static double ToUnixTime(this DateTime dt) {
			var epoch = new DateTime(1970, 1, 1);
			return (dt - epoch).TotalMilliseconds;
		}

		public static DateTime ConvertFromUnixTimestamp(this double timestamp) {
			DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return origin.AddSeconds(timestamp);
		}

		/// <summary>
		/// Returns true is the date was a weekday.  False if weekend.
		/// </summary>
		public static bool IsWeekday(this DateTime dt) {
			return (dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday);
		}

		/// <summary>
		/// Returns true is the date was a weekend.  False if weekday.
		/// </summary>
		public static bool IsWeekend(this DateTime dt) {
			return (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday);
		}

		/// <summary>
		/// Returns empty string if date is null
		/// </summary>
		public static string ToShortDateString(this DateTime? dateTime)     {
				return dateTime.ToShortDateString("");
		}

		/// <summary>
		/// Returns empty string if date is null
		/// </summary>
		public static string ToShortDateString(this DateTime? dateTime, string returnIfNull){
			if (dateTime.HasValue) 
				return dateTime.Value.ToShortDateString();
			else  
				return returnIfNull;
		}

	}
}
