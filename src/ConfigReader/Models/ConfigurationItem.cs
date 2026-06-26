using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigReader.Models
{

    /// ConfigurationReader tarafından kullanılan konfigürasyon kaydını temsil ediyor
    /// Storage üzerinden okunan veriler bu model ile bellekte tutar
    public sealed class ConfigurationItem
    {
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
