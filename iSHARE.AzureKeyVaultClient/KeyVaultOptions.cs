
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

        /// <summary>
        /// Signed JWTs MUST contain an array of the complete certificate chain that should be used for validating the
        /// JWT’s signature in the x5c header parameter up until an Issuing CA is listed from the iSHARE Trusted List.
        ///
        /// This property contains public secret names of base64 certificates stored in azure cloud.
        /// Names are separated by ';' symbol from intermediate to root CA (root CA is the last one).
        /// </summary>
        public string PublicKeyChainSecretNames { get; set; }

        public string KeyIdentifier => KeyVaultUri + "keys/" + PrivateKeyName;

        public void Validate(ConfigurationOptionsValidator validateConfigurationOptions)
        {
            ConfigurationException.AssertUri(KeyVaultUri, $"{nameof(KeyVaultOptions)}.{nameof(KeyVaultUri)}");
            ConfigurationException.AssertNotNull(PublicKeySecretName, $"{nameof(KeyVaultOptions)}.{nameof(PublicKeySecretName)}");
            ConfigurationException.AssertNotNull(PrivateKeyName, $"{nameof(KeyVaultOptions)}.{nameof(PrivateKeyName)}");
        }
    }
}
