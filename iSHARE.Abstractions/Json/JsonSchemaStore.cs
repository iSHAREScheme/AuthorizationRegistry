using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using iSHARE.Abstractions.Json.Interfaces;
using Manatee.Json;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;

namespace iSHARE.Abstractions.Json
{
    public class JsonSchemaStore : IJsonSchemaStore
    {
        private readonly ConcurrentDictionary<string, IJsonSchemaValidator> _store
            = new ConcurrentDictionary<string, IJsonSchemaValidator>();

        public IJsonSchemaValidator GetSchema(JsonSchema jsonSchema)
        {
            var fileName = JsonSchemaEnumToFileNameMapper.Map(jsonSchema);

            return _store.GetOrAdd(fileName, _ =>
            {
                var assembly = Assembly.GetAssembly(typeof(JsonSchemaStore));
                var absoluteResourcePath = $"{typeof(JsonSchemaStore).Namespace}.Schemas.{fileName}";
                using var stream = assembly.GetManifestResourceStream(absoluteResourcePath);
                using var reader = new StreamReader(stream);
                var content = reader.ReadToEnd();

                return LoadJsonSchemaValidator(content);
            });
        }

        private static JsonSchemaValidator LoadJsonSchemaValidator(string jsonSchema)
        {
            var schema = new JsonSerializer().Deserialize<IJsonSchema>(JsonValue.Parse(jsonSchema));

            return new JsonSchemaValidator(schema);
        }
    }
}
