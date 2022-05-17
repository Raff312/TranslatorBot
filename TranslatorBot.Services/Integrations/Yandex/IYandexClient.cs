namespace TranslatorBot.Services.Integrations.Yandex;

public interface IYandexClient {
    Task<Language?> DetectLanguageAsync(DetectLanguageRequest request);
    Task<TranslateResult?> TranslateAsync(TranslateRequest request);
    Task<IamToken?> GetIamTokenAsync();
}