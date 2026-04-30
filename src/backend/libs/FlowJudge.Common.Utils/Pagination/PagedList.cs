using System.Collections;

namespace FlowJudge.Common.Utils.Pagination
{
    public class PagedList<TItem> : IReadOnlyCollection<TItem>
    {
        private readonly List<TItem> _items;

        public PagedList(IEnumerable<TItem> items, int pageSize, int pageNumber, int totalCount)
        {
            _items = new(items);
            PageSize = pageSize;
            PageNumber = pageNumber;
            TotalCount = totalCount;
        }

        public int Count => _items.Count;
        public int PageSize { get; }
        public int PageNumber { get; }
        public int TotalCount { get; }

        public IEnumerator<TItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
    