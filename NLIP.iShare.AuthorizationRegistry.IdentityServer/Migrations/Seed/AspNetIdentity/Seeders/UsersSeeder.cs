using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLIP.iShare.AuthorizationRegistry.IdentityServer.Data;
using NLIP.iShare.AuthorizationRegistry.IdentityServer.Models;
using NLIP.iShare.EntityFramework;

namespace NLIP.iShare.AuthorizationRegistry.IdentityServer.Migrations.Seed.AspNetIdentity.Seeders
{
    internal class UsersSeeder : AspNetIdentityServerDatabaseSeeder, IDatabaseSeeder<UserDbContext>
    {
        public UsersSeeder(
            string environment,
            ISeedDataProvider<UserDbContext> seedDataProvider,
            UserManager<AspNetUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UsersSeeder> logger)
            : base(environment, seedDataProvider, userManager, roleManager, logger)
        {
        }
        public string EnvironmentName => Environment;
        public void Seed()
        {
            Logger.LogInformation("AuthorizationRegistry.AspNetIdentity seed for {environment}", Environment);

            SeedRoles("roles.json").Wait();
            SeedUsers("aspNetUsers.json").Wait();
        }

        public static UsersSeeder CreateSeeder(IServiceProvider srv, string environment) => new UsersSeeder(environment,
            srv.GetService<ISeedDataProvider<UserDbContext>>(),                
            srv.GetService<UserManager<AspNetUser>>(),
            srv.GetService<RoleManager<IdentityRole>>(),
            srv.GetService<ILogger<UsersSeeder>>()
        );
    }
}