namespace ConfigReader.Api.Models
{
    public sealed class MongoDbOptions
    {
        public string ConnectionString { get; set; } = default!;
        public string DatabaseName { get; set; } = default!;
        public string CollectionName { get; set; } = default!;
    }
}
