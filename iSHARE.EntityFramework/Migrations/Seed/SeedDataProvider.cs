using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace iSHARE.EntityFramework.Migrations.Seed
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

        public TEntity[] GetEntities<TEntity>(string source, string environment) where TEntity : class
        {
            try
            {
                _logger.LogInformation(
                    "Get entities data from path: {path}, assembly: {assembly}.",
                    $"{_sourcesPrefix}.{environment}.{source}",
                    _sourcesAssembly.GetName());
                var json = JsonLoader.GetByName(source, environment, _sourcesPrefix, _sourcesAssembly);

                _logger.LogInformation("Retrieved json: {json}", json);

                var items = ImportFromJson.DeserializeCollectionOf<TEntity>(json);

                _logger.LogInformation("Found {count} item(s)", items.Length);
                return items;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Couldn't retrieve entities.");
                throw;
            }
        }

        public string GetRaw(string source, string environment)
        {
            _logger.LogInformation("Get entities data from {source}, {environment}", source, environment);
            var raw = JsonLoader.GetByName(source, environment, _sourcesPrefix, _sourcesAssembly);

            _logger.LogInformation("Found item ", raw != null);
            return raw;
        }
    }
}
