using System.Linq;

namespace NLIP.iShare.Configuration.Configurations
{
    public class PartyDetailsOptions : IValidateOptions
    {
        public string PrivateKey { get; set; }
        public string ClientId { get; set; }
        public string Name { get; set; }
        public string BaseUri { get; set; }
        public string Thumbprint { get; set; }
        public string[] PublicKeys { get; set; }

        public void Validate(ConfigurationOptionsValidator validateConfigurationOptions)
        {
            var required = validateConfigurationOptions?.Environment == "Live";
            ConfigurationException.AssertNotNull(PrivateKey, $"{nameof(PartyDetailsOptions)}.{nameof(PrivateKey)}");
            ConfigurationException.AssertNotNull(ClientId, $"{nameof(PartyDetailsOptions)}.{nameof(ClientId)}");
            ConfigurationException.AssertNotNull(Name, $"{nameof(PartyDetailsOptions)}.{nameof(Name)}");
            ConfigurationException.AssertThumbprint(Thumbprint, $"{nameof(PartyDetailsOptions)}.{nameof(Thumbprint)}", required);
            ConfigurationException.AssertUri(BaseUri, $"{nameof(PartyDetailsOptions)}.{nameof(BaseUri)}");

            if (!PublicPrivatePairValidator.ValidateCryptographicPair(PublicKeys.FirstOrDefault(), PrivateKey))
            {
                throw new ConfigurationException($"The private/public pair is not valid because the signing/verification failed.");
            }

        }
    }
}
