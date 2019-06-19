using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using Manatee.Json;
using Manatee.Json.Schema;

namespace iSHARE.Api.Configurations
{
    public interface IJsonSchemaStore
    {
        IJsonSchema GetSchema(string fileName);
    }

    public class JsonSchemaStore : IJsonSchemaStore
    {
        private readonly ConcurrentDictionary<string, IJsonSchema> _store;

        public JsonSchemaStore()
        {
            _store = new ConcurrentDictionary<string, IJsonSchema>();
        }

        public IJsonSchema GetSchema(string fileName)
        {
            return _store.GetOrAdd(fileName, _ =>
            {
                var assembly = Assembly.GetAssembly(typeof(iSHARE.IdentityServer
                    .Configuration));
                using (var stream = assembly.GetManifestResourceStream(
                    $"{typeof(iSHARE.IdentityServer.Configuration).Namespace}.Schemas.{fileName}"))
                {

                    using (var reader = new StreamReader(stream))
                    {
                        var content = reader.ReadToEnd();
                        var schema = new Manatee.Json.Serialization.JsonSerializer()
                            .Deserialize<IJsonSchema>(JsonValue.Parse(content));

                        return schema;
                    }
                }
            });

        }
    }

}
