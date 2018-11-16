using NLIP.iShare.Configuration.Configurations;

namespace NLIP.iShare.SchemeOwner.Client
{
    public class SchemeOwnerClientOptions: IValidateOptions
    {
        public string BaseUri { get; set; }
        public string Thumbprint { get; set; }
        public void Validate(ConfigurationOptionsValidator validateConfigurationOptions)
        {
            var required = validateConfigurationOptions?.Environment == "Live";
            ConfigurationException.AssertThumbprint(Thumbprint, $"{nameof(SchemeOwnerClientOptions)}.{nameof(Thumbprint)}", required: required);
            ConfigurationException.AssertUri(BaseUri, $"{nameof(SchemeOwnerClientOptions)}.{nameof(BaseUri)}");
        }
    }
}
