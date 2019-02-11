using iSHARE.Configuration.Configurations;

namespace iSHARE.IdentityServer
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
            ValidateThumbprints();

            ValidateStoreLocation();

            ValidateCertificates();
        }

        private void ValidateThumbprints()
        {
            if (!string.IsNullOrEmpty(CARootThumbprint))
            {
                ConfigurationException.AssertThumbprint(CARootThumbprint, $"{nameof(PkiOptions)}.{nameof(CARootThumbprint)}");
            }

            if (!string.IsNullOrEmpty(IAThumbprint))
            {
                ConfigurationException.AssertThumbprint(IAThumbprint, $"{nameof(PkiOptions)}.{nameof(IAThumbprint)}");
            }
        }

        private void ValidateCertificates()
        {
            if (!string.IsNullOrEmpty(IACertificate))
            {
                ConfigurationException.AssertNotNull(IACertificate, $"{nameof(PkiOptions)}.{nameof(IACertificate)}");
            }

            if (!string.IsNullOrEmpty(CARootCertificate))
            { 
                ConfigurationException.AssertNotNull(CARootCertificate, $"{nameof(PkiOptions)}.{nameof(CARootCertificate)}");
            }
        }

        private void ValidateStoreLocation()
        {
            var usesThumbprints = !string.IsNullOrEmpty(CARootThumbprint) || !string.IsNullOrEmpty(IAThumbprint);
            if (usesThumbprints 
                    && !string.IsNullOrEmpty(StoreLocation)
                    && !(StoreLocation == "CurrentUser" || StoreLocation == "LocalMachine"))
            {
                throw new ConfigurationException(
                    $"The value of `{nameof(PkiOptions)}.{nameof(StoreLocation)}` should be `CurrentUser` or `LocalMachine`, " +
                    $"but it was { StoreLocation }.");
            }
        }
    }
}
