using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace TranslatorBot.JobManager.Infrastructure;

public class CoreJobActivator : JobActivator {
    private readonly IServiceProvider _provider;

    public CoreJobActivator(IServiceProvider provider) {
        _provider = provider;
    }

    public override object ActivateJob(Type type) {
        return _provider.GetRequiredService(type);
    }
}