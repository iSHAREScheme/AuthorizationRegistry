using iSHARE.AuthorizationRegistry.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iSHARE.AuthorizationRegistry.Data.Configurations
{
    public class DelegationConfiguration : IEntityTypeConfiguration<Delegation>
    {
        public void Configure(EntityTypeBuilder<Delegation> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.AuthorizationRegistryId).IsRequired();
            builder.Property(c => c.PolicyIssuer).IsRequired();
            builder.Property(c => c.AccessSubject).IsRequired();
            builder.Property(c => c.Policy).IsRequired();
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
