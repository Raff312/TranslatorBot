using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using TranslatorBot.Data.MongoDb;
using TranslatorBot.Data.MongoDb.Repositories;
using TranslatorBot.Data.MongoDb.Repositories.Common;
using TranslatorBot.Entities.Data;

namespace TranslatorBot.Services.Infrastructure.IoC;

public static class RegistratioinHelpers {
    public static IServiceCollection AddMongoDb(this IServiceCollection services) {
        return services
            .AddSingleton(typeof(IMongoDatabase), x => {
                var opts = x.GetService<MongoDatabaseOptions>();
                return DataLayerConfiguration.GetDatabase(opts!.ConnectionString!);
            })
            .AddSingleton<IDataContext, MongoDbDataContext>();
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services) {
        services.AddScoped<IYandexCloudTokenRepository, YandexCloudTokenRepository>();
        return services;
    }

    public static IServiceCollection RegisterTransientByConvention<TMarker>(this IServiceCollection services, bool scoped = false) {
        return services.RegisterTransientByConvention<TMarker>(typeof(TMarker).Assembly, scoped);
    }

    public static IServiceCollection RegisterTransientByConvention<TMarker>(this IServiceCollection services, Assembly assembly, bool scoped = false) {
        var marker = typeof(TMarker);
        var impl = assembly.GetTypes().Where(x => marker.IsAssignableFrom(x)).ToArray();

        foreach (var type in marker.Assembly.GetTypes().Where(x => marker.IsAssignableFrom(x))) {
            var implName = type.Name[1..];

            var implementation = impl.FirstOrDefault(x => x.Name == implName);
            if (implementation != null) {
                if (scoped) {
                    services.AddScoped(type, implementation);
                } else {
                    services.AddTransient(type, implementation);
                }
            }
        }
        return services;
    }
}

public class MongoDatabaseOptions {
    public string? ConnectionString { get; set; }
}