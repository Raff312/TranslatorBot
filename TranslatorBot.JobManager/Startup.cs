using Hangfire;
using Hangfire.Logging.LogProviders;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Telegram.Bot;
using TranslatorBot.Data.MongoDb;
using TranslatorBot.Entities.Options;
using TranslatorBot.JobManager.HostedServices;
using TranslatorBot.JobManager.Infrastructure;
using TranslatorBot.Services.Business;
using TranslatorBot.Services.Infrastructure.IoC;
using TranslatorBot.Services.Integrations.Yandex;

namespace TranslatorBot.JobManager;

public static class Startup {
    public static void UseHangfire(this IConfigurationRoot config, IServiceProvider serviceProvider) {
        GlobalConfiguration.Configuration.UseLogProvider(new ColouredConsoleLogProvider());
        var mongoUrlBuilder = new MongoUrlBuilder(config["Database:ConnectionString"]);
        var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());

        var storageOptions = new MongoStorageOptions {
            MigrationOptions = new MongoMigrationOptions {
                MigrationStrategy = new MigrateMongoMigrationStrategy(),
                BackupStrategy = new CollectionMongoBackupStrategy()
            },
            QueuePollInterval = TimeSpan.FromSeconds(15),
            CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
        };
        GlobalConfiguration.Configuration.UseMongoStorage(mongoClient, "translatorBot-jobs", storageOptions);
        GlobalConfiguration.Configuration.UseActivator(new CoreJobActivator(serviceProvider));
    }

    public static void RegisterServices(this IServiceCollection services, IConfigurationRoot config) {
        services
            .AddSingleton(x => config)
            .AddSingleton(typeof(IMongoDatabase), x => DataLayerConfiguration.GetDatabase(config["Database:ConnectionString"]))
            .AddSingleton(config.GetSection("BotConfiguration").Get<BotConfiguration>())
            .AddSingleton(config.GetSection("YandexTranslateOptions").Get<YandexTranslateOptions>())
            .AddSingleton(config.GetSection("Database").Get<MongoDatabaseOptions>())
            .AddMongoDb()
            .AddRepositories()
            .RegisterJobs()
            .AddSingleton<IBackgroundJobScheduler>(x => new BackgroundJobScheduler(x));

        services
            .AddHttpClient("tg")
            .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(config["BotConfiguration:Token"], httpClient));

        services
            .AddHttpClient("ynd")
            .AddTypedClient<IYandexClient, YandexClient>();

        services.AddSingleton<IYandexService, YandexService>();
    }

    public static void RegisterHostedServices(this IServiceCollection services) {
        services.AddHostedService<HangfireHostedService>();
        services.AddHostedService<ConfigureWebhookHostedService>();
    }
}