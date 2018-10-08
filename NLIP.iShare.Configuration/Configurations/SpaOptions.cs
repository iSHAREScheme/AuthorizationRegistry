namespace NLIP.iShare.Configuration.Configurations
{
    public class SpaOptions : IValidateOptions
    {
        public string BaseUri { get; set; }
        public void Validate(ConfigurationOptionsValidator validateConfigurationOptions)
        {
            ConfigurationException.AssertUri(BaseUri, nameof(BaseUri));
        }
    }
}
