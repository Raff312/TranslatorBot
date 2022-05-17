using Microsoft.Extensions.Logging;
using TranslatorBot.Services.Business;

namespace TranslatorBot.JobManager.Jobs;

public class YandexIamTokenUpdateJob : BaseDurableJob {
    private readonly IYandexService _yandexService;

    public YandexIamTokenUpdateJob(
        IYandexService yandexService,
        ILoggerFactory loggerFactory
    ) : base(loggerFactory) {
        _yandexService = yandexService;
    }

    protected override Task ExecuteInternal() {
        return _yandexService.UpdateOrCreateIamToken();
    }
}