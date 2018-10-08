using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Manatee.Json;
using Manatee.Json.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NLIP.iShare.Api;
using NLIP.iShare.Configuration.Configurations;
using NLIP.iShare.IdentityServer.Validation.Interfaces;
using NLIP.iShare.Models.DelegationEvidence;
using NLIP.iShare.Models.DelegationMask;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NLIP.iShare.IdentityServer.Authorization
{
    public class AuthorizeServiceConsumerAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Name of the HTTP parameter used to identify a resource instance
        /// </summary>
        public string IdParam { get; set; }
        /// <summary>
        /// Name of the HTTP parameter used to specify attributes used in authorized request
        /// </summary>
        public string AttributesParam { get; set; }

        /// <summary>
        /// iSHARE specific type of resource
        /// </summary>
        public string ResourceType { get; set; } = "CONTAINER.DATA";

        /// <summary>
        /// iSHARE specific action identifier used to authorize resource
        /// </summary>
        public string IShareAction { get; set; } = "ISHARE.READ";

        public Type DataServiceType { get; set; }

        private readonly string _clientAssertionParam = OidcConstants.TokenRequest.ClientAssertion;
        private readonly string _delegationEvidenceParam = "delegation_evidence";


        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var query = context.HttpContext.Request.Query;
            if (!query.ContainsKey(IdParam) || !query.ContainsKey(AttributesParam))
            {
                context.Result = new BadRequestResult();
                return;
            }

            var ctx = ResolveServices(context);

            var result = IdentityResult.Failed(new IdentityError { Code = "unknown_error" });
            if (ctx.HasSignedDelegation)
            {
                result = AuthorizeByDelegationEvidence(ctx);
            }
            else
            {
                result = await AuthorizeClientAssertion(ctx).ConfigureAwait(false);

                if (result.Succeeded)
                {
                    result = await AuthorizeByAuthorizationRegistry(ctx).ConfigureAwait(false);
                }
            }
            if (!result.Succeeded)
            {
                context.Result = ctx.Result;
            }
            else
            {
                await next();
            }
        }

        private ServiceConsumerAuthorizationContext ResolveServices(ActionExecutingContext context)
        {
            var serviceProvider = context.HttpContext.RequestServices;
            var headers = context.HttpContext.Request.Headers;            

            var ctx = new ServiceConsumerAuthorizationContext
            {
                Result = context.Result,
                JsonSchema = serviceProvider.GetService<Func<string, IJsonSchema>>()("delegationEvidenceSchema.json"),
                Logger = serviceProvider.GetService<ILogger<AuthorizeServiceConsumerAttribute>>(),
                ServiceProviderDetails = serviceProvider.GetService<PartyDetailsOptions>(),
                Client = serviceProvider.GetService<IAuthorizationRegistryClient>(),
                ResourceRepository = serviceProvider.GetService(DataServiceType),
                SecretValidator = serviceProvider.GetService<ISecretValidator>(),
                User = context.HttpContext.User,
                ResourceId = context.HttpContext.Request.Query[IdParam].ToString(),
                ResourceAttributes = context.HttpContext.Request.Query[AttributesParam].ToString().Split(new[] { ',' })
            };
            ctx.HasSignedDelegation = headers.ContainsKey(_delegationEvidenceParam);
            if (ctx.HasSignedDelegation)
            {
                ctx.SignedDelegation = headers[_delegationEvidenceParam].FirstOrDefault();
            }

            ctx.HasClientAssertion = headers.ContainsKey(_clientAssertionParam);
            if (ctx.HasClientAssertion)
            {
                ctx.ClientAssertion = headers[_clientAssertionParam];
            }
            var config = serviceProvider.GetService<IConfiguration>();
            ctx.AuthorityAudience = $"{config["AuthorizationRegistry:BaseUri"]}connect/token";
            ctx.AuthorityClientId = config["AuthorizationRegistry:ClientId"];

            return ctx;
        }

        public async Task<IdentityResult> AuthorizeClientAssertion(ServiceConsumerAuthorizationContext ctx)
        {
            var result = await AuthorizeClientAssertionInternal(ctx);
            if (!result.Succeeded)
            {
                ctx.Result = new UnauthorizedResult();
                ctx.Logger.LogWarning($"client assertion validation error {result}");
            }
            return result;
        }


        private async Task<IdentityResult> AuthorizeClientAssertionInternal(ServiceConsumerAuthorizationContext ctx)
        {
            if (string.IsNullOrWhiteSpace(ctx.ClientAssertion))
            {
                return IdentityResult.Failed(new IdentityError { Code = "client_assertion_required", Description = $"header {_clientAssertionParam} is missing for authorized request" });
            }

            try
            {
                var result = await ctx.SecretValidator.ValidateAsync(new List<Secret>(), new ParsedSecret()
                {
                    Id = ctx.User.GetRequestingPartyId(),
                    Type = OidcConstants.GrantTypes.JwtBearer,
                    Credential = ctx.ClientAssertion
                });
                if (result.Success)
                {
                    return IdentityResult.Success;
                }
                return IdentityResult.Failed(new IdentityError { Code = result.Error, Description = result.ErrorDescription });
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Code = "client_assertion_validation_error", Description = ex.Message });
            }
        }

        private async Task<IdentityResult> AuthorizeByAuthorizationRegistry(ServiceConsumerAuthorizationContext ctx)
        {
            var resource = GetResource(ctx.ResourceRepository, ctx.ResourceId);
            if (resource == null)
            {
                ctx.Logger.LogWarning($"No record for {ctx.ResourceId}");
                ctx.Result = new NotFoundResult();
                return IdentityResult.Failed();
            }

            var accessSubject = ctx.User.GetRequestingPartyId();
            var entitledPartyId = GetEntitledPartyId(resource);
            var mask = new DelegationMask(ctx.ResourceId, entitledPartyId, accessSubject, ctx.ResourceAttributes, IShareAction, ResourceType);

            var delegationEvidence = await ctx.Client.GetDelegation(mask, ctx.ClientAssertion).ConfigureAwait(false);

            if (delegationEvidence == null)
            {
                ctx.Logger.LogWarning("No response from AR");
                ctx.Result = new NotFoundResult();
                return IdentityResult.Failed();
            }

            if (!IsDelegationAuthorized(delegationEvidence, ctx.ResourceId, ctx.ResourceAttributes, IShareAction))
            {
                ctx.Logger.LogWarning($"Access denied to {ctx.ResourceId}");
                ctx.Result = new ForbidResult();
                return IdentityResult.Failed();
            }
            return IdentityResult.Success;
        }

        private object GetResource(object dataService, string resourceName)
        {
            MethodInfo get = DataServiceType.GetMethod("Get", new Type[]
                {
                    typeof(string)
                });

            return get.Invoke(dataService, new object[] { resourceName });
        }

        public IdentityResult AuthorizeByDelegationEvidence(ServiceConsumerAuthorizationContext ctx)
        {
            var result = AuthorizeDelegationEvidenceInternal(ctx);
            if (!result.Succeeded)
            {
                ctx.Logger.LogWarning($"delegation evidence validation error {result}");
                ctx.Result = new UnauthorizedResult();
            }
            return result;
        }

        private IdentityResult AuthorizeDelegationEvidenceInternal(ServiceConsumerAuthorizationContext ctx)
        {
            if (string.IsNullOrEmpty(ctx.SignedDelegation))
            {
                ctx.Logger.LogWarning("No delegation evidence token provided.");
                return IdentityResult.Failed(new IdentityError { Code = "delegation_evidence_token_required", Description = $"header {_delegationEvidenceParam} is missing for authorized request" });
            }

            try
            {
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(ctx.SignedDelegation);

                var x5CCerts = jwtToken.Header["x5c"].ToString();
                if (string.IsNullOrEmpty(x5CCerts))
                {
                    ctx.Logger.LogWarning("No x5c header provided.");
                    return IdentityResult.Failed(new IdentityError { Code = "certificate_chain_required", Description = "Certificates chain is missing for authorized request" });
                }

                var chain = JsonConvert.DeserializeObject<string[]>(x5CCerts);

                var validationParams = GetValidationParameters(chain[0],
                       new[] { ctx.User.GetRequestingPartyId(), ctx.AuthorityAudience },
                       ctx.AuthorityClientId);

                var principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken.RawData, validationParams,
                    out SecurityToken securityToken);

                var delegationEvidence = jwtToken.Claims.FirstOrDefault(c => c.Type == "delegationEvidence").Value?.ToString();

                if (string.IsNullOrEmpty(delegationEvidence))
                {
                    ctx.Logger.LogWarning("Delegation evidence not provided in token.");
                    return IdentityResult.Failed(new IdentityError { Code = "delegation_evidence_required", Description = "delegationEvidence is missing from delegation_evidence_token" });
                }

                var result = ctx.JsonSchema.Validate(JsonValue.Parse(delegationEvidence));

                if (!result.Valid)
                {
                    var errors = result.Errors.Select(e => e.Message + " " + e.PropertyName).ToList();
                    var errorMessage = errors.Aggregate("", (s, i) => "" + s + "," + i);
                    ctx.Logger.LogWarning($"Errors during policy mask validation: {errorMessage}");
                    return IdentityResult.Failed(new IdentityError { Code = "json_schema_validation_failed", Description = "delegationEvidence is not in the correct format" });
                }

                var resource = GetResource(ctx.ResourceRepository, ctx.ResourceId);

                if (resource == null)
                {
                    ctx.Logger.LogWarning($"No record for {ctx.ResourceId}");
                    ctx.Result = new ForbidResult();
                    return IdentityResult.Failed(new IdentityError { Code = "container_not_found", Description = "authorization failed based on delegation evidence - no container" });
                }

                var hasContainer = false;
                var policy = GetDelegationEvidence(jwtToken);
                resource.GetType().GetProperties().ToList().ForEach(p =>
                {
                    if (p.Name == "EntitledPartyId")
                    {
                        var value = p.GetValue(resource, null).ToString();
                        hasContainer = p.GetValue(resource, null).ToString() == policy.PolicyIssuer;
                    }
                });

                if (!hasContainer)
                {
                    ctx.Logger.LogWarning($"Policy issuer not found for {ctx.ResourceId}");
                    return IdentityResult.Failed(new IdentityError { Code = "container_not_found", Description = "authorization failed based on delegation evidence - policy issuer" });
                }

                var validationResult = IsDelegationAuthorized(policy, ctx.ResourceId, ctx.ResourceAttributes, IShareAction);
                if (!validationResult)
                {
                    ctx.Logger.LogWarning("Errors during policy mask validation");
                    return IdentityResult.Failed(new IdentityError { Code = "delegation_evidence_authorization_failed", Description = "authorization failed based on delegation evidence provided" });
                }
            }
            catch (Exception ex)
            {
                ctx.Logger.LogWarning("delegation_evidence_token validation failed: {error}", ex.Message);
                return IdentityResult.Failed(new IdentityError { Code = "delegation_evidence_token_invalid", Description = "delegation_evidence_token validation failed." });
            }

            return IdentityResult.Success;
        }

        private TokenValidationParameters GetValidationParameters(string publicKey, string[] audiences, string issuer)
        {
            var cert = new X509Certificate2(Encoding.ASCII.GetBytes(publicKey));
            var key = new X509SecurityKey(cert);
            return new TokenValidationParameters
            {
                IssuerSigningKey = key,
                ValidAudiences = audiences,
                ValidIssuer = issuer
            };
        }

        private string GetEntitledPartyId(object container)
        {
            return container.GetType().GetProperty("EntitledPartyId").GetValue(container).ToString();
        }

        private bool IsDelegationAuthorized(DelegationEvidence evidence, string resourceName, string[] attributeName, string actionName)
        {
            if (DateTime.UtcNow < GetDateTimeFromTimestamp(evidence.NotBefore) || DateTime.UtcNow >= GetDateTimeFromTimestamp(evidence.NotOnOrAfter))
            {
                return false;
            }

            return evidence.PolicySets
                .SelectMany(policySet => policySet.Policies)
                .All(policy => policy.Rules.Any(rule => rule.Effect == "Permit")
                               && (policy.Target.Resource.Identifiers.Contains(resourceName)
                                   || policy.Target.Resource.Identifiers.Contains("*"))
                               && (attributeName.All(a => policy.Target.Resource.Attributes.Contains(a))
                                   || policy.Target.Resource.Attributes.Contains("*"))
                               && (policy.Target.Actions.Contains(actionName.ToUpper())
                                   || policy.Target.Actions.Contains("*")));
        }

        private DateTime GetDateTimeFromTimestamp(int timestamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.AddSeconds(timestamp);
        }

        private DelegationEvidence GetDelegationEvidence(JwtSecurityToken token)
        {
            return JsonConvert.DeserializeObject<DelegationEvidence>(
                Enumerable.FirstOrDefault<Claim>(token.Claims, c => c.Type == "delegationEvidence").Value);
        }
    }
}
