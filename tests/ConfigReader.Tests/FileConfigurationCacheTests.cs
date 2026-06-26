using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigReader.Cache;
using ConfigReader.Models;
using FluentAssertions;

namespace ConfigReader.Tests;


/// FileConfigurationCache sınıfının dosyaya yazma ve dosyadan okuma işlemlerini doğrulayan unit testlerini içeriyor
public class FileConfigurationCacheTests
{

    /// Kaydedilen konfigürasyonların dosyaya başarılı şekilde yazıldığını doğrular

    [Fact]
    public async Task SaveAsync_And_LoadAsync_Should_Return_Same_Configurations()
    {
        var applicationName = $"TEST-SERVICE-{Guid.NewGuid()}";
        var cache = new FileConfigurationCache(applicationName);

        var configurations = new List<ConfigurationItem>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "SiteName",
                Type = "string",
                Value = "soty.io",
                IsActive = true,
                ApplicationName = applicationName,
                Version = 1,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            }
        };

        await cache.SaveAsync(configurations);

        var result = await cache.LoadAsync();

        result.Should().HaveCount(1);
        result.First().Name.Should().Be("SiteName");
        result.First().Value.Should().Be("soty.io");
        result.First().ApplicationName.Should().Be(applicationName);
    }

    /// Daha önce kaydedilen konfigürasyonların dosyadan başarılı şekilde okunabildiğini doğrular

    [Fact]
    public async Task LoadAsync_Should_Return_Empty_List_When_Cache_File_Does_Not_Exist()
    {
        var applicationName = $"NON_EXISTING_SERVICE_{Guid.NewGuid()}";
        var cache = new FileConfigurationCache(applicationName);

        var result = await cache.LoadAsync();

        result.Should().BeEmpty();
    }
}