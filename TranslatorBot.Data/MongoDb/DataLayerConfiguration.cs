using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using TranslatorBot.Entities.Domain;

namespace TranslatorBot.Data.MongoDb;

public static class DataLayerConfiguration {
    private static readonly object _locker = new();
    public static bool Configured { get; private set; }

    public static IMongoDatabase GetDatabase(string connectionString) {
        var url = MongoUrl.Create(connectionString);
        var client = new MongoClient(connectionString);
        return client.GetDatabase(url.DatabaseName);
    }

    public static void Configure() {
        if (Configured) {
            return;
        }

        lock (_locker) {
            if (Configured) return;
            ConfigureInternal();
            Configured = true;
        }
    }

    private static void ConfigureInternal() {
        BsonClassMap.RegisterClassMap<YandexCloudToken>(cm => {
            cm.AutoMap();
            cm.MapProperty(x => x.Token).SetIgnoreIfDefault(true);
        });
    }
}