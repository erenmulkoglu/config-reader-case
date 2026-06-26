using ConfigReader.Api.Documents;
using ConfigReader.Api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;


namespace ConfigReader.Api.Repositories
{


    /// Yönetim API'si için MongoDB üzerinde configuration CRUD işlemlerini gerçekleştiren repository katmanıdır
    public sealed class MongoConfigurationRepository : IConfigurationRepository
    {
        private readonly IMongoCollection<ConfigurationDocument> _collection;

        public MongoConfigurationRepository(IOptions<MongoDbOptions> options)
        {
            var mongoOptions = options.Value;

            var client = new MongoClient(mongoOptions.ConnectionString);
            var database = client.GetDatabase(mongoOptions.DatabaseName);

            _collection = database.GetCollection<ConfigurationDocument>(
                mongoOptions.CollectionName);
        }


        /// Tüm konfigürasyon kayıtlarını MongoDB üzerinden getirir
        public async Task<List<ConfigurationDocument>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _collection
                .Find(_ => true)
                .SortBy(x => x.ApplicationName)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }


        /// Id bilgisine göre tek bir konfigürasyon kaydını getirir
        public async Task<ConfigurationDocument?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }


        /// Id bilgisine göre konfigürasyon kaydını siler
        public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var result = await _collection.DeleteOneAsync(
                x => x.Id == id,
                cancellationToken);

            return result.DeletedCount > 0;
        }

        /// Yeni bir konfigürasyon kaydı oluşturur.
        public async Task<ConfigurationDocument> CreateAsync(ConfigurationDocument document, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);
            return document;
        }


        /// Version kontrolü yaparak mevcut konfigürasyon kaydını günceller
        /// Version uyuşmazsa güncelleme yapılmaz
        public async Task<bool> UpdateAsync(string id, ConfigurationDocument document, long expectedVersion, CancellationToken cancellationToken = default)
        {
            var filter = Builders<ConfigurationDocument>.Filter.And(
                Builders<ConfigurationDocument>.Filter.Eq(x => x.Id, id),
                Builders<ConfigurationDocument>.Filter.Eq(x => x.Version, expectedVersion));

            var update = Builders<ConfigurationDocument>.Update
                .Set(x => x.Name, document.Name)
                .Set(x => x.Type, document.Type)
                .Set(x => x.Value, document.Value)
                .Set(x => x.IsActive, document.IsActive)
                .Set(x => x.ApplicationName, document.ApplicationName)
                .Set(x => x.UpdatedAtUtc, DateTime.UtcNow)
                .Inc(x => x.Version, 1);

            var result = await _collection.UpdateOneAsync(
                filter,
                update,
                cancellationToken: cancellationToken);

            return result.ModifiedCount == 1;
        }
    }
}
