using System.Reflection;
using iSHARE.AuthorizationRegistry.Core.Api;
using iSHARE.AuthorizationRegistry.Data.Migrations.Seed.Seeders;
using iSHARE.EntityFramework;
using iSHARE.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddSeedServices(environment,
                "iSHARE.AuthorizationRegistry.Data.Migrations.Seed",
                typeof(AuthorizationRegistryDbContext).GetTypeInfo().Assembly,
                DatabaseSeeder.CreateSeeder);

            services.AddTransient<IUsersRepository, UsersRepository>();
            services.AddTransient<ITenantUsersRepository, TenantUsersRepository>();
            services.AddTransient<IDelegationsRepository, DelegationsRepository>();
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
