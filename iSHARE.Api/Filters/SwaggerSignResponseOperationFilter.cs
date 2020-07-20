using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace iSHARE.Api.Filters
{
    public class SwaggerSignResponseOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var filterDescriptors = context.ApiDescription.ActionDescriptor.FilterDescriptors;

            var descriptor = filterDescriptors.FirstOrDefault(f => (f.Filter is SignResponseAttribute));

            if (descriptor == null)
            {
                return;
            }

            var attribute = descriptor.Filter as SignResponseAttribute;

            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Description = string.Concat(operation.Description,
                $" By default returns current state. Server response is an iSHARE signed JSON Web Token. Please refer to the models 'jwt_header' and 'jwt_payload_{attribute.TokenName}' which indicate what the decoded response will look like.");

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Do-Not-Sign",
                In = ParameterLocation.Header,
                Description = "Optional iSHARE specific boolean indicating the response SHALL not be signed.",
                Schema = new OpenApiSchema { Type = "boolean" }
            });

            operation.Responses = new OpenApiResponses
            {
                {
                    "200",
                    new OpenApiResponse
                    {
                        Description = "OK",
                        Content = new Dictionary<string, OpenApiMediaType>()
                        {
                            { "text/plain", CreateResponse(attribute.TokenName) },
                            { "application/json", CreateResponse(attribute.TokenName) },
                            { "text/json", CreateResponse(attribute.TokenName) }
                        }
                    }
                }
            };
        }

        private static OpenApiMediaType CreateResponse(string tokenName)
        {
            return new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        { tokenName, new OpenApiSchema { Type = "string" }}
                    }
                }
            };
        }
    }
}
