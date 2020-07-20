using System.Collections.Generic;
using System.Threading.Tasks;
using iSHARE.Abstractions;
using iSHARE.Configuration.Configurations;
using iSHARE.Models.DelegationEvidence;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;

namespace iSHARE.IdentityServer.UserInfo
{
    internal class ResponseGeneratorMock : IUserInfoResponseGenerator
    {
        private readonly IResponseJwtBuilder _responseJwtBuilder;
        private readonly PartyDetailsOptions _options;

        public ResponseGeneratorMock(IResponseJwtBuilder responseJwtBuilder, PartyDetailsOptions options)
        {
            _responseJwtBuilder = responseJwtBuilder;
            _options = options;
        }

        public async Task<string> ProcessAsync(HttpContext context)
        {
            return await Sign();
        }

        private async Task<string> Sign()
        {
            return await _responseJwtBuilder.Create(
                CreateMockResultObject(),
                "subject-GUID",
                _options.ClientId,
                "mocked-CTT-USER-ID",
                "delegation_token",
                new CamelCasePropertyNamesContractResolver());
        }

        private static DelegationEvidence CreateMockResultObject()
        {
            return new DelegationEvidence
            {
                PolicySets = new List<PolicySet>
                {
                    new PolicySet
                    {
                        Policies = new List<Policy>
                        {
                            new Policy
                            {
                                Rules = new List<PolicyRule>
                                {
                                    new PolicyRule("Deny")
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
