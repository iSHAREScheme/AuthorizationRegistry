using System.Collections.Generic;

namespace NLIP.iShare.Abstractions
{
    public class PagedResult<T>
    {
        public int Count { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
