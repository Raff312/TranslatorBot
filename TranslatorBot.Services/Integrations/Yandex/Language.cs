using Newtonsoft.Json;

namespace TranslatorBot.Services.Integrations.Yandex;

// https://cloud.yandex.ru/docs/translate/api-ref/Translation/detectLanguage
public class Language {
    [JsonProperty("languageCode")]
    public string? Code { get; set; }
}