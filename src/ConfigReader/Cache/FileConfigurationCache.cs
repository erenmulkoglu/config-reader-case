using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using ConfigReader.Models;

namespace ConfigReader.Cache;

public sealed class FileConfigurationCache
{
    private readonly string _filePath;



    /// Son başarılı konfigürasyon kayıtlarını dosya sisteminde saklar
    /// Storage erişilemediğinde uygulamanın son başarılı snapshot ile ayağa kalkabilmesini sağlar
    public FileConfigurationCache(string applicationName)
    {
        var safeApplicationName = applicationName.Replace(" ", "_");

        var cacheDirectory = Path.Combine(AppContext.BaseDirectory, "cache");

        Directory.CreateDirectory(cacheDirectory);

        _filePath = Path.Combine(cacheDirectory, $"{safeApplicationName}.cache.json");
    }


    /// Başarılı okunan konfigürasyon kayıtlarını uygulama adına özel cache dosyasına yazar
    public async Task SaveAsync(IReadOnlyCollection<ConfigurationItem> configurations, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(configurations, new JsonSerializerOptions
            {
                WriteIndented = true
            });

        await File.WriteAllTextAsync(_filePath, json, cancellationToken);
    }


    /// Uygulama adına ait cache dosyasını okuyarak son başarılı konfigürasyon kayıtlarını döndürür
    /// Cache dosyası yoksa boş liste döner
    public async Task<IReadOnlyCollection<ConfigurationItem>> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
            return Array.Empty<ConfigurationItem>();

        var json = await File.ReadAllTextAsync(_filePath, cancellationToken);

        if (string.IsNullOrWhiteSpace(json))
            return Array.Empty<ConfigurationItem>();

        return JsonSerializer.Deserialize<List<ConfigurationItem>>(json)
        ?? new List<ConfigurationItem>();
    }
}