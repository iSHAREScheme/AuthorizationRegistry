using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace iSHARE.Api.Swagger
{
    public class TokenEndpointDocumentFilter : IDocumentFilter
    {
        private readonly string _title;

        public TokenEndpointDocumentFilter(string title)
        {
            _title = title;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Paths.Add("/connect/token", new OpenApiPathItem
            {
                Operations = new Dictionary<OperationType, OpenApiOperation>
                {
                    {
                        OperationType.Post, new OpenApiOperation
                        {
                            Tags = new[] {new OpenApiTag {Name = "Access Token"}},
                            Summary = "Obtains access token",
                            Description = $"Used to obtain an OAuth access token from the {_title}. " +
                                          $"The format of access_token is not defined by this specification. " +
                                          $"They are left to the {_title} and should be opaque to the Service Consumer.",
                            OperationId = "OAuth2TokenPost",
                            RequestBody = CreateRequestBody(),
                            Responses = new OpenApiResponses {{"200", CreateResponseBody()}},
                        }
                    }
                }
            });
        }

        private static OpenApiResponse CreateResponseBody() => new OpenApiResponse
        {
            Description = "Ok",
            Content = new Dictionary<string, OpenApiMediaType>()
            {
                {
                    "application/json",
                    new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                {"access_token", new OpenApiSchema {Type = "string"}},
                                {"token_type", new OpenApiSchema {Type = "string"}},
                                {"expires_in", new OpenApiSchema {Type = "number"}}
                            }
                        }
                    }
                }
            }
        };

        private static OpenApiRequestBody CreateRequestBody() => new OpenApiRequestBody
        {
            Required = true,
            Content = new Dictionary<string, OpenApiMediaType>
            {
                {
                    "application/x-www-form-urlencoded", new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                {
                                    "grant_type", new OpenApiSchema
                                    {
                                        Description =
                                            "OAuth 2.0 grant type. MUST contain “client_credentials”",
                                        Type = "string"
                                    }
                                },
                                {
                                    "scope", new OpenApiSchema
                                    {
                                        Description =
                                            "OAuth 2.0 scope. Defaults to ”iSHARE”, indicating all rights are requested. " +
                                            "Other values MAY be specified by the API owner and allow to get tokens that do not include all rights",
                                        Type = "string"
                                    }
                                },
                                {
                                    "client_id", new OpenApiSchema
                                    {
                                        Description =
                                            "OpenID Connect 1.0 client ID. Used in iSHARE for all client identification for OAuth/OpenID Connect. " +
                                            "MUST contain a valid iSHARE identifier of the Service Consumer",
                                        Type = "string"
                                    }
                                },
                                {
                                    "client_assertion_type", new OpenApiSchema
                                    {
                                        Description =
                                            "OpenID Connect 1.0 client assertion type. Used in iSHARE for all client identification for OAuth/OpenID Connect. " +
                                            "MUST contain “urn:ietf:params:oauth:client-assertion-type:jwt-bearer”",
                                        Type = "string"
                                    }
                                },
                                {
                                    "client_assertion", new OpenApiSchema
                                    {
                                        Description =
                                            "OpenID Connect 1.0 client assertion. Used in iSHARE for all client identification for OAuth/OpenID Connect. " +
                                            "MUST contain JWT token conform iSHARE specifications, signed by the client.",
                                        Type = "string"
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }
}
