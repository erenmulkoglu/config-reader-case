# Dinamik konfigürasyon Okuyucu (Dynamic Configuration Reader)

## Projemizin Amacı

Bu proje, .NET uygulamalarında kullanılan statik konfigürasyon dosyaları (web.config, app.config, appsettings.json vb.) yerine merkezi ve dinamik bir konfigürasyon yönetim sistemi oluşturmak amacıyla geliştirilmiştir.

Konfigürasyon kayıtları MongoDB üzerinde tutulur ve uygulama çalışırken değiştirilebilir. Böylece deployment, restart veya recycle işlemi gerekmeksizin uygulamalar güncel konfigürasyon değerlerini kullanabilir.

# Kullanılan Teknolojiler

- .NET 8
- ASP.NET Core Web API
- React
- MongoDB
- RabbitMQ
- Docker Compose
- xUnit
- FluentAssertions
- Moq
- async/await


# Proje Yapısı

```text
src/

ConfigReader
    Dinamik konfigürasyon kütüphanesi (DLL)

ConfigReader.Api
    Konfigürasyon yönetim API'si

ConfigReader.Web
    React tabanlı yönetim ekranı

samples/

ServiceA.Console
    DLL kullanım örneği

tests/

ConfigReader.Tests
    Unit Test Projesi
```

---

# Konfigürasyon Modeli

Her kayıt aşağıdaki alanlardan oluşmaktadır

| Alan | Açıklama |
|------|----------|
| Id | Kayıt Id'si |
| Name | Konfigürasyon anahtarı |
| Type | Veri tipi |
| Value | Değer |
| IsActive | Aktif/Pasif |
| ApplicationName | Uygulama adı |

Desteklenen veri tipleri:

- string
- int
- double
- bool

---

# ConfigurationReader Kullanımı

```csharp
using ConfigReader;

var configurationReader = new ConfigurationReader(
    "SERVICE-A",
    "mongodb://localhost:27017/ConfigReaderDb",
    5000);

string siteName =
    configurationReader.GetValue<string>("SiteName");
```

---

# Dinamik Güncelleme

ConfigurationReader belirlenen süre aralıklarında MongoDB'yi kontrol eder.

```csharp
refreshTimerIntervalInMs = 5000;
```

Yeni kayıt eklenmesi veya mevcut kayıtların değiştirilmesi durumunda uygulama yeniden başlatılmadan yeni değerler okunabilir

---

# Uygulama Bazlı İzolasyon

Her servis yalnızca kendi konfigürasyon kayıtlarına erişebilir

Örnek olarak:

```text
SERVICE-A
↓

ApplicationName = SERVICE-A
```

SERVICE-B kayıtlarını göremez.

---

# IsActive Kuralı

DLL yalnızca

```text
IsActive = true
```

olan kayıtları döndürür

Pasif kayıtlar yönetim ekranında görüntülenebilir ancak GetValue<T>() tarafından okunamaz

---

# Offline Cache

Case gereksinimi doğrultusunda storage erişilemediği durumlarda kütüphane son başarılı konfigürasyon kayıtları ile çalışmaya devam eder

1) MongoDB çalışırken konfigürasyon okundu

2) MongoDB durduruldu

3) Uygulama son başarılı cache ile çalışmaya devam eder

Sonuç:

Storage erişilemediğinde uygulama çalışmasını sürdürmektedir


Bu amaçla iki seviyeli cache yapısı uygulanmıştır:

### 1. Runtime Cache

Uygulama çalışırken MongoDB erişilemez hale gelirse RAM üzerinde bulunan son başarılı konfigürasyon kayıtları kullanılmaya devam edilir

```text
MongoDB

↓

Memory Cache

↓

GetValue<T>()
```

---

### 2. File Cache

Uygulama yeniden başlatıldığında MongoDB erişilemiyorsa, önceki başarılı kayıtlar

```text
cache/{ApplicationName}.cache.json
```

dosyasından okunur

```text
MongoDB

↓

cache.json

↓

Memory Cache

↓

GetValue<T>()
```

Bu sayede sistem storage erişilemese dahi çalışmaya devam eder

---

# REST API

Sunulan servisler

