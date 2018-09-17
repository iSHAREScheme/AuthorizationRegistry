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
using NLIP.iShare.AuthorizationRegistry.IdentityServer.Data;
using NLIP.iShare.AuthorizationRegistry.IdentityServer.Migrations.Seed.Seeders;
using NLIP.iShare.Configuration.Configurations;
using NLIP.iShare.EntityFramework;
using NLIP.iShare.IdentityServer;
using NLIP.iShare.IdentityServer.Validation;
using System;
using System.Reflection;
using NLIP.iShare.AuthorizationRegistry.IdentityServer.Models;

namespace NLIP.iShare.AuthorizationRegistry.IdentityServer
{
    public static class IdentityServerConfiguration
    {
        public static void UseIdentityServer(this IApplicationBuilder app, IConfiguration configuration,
            IHostingEnvironment environment)
        {
            app.UseIdentityServer()
                .UseIdentityServerMigrations()
                .UseIdentityServerSeed<ConfigurationDbContext>(environment)
                .UseAspNetIdentityMigrations()
                .UseAspNetIdentitySeed<UserDbContext>(environment)
                ;
        }

        private static IApplicationBuilder UseAspNetIdentitySeed<UserDbContext>(this IApplicationBuilder app, IHostingEnvironment environment)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var seeder = serviceScope.ServiceProvider.GetService<IDatabaseSeeder<UserDbContext>>();
                if (seeder == null)
                {
                    throw new DatabaseSeedException(
                        $"A database seeder for {environment.EnvironmentName} could not be found in the requested namespace {typeof(IDatabaseSeeder<UserDbContext>).Namespace}. " +
                        $"Make sure a {nameof(IDatabaseSeeder<UserDbContext>)} implementation is registered for this enviroment.");
                }
                seeder.Seed();
            }
            return app;
        }

        private static IApplicationBuilder UseAspNetIdentityMigrations(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var userDbContext = serviceScope.ServiceProvider.GetRequiredService<UserDbContext>();

                userDbContext.Database.Migrate();
            }

            return app;
        }

        public static void AddIdentityServer(this IServiceCollection services,
            IConfiguration configuration, IHostingEnvironment environment, ILoggerFactory loggerFactory)
        {
            var connectionString = configuration.GetConnectionString("Main");
            services.AddDbContext<UserDbContext>(options => options.UseSqlServer(connectionString));
            services.AddIdentityServerSeedServices(environment);
            services.AddAspNetIdentitySeedServices(environment);

            //// explicit configuration of token expiration (default value)
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

            var partyDetailsOptions = services.BuildServiceProvider().GetRequiredService<PartyDetailsOptions>();
            services.AddIdentityServer(options =>
                {
                    if (environment.IsDevelopment())
                    {
                        IdentityModelEventSource.ShowPII = true;
                    }

                    options.IssuerUri = partyDetailsOptions.BaseUri;
                })
                .AddDeveloperSigningCredential()
                .AddSecretParser<JwtBearerClientAssertionSecretParser>()
                .AddSecretValidator<PrivateKeyJwtSecretValidator>()
                .AddServiceConsumerSecretValidator()
                .AddCustomTokenRequestValidators<TokenRequestValidator>()
                .AddIdentityServerStores(configuration, typeof(IdentityServerConfiguration).GetTypeInfo().Assembly)
                .AddPublicClientStore()
                .AddAspNetIdentity<AspNetUser>()
                .AddProfileService<ProfileService>()
                ;

            services.AddIdentityServerCors(loggerFactory);
        }

        private static IServiceCollection AddIdentityServerSeedServices(this IServiceCollection services, IHostingEnvironment environment)
        {
            services.AddScoped<ISeedDataProvider<ConfigurationDbContext>>(opts =>
                new SeedDataProvider<ConfigurationDbContext>(
                    opts.GetService<ILogger<SeedDataProvider<ConfigurationDbContext>>>(),
                    "NLIP.iShare.AuthorizationRegistry.IdentityServer.Migrations.Seed",
                    typeof(IdentityServerConfiguration).GetTypeInfo().Assembly)
            );

            services.AddSeed<QaDatabaseSeeder, ProdDatabaseSeeder, AccDatabaseSeeder, DevelopmentDatabaseSeeder>(
                environment);

            return services;
        }

        private static IServiceCollection AddAspNetIdentitySeedServices(this IServiceCollection services, IHostingEnvironment environment)
        {
            services.AddScoped<ISeedDataProvider<UserDbContext>>(opts =>
                new SeedDataProvider<UserDbContext>(
                    opts.GetService<ILogger<SeedDataProvider<UserDbContext>>>(),
                    "NLIP.iShare.AuthorizationRegistry.IdentityServer.Migrations.Seed.AspNetIdentity",
                    typeof(IdentityServerConfiguration).GetTypeInfo().Assembly)
            );

            services.AddScoped<Migrations.Seed.AspNetIdentity.Seeders.ProdDatabaseSeeder>();
            services.AddScoped<Migrations.Seed.AspNetIdentity.Seeders.QaDatabaseSeeder>();
            services.AddScoped<Migrations.Seed.AspNetIdentity.Seeders.DevelopmentDatabaseSeeder>();
            services.AddScoped<Migrations.Seed.AspNetIdentity.Seeders.AccDatabaseSeeder>();

            services.AddScoped(opts =>
            {
                var environmentName = environment.EnvironmentName;
                IDatabaseSeeder<UserDbContext> databaseSeeder = null;
                switch (environmentName)
                {
                    case Environments.Qa:
                        databaseSeeder = opts.GetService<Migrations.Seed.AspNetIdentity.Seeders.QaDatabaseSeeder>();
                        break;
                    case Environments.Prod:
                        databaseSeeder = opts.GetService<Migrations.Seed.AspNetIdentity.Seeders.ProdDatabaseSeeder>();
                        break;
                    case Environments.Acc:
                        databaseSeeder = opts.GetService<Migrations.Seed.AspNetIdentity.Seeders.AccDatabaseSeeder>();
                        break;
                    case Environments.Development:
                        databaseSeeder = opts.GetService<Migrations.Seed.AspNetIdentity.Seeders.DevelopmentDatabaseSeeder>();
                        break;
                    default:
                        throw new DatabaseSeedException($"{environmentName} is not registered.");
                }

                return databaseSeeder;
            });

            return services;
        }
    }
}
