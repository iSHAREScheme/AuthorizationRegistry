using IdentityServer4.Validation;
using Manatee.Json.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLIP.iShare.Configuration.Configurations;
using NLIP.iShare.IdentityServer.Validation.Interfaces;
using System.Security.Claims;

namespace NLIP.iShare.IdentityServer.Authorization
{
    public class ServiceConsumerAuthorizationContext
    {
        public IJsonSchema JsonSchema;
        public ILogger<AuthorizeServiceConsumerAttribute> Logger;
        public PartyDetailsOptions ServiceProviderDetails;
        public IAuthorizationRegistryClient Client;
        public object ResourceRepository;
        public ISecretValidator SecretValidator;
        public ClaimsPrincipal User;
        public string ResourceId;
        public string[] ResourceAttributes;
        public IActionResult Result;
        public string SignedDelegation;

        public bool HasSignedDelegation { get; internal set; }
        public bool HasClientAssertion { get; internal set; }
        public string ClientAssertion { get; internal set; }
        public string AuthorityAudience { get; set; }
        public string AuthorityClientId { get; set; }
    }
}
