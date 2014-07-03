using System;
using System.Collections.Generic;
using System.Linq;

namespace mattmc3.dotmore.Collections.Generic {
	public class PagedList<T> : List<T>, IPagedList {
		public int TotalCount { get; set; }
		public int TotalPages { get; set; }
		public int PageIndex { get; set; }
		public int PageSize { get; set; }

		public bool HasPreviousPage {
			get { return (this.PageIndex > 0); }
		}

		public bool HasNextPage {
			get {
				return (this.PageIndex * this.PageSize) <= TotalCount;
			}
		}

		public PagedList(IEnumerable<T> source, int index, int pageSize) {
			int total = source.Count();
			this.TotalCount = total;
			this.TotalPages = total / pageSize;

			if (total % pageSize > 0) TotalPages++;

			this.PageSize = pageSize;
			this.PageIndex = index;
			this.AddRange(source.Skip(index * pageSize).Take(pageSize).ToList());
		}
	}
}
