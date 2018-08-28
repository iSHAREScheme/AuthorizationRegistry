using Microsoft.EntityFrameworkCore;

namespace NLIP.iShare.EntityFramework
{
    /// <summary>
    /// Provides a list of entities based on environment
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface ISeedDataProvider<TContext>
        where TContext: DbContext
    {
        TEntity[] GetEntities<TEntity>(string source, string environment);
    }
}