using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace NLIP.iShare.EntityFramework
{
    public class ReplaceCollectionDatabaseSeeder<TContext> where TContext : DbContext
    {
        protected readonly ISeedDataProvider<TContext> SeedDataProvider;
        protected readonly ILogger Logger;
        protected readonly string Environment;
        protected readonly DbContext Context;

        protected ReplaceCollectionDatabaseSeeder(ILogger<ReplaceCollectionDatabaseSeeder<TContext>> logger,
            ISeedDataProvider<TContext> seedDataProvider,
            string environment,
            DbContext context)
        {
            Context = context;
            SeedDataProvider = seedDataProvider;
            Logger = logger;
            Environment = environment;
        }

        protected void AddOrUpdateCollection<TEntity>(string source) where TEntity : class, IEntity
        {
            Logger.LogInformation("Seed {TEntity} from {source} for {environment}", typeof(TEntity).FullName, source, Environment);

            var items = SeedDataProvider.GetEntities<TEntity>(source, Environment);

            foreach (var item in items)
            {
                Context.Set<TEntity>().AddOrUpdate(item);
            }
        }
    }
}