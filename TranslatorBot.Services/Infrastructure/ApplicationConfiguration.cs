using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace TranslatorBot.Services.Infrastructure;

public static class ApplicationConfiguration {
    public static IConfigurationBuilder AddJsonFiles(this IConfigurationBuilder configuration) {
        var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (location != null) {
            configuration.AddJsonFile(Path.Combine(location, "shared-settings.json"), optional: false, reloadOnChange: true);
            configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        }

        return configuration;
    }
}