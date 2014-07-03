using System;
using System.Collections;
using System.Collections.Generic;
using mattmc3.dotmore.Diagnostics;

namespace mattmc3.dotmore.Collections.Generic {

	/// <summary>
	/// This dictionary allows you to add items to it without checking whether the key already
	/// exists.  If it does, the value is just overwritten.  Retrieval of items that don't exist
	/// return Nothing instead of throwing an exception.  Concept and name (but no code) comes
	/// from Rhino ETL (http://www.codeproject.com/KB/cs/ETLWithCSharp.aspx).
	/// </summary>
	public class QuackingDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary {

		#region Fields/Properties

		private IDictionary<TKey, TValue> _dict;

		public int Count {
			get { return _dict.Count; }
		}

		public TValue this[TKey key] {
			get {
				TValue result;
				if (_dict.TryGetValue(key, out result)) {
					return result;
				}
				else {
					return default(TValue);
				}
			}
			set {
				if (_dict.ContainsKey(key)) {
					_dict[key] = value;
				}
				else {
					_dict.Add(key, value);
				}
			}
		}

		public ICollection<TKey> Keys {
			get { return _dict.Keys; }
		}

		public ICollection<TValue> Values {
			get { return _dict.Values; }
		}

		#endregion

		#region Constructors

		public QuackingDictionary() {
			_dict = new Dictionary<TKey, TValue>();
		}

		public QuackingDictionary(IEqualityComparer<TKey> comparer) {
			_dict = new Dictionary<TKey, TValue>(comparer);
		}

		public QuackingDictionary(IDictionary<TKey, TValue> storageDictionary) {
			if (storageDictionary == null) {
				throw new ArgumentNullException("storageDictionary", "The dictionary to use as storage cannot be null.");
			}
			_dict = storageDictionary;
		}

		#endregion

		#region Methods

		public void Clear() {
			_dict.Clear();
		}

		public bool Contains(TKey key, TValue value) {
			return _dict.Contains(new KeyValuePair<TKey, TValue>(key, value));
		}

		public bool ContainsKey(TKey key) {
			return _dict.ContainsKey(key);
		}

		public bool Remove(TKey key) {
			return _dict.Remove(key);
		}

		public bool TryGetValue(TKey key, out TValue value) {
			return _dict.TryGetValue(key, out value);
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			return _dict.GetEnumerator();
		}

		#endregion

		#region IDictionary<TKey, TValue> interface implementation

		void IDictionary<TKey, TValue>.Add(TKey key, TValue value) {
			this[key] = value;
		}

		bool IDictionary<TKey, TValue>.ContainsKey(TKey key) {
			return ContainsKey(key);
		}

		ICollection<TKey> IDictionary<TKey, TValue>.Keys {
			get {
				return this.Keys;
			}
		}

		bool IDictionary<TKey, TValue>.Remove(TKey key) {
			return Remove(key);
		}

		bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value) {
			return TryGetValue(key, out value);
		}

		ICollection<TValue> IDictionary<TKey, TValue>.Values {
			get {
				return this.Values;
			}
		}

		TValue IDictionary<TKey, TValue>.this[TKey key] {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) {
			this[item.Key] = item.Value;
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Clear() {
			Clear();
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) {
			return Contains(item.Key, item.Value);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
			_dict.CopyTo(array, arrayIndex);
		}

		int ICollection<KeyValuePair<TKey, TValue>>.Count {
			get {
				return this.Count;
			}
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly {
			get { throw new NotImplementedException(); }
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) {
			return _dict.Remove(item);
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		#endregion

		#region IDictionary implementation
		
		void IDictionary.Add(object key, object value) {
			var d = GetIDictionary();
			if (d.Contains(key)) {
				d[key] = value;
			}
			else {
				d.Add(key, value);
			}
		}

		void IDictionary.Clear() {
			Clear();
		}

		bool IDictionary.Contains(object key) {
			return GetIDictionary().Contains(key);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator() {
			return GetIDictionary().GetEnumerator();
		}

		bool IDictionary.IsFixedSize {
			get {
				return GetIDictionary().IsFixedSize;
			}
		}

		bool IDictionary.IsReadOnly {
			get {
				return GetIDictionary().IsReadOnly;
			}
		}

		ICollection IDictionary.Keys {
			get {
				return GetIDictionary().Keys;
			}
		}

		void IDictionary.Remove(object key) {
			if (_dict.ContainsKey((TKey)key)) {
				_dict.Remove((TKey)key);
			}
		}

		ICollection IDictionary.Values {
			get {
				return GetIDictionary().Values;
			}
		}

		object IDictionary.this[object key] {
			get {
				var d = GetIDictionary();
				if (d.Contains(key)) {
					return d[key];
				}
				else {
					return null;
				}
			}
			set {
				var d = GetIDictionary();
				if (d.Contains(key)) {
					d[key] = value;
				}
				else {
					d.Add(key, value);
				}
			}
		}

		void ICollection.CopyTo(Array array, int index) {
			GetIDictionary().CopyTo(array, index);
		}

		int ICollection.Count {
			get {
				return this.Count;
			}
		}

		bool ICollection.IsSynchronized {
			get {
				return GetIDictionary().IsSynchronized;
			}
		}

		object ICollection.SyncRoot {
			get {
				return GetIDictionary().SyncRoot;
			}
		}

		private IDictionary GetIDictionary() {
			return (IDictionary)_dict;
		}

		#endregion
	}
}
