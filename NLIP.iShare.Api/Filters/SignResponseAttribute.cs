using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using NLIP.iShare.Abstractions;
using NLIP.iShare.Configuration.Configurations;
using NLIP.iShare.Models.DelegationEvidence;
using System;
using System.Collections.Generic;

namespace NLIP.iShare.Api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class SignResponseAttribute : ActionFilterAttribute
    {
        public string TokenName { get; set; }
        public string ClaimName { get; set; }

        public Type JsonContractType { get; set; }

        public string SwaggerDefinitionName { get; set; }
        public bool ResponseContainsList { get; set; }

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

        public SignResponseAttribute(string tokenName, string claimName, string swaggerDefinitionName, bool responseContainsList = false)
        {
            TokenName = tokenName;
            ClaimName = claimName;
            SwaggerDefinitionName = swaggerDefinitionName;
            ResponseContainsList = responseContainsList;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var serviceProvider = context.HttpContext.RequestServices;
            var jwtBuilder = serviceProvider.GetRequiredService<IResponseJwtBuilder>();
            var logger = serviceProvider.GetRequiredService<ILogger<SignResponseAttribute>>();

            var doNotSign = string.Compare(context.HttpContext.Request.Headers["Do-Not-Sign"], "true", StringComparison.OrdinalIgnoreCase) == 0;

            if (doNotSign)
            {
                logger.LogInformation("The response is not signed.");
                return;
            }

            if (!(context.Result is ObjectResult objectResult))
            {
                logger.LogInformation("The context result is missing.");
                return;
            }

            // only successful responses should be signed            
            if (objectResult.StatusCode.GetValueOrDefault() >= 400)
            {
                return;
            }
            var partyDetails = serviceProvider.GetRequiredService<PartyDetailsOptions>();

            var subject = partyDetails.ClientId;
            var originalValue = objectResult.Value;
            if (originalValue is DelegationEvidence delegation)
            {
                subject = delegation.Target.AccessSubject;
            }

            objectResult.Value = new Dictionary<string, object> {
                {
                    TokenName, jwtBuilder.Create(
                                    originalValue,
                                    subject,
                                    partyDetails.ClientId,
                                    partyDetails.ClientId,
                                    ClaimName,
                                    ContractResolver)
                }
            };
        }
    }
}
