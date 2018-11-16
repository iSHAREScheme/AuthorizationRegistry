namespace NLIP.iShare.Models.Capabilities
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Capabilities
    {
        [JsonProperty("party_id")]
        public string PartyId { get; set; }

        [JsonProperty("ishare_roles")]
        public IReadOnlyCollection<SchemeRole> Roles { get; set; } = new List<SchemeRole>();

        [JsonProperty("supported_versions")]
        public IReadOnlyCollection<SupportedVersion> SupportedVersions { get; set; } = new List<SupportedVersion>();
    }

    public class SchemeRole
    {
        [JsonProperty("role")]
        public string Role { get; set; }
    }

    public class SupportedVersion
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("supported_features")]
        public IReadOnlyCollection<SupportedFeature> SupportedFeatures { get; set; }
    }

    public class SupportedFeature
    {
        [JsonProperty("feature")]
        public string Feature { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
    }

    public partial class Capabilities
    {
        public static Capabilities FromJson(string json) => JsonConvert.DeserializeObject<Capabilities>(json, NLIP.iShare.Models.Capabilities.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Capabilities self) => JsonConvert.SerializeObject(self, NLIP.iShare.Models.Capabilities.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}