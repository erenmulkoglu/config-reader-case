using ConfigReader.Api.Documents;
using ConfigReader.Api.Events;
using ConfigReader.Api.Messaging;
using ConfigReader.Api.Repositories;
using ConfigReader.Api.Requests;

namespace ConfigReader.Api.Services;



/// Yönetim paneli ve API için configuration CRUD operasyonlarını koordine eder
/// Repository işlemlerinden sonra RabbitMQ üzerinden configuration changed event yayınlar
public sealed class ConfigurationAdminService
{
    private const string CreatedOperation = "Created";
    private const string UpdatedOperation = "Updated";
    private const string DeletedOperation = "Deleted";

    private readonly IConfigurationRepository _repository;
    private readonly IEventPublisher _eventPublisher;

    public ConfigurationAdminService(IConfigurationRepository repository, IEventPublisher eventPublisher)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
    }


    /// Yönetim ekranında listelenmek üzere tüm konfigürasyon kayıtlarını getirir
    public Task<List<ConfigurationDocument>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _repository.GetAllAsync(cancellationToken);
    }


    /// Yeni bir konfigürasyon kaydı oluşturur ve Created event'ini RabbitMQ'ya publish eder
    public async Task<ConfigurationDocument> CreateAsync(CreateConfigurationRequest request, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var document = new ConfigurationDocument
        {
            Name = request.Name.Trim(),
            Type = request.Type.Trim(),
            Value = request.Value.Trim(),
            IsActive = request.IsActive,
            ApplicationName = request.ApplicationName.Trim(),
            Version = 1,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        var created = await _repository.CreateAsync(document, cancellationToken);

        await PublishConfigurationChangedAsync(created, CreatedOperation, cancellationToken);

        return created;
    }


    /// Var olan konfigürasyon kaydını siler
    /// Başarılı silme sonrası Deleted event'ini RabbitMQ'ya publish eder
    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var document = await _repository.GetByIdAsync(id, cancellationToken);

        if (document is null)
            return false;

        var deleted = await _repository.DeleteAsync(id, cancellationToken);

        if (!deleted)
            return false;

        await PublishConfigurationChangedAsync(document, DeletedOperation, cancellationToken);

        return true;
    }


    /// Version kontrolü ile mevcut konfigürasyon kaydını günceller
    /// Başarılı güncelleme sonrası Updated event'ini RabbitMQ'ya publish eder
    public async Task<bool> UpdateAsync(string id, UpdateConfigurationRequest request, CancellationToken cancellationToken = default)
    {
        var document = new ConfigurationDocument
        {
            Id = id,
            Name = request.Name.Trim(),
            Type = request.Type.Trim(),
            Value = request.Value.Trim(),
            IsActive = request.IsActive,
            ApplicationName = request.ApplicationName.Trim(),
            Version = request.Version + 1
        };

        var updated = await _repository.UpdateAsync(id, document, request.Version, cancellationToken);

        if (!updated)
            return false;

        await PublishConfigurationChangedAsync(document, UpdatedOperation, cancellationToken);

        return true;
    }


    /// ConfigurationDocument bilgisinden event oluşturur ve event publisher üzerinden yayınlar
    private Task PublishConfigurationChangedAsync(ConfigurationDocument document, string operation, CancellationToken cancellationToken)
    {
        var @event = CreateConfigurationChangedEvent(document, operation);

        return _eventPublisher.PublishConfigurationChangedAsync(@event, cancellationToken);
    }


    /// ConfigurationDocument nesnesini RabbitMQ'ya gönderilecek ConfigurationChangedEvent modeline dönüştürür
    private static ConfigurationChangedEvent CreateConfigurationChangedEvent(ConfigurationDocument document, string operation)
    {
        return new ConfigurationChangedEvent
        {
            Id = document.Id,
            Name = document.Name,
            Type = document.Type,
            Value = document.Value,
            IsActive = document.IsActive,
            ApplicationName = document.ApplicationName,
            Version = document.Version,
            Operation = operation,
            OccurredAtUtc = DateTime.UtcNow
        };
    }
}