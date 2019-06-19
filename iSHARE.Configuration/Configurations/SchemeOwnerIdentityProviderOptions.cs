namespace iSHARE.Configuration.Configurations
{
    public class SchemeOwnerIdentityProviderOptions : IValidateOptions
    {
        public bool Enable { get; set; }
        public string PublicKey { get; set; }
        public string AuthorityUrl { get; set; }
        public string Scope { get; set; }
        public void Validate(ConfigurationOptionsValidator validateConfigurationOptions)
        {
            if (Enable)
            {
                ConfigurationException.AssertUri(AuthorityUrl, $"{nameof(SchemeOwnerIdentityProviderOptions)}.{nameof(AuthorityUrl)}");
                ConfigurationException.AssertNotNull(Scope, $"{nameof(SchemeOwnerIdentityProviderOptions)}.{nameof(Scope)}");
            }
        }
    }
}
