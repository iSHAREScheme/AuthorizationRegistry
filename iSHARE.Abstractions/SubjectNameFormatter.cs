using System.Collections.Generic;
using System.Linq;

namespace iSHARE.Abstractions
{
    public static class SubjectNameFormatter
    {
        public static string FormatByAttributesAsc(string subjectName)
        {
            return string.IsNullOrWhiteSpace(subjectName)
                ? subjectName
                : ToString(ExtractOrderedAttributes(subjectName));
        }

        private static IEnumerable<string> ExtractOrderedAttributes(string subjectName)
        {
            return subjectName.Split(',').Select(x => x.Trim()).OrderBy(x => x);
        }

        private static string ToString(IEnumerable<string> attributes)
        {
            return attributes
                .Aggregate(string.Empty, (current, attribute) => current + $"{attribute}, ")
                .TrimEnd(',', ' ');
        }
    }
}
