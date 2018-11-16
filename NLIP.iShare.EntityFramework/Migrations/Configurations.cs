using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace NLIP.iShare.EntityFramework.Migrations
{
    public static class Configurations
    {
        public static void RegisterConfigurations<TContext>(this ModelBuilder modelBuilder)
        {
            var typesToRegister = typeof(TContext).Assembly.GetTypes()
                .Where(t => t.GetInterfaces().Any(gi =>
                    gi.IsGenericType && gi.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))).ToList();


            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.ApplyConfiguration(configurationInstance);
            }
        }
    }
}
