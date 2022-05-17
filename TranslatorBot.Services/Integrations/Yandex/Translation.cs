using Newtonsoft.Json;

namespace TranslatorBot.Services.Integrations.Yandex;

// https://cloud.yandex.ru/docs/translate/api-ref/Translation/translate
public class Translation {
    [JsonProperty("text")]
    public string? Text { get; set; }

    [JsonProperty("detectedLanguageCode")]
    public string? DetectedLanguageCode { get; set; }
}