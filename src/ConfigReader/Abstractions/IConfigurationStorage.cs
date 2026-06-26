using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigReader.Models;


namespace ConfigReader.Abstractions
{

    /// ConfigurationReader kütüphanesinin storage katmanı için soyutlama sağlayan sözleşmedir
    /// Farklı storage sağlayıcılarının (MongoDB, Redis, SQL Server, dosya sistemi vb.) aynı arayüz üzerinden kullanılabilmesini sağlıyor
    internal interface IConfigurationStorage
    {
        Task<IReadOnlyCollection<ConfigurationItem>> GetActiveConfigurationsAsync(string applicationName, CancellationToken cancellationToken = default);
    }
}
