using System.IO;
using Newtonsoft.Json;

namespace NLIP.iShare.IdentityServer.Tests
{
    public static class FileExtensions
    {
        public static string Read(this string filename) => File.ReadAllText(filename);

        public static T Read<T>(this string filename) => JsonConvert.DeserializeObject<T>(File.ReadAllText(filename));
    }
}
