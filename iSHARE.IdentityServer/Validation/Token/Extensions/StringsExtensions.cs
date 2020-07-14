using System.Diagnostics;

namespace iSHARE.IdentityServer.Validation.Token.Extensions
{
    /// <summary>
    /// Stolen from IdentityServer4.
    /// <see cref="TokenValidator"/> uses this extension, however, we couldn't reuse it from IdentityServer4 nuget due to access modifier.
    /// </summary>
    internal static class StringsExtensions
    {
        [DebuggerStepThrough]
        public static bool IsMissing(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        [DebuggerStepThrough]
        public static bool IsPresent(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
    }
}
