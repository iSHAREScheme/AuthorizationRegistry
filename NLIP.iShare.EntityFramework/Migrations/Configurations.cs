using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace NLIP.iShare.EntityFramework.Migrations
{
    public static class Configurations
    {
        public static void RegisterConfigurations<TContext>(this ModelBuilder modelBuilder)
        {
            var entitiesTypes = typeof(TContext).GetProperties()
                    .Where(c => c.PropertyType.IsGenericType && c.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                    .Select(c => c.PropertyType.GenericTypeArguments.First());

            var typesToRegister = typeof(TContext).Assembly.GetTypes()
                .Where(t => t.GetInterfaces()
                    .Any(gi => gi.IsGenericType
                           && gi.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)
                           && entitiesTypes.Any(e => e == gi.GenericTypeArguments.First()))
                );

            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.ApplyConfiguration(configurationInstance);
            }
        }
    }
}
