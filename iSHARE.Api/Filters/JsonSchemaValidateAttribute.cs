using System.Linq;
using System.Threading.Tasks;
using iSHARE.Api.Configurations;
using Manatee.Json;
using Manatee.Json.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace iSHARE.Api.Filters
{
    public class JsonSchemaValidateAttribute : ActionFilterAttribute
    {
        private readonly IJsonSchema _schema;
        private readonly ILogger<JsonSchemaValidateAttribute> _logger;

        public JsonSchemaValidateAttribute(string schemaFile, IJsonSchemaStore schemaStore, ILogger<JsonSchemaValidateAttribute> logger)
        {
            _schema = schemaStore.GetSchema(schemaFile);
            _logger = logger;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var objectModel = JsonConvert.SerializeObject(context.ActionArguments.FirstOrDefault().Value,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), NullValueHandling = NullValueHandling.Ignore });

            if (string.IsNullOrEmpty(objectModel))
            {
                _logger.LogWarning("Object not defined.");
                context.Result = new BadRequestResult();
                return;
            }

            var response = _schema.Validate(JsonValue.Parse(objectModel));

            if (!response.Valid)
            {
                var errors = response.Errors.Select(e => e.Message + " " + e.PropertyName).ToList();
                var errorMessage = errors.Aggregate("", (s, i) => "" + s + "," + i);
                _logger.LogWarning($"Errors during JSON schema validation: {errorMessage}");
                context.Result = new BadRequestObjectResult(new { error = errors });
                return;
            }

            await base.OnActionExecutionAsync(context, next);
        }
    }
}
