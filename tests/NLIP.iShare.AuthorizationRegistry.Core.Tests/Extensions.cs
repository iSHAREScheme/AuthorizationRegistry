using Newtonsoft.Json;
using System.IO;

namespace NLIP.iShare.AuthorizationRegistry.Core.Tests
{
    public static class Extensions
    {
        public static string Read(this string filename) => File.ReadAllText(filename);

        public static T Read<T>(this string filename) => JsonConvert.DeserializeObject<T>(File.ReadAllText(filename));
    }
}
