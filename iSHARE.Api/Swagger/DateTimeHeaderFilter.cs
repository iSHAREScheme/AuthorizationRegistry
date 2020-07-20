using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace iSHARE.Api.Swagger
{
    public class DateTimeHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "date_time",
                In = ParameterLocation.Query,
                Description = "Date time for which the information is requested. If provided the result becomes final and therefore MUST be cacheable.",
                Schema = new OpenApiSchema { Type = "string" }
            });
        }
    }
}