| Method | Endpoint |
|---------|----------|
| GET | /api/configurations |
| POST | /api/configurations |
| PUT | /api/configurations/{id} |
| DELETE | /api/configurations/{id} |

API üzerinden;

- kayıt listeleme
- kayıt ekleme
- kayıt güncelleme
- kayıt silme

işlemleri gerçekleştirilebilir

---

# React Yönetim Paneli

Web arayüzü aşağıdaki işlemleri desteklemektedir:

- Konfigürasyon kayıtlarını listeleme
- Yeni kayıt ekleme
- Mevcut kayıt güncelleme
- Client-side isim filtreleme

---

# RabbitMQ Event Publishing

Configuration kayıtlarında yapılan değişiklikler event olarak RabbitMQ'ya yayınlanmaktadır.

Aşağıdaki işlemler event üretir:

- Configuration oluşturma
- Configuration güncelleme
- Configuration silme

Routing Key formatı

configuration.changed.{ApplicationName}

Örnek

configuration.changed.SERVICE-A

Event Payload

- Id
- Name
- Type
- Value
- IsActive
- ApplicationName
- Version
- Operation
- OccurredAtUtc

---

# Docker Compose

Tüm sistem aşağıdaki komut ile ayağa kaldırılabilir:

```bash
docker compose up --build
```

Servisler

| Servis | Adres |
|---------|-------|
| MongoDB | localhost:27017 |
| API | http://localhost:7193 |
| React | http://localhost:5173 |

---

# Unit Test

Unit testleri çalıştırmak için

```bash
dotnet test
```

Mevcut Unit Testler

- ConfigurationValueConverter
- FileConfigurationCache
- ConfigurationAdminService Delete

Yeni özellik geliştirilirken Test Driven Development (TDD) yaklaşımı uygulanmıştır

Sürecimiz:

1. Önce başarısız test yazıldı

2. İlgili iş mantığı geliştirildi

3. Testler başarılı hale getirildi

4. Refactor yapıldı

Mevcut testler aşağıdaki veri tiplerini kapsamaktadır

- string
- int
- double
- bool

---

# Manuel Test Senaryoları

Dinamik Refresh

- React üzerinden SiteName değeri güncellendi
- Console uygulaması restart olmadan yeni değeri okumaya başladı

Sonuç: Başarılı

RabbitMQ

- Güncelleme sonrasında RabbitMQ Queue içerisine Updated event düştü

Sonuç: Başarılı

Offline Cache

- MongoDB durduruldu
- Console uygulaması son başarılı cache ile çalışmaya devam etti

Sonuç: Başarılı



---



# Case Gereksinimlerinin Karşılanması

| Gereksinim | Durum |
|------------|--------|
| .NET 8 DLL | Yapıldı |
| GetValue<T>() | Yapıldı |
| Dinamik Refresh | Yapıldı |
| Restart Gerektirmemesi | Yapıldı |
| MongoDB Storage | Yapıldı |
| IsActive Filtreleme | Yapıldı |
| Application Bazlı İzolasyon | Yapıldı |
| Storage Erişilemezse Çalışmaya Devam Etme | Yapıldı |
| Web Yönetim Paneli | Yapıldı |
| Yeni Kayıt Ekleme | Yapıldı |
| Güncelleme | Yapıldı |
| Client-side Filtreleme | Yapıldı |
| Docker Compose | Yapıldı |
| Unit Test | Yapıldı |
| Dokümantasyon | Yapıldı |
| RabbitMQ Event Publishing | Yapıldı |
| TDD Yaklaşımı | Yapıldı |
| Delete API | Yapıldı |
| Offline Cache | Yapıldı |
| Runtime Cache | Yapıldı |
| File Cache | Yapıldı |



# Mimari Kararlar

Bu projede aşağıdaki prensipler uygulanmıştır:

- SOLID prensipleri uygulandı
- Dependency Inversion yapıldı
- Repository benzeri storage soyutlaması yapıldı
- Thread-safe in-memory cache (ConcurrentDictionary) yapıldı
- Memory + File Cache
- Event Driven Communication (RabbitMQ)
- Async/Await tabanlı I/O asenkron programlama işlemleri gerçekleştirildi
- Generic tip dönüşümü (GetValue<T>) yapıldı
- İki seviyeli cache mekanizması (Memory + File) işlemleri sağlandı