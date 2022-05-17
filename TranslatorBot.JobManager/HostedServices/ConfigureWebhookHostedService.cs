using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TranslatorBot.Entities.Options;

namespace TranslatorBot.JobManager.HostedServices;

public class ConfigureWebhookHostedService : IHostedService {
    private readonly ILogger<ConfigureWebhookHostedService> _logger;
    private readonly IServiceProvider _services;
    private readonly BotConfiguration _botConfiguration;

    public ConfigureWebhookHostedService(
        ILogger<ConfigureWebhookHostedService> logger,
        IServiceProvider services,
        BotConfiguration botConfiguration
    ) {
        _logger = logger;
        _services = services;
        _botConfiguration = botConfiguration;
    }

    public async Task StartAsync(CancellationToken cancellationToken) {
        using var scope = _services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        var webhookAddress = @$"{_botConfiguration.HostAddress}/bot/{_botConfiguration.Token}";
        _logger.LogInformation("Setting webhook: {webhookAddress}", webhookAddress);
        await botClient.SetWebhookAsync(
            url: webhookAddress,
            allowedUpdates: Array.Empty<UpdateType>(),
            cancellationToken: cancellationToken
        );
    }

    public async Task StopAsync(CancellationToken cancellationToken) {
        using var scope = _services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        _logger.LogInformation("Removing webhook");
        await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
    }
}