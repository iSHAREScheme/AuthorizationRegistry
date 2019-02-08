using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using iSHARE.AuthorizationRegistry.Data.Configurations;
using iSHARE.AuthorizationRegistry.Data.Models;

namespace iSHARE.AuthorizationRegistry.Data
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
