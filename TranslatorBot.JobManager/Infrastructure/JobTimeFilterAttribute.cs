using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;

namespace TranslatorBot.JobManager.Infrastructure;

public class JobTimeFilterAttribute : JobFilterAttribute, IApplyStateFilter {
    private readonly int _expirationMinutes;

    public JobTimeFilterAttribute(int expirationMinutes = 30) {
        _expirationMinutes = expirationMinutes;
    }

    public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction) {
        context.JobExpirationTimeout = TimeSpan.FromMinutes(_expirationMinutes);
    }

    public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction) {
        context.JobExpirationTimeout = TimeSpan.FromMinutes(_expirationMinutes);
    }
}