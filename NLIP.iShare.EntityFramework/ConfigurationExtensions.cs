using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NLIP.iShare.EntityFramework
{
    public static class ConfigurationExtensions
    {
        public static bool RunMigration(this IConfiguration configuration) => configuration["RunMigration"] == "true";       

        public static void UseMigrations<TContext>(this IApplicationBuilder app,
                            IConfiguration configuration, IHostingEnvironment environment)
            where TContext : DbContext
        {
            if (configuration.RunMigration())
            {
                using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    // Automatically perform database migration
                    var context = serviceScope.ServiceProvider.GetRequiredService<TContext>();
                    context.Database.Migrate();

                    var seeder = serviceScope.ServiceProvider.GetService<IDatabaseSeeder<TContext>>();

                    if (seeder == null)
                    {
                        throw new DatabaseSeedException(
                            $"A database seeder for {environment.EnvironmentName} could not be found." +
                            $"Make sure a {nameof(IDatabaseSeeder<TContext>)} implementation is registered for this enviroment.");
                    }
                    seeder.Seed();
                }
            }
        }
        public static IServiceCollection AddSeed<TDatabaseSeederQa, TDatabaseSeederProd, TDatabaseSeederAcc, TDatabaseSeederDev>(this IServiceCollection services, IHostingEnvironment environment)
            where TDatabaseSeederQa : class, IDatabaseSeeder<ConfigurationDbContext>
            where TDatabaseSeederProd : class, IDatabaseSeeder<ConfigurationDbContext>
            where TDatabaseSeederAcc : class, IDatabaseSeeder<ConfigurationDbContext>
            where TDatabaseSeederDev : class, IDatabaseSeeder<ConfigurationDbContext>
        {
            services.AddScoped<TDatabaseSeederQa>();
            services.AddScoped<TDatabaseSeederProd>();
            services.AddScoped<TDatabaseSeederAcc>();
            services.AddScoped<TDatabaseSeederDev>();


            services.AddScoped(opts =>
            {
                var environmentName = environment.EnvironmentName;
                IDatabaseSeeder<ConfigurationDbContext> databaseSeeder;
                switch (environmentName)
                {
                    case Environments.Qa:
                        databaseSeeder = opts.GetService<TDatabaseSeederQa>();
                        break;
                    case Environments.Prod:
                        databaseSeeder = opts.GetService<TDatabaseSeederProd>();
                        break;
                    case Environments.Acc:
                        databaseSeeder = opts.GetService<TDatabaseSeederAcc>();
                        break;
                    case Environments.Development:
                        databaseSeeder = opts.GetService<TDatabaseSeederDev>();
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
