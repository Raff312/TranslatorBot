using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TranslatorBot.Data.MongoDb;
using TranslatorBot.Services.Infrastructure;
using TranslatorBot.Services.Infrastructure.IoC;

namespace TranslatorBot.Initializer;

public static class Startup {
    public static IServiceProvider ServiceProvider { get; private set; } = default!;
    public static IConfigurationRoot Configuration { get; private set; } = default!;

    public static void Configure() {
        DataLayerConfiguration.Configure();

        BuildConfiguration();

        ServiceProvider = RegisterServices().BuildServiceProvider(new ServiceProviderOptions {
            ValidateOnBuild = true
        });
    }

    private static void BuildConfiguration() {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFiles();

        Configuration = builder.Build();
    }

    private static IServiceCollection RegisterServices() {
        var services = new ServiceCollection();

        services
            .AddLogging().Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug)
            .AddSingleton(Configuration.GetSection("Database").Get<MongoDatabaseOptions>())
            .AddMongoDb()
            .AddRepositories();

        services.AddLogging(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        return services;
    }
}