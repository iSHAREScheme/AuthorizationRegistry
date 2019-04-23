using iSHARE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iSHARE.Identity.Data.Configurations
{
    public class UserConfiguration<TUser> : IEntityTypeConfiguration<TUser>
        where TUser : class, IEntity
    {
        public void Configure(EntityTypeBuilder<TUser> builder)
        {
            builder
                .HasKey(i => i.Id);
        }
    }
}
