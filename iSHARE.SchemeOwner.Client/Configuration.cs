using System;
using iSHARE.Configuration;
using iSHARE.Configuration.Configurations;
using iSHARE.TokenClient;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iSHARE.SchemeOwner.Client
{
    public static class Configuration
    {
        public static IServiceCollection AddSchemeOwnerClient(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            var options = services.ConfigureOptions<SchemeOwnerClientOptions>(configuration,
                "SchemeOwner",
                new ConfigurationOptionsValidator
                {
                    Environment = environment.EnvironmentName
                });

            services.AddTokenClient(new TokenSource
            {
                BaseUri = new Uri(options.BaseUri),
                Thumbprint = options.Thumbprint
            });

            services.AddTransient<ISchemeOwnerClient, SchemeOwnerClient>();

            return services;
        }
    }
}
