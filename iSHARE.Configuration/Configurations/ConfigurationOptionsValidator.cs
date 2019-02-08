namespace iSHARE.Configuration.Configurations
{
    public class ConfigurationOptionsValidator
    {
        public string Environment { get; set; }

        public static readonly ConfigurationOptionsValidator NullValidator = new ConfigurationOptionsValidator();
    }
}
