using System;
using System.Diagnostics;

namespace iSHARE.IdentityServer.Validation.Token.Extensions
{
    /// <summary>
    /// Stolen from IdentityServer4.
    /// <see cref="TokenValidator"/> uses this extension, however, we couldn't reuse it from IdentityServer4 nuget due to access modifier.
    /// </summary>
    internal static class DateTimeExtensions
    {
        [DebuggerStepThrough]
        public static bool HasExceeded(this DateTime creationTime, int seconds, DateTime now)
        {
            return now > creationTime.AddSeconds(seconds);
        }
    }
}
