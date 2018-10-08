using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLIP.iShare.Api;
using NLIP.iShare.Api.ApplicationInsights;
using NLIP.iShare.Api.Configurations;
using NLIP.iShare.Api.Filters;
using NLIP.iShare.AuthorizationRegistry.Core;
using NLIP.iShare.AuthorizationRegistry.Data;
using NLIP.iShare.AuthorizationRegistry.IdentityServer;
using NLIP.iShare.Configuration;
using NLIP.iShare.Configuration.Configurations;
using NLIP.iShare.EmailClient;
using NLIP.iShare.SchemeOwner.Client;

namespace NLIP.iShare.AuthorizationRegistry
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            HostingEnvironment = env;
            LoggerFactory = loggerFactory;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }
        public ILoggerFactory LoggerFactory { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot/dist";
            });

            services.ConfigurePartyDetailsOptions(Configuration, HostingEnvironment);
            services.ConfigureOptions<SpaOptions>(Configuration, "Spa");

            services.AddAuthorizationRegistryCore(Configuration);
            services.AddCors();

            services.AddSchemeOwnerClient(Configuration, HostingEnvironment);
            services.AddAuthorizationRegistryUserDbContext(Configuration, HostingEnvironment);
            services.AddIdentityServer(Configuration, HostingEnvironment, LoggerFactory);

            services.AddMvcCore()
                .AddApiExplorer() //see https://github.com/domaindrivendev/Swashbuckle.AspNetCore#swashbuckle--apiexplorer
                .AddJsonSettings()
                .AddAuthorization()
                .AddDataAnnotations()
                ;

            services.AddJwtAuthentication(HostingEnvironment, new[] { "ar.api", "iSHARE" });

            services.AddSwagger("Authorization Registry");
            services.AddEmailClient(Configuration);
            services.AddSigning();

            services.AddJsonSchema();
            services.AddFileProvider();

            services.AddApplicationInsights("ar.api", HostingEnvironment);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseCors(builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                );
            app.UseClientCaching();
            loggerFactory.AddLoggers(Configuration);
            app.UseExceptionHandler(HostingEnvironment);
            app.UseSwagger("Authorization Registry");
            app.UseIdentityServer(Configuration, HostingEnvironment);
            app.UseMigrations(Configuration, HostingEnvironment);
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseMvc();

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