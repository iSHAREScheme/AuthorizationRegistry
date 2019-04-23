using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;


namespace iSHARE.Api.Swagger
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
