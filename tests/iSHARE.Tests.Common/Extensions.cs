using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using iSHARE.Abstractions;

namespace iSHARE.Tests.Common
{
    public static class Extensions
    {
        public static Dictionary<string, string> RemoveByKey(this Dictionary<string, string> dictionary, string key)
        {
            var item = dictionary.FirstOrDefault(c => c.Key == key);

            if (item.Key == key)
            {
                dictionary.Remove(key);
            }

            return dictionary;
        }

        public static JwtSecurityToken AddPublicKeyHeader(this JwtSecurityToken token, IEnumerable<string> certificates)
        {
            if (certificates != null && certificates.Any())
            {
                token.Header.Add("x5c", certificates
                        .Where(c => !string.IsNullOrEmpty(c))
                        .Select(c => c?.ConvertToBase64Der()))
                    ;
            }

            return token;
        }

        public static JwtSecurityToken AddPublicKeyHeader(this JwtSecurityToken token, string certificate)
        {
            return token.AddPublicKeyHeader(new[] { certificate });
        }

        public static JwtSecurityToken RemoveHeader(this JwtSecurityToken token, string header)
        {
            token.Header.Remove(header);
            return token;
        }

        public static JwtSecurityToken AddHeader(this JwtSecurityToken token, string header, object value)
        {
            token.Header.Add(header, value);
            return token;
        }

        public static string[] AsArray(this string source)
        {
            return new[] { source };
        }

        public static HttpClient WithOAuthBearerToken(this HttpClient client, string accessToken)
        {
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            return client;
        }
    }
}
