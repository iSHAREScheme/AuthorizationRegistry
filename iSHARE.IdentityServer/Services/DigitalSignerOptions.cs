using iSHARE.Abstractions;
using iSHARE.Configuration.Configurations;

namespace iSHARE.IdentityServer.Services
{
    public class DigitalSignerOptions : IValidateOptions
    {
        public string PrivateKey { get; set; }
        public string RawPublicKey { get; set; }
        public string PublicKey => RawPublicKey.ConvertToBase64Der();
        public void Validate(ConfigurationOptionsValidator validateConfigurationOptions)
        {
            ConfigurationException.AssertNotNull(RawPublicKey, $"{nameof(DigitalSignerOptions)}.{nameof(RawPublicKey)}");
            ConfigurationException.AssertNotNull(PrivateKey, $"{nameof(DigitalSignerOptions)}.{nameof(PrivateKey)}");
        }
    }
}
