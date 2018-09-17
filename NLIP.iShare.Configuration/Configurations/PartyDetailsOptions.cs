using System.Linq;

namespace NLIP.iShare.Configuration.Configurations
{
    public class PartyDetailsOptions : IValidatableOptions
    {
        public string PrivateKey { get; set; }
        public string ClientId { get; set; }
        public string BaseUri { get; set; }
        public string Thumbprint { get; set; }
        public string[] PublicKeys { get; set; }
        public void Validate(ConfigurationOptionsValidator validateConfigurationOptions)
        {
            var required = validateConfigurationOptions?.Environment == "Prod";
            ConfigurationException.AssertNotNull(PrivateKey, nameof(PrivateKey));
            ConfigurationException.AssertNotNull(ClientId, nameof(ClientId));
            ConfigurationException.AssertThumbprint(Thumbprint, nameof(Thumbprint), required);
            ConfigurationException.AssertUri(BaseUri, nameof(BaseUri));

            if (!PublicPrivatePairValidator.ValidateCryptographicPair(PublicKeys.FirstOrDefault(), PrivateKey))
            {
                throw new ConfigurationException($"The private/public pair is not valid because the signing/verification failed.");
            }

        }
    }
}
