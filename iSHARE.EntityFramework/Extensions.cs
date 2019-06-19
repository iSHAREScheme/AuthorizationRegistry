using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using iSHARE.EntityFramework.Migrations.Seed;
using Microsoft.EntityFrameworkCore;

namespace iSHARE.EntityFramework
{
    public static class Extensions
    {
        public static void AddOrUpdate<T>(this DbSet<T> dbSet, T data) where T : class
        {
            var context = dbSet.GetContext();
            var ids = context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.Select(x => x.Name).ToList();

            var t = typeof(T);
            var keyFields = new List<PropertyInfo>();

            foreach (var prop in t.GetProperties())
            {
                var keyAttr = ids.Contains(prop.Name);
                if (keyAttr)
                {
                    keyFields.Add(prop);
                }
            }
            if (keyFields.Count <= 0)
            {
                throw new DatabaseSeedException($"{t.FullName} does not have a KeyAttribute field. Unable to exec AddOrUpdate call.");
            }
            var entities = dbSet.AsNoTracking().IgnoreQueryFilters().ToList();
            foreach (var keyField in keyFields)
            {
                var keyVal = keyField.GetValue(data);
                entities = entities.Where(p => p.GetType().GetProperty(keyField.Name).GetValue(p).Equals(keyVal)).ToList();
            }
            var dbVal = entities.FirstOrDefault();
            if (dbVal != null && DbValIsDeleted(dbVal))
            {
                return;
            }

            if (dbVal != null)
            {
                context.Entry(dbVal).CurrentValues.SetValues(data);
                context.Entry(dbVal).State = EntityState.Modified;
                return;
            }
            dbSet.Add(data);
        }

        private static bool DbValIsDeleted<T>(T dbVal)
        {
            var dbValType = typeof(T);
            var deleted = dbValType.GetProperties().FirstOrDefault(c => c.Name == "Deleted" || c.Name == "IsDeleted");
            if (deleted == null)
            {
                return false;
            }

            return (bool)deleted.GetValue(dbVal);
        }
    }
}

internal static class HackyDbSetGetContextTrick
{
    internal static DbContext GetContext<TEntity>(this DbSet<TEntity> dbSet)
        where TEntity : class
    {
        return (DbContext)dbSet
            .GetType().GetTypeInfo()
            .GetField("_context", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(dbSet);
    }
}

