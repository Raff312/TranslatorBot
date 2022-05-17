using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TranslatorBot.JobManager.Jobs;

namespace TranslatorBot.JobManager.Infrastructure;

public interface IBackgroundJobScheduler {
    void ScheduleJobs(IEnumerable<JobConfiguration> jobConfigs);
}

public class BackgroundJobScheduler : IBackgroundJobScheduler {
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _log;

    public BackgroundJobScheduler(IServiceProvider serviceProvider) {
        _serviceProvider = serviceProvider;
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
		_log = loggerFactory.CreateLogger("job-scheduler");
    }

    public void ScheduleJobs(IEnumerable<JobConfiguration>? jobConfigs) {
        if (jobConfigs == null) {
            return;
        }

        foreach (var jobInfo in jobConfigs) {
            var jobType = jobInfo.JobType;
            if (jobType == null) {
                _log.LogError("Can't find type for job {JobType}", jobInfo.Type);
                continue;
            }

            var job = (BaseDurableJob?)_serviceProvider.GetService(jobType);
            if (job == null) {
                _log.LogError("Can't create instance job {JobType}", jobInfo.Type);
                continue;
            }

            if (!string.IsNullOrWhiteSpace(jobInfo.Cron)) {
                RecurringJob.AddOrUpdate(() => job.Execute(), jobInfo.Cron);
            } else {
                BackgroundJob.Schedule(() => job.Execute(), TimeSpan.FromMilliseconds(1));
            }
        }
    }
}