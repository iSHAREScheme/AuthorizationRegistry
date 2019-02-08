using System.Collections.Generic;

namespace iSHARE.Abstractions
{
    public class PagedResult<T>
    {
        public int Count { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
