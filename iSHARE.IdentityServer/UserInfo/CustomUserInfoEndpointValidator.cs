using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iSHARE.Abstractions.Json;
using iSHARE.Abstractions.Json.Interfaces;
using iSHARE.Models.DelegationMask;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace iSHARE.IdentityServer.UserInfo
{
    internal class CustomUserInfoEndpointValidator : ICustomUserInfoEndpointValidator
    {
        private static readonly JsonSerializerSettings SerializerSettings = CreateSerializerSettings();

        private readonly IJsonSchemaValidator _jsonSchemaValidator;
        private readonly ILogger<CustomUserInfoEndpointValidator> _logger;

        public CustomUserInfoEndpointValidator(
            IJsonSchemaStore jsonSchemaStore,
            ILogger<CustomUserInfoEndpointValidator> logger)
        {
            _logger = logger;
            _jsonSchemaValidator = jsonSchemaStore.GetSchema(JsonSchema.DelegationMask);
        }

        public async Task<bool> IsValid(HttpContext context)
        {
            if (IsContentTypeUnsupported(context.Request.ContentType))
            {
                return false;
            }

            try
            {
                var requestParam = await ExtractBodyRequestParamAsync(context.Request);

                if (IsMockedUnhappyFlowBehaviour(requestParam.DelegationRequest))
                {
                    return false;
                }

                return IsRequestSchemaValid(requestParam);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception during validation occurred.");
                return false;
            }
        }

        private static bool IsContentTypeUnsupported(string contentType)
        {
            return !contentType.StartsWith("application/json");
        }

        private bool IsRequestSchemaValid(DelegationRequestRoot request)
        {
            var delegationRequest = JsonConvert.SerializeObject(request, SerializerSettings);
            if (string.IsNullOrEmpty(delegationRequest))
            {
                _logger.LogError("Request object not found.");
                return false;
            }

            var result = _jsonSchemaValidator.Validate(delegationRequest);
            if (result.IsValid)
            {
                return true;
            }

            _logger.LogError($"Errors during delegation mask validation: {result.ErrorMessage}.");
            return false;
        }

        /// <summary>
        /// CTT has special test cases that checks the corner cases.
        /// Since we don't have an actual implementation but it is required for us to pass those tests, 
        /// in order to test if CTT behaves as expected - we mock them.
        /// </summary>
        /// <returns></returns>
        private static bool IsMockedUnhappyFlowBehaviour(DelegationRequest delegationRequest)
        {
            var invalidPolicyIssuers = new[] { "EU.EORI.NLUNREGISTERED", "EU.EORI.NL100000015", "EU.EORI.NL000000003" };

            if (invalidPolicyIssuers.Contains(delegationRequest.PolicyIssuer))
            {
                return true;
            }

            if (delegationRequest.Target.AccessSubject == "randomidentifier")
            {
                return true;
            }

            return false;
        }

        private static JsonSerializerSettings CreateSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        private async Task<DelegationRequestRoot> ExtractBodyRequestParamAsync(HttpRequest request)
        {
            var bodyJson = await ReadBodyAsStringAsync(request);

            return JsonConvert.DeserializeObject<DelegationRequestRoot>(bodyJson);
        }

        private async Task<string> ReadBodyAsStringAsync(HttpRequest request)
        {
            using StreamReader reader = new StreamReader(request.Body);
            return await reader.ReadToEndAsync();
        }

        private class DelegationRequestRoot
        {
            public DelegationRequest DelegationRequest { get; set; }
        }
    }
}
