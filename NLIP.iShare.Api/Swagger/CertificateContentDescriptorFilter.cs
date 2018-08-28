using NLIP.iShare.Api.Swagger.Attributes;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace NLIP.iShare.Api.Swagger
{
    public class CertificateContentDescriptorFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<IParameter>();
            }
            var attribute = context
                .ApiDescription.ActionAttributes()
                .FirstOrDefault(x => x is SwaggerCertificateContentDescriptor) as SwaggerCertificateContentDescriptor;
            if (attribute == null)
            {
                return;
            }
            var parameter = new BodyParameter
            {
                Name = attribute.ParameterName,
                In = "body",
                Required = attribute.Required,
                Description = attribute.Description,
                Schema = new Schema { Type = "string" }
            };
            operation.Parameters.Add(parameter);
        }
    }
}
