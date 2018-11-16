using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace NLIP.iShare.Api.Filters
{
    public class SwaggerSignResponseDocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var filterDescriptors = context.ApiDescriptions.SelectMany(ig => ig.ActionDescriptor.FilterDescriptors);
            var isSignResponseAttribute = filterDescriptors.Any(fd => fd.Filter is SignResponseAttribute);

            if (!isSignResponseAttribute)
            {
                return;
            }

            var attributes = filterDescriptors.Where(fd => fd.Filter is SignResponseAttribute);

            if (swaggerDoc.Definitions == null)
            {
                swaggerDoc.Definitions = new Dictionary<string, Schema>();
            }

            AddJwtHeader(swaggerDoc);

            foreach (var item in attributes)
            {
                var attribute = item.Filter as SignResponseAttribute;
                AddJwtPayload(swaggerDoc, attribute);
            }
        }

        private void AddJwtHeader(SwaggerDocument swaggerDoc)
        {
            swaggerDoc.Definitions.Add(
                "jwt_header",
                new Schema
                {
                    Type = "object",
                    Required = new List<string> { "alg", "typ", "x5c" },
                    Properties = new Dictionary<string, Schema>
                    {
                        {
                            "alg",
                            new Schema
                            {
                                Type = "string",
                                Example = "RS256"
                            }
                        },
                        {
                            "typ",
                            new Schema
                            {
                                Type = "string",
                                Example = "JWT"
                            }
                        },
                        {
                            "x5c",
                            new Schema
                            {
                                Type = "array",
                                Items = new Schema
                                {
                                    Type = "string"
                                }
                            }
                        }
                    }
                });
        }

        private void AddJwtPayload(SwaggerDocument swaggerDoc, SignResponseAttribute attribute)
        {
            var key = "jwt_payload_" + attribute.TokenName;
            Schema claimSchema;
            var claimDefinitionProperties = swaggerDoc.Definitions[attribute.SwaggerDefinitionName].Properties;
            if (attribute.ResponseContainsList)
            {
                claimSchema = new Schema
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
            swaggerDoc.Definitions.Add(
                key,
                new Schema
                {
                    Type = "object",
                    Required = new[] { "iss", "sub", "aud", "jti", "exp", "iat", attribute.ClaimName },
                    Properties = new Dictionary<string, Schema>
                    {
                        {
                            "iss",
                            new Schema
                            {
                                Type = "string",
                                Example = "EU.EORI.NL123456789"
                            }
                        },
                        {
                            "sub",
                            new Schema
                            {
                                Type = "string",
                                Example = "EU.EORI.NL123456789"
                            }
                        },
                        {
                            "aud",
                            new Schema
                            {
                                Type = "string",
                                Example = "EU.EORI.NL123456789"
                            }
                        },
                        {
                            "jti",
                            new Schema
                            {
                                Type = "string",
                                Example = "378a47c4-2822-4ca5-a49a-7e5a1cc7ea59"
                            }
                        },
                        {
                            "exp",
                            new Schema
                            {
                                Type = "integer",
                                Example = "1504683475"
                            }
                        },
                        {
                            "iat",
                            new Schema
                            {
                                Type = "integer",
                                Example = "1504683475"
                            }
                        },
                        {
                            attribute.ClaimName,
                            claimSchema
                        }
                    }
                });

            
        }

        private Schema NewSchemaItem(string title, IDictionary<string, Schema> properties)
        {
            return new Schema
            {
                Type = "object",
                Title = title,
                Properties = properties
            };
        }
    }
}
