using ConfigReader;

const string applicationName = "SERVICE-A";
const string connectionString = "mongodb://localhost:27017/ConfigReaderDb";
const int refreshIntervalInMs = 3000;

using var configurationReader = new ConfigurationReader(applicationName, connectionString, refreshIntervalInMs);

Console.WriteLine("SERVICE-A Console Demo başlatıldı.");
Console.WriteLine("MongoDB üzerinde SiteName değerini lütfen değiştirelim.");
Console.WriteLine("Uygulama restart olmadan yeni değeri okuyacak.");
Console.WriteLine();

while (true)
{
    try
    {
        var siteName = configurationReader.GetValue<string>("SiteName");

        Console.WriteLine($"{DateTime.Now:HH:mm:ss} | SiteName: {siteName}");

    }
    catch (Exception ex)
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} | Error: {ex.Message}");
    }

    await Task.Delay(2000);
}