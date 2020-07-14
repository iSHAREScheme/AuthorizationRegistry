using iSHARE.Api;
using iSHARE.Api.Configurations;
using iSHARE.Configuration;
using iSHARE.Configuration.Configurations;
using iSHARE.Identity.Login;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace iSHARE.Identity.Api
{
    public class StartupSpa : Startup
    {
        public StartupSpa(IWebHostEnvironment env, IConfiguration configuration, ILoggerFactory loggerFactory)
            : base(env, configuration, loggerFactory)
        {

        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddHttpClient<ISessionHandleService, SessionHandleService>();
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot/dist";
            });

            services.ConfigureOptions<SpaOptions>(Configuration, "Spa", ConfigurationOptionsValidator.NullValidator);
        }

        public override void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseStaticFiles();

            base.Configure(app, loggerFactory);

            app.UseClientCaching();
            app.UseSpaStaticFiles();

            app.Map("/admin", admin =>
            {
                admin.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "wwwroot/dist";
                });
            });
        }

        protected void AddSpaPolicy(SchemeOwnerIdentityProviderOptions options)
        {
            MvcCoreBuilder.AddAuthorization(opt =>
            {
                opt.AddPolicy(SpaConstants.SpaPolicy,
                    policy =>
                    {
                        if (options.Enable)
                        {
                            policy.RequireClaim("scope", options.Scope);
                        }
                        else
                        {
                            policy.RequireAssertion(_ => true);
                        }
                    });
            });
        }
    }
}
