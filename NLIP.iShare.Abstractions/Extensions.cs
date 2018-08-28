using System;

namespace NLIP.iShare.Abstractions
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
    }
}
