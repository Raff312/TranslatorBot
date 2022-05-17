using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TranslatorBot.Entities.Options;
using TranslatorBot.Services.Integrations.Yandex;
using TranslatorBot.Services.Utils;

namespace TranslatorBot.Services.Business;

public class HandleUpdateService : IHandleUpdateService {
    private readonly ILogger<HandleUpdateService> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly IYandexClient _yandexClient;
    private readonly YandexTranslateOptions _yandexTranslateOptions;

    private readonly int MAX_DETECT_LANG_TEXT_LENGTH = 999;
    private readonly int MAX_MESSAGE_LENGTH = 4096;

    public HandleUpdateService(
        ILogger<HandleUpdateService> logger,
        ITelegramBotClient botClient,
        IYandexClient yandexClient,
        YandexTranslateOptions yandexTranslateOptions
    ) {
        _logger = logger;
        _botClient = botClient;
        _yandexClient = yandexClient;
        _yandexTranslateOptions = yandexTranslateOptions;
    }

    public async Task Handle(Update update) {
        var handler = GetHandler(update);

        try {
            await handler;
        } catch (Exception ex) {
            await HandleErrorAsync(ex);
        }
    }

    private Task GetHandler(Update update) {
        return update.Type switch {
            UpdateType.Message => OnMessageReceived(update.Message),
            _ => OnUnknownUpdateReceived(update)
        };
    }

    private async Task OnMessageReceived(Message? message) {
        if (message == null) {
            return;
        }

        _logger.LogInformation("Received message type: {messageType}", message.Type);

        var messageTexts = new List<string?> { message.Text, message.Caption };
        var sentMessages = await SendTextMessagesAsync(message.Chat.Id, messageTexts);

        foreach (var sendMessage in sentMessages) {
            _logger.LogInformation("The message was sent with id: {sentMessageId}", sendMessage?.MessageId);
        }
    }

    private async Task<List<Message>> SendTextMessagesAsync(long chatId, List<string?> texts) {
        var messages = new List<Message>();
        foreach (var text in texts.Where(x => !string.IsNullOrEmpty(x))) {
            messages.AddRange(await SendTextMessageAsync(chatId, text!));
        }

        return messages;
    }

    private async Task<List<Message>> SendTextMessageAsync(long chatId, string text) {
        var language = await GetLanguageAsync(text);
        var translation = await TranslateAsync(language, text);
        var translatedText = translation?.Translations?.FirstOrDefault()?.Text;

        if (string.IsNullOrEmpty(translatedText)) {
            throw new Exception("Translated text is null or empty");
        }

        var textParts = TextUtil.SplitText(translatedText, MAX_MESSAGE_LENGTH);

        var messages = new List<Message>();
        foreach (var textPart in textParts) {
            messages.Add(await _botClient.SendTextMessageAsync(chatId: chatId, text: textPart));
        }

        return messages;
    }

    private Task<Language?> GetLanguageAsync(string text) {
        var detectLanguageRequest = new DetectLanguageRequest {
            Text = string.Concat(text.Take(MAX_DETECT_LANG_TEXT_LENGTH)),
            LanguageCodeHints = new string[] { "ru", "en" },
            FolderId = _yandexTranslateOptions.FolderId
        };

        return _yandexClient.DetectLanguageAsync(detectLanguageRequest);
    }

    private Task<TranslateResult?> TranslateAsync(Language? language, string text) {
        var translationRequest = new TranslateRequest {
            SourceLanguageCode = language?.Code,
            TargetLanguageCode = GetOppositeLanguageCode(language?.Code),
            Format = "PLAIN_TEXT",
            Texts = new string[] { text },
            FolderId = _yandexTranslateOptions.FolderId
        };

        return _yandexClient.TranslateAsync(translationRequest);
    }

    private static string GetOppositeLanguageCode(string? code) {
        return code == "ru" ? "en" : "ru";
    }

    private Task OnUnknownUpdateReceived(Update update) {
        _logger.LogInformation("Unknown update type: {updateType}", update.Type);
        return Task.CompletedTask;
    }

    private Task HandleErrorAsync(Exception ex) {
        var errorMessage = ex switch {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => ex.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", errorMessage);
        return Task.CompletedTask;
    }
}