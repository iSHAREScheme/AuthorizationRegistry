using iSHARE.Configuration;
using iSHARE.Configuration.Configurations;

namespace iSHARE.SchemeOwner.Client
{
    public class SchemeOwnerClientOptions : IValidateOptions
    {
        public string BaseUri { get; set; }
        public string Thumbprint { get; set; }
        public string ClientId { get; set; }
        public void Validate(ConfigurationOptionsValidator validateConfigurationOptions)
        {
            var required = validateConfigurationOptions?.Environment == Environments.Live;
            ConfigurationException.AssertThumbprint(Thumbprint, $"{nameof(SchemeOwnerClientOptions)}.{nameof(Thumbprint)}", required: required);
            ConfigurationException.AssertNotNull(ClientId, $"{nameof(SchemeOwnerClientOptions)}.{nameof(ClientId)}");
            ConfigurationException.AssertUri(BaseUri, $"{nameof(SchemeOwnerClientOptions)}.{nameof(BaseUri)}");
        }
    }
}
