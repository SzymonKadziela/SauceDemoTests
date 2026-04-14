using Microsoft.Extensions.Configuration;
using System.IO;

namespace SauceDemoTests.Configuration
{
    public static class ConfigReader
    {
        public static TestDataConfig GetConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            var config = configuration.GetSection("TestData").Get<TestDataConfig>();
            return config ?? throw new System.InvalidOperationException(
                "Błąd krytyczny: Sekcja 'TestData' nie została znaleziona w appsettings.json!");
        }
    }
}