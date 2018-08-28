using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace NLIP.iShare.Api.Swagger
{
    public class TokenEndpointDocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Paths.Add("/connect/token", new PathItem
            {
                Post = new Operation
                {
                    Tags = new [] { "Access Token" },
                    Summary = "Obtains access token",
                    OperationId = "OAuth2TokenPost",
                    Parameters = new List<IParameter>
                    {
                        new Parameter
                        {
                            Name = "grant_type",
                            Description = "OAuth 2.0 grant type. MUST contain “client_credentials”",
                            In = "formData",
                            Type = "string",
                            Required = true
                        },new Parameter
                        {
                            Name = "scope",
                            Description = "OAuth 2.0 scope. Defaults to ”iSHARE”, indicating all rights are requested. " +
                                          "Other values MAY be specified by the API owner and allow to get " +
                                          "tokens that do not include all rights",
                            In = "formData",
                            Type = "string"
                        },
                        new Parameter
                        {
                            Name = "client_id",
                            Description = "OpenID Connect 1.0 client ID. Used in iSHARE for all client identification " +
                                          "for OAuth/OpenID Connect. MUST contain a valid iSHARE identifier " +
                                          "of the Service Consumer",
                            In = "formData",
                            Type = "string",
                            Required = true
                        },
                        new Parameter
                        {
                            Name = "client_assertion_type",

                            Description = "OpenID Connect 1.0 client assertion type. Used in iSHARE for all " +
                                          "client identification for OAuth/OpenID Connect. " +
                                          "MUST contain “urn:ietf:params:oauth:client-assertion-type:jwt-bearer”",
                            In = "formData",
                            Type = "string",
                            Required = true
                        },
                        new Parameter
                        {
                            Name = "client_assertion",
                            Description = "OpenID Connect 1.0 client assertion. Used in iSHARE for all " +
                                          "client identification for OAuth/OpenID Connect. " +
                                          "MUST contain JWT token conform iSHARE specifications, signed by the client.",
                            In = "formData",
                            Type = "string",
                            Required = true
                        }
                    },
                    Responses = new Dictionary<string, Response>
                    {
                        {
                            "200",
                            new Response
                            {
                                Description = "Success",
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
                                                Type = "string"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            });
        }
    }
}
