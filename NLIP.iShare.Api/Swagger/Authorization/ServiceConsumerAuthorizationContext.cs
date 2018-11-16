using System.Security.Claims;
using IdentityServer4.Validation;
using Manatee.Json.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLIP.iShare.Configuration.Configurations;
using NLIP.iShare.IdentityServer.Validation.Interfaces;

namespace NLIP.iShare.Api.Swagger.Authorization
{
    public class ServiceConsumerAuthorizationContext
    {
        public IJsonSchema JsonSchema { get; set; }
        public ILogger<AuthorizeServiceConsumerAttribute> Logger { get; set; }
        public PartyDetailsOptions ServiceProviderDetails { get; set; }
        public IAuthorizationRegistryClient Client { get; set; }
        public object ResourceRepository { get; set; }
        public ISecretValidator SecretValidator { get; set; }
        public ClaimsPrincipal User { get; set; }
        public string ResourceId { get; set; }
        public string[] ResourceAttributes { get; set; }
        public IActionResult Result { get; set; }
        public string SignedDelegation { get; set; }

        public bool HasSignedDelegation { get; internal set; }
        public bool HasClientAssertion { get; internal set; }
        public string ClientAssertion { get; internal set; }
        public string AuthorityAudience { get; set; }
        public string AuthorityClientId { get; set; }
    }
}
