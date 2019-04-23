using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace iSHARE.Api.Swagger
{
    public class GenerateJwsFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (!context.ApiDescription.TryGetMethodInfo(out MethodInfo methodInfo))
            {
                return;
            }

            if (methodInfo.GetCustomAttributes<SwaggerOperationForPrivateKeyAttribute>().Any())
            {
                if (operation.Parameters == null)
                {
                    operation.Parameters = new List<IParameter>();
                }

                operation.Parameters.Add(new BodyParameter
                {
                    In = "body",
                    Name = "RSA private key",
                    Description = "PEM-format RSA private key that will sign the JWS assertion. The key is discarded once the operation has completed. MUST NOT be encrypted. The body of the request MUST start with -----BEGIN RSA PRIVATE KEY----- and end with -----END RSA PRIVATE KEY-----",
                    Required = true
                });
            }
        }
    }
}
