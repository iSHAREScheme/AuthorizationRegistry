using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLIP.iShare.Api.Configurations;
using NLIP.iShare.Configuration;
using NLIP.iShare.Configuration.Configurations;

namespace NLIP.iShare.Api
{
    public class StartupSpa : Startup
    {

        public StartupSpa(IHostingEnvironment env, IConfiguration configuration, ILoggerFactory loggerFactory)
            : base(env, configuration, loggerFactory)
        {

        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot/dist";
            });

            services.ConfigureOptions<SpaOptions>(Configuration, "Spa", ConfigurationOptionsValidator.NullValidator);
        }

        public override void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            base.Configure(app, loggerFactory);

            app.UseClientCaching();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.Map("/admin", admin =>
            {
                admin.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "wwwroot/dist";
                });
            });
        }
    }
}
