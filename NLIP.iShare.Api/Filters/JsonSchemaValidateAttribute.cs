using Manatee.Json;
using Manatee.Json.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NLIP.iShare.Api.Filters
{
    public class JsonSchemaValidateAttribute : ActionFilterAttribute
    {
        private IJsonSchema _schema;
        private ILogger<JsonSchemaValidateAttribute> _logger;

        public JsonSchemaValidateAttribute(Func<string, IJsonSchema> schemaFactory, ILogger<JsonSchemaValidateAttribute> logger)
        {
            _schema = schemaFactory("delegationMaskSchema.json");
            _logger = logger;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var objectModel = context.ActionArguments.FirstOrDefault().Value?.ToString();

            if (string.IsNullOrEmpty(objectModel))
            {
                _logger.LogWarning("Object not defined.");
                context.Result = new BadRequestResult();
                return;
            }

            var result = _schema.Validate(JsonValue.Parse(objectModel));

            if (!result.Valid)
            {
                var errors = result.Errors.Select(e => e.Message + " " + e.PropertyName).ToList();
                var errorMessage = errors.Aggregate("", (s, i) => "" + s + "," + i);
                _logger.LogWarning($"Errors during JSON schema validation: {errorMessage}");
                context.Result = new BadRequestObjectResult(new { error = errors });
                return;
            }

            await base.OnActionExecutionAsync(context, next);
        }
    }
}
