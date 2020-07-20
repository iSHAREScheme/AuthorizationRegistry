using System;
using System.Reflection;
using iSHARE.Api.ApplicationInsights;
using iSHARE.Api.Attributes;
using iSHARE.Api.Configurations;
using iSHARE.Api.Convertors;
using iSHARE.Api.Filters;
using iSHARE.Api.Swagger;
using iSHARE.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace iSHARE.Api
{
    public class Startup
    {
        private static readonly string AllowedCorsPolicy = "AllowedCorsPolicy";

        protected internal string SwaggerName;
        protected internal string ApplicationInsightsName;
        protected internal string SpaScope;
        protected internal Assembly[] DocumentingAssemblies;
        protected internal string ApiVersion = "v1.9";

        public Startup(IWebHostEnvironment env, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            HostingEnvironment = env;
            LoggerFactory = loggerFactory;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; }
        public ILoggerFactory LoggerFactory { get; }
        public IMvcCoreBuilder MvcCoreBuilder { get; private set; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.ConfigurePartyDetailsOptions(Configuration, HostingEnvironment);
            services.AddSigning();

            services.AddCors(options => options.AddPolicy(AllowedCorsPolicy, BuildCorsPolicy()));

            MvcCoreBuilder = services.AddMvcCore(o =>
                {
                    if (HostingEnvironment.IsDevelopment())
                    {
                        return;
                    }

                    var genericErrorMessage = "Invalid value.";
                    o.ModelBindingMessageProvider.SetValueIsInvalidAccessor((x) => genericErrorMessage);
                    o.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor((x) => genericErrorMessage);
                    o.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => genericErrorMessage);
                    o.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor(
                        x => genericErrorMessage);
                })
                .ConfigureApiBehaviorOptions(o => o.SuppressMapClientErrors = true)
                .AddApiExplorer() //see https://github.com/domaindrivendev/Swashbuckle.AspNetCore#swashbuckle--apiexplorer
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new TrimmingConverter());
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                })
                .AddAuthorization()
                .AddDataAnnotations();

            services.AddJwtAuthentication(HostingEnvironment, Configuration);
            services.AddSwagger(SwaggerName, HostingEnvironment, ApiVersion, DocumentingAssemblies);
            services.AddJsonSchema();
            services.AddFileProvider(HostingEnvironment);
            services.AddApplicationInsights(ApplicationInsightsName);
            services.AddApplicationInsightsTelemetry();

            services.AddTransient<HideLiveApiMethodAttribute>();
            services.AddTransient<HideProductionMethodAttribute>();
            services.AddTransient<ClientIdCheckFilter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseAuthorization();
            app.UseExceptionHandler(HostingEnvironment);
            app.UseSwagger(SwaggerName, ApiVersion);
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        protected void ConfigureCors(IApplicationBuilder app)
        {
            app.UseCors(AllowedCorsPolicy);
        }

        private Action<CorsPolicyBuilder> BuildCorsPolicy()
        {
            return builder =>
            {
                builder.AllowAnyHeader().AllowAnyMethod();
                if (HostingEnvironment.IsDevelopment())
                {
                    builder
                        .WithOrigins(
                            "http://localhost:4200",
                            "http://localhost:4201",
                            "http://localhost:4202",
                            "http://localhost:4203")
                        .AllowCredentials();
                }
                else
                {
                    builder.AllowAnyOrigin();
                }
            };
        }
    }
}
