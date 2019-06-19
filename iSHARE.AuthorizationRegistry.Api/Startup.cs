using iSHARE.Api;
using iSHARE.AuthorizationRegistry.Core;
using iSHARE.AuthorizationRegistry.Data;
using iSHARE.AzureKeyVaultClient;
using iSHARE.Configuration.Configurations;
using iSHARE.EmailClient;
using iSHARE.EntityFramework;
using iSHARE.Identity;
using iSHARE.Identity.Api;
using iSHARE.Identity.Data;
using iSHARE.IdentityServer;
using iSHARE.IdentityServer.Data;
using iSHARE.TokenClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace iSHARE.AuthorizationRegistry.Api
{
    public class Startup : StartupSpa
    {
        public IIdentityServerBuilder IdentityServerBuilder { get; private set; }
        public Startup(IHostingEnvironment env, IConfiguration configuration, ILoggerFactory loggerFactory) : base(env, configuration, loggerFactory)
        {
            SwaggerName = "Authorization Registry";
            ApplicationInsightsName = "ar.api";
            SpaScope = "ar.api";
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            const string seedRoot = "iSHARE.AuthorizationRegistry.Api.Seed.";
            var idpOptions = services.AddSchemeOwnerIdentityProviderOptions(Configuration);
            services.AddTokenClient();
            services.AddCore(Configuration);
            services.AddEmailClient(Configuration);
            services.AddDb(Configuration, HostingEnvironment);

            IdentityServerBuilder = services.AddIdentityServer(Configuration, HostingEnvironment, LoggerFactory)
                .AddPki()
                .AddIdentityServerDb(Configuration, HostingEnvironment, $"{seedRoot}IdentityServer", typeof(Startup).Assembly)
                .AddConsumer()
                .AddSchemeOwnerValidator(Configuration, HostingEnvironment)
                ;
            services.AddDigitalSigner(Configuration, HostingEnvironment, LoggerFactory);
            services.AddDefaultTokenSignatureVerifier();


            base.ConfigureServices(services);

            services.AddDefaultSpaAuthentication(HostingEnvironment);

            if (!idpOptions.Enable)
            {
                IdentityServerBuilder.AddIdentity<AspNetUser, AspNetUserDbContext>();

                services.AddIdentityServices<AspNetUser, AspNetUserDbContext>(
                    Configuration,
                    HostingEnvironment,
                    $"{seedRoot}Identity",
                    typeof(Startup).Assembly
                );
            }
            AddSpaPolicy(idpOptions);
        }

        public override void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var idpOptions = serviceScope.ServiceProvider.GetRequiredService<SchemeOwnerIdentityProviderOptions>();
                if (!idpOptions.Enable)
                {
                    app.UseMigrations<AspNetUserDbContext>(Configuration).UseSeed<AspNetUserDbContext>(HostingEnvironment);
                }
            }
            ConfigureCors(app);

            app.UseMigrations(Configuration, HostingEnvironment);
            app.UseIdentityServer() // this calls UseAuth
               .UseIdentityServerDb(Configuration, HostingEnvironment);

            base.Configure(app, loggerFactory); // this calls UseMvc, the order needs to be UseAuth, then UseMvc

        }
    }
}
