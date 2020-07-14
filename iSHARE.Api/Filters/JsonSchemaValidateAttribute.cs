using System.Linq;
using System.Threading.Tasks;
using iSHARE.Abstractions.Json;
using iSHARE.Abstractions.Json.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace iSHARE.Api.Filters
{
    public class JsonSchemaValidateAttribute : ActionFilterAttribute
    {
        private static readonly JsonSerializerSettings SerializerSettings = CreateSerializerSettings();

        private readonly IJsonSchemaValidator _schema;
        private readonly ILogger<JsonSchemaValidateAttribute> _logger;

        public JsonSchemaValidateAttribute(
            JsonSchema jsonSchema,
            IJsonSchemaStore schemaStore,
            ILogger<JsonSchemaValidateAttribute> logger)
        {
            _schema = schemaStore.GetSchema(jsonSchema);
            _logger = logger;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var objectModel = JsonConvert.SerializeObject(
                context.ActionArguments.FirstOrDefault().Value,
                SerializerSettings);

            if (string.IsNullOrEmpty(objectModel))
            {
                _logger.LogWarning("Object not defined.");
                context.Result = new BadRequestResult();
                return;
            }

            var response = _schema.Validate(objectModel);

            if (!response.IsValid)
            {
                _logger.LogWarning($"Errors during JSON schema validation: {response.ErrorMessage}");
                context.Result = new BadRequestObjectResult(new { error = response.Errors });
                return;
            }

            await base.OnActionExecutionAsync(context, next);
        }

        private static JsonSerializerSettings CreateSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
        }
    }
}
