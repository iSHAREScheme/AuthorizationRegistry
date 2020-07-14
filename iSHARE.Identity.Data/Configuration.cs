using System.Reflection;
using iSHARE.EntityFramework;
using iSHARE.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iSHARE.Identity.Data
{
    public static class Configuration
    {
        public static void AddIdentityServices<TUser, TUserContext>(this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            string @namespace,
            Assembly assembly)
            where TUserContext : DbContext
            where TUser : class, IAspNetUser
        {
            var connectionString = configuration.GetConnectionString("Main");
            services.AddDbContext<TUserContext>(options => options.UseSqlServer(connectionString));
            services.AddSeedServices<TUserContext>(environment,
                @namespace,
                assembly,
                AspNetIdentityServerDatabaseSeeder<TUserContext, TUser>.CreateSeeder);
        }
    }
}
