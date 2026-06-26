namespace ConfigReader.Api.Models
{

    /// RabbitMQ bağlantı ayarlarını temsil ediyor

    public sealed class RabbitMqOptions
    {
        public string HostName { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string ExchangeName { get; set; } = default!;
    }
}
