using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigReader.Models;


namespace ConfigReader.Abstractions
{
    internal interface IConfigurationStorage
    {
        Task<IReadOnlyCollection<ConfigurationItem>> GetActiveConfigurationsAsync(string applicationName, CancellationToken cancellationToken = default);
    }
}
