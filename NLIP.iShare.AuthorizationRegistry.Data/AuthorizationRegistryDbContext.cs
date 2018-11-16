using Microsoft.EntityFrameworkCore;
using NLIP.iShare.AuthorizationRegistry.Data.Models;
using NLIP.iShare.EntityFramework.Migrations;

namespace NLIP.iShare.AuthorizationRegistry.Data
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