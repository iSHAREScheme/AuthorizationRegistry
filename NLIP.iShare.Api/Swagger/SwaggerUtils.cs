using System;

namespace NLIP.iShare.Api.Swagger
{
    internal static class SwaggerUtils
    {
        public static string NormalizeModelName(string modelName) 
            => modelName.Replace("ViewModel", "", StringComparison.CurrentCultureIgnoreCase);
    }
}
