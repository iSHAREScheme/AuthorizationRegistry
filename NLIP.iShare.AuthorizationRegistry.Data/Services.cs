using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLIP.iShare.AuthorizationRegistry.Data.Migrations;
using NLIP.iShare.EntityFramework;
using System.Reflection;
using NLIP.iShare.AuthorizationRegistry.Data.Migrations.Seed;

namespace NLIP.iShare.AuthorizationRegistry.Data
{
    public static class Services
    {
        public static void AddAuthorizationRegistryUserDbContext(this IServiceCollection services,
            IConfiguration configuration,
            IHostingEnvironment environment)
        {
            services.AddDbContext<AuthorizationRegistryDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("Main")));

            services.AddScoped<ISeedDataProvider<AuthorizationRegistryDbContext>>(opts => new SeedDataProvider<AuthorizationRegistryDbContext>(
                opts.GetService<ILogger<SeedDataProvider<AuthorizationRegistryDbContext>>>(),
                typeof(AuthorizationRegistrySeedDataProvider).GetTypeInfo().Namespace,
                typeof(AuthorizationRegistryDbContext).GetTypeInfo().Assembly)
            );

            services.AddScoped<ProdDatabaseSeeder>();
            services.AddScoped<QaDatabaseSeeder>();
            services.AddScoped<DevelopmentDatabaseSeeder>();
            services.AddScoped<AccDatabaseSeeder>();

            services.AddScoped(opts =>
            {
                var environmentName = environment.EnvironmentName;
                IDatabaseSeeder<AuthorizationRegistryDbContext> databaseSeeder;
                switch (environmentName)
                {
                    case Environments.Qa:
                        databaseSeeder = opts.GetService<QaDatabaseSeeder>();
                        break;
                    case Environments.Prod:
                        databaseSeeder = opts.GetService<ProdDatabaseSeeder>();
                        break;
                    case Environments.Acc:
                        databaseSeeder = opts.GetService<AccDatabaseSeeder>();
                        break;
                    case Environments.Development:
                        databaseSeeder = opts.GetService<DevelopmentDatabaseSeeder>();
                        break;
                    default: throw new DatabaseSeedException($"{environmentName} is not registered.");
                }

                return databaseSeeder;
            });
        }

        public static void UseAuthorizationRegistryStore(this IApplicationBuilder app,
            IConfiguration configuration,
            IHostingEnvironment environment)
        {
            app.UseMigrations<AuthorizationRegistryDbContext>(configuration, environment);
        }
    }
}
