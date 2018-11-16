using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NLIP.iShare.AuthorizationRegistry.Data.Configurations;
using NLIP.iShare.AuthorizationRegistry.Data.Models;

namespace NLIP.iShare.AuthorizationRegistry.Data
{
    public class AspNetUserDbContext : IdentityDbContext<AspNetUser>
    {
        public AspNetUserDbContext(DbContextOptions<AspNetUserDbContext> options)
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
