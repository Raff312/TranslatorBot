using Microsoft.Extensions.Logging;
using TranslatorBot.JobManager.Infrastructure;

namespace TranslatorBot.JobManager.Jobs;

public abstract class BaseJob {
    protected readonly ILogger Logger;

    protected BaseJob(ILoggerFactory loggerFactory) {
        Logger = loggerFactory.CreateLogger("job");
    }
}

public abstract class BaseDurableJob : BaseJob {
    protected BaseDurableJob(ILoggerFactory loggerFactory) : base(loggerFactory) {
	}

    [JobTimeFilter]
    public async Task Execute() {
        Logger.LogInformation("{Type} is started", GetType());
        await ExecuteInternal();
        Logger.LogInformation("{Type} is finished", GetType());
    }

    protected abstract Task ExecuteInternal();
}