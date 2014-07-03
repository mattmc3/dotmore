using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace mattmc3.dotmore.Collections.Generic {
	/// <summary>
	/// An IList implementation that flexes IQueryable's delayed loading
	/// </summary>
	public class LazyList<T> : IList<T> {
		#region Fields/Properties
		private IQueryable<T> _query;

		protected IList<T> InnerList {
			get {
				if (_inner == null)
					_inner = _query.ToList();
				return _inner;
			}
		}
		private IList<T> _inner;

		public T this[int index] {
			get {
				return InnerList[index];
			}
			set {
				InnerList[index] = value;
			}
		}
		#endregion

		#region Constructors
		public LazyList() {
			_inner = new List<T>();
		}

		public LazyList(IQueryable<T> query) {
			this._query = query;
		}

		#endregion

		public int IndexOf(T item) {
			return InnerList.IndexOf(item);
		}

		public void Insert(int index, T item) {
			InnerList.Insert(index, item);
		}

		public void RemoveAt(int index) {
			InnerList.RemoveAt(index);
		}

		public void Add(T item) {
			_inner = _inner ?? new List<T>();
			InnerList.Add(item);
		}

		public void Add(object ob) {
			throw new NotImplementedException("This is for serialization");
		}

		public void Clear() {
			if (_inner != null)
				InnerList.Clear();
		}

		public bool Contains(T item) {
			return InnerList.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex) {
			InnerList.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item) {
			return InnerList.Remove(item);
		}

		public int Count {
			get {
				return InnerList.Count;
			}
		}

		public bool IsReadOnly {
			get {
				return InnerList.IsReadOnly;
			}
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return InnerList.GetEnumerator();
		}

		public IEnumerator GetEnumerator() {
			return ((IEnumerable)InnerList).GetEnumerator();
		}
	}
}
