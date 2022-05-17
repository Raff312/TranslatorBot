using Newtonsoft.Json;

namespace TranslatorBot.Services.Integrations.Yandex;

// https://cloud.yandex.ru/docs/translate/api-ref/Translation/translate
public class TranslateResult {
    [JsonProperty("translations")]
    public Translation[]? Translations { get; set; }
}