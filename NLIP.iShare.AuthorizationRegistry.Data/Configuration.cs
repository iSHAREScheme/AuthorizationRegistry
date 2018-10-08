using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLIP.iShare.EntityFramework;
using System.Reflection;
using NLIP.iShare.AuthorizationRegistry.Data.Migrations.Seed;

namespace NLIP.iShare.AuthorizationRegistry.Data
{
    public static class Configuration
    {
        public static void AddAuthorizationRegistryUserDbContext(this IServiceCollection services,
            IConfiguration configuration,
            IHostingEnvironment environment)
        {
            services.AddDbContext<AuthorizationRegistryDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("Main")));

            services.AddSeedServices<AuthorizationRegistryDbContext>(environment,
                "NLIP.iShare.AuthorizationRegistry.Data.Migrations.Seed",
                typeof(AuthorizationRegistryDbContext).GetTypeInfo().Assembly,
                DatabaseSeeder.CreateSeeder);
        }

        public static void UseMigrations(this IApplicationBuilder app,
            IConfiguration configuration,
            IHostingEnvironment environment)
        {
            app.UseMigrations<AuthorizationRegistryDbContext>(configuration, environment);
            app.UseSeed<AuthorizationRegistryDbContext>(environment);
        }
    }
}
