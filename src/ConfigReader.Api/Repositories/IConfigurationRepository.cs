using ConfigReader.Api.Documents;



namespace ConfigReader.Api.Repositories
{
    public interface IConfigurationRepository
    {
        Task<List<ConfigurationDocument>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ConfigurationDocument> CreateAsync(ConfigurationDocument document, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(string id, ConfigurationDocument document, long expectedVersion, CancellationToken cancellationToken = default);
        Task<ConfigurationDocument?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}
