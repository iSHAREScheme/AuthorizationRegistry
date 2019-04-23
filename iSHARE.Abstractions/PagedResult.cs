using System.Collections.Generic;
using Newtonsoft.Json;

namespace iSHARE.Abstractions
{
    public class PagedResult<T>
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("data")]
        public IEnumerable<T> Data { get; set; }
    }
}
