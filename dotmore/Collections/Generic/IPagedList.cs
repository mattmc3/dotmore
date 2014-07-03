using System;
using System.Linq;

namespace mattmc3.dotmore.Collections.Generic {
	interface IPagedList {
		int TotalCount { get; set; }
		int TotalPages { get; set; }
		int PageIndex { get; set; }
		int PageSize { get; set; }
		bool HasPreviousPage { get; }
		bool HasNextPage { get; }
	}
}
