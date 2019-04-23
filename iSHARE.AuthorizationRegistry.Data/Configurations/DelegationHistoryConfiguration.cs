using iSHARE.AuthorizationRegistry.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iSHARE.AuthorizationRegistry.Data.Configurations
{
    public class DelegationHistoryConfiguration : IEntityTypeConfiguration<DelegationHistory>
    {
        public void Configure(EntityTypeBuilder<DelegationHistory> builder)
        {
            builder.HasKey(c => c.Id);
        }
    }
}
