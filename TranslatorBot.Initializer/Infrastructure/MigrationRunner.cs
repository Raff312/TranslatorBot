using System.Reflection;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDBMigrations;
using MongoDBMigrations.Core;
using MongoDBMigrations.Document;

namespace TranslatorBot.Initializer.Infrastructure;

public class MigrationRunner {
    private readonly IMigrationRunner _runner;
    private readonly MongoUrl _url;

    public MigrationRunner(IConfigurationRoot configuration) {
        var connectionString = configuration["Database:ConnectionString"];
        _url = MongoUrl.Create(connectionString);

        _runner = new MigrationEngine()
            .UseDatabase(connectionString, _url.DatabaseName)
            .UseAssembly(Assembly.GetExecutingAssembly())
            .UseSchemeValidation(false)
            .UseProgressHandler(ProgressHandler);
    }

    private static void ProgressHandler(InterimMigrationResult result) {
        Console.WriteLine($"{result.CurrentNumber} -> {result.TargetVersion}: {result.MigrationName}");
    }

    public Task Run(string? version) {
        if (version == "-c" || version == "-current") {
            Console.WriteLine(GetCurrentDbVersion());
            return Task.CompletedTask;
        } else if (version == "-l" || version == "-latest") {
            Console.WriteLine(GetRequiredDbVersion());
            return Task.CompletedTask;
        }

        var result = string.IsNullOrWhiteSpace(version) ? _runner.Run() : _runner.Run(version);

        Console.WriteLine($"Migration is done. Executed: {result.InterimSteps.Count}, current version: {result.CurrentVersion}");
        return Task.CompletedTask;
    }

    private static string? GetRequiredDbVersion() {
        return Assembly.GetExecutingAssembly()
            .GetTypes().Where(x => x.IsClass && !x.IsAbstract && typeof(IMigration).IsAssignableFrom(x))
            .Select(x => (IMigration)Activator.CreateInstance(x)!)
            .Select(x => x!.Version)
            .OrderBy(x => x)
            .LastOrDefault().ToString();
    }

    private string GetCurrentDbVersion() {
        var database = new MongoClient(_url).GetDatabase(_url.DatabaseName);
        var manager = new DatabaseManager(database, MongoEmulationEnum.None);
        return manager.GetVersion().ToString();
    }
}