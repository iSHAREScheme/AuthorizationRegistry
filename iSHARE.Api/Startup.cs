using System.Reflection;
using iSHARE.Api.ApplicationInsights;
using iSHARE.Api.Attributes;
using iSHARE.Api.Configurations;
using iSHARE.Api.Convertors;
using iSHARE.Api.Filters;
using iSHARE.Api.Swagger;
using iSHARE.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace iSHARE.Api
{
    public class Startup
    {
        protected internal string SwaggerName;
        protected internal string ApplicationInsightsName;
        protected internal string SpaScope;
        protected internal Assembly[] DocumentingAssemblies;
        protected internal string ApiVersion = "v1.9";

        public Startup(IHostingEnvironment env, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            HostingEnvironment = env;
            LoggerFactory = loggerFactory;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }
        public ILoggerFactory LoggerFactory { get; }
        public IMvcCoreBuilder MvcCoreBuilder { get; private set; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.ConfigurePartyDetailsOptions(Configuration, HostingEnvironment);
            services.AddSigning();


            MvcCoreBuilder = services.AddMvcCore(o =>
                    {
                        if (!HostingEnvironment.IsDevelopment())
                        {
                            var genericErrorMessage = "Invalid value.";
                            o.ModelBindingMessageProvider.SetValueIsInvalidAccessor((x) => genericErrorMessage);
                            o.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor((x) => genericErrorMessage);
                            o.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => genericErrorMessage);
                            o.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor((x) => genericErrorMessage);
                        }
                    })
                .AddApiExplorer() //see https://github.com/domaindrivendev/Swashbuckle.AspNetCore#swashbuckle--apiexplorer
                .AddJsonFormatters()
                .AddAuthorization()
                .AddDataAnnotations()
                .AddJsonOptions(options => { options.SerializerSettings.Converters.Add(new TrimmingConverter()); })
                ;
            services.AddJwtAuthentication(HostingEnvironment, Configuration);
            services.AddSwagger(SwaggerName, HostingEnvironment, ApiVersion, DocumentingAssemblies);
            services.AddJsonSchema();
            services.AddFileProvider(HostingEnvironment);
            services.AddApplicationInsights(ApplicationInsightsName, HostingEnvironment);

            services.AddTransient<HideLiveApiMethodAttribute>();
            services.AddTransient<HideProductionMethodAttribute>();
            services.AddTransient<ClientIdCheckFilter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLoggers(Configuration);

            app.UseExceptionHandler(HostingEnvironment);
            app.UseSwagger(SwaggerName, ApiVersion);
            app.UseMvc();
        }

        protected void ConfigureCors(IApplicationBuilder app)
        {
            app.UseCors(builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
            );
        }
    }
}
