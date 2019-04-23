using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace iSHARE.Models
{
    public static class FriendlyIdGenerator
    {
        public static string New(string prefix)
        {
            const int length = 10;
            const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const int byteSize = 0x100;
            var allowedCharSet = new HashSet<char>(allowedChars).ToArray();

            using (var rng = RandomNumberGenerator.Create())
            {
                var result = new StringBuilder();
                var buf = new byte[128];
                while (result.Length < length)
                {
                    rng.GetBytes(buf);
                    for (var i = 0; i < buf.Length && result.Length < length; ++i)
                    {
                        var outOfRangeStart = byteSize - byteSize % allowedCharSet.Length;
                        if (outOfRangeStart <= buf[i])
                        {
                            continue;
                        }
                        result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
                    }
                }
                return $"{prefix}.{result.ToString().ToUpper(CultureInfo.CurrentCulture)}";
            }
        }
    }
}
