using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NLIP.iShare.AuthorizationRegistry.Data.Models;

namespace NLIP.iShare.AuthorizationRegistry.Data.Configurations
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
