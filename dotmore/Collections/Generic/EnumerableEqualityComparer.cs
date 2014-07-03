using System;
using System.Collections.Generic;

namespace mattmc3.dotmore.Collections.Generic {
	/// <summary>
	/// Comparison helper class for IEnumerable&lt;T&gt;
	/// </summary>
	/// <remarks>
	/// Handy for composite key indexing for IDictionary objects 
	/// </remarks>
	public class EnumerableEqualityComparer<T> : System.Collections.Generic.EqualityComparer<IEnumerable<T>> {
		private IEqualityComparer<T> _cmp;
		public EnumerableEqualityComparer() {
		}

		public EnumerableEqualityComparer(IEqualityComparer<T> cmp) {
			_cmp = cmp;
		}

		public override bool Equals(IEnumerable<T> x, IEnumerable<T> y) {
			IEnumerator<T> xenum = x.GetEnumerator();
			IEnumerator<T> yenum = y.GetEnumerator();

			while (xenum.MoveNext()) {
				if (!yenum.MoveNext()) return false;
				if (_cmp != null) {
					if (!_cmp.Equals(xenum.Current, yenum.Current)) return false;
				}
				else {
					if (!xenum.Current.Equals(yenum.Current)) return false;
				}
			}

			return !yenum.MoveNext();
		}

		public override int GetHashCode(IEnumerable<T> obj) {
			int hash = 0;
			IEnumerator<T> enr = obj.GetEnumerator();
			while (enr.MoveNext()) {
				if (_cmp != null) {
					var addtlHash = (enr.Current != null ? _cmp.GetHashCode(enr.Current) : 0);
					hash = hash ^ addtlHash;
				}
				else {
					hash = hash ^ enr.Current.GetHashCode();
				}
			}
			return hash;
		}
	}

}
