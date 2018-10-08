using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace NLIP.iShare.EntityFramework
{
    public static class ConfigurationExtensionsIdentityServer
    {
        public static IApplicationBuilder UseIdentityServerMigrations(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var persistedGrantDbContext = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                var configurationDbContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                configurationDbContext.Database.Migrate();
                persistedGrantDbContext.Database.Migrate();
            }

            return app;
        }

        public static IIdentityServerBuilder AddIdentityServerStores(this IIdentityServerBuilder services,
            IConfiguration configuration, Assembly migrationsAssembly)
        {            
            var connectionString = configuration.GetConnectionString("Main");

            services
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly.GetName().Name));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly.GetName().Name));
                })
                ;
            return services;
        }
    }
}
