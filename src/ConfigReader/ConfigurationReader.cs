using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using ConfigReader.Abstractions;
using ConfigReader.Converters;
using ConfigReader.Exceptions;
using ConfigReader.Models;
using ConfigReader.Storage;
using ConfigReader.Cache;

namespace ConfigReader
{

    /// Merkezi storage üzerinden konfigürasyon değerlerini okuyan ana kütüphanemiz
    /// Uygulama bazlı izolasyon, periyodik refresh, memory cache ve file cache fallback mekanizmalarını yönetiyor
    public sealed class ConfigurationReader : IDisposable
    {
        private readonly string _applicationName;
        private readonly IConfigurationStorage _storage;
        private readonly PeriodicTimer _timer;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly FileConfigurationCache _fileCache;

        private ConcurrentDictionary<string, ConfigurationItem> _cache = new(StringComparer.OrdinalIgnoreCase);

        private readonly Task _refreshTask;


    /// ConfigurationReader örneğini verilen uygulama adı, storage bağlantısı ve refresh süresi ile başlatır
    /// İlk başarılı okumayı senkron olarak yapar ve arka planda periyodik refresh sürecini başlatır
        public ConfigurationReader(string applicationName, string connectionString, int refreshTimerIntervalInMs)
        {
            if (string.IsNullOrWhiteSpace(applicationName))
                throw new ArgumentException("Uygulama ismi boş olamaz.", nameof(applicationName));

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string boş olamaz.", nameof(connectionString));

            if (refreshTimerIntervalInMs <= 0)
                throw new ArgumentException("Yenileme zamanlayıcısı aralığı sıfırdan büyük olmalıdır.", nameof(refreshTimerIntervalInMs));

            _applicationName = applicationName.Trim();
            _storage = new MongoConfigurationStorage(connectionString);
            _fileCache = new FileConfigurationCache(_applicationName);
            _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(refreshTimerIntervalInMs));

            RefreshAsync(_cancellationTokenSource.Token).GetAwaiter().GetResult();

            _refreshTask = Task.Run(() => RefreshPeriodicallyAsync(_cancellationTokenSource.Token));
        }


        /// Verilen konfigürasyon anahtarını cache üzerinden okuyarak istenen tipe dönüştürür
        /// Değer bulunamazsa ConfigurationNotFoundException fırlatır
        public T GetValue<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Configuration key boş olamaz.", nameof(key));

            if (!_cache.TryGetValue(key.Trim(), out var item))
                throw new ConfigurationNotFoundException(key);

            return ConfigurationValueConverter.Convert<T>(key, item.Value);
        }

        /// Parametrik olarak verilen süre aralığında storage'ı kontrol eder
        /// Yeni eklenen veya değişen konfigürasyonları memory cache'e taşır
        private async Task RefreshPeriodicallyAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (await _timer.WaitForNextTickAsync(cancellationToken))
                {
                    await RefreshAsync(cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        /// Storage üzerinden aktif konfigürasyonları okur
        /// Başarılı okuma sonrası memory cache ve file cache güncellenir
        /// Storage erişilemezse önce mevcut memory cache, memory cache boşsa file cache kullanılır
        private async Task RefreshAsync(CancellationToken cancellationToken)
        {
            try
            {
                var configurations = await _storage.GetActiveConfigurationsAsync(
                    _applicationName,
                    cancellationToken);

                SetCache(configurations);

                await _fileCache.SaveAsync(
                    configurations,
                    cancellationToken);
            }
            catch
            {
                if (!_cache.IsEmpty)
                    return;

                var cachedConfigurations = await _fileCache.LoadAsync(
                    cancellationToken);

                if (cachedConfigurations.Any())
                    SetCache(cachedConfigurations);
            }
        }


        /// Gelen konfigürasyon listesini thread-safe ConcurrentDictionary yapısına dönüştürür
        /// Cache değişimi atomik olarak yapıldığı için okuma yapan thread'ler tutarlı snapshot görür
        private void SetCache(IReadOnlyCollection<ConfigurationItem> configurations)
        {
            _cache = new ConcurrentDictionary<string, ConfigurationItem>(configurations.ToDictionary(
                    x => x.Name,
                    x => x,
                    StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase);
        }

        /// Arka plan refresh işlemini durdurur ve kullanılan kaynakları temizler
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _timer.Dispose();
            _cancellationTokenSource.Dispose();
        }
    }
}
