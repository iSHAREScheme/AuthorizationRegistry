using iSHARE.Identity.Data.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace iSHARE.Identity.Data
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
