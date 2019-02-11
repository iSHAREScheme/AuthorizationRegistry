using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using iSHARE.AuthorizationRegistry.Data.Models;

namespace iSHARE.AuthorizationRegistry.Data.Configurations
{
    public class DelegationConfiguration: IEntityTypeConfiguration<Delegation>
    {
        public void Configure(EntityTypeBuilder<Delegation> builder)
        {
            builder
                .HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
            builder
                .HasOne(x => x.UpdatedBy)
                .WithMany()
                .HasForeignKey(x => x.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
