using Manatee.Json;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLIP.iShare.Api;
using NLIP.iShare.Api.ApplicationInsights;
using NLIP.iShare.Api.Swagger;
using NLIP.iShare.AuthorizationRegistry.Core;
using NLIP.iShare.AuthorizationRegistry.Data;
using NLIP.iShare.AuthorizationRegistry.IdentityServer;
using NLIP.iShare.EmailClient;
using NLIP.iShare.SchemeOwner.Client;
using NLIP.iShare.TokenClient;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;

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
            services.AddAuthorizationRegistryCore(Configuration);
            services.AddCors();

            services.AddTokenClient(new[]
            {
                new TokenSource
                {
                    BaseUri = new Uri(Configuration["SchemeOwner:BaseUri"]),
                    Thumbprint = Configuration["SchemeOwner:Thumbprint"]
                }
            });            
            services.AddSchemeOwnerClient();
            services.AddAuthorizationRegistryUserDbContext(Configuration, HostingEnvironment);
            services.AddIdentityServerForAuthorizationRegistry(Configuration, HostingEnvironment, LoggerFactory);

            services.AddMvcCore()
                .AddApiExplorer() //see https://github.com/domaindrivendev/Swashbuckle.AspNetCore#swashbuckle--apiexplorer
                .AddCustomJsonSettings()
                .AddAuthorization()
                .AddDataAnnotations()
                ;

            services.AddJwtAuthentication(Configuration, HostingEnvironment, new string[] { "ar.api", "iSHARE" });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Authorization Registry", Version = "v1" });
                c.DocumentFilter<TokenEndpointDocumentFilter>();
                c.OperationFilter<AuthorizationHeaderFilter>();
            });
            services.AddEmailClient(Configuration, LoggerFactory);

            services.AddSingleton<IJsonSchema>(srv => {
                var schemaJson = JsonValue.Parse(File.ReadAllText("delegationMaskSchema.json"));
                return new JsonSerializer().Deserialize<IJsonSchema>(schemaJson);
            });

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
            app.UseCustomResponseCaching(Configuration, HostingEnvironment);

            loggerFactory.AddFile("App_Data/Logs/iSHARE-{Date}.log");
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCustomExceptionHandler(HostingEnvironment);
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Authorization Registry");
            });

            app.UseIdentityServerServiceAuthorizationRegistry(Configuration, HostingEnvironment);
            app.UseAuthorizationRegistryStore(Configuration, HostingEnvironment);
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMvc();

        }
    }
}