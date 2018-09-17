using NLIP.iShare.Api.Swagger.Attributes;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace NLIP.iShare.Api.Swagger
{
    public class DateTimeHeaderFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<IParameter>();
            }

            var attribute = context
                .ApiDescription
                .ControllerAttributes()
                .Union(context.ApiDescription.ActionAttributes())
                .FirstOrDefault(x => x is SwaggerDateTimeContentDescriptor) as SwaggerDateTimeContentDescriptor;

            if (attribute == null)
            {
                return;
            }

            var parameter = new Parameter
            {
                Name = attribute.ParameterName,
                In = "query",
                Required = attribute.Required,
                Description = attribute.Description,
                Type = "string"
            };
            operation.Parameters.Add(parameter);
        }
    }
}
