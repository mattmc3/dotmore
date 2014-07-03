using System;
using System.Collections.Generic;

namespace mattmc3.dotmore.Collections.Generic {
	/// <summary>
	/// Provides functionality for creation of an EqualityComparer with lambda
	/// Equals and GetHashCode functions.
	/// </summary>
	public class EqualityComparer2<T> : EqualityComparer<T> {
		private readonly Func<T, T, bool> _equalsFunction;
		private readonly Func<T, int> _hashFunction;

		#region Constructors

		public EqualityComparer2(Func<T, T, bool> equalsFunction)
			: this(equalsFunction, o => 0) {
		}

		public EqualityComparer2(Func<T, T, bool> equalsFunction, Func<T, int> hashFunction) {
			if (equalsFunction == null) throw new ArgumentNullException("equalsFunction");
			if (hashFunction == null) throw new ArgumentNullException("hashFunction");
			_equalsFunction = equalsFunction;
			_hashFunction = hashFunction;
		}

		#endregion

		public override bool Equals(T x, T y) {
			return _equalsFunction(x, y);
		}

		public override int GetHashCode(T obj) {
			return _hashFunction(obj);
		}

	}
}
