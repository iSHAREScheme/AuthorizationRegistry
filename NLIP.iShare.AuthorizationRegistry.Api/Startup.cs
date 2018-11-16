using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLIP.iShare.Api;
using NLIP.iShare.AuthorizationRegistry.Api.Attributes;
using NLIP.iShare.AuthorizationRegistry.Core;
using NLIP.iShare.AuthorizationRegistry.Data;
using NLIP.iShare.AuthorizationRegistry.Data.Models;
using NLIP.iShare.EmailClient;
using NLIP.iShare.EntityFramework;
using NLIP.iShare.Identity;
using NLIP.iShare.Identity.Data;
using NLIP.iShare.IdentityServer;
using NLIP.iShare.IdentityServer.Data;
using NLIP.iShare.TokenClient;

namespace NLIP.iShare.AuthorizationRegistry.Api
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

            services.AddTokenClient();
            services.AddCore(Configuration);
            services.AddEmailClient(Configuration);
            services.AddDb(Configuration, HostingEnvironment);

            const string seedRoot = "NLIP.iShare.AuthorizationRegistry.Api.Seed.";
            IdentityServerBuilder = services.AddIdentityServer(Configuration, HostingEnvironment, LoggerFactory)
                .AddIdentityServerDb(Configuration, HostingEnvironment, $"{seedRoot}IdentityServer", typeof(Startup).Assembly)
                .AddConsumer()
                .AddIdentity<AspNetUser, AspNetUserDbContext>()
                ;

            services.AddIdentityServices<AspNetUser, AspNetUserDbContext>(
                Configuration,
                HostingEnvironment,
                $"{seedRoot}Identity",
                typeof(Startup).Assembly
            );

            services.ConfigureSwaggerGen(c => c.OperationFilter<SwaggerAuthorizeDelegationRequestFilter>());

            base.ConfigureServices(services); // this needs to be called after .AddIdentity<TUser, TContext>
                                              // since it configures the JwtAuth and will make that as the default challenge
                                              // see https://stackoverflow.com/a/45853589/782754
        }

        public override void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {

            app.UseMigrations(Configuration, HostingEnvironment);
            app.UseMigrations<AspNetUserDbContext>(Configuration).UseSeed<AspNetUserDbContext>(HostingEnvironment);
            app.UseIdentityServer() // this calls UseAuth
               .UseIdentityServerDb(Configuration, HostingEnvironment);

            base.Configure(app, loggerFactory); // this calls UseMvc, the order needs to be UseAuth, then UseMvc

        }
    }
}