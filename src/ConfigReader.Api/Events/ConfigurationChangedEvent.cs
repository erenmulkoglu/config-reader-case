namespace ConfigReader.Api.Events
{


    /// Konfigürasyon üzerinde gerçekleşen değişikliği temsil eden event modelidir
    /// RabbitMQ üzerinden publish edilir
    public sealed class ConfigurationChangedEvent
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Value { get; set; } = default!;
        public bool IsActive { get; set; }
        public string ApplicationName { get; set; } = default!;
        public long Version { get; set; }
        public string Operation { get; set; } = default!;
        public DateTime OccurredAtUtc { get; set; }
    }
}
