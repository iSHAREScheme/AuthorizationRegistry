namespace iSHARE.Configuration.Configurations
{
    public class SpaOptions : IValidateOptions
    {
        public string BaseUri { get; set; }
        public string ApplicationName { get; set; }
        public bool? TwoFactorEnabled { get; set; }
        public string SpaClientId { get; set; }
        public string SpaClientSecret { get; set; }
        public string ApiName { get; set; }
        public string ApiSecret { get; set; }
        public void Validate(ConfigurationOptionsValidator validateConfigurationOptions)
        {
            ConfigurationException.AssertUri(BaseUri, $"{nameof(SpaOptions)}.{nameof(BaseUri)}");
            ConfigurationException.AssertNotNull(ApplicationName, $"{nameof(SpaOptions)}.{nameof(ApplicationName)}");
            ConfigurationException.AssertNotNull(TwoFactorEnabled, $"{nameof(SpaOptions)}.{nameof(TwoFactorEnabled)}");
            ConfigurationException.AssertNotNull(SpaClientId, $"{nameof(SpaOptions)}.{nameof(SpaClientId)}");
            ConfigurationException.AssertNotNull(SpaClientSecret, $"{nameof(SpaOptions)}.{nameof(SpaClientSecret)}");
            ConfigurationException.AssertNotNull(ApiName, $"{nameof(SpaOptions)}.{nameof(ApiName)}");
            ConfigurationException.AssertNotNull(ApiSecret, $"{nameof(SpaOptions)}.{nameof(ApiSecret)}");
        }
    }
}
