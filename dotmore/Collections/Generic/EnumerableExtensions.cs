using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace mattmc3.dotmore.Collections.Generic {

	public static class EnumerableExtensions {
		public static T Coalesce<T>(this IEnumerable<T> e) {
			if (e == null) return default(T);
			return e.Where(x => x != null).FirstOrDefault();
		}

		public static IEnumerable<T> Prepend<T>(this IEnumerable<T> e, T firstElement) {
			return (new T[] { firstElement }).Concat(e);
		}

		public static IEnumerable<T> Append<T>(this IEnumerable<T> e, T lastElement) {
			return e.Concat(new T[] { lastElement });
		}

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> e) {
			var list = e.ToList();
			var random = new Random();

			for (int i = list.Count - 1; i >= 0; i--) {
				int r = random.Next(i + 1);
				yield return list[r];
				list[r] = list[i];
			}
		}

		/// <summary>
		/// Cycles through each item in the collection the specified number of times.
		/// If no limit is specified, the cycle is infinite.
		/// </summary>
		public static IEnumerable<T> Cycle<T>(this IEnumerable<T> e, int? numberOfCycles = null) {
			if (e == null) yield break;

			var isEmpty = false;
			var count = 0;
			while (!isEmpty && (!numberOfCycles.HasValue || count < numberOfCycles)) {
				// if the enumerable is empty, this could blow up with an unintended
				// infinite loop.  Empty enumerables should not cycle.
				isEmpty = true;
				foreach (T item in e) {
					yield return item;
					isEmpty = false;
				}
				count += 1;
			} 
		}

		/// <summary>
		/// Groups items together in an enumerable while a condition is true
		/// </summary>
		/// <remarks>http://stackoverflow.com/questions/16306770/how-can-i-efficiently-do-master-detail-grouping-in-linq-from-a-flat-enumerable-w/16307599?noredirect=1#comment23349239_16307599</remarks>
		public static IEnumerable<IEnumerable<T>> GroupWhile<T>(this IEnumerable<T> source, Func<T, bool> predicate) {
			using (var iterator = source.GetEnumerator()) {
				if (!iterator.MoveNext()) {
					yield break;
				}

				List<T> list = new List<T>() { iterator.Current };

				while (iterator.MoveNext()) {
					if (predicate(iterator.Current)) {
						list.Add(iterator.Current);
					}
					else {
						yield return list;
						list = new List<T>() { iterator.Current };
					}
				}
				yield return list;
			}
		} 

		public static PagedList<T> ToPagedList<T>(this IEnumerable<T> source, int index, int pageSize = 10) {
			return new PagedList<T>(source, index, pageSize);
		}

		public static OrderedDictionary<TKey, TSource> ToOrderedDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) {
			Func<TSource, TSource> elementSelector = (x) => x;
			return GetOrderedDictionaryImpl(source, keySelector, elementSelector, null);
		}

		public static OrderedDictionary<TKey, TElement> ToOrderedDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) {
			return GetOrderedDictionaryImpl(source, keySelector, elementSelector, null);
		}

		public static OrderedDictionary<TKey, TSource> ToOrderedDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) {
			Func<TSource, TSource> elementSelector = (x) => x;
			return GetOrderedDictionaryImpl(source, keySelector, elementSelector, comparer);
		}

		public static OrderedDictionary<TKey, TElement> ToOrderedDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer) {
			return GetOrderedDictionaryImpl(source, keySelector, elementSelector, comparer);
		}

		public static string ToDelimitedString(this IEnumerable source, string delimiter = ",", string quote = "\"", Func<object, string> convertToString = null) {
			if (convertToString == null) {
				convertToString = x => (x == null ? "" : x.ToString());
			}

			var data = (
				from object a in source
				let s = convertToString(a)
				select s.Contains(delimiter) || s.Contains("\r") || s.Contains("\n") || s.Contains(quote) ? String.Format("{0}{1}{0}", quote, s) : s
			);
			return String.Join(",", data);
		}

		private static OrderedDictionary<TKey, TElement> GetOrderedDictionaryImpl<TSource, TKey, TElement>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer) {
			OrderedDictionary<TKey, TElement> result = null;

			if (comparer == null) {
				result = new OrderedDictionary<TKey, TElement>();
			}
			else {
				result = new OrderedDictionary<TKey, TElement>(comparer);
			}

			foreach (TSource sourceItem in source) {
				result.Add(keySelector(sourceItem), elementSelector(sourceItem));
			}

			return result;
		}

	}

}
