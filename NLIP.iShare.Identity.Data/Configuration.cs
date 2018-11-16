using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLIP.iShare.EntityFramework;
using NLIP.iShare.Models;

namespace NLIP.iShare.Identity.Data
{
    public static class Configuration
    {
        public static void AddIdentityServices<TUser, TUserContext>(this IServiceCollection services,
            IConfiguration configuration,
            IHostingEnvironment environment,
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
