using ConfigReader.Api.Events;



namespace ConfigReader.Api.Messaging
{
    public interface IEventPublisher
    {
        Task PublishConfigurationChangedAsync(ConfigurationChangedEvent @event, CancellationToken cancellationToken = default);
    }
}
