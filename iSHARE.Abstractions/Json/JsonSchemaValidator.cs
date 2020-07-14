using iSHARE.Abstractions.Json.Interfaces;
using Manatee.Json;
using Manatee.Json.Schema;

namespace iSHARE.Abstractions.Json
{
    /// <summary>
    /// Responsible for validating if given json corresponds to loaded json schema.
    /// Mainly used to wrap Manatee.Json implementation, so nuget packages won't be installed to each project.
    /// </summary>
    internal class JsonSchemaValidator : IJsonSchemaValidator
    {
        private readonly IJsonSchema _jsonSchema;

        public JsonSchemaValidator(IJsonSchema jsonSchema)
        {
            _jsonSchema = jsonSchema;
        }

        public JsonSchemaValidationResult Validate(string json)
        {
            var result = _jsonSchema.Validate(JsonValue.Parse(json));

            return result.Valid
                ? JsonSchemaValidationResult.Success()
                : JsonSchemaValidationResult.FromError(result.Errors);
        }
    }
}
