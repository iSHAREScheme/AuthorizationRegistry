using Microsoft.EntityFrameworkCore;

namespace NLIP.iShare.EntityFramework.Migrations.Seed
{
    /// <summary>
    /// Provides a list of entities based on environment
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface ISeedDataProvider<TContext>
        where TContext: DbContext
    {
        TEntity[] GetEntities<TEntity>(string source, string environment) where TEntity : class;
        string GetRaw(string source, string environment);
    }
}