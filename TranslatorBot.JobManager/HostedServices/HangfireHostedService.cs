using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TranslatorBot.JobManager.Infrastructure;

namespace TranslatorBot.JobManager.HostedServices;

public class HangfireHostedService : IHostedService, IDisposable {
    private readonly IBackgroundJobScheduler _runner;
    private readonly IConfigurationRoot _configuration;
    private BackgroundJobServer? _hangfire;
    private readonly ILogger _logger;

    public HangfireHostedService(IBackgroundJobScheduler runner, IConfigurationRoot configuration, ILoggerFactory loggerFactory) {
        _runner = runner;
        _configuration = configuration;
        _logger = loggerFactory.CreateLogger("job-service");
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        var jobs = _configuration.GetSection("RecurrentJobs").Get<JobConfiguration[]>();
        _runner.ScheduleJobs(jobs);

        _logger.LogInformation("Starting, recurring jobs: {JobsLength}", jobs?.Length ?? 0);

        _hangfire = new BackgroundJobServer(new BackgroundJobServerOptions {
            HeartbeatInterval = TimeSpan.FromSeconds(10),
            SchedulePollingInterval = TimeSpan.FromSeconds(10),
            WorkerCount = 50,
            ServerName = "bot.jm-0"
        }, JobStorage.Current);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("Stopping");
        if (_hangfire != null) {
            _hangfire.SendStop();
            await _hangfire.WaitForShutdownAsync(cancellationToken);
        }

        _logger.LogInformation("Stopped");
    }

    public void Dispose() {
        _hangfire?.Dispose();
        GC.SuppressFinalize(this);
    }
}