using System;

namespace NLIP.iShare.Api.Swagger.Attributes
{
    public class SwaggerCertificateContentDescriptor : Attribute
    {
        public bool Required { get; set; }
        public string ParameterName { get; set; }
        public string Description { get; set; }
        public SwaggerCertificateContentDescriptor()
        {
            ParameterName = "certificate";
            Required = true;
            Description = "Provides the certificate to be validated in X.509 DER format that is base_64 encoded. Note that the entire value MUST be url encoded.";
        }
    }
}
