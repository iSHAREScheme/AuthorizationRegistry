using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace NLIP.iShare.EntityFramework
{
    public class SeedDataProvider<TContext> : ISeedDataProvider<TContext>
        where TContext : DbContext
    {
        private readonly ILogger _logger;
        private readonly string _sourcesPrefix;
        private readonly Assembly _sourcesAssembly;

        public SeedDataProvider(ILogger<SeedDataProvider<TContext>> logger, string sourcesPrefix, Assembly sourcesAssembly)
        {
            _logger = logger;

            _sourcesPrefix = sourcesPrefix;
            _sourcesAssembly = sourcesAssembly;
        }
        public TEntity[] GetEntities<TEntity>(string source, string environment)
        {
            _logger.LogInformation("Get entities data from {source}, {environment}", source, environment);
            var json = JsonLoader.GetByName(source, environment, _sourcesPrefix, _sourcesAssembly);

            var items = ImportFromJson.DeserializeCollectionOf<TEntity>(json);

            _logger.LogInformation("Found {count} item(s)", items.Length);
            return items;
        }
    }
}