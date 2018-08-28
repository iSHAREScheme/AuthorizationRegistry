using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLIP.iShare.Abstractions.Email;
using NLIP.iShare.EmailClient.Models;

namespace NLIP.iShare.EmailClient
{
    public static class Configuration
    {
        public static void AddEmailClient(this IServiceCollection services, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            services.AddSingleton<IEmailClient, EmailClient>(provider => new EmailClient(
                new EmailConfiguration(
                    configuration["Email:SendGridKey"],
                    new EmailAddress(configuration["Email:From:Address"], configuration["Email:From:DisplayName"])),
                loggerFactory.CreateLogger<EmailClient>()));
        }
    }
}
