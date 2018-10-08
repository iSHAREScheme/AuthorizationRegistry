using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLIP.iShare.Configuration.Configurations;

namespace NLIP.iShare.Configuration
{
    public static class ConfigurationExtensions
    {
        public static TOptions ConfigureOptions<TOptions>(this IServiceCollection services,
            IConfiguration configuration,
            string sectionKey,
            ConfigurationOptionsValidator validateConfigurationOptions = null)
            where TOptions : class, IValidateOptions
        {
            var section = configuration.GetSection(sectionKey);
            var options = section.Get<TOptions>();

            if (options == null)
            {
                throw new ConfigurationException($"The section `{sectionKey}` is not present in the settings.");
            }

            options.Validate(validateConfigurationOptions);

            services.AddSingleton(options);

            return options;
        }

        public static PartyDetailsOptions ConfigurePartyDetailsOptions(this IServiceCollection services,
            IConfiguration configuration, IHostingEnvironment environment)
        {
            return services.ConfigureOptions<PartyDetailsOptions>(configuration, "PartyDetails", new ConfigurationOptionsValidator
            {
                Environment = environment.EnvironmentName
            });
        }
    }
}
