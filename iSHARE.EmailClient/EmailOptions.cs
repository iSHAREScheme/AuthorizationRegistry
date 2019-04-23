using System;
using System.Net.Mail;
using iSHARE.Abstractions.Email;
using iSHARE.Configuration.Configurations;

namespace iSHARE.EmailClient
{
    public class EmailOptions : IValidateOptions
    {
        public string SendGridKey { get; set; }
        public EmailAddress From { get; set; }
        public void Validate(ConfigurationOptionsValidator validateConfigurationOptions)
        {
            ConfigurationException.AssertNotNull(SendGridKey, $"{nameof(EmailOptions)}.{nameof(SendGridKey)}");

            if (From == null)
            {
                throw new ConfigurationException($"Either the `{nameof(EmailOptions)}.{nameof(From)}` key was not found in the configuration or its value was not set.");
            }

            ConfigurationException.AssertNotNull(From.Address, $"{nameof(EmailOptions)}.{nameof(From)}{nameof(From.Address)}");
            ConfigurationException.AssertNotNull(From.DisplayName, $"{nameof(EmailOptions)}.{nameof(From)}{nameof(From.DisplayName)}");

            try
            {
                var address = new MailAddress(From.Address);
            }
            catch (FormatException)
            {
                throw new ConfigurationException($"The value of `{nameof(EmailOptions)}.{nameof(From)}.{nameof(From.Address)}` key must be a valid email address.");
            }
        }
    }
}
