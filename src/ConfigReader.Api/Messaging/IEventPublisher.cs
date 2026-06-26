using ConfigReader.Api.Events;



namespace ConfigReader.Api.Messaging
{

    /// Konfigürasyon değişikliklerini message broker üzerine yayınlayan servis sözleşmesidir

    public interface IEventPublisher
    {

        /// Konfigürasyon değişiklik olayını message broker'a yayınlıyor

        Task PublishConfigurationChangedAsync(ConfigurationChangedEvent @event, CancellationToken cancellationToken = default);
    }
}
