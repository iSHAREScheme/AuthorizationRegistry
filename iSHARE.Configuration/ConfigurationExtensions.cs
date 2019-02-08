using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using iSHARE.Configuration.Configurations;

namespace iSHARE.Configuration
{
    public static class ConfigurationExtensions
    {
        public static TOptions ConfigureOptions<TOptions>(this IServiceCollection services,
            IConfiguration configuration,
            string sectionKey,
            ConfigurationOptionsValidator validateConfigurationOptions)
            where TOptions : class, IValidateOptions
        {
            var options = Get<TOptions>(configuration, sectionKey);

            options.Validate(validateConfigurationOptions);

            services.AddSingleton(options);

            return options;
        }

        
        public static TOptions ConfigureOptions<TOptions>(this IServiceCollection services,
            IConfiguration configuration,
            string sectionKey)
            where TOptions : class
        {
            var options = Get<TOptions>(configuration, sectionKey);

            services.AddSingleton(options);

            return options;
        }
        public static TOptions ConfigureRuntimeOptions<TOptions>(this IServiceCollection services,
            IConfiguration configuration,
            string sectionKey,
            TOptions @default)
            where TOptions : class
        {
            var options = GetOptionsFromSection<TOptions>(configuration, sectionKey) ?? @default;
            services.AddSingleton(options);
            return options;
        }


        private static TOptions Get<TOptions>(IConfiguration configuration, string sectionKey)
            where TOptions : class
        {
            var options = GetOptionsFromSection<TOptions>(configuration, sectionKey);

            if (options == null)
            {
                throw new ConfigurationException($"The section `{sectionKey}` is not present in the settings.");
            }

            return options;
        }

        private static TOptions GetOptionsFromSection<TOptions>(IConfiguration configuration, string sectionKey)
            where TOptions : class
        {
            var section = configuration.GetSection(sectionKey);
            var options = section.Get<TOptions>();
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
