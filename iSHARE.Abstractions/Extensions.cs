using System;
using System.Collections.Generic;
using System.Linq;

namespace iSHARE.Abstractions
{
    public static class Extensions
    {
        /// <summary>
        /// Throws a <exception cref="ArgumentNullException"></exception> if the source value is not provided
        /// </summary>
        /// <param name="value">Value to check if it is null or empty</param>
        /// <param name="paramName">Additional identifier of the value to be checked</param>
        /// <exception cref="ArgumentNullException">Exception to throw when the value is null or empty</exception>
        public static void NotNullOrEmpty(this string value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// Determines if the value is present in the source ignoring the case
        /// </summary>
        /// <param name="source">The source where the value is searched</param>
        /// <param name="value">The value that is searched</param>
        /// <param name="comparison">The search policy that can be set, which by default is StringComparison.OrdinalIgnoreCase</param>
        /// <returns>True if value is present in source, otherwise false</returns>
        public static bool Has(this string source, string value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return source?.IndexOf(value, comparison) >= 0;
        }

        /// <summary>
        /// Determines if the value is present in any of element from the source
        /// </summary>
        /// <param name="source">The source of elements where the value is present in any of the element</param>
        /// <param name="value">The value that is searched</param>
        /// <param name="comparison">The search policy that can be set, which by default is StringComparison.OrdinalIgnoreCase</param>
        /// <returns>True if value is present in any of the source elements, otherwise false</returns>
        public static bool Has(this IEnumerable<string> source, string value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return source.Any(c => c.Has(value, comparison));
        }
    }
}
