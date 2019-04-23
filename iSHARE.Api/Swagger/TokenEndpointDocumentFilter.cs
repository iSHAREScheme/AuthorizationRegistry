using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;
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

        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Paths.Add("/connect/token", new PathItem
            {
                Post = new Operation
                {
                    Tags = new[] { "Access Token" },
                    Summary = "Obtains access token",
                    Description = $"Used to obtain an OAuth access token from the {_title}. " +
                                  $"The format of access_token is not defined by this specification. " +
                                  $"They are left to the {_title} and should be opaque to the Service Consumer.",
                    OperationId = "OAuth2TokenPost",
                    Parameters = new List<IParameter>
                    {
                        new Parameter
                        {
                            Name = "grant_type",
                            Description = "OAuth 2.0 grant type. MUST contain “client_credentials”",
                            In = "formData",
                            Required = true
                        },
                        new Parameter
                        {
                            Name = "scope",
                            Description = "OAuth 2.0 scope. Defaults to ”iSHARE”, indicating all rights are requested. " +
                                          "Other values MAY be specified by the API owner and allow to get tokens that do not include all rights",
                            In = "formData",
                            Required = true
                        },
                        new Parameter
                        {
                            Name = "client_id",
                            Description = "OpenID Connect 1.0 client ID. Used in iSHARE for all client identification for OAuth/OpenID Connect. " +
                                          "MUST contain a valid iSHARE identifier of the Service Consumer",
                            In = "formData",
                            Required = true
                        },
                        new Parameter
                        {
                            Name = "client_assertion_type",
                            Description = "OpenID Connect 1.0 client assertion type. Used in iSHARE for all client identification for OAuth/OpenID Connect. " +
                                          "MUST contain “urn:ietf:params:oauth:client-assertion-type:jwt-bearer”",
                            In = "formData",
                            Required = true
                        },
                        new Parameter
                        {
                            Name = "client_assertion",
                            Description = "OpenID Connect 1.0 client assertion. Used in iSHARE for all client identification for OAuth/OpenID Connect. " +
                                          "MUST contain JWT token conform iSHARE specifications, signed by the client.",
                            In = "formData",
                            Required = true
                        }
                    },
                    Responses = new Dictionary<string, Response>
                    {
                        {
                            "200", _tokenResponse
                        }
                    }
                }
            });
        }

        private readonly Response _tokenResponse = new Response
        {
            Description = "Ok",
            Schema = new Schema
            {
                Type = "object",
                Properties = new Dictionary<string, Schema>
                {
                    {
                        "access_token",
                        new Schema
                        {
                            Type = "string"
                        }
                    },
                    {
                        "token_type",
                        new Schema
                        {
                            Type = "string"
                        }
                    },
                    {
                        "expires_in",
                        new Schema
                        {
                            Type = "number"
                        }
                    }
                }
            }
        };
    }
}
