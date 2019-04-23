using System;
using System.Linq;
using System.Reflection;
using iSHARE.Configuration;
using iSHARE.EntityFramework.Migrations.Seed;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace iSHARE.EntityFramework
{
    public static class ConfigurationExtensions
    {
        public static bool RunMigration(this IConfiguration configuration) => configuration["RunMigration"] == "true";

        public static IApplicationBuilder UseMigrations<TContext>(this IApplicationBuilder app,
            IConfiguration configuration)
            where TContext : DbContext
        {
            if (configuration.RunMigration())
            {
                using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<TContext>();

                    context.Database.Migrate();
                }
            }
            return app;
        }

        public static IApplicationBuilder UseSeed<TContext>(this IApplicationBuilder app,
            IHostingEnvironment environment)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var seederFactory = serviceScope.ServiceProvider.GetService<Func<IDatabaseSeeder<TContext>>>();
                var seeder = seederFactory?.Invoke();
                if (seeder == null)
                {
                    throw new DatabaseSeedException(
                        $"A database seeder for {environment.EnvironmentName} could not be found in the requested namespace {typeof(IDatabaseSeeder<TContext>).Namespace}. " +
                        $"Make sure a {nameof(IDatabaseSeeder<TContext>)}<{typeof(TContext).Name}> implementation is registered for this environment.");
                }

                seeder.Seed();
            }
            return app;
        }

        public static IServiceCollection AddSeedServices<TContext>(this IServiceCollection services,
            IHostingEnvironment environment,
            string @namespace,
            Assembly sourcesAssembly,
            Func<IServiceProvider, string, IDatabaseSeeder<TContext>> seederFactory)
            where TContext : DbContext
        {
            services.AddScoped<ISeedDataProvider<TContext>>(opts =>
                new SeedDataProvider<TContext>(
                    opts.GetService<ILogger<SeedDataProvider<TContext>>>(),
                    @namespace,
                    sourcesAssembly)
            );

            services.AddScoped(srv => seederFactory(srv, Environments.Development));
            services.AddScoped(srv => seederFactory(srv, Environments.QaLive));
            services.AddScoped(srv => seederFactory(srv, Environments.QaTest));
            services.AddScoped(srv => seederFactory(srv, Environments.Test));
            services.AddScoped(srv => seederFactory(srv, Environments.Live));

            services.AddSeed<TContext>(environment);

            return services;
        }

        public static IServiceCollection AddSeedFactory<TContext>(this IServiceCollection services,
          Func<IServiceProvider, string, IDatabaseSeeder<TContext>> seederFactory, string environmentName)
          where TContext : DbContext
            => services.AddScoped(srv => seederFactory(srv, environmentName));

        public static IServiceCollection AddSeed<TContext>(this IServiceCollection services, IHostingEnvironment environment)
        {
            services.AddScoped((Func<IServiceProvider, Func<IDatabaseSeeder<TContext>>>)(opts => () =>
                {
                    var environmentName = environment.EnvironmentName;

                    return opts.GetServices<IDatabaseSeeder<TContext>>()
                        .LastOrDefault(c => c.EnvironmentName == environmentName);
                })
            );
            return services;
        }
    }
}
