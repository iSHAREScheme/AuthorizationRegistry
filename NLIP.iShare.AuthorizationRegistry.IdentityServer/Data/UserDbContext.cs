using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NLIP.iShare.AuthorizationRegistry.IdentityServer.Configurations;
using NLIP.iShare.AuthorizationRegistry.IdentityServer.Models;

namespace NLIP.iShare.AuthorizationRegistry.IdentityServer.Data
{
    public class UserDbContext : IdentityDbContext<AspNetUser>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new AspNetUserConfiguration());
        }
    }
}
