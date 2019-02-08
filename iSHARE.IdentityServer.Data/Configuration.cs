using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using iSHARE.EntityFramework;
using iSHARE.IdentityServer.Data.Migrations.Seed;

namespace iSHARE.IdentityServer.Data
{
    public static class Configuration
    {
        public static IApplicationBuilder UseIdentityServerDb(this IApplicationBuilder app,
            IConfiguration configuration,
            IHostingEnvironment environment
            )
        {
            app.UseMigrations<PersistedGrantDbContext>(configuration);
            app.UseMigrations<ConfigurationDbContext>(configuration)
               .UseSeed<ConfigurationDbContext>(environment);

            return app;
        }

        public static IIdentityServerBuilder AddIdentityServerDb(this IIdentityServerBuilder identityServerBuilder,
            IConfiguration configuration,
            IHostingEnvironment environment,
            string @namespace,
            Assembly assembly
            )
        {
            var connectionString = configuration.GetConnectionString("Main");

            identityServerBuilder
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(typeof(Configuration).GetTypeInfo().Assembly.GetName().Name));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(typeof(Configuration).GetTypeInfo().Assembly.GetName().Name));
                })
                ;

            var services = identityServerBuilder.Services;

            services
                .AddSeedServices(environment, @namespace, assembly, ApiResourcesSeeder.CreateSeeder)
                .AddSeedServices(environment, @namespace, assembly, ClientsDatabaseSeeder.CreateSeeder)
                ;

            return identityServerBuilder;
        }
    }
}
