using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConfigReader.Documents
{

    /// ConfigurationReader kütüphanesi tarafından kullanılan konfigürasyon dokümanını temsil ediyor
    /// Storage katmanından okunan kayıtlar bu model ile taşınıyor
    internal sealed class ConfigurationDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Value { get; set; } = default!;
        public bool IsActive { get; set; }
        public string ApplicationName { get; set; } = default!;
        public long Version { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
