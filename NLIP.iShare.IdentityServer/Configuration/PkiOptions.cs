using NLIP.iShare.Configuration.Configurations;
using System;

namespace NLIP.iShare.IdentityServer.Configuration
{
    public class PkiOptions : IValidateOptions
    {
        public string StoreLocation { get; set; }
        public string IAThumbprint { get; set; }
        public string CARootThumbprint { get; set; }
        public string IACertificate { get; set; }
        public string CARootCertificate { get; set; }
        public void Validate(ConfigurationOptionsValidator validateConfigurationOptions)
        {
            ValidateThumbprints(validateConfigurationOptions);

            ValidateCertificates(validateConfigurationOptions);
        }

        private void ValidateThumbprints(ConfigurationOptionsValidator validateConfigurationOptions)
        {
            var required = !IsQa(validateConfigurationOptions);            

            if (required)
            {
                ConfigurationException.AssertThumbprint(CARootThumbprint, nameof(CARootThumbprint));
                ConfigurationException.AssertThumbprint(IAThumbprint, nameof(IAThumbprint));
            }
        }

        private void ValidateCertificates(ConfigurationOptionsValidator validateConfigurationOptions)
        {
            var required = IsQa(validateConfigurationOptions);
            if (required)
            {
                ConfigurationException.AssertNotNull(IACertificate, nameof(IACertificate));
                ConfigurationException.AssertNotNull(CARootCertificate, nameof(CARootCertificate));
            }

            if (!string.IsNullOrEmpty(StoreLocation) && StoreLocation != "CurrentUser" && StoreLocation != "LocalMachine")
            {
                throw new ConfigurationException(
                    $"The value of `{nameof(StoreLocation)}` should be `CurrentUser` or `LocalMachine`.");
            }
        }

        private static bool IsQa(ConfigurationOptionsValidator validateConfigurationOptions)
        {
            return validateConfigurationOptions
                       ?.Environment
                       .StartsWith("Qa", StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}   
