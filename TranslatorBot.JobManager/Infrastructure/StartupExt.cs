using Microsoft.Extensions.DependencyInjection;
using TranslatorBot.JobManager.Jobs;

namespace TranslatorBot.JobManager.Infrastructure;

public static class StartupExt {
    public static IServiceCollection RegisterJobs(this IServiceCollection services) {
        var marker = typeof(BaseJob);
        var implementations = typeof(BaseJob).Assembly.GetTypes().Where(x => marker.IsAssignableFrom(x) && x.IsClass && !x.IsAbstract);

        foreach (var implementation in implementations) {
            services.AddTransient(implementation);
        }

        return services;
    }
}