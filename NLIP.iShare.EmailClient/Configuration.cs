using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLIP.iShare.Abstractions.Email;
using NLIP.iShare.Configuration;
using NLIP.iShare.Configuration.Configurations;
using System.Collections.Generic;

namespace NLIP.iShare.EmailClient
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
