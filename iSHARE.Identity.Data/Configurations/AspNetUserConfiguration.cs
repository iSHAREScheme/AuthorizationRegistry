using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iSHARE.Identity.Data.Configurations
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
