using System;
using System.Collections.Generic;
using System.Linq;

namespace Bank_Configuration_Portal.Common.Paging
{
    public sealed class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; private set; }
        public int TotalCount { get; private set; }
        public int Page { get; private set; }
        public int PageSize { get; private set; }

        public int TotalPages
        {
            get { return PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize); }
        }
        public bool HasPrevious { get { return Page > 1; } }
        public bool HasNext { get { return Page < TotalPages; } }

        public PagedResult(IEnumerable<T> items, int totalCount, int page, int pageSize)
        {
            if (items == null) items = Enumerable.Empty<T>();

            var list = items as IReadOnlyList<T>;
            if (list == null) list = new List<T>(items);

            Items = list;
            TotalCount = totalCount;
            Page = page < 1 ? 1 : page;
            PageSize = pageSize <= 0 ? 6 : pageSize;
        }

        public static PagedResult<T> Empty(int page, int pageSize)
        {
            return new PagedResult<T>(Enumerable.Empty<T>(), 0, page, pageSize);
        }
    }

    public sealed class PagedRequest
    {
        public int Page { get; private set; }
        public int PageSize { get; private set; }

        public PagedRequest(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }
    }
}
