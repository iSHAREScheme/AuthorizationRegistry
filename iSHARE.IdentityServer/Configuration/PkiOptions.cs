using iSHARE.Configuration.Configurations;

namespace iSHARE.IdentityServer
{
    public class PkiOptions : IValidateOptions
    {
        public string[] CertificateAuthorities { get; set; }
        public void Validate(ConfigurationOptionsValidator validateConfigurationOptions)
        {
            ConfigurationException.AssertNotNull(CertificateAuthorities, $"{nameof(PkiOptions)}.{nameof(CertificateAuthorities)}");
        }
    }
}
