using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLIP.iShare.Abstractions;
using System.Collections.Generic;

namespace NLIP.iShare.Api.Filters
{
    public class SignResponse : ActionFilterAttribute
    {
        public string TokenName { get; set; }
        public string ClaimName { get; set; }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var jwtBuilder = context.HttpContext.RequestServices.GetRequiredService<IResponseJwtBuilder>();
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<SignResponse>>();

            var doNotSign = string.Compare(context.HttpContext.Request.Headers["Do-Not-Sign"], "true", true) == 0;

            if (doNotSign)
            {
                logger.LogInformation("The response is not signed.");
                return;
            }

            var clientId = context.HttpContext.User.GetRequestingPartyId();
            if (clientId == null)
            {
                logger.LogWarning("The client_id claim was not found.");
                return;
            }

            var objectResult = context.Result as ObjectResult;
            if (objectResult == null)
            {
                logger.LogInformation("The context result is missing.");
                return;
            }

            objectResult.Value = new Dictionary<string, object> {
                {  TokenName, jwtBuilder.Create(objectResult.Value, clientId, ClaimName) }
            };
        }
    }
}
