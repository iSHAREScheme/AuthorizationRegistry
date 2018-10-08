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
    public class SignResponseAttribute : ActionFilterAttribute
    {
        public string TokenName { get; set; }
        public string ClaimName { get; set; }

        public Type JsonContractType { get; set; }

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

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var serviceProvider = context.HttpContext.RequestServices;
            var jwtBuilder = serviceProvider.GetRequiredService<IResponseJwtBuilder>();
            var logger = serviceProvider.GetRequiredService<ILogger<SignResponseAttribute>>();

            var doNotSign = string.Compare(context.HttpContext.Request.Headers["Do-Not-Sign"], "true", true) == 0;

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

            var issuer = partyDetails.ClientId;
            var subject = partyDetails.ClientId;
            var audience = partyDetails.ClientId;

            var originalValue = objectResult.Value;
            if (originalValue is DelegationEvidence delegation)
            {
                subject = delegation.Target.AccessSubject;
            }

            objectResult.Value = new Dictionary<string, object> {
                {  TokenName, jwtBuilder.Create(
                    payloadObject: originalValue,
                    subject: subject,
                    issuer: issuer,
                    audience: audience,
                    payloadObjectClaim: ClaimName,
                    contractResolver: ContractResolver)
                }
            };


        }
    }
}
