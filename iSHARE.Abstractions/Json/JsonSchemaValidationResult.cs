using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Schema;

namespace iSHARE.Abstractions.Json
{
    public class JsonSchemaValidationResult
    {
        private JsonSchemaValidationResult()
        {
        }

        private JsonSchemaValidationResult(IReadOnlyList<string> errors)
        {
            Errors = errors;
            ErrorMessage = string.Join("; ", errors);
        }
        
        public bool IsValid => ErrorMessage == null;

        public IReadOnlyList<string> Errors { get; }

        public string ErrorMessage { get; }

        internal static JsonSchemaValidationResult FromError(IEnumerable<SchemaValidationError> errors)
        {
            var formattedErrors = errors.Select(e => $"{e.Message}: {e.PropertyName}").ToList();
            return new JsonSchemaValidationResult(formattedErrors);
        }

        internal static JsonSchemaValidationResult Success() => new JsonSchemaValidationResult();
    }
}
