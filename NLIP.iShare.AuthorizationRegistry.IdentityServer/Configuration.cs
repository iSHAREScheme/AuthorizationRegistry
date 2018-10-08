using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using NLIP.iShare.Api.Configurations;
using NLIP.iShare.AuthorizationRegistry.IdentityServer.Data;
using NLIP.iShare.AuthorizationRegistry.IdentityServer.Migrations.Seed.AspNetIdentity.Seeders;
using NLIP.iShare.AuthorizationRegistry.IdentityServer.Models;
using NLIP.iShare.Configuration.Configurations;
using NLIP.iShare.EntityFramework;
using NLIP.iShare.IdentityServer;
using NLIP.iShare.IdentityServer.Validation;
using System;
using System.Reflection;

namespace NLIP.iShare.AuthorizationRegistry.IdentityServer
{
    public static class Configuration
    {
        public static void UseIdentityServer(this IApplicationBuilder app, IConfiguration configuration,
            IHostingEnvironment environment)
        {
            app.UseIdentityServer()
                .UseIdentityServerMigrations()
                .UseSeed<ConfigurationDbContext>(environment)
                .UseMigrations<UserDbContext>(configuration, environment)
                .UseSeed<UserDbContext>(environment)
                ;
        }
        
        public static void AddIdentityServer(this IServiceCollection services,
            IConfiguration configuration, IHostingEnvironment environment, ILoggerFactory loggerFactory)
        {
            services.AddIdentityServerSeedServices(environment,
                "NLIP.iShare.AuthorizationRegistry.IdentityServer.Migrations.Seed",
                typeof(UserDbContext).GetTypeInfo().Assembly);

            services.AddAspNetIdentityDb(configuration, environment);

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromDays(1);
            });

            services.AddIdentity<AspNetUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 8;
                })
                .AddEntityFrameworkStores<UserDbContext>()
                .AddDefaultTokenProviders()
                ;

            services.AddTransient<IProfileService, ProfileService>();


            services.AddIdentityServer(options =>
                {
                    var partyDetailsOptions = services.BuildServiceProvider().GetRequiredService<PartyDetailsOptions>();
                    if (environment.IsDevelopment())
                    {
                        IdentityModelEventSource.ShowPII = true;
                    }

                    options.IssuerUri = partyDetailsOptions.BaseUri;
                })
                .AddPki(configuration, environment.IsQa(), new ConfigurationOptionsValidator { Environment = environment.EnvironmentName})
                .AddDeveloperSigningCredential()
                .AddSecretParser<JwtBearerClientAssertionSecretParser>()
                .AddSecretValidator<PrivateKeyJwtSecretValidator>()
                .AddServiceConsumerSecretValidator()
                .AddCustomTokenRequestValidators(typeof(TokenRequestValidator))
                .AddIdentityServerStores(configuration, typeof(Configuration).GetTypeInfo().Assembly)
                .AddPublicClientStore()
                .AddAspNetIdentity<AspNetUser>()
                .AddProfileService<ProfileService>()
                ;

            services.AddIdentityServerCors(loggerFactory);
        }


        private static void AddAspNetIdentityDb(this IServiceCollection services, IConfiguration configuration,
            IHostingEnvironment environment)
        {
            var connectionString = configuration.GetConnectionString("Main");
            services.AddDbContext<UserDbContext>(options => options.UseSqlServer(connectionString));
            services.AddSeedServices<UserDbContext>(environment,
                "NLIP.iShare.AuthorizationRegistry.IdentityServer.Migrations.Seed.AspNetIdentity",
                typeof(UserDbContext).GetTypeInfo().Assembly,
                UsersSeeder.CreateSeeder);         
        }
    }
}
