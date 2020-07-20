using System;
using System.Linq;
using iSHARE.Abstractions;

namespace iSHARE.Tests.Common
{
    public static class TestUtils
    {
        public static string GetDelegationEvidence(string audience,
            string policyIssuer,
            string containerId,
            string[] attributes,
            string serviceProvider,
            string[] actions,
            string effect)
        {
            var delegationEvidence = $@"
            ""delegationEvidence"": {{
                ""notBefore"": 1529574873,
                ""notOnOrAfter"": 1599574873,
                ""policyIssuer"": ""{ policyIssuer }"",
                ""target"": {{
                    ""accessSubject"": ""{ TestData.AbcTrucking.ClientId }""
                }},
                ""policySets"": [
                    {{
                    ""maxDelegationDepth"": 5,
                    ""target"": {{
                        ""environment"": {{
                        ""licenses"": [""ISHARE.0001""]
                        }}
                    }},
                    ""policies"": [
                        {{
                        ""target"": {{
                            ""resource"": {{
                                ""type"": ""GS1.CONTAINER"",
                                ""identifiers"": [""{ containerId }""],
                                ""attributes"": [ {ArrayBody(attributes) } ]
                            }},
                            ""actions"": [ { ArrayBody(actions) }],
                            ""environment"": {{
                                         ""serviceProviders"": [""{ serviceProvider }""]
                                    }}
                        }},
                        ""rules"": [
                            {{
                            ""effect"": ""{ effect }""
                            }}
                        ]
                        }}
                    ]
                    }}
                ]
                }}";

            var delegationEvidenceToken = $@"{{
                            ""alg"": ""RS256"",
                            ""typ"": ""JWT"",
                            ""x5c"": [""{ TestData.AuthorizationRegistry.PublicKey.ConvertToBase64Der() }""]
                          }}
                          .
                          {{
                            ""iss"": ""{ TestData.AuthorizationRegistry.ClientId }"",
                            ""sub"": ""{ TestData.AbcTrucking.ClientId }"",
                            ""aud"": ""{ audience }"",
                            ""jti"": ""{ Guid.NewGuid():N}"",
                            ""exp"": ""{ DateTime.UtcNow.AddSeconds(30).ToEpoch() }"",
                            ""iat"": ""{ DateTime.UtcNow.ToEpoch() }"",
                            ""nbf"": ""{ DateTime.UtcNow.ToEpoch() }"",
                            { delegationEvidence }

                          }}".SignJwt(TestData.AuthorizationRegistry.PrivateKey);

            return delegationEvidenceToken;
        }

        private static string ArrayBody(string[] actions) => string.Join(", ", actions.Select(a => "\"" + a + "\""));

        public static string ConnectTokenEndpoint(string baseUri) => $"{baseUri.TrimEnd('/')}/connect/token";
    }
}
