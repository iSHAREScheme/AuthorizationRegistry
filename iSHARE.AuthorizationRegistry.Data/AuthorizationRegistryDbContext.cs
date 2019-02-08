using Microsoft.EntityFrameworkCore;
using iSHARE.AuthorizationRegistry.Data.Models;
using iSHARE.EntityFramework.Migrations;

namespace iSHARE.AuthorizationRegistry.Data
{
    public class AuthorizationRegistryDbContext : DbContext
    {
        public DbSet<Delegation> Delegations { get; set; }

        public DbSet<DelegationHistory> DelegationsHistories { get; set; }

        public DbSet<User> Users { get; set; }

        public AuthorizationRegistryDbContext(DbContextOptions<AuthorizationRegistryDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.RegisterConfigurations<AuthorizationRegistryDbContext>();
        }
    }
}