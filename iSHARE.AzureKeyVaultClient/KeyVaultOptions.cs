
using iSHARE.Configuration.Configurations;

namespace iSHARE.AzureKeyVaultClient
{
    internal class KeyVaultOptions : IValidateOptions
    {
        public string KeyVaultUri { get; set; }
        public string PublicKeySecretName { get; set; }
        public string PrivateKeyName { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public string KeyIdentifier => KeyVaultUri + "keys/" + PrivateKeyName;

        public void Validate(ConfigurationOptionsValidator validateConfigurationOptions)
        {
            ConfigurationException.AssertUri(KeyVaultUri, $"{nameof(KeyVaultOptions)}.{nameof(KeyVaultUri)}");
            ConfigurationException.AssertNotNull(PublicKeySecretName, $"{nameof(KeyVaultOptions)}.{nameof(PublicKeySecretName)}");
            ConfigurationException.AssertNotNull(PrivateKeyName, $"{nameof(KeyVaultOptions)}.{nameof(PrivateKeyName)}");
        }
    }
}
