using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NLIP.iShare.EntityFramework
{
    public static class ImportFromJson
    {
        public static TEntity[] DeserializeCollectionOf<TEntity>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return new TEntity[] { };
            }

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new PrivateOrInternalPropertiesResolver()
            };

            return JsonConvert.DeserializeObject<TEntity[]>(json, settings);
        }

        public class PrivateOrInternalPropertiesResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                .Select(p => CreateProperty(p, memberSerialization))
                                .Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                .Select(f => CreateProperty(f, memberSerialization)))
                                .ToList();

                foreach (var prop in props)
                {
                    prop.Writable = true;
                    prop.Readable = true;
                }

                return props;
            }
        }
    }
}
