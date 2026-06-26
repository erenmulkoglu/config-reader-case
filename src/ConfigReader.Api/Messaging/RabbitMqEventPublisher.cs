using ConfigReader.Api.Events;
using ConfigReader.Api.Models;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ConfigReader.Api.Messaging;


/// Configuration değişikliklerini RabbitMQ üzerine event olarak yayınlayan publisher implementasyonudur
public sealed class RabbitMqEventPublisher : IEventPublisher
{
    private readonly RabbitMqOptions _options;

    public RabbitMqEventPublisher(IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
    }


    /// ConfigurationChangedEvent mesajını Rabbitmq exchange üzerine publish eder
    /// Routing key formatı: configuration.changed.{ApplicationName}
    public Task PublishConfigurationChangedAsync(ConfigurationChangedEvent @event, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"RabbitMQ Publish: {@event.Operation} - {@event.Name} - {@event.ApplicationName}");
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            UserName = _options.UserName,
            Password = _options.Password
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: _options.ExchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false);

        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);

        var properties = channel.CreateBasicProperties();
        properties.ContentType = "application/json";
        properties.DeliveryMode = 2;

        var routingKey = $"configuration.changed.{@event.ApplicationName}";

        channel.BasicPublish(exchange: _options.ExchangeName, routingKey: routingKey, basicProperties: properties, body: body);

        return Task.CompletedTask;
    }
}