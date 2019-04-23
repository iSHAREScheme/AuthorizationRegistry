using iSHARE.Configuration;
using iSHARE.Configuration.Configurations;

namespace iSHARE.AuthorizationRegistry.Client
{
    public class AuthorizationRegistryClientOptions : IValidateOptions
    {
        public string BaseUri { get; set; }
        public string Thumbprint { get; set; }
        public string ClientId { get; set; }
        public void Validate(ConfigurationOptionsValidator validateConfigurationOptions)
        {
            var required = validateConfigurationOptions?.Environment == Environments.Live;
            ConfigurationException.AssertThumbprint(Thumbprint, $"{nameof(AuthorizationRegistryClientOptions)}.{nameof(Thumbprint)}", required: required);
            ConfigurationException.AssertUri(BaseUri, $"{nameof(AuthorizationRegistryClientOptions)}.{nameof(BaseUri)}");
            ConfigurationException.AssertNotNull(ClientId, $"{nameof(AuthorizationRegistryClientOptions)}.{nameof(ClientId)}");
        }
    }
}
