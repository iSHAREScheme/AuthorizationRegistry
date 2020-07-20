using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace iSHARE.IdentityServer.Validation.Token.Logging
{
    /// <summary>
    /// Stolen from IdentityServer4. <see cref="TokenValidator"/> uses logger which needs this serializer.
    /// We couldn't reuse it from IdentityServer4 nuget due to access modifier.
    /// </summary>
    internal static class LogSerializer
    {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            Formatting = Formatting.Indented
        };

        static LogSerializer()
        {
            JsonSettings.Converters.Add(new StringEnumConverter());
        }

        public static string Serialize(object logObject)
        {
            return JsonConvert.SerializeObject(logObject, JsonSettings);
        }
    }
}
