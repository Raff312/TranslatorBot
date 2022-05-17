using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TranslatorBot.Data.MongoDb;
using TranslatorBot.Services.Infrastructure;

namespace TranslatorBot.JobManager;

public class Program {
    private static IConfigurationRoot? _configuration;

    public static async Task Main() {
        using var host = CreateHostBuilder().Build();
        _configuration?.UseHangfire(host.Services);
        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder() {
        return Host.CreateDefaultBuilder()
            .ConfigureHostConfiguration(config => {
                _configuration = config.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFiles()
                    .Build();

                DataLayerConfiguration.Configure();
            })
            .ConfigureServices((hostedContext, services) => {
                services.RegisterServices(_configuration!);
                services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Debug));
                services.RegisterHostedServices();
            })
            .UseDefaultServiceProvider((context, options) => {
                options.ValidateOnBuild = false;
            })
            .ConfigureLogging((hostContext, builder) => {
                builder.AddConsole();
            });
    }
}
