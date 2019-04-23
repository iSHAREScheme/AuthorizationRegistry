using System.Collections.Generic;
using iSHARE.Abstractions.Email;
using iSHARE.Configuration;
using iSHARE.Configuration.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iSHARE.EmailClient
{
    public static class Configuration
    {
        public static void AddEmailClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureOptions<EmailOptions>(configuration, "Email", ConfigurationOptionsValidator.NullValidator);

            services.AddSingleton<ITemplateService, TemplateService>();
            services.AddSingleton<IEmailClient, EmailClient>();
            services.AddTransient(srv =>
            {
                var partyDetailsOptions = services
                    .BuildServiceProvider()
                    .GetRequiredService<PartyDetailsOptions>();
                return new EmailTemplatesData
                {
                    EmailData = new Dictionary<string, string>
                    {
                        { "LogoUrl", partyDetailsOptions.BaseUri + "images/ishare_logo.png"}
                    }
                };
            });
        }
    }
}
