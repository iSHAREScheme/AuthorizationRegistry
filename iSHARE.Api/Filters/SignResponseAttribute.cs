using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iSHARE.Abstractions;
using iSHARE.Api.Configurations;
using iSHARE.Configuration.Configurations;
using iSHARE.Models;
using iSHARE.Models.DelegationEvidence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace iSHARE.Api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class SignResponseAttribute : ActionFilterAttribute
    {
        public string TokenName { get; set; }
        public string ClaimName { get; set; }
        public Type JsonContractType { get; set; }
        public string SwaggerDefinitionName { get; set; }
        public bool ResponseContainsList { get; set; }
        public bool AnonymousUsage { get; set; }

        private IContractResolver _contractResolver;

        public IContractResolver ContractResolver
        {
            get
            {
                if (JsonContractType != null && _contractResolver?.GetType() != JsonContractType)
                {
                    _contractResolver = Activator.CreateInstance(JsonContractType) as IContractResolver;
                }
                return _contractResolver;
            }
        }

        public SignResponseAttribute(string tokenName,
                                     string claimName,
                                     string swaggerDefinitionName,
                                     bool responseContainsList = false)
        {
            TokenName = tokenName;
            ClaimName = claimName;
            SwaggerDefinitionName = swaggerDefinitionName;
            ResponseContainsList = responseContainsList;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var shouldSign = ShouldSign(context);

            if (shouldSign)
            {
                await Sign(context);
            }

            await next();
        }

        private async Task Sign(ResultExecutingContext context)
        {
            var serviceProvider = context.HttpContext.RequestServices;
            var jwtBuilder = serviceProvider.GetRequiredService<IResponseJwtBuilder>();
            var partyDetails = serviceProvider.GetRequiredService<PartyDetailsOptions>();
            var logger = serviceProvider.GetRequiredService<ILogger<SignResponseAttribute>>();

            var objectResult = context.Result as ObjectResult;
            if (objectResult == null)
            {
                logger.LogWarning("The response is not of ObjectResult type so it won't be signed.");
                return;
            }

            var subject = partyDetails.ClientId;
            var originalValue = objectResult.Value;
            if (originalValue is DelegationEvidence delegation)
            {
                subject = delegation.Target.AccessSubject;
            }

            var audience = AnonymousUsage && !context.HttpContext.User.Identity.IsAuthenticated
                ? null
                : context.HttpContext.User.TryGetRequestingClientId();
            var jwt = await jwtBuilder.Create(
                originalValue,
                subject,
                partyDetails.ClientId,
                audience,
                ClaimName,
                ContractResolver);
            objectResult.Value = new Dictionary<string, object>
            {
                {
                    TokenName, jwt
                }
            };
        }

        private static bool ShouldSign(ResultExecutingContext context)
        {
            var serviceProvider = context.HttpContext.RequestServices;
            var logger = serviceProvider.GetRequiredService<ILogger<SignResponseAttribute>>();
            var environment = serviceProvider.GetRequiredService<IHostingEnvironment>();
            if (environment.IsLiveOrQaLive())
            {
                return true;
            }

            var doNotSign = string.Compare(context.HttpContext.Request.Headers["Do-Not-Sign"], "true",
                                StringComparison.OrdinalIgnoreCase) == 0;

            if (doNotSign)
            {
                logger.LogInformation("The response is not signed because the http header was provided and it overrides this behavior.");
                return false;
            }

            var objectResult = context.Result as ObjectResult;
            if (objectResult == null)
            {
                logger.LogInformation("The context result is missing.");
                return false;
            }

            if (objectResult.Value is ResponseMessage)
            {
                logger.LogInformation("Response messages should not be signed.");
                return false;

            }

            // only successful responses should be signed            
            if (objectResult.StatusCode.GetValueOrDefault() >= 400)
            {
                return false;
            }

            return true;
        }
    }
}
