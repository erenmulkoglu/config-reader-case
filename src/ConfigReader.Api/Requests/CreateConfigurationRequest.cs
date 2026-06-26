namespace ConfigReader.Api.Requests
{

    /// Yeni bir konfigürasyon kaydı oluşturmak için kullanılan istek modelidir
    public sealed class CreateConfigurationRequest
    {
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Value { get; set; } = default!;
        public bool IsActive { get; set; }
        public string ApplicationName { get; set; } = default!;
    }
}
