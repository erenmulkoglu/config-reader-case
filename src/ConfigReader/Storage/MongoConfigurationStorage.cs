using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigReader.Abstractions;
using ConfigReader.Documents;
using ConfigReader.Models;
using MongoDB.Driver;

namespace ConfigReader.Storage
{
    internal sealed class MongoConfigurationStorage : IConfigurationStorage
    {
        private readonly IMongoCollection<ConfigurationDocument> _collection;

        public MongoConfigurationStorage(string connectionString)
        {
            var mongoUrl = MongoUrl.Create(connectionString);
            var client = new MongoClient(mongoUrl);

            var databaseName = string.IsNullOrWhiteSpace(mongoUrl.DatabaseName) ? "ConfigReaderDb" : mongoUrl.DatabaseName;

            var database = client.GetDatabase(databaseName);

            _collection = database.GetCollection<ConfigurationDocument>("Configurations");
        }

        public async Task<IReadOnlyCollection<ConfigurationItem>> GetActiveConfigurationsAsync(string applicationName, CancellationToken cancellationToken = default)
        {
            var documents = await _collection
                .Find(x => x.ApplicationName == applicationName && x.IsActive)
                .ToListAsync(cancellationToken);

            return documents.Select(MapToModel).ToList();
        }

        private static ConfigurationItem MapToModel(ConfigurationDocument document)
        {
            return new ConfigurationItem
            {
                Id = document.Id,
                Name = document.Name,
                Type = document.Type,
                Value = document.Value,
                IsActive = document.IsActive,
                ApplicationName = document.ApplicationName,
                Version = document.Version,
                CreatedAtUtc = document.CreatedAtUtc,
                UpdatedAtUtc = document.UpdatedAtUtc
            };
        }
    }
}
