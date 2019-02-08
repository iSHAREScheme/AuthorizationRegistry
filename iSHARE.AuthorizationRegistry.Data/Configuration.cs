using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using iSHARE.EntityFramework;
using System.Reflection;
using iSHARE.AuthorizationRegistry.Data.Migrations.Seed.Seeders;


namespace iSHARE.AuthorizationRegistry.Data
{
    public static class Configuration
    {
        public static void AddDb(this IServiceCollection services,
            IConfiguration configuration,
            IHostingEnvironment environment)
        {
            var connectionString = configuration.GetConnectionString("Main");
            services.AddDbContext<AuthorizationRegistryDbContext>(options => options.UseSqlServer(connectionString));

            services.AddSeedServices<AuthorizationRegistryDbContext>(environment,
                "iSHARE.AuthorizationRegistry.Data.Migrations.Seed",
                typeof(AuthorizationRegistryDbContext).GetTypeInfo().Assembly,
                DatabaseSeeder.CreateSeeder);
        }

        public static void UseMigrations(this IApplicationBuilder app,
            IConfiguration configuration,
            IHostingEnvironment environment)
        {
            app.UseMigrations<AuthorizationRegistryDbContext>(configuration);
            app.UseSeed<AuthorizationRegistryDbContext>(environment);
        }
    }
}
