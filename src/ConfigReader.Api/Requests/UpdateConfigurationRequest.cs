namespace ConfigReader.Api.Requests
{


    /// Mevcut bir konfigürasyon kaydını güncellemek için kullanılan istek modelidir
    public sealed class UpdateConfigurationRequest
    {
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Value { get; set; } = default!;
        public bool IsActive { get; set; }
        public string ApplicationName { get; set; } = default!;
        public long Version { get; set; }
    }
}
