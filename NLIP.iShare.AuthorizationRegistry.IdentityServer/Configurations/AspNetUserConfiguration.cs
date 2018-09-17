using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NLIP.iShare.AuthorizationRegistry.IdentityServer.Models;
using NLIP.iShare.Models;

namespace NLIP.iShare.AuthorizationRegistry.IdentityServer.Configurations
{
    public class AspNetUserConfiguration : IEntityTypeConfiguration<AspNetUser>
    {
        public void Configure(EntityTypeBuilder<AspNetUser> builder)
        {
            builder
                .HasIndex(i => i.NormalizedUserName)
                .IsUnique(false);

            builder
                .HasQueryFilter(u => !u.IsDeleted);
        }
    }
}
