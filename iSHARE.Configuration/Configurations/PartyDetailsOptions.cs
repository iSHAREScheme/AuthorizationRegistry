namespace iSHARE.Configuration.Configurations
{
    public class PartyDetailsOptions : IValidateOptions
    {
        public string ClientId { get; set; }
        public string Name { get; set; }
        public string BaseUri { get; set; }

        public bool IdPEnabled { get; set; }

        public void Validate(ConfigurationOptionsValidator validateConfigurationOptions)
        {
            ConfigurationException.AssertNotNull(ClientId, $"{nameof(PartyDetailsOptions)}.{nameof(ClientId)}");
            ConfigurationException.AssertNotNull(Name, $"{nameof(PartyDetailsOptions)}.{nameof(Name)}");
            ConfigurationException.AssertUri(BaseUri, $"{nameof(PartyDetailsOptions)}.{nameof(BaseUri)}");
        }
    }
}
