namespace NLIP.iShare.Configuration.Configurations
{
    public class SpaOptions : IValidateOptions
    {
        public string BaseUri { get; set; }
        public string ApplicationName { get; set; }
        public bool? TwoFactorEnabled { get; set; }
        public void Validate(ConfigurationOptionsValidator validateConfigurationOptions)
        {
            ConfigurationException.AssertUri(BaseUri, $"{nameof(SpaOptions)}.{nameof(BaseUri)}");
            ConfigurationException.AssertNotNull(ApplicationName, $"{nameof(SpaOptions)}.{nameof(ApplicationName)}");
            ConfigurationException.AssertNotNull(TwoFactorEnabled, $"{nameof(SpaOptions)}.{nameof(TwoFactorEnabled)}");
        }
    }
}
