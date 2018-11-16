using NLIP.iShare.Api.Swagger;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace NLIP.iShare.Api.Filters
{
    public class SwaggerSignResponseOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
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
                operation.Parameters = new List<IParameter>();
            }

            operation.Description = string.Concat(operation.Description, $" By default returns current state. Server response is an iSHARE signed JSON Web Token. Please refer to the models 'jwt_header' and 'jwt_payload_{attribute.TokenName}' which indicate what the decoded response will look like.");

            operation.Parameters.Add(new Parameter
            {
                Name = "Do-Not-Sign",
                In = "header",
                Description = "Optional iSHARE specific boolean indicating the response SHALL not be signed.",
                Type = "boolean"
            });

            operation.Responses = new Dictionary<string, Response>
            {
                {
                    "200",
                    new Response
                    {
                        Description = "OK",
                        Schema = new Schema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, Schema>
                            {
                                {
                                    attribute.TokenName,
                                    new Schema
                                    {
                                        Type = "string"
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
