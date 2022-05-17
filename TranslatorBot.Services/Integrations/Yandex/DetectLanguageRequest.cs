using Newtonsoft.Json;

namespace TranslatorBot.Services.Integrations.Yandex;

// https://cloud.yandex.ru/docs/translate/api-ref/Translation/detectLanguage
public class DetectLanguageRequest {
    [JsonProperty("text")]
    public string? Text { get; set; }

    [JsonProperty("languageCodeHints")]
    public string[]? LanguageCodeHints { get; set; }

    [JsonProperty("folderId")]
    public string? FolderId { get; set; }
}