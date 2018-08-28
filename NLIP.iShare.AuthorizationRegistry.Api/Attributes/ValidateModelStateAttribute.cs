using Manatee.Json;
using Manatee.Json.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace NLIP.iShare.AuthorizationRegistry.Api.Attributes
{
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        private readonly IJsonSchema _schema;
        private readonly ILogger<ValidateModelStateAttribute> _logger;

        public ValidateModelStateAttribute(IJsonSchema schema, ILogger<ValidateModelStateAttribute> logger)
        {
            _schema = schema;
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var maskText = context.ActionArguments.FirstOrDefault().Value?.ToString();

            if (string.IsNullOrEmpty(maskText))
            {
                _logger.LogWarning("Policy mask is missing or is not in a JSON format.");

                context.Result = new BadRequestObjectResult(new { error = "Policy mask is missing or is not in a JSON format." });
                return;
            }

            var result = _schema.Validate(JsonValue.Parse(maskText));

            if (!result.Valid)
            {
                var errors = result.Errors.Select(e => e.Message + " " + e.PropertyName);
                _logger.LogWarning("Errors during policy mask validation", errors);

                context.Result = new BadRequestObjectResult(new { error = errors });
            }
        }
    }
}
