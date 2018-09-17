using System;

namespace NLIP.iShare.Api.Swagger.Attributes
{
    public class SwaggerDateTimeContentDescriptor : Attribute
    {
        public bool Required { get; set; }
        public string ParameterName { get; set; }
        public string Description { get; set; }
        public SwaggerDateTimeContentDescriptor()
        {
            ParameterName = "date_time";
            Required = false;
            Description = "Date time for which the information is requested. If provided the result becomes final and therefore MUST be cacheable.";
        }
    }
}
