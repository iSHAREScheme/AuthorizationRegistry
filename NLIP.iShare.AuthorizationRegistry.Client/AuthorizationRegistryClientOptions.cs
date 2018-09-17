using NLIP.iShare.Configuration.Configurations;

namespace NLIP.iShare.AuthorizationRegistry.Client
{
    public class AuthorizationRegistryClientOptions: IValidatableOptions
    {
        public string BaseUri { get; set; }
        public string Thumbprint { get; set; }
        public void Validate(ConfigurationOptionsValidator validateConfigurationOptions)
        {
            var required = validateConfigurationOptions?.Environment == "Prod";
            ConfigurationException.AssertThumbprint(Thumbprint, nameof(Thumbprint), required: required);
            ConfigurationException.AssertUri(BaseUri, nameof(BaseUri));
        }
    }
}
