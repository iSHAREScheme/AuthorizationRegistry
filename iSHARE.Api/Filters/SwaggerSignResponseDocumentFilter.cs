using System.Collections.Generic;
using System.Linq;
using iSHARE.Api.Swagger;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace iSHARE.Api.Filters
{
    public class SwaggerSignResponseDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var filterDescriptors = context.ApiDescriptions.SelectMany(ig => ig.ActionDescriptor.FilterDescriptors).ToArray();
            var isSignResponseAttribute = filterDescriptors.Any(fd => fd.Filter is SignResponseAttribute);

            if (!isSignResponseAttribute)
            {
                return;
            }

            var attributes = filterDescriptors.Where(fd => fd.Filter is SignResponseAttribute);

            if (swaggerDoc.Components.Schemas == null)
            {
                swaggerDoc.Components.Schemas = new Dictionary<string, OpenApiSchema>();
            }

            AddJwtHeader(swaggerDoc);

            foreach (var item in attributes)
            {
                var attribute = item.Filter as SignResponseAttribute;
                AddJwtPayload(swaggerDoc, attribute);
            }
        }

        private void AddJwtHeader(OpenApiDocument swaggerDoc)
        {
            swaggerDoc.Components.Schemas.Add(
                "jwt_header",
                new OpenApiSchema
                {
                    Type = "object",
                    Required = new HashSet<string> { "alg", "typ", "x5c" },
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        {
                            "alg",
                            new OpenApiSchema
                            {
                                Type = "string",
                                Example = new OpenApiString("RS256")
                            }
                        },
                        {
                            "typ",
                            new OpenApiSchema
                            {
                                Type = "string",
                                Example = new OpenApiString("JWT")
                            }
                        },
                        {
                            "x5c",
                            new OpenApiSchema
                            {
                                Type = "array",
                                Items = new OpenApiSchema
                                {
                                    Type = "string"
                                }
                            }
                        }
                    }
                });
        }

        private void AddJwtPayload(OpenApiDocument swaggerDoc, SignResponseAttribute attribute)
        {
            var key = "jwt_payload_" + attribute.TokenName;
            OpenApiSchema claimSchema;
            var normalizedDefinitionName = SwaggerUtils.NormalizeModelName(attribute.SwaggerDefinitionName);
            var claimDefinitionProperties = swaggerDoc.Components.Schemas[normalizedDefinitionName].Properties;

            if (attribute.ResponseContainsList)
            {
                claimSchema = new OpenApiSchema
                {
                    Type = "array",
                    Title = attribute.ClaimName,
                    Items = NewSchemaItem(attribute.SwaggerDefinitionName, claimDefinitionProperties)
                };
            }
            else
            {
                claimSchema = NewSchemaItem(attribute.ClaimName, claimDefinitionProperties);
            }

            swaggerDoc.Components.Schemas.Add(
                key,
                new OpenApiSchema
                {
                    Type = "object",
                    Required = new HashSet<string> { "iss", "sub", attribute.AnonymousUsage ? "" : "aud", "jti", "exp", "iat", attribute.ClaimName },
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        {
                            "iss",
                            new OpenApiSchema
                            {
                                Type = "string",
                                Example = new OpenApiString("EU.EORI.NL123456789")
                            }
                        },
                        {
                            "sub",
                            new OpenApiSchema
                            {
                                Type = "string",
                                Example = new OpenApiString("EU.EORI.NL123456789")
                            }
                        },
                        {
                            "aud",
                            new OpenApiSchema
                            {
                                Type = "string",
                                Example = new OpenApiString("EU.EORI.NL123456789")
                            }
                        },
                        {
                            "jti",
                            new OpenApiSchema
                            {
                                Type = "string",
                                Example = new OpenApiString("378a47c4-2822-4ca5-a49a-7e5a1cc7ea59")
                            }
                        },
                        {
                            "exp",
                            new OpenApiSchema
                            {
                                Type = "integer",
                                Example = new OpenApiString("1504683475")
                            }
                        },
                        {
                            "iat",
                            new OpenApiSchema
                            {
                                Type = "integer",
                                Example = new OpenApiString("1504683475")
                            }
                        },
                        {
                            attribute.ClaimName,
                            claimSchema
                        }
                    }
                });
        }

        private OpenApiSchema NewSchemaItem(string title, IDictionary<string, OpenApiSchema> properties)
        {
            return new OpenApiSchema
            {
                Type = "object",
                Title = title,
                Properties = properties
            };
        }
    }
}
