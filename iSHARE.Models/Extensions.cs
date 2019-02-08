using System.Collections.Generic;
using System.Linq;

namespace iSHARE.Models
{
    internal static class Extensions
    {
        public static bool HasElements(this IEnumerable<string> source)
            => source != null && source.Any();
        public static bool Has(this IEnumerable<string> source, string element) 
            => source.Contains("*") || source.Contains(element);
        public static bool HasAny(this IEnumerable<string> source, IEnumerable<string> elements)
            => source.Contains("*") || elements.Any(a => source.Contains(a));
    }
}
