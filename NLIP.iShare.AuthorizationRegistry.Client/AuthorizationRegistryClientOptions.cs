using NLIP.iShare.Configuration.Configurations;

namespace NLIP.iShare.AuthorizationRegistry.Client
{
    public class AuthorizationRegistryClientOptions: IValidateOptions
    {
        public string BaseUri { get; set; }
        public string Thumbprint { get; set; }
        public string ClientId { get; set; }
        public void Validate(ConfigurationOptionsValidator validateConfigurationOptions)
        {
            var required = validateConfigurationOptions?.Environment == "Live";
            ConfigurationException.AssertThumbprint(Thumbprint, nameof(Thumbprint), required: required);
            ConfigurationException.AssertUri(BaseUri, nameof(BaseUri));
            ConfigurationException.AssertNotNull(ClientId,nameof(ClientId));
        }
    }
}
