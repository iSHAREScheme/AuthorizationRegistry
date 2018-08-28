using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;


namespace NLIP.iShare.Api.Swagger
{
    public class Parameter : IParameter
    {
        public string Name { get; set; }
        public string In { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public Dictionary<string, object> Extensions { get; }
        public string Type { get; set; }
    }
}
