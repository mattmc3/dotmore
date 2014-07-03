using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Collections.Specialized;

namespace mattmc3.dotmore.Diagnostics {
	[DebuggerDisplay("{Value}", Name = "[{Index}]: {Key}")]
	internal class IndexedKeyValuePairs {
		public IDictionary Dictionary { get; private set; }
		public int Index { get; private set; }
		public object Key { get; private set; }
		public object Value { get; private set; }
		
		public IndexedKeyValuePairs(IDictionary dictionary, int index, object key, object value) {
			Index = index;
			Value = value;
			Key = key;
			Dictionary = dictionary;
		}
	}

	internal class OrderedDictionaryDebugView {

		private IOrderedDictionary _dict;
		public OrderedDictionaryDebugView(IOrderedDictionary dict) {
			_dict = dict;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
		public IndexedKeyValuePairs[] IndexedKeyValuePairs {
			get {
				IndexedKeyValuePairs[] nkeys = new IndexedKeyValuePairs[_dict.Count];

				int i = 0;
				foreach (object key in _dict.Keys) {
					nkeys[i] = new IndexedKeyValuePairs(_dict, i, key, _dict[key]);
					i += 1;
				}
				return nkeys;
			}
		}
	}
}
